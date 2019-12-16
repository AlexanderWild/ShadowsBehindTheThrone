using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code
{
    public class MenuZeitgeistSecond : MenuInner
    {
        public Map map;
        public Society soc;

        public List<string> costs = new List<string>();
        public List<string> titles = new List<string>();
        public List<ZeitOpt> zeits = new List<ZeitOpt>();

        public Person pSpeak;

        public MenuZeitgeistSecond(Map map,Zeit zeit, Person pSpeaker)
        {
            this.map = map;
            this.pSpeak = pSpeaker;

            foreach (ZeitOpt opt in zeit.opts)
            {
                costs.Add("");
                titles.Add(opt.getEffectsDesc());
                zeits.Add(opt);
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
            /*
            if (zeits[opt].zeit.exploitedByPlayer)
            {
                map.world.prefabStore.popMsg("This zeitgeist manipulation event has already been exploited by your enthralled. You may only exploit each event once.");
                map.world.soundSource.failure();
                return;
            }
            if (pSpeak != null)
            {
                zeits[opt].implement(pSpeak);
                zeits[opt].zeit.exploitedByPlayer = true;

                map.world.soundSource.threeNoteStrings();
                map.world.prefabStore.popImgMsg("Zeitgeist Exploited",
                    "You exploit a recent event for political purposes, to manipulate the zeitgeist of the culture."
                    + " How much each noble is affected will depend on their relationship with the speaker. The more they like "
                    + pSpeak.getRankAndName() + ", the more their opinions will shift.",
                    "default",
                    "zeitgeist");
            }
            else
            {
                map.world.prefabStore.popMsg("A human society's zeitgeist can be modified by either an agent, through certain actions, or an enthralled lord, through zeitgeist actions.");
            }
            */
        }

        public override string getMenuBody()
        {
            return "Any event can be seen in a number of ways, and different events have different possible ways for the demagogue to affect the mood of the nation."
                + "\n\nOnly one option may be exploited per triggering event. The degree to which they will affect a society is based either on how much each noble likes "
                +  "the speaker, or how close their city is to the speaker (if the speaker is an agent on the world map).";
        }

        public override string getMenuTitle()
        {
            return "Zeitgeist Response";
        }

        public override int getNOpts()
        {
            return costs.Count;
        }
    }
}
