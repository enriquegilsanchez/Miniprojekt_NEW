using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.TerrainTools;
using UnityEngine.Tilemaps;

public class RoomGenerator : MonoBehaviour
{
    [SerializeField]
    private Tilemap floorTilemap;

    [SerializeField]
    private Tilemap wallTilemap;

    [SerializeField]
    private TileBase c1Tile;

    [SerializeField]
    private TileBase c2Tile;

    [SerializeField]
    private TileBase c3Tile;

    [SerializeField]
    private TileBase c4Tile;

    [SerializeField]
    private TileBase floorC1Tile;

    [SerializeField]
    private TileBase floorC2Tile;

    [SerializeField]
    private TileBase floorC3Tile;

    [SerializeField]
    private TileBase floorC4Tile;

    [SerializeField]
    private TileBase floorLeftTile;

    [SerializeField]
    private TileBase floorRightTile;

    [SerializeField]
    private TileBase floorTopTile;

    [SerializeField]
    private TileBase floorBottomTile;

    [SerializeField]
    private TileBase floorMiddleTile;

    [SerializeField]
    private TileBase wallC1Tile;

    [SerializeField]
    private TileBase wallC2Tile;

    [SerializeField]
    private TileBase wallC3Tile;

    [SerializeField]
    private TileBase wallC4Tile;

    [SerializeField]
    private TileBase wallMiddleUpperTile;

    [SerializeField]
    private TileBase wallMiddleLowerTile;

    [SerializeField]
    private TileBase wallTopTile;

    [SerializeField]
    private TileBase wallLeftTile;

    [SerializeField]
    private TileBase wallRightTile;

    [SerializeField]
    private TileBase wallBottonTile;

    /// <summary>
    /// Generates a Grid of DungeonTiles for the given room rect and paints all room tiles
    /// </summary>
    /// <param name="room">Room to be Converted to Dungeon Tiles</param>
    /// <returns>A Grid of dungeon tiles</returns>
    internal DungeonTile[,] RoomToDungeonTiles(Rect room)
    {
        int xMin = (int)room.xMin;
        int xMax = (int)room.xMax;
        int yMin = (int)room.yMin;
        int yMax = (int)room.yMax;
        DungeonTile[,] tiles = new DungeonTile[(xMax - xMin), (yMax - yMin)];
        int[] position = { 0, 0 };
        string type = "";
        int arrx = 0;
        int arry = 0;
        for (var i = yMax - 1; i >= yMin; i--)
        {
            for (var j = xMin; j < xMax; j++)
            {
                position[0] = j;
                position[1] = i;
                type = DetermineTileType(j, i, xMin, xMax, yMin, yMax);
                tiles[arrx, arry] = new DungeonTile(position, type);
                arrx++;
            }
            arry++;
            arrx = 0;
        }
        return tiles;
    }

    // private void PaintFloorTiles(IEnumerable<Vector2Int> floorPositions)
    // {
    //     PaintTiles(floorPositions, floorTilemap, floorTile);
    // }

    // private void PaintTiles(IEnumerable<Vector2Int> positions, Tilemap tilemap, TileBase tile)
    // {
    //     foreach (var position in positions)
    //     {
    //         PaintSingleTile(tilemap, tile, position);
    //     }
    // }

    /// <summary>
    /// Paints a single tile at the given position on the specified tilemap
    /// </summary>
    /// <param name="tilemap"></param>
    /// <param name="tile"></param>
    /// <param name="position"></param>
    private void PaintSingleTile(Tilemap tilemap, TileBase tile, Vector2Int position)
    {
        var tilePosition = tilemap.WorldToCell((Vector3Int)position);
        tilemap.SetTile(tilePosition, tile);
    }

