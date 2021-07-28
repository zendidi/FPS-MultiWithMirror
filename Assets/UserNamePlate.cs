using UnityEngine;
using UnityEngine.UI;


public class UserNamePlate : MonoBehaviour
{
    [SerializeField]
    private Text UsernameText;
    [SerializeField]
    private Player Player;
    [SerializeField]
    private RectTransform HealthBarFill;
    // Update is called once per frame
    void Update()
    {
        UsernameText.text = Player.username;
        HealthBarFill.localScale = new Vector3(Player.GetHealthPct(), 1f,1f);
    }
}
