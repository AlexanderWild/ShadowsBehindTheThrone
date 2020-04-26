using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code
{
    public class Unit
    {
        public string name;
        public Location location;
        public GraphicalUnit outer;


        public Unit(Location loc)
        {
            this.location = loc;
        }

        public void turnTick(Map map)
        {
            int q = Eleven.random.Next(location.getNeighbours().Count);
            location.units.Remove(this);
            this.location = location.getNeighbours()[q];
            location.units.Add(this);
        }

        public Sprite getSprite(World world)
        {
            return world.textureStore.unit_lookingGlass;
        }
    }
}
