using UnityEngine;

[CreateAssetMenu]
public class BarrelType : ScriptableObject
{
    [field: SerializeField, Range(1f, 8f)] public float Radius { get; set; } = 1f;
    [field: SerializeField] public float Damage { get; set; } = 10f;
    [field: SerializeField] public Color BarrelColor { get; set; } = Color.red;
}
