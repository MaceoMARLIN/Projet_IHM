using UnityEngine;

/// <summary>
/// Fait pointer une flèche d'UI (Image, en rotation 2D dans le Canvas) vers :
/// - le déchet actif le plus proche, verrouillé tant qu'il n'est pas grab ou détruit
///   (ne change pas de cible si un autre déchet devient plus proche entre-temps) ;
/// - la poubelle correspondant au tag du déchet, dès que celui-ci est grab par le joueur.
///
/// À placer sur un GameObject possédant un RectTransform (l'image de la flèche elle-même,
/// ou un objet dédié si tu préfères séparer logique et visuel).
/// </summary>
public class TargetArrowUI : MonoBehaviour
{
    [Header("Références")]
    [Tooltip("RectTransform de la flèche à faire tourner. Si vide, utilise celui de ce GameObject.")]
    [SerializeField] private RectTransform arrowRectTransform;
    [Tooltip("Transform du joueur (position + orientation de référence). Si vide, utilise Camera.main.")]
    [SerializeField] private Transform player;

    [Header("Layers")]
    [SerializeField] private LayerMask dechetsLayerMask;
    [SerializeField] private LayerMask poubellesLayerMask;

    [Header("Recherche de cible")]
    [Tooltip("Rayon de recherche autour du joueur pour trouver déchets/poubelles.")]
    [SerializeField] private float searchRadius = 100f;

    [Header("Offset de rotation")]
    [Tooltip("Angle ajouté à la rotation calculée si l'image de la flèche ne pointe pas vers le haut par défaut.")]
    [SerializeField] private float rotationOffset = 0f;

    private Transform currentDechetTarget;
    private Transform currentPoubelleTarget;
    private GameObject currentGrabbedDechet;
    private bool isDechetGrabbed;

    private void Awake()
    {
        if (arrowRectTransform == null)
        {
            arrowRectTransform = GetComponent<RectTransform>();
        }

        if (player == null && Camera.main != null)
        {
            player = Camera.main.transform;
        }
    }

    private void OnEnable()
    {
        DechetGrabInteraction.OnDechetGrabbed += HandleDechetGrabbed;
        DechetGrabInteraction.OnDechetReleased += HandleDechetReleased;
    }

    private void OnDisable()
    {
        DechetGrabInteraction.OnDechetGrabbed -= HandleDechetGrabbed;
        DechetGrabInteraction.OnDechetReleased -= HandleDechetReleased;
    }

    private void Update()
    {
        if (isDechetGrabbed)
        {
            if (currentGrabbedDechet == null)
            {
                isDechetGrabbed = false;
                currentPoubelleTarget = null;
                currentDechetTarget = null;
            }
            else
            {
                if (!IsTargetValid(currentPoubelleTarget))
                {
                    currentPoubelleTarget = FindNearestTransform(poubellesLayerMask, currentGrabbedDechet.tag);
                }

                if (currentPoubelleTarget != null)
                {
                    PointArrowTo(currentPoubelleTarget.position);
                }
                return;
            }
        }

        if (!IsTargetValid(currentDechetTarget))
        {
            currentDechetTarget = FindNearestTransform(dechetsLayerMask, null);
        }

        if (currentDechetTarget != null)
        {
            PointArrowTo(currentDechetTarget.position);
        }
    }

    private void HandleDechetGrabbed(GameObject dechet)
    {
        currentGrabbedDechet = dechet;
        isDechetGrabbed = true;

        currentPoubelleTarget = FindNearestTransform(poubellesLayerMask, dechet.tag);
    }

    private void HandleDechetReleased(GameObject dechet)
    {
        if (dechet != currentGrabbedDechet)
        {
            return;
        }

        isDechetGrabbed = false;
        currentGrabbedDechet = null;
        currentPoubelleTarget = null;

        // On relâche le verrouillage pour que Update() cherche une nouvelle cible
        // (le déchet relâché redevient un candidat comme un autre s'il est toujours présent).
        currentDechetTarget = null;
    }

    /// <summary>
    /// Cherche le Transform le plus proche du joueur parmi les objets d'un layer donné,
    /// en filtrant optionnellement par tag (utile pour trouver la bonne poubelle).
    /// </summary>
    private Transform FindNearestTransform(LayerMask layerMask, string requiredTag)
    {
        if (player == null) return null;

        Collider[] colliders = Physics.OverlapSphere(player.position, searchRadius, layerMask);

        Transform nearest = null;
        float nearestSqrDistance = float.MaxValue;

        foreach (Collider col in colliders)
        {
            if (!IsTargetValid(col?.transform))
            {
                continue;
            }

            if (!string.IsNullOrEmpty(requiredTag) && !col.CompareTag(requiredTag))
            {
                continue;
            }

            float sqrDistance = (col.transform.position - player.position).sqrMagnitude;
            if (sqrDistance < nearestSqrDistance)
            {
                nearestSqrDistance = sqrDistance;
                nearest = col.transform;
            }
        }

        return nearest;
    }

    private bool IsTargetValid(Transform target)
    {
        return target != null && target.gameObject.activeInHierarchy;
    }

    /// <summary>
    /// Oriente la flèche (en 2D, dans le plan du Canvas) vers une position cible en world space,
    /// en se basant sur la direction relative au joueur.
    /// </summary>
    private void PointArrowTo(Vector3 targetWorldPosition)
    {
        if (player == null || arrowRectTransform == null) return;

        Vector3 direction = targetWorldPosition - player.position;
        direction.y = 0f; // on ignore la hauteur pour une flèche à plat (vue de dessus / boussole)

        if (direction.sqrMagnitude < 0.0001f) return;

        // Direction relative à l'orientation du joueur, pour que la flèche tourne
        // correctement quel que soit le sens dans lequel le joueur regarde.
        Vector3 localDirection = player.InverseTransformDirection(direction);
        float angle = Mathf.Atan2(localDirection.x, localDirection.z) * Mathf.Rad2Deg;

        arrowRectTransform.localRotation = Quaternion.Euler(0f, 0f, -angle + rotationOffset);
    }
}