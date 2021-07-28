using UnityEngine;
using UnityEngine.UI;
public class PlayeurUI : MonoBehaviour
{
    [SerializeField]
    private RectTransform thrusterFuelFill;
    [SerializeField]
    private RectTransform healthBarFill;

    private PlayerController controller;
    private Player player;
    private WeaponManager weaponManager;

    [SerializeField]
    private GameObject pauseMenu;

    [SerializeField]
    private GameObject scoreBoard;

    [SerializeField]
    private Text AmmoText;


    public void setPlayer (Player _player)
    {
        player = _player;
        controller = player.GetComponent<PlayerController>();
        weaponManager = player.GetComponent<WeaponManager>();
    }

    private void Start()
    {
        PauseMenu.isOn = false;
    }
    private void Update()
    {
        if (controller!= null)
        {
            SetFuelAmount(controller.GetThrusterFuelAmount());
            SetHealthAmount(player.GetHealthPct());
            SetAmmoAmount(weaponManager.currentMagazineSize);
        }
        

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            scoreBoard.SetActive(true);
        }
        else if (Input.GetKeyUp(KeyCode.Tab))
        {
            scoreBoard.SetActive(false);
        }
    }

    public void TogglePauseMenu()
    {
        pauseMenu.SetActive(!pauseMenu.activeSelf);
        PauseMenu.isOn = pauseMenu.activeSelf;
    }

    void SetFuelAmount(float _amount)
    {
        thrusterFuelFill.localScale = new Vector3(1f, _amount, 1f);
    }

    void SetHealthAmount(float _amount)
    {
        healthBarFill.localScale = new Vector3(1f, _amount, 1f);
    }

    void SetAmmoAmount(int _amount)
    {
        AmmoText.text = _amount.ToString();
    }
}
