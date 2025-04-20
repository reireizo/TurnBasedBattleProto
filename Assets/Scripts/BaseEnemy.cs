using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BaseEnemy
{
    public string name;

    public enum Type
    {
        FIRE,
        ELEC,
        WATER,
        ICE,
        ALMIGHTY
    }

    public enum Rarity
    {
        COMMON,
        UNCOMMON,
        RARE,
        SUPERRARE
    }

    public Type enemyType;
    public Rarity rarity;

    public float maxHP;
    public float currentHP;

    public float maxMP;
    public float currentMP;

    public float baseATK;
    public float curATK;
    public float baseDEF;
    public float curDEF;
}
