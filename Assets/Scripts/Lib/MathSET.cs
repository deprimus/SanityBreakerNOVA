using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathSET
{
    public static float Map(float n, float start1, float end1, float start2, float end2)
    {
        return ((n - start1) / (end1 - start1)) * (end2 - start2) + start2;
    }

    public static bool WithinThreshold(Vector3 x, Vector3 y, float threshold)
    {
        return Vector3.Distance(x, y) <= threshold;
    }
}