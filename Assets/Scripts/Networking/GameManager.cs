using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    
    
    public Camera LoadingCamera;
    public Canvas LoadingCanvas;
    private PlayerManager playerManager;
    

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

        StartCoroutine(HandleLoading(clientId));
    }
    private IEnumerator HandleLoading(ulong clientId)
    {
        // Show loading visuals
        LoadingCamera.enabled = true;
        LoadingCanvas.transform.Find("LoadingText").gameObject.SetActive(true);

        // Orbit for ~30 seconds
        yield return new WaitForSeconds(30f);

        // Hide loading visuals
        LoadingCamera.enabled = false;
        LoadingCanvas.transform.Find("LoadingText").gameObject.SetActive(false);

        // --- Server ops after loading ---
        if (NetworkManager.Singleton.IsServer)
        {
            playerManager.SpawnClient(clientId, "hunter");
            playerManager.Spawn("prey");
        }
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
