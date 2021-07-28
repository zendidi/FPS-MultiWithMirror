using Mirror;
using UnityEngine;
using System.Collections;

public class WeaponManager : NetworkBehaviour
{
    [SerializeField]
    private WeaponData primaryWeapon;
    private WeaponData currentWeapon;
    private WeaponGraphics currentGraphics;

    [SerializeField]
    private Transform weaponHolder;

    [SerializeField]
    private string weaponLayerName = "Weapon";
    private string weaponOtherPlayerLayerName = "RemotePlayer";

    [HideInInspector]
    public int currentMagazineSize;

    public bool isReloading= false;
    void Start()
    {
        EquipWeapon(primaryWeapon);
    }

    public WeaponData GetCurrentWeapon()
    {
        return currentWeapon;
    }
    public WeaponGraphics GetCurrentGraphics()
    {
        return currentGraphics;
    }

    public void EquipWeapon(WeaponData _weapon)
    {
        currentWeapon = _weapon;
        currentMagazineSize = _weapon.magazineSize;

        GameObject weaponIns =Instantiate(currentWeapon.graphics, weaponHolder.position, weaponHolder.rotation);
        weaponIns.transform.SetParent(weaponHolder);

        currentGraphics = weaponIns.GetComponent<WeaponGraphics>();

        if (currentGraphics== null)
        {
            Debug.LogError("Pas de script weaponGraphics sur l'arme : " + weaponIns.name);
        }
        if (isLocalPlayer)
        {
            Util.SetLayerRecursively(weaponIns, LayerMask.NameToLayer(weaponLayerName));
        }
        else
        {
            Util.SetLayerRecursively(weaponIns, LayerMask.NameToLayer(weaponOtherPlayerLayerName));
        }
    }

    public IEnumerator Reload()
    {
        if (isReloading)
        {
            yield break;
        }

        Debug.Log(" Reloading...");
        isReloading = true;

        CmdOnReload();
        yield return new WaitForSeconds(currentWeapon.reloadTime);
        currentMagazineSize = currentWeapon.magazineSize;

        isReloading = false;
        Debug.Log(" Reloading Done");
    }

    [Command]

    void CmdOnReload()
    {
        RpcOnReload();
    }

    [ClientRpc]
    void RpcOnReload()
    {
        Animator animator = currentGraphics.GetComponent<Animator>();
        if (animator!= null)
        {
            animator.SetTrigger("Reload");
        }
    }
}
