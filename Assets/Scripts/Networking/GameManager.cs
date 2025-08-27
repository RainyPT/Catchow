using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;

public class GameManager : NetworkBehaviour
{
    
    
    public Camera LoadingCamera;
    public Canvas LoadingCanvas;

    public NetworkVariable<Dictionary<FixedString32Bytes, FixedString32Bytes>> gameInfo = new NetworkVariable<Dictionary<FixedString32Bytes, FixedString32Bytes>>(
     new Dictionary<FixedString32Bytes, FixedString32Bytes>(),
     NetworkVariableReadPermission.Owner,
     NetworkVariableWritePermission.Server
 );

    public NetworkVariable<float> countdown = new NetworkVariable<float>(30,
    NetworkVariableReadPermission.Everyone,
    NetworkVariableWritePermission.Server
);

    

    private void OnEnable()
    {
        NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnected;
        NetworkManager.Singleton.OnServerStopped += HandleServerStopped;
        if (!NetworkManager.Singleton.IsHost) return;
        gameInfo.Value["hunter_uid"] = "";
        gameInfo.Value["prey_uid"] = "";
        LoadingCanvas.transform.Find("LoadingText").gameObject.SetActive(true);
        

        NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
        
    }
    private void OnDisable()
    {
        if (NetworkManager.Singleton == null) return;
        NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnected;
        if (!NetworkManager.Singleton.IsHost) return;
        NetworkManager.Singleton.OnClientConnectedCallback -= HandleClientConnected;
        
    }
    public static GameManager instance;
    private void Start()
    {
        instance = this;
    }

    [Rpc(SendTo.Server)]
    public void updateHunterIdRpc(ulong clientId)
    {
        if (gameInfo.Value["prey_uid"] == clientId.ToString()) return;

        if (gameInfo.Value["hunter_uid"] == "")
        {
            gameInfo.Value["hunter_uid"] = clientId.ToString();
            LockHunterButtonRpc();
        }
            

    }

    
    [Rpc(SendTo.Server)]
    public void updatePreyIdRpc(ulong clientId)
    {
        if (gameInfo.Value["hunter_uid"] == clientId.ToString()) return;

        if (gameInfo.Value["prey_uid"] == "")
        {
            gameInfo.Value["prey_uid"] = clientId.ToString();
            LockPreyButtonRpc();
        }
    }
    private void HandleClientConnected(ulong clientId)
    {
        if (LoadingCanvas != null)
        {
            LoadingCanvas.transform.Find("LoadingText").gameObject.SetActive(false);
        }

        ChoosingRoleMenuRPC();
    }

    bool inChoosingRoleMenu = false;

    [Rpc(SendTo.ClientsAndHost)]
    private void ChoosingRoleMenuRPC()
    {
        Ingame_menu_Manager.igm_instance.LoadChoosingMenu();
        inChoosingRoleMenu =true;
        countdown.OnValueChanged += (oldValue, newValue) =>
        {
            Ingame_menu_Manager.igm_instance.UpdateChoosingMenuCountdown(newValue);
        };

    }

    [Rpc(SendTo.ClientsAndHost)]
    private void UnloadChoosingMenuRpc()
    {
        Ingame_menu_Manager.igm_instance.UnloadChoosingMenu();
        
        countdown.OnValueChanged -= (oldValue, newValue) =>
        {
            Ingame_menu_Manager.igm_instance.UpdateChoosingMenuCountdown(newValue);
        };

    }

    [Rpc(SendTo.ClientsAndHost)]
    private void LockHunterButtonRpc()
    {
        Ingame_menu_Manager.igm_instance.LockHunterButton();

    }

    [Rpc(SendTo.ClientsAndHost)]
    private void LockPreyButtonRpc()
    {
        Ingame_menu_Manager.igm_instance.LockPreyButton();
    }

    public GameObject CookiePrefab;
    public GameObject AmmoCratePrefab;
    public Transform SpawnsCookies;

    [Rpc(SendTo.ClientsAndHost)]
    private void SpawnCookiesRpc(ulong preyId)
    {
        if (NetworkManager.Singleton.LocalClientId != preyId) return;
        for (int i = 0; i < SpawnsCookies.childCount; i++)
        {
            Transform spawn = SpawnsCookies.GetChild(i);
            GameObject cookie = Instantiate(CookiePrefab, spawn.position, CookiePrefab.transform.rotation);

        }

    }

    [Rpc(SendTo.ClientsAndHost)]
    private void SpawnAmmoCratesRpc(ulong hunterid)
    {
        if (NetworkManager.Singleton.LocalClientId != hunterid) return;
        for (int i = 0; i < SpawnsCookies.childCount; i++)
        {
            Transform spawn = SpawnsCookies.GetChild(i);
            GameObject cookie = Instantiate(AmmoCratePrefab, spawn.position, AmmoCratePrefab.transform.rotation);
        }

    }

    bool gameEnded = false;
    private void Update()
    {
        if (!NetworkManager.Singleton.IsServer) return;

        if (inChoosingRoleMenu)
        {
            countdown.Value -= Time.deltaTime;
        }

        if (inChoosingRoleMenu && countdown.Value <= 0)
        {
            inChoosingRoleMenu = false;
            UnloadChoosingMenuRpc();
            StartGame();
            countdown.Value = 10;
        }
        if (gameEnded)
        {
            if(countdown.Value > 0){
                countdown.Value -= Time.deltaTime;
            }
            else
            {
                Disconnect();
            }
            
        }
       
    }
    private void StartGame()
    {


        PlayerManager playerManager = GetComponent<PlayerManager>();
        playerManager.SpawnHunterServerRpc(ulong.Parse(gameInfo.Value["hunter_uid"].ToString()));
        playerManager.SpawnPreyServerRpc(ulong.Parse(gameInfo.Value["prey_uid"].ToString()));

        SpawnCookiesRpc(ulong.Parse(gameInfo.Value["prey_uid"].ToString()));
        SpawnAmmoCratesRpc(ulong.Parse(gameInfo.Value["hunter_uid"].ToString()));
        
    }


    [Rpc(SendTo.Server)]
    public void EndGameRpc(string message)
    {
        PlayerManager playerManager = GetComponent<PlayerManager>();
        playerManager.DespawnHunterServerRpc();
        playerManager.DespawnPreyServerRpc();
        EndGameIGMRpc(message);
        gameEnded = true;
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void EndGameIGMRpc(string message)
    {
        Ingame_menu_Manager.igm_instance.ShowGameEnd(message);
    }

    private void Disconnect() {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.Shutdown();
            Destroy(NetworkManager.Singleton.gameObject);
        }

        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene("Menu", LoadSceneMode.Single);
    }
    private void HandleServerStopped(bool wasHost)
    {
        Disconnect();
    }
    private void HandleClientDisconnected(ulong clientId)
    {
        Disconnect();
    }
    
}