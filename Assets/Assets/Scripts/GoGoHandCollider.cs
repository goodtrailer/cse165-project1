using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GoGoHandCollider : MonoBehaviour
{
    private HashSet<Collider> intersections = new HashSet<Collider>();
    public IReadOnlyCollection<Collider> Intersections => intersections;

    public Collider ClosestIntersection { get; private set; } = null;

    void OnTriggerExit(Collider other)
    {
        intersections.Remove(other);
        recomputeClosestIntersection();
    }

    void OnTriggerEnter(Collider other)
    {
        intersections.Add(other);
        recomputeClosestIntersection();
    }

    private void recomputeClosestIntersection()
    {
        ClosestIntersection = null;

        float minDistance = float.PositiveInfinity;
        foreach (Collider c in intersections)
        {
            if (c.IsDestroyed())
            {
                intersections.Remove(c);
                continue;
            }

            float distance = (c.transform.position - transform.position).magnitude;
            if (distance < minDistance)
            {
                minDistance = distance;
                ClosestIntersection = c;
            }
        }
    }
}
