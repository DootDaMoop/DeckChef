using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Transform originalParent;
    private Vector2 originalPosition;

    [SerializeField] private Canvas canvas;

    private void Awake() {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvas == null) {
            canvas = FindObjectOfType<Canvas>();
        }
    }

    public void OnBeginDrag(PointerEventData eventData) {
        originalParent = transform.parent;
        originalPosition = rectTransform.anchoredPosition;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.8f;

        rectTransform.SetParent(canvas.transform); 
    }

    public void OnDrag(PointerEventData eventData) {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData) {
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;

        transform.SetParent(originalParent);
        rectTransform.anchoredPosition = originalPosition;
    }
}
