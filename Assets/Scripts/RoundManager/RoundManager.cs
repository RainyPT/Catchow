using Unity.Netcode;
using UnityEngine;

public class RoundManager : MonoBehaviour
{
    public static RoundManager Instance;
    public NetworkVariable<int> score = new NetworkVariable<int>(
    0,
    NetworkVariableReadPermission.Owner,
    NetworkVariableWritePermission.Server
    );
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        score.OnValueChanged += (oldValue, newValue) =>
        {
            Ingame_menu_Manager.igm_instance.UpdateCookieCountUI(newValue);
        };
    }

    [ServerRpc]
    public void AddScoreServerRpc(ServerRpcParams rpcParams = default)
    {
        score.Value += 1;
    }
}
