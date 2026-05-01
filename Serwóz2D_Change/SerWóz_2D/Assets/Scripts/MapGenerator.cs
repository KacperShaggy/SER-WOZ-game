using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;

/// <summary>
/// Handles procedural map generation, UI transitions, and game state flow.
/// </summary>
public class MapGenerator : MonoBehaviour
{
    /// <summary>Main gameplay UI.</summary>
    [SerializeField] GameObject gameViewUI;

    /// <summary>Main menu UI.</summary>
    [SerializeField] GameObject menuViewUI;

    /// <summary>Defeat screen UI.</summary>
    [SerializeField] GameObject defeatViewUI;

    /// <summary>Current game movement speed.</summary>
    [SerializeField]
    public float gameSpeed = 2.5f;

    /// <summary>Indicates if the level has been completed.</summary>
    public bool levelComplete = false;

    /// <summary>Stops train movement when true.</summary>
    public bool stopTrain = false;

    /// <summary>Prefab for standard terrain chunk.</summary>
    [SerializeField]
    private GameObject chunkPrefab;

    /// <summary>Prefab for workshop chunk.</summary>
    [SerializeField]
    private GameObject workshopChunkPrefab;

    /// <summary>UI panel for workshop interaction.</summary>
    [SerializeField] private RectTransform sidePanel;

    /// <summary>Speed of panel sliding animation.</summary>
    public float slideSpeed = 500f;

    /// <summary>Reference to Workshop component.</summary>
    Workshop workshop;

    /// <summary>Hidden position of side panel.</summary>
    private Vector2 hiddenPosition;

    /// <summary>Visible position of side panel.</summary>
    private Vector2 shownPosition;

    /// <summary>Currently running UI animation coroutine.</summary>
    private Coroutine currentAnimation;

    /// <summary>Reference to enemy spawner.</summary>
    public Spawner spawner;

    /// <summary>Last generated chunk.</summary>
    private GameObject lastChunk;

    /// <summary>Starting X position (unused currently).</summary>
    private float startX = 0f;

    /// <summary>
    /// Initializes UI states and creates initial map chunk.
    /// </summary>
    void Start()
    {
        shownPosition = sidePanel.anchoredPosition;
        hiddenPosition = new Vector2(
            sidePanel.anchoredPosition.x + sidePanel.rect.width,
            sidePanel.anchoredPosition.y
        );

        sidePanel.position = hiddenPosition;

        gameViewUI.SetActive(false);
        menuViewUI.SetActive(true);
        defeatViewUI.SetActive(false);

        workshop = transform.GetComponent<Workshop>();

        CreateChunk(false, true);
        HidePanel();
    }

    /// <summary>
    /// Starts the game from the menu.
    /// </summary>
    public void startGameButton()
    {
        gameViewUI.SetActive(true);
        menuViewUI.SetActive(false);
        spawner.StartWave();
    }

    /// <summary>
    /// Triggers defeat state and displays defeat UI.
    /// </summary>
    public void Defeat()
    {
        gameViewUI.SetActive(false);
        menuViewUI.SetActive(false);
        defeatViewUI.SetActive(true);
    }

    /// <summary>
    /// Reloads the current scene.
    /// </summary>
    public void PlayAgain()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
    }

    /// <summary>
    /// Creates a new map chunk.
    /// </summary>
    /// <param name="isWorkshop">Whether the chunk is a workshop.</param>
    /// <param name="first">Whether this is the initial chunk.</param>
    void CreateChunk(bool isWorkshop = false, bool first = false)
    {
        GameObject _chunkPrefab;

        if (isWorkshop)
            _chunkPrefab = workshopChunkPrefab;
        else
            _chunkPrefab = chunkPrefab;

        GameObject newChunk = Instantiate(_chunkPrefab, transform);
        newChunk.transform.parent = transform;

        Tilemap tilemap = newChunk.GetComponentInChildren<Tilemap>();

        /// <summary>Calculate chunk width based on tilemap.</summary>
        float chunkWidth = tilemap.cellBounds.size.x * tilemap.layoutGrid.cellSize.x;

        lastChunk = newChunk;

        /// <summary>Position new chunk.</summary>
        newChunk.transform.position = new Vector3(chunkWidth, 0, 0);

        if (isWorkshop)
            stopTrain = true;

        /// <summary>Create additional initial chunk.</summary>
        if (first)
        {
            GameObject newChunk2 = Instantiate(_chunkPrefab, transform);
            newChunk2.transform.parent = transform;
            newChunk2.transform.position = new Vector3(0, 0, 0);
        }
    }

    /// <summary>
    /// Shows the workshop side panel with animation.
    /// </summary>
    public void ShowPanel()
    {
        sidePanel.gameObject.SetActive(true);

        if (currentAnimation != null)
            StopCoroutine(currentAnimation);

        currentAnimation = StartCoroutine(
            Slide(sidePanel.anchoredPosition, shownPosition)
        );
    }

    /// <summary>
    /// Hides the workshop side panel with animation.
    /// </summary>
    public void HidePanel()
    {
        if (currentAnimation != null)
            StopCoroutine(currentAnimation);

        currentAnimation = StartCoroutine(
            Slide(sidePanel.anchoredPosition, hiddenPosition, true)
        );
    }

    /// <summary>
    /// Smoothly interpolates panel position.
    /// </summary>
    /// <param name="start">Starting position.</param>
    /// <param name="end">Target position.</param>
    /// <param name="disableAfter">Whether to disable panel after animation.</param>
    /// <returns>Coroutine enumerator.</returns>
    IEnumerator Slide(Vector2 start, Vector2 end, bool disableAfter = false)
    {
        float t = 0f;

        float duration = Vector2.Distance(start, end) / slideSpeed;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            sidePanel.anchoredPosition = Vector2.Lerp(start, end, t);
            yield return null;
        }

        sidePanel.anchoredPosition = end;

        //if (disableAfter)
        //    sidePanel.gameObject.SetActive(false);
    }

    /// <summary>
    /// Marks level as completed.
    /// </summary>
    public void CompleteLevel()
    {
        levelComplete = true;
    }

    /// <summary>
    /// Resumes expedition gameplay after workshop.
    /// </summary>
    public void GoExpedition()
    {
        workshop.isWorkshopOpen = false;
        workshop.CancelHovering();

        gameSpeed = 4f;
        stopTrain = false;
        levelComplete = false;

        HidePanel();
    }

    /// <summary>
    /// Placeholder for chunk destruction logic (not implemented).
    /// </summary>
    void DestroyLastChunk()
    {

    }

    /// <summary>
    /// Main update loop handling movement and chunk generation.
    /// </summary>
    void Update()
    {
        /// <summary>Move map to simulate train movement.</summary>
        transform.position += Vector3.left * gameSpeed * Time.deltaTime;

        /// <summary>Check if last chunk reached threshold.</summary>
        if (lastChunk.transform.position.x <= 0)
        {
            if (stopTrain)
            {
                /// <summary>Stop train and open workshop.</summary>
                gameSpeed = 0f;
                ShowPanel();
                workshop.isWorkshopOpen = true;
            }
            else if (levelComplete)
            {
                /// <summary>Create workshop chunk after level completion.</summary>
                Debug.Log("Creating workshop chunk");
                CreateChunk(workshopChunkPrefab);
                levelComplete = false;
            }
            else
            {
                /// <summary>Create regular chunk.</summary>
                CreateChunk();
            }
        }
    }
}