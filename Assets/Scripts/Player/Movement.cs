using Unity.Netcode;
using UnityEngine;

public class Moviment : NetworkBehaviour
{
    public CharacterController characterController;
    public float speed = 12f;
    public Transform player_body;
    public float gravity=-9.81f;
    public float jumpheight = 3f;
    Vector3 velocity;
    public Transform ground_check;
    public float ground_distance=0.1f;
    public LayerMask ground_mask;
    private bool isGrounded;
    void Update()
    {
        if (!IsOwner) return;

        isGrounded = Physics.CheckSphere(ground_check.position, ground_distance, ground_mask);
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        //transform.localRotation = player_body.localRotation;
        Vector3 move = player_body.right * x + player_body.forward * z;
        characterController.Move(move* speed*Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded) {
                velocity.y = Mathf.Sqrt(jumpheight * -2f * gravity);
        }
        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity*Time.deltaTime);
    }
}
