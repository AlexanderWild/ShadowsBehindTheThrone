using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

namespace Assets.Code
{
    public class UIMidLower : MonoBehaviour 
    {
        public Text buttonText;
        public World world;
        public Button agentButton;

        public int state = 0;
        public static int STATE_ALL_BUSY = 1;
        public static int STATE_IDLE = 2;
        public static int STATE_ALL_MOVED = 3;
        public static Color yellow = new Color(0.9f, 0.8f, 0);
        public static Color red = new Color(0.5f, 0.0f, 0);


        public void Update()
        {
            if (state == STATE_IDLE)
            {
                float col = (float) Math.Abs(Math.Cos(Time.time*2));
                buttonText.color = new Color(0, col, 0);
            }
        }

        public void bButton()
        {
            List<Unit> freeAgents = new List<Unit>();
            List<Unit> busyAgents = new List<Unit>();
            List<Unit> movedAgents = new List<Unit>();

            foreach (Unit u in world.map.units)
            {
                if (u.isEnthralled())
                {
                    if (u.movesTaken == 0)
                    {
                        if (u.task == null)
                        {
                            freeAgents.Add(u);
                        }
                        else if (u.task is Task_Disrupted == false)
                        {
                            busyAgents.Add(u);
                        }
                    }
                    else
                    {
                        movedAgents.Add(u);
                    }
                }
            }
            List<Unit> allAgents = new List<Unit>();
            allAgents.AddRange(freeAgents);
            allAgents.AddRange(busyAgents);
            allAgents.AddRange(movedAgents);

            if (allAgents.Count == 0) { return; }

            int index = 0;
            if (GraphicalMap.selectedSelectable is Unit)
            {
                if (allAgents.Contains((Unit)GraphicalMap.selectedSelectable))
                {
                    
                    index = allAgents.IndexOf((Unit)GraphicalMap.selectedSelectable);
                    if (index < allAgents.Count - 1)
                    {
                        index += 1;
                    }
                    else
                    {
                        index = 0;
                    }
                }
            }
            GraphicalMap.selectedSelectable = allAgents[index];
            GraphicalMap.panTo(allAgents[index].location.hex.x, allAgents[index].location.hex.y);
            world.ui.checkData();
        }

        public void checkData()
        {
            int agents = 0;
            int agentsIdle = 0;
            int agentsAvailable = 0;
            foreach (Unit u in world.map.units)
            {
                if (u.isEnthralled())
                {
                    agents += 1;
                    if (u.movesTaken == 0)
                    {
                        if (u.task == null)
                        {
                            agentsIdle += 1;
                        }
                        else if (u.task is Task_Disrupted == false)
                        {
                            agentsAvailable += 1;
                        }
                    }
                }
            }
            if (agents == 0) { agentButton.gameObject.SetActive(false);state = 0; return; }

            agentButton.gameObject.SetActive(true);

            if (agentsIdle > 0)
            {
                if (agentsIdle == 1)
                {
                    buttonText.text = "" + agentsIdle + " Agent Idle";
                }
                else
                {
                    buttonText.text = "" + agentsIdle + " Agents Idle";
                }
                state = STATE_IDLE;
                buttonText.color = yellow;
            }else if (agentsAvailable > 0)
            {
                buttonText.text = "All Agents Busy";
                state = STATE_ALL_BUSY;
                buttonText.color = yellow;
            }
            else
            {
                buttonText.text = "No Agents Ready";
                state = STATE_ALL_MOVED;
                buttonText.color = red;
            }
        }
    }
}
