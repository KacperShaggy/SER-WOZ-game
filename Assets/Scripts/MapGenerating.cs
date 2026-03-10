using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class MapGenerating : MonoBehaviour
{
    public GameObject NodePrefab;
    public GameObject RailsPrefab;
    public GameObject RailsNodePrefab;
    public GameObject NextToRailsPrefab;

    private float nodeSize = 2f;
    private int additionNodeOffset = 2; // Dodatkowe wêz³y po ka¿dej stronie, aby zapewniæ pe³ne pokrycie

    // Start is called before the first frame update
    void Start()
    {
        CreateGroundRow();
    }

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
        return Vector3.zero; // Zwraca zero, jeœli promieñ nie przecina p³aszczyzny
    }

    public void CreateGroundRow()
    {
        float WidthDistance = Vector3.Distance(PointFromCamera(new Vector2(0, 1)), PointFromCamera(new Vector2(1, 1)));
        float HeightDistance = Vector3.Distance(PointFromCamera(new Vector2(0, 0)), PointFromCamera(new Vector2(0, 1)));

        int nodeNumber = (int)(WidthDistance/2/nodeSize) + additionNodeOffset;
        float nodeTravelDistnace = HeightDistance + 2 * additionNodeOffset * nodeSize;

        GameObject groundRow = new GameObject("GroundRow");

        GroundRow groundRowScript = groundRow.AddComponent<GroundRow>();
        groundRowScript.speed = 5f;
        groundRowScript.mapGenerating = this;
        groundRowScript.distanceToTravel = nodeTravelDistnace;

        groundRow.transform.position = transform.position + new Vector3(0,0, HeightDistance/2 + additionNodeOffset * nodeSize);


        for (int i = -nodeNumber; i <= nodeNumber; i++)
        {
            GameObject node;
            if (i==0)
            {
                node = Instantiate(RailsNodePrefab, groundRow.transform.position + new Vector3(i * nodeSize, 0, 0), RailsNodePrefab.transform.rotation);
            }
            else if(i == -1 || i == 1)
            {
                float rotation = 0;
                if (i == 1)
                    rotation = 180;

                node = Instantiate(NextToRailsPrefab, groundRow.transform.position + new Vector3(i * nodeSize, 0, 0), NextToRailsPrefab.transform.rotation * Quaternion.Euler(0f, 0f, rotation));
            }
            else
            {
                node = Instantiate(NodePrefab, groundRow.transform.position + new Vector3(i * nodeSize, 0, 0), NodePrefab.transform.rotation);
            }
            node.transform.parent = groundRow.transform;

        }

        GameObject rails = Instantiate(RailsPrefab);
        rails.transform.position = groundRow.transform.position + new Vector3(0, 1.1f, 0);
        rails.transform.parent = groundRow.transform;

        groundRow.transform.parent = transform;
    }
}
