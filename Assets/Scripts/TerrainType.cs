using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "TerrainType")]
public class TerrainType : ScriptableObject
{
    [SerializeField] private List<SpriteTableEntry> spriteTable;
    public List<SpriteTableEntry> SpriteTable => spriteTable;

    public bool IsGatherable;
}
