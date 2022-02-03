using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class handles every tile in our map
// We can expand it to include special tiles ie environmental hazards
public class TileNode : MonoBehaviour
{
    // 0 == empty space / out of map
    // 1 = room
    // 2 = corridor
    public int value;
    public int x;
    public int y;
    // public bool isDoor;


    // Start is called before the first frame update
    void Start()
    {
        value = 0;
    }
}