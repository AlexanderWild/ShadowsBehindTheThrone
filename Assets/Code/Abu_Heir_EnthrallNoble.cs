using UnityEngine;


namespace Assets.Code
{
    public class Abu_Heir_EnthrallNoble: AbilityUnit
    {

        public override void castInner(Map map, Unit u)
        {
            if (map.overmind.enthralled != null)
            {
                map.world.prefabStore.popMsg("You already have an enthralled noble, and may only have one at a time. Use apoptosis to kill them if you need a new one.");
                return;
            }


            if (u.location.person() != null)
            {
                u.location.person().state = Person.personState.enthralled;
                map.overmind.enthralled = u.location.person();

                AchievementManager.unlockAchievement(SteamManager.achievement_key.CULT_GROWS);
            }
            else
            {

                map.world.prefabStore.popMsg("No noble present in this location.");
                return;
            }

            u.location.map.world.prefabStore.popImgMsg(u.getName() + " brings " + u.location.person().getFullName() + " under your command, enthralling them to your will.",
                u.location.map.world.wordStore.lookup("ABILITY_UNIT_ENTHRALL_NOBLE"),4);

            map.hintSystem.popHint(HintSystem.hintType.ENTHRALLED_NOBLES);
        }
        public override bool castable(Map map, Unit u)
        {
            if (u.person == null) { return false; }
            if (u.location.person() == null) { return false; }
            if (u.location.settlement == null) { return false; }
            if (u.location.map.overmind.enthralled != null) { return false; }
            if (u.location.settlement.infiltration < World.staticMap.param.ability_unit_enthrallNobleReq) { return false; }
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
            return 0;
        }
        public override string specialCost()
        {
            return " ";
        }


        public override string getDesc()
        {
            return "Takes full control of a noble, allowing you to vote through them, and to access the advanced abilities associated with aristocratic society."
                + "\n[Requires no other enthralled noble and infiltration above " + (int)(100*World.staticMap.param.ability_unit_enthrallNobleReq) + "%]";
        }

        public override string getName()
        {
            return "Enthrall Noble";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_enshadow;
        }
    }
}