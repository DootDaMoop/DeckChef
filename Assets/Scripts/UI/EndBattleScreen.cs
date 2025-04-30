using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndBattleScreen : MonoBehaviour
{
    [SerializeField] private Button continueButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private TextMeshProUGUI resultText;

    private void Awake() {
        gameObject.SetActive(false);
    }

    private void OnEnable() {
        if (continueButton != null) {
            continueButton.onClick.AddListener(OnContinueButtonClicked);
        }
        if (mainMenuButton != null) {
            mainMenuButton.onClick.AddListener(OnMainMenuButtonClicked);
        }

        if (resultText != null && gameObject.name.Contains("Victory")) {
            // TODO: show rewards as well
            resultText.text = "Victory!";
        } else if (resultText != null && gameObject.name.Contains("Defeat")) {
            resultText.text = "Defeat!";
        }
    }

    private void OnDisable() {
        if (continueButton != null) {
            continueButton.onClick.RemoveListener(OnContinueButtonClicked);
        }
        if (mainMenuButton != null) {
            mainMenuButton.onClick.RemoveListener(OnMainMenuButtonClicked);
        }
    }

    private void OnContinueButtonClicked() {
        gameObject.SetActive(false);
        TurnManager.instance.ReturnToDungeon();
    }

    private void OnMainMenuButtonClicked() {
        gameObject.SetActive(false);
        TurnManager.instance.ReturnToMainMenu();
    }
}
