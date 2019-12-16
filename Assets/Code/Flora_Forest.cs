using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code
{
    public class Flora_Forest : Flora
    {
        public Flora_Forest(Hex hex) : base(hex)
        {
            name = "Forest";
        }

        public override Sprite getSprite()
        {
            if (hex.terrain == Hex.terrainType.SNOW) { return hex.map.world.textureStore.flora_forestSnow; }
            return hex.map.world.textureStore.flora_forest;
        }

        public override void turnTick()
        {
        }
    }
}
