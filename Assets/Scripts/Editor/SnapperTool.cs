using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

//TODO: 3. додати підтримку для полярної системи координат
//TODO: 4. зробити сітку зберігатися між сесіями Юніті(закрив і відкрив Юніті - сітка зберігла свої параметри)

public class SnapperTool : EditorWindow
{
    private const string UNDO_SNAP = "snap objects";

    private enum GridType
    {
        Cartesian,
        Polar
    }
    // [SerializeField] private bool _polarGrid;
    [SerializeField] private GridType _gridType;
    [SerializeField] private bool _useAngularDivision;
    [SerializeField] private int _angularDivisions = 24;
    [SerializeField] private float _angleStep;
    // [SerializeField] private float _distanceStep;
    [SerializeField] private float _gridStep = 1f;
    [SerializeField] private float _cartesianExtent = 3f;
    [SerializeField] private float _polarExtent = 3f;
    private float _snapPositionIndicatorSize = .1f;

    private SerializedObject _so;
    private SerializedProperty _propUseAngularDivision;
    private SerializedProperty _propAngularDivisions;
    private SerializedProperty _propAngleStep;
    // private SerializedProperty _propDistanceStep;
    private SerializedProperty _propGridType;
    private SerializedProperty _propGridStep;
    private SerializedProperty _propCartesianExtent;
    private SerializedProperty _propPolarExtent;

    [MenuItem("Tools/Snapper")]
    private static void OpenSnapperTool() => GetWindow<SnapperTool>("Snapper");

    private void OnEnable()
    {
        _so = new SerializedObject(this);
        _propUseAngularDivision = _so.FindProperty(nameof(_useAngularDivision));
        _propAngularDivisions = _so.FindProperty(nameof(_angularDivisions));
        _propAngleStep = _so.FindProperty(nameof(_angleStep));
        // _propDistanceStep = _so.FindProperty(nameof(_distanceStep));
        _propGridType = _so.FindProperty(nameof(_gridType));
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

        switch (_gridType)
        {
            case GridType.Polar:
            {
                DrawPolarGrid();
                break;
            }
            case GridType.Cartesian:
            default:
            {
                DrawCartesianGrid();
                break;
            }
        }

        Handles.color = Color.white;
    }

    private void DrawPolarGrid()
    {
        var circlesAmount = Mathf.RoundToInt(_polarExtent / _gridStep);
        var radiusOuter = circlesAmount * _gridStep;
        int linesCount;
        // Vector3 rotationAngle;
        if (_useAngularDivision)
        {
            linesCount = _angularDivisions;
            // rotationAngle = Vector3.up * (360f / linesCount);
        }
        else
        {
            linesCount = _angleStep > 0 ? Mathf.FloorToInt(360 / _angleStep) : 0;
            // rotationAngle = Vector3.up * _angleStep;
        }
                
        // var rotation = Matrix4x4.Rotate(Quaternion.Euler(rotationAngle));

        Handles.color = Color.black;
        Handles.zTest = CompareFunction.LessEqual;
        for (var i = 1; i <= circlesAmount; i++)
        {
            Handles.DrawWireDisc(Vector3.zero, Vector3.up, _gridStep * i);
        }

        const float TAU = 6.28318530718f;
        for (var i = 0; i < linesCount; i++)
        {
            var t = i / (float)linesCount;
            var angRad = t * TAU; //turns to radians
            var x = Mathf.Cos(angRad);
            var z = Mathf.Sin(angRad);
            var dir = new Vector3(x, 0, z);
            Handles.DrawAAPolyLine(Vector3.zero, dir * radiusOuter);
        }

        // var nextPoint = Vector3.right * (circlesAmount * _gridStep);
        // for (var i = 0; i < linesCount; i++)
        // {
        //     Handles.DrawAAPolyLine(Vector3.zero, nextPoint);
        //     nextPoint = rotation * nextPoint;
        // }
    }

    private void DrawCartesianGrid()
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

    private void OnGUI()
    {
        _so.Update();
        EditorGUILayout.PropertyField(_propGridType);
        EditorGUILayout.PropertyField(_propGridStep);
        using (new EditorGUILayout.HorizontalScope())
        {
            using (new EditorGUILayout.VerticalScope())
            {
                using (new EditorGUI.DisabledScope(_gridType == GridType.Polar))
                {
                    EditorGUILayout.PropertyField(_propCartesianExtent);
                }
            }
            using (new EditorGUILayout.VerticalScope())
            {
                using (new EditorGUI.DisabledScope(_gridType == GridType.Cartesian))
                {
                    EditorGUILayout.PropertyField(_propPolarExtent);
                    EditorGUILayout.PropertyField(_propUseAngularDivision);
                    if (_useAngularDivision)
                    {
                        EditorGUILayout.PropertyField(_propAngularDivisions);
                        _propAngularDivisions.intValue = Mathf.Max(3, _propAngularDivisions.intValue);
                    }
                    else
                    {
                        EditorGUILayout.PropertyField(_propAngleStep);
                        _propAngleStep.floatValue = Mathf.Max(10, _propAngleStep.floatValue);
                    }
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
