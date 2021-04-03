using UnityEngine;


namespace Assets.Code
{
    public class Ab_Enth_Enthrall: Ability
    {
        public override void cast(Map map, Hex hex)
        {
            base.cast(map, hex);

            Person target = hex.location.person();
            castInner(map, target);
        }
        public override void castInner(Map map, Person person)
        {
            person.state = Person.personState.enthralled;
            map.overmind.enthralled = person;

            map.overmind.availableEnthrallments -= 1;
            map.overmind.computeEnthralled();

            string addMsg = "\n\nYou have " + map.overmind.availableEnthrallments + " enthrallment uses remaining.You will regain one every " +
                map.param.overmind_enthrallmentUseRegainPeriod + " turns if you are below maximum.";

            map.world.prefabStore.popImgMsg(
                "You enthrall " + map.overmind.enthralled.getFullName() + ". They are now, until they die, your instrument in this world. Their votes are guided by your hand, and they will"
                + " act as you command within their society."+ addMsg,
                map.world.wordStore.lookup("ABILITY_ENTHRALL"));

            map.hintSystem.popHint(HintSystem.hintType.ENTHRALLED_NOBLES);
        }
        public override bool castable(Map map, Person person)
        {
            if (map.overmind.enthralled != null) { return false; }
            if (map.overmind.availableEnthrallments < 1) { return false; }
            if (map.overmind.nEnthralled >= map.overmind.maxEnthralled) { return false; }

            return person.enthrallable();
        }

        public override bool castable(Map map, Hex hex)
        {
            if (map.overmind.enthralled != null) { return false; }

            if (hex.location == null) { return false; }
            if (hex.location.soc == null) { return false; }
            if (hex.location.soc is Society == false) { return false; }
            if (hex.location.settlement == null) { return false; }
            if (hex.location.settlement.title == null) { return false; }
            if (hex.location.settlement.title.heldBy == null) { return false; }

            Person p = hex.location.settlement.title.heldBy;
            return castable(map,p);
        }

        public override int getCost()
        {
            return 0;
        }

        public override string getDesc()
        {
            return "Enthralls a lower-prestige member of a society, only certain low ranked nobles and broken nobles can be enthralled."
                + "\n[You require an enthrallment use, and to not be at your enthralled cap. You may only have one enthralled noble at a time]";
        }

        public override string getName()
        {
            return "Enthrall";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_enshadow;
        }
    }
}