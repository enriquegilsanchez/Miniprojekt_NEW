using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class DungeonTile
{
    public int[] position;
    public string type;

    public DungeonTile(int[] givenPosition, string givenType)
    {
        position = givenPosition;
        type = givenType;
    }

    public override string ToString()
    {
        return "Tiletype " + type + " position: { " + position[0] + ", " + position[1] + " }";
    }
}
