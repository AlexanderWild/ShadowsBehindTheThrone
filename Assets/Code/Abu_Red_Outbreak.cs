using System.Collections.Generic;
using UnityEngine;


namespace Assets.Code
{
    public class Abu_Red_Outbreak: AbilityUnit
    {

        public override void castInner(Map map, Unit u)
        {
            List<string> disease = new List<string>();
            foreach (Location l2 in u.location.getNeighbours())
            {
                foreach (Property pr in l2.properties)
                {
                    if (pr.proto.isDisease)
                    {
                        disease.Add(pr.proto.name);
                    }
                }
            }

            Task_Outbreak task = new Task_Outbreak();
            task.diseasesToSpread = disease;
            u.task = task;

            u.location.map.world.prefabStore.popImgMsg(u.getName() + " begins spreading disease within " + u.location.getName() + "."
                + " Once they complete this task, they will spread disease from neighbouring locations to this one. This bypasses immunity.",
                u.location.map.world.wordStore.lookup("ABILITY_RED_OUTBREAK"),5);

        }
        public override bool castable(Map map, Unit u)
        {
            if (u.location.settlement == null) { return false; }
            if (u.location.soc is Society == false) { return false; }
            foreach (Location l2 in u.location.getNeighbours())
            {
                foreach (Property pr in l2.properties)
                {
                    if (pr.proto.isDisease)
                    {
                        return true;
                    }
                }
            }
            return false;
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
            return "Spreads a neighbouring disease to this location. Takes time equal to security level."
                + "\n[Requires a human settlement in a society adjacent to a settlement with disease]";
        }

        public override string specialCost()
        {
            return "Minor Evidence";
        }
        public override string getName()
        {
            return "Outbreak";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_redDeath;
        }
    }
}