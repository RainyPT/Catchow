using Unity.Collections.LowLevel.Unsafe;
using Unity.Netcode;
using UnityEngine;

public class HunterScript : NetworkBehaviour
{
    public Camera playercamera;
    public NetworkVariable<int> hunter_bullets = new NetworkVariable<int>(
    2,
    NetworkVariableReadPermission.Owner,
    NetworkVariableWritePermission.Server
    );
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

    [ServerRpc]
    private void ShootServerRpc(ServerRpcParams rpcParams = default)
    {
        if (hunter_bullets.Value <= 0) return;
        hunter_bullets.Value--;

        ulong shooterId = rpcParams.Receive.SenderClientId;

        // Perform raycast from shooter's camera
        Vector3 origin = playercamera.transform.position;
        Vector3 direction = playercamera.transform.forward;

        if (Physics.Raycast(origin, direction, out RaycastHit hit, 50))
        {

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
