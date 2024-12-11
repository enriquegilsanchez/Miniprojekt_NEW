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
        DungeonTile[,] tiles = new DungeonTile[(xMax - xMin), (yMax - yMin)];
        Debug.Log(xMax - xMin + " " + (yMax - yMin));
        int[] position = { 0, 0 };
        string type = "";
        int arrx = 0;
        int arry = 0;
        Debug.Log("xMax = " + xMax + "yMax = " + yMax);

        for (var i = yMax; i >= yMin; i--)
        {
            for (var j = xMin; j <= xMax; j++)
            {
                position[0] = j;
                position[1] = i;
                type = DetermineRoomType(j, i, xMin, xMax, yMin, yMax);
                tiles[arrx, arry] = new DungeonTile(position, type);
                Debug.Log("x = " + j + " y = " + i + "arr vars = " + arrx + " " + arry);
                arrx++;
            }
            arry++;
            arrx = 0;
        }
        return tiles;
    }

    public static string DetermineRoomType(int x, int y, int xMin, int xMax, int yMin, int yMax)
    {
        if (x == xMin && y == yMax)
        {
            return "c4";
        }
        if (x == xMax && y == yMax)
        {
            return "c3";
        }
        if (x == xMax && y == yMin)
        {
            return "c2";
        }
        if (x == xMin && y == yMin)
        {
            return "c1";
        }
        if (x > xMin && x < xMax && y >= yMax - 1)
        {
            return "wall_top";
        }
        if (x == xMin && y > yMin && y < yMax)
        {
            return "wall_left";
        }
        if (x == xMax && y > yMin && y < yMax)
        {
            return "wall_right";
        }
        if (x > xMin && x < xMax && y == yMin)
        {
            return "wall_bottom";
        }

        return "floor";
    }

}
