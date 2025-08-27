using UnityEngine;
using Unity.Netcode;
using System.Xml;
using TMPro;

public class Ingame_menu_Manager : MonoBehaviour
{
    public static bool isOpen = false;
    public GameObject main;
    public GameObject loadingText;
    public static Ingame_menu_Manager igm_instance;
    public GameObject hunter_GUI;
    public GameObject prey_GUI;
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

    public void LoadHunterPanel()
    {
        hunter_GUI.SetActive(true);
    }
    public void LoadPreyPanel()
    {
        prey_GUI.SetActive(true);
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
