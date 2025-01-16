using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadTree
{
    public QuadTree q1;
    public QuadTree q2;
    public QuadTree q3;
    public QuadTree q4;
    public Rect container;
    public Rect room;

    public int quadrant;
    public int depth;
    public int roomNumber;

    public QuadTree(Rect givenContainer, int givenDepth, int givenQuadrant)
    {
        container = givenContainer;
        quadrant = givenQuadrant;
        depth = givenDepth;
    }

    internal static QuadTree SplitTree(
        int minSize,
        int maxSize,
        Rect container,
        int givenDepth = 0,
        int givenQuadrant = 0
    )
    {
        var currentNode = new QuadTree(container, givenDepth, givenQuadrant);
        var newContainers = SplitContainer(container);

        if (container.width <= 2 * minSize || container.height <= 2 * minSize)
        {
            return currentNode;
        }

        if (givenDepth == 0)
        {
            currentNode.q1 = SplitTree(minSize, maxSize, newContainers[0], givenDepth + 1, 1);
            currentNode.q2 = SplitTree(minSize, maxSize, newContainers[1], givenDepth + 1, 2);
            currentNode.q3 = SplitTree(minSize, maxSize, newContainers[2], givenDepth + 1, 3);
            currentNode.q4 = SplitTree(minSize, maxSize, newContainers[3], givenDepth + 1, 4);
            return currentNode;
        }
        else
        {
            var splitProbability = Random.Range(0, 100) > 50 ? true : false;
            // Debug.Log("splitProb: " + splitProbability);
            if (splitProbability && givenDepth < 3)
            {
                currentNode.q1 = SplitTree(minSize, maxSize, newContainers[0], givenDepth + 1, 1);
                currentNode.q2 = SplitTree(minSize, maxSize, newContainers[1], givenDepth + 1, 2);
                currentNode.q3 = SplitTree(minSize, maxSize, newContainers[2], givenDepth + 1, 3);
                currentNode.q4 = SplitTree(minSize, maxSize, newContainers[3], givenDepth + 1, 4);
            }

            return currentNode;
        }
    }

    public static Rect[] SplitContainer(Rect givenContainer)
    {
        Rect c1,
            c2,
            c3,
            c4;
        var newWidth = (int)(givenContainer.width * 0.5f);
        var newHeight = (int)(givenContainer.height * 0.5f);

        c1 = new Rect(givenContainer.x, givenContainer.y, newWidth, newHeight);
        c2 = new Rect(givenContainer.x + newWidth, givenContainer.y, newWidth, newHeight);
        c3 = new Rect(
            givenContainer.x + newWidth,
            givenContainer.y + newHeight,
            newWidth,
            newHeight
        );
        c4 = new Rect(givenContainer.x, givenContainer.y + newHeight, newWidth, newHeight);

        // Debug Info
        // Debug.Log("-----------------Start Containers-------------------" +
        //     "\nc1 x: " + c1.x + "\nc1 y: " + c1.y +
        //     "\nc1 width: " + c1.width + "\n c1 height: " + c1.height +
        //     "\nc2 x: " + c2.x + "\nc2 y: " + c2.y +
        //     "\nc2 width: " + c2.width + "\n c2 height: " + c2.height +
        //     "\nc3 x: " + c3.x + "\nc3 y: " + c3.y +
        //     "\nc3 width: " + c3.width + "\n c3 height: " + c3.height +
        //     "\nc4 x: " + c4.x + "\nc4 y: " + c4.y +
        //     "\nc4 width: " + c4.width + "\n c4 height: " + c4.height +
        //     "\n----------------------End Containers---------------------"
        //     );
        return new Rect[] { c1, c2, c3, c4 };
    }

    // Generates a single room
    public static Rect GenerateRoom(Rect container, int minSize, int maxSize)
    {
        int width = 0;
        int height = 0;
        // Limit the maximum room size for playability
        if ((int)container.width > maxSize)
        {
            // width = Random.Range((int)container.width / 4, (int)container.width - 5);
            // height = Random.Range((int)container.height / 4, (int)container.height - 5);
            width = Random.Range(minSize, maxSize);
            height = Random.Range(minSize, maxSize);
        }
        else
        {
            width = Random.Range(minSize, (int)container.width - 5);
            height = Random.Range(minSize, (int)container.height - 5);
        }

        // Debug.Log("QUAD ROOM width: " + width + " height: " + height);

        // int x = Random.Range((int)container.x + 5, (int)container.xMax - width - 5);
        // int y = Random.Range((int)container.y + 5, (int)container.yMax - height - 5);
        int x = (int)container.center.x - width / 2;
        int y = (int)container.center.y - height / 2;

        return new Rect(x, y, width, height);
    }

    /// Generates and places rooms in all leaf nodes of the QuadTree
    public static void PlaceRooms(QuadTree tree, int minSize, int maxSize)
    {
        if (tree == null)
        {
            return;
        }

        PlaceRooms(tree.q1, minSize, maxSize);
        PlaceRooms(tree.q2, minSize, maxSize);
        PlaceRooms(tree.q3, minSize, maxSize);
        PlaceRooms(tree.q4, minSize, maxSize);

        if (IsLeaf(tree))
        {
            tree.room = GenerateRoom(tree.container, minSize, maxSize);
        }
    }

    public static bool IsLeaf(QuadTree tree)
    {
        if (tree.q1 != null || tree.q2 != null || tree.q3 != null || tree.q4 != null)
            return false;

        return true;
    }
}
