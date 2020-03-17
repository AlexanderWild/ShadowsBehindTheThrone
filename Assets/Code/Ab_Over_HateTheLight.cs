using UnityEngine;

using System;

namespace Assets.Code
{
    public class Ab_Over_HateTheLight: Ability
    {
        public override void cast(Map map, Hex hex)
        {
            base.cast(map, hex);

            Person other = hex.location.person();
            castInner(map, other);
        }

        public override void castInner(Map map, Person other)
        {
            int nTouched = 0;
            foreach (SocialGroup sg in map.socialGroups)
            {
                if (sg is Society)
                {
                    Society soc = (Society)sg;
                    foreach (Person p in soc.people)
                    {
                        bool affected = false;
                        foreach (Person p2 in soc.people)
                        {
                            if (p2 == p) { continue; }
                            double delta = p.shadow - p2.shadow;//How much more shadow we have than they do
                            if (delta <= 0) { continue; }

                            affected = true;
                            delta *= -map.param.ability_hateTheLightMult;
                            p.getRelation(p2).addLiking(delta, "Hate the Light", map.turn);
                        }
                        if (affected)
                        {
                            nTouched += 1;
                        }
                    }
                }
            }

            map.world.prefabStore.popImgMsg("The enshadowed of the world grow to hate those not touched by your shadow. " + nTouched + " nobles have gained dislike for the non-enshadowed of their nations.",
                map.world.wordStore.lookup("ABILITY_HATE_THE_LIGHT"));
        }
        public override bool castable(Map map, Person person)
        {
            return true;
        }

        public override bool castable(Map map, Hex hex)
        {
            return true;
        }
        public override int getCost()
        {
            return World.staticMap.param.ability_hateTheLightCost;
        }
        public override string getDesc()
        {
            return "Causes all enshadowed nobles, world-wide, to gain up to " + World.staticMap.param.ability_hateTheLightMult + " disliking for non-enshadowed nobles in their societies.";
        }

        public override string getName()
        {
            return "Hate the Light";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_enshadow;
        }
    }
}