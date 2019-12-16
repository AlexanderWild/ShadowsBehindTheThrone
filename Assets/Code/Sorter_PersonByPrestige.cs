using UnityEngine;

using System;
using System.Collections.Generic;

namespace Assets.Code
{
    public class Sorter_PersonByPrestige : IComparer<Person>
    {
        public string reason;
        public double amount;
        public int turn;

        public int Compare(Person x, Person y)
        {
            return Math.Sign(x.prestige - y.prestige);
        }
    }
}