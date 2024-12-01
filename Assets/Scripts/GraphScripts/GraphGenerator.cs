using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEditor;
using UnityEngine;

public class GraphGenerator : MonoBehaviour
{

    public bool DebugDraw;

    public BspTree tree;

    [SerializeField] int bspSize;
    [SerializeField] int[] bspPos = { 0, 0 };
    [SerializeField] int numberOfSplits;
    [SerializeField] int widthMin;
    [SerializeField] int widthMax;
    [SerializeField] int heightMin;
    [SerializeField] int heightMax;
    // Start is called before the first frame update
    void Start()
    {
        Rect rect = new Rect(bspPos[0], bspPos[1], bspSize, bspSize);
        tree = BspTree.splitTree(numberOfSplits, rect);
        PrintLeafSizes(tree);
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
        Gizmos.color = Color.white;

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

        if (BspTree.isLeaf(tree))
        {
            int width = Random.Range(widthMin, widthMax);
            int height = Random.Range(heightMin, heightMax);

            Gizmos.DrawCube(tree.container.center, new UnityEngine.Vector2(width, height));
        }
    }

    void PrintLeafSizes(BspTree tree)
    {
        if (tree == null)
            return;

        PrintLeafSizes(tree.leftChild);
        PrintLeafSizes(tree.rightChild);

        if (BspTree.isLeaf(tree))
        {
            Debug.Log("----------------------------------------------------");
            Debug.Log("Leaf size: width = " + tree.container.width + " height = " + tree.container.height);
            Debug.Log("----------------------------------------------------");
        }
    }

    void DrawSizeIndicator()
    {
        Debug.DrawLine(new UnityEngine.Vector2(bspPos[0], bspPos[1] - 10), new UnityEngine.Vector2(bspPos[0] + bspSize, bspPos[1] - 10));
        Debug.DrawLine(new UnityEngine.Vector2(bspPos[0] - 10, bspPos[1]), new UnityEngine.Vector2(bspPos[0] - 10, bspPos[1] + bspSize));
        Handles.BeginGUI();
        GUI.color = Color.white;
        Handles.Label(new UnityEngine.Vector2(bspPos[0] + bspSize / 2 - 50, bspPos[1] - 30), "width: " + bspSize);
        Handles.Label(new UnityEngine.Vector2(bspPos[0] - 100, bspPos[1] + bspSize / 2), "height: " + bspSize);
        Handles.EndGUI();

    }
}
