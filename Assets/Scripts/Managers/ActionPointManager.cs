using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ActionPointManager : MonoBehaviour
{
    public static ActionPointManager instance;

    [Header("Action Points")]
    [SerializeField] private int maxActionPoints = 3;
    [SerializeField] private int currentActionPoints = 0;
    [Header("Events")]
    public UnityEvent<int, int> onActionPointsChanged;

    private void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    private void Start() {
        ResetActionPoints();
    }

    public bool HasEnoughActionPoints(int points) {
        return currentActionPoints >= points;
    }

    public void ResetActionPoints() {
        currentActionPoints = maxActionPoints;
        onActionPointsChanged?.Invoke(currentActionPoints, maxActionPoints);
    }

    public bool UseActionPoints(int points) {
        if (HasEnoughActionPoints(points)) {
            currentActionPoints -= points;
            onActionPointsChanged?.Invoke(currentActionPoints, maxActionPoints);
            Debug.Log($"Used {points} action points. Remaining: {currentActionPoints}");
            if (currentActionPoints <= 0) {
                Debug.Log("No action points left! Ending turn.");
                TurnManager.instance.EndPlayerTurn();
            }
            return true;
        }

        Debug.Log("Not enough action points!");
        return false;
    }

    public int GetCurrentActionPoints() {
        return currentActionPoints;
    }

    public int GetMaxActionPoints() {
        return maxActionPoints;
    }
}
