using UnityEngine;
using System.Collections.Generic;

namespace Assets.Code
{
    public class Abu_Save_LinkFates: AbilityUnit
    {

        public override void castInner(Map map, Unit u)
        {
            Unit_Saviour saviour = (Unit_Saviour)u;

            saviour.linkedFates = true;

            u.location.map.world.prefabStore.popImgMsg(u.getName() + " publicly throws their support behind your enthralled noble, " + map.overmind.enthralled.getFullName() + ", granting them prestige "
                +"based on how adored The Saviour is." + map.overmind.enthralled.getFullName() + "'s prestige will be affected by how nobles in " + map.overmind.enthralled.society.getName() +
                " view " + u.getName() + ". The more they like them, the higher the prestige boost. However, if their suspicions grow, their prestige change may even become negative.",
                u.location.map.world.wordStore.lookup("ABILITY_SAVIOUR_LINKED_FATES"),7);

        }
        public override bool castable(Map map, Unit u)
        {
            if (u.location.soc == null) { return false; }
            if (u is Unit_Saviour == false) { return false; }
            if (map.overmind.enthralled == null) { return false; }
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
            return "";
        }
        public override string getDesc()
        {
            return "Links your enthralled noble's prestige to the nation's liking for The Saviour. If the nobles of the enthralled noble's nation like the Saviour they will gain "
                + " maximum prestige, but if the nobles dislike The Saviour, perhaps due to suspicion, their maximum prestige could drop"
                + "\n[Requires an enthralled noble to exist]";
        }

        public override string getName()
        {
            return "Link Fates";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_saviour;
        }
    }
}