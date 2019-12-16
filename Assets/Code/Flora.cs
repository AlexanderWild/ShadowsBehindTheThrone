using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace Assets.Code
{
    public abstract class Flora
    {
        public Hex hex;
        public string name = "Plants";

        public Flora(Hex hex)
        {
            this.hex = hex;
        }

        public abstract void turnTick();
        public abstract Sprite getSprite();
        public virtual string getInfo() { return ""; }
    }
}
