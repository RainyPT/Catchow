using UnityEngine;
using UnityEngine.UI;

public class PlayButtonScript : MonoBehaviour
{
    [SerializeField] private GameObject PlayPanel;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(ChangeToPlayPanel);
    }


    private void ChangeToPlayPanel()
    {
        this.transform.parent.gameObject.SetActive(false);
        if (this.PlayPanel != null)
        {
            PlayPanel.SetActive(true);
        }
        
    }
}
