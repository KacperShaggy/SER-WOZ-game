```csharp id = "p7x2kd"
using UnityEngine;

/**
 * @class MapGenerating
 * @brief Responsible for generating rows of ground tiles based on camera view.
 *
 * This class dynamically creates ground rows that fill the visible area of the camera.
 * Each row consists of multiple nodes (tiles), including special tiles like rails.
 */
public class MapGenerating : MonoBehaviour
{
    /** @brief Default ground tile prefab */
    [SerializeField]
    private GameObject nodePrefab;

    /** @brief Rails object prefab */
    [SerializeField]
    private GameObject railsPrefab;

    /** @brief Special node with rails */
    [SerializeField]
    private GameObject railsNodePrefab;

    /** @brief Node next to rails */
    [SerializeField]
    private GameObject nextToRailsPrefab;

    /** @brief Size of a single node (tile) */
    [SerializeField]
    private float nodeSize = 2f;

    /** @brief Extra nodes added on each side to ensure full coverage */
    [SerializeField]
    private int additionalNodeOffset = 2;

    /**
     * @brief Unity Start method.
     *
     * Generates the initial ground row.
     */
    private void Start()
    {
        CreateGroundRow();
    }

    /**
     * @brief Converts viewport coordinates to a world position on the ground plane.
     *
     * @param viewportPos 2D position in viewport space (0–1)
     * @return World position where the ray intersects the ground plane
     */
    private Vector3 PointFromCamera(Vector2 viewportPos)
    {
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(viewportPos.x, viewportPos.y, 0f));

        if (groundPlane.Raycast(ray, out float distance))
        {
            return ray.GetPoint(distance);
        }

        return Vector3.zero;
    }

    /**
     * @brief Creates a new row of ground tiles.
     *
     * The row width is based on camera view. It includes:
     * - center rail node
     * - adjacent special tiles
     * - standard ground tiles
     *
     * Also attaches movement logic via GroundRow component.
     */
    public void CreateGroundRow()
    {
        // Calculate visible area dimensions
        float widthDistance = Vector3.Distance(
            PointFromCamera(new Vector2(0f, 1f)),
            PointFromCamera(new Vector2(1f, 1f))
        );

        float heightDistance = Vector3.Distance(
            PointFromCamera(new Vector2(0f, 0f)),
            PointFromCamera(new Vector2(0f, 1f))
        );

        int nodeCount = (int)(widthDistance / 2f / nodeSize) + additionalNodeOffset;
        float travelDistance = heightDistance + 2f * additionalNodeOffset * nodeSize;

        // Create parent object
        GameObject groundRow = new GameObject("GroundRow");

        // Attach movement script
        GroundRow groundRowScript = groundRow.AddComponent<GroundRow>();
        groundRowScript.SetDependencies(this, 5f, travelDistance);

        // Position row in front of camera
        groundRow.transform.position = transform.position +
            new Vector3(0f, 0f, heightDistance / 2f + additionalNodeOffset * nodeSize);

        // Generate tiles
        for (int i = -nodeCount; i <= nodeCount; i++)
        {
            GameObject nodePrefabToUse;
            Quaternion rotation = Quaternion.identity;

            if (i == 0)
            {
                nodePrefabToUse = railsNodePrefab;
            }
            else if (i == -1 || i == 1)
            {
                nodePrefabToUse = nextToRailsPrefab;

                if (i == 1)
                {
                    rotation = Quaternion.Euler(0f, 180f, 0f);
                }
            }
            else
            {
                nodePrefabToUse = nodePrefab;
            }

            GameObject node = Instantiate(
                nodePrefabToUse,
                groundRow.transform.position + new Vector3(i * nodeSize, 0f, 0f),
                nodePrefabToUse.transform.rotation * rotation
            );

            node.transform.SetParent(groundRow.transform);
        }

        // Add rails object
        GameObject rails = Instantiate(railsPrefab);
        rails.transform.position = groundRow.transform.position + new Vector3(0f, 1.1f, 0f);
        rails.transform.SetParent(groundRow.transform);

        // Parent to generator
        groundRow.transform.SetParent(transform);
    }
}
```
