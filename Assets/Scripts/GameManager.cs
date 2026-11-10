using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Start Screen References")]
    [SerializeField] private GameObject startPanel; // Naya Start Panel slot

    [Header("In-Game HUD References")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI timerText;

    [Header("Game Over Screen References")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI gameOverReasonText;
    [SerializeField] private TextMeshProUGUI gameOverSoulsText;

    [Header("Win Screen References")]
    [SerializeField] private GameObject winPanel;
    [SerializeField] private TextMeshProUGUI winSoulsText;
    [SerializeField] private TextMeshProUGUI winTimeLeftText;
    [SerializeField] private TextMeshProUGUI winRankText;

    [Header("3-Star UI References")]
    [SerializeField] private Image star1Image;
    [SerializeField] private Image star2Image;
    [SerializeField] private Image star3Image;
    [SerializeField] private Sprite goldStarSprite;
    [SerializeField] private Sprite emptyStarSprite;

    [Header("Portal & Target")]
    [SerializeField] private GameObject portalObject;
    [SerializeField] private int targetBlueSouls = 10;

    [Header("Prefabs")]
    [SerializeField] private GameObject soulPrefab;
    [SerializeField] private GameObject cursedSoulPrefab;
    [SerializeField] private GameObject ghostPrefab;

    [Header("Spawning Settings")]
    [SerializeField] private float gameDuration = 90f;
    [SerializeField] private float spawnInterval = 0.8f;
    [SerializeField] private float spawnRangeX = 18f;
    [SerializeField] private float spawnRangeY = 10f;

    private int blueSoulsCollected = 0;
    private float timeRemaining;
    private bool isGameOver = false;
    private bool isGameStarted = false; // Game shuru hua ya nahi
    private bool portalOpened = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        timeRemaining = gameDuration;

        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (winPanel != null) winPanel.SetActive(false);
        if (portalObject != null) portalObject.SetActive(true);

        UpdateUI();
        UpdateTimerUI();

        // Start Panel check: agar panel laga hai toh game freeze rakho
        if (startPanel != null)
        {
            startPanel.SetActive(true);
            Time.timeScale = 0f; // Game paused
            isGameStarted = false;
        }
        else
        {
            StartGame();
        }
    }

    // Naya method: Start button click hone par chalega
    public void StartGame()
    {
        if (startPanel != null)
        {
            startPanel.SetActive(false);
        }

        Time.timeScale = 1f; // Gameplay start
        isGameStarted = true;
        StartCoroutine(SpawnRoutine());
    }

    private void Update()
    {
        // Jab tak start button na dabao ya game over ho tab tak timer na chale
        if (!isGameStarted || isGameOver) return;

        timeRemaining -= Time.deltaTime;
        if (timeRemaining <= 0)
        {
            timeRemaining = 0;
            TriggerGameOver("TIME RAN OUT!");
        }

        UpdateTimerUI();
    }

    private IEnumerator SpawnRoutine()
    {
        while (!isGameOver && isGameStarted)
        {
            yield return new WaitForSeconds(spawnInterval);
            SpawnRandomSoul();
        }
    }

    private void SpawnRandomSoul()
    {
        Vector2 spawnPos = new Vector2(
            Random.Range(-spawnRangeX, spawnRangeX),
            Random.Range(-spawnRangeY, spawnRangeY)
        );

        GameObject prefabToSpawn = (Random.value < 0.35f && cursedSoulPrefab != null) ? cursedSoulPrefab : soulPrefab;
        if (prefabToSpawn != null) Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);
    }

    public void AddBlueSoul()
    {
        if (isGameOver || !isGameStarted) return;
        blueSoulsCollected++;
        UpdateUI();

        if (blueSoulsCollected >= targetBlueSouls && !portalOpened)
        {
            OpenPortal();
        }
    }

    public void AddScore(int amount)
    {
        if (isGameOver || !isGameStarted) return;
        blueSoulsCollected = Mathf.Max(0, blueSoulsCollected + amount);
        UpdateUI();

        if (blueSoulsCollected >= targetBlueSouls && !portalOpened)
        {
            OpenPortal();
        }
    }

    public void DeductScore(int penalty)
    {
        AddScore(-penalty);
    }

    public void SpawnExtraGhost()
    {
        if (ghostPrefab == null || isGameOver || !isGameStarted) return;

        Vector2 spawnPos = new Vector2(
            Random.Range(-spawnRangeX, spawnRangeX),
            Random.Range(-spawnRangeY, spawnRangeY)
        );
        Instantiate(ghostPrefab, spawnPos, Quaternion.identity);
    }

    private void OpenPortal()
    {
        portalOpened = true;
        if (portalObject != null)
        {
            Portal portalScript = portalObject.GetComponent<Portal>();
            if (portalScript != null) portalScript.ActivatePortal();
        }
    }

    // WIN SCREEN WITH STAR CALCULATION
    public void PlayerEscaped()
    {
        if (isGameOver) return;
        isGameOver = true;
        Time.timeScale = 0f;

        if (winPanel != null) winPanel.SetActive(true);

        int starsEarned = 1; // 1 Star guaranteed for escaping
        if (timeRemaining >= 30f) starsEarned++;
        if (blueSoulsCollected >= 15) starsEarned++;

        // Apply Star Sprites
        SetStarSprite(star1Image, starsEarned >= 1);
        SetStarSprite(star2Image, starsEarned >= 2);
        SetStarSprite(star3Image, starsEarned >= 3);

        if (winSoulsText != null) winSoulsText.text = "Souls Escaped: " + blueSoulsCollected;

        if (winTimeLeftText != null)
        {
            int mins = Mathf.FloorToInt(timeRemaining / 60F);
            int secs = Mathf.FloorToInt(timeRemaining % 60F);
            winTimeLeftText.text = string.Format("Time Left: {0:00}:{1:00}", mins, secs);
        }

        if (winRankText != null)
        {
            if (starsEarned == 3) winRankText.text = "Rank: S-GRADE";
            else if (starsEarned == 2) winRankText.text = "Rank: A-GRADE";
            else winRankText.text = "Rank: B-GRADE";
        }
    }

    private void SetStarSprite(Image starImg, bool isEarned)
    {
        if (starImg != null)
        {
            starImg.sprite = isEarned ? goldStarSprite : emptyStarSprite;
        }
    }

    // GAME OVER SCREEN
    public void TriggerGameOver(string reason)
    {
        if (isGameOver) return;
        isGameOver = true;
        Time.timeScale = 0f;

        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        if (gameOverReasonText != null) gameOverReasonText.text = reason;
        if (gameOverSoulsText != null) gameOverSoulsText.text = "Souls Collected: " + blueSoulsCollected;
    }

    private void UpdateUI()
    {
        if (scoreText != null) scoreText.text = "Souls: " + blueSoulsCollected + " / " + targetBlueSouls;
    }

    private void UpdateTimerUI()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(timeRemaining / 60F);
            int seconds = Mathf.FloorToInt(timeRemaining % 60F);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}