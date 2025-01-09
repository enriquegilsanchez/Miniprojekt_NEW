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
    public Tilemap floorTilemap;
    public List<DungeonTile[,]> allTiles = new List<DungeonTile[,]>();

    // Start is called before the first frame update
    void Start()
    {
        allTiles.Clear();
        Rect rect = new Rect(quadPos[0], quadPos[1], quadSize, quadSize);
        tree = QuadTree.SplitTree(minSize, maxSize, rect);
        QuadTree.PlaceRooms(tree, minSize, maxSize);
        // Adds all rooms
        GenerateRoomGrids(tree);

        // For testing single room classification
        // Rect testRoom = new Rect(0, 0, 5, 7);
        // allTiles.Add(roomGenerator.RoomToDungeonTiles(testRoom));
        // var testPos = floorTilemap.WorldToCell(new Vector3Int(0, 0, 0));
    }

    void OnDrawGizmos()
    {
        if (DebugDraw && tree != null)
        {
            DrawQuadTreeContainers(tree);
            DrawSizeIndicator();
            // DrawRooms(tree);
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
}
