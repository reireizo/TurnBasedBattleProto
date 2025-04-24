using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetButton : MonoBehaviour
{
    public GameObject TargetObject;
    
    public void SelectTarget()
    {
        GameObject.Find("BattleManager").GetComponent<BattleStateMachine>().TargetInput(TargetObject);
    }

    public void HideSelector()
    {
        TargetObject.transform.Find("Selector").gameObject.SetActive(false);
    }
    public void ShowSelector()
    {
        TargetObject.transform.Find("Selector").gameObject.SetActive(true);
    }
}
