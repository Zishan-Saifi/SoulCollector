using UnityEngine;

public class CursedSoul : MonoBehaviour
{
    [SerializeField] private int penaltyAmount = 2;
    [SerializeField] private float lifetime = 12f;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("CURSED SOUL TOUCHED! PENALTY & SUMMONING GHOST!");

            if (GameManager.Instance != null)
            {
                // Score deduct karo
                GameManager.Instance.DeductScore(penaltyAmount);

                // Naya ghost summon karo!
                GameManager.Instance.SpawnExtraGhost();
            }

            Destroy(gameObject);
        }
    }
}