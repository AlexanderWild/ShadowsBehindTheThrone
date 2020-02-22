using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Code
{
    public class God_Omni : God
    {
        public override string getDescFlavour()
        {
            return "Flavour here";
        }

        public override string getDescMechanics()
        {
            return "mechanics here";
        }

        public override string getName()
        {
            return "Omnipresent Darkness";
        }

        public override void onStart(Map map)
        {
            map.overmind.powers.Add(new Ab_Fishman_Lair());
            map.overmind.powers.Add(new Ab_Fishman_CultOfTheDeep());
            //powers.Add(new Ab_Fishman_Call());
            map.overmind.powers.Add(new Ab_Fishman_Attack());
            map.overmind.powers.Add(new Ab_Fishman_HauntingSong());
            map.overmind.powers.Add(new Ab_UnholyFlesh_Seed());
            map.overmind.powers.Add(new Ab_UnholyFlesh_Screetching());
            map.overmind.powers.Add(new Ab_UnholyFlesh_Attack());
            map.overmind.powers.Add(new Ab_UnholyFlesh_Defend());
            map.overmind.powers.Add(new Ab_UnholyFlesh_Grow());
        }
    }
}
