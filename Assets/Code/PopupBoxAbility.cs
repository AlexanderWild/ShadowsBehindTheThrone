using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code
{
    public class PopupBoxAbility : MonoBehaviour,PopupScrollable
    {
        public Text title;
        public Text body;
        public Text cost;
        public Text cooldown;
        public Image icon;
        public GameObject mover;
        public float targetY;
        public Ability ability;
        public Hex hex;
        public Person person;
        public bool usable = false;
        public Image background;


        public void Update()
        {
            Vector3 loc = new Vector3(mover.transform.position.x, targetY, mover.transform.position.z);
            Vector3 delta = loc - mover.transform.position;
            if (delta.magnitude > 0.02f)
            {
                delta *= 0.075f;
            }
            mover.transform.Translate(delta);
        }

        public void setTo(Ability a,Person person)
        {
            ability = a;
            this.person = person;
            title.text = a.getName();
            body.text = a.getDesc();
            icon.sprite = a.getSprite(person.map);
            if (a.specialCost() != null) { cost.text = a.specialCost(); }
            else { cost.text = "" + a.getCost(); }
            usable = a.castable(person.map, person);
            if (a.getCooldown() > 0)
            {
                if (person.map.turn - a.turnLastCast < a.getCooldown())
                {
                    cooldown.text = "On Cooldown (" + (a.getCooldown() - (person.map.turn - a.turnLastCast)) + ")";
                    usable = false;
                }
                else
                {
                    cooldown.text = "Cooldown: " + a.getCooldown();
                }
            }
            else
            {
                cooldown.text = "";
            }

            if (!usable)
            {
                background.color = new Color(1, 1, 1, 0.5f);
                icon.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
            }
        }
        public void setTo(Ability a,Hex hex)
        {
            ability = a;
            this.hex = hex;
            title.text = a.getName();
            body.text = a.getDesc();
            icon.sprite = a.getSprite(hex.map);
            if (a.specialCost() != null) { cost.text = a.specialCost(); }
            else { cost.text = "" + a.getCost(); }
            usable = a.castable(hex.map, hex);
            if (a.getCooldown() > 0)
            {
                if (hex.map.turn - a.turnLastCast  < a.getCooldown())
                {
                    cooldown.text = "On Cooldown (" + (a.getCooldown() - (hex.map.turn - a.turnLastCast)) + ")";
                    usable = false;
                }
                else
                {
                    cooldown.text = "Cooldown: " + a.getCooldown();
                }
            }
            else
            {
                cooldown.text = "";
            }

            if (!usable)
            {
                background.color = new Color(1, 1, 1, 0.5f);
                icon.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
            }
        }

        public float ySize()
        {
            return 100;
        }

        public void setTargetY(float y)
        {
            targetY = y;
        }
        public void clicked(Map map)
        {
            //selector.selected(person,agent);
            if (hex != null)
            {
                ability.cast(map, hex);
            }
            else
            {
                ability.cast(map,person);
            }
        }

        public string getTitle()
        {
            //return person.getFullName();
            return ability.getName();
        }

        public string getBody()
        {
            string reply = "Ability selection.";
            reply += "\n\nAbilities are performed on locations or people. They have restrictions on which locations are valid targets.";
            if (World.staticMap.param.overmind_singleAbilityPerTurn)
            {
                reply += " Only one ability may be performed per turn.";
            }
            reply += "\n\nAbilities come in two forms: actions and powers. Actions are societal, are performed by your enthralled and may be done at any time.";
            reply += "\nPowers require you to spend power to perform them. Your power may go negative, allowing you to perform an expensive in an emergency.";
            return reply;
        }

        public bool overwriteSidebar()
        {
            return true;
        }

        public bool selectable()
        {
            return usable;
        }
    }
}
