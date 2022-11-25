using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Allow to count collision with a GameObject tagged water
/// WARNING : does not count distinct water collisions
/// </summary>
public class DummyWaterCounter : MonoBehaviour
{
    public string waterTag = "water";

    public int counter = 0;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == waterTag)
        {
            counter += 1;
        }
    }
}
