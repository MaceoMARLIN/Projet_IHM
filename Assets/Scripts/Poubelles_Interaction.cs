using UnityEngine;

// Détecte la collision avec un objet du layer "Dechets" :
// - Si les tags correspondent (bon tri), le score augmente de 1.
// - Sinon (mauvais tri), le score diminue de 1.
// Dans les deux cas, le déchet est détruit.
public class PoubelleCollisionHandler : MonoBehaviour
{
    [Header("Paramètres")]
    [SerializeField] private string dechetsLayerName = "Dechets";

    [Header("Références")]
    [SerializeField] private GameObject visualFeedbackCylinder;
    [SerializeField] private Material correctMaterial;
    [SerializeField] private Material incorrectMaterial;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip correctSound;
    [SerializeField] private AudioClip incorrectSound;

    private int dechetsLayerIndex;
    private MeshRenderer feedbackRenderer;

    private void Awake()
    {
        dechetsLayerIndex = LayerMask.NameToLayer(dechetsLayerName);

        if (visualFeedbackCylinder != null)
        {
            feedbackRenderer = visualFeedbackCylinder.GetComponentInChildren<MeshRenderer>();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        HandlePotentialDechet(collision.gameObject);
    }

    private void HandlePotentialDechet(GameObject other)
    {
        if (other.layer != dechetsLayerIndex)
        {
            return;
        }

        bool sameTag = other.CompareTag(gameObject.tag);

        if (ScoreManager.Instance != null)
        {
            if (sameTag)
            {
                audioSource.PlayOneShot(correctSound);
                ScoreManager.Instance.AddPoint();
            }
            else
            {
                audioSource.PlayOneShot(incorrectSound);
                ScoreManager.Instance.RemovePoint();
            }
        }

        if (feedbackRenderer != null)
        {
            feedbackRenderer.material = sameTag ? correctMaterial : incorrectMaterial;
        }

        Destroy(other);
    }
}
