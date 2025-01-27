using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;

public class RoomConnector : MonoBehaviour
{
    public List<Room> rooms;

    [SerializeField]
    public GameObject doorPrefab;

    public string[] possibleConnections1 = { "2", "21", "211", "4", "41", "411" };
    public string[] possibleConnections2 = { "12", "122", "3", "32", "322" };
    public string[] possibleConnections3 = { "23", "233", "4", "43", "433" };
    public string[] possibleConnections4 = { "14", "144", "34", "344" };

    // Depth 2
    // Quadrant 1
    public string[] possibleConnections11 = { "12", "121", "14", "141" };
    public string[] possibleConnections12 = { "112", "13", "132", "21", "211" };
    public string[] possibleConnections13 = { "123", "14" };
    public string[] possibleConnections14 = { "114", "41", "411" };

    // Quadrant 2
    public string[] possibleConnections21 = { "22", "24", "221", "241" };
    public string[] possibleConnections22 = { "23", "232" };
    public string[] possibleConnections23 = { "24", "243", "32", "322" };
    public string[] possibleConnections24 = { "" };

    // Quadrant 3
    public string[] possibleConnections31 = { "32", "321", "34", "341" };
    public string[] possibleConnections32 = { "233", "33", "311", "332" };
    public string[] possibleConnections33 = { "34", "343" };
    public string[] possibleConnections34 = { "43", "433", "334" };

    // Quadrant 4
    public string[] possibleConnections41 = { "42", "421", "44", "441" };
    public string[] possibleConnections42 = { "43", "432" };
    public string[] possibleConnections43 = { "344", "44", "423", "443" };
    public string[] possibleConnections44 = { "434", "414" };

    public Dictionary<string, string> disallowedConnections = new Dictionary<string, string>();

    public RoomConnector()
    {
        rooms = new List<Room>();
        disallowedConnections.Add("113", "131");
        disallowedConnections.Add("124", "142");
        disallowedConnections.Add("213", "231");
        disallowedConnections.Add("224", "242");
        disallowedConnections.Add("313", "331");
        disallowedConnections.Add("324", "342");
        disallowedConnections.Add("413", "431");
        disallowedConnections.Add("424", "442");
    }

    public void GenerateRoomList(QuadTree currentNode, QuadTree previousNode, string sequence)
    {
        if (currentNode == null)
        {
            return;
        }

        if (QuadTree.IsLeaf(currentNode))
        {
            // Debug.Log(sequence);
            var trimmedSequence = (sequence + currentNode.quadrant).TrimStart("0").Trim();
            rooms.Add(new Room(currentNode.room, previousNode, currentNode.depth, trimmedSequence));
        }

        GenerateRoomList(currentNode.q1, currentNode, sequence + currentNode.quadrant);
        GenerateRoomList(currentNode.q2, currentNode, sequence + currentNode.quadrant);
        GenerateRoomList(currentNode.q3, currentNode, sequence + currentNode.quadrant);
        GenerateRoomList(currentNode.q4, currentNode, sequence + currentNode.quadrant);
    }

    public void ConnectLayerThree()
    {
        // Debug.Log("loop start, rooms count = " + rooms.Count + "\n");
        for (int i = 0; i < rooms.Count; i++)
        {
            if (rooms[i].layer != 3)
                continue;

            // Debug.Log("outer loop, i = " + i + "\n");
            for (int j = i + 1; j < rooms.Count; j++)
            {
                // Skips every Layer except for layer 4
                if (rooms[j].layer != 3)
                    continue;
                // Debug.Log("inner loop, j = " + j + "\n");
                // Debug.Log(
                //     "Room Nr: "
                //         + rooms[i].roomNumber
                //         + "sequence [3] = "
                //         + rooms[i].sequence[3]
                //         + "\n"
                // );
                // Skips diagonal connections
                if (
                    (rooms[i].sequence[2] == '1' && rooms[j].sequence[2] == '3')
                    || (rooms[i].sequence[2] == '2' && rooms[j].sequence[2] == '4')
                    || (rooms[i].sequence[2] == '3' && rooms[j].sequence[2] == '1')
                    || (rooms[i].sequence[2] == '4' && rooms[j].sequence[2] == '2')
                )
                {
                    continue;
                }
                if (rooms[i].motherNode == rooms[j].motherNode && rooms[i] != rooms[j])
                {
                    rooms[i].connectedTo.Add(rooms[j]);
                    rooms[j].connectedTo.Add(rooms[i]);
                }
            }
        }
    }

