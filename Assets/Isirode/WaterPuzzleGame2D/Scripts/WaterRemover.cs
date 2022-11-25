using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Allow to remove GameObject tagged water on collision
/// </summary>
public class WaterRemover : MonoBehaviour
{
    public string waterTag = "water";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == waterTag)
        {
            Debug.Log("Removing water");
            GameObject.Destroy(collision.gameObject);
        }
    }
}
