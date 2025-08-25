using UnityEngine;
using Unity.Netcode;
using System.Xml;

public class Ingame_menu_Manager : MonoBehaviour
{
    public static bool isOpen = false;
    public GameObject main;
    public GameObject loadingText;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleMenu();
        }
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
