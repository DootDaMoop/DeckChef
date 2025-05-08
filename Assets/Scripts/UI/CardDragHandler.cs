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
            PlayerHealth player = hit.collider.GetComponent<PlayerHealth>();
            CardData cardData = cardUI.GetCardData();

            if (player != null) {
                if (cardData.cardAbilityType == CardAbilityType.Heal || cardData.cardAbilityType == CardAbilityType.Defense) {
                    Debug.Log("Player hit: " + player.name);
                    PlayCardOnPlayer(player);
                    return;
                } else {
                    Debug.Log("Cannot play this card on player.");
                }
            }

            if (enemy != null) {
                if (cardData.cardAbilityType == CardAbilityType.Damage) {
                    Debug.Log("Enemy hit: " + enemy.name);
                    PlayCardOnEnemy(enemy);
                    return;
                } else {
                    Debug.Log("Cannot play this card on enemy.");
                }
            }
        }
        ReturnToHand();
    }

    public void PlayCardOnEnemy(Enemy enemy) {
        CardData cardData = cardUI.GetCardData();

        if (TurnManager.instance != null && !TurnManager.instance.CanPlayCard(cardData)) {
            Debug.Log("Cannot play card this turn.");
            ReturnToHand();
            return;
        }

        if(TurnManager.instance != null) {
            TurnManager.instance.UseActionPointsForCard(cardData);
        }

        enemy.ReceiveCard(cardData);
        CardManager.instance.PlayCard(cardData);
        Destroy(gameObject);
    }

    public void PlayCardOnPlayer(PlayerHealth player) {
        CardData cardData = cardUI.GetCardData();

        if (TurnManager.instance != null && !TurnManager.instance.CanPlayCard(cardData)) {
            Debug.Log("Cannot play card this turn.");
            ReturnToHand();
            return;
        }

        if(TurnManager.instance != null) {
            TurnManager.instance.UseActionPointsForCard(cardData);
        }

        if (cardData.cardAbilityType == CardAbilityType.Heal) {
            int healAmount = 15;
            player.Heal(healAmount);
        } else if (cardData.cardAbilityType == CardAbilityType.Defense) {
            int defenseAmount = 15;
            player.AddShield(defenseAmount);
        }

        CardManager.instance.PlayCard(cardData);
        Destroy(gameObject);
    }

    public void CombineWithCard(CardUI targetCard) {
        CardData selectedCardData = cardUI.GetCardData();
        CardData targetCardData = targetCard.GetCardData();

        if (CardCombinationManager.instance.CanCombine(selectedCardData, targetCardData)) {
            if (TurnManager.instance != null && !TurnManager.instance.CanPlayCard(selectedCardData)) {
                Debug.Log("Not enough Action Points.");
                ReturnToHand();
                return;
            }
            
            TurnManager.instance.UseActionPointsForCard(selectedCardData);

            TechniqueCard techniqueCard = null;
            IngredientCard ingredientCard = null;
            GameObject ingredientObject = null;

            if (selectedCardData is TechniqueCard && targetCardData is IngredientCard) {
                techniqueCard = selectedCardData as TechniqueCard;
                ingredientCard = targetCardData as IngredientCard;
                ingredientObject = targetCard.gameObject;
            } else if (targetCardData is TechniqueCard && selectedCardData is IngredientCard) {
                techniqueCard = targetCardData as TechniqueCard;
                ingredientCard = selectedCardData as IngredientCard;
                ingredientObject = gameObject;
            }

            if (techniqueCard != null && ingredientCard != null) {
                if (techniqueCard.cookTime > 0) {
                    CookingProgress cookingProgress = ingredientObject.GetComponent<CookingProgress>();
                    if (cookingProgress == null) {
                        cookingProgress = ingredientObject.AddComponent<CookingProgress>();
                    }
                    cookingProgress.Initialize(techniqueCard, ingredientCard);

                    if (selectedCardData is TechniqueCard) {
                        Destroy(gameObject);
                    } else {
                        Destroy(targetCard.gameObject);
                    }
                } else {
                    CardData resultCard = CardCombinationManager.instance.GetCombinationResults(techniqueCard, ingredientCard);
                    CardManager.instance.RemoveCardFromHand(selectedCardData);
                    CardManager.instance.AddCardToHand(targetCardData);
                    CardManager.instance.AddCardToHand(resultCard);
                    CardManager.instance.ReturnTechnniqueCard(techniqueCard);

                    Destroy(targetCard.gameObject);
                    Destroy(gameObject);
                }
            }
        } else {
            Debug.Log("Cannot combine cards.");
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
