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
    public GameObject selector;

    public GameObject targetToAttack;
    bool actionStarted = false;
    Vector3 startPosition;
    float animSpeed = 10f;

    void Start()
    {
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
        //do damage
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

    public void TakeDamage(float getDamageAmount)
    {
        player.currentHP -= getDamageAmount;
        Debug.Log(player.actorName + " has taken " + getDamageAmount + " damage!");
        if (player.currentHP <= 0)
        {
            currentState = TurnState.DEAD;
        }
    }
}
