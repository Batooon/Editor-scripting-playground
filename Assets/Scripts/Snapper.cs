using UnityEditor;
using UnityEngine;

public static class Snapper
{
    private const string UNDO_SNAP = "snap objects";
    
    [MenuItem("Edit/Snap selected objects")]
    public static void SnapTheThings()
    {
        foreach (var go in Selection.gameObjects)
        {
            Undo.RecordObject(go.transform, UNDO_SNAP);
            go.transform.position = go.transform.position.Round();
        }
    }

    public static Vector3 Round(this Vector3 v)
    {
        for (var i = 0; i < 3; i++)
        {
            v[i] = Mathf.Round(v[i]);
        }

        return v;
    }
}
