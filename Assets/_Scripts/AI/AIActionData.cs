using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Responsible for range booleans
public class AIActionData : MonoBehaviour
{
    [field: SerializeField]
    public bool TargetSpotted { get; set; }

    [field: SerializeField]
    public bool Arrived { get; set; }

    [field: SerializeField]
    public bool Attack { get; set; }
}
