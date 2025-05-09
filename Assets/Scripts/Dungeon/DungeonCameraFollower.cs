using UnityEngine;

public class DungeonCameraFollower : MonoBehaviour
{
    [SerializeField] private float smoothSpeed = 0.125f;
    [SerializeField] private Vector3 offset = new Vector3(0, 0, -10);
    
    private DungeonNavigator dungeonNavigator;
    private Camera mainCamera;
    private Canvas dungeonCanvas;
    
    private void Start() {
        dungeonNavigator = FindObjectOfType<DungeonNavigator>();
        mainCamera = Camera.main;
        dungeonCanvas = FindObjectOfType<Canvas>();
        
        if (dungeonNavigator == null) {
            Debug.LogError("DungeonNavigator not found!");
        }
        
        if (dungeonCanvas == null) {
            Debug.LogError("Canvas not found!");
        }
    }
    
    private void LateUpdate() {
        if (dungeonNavigator == null || dungeonCanvas == null) return;
        
        DungeonNodeData currentNode = dungeonNavigator.GetCurrentNode();
        if (currentNode == null) return;
        
        // Find UI element for current node
        DungeonNodeUI targetNodeUI = null;
        DungeonNodeUI[] allNodes = FindObjectsOfType<DungeonNodeUI>();
        
        foreach (var nodeUI in allNodes) {
            if (nodeUI.GetNodeData() == currentNode) {
                targetNodeUI = nodeUI;
                break;
            }
        }
        
        if (targetNodeUI == null) return;
        
        // Convert from Canvas space to World space
        Vector3 targetPosition;
        
        if (dungeonCanvas.renderMode == RenderMode.ScreenSpaceOverlay) {
            // For screen space overlay canvas
            Vector3 screenPoint = RectTransformUtility.WorldToScreenPoint(null, targetNodeUI.transform.position);
            RectTransformUtility.ScreenPointToWorldPointInRectangle(
                dungeonCanvas.GetComponent<RectTransform>(),
                screenPoint,
                mainCamera,
                out targetPosition);
            
            // Add depth offset
            targetPosition.z = transform.position.z;
        } else {
            // For world space canvas
            targetPosition = targetNodeUI.transform.position + offset;
        }
        
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, targetPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }
}