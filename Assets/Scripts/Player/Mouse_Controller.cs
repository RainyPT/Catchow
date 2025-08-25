using System.Globalization;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerControllerInputSystem : NetworkBehaviour
{
    public float mouse_sens = 100f;
    public GameObject player_body;
    private Transform head, body;
    public Camera fpv;
    private float x_rotation=0f;
    public GameObject Cameras;

    void Start()
    {
        if (!IsOwner) return;


        if (IsOwner) Cameras.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;
        head= player_body.transform.Find("Head");
        if (fpv != null && head != null)
        {
            fpv.transform.SetParent(head);
            fpv.transform.localPosition = new Vector3(0f, 0f, 0.5f);
            fpv.transform.localRotation = Quaternion.identity;
        }
    }

    void Update()
    {
        
        if (!IsOwner) return;
        if (Ingame_menu_Manager.isOpen) return;
        float mouse_x = Input.GetAxis("Mouse X") * mouse_sens * Time.deltaTime;
        float mouse_y = Input.GetAxis("Mouse Y") * mouse_sens * Time.deltaTime;

        x_rotation -= mouse_y;
        x_rotation = Mathf.Clamp(x_rotation ,- 70f, 50f);

        head.rotation = Quaternion.Euler(x_rotation, player_body.transform.eulerAngles.y, 0f);

        transform.Rotate(Vector3.up * mouse_x);

    }
}
