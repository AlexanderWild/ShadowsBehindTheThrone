using UnityEngine;


namespace Assets.Code
{
    public class Ab_Enth_TrustingFool : Ability
    {
        public override void cast(Map map, Hex hex)
        {
            base.cast(map, hex);

            Person person = hex.location.person();
            castInner(map, person);
        }
        public override void castInner(Map map, Person person)
        {
            person.getRelation(map.overmind.enthralled).suspicion = 0;

            map.world.prefabStore.popImgMsg(
                 person.getFullName() + " forgets all previous suspicions, and trusts " + map.overmind.enthralled.getFullName() + "'s claims of innocence. They may soon notice evidence, if any remains, of course.",
                map.world.wordStore.lookup("ABILITY_TRUSTING_FOOL"));
        }
        public override bool castable(Map map, Person person)
        {
            if (map.overmind.enthralled == null) { return false; }
            return person.state != Person.personState.enthralled;
        }

        public override bool castable(Map map, Hex hex)
        {
            if (hex.map.overmind.enthralled == null) { return false; }
            if (hex.location == null) { return false; }
            if (hex.location.settlement == null) { return false; }
            if (hex.location.soc == null || (hex.location.soc is Society == false)) { return false; }
            if (hex.location.person() != null && hex.location.person().state == Person.personState.enthralled) { return false; }

            return true;

        }

        public override int getCost()
        {
            return World.staticMap.param.ability_trustingFoolCost;
        }

        public override int getCooldown()
        {
            return World.staticMap.param.ability_trustingFoolCooldown;
        }
        public override string getDesc()
        {
            return "Causes the target noble to lose ALL suspicion of your enthralled (although it can return if they see evidence)."
                + "\n[Requires a non-enthralled noble, and for you to have an enthralled in existence]";
        }

        public override string getName()
        {
            return "Trusting Fool";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_enshadow;
        }
    }
}