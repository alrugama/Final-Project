using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimRightAction : AIAction
{
    public override void TakeAction()
    {
        //Debug.Log("In aim right: " + Vector3.right);
        aiMovementData.PointOfInterest = enemyBrain.transform.position + Vector3.right;
        Debug.Log("In aim right transform: " + enemyBrain.transform.position);
        Debug.Log("In aim right poi: " + aiMovementData.PointOfInterest);
        enemyBrain.Aim(aiMovementData.PointOfInterest);
    }
}