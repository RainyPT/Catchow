using UnityEngine;
using Unity.Netcode;

public class PlayerManager : MonoBehaviour
{
    public Transform SpawnsPrey;
    public Transform SpawnsHunter;
    public GameObject HunterPrefab;
    public GameObject PreyPrefab;


    public void SpawnClient(ulong clientId,string role) {
        if(role == "hunter"){
            Transform spawnhunter = SpawnsHunter.GetChild(Random.Range(0, SpawnsHunter.childCount));
            GameObject Hunter = Instantiate(HunterPrefab, spawnhunter.position, spawnhunter.rotation);
            Hunter.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
            return;
        }
        Transform spawnprey = SpawnsPrey.GetChild(Random.Range(0, SpawnsPrey.childCount));
        GameObject Prey = Instantiate(HunterPrefab, spawnprey.position, spawnprey.rotation);
        Prey.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
    }

    public void Spawn(string role)
    {
        if (role == "hunter")
        {
            Transform spawnhunter = SpawnsHunter.GetChild(Random.Range(0, SpawnsHunter.childCount));
            GameObject Hunter = Instantiate(HunterPrefab, spawnhunter.position, spawnhunter.rotation);
            Hunter.GetComponent<NetworkObject>().Spawn( true);
            return;
        }
        Transform spawnprey = SpawnsPrey.GetChild(Random.Range(0, SpawnsPrey.childCount));
        GameObject Prey = Instantiate(HunterPrefab, spawnprey.position, spawnprey.rotation);
        Prey.GetComponent<NetworkObject>().Spawn(true);
    }
}
