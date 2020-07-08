using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code
{
    public class UIScrollableRight : MonoBehaviour
    {
        public UIMaster master;
        public Text title;
        //public Text body;

        public Text viewSocButtonText;

        public Society activeSociety;

        //public GameObject portraitPrefab;
        //public GameObject testButtonObj;
        public RectTransform listContent;

        public Toggle bPeople;
        public Toggle bPlaces;
        public Toggle bVotes;
        public Toggle bMessages;
        public Toggle bThreats;
        public Toggle bWars;

        public Text bThreatsText;

        public enum Tab { People, Places, Votes,Messages, Actions ,War,Threats};
        private Tab currentTab;

        public void Start()
        {
            currentTab = Tab.Messages;
            activeSociety = null;
        }
        public void checkData()
        {
            title.text = "";
            
            // FIXME: check if this needs to be done?
            foreach (Transform t in listContent)
            {
                GameObject.Destroy(t.gameObject);
            }


            if (master.world.map != null && master.world.map.overmind != null)
            {
                setThreatButton();
            }
            activeSociety = getSociety(GraphicalMap.selectedHex);
            if (currentTab == Tab.Messages)
            {
                title.text = "EVENT MESSAGES";
                fillMessagesTab();
                return;
            }
            else if (currentTab == Tab.Threats)
            {
                title.text = "THREATS";
                fillThreatsTab();
                return;
            }
            else if (currentTab == Tab.War)
            {
                HashSet<War> wars = new HashSet<War>();
                foreach (SocialGroup sg in master.world.map.socialGroups)
                {
                    foreach (DipRel rel in sg.relations.values)
                    {
                        if (rel.other(sg).isGone() == false && rel.war != null)
                        {
                            wars.Add(rel.war);
                        }
                    }
                }
                foreach (War war in wars)
                {
                    GameObject sp = Instantiate(master.world.prefabStore.prefabPersonPortrait, listContent);
                    sp.GetComponent<Portrait>().SetInfo(war);
                }
            }

            if (activeSociety == null) return;

            //title.text = soc.getName();
            switch (currentTab)
            {
                case Tab.People:
                    {
                        foreach (Person p in activeSociety.people)
                        {
                            GameObject pp = Instantiate(master.world.prefabStore.prefabPersonPortrait, listContent);
                            pp.GetComponent<Portrait>().SetInfo(p);
                        }

                        break;
                    }
                case Tab.Places:
                    {
                        foreach (Settlement s in getSettlements(activeSociety))
                        {
                            GameObject sp = Instantiate(master.world.prefabStore.prefabPersonPortrait, listContent);
                            sp.GetComponent<Portrait>().SetInfo(s);
                        }

                        break;
                    }
                case Tab.Votes:
                    {
                        if (activeSociety.voteSession != null)
                        {
                            title.text = activeSociety.voteSession.issue.ToString();

                            Person p = null;
                            if (master.state == UIMaster.uiState.SOCIETY && GraphicalSociety.focus != null)
                                p = GraphicalSociety.focus;

                            Hex hex = GraphicalMap.selectedHex;
                            if (hex != null && hex.settlement != null && hex.settlement.title != null && hex.settlement.title.heldBy != null)
                                p = hex.settlement.title.heldBy;

                            activeSociety.voteSession.assignVoters();
                            foreach (VoteOption v in activeSociety.voteSession.issue.options)
                            {
                                GameObject vp = Instantiate(master.world.prefabStore.prefabPersonPortrait, listContent);
                                vp.GetComponent<Portrait>().SetInfo(activeSociety.voteSession.issue, v, p);
                            }
                        }

                        break;
                    }
            }
        }


        public void setThreatButton()
        {
            List<MsgEvent> threats = master.world.map.overmind.getThreats();
            double worst = 100;
            foreach (MsgEvent item in threats)
            {
                if (item.priority < worst)
                {
                    worst = item.priority;
                }
            }
            if (worst > MsgEvent.LEVEL_GRAY)
            {
                bThreatsText.color = new Color(0f, 0.7f, 0f);
            }
            else if (worst > MsgEvent.LEVEL_YELLOW)
            {
                bThreatsText.color = new Color(1f, 1f, 0.7f);
            }
            else if (worst > MsgEvent.LEVEL_ORANGE)
            {
                bThreatsText.color = new Color(0.8f, 0.8f, 0.3f);
            }
            else
            {
                bThreatsText.color = new Color(0.8f, 0.0f, 0.0f);
            }
            bThreatsText.text = threats.Count + " Threats";

        }
        public void fillMessagesTab()
        {
            if (master.world == null) { return; }
            if (master.world.map == null) { return; }

            master.world.map.turnMessages.Sort();
            foreach (MsgEvent msg in master.world.map.turnMessages)
            {
                GameObject obj = Instantiate(master.world.prefabStore.mapMsg, listContent);
                obj.GetComponent<MonoMapMsg>().SetInfo(msg);
            }
        }

        public void fillThreatsTab()
        {
            if (master.world == null) { return; }
            if (master.world.map == null) { return; }
            List<MsgEvent> threats = master.world.map.overmind.getThreats();
            threats.Sort();
            foreach (MsgEvent msg in threats)
            {
                GameObject obj = Instantiate(master.world.prefabStore.mapMsg, listContent);
                obj.GetComponent<MonoMapMsg>().SetInfo(msg);
            }
        }

        public void bTestClick(int i)
        {
            World.log("Received data " + i);
        }

        public void onToggle(bool b)
        {
            master.world.audioStore.playClick();
            if (bPeople.isOn) currentTab = Tab.People;
            else if (bPlaces.isOn) currentTab = Tab.Places;
            else if (bVotes.isOn) currentTab = Tab.Votes;
            else if (bMessages.isOn) currentTab = Tab.Messages;
            else if (bThreats.isOn) currentTab = Tab.Threats;
            else if (bWars.isOn) currentTab = Tab.War;

            checkData();
        }

        public void onClick()
        {
            if (activeSociety == null) { return; }
            
            master.world.audioStore.playClick();
            if (master.state == UIMaster.uiState.WORLD)
                master.setToSociety(activeSociety);
            else
                master.setToWorld();

            master.checkData();
        }

        public Society getSociety(Hex h)
        {
            if (h == null) return null;
            if (h.location == null) return null;
            if (h.location.soc == null) return null;
            if (!(h.location.soc is Society)) return null;

            return (Society)h.location.soc;
        }

        private List<Settlement> getSettlements(Society s)
        {
            // FIXME: LINQ :(
            return s.map.locations
                .Where(l => l.soc == s && l.settlement != null)
                .Select(l => l.settlement)
                .ToList();
        }

        public void checkData_old()
        {
            Hex hex = GraphicalMap.selectedHex;
            if (hex == null)
            {
                title.text = "";
                //body.text = "";
            }
            else
            {
                title.text = GraphicalMap.selectedHex.getName();
                string bodyText = "Body text for hex " + GraphicalMap.selectedHex.getName();

                bodyText += "\nAttachedTo " + GraphicalMap.selectedHex.territoryOf.hex.getName();
                bodyText += "\nProvince: " + hex.province.name;
                if (hex.location != null)
                {
                    if (hex.location.settlement != null)
                    {
                        if (hex.location.settlement.title != null)
                        {
                            if (hex.location.settlement.title.heldBy != null)
                            {
                                bodyText += "\nTitle held by: " + hex.location.settlement.title.heldBy.getFullName();
                            }
                            else
                            {
                                bodyText += "\nTitle currently unheld";

                            }
                        }
                    }

                    if (hex.location.soc != null)
                    {
                        bodyText += "\nSocial group: " + hex.location.soc.getName();
                        if (hex.location.soc is Society)
                        {
                            Society locSoc = (Society)hex.location.soc;

                            foreach (EconEffect effect in locSoc.econEffects)
                            {
                                bodyText += "\nEcon from " + effect.from.name + " to " + effect.to.name;
                            }

                            foreach (Person p in locSoc.people)
                            {
                                bodyText += "\n   -" + p.getFullName();
                            }

                            if (locSoc.offensiveTarget != null)
                            {
                                bodyText += "\nOffensive: " + locSoc.offensiveTarget.getName();
                            }
                            else
                            {
                                bodyText += "\nOffensive: None";
                            }

                        }
                    }
                }
                foreach (EconTrait t in hex.province.econTraits)
                {
                    bodyText += "\n  Industry: " + t.name;
                }
                //body.text = bodyText;
            }
        }
    }
}
