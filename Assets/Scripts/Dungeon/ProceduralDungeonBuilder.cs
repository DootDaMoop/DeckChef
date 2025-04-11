using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralDungeonBuilder : MonoBehaviour
{
    public static DungeonGraphData Generate(int numRooms = 6) { // Default to 6, TODO: make this a variable in the editor
        var graph = ScriptableObject.CreateInstance<DungeonGraphData>();
        graph.nodes = new List<DungeonNodeData>();

        Dictionary<Vector2Int, DungeonNodeData> occupied = new Dictionary<Vector2Int, DungeonNodeData>();
        Vector2Int currentPosition = Vector2Int.zero;

        // Start node init
        var startNode = CreateNode(currentPosition, true, RoomType.Start);
        graph.nodes.Add(startNode);
        occupied[currentPosition] = startNode;

        DungeonNodeData previousNode = startNode;

        for (int i = 1; i < numRooms; i++) {
            Vector2Int direction = GetRandomDirection();
            int pathLength = Random.Range(2,7);

            List<DungeonNodeData> pathNodes = new List<DungeonNodeData>();
            for (int j = 0; j < pathLength; j++) {
                currentPosition += direction;
                if (occupied.ContainsKey(currentPosition)) {
                    continue;
                }

                var pathNode = CreateNode(currentPosition, false, RoomType.None); // TODO: Path nodes roomtype random later
                graph.nodes.Add(pathNode);
                occupied[currentPosition] = pathNode;
                pathNodes.Add(pathNode);
            }

            currentPosition += direction;
            if (occupied.ContainsKey(currentPosition)) {
                continue;
            }

            bool isBoss = (i == numRooms - 1);
            var mainType = isBoss ? RoomType.Boss : GetRandomRoomType();
            var mainNode = CreateNode(currentPosition, true, mainType);
            graph.nodes.Add(mainNode);
            occupied[currentPosition] = mainNode;

            DungeonNodeData lastNode = previousNode;
            foreach (var path in pathNodes) {
                lastNode.connectedNodes.Add(path);
                lastNode = path;
            }
            lastNode.connectedNodes.Add(mainNode);

            previousNode = mainNode;
        }

        return graph;
    }

    private static DungeonNodeData CreateNode(Vector2Int position, bool isMainNode, RoomType roomType) {
        var node = ScriptableObject.CreateInstance<DungeonNodeData>();
        node.gridPosition = position;
        node.roomType = roomType;
        node.connectedNodes = new List<DungeonNodeData>();
        node.isMainNode = isMainNode;
        return node;
    }

    private static RoomType GetRandomRoomType() {
        RoomType[] roomTypes = (RoomType[])System.Enum.GetValues(typeof(RoomType));
        List<RoomType> validRoomTypes = new List<RoomType>(roomTypes);
        validRoomTypes.Remove(RoomType.Start);
        validRoomTypes.Remove(RoomType.Boss);
        return validRoomTypes[Random.Range(0, validRoomTypes.Count)];
    }

    private static Vector2Int GetRandomDirection() {
        List<Vector2Int> directions = new List<Vector2Int> {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };

        return directions[Random.Range(0, directions.Count)];
    }
}
