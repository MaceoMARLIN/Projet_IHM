using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class ScrollbarData
{
    public float value;
    public string text;
}

[System.Serializable]
public class ScoreData
{
    public int score;
    public int erreurs;
    public int temps;
    public List<ScrollbarData> scrollbars = new List<ScrollbarData>();
}

public class ScoreFinal : MonoBehaviour
{
    [Header("References UI with values")]
    [SerializeField] private TMPro.TextMeshProUGUI scoreText;
    [SerializeField] private TMPro.TextMeshProUGUI erreurText;
    [SerializeField] private TMPro.TextMeshProUGUI tempsText;

    [Header("References UI to display values")]
    [SerializeField] private TMPro.TextMeshProUGUI scoreFinalText;
    [SerializeField] private TMPro.TextMeshProUGUI erreurFinalText;
    [SerializeField] private TMPro.TextMeshProUGUI tempsFinalText;
    [SerializeField] private TMPro.TextMeshProUGUI ResultFinalText;
    [SerializeField] private Canvas gameOverMenu;

    [Header("Scrollbar References")]
    [SerializeField] private UnityEngine.UI.Scrollbar scrollbar1;
    [SerializeField] private UnityEngine.UI.Scrollbar scrollbar2;
    [SerializeField] private UnityEngine.UI.Scrollbar scrollbar3;
    [SerializeField] private UnityEngine.UI.Scrollbar scrollbar4;

    [Header("Associated texts for each scrollbar")]
    [SerializeField] private TMPro.TextMeshProUGUI scrollbarText1;
    [SerializeField] private TMPro.TextMeshProUGUI scrollbarText2;
    [SerializeField] private TMPro.TextMeshProUGUI scrollbarText3;
    [SerializeField] private TMPro.TextMeshProUGUI scrollbarText4;

    [Header("JSON export")]
    [SerializeField] private string jsonFileName = "score_results.json";

    void Start()
    {
    }

    void Update()
    {
        if (gameOverMenu != null && gameOverMenu.enabled)
        {
            if (scoreFinalText != null)
                scoreFinalText.text = scoreText != null ? scoreText.text : "0";

            if (erreurFinalText != null)
                erreurFinalText.text = erreurText != null ? erreurText.text : "0";

            if (tempsFinalText != null)
                tempsFinalText.text = tempsText != null ? tempsText.text : "0";

            int scoreValue = TryParseScore(scoreText != null ? scoreText.text : "0");

            if (ResultFinalText != null)
            {
                ResultFinalText.text = scoreValue > 0
                    ? "Succès ! Majorité des déchets triés correctement."
                    : "Échec ! Majorité des déchets triés incorrectement.";
            }
        }
    }

    private int TryParseScore(string text)
    {
        if (string.IsNullOrEmpty(text))
            return 0;

        string numericText = "";
        bool hasSign = false;

        foreach (char c in text)
        {
            if ((c == '-' || c == '+') && !hasSign && numericText.Length == 0)
            {
                numericText += c;
                hasSign = true;
            }
            else if (char.IsDigit(c))
            {
                numericText += c;
            }
        }

        return int.TryParse(numericText, out int value) ? value : 0;
    }

    public void ScoreToJson()
    {
        int scoreValue = TryParseScore(scoreFinalText != null ? scoreFinalText.text : (scoreText != null ? scoreText.text : "0"));
        int erreurValue = TryParseScore(erreurFinalText != null ? erreurFinalText.text : (erreurText != null ? erreurText.text : "0"));
        int tempsValue = TryParseScore(tempsFinalText != null ? tempsFinalText.text : (tempsText != null ? tempsText.text : "0"));

        ScoreData scoreData = new ScoreData
        {
            score = scoreValue,
            erreurs = erreurValue,
            temps = tempsValue
        };

        scoreData.scrollbars.Add(CreateScrollbarEntry(scrollbar1, scrollbarText1));
        scoreData.scrollbars.Add(CreateScrollbarEntry(scrollbar2, scrollbarText2));
        scoreData.scrollbars.Add(CreateScrollbarEntry(scrollbar3, scrollbarText3));
        scoreData.scrollbars.Add(CreateScrollbarEntry(scrollbar4, scrollbarText4));

        string json = JsonUtility.ToJson(scoreData, true);
        string projectPath = Path.GetDirectoryName(Application.dataPath);
        string filePath = Path.Combine(projectPath, jsonFileName);

        try
        {
            File.WriteAllText(filePath, json);
            Debug.Log("Score exporté en JSON : " + filePath);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Erreur lors de l'écriture du JSON : " + e.Message);
        }
    }

    public void scoreToJson()
    {
        ScoreToJson();
    }

    private ScrollbarData CreateScrollbarEntry(UnityEngine.UI.Scrollbar scrollbar, TMPro.TextMeshProUGUI associatedText)
    {
        return new ScrollbarData
        {
            value = scrollbar != null ? scrollbar.value : 0f,
            text = associatedText != null ? associatedText.text : string.Empty
        };
    }
}
