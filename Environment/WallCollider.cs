using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallCollider : MonoBehaviour
{
    public List<GameObject> CollidedObjects = new List<GameObject>();

    private void OnTriggerEnter(Collider other)
    {
        if (!CollidedObjects.Contains(other.gameObject) && other.CompareTag("Wall"))
        {
            CollidedObjects.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (CollidedObjects.Contains(other.gameObject) && other.CompareTag("Wall"))
        {
            CollidedObjects.Remove(other.gameObject);
        }
    }
}
