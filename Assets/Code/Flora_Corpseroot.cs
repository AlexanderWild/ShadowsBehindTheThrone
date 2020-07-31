using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code
{
    public class Flora_Corpseroot : Flora
    {
        public Flora_Corpseroot(Hex hex) : base(hex)
        {
            name = "Corpseroot";
        }

        public override Sprite getSprite()
        {
            return hex.map.world.textureStore.flora_corpseroot;
        }

        public override void turnTick()
        {
        }
    }
}
