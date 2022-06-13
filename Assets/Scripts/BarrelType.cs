using UnityEngine;

[CreateAssetMenu]
public class BarrelType : ScriptableObject
{
    [SerializeField, Range(1f, 8f)] public float Radius = 1f;
    [SerializeField] public float Damage = 10f;
    [SerializeField] public Color BarrelColor = Color.red;
}
