using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class BspTree
{
    public BspTree leftChild;
    public BspTree rightChild;
    public Rect container;

    public BspTree(Rect givenContainer)
    {
        container = givenContainer;
    }

    internal static BspTree splitTree(int splitTimes, Rect container)
    {

        var currentNode = new BspTree(container);

        if (splitTimes == 0)
        {
            return currentNode;
        }

        var newContainers = splitContainer(container);
        currentNode.leftChild = splitTree(splitTimes - 1, newContainers[0]);
        Debug.Log("splitTimes after left: " + splitTimes);

        currentNode.rightChild = splitTree(splitTimes - 1, newContainers[1]);
        Debug.Log("splitTimes after right: " + splitTimes);

        return currentNode;

    }

    public static Rect[] splitContainer(Rect givenContainer)
    {

        Rect container1, container2;

        var splitVertical = Random.value < 0.5f;

        Debug.Log("splitVertical: " + splitVertical);

        if (splitVertical)
        {
            var newWidth = (int)(givenContainer.width * Random.Range(0.3f, 0.6f));
            container1 = new Rect(givenContainer.x, givenContainer.y, newWidth, givenContainer.height);
            container2 = new Rect(givenContainer.x + container1.width, givenContainer.y, givenContainer.width - container1.width, givenContainer.height);
        }
        else
        {
            var newHeight = (int)(givenContainer.height * Random.Range(0.3f, 0.6f));
            container1 = new Rect(givenContainer.x, givenContainer.y, givenContainer.width, newHeight);
            container2 = new Rect(givenContainer.x, givenContainer.y + container1.height, givenContainer.width, givenContainer.height - container1.height);
        }
        Debug.Log("-----------------Start Containers-------------------");
        Debug.Log("c1 x: " + container1.x);
        Debug.Log("c1 y: " + container1.y);
        Debug.Log("c1 width: " + container1.width);
        Debug.Log("c1 height: " + container1.height);
        Debug.Log("----------------------------------------------------");
        Debug.Log("c2 x: " + container2.x);
        Debug.Log("c2 y: " + container2.y);
        Debug.Log("c2 width: " + container2.width);
        Debug.Log("c2 height: " + container2.height);
        Debug.Log("-----------------End Containers---------------------");
        return new Rect[] { container1, container2 };
    }

}
