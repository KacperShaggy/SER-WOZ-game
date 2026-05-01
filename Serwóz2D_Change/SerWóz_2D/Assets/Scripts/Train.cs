using UnityEngine;
using System.Collections.Generic;
using TMPro;

/// <summary>
/// Represents a single carriage attached to the train.
/// </summary>
[System.Serializable]
class Carriage
{
    /// <summary>Internal counter used for numbering (note: resets per instance).</summary>
    int numberCounter = 0;

    /// <summary>Current speed of the carriage (used when detached).</summary>
    public float speed = 0f;

    /// <summary>Identifier number of the carriage.</summary>
    public int number;

    /// <summary>Indicates whether the carriage is still attached to the train.</summary>
    public bool joined;

    /// <summary>Reference to the carriage GameObject.</summary>
    public GameObject carriageObject;

    /// <summary>
    /// Initializes a new carriage instance.
    /// </summary>
    /// <param name="_carriage">GameObject representing the carriage.</param>
    public Carriage(GameObject _carriage)
    {
        this.number = numberCounter;
        numberCounter++;

        this.joined = true;
        this.carriageObject = _carriage;
    }
}

/// <summary>
/// Controls the train behavior, including health, carriages, and coin management.
/// </summary>
public class Train : MonoBehaviour
{
    /// <summary>Initial health value of the train.</summary>
    [SerializeField] private float startHealth = 100f;

    /// <summary>Current health of the train.</summary>
    [SerializeField] private float health;

    /// <summary>List of all train carriages.</summary>
    [SerializeField] private List<Carriage> carriages = new List<Carriage>();

    /// <summary>UI text displaying the current coin count.</summary>
    [SerializeField] public TMP_Text coinNumberText;

    /// <summary>Current number of coins owned by the player.</summary>
    public int coinNumber = 0;

    /// <summary>Reference to map generator (used for defeat condition).</summary>
    public MapGenerator mapGenerator;

    /// <summary>
    /// Initializes the train and collects all initial carriages.
    /// </summary>
    void Start()
    {
        foreach (Transform child in transform)
        {
            Debug.Log(child.name);

            if (child.CompareTag("Carriage"))
            {
                carriages.Add(new Carriage(child.gameObject));
            }
        }

        health = startHealth;
    }

    /// <summary>
    /// Returns the number of currently tracked carriages.
    /// </summary>
    /// <returns>Number of carriages.</returns>
    public int GetCarriageNumber()
    {
        return carriages.Count;
    }

    /// <summary>
    /// Applies damage to the train.
    /// </summary>
    /// <param name="damage">Amount of damage to apply.</param>
    public void TakeDamage(float damage)
    {
        health -= damage;
    }

    /// <summary>
    /// Gets the position of the last attached carriage.
    /// </summary>
    /// <returns>World position of the last joined carriage or train position if none.</returns>
    public Vector3 getLastCarriagePosition()
    {
        Vector3 lastCarriagePosition = transform.position;

        foreach (Carriage carriage in carriages)
        {
            if (carriage.joined)
            {
                lastCarriagePosition = carriage.carriageObject.transform.position;
            }
        }

        return lastCarriagePosition;
    }

    /// <summary>
    /// Adds a new carriage to the train.
    /// </summary>
    /// <param name="newGameObject">GameObject representing the new carriage.</param>
    public void AddCarrigae(GameObject newGameObject)
    {
        carriages.Add(new Carriage(newGameObject));
    }

    /// <summary>
    /// Main update loop handling damage consequences and carriage movement.
    /// </summary>
    void Update()
    {
        /// <summary>Handle carriage detachment when health reaches zero.</summary>
        if (health <= 0)
        {
            health = startHealth;

            Carriage lastCarriage = null;

            /// <summary>Find any joined carriage as starting point.</summary>
            foreach (Carriage carriage in carriages)
            {
                if (carriage.joined)
                {
                    lastCarriage = carriage;
                    break;
                }
            }

            /// <summary>Find the furthest (last) joined carriage.</summary>
            if (lastCarriage != null)
            {
                foreach (Carriage carriage in carriages)
                {
                    if (carriage.joined)
                    {
                        if (carriage.carriageObject.transform.position.x < lastCarriage.carriageObject.transform.position.x)
                        {
                            lastCarriage = carriage;
                        }
                    }
                }

                /// <summary>Detach the last carriage.</summary>
                lastCarriage.joined = false;
            }

            /// <summary>Check if any carriage remains attached.</summary>
            Carriage lastCarriage2 = null;

            foreach (Carriage carriage in carriages)
            {
                if (carriage.joined)
                {
                    lastCarriage2 = carriage;
                    break;
                }
            }

            /// <summary>If no carriages remain, trigger defeat.</summary>
            if (lastCarriage2 == null)
            {
                mapGenerator.Defeat();
            }
        }

        /// <summary>Move detached carriages away and accelerate them.</summary>
        foreach (Carriage carriage in carriages)
        {
            if (!carriage.joined)
            {
                carriage.carriageObject.transform.position += Vector3.left * Time.deltaTime * carriage.speed;
                carriage.speed += 0.02f;
            }
        }
    }
}