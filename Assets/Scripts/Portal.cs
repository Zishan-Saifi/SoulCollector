using System.Collections;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [Header("Gate Sprites")]
    [SerializeField] private Sprite closedGateSprite;
    [SerializeField] private Sprite activeGateSprite;

    [Header("Entrance Settings")]
    [SerializeField] private Transform portalCenterPoint; // Jis point par player khinchega
    [SerializeField] private float enterDuration = 0.8f;   // Kitni der mein andar vanish hoga

    private SpriteRenderer sr;
    private Collider2D col;
    private bool isPortalActive = false;
    private bool playerEntering = false;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();

        if (sr != null && closedGateSprite != null)
        {
            sr.sprite = closedGateSprite;
        }

        if (col != null)
        {
            col.enabled = false;
        }
    }

    public void ActivatePortal()
    {
        isPortalActive = true;

        if (sr != null && activeGateSprite != null)
        {
            sr.sprite = activeGateSprite;
        }

        if (col != null)
        {
            col.enabled = true;
        }

        Debug.Log("GATE IS OPEN: ESCAPE!");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isPortalActive && !playerEntering && collision.CompareTag("Player"))
        {
            StartCoroutine(PlayerEnterPortalRoutine(collision.gameObject));
        }
    }

    private IEnumerator PlayerEnterPortalRoutine(GameObject player)
    {
        playerEntering = true;

        // 1. Player ka input aur movement band karo
        MonoBehaviour movementScript = player.GetComponent("PlayerController") as MonoBehaviour;
        if (movementScript != null)
        {
            movementScript.enabled = false;
        }

        Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
        if (playerRb != null)
        {
            playerRb.linearVelocity = Vector2.zero;
            playerRb.bodyType = RigidbodyType2D.Kinematic; // Physics rok do
        }

        Vector3 startPos = player.transform.position;
        Vector3 targetPos = portalCenterPoint != null ? portalCenterPoint.position : transform.position;
        Vector3 startScale = player.transform.localScale;
        SpriteRenderer playerSr = player.GetComponent<SpriteRenderer>();

        float elapsed = 0f;

        // 2. Player portal ke andar khinchta hai aur shrink/fade hota hai
        while (elapsed < enterDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / enterDuration;

            // Center ki taraf move
            player.transform.position = Vector3.Lerp(startPos, targetPos, t);

            // Scale down (shrink into portal)
            player.transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t);

            // Fade out transparency
            if (playerSr != null)
            {
                Color c = playerSr.color;
                c.a = Mathf.Lerp(1f, 0f, t);
                playerSr.color = c;
            }

            yield return null;
        }

        player.SetActive(false); // Player disappear

        yield return new WaitForSeconds(0.2f);

        // 3. Pura andar jaane ke baad Win Screen open karo
        GameManager.Instance.PlayerEscaped();
    }
}