using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    PlayerPanelStats stats;
    public GameObject PlayerPanel;
    Transform playerPanelSpacer;
    public Image ProgressBar;

    public GameObject selector;

    public GameObject targetToAttack;
    bool actionStarted = false;
    Vector3 startPosition;
    float animSpeed = 10f;

    bool alive = true;

    void Start()
    {
        playerPanelSpacer = GameObject.Find("BattleCanvas").transform.Find("PlayerPanel").Find("Spacer");
        CreatePlayerPanel();
        startPosition = transform.position;
        currentCooldown = UnityEngine.Random.Range(0f, 0.25f);
        selector.SetActive(false);
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
                StartCoroutine(TimeForAction());
                break;
            case TurnState.DEAD:
                if (!alive)
                {
                    return;
                }
                else
                {
                    this.gameObject.tag = "DeadPlayer";
                    battleManager.PlayerParty.Remove(this.gameObject);
                    if (this.gameObject == battleManager.playersToManage[0])
                    {
                        battleManager.ClearMagicPanel();
                    }
                    battleManager.playersToManage.Remove(this.gameObject);
                    selector.SetActive(false);
                    battleManager.AttackPanel.SetActive(false);
                    battleManager.MagicPanel.SetActive(false);
                    battleManager.TargetPanel.SetActive(false);
                    if (battleManager.PlayerParty.Count > 0)
                    {
                        for(int i = 0; i < battleManager.PerformList.Count; i++)
                        {
                            if(battleManager.PerformList[i].AttacksGameObject == this.gameObject)
                            {
                                battleManager.PerformList.Remove(battleManager.PerformList[i]);
                            }
                            if(battleManager.PerformList[i].TargetGameObject == this.gameObject)
                            {
                                battleManager.PerformList[i].TargetGameObject = battleManager.PlayerParty[UnityEngine.Random.Range(0, battleManager.PlayerParty.Count)];
                            }
                        }
                    }
                    this.gameObject.GetComponent<MeshRenderer>().material.color = new Color32(105, 105, 105, 255);
                    battleManager.battleStates = BattleStateMachine.PerformAction.CHECKALIVE;
                    alive = false;
                }
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

    IEnumerator TimeForAction()
    {
        if (actionStarted)
        {
            yield break;
        }

        actionStarted = true;
        Vector3 targetPosition = new Vector3(targetToAttack.transform.position.x + 1.5f, targetToAttack.transform.position.y, targetToAttack.transform.position.z);
        while(MoveTowardsTarget(targetPosition))
        {
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);
        DoDamage(battleManager.PerformList[0].ChosenAttack);
        Vector3 firstPosition = startPosition;
        while(MoveTowardsStart(firstPosition))
        {
            yield return null;
        }
        if (battleManager.battleStates != BattleStateMachine.PerformAction.WIN)
        {
            battleManager.PerformList.RemoveAt(0);
            battleManager.battleStates = BattleStateMachine.PerformAction.WAIT;
            actionStarted = false;
            currentCooldown = 0f;
            currentState = TurnState.PROCESSING;
        }
    }

    bool MoveTowardsTarget(Vector3 target)
    {
        return target != (transform.position = Vector3.MoveTowards(transform.position, target, animSpeed * Time.deltaTime));
    }
    bool MoveTowardsStart(Vector3 target)
    {
        return target != (transform.position = Vector3.MoveTowards(transform.position, target, animSpeed * Time.deltaTime));
    }

    public void TakeDamage(float getDamageAmount)
    {
        player.currentHP -= getDamageAmount;
        Debug.Log(player.actorName + " has taken " + getDamageAmount + " damage!");
        if (player.currentHP <= 0)
        {
            player.currentHP = 0;
            currentState = TurnState.DEAD;
        }
        UpdateHPBar();
    }

    void CreatePlayerPanel()
    {
        PlayerPanel = Instantiate(PlayerPanel, playerPanelSpacer);
        stats = PlayerPanel.GetComponent<PlayerPanelStats>();
        stats.playerName.text = player.actorName;
        UpdateHPBar();
        UpdateMPBar();
        ProgressBar = stats.playerProgressImage;
    }

    void UpdateHPBar()
    {
        stats.playerHP.text = player.currentHP.ToString();
        float percentHP = player.currentHP / player.maxHP;
        stats.playerHPImage.transform.localScale = new Vector3(Mathf.Clamp(percentHP, 0, 1), stats.playerHPImage.transform.localScale.y, stats.playerHPImage.transform.localScale.z);
    }

    void UpdateMPBar()
    {
        stats.playerMP.text = player.currentMP.ToString();
        float percentMP = player.currentMP / player.maxMP;
        stats.playerMPImage.transform.localScale = new Vector3(Mathf.Clamp(percentMP, 0, 1), stats.playerMPImage.transform.localScale.y, stats.playerMPImage.transform.localScale.z);
    }

    void DoDamage(BaseAttack usedAttack)
    {
        EnemyStateMachine targetEnemy = targetToAttack.GetComponent<EnemyStateMachine>();
        float baseDamage;
        float damageSkillModded;
        float finalDamage;
        switch(usedAttack.attackType)
        {
            case BaseAttack.StatType.PHYS:
                baseDamage = player.currentATK - (targetEnemy.enemy.currentDEF /2);
                damageSkillModded = baseDamage * (usedAttack.attackDamage / 100f);
                finalDamage = damageSkillModded * (player.strength / 10f);
                targetEnemy.TakeDamage(finalDamage);
                break;
            case BaseAttack.StatType.MAGIC:
                baseDamage = player.currentMAT - (targetEnemy.enemy.currentMDF /2);
                damageSkillModded = baseDamage * (usedAttack.attackDamage / 100f);
                finalDamage = damageSkillModded * (player.magic / 10f);
                targetEnemy.TakeDamage(finalDamage);
                break;
        }
    }
}
