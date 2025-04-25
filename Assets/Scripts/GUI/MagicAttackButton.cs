using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicAttackButton : MonoBehaviour
{
    public BaseAttack magicAttackToPerform;

    public void CastMagicAttack()
    {
        GameObject.Find("BattleManager").GetComponent<BattleStateMachine>().SpellInput(magicAttackToPerform);
    }
}
