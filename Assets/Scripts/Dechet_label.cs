using TMPro;
using UnityEngine;

/// <summary>
/// À placer sur chaque objet du layer "Dechets".
/// Gère un texte (TextMeshPro, en World Space) affiché au-dessus du déchet,
/// visible uniquement lorsque l'objet est grab, toujours positionné au-dessus
/// du déchet et orienté vers la caméra du joueur.
/// </summary>
public class DechetLabel : MonoBehaviour
{
    [Header("Références")]
    [Tooltip("Le TextMeshPro (3D) ou TextMeshProUGUI (World Space Canvas) affiché au-dessus du déchet.")]
    [SerializeField] private TMP_Text label;
    [SerializeField] private Color couleurCorrecte;

    [Tooltip("Caméra à regarder. Si vide, utilise Camera.main.")]
    [SerializeField] private Camera targetCamera;

    [Header("Position")]
    [Tooltip("Décalage du texte par rapport au centre du déchet (en local, dans le repère du déchet).")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 0.5f, 0f);

    private bool isVisible;

    private void Awake()
    {
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }

        Hide();
    }

    private void LateUpdate()
    {
        if (!isVisible || label == null || targetCamera == null)
        {
            return;
        }

        Transform labelTransform = label.transform;

        // Position : toujours au-dessus du déchet (indépendamment du parentage).
        labelTransform.position = transform.position + offset;

        // Rotation : toujours face à la caméra (billboard).
        Vector3 directionToCamera = labelTransform.position - targetCamera.transform.position;
        if (directionToCamera.sqrMagnitude > 0.0001f)
        {
            labelTransform.rotation = Quaternion.LookRotation(directionToCamera);
        }
    }

    /// <summary>
    /// Affiche le texte. À appeler quand le déchet est grab.
    /// </summary>
    public void Show()
    {
        isVisible = true;
        if (label != null)
        {
            label.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Cache le texte. À appeler quand le déchet est relâché.
    /// </summary>
    public void Hide()
    {
        isVisible = false;
        if (label != null)
        {
            label.gameObject.SetActive(false);
        }
    }

    public void SetCorrectColor(bool active)
    {
        couleurCorrecte.a = 1f; // Assurez-vous que la couleur est complètement opaque
        if (active)
        {
            label.color = couleurCorrecte;
        }
        else
        {
            label.color = Color.white; // réinitialise la couleur par défaut du TMP
        }
    }
}