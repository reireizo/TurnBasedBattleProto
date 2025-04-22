using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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

    public enum PlayerGUI
    {
        ACTIVATE,
        WAITING,
        INPUT1,
        INPUT2,
        DONE
    }
    public PlayerGUI playerInput;
    public List<GameObject> playersToManage = new List<GameObject>();
    HandleTurn PlayerChoice;
    public GameObject targetButton;
    public Transform spacer;


    void Start()
    {
        battleStates = PerformAction.WAIT;
        EnemyParty.AddRange(GameObject.FindGameObjectsWithTag("BattleEnemy"));
        PlayerParty.AddRange(GameObject.FindGameObjectsWithTag("BattlePlayer"));

        TargetButtons();
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

    void TargetButtons()
    {
        foreach(GameObject enemy in EnemyParty)
        {
            GameObject newButton = Instantiate(targetButton, spacer);
            TargetButton button = newButton.GetComponent<TargetButton>();

            EnemyStateMachine currentEnemy = enemy.GetComponent<EnemyStateMachine>();

            TMP_Text buttonText = newButton.GetComponentInChildren<TMP_Text>();
            buttonText.text = currentEnemy.enemy.name;

            button.TargetObject = enemy;
        }
    }
}
