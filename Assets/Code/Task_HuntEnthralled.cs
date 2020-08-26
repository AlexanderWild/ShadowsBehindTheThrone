using UnityEngine;

namespace Assets.Code
{
    public class Task_HuntEnthralled : Task
    {
        public Unit target;
        public int turnsLeft;

        public Task_HuntEnthralled(Unit prey)
        {
            this.target = prey;
        }

        public override string getShort()
        {
            return "Hunting " + target.getName();
        }

        public override string getLong()
        {
            return getShort();
        }

        public override void turnTick(Unit unit)
        {
            //Nothing, just used to get the UI on screen
        }
    }
}