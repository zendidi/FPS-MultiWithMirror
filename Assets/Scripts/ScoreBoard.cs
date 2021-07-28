using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreBoard : MonoBehaviour
{
    [SerializeField]
    private GameObject playerScoreBoardItem;   
    [SerializeField]
    private Transform playerScoreBoardList;

    private void OnEnable()
    {
        //Récupérer une array de tous les joueurs du serveur
        Player[] players = GameManager.GetAllPlayers();

        //loop sur l'array et mise en place d'une ligne de UI pour chaque joueur
        foreach (Player player in players)
        {
            GameObject itemGO = Instantiate(playerScoreBoardItem, playerScoreBoardList);
            playerScoreBoardItem item = itemGO.GetComponent<playerScoreBoardItem>();

            if (item!=null)
            {
                item.Setup(player);
            }
        }     
    }

    private void OnDisable()
    {
        foreach (Transform child in playerScoreBoardList)
        {
            Destroy(child.gameObject);
        }
    }
}
