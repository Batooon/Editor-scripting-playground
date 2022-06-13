using UnityEditor;
using UnityEngine;

public class SnapperTool : EditorWindow
{
    private const string UNDO_SNAP = "snap objects";
    
    [MenuItem("Tools/Snapper")]
    private static void OpenSnapperTool() => GetWindow<SnapperTool>("Snapper");

    private void OnEnable()
    {
        Selection.selectionChanged += Repaint;
        SceneView.duringSceneGui += DuringSceneGUI;
    }

    private void OnDisable()
    {
        Selection.selectionChanged -= Repaint;
        SceneView.duringSceneGui -= DuringSceneGUI;
    }

    private void DuringSceneGUI(SceneView sceneView)
    {
        Handles.DrawLine(Vector3.zero, Vector3.up);
    }

    private void OnGUI()
    {
        using (new EditorGUI.DisabledScope(Selection.gameObjects.Length == 0))
        {
            if (GUILayout.Button("SnapSelection"))
            {
                SnapSelection();
            }
        }
    }

    private void SnapSelection()
    {
        foreach (var go in Selection.gameObjects)
        {
            Undo.RecordObject(go.transform, UNDO_SNAP);
            go.transform.position = go.transform.position.Round();
        }
    }
}
