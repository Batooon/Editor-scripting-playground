using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BarrelType))]
public class BarrelTypeEditor : Editor
{
    private enum Things
    {
        Bleep, Bloop, Blap
    }

    private Things _things;
    private float _someValue;
    private Transform _lightTransform;
    
    public override void OnInspectorGUI()
    {
        //old way
        // GUILayout.BeginHorizontal();
        //
        // GUILayout.Label("Things:", GUILayout.Width(60f));
        //
        // if (GUILayout.Button("Do a thing"))
        //     Debug.Log("did the thing");
        //
        // _things = (Things)EditorGUILayout.EnumPopup(_things);
        //
        // GUILayout.EndHorizontal();

        //new way, safer(you can use return statements)

        using (new GUILayout.VerticalScope(EditorStyles.helpBox))
        {
            GUILayout.Label("Category:", EditorStyles.boldLabel);
            using (new GUILayout.HorizontalScope(EditorStyles.helpBox))
            {
                GUILayout.Label("Things:", GUILayout.Width(60f));
        
                if (GUILayout.Button("Do a thing"))
                    Debug.Log("did the thing");
        
                _things = (Things)EditorGUILayout.EnumPopup(_things);
            }
        
            GUILayout.Label("Things:", EditorStyles.toolbar);
            GUILayout.Label("Things:", GUI.skin.button);
        }
        
        GUILayout.Space(40f);
        
        _lightTransform =
            EditorGUILayout.ObjectField("Put some shit in here:", _lightTransform, typeof(Transform),
                true) as Transform;


        // explicit positioning using Rect
        // GUI
        // EditorGUI

        // implicit positioning, auto-layout
        // GUILayout
        // EditorGUILayout
    }
}
