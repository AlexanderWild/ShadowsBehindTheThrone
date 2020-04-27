using UnityEngine;

using System;

namespace Assets.Code
{
    public class Ab_Fash_InstillDread: Ability
    {
        public override void cast(Map map, Hex hex)
        {
            base.cast(map, hex);
            castInner(map, hex.location.person());
        }
        public override void castInner(Map map, Person person)
        {

            ThreatItem item = person.getGreatestThreat();
            if (item == null) { return; }

            double prevThreat = item.threat;
            double addDread = item.threat * map.param.ability_instillDreadMult;
            bool hitCap = false;
            if (item.threat + addDread >= 200)
            {
                hitCap = true;
                item.threat = 200 - item.threat;//Now at 200
            }
            else
            {
                item.temporaryDread += addDread;
                item.threat += addDread;
            }

            string msgs = "You draw upon " + person.getFullName() + "'s fears, adding to their dread of " + item.getTitle() + ". Their threat estimate has gone from "
                + (int)(prevThreat) + " to " + (int)(item.threat) + ".";
            if (hitCap) { msgs += "\n(It is now at its maximum value)"; }
            map.world.prefabStore.popImgMsg(
                msgs,
                map.world.wordStore.lookup("ABILITY_INSTILL_DREAD"));
        }

        public override bool castable(Map map, Person person)
        {
            return true;
        }

        public override bool castable(Map map, Hex hex)
        {
            if (hex.location == null) { return false; }
            if (hex.location.person() == null) { return false; }
            return castable(map, hex.location.person());
        }

        public override int getCooldown()
        {
            return World.staticMap.param.ability_instillDreadCooldown;
        }
        public override int getCost()
        {
            return World.staticMap.param.ability_instillDreadCost;
        }

        public override string getDesc()
        {
            return "Causes a person to gain additional temporary dread towards their greatest fear, proportional to the amount of threat they currently believe this thing to pose."
                + "\n[Requires a noble]";
        }

        public override string getName()
        {
            return "Instill Dread";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_axes;
        }
    }
}