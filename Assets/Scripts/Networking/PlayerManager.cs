using UnityEngine;
using Unity.Netcode;

public class PlayerManager : MonoBehaviour
{
    public Transform SpawnsPrey;
    public Transform SpawnsHunter;
    public Transform SpawnsCookies;
    public GameObject HunterPrefab;
    public GameObject PreyPrefab;
    public GameObject CookiePrefab;


    public void SpawnClient(ulong clientId, string role)
    {
        if (role == "hunter")
        {
            Transform spawnhunter = SpawnsHunter.GetChild(Random.Range(0, SpawnsHunter.childCount));
            GameObject Hunter = Instantiate(HunterPrefab, spawnhunter.position, spawnhunter.rotation);
            Hunter.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
            return;
        }
        Transform spawnprey = SpawnsPrey.GetChild(Random.Range(0, SpawnsPrey.childCount));
        GameObject Prey = Instantiate(HunterPrefab, spawnprey.position, spawnprey.rotation);
        Prey.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
    }

    public void SpawnServerOwner(string role)
    {
        if (role == "hunter")
        {
            Transform spawnhunter = SpawnsHunter.GetChild(Random.Range(0, SpawnsHunter.childCount));
            GameObject Hunter = Instantiate(HunterPrefab, spawnhunter.position, spawnhunter.rotation);
            Hunter.GetComponent<NetworkObject>().Spawn(true);
            return;
        }
        Transform spawnprey = SpawnsPrey.GetChild(Random.Range(0, SpawnsPrey.childCount));
        GameObject Prey = Instantiate(HunterPrefab, spawnprey.position, spawnprey.rotation);
        Prey.GetComponent<NetworkObject>().Spawn(true);
    }

    public void SpawnCookies()
    {
        for (int i = 0; i < SpawnsCookies.childCount; i++)
        {
            Transform spawn = SpawnsCookies.GetChild(i);
            GameObject cookie = Instantiate(CookiePrefab, spawn.position, spawn.rotation);
            cookie.GetComponent<NetworkObject>().Spawn(true);
        }
    }
}
