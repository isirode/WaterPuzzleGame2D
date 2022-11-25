using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterEnterObjective : MonoBehaviour
{
    public string waterTag = "water";

    public int counter = 0;

    public int collisionObjective = 10;
    private bool objectiveWasReached = false;

    public delegate void ObjectiveReachedDelegate(GameObject gameObject);
    public event ObjectiveReachedDelegate ObjectiveReached;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == waterTag)
        {
            counter += 1;
        }
        if (counter >= collisionObjective && objectiveWasReached == false)
        {
            Debug.Log("Objective reached");
            objectiveWasReached = true;
            ObjectiveReached?.Invoke(gameObject);
        }
    }

    public void ResetState()
    {
        counter = 0;
        objectiveWasReached = false;
    }
}
