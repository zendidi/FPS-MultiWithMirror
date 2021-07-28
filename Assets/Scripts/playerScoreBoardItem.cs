using UnityEngine;
using UnityEngine.UI;

public class playerScoreBoardItem : MonoBehaviour
{
    [SerializeField]
    Text usernameText;

    [SerializeField]
    Text killsText;
    [SerializeField]
    Text deathsText;

    public void Setup(Player _player)
    {
        usernameText.text = _player.username;
        killsText.text = "Kills : " + _player.kills;
        deathsText.text = "Deaths : " + _player.deaths;
    }
}
