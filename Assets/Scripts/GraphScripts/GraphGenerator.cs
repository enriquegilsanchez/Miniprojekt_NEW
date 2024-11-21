using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphGenerator : MonoBehaviour
{

    public bool DebugDraw;

    public BspTree tree;
    // Start is called before the first frame update
    void Start()
    {
        Rect rect = new Rect(960, 540, 50, 50);
        tree = BspTree.splitTree(4, rect);
    }
    void OnDrawGizmos()
    {
        if (DebugDraw && tree != null)
        {
            DrawBspTree(tree);
        }
    }

    void DrawBspTree(BspTree node)
    {
        Gizmos.color = Color.white;

        Gizmos.DrawWireCube(node.container.center, node.container.size);

        if (node.leftChild != null) DrawBspTree(node.leftChild);
        if (node.rightChild != null) DrawBspTree(node.rightChild);
    }
}
