using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ExplosiveBarrelsHandler : MonoBehaviour
{
    public static readonly List<ExplosiveBarrel> ExistingBarrels = new List<ExplosiveBarrel>();

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Handles.zTest = CompareFunction.LessEqual;
        var handlerPosition = transform.position;
        foreach (var barrel in ExistingBarrels)
        {
            var barrelPosition = barrel.transform.position;
            var halfHeight = (handlerPosition.y - barrelPosition.y) * .5f;
            var tangentOffset = Vector3.up * halfHeight;
            Handles.DrawBezier(handlerPosition, barrelPosition,
                handlerPosition - tangentOffset, barrelPosition + tangentOffset,
                barrel.BarrelColor,
                EditorGUIUtility.whiteTexture,
                1f);
        }
    }
#endif
}
