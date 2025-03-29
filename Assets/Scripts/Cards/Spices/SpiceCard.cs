using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Spice Card", menuName = "Cards/Spice Card")]
public class SpiceCard : CardData
{
    public BuffType buffType;
    public float buffAmount;
}