    /// <summary>
    /// Determines the tile Type and paints the correct Tile immediately
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="xMin"></param>
    /// <param name="xMax"></param>
    /// <param name="yMin"></param>
    /// <param name="yMax"></param>
    /// <returns></returns>
    private string DetermineTileType(int x, int y, int xMin, int xMax, int yMin, int yMax)
    {
        // ------------------ Room Corner Tiles ----------------------------
        if (x == xMin && y == yMax - 1)
        {
            PaintSingleTile(wallTilemap, c4Tile, new Vector2Int(x, y));
            return "c4";
        }
        if (x == xMax - 1 && y == yMax - 1)
        {
            PaintSingleTile(wallTilemap, c3Tile, new Vector2Int(x, y));
            return "c3";
        }
        if (x == xMax - 1 && y == yMin)
        {
            PaintSingleTile(wallTilemap, c2Tile, new Vector2Int(x, y));
            return "c2";
        }
        if (x == xMin && y == yMin)
        {
            PaintSingleTile(wallTilemap, c1Tile, new Vector2Int(x, y));
            return "c1";
        }

        // ---------------------- Upper Wall Tiles --------------------------

        // Corners
        if (x == xMin + 1 && y == yMax - 2)
        {
            PaintSingleTile(wallTilemap, wallC4Tile, new Vector2Int(x, y));
            return "wall_c4";
        }
        if (x == xMax - 2 && y == yMax - 2)
        {
            PaintSingleTile(wallTilemap, wallC3Tile, new Vector2Int(x, y));
            return "wall_c3";
        }
        if (x == xMax - 2 && y == yMax - 3)
        {
            PaintSingleTile(wallTilemap, wallC2Tile, new Vector2Int(x, y));
            return "wall_c2";
        }
        if (x == xMin + 1 && y == yMax - 3)
        {
            PaintSingleTile(wallTilemap, wallC1Tile, new Vector2Int(x, y));
            return "wall_c1";
        }

        // Upper and Lower Infill
        if (x > xMin + 1 && x < xMax - 2 && y == yMax - 2)
        {
            PaintSingleTile(wallTilemap, wallMiddleUpperTile, new Vector2Int(x, y));
            return "wall_middle_upper";
        }
        if (x > xMin + 1 && x < xMax - 2 && y == yMax - 3)
        {
            PaintSingleTile(wallTilemap, wallMiddleLowerTile, new Vector2Int(x, y));
            return "wall_middle_lower";
        }

        // ----------------------- Surrounding wall tiles --------------------------
        if (x == xMin && y > yMin && y < yMax - 1)
        {
            PaintSingleTile(wallTilemap, wallLeftTile, new Vector2Int(x, y));
            return "wall_left";
        }
        if (x == xMax - 1 && y > yMin && y < yMax - 1)
        {
            PaintSingleTile(wallTilemap, wallRightTile, new Vector2Int(x, y));
            return "wall_right";
        }
        if (x > xMin && x < xMax - 1 && y == yMax - 1)
        {
            PaintSingleTile(wallTilemap, wallTopTile, new Vector2Int(x, y));
            return "wall_top";
        }
        if (x > xMin && x < xMax - 1 && y == yMin)
        {
            PaintSingleTile(wallTilemap, wallBottonTile, new Vector2Int(x, y));
            return "wall_bottom";
        }

        // -------------------- Floor Tiles ---------------------------

        // Corners
        if (x == xMin + 1 && y == yMax - 4)
        {
            PaintSingleTile(floorTilemap, floorC4Tile, new Vector2Int(x, y));
            return "floor_c4";
        }
        if (x == xMax - 2 && y == yMax - 4)
        {
            PaintSingleTile(floorTilemap, floorC3Tile, new Vector2Int(x, y));
            return "floor_c3";
        }
        if (x == xMax - 2 && y == yMin + 1)
        {
            PaintSingleTile(floorTilemap, floorC2Tile, new Vector2Int(x, y));
            return "floor_c2";
        }
        if (x == xMin + 1 && y == yMin + 1)
        {
            PaintSingleTile(floorTilemap, floorC1Tile, new Vector2Int(x, y));
            return "floor_c1";
        }

        // Surrounding Floor Tiles
        if (x > xMin + 1 && x < xMax - 2 && y == yMax - 4)
        {
            PaintSingleTile(floorTilemap, floorTopTile, new Vector2Int(x, y));
            return "floor_top";
        }
        if (x > xMin + 1 && x < xMax - 2 && y == yMin + 1)
        {
            PaintSingleTile(floorTilemap, floorBottomTile, new Vector2Int(x, y));
            return "floor_bottom";
        }
        if (x == xMin + 1 && y > yMin + 1 && y < yMax - 4)
        {
            PaintSingleTile(floorTilemap, floorLeftTile, new Vector2Int(x, y));
            return "floor_left";
        }
        if (x == xMax - 2 && y > yMin + 1 && y < yMax - 4)
        {
            PaintSingleTile(floorTilemap, floorRightTile, new Vector2Int(x, y));
            return "floor_right";
        }

        // Middle Infill
        PaintSingleTile(floorTilemap, floorMiddleTile, new Vector2Int(x, y));
        return "floor_middle";
    }
}
