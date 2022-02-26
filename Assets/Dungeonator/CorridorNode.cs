using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorridorNode
{
    public List<TileNode> tileList = new List<TileNode>();
    public List<RoomNode> roomList = new List<RoomNode>();


    public bool IsBorderingRoom()
    {
        foreach(TileNode tile in tileList)
        {
            for(int x = -1; x <= 1; x++)
            {
                for(int y = -1; y <= 1; y++)
                {   
                    TileNode check = MapGenerator.map[tile.x + x, tile.y + y];
                    if(check.value == 1 && check.room != roomList[0] && check.room != roomList[1])
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public void NullifyCorridor()
    {
        foreach(TileNode tile in tileList)
        {
            MapGenerator.map[tile.x, tile.y].value = 0;
        }
    }
}
