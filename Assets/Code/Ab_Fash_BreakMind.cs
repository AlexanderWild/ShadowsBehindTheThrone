﻿using UnityEngine;

using System;

namespace Assets.Code
{
    public class Ab_Fash_BreakMind: Ability
    {
        public override void cast(Map map, Hex hex)
        {
            base.cast(map, hex);

            castInner(map, hex.location.person());
        }
        public override void castInner(Map map, Person person)
        {
            string msgs = "You shatter the mind of " + person.getFullName() + ", as their fears and terrors overwhelm them. They now see enemies everywhere they look, and desperately seek to defend themselves against these nebulous foes.";
            person.sanity = 0;
            person.madness = new Insanity_Paranoid();

            map.world.prefabStore.popImgMsg(
                msgs,
                map.world.wordStore.lookup("ABILITY_BREAK_MIND"));
        }

        public override bool castable(Map map, Person person)
        {
            if (person.sanity >= map.param.ability_breakMindMaxSanity) { return false; }
            if (person.getGreatestThreat() == null) { return false; }
            if (person.getGreatestThreat().threat < map.param.person_fearLevel_terrified) { return false; }
            return true;
        }

        public override bool castable(Map map, Hex hex)
        {
            if (hex.location == null) { return false; }
            if (hex.location.person() == null) { return false; }
            return castable(map, hex.location.person());
        }

        public override int getCooldown()
        {
            return World.staticMap.param.ability_breakMindCooldown;
        }
        public override int getCost()
        {
            return 0;
        }

        public override string getDesc()
        {
            return "Breaks a noble's mind with fear, causing them to become paranoid. This can change insanity types to paranoid if they are already insane."
                + "\n[Requires a noble with sanity < " + World.staticMap.param.ability_breakMindMaxSanity + " sanity and fear level of 'terrified']";
        }

        public override string getName()
        {
            return "Break Mind";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_axes;
        }
    }
}