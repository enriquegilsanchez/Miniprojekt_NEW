using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEditor;
using UnityEngine;

public class QuadTreeGenerator : MonoBehaviour
{
    public bool DebugDraw;

    public QuadTree tree;

    [SerializeField] int quadSize;
    [SerializeField] int[] quadPos = { 0, 0 };
    [SerializeField] int minSize;

    // Start is called before the first frame update
    void Start()
    {
        Rect rect = new Rect(quadPos[0], quadPos[1], quadSize, quadSize);
        tree = QuadTree.SplitTree(minSize, rect);
        PrintLeafSizes(tree);
        QuadTree.PlaceRooms(tree, minSize);
    }
    void OnDrawGizmos()
    {
        if (DebugDraw && tree != null)
        {
            DrawQuadTree(tree);
            DrawSizeIndicator();
            DrawRooms(tree);
        }
    }

    void DrawQuadTree(QuadTree tree)
    {
        Gizmos.color = Color.grey;

        Gizmos.DrawWireCube(tree.container.center, tree.container.size);

        if (tree.q1 != null) DrawQuadTree(tree.q1);
        if (tree.q2 != null) DrawQuadTree(tree.q2);
        if (tree.q3 != null) DrawQuadTree(tree.q3);
        if (tree.q4 != null) DrawQuadTree(tree.q4);
    }

    void DrawRooms(QuadTree tree)
    {
        if (tree == null)
            return;

        Gizmos.color = Color.white;

        DrawRooms(tree.q1);
        DrawRooms(tree.q2);
        DrawRooms(tree.q3);
        DrawRooms(tree.q4);

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
            "----------------------------------------------------\n" +
            "QUAD Leaf size: width = " + tree.container.width + " height = " + tree.container.height +
            "\n----------------------------------------------------");
            // Debug.Log("Leaf size: width = " + tree.container.width + " height = " + tree.container.height);
            // Debug.Log("\n----------------------------------------------------");
        }
    }

    void DrawSizeIndicator()
    {
        Debug.DrawLine(new UnityEngine.Vector2(quadPos[0], quadPos[1] - 10), new UnityEngine.Vector2(quadPos[0] + quadSize, quadPos[1] - 10));
        Debug.DrawLine(new UnityEngine.Vector2(quadPos[0] - 10, quadPos[1]), new UnityEngine.Vector2(quadPos[0] - 10, quadPos[1] + quadSize));
        Handles.BeginGUI();
        GUI.color = Color.white;
        Handles.Label(new UnityEngine.Vector2(quadPos[0] + quadSize / 2 - 60, quadPos[1] + quadSize + 50), "Method: QuadTree");
        Handles.Label(new UnityEngine.Vector2(quadPos[0] + quadSize / 2 - 60, quadPos[1] - 30), "width: " + quadSize);
        Handles.Label(new UnityEngine.Vector2(quadPos[0] - 150, quadPos[1] + quadSize / 2), "height: " + quadSize);
        Handles.EndGUI();

    }
}
