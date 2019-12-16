using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code
{
    public class MenuZeitgeist : MenuInner
    {
        public Map map;
        public Society soc;

        public List<string> costs = new List<string>();
        public List<string> titles = new List<string>();
        public List<Zeit> zeits = new List<Zeit>();

        public Person pSpeak;

        public MenuZeitgeist(Map map,Society society,Person pSpeaker)
        {
            this.map = map;
            this.soc = society;
            this.pSpeak = pSpeaker;
                
            foreach (Zeit zeit in society.zeits)
            {
                string time = "Time Left: " + (map.param.society_zeitDuration-(map.turn - zeit.turnMade));
                if (zeit.exploitedByPlayer) { time += "\nHas Been Exploited"; }

                costs.Add(time);
                titles.Add(zeit.eventDesc + "\nWeight " + (int)(100*zeit.weight) + "%");
                zeits.Add(zeit);
            }
        }

        public override string getBoxCost(int opt)
        {
            return costs[opt];
        }

        public override string getBoxTitle(int opt)
        {
            return titles[opt];
        }

        public override void optSelected(int opt)
        {
            MenuZeitgeistSecond menu = new MenuZeitgeistSecond(map, zeits[opt],pSpeak);
            //map.world.ui.addBlockerDontHide(map.world.prefabStore.getScrollsetMenu(menu).gameObject);
        }

        public override string getMenuBody()
        {
            return "The zeitgeist (from the German for \"Spirit of the Times\" of a culture is its current state of being, politically and socially."
                + "\n\nOver time, various events shape cultures, as they occur and the peoples of the nation respond to them. How they choose to respond to the events " +
                "can have outcomes far outweighing the triggering events themselves."
                + "\n\nYour enthralled, both agents and nobles, can manipulate the zeitgeist of a nation by latching onto a recent event, and pushing your own agenda."
                + " Events have different \"Weights\", which determine how powerfully they can affect the opinions of the nobles. Loss of a city far outweighs the loss of an army, for example.";
        }

        public override string getMenuTitle()
        {
            return "Zeitgeist";
        }

        public override int getNOpts()
        {
            return costs.Count;
        }
    }
}
