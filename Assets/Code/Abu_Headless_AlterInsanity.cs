using System.Collections.Generic;
using UnityEngine;


namespace Assets.Code
{
    public class Abu_Headless_AlterInsanity: AbilityUnit
    {

        public override void castInner(Map map, Unit u)
        {
            Unit_HeadlessHorseman head = (Unit_HeadlessHorseman)u;

            u.location.person().madness = new Insanity_Paranoid();

            u.location.map.world.prefabStore.popImgMsg("The horseman's haunting has caused " + u.location.person().getFullName() + " to become paranoid. They will begin to suspect other nobles, " +
                "and may well vote to have them executed.",
                u.location.map.world.wordStore.lookup("ABILITY_HEADLESS_ALTER_INSANITY"),img:3);

        }
        public override bool castable(Map map, Unit u)
        {
            if (u.location.person() == null) { return false; }
            if (u.location.person().sanity > 0) { return false; }
            if (u.location.person().madness is Insanity_Paranoid) { return false; }
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

        public override string getDesc()
        {
            return "Causes a noble who is currently insane (use other agents, creatures or enthralled to create madness) to become paranoid, so they accuse each other, allowing trials to succeed."
                + "\n[Requires an insane noble at your location who is not paranoid]";
        }

        public override string getName()
        {
            return "Alter Insanity";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_pumpkin;
        }
    }
}