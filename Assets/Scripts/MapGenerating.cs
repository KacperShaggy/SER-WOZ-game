using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

/**
 * @class MapGenerating
 * @brief Responsible for procedural generation of ground rows and map elements.
 *
 * This class dynamically creates rows of ground nodes based on the camera's
 * visible area. It ensures that the map continuously extends as rows move
 * backward, creating an infinite scrolling environment.
 */
public class MapGenerating : MonoBehaviour
{
    /// <summary>
    /// Prefab used for standard ground nodes.
    /// </summary>
    public GameObject NodePrefab;

    /// <summary>
    /// Prefab containing the rail model.
    /// </summary>
    public GameObject RailsPrefab;

    /// <summary>
    /// Prefab for the ground node located directly under the rails.
    /// </summary>
    public GameObject RailsNodePrefab;

    /// <summary>
    /// Prefab for nodes located next to the rails.
    /// </summary>
    public GameObject NextToRailsPrefab;

    /// <summary>
    /// Size of a single ground node.
    /// </summary>
    private float nodeSize = 2f;

    /// <summary>
    /// Additional nodes added on each side of the map to ensure full coverage.
    /// </summary>
    private int additionNodeOffset = 2; // Dodatkowe wêz³y po ka¿dej stronie, aby zapewniæ pe³ne pokrycie

    /**
     * @brief Called when the map generator starts.
     *
     * Creates the initial ground row.
     */
    // Start is called before the first frame update
    void Start()
    {
        CreateGroundRow();
    }

    /**
     * @brief Converts a camera viewport position into a point on the ground plane.
     *
     * Casts a ray from the camera through a viewport coordinate and finds
     * the intersection with the ground plane (Y = 0).
     *
     * @param camera2DPos 2D viewport position (0–1 range).
     * @return Vector3 Position on the ground plane corresponding to the viewport point.
     */
    private Vector3 PointFromCamera(Vector2 camera2DPos)
    {
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero); // Y = 0, to mog³o by byæ poza scopem
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(camera2DPos.x, camera2DPos.y, 0f)); // lewy górny róg

        float distance;

        if (groundPlane.Raycast(ray, out distance))
        {
            Vector3 hitPoint = ray.GetPoint(distance);
            return hitPoint;
        }

        /// <summary>
        /// Returns zero if the ray does not intersect the ground plane.
        /// </summary>
        return Vector3.zero;
    }

    /**
     * @brief Creates a new row of ground nodes.
     *
     * Calculates the visible area of the camera and generates enough
     * ground nodes to cover the width. Also places rails and special
     * nodes in the center area.
     */
    public void CreateGroundRow()
    {
        float WidthDistance = Vector3.Distance(PointFromCamera(new Vector2(0, 1)), PointFromCamera(new Vector2(1, 1)));
        float HeightDistance = Vector3.Distance(PointFromCamera(new Vector2(0, 0)), PointFromCamera(new Vector2(0, 1)));

        int nodeNumber = (int)(WidthDistance / 2 / nodeSize) + additionNodeOffset;
        float nodeTravelDistnace = HeightDistance + 2 * additionNodeOffset * nodeSize;

        /// <summary>
        /// Creates a parent object for the generated ground row.
        /// </summary>
        GameObject groundRow = new GameObject("GroundRow");

        GroundRow groundRowScript = groundRow.AddComponent<GroundRow>();
        groundRowScript.speed = 5f;
        groundRowScript.mapGenerating = this;
        groundRowScript.distanceToTravel = nodeTravelDistnace;

        groundRow.transform.position = transform.position + new Vector3(0, 0, HeightDistance / 2 + additionNodeOffset * nodeSize);

        /**
         * @brief Generates ground nodes across the width of the map.
         *
         * Special prefabs are used for nodes under the rails and next to them.
         */
        for (int i = -nodeNumber; i <= nodeNumber; i++)
        {
            GameObject node;

            if (i == 0)
            {
                /// <summary>
                /// Center node containing the rail base.
                /// </summary>
                node = Instantiate(RailsNodePrefab, groundRow.transform.position + new Vector3(i * nodeSize, 0, 0), RailsNodePrefab.transform.rotation);
            }
            else if (i == -1 || i == 1)
            {
                /// <summary>
                /// Nodes located next to the rails.
                /// </summary>
                float rotation = 0;

                if (i == 1)
                    rotation = 180;

                node = Instantiate(NextToRailsPrefab, groundRow.transform.position + new Vector3(i * nodeSize, 0, 0), NextToRailsPrefab.transform.rotation * Quaternion.Euler(0f, 0f, rotation));
            }
            else
            {
                /// <summary>
                /// Standard ground node.
                /// </summary>
                node = Instantiate(NodePrefab, groundRow.transform.position + new Vector3(i * nodeSize, 0, 0), NodePrefab.transform.rotation);
            }

            node.transform.parent = groundRow.transform;
        }

        /// <summary>
        /// Instantiates the rail model above the center node.
        /// </summary>
        GameObject rails = Instantiate(RailsPrefab);
        rails.transform.position = groundRow.transform.position + new Vector3(0, 1.1f, 0);
        rails.transform.parent = groundRow.transform;

        /// <summary>
        /// Sets the generated row as a child of the map generator.
        /// </summary>
        groundRow.transform.parent = transform;
    }
}