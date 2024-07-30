using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System.Linq;
using UnityEngine.Tilemaps;

public class Room
{
    public Vector2 centerPos { get; set; }
    public Vector2 size { get; set; }
    public bool isMainRoom { get; set; }


    public Room(Vector2 centerPos, Vector2 size, bool isMainRoom = false)
    {
        this.centerPos = centerPos;
        this.size = size;
        this.isMainRoom = isMainRoom;
    }

    public bool IsPointInRoom(Vector2 point)
    {
        Vector2 roomMin = centerPos - (size / 2);
        Vector2 roomMax = centerPos + (size / 2);

        return (point.x >= roomMin.x && point.x <= roomMax.x && point.y >= roomMin.y && point.y <= roomMax.y);
    }
}


public class ProceduralGenerator : MonoBehaviour
{
    [SerializeField] private int numberOfRooms = 10;
    [SerializeField] private Vector2 ellipse;
    [SerializeField] private Vector2 randomWidth;
    [SerializeField] private Vector2 randomHeight;

    [SerializeField] private float meanMultiplier;
    [SerializeField] private float randomAdd;

    [SerializeField] private Grid grid;
    [SerializeField] private TileVisualizer tileVisualizer;

    bool isFinish = false;

    bool OnGizmos = false;
    bool OnGizmos2 = false;
    bool OnGizmos3 = false;

    private List<Room> rooms = new List<Room>();
    private List<Room> mainRooms = new List<Room>();    
    private List<Triangle> triangles = new List<Triangle>();    
    private List<Edge> connections = new List<Edge>();
    public List<Edge> hallways = new List<Edge>();
    public List<Room> finalRooms = new List<Room>();

    public void Generate()
    {
        //StartCoroutine(Cor_SpawnRoom(numberOfRooms));
        SpawnRoom(numberOfRooms);
        SeparateRoom(rooms);
        GetMainRooms(rooms);
        Triangulate();
        MinimumSpanningTree();
        CreateHallways();
    }

    [Button]
    private void ToggleGizmos()
    {
        OnGizmos = !OnGizmos;
    }

    [Button]
    private void ToggleGizmos2()
    {
        OnGizmos2 = !OnGizmos2;
    }

    [Button]
    private void ToggleGizmos3()
    {
        OnGizmos3 = !OnGizmos3;
    }

    private void SpawnRoom(int numberOfRooms)
    {
        for (int i = 0; i < numberOfRooms; i++)
        {
            Vector2 centerPos = Utilities.GetRandomPointInEllipse(ellipse.x, ellipse.y);

            Vector2 size = new Vector2((int)Random.Range(randomWidth.x, randomWidth.y), (int)Random.Range(randomHeight.x, randomHeight.y));

            if (size.x % 2 != 0) size.x--;
            if (size.y % 2 != 0) size.y--;

            Room room = new Room(centerPos, size);
            rooms.Add(room);
        }
    }

    private void SeparateRoom(List<Room> rooms)
    {
        bool overlapping = true;
        int iteration = 0;
        int maxIteration = 1000;
        while (overlapping && iteration < maxIteration)
        {
            overlapping = false;
            foreach (var roomA in rooms)
            {
                foreach (var roomB in rooms)
                {
                    if (roomA != roomB && Utilities.AreRoomsOverlapping(roomA, roomB))
                    {
                        Vector2 push = roomA.centerPos - roomB.centerPos;
                        if (push == Vector2.zero)
                        {
                            push = Random.insideUnitCircle.normalized;
                        }
                        push.Normalize();
                        push = Utilities.RoundVector2(push);

                        roomA.centerPos += push;
                        roomB.centerPos -= push;

                        overlapping = true;
                    }
                }
            }
            iteration++;
        }
    }

    private void GetMainRooms(List<Room> rooms)
    {
        float mainRoomSize = meanMultiplier * Utilities.GetMeanRoom(rooms);

        for (int i = 0; i < numberOfRooms; i++)
        {
            float size = (rooms[i].size.x + rooms[i].size.y) / 2;

            if (size >= mainRoomSize) rooms[i].isMainRoom = true;
        }

        mainRooms = Utilities.GetMainRooms(rooms);
    }

