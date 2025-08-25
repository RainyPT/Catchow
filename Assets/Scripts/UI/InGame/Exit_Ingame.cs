using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class Exit_Ingame : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(Quit);
    }

    // Update is called once per frame
    void Quit()
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
