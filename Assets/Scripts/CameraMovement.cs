using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float distance = 1f;      // Odleg³oœæ ruchu (1 metr)
    public float moveSpeed = 1f;     // Prêdkoœæ ruchu
    public float waitTime = 0f;    // Pauza miêdzy ruchami

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
        StartCoroutine(MoveCamera());
    }

    IEnumerator MoveCamera()
    {
        while (true)
        {
            // Ruch do przodu
            yield return MoveToPosition(startPosition + new Vector3(0,0, distance));

            yield return new WaitForSeconds(waitTime);

            // Ruch do ty³u
            yield return MoveToPosition(startPosition);

            yield return new WaitForSeconds(waitTime);
        }
    }

    IEnumerator MoveToPosition(Vector3 target)
    {
        while (Vector3.Distance(transform.position, target) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                target,
                moveSpeed * Time.deltaTime
            );

            yield return null;
        }

        transform.position = target;
    }
}
