using UnityEngine;

using System;

namespace Assets.Code
{
    public class Ab_Soc_JoinLoyalists: Ability
    {
        public override void cast(Map map, Hex hex)
        {
            base.cast(map, hex);


            map.world.prefabStore.popImgMsg(
                "Your enthralled will now add its military weight to the loyal nobles in their society, and will remain loyal in a civil war if one breaks out."
                +"\nThis change will be reflected next turn",
                map.world.wordStore.lookup("SOC_JOIN_LOYALISTS"));
        }

        public override bool castable(Map map, Hex hex)
        {
            if (map.overmind.enthralled == null) { return false; }
            if (map.overmind.enthralled == map.overmind.enthralled.society.getSovreign()) { return false; }
                if (map.overmind.enthralled.rebellingFrom != map.overmind.enthralled.society) { return false; }

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
            return "Tells your enthralled to join the loyalists and support the current sovreign, opposing any rebel nobles who may start a civil war to break away from your society."
           
                + "\n[Requires an enthralled noble who is set to rebelling]";
        }

        public override string getName()
        {
            return "Join Loyalists";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_axes;
        }
    }
}