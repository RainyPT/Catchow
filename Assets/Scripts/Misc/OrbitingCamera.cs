using UnityEngine;

public class TimedOrbitCamera : MonoBehaviour
{
    [SerializeField] private Vector3 orbitPoint = new Vector3(33f, 3.4f, 31f);
    [SerializeField] private float orbitSpeed = 20f;
    [SerializeField] private Vector3 orbitAxis = Vector3.up;


    void Update()
    {
        transform.RotateAround(orbitPoint, orbitAxis, orbitSpeed * Time.deltaTime);
        transform.LookAt(orbitPoint);
    }
}
