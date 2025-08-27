using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{
    
    
    public Camera LoadingCamera;
    public Canvas LoadingCanvas;

    public NetworkVariable<Dictionary<FixedString32Bytes, FixedString32Bytes>> gameInfo = new NetworkVariable<Dictionary<FixedString32Bytes, FixedString32Bytes>>(
     new Dictionary<FixedString32Bytes, FixedString32Bytes>(),
     NetworkVariableReadPermission.Owner,
     NetworkVariableWritePermission.Server
 );

    private void OnEnable()
    {
        if (!NetworkManager.Singleton.IsHost) return;

        gameInfo.Value["hunter_uid"] = NetworkManager.Singleton.LocalClientId.ToString();
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
    private void HandleClientConnected(ulong clientId)
    {
        if(LoadingCamera != null) {
            LoadingCamera.enabled = false;
        }
        if (LoadingCanvas != null)
        {
            LoadingCanvas.transform.Find("LoadingText").gameObject.SetActive(false);
        }

        //Activate choose perk funct
        gameInfo.Value["prey_uid"] = clientId.ToString();
        
        PlayerManager playerManager = GetComponent<PlayerManager>();
        playerManager.SpawnServerOwner(ulong.Parse(gameInfo.Value["hunter_uid"].ToString()), "");
        playerManager.SpawnClient(clientId, "");

        playerManager.SpawnCookies(ulong.Parse(gameInfo.Value["hunter_uid"].ToString()));
        playerManager.SpawnAmmoCrates(clientId);
    }

    private void HandleClientDisconnected(ulong clientId)
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.Shutdown();
            Destroy(NetworkManager.Singleton.gameObject);
        }

        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene("Menu", LoadSceneMode.Single);
    }
}