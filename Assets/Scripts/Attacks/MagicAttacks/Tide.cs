using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tide : BaseAttack
{
    public Tide()
    {
        attackName = "Tide";
        attackDescription = "Moderate Water spell. 150% Magic damage.";
        attackType = StatType.MAGIC;
        attackCost = 15f;
        attackDamage = 150f;
    }
}
