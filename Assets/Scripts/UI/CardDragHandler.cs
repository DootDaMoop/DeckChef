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
    private CardUI cardUI;

    [SerializeField] private Canvas canvas;
    private Camera mainCamera;

    private void Awake() {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        cardUI = GetComponent<CardUI>();

        if (canvas == null) {
            canvas = FindObjectOfType<Canvas>();
        }

        mainCamera = Camera.main;
        if (mainCamera == null) {
            Debug.LogError("Main camera not found.");
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

        Ray ray = mainCamera.ScreenPointToRay(eventData.position);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
        if (hit.collider != null) {
            Enemy enemy = hit.collider.GetComponent<Enemy>();
            if (enemy != null) {
                Debug.Log("Enemy hit: " + enemy.name);
                PlayCardOnEnemy(enemy);
                return;
            }

            // CardUI targetCard = hit.collider.GetComponent<CardUI>();
            // if (targetCard != null && targetCard != cardUI) {
            //     Debug.Log("Card hit: " + targetCard.name);
            //     CombineWithCard(targetCard);
            //     return;
            // }
        }
        ReturnToHand();
    }

    public void PlayCardOnEnemy(Enemy enemy) {
        CardData cardData = cardUI.GetCardData();
        enemy.ReceiveCard(cardData);
        CardManager.instance.PlayCard(cardData);
        Destroy(gameObject);
    }

    public void CombineWithCard(CardUI targetCard) {
        CardData selectedCardData = cardUI.GetCardData();
        CardData targetCardData = targetCard.GetCardData();

        if (CardCombinationManager.instance.CanCombine(selectedCardData, targetCardData)) {
            CardData resultCard = CardCombinationManager.instance.GetCombinationResults(selectedCardData, targetCardData);
            CardManager.instance.RemoveCardFromHand(selectedCardData);
            CardManager.instance.RemoveCardFromHand(targetCardData);
            CardManager.instance.AddCardToHand(resultCard);
            Destroy(targetCard.gameObject);
            Destroy(gameObject);
        } else {
            Debug.Log("Cannot combine these cards.");
            ReturnToHand();
        }
    }

    private void ReturnToHand() {
        transform.SetParent(originalParent);
        transform.SetSiblingIndex(originalSiblingIndex);
        rectTransform.anchoredPosition = originalPosition;
        LeanTween.scale(gameObject, Vector3.one, 0.2f).setEaseOutBack();
        LeanTween.rotateZ(gameObject, 0f, 0.2f).setEaseOutBack();
    }
}
