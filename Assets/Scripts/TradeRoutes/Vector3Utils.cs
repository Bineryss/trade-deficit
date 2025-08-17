using UnityEngine;
using UnityEngine.Splines;
using System.Collections.Generic;

public static class NavMeshSpline
{
    /*  SmoothViaSpline
     *  ---------------
     *  raw      : NavMesh corner array (or any Vector3 list)
     *  samples  : how many EQUALLY-spaced points you want back
     *  closed   : true = loop spline
     */
    public static Vector3[] SmoothViaSpline(Vector3[] raw,
                                            int samples   = 50,
                                            bool closed   = false)
    {
        if (raw == null || raw.Length < 2)  return raw;

        // 1. Build a spline in memory
        var knots = new BezierKnot[raw.Length];
        for (int i = 0; i < raw.Length; i++)
            knots[i] = new BezierKnot(raw[i], Vector3.forward, Vector3.back);

        var spline = new SplineContainer();
        // int curveID = spline.AddSpline(new Spline(knots, closed));

        // 2. Sample the spline at constant distance
        var result = new Vector3[samples];
        for (int i = 0; i < samples; i++)
        {
            float t = i / (samples - 1f);                       // 0-1
            // result[i] = spline.EvaluatePosition(curveID, t);    // world-space Vector3
        }
        return result;
    }
}
