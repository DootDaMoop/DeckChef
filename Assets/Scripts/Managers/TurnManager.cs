using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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

    [Header("Turn Settings")]
    [SerializeField] private TurnState currentTurnState;
    [SerializeField] private float turnDelay = 0.5f;

    [Header("Events")]
    public UnityEvent onPlayerTurnStart;
    public UnityEvent onPlayerTurnEnd;
    public UnityEvent onEnemyTurnStart;
    public UnityEvent onEnemyTurnEnd;
    public UnityEvent onVictory;
    public UnityEvent onDefeat;

    private CardManager cardManager;
    private List<EnemyController> activeEnemies = new List<EnemyController>();

    private void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    private void Start() {
        cardManager = CardManager.instance;
        //StartCoroutine(StartGame());
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
        if (cardManager != null && cardManager.GetHand().Count > 0) {
            cardManager.DrawCard();
        }

        onPlayerTurnStart?.Invoke();

        EnablePlayerControls(true);
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
        onVictory?.Invoke();
        // Handle victory logic here (e.g., show victory screen, reward player, etc.)
    }
    
    public void SetDefeatState() {
        currentTurnState = TurnState.Defeat;
        onDefeat?.Invoke();
        // Handle defeat logic here (e.g., show defeat screen, reset game, etc.)
    }

    private void RefreshEnemyList() {
        activeEnemies.Clear();
        EnemyController[] enemies = FindObjectsOfType<EnemyController>();

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
}
