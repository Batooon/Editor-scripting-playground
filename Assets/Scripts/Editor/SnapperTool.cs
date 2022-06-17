using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

//TODO: 3. додати підтримку для полярної системи координат
//TODO: 4. зробити сітку зберігатися між сесіями Юніті(закрив і відкрив Юніті - сітка зберігла свої параметри)

public class SnapperTool : EditorWindow
{
    private const string UNDO_SNAP = "snap objects";

    [SerializeField] private bool _polarGrid;
    [SerializeField] private float _angleStep;
    [SerializeField] private float _distanceStep;
    [SerializeField] private float _gridStep = 1f;
    [SerializeField] private float _cartesianExtent = 3f;
    [SerializeField] private float _polarExtent = 3f;
    private float _snapPositionIndicatorSize = .1f;

    private SerializedObject _so;
    private SerializedProperty _propAngleStep;
    private SerializedProperty _propDistanceStep;
    private SerializedProperty _propPolarGrid;
    private SerializedProperty _propGridStep;
    private SerializedProperty _propCartesianExtent;
    private SerializedProperty _propPolarExtent;

    [MenuItem("Tools/Snapper")]
    private static void OpenSnapperTool() => GetWindow<SnapperTool>("Snapper");

    private void OnEnable()
    {
        _so = new SerializedObject(this);
        _propAngleStep = _so.FindProperty(nameof(_angleStep));
        _propDistanceStep = _so.FindProperty(nameof(_distanceStep));
        _propPolarGrid = _so.FindProperty(nameof(_polarGrid));
        _propGridStep = _so.FindProperty(nameof(_gridStep));
        _propCartesianExtent = _so.FindProperty(nameof(_cartesianExtent));
        _propPolarExtent = _so.FindProperty(nameof(_polarExtent));

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
        if (Event.current.type != EventType.Repaint)
            return;
        
        if (_propPolarGrid.boolValue)
        {
            var circlesAmount = Mathf.RoundToInt((_polarExtent * 2) / _distanceStep);
            var linesCount = _angleStep > 0 ? Mathf.FloorToInt(360 / _angleStep) : 0;
            var rotationAngle = new Vector3(0, _angleStep, 0);
            var rotation = Matrix4x4.Rotate(Quaternion.Euler(rotationAngle));

            Handles.color = Color.black;
            Handles.zTest = CompareFunction.LessEqual;
            for (var i = 1; i <= circlesAmount; i++)
            {
                Handles.DrawWireDisc(Vector3.zero, Vector3.up, _distanceStep * i);
            }

            var nextPoint = Vector3.right * (circlesAmount * _distanceStep);
            for (var i = 0; i < linesCount; i++)
            {
                Handles.DrawAAPolyLine(Vector3.zero, nextPoint);
                nextPoint = rotation * nextPoint;
            }
        }
        else
        {
            var lineCount = Mathf.RoundToInt((_cartesianExtent * 2) / _gridStep);
            if (lineCount % 2 == 0)
                lineCount++;
            var halfLineCount = lineCount / 2;
            
            foreach (var obj in Selection.gameObjects)
            {
                var pos = obj.transform.position;
                var center = pos.Round(_gridStep);

                Handles.color = Color.red;
                Handles.zTest = CompareFunction.Always;
                
                Handles.SphereHandleCap(0,
                    center,
                    Quaternion.identity,
                    _snapPositionIndicatorSize,
                    EventType.Repaint);

                Handles.color = Color.black;
                Handles.zTest = CompareFunction.LessEqual;

                for (var i = 0; i < lineCount; i++)
                {
                    var offset = i - halfLineCount;

                    var x = offset * _gridStep + center.x;
                    var z0 = center.z + halfLineCount * _gridStep;
                    var z1 = center.z - halfLineCount * _gridStep;
                    var p0 = new Vector3(x, center.y, z0);
                    var p1 = new Vector3(x, center.y, z1);
                    Handles.DrawAAPolyLine(p0, p1);

                    x = offset * _gridStep + center.z;
                    z0 = center.x + halfLineCount * _gridStep;
                    z1 = center.x - halfLineCount * _gridStep;
                    p0.Set(z0, center.y, x);
                    p1.Set(z1, center.y, x);
                    Handles.DrawAAPolyLine(p0, p1);
                }
            }
        }

        Handles.color = Color.white;
    }

    private void OnGUI()
    {
        _so.Update();
        EditorGUILayout.PropertyField(_propPolarGrid);
        using (new EditorGUILayout.HorizontalScope())
        {
            using (new EditorGUILayout.VerticalScope())
            {
                using (new EditorGUI.DisabledScope(_propPolarGrid.boolValue))
                {
                    EditorGUILayout.PropertyField(_propGridStep);
                    EditorGUILayout.PropertyField(_propCartesianExtent);
                }
            }
            using (new EditorGUILayout.VerticalScope())
            {
                using (new EditorGUI.DisabledScope(_propPolarGrid.boolValue == false))
                {
                    EditorGUILayout.PropertyField(_propPolarExtent);
                    EditorGUILayout.PropertyField(_propAngleStep);
                    EditorGUILayout.PropertyField(_propDistanceStep);
                }
            }
        }
        _so.ApplyModifiedProperties();
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
            go.transform.position = newPosition.Round(_gridStep);
        }
    }
}
