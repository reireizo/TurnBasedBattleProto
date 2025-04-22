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

    public GameObject AttackPanel;
    public GameObject TargetPanel;


    void Start()
    {
        battleStates = PerformAction.WAIT;
        playerInput = PlayerGUI.ACTIVATE;
        EnemyParty.AddRange(GameObject.FindGameObjectsWithTag("BattleEnemy"));
        PlayerParty.AddRange(GameObject.FindGameObjectsWithTag("BattlePlayer"));

        AttackPanel.SetActive(false);
        TargetPanel.SetActive(false);

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
                    PlayerStateMachine playerMachine = performer.GetComponent<PlayerStateMachine>();
                    playerMachine.targetToAttack = PerformList[0].TargetGameObject;
                    playerMachine.currentState = PlayerStateMachine.TurnState.ACTION;
                }
                battleStates = PerformAction.PERFORMACTION;
                break;
            case PerformAction.PERFORMACTION:

                break;
        }

        switch (playerInput)
        {
            case PlayerGUI.ACTIVATE:
                if(playersToManage.Count > 0)
                {
                    playersToManage[0].transform.Find("Selector").gameObject.SetActive(true);
                    PlayerChoice = new HandleTurn();
                    AttackPanel.SetActive(true);
                    playerInput = PlayerGUI.WAITING;
                }
                break;
            case PlayerGUI.WAITING:

                break;
            case PlayerGUI.DONE:
                PlayerInputDone();
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
            buttonText.text = currentEnemy.enemy.actorName;

            button.TargetObject = enemy;
        }
    }

    public void AttackInput()
    {
        PlayerChoice.Attacker = playersToManage[0].name;
        PlayerChoice.AttacksGameObject = playersToManage[0];
        PlayerChoice.Type = "Player";

        AttackPanel.SetActive(false);
        TargetPanel.SetActive(true);
    }

    public void TargetInput(GameObject chosenEnemy)
    {
        PlayerChoice.TargetGameObject = chosenEnemy;
        playerInput = PlayerGUI.DONE;
    }

    void PlayerInputDone()
    {
        PerformList.Add(PlayerChoice);
        TargetPanel.SetActive(false);
        playersToManage[0].transform.Find("Selector").gameObject.SetActive(false);
        playersToManage.RemoveAt(0);
        playerInput = PlayerGUI.ACTIVATE;
    }
}
