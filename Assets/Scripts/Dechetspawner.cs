using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Génère aléatoirement des déchets dans une zone de la scène lorsque SpawnDechet()
/// est appelée (ex: depuis le OnClick d'un bouton UI), en respectant un nombre
/// maximum de déchets présents simultanément.
/// À placer sur un GameObject vide dans la scène.
/// </summary>
public class DechetSpawner : MonoBehaviour
{
    [Header("Préfabs de déchets")]
    [Tooltip("Liste des préfabs de déchets pouvant être générés (un est choisi au hasard à chaque spawn).")]
    [SerializeField] private List<GameObject> dechetPrefabs = new List<GameObject>();

    [Header("Zone de spawn")]
    [Tooltip("Centre de la zone de spawn. Si vide, utilise la position de ce GameObject.")]
    [SerializeField] private Transform spawnAreaCenter;
    [Tooltip("Dimensions de la zone de spawn (en mètres, sur X/Y/Z) autour du centre.")]
    [SerializeField] private Vector3 spawnAreaSize = new Vector3(10f, 0f, 10f);

    [Header("Paramètres de génération")]
    [Tooltip("Nombre maximum de déchets présents simultanément dans la scène.")]
    [SerializeField] private int maxDechetsAlive = 15;
    [Tooltip("Applique une rotation aléatoire sur l'axe Y au moment du spawn.")]
    [SerializeField] private bool randomYRotation = true;
    [Tooltip("Parent sous lequel les déchets générés seront rangés dans la hiérarchie (facultatif).")]
    [SerializeField] private Transform spawnParent;
    [Tooltip("Nombre minimum de déchets générés en un seul appel de SpawnRandomBatch().")]
    [SerializeField] private int minBatchCount = 1;
    [Tooltip("Nombre maximum de déchets générés en un seul appel de SpawnRandomBatch().")]
    [SerializeField] private int maxBatchCount = 10;

    private readonly List<GameObject> spawnedDechets = new List<GameObject>();

    private void Start()
    {
        //SpawnRandomBatch();
    }

    private void Awake()
    {
        if (spawnAreaCenter == null)
        {
            spawnAreaCenter = transform;
        }
    }

    /// <summary>
    /// Génère un nombre aléatoire de déchets (entre minBatchCount et maxBatchCount inclus).
    /// À appeler depuis l'événement OnClick d'un bouton UI,
    /// ou depuis n'importe quel autre script (ex: spawnerScript.SpawnRandomBatch()).
    /// </summary>
    public void SpawnRandomBatch()
    {
        int amountToSpawn = Random.Range(minBatchCount, maxBatchCount + 1);

        for (int i = 0; i < amountToSpawn; i++)
        {
            SpawnDechet();
        }
    }

    /// <summary>
    /// Génère un seul déchet. À appeler depuis l'événement OnClick d'un bouton UI,
    /// ou depuis n'importe quel autre script (ex: spawnerScript.SpawnDechet()).
    /// </summary>
    public void SpawnDechet()
    {
        // Nettoie la liste des déchets détruits (ramassés) avant de vérifier le nombre présent.
        spawnedDechets.RemoveAll(dechet => dechet == null);

        if (spawnedDechets.Count >= maxDechetsAlive)
        {
            Debug.Log("DechetSpawner : nombre maximum de déchets atteint, spawn annulé.");
            return;
        }

        if (dechetPrefabs.Count == 0)
        {
            Debug.LogWarning("DechetSpawner : aucun préfab de déchet n'est assigné.");
            return;
        }

        GameObject prefabToSpawn = dechetPrefabs[Random.Range(0, dechetPrefabs.Count)];
        Vector3 spawnPosition = GetRandomSpawnPosition();
        Quaternion spawnRotation = randomYRotation
            ? Quaternion.Euler(0f, Random.Range(0f, 360f), 0f)
            : prefabToSpawn.transform.rotation;

        GameObject newDechet = Instantiate(prefabToSpawn, spawnPosition, spawnRotation, spawnParent);
        spawnedDechets.Add(newDechet);
    }

    private Vector3 GetRandomSpawnPosition()
    {
        Vector3 center = spawnAreaCenter.position;

        float randomX = Random.Range(-spawnAreaSize.x / 2f, spawnAreaSize.x / 2f);
        float randomY = Random.Range(-spawnAreaSize.y / 2f, spawnAreaSize.y / 2f);
        float randomZ = Random.Range(-spawnAreaSize.z / 2f, spawnAreaSize.z / 2f);

        return center + new Vector3(randomX, randomY, randomZ);
    }

    // Affiche la zone de spawn dans l'éditeur pour faciliter le réglage.
    private void OnDrawGizmosSelected()
    {
        Transform center = spawnAreaCenter != null ? spawnAreaCenter : transform;
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(center.position, spawnAreaSize);
    }
}