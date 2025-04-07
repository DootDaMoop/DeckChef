using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dungeon Graph", menuName = "Dungeon/Graph")]
public class DungeonGraphData : ScriptableObject
{
    public List<DungeonNodeData> nodes;
}
