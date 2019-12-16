using UnityEngine;
using System.Collections;

namespace Assets.Code
{
    public class KillOrder
    {
        public Person person;
        public string reason;

        public KillOrder(Person person, string v)
        {
            this.person = person;
            this.reason = v;
        }
    }
}
