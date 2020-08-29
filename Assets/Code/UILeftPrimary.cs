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
        public Text socEcon;
        public Text socThreat;
        public Text locText;
        public Text locInfoTitle;
        public Text locInfoBody;
        public Text locNumsBody;
        public Text locNumsNumbers;
        public Text locFlavour;
        public Text body;
        public Text personTitle;
        public Text personBody;
        public Text personAwarenss;
        public Text personAwarenssDesc;
        public Text personShadowAndEvidenceVals;
        public Text maskTitle;
        public Text maskBody;
        public Text prestigeText;
        public Text prestigeDescText;
        public Text actionText;
        public Text actionDesc;
        public Text infiltrationText;
        public Text infiltrationVals;
        public Text securityDescText;
        public Image profileBack;
        public Image profileMid;
        public Image profileFore;
        public Image profileBorder;
        public Image titleTextDarkener;
        public Image bodyTextDarkener;
        public Image flag1;
        public Image flag2;
        public GameObject screenPerson;
        public GameObject screenSociety;
        public GameObject screenLocation;
        public GameObject insanityDescBox;
        public GameObject actionDescBox;
        public GameObject prestigeDescBox;
        public GameObject awarenessDescBox;
        public GameObject personAwarenessBlock;
        public GameObject socTypeBox;
        public GameObject[] traits;
        public GameObject[] traitDescBoxes;
        public Text[] traitNames;
        public Text[] traitDescs;
        public Text insanityText;
        public Text insanityDescText;
        public Text socTypeTitle;
        public Text socTypeDesc;

        public UILeftUnit uiUnit;

        public Button votingButton;
        public Button powerButton;
        public Text powerButtonText;
        public Button abilityButton;
        public Text abilityButtonText;

        public Button unlandedViewButton;
        public Button neighborViewButton;
        public Button hierarchyViewButton;
        public Button dynamicViewButton;

        public enum tabState { PERSON, SOCIETY, LOCATION };
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
            master.world.audioStore.playClick();

            if (GraphicalMap.selectedSelectable != null)
            {
                if (GraphicalMap.selectedSelectable is Unit)
                {
                    Unit unit = (Unit)GraphicalMap.selectedSelectable;
                    GraphicalMap.selectedHex = unit.location.hex;
                }else if (GraphicalMap.selectedSelectable is Property)
                {
                    Property pr = (Property)GraphicalMap.selectedSelectable;
                    GraphicalMap.selectedHex = pr.location.hex;
                }
                GraphicalMap.selectedSelectable = null;
            }
            state = tabState.PERSON;
            checkData();
        }

        public void bSetStateSociety()
        {
            master.world.audioStore.playClick();

            if (GraphicalMap.selectedSelectable != null)
            {
                if (GraphicalMap.selectedSelectable is Unit)
                {
                    Unit unit = (Unit)GraphicalMap.selectedSelectable;
                    GraphicalMap.selectedHex = unit.location.hex;
                }
                else if (GraphicalMap.selectedSelectable is Property)
                {
                    Property pr = (Property)GraphicalMap.selectedSelectable;
                    GraphicalMap.selectedHex = pr.location.hex;
                }
                GraphicalMap.selectedSelectable = null;
            }
            state = tabState.SOCIETY;
            checkData();
        }

        public void bSetStateLocation()
        {
            master.world.audioStore.playClick();

            if (GraphicalMap.selectedSelectable != null)
            {
                if (GraphicalMap.selectedSelectable is Unit)
                {
                    Unit unit = (Unit)GraphicalMap.selectedSelectable;
                    GraphicalMap.selectedHex = unit.location.hex;
                }
                else if (GraphicalMap.selectedSelectable is Property)
                {
                    Property pr = (Property)GraphicalMap.selectedSelectable;
                    GraphicalMap.selectedHex = pr.location.hex;
                }
                GraphicalMap.selectedSelectable = null;
            }
            state = tabState.LOCATION;
            checkData();
        }

        public void bViewRelations()
        {
            master.world.audioStore.playClick();

            if (GraphicalMap.selectedHex == null) { return; }
            if (GraphicalMap.selectedHex.location == null) { return; }
            if (GraphicalMap.selectedHex.location.person() == null) { return; }

            List<RelObj> rels = new List<RelObj>();
            foreach (RelObj rel in GraphicalMap.selectedHex.location.person().relations.Values)
            {
                if (Math.Abs(rel.getLiking()) > 10)
                {
                    rels.Add(rel);
                }
            }
            rels.Sort(uiUnit);

            master.addBlocker(master.world.prefabStore.getScrollSetRelations(rels).gameObject);
        }

        public void bShowUnlanded()
        {
            master.world.audioStore.playClick();

            maskTitle.text = "Unlanded Liking View";
            GraphicalSociety.refreshUnlanded(GraphicalSociety.focus);
        }

        public void bShowNeighbor()
        {
            master.world.audioStore.playClick();

            GraphicalSociety.offX = 0;
            GraphicalSociety.offY = 0;
            maskTitle.text = "Neighbor Liking View";
            GraphicalSociety.refreshNeighbor(GraphicalSociety.focus);
        }

        public void bShowHierarchy()
        {
            master.world.audioStore.playClick();

            GraphicalSociety.offX = 0;
            GraphicalSociety.offY = 0;
            maskTitle.text = "Country Hierarchy View";
            GraphicalSociety.refreshHierarchy(null);
        }

        public void bShowDynamic()
        {
            master.world.audioStore.playClick();

            GraphicalSociety.offX = 0;
            GraphicalSociety.offY = 0;
            maskTitle.text = "Dynamic Relationship View";
            GraphicalSociety.refreshDynamic(null);
        }

        public void showPersonInfo(Person p)
        {
            profileBack.enabled = true;
            profileMid.enabled = true;
            profileFore.enabled = true;
            profileBorder.enabled = true;
            //Done to unfuck the distortion of images which periodically occurs
            profileBack.sprite = null;
            profileMid.sprite = null;
            profileFore.sprite = null;
            profileBorder.sprite = null;
            profileBack.sprite = p.getImageBack();
            profileMid.sprite = p.getImageMid();
            profileFore.sprite = p.getImageFore();
            if (p.society.getSovreign() == p) { profileBorder.sprite = p.map.world.textureStore.slotKing; }
            else if (p.titles.Count > 0) { profileBorder.sprite = p.map.world.textureStore.slotDuke; }
            else { profileBorder.sprite = p.map.world.textureStore.slotCount; }

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
            string bodyText = "";
            if (p.getDirectSuperiorIfAny() != null)
            {
                bodyText += "\nDirect Superior: " + p.getDirectSuperiorIfAny().getFullName();
            }
            else
            {
                bodyText += "\nDirect Superior: None";
            }
            prestigeText.text = "Prestige: " + (int)(p.prestige) + "\nPrestige Moving towards: " + (int)(p.getTargetPrestige(null));
            personShadowAndEvidenceVals.text = (int)(p.shadow * 100) + "%\n" + (int)(p.evidence * 100) + "%";
            // bodyText += "\nShadow: " + (int)(p.shadow * 100) + "%";
            //bodyText += "\nEvidence: " + (int)(p.evidence * 100) + "%";
            bodyText += "\nMilitarism: " + (int)(p.politics_militarism * 100) + "%";
            bodyText += " (" + p.getMilitarismInfo() + ")";

            bodyText += "\n";

            personAwarenss.text = (int)(p.awareness * 100) + "%";
            if (p.action == null)
            {
                actionText.text = "Not Taking Action";
                actionDesc.text = "Characters can take actions if they become aware and world panic is sufficiently high.";
                actionDesc.text = "\n\nThis character is not taking an action.";
            }
            else
            {
                actionText.text = p.action.getShort();
                actionDesc.text = "Characters can take actions if they become aware and world panic is sufficiently high.";
                actionDesc.text = "\n\n" + p.action.getLong();
            }

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
                bodyText += "\nFear of threat: " + (int)threat.threat;
                if (threat.threat > World.staticMap.param.person_fearLevel_terrified) { bodyText += "\n[Terrified]"; }
                else if (threat.threat > World.staticMap.param.person_fearLevel_afraid) { bodyText += "\n[Afraid]"; }
                else { bodyText += "\n[Moderate Concern]"; }
            }
            else
            {
                bodyText += "\nNot feeling threatened.";
            }

            personBody.text = bodyText;


            insanityText.text = "Sanity state: " + p.madness.name;
            insanityText.text += "\nSanity: " + ((int)p.sanity) + " Maximum: " + p.maxSanity;

            insanityDescText.text = p.madness.desc + "\n\n" +
                "Characters have a sanity score. If this value drops to zero, they become insane, and begin to act in an erratic and dangerous manner. "
                + "Characters will dislike insane characters to a moderate degree, and insane characters will occasionally lash out, further reducing their relationships."
                   + "\nYou can cause reduce sanity using certain abilities.";

            personAwarenssDesc.text = "A person's awareness is how much they have realised about the threat their world faces.\nIt allows them to take actions against the darkenss directly." +
                "\nSome nobles gain awareness each time you expend power, they can also gain awareness by gaining suspicion as they seen evidence, and can be warned by their fellow nobles, especially neighbours."
                + "\nAwareness gain can be increased by being in a place of learning, or increased or decreased by traits."
                +"\n\nThis noble has an awareness rate of " + (int)(100*p.getAwarenessMult()) + "%";

            prestigeDescText.text = "";
            List<string> prestigeReasons = new List<string>();
            p.getTargetPrestige(prestigeReasons);
            foreach (string s in prestigeReasons){
                prestigeDescText.text += "*" + s + "\n";
            }

            for (int i = 0; i < traits.Length; i++)
            {
                traits[i].SetActive(false);
                traitDescBoxes[i].SetActive(false);
            }
            for (int i = 0; i < p.traits.Count; i++)
            {
                traits[i].SetActive(true);
                traitNames[i].text = p.traits[i].name;
                traitDescs[i].text = p.traits[i].desc;
            }
        }

        public void showLocationInfo()
        {
            if (GraphicalMap.selectedHex == null || GraphicalMap.selectedHex.location == null)
            {
                locInfoTitle.text = "No location selected";
                locInfoBody.text = "No location selected";
                locNumsBody.text = "";
                locNumsNumbers.text = "";
                locFlavour.text = "";
                infiltrationText.text = "No Infiltration Possible";
                infiltrationVals.text = "";
                securityDescText.text = "";
                personShadowAndEvidenceVals.text = "";
            }
            else
            {
                Map map = World.staticMap;
                Location loc = GraphicalMap.selectedHex.location;
                locInfoTitle.text = loc.getName();
                string bodyText = "";
                double hab = loc.hex.getHabilitability() - map.param.mapGen_minHabitabilityForHumans;
                hab *= 1d / (1 - map.param.mapGen_minHabitabilityForHumans);


                string valuesBody = "";
                string valuesNumbers = "";

                infiltrationText.text = "No Infiltration Possible";
                securityDescText.text = "";
                Hex hex = loc.hex;
                bodyText += "\nProvince: " + hex.province.name;

                foreach (EconTrait t in hex.province.econTraits)
                {
                    bodyText += "\nIndustry: " + t.name;
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
                        valuesBody += "\nPrestige:";
                        valuesNumbers += "\n" + Eleven.toMaxLen(hex.location.settlement.getPrestige(), 4);
                        valuesBody += "\nBase Prestige:";
                        valuesNumbers += "\n" + Eleven.toMaxLen(hex.location.settlement.basePrestige, 4);
                        valuesBody += "\nMilitary Cap Add:";
                        valuesNumbers += "\n" + hex.location.settlement.getMilitaryCap();
                        valuesBody += "\nMilitary Regen";
                        valuesNumbers += "\n" + hex.location.settlement.militaryRegenAdd;
                    }
                }

                if (loc.settlement != null)
                {
                    if (loc.soc != null && loc.soc is Society)
                    {
                        List<ReasonMsg> reasons = new List<ReasonMsg>();
                        int sec = loc.settlement.getSecurity(reasons);
                        infiltrationText.text = "Infiltration:\nSecurity Level:";
                        infiltrationVals.text = "" + (int)(100 * loc.settlement.infiltration) + "%\n" + (int)(sec);
                        securityDescText.text = "Security Level: " + sec;
                        foreach (ReasonMsg msg in reasons)
                        {
                            securityDescText.text += "\n+" + (int)(msg.value) + " from " + msg.msg;
                        }
                    }
                    locFlavour.text = loc.settlement.getFlavour();
                    if (loc.settlement is Set_City)
                    {
                        valuesBody += "\n" + ((Set_City)loc.settlement).getStatsDesc();
                        valuesNumbers += "\n" + ((Set_City)loc.settlement).getStatsValues();
                    }

                    if (hex.location.settlement.embeddedUnit != null)
                    {
                        bodyText += "\nARMY IN GARRISON (Strength " + hex.location.settlement.embeddedUnit.hp + ")";
                    }
                }

                valuesBody += "\nTemperature ";
                valuesNumbers += "\n" + (int)(loc.hex.getTemperature() * 100) + "%";
                valuesBody += "\nHabilitability ";
                valuesNumbers += "\n" + (int)(hab * 100) + "%";

                locNumsBody.text = valuesBody;
                locNumsNumbers.text = valuesNumbers;
                locInfoBody.text = bodyText;
                
            }
        }

        public void setToEmpty()
        {
            profileBack.enabled = false;
            profileMid.enabled = false;
            profileFore.enabled = false;
            personTitle.text = "No Person Selected";
            locText.text = "";
            personBody.text = "";
            prestigeText.text = "";
            insanityText.text = "";
            socTypeTitle.text = "";
            socTypeDesc.text = "";
            socThreat.text = "";
            socEcon.text = "";
            body.text = "";
            actionText.text = "";
            personAwarenss.text = "";
            flag1.color = Color.clear;
            flag2.color = Color.clear;
            title.text = "Nothing Selected";
            socTypeBox.SetActive(false);
            prestigeDescText.text = "Characters have a prestige score. This approaches a target value over time. Prestige is affected by the settlement a character rules (if any) and any other titles they hold.";
            insanityDescText.text = "Characters have a sanity score. If this value drops to zero, they become insane, and begin to act in an erratic and dangerous manner."
                   + "\nYou can cause reduce sanity using certain abilities.";
            actionDesc.text = "Characters can take actions if they become aware and world panic is sufficiently high.";
            for (int i = 0; i < traits.Length; i++)
            {
                traits[i].SetActive(false);
                traitDescBoxes[i].SetActive(false);
            }
        }

        public void bInsanityDescClick()
        {
            master.world.audioStore.playClickInfo();
            insanityDescBox.SetActive(!insanityDescBox.activeInHierarchy);
        }
        public void bPrestigeDescClick()
        {
            master.world.audioStore.playClickInfo();
            prestigeDescBox.SetActive(!prestigeDescBox.activeInHierarchy);
        }
        public void bAwarenessDescClick()
        {
            master.world.audioStore.playClickInfo();
            awarenessDescBox.SetActive(!awarenessDescBox.activeInHierarchy);
        }
        public void bActionDescBox()
        {
            master.world.audioStore.playClickInfo();
            actionDescBox.SetActive(!actionDescBox.activeInHierarchy);
        }
        public void bTypeDesc()
        {
            master.world.audioStore.playClickInfo();
            socTypeBox.SetActive(!socTypeBox.activeInHierarchy);
        }
        public void bTraitDesc1()
        {
            master.world.audioStore.playClickInfo();
            traitDescBoxes[0].SetActive(!traitDescBoxes[0].activeInHierarchy);
        }
        public void bTraitDesc2()
        {
            master.world.audioStore.playClickInfo();
            traitDescBoxes[1].SetActive(!traitDescBoxes[1].activeInHierarchy);
        }
        public void bTraitDesc3()
        {
            master.world.audioStore.playClickInfo();
            traitDescBoxes[2].SetActive(!traitDescBoxes[2].activeInHierarchy);
        }

        public void checkData()
        {

            Hex hex = GraphicalMap.selectedHex;

            if (World.staticMap != null)
            {
                bool canUsePower = master.world.map.overmind.power > 0;
                bool canUseAbility = true;
                bool canVote = hex != null && hex.location != null && hex.location.soc != null && hex.location.soc is Society && ((Society)hex.location.soc).voteSession != null;

                if (World.staticMap.param.overmind_singleAbilityPerTurn && master.world.map.overmind.hasTakenAction) {
                    canUsePower = false;
                    canUseAbility = false;
                }
                if (GraphicalMap.selectedSelectable is Unit)
                {
                    canUseAbility = ((Unit)GraphicalMap.selectedSelectable).isEnthralled() && ((Unit)GraphicalMap.selectedSelectable).lastTurnActionTaken != World.staticMap.turn;
                }

                if (master.state == UIMaster.uiState.WORLD)
                {
                    if (GraphicalMap.selectedSelectable == null)
                    {
                        abilityButtonText.text = "Use Ability (" + master.world.map.overmind.countAvailableAbilities(hex) + ")";
                        powerButtonText.text = "Use Power (" + master.world.map.overmind.countAvailablePowers(hex) + ")";
                    }
                    else if (GraphicalMap.selectedSelectable is Unit)
                    {
                        abilityButtonText.text = "Use Ability (" + master.world.map.overmind.countAvailableAbilities((Unit)GraphicalMap.selectedSelectable) + ")";
                        powerButtonText.text = "Use Power (" + master.world.map.overmind.countAvailablePowers((Unit)GraphicalMap.selectedSelectable) + ")";
                        canVote = ((Unit)GraphicalMap.selectedSelectable).person != null && ((Unit)GraphicalMap.selectedSelectable).person.state == Person.personState.enthralledAgent;
                    }
                    else
                    {
                        canUseAbility = false;
                        canUsePower = false;
                        canVote = false;
                    }
                }
                else if (master.state == UIMaster.uiState.SOCIETY)
                {
                    if (GraphicalSociety.focus == null)
                    {
                        canUseAbility = false;
                        canUsePower = false;
                    }
                    abilityButtonText.text = "Use Ability (" + master.world.map.overmind.countAvailableAbilities(GraphicalSociety.focus) + ")";
                    powerButtonText.text = "Use Power (" + master.world.map.overmind.countAvailablePowers(GraphicalSociety.focus) + ")";
                }
                powerButton.gameObject.SetActive(canUsePower);
                abilityButton.gameObject.SetActive(canUseAbility);
                votingButton.gameObject.SetActive(canVote);

                personAwarenessBlock.SetActive(World.staticMap.param.useAwareness == 1);
            }


            if (master.state == UIMaster.uiState.WORLD)
                maskTitle.text = GraphicalMap.map.masker.getTitleText();
            locText.text = "";

            //Disable all, re-enable as per need
            screenSociety.SetActive(false);
            screenPerson.SetActive(false);
            screenLocation.SetActive(false);
            uiUnit.gameObject.SetActive(false);

            if (GraphicalMap.selectedSelectable != null && GraphicalMap.selectedSelectable is Unit)
            {
                uiUnit.setTo((Unit)GraphicalMap.selectedSelectable);
                uiUnit.gameObject.SetActive(true);
            }
            else if (GraphicalMap.selectedSelectable != null && GraphicalMap.selectedSelectable is Property)
            {
                Property sel = (Property)GraphicalMap.selectedSelectable;
                screenSociety.SetActive(true);
                setToEmpty();

                socTitle.text = sel.proto.name;
                socTypeDesc.text = "Effects remain bound to locations, regardless of societal and political change, until they expire or are dispelled by another means." +
                    " \n(Rapidly select a location without selecting properties by holding CTRL while clicking on it)";
                if (sel.proto.decaysOverTime)
                {
                    title.text = sel.proto.name;
                    socTypeTitle.text = "Turns Remaining: " + sel.charge;
                }
                else
                {
                    title.text = sel.proto.name;
                    socTypeTitle.text = "Indefinite Effect";
                    socTypeDesc.text = "Effects remain bound to locations, regardless of societal and political change. This one decays over time.";
                }
                string bodyText = sel.proto.getDescription();
                body.text = bodyText;
            }
            else if (state == tabState.PERSON)
            {
                screenPerson.SetActive(true);
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
                    setToEmpty();
                }
            }
            else if (state == tabState.LOCATION)
            {
                screenLocation.SetActive(true);
                showLocationInfo();
            }
            else if (state == tabState.SOCIETY)
            {
                screenSociety.SetActive(true);
                flag1.color = Color.clear;
                flag2.color = Color.clear;
                if (hex != null && hex.location != null && hex.location.soc != null)
                {
                    flag1.color = hex.location.soc.color;
                    flag2.color = hex.location.soc.color2;
                    title.text = GraphicalMap.selectedHex.getName();
                    locText.text = "";
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

                    if (hex.location.settlement != null && hex.location.settlement.title != null)
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
                    bodyText += "\nSocial group: " + hex.location.soc.getName();
                    socTypeTitle.text = hex.location.soc.getTypeName();
                    socTypeDesc.text = hex.location.soc.getTypeDesc();
                    if (hex.location.soc is Society)
                    {
                        Society locSoc = (Society)hex.location.soc;

                        if (locSoc.voteSession != null)
                        {
                            bodyText += "\nVoting on: " + locSoc.voteSession.issue.ToString();
                            bodyText += "\nTurns Remaining: " + locSoc.voteSession.timeRemaining;
                        }

                        string econEffects = "";
                        foreach (EconEffect effect in locSoc.econEffects)
                        {
                            econEffects += "Econ from " + effect.from.name + " to " + effect.to.name + "\n";
                        }
                        socEcon.text = econEffects;
                        

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
                    body.text = bodyText;

                    string strThreat = "";
                    List<ReasonMsg> msgs = new List<ReasonMsg>();
                    double threat = hex.location.soc.getThreat(msgs);
                    strThreat += "Threat: " + (int)threat;
                    foreach (ReasonMsg msg in msgs)
                    {
                        strThreat += "\n   " + msg.msg + " " + (int)msg.value;
                    }
                    socThreat.text = strThreat;
                }
                else
                {

                    this.setToEmpty();
                }
            }
        }
    }
}
