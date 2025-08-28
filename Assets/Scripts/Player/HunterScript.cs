using Unity.Collections.LowLevel.Unsafe;
using Unity.Netcode;
using UnityEngine;
using static UnityEngine.UI.Image;

public class HunterScript : NetworkBehaviour
{
    public PlayerScript playerScript;
    public GameObject crosshair;
    public NetworkVariable<int> hunter_bullets = new NetworkVariable<int>(
    2,
    NetworkVariableReadPermission.Owner,
    NetworkVariableWritePermission.Server
    );
    public AudioSource shootSoundSource;

    void Start()
    {
        if (!IsOwner) return;
        Ingame_menu_Manager.igm_instance.LoadHunterPanel();
        Ingame_menu_Manager.igm_instance.UpdateBulletUI(2);
        hunter_bullets.OnValueChanged += (oldValue, newValue) =>
        {
            Ingame_menu_Manager.igm_instance.UpdateBulletUI(newValue);
        };
    }

    [ServerRpc]
    private void AddBulletsServerRpc(ServerRpcParams rpcParams = default)
    {
        hunter_bullets.Value += 3;

    }

    [Rpc(SendTo.Server)]
    private void ShootServerRpc()
    {
        if (hunter_bullets.Value <= 0) return;
        hunter_bullets.Value--;
        playerScript._playerCamera.transform.localRotation *= Quaternion.Euler(5f, 0f, 0f); // Camera recoil
        PlayShootingSoundRpc();

        Vector3 origin = playerScript._playerCamera.transform.position;
        Vector3 direction = playerScript._playerCamera.transform.forward;

        int layerMask = LayerMask.GetMask("Prey");

        if (Physics.Raycast(origin, direction, out RaycastHit hit, 100, layerMask))
        {
            //DrawRayDebugRpc(origin, direction);
            var isPrey = hit.collider.CompareTag("Prey");
            if (isPrey)
            {

                var preyStuff = hit.collider.GetComponent<PreyScript>();
                if (preyStuff.prey_health != null)
                {
                    preyStuff.TakeDamage(1);
                }
            }
        }
    }
    [Rpc(SendTo.ClientsAndHost)]
    void DrawRayDebugRpc(Vector3 origin, Vector3 direction)
    {
        Debug.DrawRay(origin, direction);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void PlayShootingSoundRpc()
    {
        shootSoundSource.Play();
    }
    void Update()
    {
        if (!IsOwner) return;

        if (Input.GetMouseButtonDown(0))
        {
            ShootServerRpc();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Ammo"))
        {
            AddBulletsServerRpc();
            other.gameObject.SetActive(false);
        }

    }
}
