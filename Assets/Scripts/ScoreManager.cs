using TMPro;
using UnityEngine;

// Gère le score du jeu et son affichage via un TextMeshPro.
public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [Header("Références")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI numberOfDechetsTextPommes;
    [SerializeField] private TextMeshProUGUI numberOfDechetsTextEmballages;
    [SerializeField] private TextMeshProUGUI numberOfDechetsTextVerres;

    [Header("Tags des déchets")]
    [SerializeField] private string pommesTag = "Pommes";
    [SerializeField] private string emballagesTag = "Emballages";
    [SerializeField] private string verresTag = "Verres";


    [Header("Paramètres")]
    [Tooltip("Préfixe affiché avant le score, ex: 'Score : '")]
    [SerializeField] private string scorePrefix = "Score : ";

    private int currentScore = 0;
    private int dechetsLayer;

    private void Awake()
    {
        // Mise en place du singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        dechetsLayer = LayerMask.NameToLayer("Dechets");
        UpdateScoreDisplay();
    }

    private void Update()
    {
        UpdateDechetCounts();
    }

    private void UpdateDechetCounts()
    {
        if (numberOfDechetsTextPommes != null)
        {
            numberOfDechetsTextPommes.text = CountDechetsWithTag(pommesTag).ToString();
        }

        if (numberOfDechetsTextEmballages != null)
        {
            numberOfDechetsTextEmballages.text = CountDechetsWithTag(emballagesTag).ToString();
        }

        if (numberOfDechetsTextVerres != null)
        {
            numberOfDechetsTextVerres.text = CountDechetsWithTag(verresTag).ToString();
        }
    }

    private int CountDechetsWithTag(string tag)
    {
        if (string.IsNullOrEmpty(tag) || dechetsLayer < 0)
        {
            return 0;
        }

        var objectsWithTag = GameObject.FindGameObjectsWithTag(tag);
        int count = 0;
        foreach (var obj in objectsWithTag)
        {
            if (obj != null && obj.layer == dechetsLayer)
            {
                count++;
            }
        }

        return count;
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