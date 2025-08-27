using UnityEngine;
using Unity.Netcode;

public class CookieManager : NetworkBehaviour
{
    [SerializeField] private float rotationSpeed = 50f; // Rotation Speed (editable in inspector)
    private float currentY;
    void Start()
    {
        
    }


    void Update()
    {
        currentY += rotationSpeed * Time.deltaTime;
        transform.rotation = Quaternion.Euler(
            transform.rotation.eulerAngles.x,
            currentY,
            transform.rotation.eulerAngles.z
        );
    }
    
    
}
