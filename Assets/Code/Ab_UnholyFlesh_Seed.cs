using UnityEngine;


namespace Assets.Code
{
    public class Ab_UnholyFlesh_Seed: Ability
    {
        public override void cast(Map map, Hex hex)
        {
            base.cast(map, hex);
            if (!castable(map, hex)) { return; }


            SG_UnholyFlesh soc = null;
            foreach (SocialGroup sg in map.socialGroups)
            {
                if (sg is SG_UnholyFlesh)
                {
                    soc = (SG_UnholyFlesh)sg;
                }
            }
            if (soc == null)
            {
                map.socialGroups.Add(new SG_UnholyFlesh(map, hex.location));
            }
            else
            {
                hex.location.soc = soc;
            }
            
            hex.location.settlement = new Set_UnholyFlesh_Seed(hex.location);

            map.world.prefabStore.popImgMsg(
                "You begin a growth of Unholy Flesh. This swarm of fleshy protrustions is a reasonable combattant, but excells at instilling fear in nobles, aiding political goals."
                + "\nYou may continue to grow and control this social group with further actions",
                map.world.wordStore.lookup("UNHOLY_FLESH_SEED"));
        }

        public override void playSound(AudioStore audioStore)
        {
            audioStore.playActivateFlesh();
        }

        public override bool castable(Map map, Hex hex)
        {
            if (hex.location == null) { return false; }
            if (hex.location.soc != null) { return false; }
            if (hex.location.settlement != null) { return false; }
            if (hex.location.isOcean) { return false; }
            return true;
        }

        public override int getCost()
        {
            return World.staticMap.param.ability_fleshSeedCost;
        }

        public override string getDesc()
        {
            return "Creates a seed from which the unholy flesh can erupt. Violent and horrifying, the flesh is unsubtle and obvious, and will quickly attract the attention of the nations of men."
                 + "\nCan be expanded with further abilities once seeded."
                 + "\n[Requires an empty land location]";
        }

        public override string getName()
        {
            return "Seed the flesh";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_ghoul;
        }
    }
}