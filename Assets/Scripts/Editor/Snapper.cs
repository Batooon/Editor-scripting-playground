using UnityEditor;
using UnityEngine;

public static class Snapper
{
    private const string UNDO_SNAP = "snap objects";

    [MenuItem("Edit/Snap selected objects %&s", isValidateFunction: true)]
    public static bool SnapTheThingsValidate()
    {
        return Selection.gameObjects.Length > 0;
    }

    [MenuItem("Edit/Snap selected objects %&s")]
    public static void SnapTheThings()
    {
        foreach (var go in Selection.gameObjects)
        {
            Undo.RecordObject(go.transform, UNDO_SNAP);
            go.transform.position = go.transform.position.Round();
        }
    }
}
