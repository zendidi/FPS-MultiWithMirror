using Mirror;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(Player))]
public class PlayerSetup : NetworkBehaviour
{
    [SerializeField]
    Behaviour[] componentsToDisable;

    [SerializeField]
    private string remoteLayerName= "RemotePlayer";

    [SerializeField]
    private string dontDrawLayerName = "DontDraw";

    [SerializeField]
    private GameObject playerGraphics;

    [SerializeField]
    private GameObject playerNameplateGraphics;

    [SerializeField]
    private GameObject playerUIPrefab;
   

    [HideInInspector]
    public GameObject playerUiInstance;
    private void Start()
    {
        if (!isLocalPlayer)// isLocalPlayer détecte si le player est celui de la session
        {
            DisableComponents();
            AssignRemoteLayer();
        }
        else
        {
           
            // Désactiver la partie graphique du joueur local
            Util.SetLayerRecursively(playerGraphics, LayerMask.NameToLayer(dontDrawLayerName));
            Util.SetLayerRecursively(playerNameplateGraphics, LayerMask.NameToLayer(dontDrawLayerName));

            //Création du UI du joueur local
            playerUiInstance = Instantiate(playerUIPrefab);

            // configuration du UI
            PlayeurUI ui = playerUiInstance.GetComponent<PlayeurUI>();

            if (ui==null)
            {
                Debug.LogError("Pas de script playeur UI sur le player Ui instance");
            }
            else
            {
                ui.setPlayer(GetComponent<Player>());
            }
            GetComponent<Player>().setUp();

            string username = UserAccountManager.LoggedInUsername;

            CmdSetUsername(transform.name, username);
        }

        
    }

    [Command]
    void CmdSetUsername(string playerID, string username)
    {
        Player player = GameManager.GetPlayer(playerID);
        if (player!= null)
        {
            Debug.Log(username + " has joined");
            player.username = username;
        }
        
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        string netID = GetComponent<NetworkIdentity>().netId.ToString();
        Player player = GetComponent<Player>();

        GameManager.RegisterPlayer(netID,player);
    }

    private void AssignRemoteLayer()
    {
        //gameObject.layer = LayerMask.NameToLayer(remoteLayerName);
        Util.SetLayerRecursively(gameObject, LayerMask.NameToLayer(remoteLayerName));
    }
    private void DisableComponents()
    {
        for (int i = 0; i < componentsToDisable.Length; i++)
        {
            componentsToDisable[i].enabled = false; // On désactive les composants désirés
        }
    }

    private void OnDisable()
    {
        Destroy(playerUiInstance);
        if (isLocalPlayer)
        {
            GameManager.instance.SetSceneCameraActive(true);
        }
   
        GameManager.UnregisterPlayer(transform.name);
    }
}
