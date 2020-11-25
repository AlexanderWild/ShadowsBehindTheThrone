using UnityEngine;


namespace Assets.Code
{
    public class Ab_Fishman_Withdraw: Ability
    {
        public override void cast(Map map, Hex hex)
        {
            base.cast(map, hex);
            if (!castable(map, hex)) { return; }

            if (hex.location.soc != null && (hex.location.soc is SG_Fishmen))
            {
                SG_Fishmen flesh = (SG_Fishmen)hex.location.soc;
                flesh.warState = SG_UnholyFlesh.warStates.DEFEND; 

                map.world.prefabStore.popImgMsg(
                    "The Deep Ones return to the sea, to rebuild their strength, and defend their sunken temples.",
                    map.world.wordStore.lookup("ABILITY_FISH_WITHDRAW"));

                foreach (Unit u in map.units)
                {
                    if (u is Unit_Fishman)
                    {
                        //Reset your tasks so you immediately retreat
                        if (u.task is Task_GoToLocationAgressively)
                        {
                            u.task = null;
                        }
                    }
                }
            }

        }

        public override void playSound(AudioStore audioStore)
        {
            audioStore.playActivateFlesh();
        }

        public override bool castable(Map map, Hex hex)
        {
            if (hex.location == null) { return false; }
            if (hex.location.soc == null) { return false; }
            if (hex.location.soc is SG_Fishmen) { return true; }
            return false;
        }

        public override int getCost()
        {
            return 1;
        }

        public override string getDesc()
        {
            return "Causes the Deep Ones to immediately begin to return home to the sea."
                 + "\n[Requires an existing deep one location]";
        }

        public override string getName()
        {
            return "Withdraw to the sea";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_fishman;
        }
    }
}