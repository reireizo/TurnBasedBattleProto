using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrongSlash : BaseAttack
{
    public StrongSlash()
    {
        attackName = "Strong Slash";
        attackDescription = "Stronger Slash attack, +50% damage.";
        attackType = StatType.PHYS;
        attackCost = 0f;
        attackDamage = 150f;
    }
}
