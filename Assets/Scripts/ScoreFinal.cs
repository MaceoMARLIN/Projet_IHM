using UnityEngine;

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

    void Start()
    {
    }

    // Update is called once per frame
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
}
