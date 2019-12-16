using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code
{
    public class PrefabStore : MonoBehaviour
    {
        public World world;
        public UIMaster ui { get { return world.ui; } }

        public GameObject graphicalProperty;
        public GameObject graphicalHex;
        public GameObject graphicalUnit;
        public GameObject prefabBaseHex;
        public GameObject prefabPlayback;
        public GameObject prefabNameTag;
        public GameObject prefabNameTagSmall;
        public GameObject prefabSocietyName;
        public GameObject prefabHexEdgeSprite;
        public GameObject prefabHexEdge2Sprite;
        public GameObject prefabIconHex;
        public GameObject prefabWorldMap;
        public GameObject prefabLinkLine;
        public GameObject prefabMsg;
        public GameObject prefabImgMsg;
        public GameObject prefabSlot;
        public GameObject prefabParticleCombat;
        public GameObject prefabAlertPopup;
        public GameObject prefabVote;
        public GameObject prefabVoteReasons;
        public GameObject scrollSet;
        public GameObject menuBox;
        public GameObject menuIconicBox;
        public GameObject personBox;
        public GameObject voteBox;
        public GameObject abilityBox;
        public GameObject popScrollVoteIssue;
        public GameObject xBoxDate;
        public GameObject xBoxThreat;
        public GameObject xScrollSet;
        public GameObject boxInvite;
        public GameObject activityBox;
        public GameObject actChain1;
        public GameObject prefabPersonInspect;
        public GameObject prefabSocietyInspect;
        public GameObject pfbGraphicalProperty;
        public GameObject mapMsg;
        public GameObject prefabVictoryBox;
        public GameObject popTutorial;


        public PopupNameTag getNameTag(string name, Color color)
        {
            GameObject obj = Instantiate(prefabNameTag) as GameObject;
            PopupNameTag news = obj.GetComponent<PopupNameTag>();
            news.words.text = name;
            news.words.color = color;
            return news;
        }
        public PopupNameTag getNameTagSmall(string name)
        {
            GameObject obj = Instantiate(prefabNameTagSmall) as GameObject;
            PopupNameTag news = obj.GetComponent<PopupNameTag>();
            news.words.text = name;
            return news;
        }

        public GraphicalHex getGraphicalHex(Hex hex)
        {
            GameObject general = Instantiate(graphicalHex) as GameObject;
            GraphicalHex specific = general.GetComponent<GraphicalHex>();
            specific.hex = hex;
            specific.world = hex.map.world;
            specific.map = hex.map;
            specific.checkData();
            return specific;
        }

        public GraphicalLink getGraphicalLink(Link link)
        {
            GameObject general = Instantiate(prefabLinkLine) as GameObject;
            GraphicalLink specific = general.GetComponent<GraphicalLink>();
            specific.setTo(link);

            return specific;
        }

        public GraphicalSlot getGraphicalSlot(Person p)
        {
            GameObject obj = Instantiate(prefabSlot) as GameObject;
            GraphicalSlot specific = obj.GetComponent<GraphicalSlot>();

            GraphicalSociety.loadedSlots.Add(specific);
            specific.world = world;

            specific.setTo(p);
            return specific;
        }

        public GameObject getHexEdgeSprite()
        {
            return Instantiate(prefabHexEdgeSprite) as GameObject;
        }
        public GameObject getHexEdge2Sprite()
        {
            return Instantiate(prefabHexEdge2Sprite) as GameObject;
        }

        private PopupXScroll getInnerXScrollSet()
        {
            GameObject obj = Instantiate(xScrollSet) as GameObject;
            PopupXScroll specific = obj.GetComponent<PopupXScroll>();
            if (specific == null) { World.log("Unable to find scrip subobject"); }
            specific.ui = world.ui;
            specific.next.onClick.AddListener(delegate { specific.bNext(); });
            specific.prev.onClick.AddListener(delegate { specific.bPrev(); });
            specific.cancel.onClick.AddListener(delegate { specific.bCancel(); });
            return specific;
        }

        public PopupXScroll getScrollSetThreats(List<ThreatItem> threats)
        {
            PopupXScroll specific = getInnerXScrollSet();

            List<ThreatItem> dupe = new List<ThreatItem>();
            foreach (ThreatItem item in threats)
            {
                dupe.Add(item);
            }
            List<ThreatItem> ordered = new List<ThreatItem>();
            int viewedCount = 0;
            while (dupe.Count > 0)
            {
                ThreatItem best = dupe[0];
                foreach (ThreatItem item in dupe)
                {
                    if (item.threat > best.threat)
                    {
                        best = item;
                    }
                }
                if (best.threat < 75 && best.group != null && best.group is Society && viewedCount >= 3)
                {
                    //Skip it, we don't need to list all minor factions we aren't scared of
                }
                else
                {
                    viewedCount += 1;
                    ordered.Add(best);
                }
                dupe.Remove(best);
            }

            foreach (ThreatItem item in ordered)
            {
                PopupXBoxThreat box = getThreatBox(item);
                box.gameObject.transform.SetParent(specific.gameObject.transform);
                specific.scrollables.Add(box);
            }

            return specific;
        }

        public PopupXScroll getScrollSetVotes(Person p, VoteIssue vi)
        {
            PopupXScroll specific = getInnerXScrollSet();

            foreach (VoteOption vo in vi.options)
            {
                PopupXBoxThreat box = getVoteReasonBox(vi, vo, p);
                box.gameObject.transform.SetParent(specific.gameObject.transform);
                specific.scrollables.Add(box);
            }

            return specific;
        }

        public PopupXBoxThreat getThreatBox(ThreatItem item)
        {
            GameObject obj = Instantiate(xBoxThreat) as GameObject;
            PopupXBoxThreat specific = obj.GetComponent<PopupXBoxThreat>();
            specific.setTo(item);
            specific.body = "The perceived threats and a breakdown of the reasons.";

            return specific;
        }

        public PopupXBoxThreat getVoteReasonBox(VoteIssue vi, VoteOption vo, Person p)
        {
            GameObject obj = Instantiate(xBoxThreat) as GameObject;
            PopupXBoxThreat specific = obj.GetComponent<PopupXBoxThreat>();
            specific.setTo(vi, vo, p);
            specific.body = "The available vote options and the reasons behind each choice.";

            return specific;
        }

        private PopupScrollSet getInnerScrollSet()
        {
            GameObject obj = Instantiate(scrollSet) as GameObject;
            PopupScrollSet specific = obj.GetComponent<PopupScrollSet>();
            specific.ui = ui;
            specific.next.onClick.AddListener(delegate { specific.bNext(); });
            specific.prev.onClick.AddListener(delegate { specific.bPrev(); });
            specific.cancel.onClick.AddListener(delegate { specific.bCancel(); });
            specific.select.onClick.AddListener(delegate { specific.bSelect(); });
            return specific;
        }

        public PopupScrollSet getScrollSet(Ab_Soc_ProposeVote ab,Society soc,List<VoteIssue> issues)
        {
            PopupScrollSet specific = getInnerScrollSet();

            foreach (VoteIssue issue in issues)
            {
                PopupBoxVoteIssue box = getVoteIssueBox(ab,soc,issue);
                box.gameObject.transform.SetParent(specific.gameObject.transform);
                specific.scrollables.Add(box);
            }

            return specific;
        }

        public PopupTutorialMsg getTutorial(int item)
        {
            GameObject obj = Instantiate(popTutorial) as GameObject;
            PopupTutorialMsg msg = obj.GetComponent<PopupTutorialMsg>();
            msg.bDismiss.onClick.AddListener(delegate { msg.dismiss(); });
            msg.ui = ui;
            msg.setTo(item);
            return msg;
        }

        private PopupBoxVoteIssue getVoteIssueBox(Ab_Soc_ProposeVote ab,Society soc, VoteIssue issue)
        {
            GameObject obj = Instantiate(popScrollVoteIssue) as GameObject;
            PopupBoxVoteIssue msg = obj.GetComponent<PopupBoxVoteIssue>();
            msg.setTo(ab,soc, issue);
            return msg;
        }

        public PopupScrollSet getScrollSet(List<int> indices)
        {
            PopupScrollSet specific = getInnerScrollSet();

            foreach (int q in indices)
            {
                PopupBoxPerson box = getPersonBox();
                box.gameObject.transform.SetParent(specific.gameObject.transform);
                specific.scrollables.Add(box);
            }

            return specific;
        }
        public PopupScrollSet getScrollSet(List<Ability> abilities, List<Ability> abilities_uncastable, Person person)
        {
            PopupScrollSet specific = getInnerScrollSet();

            foreach (Ability b in abilities)
            {
                PopupBoxAbility box = getAbilityBox(b, person);
                box.gameObject.transform.SetParent(specific.gameObject.transform);
                specific.scrollables.Add(box);
            }
            foreach (Ability b in abilities_uncastable)
            {
                PopupBoxAbility box = getAbilityBox(b, person);
                box.gameObject.transform.SetParent(specific.gameObject.transform);
                specific.scrollables.Add(box);
            }

            return specific;
        }
        public PopupScrollSet getScrollSet(List<Ability> abilities, List<Ability> abilities_uncastable, Hex hex)
        {
            PopupScrollSet specific = getInnerScrollSet();

            foreach (Ability b in abilities)
            {
                PopupBoxAbility box = getAbilityBox(b,hex);
                box.gameObject.transform.SetParent(specific.gameObject.transform);
                specific.scrollables.Add(box);
            }
            foreach (Ability b in abilities_uncastable)
            {
                PopupBoxAbility box = getAbilityBox(b, hex);
                box.gameObject.transform.SetParent(specific.gameObject.transform);
                specific.scrollables.Add(box);
            }

            return specific;
        }
        public PopupScrollSet getScrollSet(VoteSession sess,List<VoteOption> votes)
        {
            PopupScrollSet specific = getInnerScrollSet();

            foreach (VoteOption b in votes)
            {
                PopupBoxVote box = getVoteBox(sess,b);
                box.gameObject.transform.SetParent(specific.gameObject.transform);
                specific.scrollables.Add(box);
            }

            return specific;
        }
        public PopupBoxVote getVoteBox(VoteSession sess,VoteOption option)
        {
            GameObject obj = Instantiate(voteBox) as GameObject;
            PopupBoxVote specific = obj.GetComponent<PopupBoxVote>();
            specific.setTo(sess,option);

            return specific;
        }
        public MonoMapMsg getMapMsg(MsgEvent ev)
        {
            GameObject obj = Instantiate(mapMsg) as GameObject;
            MonoMapMsg msg = obj.GetComponent<MonoMapMsg>();
            return msg;
        }

        public PopupBoxPerson getPersonBox()
        {
            GameObject obj = Instantiate(personBox) as GameObject;
            PopupBoxPerson specific = obj.GetComponent<PopupBoxPerson>();
            //specific.setTo(viewer, p, select);

            return specific;
        }
        public PopupBoxAbility getAbilityBox(Ability a,Hex hex)
        {
            GameObject obj = Instantiate(abilityBox) as GameObject;
            PopupBoxAbility specific = obj.GetComponent<PopupBoxAbility>();
            specific.setTo(a,hex);

            return specific;
        }
        public PopupBoxAbility getAbilityBox(Ability a, Person person)
        {
            GameObject obj = Instantiate(abilityBox) as GameObject;
            PopupBoxAbility specific = obj.GetComponent<PopupBoxAbility>();
            specific.setTo(a, person);

            return specific;
        }
        public void popMsg(string words)
        {
            if (world.displayMessages == false) { return; }

            GameObject obj = Instantiate(prefabMsg) as GameObject;
            PopupMsg specific = obj.GetComponent<PopupMsg>();
            specific.ui = ui;
            specific.text.text = words;
            specific.bDismiss.onClick.AddListener(delegate { specific.dismiss(); });
            ui.addBlocker(specific.gameObject);
        }
        public void popImgMsg(string body,string flavour)
        {
            if (world.displayMessages == false) { return; }

            GameObject obj = Instantiate(prefabImgMsg) as GameObject;
            PopupImgMsg specific = obj.GetComponent<PopupImgMsg>();
            specific.ui = ui;
            specific.textBody.text = body;
            specific.textFlavour.text = flavour;
            specific.bDismiss.onClick.AddListener(delegate { specific.dismiss(); });
            ui.addBlocker(specific.gameObject);
        }

        internal GraphicalProperty getGraphicalProperty(Map map, Property p)
        {
            GameObject obj = Instantiate(graphicalProperty) as GameObject;
            GraphicalProperty property = obj.GetComponent<GraphicalProperty>();
            property.setTo(p,world);
            return property;
        }

        public PopupBaseHex getBaseHex()
        {
            GameObject obj = Instantiate(prefabBaseHex) as GameObject;
            PopupBaseHex specific = obj.GetComponent<PopupBaseHex>();

            return specific;
        }

        public PopupPlayback getPlayback(World world, Map map)
        {

            GameObject objHex = Instantiate(prefabPlayback) as GameObject;
            PopupPlayback h = objHex.GetComponent<PopupPlayback>();
            h.back.onClick.AddListener(delegate { h.buttonBack(); });
            h.pause.onClick.AddListener(delegate { h.buttonPause(); });
            h.replay.onClick.AddListener(delegate { h.buttonReplay(); });


            h.setup(world, map);
            return h;
        }

        public void popVictoryBox()
        {
            if (world.displayMessages == false) { return; }

            GameObject obj = Instantiate(prefabVictoryBox) as GameObject;
            PopupVictory specific = obj.GetComponent<PopupVictory>();
            specific.bDismiss.onClick.AddListener(delegate { specific.dismiss(); });
            specific.ui = ui;
            ui.addBlocker(specific.gameObject);
        }
        /*
        public PopupBoxPerson getPersonBox(Person p, Selector_Person select, Person viewer)
        {
            GameObject obj = Instantiate(personBox) as GameObject;
            PopupBoxPerson specific = obj.GetComponent<PopupBoxPerson>();
            specific.setTo(viewer, p, select);

            return specific;
        }
        public void particleCombat(Hex a, Hex b)
        {
            if (a.outer == null && b.outer == null) { return; }//Particles may not be invisible

            GameObject obj = Instantiate(prefabParticleCombat) as GameObject;
            ParticleCombat specific = obj.GetComponent<ParticleCombat>();
            specific.map = ui.world.map;
            specific.a = a;
            specific.b = b;
        }

        public void popImgMsg(string title, string words, string imgName, string flavour)
        {
            if (world.displayMessages == false) { return; }

            GameObject obj = Instantiate(prefabImgMsg) as GameObject;
            PopupImgMsg specific = obj.GetComponent<PopupImgMsg>();
            specific.ui = ui;
            specific.image.sprite = world.textureStore.images[imgName];
            specific.text.text = words;
            specific.flavourText.text = TextStore.getFlavour(flavour);
            specific.bDismiss.onClick.AddListener(delegate { specific.dismiss(); });
            ui.addBlocker(specific.gameObject);
        }
        public void popImgMsgToFrontOfQueue(string title, string words, string imgName, string flavour)
        {
            if (world.displayMessages == false) { return; }

            GameObject obj = Instantiate(prefabImgMsg) as GameObject;
            PopupImgMsg specific = obj.GetComponent<PopupImgMsg>();
            specific.ui = ui;
            specific.image.sprite = world.textureStore.images[imgName];
            specific.text.text = words;
            specific.flavourText.text = TextStore.getFlavour(flavour);
            specific.bDismiss.onClick.AddListener(delegate { specific.dismiss(); });
            ui.addBlockerToQueueFrontDontHide(specific.gameObject);
        }


        public PopupXScroll getScrollSetDates(List<string> actTexts, List<string> dateTexts, List<string> plotTexts)
        {
            PopupXScroll specific = getInnerXScrollSet();

            for (int i = 0; i < actTexts.Count; i++)
            {
                PopupXBoxDate box = getDateBox(actTexts[i], dateTexts[i], plotTexts[i]);
                box.gameObject.transform.SetParent(specific.gameObject.transform);
                specific.scrollables.Add(box);
            }

            return specific;
        }

        public PopupScrollSet getScrollSet(List<Activity> activities, Person person)
        {
            PopupScrollSet specific = getInnerScrollSet();

            foreach (Activity act in activities)
            {
                PopupBoxActivity box = getActivityBox(person, act);
                box.gameObject.transform.SetParent(specific.gameObject.transform);
                specific.scrollables.Add(box);
            }

            return specific;
        }
        public PopupBoxActivity getActivityBox(Person person, Activity act)
        {
            GameObject obj = Instantiate(activityBox) as GameObject;
            PopupBoxActivity specific = obj.GetComponent<PopupBoxActivity>();
            specific.setTo(act, person);

            return specific;
        }
        public GraphicalProperty getGraphicalProperty(Property property)
        {
            GameObject obj = Instantiate(pfbGraphicalProperty) as GameObject;
            GraphicalProperty specific = obj.GetComponent<GraphicalProperty>();
            specific.setTo(property);
            property.outer = specific;

            return specific;
        }
        public PopupBoxInvite getBoxInvite(Invite invite)
        {
            GameObject obj = Instantiate(boxInvite) as GameObject;
            PopupBoxInvite specific = obj.GetComponent<PopupBoxInvite>();
            specific.setTo(invite);

            return specific;
        }

        public PopupXBoxDate getDateBox(string act, string date, string plot)
        {
            GameObject obj = Instantiate(xBoxDate) as GameObject;
            PopupXBoxDate specific = obj.GetComponent<PopupXBoxDate>();
            specific.setTo(act, date, plot);

            return specific;
        }

        public PopupActivityChain getActivityChain(ActivityInstance activity, Person guest, int phase)
        {
            GameObject obj = Instantiate(actChain1) as GameObject;
            PopupActivityChain specific = obj.GetComponent<PopupActivityChain>();
            specific.ui = ui;
            specific.setup(activity, guest, phase);
            specific.dismiss.onClick.AddListener(delegate { specific.bDismiss(); });
            specific.act.onClick.AddListener(delegate { specific.bAct(); });

            return specific;
        }
        public void popMsg(string words)
        {
            if (world.displayMessages == false) { return; }

            GameObject obj = Instantiate(prefabMsg) as GameObject;
            PopupMsg specific = obj.GetComponent<PopupMsg>();
            specific.ui = ui;
            specific.text.text = words;
            specific.bDismiss.onClick.AddListener(delegate { specific.dismiss(); });
            ui.addBlocker(specific.gameObject);
        }

        public PopupAlert popupAlert(string title, string words, Vote targetVote, Society targetSoc, Person targetPerson, Hex targetHex, string lockout)
        {

            GameObject obj = Instantiate(prefabAlertPopup) as GameObject;
            PopupAlert news = obj.GetComponent<PopupAlert>();
            news.ui = ui;
            news.text.text = words;
            news.title.text = title;
            news.targetSoc = targetSoc;
            news.targetPerson = targetPerson;
            news.targetHex = targetHex;
            news.targetVote = targetVote;
            news.matchString = lockout;
            news.similarString.text = lockout;

            news.flagLeft.color = new Color(0, 0, 0, 0);
            news.flagRight.color = new Color(0, 0, 0, 0);
            if (news.targetSoc != null)
            {
                news.flagLeft.color = targetSoc.color;
                news.flagRight.color = targetSoc.color;
            }

            if (targetVote == null)
            {
                news.buttonVote.gameObject.SetActive(false);
            }


            news.dismiss.onClick.AddListener(delegate { news.bDismiss(); });
            news.dismissPerma.onClick.AddListener(delegate { news.bDismissPerma(); });
            news.buttonGoto.onClick.AddListener(delegate { news.bGoto(); });
            news.buttonVote.onClick.AddListener(delegate { news.bVote(); });
            ui.addBlocker(news.gameObject);
            return news;
        }

        public PopupVote getVote(Vote v, Person p)
        {

            GameObject obj = Instantiate(prefabVote) as GameObject;
            PopupVote specific = obj.GetComponent<PopupVote>();
            specific.ui = ui;
            specific.setTo(v, p);

            specific.buttons[0].onClick.AddListener(delegate { specific.receiveVote(0); });
            specific.buttons[1].onClick.AddListener(delegate { specific.receiveVote(1); });
            specific.buttons[2].onClick.AddListener(delegate { specific.receiveVote(2); });
            specific.buttons[3].onClick.AddListener(delegate { specific.receiveVote(3); });

            specific.bAbstain.onClick.AddListener(delegate { specific.receiveVote(-1); });
            specific.bDismiss.onClick.AddListener(delegate { specific.dismiss(); });
            specific.bSeeReasons.onClick.AddListener(delegate { specific.seeReasons(); });
            return specific;
        }
        public PopupVoteReasons getVoteReasons(Vote v, Person p)
        {

            GameObject obj = Instantiate(prefabVoteReasons) as GameObject;
            PopupVoteReasons specific = obj.GetComponent<PopupVoteReasons>();
            specific.ui = ui;
            specific.setTo(v, p);

            specific.dismiss.onClick.AddListener(delegate { specific.bDismiss(); });
            return specific;
        }
        public PopupPersonInspect getPersonInspect(Person p)
        {

            GameObject obj = Instantiate(prefabPersonInspect) as GameObject;
            PopupPersonInspect specific = obj.GetComponent<PopupPersonInspect>();
            specific.ui = ui;
            specific.setTo(p);

            specific.bViewThreats.onClick.AddListener(delegate { specific.viewThreats(); });
            specific.bDismiss.onClick.AddListener(delegate { specific.dismiss(); });
            return specific;
        }
        public PopupSocietyInspect getSocietyInspect(Society p)
        {

            GameObject obj = Instantiate(prefabSocietyInspect) as GameObject;
            PopupSocietyInspect specific = obj.GetComponent<PopupSocietyInspect>();
            specific.ui = ui;
            specific.setTo(p);

            specific.bViewThreats.onClick.AddListener(delegate { specific.viewThreats(); });
            specific.bDismiss.onClick.AddListener(delegate { specific.dismiss(); });
            return specific;
        }
        public GraphicalSlot getGraphicalSlot(Slot s)
        {
            foreach (GraphicalSlot loaded in GraphicalSociety.loadedSlots)
            {
                if (loaded.slot == s)
                {
                    loaded.apoptose = false;
                    s.outer = loaded;
                    return loaded;
                }
            }

            GameObject obj = Instantiate(prefabSlot) as GameObject;
            GraphicalSlot specific = obj.GetComponent<GraphicalSlot>();


            GraphicalSociety.loadedSlots.Add(specific);
            specific.world = world;

            specific.slot = s;
            s.outer = specific;
            s.outer.apoptose = false;
            specific.inner = s.person;


            specific.checkData();
            return specific;
        }

        private PopupScrollSet getInnerScrollSet()
        {
            GameObject obj = Instantiate(scrollSet) as GameObject;
            PopupScrollSet specific = obj.GetComponent<PopupScrollSet>();
            specific.ui = ui;
            specific.next.onClick.AddListener(delegate { specific.bNext(); });
            specific.prev.onClick.AddListener(delegate { specific.bPrev(); });
            specific.cancel.onClick.AddListener(delegate { specific.bCancel(); });
            specific.select.onClick.AddListener(delegate { specific.bSelect(); });
            return specific;
        }

        public PopupBoxMenu getMenuBox(MenuInner inner, int opt)
        {

            GameObject obj = Instantiate(menuBox) as GameObject;
            PopupBoxMenu specific = obj.GetComponent<PopupBoxMenu>();
            specific.menuInner = inner;
            specific.menuOpt = opt;
            specific.button.onClick.AddListener(delegate { specific.clicked(world.map); });
            specific.tTitle.text = inner.getBoxTitle(opt);
            specific.tCost.text = inner.getBoxCost(opt);

            return specific;
        }
        public PopupBoxIconicMenu getMenuIconicBox(MenuIconicInner inner, int opt)
        {

            GameObject obj = Instantiate(menuIconicBox) as GameObject;
            PopupBoxIconicMenu specific = obj.GetComponent<PopupBoxIconicMenu>();
            specific.menuInner = inner;
            specific.menuOpt = opt;
            specific.button.onClick.AddListener(delegate { specific.clicked(world.map); });
            specific.tTitle.text = inner.getBoxTitle(opt);
            specific.tCost.text = inner.getBoxCost(opt);
            specific.img.sprite = inner.getIcon(opt);

            return specific;
        }
        public PopupBoxPerson getPersonBox(Person p, Selector_Person select, Person viewer)
        {
            GameObject obj = Instantiate(personBox) as GameObject;
            PopupBoxPerson specific = obj.GetComponent<PopupBoxPerson>();
            specific.setTo(viewer, p, select);

            return specific;
        }
        public PopupScrollSet getScrollSet(List<Person> people, Selector_Person select, Person viewer)
        {
            PopupScrollSet specific = getInnerScrollSet();

            foreach (Person act in people)
            {
                PopupBoxPerson box = getPersonBox(act, select, viewer);
                box.gameObject.transform.SetParent(specific.gameObject.transform);
                specific.scrollables.Add(box);
            }
            return specific;
        }
        public PopupScrollSet getScrollsetMenu(MenuIconicInner inner)
        {
            PopupScrollSet set = getInnerScrollSet();

            set.body.text = inner.getMenuBody();
            set.title.text = inner.getMenuTitle();

            for (int i = 0; i < inner.getNOpts(); i++)
            {
                PopupBoxIconicMenu box = getMenuIconicBox(inner, i);
                box.gameObject.transform.SetParent(set.gameObject.transform);
                set.scrollables.Add(box);
                box.set = set;
            }
            return set;
        }
        public PopupScrollSet getScrollsetMenu(MenuInner inner)
        {
            PopupScrollSet set = getInnerScrollSet();

            set.body.text = inner.getMenuBody();
            set.title.text = inner.getMenuTitle();

            for (int i = 0; i < inner.getNOpts(); i++)
            {
                PopupBoxMenu box = getMenuBox(inner, i);
                box.gameObject.transform.SetParent(set.gameObject.transform);
                set.scrollables.Add(box);
                box.set = set;
            }
            return set;
        }

        public PopupWorldMap getWorldMap(World world, Map map)
        {

            GameObject objHex = Instantiate(prefabWorldMap) as GameObject;
            PopupWorldMap h = objHex.GetComponent<PopupWorldMap>();
            h.back.onClick.AddListener(delegate { h.buttonBack(); });

            h.setup(world, map);
            return h;
        }

        public PopupIconHex getIconHex()
        {
            GameObject obj = Instantiate(prefabIconHex) as GameObject;
            PopupIconHex news = obj.GetComponent<PopupIconHex>();
            return news;
        }
        public PopupNameTag getSocietyName(string name)
        {
            GameObject obj = Instantiate(prefabSocietyName) as GameObject;
            PopupNameTag news = obj.GetComponent<PopupNameTag>();
            news.words.text = name;
            return news;
        }
        */
    }
}
