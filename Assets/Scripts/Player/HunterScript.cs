using Unity.Netcode;
using UnityEngine;

public class HunterScript : NetworkBehaviour
{
    public PlayerScript playerScript;
    public GameObject crosshair;
    public NetworkVariable<int> hunter_bullets = new NetworkVariable<int>(
    2,
    NetworkVariableReadPermission.Owner,
    NetworkVariableWritePermission.Server
    );
    public NetworkVariable<float> hunter_shoot_delay = new NetworkVariable<float>(
    0.5f,
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
    private void ShootServerRpc(bool hasHitPrey)
    {
        if (hunter_bullets.Value <= 0 || hunter_shoot_delay.Value>0f) return;
        hunter_shoot_delay.Value = 0.5f;
        hunter_bullets.Value--;
        playerScript._playerCamera.transform.localRotation *= Quaternion.Euler(5f, 0f, 0f);
        PlayShootingSoundRpc();
        if (!hasHitPrey) return;
        var preyStuff = GameObject.FindGameObjectWithTag("Prey").GetComponent<PreyScript>();
        if (preyStuff.prey_health != null)
        {
            preyStuff.TakeDamage(1);
        }
        
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void PlayShootingSoundRpc()
    {
        shootSoundSource.Play();
    }
    void Update()
    {
        if (IsServer)
        {
            hunter_shoot_delay.Value -= Time.deltaTime;
        }
        if (!IsOwner) return;

        if (Input.GetMouseButtonDown(0))
        {
            int layerMask = LayerMask.GetMask("Prey");
            Vector3 origin = playerScript._playerCamera.transform.position;
            Vector3 direction = playerScript._playerCamera.transform.forward;
            bool hasHitPrey = Physics.Raycast(origin, direction, out RaycastHit hit, 100, layerMask);
            ShootServerRpc(hasHitPrey);
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
