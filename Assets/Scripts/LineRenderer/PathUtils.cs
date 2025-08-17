using System.Collections.Generic;
using UnityEngine;

public static class PathUtils
{
    // Smooth corners by inserting a short arc around each interior vertex.
    // cornerDistance: how far to pull back along each side (world units), e.g., 0.2
    // arcSamples: how many points along each arc (2–8), e.g., 4
    public static List<Vector3> RoundCorners(Vector3[] path, float cornerDistance = 0.2f, int arcSamples = 4)
    {
        var pts = path;
        if (pts == null || pts.Length < 3) return new List<Vector3>(pts ?? new Vector3[0]);

        var result = new List<Vector3>(pts.Length + pts.Length * (arcSamples - 1));

        // Keep first point
        result.Add(pts[0]);

        for (int i = 1; i < pts.Length - 1; i++)
        {
            Vector3 A = pts[i - 1];
            Vector3 B = pts[i];
            Vector3 C = pts[i + 1];

            // Directions on XZ
            Vector3 AB = new Vector3(B.x - A.x, 0, B.z - A.z);
            Vector3 CB = new Vector3(B.x - C.x, 0, B.z - C.z);
            float lenAB = AB.magnitude;
            float lenCB = CB.magnitude;

            // If any side is too short, just keep B
            if (lenAB < 1e-4f || lenCB < 1e-4f)
            {
                result.Add(B);
                continue;
            }

            Vector3 dirBA = -AB / lenAB; // from B toward A
            Vector3 dirBC = (C - B); dirBC = new Vector3(dirBC.x, 0, dirBC.z);
            dirBC.Normalize();           // from B toward C

            // Pull-back distance along each side (clamp so we don't overshoot)
            float d = Mathf.Min(cornerDistance, 0.49f * Mathf.Min(lenAB, lenCB));

            // Tangent points along each segment
            Vector3 T1 = new Vector3(B.x + dirBA.x * d, B.y + (A.y - B.y) * (d / lenAB), B.z + dirBA.z * d);
            Vector3 T2 = new Vector3(B.x + dirBC.x * d, B.y + (C.y - B.y) * (d / lenCB), B.z + dirBC.z * d);

            // If the corner is nearly straight, just keep B
            Vector3 v1 = (A - B); v1.y = 0; v1.Normalize();
            Vector3 v2 = (C - B); v2.y = 0; v2.Normalize();
            float dot = Mathf.Clamp(Vector3.Dot(v1, v2), -1f, 1f);
            float angle = Mathf.Acos(dot); // radians
            if (angle < 1f * Mathf.Deg2Rad) // ~1 degree
            {
                result.Add(B);
                continue;
            }

            // Build a simple circular arc in XZ between T1 and T2 around corner B.
            // We'll approximate center by intersecting two lines perpendicular to segment tangents at T1/T2.
            // dir1: tangent direction at T1 along BA (pointing toward B), rotate -90 to aim inside corner.
            Vector3 t1Dir = (B - T1); t1Dir.y = 0; t1Dir.Normalize();
            Vector3 t2Dir = (B - T2); t2Dir.y = 0; t2Dir.Normalize();
            Vector3 n1 = new Vector3(t1Dir.z, 0, -t1Dir.x); // -90°
            Vector3 n2 = new Vector3(-t2Dir.z, 0, t2Dir.x); // +90°

            if (!LineLineIntersectionXZ(T1, n1, T2, n2, out Vector3 center))
            {
                // Fallback: just insert B if we can't find a reasonable center
                result.Add(B);
                continue;
            }

            // Angles around center
            float r = Mathf.Sqrt((T1.x - center.x)*(T1.x - center.x) + (T1.z - center.z)*(T1.z - center.z));
            if (r < 1e-5f)
            {
                result.Add(B);
                continue;
            }

            float a1 = Mathf.Atan2(T1.z - center.z, T1.x - center.x);
            float a2 = Mathf.Atan2(T2.z - center.z, T2.x - center.x);

            // Choose shortest signed arc direction
            float delta = ShortestAngle(a2 - a1);

            // Add T1, arc points (skip T1 duplicate), T2
            result.Add(T1);
            for (int s = 1; s < arcSamples; s++)
            {
                float t = s / (float)arcSamples;
                float ang = a1 + delta * t;
                // Y along the chord T1->T2
                float y = Mathf.Lerp(T1.y, T2.y, t);
                Vector3 p = new Vector3(
                    center.x + Mathf.Cos(ang) * r,
                    y,
                    center.z + Mathf.Sin(ang) * r
                );
                result.Add(p);
            }
            result.Add(T2);
        }

        // Keep last point
        result.Add(pts[^1]);
        return result;
    }

    public static Vector3[] ReduceClosePoints(Vector3[] points, float minDistance)
    {
        if (points == null || points.Length == 0) return new Vector3[0];

        List<Vector3> reducedPoints = new List<Vector3> { points[0] };

        for (int i = 1; i < points.Length; i++)
        {
            if (Vector3.Distance(reducedPoints[reducedPoints.Count - 1], points[i]) > minDistance)
            {
                reducedPoints.Add(points[i]);
            }
            else
            {
                reducedPoints[reducedPoints.Count - 1] = Vector3.Lerp(
                    reducedPoints[reducedPoints.Count - 1],
                    points[i],
                    0.5f
                );
            }
        }

        return reducedPoints.ToArray();
    }
    static bool LineLineIntersectionXZ(Vector3 p1, Vector3 d1, Vector3 p2, Vector3 d2, out Vector3 intersection)
    {
        intersection = Vector3.zero;
        float a1 = d1.x; float b1 = -d2.x; float c1 = p2.x - p1.x;
        float a2 = d1.z; float b2 = -d2.z; float c2 = p2.z - p1.z;
        float det = a1 * b2 - a2 * b1;
        if (Mathf.Abs(det) < 1e-6f) return false;
        float t = (c1 * b2 - c2 * b1) / det;
        intersection = new Vector3(p1.x + t * d1.x, (p1.y + p2.y) * 0.5f, p1.z + t * d1.z);
        return true;
    }

    static float ShortestAngle(float a)
    {
        while (a > Mathf.PI) a -= 2*Mathf.PI;
        while (a < -Mathf.PI) a += 2*Mathf.PI;
        return a;
    }
}
