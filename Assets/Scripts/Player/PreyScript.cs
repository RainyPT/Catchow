using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class PreyScript : NetworkBehaviour
{
    public NetworkVariable<int> prey_health = new NetworkVariable<int>(
    5,
    NetworkVariableReadPermission.Owner,
    NetworkVariableWritePermission.Server
    );

    private void Start()
    {
        if (!IsOwner) return;
        Ingame_menu_Manager.igm_instance.LoadPreyPanel();
        Ingame_menu_Manager.igm_instance.UpdateHealthUI(5);
        prey_health.OnValueChanged += (oldValue, newValue) =>
        {
            Ingame_menu_Manager.igm_instance.UpdateHealthUI(newValue);
        };
    }

    public void TakeDamage(int amount)
    {
        if (!IsServer) return;

        prey_health.Value -= amount;
        if (prey_health.Value <= 0)
        {
            prey_health.Value = 0;
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Cookie"))
        {
            RoundManager.Instance.AddScoreServerRpc();
            other.gameObject.SetActive(false);
        }

    }
}
