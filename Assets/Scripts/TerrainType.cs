using UnityEngine;

[CreateAssetMenu(menuName = "TerrainType")]
public class TerrainType : ScriptableObject
{
    [SerializeField] private Sprite sprite;
    public Sprite Sprite => sprite;
}
