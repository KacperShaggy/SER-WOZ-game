using UnityEngine;

/// <summary>
/// Simple camera movement script that applies a horizontal sine wave motion.
/// </summary>
public class CameraMovement : MonoBehaviour
{
    /// <summary>Amplitude of camera oscillation (how far it moves).</summary>
    public float amplitude = 0.5f;

    /// <summary>Frequency of camera oscillation (how fast it moves).</summary>
    public float frequency = 1f;

    /// <summary>Initial camera position reference.</summary>
    private Vector3 startPos;

    /// <summary>
    /// Stores the starting position of the camera.
    /// </summary>
    void Start()
    {
        startPos = transform.position;
    }

    /// <summary>
    /// Applies horizontal sine-wave movement each frame.
    /// </summary>
    void Update()
    {
        float offsetX = Mathf.Sin(Time.time * frequency) * amplitude;

        transform.position = new Vector3(
            startPos.x + offsetX,
            startPos.y,
            startPos.z
        );
    }
}