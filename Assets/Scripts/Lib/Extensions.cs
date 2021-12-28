using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{
    public static Color Invert(this Color color)
    {
        return new Color(1f - color.r, 1f - color.g, 1f - color.b);
    }
}