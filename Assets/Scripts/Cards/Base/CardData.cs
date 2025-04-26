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
}