    private void Triangulate()
    {
        List<Vector2> points = new List<Vector2>();

        foreach (var room in mainRooms)
        {
            points.Add(room.centerPos);
        }

        Triangle superTriangle = DelaunayTriangulation.GetSuperTriangle(points);

        triangles.Add(superTriangle);

        foreach (var point in points)
        {
            triangles = DelaunayTriangulation.AddVertex(point, triangles);
        }

        triangles = triangles.Where(triangle =>
        {
            if (triangle.V0 == superTriangle.V0 || triangle.V0 == superTriangle.V1 || triangle.V0 == superTriangle.V2 ||
              triangle.V1 == superTriangle.V0 || triangle.V1 == superTriangle.V1 || triangle.V1 == superTriangle.V2 ||
              triangle.V2 == superTriangle.V0 || triangle.V2 == superTriangle.V1 || triangle.V2 == superTriangle.V2) return false;
            return true;
        }).ToList();
    }

    private void MinimumSpanningTree()
    {
        connections = Utilities.MinimumSpanningTree(Utilities.GetEdgeFromTriangles(triangles));

        foreach (var triangle in triangles)
        {
            foreach (var edge in triangle.edges)
            {
                if (!connections.Contains(edge))
                {
                    int rand = Random.Range(0, 100);

                    if (rand <= randomAdd) connections.Add(edge);
                }
            }
        }

    }

    private void CreateHallways()
    {
        foreach (var connection in connections)
        {
            CreateHallway(connection);
            isFinish = true;
        }

        if (isFinish) DrawFloor();
    }

    private IEnumerator Cor_SpawnRoom(int numberOfRooms)
    {
        for (int i = 0; i < numberOfRooms; i++)
        {
            Vector2 centerPos = Utilities.GetRandomPointInEllipse(ellipse.x, ellipse.y);

            Vector2 size = new Vector2((int)Random.Range(randomWidth.x, randomWidth.y), (int)Random.Range(randomHeight.x, randomHeight.y));

            if (size.x % 2 != 0) size.x--;
            if (size.y % 2 != 0) size.y--;

            Room room = new Room(centerPos, size);
            rooms.Add(room);
        }

        yield return StartCoroutine(Cor_SeparateRoom(rooms));        
    }

    private IEnumerator Cor_SeparateRoom(List<Room> rooms)
    {
        bool overlapping = true;
        int iteration = 0;
        int maxIteration = 1000;
        while (overlapping && iteration < maxIteration)
        {
            overlapping = false;
            foreach (var roomA in rooms)
            {
                foreach (var roomB in rooms)
                {
                    if (roomA != roomB && Utilities.AreRoomsOverlapping(roomA, roomB))
                    {
                        Vector2 push = roomA.centerPos - roomB.centerPos;
                        if (push == Vector2.zero)
                        {
                            push = Random.insideUnitCircle.normalized;
                        }
                        push.Normalize();
                        push = Utilities.RoundVector2(push);

                        roomA.centerPos += push;
                        roomB.centerPos -= push;

                        overlapping = true;
                    }
                }  
            }
            iteration++;
            yield return null;
        }

        yield return StartCoroutine(Cor_GetMainRooms(rooms));
    }

    private IEnumerator Cor_GetMainRooms(List<Room> rooms)
    {
        float mainRoomSize = meanMultiplier * Utilities.GetMeanRoom(rooms);

        for (int i = 0; i < numberOfRooms; i++)
        {
            float size = (rooms[i].size.x + rooms[i].size.y) / 2;

            if (size >= mainRoomSize) rooms[i].isMainRoom = true;
        }

        mainRooms = Utilities.GetMainRooms(rooms);

        yield return StartCoroutine(Cor_DelaunayTriangulation());
    }

    private IEnumerator Cor_DelaunayTriangulation()
    {
        List<Vector2> points = new List<Vector2>();

        foreach (var room in mainRooms)
        {
            points.Add(room.centerPos);
        }

        Triangle superTriangle = DelaunayTriangulation.GetSuperTriangle(points);

        triangles.Add(superTriangle);

        foreach (var point in points)
        {
            triangles = DelaunayTriangulation.AddVertex(point, triangles);
            yield return new WaitForSeconds(0.1f);
        }

        triangles = triangles.Where(triangle =>
        {
            if (triangle.V0 == superTriangle.V0 || triangle.V0 == superTriangle.V1 || triangle.V0 == superTriangle.V2 ||
              triangle.V1 == superTriangle.V0 || triangle.V1 == superTriangle.V1 || triangle.V1 == superTriangle.V2 ||
              triangle.V2 == superTriangle.V0 || triangle.V2 == superTriangle.V1 || triangle.V2 == superTriangle.V2) return false;
            return true;
        }).ToList();

        yield return StartCoroutine(Cor_MinimumSpanningTree());
    }

