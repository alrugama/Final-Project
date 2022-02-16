using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MapGenerator : MonoBehaviour
{
    [SerializeField]
    private TileSpritePlacer AutoTiler;
    [SerializeField]
    private GameObject LightPrefab;
    [SerializeField]
    private GameObject ShopKeeper;
    [SerializeField]
    private GameObject Spawner;
    [SerializeField]
    private GameObject BossSpawner;
    [SerializeField]
    private GameObject EntryCollider;
    public int columns;
    public int rows;
    TileNode[,] map;

    // Min dimensions of rooms
    public Vector2Int minRoomDim;
    public Vector2Int minEntryDim;
    public Vector2Int maxEntryDim;

    public Vector2Int minNormalDim;
    public Vector2Int maxNormalDim;

    public Vector2Int minLargeDim;
    public Vector2Int maxLargeDim;

    public Vector2Int minBossDim;
    public Vector2Int maxBossDim;

    public bool UseCellular;
    public string Seed;
    public bool useRandomSeed;
    [Range(0, 100)]
    public int wallPercent;
    public int wallCount;
    
    public int Iterations;

    public bool HasEntry = false;
    public bool HasBoss = false;
    public int numLargeRooms;
    public int BossRoom = 1;
    public int normalRooms;
    public int overgrownRooms;

    // Queues for the BSP algorithm
    private List<int[]> queue = new List<int[]>();
    private Queue<int[]> roomsList = new Queue<int[]>();
    private List<TileNode> roomTiles = new List<TileNode>();
    private List<RoomNode> Rooms = new List<RoomNode>();
    private List<CorridorNode> Corridors = new List<CorridorNode>();

    //private AstarPath AStar;


    public TileNode[,] GenerateMap()
    {
        map = new TileNode[columns, rows];

        FillMap();
        BinarySpace();

        DrawMap();
        // AStar = GameObject.FindGameObjectWithTag("AStar").GetComponent<AstarPath>();
        AstarPath.active.Scan();

        OnDrawGizmos();

        return map;
    }
    void DrawMap(){
        AutoTiler.Clear();
        AutoTiler.PaintFloorTiles(roomTiles);
        AutoTiler.PaintCollisions(roomTiles, map);
    }

    void FillMap(){
        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                map[x, y] = new TileNode();
                map[x, y].x = x;
                map[x, y].y = y;
            }
        }
    }

    void BinarySpace()
    {
        // Instantiate first room space (whole map)
        var temp = new int[4];
        temp[0] = 0;
        temp[1] = 0;
        temp[2] = columns - 1;
        temp[3] = rows - 1;
        // This will be our queue of room spaces
        queue.Add(temp);

        int count = 0;
        // Main loop that splits map
        // Debug.Log("Before Binary Space splitting loop");
        while(queue.Count > 0) 
        {
            // print("Times looped: " + count);
            if (count > 200){
                Debug.Log("Too many rooms created");
                break;
            }
            // Debug.Log("In Binary Space splitting loop");
            // print("COUNT: " + queue.Count);
            // print("Times looped: " + count);
            int[] space;
            if (HasEntry)
            {
                // note: This is not optimal, very slow
                space = queue[0];
                queue.RemoveAt(0);
            }
            else
            {
                space = queue[queue.Count - 1];
                queue.RemoveAt(queue.Count - 1);
            }
            var x1 = space[0];
            var y1 = space[1];
            var x2 = space[2];
            var y2 = space[3];

            // print("x1, y1, x2, y2: " + x1 + ", " + y1 + ", " + x2 + ", " + y2);

            var width = x2 - x1;
            var height = y2 - y1;

            // This gives us larger rooms
            if (UnityEngine.Random.Range(0, 100) < 20)
            {
                if (width > minNormalDim.x && height > minNormalDim.y)
                {
                    // Randomly choose which split we prefer
                    if (UnityEngine.Random.Range(0, 100) < 50)
                    {
                        if (height >= minNormalDim.y * 2)
                        {
                            SplitHorizontal(space);
                        }
                        else if (width >= minNormalDim.x * 2)
                        {
                            SplitVertical(space);
                        }
                        else
                        {
                            roomsList.Enqueue(space);
                        }
                    }
                    else
                    {
                        if (width >= minNormalDim.x * 2)
                        {
                            SplitVertical(space);
                        }
                        else if (height >= minNormalDim.y * 2)
                        {
                            SplitHorizontal(space);
                        }
                        else
                        {
                            roomsList.Enqueue(space);
                        }
                    }
                }
            }
            // This gives us smaller rooms
            else
            {
                if (width > minRoomDim.x && height > minRoomDim.y)
                {
                    // Randomly choose which split we prefer
                    if (UnityEngine.Random.Range(0, 100) < 50)
                    {
                        if (height >= minRoomDim.y * 2)
                        {
                            SplitHorizontal(space);
                        }
                        else if (width >= minRoomDim.x * 2)
                        {
                            SplitVertical(space);
                        }
                        else
                        {
                            roomsList.Enqueue(space);
                        }
                    }
                    else
                    {
                        if (width >= minRoomDim.x * 2)
                        {
                            SplitVertical(space);
                        }
                        else if (height >= minRoomDim.y * 2)
                        {
                            SplitHorizontal(space);
                        }
                        else
                        {
                            roomsList.Enqueue(space);
                        }
                    }
                }
            }
            count++;
        }

        // This is where we create the rooms
        int tempCount = 0;
        while (roomsList.Count > 0)
        {
            int[] room = roomsList.Dequeue();

            int x1 = (int)room[0];
            int y1 = (int)room[1];
            int x2 = (int)room[2];
            int y2 = (int)room[3];

            GameObject tempRoom = new GameObject(tempCount.ToString());
            tempRoom.AddComponent<RoomNode>();

            RoomNode NewRoom = tempRoom.GetComponent<RoomNode>();
            if (!HasEntry)
            {
                NewRoom.RoomType = "Start";
                NewRoom.MaxNeighbors = 1;
                HasEntry = true;
            }
            else if (!HasBoss && roomsList.Count == 0)
            {
                NewRoom.RoomType = "Boss";
                NewRoom.MaxNeighbors = 2;
                HasBoss = true;
            }
            else
            {
                NewRoom.RoomType = "Normal";
                NewRoom.MaxNeighbors = 3;
            }

            // Here we fill the negative space with empty space 
            // I.e. room creation
            for (int i = x1 + 1; i < x2 - 1; i++)
            {
                for (int j = y1 + 1; j < y2 - 1; j++)
                {
                    map[i, j].value = 1;
                    map[i,j].room = NewRoom;
                    roomTiles.Add(map[i, j]);
                    NewRoom.tileList.Add(map[i, j]);
                }
            }
            AddLights(x1, y1, x2, y2, NewRoom);
            NewRoom.CalculateCenter();
            if(NewRoom.RoomType == "Start")
            {
                SpawnPlayer(NewRoom);
            }

            Rooms.Add(NewRoom);
            tempCount++;
        }
        SortRooms();
        AddCorridors();
        AddEntryColliders();
        AddSpawners();
    }

    void AddSpawners()
    {
        foreach(RoomNode room in Rooms)
        {
            Vector3 pos1 = new Vector3(room.roomCenter.x + 3, room.roomCenter.y + 3, 0);
            if (room.RoomType == "Start")
            {
                // Instantiate(ShopKeeper, pos1, Quaternion.identity);
                continue;
            }
            pos1 = new Vector3(room.roomCenter.x, room.roomCenter.y, 0);
            GameObject spawner;
            if(room.RoomType == "Boss")
            {
                spawner = Instantiate(BossSpawner, pos1, Quaternion.identity);
            }
            else
            {
                spawner = Instantiate(Spawner, pos1, Quaternion.identity);
            }
            spawner.transform.parent = room.transform;

            room.gameObject.AddComponent<RoomClearCheck>();
        }
    }

    void AddEntryColliders()
    {
        foreach(CorridorNode corridor in Corridors)
        {
            if (corridor != null && corridor.tileList.Count > 0)
            {
                roomTiles.AddRange(corridor.tileList);
                Debug.Log("Tilelist count: " + corridor.tileList.Count);
                // GRab 1st element from tilelist and roomlist
                TileNode tile1 = corridor.tileList[0];
                RoomNode room1 = corridor.roomList[0];
                
                if(room1.RoomType != "Start")
                {
                    // instantiate entry collider at 1st tile coupled with 1st room
                    Vector3 pos1 = new Vector3(tile1.x, tile1.y, 0);
                    GameObject entryCollider1 = Instantiate(EntryCollider, pos1, Quaternion.identity);
                    entryCollider1.transform.parent = room1.transform;
                }

                //grab 2nd element
                TileNode tile2 = corridor.tileList.Last();
                RoomNode room2 = corridor.roomList.Last();

                // instantiate entry collider at 2nd tile coupled with 2nd room
                Vector3 pos2 = new Vector3(tile2.x, tile2.y, 0);
                GameObject entryCollider2 = Instantiate(EntryCollider, pos2, Quaternion.identity);
                entryCollider2.transform.parent = room2.transform;
            }
            
        }
    }

    void SpawnPlayer(RoomNode SpawnRoom)
    {
        var Player = GameObject.FindGameObjectWithTag("Player");
        Vector3 spawnPosition = new Vector3(SpawnRoom.roomCenter.x, SpawnRoom.roomCenter.y, 0);
        Player.transform.position = spawnPosition;
    }

    RoomNode CreateRoom(int x1, int y1, int x2, int y2)
    {
        RoomNode New = new RoomNode("Normal");

        int area = (x2 - x1) * (y2 - y1);
        // if (area)

        return New;
    }

    // This function sorts all Rooms according to their distance from eachother
    // This helps with optimizing corridor creation
    void SortRooms()
    {
        // Go through every room
        foreach(RoomNode room in Rooms)
        {
            // Go through all other rooms
            foreach(RoomNode neighbor in Rooms)
            {
                // Not including itself
                if (neighbor == room)
                {
                    continue;
                }

                SortByDistance(room, neighbor);
            }
        }
    }

    // This function goes through current list of rooms and inserts room in question accordingly
    void SortByDistance(RoomNode room, RoomNode neighbor)
    {
        for(int i = 0; i < room.RoomsByDistance.Count; i++)
        {
            var check = room.RoomsByDistance[i];
            var distance1 = Vector2.Distance(room.roomCenter, check.roomCenter);
            var distance2 = Vector2.Distance(room.roomCenter, neighbor.roomCenter);
            if (distance2 < distance1)
            {
                room.RoomsByDistance.Insert(i, neighbor);
                return;
            }
        }
        // If neighbor is furthest away out of all compared so far add it to back of list
        room.RoomsByDistance.Add(neighbor);
    }

    void AddCorridors()
    {
        /* List<Vector2Int> roomCenters = new List<Vector2Int>();
        foreach (RoomNode Room in Rooms)
        {
            roomCenters.Add(Room.roomCenter);
        }

        var currentCenter = roomCenters[Random.Range(0, roomCenters.Count)];
        roomCenters.Remove(currentCenter);

        int i = 0;
        while(roomCenters.Count > 0)
        {
            if (i > 100)
            {
                Debug.Log("While loop timed out");
                return; 
            }
            Vector2Int closest = FindClosestPoint(currentCenter, roomCenters);
            roomCenters.Remove(closest);
            CreateCorridor(currentCenter, closest);

            currentCenter = closest;
            i++;
        }*/

        foreach (RoomNode room in Rooms)
        {
            foreach (RoomNode neighbor in room.RoomsByDistance)
            {
                ConnectRooms(room, neighbor);
            }
        }
    }

    void ConnectRooms(RoomNode room, RoomNode neighbor)
    {
        if (room.NeighborCount >= room.MaxNeighbors)
        {
            return;
        }
        if (neighbor.NeighborCount >= neighbor.MaxNeighbors)
        {
            return;
        }
        CorridorNode corridor = CreateCorridor(room, neighbor);
        if (corridor == null)
        {
            return;
        }
        Corridors.Add(corridor);

    }

    private CorridorNode CreateCorridor(RoomNode room, RoomNode neighbor)
    {
        Vector2Int currentRoomCenter = room.roomCenter;
        Vector2Int destination = neighbor.roomCenter;
        var position = currentRoomCenter;
        CorridorNode corridor = new CorridorNode();
        // Here we randomly choose a directional preference
        if (Random.Range(0, 100) < 50)
        {
            while (position.y != destination.y)
            {
                if (destination.y > position.y)
                {
                    position += Vector2Int.up;
                }
                else
                {
                    position += Vector2Int.down;
                }
                if (map[position.x, position.y].value == 0)
                {
                    map[position.x, position.y].value = 2;
                    corridor.tileList.Add(map[position.x, position.y]);
                }
                else if (map[position.x, position.y].value == 1)
                {
                    if (map[position.x, position.y].room != room && map[position.x, position.y].room != neighbor)
                    {
                        corridor = null;
                        return null;
                    }
                }
            }
            while (position.x != destination.x)
            {
                if (destination.x > position.x)
                {
                    position += Vector2Int.right;
                }
                else
                {
                    position += Vector2Int.left;
                }
                if (map[position.x, position.y].value == 0)
                {
                    map[position.x, position.y].value = 2;
                    corridor.tileList.Add(map[position.x, position.y]);
                }
                else if (map[position.x, position.y].value == 1)
                {
                    if (map[position.x, position.y].room != room && map[position.x, position.y].room != neighbor)
                    {
                        corridor = null;
                        return null;
                    }
                }
            }
        }
        else
        {
            while (position.x != destination.x)
            {
                if (destination.x > position.x)
                {
                    position += Vector2Int.right;
                }
                else
                {
                    position += Vector2Int.left;
                }
                if (map[position.x, position.y].value == 0)
                {
                    map[position.x, position.y].value = 2;
                    corridor.tileList.Add(map[position.x, position.y]);
                }
                else if (map[position.x, position.y].value == 1)
                {
                    if (map[position.x, position.y].room != room && map[position.x, position.y].room != neighbor)
                    {
                        corridor = null;
                        return null;
                    }
                }
            }
            while (position.y != destination.y)
            {
                if (destination.y > position.y)
                {
                    position += Vector2Int.up;
                }
                else
                {
                    position += Vector2Int.down;
                }
                if (map[position.x, position.y].value == 0)
                {
                    map[position.x, position.y].value = 2;
                    corridor.tileList.Add(map[position.x, position.y]);
                }
                else if (map[position.x, position.y].value == 1)
                {
                    if (map[position.x, position.y].room != room && map[position.x, position.y].room != neighbor)
                    {
                        corridor = null;
                        return null;
                    }
                }
            }
        }
        room.NeighborRooms.Add(neighbor);
        neighbor.NeighborRooms.Add(room);

        corridor.roomList.Add(room);
        corridor.roomList.Add(neighbor);

        return corridor;
    }

    private Vector2Int FindClosestPoint(Vector2Int currentCenter, List<Vector2Int> roomCenters)
    {
        Vector2Int closest = Vector2Int.zero;
        float distance = float.MaxValue;
        foreach (var position in roomCenters)
        {
            float currentDistance = Vector2.Distance(position, currentCenter);
            if (currentDistance < distance)
            {
                distance = currentDistance;
                closest = position;
            }
        }
        return closest;
    }

    void AddLights(int x1, int y1, int x2, int y2, RoomNode room)
    {
        var lightPrefab1 = Instantiate(LightPrefab, new Vector2(x1 + 4, y1 + 4), Quaternion.identity);
        var lightPrefab2 = Instantiate(LightPrefab, new Vector2(x1 + 4, y2 - 4), Quaternion.identity);
        var lightPrefab3 = Instantiate(LightPrefab, new Vector2(x2 - 4, y1 + 4), Quaternion.identity);
        var lightPrefab4 = Instantiate(LightPrefab, new Vector2(x2 - 4, y2 - 4), Quaternion.identity);

        lightPrefab1.transform.parent = room.gameObject.transform;
        lightPrefab2.transform.parent = room.gameObject.transform;
        lightPrefab3.transform.parent = room.gameObject.transform;
        lightPrefab4.transform.parent = room.gameObject.transform;
    }

    // Splits map horizontally
    void SplitHorizontal(int[] roomSpace)
    {
        var x1 = roomSpace[0];
        var y1 = roomSpace[1];
        var x2 = roomSpace[2];
        var y2 = roomSpace[3];

        var ySplit = UnityEngine.Random.Range(y1 + 1, y2);
        var room1 = new int[] { x1, y1, x2, ySplit };
        var room2 = new int[] { x1, ySplit + 1, x2, y2 };

        if (UnityEngine.Random.Range(0, 100) < 50)
        {
            queue.Add(room1);
            queue.Add(room2);
        }
        else
        {
            queue.Add(room2);
            queue.Add(room1);
        }
    }

    // Splits map vertically
    void SplitVertical(int[] roomSpace)
    {
        var x1 = roomSpace[0];
        var y1 = roomSpace[1];
        var x2 = roomSpace[2];
        var y2 = roomSpace[3];
        var xSplit = UnityEngine.Random.Range(x1 + 1, x2);
        var room1 = new int[] { x1, y1, xSplit, y2 };
        var room2 = new int[] { xSplit + 1, y1, x2, y2 };

        // print("In vertical.");
        if (UnityEngine.Random.Range(0, 100) < 50)
        {
            queue.Add(room1);
            queue.Add(room2);
        }
        else
        {
            queue.Add(room2);
            queue.Add(room1);
        }

    }

    void OnDrawGizmos()
    {
        /* if (enemies != null)
        {
            foreach (Vector3 enemy in enemies)
            {
                // print("Enemy X: " + enemy.x);
                // print("Enemy Y: " + enemy.y);
                // print("Enemy Z: " + enemy.z);
                Gizmos.DrawSphere(enemy, displayRadius);
            }
        }*/
        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                // Debug.Log("(x, y) = (" + x + ", " + y+ ")");
                if (map[x, y].value == 0)
                {
                    Gizmos.color = new Color(0, 0, 0, 1f);
                }
                else if (map[x, y].value == 1)
                {
                    if (map[x, y].room.RoomType == "Start")
                    {
                        Gizmos.color = new Color(0, 255, 0, 1f);
                    }
                    else if (map[x, y].room.RoomType == "Boss")
                    {
                        Gizmos.color = new Color(255, 0, 0, 1f);
                    }
                    else
                    {
                        Gizmos.color = new Color(255, 255, 0, 1f);
                    }
                    
                }
                else
                {
                    Gizmos.color = new Color(0, 255, 255, 1f);
                }

                Gizmos.DrawCube(new Vector3(x, y, 0), new Vector3(1, 1, 1));
            }
        }
    }
}