using UnityEditor;
using UnityEngine;

[ExecuteAlways]
public class ExplosiveBarrel : MonoBehaviour
{
    [field: SerializeField] public BarrelType TypeBarrel { get; private set; }
    
    private readonly int _shaderPropColor = Shader.PropertyToID("_Color");

    private MaterialPropertyBlock _materialPropertyBlock;

    private MaterialPropertyBlock Mpb => _materialPropertyBlock ??= new MaterialPropertyBlock();

    private void OnValidate()
    {
        TryApplyColor();
    }

    public void TryApplyColor()
    {
        if (TypeBarrel == null)
            return;
        var rnd = GetComponent<MeshRenderer>();
        Mpb.SetColor(_shaderPropColor, TypeBarrel.BarrelColor);
        rnd.SetPropertyBlock(Mpb);
    }
    
    private void OnEnable()
    {
        ExplosiveBarrelsHandler.ExistingBarrels.Add(this);
    }

    private void OnDisable()
    {
        ExplosiveBarrelsHandler.ExistingBarrels.Remove(this);
    }

    private void OnDrawGizmos()
    {
        if (TypeBarrel == null)
            return;
        Handles.color = TypeBarrel.BarrelColor;
        Handles.DrawWireDisc(transform.position, transform.up, TypeBarrel.Radius);
        Handles.color = Color.white;
        // Gizmos.DrawWireSphere(transform.position, _radius);
    }
}
