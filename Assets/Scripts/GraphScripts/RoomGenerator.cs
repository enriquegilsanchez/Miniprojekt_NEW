using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEditor;
using UnityEngine;

public class RoomGenerator
{
    internal static DungeonTile[,] RoomToGrid(Rect room)
    {
        int xMin = (int)room.xMin;
        int xMax = (int)room.xMax;
        int yMin = (int)room.yMin;
        int yMax = (int)room.yMax;
        DungeonTile[,] tiles = new DungeonTile[xMax, yMax];
        int[] position = { 0, 0 };
        string type = "";

        for (var i = yMax; i > yMin; i--)
        {
            for (var j = xMin; j < xMax; j++)
            {
                position[0] = (int)room.x;
                position[1] = (int)room.y;
                type = DetermineRoomType(j, i, xMin, xMax, yMin, yMax);
                tiles[i, j] = new DungeonTile(position, type);
                Debug.Log("gone over tile x = " + j + " y = " + i);
                Debug.Log(tiles[i, j].ToString());
            }
        }

        Debug.Log("did something");
        return tiles;
    }

    public static string DetermineRoomType(int x, int y, int xMin, int xMax, int yMin, int yMax)
    {
        if (x > xMin && x < xMax && y >= yMax - 1)
            return "wall_top";
        if (x == xMin && y == yMax)
            return "c4";
        if (x == xMax && y == yMax)
            return "c3";
        if (x == xMax && y == yMin)
            return "c2";
        if (x == xMin && y == yMin)
            return "c1";
        if (x == xMin && y > yMin && y < yMax)
            return "wall_left";
        if (x == xMax && y > yMin && y < yMax)
            return "wall_right";
        if (x > xMin && x < xMax && y == yMin)
            return "wall_bottom";
        return "floor";
    }

}
