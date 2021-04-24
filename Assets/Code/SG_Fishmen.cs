using UnityEngine;


namespace Assets.Code
{
    public class SG_Fishmen : SocialGroup
    {
        public SG_UnholyFlesh.warStates warState = SG_UnholyFlesh.warStates.ATTACK;

        public bool hasAttacked = false;

        public SG_Fishmen(Map map,Location startingLocation) : base(map)
        {
            float colourReducer = 0.25f;
            float colourReducer2 = 0.5f;
            color = new Color(
                (float)(Eleven.random.NextDouble()* colourReducer),
                (float)Eleven.random.NextDouble() * colourReducer2,
                (float)Eleven.random.NextDouble() * colourReducer2);
            color2 = new Color(
                (float)(Eleven.random.NextDouble() * colourReducer),
                (float)Eleven.random.NextDouble() * colourReducer2,
                (float)Eleven.random.NextDouble() * colourReducer2);
            this.setName("Lights in the Deep");

            startingLocation.soc = this;
            this.threat_mult = map.param.dark_fishmanStartingThreatMult;
        }

        public override string getTypeName()
        {
            return "Player Controlled";
        }
        public override string getTypeDesc()
        {
            return "A race of aquatic life-forms. They grow by expanding their undersea cities, then corrupting humans to join them as armies. Is considered lower threat by most nobles " +
                "until they attack the human forces. They can affect coastal cities if they neighbour them to harm the nobles therein.";
        }


        public override bool isDark()
        {
            return true;
        }
        public override bool hostileTo(Unit u)
        {
            if (this.getRel(u.society).state == DipRel.dipState.war) { return true; }
            //if (u.society.isDark()) { return false; }
            //return !u.isEnthralled();
            return false;
        }

        public override void turnTick()
        {
            base.turnTick();
            bool atWar = isAtWar();
            foreach (Location loc in map.locations)
            {
                if (loc.soc == this)
                {
                    if (loc.settlement == null)
                    {
                        loc.soc = null;
                    }
                    else
                    {
                        loc.hex.purity = 0;
                    }
                    //Withdraw from the land when you're done
                    if (loc.soc == this && (!loc.isOcean) && (atWar == false))
                    {
                        loc.soc = null;
                    }
                }
            }
        }
        public override bool hasEnthralled()
        {
            return true;
        }

        public override void takeLocationFromOther(SocialGroup def, Location taken)
        {
            base.takeLocationFromOther(def, taken);
            
            if (taken.settlement != null)
            {
                if (taken.person() != null)
                {
                    taken.person().die("Killed by DeepOnes as they overran " + taken.getName(),true);
                }
                //taken.settlement = new Set_UnholyFlesh_Ganglion(taken);
                //taken.settlement = new Set_Ruins(taken);
                taken.settlement.fallIntoRuin();
            }
        }
    }
}