    #region Raumverbindung
    public void ConnectAllLayers()
    {
        char[] charArray = { };
        var reversedSequence = "";
        for (int i = 0; i < rooms.Count; i++)
        {
            PlaceDoors(doorPrefab, rooms[i]);
            for (int j = i - 1; j < rooms.Count; j++)
            {
                if (j < 0 || j == i)
                    continue;
                Debug.Log(
                    "Currently comparing room "
                        + (i + 1)
                        + " and "
                        + (j + 1)
                        + "\n"
                        + "room "
                        + (i + 1)
                        + " sequence: "
                        + rooms[i].sequence
                        + " room "
                        + (j + 1)
                        + " sequence: "
                        + rooms[j].sequence
                );
                //skip diagonal connections
                if (
                    rooms[i].layer == 3
                    && rooms[j].layer == 3
                    && rooms[i].sequence[0] == rooms[j].sequence[0]
                    && rooms[i].sequence[1] == rooms[j].sequence[1]
                    && (
                        (rooms[i].sequence[2] == '1' && rooms[j].sequence[2] == '3')
                        || (rooms[i].sequence[2] == '2' && rooms[j].sequence[2] == '4')
                        || (rooms[i].sequence[2] == '3' && rooms[j].sequence[2] == '1')
                        || (rooms[i].sequence[2] == '4' && rooms[j].sequence[2] == '2')
                    )
                )
                {
                    Debug.Log("Skipped connection for room = " + (i + 1) + "and " + (j + 1));
                    continue;
                }

                // 1.   Depth 3: Connect Quadrants with each other, hier werden 2 und 1 schraeg verbuinden
                if (
                    rooms[i].layer == 3
                    && rooms[j].layer == 3
                    && !checkDisallowed(rooms[i].sequence, rooms[j].sequence)
                )
                {
                    charArray = rooms[j].sequence.Substring(1).ToCharArray();
                    Array.Reverse(charArray);
                    reversedSequence = new string(charArray);
                    if (
                        rooms[i].sequence[0] == rooms[j].sequence[0]
                        && rooms[i].sequence.Substring(1) == reversedSequence
                    )
                    {
                        Debug.Log("Connected  room" + (i + 1) + " and " + (j + 1) + " in step 1");
                        rooms[i].connectedTo.Add(rooms[j]);
                        rooms[j].connectedTo.Add(rooms[i]);
                    }
                }
                // 2.   Depth 3: Connect  inside same quadrant
                if (
                    rooms[i].layer == 3
                    && rooms[j].layer == 3
                    && rooms[i].motherNode == rooms[j].motherNode
                    && rooms[i] != rooms[j]
                    && !checkDisallowed(rooms[i].sequence, rooms[j].sequence)
                )
                {
                    Debug.Log("Connected room " + (i + 1) + " and " + (j + 1) + " in step 2");
                    rooms[i].connectedTo.Add(rooms[j]);
                    rooms[j].connectedTo.Add(rooms[i]);
                    // continue; // Should speed up
                }
                // 3.   Connect at other depths, starting with 1
                if (
                    rooms[i].sequence == "1"
                    && possibleConnections1.Contains(rooms[j].sequence)
                    && !rooms[i].connectedTo.Contains(rooms[j])
                    && !checkDisallowed(rooms[i].sequence, rooms[j].sequence)
                )
                {
                    Debug.Log("Connected room " + (i + 1) + " and " + (j + 1) + " in step 3");
                    rooms[i].connectedTo.Add(rooms[j]);
                    rooms[j].connectedTo.Add(rooms[i]);
                }
                // 4.
                if (
                    rooms[i].sequence == "2"
                    && possibleConnections2.Contains(rooms[j].sequence)
                    && !rooms[i].connectedTo.Contains(rooms[j])
                    && !checkDisallowed(rooms[i].sequence, rooms[j].sequence)
                )
                {
                    Debug.Log("Connected room " + (i + 1) + " and " + (j + 1) + " in step 4");
                    rooms[i].connectedTo.Add(rooms[j]);
                    rooms[j].connectedTo.Add(rooms[i]);
                }
                // 5.
                if (
                    rooms[i].sequence == "3"
                    && possibleConnections3.Contains(rooms[j].sequence)
                    && !rooms[i].connectedTo.Contains(rooms[j])
                    && !checkDisallowed(rooms[i].sequence, rooms[j].sequence)
                )
                {
                    Debug.Log("Connected room " + (i + 1) + " and " + (j + 1) + " in step 5");
                    rooms[i].connectedTo.Add(rooms[j]);
                    rooms[j].connectedTo.Add(rooms[i]);
                }
                // 6.
                if (
                    rooms[i].sequence == "4"
                    && possibleConnections4.Contains(rooms[j].sequence)
                    && !rooms[i].connectedTo.Contains(rooms[j])
                    && !checkDisallowed(rooms[i].sequence, rooms[j].sequence)
                )
                {
                    Debug.Log("Connected room " + (i + 1) + " and " + (j + 1) + " in step 6");
                    rooms[i].connectedTo.Add(rooms[j]);
                    rooms[j].connectedTo.Add(rooms[i]);
                }
                // Depth 2

                // Quadrant 1
                // 7.
                if (
                    rooms[i].sequence == "11"
                    && possibleConnections11.Contains(rooms[j].sequence)
                    && !rooms[i].connectedTo.Contains(rooms[j])
                    && !checkDisallowed(rooms[i].sequence, rooms[j].sequence)
                )
                {
                    Debug.Log("Connected room " + (i + 1) + " and " + (j + 1) + " in step 7");
                    rooms[i].connectedTo.Add(rooms[j]);
                    rooms[j].connectedTo.Add(rooms[i]);
                }
                // 8.
                if (
                    rooms[i].sequence == "12"
                    && possibleConnections12.Contains(rooms[j].sequence)
                    && !rooms[i].connectedTo.Contains(rooms[j])
                    && !checkDisallowed(rooms[i].sequence, rooms[j].sequence)
                )
                {
                    Debug.Log("Connected room " + (i + 1) + " and " + (j + 1) + " in step 8");
                    rooms[i].connectedTo.Add(rooms[j]);
                    rooms[j].connectedTo.Add(rooms[i]);
                }
                // 9.
                if (
                    rooms[i].sequence == "13"
                    && possibleConnections13.Contains(rooms[j].sequence)
                    && !rooms[i].connectedTo.Contains(rooms[j])
                    && !checkDisallowed(rooms[i].sequence, rooms[j].sequence)
                )
                {
                    Debug.Log("Connected room " + (i + 1) + " and " + (j + 1) + " in step 9");
                    rooms[i].connectedTo.Add(rooms[j]);
                    rooms[j].connectedTo.Add(rooms[i]);
                }
                // 10.
                if (
                    rooms[i].sequence == "14"
                    && possibleConnections14.Contains(rooms[j].sequence)
                    && !rooms[i].connectedTo.Contains(rooms[j])
                    && !checkDisallowed(rooms[i].sequence, rooms[j].sequence)
                )
                {
                    Debug.Log("Connected room " + (i + 1) + " and " + (j + 1) + " in step 10");
                    rooms[i].connectedTo.Add(rooms[j]);
                    rooms[j].connectedTo.Add(rooms[i]);
                }

                // Quadrant 2
                // 11.
                if (
                    rooms[i].sequence == "21"
                    && possibleConnections21.Contains(rooms[j].sequence)
                    && !rooms[i].connectedTo.Contains(rooms[j])
                    && !checkDisallowed(rooms[i].sequence, rooms[j].sequence)
                )
                {
                    Debug.Log("Connected room " + (i + 1) + " and " + (j + 1) + " in step 11");
                    rooms[i].connectedTo.Add(rooms[j]);
                    rooms[j].connectedTo.Add(rooms[i]);
                }
                // 12.
                if (
                    rooms[i].sequence == "22"
                    && possibleConnections22.Contains(rooms[j].sequence)
                    && !rooms[i].connectedTo.Contains(rooms[j])
                    && !checkDisallowed(rooms[i].sequence, rooms[j].sequence)
                )
                {
                    Debug.Log("Connected room " + (i + 1) + " and " + (j + 1) + " in step 12");
                    rooms[i].connectedTo.Add(rooms[j]);
                    rooms[j].connectedTo.Add(rooms[i]);
                }
                // 13.
                if (
                    rooms[i].sequence == "23"
                    && possibleConnections23.Contains(rooms[j].sequence)
                    && !rooms[i].connectedTo.Contains(rooms[j])
                    && !checkDisallowed(rooms[i].sequence, rooms[j].sequence)
                )
                {
                    Debug.Log("Connected room " + (i + 1) + " and " + (j + 1) + " in step 13");
                    rooms[i].connectedTo.Add(rooms[j]);
                    rooms[j].connectedTo.Add(rooms[i]);
                }
                // 14
                if (
                    rooms[i].sequence == "24"
                    && possibleConnections24.Contains(rooms[j].sequence)
                    && !rooms[i].connectedTo.Contains(rooms[j])
                    && !checkDisallowed(rooms[i].sequence, rooms[j].sequence)
                )
                {
                    Debug.Log("Connected room " + (i + 1) + " and " + (j + 1) + " in step 14");
                    rooms[i].connectedTo.Add(rooms[j]);
                    rooms[j].connectedTo.Add(rooms[i]);
                }

                // Quadrant 3
                // 15.
                if (
                    rooms[i].sequence == "31"
                    && possibleConnections31.Contains(rooms[j].sequence)
                    && !rooms[i].connectedTo.Contains(rooms[j])
                    && !checkDisallowed(rooms[i].sequence, rooms[j].sequence)
                )
                {
                    Debug.Log("Connected room " + (i + 1) + " and " + (j + 1) + " in step 15");
                    rooms[i].connectedTo.Add(rooms[j]);
                    rooms[j].connectedTo.Add(rooms[i]);
                }
                //16.
                if (
                    rooms[i].sequence == "32"
                    && possibleConnections32.Contains(rooms[j].sequence)
                    && !rooms[i].connectedTo.Contains(rooms[j])
                    && !checkDisallowed(rooms[i].sequence, rooms[j].sequence)
                )
                {
                    Debug.Log("Connected room " + (i + 1) + " and " + (j + 1) + " in step 16");
                    rooms[i].connectedTo.Add(rooms[j]);
                    rooms[j].connectedTo.Add(rooms[i]);
                }
                // 17.
                if (
                    rooms[i].sequence == "33"
                    && possibleConnections33.Contains(rooms[j].sequence)
                    && !rooms[i].connectedTo.Contains(rooms[j])
                    && !checkDisallowed(rooms[i].sequence, rooms[j].sequence)
                )
                {
                    Debug.Log("Connected room " + (i + 1) + " and " + (j + 1) + " in step 17");
                    rooms[i].connectedTo.Add(rooms[j]);
                    rooms[j].connectedTo.Add(rooms[i]);
                }
                // 18.
                if (
                    rooms[i].sequence == "34"
                    && possibleConnections34.Contains(rooms[j].sequence)
                    && !rooms[i].connectedTo.Contains(rooms[j])
                    && !checkDisallowed(rooms[i].sequence, rooms[j].sequence)
                )
                {
                    Debug.Log("Connected room " + (i + 1) + " and " + (j + 1) + " in step 18");
                    rooms[i].connectedTo.Add(rooms[j]);
                    rooms[j].connectedTo.Add(rooms[i]);
                }

                // Quadrant 4
                // 19.
                if (
                    rooms[i].sequence == "41"
                    && possibleConnections41.Contains(rooms[j].sequence)
                    && !rooms[i].connectedTo.Contains(rooms[j])
                    && !checkDisallowed(rooms[i].sequence, rooms[j].sequence)
                )
                {
                    Debug.Log("Connected room " + (i + 1) + " and " + (j + 1) + " in step 19");
                    rooms[i].connectedTo.Add(rooms[j]);
                    rooms[j].connectedTo.Add(rooms[i]);
                }
                // 20.
                if (
                    rooms[i].sequence == "42"
                    && possibleConnections42.Contains(rooms[j].sequence)
                    && !rooms[i].connectedTo.Contains(rooms[j])
                    && !checkDisallowed(rooms[i].sequence, rooms[j].sequence)
                )
                {
                    Debug.Log("Connected room " + (i + 1) + " and " + (j + 1) + " in step 20");
                    rooms[i].connectedTo.Add(rooms[j]);
                    rooms[j].connectedTo.Add(rooms[i]);
                }
                // 21.
                if (
                    rooms[i].sequence == "43"
                    && possibleConnections43.Contains(rooms[j].sequence)
                    && !rooms[i].connectedTo.Contains(rooms[j])
                    && !checkDisallowed(rooms[i].sequence, rooms[j].sequence)
                )
                {
                    Debug.Log("Connected room " + (i + 1) + " and " + (j + 1) + " in step 21");
                    rooms[i].connectedTo.Add(rooms[j]);
                    rooms[j].connectedTo.Add(rooms[i]);
                }
                // 22.
                if (
                    rooms[i].sequence == "44"
                    && possibleConnections44.Contains(rooms[j].sequence)
                    && !rooms[i].connectedTo.Contains(rooms[j])
                    && !checkDisallowed(rooms[i].sequence, rooms[j].sequence)
                )
                {
                    Debug.Log("Connected room " + (i + 1) + " and " + (j + 1) + " in step 22");
                    rooms[i].connectedTo.Add(rooms[j]);
                    rooms[j].connectedTo.Add(rooms[i]);
                }
            }
        }
    }

