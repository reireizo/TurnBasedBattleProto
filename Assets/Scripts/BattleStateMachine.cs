using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleStateMachine : MonoBehaviour
{

    public enum PerformAction
    {
        WAIT,
        TAKEACTION,
        PERFORMACTION
    }

    public PerformAction battleStates;

    public List<HandleTurn> PerformList = new List<HandleTurn>();
    public List<GameObject> PlayerParty = new List<GameObject>();
    public List<GameObject> EnemyParty = new List<GameObject>();

    void Start()
    {
        battleStates = PerformAction.WAIT;
        EnemyParty.AddRange(GameObject.FindGameObjectsWithTag("BattleEnemy"));
        PlayerParty.AddRange(GameObject.FindGameObjectsWithTag("BattlePlayer"));
    }

    void Update()
    {
        switch (battleStates)
        {
            case PerformAction.WAIT:
                if (PerformList.Count > 0)
                {
                    battleStates = PerformAction.TAKEACTION;
                }
                break;
            case PerformAction.TAKEACTION:
                GameObject performer = GameObject.Find(PerformList[0].Attacker);
                if (PerformList[0].Type == "Enemy")
                {
                    EnemyStateMachine enemyMachine = performer.GetComponent<EnemyStateMachine>();
                    enemyMachine.targetToAttack = PerformList[0].TargetGameObject;
                    enemyMachine.currentState = EnemyStateMachine.TurnState.ACTION;
                }
                if (PerformList[0].Type == "Player")
                {
                    
                }
                battleStates = PerformAction.PERFORMACTION;
                break;
            case PerformAction.PERFORMACTION:

                break;
        }
    }

    public void CollectActions(HandleTurn input)
    {
        PerformList.Add(input);
    }
}
