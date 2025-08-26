using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    
    
    public Camera LoadingCamera;
    public Canvas LoadingCanvas;
    private PlayerManager playerManager;

    private string[] roles = { "prey", "hunter" };

    private void OnEnable()
    {
        if (!NetworkManager.Singleton.IsServer) return;
        LoadingCanvas.transform.Find("LoadingText").gameObject.SetActive(true);
        playerManager = GetComponent<PlayerManager>();

        NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnected;
    }

    private void HandleClientConnected(ulong clientId)
    {

        LoadingCamera.enabled = false;
        LoadingCanvas.transform.Find("LoadingText").gameObject.SetActive(false);

        if (!NetworkManager.Singleton.IsServer) return;



        //Activate choose perk funct

        int randIndex = Random.Range(0, roles.Length);

        playerManager.SpawnServerOwner(roles[0]);

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
