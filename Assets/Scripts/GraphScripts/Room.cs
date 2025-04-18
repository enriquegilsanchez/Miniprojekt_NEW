using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room
{
    public Rect rect;
    public QuadTree motherNode;
    public int roomNumber;
    public int layer;
    public List<Room> connectedTo;
    public string sequence;
    public GameObject upperDoor;
    public GameObject lowerDoor;
    public GameObject leftDoor;
    public GameObject rightDoor;
    public bool isCleared;

    public int numberOfEnemies;

    public Room(Rect givenRect, QuadTree givenNode, int givenLayer, string givenSequence)
    {
        rect = givenRect;
        motherNode = givenNode;
        layer = givenLayer;
        sequence = givenSequence;
        connectedTo = new List<Room>();
        isCleared = false;
        numberOfEnemies = 0;
    }

    public override string ToString()
    {
        return "roomNr: "
            + roomNumber
            + " center position: "
            + rect.center.ToString()
            + " layer: "
            + layer
            + " sequence: "
            + sequence
            + "\n";
    }
}
