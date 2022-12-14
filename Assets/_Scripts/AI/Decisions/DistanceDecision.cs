using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceDecision : AIDecision
{
    [field: SerializeField]
    [field: Range(0.1f, 10)]
    public float Distance { get; set; } = 5;

    public override bool MakeADecision()
    {
        if (Vector3.Distance(aiMovementData.PointOfInterest, transform.position) < Distance)
        {
            aiActionData.TargetSpotted = true;
        }
        else
        {
            aiActionData.TargetSpotted = false;
        }
        return aiActionData.TargetSpotted;
    }

    protected void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, Distance);
        Gizmos.color = Color.white;
    }
}
