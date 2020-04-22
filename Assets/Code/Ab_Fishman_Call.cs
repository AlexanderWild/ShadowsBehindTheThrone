using UnityEngine;

using System;

namespace Assets.Code
{
    public class Ab_Fishman_Call: Ability
    {
        public override void cast(Map map, Hex hex)
        {
            base.cast(map, hex);
            if (!castable(map, hex)) { return; }

            Settlement raidTarget = null;
            SG_Fishmen receiver = null;
            foreach (Location l in hex.location.getNeighbours())
            {
                if (l.soc is Society && l.settlement != null)
                {
                    raidTarget = l.settlement;
                }
                if (l.soc is SG_Fishmen)
                {
                    receiver = (SG_Fishmen)l.soc;
                }
            }
            double start = receiver.currentMilitary;
            receiver.currentMilitary = Math.Min(receiver.currentMilitary + map.param.ability_fishmanRaidMilAdd, receiver.maxMilitary);
            receiver.temporaryThreat += map.param.ability_fishmanRaidTemporaryThreat;
            double delta = receiver.currentMilitary - start;

            map.world.prefabStore.popImgMsg("You call to the land, and enough land dweller slip into the sea to add " + (int)(delta+0.4) + " military might to your Deep Ones.",
                map.world.wordStore.lookup("ABILITY_FISHMAN_CALL"));
        }

        public override void playSound(AudioStore audioStore)
        {
            audioStore.playActivateFishmen();
        }

        public override bool castable(Map map, Hex hex)
        {
            if (hex.location == null) { return false; }
            if (hex.location.soc == null) { return false; }
            if (!(hex.location.soc is Society)) { return false; }
            if (hex.location.settlement == null) { return false; }

            foreach (Location l in hex.location.getNeighbours())
            {
                if (l.soc != null && (l.soc is SG_Fishmen))
                {
                    return true;
                }
            }
            return false;
        }

        public override int getCost()
        {
            return World.staticMap.param.ability_fishmanRaidCost;
        }

        public override string getDesc()
        {
            return "Deep One sirens lure humans from coastal cities to turn into Deep One soldiers. Adds " + World.staticMap.param.ability_fishmanRaidMilAdd + " military, but adds "
                + World.staticMap.param.ability_fishmanRaidTemporaryThreat + " temporary threat."
                 + "\n[Requires a human settlement bordering a Deep One-held location]";
        }

        public override string getName()
        {
            return "Call of the Deep";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_fishman;
        }
    }
}