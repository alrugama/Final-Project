using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquidBoss : MonoBehaviour
{
    public static bool inCyclone = false;
    public static int cycloneAttempts = 3;
    public static int cycloneRange = 3;
    public static bool atCycloneDest = false;
    public BossMovement bossMovement;
    public BossAnimations bossAnimator;

    public void Start()
    {
        inCyclone = false;
        atCycloneDest = false;
        cycloneAttempts = 3;
        bossMovement = transform.parent.GetComponent<BossMovement>();
        bossAnimator = transform.parent.GetComponentInChildren<BossAnimations>();
    }

    public void Update()
    {
        CheckCyclone();
    }

    public void CheckCyclone()
    {
        bossAnimator.SetCycloneAnimation(!atCycloneDest);
    }

    public void AdjustCycloneSpeed(bool val)
    {
        if(val)
        {
            inCyclone = true;
        }
        else
        {
            inCyclone = false;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, SquidBoss.cycloneRange);
        Gizmos.color = Color.white;
    }
}
