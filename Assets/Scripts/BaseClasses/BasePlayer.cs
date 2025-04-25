using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BasePlayer: BaseClass
{
    public int strength;
    public int magic;
    public int vitality;
    public int agility;

    public List<BaseAttack> knownSpells = new List<BaseAttack>();
}
