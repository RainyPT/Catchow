using System.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class PreyScript : NetworkBehaviour
{
    public NetworkVariable<int> prey_health = new NetworkVariable<int>(
    5,
    NetworkVariableReadPermission.Owner,
    NetworkVariableWritePermission.Server
    );

    public NetworkVariable<int> prey_score = new NetworkVariable<int>(
    10,
    NetworkVariableReadPermission.Owner,
    NetworkVariableWritePermission.Server
    );

    private Animator _characterAnimator;
    private PlayerScript p_Script;

    private void Start()
    {
        p_Script = GetComponent<PlayerScript>();
        _characterAnimator = p_Script._characterAnimator;
        if (!IsOwner) return;
        Ingame_menu_Manager.igm_instance.LoadPreyPanel();
        Ingame_menu_Manager.igm_instance.UpdateHealthUI(5);
        prey_health.OnValueChanged += (oldValue, newValue) =>
        {
            Ingame_menu_Manager.igm_instance.UpdateHealthUI(newValue);
        };
        prey_score.OnValueChanged += (oldValue, newValue) =>
        {
            Ingame_menu_Manager.igm_instance.UpdateCookieCountUI(newValue);
        };
    }

    public NetworkVariable<float> boostingTime = new NetworkVariable<float>(
   0f,
   NetworkVariableReadPermission.Owner,
   NetworkVariableWritePermission.Server
   );

    public NetworkVariable<bool> isBoosting = new NetworkVariable<bool>(
   false,
   NetworkVariableReadPermission.Owner,
   NetworkVariableWritePermission.Server
   );

    public void TakeDamage(int amount)
    {

        prey_health.Value -= amount;
        if (prey_health.Value <= 0)
        {
            GameManager.instance.EndGameRpc("Prey died!");
        }

        RunBoostRpc();
        
    }

    private void Update()
    {
        if (IsServer)
        {
            if (boostingTime.Value > 0f)
            {
                boostingTime.Value -= Time.deltaTime;
            }
            else
            {
                if (isBoosting.Value == true)
                {
                    StopRunBoostRpc();
                    isBoosting.Value = false;
                }
            }
            
        }
    }




    void RunBoostRpc()
    {
        isBoosting.Value = true;
        p_Script._moveSpeed.Value += 5f;
        boostingTime.Value = 5f;
        _characterAnimator.SetBool("isRunning", true);
    }

    [Rpc(SendTo.Server)]
    void StopRunBoostRpc()
    {
        p_Script._moveSpeed.Value -= 5f;
        isBoosting.Value = false;
        boostingTime.Value = 0f;
        _characterAnimator.SetBool("isRunning", false);
    }

    [ServerRpc]
    public void AddScoreServerRpc(ServerRpcParams rpcParams = default)
    {
        prey_score.Value += 1;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Cookie"))
        {
            AddScoreServerRpc();
            other.gameObject.SetActive(false);
        }
        if (other.CompareTag("Extraction"))
        {
            if(prey_score.Value < 10) return;
            GameManager.instance.EndGameRpc("Prey extracted sucessfully!");
        }

    }
}
