using UnityEngine;

namespace Assets.Code
{
    public abstract class Task
    {
        public abstract string getShort();
        public abstract string getLong();
        public abstract void turnTick(Unit unit);

        public virtual bool isBusy()
        {
            return true;
        }
    }
}