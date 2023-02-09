using Assets.Scripts.Both.Creature;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCalculate : MonoBehaviour
{
    public static DamageCalculate Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    public void DamageTo(ICreature creature, int damage)
    {
        //Debug.Log(creature.GetStats(Assets.Scripts.Both.Scriptable.StatsType.Health).GetValue() + " " + damage);
        creature.GetStats(Assets.Scripts.Both.Scriptable.StatsType.Health).SetValue(-damage);
    }
}
