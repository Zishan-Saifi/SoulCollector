using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 6f;

    [Header("Player Ground Boundaries")]
    [SerializeField] private bool enableBounds = true;
    [SerializeField] private float minX = -17f;
    [SerializeField] private float maxX = 17f;
    [SerializeField] private float minY = -9.5f;
    [SerializeField] private float maxY = 9.5f;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Gravity zero aur rotation freeze rakho
        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.freezeRotation = true;
        }
    }

    void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        Vector2 movement = new Vector2(moveX, moveY).normalized;

        if (rb != null)
        {
            rb.linearVelocity = movement * moveSpeed; // Unity 6 mein linearVelocity ya velocity
        }
        else
        {
            transform.position += (Vector3)(movement * moveSpeed * Time.deltaTime);
        }

        // Player ko ground ke borders ke andar lock rakhna
        if (enableBounds)
        {
            float clampedX = Mathf.Clamp(transform.position.x, minX, maxX);
            float clampedY = Mathf.Clamp(transform.position.y, minY, maxY);
            transform.position = new Vector3(clampedX, clampedY, transform.position.z);
        }

        // Left / Right mudne ke liye SpriteRenderer flip use karo (Scale nahi!)
        if (spriteRenderer != null)
        {
            if (moveX < 0)
            {
                spriteRenderer.flipX = true;  // Left face
            }
            else if (moveX > 0)
            {
                spriteRenderer.flipX = false; // Right face
            }
        }
    }
}