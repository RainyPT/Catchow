using UnityEngine;
using UnityEngine.UI;
public class Exit_Initial : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(ExitOutApplication);
    }

    // Update is called once per frame
    void ExitOutApplication()
    {
        Application.Quit();
    }
}
