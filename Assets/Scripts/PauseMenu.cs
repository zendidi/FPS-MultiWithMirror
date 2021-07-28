using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PauseMenu : NetworkBehaviour
{
    public static bool isOn = false;

    private NetworkManager networkManager;

    private void Start()
    {
        networkManager = NetworkManager.singleton;
    }

    public void  LeaveRoomButton()
    {
        if (isClientOnly)
        {   // si un joueur quitte la partie on vérifie si il est hôte du serveur
            networkManager.StopClient();
        }
        else
        {   // si il est hôte ça coupe tout
            networkManager.StopHost();
        }
    }

}
