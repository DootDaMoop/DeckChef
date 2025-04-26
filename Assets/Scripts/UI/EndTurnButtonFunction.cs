using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndTurnButtonFunction : MonoBehaviour
{
    private Button endTurnButton;

    private void Awake()
    {
        endTurnButton = GetComponent<Button>();
        if (endTurnButton != null) {
            Debug.Log("EndTurnButton found in EndTurnButtonFunction script.");
            endTurnButton.onClick.AddListener(OnEndTurnButtonClicked);
        }
    }

    private void OnEndTurnButtonClicked()
    {
        if (TurnManager.instance != null && TurnManager.instance.IsPlayerTurn()) {
            TurnManager.instance.ManualEndPlayerTurn();
        }
    }

    private void OnEnable() {
        if (TurnManager.instance != null) {
            TurnManager.instance.onPlayerTurnStart.AddListener(EnableButton);
            TurnManager.instance.onPlayerTurnEnd.AddListener(DisableButton);
        }
    }

    private void OnDisable() {
        if (TurnManager.instance != null) {
            TurnManager.instance.onPlayerTurnStart.RemoveListener(EnableButton);
            TurnManager.instance.onPlayerTurnEnd.RemoveListener(DisableButton);
        }
    }

    private void EnableButton() {
        if (endTurnButton != null) {
            endTurnButton.interactable = true;
        }
    }

    private void DisableButton() {
        if (endTurnButton != null) {
            endTurnButton.interactable = false;
        }
    }
}
