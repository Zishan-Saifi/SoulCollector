using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;       // Player ka reference
    [SerializeField] private float smoothSpeed = 5f;  // Camera kitna smooth chalega
    [SerializeField] private Vector3 offset = new Vector3(0, 0, -10f); // 2D camera Z hamesha -10 rehta hai

    [Header("Camera Ground Boundaries")]
    [SerializeField] private bool enableBounds = true;
    [SerializeField] private float minX = -12f;
    [SerializeField] private float maxX = 12f;
    [SerializeField] private float minY = -7f;
    [SerializeField] private float maxY = 7f;

    void LateUpdate()
    {
        if (target == null) return;

        // Player ke piche smoothly chalna
        Vector3 targetPos = target.position + offset;

        // Camera ko ground ke andar clamp karna
        if (enableBounds)
        {
            targetPos.x = Mathf.Clamp(targetPos.x, minX, maxX);
            targetPos.y = Mathf.Clamp(targetPos.y, minY, maxY);
        }

        transform.position = Vector3.Lerp(transform.position, targetPos, smoothSpeed * Time.deltaTime);
    }
}