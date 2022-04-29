using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatchetBossDecisionAction : AIAction
{
  public override void TakeAction()
  {
      //SquidBoss.inDecision = true;
      //RatchetBoss.inDecision = true;
      int r = Random.Range(0, 50);
      Debug.Log("In Ratchet Boss decision action");
      Debug.Log("Random number = " + r);
      if(r < 50)
      {
        RatchetBoss.inCharge = true;
        Debug.Log("In Charge = " + RatchetBoss.inCharge);
      }
      else
      {
        RatchetBoss.inSlam = true;
        Debug.Log("In Slam = " + RatchetBoss.inSlam);
      }
      
  }
}
