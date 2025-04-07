using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DungeonNodeUI : MonoBehaviour
{
    public TextMeshProUGUI nodeRoomTypeText;

    public void Setup(DungeonNodeData nodeData) {
        nodeRoomTypeText.text = nodeData.roomType.ToString();
    }
}
