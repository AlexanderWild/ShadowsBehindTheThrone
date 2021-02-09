using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Code
{
    public abstract class Cloud
    {
        public abstract Sprite getSprite();

        public abstract void turnTick(Hex hex);
    }
}
