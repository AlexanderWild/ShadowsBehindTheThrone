using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code
{
    public class UIMaster : MonoBehaviour
    {
        public World world;
        public GameObject uiMaster;
        public UIMainMenu uiMainMenu;
        public UIScrollableRight uiScrollables;
        public UIMidTop uiMidTop;
        public UILeftPrimary uiLeftPrimary;
        public UIVoting uiVoting;
        public UIMidLower uIMidLower;
        public UIMusic uiMusic;
        public UIInputs uiInputs;
        public GameObject endTurnButton;

        public List<GameObject> blockerQueue;
        public List<GameObject> blockerQueueDelayed = new List<GameObject>();
        public GameObject blocker;
        public GameObject hexSelector;
        public InputField cheatField;
        public GameObject viewSocietyButton;
        public Text bViewSocietyText;
        //public ViewSelector_Person viewSelector;

        //public List<Alert> alertQueue = new List<Alert>();

        public enum uiState {  SOCIETY, WORLD, BACKGROUND, MAIN_MENU,VOTING };
        public uiState state = uiState.MAIN_MENU;

        public void Update()
        {

            if (world.map != null)
            {
                if (GraphicalMap.selectedHex == null || GraphicalMap.selectedHex.outer == null)
                {
                    hexSelector.SetActive(false);
                }
                else
                {
                    hexSelector.SetActive(true);
                    hexSelector.transform.localScale = new Vector3(GraphicalMap.scale, GraphicalMap.scale, 1);
                    hexSelector.transform.localPosition = GraphicalMap.getLoc(GraphicalMap.selectedHex) + new Vector3(0, 0, -5);
                }

                if (state == uiState.WORLD)
                {
                    GraphicalMap.tick();
                    //if (GraphicalMap.selectedHex == null)
                    //{
                    //    //uiCity.gameObject.SetActive(false);
                    //    uiScrollables.gameObject.SetActive(true);
                    //}
                    //else
                    //{
                    //   uiScrollables.gameObject.SetActive(true);
                    //}
                }
                else if (state == uiState.SOCIETY)
                {
                    //GraphicalSociety.tick();
                }
            }
            else
            {

                hexSelector.SetActive(false);
            }

            if (state == uiState.WORLD)
            {
                uiLeftPrimary.titleTextDarkener.enabled = GraphicalMap.map.masker.mask != MapMaskManager.maskType.NONE;
                uiLeftPrimary.bodyTextDarkener.enabled = GraphicalMap.map.masker.mask == MapMaskManager.maskType.LIKING_ME 
                    || GraphicalMap.map.masker.mask == MapMaskManager.maskType.LIKING_THEM
                || GraphicalMap.map.masker.mask == MapMaskManager.maskType.VOTE_EFFECT;
                if (world.map.masker.mask != MapMaskManager.maskType.NONE)
                {
                    uiLeftPrimary.maskTitle.text = GraphicalMap.map.masker.getTitleText();
                    uiLeftPrimary.maskBody.text = GraphicalMap.map.masker.getBodyText();
                }
                else
                {
                    uiLeftPrimary.maskTitle.text = "";
                    uiLeftPrimary.maskBody.text = "";
                }

                //uiLeftPrimary.unlandedViewButton.gameObject.SetActive(false);
                uiLeftPrimary.neighborViewButton.gameObject.SetActive(false);
                uiLeftPrimary.hierarchyViewButton.gameObject.SetActive(false);
                //uiLeftPrimary.dynamicViewButton.gameObject.SetActive(false);
                endTurnButton.SetActive(true);
            }
            else if (state == uiState.SOCIETY)
            {
                uiLeftPrimary.maskBody.text = "";
                uiLeftPrimary.titleTextDarkener.enabled = true;
                uiLeftPrimary.bodyTextDarkener.enabled = false;

                //uiLeftPrimary.unlandedViewButton.gameObject.SetActive(true);
                uiLeftPrimary.neighborViewButton.gameObject.SetActive(true);
                uiLeftPrimary.hierarchyViewButton.gameObject.SetActive(GraphicalSociety.activeSociety != null && GraphicalSociety.activeSociety.getSovreign() != null);
                //uiLeftPrimary.dynamicViewButton.gameObject.SetActive(true);
                endTurnButton.SetActive(false);

                GraphicalSociety.update();
            }
            else
            {
                uiLeftPrimary.maskTitle.text = "";
                uiLeftPrimary.maskBody.text = "";
                uiLeftPrimary.titleTextDarkener.enabled = false;
                uiLeftPrimary.bodyTextDarkener.enabled = false;

                uiLeftPrimary.unlandedViewButton.gameObject.SetActive(false);
                uiLeftPrimary.neighborViewButton.gameObject.SetActive(false);
                uiLeftPrimary.hierarchyViewButton.gameObject.SetActive(false);
            }
            
            if (state == uiState.MAIN_MENU)
            {
                uiMainMenu.continueButton.gameObject.SetActive(World.staticMap != null);
            }

            checkBlockerQueue();
        }


        public void takeCheatCommand()
        {
            Cheat.takeCommand(world.map, cheatField.text);
            cheatField.text = "";
        }

        public void bEndTurn()
        {
            if (world.turnLock) { return; }
            if (blocker != null) { return; }

           // if (alertQueue.Count > 0) { bViewAlerts(); return; }

            world.audioStore.playClick();

            world.turnLock = true;
            world.map.turnTick();
            world.turnLock = false;

            checkData();

            if (World.autosavePeriod != -1 && world.map.turn % World.autosavePeriod == 0)
            {
                world.prefabStore.popAutosave();
            }
        }

        public void checkData()
        {

            if (state == uiState.SOCIETY)
            {
                //GraphicalSociety.checkData();
                //uiSociety.setTo(GraphicalSociety.focal);
                bViewSocietyText.text = "View World";
                viewSocietyButton.SetActive(true);
            }
            else if (state == uiState.WORLD)
            {
                GraphicalMap.checkData();
                uIMidLower.checkData();
                bViewSocietyText.text = "View Society";
                viewSocietyButton.SetActive(GraphicalMap.selectedHex != null && GraphicalMap.selectedHex.location != null && GraphicalMap.selectedHex.location.soc is Society);
            }
            else
            {
                viewSocietyButton.SetActive(false);
            }

            if (state == uiState.BACKGROUND) { return; }
            if (state == uiState.MAIN_MENU) { return; }

            world.map.overmind.computeEnthralled();
            uiLeftPrimary.checkData();
            uiMidTop.checkData();
            uiScrollables.checkData();
        }

        public void bPowers()
        {
            if (world.turnLock) { return; }
            if (blocker != null) { return; }

            world.audioStore.playClick();
            if (state == uiState.WORLD)
            {
                if (GraphicalMap.selectedSelectable != null && GraphicalMap.selectedSelectable is Unit)
                {
                    Unit u = (Unit)GraphicalMap.selectedSelectable;
                    if (u.isEnthralled())
                    {
                        List<Ability> abilities = world.map.overmind.getAvailablePowers(u);
                        List<Ability> uncastables = new List<Ability>();
                        foreach (Ability a in u.powers)
                        {
                            if (abilities.Contains(a) == false)
                            {
                                uncastables.Add(a);
                            }
                        }
                        if (abilities.Count + uncastables.Count > 0)
                        {
                            addBlocker(world.prefabStore.getScrollSet(abilities, uncastables, u).gameObject);
                        }
                    }
                    else
                    {
                        List<Ability> abilities = world.map.overmind.getAvailablePowers((Unit)GraphicalMap.selectedSelectable);
                        List<Ability> uncastables = new List<Ability>();
                        foreach (Ability a in world.map.overmind.powers)
                        {
                            if (abilities.Contains(a) == false)
                            {
                                uncastables.Add(a);
                            }
                        }
                        addBlocker(world.prefabStore.getScrollSet(abilities, uncastables, (Unit)GraphicalMap.selectedSelectable).gameObject);
                        return;
                    }
                }
                else if (GraphicalMap.selectedHex != null)
                {
                    List<Ability> abilities = world.map.overmind.getAvailablePowers(GraphicalMap.selectedHex);
                    List<Ability> uncastables = new List<Ability>();
                    foreach (Ability a in world.map.overmind.powers)
                    {
                        if (abilities.Contains(a) == false)
                        {
                            uncastables.Add(a);
                        }
                    }
                    addBlocker(world.prefabStore.getScrollSet(abilities, uncastables, GraphicalMap.selectedHex).gameObject);
                }
            }else if (state == uiState.SOCIETY)
            {
                if (GraphicalSociety.focus == null) { return; }

                List<Ability> abilities = world.map.overmind.getAvailablePowers(GraphicalSociety.focus);
                List<Ability> uncastables = new List<Ability>();
                foreach (Ability a in world.map.overmind.powers)
                {
                    if (abilities.Contains(a) == false)
                    {
                        uncastables.Add(a);
                    }
                }
                addBlocker(world.prefabStore.getScrollSet(abilities, uncastables, GraphicalSociety.focus).gameObject);

            }
        }
        public void bActions()
        {

            if (world.turnLock) { return; }
            if (blocker != null) { return; }

            world.audioStore.playClick();
            if (state == uiState.WORLD)
            {
                if (GraphicalMap.selectedSelectable != null && GraphicalMap.selectedSelectable is Unit)
                {
                    Unit u = (Unit)GraphicalMap.selectedSelectable;
                    if (u.isEnthralled())
                    {
                        List<Ability> abilities = world.map.overmind.getAvailableAbilities(u);
                        List<Ability> uncastables = new List<Ability>();
                        foreach (Ability a in u.abilities)
                        {
                            if (abilities.Contains(a) == false)
                            {
                                uncastables.Add(a);
                            }
                        }
                        if (abilities.Count + uncastables.Count > 0)
                        {
                            addBlocker(world.prefabStore.getScrollSet(abilities, uncastables, u).gameObject);
                        }
                    }
                    else
                    {
                        List<Ability> abilities = world.map.overmind.getAvailableAbilities((Unit)GraphicalMap.selectedSelectable);
                        List<Ability> uncastables = new List<Ability>();
                        foreach (Ability a in world.map.overmind.abilities)
                        {
                            if (abilities.Contains(a) == false)
                            {
                                uncastables.Add(a);
                            }
                        }
                        addBlocker(world.prefabStore.getScrollSet(abilities, uncastables, (Unit)GraphicalMap.selectedSelectable).gameObject);
                    }
                }
                else
                {
                    if (GraphicalMap.selectedHex == null) { return; }
                    List<Ability> abilities = world.map.overmind.getAvailableAbilities(GraphicalMap.selectedHex);
                    List<Ability> uncastables = new List<Ability>();
                    foreach (Ability a in world.map.overmind.abilities)
                    {
                        if (abilities.Contains(a) == false)
                        {
                            uncastables.Add(a);
                        }
                    }
                    addBlocker(world.prefabStore.getScrollSet(abilities, uncastables, GraphicalMap.selectedHex).gameObject);
                }
            }else if (state == uiState.SOCIETY)
            {

                if (GraphicalSociety.focus == null) { return; }
                List<Ability> abilities = world.map.overmind.getAvailableAbilities(GraphicalSociety.focus);
                List<Ability> uncastables = new List<Ability>();
                foreach (Ability a in world.map.overmind.abilities)
                {
                    if (abilities.Contains(a) == false)
                    {
                        uncastables.Add(a);
                    }
                }
                addBlocker(world.prefabStore.getScrollSet(abilities, uncastables, GraphicalSociety.focus).gameObject);
            }
        }

        public void bViewSociety()
        {
            if (state == uiState.SOCIETY)
            {
                if (GraphicalSociety.focus != null)
                {
                    Location loc = GraphicalSociety.focus.getLocation();
                    if (loc != null)
                    {
                        Hex hex = loc.hex;
                        GraphicalMap.panTo(hex.x, hex.y);
                        GraphicalMap.selectedHex = hex;
                    }
                }

                world.audioStore.playClick();
                setToWorld();
            }
            else if (state == uiState.WORLD)
            {
                if (GraphicalMap.selectedHex == null) { return; }
                if (GraphicalMap.selectedHex.owner == null) { return; }
                if (GraphicalMap.selectedHex.owner is Society == false) { return; }

                world.audioStore.playClick();

                GraphicalSociety.focus = GraphicalMap.selectedHex.location.person();
                setToSociety((Society)GraphicalMap.selectedHex.owner);
            }
        }

        public void bViewEnthralled()
        {
            world.audioStore.playClick();
            if (world.map.overmind.enthralled == null)
            {
                world.prefabStore.popMsg("You have no enthralled noble currently\n\nInfiltrate a location using your agents (merchants and vampires excell at this) and enthrall the noble using a Heirophant, or, if playing a political game," +
                    " you may enthrall a low-prestige noble (highlighted with flashing purple background) using the \"Enthrall\" power");
                return;
            }
            if (state == uiState.SOCIETY)
            {
                if (GraphicalSociety.focus.society != world.map.overmind.enthralled.society)
                {
                    world.prefabStore.popMsg("Your enthralled does not belong to this society");
                    return;
                }
                GraphicalSociety.refreshNeighbor(world.map.overmind.enthralled);
            }
            else
            {
                GraphicalSociety.focus = world.map.overmind.enthralled;
                setToSociety(world.map.overmind.enthralled.society);
            }
        }

        public void setToBackground()
        {
            state = uiState.BACKGROUND;

            uiMainMenu.gameObject.SetActive(false);
            uiLeftPrimary.gameObject.SetActive(false);
            uiScrollables.gameObject.SetActive(false);
            uiMidTop.gameObject.SetActive(false);
            uiVoting.gameObject.SetActive(false);
            uIMidLower.gameObject.SetActive(false);
            hexSelector.SetActive(false);

            GraphicalMap.purge();
            GraphicalSociety.purge();
        }

        public void setToWorld()
        {
            state = uiState.WORLD;

            uiMainMenu.gameObject.SetActive(false);
            uiLeftPrimary.gameObject.SetActive(true);
            uiScrollables.gameObject.SetActive(true);
            uiMidTop.gameObject.SetActive(true);
            uiVoting.gameObject.SetActive(false);
            uIMidLower.gameObject.SetActive(true);
            hexSelector.SetActive(true);

            uiScrollables.viewSocButtonText.text = "View Society";

            GraphicalSociety.purge();

            checkData();
        }

        public void setToSociety(Society soc)
        {
            state = uiState.SOCIETY;

            uiMainMenu.gameObject.SetActive(false);
            uiLeftPrimary.gameObject.SetActive(true);
            uiScrollables.gameObject.SetActive(true);
            uiMidTop.gameObject.SetActive(true);
            uiVoting.gameObject.SetActive(false);
            uIMidLower.gameObject.SetActive(false);
            hexSelector.SetActive(false);

            uiScrollables.viewSocButtonText.text = "View World";

            GraphicalSociety.purge();
            GraphicalMap.purge();

            GraphicalSociety.setup(soc);

            uiLeftPrimary.maskTitle.text = "Neighbor Liking View";
            GraphicalSociety.refreshNeighbor(GraphicalSociety.focus);

            checkData();
        }

        public void setToMainMenu()
        {

            state = uiState.MAIN_MENU;

            uiMainMenu.gameObject.SetActive(true);
            uiLeftPrimary.gameObject.SetActive(false);
            uiScrollables.gameObject.SetActive(false);
            uiMidTop.gameObject.SetActive(false);
            uiVoting.gameObject.SetActive(false);
            uIMidLower.gameObject.SetActive(false);
            hexSelector.SetActive(false);


            if (World.staticMap != null)
            {
                GraphicalSociety.purge();
                GraphicalMap.purge();
            }
        }
        public void setToVoting()
        {

            state = uiState.VOTING ;

            uiMainMenu.gameObject.SetActive(false);
            uiLeftPrimary.gameObject.SetActive(false);
            uiScrollables.gameObject.SetActive(false);
            uiMidTop.gameObject.SetActive(false);
            uiVoting.gameObject.SetActive(true);
            uIMidLower.gameObject.SetActive(false);
            hexSelector.SetActive(false);


            if (World.staticMap != null)
            {
                GraphicalSociety.purge();
                GraphicalMap.purge();
            }
        }

        public void bVoting()
        {
            if (GraphicalMap.selectedSelectable != null)
            {
                if (GraphicalMap.selectedSelectable is Unit)
                {
                    Unit u = (Unit)GraphicalMap.selectedSelectable;
                    if (u.person != null && u.person.state == Person.personState.enthralledAgent)
                    {
                        if (u.location.soc != null && u.location.soc is Society)
                        {
                            Society soc = (Society)u.location.soc;
                            if (soc.voteSession != null)
                            {
                                uiVoting.populate(soc,u.person);
                                setToVoting();
                            }
                        }
                    }
                }
                return;
            }

            if (GraphicalMap.selectedHex == null) { return; }
            if (GraphicalMap.selectedHex.location == null) { return; }
            if (GraphicalMap.selectedHex.location.soc == null) { return; }
            if (GraphicalMap.selectedHex.location.soc is Society == false) { return; }
            if (((Society)GraphicalMap.selectedHex.location.soc).voteSession == null) { return; }
            Person interacter = null;
            foreach (Person p in ((Society)GraphicalMap.selectedHex.location.soc).people)
            {
                if (p.state == Person.personState.enthralled) { interacter = p; }
            }
            uiVoting.populate((Society)GraphicalMap.selectedHex.location.soc, interacter);
            setToVoting();
        }

        //public void bViewWorld()
        //{
        //    setToWorld();

        //    if (GraphicalSociety.focal != null && GraphicalSociety.focal.slot != null) { GraphicalSociety.focalSlot = GraphicalSociety.focal.slot; }
        //    if (GraphicalSociety.focalSlot != null)
        //    {
        //        if (GraphicalSociety.focalSlot.domain != null)
        //        {
        //            Hex hex = GraphicalSociety.focalSlot.domain.loc.hex;
        //            GraphicalMap.panTo(hex.x, hex.y);
        //        }
        //    }
        //}

        //public void bWorldMap()
        //{
        //    if (world.turnLock) { return; }
        //    if (blocker != null) { return; }
        //    setToBackground();

        //    PopupWorldMap worldMap = world.prefabStore.getWorldMap(world, world.map);
        //    addBlocker(worldMap.gameObject);
        //}
        //public void bPlayback()
        //{
        //    if (world.turnLock) { return; }
        //    if (blocker != null) { return; }
        //    setToBackground();

        //    PopupPlayback worldMap = world.prefabStore.getPlayback(world, world.map);
        //    addBlockerDontHide(worldMap.gameObject);
        //}

        //public void addAlert(Vote v, Person p, Society soc, Hex hex, string title, string msg, string lockout)
        //{
        //    if (world.displayMessages == false) { return; }
        //    if (world.map.permaDismissed.Contains(lockout)) { return; }
        //    Alert a = new Alert();
        //    a.targetVote = v;
        //    a.targetHex = hex;
        //    a.targetPerson = p;
        //    a.targetSociety = soc;
        //    a.title = title;
        //    a.words = msg;
        //    a.lockout = lockout;
        //    //alertQueue.Add(a);
        //    Alert alert = a;
        //    world.prefabStore.popupAlert(alert.title, alert.words, alert.targetVote, alert.targetSociety, alert.targetPerson, alert.targetHex, alert.lockout);
        //}

        public void addBlocker(GameObject block)
        {
            //if (CheatMenu.nopopups)
            //{
            //    Destroy(block);
            //    return;
            //}

            if (blocker == null)
            {
                blocker = block;
            }
            else
            {
                blockerQueue.Add(block);
                block.SetActive(false);//Hide all non-main items
            }
            checkData();
        }


        public void addBlockerToDelayedQueue(GameObject block)
        {
            if (blocker == null)
            {
                blocker = block;
            }
            else
            {
                blockerQueueDelayed.Add(block);
                block.SetActive(false);//Hide all non-main items
            }
            checkData();
        }
        public void addBlockerToQueueFrontDontHide(GameObject block)
        {
            if (blocker == null)
            {
                blocker = block;
            }
            else
            {
                blockerQueue.Insert(0, block);
            }
            checkData();
        }

        public void addBlockerDontHide(GameObject block)
        {
            //if (CheatMenu.nopopups)
            //{
            //    Destroy(block);
            //    return;
            //}

            if (blocker == null)
            {
                blocker = block;
            }
            else
            {
                blockerQueue.Add(block);
            }
            checkData();
        }


        public void bTutorial()
        {
            world.audioStore.playClick();
            for (int i = 0; i < world.textureStore.tutorialImages.Count; i++)
            {
                PopupTutorialMsg msg = world.prefabStore.getTutorial(i);
                addBlocker(msg.gameObject);
            }
        }

        public void checkBlockerQueue()
        {
            if (blocker == null)
            {
                if (blockerQueue.Count > 0)
                {
                    GameObject next = blockerQueue[0];
                    blockerQueue.RemoveAt(0);

                    next.SetActive(true);//Show new item from queue
                    blocker = next;
                    return;
                }
                if (blockerQueueDelayed.Count > 0)
                {
                    GameObject next = blockerQueueDelayed[0];
                    blockerQueueDelayed.RemoveAt(0);

                    next.SetActive(true);//Show new item from queue
                    blocker = next;
                    return;
                }
            }
        }

        public void removeBlocker(GameObject block)
        {
            if (blocker == block)
            {
                if (blockerQueue.Count > 0)
                {
                    GameObject next = blockerQueue[0];
                    blockerQueue.RemoveAt(0);

                    next.SetActive(true);//Show new item from queue
                    blocker = next;
                }
                DestroyImmediate(block);
            }
            else if (blockerQueue.Contains(block))
            {
                blockerQueue.Remove(block);
                DestroyImmediate(block);
            }

            if (state == uiState.BACKGROUND)
            {
                setToWorld();
            }
            else if (state == uiState.WORLD)
            {
                GraphicalMap.checkData();
            }
            else
            {
                //GraphicalSociety.checkData();
            }
            checkData();
        }

        public void bMainMenu()
        {
            world.audioStore.playClick();
            this.setToMainMenu();
        }

        public void bViewThreats()
        {
            Person p = null;

            if (state == uiState.SOCIETY && GraphicalSociety.focus != null)
            {
                p = GraphicalSociety.focus;
            }
            else
            {
                Hex hex = GraphicalMap.selectedHex;
                if (hex != null && hex.settlement != null && hex.settlement.title != null && hex.settlement.title.heldBy != null)
                    p = hex.settlement.title.heldBy;
            }

            if (p == null)
            {
                return;
            }

            try
            {
                addBlocker(world.prefabStore.getScrollSetThreats(p.threatEvaluations).gameObject);
            }catch(Exception e)
            {
                return;
            }

            world.audioStore.playClick();
        }

        public void bViewVotes()
        {
            Person p = null;
            if (state == uiState.SOCIETY && GraphicalSociety.focus != null)
            {
                p = GraphicalSociety.focus;
            }
            else
            {
                Hex hex = GraphicalMap.selectedHex;
                if (hex != null && hex.settlement != null && hex.settlement.title != null && hex.settlement.title.heldBy != null)
                    p = hex.settlement.title.heldBy;
            }

            if (p == null)
                return;

            VoteSession vs = p.society.voteSession;
            if (vs == null)
                return;

            world.audioStore.playClick();

            vs.assignVoters();
            addBlocker(world.prefabStore.getScrollSetVotes(p, vs.issue).gameObject);
        }
    }
}
