using System.Collections;
using UnityEngine;

public class GhostEnemy : MonoBehaviour
{
    [Header("Speeds")]
    [SerializeField] private float patrolSpeed = 1.4f;    // Normal ghoomne ki aaram wali speed
    [SerializeField] private float chaseSpeed = 3.6f;     // Player ko dekhne par tez bhagne ki speed

    [Header("Detection Ranges")]
    [SerializeField] private float detectionRange = 6.5f; // Kitne paas aane par chase shuru karega
    [SerializeField] private float loseSightRange = 9.5f;  // Kitna door jaane par chase chhod dega

    [Header("Patrol Timers")]
    [SerializeField] private float changeDirectionTime = 2.5f;

    [Header("Floating Bobbing Effect")]
    [SerializeField] private float bobFrequency = 3f;
    [SerializeField] private float bobAmplitude = 0.15f;

    private Transform playerTransform;
    private SpriteRenderer sr;
    private Vector2 wanderDirection;
    private bool isChasing = false;
    private float initialYOffset = 0f;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();

        // Tag "Player" se player ko dhoondo
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }

        // Random direction mein ghoomna shuru karo
        StartCoroutine(WanderRoutine());
    }

    private IEnumerator WanderRoutine()
    {
        while (true)
        {
            if (!isChasing)
            {
                // Random 2D direction choose karo
                wanderDirection = Random.insideUnitCircle.normalized;

                // Sprite flip facing movement direction
                FlipSprite(wanderDirection.x);
            }

            yield return new WaitForSeconds(Random.Range(1.5f, changeDirectionTime));
        }
    }

    private void Update()
    {
        if (playerTransform == null)
        {
            PatrolMove();
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        // State Transition Logic:
        if (!isChasing && distanceToPlayer <= detectionRange)
        {
            // Player detection range mein aaya -> Chase Shuru!
            isChasing = true;
            Debug.Log("GHOST SPOTTED PLAYER! RUSHING!");
        }
        else if (isChasing && distanceToPlayer > loseSightRange)
        {
            // Player bhaag kar door nikal gaya -> Chase band, wapas normal patrol
            isChasing = false;
            Debug.Log("GHOST LOST PLAYER SIGHT! RETURNING TO PATROL.");
        }

        // Movement execute karo state ke mutabiq
        if (isChasing)
        {
            ChasePlayer();
        }
        else
        {
            PatrolMove();
        }

        // Natural floating/hovering animation
        ApplyFloatEffect();
    }

    private void ChasePlayer()
    {
        // Player ki taraf tez speed se move karo
        transform.position = Vector2.MoveTowards(
            transform.position,
            playerTransform.position,
            chaseSpeed * Time.deltaTime
        );

        // Player ki taraf face karo
        FlipSprite(playerTransform.position.x - transform.position.x);
    }

    private void PatrolMove()
    {
        // Normal random slow movement
        transform.Translate(wanderDirection * patrolSpeed * Time.deltaTime, Space.World);
    }

    private void FlipSprite(float directionX)
    {
        if (sr != null)
        {
            if (directionX > 0.05f)
                sr.flipX = false;
            else if (directionX < -0.05f)
                sr.flipX = true;
        }
    }

    private void ApplyFloatEffect()
    {
        // Halka sa up-down bobbing effect taaki ghost hawa mein tairta hua lage
        float bob = Mathf.Sin(Time.time * bobFrequency) * bobAmplitude * Time.deltaTime;
        transform.position += new Vector3(0, bob, 0);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Player ko pakadne par Game Over
        if (collision.CompareTag("Player"))
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.TriggerGameOver("CAUGHT BY THE DEAD!");
            }
        }
    }
}