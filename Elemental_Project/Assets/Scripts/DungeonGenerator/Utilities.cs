using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public static class Utilities
{
    public static Vector2 GetRandomPointInEllipse(float ellipse_width, float ellipse_height)
    {
        float t = 2 * Mathf.PI * Random.value;
        float u = Random.Range(0, 1f) + Random.Range(0, 1f);
        float r = 0;

        r = (u > 1) ? 2 - u : u;

        float x = RoundM(ellipse_width * r * Mathf.Cos(t) / 2, 1); 
        float y = RoundM(ellipse_height * r * Mathf.Sin(t) / 2, 1);

        return new Vector2(x, y);

    }

    private static float RoundM(float n, float m)
    {
        return Mathf.Floor(((n + m - 1) / m)) * m;
    }

    public static bool AreRoomsOverlapping(Room a, Room b)
    {
        return (Mathf.Abs(a.centerPos.x - b.centerPos.x) < (Mathf.Round(a.size.x / 2) + Mathf.Round(b.size.x / 2) + 3)) &&
               (Mathf.Abs(a.centerPos.y - b.centerPos.y) < (Mathf.Round(a.size.y / 2) + Mathf.Round(b.size.y / 2) + 3));
    }

    public static Vector2 RoundVector2(Vector2 vct)
    {
        return new Vector2(Mathf.Round(vct.x), Mathf.Round(vct.y));
    }

    public static float GetMeanRoom(List<Room> rooms)
    {
        float mean = 0;
        foreach (Room room in rooms)
        {
            mean += (room.size.x + room.size.y) / 2;
        }

        mean = mean / rooms.Count;

        return Mathf.Round(mean);
    }

    public static List<Room> GetMainRooms(List<Room> rooms)
    {
        return rooms.FindAll(room => room.isMainRoom);
    }

    public static List<Edge> MinimumSpanningTree(List<Edge> edges)
    {
        List<Edge> mst = new List<Edge>();
        Dictionary<Vector2, Vector2> parent = new Dictionary<Vector2, Vector2>();
        foreach (var edge in edges)
        {
            parent[edge.V0] = edge.V0;
            parent[edge.V1] = edge.V1;
        }

        edges.Sort((a, b) =>
        {
            float distA = Vector2.Distance(a.V0, a.V1);
            float distB = Vector2.Distance(b.V0, b.V1);
            return distA.CompareTo(distB);
        });

        foreach (var edge in edges)
        {
            Vector2 root1 = Find(parent, edge.V0);
            Vector2 root2 = Find(parent, edge.V1);

            if (root1 != root2)
            {
                mst.Add(edge);
                parent[root1] = root2;
            }
        }

        return mst;
    }

    public static Vector2 Find(Dictionary<Vector2, Vector2> parent, Vector2 vertex)
    {
        if (parent[vertex] == vertex)
        {
            return vertex;
        }
        return Find(parent, parent[vertex]);
    }

    public static List<Edge> GetEdgeFromTriangles(List<Triangle> triangles)
    {
        List<Edge> edges = new List<Edge>();
        foreach (var triangle in triangles)
        {
            foreach (var edge in triangle.edges)
            {
                edges.Add(edge);
            }
        }
        return edges;
    }

}
