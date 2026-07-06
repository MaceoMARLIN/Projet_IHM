using UnityEngine;

// Script d'interaction 3D basé sur le raycast permettant de "grab" les objets appartenant au layer "Dechets", de les rapprocher/éloigner avec le scroll, et de les relâcher avec un clic.
public class DechetGrabInteraction : MonoBehaviour
{
    [Header("Références")]
    [Tooltip("Caméra utilisée pour le raycast. Si vide, utilise Camera.main.")]
    [SerializeField] private Camera playerCamera;

    [Header("Paramètres de détection")]
    [SerializeField] private float grabRange = 5f;
    [SerializeField] private LayerMask dechetsLayerMask;

    [Header("Paramètres de tenue de l'objet")]
    [Tooltip("Distance initiale de l'objet par rapport au joueur au moment du grab.")]
    [SerializeField] private float defaultHoldDistance = 2f;
    [SerializeField] private float minHoldDistance = 1f;
    [SerializeField] private float maxHoldDistance = 6f;
    [Tooltip("Vitesse à laquelle le scroll rapproche/éloigne l'objet.")]
    [SerializeField] private float scrollSpeed = 2f;
    [Tooltip("Vitesse de lissage du déplacement de l'objet tenu.")]
    [SerializeField] private float followSmoothSpeed = 15f;

    [Header("Contrôles")]
    [SerializeField] private KeyCode grabKey = KeyCode.Mouse0;
    [SerializeField] private string scrollAxisName = "Mouse ScrollWheel";

    private Rigidbody heldRigidbody;
    private float currentHoldDistance;

    // Sauvegarde des propriétés physiques d'origine de l'objet tenu
    private bool originalUseGravity;
    private bool originalIsKinematic;
    private RigidbodyConstraints originalConstraints;

    private void Awake()
    {
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(grabKey))
        {
            if (heldRigidbody == null)
            {
                TryGrab();
            }
            else
            {
                Release();
            }
        }

        if (heldRigidbody != null)
        {
            HandleScroll();
            MoveHeldObject();
        }
    }

    // Lance un raycast depuis la caméra pour tenter d'attraper un déchet.
    private void TryGrab()
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        if (Physics.Raycast(ray, out RaycastHit hit, grabRange, dechetsLayerMask))
        {
            Rigidbody rb = hit.collider.attachedRigidbody;
            if (rb != null)
            {
                Grab(rb);
            }
        }
    }

    private void Grab(Rigidbody rb)
    {
        heldRigidbody = rb;

        // Sauvegarde de l'état physique d'origine
        originalUseGravity = rb.useGravity;
        originalIsKinematic = rb.isKinematic;
        originalConstraints = rb.constraints;

        // On désactive la gravité et on fige les rotations pendant le grab
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.useGravity = false;
        rb.isKinematic = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        // Distance initiale = distance actuelle entre le joueur et l'objet (clampée)
        float distanceToObject = Vector3.Distance(playerCamera.transform.position, rb.position);
        currentHoldDistance = Mathf.Clamp(distanceToObject, minHoldDistance, maxHoldDistance);

        if (currentHoldDistance <= 0f)
        {
            currentHoldDistance = defaultHoldDistance;
        }

        // Affiche le texte descriptif du déchet (si présent) pendant le grab.
        DechetLabel label = rb.GetComponent<DechetLabel>();
        if (label != null)
        {
            label.Show();
        }
    }

    // Ajuste la distance de tenue en fonction du scroll de la molette.
    // Scroll vers le haut = éloigne, scroll vers le bas = rapproche.
    private void HandleScroll()
    {
        float scrollInput = Input.GetAxis(scrollAxisName);

        if (Mathf.Abs(scrollInput) > 0.0001f)
        {
            currentHoldDistance += scrollInput * scrollSpeed;
            currentHoldDistance = Mathf.Clamp(currentHoldDistance, minHoldDistance, maxHoldDistance);
        }
    }

    // Déplace l'objet tenu vers le point cible devant la caméra, à la distance actuelle.
    private void MoveHeldObject()
    {
        Vector3 targetPosition = playerCamera.transform.position
                                  + playerCamera.transform.forward * currentHoldDistance;

        heldRigidbody.MovePosition(
            Vector3.Lerp(heldRigidbody.position, targetPosition, followSmoothSpeed * Time.deltaTime)
        );
    }

    // Relâche l'objet tenu et restaure ses propriétés physiques d'origine.
    private void Release()
    {
        if (heldRigidbody == null) return;

        heldRigidbody.useGravity = originalUseGravity;
        heldRigidbody.isKinematic = originalIsKinematic;
        heldRigidbody.constraints = originalConstraints;

        // Cache le texte descriptif du déchet (si présent) au moment du relâchement.
        DechetLabel label = heldRigidbody.GetComponent<DechetLabel>();
        if (label != null)
        {
            label.Hide();
        }

        heldRigidbody = null;
    }
}