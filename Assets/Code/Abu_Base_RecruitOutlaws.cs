using UnityEngine;


namespace Assets.Code
{
    public class Abu_Base_RecruitOutlaws: AbilityUnit
    {

        public override void castInner(Map map, Unit u)
        {
            u.hp += 1;
            if (u.hp > u.maxHp)
            {
                u.hp = u.maxHp;
            }

            u.location.map.world.prefabStore.popImgMsg(u.getName() + " recruits from " + u.location.getName() +
                ", replenishing their forces.",
                u.location.map.world.wordStore.lookup("ABILITY_RECRUIT_OUTLAWS"));
        }
        public override bool castable(Map map, Unit u)
        {
            if (u.hp == u.maxHp) { return false; }
            if (u.location.soc != null) { return false; }
            if (u.location.isOcean) { return false; }
            foreach (Location l2 in u.location.getNeighbours())
            {
                if (l2.soc is Society) { return true; }
            }
            return false;
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
            return 10;
        }

        public override string getDesc()
        {
            return "Recruits from the local population of outlaws and exiles, replenishing the forces of this agent (+1HP)"
                + "\n[Requires an empty non-ocean location next to a human society]";
        }

        public override string getName()
        {
            return "Recruit Outlaws";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_shield;
        }
    }
}