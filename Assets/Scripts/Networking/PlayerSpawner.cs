using Unity.Netcode;
using UnityEngine;

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
    }

    private void HandleClientConnected(ulong clientId)
    {
        LoadingCamera.enabled = false;

        if (!NetworkManager.Singleton.IsServer) return;

        Transform spawnprey = SpawnsPrey.GetChild(Random.Range(0, SpawnsPrey.childCount));
        Transform spawnhunter = SpawnsHunter.GetChild(Random.Range(0, SpawnsHunter.childCount));


        GameObject Hunter = Instantiate(PreyPrefab, spawnhunter.position, spawnhunter.rotation);
        Hunter.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);

        GameObject Prey = Instantiate(HunterPrefab, spawnprey.position, spawnprey.rotation);
        Prey.GetComponent<NetworkObject>().Spawn();
    }
}
