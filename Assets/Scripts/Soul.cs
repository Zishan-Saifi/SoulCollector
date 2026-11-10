using UnityEngine;

public class Soul : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(FloatEffect());
    }

    private System.Collections.IEnumerator FloatEffect()
    {
        Vector3 startPos = transform.position;
        while (true)
        {
            float newY = startPos.y + Mathf.Sin(Time.time * 3f) * 0.15f;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
            yield return null;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Naye GameManager mein 10 Blue Souls count karega aur Portal kholega
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddBlueSoul();
            }

            Destroy(gameObject);
        }
    }
}