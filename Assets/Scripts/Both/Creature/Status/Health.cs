using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Both.Creature.Status
{
    public class Health : Stats
    {
        [SerializeField] private bool isDead;

        public Health(int max) : base(max)
        {
            isDead = false;

            OnStatsChange += OnDead;
        }

        private void OnDead(object sender, StatsChangeEventArgs e)
        {
            if (e.NewValue == 0)
            {
                isDead = true;
            }

            OnDeadEvent?.Invoke(isDead);
        }

        public event Action<bool> OnDeadEvent;
    }
}