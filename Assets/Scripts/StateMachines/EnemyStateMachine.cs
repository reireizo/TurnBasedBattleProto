using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        battleManager.PerformList.RemoveAt(0);
        battleManager.battleStates = BattleStateMachine.PerformAction.WAIT;
        actionStarted = false;
        currentCooldown = 0f;
        currentState = TurnState.PROCESSING;
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
        float baseDamage = enemy.currentATK - (targetPlayer.player.currentDEF /2);
        float damageSkillModded = baseDamage * (usedAttack.attackDamage / 100f);
        float finalDamage = damageSkillModded * (10f / targetPlayer.player.vitality);
        targetPlayer.TakeDamage(finalDamage);
    }
}
