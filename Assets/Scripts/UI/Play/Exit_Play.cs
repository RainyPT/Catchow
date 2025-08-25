using UnityEngine;
using UnityEngine.UI;

public class Exit_Play : MonoBehaviour
{
    [SerializeField] private GameObject InitialPanel;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(BackToInitialPanel);
    }

    void BackToInitialPanel()
    {
        this.transform.parent.gameObject.SetActive(false);
        if (this.InitialPanel != null)
        {
            InitialPanel.SetActive(true);
        }
    }
}
