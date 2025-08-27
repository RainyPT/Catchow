using UnityEngine;
using Unity.Netcode;

public class PlayerManager : MonoBehaviour
{
    public Transform SpawnsPrey;
    public Transform SpawnsHunter;
    
    public GameObject HunterPrefab;
    public GameObject PreyPrefab;
   


    [Rpc(SendTo.Server)]
    public void SpawnPreyServerRpc(ulong preyId)
    {
        Transform spawnprey = SpawnsPrey.GetChild(Random.Range(0, SpawnsPrey.childCount));
        GameObject Prey = Instantiate(PreyPrefab, spawnprey.position, spawnprey.rotation);
        Prey.GetComponent<NetworkObject>().SpawnAsPlayerObject(preyId, true);
    }

    [Rpc(SendTo.Server)]
    public void SpawnHunterServerRpc(ulong hunterId)
    {
        Transform spawnhunter = SpawnsHunter.GetChild(Random.Range(0, SpawnsHunter.childCount));
        GameObject Hunter = Instantiate(HunterPrefab, spawnhunter.position, spawnhunter.rotation);
        Hunter.GetComponent<NetworkObject>().SpawnAsPlayerObject(hunterId, true);
    }

    [Rpc(SendTo.Server)]
    public void DespawnPreyServerRpc()
    {
        GameObject Prey = GameObject.FindGameObjectWithTag("Prey");
        Prey.GetComponent<NetworkObject>().Despawn();
    }

    [Rpc(SendTo.Server)]
    public void DespawnHunterServerRpc()
    {
        GameObject Hunter = GameObject.FindGameObjectWithTag("Hunter");
        Hunter.GetComponent<NetworkObject>().Despawn();
    }



}
