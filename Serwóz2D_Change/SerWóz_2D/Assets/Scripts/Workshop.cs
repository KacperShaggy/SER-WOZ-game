using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles workshop interactions such as buying, placing, and removing towers or carriages.
/// </summary>
public class Workshop : MonoBehaviour
{
    /// <summary>Reference to the player's train.</summary>
    public Train train;

    /// <summary>Prefab for creating new carriages.</summary>
    public GameObject carriagePrefab;

    /// <summary>Determines if the workshop UI is open.</summary>
    public bool isWorkshopOpen = false;

    /// <summary>Prefab for shooting tower.</summary>
    public GameObject shootingTowerPrefab;

    /// <summary>Prefab for active tower.</summary>
    public GameObject activeTowerPrefab;

    /// <summary>Currently hovered (not yet placed) tower.</summary>
    public GameObject hoveringTower;

    /// <summary>Sprite renderer used for highlighting.</summary>
    private SpriteRenderer _spriteRenderer;

    /// <summary>Material property block for shader manipulation.</summary>
    private MaterialPropertyBlock _propertyBlock;

    /// <summary>Intensity of highlight effect.</summary>
    private float highlightFactor = 0.1f;

    /// <summary>Color used when placement is valid.</summary>
    public Color hoverColor;

    /// <summary>Color used when placement is invalid.</summary>
    public Color WrongSpotColor;

    /// <summary>Last hovered tower (for highlight reset).</summary>
    private GameObject lastHovered;

    /// <summary>Current price of selected item.</summary>
    private int price;

    /// <summary>
    /// Initializes material property block.
    /// </summary>
    private void Start()
    {
        _propertyBlock = new MaterialPropertyBlock();
    }

    /// <summary>
    /// Cancels current hovering tower and destroys it.
    /// </summary>
    public void CancelHovering()
    {
        if (hoveringTower != null)
        {
            Destroy(hoveringTower);
            hoveringTower = null;
        }
    }

