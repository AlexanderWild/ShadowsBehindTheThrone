using UnityEngine;


namespace Assets.Code
{
    public class SG_AgentDark : Society
    {
        public SG_AgentDark(Map map) : base(map)
        {
            name = "The Darkness";
            color = Color.black;
            color2 = Color.black;
        }
        public override bool hasEnthralled()
        {
            return true;
        }

        public override string getName()
        {
            return "The Dark";
        }

        public override bool hostileTo(Unit u)
        {
            return false;
        }

        public override void turnTick()
        {
        }
    }
}