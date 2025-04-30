using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public enum TurnState {
    Start,
    PlayerTurn,
    EnemyTurn,
    Victory,
    Defeat,
}

public class TurnManager : MonoBehaviour
{
    public static TurnManager instance;
    private ActionPointManager actionPointManager;

    [Header("Turn Settings")]
    [SerializeField] private TurnState currentTurnState;
    [SerializeField] private float turnDelay = 0.5f;
    private int playerTurnCount = 0;

    [Header("Victory/Defeat Settings")]
    [SerializeField] private GameObject victoryScreen;
    [SerializeField] private GameObject defeatScreen;
    [SerializeField] private float endGameDelay = 2f;
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    [SerializeField] private string dungeonSceneName = "Dungeon";

    [Header("Events")]
    public UnityEvent onPlayerTurnStart;
    public UnityEvent onPlayerTurnEnd;
    public UnityEvent onEnemyTurnStart;
    public UnityEvent onEnemyTurnEnd;
    public UnityEvent onVictory;
    public UnityEvent onDefeat;

    private CardManager cardManager;
    private List<Enemy> activeEnemies = new List<Enemy>();

    private void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    private void Start() {
        cardManager = CardManager.instance;
        actionPointManager = ActionPointManager.instance;
        StartCoroutine(StartBattle());
    }

    public IEnumerator StartBattle() {
        currentTurnState = TurnState.Start;

        RefreshEnemyList();

        if (cardManager != null) {
            cardManager.DrawStartingHand();
        }
        yield return new WaitForSeconds(turnDelay);
        StartPlayerTurn();
    }

    public void StartPlayerTurn() {
        currentTurnState = TurnState.PlayerTurn;
        playerTurnCount++;
        if (cardManager != null && cardManager.GetHand().Count > 0) {
            cardManager.DrawCard();
        }

        if (actionPointManager != null) {
            actionPointManager.ResetActionPoints();
        }

        onPlayerTurnStart?.Invoke();
        EnablePlayerControls(true);
    }

    public void ManualEndPlayerTurn() {
        if (currentTurnState == TurnState.PlayerTurn) {
            EndPlayerTurn();
        }
    }

    public bool CanPlayCard(CardData card) {
        if (actionPointManager != null && actionPointManager.HasEnoughActionPoints(card.actionPointCost)) {
            return true;
        }
        return false;
    }

    public bool UseActionPointsForCard(CardData card) {
        if (actionPointManager == null) {
            return true;
        }

        return actionPointManager.UseActionPoints(card.actionPointCost);
    }

    public void EndPlayerTurn() {
        EnablePlayerControls(false);
        onPlayerTurnEnd?.Invoke();

        StartCoroutine(StartEnemyTurnDelay());
    }

    private IEnumerator StartEnemyTurnDelay() {
        yield return new WaitForSeconds(turnDelay);
        StartEnemyTurn();
    }

    public void StartEnemyTurn() {
        currentTurnState = TurnState.EnemyTurn;
        onEnemyTurnStart?.Invoke();

        StartCoroutine(EnemyTurnRoutine());
    }

    private IEnumerator EnemyTurnRoutine() {
        RefreshEnemyList();

        if (activeEnemies.Count == 0) {
            SetVictoryState();
            yield break;
        }

        foreach (var enemy in activeEnemies) {
            yield return StartCoroutine(enemy.TakeAction());
            yield return new WaitForSeconds(turnDelay);

            if (ShouldEndBattle()) {
                SetDefeatState();
                yield break;
            }
        }

        onEnemyTurnEnd?.Invoke();
        yield return new WaitForSeconds(turnDelay);
        StartPlayerTurn();
    }

    public void SetVictoryState() {
        currentTurnState = TurnState.Victory;
        victoryScreen.SetActive(true);
        StartCoroutine(ShowVictoryScreen());
        onVictory?.Invoke();
    }
    
    public void SetDefeatState() {
        currentTurnState = TurnState.Defeat;
        defeatScreen.SetActive(true);
        StartCoroutine(ShowDefeatScreen());
        onDefeat?.Invoke();
    }

    private IEnumerator ShowVictoryScreen() {
        yield return new WaitForSeconds(endGameDelay);
        
        if (victoryScreen != null) {
            victoryScreen.SetActive(true);
        } else {
            Debug.LogWarning("Victory screen not assigned in TurnManager.");
        }
    }

    private IEnumerator ShowDefeatScreen() {
        yield return new WaitForSeconds(endGameDelay);
        
        if (defeatScreen != null) {
            defeatScreen.SetActive(true);
        } else {
            Debug.LogWarning("Defeat screen not assigned in TurnManager.");
        }
    }

    public void ReturnToMainMenu() {
        SceneManager.LoadScene(mainMenuSceneName);
    }
    public void ReturnToDungeon() {
        SceneManager.LoadScene(dungeonSceneName);
    }

    private void RefreshEnemyList() {
        activeEnemies.Clear();
        Enemy[] enemies = FindObjectsOfType<Enemy>();

        foreach (var enemy in enemies) {
            if (enemy.gameObject.activeInHierarchy && enemy.IsAlive()) {
                activeEnemies.Add(enemy);
            }
        }
    }

    private bool ShouldEndBattle() {
        // Logic to determine if the battle should end (e.g., player health, enemy health, etc.)
        // replace with player health system logic
        return false;
    }

    private void EnablePlayerControls(bool enable) {
        // Enable or disable player controls here (e.g., UI, input, etc.)
        // Example: PlayerController.instance.EnableControls(enable);
    }

    public TurnState GetCurrentTurnState() {
        return currentTurnState;
    }

    public bool IsPlayerTurn() {
        return currentTurnState == TurnState.PlayerTurn;
    }

    public int GetPlayerTurnCount() {
        return playerTurnCount;
    }
}
