using UnityEngine;

public class GroundRow : MonoBehaviour
{
    private float nodeSize = 2f;
    //wolalbym zeby to nie bylo public
    public float speed = 5f;

    public MapGenerating mapGenerating;

    private Vector3 startPosition;
    private float distanceTravelled = 0f;
    public float distanceToTravel;

    private bool createdComplete = false; 
    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.back  * speed * Time.deltaTime);
        distanceTravelled = Vector3.Distance(startPosition, transform.position);

        if (!createdComplete && distanceTravelled >= nodeSize)
        {
            createdComplete = true;
            //utworz nowy node
            mapGenerating.CreateGroundRow();
        }

        if(distanceTravelled >= distanceToTravel)
        {
            Destroy(gameObject);
        }
    }
}
