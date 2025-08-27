using Unity.Netcode;
using UnityEngine;

public class Movement : NetworkBehaviour
{
    public CharacterController characterController;
    public NetworkVariable<float> speed = new NetworkVariable<float>(5.0f,
    NetworkVariableReadPermission.Everyone,
    NetworkVariableWritePermission.Owner);
    public Transform player_body;
    public float gravity = -9.81f;
    public float jumpheight = 3f;
    Vector3 velocity;
    public Transform ground_check;
    public float ground_distance = 0.1f;
    public LayerMask ground_mask;
    private bool isGrounded;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }
    void Update()
    {

        if (!IsOwner) return;

        isGrounded = Physics.CheckSphere(ground_check.position, ground_distance, ground_mask);
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpheight * -2f * gravity);
        }
        velocity.y += gravity * Time.deltaTime;

        characterController.Move(velocity * Time.deltaTime);

        if (Ingame_menu_Manager.isOpen) return;

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 move = player_body.right * x + player_body.forward * z;
        characterController.Move(move * speed.Value * Time.deltaTime);

        bool running = move.magnitude > 0.1f;
        if (running) UpdateRunningServerRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void UpdateRunningServerRpc()
    {
        if (!GetComponent<AudioSource>().isPlaying)
        {
            GetComponent<AudioSource>().Play();
        }
    }
}
