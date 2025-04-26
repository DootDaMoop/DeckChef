using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ActionPointDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI actionPointText;
    [SerializeField] private Image[] actionPointIcons;

    private void Start()
    {
        if (ActionPointManager.instance != null)
        {
            ActionPointManager.instance.onActionPointsChanged.AddListener(UpdateDisplay);
            UpdateDisplay(ActionPointManager.instance.GetCurrentActionPoints(), ActionPointManager.instance.GetMaxActionPoints());
        }
    }

    private void UpdateDisplay(int currentActionPoints, int maxActionPoints)
    {
        if (actionPointText != null)
        {
            actionPointText.text = $"{currentActionPoints}/{maxActionPoints}";
        }

        if (actionPointIcons != null && actionPointIcons.Length > 0)
        {
            for (int i = 0; i < actionPointIcons.Length; i++)
            {
                actionPointIcons[i].gameObject.SetActive(i < currentActionPoints);
            }
        }

    }
}
