using UnityEngine;
using Unity.Netcode;

public class Ingame_menu_Manager : MonoBehaviour
{
    public static bool isOpen = false;
    public GameObject canvas;

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

        canvas.SetActive(isOpen);
    }


}
