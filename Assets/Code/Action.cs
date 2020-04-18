using UnityEngine;

namespace Assets.Code
{
    public abstract class Action
    {
        public abstract string getShort();
        public abstract string getLong();
        public abstract void turnTick(Person person);
    }
}