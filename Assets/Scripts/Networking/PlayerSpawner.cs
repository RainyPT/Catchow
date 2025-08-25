using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject HunterPrefab;
    public GameObject PreyPrefab;
    public Transform SpawnsPrey;
    public Transform SpawnsHunter;
    public Camera LoadingCamera;

    private void OnEnable()
    {
        if (!NetworkManager.Singleton.IsServer) return;
        NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnected;
    }

    private void HandleClientConnected(ulong clientId)
    {
        LoadingCamera.enabled = false;

        if (!NetworkManager.Singleton.IsServer) return;

        Transform spawnprey = SpawnsPrey.GetChild(Random.Range(0, SpawnsPrey.childCount));
        Transform spawnhunter = SpawnsHunter.GetChild(Random.Range(0, SpawnsHunter.childCount));


        GameObject Hunter = Instantiate(HunterPrefab, spawnhunter.position, spawnhunter.rotation);
        Hunter.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);

        GameObject Prey = Instantiate(PreyPrefab, spawnprey.position, spawnprey.rotation);
        Prey.GetComponent<NetworkObject>().Spawn();
    }

    private void HandleClientDisconnected(ulong clientId)
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.Shutdown();
            Destroy(NetworkManager.Singleton.gameObject);
        }

        Time.timeScale = 1f;

        SceneManager.LoadScene("Menu");
    }
}
