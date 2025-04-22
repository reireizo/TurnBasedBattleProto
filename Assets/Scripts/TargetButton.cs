using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetButton : MonoBehaviour
{
    public GameObject TargetObject;
    
    void SelectTarget()
    {
        GameObject.Find("BattleManager").GetComponent<BattleStateMachine>();
    }
}
