using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Slash : BaseAttack
{
    public Slash()
    {
        attackName = "Slash";
        attackDescription = "Normal Slash attack, normal damage";
        attackType = StatType.PHYS;
        attackCost = 0f;
        attackDamage = 100f;
    }
}
