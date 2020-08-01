using System;
using UnityEngine;


namespace Assets.Code
{
    public class Abu_Doctor_DumpBodies: AbilityUnit
    {

        public override void castInner(Map map, Unit u)
        {
            Unit_NecroDoctor doc = (Unit_NecroDoctor)u;

            int nDumped = doc.corpses;
            Set_Corpseroot set = (Set_Corpseroot)u.location.settlement;


            if (set.embeddedUnit == null)
            {
                set.embeddedUnit = new Unit_ArmyOfTheDead(u.location, u.location.soc);
                u.location.units.Remove(set.embeddedUnit);
            }
            int space = set.embeddedUnit.maxHp - set.embeddedUnit.hp;
            nDumped = Math.Min(space, nDumped);

            doc.corpses -= nDumped;
            set.embeddedUnit.hp += nDumped;

            u.location.map.world.prefabStore.popImgMsg(u.getName() + " dumps the bodies onto the corpseroot, allowing them to remain in wait for when you call upon them to rise and attack the living.",
                u.location.map.world.wordStore.lookup("ABILITY_DOCTOR_DUMP_BODIES"));

        }
        public override bool castable(Map map, Unit u)
        {
            if (u.location.soc == null) { return false; }
            if (u.location.soc is SG_Undead == false) { return false; }
            if (u.location.settlement == null) { return false; }
            if (u.location.settlement is Set_Corpseroot == false) { return false; }
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

        public override string getDesc()
        {
            return "Dumps the bodies from the cart onto a corpseroot patch, where the bodies will remain till they can be reanimated."
                + "\n[Requires a corpseroot patch]";
        }

        public override string getName()
        {
            return "Dump Bodies";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_corpseroot;
        }
    }
}