using UnityEngine;


namespace Assets.Code
{
    public class Abu_Base_Recruit: AbilityUnit
    {

        public override void castInner(Map map, Unit u)
        {
            u.hp += 1;
            if (u.hp > u.maxHp)
            {
                u.hp = u.maxHp;
            }
            u.location.map.world.prefabStore.popImgMsg(u.getName() + " recruits from " + u.location.getName() + ", replenishing their forces. They are now at " + u.hp + "/" + u.maxHp + ".",
                u.location.map.world.wordStore.lookup("ABILITY_RECRUIT"));

        }
        public override bool castable(Map map, Unit u)
        {
            if (u.hp == u.maxHp) { return false; }
            if (u.location.settlement == null) { return false; }
            if (u.location.soc is Society == false) { return false; }
            return true;
        }

        public override void cast(Map map, Hex hex) { }
        public override bool castable(Map map, Hex hex)
        {
            return false;
        }

        public override int getCost()
        {
            return 0;
        }

        public override int getCooldown()
        {
            return 3;
        }

        public override string getDesc()
        {
            return "Recruits from the local population, replenishing the forces of this agent (+1HP)"
                + "\n[Requires a human settlement in a society]";
        }

        public override string getName()
        {
            return "Recruit";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_shield;
        }
    }
}