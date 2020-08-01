using UnityEngine;


namespace Assets.Code
{
    public class Abu_Doctor_SowCorpseroot: AbilityUnit
    {

        public override void castInner(Map map, Unit u)
        {

            SocialGroup sg = null;
            foreach (SocialGroup soc in map.socialGroups)
            {
                if (soc is SG_Undead)
                {
                    sg = soc;
                }
            }
            if (sg == null) { 
                sg = new SG_Undead(map, u.location);
                map.socialGroups.Add(sg);
            }
            u.location.soc = sg;
            u.location.settlement = new Set_Corpseroot(u.location,u.location.soc);
            u.location.units.Remove(u.location.settlement.embeddedUnit);

            foreach (Hex hex in u.location.territory)
            {
                if (map.landmass[hex.x][hex.y] && Eleven.random.NextDouble() < 0.5)
                {
                    hex.flora = new Flora_Corpseroot(hex);
                }
            }

            u.location.map.world.prefabStore.popImgMsg(u.getName() + " sows the ground with corpseroot, allowing the bodies they loot to be stored until they are raised back to life.",
                u.location.map.world.wordStore.lookup("ABILITY_DOCTOR_SOW_CORPSEROOT"));

        }
        public override bool castable(Map map, Unit u)
        {
            if (u.location.soc != null) { return false; }
            if (u.location.settlement != null) { return false; }
            if (u.location.isOcean) { return false; }
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
            return World.staticMap.param.unit_doctor_sowCorpserootCooldown;
        }

        public override string getDesc()
        {
            return "Plants a field of corpseroot, which will prevent the dead from decaying, keeping them ready until you wish to raise them as an undead army."
                + "\n[Requires an empty location]";
        }

        public override string getName()
        {
            return "Sow Corpseroot";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_corpseroot;
        }
    }
}