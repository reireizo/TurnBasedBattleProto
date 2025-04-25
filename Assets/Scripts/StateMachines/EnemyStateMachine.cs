using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Comparers;

public class EnemyStateMachine : MonoBehaviour
{
    BattleStateMachine battleManager;
    public BaseEnemy enemy;

    public enum TurnState
    {
        PROCESSING,
        CHOOSEACTION,
        WAITING,
        ACTION,
        DEAD
    }
    public TurnState currentState;
    [SerializeField]float currentCooldown = 0f;
    float maxCooldown = 7f;
    public GameObject selector;

    Vector3 startPosition;
    bool actionStarted = false;
    public GameObject targetToAttack;
    float animSpeed = 10f;
    bool alive = true;

    void Start()
    {
        selector.SetActive(false);
        currentState = TurnState.PROCESSING;
        battleManager = GameObject.Find("BattleManager").GetComponent<BattleStateMachine>();
        startPosition = transform.position;
    }

    void Update()
    {
        switch(currentState)
        {
            case TurnState.PROCESSING:
                UpdateProgressBar();
                break;
            case TurnState.CHOOSEACTION:
                ChooseAction();
                currentState = TurnState.WAITING;
                break;
            case TurnState.WAITING:

                break;
            case TurnState.ACTION:
                StartCoroutine (TimeForAction());
                break;
            case TurnState.DEAD:
                if (!alive)
                {
                    return;
                }
                else
                {
                    this.gameObject.tag = "DeadEnemy";
                    battleManager.EnemyParty.Remove(this.gameObject);
                    selector.SetActive(false);
                    if (battleManager.EnemyParty.Count > 0)
                    {
                        for(int i = 0; i < battleManager.PerformList.Count; i++)
                        {
                            if(battleManager.PerformList[i].AttacksGameObject == this.gameObject)
                            {
                                battleManager.PerformList.Remove(battleManager.PerformList[i]);
                            }
                            if(battleManager.PerformList[i].TargetGameObject == this.gameObject)
                            {
                                battleManager.PerformList[i].TargetGameObject = battleManager.EnemyParty[UnityEngine.Random.Range(0, battleManager.EnemyParty.Count)];
                            }
                        }
                    }
                    this.gameObject.GetComponent<MeshRenderer>().material.color = new Color32(105, 105, 105, 255);
                    battleManager.CreateTargetButtons();
                    battleManager.battleStates = BattleStateMachine.PerformAction.CHECKALIVE;
                    alive = false;

                }
                break;
        }
    }

    void UpdateProgressBar()
    {
        currentCooldown += Time.deltaTime;
        if (currentCooldown >= maxCooldown)
        {
            currentState = TurnState.CHOOSEACTION;
        }
    }

    void ChooseAction()
    {
        HandleTurn myAttack = new HandleTurn();
        myAttack.Attacker = enemy.actorName;
        myAttack.Type = "Enemy";
        myAttack.AttacksGameObject = this.gameObject;
        myAttack.TargetGameObject = battleManager.PlayerParty[Random.Range(0, battleManager.PlayerParty.Count)];

        int num = Random.Range(0, enemy.AttackList.Count);
        myAttack.ChosenAttack = enemy.AttackList[num];
        Debug.Log(this.gameObject.name + " has chosen " + myAttack.ChosenAttack.attackName + " on " + myAttack.TargetGameObject.name);

        battleManager.CollectActions(myAttack);
    }

    IEnumerator TimeForAction()
    {
        if (actionStarted)
        {
            yield break;
        }

        actionStarted = true;
        Vector3 targetPosition = new Vector3(targetToAttack.transform.position.x - 1.5f, targetToAttack.transform.position.y, targetToAttack.transform.position.z);
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
        if (battleManager.battleStates != BattleStateMachine.PerformAction.LOSE)
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

    void DoDamage(BaseAttack usedAttack)
    {
        PlayerStateMachine targetPlayer = targetToAttack.GetComponent<PlayerStateMachine>();
        float baseDamage;
        float damageSkillModded;
        float finalDamage;
        switch(usedAttack.attackType)
        {
            case BaseAttack.StatType.PHYS:
                baseDamage = Mathf.Floor(enemy.currentATK - (targetPlayer.player.currentDEF /2));
                damageSkillModded = Mathf.Floor(baseDamage * (usedAttack.attackDamage / 100f));
                finalDamage = Mathf.Floor(damageSkillModded * (10f / targetPlayer.player.vitality));
                targetPlayer.TakeDamage(finalDamage);
                break;
            case BaseAttack.StatType.MAGIC:
                baseDamage = Mathf.Floor(enemy.currentMAT - (targetPlayer.player.currentMDF /2));
                damageSkillModded = Mathf.Floor(baseDamage * (usedAttack.attackDamage / 100f));
                finalDamage = Mathf.Floor(damageSkillModded * (10f / targetPlayer.player.magic));
                targetPlayer.TakeDamage(finalDamage);
                break;
        }
    }

    public void TakeDamage(float getDamageAmount)
    {
        enemy.currentHP -= getDamageAmount;
        Debug.Log(enemy.actorName + " has taken " + getDamageAmount + " damage!");
        if (enemy.currentHP <= 0)
        {
            enemy.currentHP = 0;
            currentState = TurnState.DEAD;
        }
    }
}
