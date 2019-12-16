using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code
{
    public class UILeftPrimary : MonoBehaviour
    {
        public UIMaster master;
        public Text title;
        public Text socTitle;
        public Text locText;
        public Text body;
        public Text personTitle;
        public Text personBody;
        public Text maskTitle;
        public Text maskBody;
        public Image profileBack;
        public Image profileMid;
        public Image profileFore;
        public Image titleTextDarkener;
        public Image bodyTextDarkener;
        public GameObject screenPerson;
        public GameObject screenSociety;

        public Button powerButton;
        public Text powerButtonText;
        public Button abilityButton;
        public Text abilityButtonText;

        public enum tabState { PERSON,SOCIETY};
        public tabState state = tabState.SOCIETY;

        public void Start()
        {
        }

        private Society getSociety(Hex h)
        {
            if (h == null) return null;
            if (h.location == null) return null;
            if (h.location.soc == null) return null;
            if (!(h.location.soc is Society)) return null;

            return (Society)h.location.soc;
        }

        public void bSetStatePerson()
        {
            state = tabState.PERSON;
            checkData();
        }

        public void bSetStateSociety()
        {
            state = tabState.SOCIETY;
            checkData();
        }

        public void showPersonInfo(Person p)
        {
            profileBack.enabled = true;
            profileMid.enabled = true;
            profileFore.enabled = true;
            //Done to unfuck the distortion of images which periodically occurs
            profileBack.sprite = null;
            profileMid.sprite = null;
            profileFore.sprite = null;
            profileBack.sprite = p.getImageBack();
            profileMid.sprite = p.getImageMid();
            profileFore.sprite = p.getImageFore();
            personTitle.text = p.getFullName();
            TitleLanded title = p.title_land;
            if (title == null)
            {
                locText.text = "No Landed Title";
            }
            else
            {
                locText.text = "of " + title.settlement.name;
            }
            string bodyText = "Prestige: " + (int)(p.prestige);
            bodyText += "\nPrestige Moving towards: " + (int)(p.targetPrestige);
            bodyText += "\nShadow: " + (int)(p.shadow*100) + "%";
            bodyText += "\nEvidence: " + (int)(p.evidence*100) + "%";
            bodyText += "\nMilitarism: " + (int)(p.politics_militarism*100) + "%";
            bodyText += " (" + p.getMilitarismInfo() + ")";

            bodyText += "\n";

            Society soc = getSociety(GraphicalMap.selectedHex);
            VoteSession vote = (soc != null) ? soc.voteSession : null;

            if (vote != null)
            {
                bodyText += "\nVoting on: " + vote.issue.ToString();

                VoteOption vo = p.getVote(vote);
                bodyText += "\n\tto " + vo.info(vote.issue);
            }
            else
            {
                bodyText += "\nNot voting.";
            }

            bodyText += "\n";

            ThreatItem threat = p.getGreatestThreat();
            if (threat != null)
            {
                bodyText += "\nBelieved Greatest Threat: " + threat.getTitle();
            }
            else
            {
                bodyText += "\nNot feeling threatened.";
            }

            personBody.text = bodyText;
        }

        public void checkData()
        {

            Hex hex = GraphicalMap.selectedHex;

            if (World.staticMap != null)
            {
                if (World.staticMap.param.overmind_singleAbilityPerTurn)
                {
                    powerButton.gameObject.SetActive(master.world.map.overmind.power > 0 && (master.world.map.overmind.hasTakenAction == false));
                    abilityButton.gameObject.SetActive((master.world.map.overmind.hasTakenAction == false));
                }
                else
                {
                    powerButton.gameObject.SetActive(master.world.map.overmind.power > 0);
                }

                if (master.state == UIMaster.uiState.WORLD)
                {
                    abilityButtonText.text = "Use Ability (" + master.world.map.overmind.countAvailableAbilities(hex) + ")";
                    powerButtonText.text = "Use Power (" + master.world.map.overmind.countAvailablePowers(hex) + ")";
                }
                else if (master.state == UIMaster.uiState.SOCIETY)
                {

                    abilityButtonText.text = "Use Ability (" + master.world.map.overmind.countAvailableAbilities(GraphicalSociety.focus) + ")";
                    powerButtonText.text = "Use Power (" + master.world.map.overmind.countAvailablePowers(GraphicalSociety.focus) + ")";
                }
            }


            maskTitle.text = GraphicalMap.map.masker.getTitleText();

            if (GraphicalMap.selectedProperty != null)
            {
                screenSociety.SetActive(true);
                screenPerson.SetActive(false);
                socTitle.text = GraphicalMap.selectedProperty.proto.name;
                if (GraphicalMap.selectedProperty.proto.decaysOverTime)
                {
                    title.text = "Turns Remaining: " + GraphicalMap.selectedProperty.charge;
                }
                else
                {
                    title.text = "Indefinite Effect";
                }
                string bodyText = GraphicalMap.selectedProperty.proto.getDescription();
                body.text = bodyText;
            }
            else if (state == tabState.PERSON)
            {
                screenPerson.SetActive(true);
                screenSociety.SetActive(false);
                if (master.state == UIMaster.uiState.SOCIETY && GraphicalSociety.focus != null)
                {
                    Person p = GraphicalSociety.focus;
                    showPersonInfo(p);
                }
                else if (hex != null && hex.settlement != null && hex.settlement.title != null && hex.settlement.title.heldBy != null)
                {
                    Person p = hex.settlement.title.heldBy;
                    showPersonInfo(p);
                }
                else
                {
                    profileBack.enabled = false;
                    profileMid.enabled = false;
                    profileFore.enabled = false;
                    personTitle.text = "No Person Selected";
                    locText.text = "";
                    personBody.text = "";
                }
            }
            else if (state == tabState.SOCIETY)
            {
                screenPerson.SetActive(false);
                screenSociety.SetActive(true);
                if (hex == null)
                {
                    title.text = "";
                    body.text = "";
                    socTitle.text = "";
                }
                else
                {
                    title.text = GraphicalMap.selectedHex.getName();
                    if (GraphicalMap.selectedHex.location != null && GraphicalMap.selectedHex.location.soc != null)
                    {
                        socTitle.text = GraphicalMap.selectedHex.location.soc.getName();
                    }
                    else
                    {
                        socTitle.text = "";
                    }
                    string bodyText = "";
                    //bodyText += "Body text for hex " + GraphicalMap.selectedHex.getName();
                    //bodyText += "\nAttachedTo " + GraphicalMap.selectedHex.territoryOf.hex.getName();
                    bodyText += "\nProvince: " + hex.province.name;
                    foreach (EconTrait t in hex.province.econTraits)
                    {
                        bodyText += "\n  Industry: " + t.name;
                    }

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
                            bodyText += "\nMilitary Cap Add: " + hex.location.settlement.getMilitaryCap();
                            bodyText += "\nMilitary Regen: " + hex.location.settlement.militaryRegenAdd;
                        }

                        if (hex.location.soc != null)
                        {
                            bodyText += "\nSocial group: " + hex.location.soc.getName();
                            if (hex.location.soc is Society)
                            {
                                Society locSoc = (Society)hex.location.soc;

                                if (locSoc.voteSession != null)
                                {
                                    bodyText += "\nVoting on: " + locSoc.voteSession.issue.ToString();
                                }
                                foreach (EconEffect effect in locSoc.econEffects)
                                {
                                    bodyText += "\nEcon from " + effect.from.name + " to " + effect.to.name;
                                }

                                foreach (Person p in locSoc.people)
                                {
                                    //bodyText += "\n   -" + p.getFullName();
                                }

                                bodyText += "\nMILITARY POSTURE: " + locSoc.posture;
                                if (locSoc.offensiveTarget != null)
                                {
                                    bodyText += "\nOffensive: " + locSoc.offensiveTarget.getName();
                                }
                                else
                                {
                                    bodyText += "\nOffensive: None";
                                }
                                if (locSoc.defensiveTarget != null)
                                {
                                    bodyText += "\nDefensive: " + locSoc.defensiveTarget.getName();
                                }
                                else
                                {
                                    bodyText += "\nDefensive: None";
                                }
                                bodyText += "\nRebel cap " + locSoc.data_rebelLordsCap;
                                bodyText += "\nLoyal cap " + locSoc.data_loyalLordsCap;
                                bodyText += "\nStability: " + (int)(locSoc.data_societalStability * 100) + "%";
                                if (locSoc.instabilityTurns > 0)
                                {
                                    bodyText += "\nTURNS TILL CIVIL WAR: " + (locSoc.map.param.society_instablityTillRebellion - locSoc.instabilityTurns);
                                }

                            }

                            List<ReasonMsg> msgs = new List<ReasonMsg>();
                            double threat = hex.location.soc.getThreat(msgs);
                            bodyText += "\nThreat: " + (int)threat;
                            foreach (ReasonMsg msg in msgs)
                            {
                                bodyText += "\n   " + msg.msg + " " + (int)msg.value;
                            }
                        }

                        foreach (Property p in hex.location.properties)
                        {
                            bodyText += "\nProperty " + p.proto.name;
                        }
                    }
                    body.text = bodyText;
                }
            }
        }
    }
}
