using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flame : BaseAttack
{
    public Flame()
    {
        attackName = "Flame";
        attackDescription = "Basic Fire spell. 120% Magic damage.";
        attackType = StatType.MAGIC;
        attackCost = 4f;
        attackDamage = 120f;
    }
}
