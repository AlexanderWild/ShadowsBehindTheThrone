﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code
{
    public class GraphicalUnit : MonoBehaviour
    {
        public World world;
        public SpriteRenderer unitLayer;
        public SpriteRenderer borderLayer1;
        public SpriteRenderer borderLayer2;
        public SpriteRenderer hostilityBorder;
        public GameObject hpBar;
        public Text hpText;
        public int lastMapChange = -1;//Tracks if the map has shrunk or moved, so you need to alter your position instantly
        public bool wasSelectedUnit = false;
        public static Color col_medDark = new Color(0.5f, 0.5f, 0.5f);

        public Unit unit;

        public void Update()
        {
            if (unit.location.units.Contains(unit) == false)
            {
                unit.outer = null;
                Destroy(gameObject);
                return;
            }
            if (unit.location.hex.outer == null)
            {
                unit.outer = null;
                Destroy(this.gameObject);
                return;
            }

            gameObject.transform.localScale = new Vector3(0.75f * GraphicalMap.scale, 0.75f * GraphicalMap.scale, 1);

            Vector3 loc = GraphicalMap.getLoc(unit.location.hex);

            float radius = 0.55f * GraphicalMap.scale;
            if (unit.location.hex == GraphicalMap.selectedHex || unit == GraphicalMap.selectedSelectable)
            {
                radius = 1.1f * GraphicalMap.scale;
                loc = loc + new Vector3(0, 0, -5f);
            }
            else
            {
                loc = loc + new Vector3(0, 0, -0.105f);
            }


            double ang = unit.location.units.IndexOf(unit);
            ang /= unit.location.units.Count + unit.location.properties.Count;
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

            if (GraphicalMap.selectedSelectable == unit)
            {
                wasSelectedUnit = true;
                //if (((int)(Time.time*2))%2 == 0)
                //{
                float size = ((float)Math.Abs(Math.Sin(2 * Time.time)) * 0.5f + 0.5f) * GraphicalMap.scale;
                gameObject.transform.localScale = new Vector3(size, size, 1);
                //}
            }


            if (unit.person != null && unit.person.state == Person.personState.enthralledAgent)
            {
                borderLayer1.color = Color.black;
                borderLayer2.color = Color.black;
            }
            else if (unit.society == null || unit.dontDisplayBorder)
            {
                borderLayer1.color = Color.clear;
                borderLayer2.color = Color.clear;
            }
            else
            {
                borderLayer1.color = unit.society.color;
                borderLayer2.color = unit.society.color2;
            }
        }

        public void setTo(Unit unit,World world)
        {
            this.unit = unit;
            this.unitLayer.sprite = unit.getSprite(world);
        }

        public void checkData()
        {
            if (unit.person != null && unit.person.state == Person.personState.enthralledAgent)
            {
                borderLayer1.color = Color.black;
                borderLayer2.color = Color.black;
            }
            else if (unit.society == null || unit.dontDisplayBorder)
            {
                borderLayer1.color = Color.clear;
                borderLayer2.color = Color.clear;
            }
            else
            {
                borderLayer1.color = unit.society.color;
                borderLayer2.color = unit.society.color2;
            }

            /*
            hpBar.SetActive(unit.isMilitary);
            if (unit.isMilitary)
            {
                hpText.text = "" + (int)(unit.getStrength());
            }
            unitLayer.sprite = unit.getSprite();
            */
        }
    }
}
