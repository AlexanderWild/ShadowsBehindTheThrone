using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code
{
    public class GraphicalProperty : MonoBehaviour
    {
        public World world;
        public SpriteRenderer unitLayer;
        public int lastMapChange = -1;//Tracks if the map has shrunk or moved, so you need to alter your position instantly
        public bool wasSelectedUnit = false;
        public static Color col_medDark = new Color(0.5f, 0.5f, 0.5f);
        
        Property property;
        Location location = null;

        public void Update()
        {
            if (location.properties.Contains(property) == false)
            {
                property.outer = null;
                Destroy(gameObject);
                return;
            }
            if (location.hex.outer == null)
            {
                property.outer = null;
                Destroy(this.gameObject);
                return;
            }

            gameObject.transform.localScale = new Vector3(0.5f * GraphicalMap.scale, 0.5f * GraphicalMap.scale, 1);

            Vector3 loc = GraphicalMap.getLoc(location.hex);

            float radius = 0.4f * GraphicalMap.scale;
            if (location.hex == GraphicalMap.selectedHex || property == GraphicalMap.selectedSelectable)
            {
                radius = 0.7f * GraphicalMap.scale;
                loc = loc + new Vector3(0, 0, -5f);
            }
            else
            {
                loc = loc + new Vector3(0, 0, -0.105f);
            }


            double ang = location.properties.IndexOf(property) + location.units.Count;
            ang /= location.units.Count + location.properties.Count;
            ang *= 6.28;
            loc.x += (float)(radius * Math.Sin(ang));
            loc.y += (float)(radius * Math.Cos(ang));



            Vector3 delta = loc - transform.position;
            if (GraphicalMap.lastMapChange == lastMapChange)
            {
                if (delta.magnitude > 0.175f)
                {
                    delta.Normalize();
                    delta *= 0.175f;
                }
            }
            else
            {
                lastMapChange = GraphicalMap.lastMapChange;
            }
            delta.z = loc.z - transform.position.z;
            transform.Translate(delta);
            
            //hostilityBorder.color = Color.clear;

            if (GraphicalMap.selectedSelectable != null)
            {
                if (GraphicalMap.selectedSelectable == property)
                {
                    wasSelectedUnit = true;
                    //if (((int)(Time.time*2))%2 == 0)
                    //{
                    float size = ((float)Math.Abs(Math.Sin(2 * Time.time)) * 0.5f + 0.5f) * GraphicalMap.scale;
                    gameObject.transform.localScale = new Vector3(size, size, 1);
                    //}
                }
            }
        }

        public void setTo(Property p,World world)
        {
            this.property = p;
            this.location = p.location;
            this.unitLayer.sprite = p.proto.getSprite(world);
        }

    }
}
