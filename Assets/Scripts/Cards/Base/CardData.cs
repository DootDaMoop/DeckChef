using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CardData : ScriptableObject
{
    public string cardName;
    [TextArea] public string cardDescription;
    public int actionPointCost = 1;
    public CardType cardType;
    public Sprite cardImage;

    [SerializeField] private Sprite _cardFrame;
    public Sprite cardFrame {
        get {
            if (_cardFrame != null) {
                return _cardFrame;
            }

            switch (cardType) {
                case CardType.Ingredient:
                    return Resources.Load<Sprite>("Art/cardfood-atk");
                case CardType.Spice:
                    return Resources.Load<Sprite>("Art/cardfood-atk");
                case CardType.Technique:
                    return Resources.Load<Sprite>("Art/cardtechnique");
                default:
                    return Resources.Load<Sprite>("Art/cardfood-atk");
            }
        } set {
            _cardFrame = value;
        }
    }
}