using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEditor;
using UnityEngine;

public class BspGenerator : MonoBehaviour
{

    public bool DebugDraw;

    public BspTree tree;

    [SerializeField] int bspSize;
    [SerializeField] int[] bspPos = { 0, 0 };
    [SerializeField] int numberOfSplits;

    // Start is called before the first frame update
    void Start()
    {
        Rect rect = new Rect(bspPos[0], bspPos[1], bspSize, bspSize);
        tree = BspTree.splitTree(numberOfSplits, rect);
        // PrintLeafSizes(tree);
        BspTree.placeRooms(tree);
    }
    void OnDrawGizmos()
    {
        if (DebugDraw && tree != null)
        {
            DrawBspTree(tree);
            DrawSizeIndicator();
            DrawRooms(tree);
        }
    }

    void DrawBspTree(BspTree tree)
    {
        Gizmos.color = Color.grey;

        Gizmos.DrawWireCube(tree.container.center, tree.container.size);

        if (tree.leftChild != null) DrawBspTree(tree.leftChild);
        if (tree.rightChild != null) DrawBspTree(tree.rightChild);
    }

    void DrawRooms(BspTree tree)
    {
        if (tree == null)
            return;

        Gizmos.color = Color.white;

        DrawRooms(tree.leftChild);
        DrawRooms(tree.rightChild);

        if (BspTree.IsLeaf(tree))
        {
            Gizmos.DrawCube(tree.room.center, tree.room.size);
        }
    }

    void PrintLeafSizes(BspTree tree)
    {
        if (tree == null)
            return;

        PrintLeafSizes(tree.leftChild);
        PrintLeafSizes(tree.rightChild);

        if (BspTree.IsLeaf(tree))
        {
            Debug.Log(
            "----------------------------------------------------\n" +
            "BSP Leaf size: width = " + tree.container.width + " height = " + tree.container.height +
            "\n----------------------------------------------------");
            // Debug.Log("Leaf size: width = " + tree.container.width + " height = " + tree.container.height);
            // Debug.Log("\n----------------------------------------------------");
        }
    }

    void DrawSizeIndicator()
    {
        Debug.DrawLine(new UnityEngine.Vector2(bspPos[0], bspPos[1] - 10), new UnityEngine.Vector2(bspPos[0] + bspSize, bspPos[1] - 10));
        Debug.DrawLine(new UnityEngine.Vector2(bspPos[0] - 10, bspPos[1]), new UnityEngine.Vector2(bspPos[0] - 10, bspPos[1] + bspSize));
        Handles.BeginGUI();
        GUI.color = Color.white;
        Handles.Label(new UnityEngine.Vector2(bspPos[0] + bspSize / 2 - 60, bspPos[1] + bspSize + 50), "Method: BspTree");
        Handles.Label(new UnityEngine.Vector2(bspPos[0] + bspSize / 2 - 60, bspPos[1] - 30), "width: " + bspSize);
        Handles.Label(new UnityEngine.Vector2(bspPos[0] - 150, bspPos[1] + bspSize / 2), "height: " + bspSize);
        Handles.EndGUI();

    }
}

