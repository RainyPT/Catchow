using UnityEngine;

public class CookieManager : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 50f; // Rotation Speed (editable in inspector)
    private float currentY;
    void Start()
    {
        
    }

    void Update()
    {
        // Rotation Animation
        currentY += rotationSpeed * Time.deltaTime;
        transform.rotation = Quaternion.Euler(
            transform.rotation.eulerAngles.x,
            currentY,
            transform.rotation.eulerAngles.z
        );
    }
    
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player"))
        {
            RoundManager.Instance.AddScore();
            Destroy(gameObject);
        }    
    }
}
