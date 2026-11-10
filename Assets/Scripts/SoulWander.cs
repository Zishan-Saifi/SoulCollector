using UnityEngine;

public class SoulWander : MonoBehaviour
{
    [Header("Wander Settings")]
    [SerializeField] private float moveSpeed = 1.2f;
    [SerializeField] private float changeDirectionInterval = 2f;
    [SerializeField] private float mapLimitX = 14f;
    [SerializeField] private float mapLimitY = 8f;

    private Vector2 moveDirection;
    private float timer;

    void Start()
    {
        PickNewDirection();
    }

    void Update()
    {
        // Smoothly move in current direction
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);

        // Timer to change direction
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            PickNewDirection();
        }

        // Keep inside graveyard borders
        Vector3 pos = transform.position;
        if (Mathf.Abs(pos.x) > mapLimitX || Mathf.Abs(pos.y) > mapLimitY)
        {
            // Reverse direction back to center
            moveDirection = -pos.normalized;
            timer = changeDirectionInterval;
        }
    }

    private void PickNewDirection()
    {
        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        moveDirection = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        timer = Random.Range(changeDirectionInterval * 0.7f, changeDirectionInterval * 1.3f);
    }
}