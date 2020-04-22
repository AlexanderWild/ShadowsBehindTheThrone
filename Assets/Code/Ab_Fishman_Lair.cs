using UnityEngine;


namespace Assets.Code
{
    public class Ab_Fishman_Lair: Ability
    {
        public override void cast(Map map, Hex hex)
        {
            base.cast(map, hex);
            if (!castable(map, hex)) { return; }


            SG_Fishmen soc = null;
            foreach (SocialGroup sg in map.socialGroups)
            {
                if (sg is SG_Fishmen)
                {
                    soc = (SG_Fishmen)sg;
                }
            }
            if (soc == null)
            {
                map.socialGroups.Add(new SG_Fishmen(map, hex.location));
            }
            else
            {
                hex.location.soc = soc;
            }

            map.world.prefabStore.popImgMsg("You create a Deep One colony at " + hex.location.getName() + ". Deep Ones are stealthy, but need to raid the surface to build up numbers."
                + " Doing so will create temporary threat (which in turn will create some permanent threat), so be sure to distract the surface and to avoid drawing too much attention when the human nobles aren't busy.",
                map.world.wordStore.lookup("ABILITY_FISHMAN_LAIR"));
            hex.location.settlement = new Set_Fishman_Lair(hex.location);

        }

        public override void playSound(AudioStore audioStore)
        {
            audioStore.playActivateFishmen();
        }

        public override bool castable(Map map, Hex hex)
        {
            if (hex.location == null) { return false; }
            if (hex.location.soc != null) { return false; }
            if (hex.location.settlement != null) { return false; }
            if (!hex.location.isOcean) { return false; }
            return true;
        }

        public override int getCost()
        {
            return World.staticMap.param.ability_fishmanLairCost;
        }

        public override string getDesc()
        {
            return "Creates a Deep One lair. This creates or expands your Deep One civilisation. Deep One are covert and difficult to notice until revealed by taking action, but must raid to build population."
                + "\n[Requires an empty ocean location]";
        }

        public override string getName()
        {
            return "Establish Deep One Lair";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_fishman;
        }
    }
}