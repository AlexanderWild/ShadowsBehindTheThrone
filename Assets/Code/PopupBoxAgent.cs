using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code
{
    public class PopupBoxAgent : MonoBehaviour,PopupScrollable
    {
        public Text title;
        public Text body;
        public Image icon;
        public GameObject mover;
        public float targetY;
        public int index;
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

        public void setTo(int index,string agentName,string agentDesc,Sprite img)
        {
            this.index = index;
            title.text = agentName;
            body.text = agentDesc;
            icon.sprite = img;
        }

        public float ySize()
        {
            return 150;
        }

        public void setTargetY(float y)
        {
            targetY = y;
        }
        public void clicked(Map map)
        {
            //selector.selected(person,agent);
            map.world.audioStore.playActivate();
            Hex hex = GraphicalMap.selectedHex;
            if (hex != null && hex.location != null)
            {
                if (index == Ab_Over_CreateAgent.VAMPIRE)
                {
                    Unit agent = new Unit_Vampire(hex.location, map.soc_dark);
                    map.world.prefabStore.popImgMsg(
                        "You draw upon the creatures of darkness, and choose one to serve as your instrument in this world",
                        map.world.wordStore.lookup("ABILITY_CREATE_AGENT"));

                    agent.person = new Person(map.soc_dark);
                    agent.person.state = Person.personState.enthralledAgent;
                    agent.person.unit = agent;
                    agent.person.traits.Clear();
                    map.units.Add(agent);

                    Evidence ev = new Evidence(map.turn);
                    ev.pointsTo = agent;
                    ev.weight = 0.66;
                    agent.location.evidence.Add(ev);

                    agent.task = null;
                    GraphicalMap.selectedSelectable = agent;
                }

                if (index == Ab_Over_CreateAgent.DOCTOR)
                {
                    Unit agent = new Unit_NecroDoctor(hex.location, map.soc_dark);
                    map.world.prefabStore.popImgMsg(
                        "You draw upon the creatures of darkness, and choose one to serve as your instrument in this world",
                        map.world.wordStore.lookup("ABILITY_CREATE_AGENT"));

                    agent.person = new Person(map.soc_dark);
                    agent.person.state = Person.personState.enthralledAgent;
                    agent.person.unit = agent;
                    agent.person.traits.Clear();
                    map.units.Add(agent);

                    Evidence ev = new Evidence(map.turn);
                    ev.pointsTo = agent;
                    ev.weight = 0.66;
                    agent.location.evidence.Add(ev);

                    agent.task = null;
                    GraphicalMap.selectedSelectable = agent;
                }

                if (index == Ab_Over_CreateAgent.SEEKER)
                {
                    Unit agent = new Unit_Seeker(hex.location, map.soc_dark);
                    map.world.prefabStore.popImgMsg(
                        "You draw upon the creatures of darkness, and choose one to serve as your instrument in this world",
                        map.world.wordStore.lookup("ABILITY_CREATE_AGENT"));

                    agent.person = new Person(map.soc_dark);
                    agent.person.state = Person.personState.enthralledAgent;
                    agent.person.unit = agent;
                    agent.person.traits.Clear();
                    map.units.Add(agent);

                    Unit_Seeker.addForgottenSecrets(map);

                    Evidence ev = new Evidence(map.turn);
                    ev.pointsTo = agent;
                    ev.weight = 0.66;
                    agent.location.evidence.Add(ev);

                    agent.task = null;
                    GraphicalMap.selectedSelectable = agent;
                }

                if (index == Ab_Over_CreateAgent.PUMPKIN)
                {
                    Unit agent = new Unit_HeadlessHorseman(hex.location, map.soc_dark);
                    map.world.prefabStore.popImgMsg(
                        "You draw upon the creatures of darkness, and choose one to serve as your instrument in this world",
                        "The horseman rides again!",3);

                    agent.person = new Person(map.soc_dark);
                    agent.person.state = Person.personState.enthralledAgent;
                    agent.person.unit = agent;
                    agent.person.traits.Clear();
                    map.units.Add(agent);


                    Evidence ev = new Evidence(map.turn);
                    ev.pointsTo = agent;
                    ev.weight = 0.66;
                    agent.location.evidence.Add(ev);

                    agent.task = null;
                    GraphicalMap.selectedSelectable = agent;
                }
                if (index == Ab_Over_CreateAgent.HEIROPHANT)
                {
                    Unit agent = new Unit_DarkHeirophant(hex.location, map.soc_dark);
                    map.world.prefabStore.popImgMsg(
                        "You draw upon the creatures of darkness, and choose one to serve as your instrument in this world",
                        "The cult begins, its hidden shrines to the dark powers are raised, and await the worshippers.", 4);

                    agent.person = new Person(map.soc_dark);
                    agent.person.state = Person.personState.enthralledAgent;
                    agent.person.unit = agent;
                    agent.person.traits.Clear();
                    map.units.Add(agent);


                    Evidence ev = new Evidence(map.turn);
                    ev.pointsTo = agent;
                    ev.weight = 0.66;
                    agent.location.evidence.Add(ev);

                    agent.task = null;
                    GraphicalMap.selectedSelectable = agent;
                }
                if (!map.overmind.isFirstEnthralledAgent)
                {
                    foreach (Ability a in map.overmind.powers)
                    {
                        if (a is Ab_Over_CreateAgent)
                        {
                            a.turnLastCast = map.turn;
                        }
                    }
                }
                map.overmind.availableEnthrallments -= 1;
                map.hasEnthralledAnAgent = true;

                AchievementManager.unlockAchievement(SteamManager.achievement_key.FIRST_AGENT);
            }
        }

        public string getTitle()
        {
            return this.title.text;
        }

        public string getBody()
        {
            string reply = "Agent selection.";
            reply += "Various agents are available, each with their own sets of abilities and mechanics. Choose one to create.";
            return reply;
        }

        public bool overwriteSidebar()
        {
            return true;
        }

        public bool selectable()
        {
            return true;
        }
    }
}