    public bool checkDisallowed(string roomOneSequence, string roomTwoSequence)
    {
        var output = "";
        if (disallowedConnections.ContainsKey(roomOneSequence))
        {
            disallowedConnections.TryGetValue(roomOneSequence, out output);
            if (output == roomTwoSequence)
            {
                Debug.Log(
                    "checkDisallowed for "
                        + roomOneSequence
                        + " and "
                        + roomTwoSequence
                        + " result: true"
                );
                return true;
            }
        }
        if (disallowedConnections.ContainsKey(roomTwoSequence))
        {
            disallowedConnections.TryGetValue(roomTwoSequence, out output);
            if (output == roomOneSequence)
            {
                Debug.Log(
                    "checkDisallowed for "
                        + roomOneSequence
                        + " and "
                        + roomTwoSequence
                        + " result: true"
                );
                return true;
            }
        }
        Debug.Log(
            "checkDisallowed for " + roomOneSequence + " and " + roomTwoSequence + " result: false"
        );
        return false;
    }

    #endregion

    public void PrintConnectedRooms()
    {
        foreach (var room in rooms)
        {
            foreach (var connectedRoom in room.connectedTo)
            {
                Debug.Log(
                    "Room Nr: "
                        + room.roomNumber
                        + " is connected to Room Nr: "
                        + connectedRoom.roomNumber
                        + "\n"
                );
            }
        }
    }

