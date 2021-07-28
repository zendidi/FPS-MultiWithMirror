using Mirror;
using UnityEngine;

[RequireComponent(typeof(WeaponManager))]
public class PlayerShoot : NetworkBehaviour
{
    

    [SerializeField]
    private Camera cam;
    [SerializeField]
    private LayerMask mask;

    private WeaponManager weaponManager;
    private WeaponData currentWeapon;

    void Start()
    {
        if (cam==null)
        {
            Debug.LogError("Pas de camera renseignée sur le système de tir");
            enabled = false;
        }
        weaponManager = GetComponent<WeaponManager>();
        currentWeapon = weaponManager.GetCurrentWeapon();
    }

    private void Update()
    {
        currentWeapon = weaponManager.GetCurrentWeapon();
        if (PauseMenu.isOn)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.R) && weaponManager.currentMagazineSize< currentWeapon.magazineSize)
        {
            StartCoroutine(weaponManager.Reload());
            return;
        }

        if (currentWeapon==null)
        {
            return;
        }
      
        if (currentWeapon.fireRate <= 0f)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                Shoot();
            }
        }
        else
        {
            if (Input.GetButtonDown("Fire1"))
            {
                InvokeRepeating("Shoot", 0f, 1f / currentWeapon.fireRate);
            }
            else if (Input.GetButtonUp("Fire1"))
            {
                CancelInvoke("Shoot");
            }
        }
    }

    [Command]
    void cmdOnHit(Vector3 pos,Vector3 normal)
    {
        RpcDoHitEffect(pos,normal);
    }

    [ClientRpc]
    void RpcDoHitEffect(Vector3 pos, Vector3 normal)
    {
        GameObject hitEffect =Instantiate(weaponManager.GetCurrentGraphics().hitEffectPrefab, pos, Quaternion.LookRotation(normal));
        Destroy(hitEffect, 2f);
    }

    [Command]
    // Fonction appelée sur le serveur lorsque le joueur tire 
    void cmdOnShoot()
    {
        RpcDoShootEffect();
    }

    [ClientRpc]
    // Fait apparaitre les effets de tir chez tout les clients.
    void RpcDoShootEffect()
    {
        weaponManager.GetCurrentGraphics().muzzleFlash.Play();
    }

    [Client]
    private void Shoot()
    {
        if (!isLocalPlayer || weaponManager.isReloading)
        {
            return;
        }

        if (weaponManager.currentMagazineSize <= 0)
        {
            StartCoroutine(weaponManager.Reload());
            return;
        }

        weaponManager.currentMagazineSize--;
        Debug.Log("IL nous reste " + weaponManager.currentMagazineSize + " balles dans le chargeur");
        cmdOnShoot();
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position,cam.transform.forward,out hit, currentWeapon.range,mask ))
        {
            if (hit.collider.tag=="Player")
            {
                CmdPlayerShot(hit.collider.name, currentWeapon.damage, transform.name);
            }

            cmdOnHit(hit.point, hit.normal);
        }

        if (weaponManager.currentMagazineSize <= 0)
        {
            StartCoroutine(weaponManager.Reload());
            return;
        }
    }

    [Command]
    private void CmdPlayerShot(string playerID, float damage, string sourceID)
    {
        Debug.Log(playerID + " just got shot");

        Player player = GameManager.GetPlayer(playerID);
        player.RpcTakeDamage(damage, sourceID);
    }
}
