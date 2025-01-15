using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomConnector : MonoBehaviour
{
    public List<Room> rooms;

    public RoomConnector()
    {
        rooms = new List<Room>();
    }

    public void GenerateRoomList(QuadTree currentNode, QuadTree previousNode, string sequence)
    {
        if (currentNode == null)
        {
            return;
        }

        if (QuadTree.IsLeaf(currentNode))
        {
            Debug.Log(sequence);
            rooms.Add(new Room(currentNode.room, previousNode, currentNode.depth, sequence));
        }

        GenerateRoomList(currentNode.q1, currentNode, sequence + currentNode.quadrant);
        GenerateRoomList(currentNode.q2, currentNode, sequence + currentNode.quadrant);
        GenerateRoomList(currentNode.q3, currentNode, sequence + currentNode.quadrant);
        GenerateRoomList(currentNode.q4, currentNode, sequence + currentNode.quadrant);
    }
}
