using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Code
{
    public class Cloud_Fog : Cloud
    {
        public int age = 0;
        public static int maxAge = 5;
        public override Sprite getSprite()
        {
            return World.self.textureStore.cloud_fog;
        }

        public override void turnTick(Hex hex)
        {
            //if (Eleven.random.Next(2) == 0) {
                age += 1;
            //}
            if (age > 6)
            {
                hex.cloud = null;
            }
            else
            {
                int c = 0;
                Hex target = null;
                for (int i = -1; i <= 1; i++)
                {
                    int x = i + hex.x;
                    if (x < 0) { continue; }
                    if (x >= hex.map.sizeX) { continue; }
                    for (int j = -1; j <= 1; j++)
                    {
                        int y = j + hex.y;
                        if (y < 0) { continue; }
                        if (y >= hex.map.sizeY) { continue; }

                        if (hex.map.grid[x][y].cloud == null || hex.map.grid[x][y].cloud is Cloud_Fog)
                        {
                            c += 1;
                            if (Eleven.random.Next(c) == 0)
                            {
                                target = hex.map.grid[x][y];
                            }
                        }
                    }
                }
                if (target != null)
                {
                    if (target.cloud == null)
                    {
                        target.cloud = this;
                        Cloud_Fog child = new Cloud_Fog();
                        child.age = this.age + 1;
                        hex.cloud = child;
                    }
                    else if (this.age < maxAge - 2)
                    {
                        Cloud_Fog recipient = (Cloud_Fog)target.cloud;
                        if (recipient.age >= this.age - 2)
                        {
                            recipient.age -= 2;
                            this.age += 2;
                        }
                    }
            }
        }
        }
    }
}
