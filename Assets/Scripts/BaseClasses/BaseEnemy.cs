using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BaseEnemy: BaseClass
{
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
}
