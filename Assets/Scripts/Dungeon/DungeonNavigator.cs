using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DungeonNavigator : MonoBehaviour
{
    public static DungeonNavigator instance;
    
    [Header("Navigation")]
    [SerializeField] private string battleSceneName = "BattleScene";
    [SerializeField] private DungeonNodeData currentNode;
    [SerializeField] private List<DungeonNodeData> visitedNodes = new List<DungeonNodeData>();
    [Header("References")]
    [SerializeField] private DungeonManager dungeonManager;
    [ContextMenu("Refresh All Node Visuals")]
    public void DebugRefreshNodeVisuals() {
        UpdateAllNodeVisuals();
    }
    
    private void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }
    
    private void Start() {
        if (dungeonManager == null) {
            dungeonManager = FindObjectOfType<DungeonManager>();
        }
        
        StartCoroutine(SetupInitialNode());
    }

    private IEnumerator SetupInitialNode() {
    // Wait for dungeon generation to complete
    yield return new WaitForEndOfFrame();
    
    if (dungeonManager == null || dungeonManager.dungeonGraphData == null) {
        Debug.LogError("No dungeon nodes available!");
        yield break; // Exit early if no dungeon data
    }
    
    DungeonGraphData graph = dungeonManager.dungeonGraphData;
    
    if (DungeonManager.returningFromCombat) {
        // Returning from combat - restore the last position
        int lastNodeX = PlayerPrefs.GetInt("LastNodeX", 0);
        int lastNodeY = PlayerPrefs.GetInt("LastNodeY", 0);
        Vector2Int lastNodePos = new Vector2Int(lastNodeX, lastNodeY);
        
        DungeonNodeData lastNode = null;
        
        // Find the node matching the saved position
        foreach (var node in graph.nodes) {
            if (node.gridPosition == lastNodePos) {
                lastNode = node;
                break;
            }
        }
        
        if (lastNode != null) {
            Debug.Log("Restored player position to " + lastNode.roomType + " at " + lastNode.gridPosition);
            SetCurrentNode(lastNode);
        } else {
            Debug.LogWarning("Could not find saved node position - defaulting to start node");
            FindAndSetStartNode(graph);
        }
    } else {
        // New game - find a starting node
        FindAndSetStartNode(graph);
    }
    
    // Update visuals for all nodes
    UpdateAllNodeVisuals();
    }


    private void UpdateAllNodeVisuals() {
    DungeonNodeUI[] allNodes = FindObjectsOfType<DungeonNodeUI>();
    foreach (var nodeUI in allNodes) {
        nodeUI.UpdateNodeVisuals();
    }
    Debug.Log("Updated visuals for " + allNodes.Length + " nodes");
    }

    private void FindAndSetStartNode(DungeonGraphData graph) {
    // Find the start node
    foreach (var node in graph.nodes) {
        if (node.roomType == RoomType.Start) {
            SetCurrentNode(node);
            Debug.Log("Starting at node: " + node.roomType + " at " + node.gridPosition);
            return;
        }
    }
    
    // Fallback to first main node if no start node found
    foreach (var node in graph.nodes) {
        if (node.isMainNode) {
            SetCurrentNode(node);
            Debug.Log("Starting at main node: " + node.roomType + " at " + node.gridPosition);
            return;
        }
    }
    
    // Last resort - use first node
    if (graph.nodes.Count > 0) {
        SetCurrentNode(graph.nodes[0]);
        Debug.Log("Starting at first node: " + graph.nodes[0].roomType + " at " + graph.nodes[0].gridPosition);
    }
}


    
    public void SetCurrentNode(DungeonNodeData node) {
        currentNode = node;
        if (!visitedNodes.Contains(node)) {
            visitedNodes.Add(node);
        }
        
        // Handle room based on type
        HandleRoomInteraction(node);
    }
    
    public bool CanMoveToNode(DungeonNodeData targetNode) {
        if (currentNode == null) return false;
        
        // Can only move to connected nodes
        return currentNode.connectedNodes.Contains(targetNode);
    }
    
    public void MoveToNode(DungeonNodeData targetNode) {
        if (CanMoveToNode(targetNode)) {
            DungeonNodeData previousNode = currentNode;
            SetCurrentNode(targetNode);
            UpdateAllNodeVisuals();
        } else {
            Debug.LogWarning("Cannot move to non-connected node!");
        }
    }
    
    private void HandleRoomInteraction(DungeonNodeData node) {
        switch (node.roomType) {
            case RoomType.Enemy:
                StartCoroutine(StartCombat());
                break;
                
            case RoomType.Boss:
                //StartCoroutine(StartBossCombat());
                StartCoroutine(StartCombat());
                break;
                
            case RoomType.Loot:
                // TODO: Implement loot room functionality
                Debug.Log("Found loot!");
                break;
                
            case RoomType.Event:
                // TODO: Implement event room functionality
                Debug.Log("Triggered event!");
                break;
                
            case RoomType.Start:
                Debug.Log("At starting point of dungeon");
                break;
                
            case RoomType.None:
            default:
                Debug.Log("Empty room");
                break;
        }
    }
    
    private IEnumerator StartCombat() {
        // Save state before combat
        PlayerPrefs.SetInt("LastNodeX", currentNode.gridPosition.x);
        PlayerPrefs.SetInt("LastNodeY", currentNode.gridPosition.y);

        DungeonManager.returningFromCombat = true;
        
        SceneManager.LoadScene(battleSceneName);
        
        yield return null;
    }
    
    // TODO: Finish this
    /*private IEnumerator StartBossCombat() {
        // Similar to normal combat but with boss enemies
        PlayerPrefs.SetInt("LastNodeX", currentNode.gridPosition.x);
        PlayerPrefs.SetInt("LastNodeY", currentNode.gridPosition.y);
        PlayerPrefs.SetInt("IsBossFight", 1);
        
        SceneManager.LoadScene(battleSceneName);
        
        yield return null;
    }*/
    
    public void ReturnFromCombat(bool victorious) {
        if (victorious) {
            // TODO: Mark current node as cleared
            // Maybe change its color or appearance
            Debug.Log("Returned victorious from combat!");
        }
        else {
            
            Debug.Log("Defeated in combat!");
        }
    }

    public bool IsNodeVisited(DungeonNodeData node) {
        return visitedNodes.Contains(node);
    }
    
    public DungeonNodeData GetCurrentNode() {
        return currentNode;
    }
}