    public void PlaceDoors(GameObject doorPrefab, Room room)
    {
        var leftX = room.rect.center.x - room.rect.width * 0.5 + 1;
        var leftY = room.rect.center.y;
        var leftPos = new Vector2((float)leftX, (float)leftY);

        var rightX = room.rect.center.x + room.rect.width * 0.5 - 1;
        var rightY = room.rect.center.y;
        var rightPos = new Vector2((float)rightX, (float)rightY);

        var upX = room.rect.center.x;
        var upY = room.rect.center.y + room.rect.height * 0.5 - 3;
        var upPos = new Vector2((float)upX, (float)upY);

        var downX = room.rect.center.x;
        var downY = room.rect.y + 1;
        var downPos = new Vector2((float)downX, (float)downY);

        var upperRotation = UnityEngine.Quaternion.Euler(0, 0, 0 + 90);

        room.leftDoor = Instantiate(doorPrefab, leftPos, doorPrefab.transform.rotation);
        room.rightDoor = Instantiate(doorPrefab, rightPos, doorPrefab.transform.rotation);
        room.upperDoor = Instantiate(doorPrefab, upPos, upperRotation);
        room.lowerDoor = Instantiate(doorPrefab, downPos, upperRotation);

        room.leftDoor.GetComponent<Door>().position = "left";
        room.rightDoor.GetComponent<Door>().position = "right";
        room.upperDoor.GetComponent<Door>().position = "upper";
        room.lowerDoor.GetComponent<Door>().position = "lower";

        // Deactivate all doors
        room.leftDoor.SetActive(false);
        room.rightDoor.SetActive(false);
        room.upperDoor.SetActive(false);
        room.lowerDoor.SetActive(false);
    }

