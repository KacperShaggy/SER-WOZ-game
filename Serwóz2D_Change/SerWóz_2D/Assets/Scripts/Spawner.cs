using UnityEngine;
using System.Collections.Generic;
using TMPro;

/// <summary>
/// Handles enemy wave generation and spawning logic.
/// </summary>
public class Spawner : MonoBehaviour
{
    /// <summary>Reference to the map generator (controls game state).</summary>
    [SerializeField] private MapGenerator mapGenerator;

    /// <summary>List of currently active enemies in the wave.</summary>
    [SerializeField] public List<GameObject> spawnEnemies = new List<GameObject>();

    /// <summary>Maximum number of waves in the level.</summary>
    [SerializeField] int maxWaveNumber = 5;

    /// <summary>Current wave index.</summary>
    [SerializeField] int currentWaveNumber = 1;

    /// <summary>Number of enemies spawned per wave.</summary>
    [SerializeField] int waveLevel = 6;

    /// <summary>Increment applied to waveLevel after each wave.</summary>
    [SerializeField] int waveUpgrade = 2;

    /// <summary>UI text displaying current wave progress.</summary>
    [SerializeField] private TMP_Text waveCounterText;

    /// <summary>Enemy prefab used for instantiation.</summary>
    [SerializeField] private GameObject enemyPrefab;

    /// <summary>Vertical spawn distance from spawner position.</summary>
    [SerializeField] private float spawnDistanceY = 10f;

    /// <summary>Horizontal spawn range from spawner position.</summary>
    [SerializeField] private float spawnDistanceX = 5f;

    /// <summary>Fixed Z position for spawned enemies.</summary>
    private float zPosition = -1f;

    /// <summary>Indicates whether the game is currently running.</summary>
    private bool isGamePlaying = false;

    /// <summary>
    /// Starts the wave system and generates the first wave.
    /// </summary>
    public void StartWave()
    {
        isGamePlaying = true;
        GenerateWave();
    }

    /// <summary>
    /// Generates a new wave of enemies.
    /// </summary>
    public void GenerateWave()
    {
        waveCounterText.text = $"Wave: {currentWaveNumber} / {maxWaveNumber}";

        spawnEnemies = new List<GameObject>();

        for (int i = 0; i < waveLevel; i++)
        {
            SpawnEnemy();
        }

        waveLevel += waveUpgrade;
    }

    /// <summary>
    /// Spawns a single enemy at a randomized position.
    /// </summary>
    void SpawnEnemy()
    {
        /// <summary>Determine vertical spawn position.</summary>
        float yPos = transform.position.y + spawnDistanceY;

        if (Random.value < 0.5f)
        {
            yPos = transform.position.y - spawnDistanceY;
        }

        /// <summary>Determine horizontal spawn position.</summary>
        float xPos = transform.position.x + Random.Range(-spawnDistanceX, spawnDistanceX);

        /// <summary>Instantiate enemy.</summary>
        GameObject newEnemy = Instantiate(
            enemyPrefab,
            new Vector3(xPos, yPos, zPosition),
            Quaternion.identity
        );

        /// <summary>Assign shared enemy list reference.</summary>
        newEnemy.gameObject.GetComponent<Enemy>().enemiesList = spawnEnemies;

        spawnEnemies.Add(newEnemy);
    }

    /// <summary>
    /// Resets expedition progress (e.g., when restarting).
    /// </summary>
    public void NewExpeditionClick()
    {
        currentWaveNumber = 0;
    }

    /// <summary>
    /// Main update loop controlling wave progression and game state.
    /// </summary>
    void Update()
    {
        if (!isGamePlaying) return;

        /// <summary>Check if wave is cleared and more waves remain.</summary>
        if (currentWaveNumber < maxWaveNumber && spawnEnemies.Count == 0)
        {
            currentWaveNumber++;
            GenerateWave();
        }
        /// <summary>All waves completed.</summary>
        else if (currentWaveNumber >= maxWaveNumber && spawnEnemies.Count == 0)
        {
            mapGenerator.CompleteLevel();
        }
        else
        {
            /// <summary>Wave in progress.</summary>
            mapGenerator.GoExpedition();
        }
    }
}