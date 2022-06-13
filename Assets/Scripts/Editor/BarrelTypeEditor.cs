using UnityEditor;

[CustomEditor(typeof(BarrelType)), CanEditMultipleObjects]
public class BarrelTypeEditor : Editor
{
    private SerializedObject _so;
    private SerializedProperty _propRadius;
    private SerializedProperty _propDamage;
    private SerializedProperty _propColor;
    
    private void OnEnable()
    {
        _so = serializedObject;
        _propRadius = _so.FindProperty("Radius");
        _propDamage = _so.FindProperty("Damage");
        _propColor = _so.FindProperty("BarrelColor");
    }

    public override void OnInspectorGUI()
    {
        _so.Update();
        EditorGUILayout.PropertyField(_propRadius);
        EditorGUILayout.PropertyField(_propDamage);
        EditorGUILayout.PropertyField(_propColor);
        if (_so.ApplyModifiedProperties())
        {
            ExplosiveBarrelsHandler.UpdateAllBarrelColors();
        }
    }
}
