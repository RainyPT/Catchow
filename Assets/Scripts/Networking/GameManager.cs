using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    
    
    public Camera LoadingCamera;
    public Canvas LoadingCanvas;

    private string[] roles = { "prey", "hunter" };

    private void OnEnable()
    {
        NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnected;
        Debug.Log("OnEnable isHost: " + NetworkManager.Singleton.IsHost);
        if (!NetworkManager.Singleton.IsHost) return;
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
        Debug.Log("HandleClientConnected isHost: " + NetworkManager.Singleton.IsHost);
        ulong hostId = NetworkManager.Singleton.LocalClientId;
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            return;
        }
        if(LoadingCamera != null) {
            LoadingCamera.enabled = false;
        }
        if (LoadingCanvas != null)
        {
            LoadingCanvas.transform.Find("LoadingText").gameObject.SetActive(false);
        }

        //Activate choose perk funct
        PlayerManager playerManager = GetComponent<PlayerManager>();
        playerManager.SpawnServerOwner(hostId,roles[0]);
        playerManager.SpawnClient(clientId, roles[1]);
        playerManager.SpawnCookies();
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
