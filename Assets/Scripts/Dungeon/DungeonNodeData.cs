using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dungeon Node", menuName = "Dungeon/Node")]
public class DungeonNodeData : ScriptableObject
{
    public RoomType roomType;
    public Vector2Int gridPosition;
    public List<DungeonNodeData> connectedNodes;
}
