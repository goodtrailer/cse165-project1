using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoGoHandCollider : MonoBehaviour
{
    private HashSet<Collider> intersections = new HashSet<Collider>();
    public IReadOnlyCollection<Collider> Intersections => intersections;

    private bool intersectionsChanged = false;
    private Collider closestIntersection = null;
    public Collider ClosestIntersection
    {
        get
        {
            if (!intersectionsChanged)
                return closestIntersection;

            intersectionsChanged = false;
            closestIntersection = null;

            float minDistance = float.PositiveInfinity;
            foreach (Collider c in intersections)
            {
                float distance = (c.transform.position - transform.position).magnitude;
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestIntersection = c;
                }
            }

            return closestIntersection;
        }
    }

    void OnTriggerExit(Collider other)
    {
        intersectionsChanged = true;
        intersections.Remove(other);
    }

    void OnTriggerEnter(Collider other)
    {
        intersectionsChanged = true;
        intersections.Add(other);
    }
}
