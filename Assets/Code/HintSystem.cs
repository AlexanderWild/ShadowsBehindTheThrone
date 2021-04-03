using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code
{
    public class HintSystem
    {
        public enum hintType {  INFILTRATION,ENTHRALLED_NOBLES};
        public bool[] hasShown;
        public bool disabled = false;
        public Map map;

        public HintSystem(Map map)
        {
            hasShown = new bool[64];
            this.map = map;
        }

        public void popHint(hintType hint)
        {
            if (disabled) { return; }
            if (hasShown[(int)hint]) { return; }

            if (hint == hintType.INFILTRATION)
            {
                String msg = "Infiltration is a core element of agent gameplay, and a very good strategy is to begin building infiltration with your first few agents, before moving on to making use of this foundation with your second wave (possibly after the first have met an unfortunate end)."
                    + "It represents your cultists, embedded in human society, working their way into all levels of human society."
                    + "\n\nInfiltration can be seen on the location information, by a map view or by dark tendrils reaching into infiltrated locations' map icons. It is very hard for humans to remove your infiltration (it almost always requires moving or removing the location's noble), and many agents can benefit"
                    + " from operating in infilrated locations."
                    + "\n\nVampires and Merchants can infiltrate at first, then a Seeker or Plague Doctor can act unnoticed, once the initial work has been set up to let them operate efficiently.";
                map.world.prefabStore.popMsgHint(msg, "Hint: Infiltration");
            }
            if (hint == hintType.ENTHRALLED_NOBLES)
            {

                String msg = "Enthralled Nobles are complex tools, with a range of strategies. The primary strategy is the political career."
                    + "\n\nYour enthralled noble often starts at a very low level, and must rise the ranks through careful ally building and sabotage of your rivals (using your agents or Dark Powers)."
                    + "\n\nIf they reach a high rank in society, they can enshadow themselves, and spread shadow to all the nobles of lesser prestige."
                    + "\n\nIf they reach the top of the society, they can declare a Dark Empire using an ability, which will spread shadow to all the society's nobles over time, advancing your progress to victory considerably, " +
                    "and giving you a warlike collection of broken nobles to use to invade your weaker neighbours to spread the shadow still further.";
                map.world.prefabStore.popMsgHint(msg, "Hint: Enthralled Nobles");
            }
        }
    }
}
