using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{
   private void OnTriggerEnter2D(Collider2D collision)
   {
      if (collision.tag == "Player")
      {
          
           Player player = FindObjectOfType<Player>();
           player?.Heal();
           popup popup = FindObjectOfType<popup>();
            popup.SetText("health");
            popup.ShowText();
             Destroy(gameObject);
           
      }
   }
}
