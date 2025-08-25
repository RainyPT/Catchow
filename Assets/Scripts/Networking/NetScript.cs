using Unity.Netcode;
using UnityEngine;

public class NetScript : MonoBehaviour
{
    private void Awake()
    {
        if (FindObjectsByType<NetworkManager>(FindObjectsSortMode.None).Length > 1)
        {
            Destroy(gameObject); // Kill duplicates
            return;
        }

        DontDestroyOnLoad(gameObject); // Persist between scenes
    }
}
