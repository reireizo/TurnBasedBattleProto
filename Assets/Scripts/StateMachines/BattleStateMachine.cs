using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleStateMachine : MonoBehaviour
{

    public enum PerformAction
    {
        WAIT,
        TAKEACTION,
        PERFORMACTION,
        CHECKALIVE,
        WIN,
        LOSE
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
    public Transform targetSpacer;
    public Transform actionSpacer;
    public Transform magicSpacer;

    public GameObject AttackPanel;
    public GameObject TargetPanel;
    public GameObject MagicPanel;

    public GameObject actionButton;
    public GameObject magicButton;

    //public List<GameObject> AttackButtons = new List<GameObject>();
    public List<GameObject> MagicButtons = new List<GameObject>();
    public List<GameObject> TargetButtons = new List<GameObject>();


    void Start()
    {
        battleStates = PerformAction.WAIT;
        playerInput = PlayerGUI.ACTIVATE;
        EnemyParty.AddRange(GameObject.FindGameObjectsWithTag("BattleEnemy"));
        PlayerParty.AddRange(GameObject.FindGameObjectsWithTag("BattlePlayer"));

        CreateAttackButtons();

        AttackPanel.SetActive(false);
        TargetPanel.SetActive(false);
        MagicPanel.SetActive(false);

        CreateTargetButtons();
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
                    int loops = 0;
                    for (int i = 0; i < PlayerParty.Count; i++)
                    {
                        if(PerformList[0].TargetGameObject == PlayerParty[i])
                        {
                            enemyMachine.targetToAttack = PerformList[0].TargetGameObject;
                            enemyMachine.currentState = EnemyStateMachine.TurnState.ACTION;
                            break;
                        }
                        else
                        {
                            loops += 1;
                            if (loops == PlayerParty.Count)
                            {
                                PerformList[0].TargetGameObject = PlayerParty[Random.Range(0, PlayerParty.Count)];
                                enemyMachine.targetToAttack = PerformList[0].TargetGameObject;
                                enemyMachine.currentState = EnemyStateMachine.TurnState.ACTION;
                            }
                        }
                    }
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

            case PerformAction.CHECKALIVE:
                if (PlayerParty.Count < 1)
                {
                    battleStates = PerformAction.LOSE;
                }
                else if (EnemyParty.Count < 1)
                {
                    battleStates = PerformAction.WIN;
                }
                else
                {
                    playerInput = PlayerGUI.ACTIVATE;
                    battleStates = PerformAction.WAIT;
                }
                break;
            case PerformAction.WIN:
                PerformList.Clear();
                foreach (GameObject alivePlayer in PlayerParty)
                {
                    PlayerStateMachine playerMachine = alivePlayer.GetComponent<PlayerStateMachine>();
                    playerMachine.currentState = PlayerStateMachine.TurnState.WAITING;
                    GameManager.Instance.LoadMainMenu();
                }
                break;
            case PerformAction.LOSE:
                PerformList.Clear();
                foreach (GameObject aliveEnemy in EnemyParty)
                {
                    EnemyStateMachine playerMachine = aliveEnemy.GetComponent<EnemyStateMachine>();
                    playerMachine.currentState = EnemyStateMachine.TurnState.WAITING;
                    GameManager.Instance.LoadMainMenu();
                }
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

    public void CreateTargetButtons()
    {
        foreach (GameObject button in TargetButtons)
        {
            Destroy(button);
        }
        TargetButtons.Clear();
        foreach(GameObject enemy in EnemyParty)
        {
            GameObject newButton = Instantiate(targetButton, targetSpacer);
            TargetButton button = newButton.GetComponent<TargetButton>();

            EnemyStateMachine currentEnemy = enemy.GetComponent<EnemyStateMachine>();

            TMP_Text buttonText = newButton.GetComponentInChildren<TMP_Text>();
            buttonText.text = currentEnemy.enemy.actorName;

            button.TargetObject = enemy;
            TargetButtons.Add(newButton);
        }
    }

    public void AttackInput()
    {
        PlayerChoice.Attacker = playersToManage[0].name;
        PlayerChoice.AttacksGameObject = playersToManage[0];
        PlayerChoice.Type = "Player";
        PlayerChoice.ChosenAttack = playersToManage[0].GetComponent<PlayerStateMachine>().player.AttackList[0];

        AttackPanel.SetActive(false);
        TargetPanel.SetActive(true);
    }

    public void MagicInput()
    {
        if (playersToManage[0].GetComponent<PlayerStateMachine>().player.knownSpells.Count > 0)
        {
            CreateMagicButtons();
            PlayerChoice.Attacker = playersToManage[0].name;
            PlayerChoice.Type = "Player";

            AttackPanel.SetActive(false);
            MagicPanel.SetActive(true);
        }
    }

    public void SpellInput(BaseAttack chosenSpell)
    {
        PlayerChoice.ChosenAttack = chosenSpell;
        MagicPanel.SetActive(false);
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
        ClearMagicPanel();
        playersToManage[0].transform.Find("Selector").gameObject.SetActive(false);
        playersToManage.RemoveAt(0);
        playerInput = PlayerGUI.ACTIVATE;
    }

    public void ClearMagicPanel()
    {
        foreach (GameObject button in MagicButtons)
        {
            Destroy(button);
        }
    }
    /*public void ClearTargetPanel()
    {
        foreach (GameObject button in TargetButtons)
        {
            Destroy(button);
        }
    }*/

    void CreateAttackButtons()
    {
        GameObject AttackButton = Instantiate(actionButton, actionSpacer);
        TMP_Text AttackButtonText = AttackButton.transform.Find("ActionText").GetComponent<TMP_Text>();
        AttackButtonText.text = "Attack";
        AttackButton.GetComponent<Button>().onClick.AddListener(() => AttackInput());
        //AttackButtons.Add(AttackButton);

        GameObject MagicButton = Instantiate(actionButton, actionSpacer);
        TMP_Text MagicButtonText = MagicButton.transform.Find("ActionText").GetComponent<TMP_Text>();
        MagicButtonText.text = "Magic";
        MagicButton.GetComponent<Button>().onClick.AddListener(() => MagicInput());
    }

    void CreateMagicButtons()
    {
        foreach (BaseAttack spell in playersToManage[0].GetComponent<PlayerStateMachine>().player.knownSpells)
        {
            GameObject MagicButton = Instantiate(magicButton, magicSpacer);
            TMP_Text MagicButtonText = MagicButton.transform.Find("ActionText").GetComponent<TMP_Text>();
            MagicButtonText.text = spell.attackName;
            MagicAttackButton buttonScript = MagicButton.GetComponent<MagicAttackButton>();
            buttonScript.magicAttackToPerform = spell;
            MagicButtons.Add(MagicButton);
        }
    }
}
