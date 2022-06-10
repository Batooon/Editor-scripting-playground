using UnityEditor;
using UnityEngine;

[ExecuteAlways]
public class ExplosiveBarrel : MonoBehaviour
{
    [SerializeField, Range(1f, 8f)] private float _radius = 1f;
    [SerializeField, Range(1f, 100f)] private float _damage = 10f;
    [SerializeField] private Color _color = Color.red;
    public Color BarrelColor => _color;
    private int _shaderPropColor = Shader.PropertyToID("_Color");

    private MaterialPropertyBlock _materialPropertyBlock;

    private MaterialPropertyBlock Mpb => _materialPropertyBlock ??= new MaterialPropertyBlock();

    private void OnValidate()
    {
        ApplyColor();
    }

    private void ApplyColor()
    {
        var renderer = GetComponent<MeshRenderer>();
        Mpb.SetColor(_shaderPropColor, _color);
        renderer.SetPropertyBlock(Mpb);
    }
    
    private void OnEnable()
    {
        ExplosiveBarrelsHandler.ExistingBarrels.Add(this);
    }

    private void OnDisable()
    {
        ExplosiveBarrelsHandler.ExistingBarrels.Remove(this);
    }

    private void OnDrawGizmosSelected()
    {
        Handles.color = _color;
        Handles.DrawWireDisc(transform.position, transform.up, _radius);
        Handles.color = Color.white;
        // Gizmos.DrawWireSphere(transform.position, _radius);
    }
}