    private IEnumerator Cor_MinimumSpanningTree()
    {
        connections = Utilities.MinimumSpanningTree(Utilities.GetEdgeFromTriangles(triangles));
        yield return new WaitForSeconds(1f);

        foreach (var triangle in triangles)
        {
            foreach (var edge in triangle.edges)
            {
                if (!connections.Contains(edge))
                {
                    int rand = Random.Range(0, 100);

                    if (rand <= randomAdd) connections.Add(edge);
                }
            }
        }

        yield return null;
    }

    private IEnumerator Cor_CreateHallways()
    {
        foreach (var connection in connections)
        {
            CreateHallway(connection);
            DrawHallway(connection.V0, connection.V1);
            yield return new WaitForSeconds(0.1f);

            isFinish = true;
        }
        // if (isFinish) DrawFloor();
    }

    private void CreateHallway(Edge connection)
    {
        Vector2 startPoint = connection.V0;
        Vector2 endPoint = connection.V1;

        DrawHallway(startPoint, endPoint);
    }

    private void OnDrawGizmos()
    {
        if (!OnGizmos) return;

        if (rooms.Count <= 0) return;
        Gizmos.color = Color.green;
        foreach (var room in rooms)
        {
            Gizmos.DrawWireCube((Vector3)room.centerPos, (Vector3)room.size);
        }

        if (!OnGizmos2) return;
        Gizmos.color = Color.red;
        foreach (var room in mainRooms)
        {
            Gizmos.DrawWireCube((Vector3)room.centerPos, (Vector3)room.size);
        }

        if (!OnGizmos3) return;

        if (finalRooms.Count <= 0) return;
        Gizmos.color = Color.red;
        foreach (var room in finalRooms)
        {
            Gizmos.DrawWireCube((Vector3)room.centerPos, (Vector3)room.size);
        }

        if (hallways.Count <= 0) return;
        Gizmos.color = Color.white;
        foreach (var edge in hallways)
        {
            Gizmos.DrawLine(edge.V0, edge.V1);
        }
    }

    private void DrawFloor()
    {
        foreach (var room in finalRooms)
        {
            tileVisualizer.PaintRoom(room.centerPos, room.size);
        }

        foreach (var connection in hallways)
        {
            tileVisualizer.PaintCorridor(connection);
        }
    }

    private void DrawHallway(Vector2 startPoint, Vector2 endPoint)
    {
        float deltaX = Mathf.Abs(startPoint.x - endPoint.x);
        float deltaY = Mathf.Abs(startPoint.y - endPoint.y);

        Room roomA = GetRoomByCenterPosition(startPoint);
        Room roomB = GetRoomByCenterPosition(endPoint);

        if (deltaX <= 0)
        {
            ProceduralHallways(startPoint, endPoint, roomA);
        }
        else if (deltaY <= 0)
        {
            ProceduralHallways(startPoint, endPoint, roomA);
        }
        else
        {
            Vector2 lPoint = new Vector2(startPoint.x, endPoint.y);
            tileVisualizer.Paint9x9(lPoint);
            ProceduralHallways(startPoint, lPoint, roomA);
            ProceduralHallways(endPoint, lPoint, roomB);
        }
    }

    private void ProceduralHallways(Vector2 startPos, Vector2 endPos, Room targetRoom)
    {
        if (!finalRooms.Contains(targetRoom)) finalRooms.Add(targetRoom);

        if (targetRoom.IsPointInRoom(endPos)) return;

        Vector2 direction = endPos - startPos;
        direction = direction.normalized;

        Vector2 currentPos = GetPointInEdgeRoomByDirection(startPos, direction, Vector2.zero, targetRoom);
        Vector2 prevPos = currentPos;

        Room endRoom = null;

        if (IsPointInRooms(rooms, endPos))
        {
            endRoom = GetRoom(rooms, endPos);
        }

        while (currentPos != endPos)
        {
            currentPos += direction;

            if (IsPointInRooms(rooms, currentPos))
            {
                Room room = GetRoom(rooms, currentPos);
                hallways.Add(new Edge(prevPos, currentPos));

                if (!finalRooms.Contains(room)) finalRooms.Add(room);

                if (endRoom != null && room.centerPos == endRoom.centerPos) break;


                Vector2 newDir = room.centerPos - currentPos;

                currentPos = GetPointInEdgeRoomByDirection(room.centerPos, direction, newDir, room);
            }

            else
            {
                hallways.Add(new Edge(prevPos, currentPos));
            }

            prevPos = currentPos;
        }
    }

