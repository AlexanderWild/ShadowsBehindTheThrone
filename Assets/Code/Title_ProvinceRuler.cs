using UnityEngine;
using System.Collections;

namespace Assets.Code{
    public class Title_ProvinceRuler : Title
    {
        public Province province;
        public Title_ProvinceRuler(Society soc,Province province):base(soc)
        {
            nameM = "Duke";
            nameF = "Duchess";
            this.province = province;
        }

        public override void turnTick()
        {
            base.turnTick();
        }
        public override string getName()
        {
            return "Dukedom of " + province.name;
        }
        public override double getPrestige()
        {
            return society.map.param.society_sovreignPrestige;
        }
    }
}
