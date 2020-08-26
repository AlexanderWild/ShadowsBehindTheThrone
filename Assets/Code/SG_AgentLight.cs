using UnityEngine;


namespace Assets.Code
{
    public class SG_AgentLight : Society
    {
        public SG_AgentLight(Map map) : base(map)
        {
            name = "The Light";
            color = new Color(1f, 1f, 0.7f);
            color2 = Color.white;
        }
        public override bool hasEnthralled()
        {
            return false;
        }

        public override string getName()
        {
            return "The Light";
        }

        public override bool hostileTo(Unit u)
        {
            if (u.isEnthralled()) { return true; }
            return false;
        }

        public override void turnTick()
        {
        }
    }
}