using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonManager : MonoBehaviour
{
    private DungeonGraphData dungeonGraphData;
    private int numRooms = 6;
    public GameObject nodePrefab;
    public Transform nodeParent; // DungeonMapPanel
    public float nodeSpacing = 150f;

    private void Start() {
        dungeonGraphData = ProceduralDungeonBuilder.Generate(numRooms);
        GenerateDungeon();
    }

    private void GenerateDungeon() {
        Dictionary<DungeonNodeData, GameObject> spawnedNodes = new Dictionary<DungeonNodeData, GameObject>();

        foreach (DungeonNodeData nodeData in dungeonGraphData.nodes) {
            GameObject nodeObject = Instantiate(nodePrefab, nodeParent);
            Vector2 position = (Vector2)nodeData.gridPosition * nodeSpacing;
            nodeObject.GetComponent<RectTransform>().anchoredPosition = position;
            nodeObject.name = nodeData.roomType.ToString() + " Node";
            
            var visual = nodeObject.GetComponent<DungeonNodeUI>();
            if (visual != null) {
                visual.Setup(nodeData);
            } else {
                Debug.LogWarning("DungeonNodeUI component not found on the node prefab.");
            }

            spawnedNodes[nodeData] = nodeObject;
            // TODO: Add logic to position connected nodes based on their connections
        }
    }
}
