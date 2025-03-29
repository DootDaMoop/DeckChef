using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CardData : ScriptableObject
{
    public string cardName;
    [TextArea] public string description;
    public int cost;
    public CardType cardType;
    public Sprite cardImage;
}