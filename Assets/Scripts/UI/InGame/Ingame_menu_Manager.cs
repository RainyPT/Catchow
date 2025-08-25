using UnityEngine;
using Unity.Netcode;

public class Ingame_menu_Manager : MonoBehaviour
{
    private bool isOpen = false;
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

        canvas.SetActive(isOpen);
    }


}
