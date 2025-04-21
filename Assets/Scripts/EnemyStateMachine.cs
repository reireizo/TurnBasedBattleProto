using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateMachine : MonoBehaviour
{
    public BaseEnemy enemy;

    public enum TurnState
    {
        PROCESSING,
        ADDTOLIST,
        WAITING,
        SELECTING,
        ACTION,
        DEAD
    }
    public TurnState currentState;
    float currentCooldown = 0f;
    float maxCooldown = 7f;

    void Start()
    {
        currentState = TurnState.PROCESSING;
    }

    void Update()
    {
        switch(currentState)
        {
            case TurnState.PROCESSING:
                UpdateProgressBar();
                break;
            case TurnState.ADDTOLIST:

                break;
            case TurnState.WAITING:

                break;
            case TurnState.SELECTING:

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
        if (currentCooldown >= maxCooldown)
        {
            currentState = TurnState.ADDTOLIST;
        }
    }
}
