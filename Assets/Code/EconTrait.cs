using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Code
{
    public class EconTrait
    {

        public bool provinceIndustry = false;
        public string name = "defaultEconName";
        
        public static void loadTraits(Map map)
        {
            EconTrait t;

            t = new EconTrait(); map.globalist.allEconTraits.Add(t);
            t.name = "Iron";
            t.provinceIndustry = true;

            t = new EconTrait(); map.globalist.allEconTraits.Add(t);
            t.name = "Wine";
            t.provinceIndustry = true;

            t = new EconTrait(); map.globalist.allEconTraits.Add(t);
            t.name = "Gold";
            t.provinceIndustry = true;

            t = new EconTrait(); map.globalist.allEconTraits.Add(t);
            t.name = "Silver";
            t.provinceIndustry = true;
        }

    }
}
