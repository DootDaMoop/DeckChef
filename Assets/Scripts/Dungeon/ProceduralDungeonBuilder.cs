using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralDungeonBuilder : MonoBehaviour
{
    public static DungeonGraphData Generate(int numRooms = 6) { // Default to 6, TODO: make this a variable in the editor
        var graph = ScriptableObject.CreateInstance<DungeonGraphData>();
        graph.nodes = new List<DungeonNodeData>();

        Dictionary<Vector2Int, DungeonNodeData> occupied = new Dictionary<Vector2Int, DungeonNodeData>();
        List<Vector2Int> openPositions = new List<Vector2Int>();

        Vector2Int startPosition = new Vector2Int(0, 0);
        Vector2Int currentPosition = startPosition;

        var startNode = ScriptableObject.CreateInstance<DungeonNodeData>();
        startNode.roomType = GetRandomRoomType();
        startNode.connectedNodes = new List<DungeonNodeData>();
        startNode.gridPosition = startPosition;
        graph.nodes.Add(startNode);
        occupied[startPosition] = startNode;
        openPositions.AddRange(GetNeighbors(currentPosition));

        for (int i = 1; i < numRooms; i++) {
            if (openPositions.Count == 0) {
                Debug.LogWarning("No more open positions available. Stopping generation.");
                break;
            }

            int randomIndex = Random.Range(0, openPositions.Count);
            currentPosition = openPositions[randomIndex];
            openPositions.RemoveAt(randomIndex);

            var newNode = ScriptableObject.CreateInstance<DungeonNodeData>();
            newNode.roomType = GetRandomRoomType();
            newNode.connectedNodes = new List<DungeonNodeData>();
            newNode.gridPosition = currentPosition;

            graph.nodes.Add(newNode);
            occupied[currentPosition] = newNode;

            var neighbors = GetNeighbors(currentPosition);
            foreach (var neighbor in neighbors) {
                if (occupied.TryGetValue(neighbor, out var neighborNode)) {
                    neighborNode.connectedNodes.Add(newNode);
                    break;
                }
            }

            foreach (var neighbor in GetNeighbors(currentPosition)) {
                if (!occupied.ContainsKey(neighbor) && !openPositions.Contains(neighbor)) {
                    openPositions.Add(neighbor);
                }
            }
            
            Debug.Log($"Room {i}: {newNode.roomType} at {currentPosition}");
        }

        return graph;
    }

    private static RoomType GetRandomRoomType() {
        RoomType[] roomTypes = (RoomType[])System.Enum.GetValues(typeof(RoomType));
        return roomTypes[Random.Range(0, roomTypes.Length)];
    }

    private static List<Vector2Int> GetNeighbors(Vector2Int position){
        return new List<Vector2Int> {
            position + Vector2Int.up,
            position + Vector2Int.down,
            position + Vector2Int.left,
            position + Vector2Int.right
        };
    }
}
