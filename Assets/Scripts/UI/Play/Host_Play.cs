using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using Unity.Networking;
using UnityEngine.SceneManagement;

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
        if (NetworkManager.Singleton.StartHost())
        {
            Debug.Log("Host started.");
            NetworkManager.Singleton.SceneManager.LoadScene("DevScene", LoadSceneMode.Single);
        }
    }
}
