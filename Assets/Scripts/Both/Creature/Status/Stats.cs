using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Both.Creature.Status
{
    [Serializable]
    public abstract class Stats : IStats
    {
        [SerializeField] private int current;
        [SerializeField] private int max;
        [SerializeField] private int temporary;
        private float temporaryReset;

        protected int Current 
        { 
            get => current; 
            set 
            {
                var old = current;
                var n = Mathf.Clamp(value, 0, Max);

                if (n == old) return;

                current = n;
                OnStatsChange?.Invoke(this, new StatsChangeEventArgs(old + Temporary, current + Temporary));
            }
        }

        protected int Max 
        { 
            get => max; 
            set => max = value; 
        }

        protected int Temporary 
        { 
            get => temporary; 
            set
            {
                var old = temporary;

                if (value == old) return;

                if (value + Current < 0) //negative stats
                {
                    temporary = -Current;
                    //StartCoroutine(ResetTemporary(temporary, temporaryReset));
                    OnStatsChange?.Invoke(this, new StatsChangeEventArgs(old + Current, temporary + Current));
                    return;
                }

                temporary = value;
                //StartCoroutine(ResetTemporary(temporary, temporaryReset));
                OnStatsChange?.Invoke(this, new StatsChangeEventArgs(old + Current, temporary + Current));
            }
        }

        protected Stats(int max)
        {
            this.max = max;
            current = this.max;
            temporary = 0;
        }

        public int GetValue(bool current = true)
        {
            return current ? Current + Temporary : Max;
        }

        public void SetValue(int addValue)
        {
            Current += addValue;
        }

        public void SetTemporary(int addValue, float secs)
        {
            temporaryReset = secs;
            Temporary += addValue;
        }

        private IEnumerator ResetTemporary(int reset, float secs)
        {
            yield return new WaitForSecondsRealtime(secs);

            temporary -= reset;
        }

        public event EventHandler<StatsChangeEventArgs> OnStatsChange;
    }

    public class StatsChangeEventArgs : EventArgs
    {
        public readonly int OldValue;
        public readonly int NewValue;

        public StatsChangeEventArgs(int oldValue, int newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }
    }

    public interface IStats 
    {
        int GetValue(bool current = true);
        void SetValue(int addValue);
        void SetTemporary(int addValue, float secs);

        event EventHandler<StatsChangeEventArgs> OnStatsChange;
    }
}