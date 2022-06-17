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

    public static Vector3 Round(this Vector3 v, float distanceSize, float angleSize)
    {
        v.Round(distanceSize);
        var currentAngle = Mathf.Asin(v.z / Mathf.Sqrt(v.sqrMagnitude)) * Mathf.Rad2Deg;
        //TODO: snap angle and find new position
        // var newAngle = RoundAngle(currentAngle, angleSize);
        return Vector3.zero;
    }

    private static float RoundAngle(float angle, float step)
    {
        return Mathf.Round(angle / step) * step;
    }
}
