using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
            var seq = (sequence + currentNode.quadrant).TrimStart("0");
            rooms.Add(new Room(currentNode.room, previousNode, currentNode.depth, seq));
        }

        GenerateRoomList(currentNode.q1, currentNode, sequence + currentNode.quadrant);
        GenerateRoomList(currentNode.q2, currentNode, sequence + currentNode.quadrant);
        GenerateRoomList(currentNode.q3, currentNode, sequence + currentNode.quadrant);
        GenerateRoomList(currentNode.q4, currentNode, sequence + currentNode.quadrant);
    }

    public void ConnectLayerFour()
    {
        foreach (var room in rooms)
        {
            for (int i = 1; i < rooms.Count; i++)
            {
                if (room.layer == 4 && room.motherNode == rooms[i].motherNode)
                {
                    room.connectedTo.Add(rooms[i]);
                    rooms[i].connectedTo.Add(room);
                }
            }
        }
    }
}
