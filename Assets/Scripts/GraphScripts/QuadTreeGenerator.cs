using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class QuadTreeGenerator : MonoBehaviour
{
    public bool DebugDraw;

    public QuadTree tree;

    [SerializeField]
    int quadSize;

    [SerializeField]
    int[] quadPos = { 0, 0 };

    [SerializeField]
    int minSize;

    [SerializeField]
    int maxSize;

    [SerializeField]
    RoomGenerator roomGenerator;

    [SerializeField]
    RoomConnector roomConnector;
    public Tilemap floorTilemap;

    [SerializeField]
    public EnemySpawner enemySpawner;

    [SerializeField]
    public GameObject player;

    public List<Room> roomList;
    public Room spawnRoom;
    public Room bossRoom;
    public List<DungeonTile[,]> allTiles = new List<DungeonTile[,]>();
    private bool drawnRoomNumbers = false;

    Component gameControl;

    // Start is called before the first frame update
    void Start()
    {
        gameControl = GameObject
            .FindGameObjectWithTag("GameController")
            .GetComponent("GameControl");
        allTiles.Clear();
        Rect rect = new Rect(quadPos[0], quadPos[1], quadSize, quadSize);
        tree = QuadTree.SplitTree(minSize, maxSize, rect);
        QuadTree.PlaceRooms(tree, minSize, maxSize);
        // Adds all rooms
        GenerateRoomGrids(tree);
        CorrectFirstQuadrants(tree);
        roomConnector.GenerateRoomList(tree, null, "");
        roomConnector.ConnectAllLayers();
        roomList = roomConnector.rooms;
        AssignRoomNumbers(roomList);
        // roomConnector.PrintConnectedRooms();
        DetermineSpawnAndBossRooms(roomList);
        SetSpawnPoint();
        roomConnector.SetDoorDirections();
    }

    void OnDrawGizmos()
    {
        if (DebugDraw && tree != null)
        {
            DrawQuadTreeContainers(tree);
            DrawSizeIndicator();
            // DrawRooms(tree);
        }
        if (!drawnRoomNumbers && roomList != null)
        {
            DrawRoomNumbers(roomList);
            DrawRoomConnectors(roomList);
        }
    }

    void DrawQuadTreeContainers(QuadTree tree)
    {
        Gizmos.color = Color.grey;

        Gizmos.DrawWireCube(tree.container.center, tree.container.size);

        if (tree.q1 != null)
            DrawQuadTreeContainers(tree.q1);
        if (tree.q2 != null)
            DrawQuadTreeContainers(tree.q2);
        if (tree.q3 != null)
            DrawQuadTreeContainers(tree.q3);
        if (tree.q4 != null)
            DrawQuadTreeContainers(tree.q4);
    }

    void DrawRoomContainers(QuadTree tree)
    {
        if (tree == null)
            return;

        Gizmos.color = Color.white;

        DrawRoomContainers(tree.q1);
        DrawRoomContainers(tree.q2);
        DrawRoomContainers(tree.q3);
        DrawRoomContainers(tree.q4);

        if (QuadTree.IsLeaf(tree))
        {
            Gizmos.DrawCube(tree.room.center, tree.room.size);
        }
    }

    void PrintLeafSizes(QuadTree tree)
    {
        if (tree == null)
            return;

        PrintLeafSizes(tree.q1);
        PrintLeafSizes(tree.q2);
        PrintLeafSizes(tree.q3);
        PrintLeafSizes(tree.q4);

        if (QuadTree.IsLeaf(tree))
        {
            Debug.Log(
                "----------------------------------------------------\n"
                    + "QUAD Leaf size: width = "
                    + tree.container.width
                    + " height = "
                    + tree.container.height
                    + "\n----------------------------------------------------"
            );
            // Debug.Log("Leaf size: width = " + tree.container.width + " height = " + tree.container.height);
            // Debug.Log("\n----------------------------------------------------");
        }
    }

    void DrawSizeIndicator()
    {
        Debug.DrawLine(
            new UnityEngine.Vector2(quadPos[0], quadPos[1] - 10),
            new UnityEngine.Vector2(quadPos[0] + quadSize, quadPos[1] - 10)
        );
        Debug.DrawLine(
            new UnityEngine.Vector2(quadPos[0] - 10, quadPos[1]),
            new UnityEngine.Vector2(quadPos[0] - 10, quadPos[1] + quadSize)
        );
        Handles.BeginGUI();
        GUI.color = Color.white;
        Handles.Label(
            new UnityEngine.Vector2(quadPos[0] + quadSize / 2 - 60, quadPos[1] + quadSize + 50),
            "Method: QuadTree"
        );
        Handles.Label(
            new UnityEngine.Vector2(quadPos[0] + quadSize / 2 - 60, quadPos[1] - 30),
            "width: " + quadSize
        );
        Handles.Label(
            new UnityEngine.Vector2(quadPos[0] - 150, quadPos[1] + quadSize / 2),
            "height: " + quadSize
        );
        Handles.EndGUI();
    }

    void GenerateRoomGrids(QuadTree tree)
    {
        if (tree == null)
            return;

        GenerateRoomGrids(tree.q1);
        GenerateRoomGrids(tree.q2);
        GenerateRoomGrids(tree.q3);
        GenerateRoomGrids(tree.q4);

        if (QuadTree.IsLeaf(tree))
        {
            allTiles.Add(roomGenerator.RoomToDungeonTiles(tree.room));
        }
    }

    void CorrectFirstQuadrants(QuadTree tree)
    {
        tree.q1.quadrant = 1;
        tree.q2.quadrant = 2;
        tree.q3.quadrant = 3;
        tree.q4.quadrant = 4;
    }

    void AssignRoomNumbers(List<Room> rooms)
    {
        int roomNumber = 1;
        foreach (var room in rooms)
        {
            room.roomNumber = roomNumber;
            Debug.Log(room.ToString());
            roomNumber++;
        }
    }

    void DrawRoomNumbers(List<Room> rooms)
    {
        Handles.BeginGUI();
        GUI.color = Color.white;
        foreach (var room in rooms)
        {
            Handles.Label(
                new UnityEngine.Vector2(room.rect.center.x - 3, room.rect.center.y + 3),
                room.roomNumber.ToString()
            );
        }
        Handles.EndGUI();
    }

    void DrawRoomConnectors(List<Room> rooms)
    {
        foreach (var room in rooms)
        {
            foreach (var connectedRoom in room.connectedTo)
            {
                if (room.layer == 3)
                    Debug.DrawLine(room.rect.center, connectedRoom.rect.center, Color.red);
                if (room.layer == 2)
                    Debug.DrawLine(room.rect.center, connectedRoom.rect.center, Color.yellow);
                if (room.layer == 1)
                    Debug.DrawLine(room.rect.center, connectedRoom.rect.center, Color.magenta);
            }
        }
    }

    void DetermineSpawnAndBossRooms(List<Room> rooms)
    {
        spawnRoom = rooms[0];
        bossRoom = rooms[rooms.Count - 1];
        var minRoom = rooms[0];
        var maxRoom = rooms[0];

        foreach (var room in rooms)
        {
            if (room.rect.size.magnitude > maxRoom.rect.size.magnitude)
            {
                maxRoom = room;
            }
            if (room.rect.size.magnitude < minRoom.rect.size.magnitude)
            {
                minRoom = room;
            }
        }
        spawnRoom = minRoom;
        bossRoom = maxRoom;
        Debug.Log(
            "spawn room is room nr: "
                + spawnRoom.roomNumber
                + "\n"
                + "boss room is room nr: "
                + bossRoom.roomNumber
        );
        spawnRoom.isCleared = true;
        gameControl.SendMessage("SetSpawnRoom", spawnRoom);
        gameControl.SendMessage("SetBossRoom", bossRoom);
    }

    void SetSpawnPoint()
    {
        // Debug.Log("Should move player to: " + spawnRoom.rect.center.ToString());
        player.transform.Translate(spawnRoom.rect.center);
    }
}
