using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CardData : ScriptableObject
{
    public string cardName;
    [TextArea] public string cardDescription;
    public int actionPointCost = 1;
    public CardType cardType;
    public CardAbilityType cardAbilityType;
    public Sprite cardImage;

    [SerializeField] private Sprite _cardFrame;
    public Sprite cardFrame {
        get {
            if (_cardFrame != null) {
                return _cardFrame;
            }

            if (cardType == CardType.Technique) {
                return Resources.Load<Sprite>("Art/cardtechnique");
            } else {
                switch (cardAbilityType) {
                case CardAbilityType.Damage:
                    return Resources.Load<Sprite>("Art/cardfood-atk");
                case CardAbilityType.Heal:
                    return Resources.Load<Sprite>("Art/cardfood-hp");
                case CardAbilityType.Defense:
                    return Resources.Load<Sprite>("Art/cardfood-def");
                default:
                    return Resources.Load<Sprite>("Art/cardfood-atk");
                }
            }
        } set {
            _cardFrame = value;
        }
    }
}