    /// <summary>
    /// Main update loop handling placement and interaction logic.
    /// </summary>
    void Update()
    {
        if (!isWorkshopOpen)
            return;

        if (hoveringTower != null)
        {
            _spriteRenderer = hoveringTower.GetComponent<SpriteRenderer>();

            /// <summary>Move hovering tower to mouse position.</summary>
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            hoveringTower.transform.position = new Vector3(mousePosition.x, mousePosition.y, -5f);

            /// <summary>Raycast to detect objects under cursor.</summary>
            Ray mouseRay = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit2D[] hits = Physics2D.RaycastAll(mouseRay.origin, mouseRay.direction);

            /// <summary>Check if mouse is over a carriage.</summary>
            RaycastHit2D? carriageHit = null;

            foreach (var hit in hits)
            {
                if (hit.collider != null && hit.transform.CompareTag("Carriage"))
                {
                    carriageHit = hit;
                    break;
                }
            }

            /// <summary>Check collision with other towers.</summary>
            bool collidesWithTower = false;

            SpriteRenderer sr = hoveringTower.GetComponent<SpriteRenderer>();
            Bounds bounds = sr.bounds;

            Collider2D[] overlaps = Physics2D.OverlapBoxAll(bounds.center, bounds.size, 0f);

            foreach (var col in overlaps)
            {
                if (col.CompareTag("Tower") && col.gameObject != hoveringTower)
                {
                    collidesWithTower = true;
                    break;
                }
            }

            /// <summary>Apply highlight effect.</summary>
            _propertyBlock.SetFloat("_Factor", highlightFactor);

            if (carriageHit != null && !collidesWithTower)
            {
                _propertyBlock.SetColor("_Highlight", hoverColor); // valid placement
            }
            else
            {
                _propertyBlock.SetColor("_Highlight", WrongSpotColor); // invalid placement
            }

            _spriteRenderer.SetPropertyBlock(_propertyBlock);

            /// <summary>Handle left mouse click (place tower).</summary>
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                if (carriageHit != null && !collidesWithTower)
                {
                    var hit = carriageHit.Value;

                    train.coinNumber -= price;
                    train.coinNumberText.text = $"Coins: {train.coinNumber}";

                    _propertyBlock.SetFloat("_Factor", 0);
                    _spriteRenderer.SetPropertyBlock(_propertyBlock);

                    hoveringTower.transform.parent = hit.transform;

                    if (hoveringTower.GetComponent<StaticTower>() != null)
                        hoveringTower.GetComponent<StaticTower>().startPosition = hoveringTower.transform.localPosition;

                    hoveringTower = null;
                }
            }

            /// <summary>Handle right mouse click (cancel placement).</summary>
            if (Mouse.current.rightButton.wasPressedThisFrame)
            {
                CancelHovering();
            }
        }
        else
        {
            /// <summary>Handle hovering over existing towers.</summary>
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

            Ray mouseRay = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit2D[] hits = Physics2D.RaycastAll(mouseRay.origin, mouseRay.direction);

            GameObject hoveredObject = null;

            foreach (var hit in hits)
            {
                if (hit.collider != null && hit.transform.CompareTag("Tower"))
                {
                    hoveredObject = hit.transform.gameObject;
                    break;
                }
            }

            /// <summary>Clear highlight if nothing is hovered.</summary>
            if (hoveredObject == null)
            {
                if (lastHovered != null)
                {
                    var prevSr = lastHovered.GetComponent<SpriteRenderer>();
                    if (prevSr != null)
                        prevSr.SetPropertyBlock(new MaterialPropertyBlock());

                    lastHovered = null;
                }

                return;
            }

            /// <summary>Reset previous highlight.</summary>
            if (lastHovered != null && lastHovered != hoveredObject)
            {
                var prevSr = lastHovered.GetComponent<SpriteRenderer>();
                if (prevSr != null)
                    prevSr.SetPropertyBlock(new MaterialPropertyBlock());
            }

            lastHovered = hoveredObject;

            SpriteRenderer sr = hoveredObject.GetComponent<SpriteRenderer>();
            if (sr == null) return;

            MaterialPropertyBlock block = new MaterialPropertyBlock();
            sr.GetPropertyBlock(block);

            block.SetFloat("_Factor", highlightFactor + 0.1f);
            block.SetColor("_Highlight", WrongSpotColor);

            sr.SetPropertyBlock(block);

            /// <summary>Handle left click (remove tower).</summary>
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                Destroy(lastHovered);
            }
        }
    }

    /// <summary>
    /// Buys a shooting tower and starts placement mode.
    /// </summary>
    public void BuyShootingTowerButton()
    {
        if (train.coinNumber < 5)
            return;

        price = 5;

        GameObject newShootingTower = Instantiate(shootingTowerPrefab, transform.position, Quaternion.identity);
        newShootingTower.transform.parent = train.gameObject.transform;

        hoveringTower = newShootingTower;
    }

    /// <summary>
    /// Buys an active tower and starts placement mode.
    /// </summary>
    public void BuyActiveTowerButton()
    {
        if (train.coinNumber < 10)
            return;

        price = 10;

        GameObject newShootingTower = Instantiate(activeTowerPrefab, transform.position, Quaternion.identity);
        newShootingTower.transform.parent = train.gameObject.transform;

        hoveringTower = newShootingTower;
    }

    /// <summary>
    /// Buys a new carriage and attaches it to the train.
    /// </summary>
    public void BuyCarriage()
    {
        if (train.coinNumber < 20)
            return;

        if (train.GetCarriageNumber() >= 4)
            return;

        train.coinNumber -= 20;
        train.coinNumberText.text = $"Coins: {train.coinNumber}";

        GameObject newCarriage = Instantiate(
            carriagePrefab,
            train.getLastCarriagePosition() - new Vector3(carriagePrefab.GetComponent<SpriteRenderer>().bounds.size.x, 0, 0),
            Quaternion.identity
        );

        newCarriage.transform.parent = train.transform;
        train.AddCarrigae(newCarriage);
    }
}