using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DungeonNodeUI : MonoBehaviour
{
    public TextMeshProUGUI nodeRoomTypeText;

    public void Setup(DungeonNodeData nodeData) {
        if (nodeRoomTypeText == null) {
            Debug.LogError("nodeRoomTypeText is not assigned in the inspector.");
            return;
        } else {
            nodeRoomTypeText.text = nodeData.roomType.ToString();
        }
    }
}
