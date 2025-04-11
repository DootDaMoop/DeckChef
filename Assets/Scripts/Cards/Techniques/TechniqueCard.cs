using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Technique Card", menuName = "Cards/Technique Card")]
public class TechniqueCard : CardData
{
    public TechniqueType techniqueType;
    public int cookTime;
    public bool reusable = true;
}
