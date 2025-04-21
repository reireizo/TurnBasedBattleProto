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

    // Start is called before the first frame update
    void Start()
    {
        battleStates = PerformAction.WAIT;
        EnemyParty.AddRange(GameObject.FindGameObjectsWithTag("BattleEnemy"));
        PlayerParty.AddRange(GameObject.FindGameObjectsWithTag("BattlePlayer"));
    }

    // Update is called once per frame
    void Update()
    {
        switch (battleStates)
        {
            case PerformAction.WAIT:

                break;
            case PerformAction.TAKEACTION:

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
