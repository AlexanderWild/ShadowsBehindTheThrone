using UnityEngine;


namespace Assets.Code
{
    public class Ab_Easy_AddLiking: Ability
    {
        public override void cast(Map map, Hex hex)
        {
            base.cast(map, hex);

            castInner(map, hex.location.person());
        }
        public override void castInner(Map map, Person person)
        {

            double pastLiking = person.getRelation(map.overmind.enthralled).getLiking();
            person.getRelation(map.overmind.enthralled).addLiking(map.param.ability_addLikingAmount, "Beguilement", map.turn);
            double currentLiking = person.getRelation(map.overmind.enthralled).getLiking();

            map.world.prefabStore.popImgMsg(
                "You beguile " + person.getFullName() + ", causing them to like " + map.overmind.enthralled.getFullName() + " to a greater degree."
                + "\nTheir liking has gone from " + (int)pastLiking + " to " + (int)currentLiking + ".",
                map.world.wordStore.lookup("ABILITY_BEGUILE"));
        }

        public override bool castable(Map map, Person person)
        {
            if (map.overmind.enthralled == null) { return false; }
            return person.state != Person.personState.enthralled;
        }

        public override bool castable(Map map, Hex hex)
        {
            if (hex.location == null) { return false; }
            if (hex.location.person() == null) { return false; }

            return castable(map,hex.location.person());

        }

        public override int getCost()
        {
            return World.staticMap.param.ability_addLikingCost ;
        }

        public override string getDesc()
        {
            return "Causes a noble's liking of your enthralled to increase by " + World.staticMap.param.ability_addLikingAmount + " for a while."
                + "\n[Requires a noble]";
        }

        public override string getName()
        {
            return "Beguile";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_vampire;
        }
    }
}