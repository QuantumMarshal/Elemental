using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Edge
{
    public Vector2 V0 { get; set; }
    public Vector2 V1 { get; set; }

    public Edge(Vector2 V0, Vector2 V1)
    {
        this.V0 = V0;
        this.V1 = V1;
    }

    public bool Compare(Edge edge)
    {
        return (V0 == edge.V0 && V1 == edge.V1) || (V0 == edge.V1 && V1 == edge.V0);
    }
}

public class Triangle
{
    public Vector2 V0 { get; set; }
    public Vector2 V1 { get; set; }
    public Vector2 V2 { get; set; }

    public List<Edge> edges { get; set; }

    public Triangle(Vector2 V0, Vector2 V1, Vector2 V2)
    {
        this.V0 = V0;
        this.V1 = V1;
        this.V2 = V2;

        this.edges = new List<Edge>();
        this.edges.Add(new Edge(V0, V1));
        this.edges.Add(new Edge(V1, V2));
        this.edges.Add(new Edge(V2, V0));
    }
    
    public bool ContainEdge(Edge edge)
    {
        foreach (var e in edges)
        {
            if (e.Compare(edge))
            {
                return true;
            }
        }

        return false;
    }

    public Vector2 GetCenterRadius()
    {
        float centerX = 0, centerY = 0;

        float x1 = V0.x, y1 = V0.y;
        float x2 = V1.x, y2 = V1.y;
        float x3 = V2.x, y3 = V2.y;

        float J = 2 * (x1 - x2) * (y1 - y3) - 2 * (x1 - x3) * (y1 - y2);

        centerX = -(y1 - y2) * (x1 * x1 + y1 * y1 - x3 * x3 - y3 * y3) + (y1 - y3) * (x1 * x1 + y1 * y1 - x2 * x2 - y2 * y2);
        centerX /= J;

        centerY = (x1 - x2) * (x1 * x1 + y1 * y1 - x3 * x3 - y3 * y3) - (x1 - x3) * (x1 * x1 + y1 * y1 - x2 * x2 - y2 * y2);
        centerY /= J;


        Vector2 circumcenter = new Vector2(centerX, centerY);

        return circumcenter;
    }

    public bool IsInCircumCircle(Vector2 point)
    {
        Vector2 center = GetCenterRadius();

        return (Vector2.Distance(point, center) <= Vector2.Distance(center, V0));
    }

    public override string ToString()
    {
        string msg = $"V0: {V0}; V1: {V1}; V2: {V2}";

        return msg;
    }
}

public static class DelaunayTriangulation 
{
    public static Triangle GetSuperTriangle(List<Vector2> points)
    {
        float minX = float.MaxValue, minY = float.MaxValue;
        float maxX = float.MinValue, maxY = float.MinValue;

        foreach (var point in points)
        {
            if (point.x < minX) minX = point.x;
            if (point.y < minY) minY = point.y;
            if (point.x > maxX) maxX = point.x;
            if (point.y > maxY) maxY = point.y;
        }

        float dx = (maxX - minX) * 10;
        float dy = (maxY - minY) * 10;

        Vector2 V0 = new Vector2(minX - dx, minY - dy * 3);
        Vector2 V1 = new Vector2(minX - dx, maxY + dy);
        Vector2 V2 = new Vector2(maxX + dx * 3, maxY + dy);

        Triangle tri = new Triangle(V0, V1, V2);

        return tri;
    }

    public static List<Triangle> AddVertex (Vector2 point, List<Triangle> oldTris)
    {
        List<Edge> polygon = new List<Edge>();
        List<Triangle> badTriangles = new List<Triangle>();
        List<Triangle> triangles = oldTris;

        triangles = triangles.Where(triangle =>
        {
            if (triangle.IsInCircumCircle(point))
            {
                badTriangles.Add(triangle);
                return false;
            }

            return true;
        }).ToList();

        foreach (var triangle in badTriangles)
        {
            foreach (var edge in triangle.edges)
            {
                bool isShare = false;
                foreach (var other in badTriangles)
                {
                    if (other != triangle && other.ContainEdge(edge))
                    {
                        isShare = true;
                        break;
                    }
                }

                if (!isShare) polygon.Add(edge);
            }
        }

        polygon = UniqueEdges(polygon);

        foreach (var edge in polygon)
        {
            triangles.Add(new Triangle(edge.V0, edge.V1, point));
        }

        return triangles;
    }

    private static List<Edge> UniqueEdges(List<Edge> edges)
    {
        HashSet<Edge> result = new HashSet<Edge>(edges);
        List<Edge> uniqueEdges = new List<Edge>(result);

        return uniqueEdges;
    }
}
