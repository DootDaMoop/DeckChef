using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class DungeonNodeUI : MonoBehaviour
{
    public TextMeshProUGUI nodeRoomTypeText;
    private DungeonNodeData nodeData;
    private Button nodeButton;

    [SerializeField] private Color defaultColor = Color.white;
    [SerializeField] private Color visitedColor = Color.gray;
    [SerializeField] private Color currentColor = Color.green;
    [SerializeField] private Color availableColor = Color.yellow;
    private Image nodeImage;

    private void Awake() {
        nodeButton = GetComponent<Button>();
        if (nodeButton == null) {
            nodeButton = gameObject.AddComponent<Button>();
        }

        nodeImage = GetComponent<Image>();
        if (nodeImage == null) {
            nodeImage = gameObject.AddComponent<Image>();
        }

        nodeButton.onClick.AddListener(OnNodeClicked);
    }

    public void Setup(DungeonNodeData nodeData) {
        this.nodeData = nodeData;
        if (nodeRoomTypeText == null && nodeData.isMainNode) {
            Debug.LogError("nodeRoomTypeText is not assigned in the inspector.");
            return;
        } else if (nodeRoomTypeText != null && nodeData.isMainNode) {
            nodeRoomTypeText.text = nodeData.roomType.ToString();
        }

        UpdateNodeVisuals();
    }

    private void OnNodeClicked() {
        if (DungeonNavigator.instance != null) {
            if (DungeonNavigator.instance.CanMoveToNode(nodeData)) {
                DungeonNavigator.instance.MoveToNode(nodeData);
                UpdateNodeVisuals();
            } else {
                Debug.Log("Cannot move to this node.");
            }
        }
    }

    public void UpdateNodeVisuals() {
        if (DungeonNavigator.instance == null || nodeImage == null) {
            return;
        }

        DungeonNodeData currentNode = DungeonNavigator.instance.GetCurrentNode();

        if (currentNode == nodeData) {
            nodeImage.color = currentColor;
        } else if (DungeonNavigator.instance.CanMoveToNode(nodeData)) {
            nodeImage.color = availableColor;
        } else {
            bool isVisited = DungeonNavigator.instance.IsNodeVisited(nodeData);

            if (isVisited) {
                nodeImage.color = visitedColor;
            } else {
                nodeImage.color = defaultColor;
            }
        }
    }

    private void OnDestroy() {
        if (nodeButton != null) {
            nodeButton.onClick.RemoveListener(OnNodeClicked);
        }
    }

    public DungeonNodeData GetNodeData() {
        return nodeData;
    }
}
