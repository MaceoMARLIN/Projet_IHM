using TMPro;
using UnityEngine;

// Gère le score du jeu et son affichage via un TextMeshPro.
public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [Header("Références")]
    [SerializeField] private TextMeshProUGUI scoreText;

    [Header("Paramètres")]
    [Tooltip("Préfixe affiché avant le score, ex: 'Score : '")]
    [SerializeField] private string scorePrefix = "Score : ";

    private int currentScore = 0;

    private void Awake()
    {
        // Mise en place du singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        UpdateScoreDisplay();
    }

    public void AddPoint()
    {
        currentScore++;
        UpdateScoreDisplay();
    }

    public void RemovePoint()
    {
        currentScore--;
        UpdateScoreDisplay();
    }

    public int GetScore()
    {
        return currentScore;
    }

    private void UpdateScoreDisplay()
    {
        if (scoreText != null)
        {
            scoreText.text = scorePrefix + currentScore;
        }
    }
}