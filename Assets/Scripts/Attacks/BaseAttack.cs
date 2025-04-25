using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BaseAttack : MonoBehaviour
{
    public string attackName;
    public string attackDescription;
    public enum StatType
    {
        PHYS,
        MAGIC
    }
    public StatType attackType;
    public float attackDamage;
    public float attackCost;
}
