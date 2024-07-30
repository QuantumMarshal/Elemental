using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileVisualizer : MonoBehaviour
{
    public Tilemap floorTileMap;

    public TileBase tileRules;

    public void PaintRoom(Vector2 center, Vector2 size)
    {
        int startX = (int)center.x - (int)(size.x / 2);
        int startY = (int)center.y - (int)(size.y / 2);
        int endX = (int)center.x + (int)(size.x / 2);
        int endY = (int)center.y + (int)(size.y / 2);

        Vector3Int startPos = new Vector3Int(startX, startY, 0);
        Vector3Int endPos = new Vector3Int(endX, endY, 0);

        BoxFill(floorTileMap, tileRules, startPos, endPos);
    }

    public void PaintCorridor(Edge connection)
    {
        Vector3Int startPos = Vector3Int.RoundToInt(connection.V0);
        Vector3Int endPos = Vector3Int.RoundToInt(connection.V1);

        Vector2Int direction = Vector2Int.RoundToInt(connection.V0 - connection.V1);

        if (direction.x != 0)
        {
            startPos.y += 2;
            endPos.y -= 2;

            BoxFill(floorTileMap, tileRules, startPos, endPos);
            BoxFill(floorTileMap, tileRules, startPos, endPos);
        }
        else if (direction.y != 0)
        {
            startPos.x += 2;
            endPos.x -= 2;

            BoxFill(floorTileMap, tileRules, startPos, endPos);
        }
    }

    public void Paint9x9(Vector2 vertex)
    {
        Vector3Int startPos = Vector3Int.RoundToInt(new Vector3(vertex.x - 1, vertex.y - 1, 0));
        Vector3Int endPos = Vector3Int.RoundToInt(new Vector3(vertex.x + 1, vertex.y + 1, 0));

        BoxFill(floorTileMap, tileRules, startPos, endPos);
    }

    private void BoxFill(Tilemap map, TileBase tile, Vector3Int start, Vector3Int end)
    {
        //Determine directions on X and Y axis
        var xDir = start.x < end.x ? 1 : -1;
        var yDir = start.y < end.y ? 1 : -1;
        //How many tiles on each axis?
        int xCols = 1 + Mathf.Abs(start.x - end.x);
        int yCols = 1 + Mathf.Abs(start.y - end.y);
        //Start painting
        for (var x = 0; x < xCols; x++)
        {
            for (var y = 0; y < yCols; y++)
            {
                var tilePos = start + new Vector3Int(x * xDir, y * yDir, 0);
                map.SetTile(tilePos, tile);
            }
        }
    }
}
