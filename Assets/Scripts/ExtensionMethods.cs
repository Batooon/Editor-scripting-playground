using UnityEngine;

public static class ExtensionMethods
{
    public static Vector3 Round(this Vector3 v)
    {
        for (var i = 0; i < 3; i++)
        {
            v[i] = Mathf.Round(v[i]);
        }

        return v;
    }

    public static Vector3 Round(this Vector3 v, float size)
    {
        return Round(v / size) * size;
    }

    public static float Round(this float v, float step)
    {
        return Mathf.Round(v / step) * step;
    }
}
