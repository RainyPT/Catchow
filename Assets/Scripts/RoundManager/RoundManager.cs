using UnityEngine;

public class RoundManager : MonoBehaviour
{
    public static RoundManager Instance; // Singleton reference
    private int score = 0;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddScore()
    {
        score += 1;
        Debug.Log("Score: " + score);
    }

    public int GetScore()
    {
        return score;
    }
}
