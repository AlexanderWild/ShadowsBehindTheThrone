using UnityEngine;


namespace Assets.Code
{
    public class Ab_UnholyFlesh_Withdraw: Ability
    {
        public override void cast(Map map, Hex hex)
        {
            base.cast(map, hex);
            if (!castable(map, hex)) { return; }

            if (hex.location.soc != null && (hex.location.soc is SG_UnholyFlesh))
            {
                SG_UnholyFlesh flesh = (SG_UnholyFlesh)hex.location.soc;
                flesh.warState = SG_UnholyFlesh.warStates.DEFEND; 

                map.world.prefabStore.popImgMsg(
                    "The flesh beings its retreat, defending where it can, protecting its vulnerable ganglia",
                    map.world.wordStore.lookup("UNHOLY_FLESH_WITHDRAW"));

                foreach (Unit u in map.units)
                {
                    if (u is Unit_Flesh)
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
            if (hex.location.soc is SG_UnholyFlesh) { return true; }
            return false;
        }

        public override int getCost()
        {
            return 1;
        }

        public override string getDesc()
        {
            return "Causes flesh to cease its attack, and retreat until told to attack. Increases defence slowly on ganglia."
                 + "\n[Requires an existing unholy flesh location]";
        }

        public override string getName()
        {
            return "Withdraw the Flesh";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_ghoul;
        }
    }
}