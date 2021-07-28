using UnityEngine;
using Mirror;
using System.Collections;

[RequireComponent(typeof(PlayerSetup))]
public class Player : NetworkBehaviour
{
    [SyncVar]
    private bool _isDead = false;
    public bool isDead
    {
        get { return _isDead; }
        protected set { _isDead = value; }
    }

    [SerializeField]
    private float maxHealth = 100f;

    [SyncVar]
    private float currentHealth;

    public float GetHealthPct()
    {
        return (float)currentHealth / maxHealth;
    }

    [SyncVar]
    public string username = " Player";

    public int kills;
    public int deaths;

    [SerializeField]
    private Behaviour[] disableOnDeath;

    [SerializeField]
    private GameObject[] disableObjectsOnDeath;

    private bool[] wasEnabledOnStart;
    [SerializeField]
    private GameObject deathEffect;

    [SerializeField]
    private GameObject spawnEffect;

    private bool firstSetup = true;
    public void setUp()
    {
        //changement de camera
        if (isLocalPlayer)
        {

            GameManager.instance.SetSceneCameraActive(false);
            GetComponent<PlayerSetup>().playerUiInstance.SetActive(true);
        }
 
        cmdBroadcastNewPlayerSetup();
    }

    [Command(requiresAuthority = false)]
     private void cmdBroadcastNewPlayerSetup()
    {
        RpcSetupPlayerOnAllClients();
    }

    [ClientRpc]
    private void RpcSetupPlayerOnAllClients()
    {
        if (firstSetup)
        {
            wasEnabledOnStart = new bool[disableOnDeath.Length];
            for (int i = 0; i < disableOnDeath.Length; i++)
            {
                wasEnabledOnStart[i] = disableOnDeath[i].enabled;
            }
            firstSetup = false;
        }
        SetDefaults();
    }

    public void SetDefaults()
    {
        isDead = false;
        currentHealth = maxHealth;

        //réactice les scripts du jouer
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled= wasEnabledOnStart[i];
        }
       
        // réactive les gameobjects du joueur
        for (int i = 0; i < disableObjectsOnDeath.Length; i++)
        {
            disableObjectsOnDeath[i].SetActive(true);
        }

        // réactive le collider du joueur.
        Collider col = GetComponent<Collider>();
        if (col!=null)
        {
            col.enabled = true;
        }

     

        GameObject _GfxIns = Instantiate(spawnEffect, transform.position, Quaternion.identity);
        Destroy(_GfxIns, 2.5f);
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(GameManager.instance.matchSettings.respawnTimer);
       
        Transform spawnPoint = NetworkManager.singleton.GetStartPosition();
        transform.position = spawnPoint.position;
        transform.rotation = spawnPoint.rotation;

        yield return new WaitForSeconds(0.1f);
        setUp();
    }
 
    private void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            RpcTakeDamage(10, "joueur");
        }
       
    }

    [ClientRpc]
    public void RpcTakeDamage(float amount, string sourceID)
    {
        if (isDead)
        {
            return;
        }
        currentHealth -= amount;
        Debug.Log(transform.name + " a maintenant : " + currentHealth + "points de vie");

        if (currentHealth<=0)
        {
            Die(sourceID);
        }
    }

    private void Die(string sourceID)
    {
        isDead = true;

        Player sourcePlayer = GameManager.GetPlayer(sourceID);
        if (sourcePlayer!= null)
        {
            sourcePlayer.kills++;
            GameManager.instance.onPlayerKilledCallback.Invoke(username, sourcePlayer.username);
        }

        deaths++;
        // Désactive les components du joueur lors de sa mort
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = false;
        }

        // Désactive les GameObject du joueur lors de sa mort
        for (int i = 0; i < disableObjectsOnDeath.Length; i++)
        {
            disableObjectsOnDeath[i].SetActive(false);
        }

        // désactive le collider du joueur
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = false;
        }
        // apparition du systeme de particule de mort
        GameObject _GfxIns =Instantiate(deathEffect, transform.position, Quaternion.identity);
        Destroy(_GfxIns, 2.5f);

        //changement de camera
        if (isLocalPlayer)
        {
            GameManager.instance.SetSceneCameraActive(true);
            GetComponent<PlayerSetup>().playerUiInstance.SetActive(false);
        }
        Debug.Log(transform.name + " a été éliminé");
        StartCoroutine(Respawn());
    }
}
