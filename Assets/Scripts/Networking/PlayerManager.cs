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
    public GameObject AmmoCratePrefab;
    public void SpawnClient(ulong clientId, string role)
    {
        Transform spawnprey = SpawnsPrey.GetChild(Random.Range(0, SpawnsPrey.childCount));
        GameObject Prey = Instantiate(PreyPrefab, spawnprey.position, spawnprey.rotation);
        Prey.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
    }


    public void SpawnServerOwner(ulong hostId,string role)
    {
        Transform spawnhunter = SpawnsHunter.GetChild(Random.Range(0, SpawnsHunter.childCount));
        GameObject Hunter = Instantiate(HunterPrefab, spawnhunter.position, spawnhunter.rotation);
        Hunter.GetComponent<NetworkObject>().SpawnAsPlayerObject(hostId,true);
    }


    public void SpawnCookies(ulong HunterId)
    {
        for (int i = 0; i < SpawnsCookies.childCount; i++)
        {
            Transform spawn = SpawnsCookies.GetChild(i);
            GameObject cookie = Instantiate(CookiePrefab, spawn.position, CookiePrefab.transform.rotation);
            cookie.GetComponent<NetworkObject>().Spawn();
            if (NetworkManager.Singleton.IsHost && NetworkManager.Singleton.LocalClientId == HunterId)
            {
                cookie.SetActive(false);
            }
            else
            {
                cookie.GetComponent<NetworkObject>().NetworkHide(HunterId);
            }
        }

    }

    public void SpawnAmmoCrates(ulong preyId)
    {
        for (int i = 0; i < SpawnsCookies.childCount; i++)
        {
            Transform spawn = SpawnsCookies.GetChild(i);
            GameObject cookie = Instantiate(AmmoCratePrefab, spawn.position, AmmoCratePrefab.transform.rotation);
            cookie.GetComponent<NetworkObject>().Spawn();
            if (NetworkManager.Singleton.IsHost && NetworkManager.Singleton.LocalClientId == preyId)
            {
                cookie.SetActive(false);
            }
            else
            {
            cookie.GetComponent<NetworkObject>().NetworkHide(preyId);
            }
        }

    }

}
