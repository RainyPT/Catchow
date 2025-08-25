using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using Unity.Networking;

public class Host_Play : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(HostGame);
    }

    // Update is called once per frame
    void HostGame()
    {
        NetworkManager.Singleton.StartHost();
    }
}
