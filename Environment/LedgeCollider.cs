using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LedgeCollider : MonoBehaviour
{
    public List<GameObject> CollidedObjects = new List<GameObject>();

    private void OnTriggerEnter(Collider other)
    {
        if (!CollidedObjects.Contains(other.gameObject))
        {
            CollidedObjects.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (CollidedObjects.Contains(other.gameObject))
        {
            CollidedObjects.Remove(other.gameObject);
        }
    }
}
