using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

//TODO: 2. малювати цю сітку з виставленим кроком(навколо вибраних об'єктів/показувати позицію переміщення після снаппінгу)
//TODO: 3. додати підтримку для полярної системи координат
//TODO: 4. зробити сітку зберігатися між сесіями Юніті(закрив і відкрив Юніті - сітка зберігла свої параметри)

public class SnapperTool : EditorWindow
{
    private const string UNDO_SNAP = "snap objects";
    
    private float _gridStep = 1f;
    private int _gridSize = 3;

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
        if (Selection.gameObjects.Length == 0)
            return;
        Handles.zTest = CompareFunction.Less;
        var offset = _gridStep * .5f;
        Handles.color = Color.black;

        foreach (var obj in Selection.gameObjects)
        {
            var pos = obj.transform.position;
            var center = GetSnappedPosition(pos);

            for (var i = 0; i < _gridSize * 2 + 1; i++)
            {
                // var y = (i - _gridSize) * _gridStep + center.y;
                
                var z = (i-_gridSize)*_gridStep + center.z;
                var p1 = new Vector3(center.x - _gridSize * _gridStep - offset, center.y, z);
                var p2 = new Vector3(center.x + _gridSize * _gridStep + offset, center.y, z);
                Handles.DrawLine(p1, p2);

                var x = (i-_gridSize) * _gridStep + center.x;
                p1.Set(x, center.y, center.z + _gridSize * _gridStep + offset);
                p2.Set(x, center.y, center.z - _gridSize * _gridStep - offset);
                Handles.DrawLine(p1, p2);
            }
        }

        Handles.color = Color.white;
    }

    private void OnGUI()
    {
        _gridStep = EditorGUILayout.FloatField("Grid step", _gridStep);
        _gridSize = EditorGUILayout.IntField("Grid size", _gridSize);
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

            var newPosition = go.transform.position;
            go.transform.position = GetSnappedPosition(newPosition);
        }
    }

    private Vector3 GetSnappedPosition(Vector3 pos)
    {
        var newPosition = pos;
        for (var i = 0; i < 3; i++)
        {
            var coordinate = newPosition[i];
            var delta = coordinate % _gridStep;
            if (delta > _gridStep * .5f)
            {
                coordinate += _gridStep - delta;
            }
            else
            {
                coordinate -= delta;
            }

            newPosition[i] = coordinate;
        }

        return newPosition;
    }
}