    private Vector2 GetPointInEdgeRoomByDirection(Vector2 centerPos, Vector2 direction, Vector2 newDirection, Room targetRoom)
    {
        Vector2 res = centerPos;

        if (direction == Vector2.up)
        {
            res.y += targetRoom.size.y / 2;
            res.x -= newDirection.x;
        }

        else if (direction == Vector2.down)
        {
            res.y -= targetRoom.size.y / 2;
            res.x -= newDirection.x;
        }

        else if (direction == Vector2.left)
        {
            res.x -= targetRoom.size.x / 2;
            res.y -= newDirection.y;
        }

        else if (direction == Vector2.right)
        {
            res.x += targetRoom.size.x / 2;
            res.y -= newDirection.y;
        }

        return res;
    }

    private bool VectorGreater(Vector2 left, Vector2 right)
    {
        return (left.x >= right.x) && (left.y >= right.y);
    }

    private bool IsPointInRooms(List<Room> rooms, Vector2 point)
    {
        foreach (var room in rooms)
        {
            if (room.IsPointInRoom(point))
            {
                return true;
            }
        }

        return false;
    }

    private Room GetRoomByCenterPosition(Vector2 centerPos)
    {
        foreach (var room in mainRooms)
        {
            if (room.centerPos == centerPos) return room;
        }

        return null;
    }

    private Room GetRoom(List<Room> rooms, Vector2 pos)
    {
        foreach (var room in rooms)
        {
            if (room.IsPointInRoom(pos))
            {
                return room;
            }
        }

        return null;
    }

    //private List<Vector2Int> GetFloorPos(Vector2 center, Vector2 size)
    //{
    //    int startX = (int)(center.x - size.x / 2);
    //    int startY = (int)(center.y - size.y / 2);
    //    int endX = (int)(center.x + size.x / 2);
    //    int endY = (int)(center.y + size.y / 2) - 1;

    //    List<Vector2Int> floorPos = new List<Vector2Int>();

    //    for (int x = startX; x <= endX; x++)
    //    {
    //        for (int y = startY; y <= endY; y++)
    //        {
    //            floorPos.Add(new Vector2Int(x, y));
    //        }
    //    }

    //    return floorPos;
    //}

    //private List<Vector2Int> GetWallPos(Vector2 center, Vector2 size)
    //{
    //    List<Vector2Int> positions = new List<Vector2Int>();

    //    // Convert the center and size to integer coordinates
    //    Vector2Int intCenter = new Vector2Int(Mathf.RoundToInt(center.x), Mathf.RoundToInt(center.y));
    //    Vector2Int intSize = new Vector2Int(Mathf.RoundToInt(size.x), Mathf.RoundToInt(size.y));

    //    int halfWidth = intSize.x / 2;
    //    int halfHeight = intSize.y / 2;

    //    // Top wall (1 height)
    //    for (int x = intCenter.x - halfWidth; x <= intCenter.x + halfWidth; x++)
    //    {
    //        for (int y = intCenter.y + halfHeight - 3; y <= intCenter.y + halfHeight - 1; y++)
    //        {
    //            positions.Add(new Vector2Int(x, y));
    //        }
    //    }

    //    // Bottom wall (1 height)
    //    for (int x = intCenter.x - halfWidth; x <= intCenter.x + halfWidth; x++)
    //    {
    //        int y = intCenter.y - halfHeight;
    //        positions.Add(new Vector2Int(x, y));
    //    }

    //    // Left wall (1 width)
    //    for (int y = intCenter.y - halfHeight + 1; y <= intCenter.y + halfHeight - 1; y++)
    //    {
    //        int x = intCenter.x - halfWidth;
    //        positions.Add(new Vector2Int(x, y));
    //    }

    //    // Right wall (1 width)
    //    for (int y = intCenter.y - halfHeight + 1; y <= intCenter.y + halfHeight - 1; y++)
    //    {
    //        int x = intCenter.x + halfWidth;
    //        positions.Add(new Vector2Int(x, y));
    //    }

    //    return positions;
    //}






}
