using UnityEngine;


namespace Assets.Code
{
    public class Abu_Doctor_WarAgainstLife : AbilityUnit
    {
        public override void cast(Map map, Unit u)
        {
            SG_Undead theDead = (SG_Undead)u.location.soc;
            foreach (SocialGroup sg in map.socialGroups){
                if (sg is Society)
                {
                    if (theDead.getRel(sg).state != DipRel.dipState.war)
                    {
                        map.declareWar(theDead, sg);
                    }
                }
            }

            double amount = 1;
            Evidence e2 = new Evidence(map.turn);
            e2.pointsTo = u;
            e2.weight = amount;
            u.location.evidence.Add(e2);

            u.location.map.world.prefabStore.popImgMsg(u.getName() + " calls upon the dead to rise up, and begin their war against life.",
                u.location.map.world.wordStore.lookup("ABILITY_DOCTOR_WAR_AGAINST_LIFE"));
        }

        public override void playSound(AudioStore audioStore)
        {
            audioStore.playActivateFlesh();
        }

        public override bool castable(Map map, Unit u)
        {
            Hex hex = u.location.hex;
            if (hex.location == null) { return false; }
            if (hex.location.soc == null) { return false; }
            if (!(hex.location.soc is SG_Undead)) { return false; }
            if (hex.location.settlement == null) { return false; }
            if (!(hex.location.settlement is Set_Corpseroot)) { return false; }
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
        public override string specialCost()
        {
            return " ";
        }


        public override string getDesc()
        {
            return "Causes the dead to rise up, attacking all human societies. They may fail to kill all life, but can wear down the nations city by city."
                 + "\n[Requires a corpseroot location]";
        }

        public override string getName()
        {
            return "War Against Life";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_ghoul;
        }
    }
}