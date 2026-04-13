using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public CoreManager coreManager;
    public int currentLevel = 1;
    public int enemiesSpawnedThisLevel = 0;

    [Header("Spawning & Difficulty")]
    public float baseSpawnInterval = 3f;
    public float currentSpawnInterval;
    private float spawnTimer;
    public int enemiesToNextLevel = 10;

    [Header("Paths")]
    public GameObject pathPrefab;
    public float spawnRadius = 15f;
    public List<Transform> spawnPoints = new List<Transform>();
    private List<GameObject> activePaths = new List<GameObject>();

    private Material sharedBugMaterial;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        currentSpawnInterval = baseSpawnInterval;
        if (coreManager != null)
        {
            coreManager.OnHealthChanged.AddListener(UpdatePathBrightness);
            coreManager.OnGameOver.AddListener(HandleGameOver);
        }

        GeneratePathsForLevel();
    }

    void Update()
    {
        if (coreManager.currentHealth <= 0) return;

        spawnTimer += Time.deltaTime;
        if (spawnTimer >= currentSpawnInterval)
        {
            spawnTimer = 0;
            SpawnEnemy();

            enemiesSpawnedThisLevel++;
            if (enemiesSpawnedThisLevel >= enemiesToNextLevel)
            {
                LevelUp();
            }
        }
    }

    void SpawnEnemy()
    {
        if (spawnPoints.Count == 0) return;

        int r = Random.Range(0, spawnPoints.Count);
        Transform spawnPt = spawnPoints[r];

        GameObject bug = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        bug.name = "EnemyBug";
        bug.transform.position = spawnPt.position;

        Enemy enemyScript = bug.AddComponent<Enemy>();
        enemyScript.target = coreManager.transform;
        enemyScript.health = 20f + (currentLevel * 5f);
        enemyScript.speed = 2f + (currentLevel * 0.2f);

        // Setup visuals for Tron Bug
        Renderer rend = bug.GetComponent<Renderer>();
        if (sharedBugMaterial == null)
        {
            sharedBugMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            sharedBugMaterial.EnableKeyword("_EMISSION");
            sharedBugMaterial.SetColor("_EmissionColor", Color.red * 2f);
        }
        rend.sharedMaterial = sharedBugMaterial;
    }

    void LevelUp()
    {
        currentLevel++;
        enemiesSpawnedThisLevel = 0;
        enemiesToNextLevel += 5;
        currentSpawnInterval = Mathf.Max(0.5f, currentSpawnInterval * 0.9f);

        Debug.Log("Level Up! Level " + currentLevel);

        if (currentLevel == 11 || currentLevel == 21)
        {
            GeneratePathsForLevel();
        }
    }

    void GeneratePathsForLevel()
    {
        // Clear old paths
        foreach (var p in activePaths) Destroy(p);
        foreach (var sp in spawnPoints) Destroy(sp.gameObject);

        activePaths.Clear();
        spawnPoints.Clear();

        int numPaths = 1;
        if (currentLevel >= 11 && currentLevel <= 20) numPaths = 2;
        if (currentLevel >= 21) numPaths = 3;

        for (int i = 0; i < numPaths; i++)
        {
            float angle = (Mathf.PI * 2 / numPaths) * i;
            float startX = Mathf.Cos(angle) * spawnRadius;
            float startZ = Mathf.Sin(angle) * spawnRadius;

            Vector3 startPos = new Vector3(startX, 0.1f, startZ);
            Vector3 endPos = coreManager.transform.position;

            // Create Spawn Point
            GameObject sp = new GameObject("SpawnPoint");
            sp.transform.position = startPos;
            spawnPoints.Add(sp.transform);

            // Create visual path
            if (pathPrefab != null)
            {
                GameObject path = Instantiate(pathPrefab);
                path.transform.position = (startPos + endPos) / 2f;
                path.transform.LookAt(endPos);

                float length = Vector3.Distance(startPos, endPos);
                path.transform.localScale = new Vector3(1f, 1f, length);
                activePaths.Add(path);
            }
        }
        UpdatePathBrightness(coreManager.currentHealth / coreManager.maxHealth);
    }

    void UpdatePathBrightness(float healthPct)
    {
        foreach (var path in activePaths)
        {
            Renderer r = path.GetComponentInChildren<Renderer>();
            if (r != null)
            {
                r.material.SetColor("_EmissionColor", Color.cyan * healthPct);
            }
        }
    }

    void HandleGameOver()
    {
        // Destroy all enemies
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        foreach (Enemy e in enemies) Destroy(e.gameObject);
    }
}
