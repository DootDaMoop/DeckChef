using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonManager : MonoBehaviour
{
    [Header("Dungeon Setup")]
    private DungeonGraphData dungeonGraphData;
    [SerializeField] private int numRooms = 6;

    [Header("Dungeon Node UI")]
    public GameObject nodePrefab;
    public GameObject pathNodePrefab;
    public Transform nodeParent; // DungeonMapPanel
    public float mainNodeSize = 50f;
    public float pathNodeSize = 30f;

    [ContextMenu("Generate Dungeon")]
    public void RegenerateDungeon() {
        ClearDungeon();
        dungeonGraphData = ProceduralDungeonBuilder.Generate(numRooms);
        GenerateDungeon();
    }

    private void Start() {
        dungeonGraphData = ProceduralDungeonBuilder.Generate(numRooms);
        GenerateDungeon();
    }

    private void GenerateDungeon() {
        // First, create all main nodes to ensure they get priority positioning
        foreach (DungeonNodeData nodeData in dungeonGraphData.nodes) {
            if (nodeData.isMainNode) {
                CreateNodeObject(nodeData);
            }
        }
        
        // Then create path nodes
        foreach (DungeonNodeData nodeData in dungeonGraphData.nodes) {
            if (!nodeData.isMainNode) {
                CreateNodeObject(nodeData);
            }
        }
    }

    private void ClearDungeon() {
        foreach (Transform child in nodeParent) {
            Destroy(child.gameObject);
        }
    }
    
    private GameObject CreateNodeObject(DungeonNodeData nodeData) {
        GameObject prefabToUse = nodeData.isMainNode ? nodePrefab : pathNodePrefab;
        GameObject nodeObject = Instantiate(prefabToUse, nodeParent);

        Vector2 anchoredPosition = (Vector2)nodeData.gridPosition * mainNodeSize;
        
        // Apply offset for path nodes, moving them slightly towards their connected nodes
        if (!nodeData.isMainNode && nodeData.connectedNodes.Count > 0) {
            Vector2 offsetDirection = Vector2.zero;
            bool connectedToMainNode = false;
            
            // Find connected main nodes to determine offset direction
            foreach (var connectedNode in nodeData.connectedNodes) {
                if (connectedNode.isMainNode) {
                    Vector2 direction = ((Vector2)(connectedNode.gridPosition - nodeData.gridPosition)).normalized;
                    offsetDirection += direction;
                    connectedToMainNode = true;
                }
            }
            
            // If we found a direction, normalize and apply offset
            if (offsetDirection != Vector2.zero) {
                offsetDirection.Normalize();
                float halfMainNodeSize = mainNodeSize / 2f;
                float halfPathNodeSize = pathNodeSize / 2f;

                float buffer = connectedToMainNode ? 15f : 0f;

                anchoredPosition += offsetDirection * (halfMainNodeSize - halfPathNodeSize - buffer);
            }
        }

        Debug.Log($"Node Position: {nodeData.gridPosition}, Anchored Position: {anchoredPosition}, IsMain: {nodeData.isMainNode}");
        nodeObject.GetComponent<RectTransform>().anchoredPosition = anchoredPosition;
        nodeObject.name = (nodeData.isMainNode ? "[MAIN] " : "[PATH] ") + $" {nodeData.roomType} at {nodeData.gridPosition}";
        
        var visual = nodeObject.GetComponent<DungeonNodeUI>();
        if (visual != null) {
            visual.Setup(nodeData);
        }
        
        return nodeObject;
    }
}
