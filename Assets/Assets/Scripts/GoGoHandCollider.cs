using System.Collections;
using System.Collections.Generic;
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
        if (ClosestIntersection?.GetComponent<Outline>() is Outline oldOutline)
            oldOutline.enabled = false;

        ClosestIntersection = null;

        float minDistance = float.PositiveInfinity;
        foreach (Collider c in intersections)
        {
            float distance = (c.transform.position - transform.position).magnitude;
            if (distance < minDistance)
            {
                minDistance = distance;
                ClosestIntersection = c;
            }
        }

        if (ClosestIntersection?.GetComponent<Outline>() is Outline outline)
            outline.enabled = true;
    }
}
