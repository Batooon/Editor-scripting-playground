using UnityEditor;

[CustomEditor(typeof(BarrelType))]
public class BarrelTypeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var barrel = target as BarrelType;

        var newRadius = EditorGUILayout.FloatField("radius", barrel.Radius);

        if (newRadius != barrel.Radius)
        {
            Undo.RecordObject(barrel, "change barrel radius");
            barrel.Radius = newRadius;
        }
        
        barrel.Damage = EditorGUILayout.FloatField("damage", barrel.Damage);
        barrel.BarrelColor = EditorGUILayout.ColorField("color", barrel.BarrelColor);
    }
}
