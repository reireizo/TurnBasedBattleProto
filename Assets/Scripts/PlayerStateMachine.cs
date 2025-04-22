using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStateMachine : MonoBehaviour
{
    BattleStateMachine battleManager;
    public BasePlayer player;

    public enum TurnState
    {
        PROCESSING,
        ADDTOLIST,
        WAITING,
        ACTION,
        DEAD
    }
    public TurnState currentState;
    float currentCooldown = 0f;
    float maxCooldown = 5f;
    public Image ProgressBar;

    void Start()
    {
        currentState = TurnState.PROCESSING;
        battleManager = GameObject.Find("BattleManager").GetComponent<BattleStateMachine>();
    }

    void Update()
    {
        switch(currentState)
        {
            case TurnState.PROCESSING:
                UpdateProgressBar();
                break;
            case TurnState.ADDTOLIST:
                battleManager.playersToManage.Add(this.gameObject);
                currentState = TurnState.WAITING;
                break;
            case TurnState.WAITING:

                break;
            case TurnState.ACTION:

                break;
            case TurnState.DEAD:

                break;
        }
    }

    void UpdateProgressBar()
    {
        currentCooldown += Time.deltaTime;
        float percentCooldown = currentCooldown / maxCooldown;
        ProgressBar.transform.localScale = new Vector3(Mathf.Clamp(percentCooldown, 0, 1), ProgressBar.transform.localScale.y, ProgressBar.transform.localScale.z);
        if (currentCooldown >= maxCooldown)
        {
            currentState = TurnState.ADDTOLIST;
        }
    }
}
