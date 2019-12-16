using UnityEngine;

using System;

namespace Assets.Code
{
    public class Ab_Soc_JoinRebels : Ability
    {
        public override void cast(Map map, Hex hex)
        {
            base.cast(map, hex);


            map.overmind.enthralled.rebellingFrom = map.overmind.enthralled.society;

            map.world.prefabStore.popImgMsg(
                "Your enthralled will now add its military weight to the rebel nobles in their society, and will join a civil war if one breaks out."
                + "\nThis change will be reflected next turn",
                map.world.wordStore.lookup("SOC_JOIN_REBELS"));
        }

        public override bool castable(Map map, Hex hex)
        {
            if (map.overmind.enthralled == null) { return false; }
            if (map.overmind.enthralled == map.overmind.enthralled.society.getSovreign()) { return false; }
            if (map.overmind.enthralled.rebellingFrom == map.overmind.enthralled.society) { return false; }

            return true;
        }

        public override string specialCost()
        {
            return "";
        }
        public override int getCost()
        {
            return 0;
        }

        public override string getDesc()
        {
            return "Tells your enthralled to join the disatisfied nobles. If sufficient nobles (with sufficient total military cap) are opposed to their sovreign they will join a rebellion and start a civil war."
           
                + "\n[Requires an enthralled noble who is not already set to rebelling]";
        }

        public override string getName()
        {
            return "Join Rebels";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_axes;
        }
    }
}