    public void SetDoorDirections()
    {
        var checkDirection = new Vector2(0, 0);
        Door doorScript = new Door();
        foreach (var room in rooms)
        {
            foreach (var connectedRoom in room.connectedTo)
            {
                checkDirection = connectedRoom.rect.center - room.rect.center;
                checkDirection.Normalize();
                if (Math.Abs(checkDirection.x) > Math.Abs(checkDirection.y))
                {
                    if (checkDirection.x < 0)
                    {
                        Debug.Log("activated left door for room: " + room.roomNumber);
                        room.leftDoor.SetActive(true);
                        room.leftDoor.GetComponent<Door>().connectedTo = connectedRoom.rightDoor;
                        connectedRoom.rightDoor.GetComponent<Door>().connectedTo = room.leftDoor;
                    }
                    else
                    {
                        Debug.Log("activated right door for room: " + room.roomNumber);
                        room.rightDoor.SetActive(true);
                        room.rightDoor.GetComponent<Door>().connectedTo = connectedRoom.leftDoor;
                        connectedRoom.leftDoor.GetComponent<Door>().connectedTo = room.rightDoor;
                    }
                }
                else
                {
                    if (checkDirection.y > 0)
                    {
                        Debug.Log("activated up door for room: " + room.roomNumber);
                        room.upperDoor.SetActive(true);
                        room.upperDoor.GetComponent<Door>().connectedTo = connectedRoom.lowerDoor;
                        connectedRoom.lowerDoor.GetComponent<Door>().connectedTo = room.upperDoor;
                    }
                    else
                    {
                        Debug.Log("activated down door for room: " + room.roomNumber);
                        room.lowerDoor.SetActive(true);
                        room.lowerDoor.GetComponent<Door>().connectedTo = connectedRoom.upperDoor;
                        connectedRoom.upperDoor.GetComponent<Door>().connectedTo = room.lowerDoor;
                    }
                }
            }
        }
    }
}
