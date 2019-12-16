using UnityEngine;

using System;

namespace Assets.Code
{
    public class RelEvent : IComparable<RelEvent>
    {
        public string reason;
        public double amount;
        public int turn;

        public int CompareTo(RelEvent other)
        {
            if (amount == other.amount) { return 0; }
            if (Math.Abs(amount) > Math.Abs(other.amount))
            {
                return -1;
            }
            else
            {
                return 1;
            }
        }
    }
}