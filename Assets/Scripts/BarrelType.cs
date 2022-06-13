using UnityEngine;

[CreateAssetMenu]
public class BarrelType : ScriptableObject
{
    [field: SerializeField, Range(1f, 8f)] public float Radius { get; private set; } = 1f;
    [field: SerializeField] public float Damage { get; private set; } = 10f;
    [field: SerializeField] public Color BarrelColor { get; private set; } = Color.red;
}
