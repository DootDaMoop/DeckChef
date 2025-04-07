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
    private int originalSiblingIndex;
    private Vector2 lastMousePosition;

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
        originalSiblingIndex = rectTransform.GetSiblingIndex();
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.8f;

        lastMousePosition = eventData.position;
        rectTransform.SetParent(canvas.transform);
        LeanTween.scale(gameObject, Vector3.one * 1.15f, 0.2f).setEaseOutBack();
        LeanTween.rotateZ(gameObject, Random.Range(-5f, 5f), 0.2f);
    }

    public void OnDrag(PointerEventData eventData) {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        Vector2 direction = eventData.position - lastMousePosition;
        lastMousePosition = eventData.position;

        float angle = Mathf.Clamp(direction.x, -100f, 100f) * 10f;

        LeanTween.rotateZ(gameObject, angle, 0.2f).setEaseInQuad();
    }

    public void OnEndDrag(PointerEventData eventData) {
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;

        transform.SetParent(originalParent);
        transform.SetSiblingIndex(originalSiblingIndex);
        rectTransform.anchoredPosition = originalPosition;
        LeanTween.scale(gameObject, Vector3.one, 0.2f).setEaseOutBack();
        LeanTween.rotateZ(gameObject, 0f, 0.2f).setEaseOutBack();
    }
}
