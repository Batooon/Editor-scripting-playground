using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class SnapperTool : EditorWindow
{
    private string SAVE_PATH = Path.Combine(@"D:\Projects\Editor scripting playground\Assets\Scripts\Editor", "SnapperData.snp");
    private const string UNDO_SNAP = "snap objects";
    private const float TAU = 6.28318530718f;

    public enum GridType
    {
        Cartesian,
        Polar
    }

    [Serializable]
    public class GridData
    {
        public GridType Grid;
        public bool UseAngularDivision;
        public int AngularDivisions = 24;
        public float AngleStep = 15f;
        public float GridStep = 1f;
        public float CartesianExtent = 3f;
        public float PolarExtent = 3f;
    }

    [SerializeField] private GridData _gridData;
    
    private float _snapPositionIndicatorSize = .1f;

    private SerializedObject _so;
    private SerializedProperty _propUseAngularDivision;
    private SerializedProperty _propAngularDivisions;
    private SerializedProperty _propAngleStep;
    private SerializedProperty _propGridType;
    private SerializedProperty _propGridStep;
    private SerializedProperty _propCartesianExtent;
    private SerializedProperty _propPolarExtent;
    private SerializedProperty _propGridData;

    [MenuItem("Tools/Snapper")]
    private static void OpenSnapperTool() => GetWindow<SnapperTool>("Snapper");

    private void OnEnable()
    {
        try
        {
            if (File.Exists(SAVE_PATH))
            {
                var json = File.ReadAllText(SAVE_PATH);
                _gridData = JsonUtility.FromJson<GridData>(json);
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
            _gridData = new GridData();
        }

        _so = new SerializedObject(this);
        _propGridData = _so.FindProperty(nameof(_gridData));
        _propUseAngularDivision = _propGridData.FindPropertyRelative(nameof(_gridData.UseAngularDivision));
        _propAngularDivisions = _propGridData.FindPropertyRelative(nameof(_gridData.AngularDivisions));
        _propAngleStep = _propGridData.FindPropertyRelative(nameof(_gridData.AngleStep));
        _propGridType = _propGridData.FindPropertyRelative(nameof(_gridData.Grid));
        _propGridStep = _propGridData.FindPropertyRelative(nameof(_gridData.GridStep));
        _propCartesianExtent = _propGridData.FindPropertyRelative(nameof(_gridData.CartesianExtent));
        _propPolarExtent = _propGridData.FindPropertyRelative(nameof(_gridData.PolarExtent));

        Selection.selectionChanged += Repaint;
        SceneView.duringSceneGui += DuringSceneGUI;
    }

    private void OnDisable()
    {
        var json = JsonUtility.ToJson(_gridData, true);
        File.WriteAllText(SAVE_PATH, json);
        Selection.selectionChanged -= Repaint;
        SceneView.duringSceneGui -= DuringSceneGUI;
    }

    private void DuringSceneGUI(SceneView sceneView)
    {
        if (Selection.gameObjects.Length == 0)
            return;
        if (Event.current.type != EventType.Repaint)
            return;

        switch (_gridData.Grid)
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
        var circlesAmount = Mathf.RoundToInt(_gridData.PolarExtent / _gridData.GridStep);
        var radiusOuter = circlesAmount * _gridData.GridStep;
        int linesCount;
        // Vector3 rotationAngle;
        if (_gridData.UseAngularDivision)
        {
            linesCount = _gridData.AngularDivisions;
            // rotationAngle = Vector3.up * (360f / linesCount);
        }
        else
        {
            linesCount = _gridData.AngleStep > 0 ? Mathf.FloorToInt(360 / _gridData.AngleStep) : 0;
            // rotationAngle = Vector3.up * _angleStep;
        }
                
        // var rotation = Matrix4x4.Rotate(Quaternion.Euler(rotationAngle));

        Handles.color = Color.black;
        Handles.zTest = CompareFunction.LessEqual;
        for (var i = 1; i <= circlesAmount; i++)
        {
            Handles.DrawWireDisc(Vector3.zero, Vector3.up, _gridData.GridStep * i);
        }
        
        for (var i = 0; i < linesCount; i++)
        {
            var t = i / (float)linesCount;
            var angRad = t * TAU; //turns to radians
            var x = Mathf.Cos(angRad);
            var z = Mathf.Sin(angRad);
            var dir = new Vector3(x, 0, z);
            Handles.DrawAAPolyLine(Vector3.zero, dir * radiusOuter);
        }

        foreach (var obj in Selection.gameObjects)
        {
            var snappedPosition = GetSnappedPosition(obj.transform.position);
            
            Handles.color = Color.red;
            Handles.zTest = CompareFunction.Always;
                
            Handles.SphereHandleCap(0,
                snappedPosition,
                Quaternion.identity,
                _snapPositionIndicatorSize,
                EventType.Repaint);

            Handles.color = Color.black;
            Handles.zTest = CompareFunction.LessEqual;
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
        var lineCount = Mathf.RoundToInt((_gridData.CartesianExtent * 2) / _gridData.GridStep);
        if (lineCount % 2 == 0)
            lineCount++;
        var halfLineCount = lineCount / 2;
            
        foreach (var obj in Selection.gameObjects)
        {
            var pos = obj.transform.position;
            var center = GetSnappedPosition(pos);

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

                var x = offset * _gridData.GridStep + center.x;
                var z0 = center.z + halfLineCount * _gridData.GridStep;
                var z1 = center.z - halfLineCount * _gridData.GridStep;
                var p0 = new Vector3(x, center.y, z0);
                var p1 = new Vector3(x, center.y, z1);
                Handles.DrawAAPolyLine(p0, p1);

                x = offset * _gridData.GridStep + center.z;
                z0 = center.x + halfLineCount * _gridData.GridStep;
                z1 = center.x - halfLineCount * _gridData.GridStep;
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
                using (new EditorGUI.DisabledScope(_gridData.Grid == GridType.Polar))
                {
                    EditorGUILayout.PropertyField(_propCartesianExtent);
                }
            }
            using (new EditorGUILayout.VerticalScope())
            {
                using (new EditorGUI.DisabledScope(_gridData.Grid == GridType.Cartesian))
                {
                    EditorGUILayout.PropertyField(_propPolarExtent);
                    EditorGUILayout.PropertyField(_propUseAngularDivision);
                    if (_gridData.UseAngularDivision)
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

            var objPosition = go.transform.position;
            go.transform.position = GetSnappedPosition(objPosition);
        }
    }

    private Vector3 GetSnappedPosition(Vector3 originalPos)
    {
        switch (_gridData.Grid)
        {
            case GridType.Cartesian:
            default:
                return originalPos.Round(_gridData.GridStep);
            case GridType.Polar:
            {
                var v = new Vector2(originalPos.x, originalPos.z);
                var dist = v.magnitude;
                var snappedDistance = dist.Round(_gridData.GridStep);

                var angRad = Mathf.Atan2(v.y, v.x); // from 0 to TAU
                var angTurns = angRad / TAU; // from 0 to 1
                float angTurnsSnapped;
                if (_gridData.UseAngularDivision)
                {
                    angTurnsSnapped = angTurns.Round(1f / _gridData.AngularDivisions);
                }
                else
                {
                    var divisions = _gridData.AngleStep > 0 ? Mathf.FloorToInt(360 / _gridData.AngleStep) : 0;
                    angTurnsSnapped = angTurns.Round(1f / divisions);
                }

                var angRadSnapped = angTurnsSnapped * TAU;

                var newX = snappedDistance * Mathf.Cos(angRadSnapped);
                var newZ = snappedDistance * Mathf.Sin(angRadSnapped);
                return new Vector3(newX, 0, newZ);
            }
        }
    }
}
