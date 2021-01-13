using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code
{
    public class Set_University : SettlementHuman
    {
        public Set_University(Location loc) : base(loc)
        {
            int q = Eleven.random.Next(3);
            if (q == 0)
            {
                title = new TitleLanded("Professor", "Professor", this);
                name = loc.shortName + " University";
            }
            else if (q == 1)
            {
                title = new TitleLanded("Archivist", "Archivist", this);
                name = loc.shortName + " Archive";
            }
            else
            {
                title = new TitleLanded("Librarian", "Librarian", this);
                name = loc.shortName + " Library";
            }


            militaryCapAdd += 10;
            militaryRegenAdd = 2;
            isHuman = true;
        }
        public override void humanTurnTick()
        {
        }

        public override string getFlavour()
        {
            return "This location is renoun for its vast store of knowledge and history of education. Students flock to delve into the records, and tomes of the library, and to learn from the resident teachers." +
                "\n\nNOTE: The noble of this location will gain awareness faster.";
        }
        public override Sprite getSprite()
        {
            return location.map.world.textureStore.loc_minor_university;
        }
    }
}
