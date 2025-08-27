using UnityEngine;
using Unity.Netcode;
using System.Xml;
using TMPro;
using UnityEngine.UI;

public class Ingame_menu_Manager : MonoBehaviour
{
    public static bool isOpen = false;
    public GameObject main;
    public GameObject loadingText;
    public static Ingame_menu_Manager igm_instance;
    public GameObject hunter_GUI;
    public GameObject prey_GUI;
    public GameObject choosing_GUI;
    public TextMeshProUGUI gameEnded_Text;
    void Start()
    {
        igm_instance = this;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleMenu();
        }
    }

    public void ShowGameEnd(string message)
    {
        prey_GUI.SetActive(false);
        hunter_GUI.SetActive(false);
        gameEnded_Text.gameObject.SetActive(true);
        gameEnded_Text.SetText(message);
    }

    public void UpdateChoosingMenuCountdown(float value)
    {
        TextMeshProUGUI countdown = choosing_GUI.transform.Find("Countdown").GetComponent<TextMeshProUGUI>();
        int value_int = (int)value;
        countdown.text = value_int.ToString();
    }

    public void LoadHunterPanel()
    {
        hunter_GUI.SetActive(true);
        
    }
    public void LoadPreyPanel()
    {
        prey_GUI.SetActive(true);
    }
    public void LoadChoosingMenu()
    {
        choosing_GUI.SetActive(true);
        choosing_GUI.transform.Find("ChooseHunter").GetComponent<Button>().onClick.AddListener(ChooseHunter);
        choosing_GUI.transform.Find("ChoosePrey").GetComponent<Button>().onClick.AddListener(ChoosePrey);
    }
    public void UnloadChoosingMenu()
    {
        choosing_GUI.SetActive(false);
    }
    
    public void LockHunterButton()
    {
        choosing_GUI.transform.Find("ChooseHunter").GetComponent<Button>().interactable=false;
    }

    public void LockPreyButton()
    {
        choosing_GUI.transform.Find("ChoosePrey").GetComponent<Button>().interactable = false;
    }
    private void ChooseHunter()
    {
        GameManager.instance.updateHunterIdRpc(NetworkManager.Singleton.LocalClientId);
    }
    private void ChoosePrey()
    {
        GameManager.instance.updatePreyIdRpc(NetworkManager.Singleton.LocalClientId);
    }
    public void UpdateBulletUI(int bullets)
    {
        TextMeshProUGUI ammo = hunter_GUI.transform.Find("Ammo").GetComponent<TextMeshProUGUI>();
        ammo.text = bullets.ToString();
    }
    public void UpdateHealthUI(int health)
    {
        TextMeshProUGUI healthz = prey_GUI.transform.Find("Health").GetComponent<TextMeshProUGUI>();
        healthz.text = health.ToString();
    }
    public void UpdateCookieCountUI(int cookieCount)
    {
        TextMeshProUGUI _cookieCount = prey_GUI.transform.Find("Cookies").GetComponent<TextMeshProUGUI>();
        _cookieCount.text = cookieCount.ToString();
    }
    void ToggleMenu()
    {
        
        isOpen = !isOpen;
        if (isOpen) {
            Cursor.lockState = CursorLockMode.None;

        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        main.SetActive(isOpen);
    }


}
