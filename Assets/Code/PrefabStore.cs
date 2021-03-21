using System;
using System.Collections.Generic;
using System.IO;
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
        public GameObject prefabMsgAgents;
        public GameObject prefabMsgAgentsDeath;
        public GameObject prefabHint;
        public GameObject prefabMsgTreeBackground;
        public GameObject prefabPumpkinVictory;
        public GameObject prefabImgMsg;
        public GameObject prefabVoteMsg;
        public GameObject prefabCredits;
        public GameObject prefabSlot;
        public GameObject prefabParticleCombat;
        public GameObject prefabAlertPopup;
        public GameObject prefabVote;
        public GameObject prefabVoteReasons;
        public GameObject prefabIOOpts;
        public GameObject prefabKeybinds;
        public GameObject scrollSet;
        public GameObject menuBox;
        public GameObject menuIconicBox;
        public GameObject personBox;
        public GameObject voteBox;
        public GameObject abilityBox;
        public GameObject saveBox;
        public GameObject popScrollVoteIssue;
        public GameObject popAgentBox;
        public GameObject popScrollingTextBox;
        public GameObject xBoxDate;
        public GameObject xBoxThreat;
        public GameObject xScrollSet;
        public GameObject xScrollSetGods;
        public GameObject xBoxGodSelect;
        public GameObject boxInvite;
        public GameObject activityBox;
        public GameObject actChain1;
        public GameObject prefabPersonInspect;
        public GameObject prefabSocietyInspect;
        public GameObject pfbGraphicalProperty;
        public GameObject mapMsg;
        public GameObject prefabVictoryBox;
        public GameObject prefabDefeatBox;
        public GameObject prefabEndGameCyclic;
        public GameObject popTutorial;
        public GameObject prefabPersonPortrait;
        public GameObject prefabGameOptions;
        public GameObject prefabGameOptionsSimplified;
        public GameObject prefabCombatParticle;
        public GameObject prefabPopVoterBar;
        public GameObject prefabPopOptBar;
        public GameObject prefabAutosave;
        public GameObject prefabSaveName;
        public GameObject prefabLightbringerDiag;
        public GameObject prefabEvent;

        public PopOptBar getVoteOptBar(VoteOption opt, VoteSession sess)
        {
            GameObject obj = Instantiate(prefabPopOptBar) as GameObject;
            PopOptBar part = obj.GetComponent<PopOptBar>();
            part.setTo(opt, sess);
            return part;
        }
        public PopVoterBar getVoterBar(Person p)
        {
            GameObject obj = Instantiate(prefabPopVoterBar) as GameObject;
            PopVoterBar part = obj.GetComponent<PopVoterBar>();
            part.setTo(p);
            return part;
        }

        public void particleCombat(Hex a, Hex b)
        {
            if (a.outer == null || b.outer == null) { return; }//No need to make invisible particles

            GameObject obj = Instantiate(prefabCombatParticle) as GameObject;
            ParticleCombat part = obj.GetComponent<ParticleCombat>();
            part.map = world.map;
            part.a = a;
            part.b = b;
        }
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

        public GraphicalSlot getGraphicalSlotPlaceholder(string title)
        {
            GameObject obj = Instantiate(prefabSlot) as GameObject;
            GraphicalSlot specific = obj.GetComponent<GraphicalSlot>();

            specific.world = world;

            specific.setToPlaceholder(title);
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

        private PopupXScroll getInnerXScrollSetGods()
        {
            GameObject obj = Instantiate(xScrollSetGods) as GameObject;
            PopupXScroll specific = obj.GetComponent<PopupXScroll>();
            if (specific == null) { World.log("Unable to find scrip subobject"); }
            specific.ui = world.ui;
            specific.next.onClick.AddListener(delegate { specific.bNext(); });
            specific.prev.onClick.AddListener(delegate { specific.bPrev(); });
            specific.cancel.onClick.AddListener(delegate { specific.bCancel(); });
            return specific;
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

        public PopupScrollSet getScrollSetAgents(List<int> indices, List<string> titles, List<string> bodies, List<Sprite> icons)
        {
            PopupScrollSet specific = getInnerScrollSet();

            for (int i = 0; i < bodies.Count; i++)
            {
                PopupBoxAgent box = getAgentBox(indices[i], icons[i], titles[i], bodies[i]);
                box.gameObject.transform.SetParent(specific.gameObject.transform);
                specific.scrollables.Add(box);
            }

            return specific;
        }
        public PopupScrollSet getScrollSetText(List<string> items, bool invertOrder, SelectClickReceiver receiver = null)
        {
            PopupScrollSet specific = getInnerScrollSet();

            if (!invertOrder)
            {
                for (int i = 0; i < items.Count; i++)
                {
                    PopupBoxText box = getScrollingTextBox(items[i]);
                    box.gameObject.transform.SetParent(specific.gameObject.transform);
                    box.clickReceiver = receiver;
                    specific.scrollables.Add(box);
                }
            }
            else
            {
                for (int i = items.Count - 1; i >= 0; i--)
                {
                    PopupBoxText box = getScrollingTextBox(items[i]);
                    box.gameObject.transform.SetParent(specific.gameObject.transform);
                    box.clickReceiver = receiver;
                    specific.scrollables.Add(box);
                }
            }

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

        public PopupXScroll getScrollSetGods(List<God> gods)
        {
            PopupXScroll specific = getInnerXScrollSetGods();

            foreach (God item in gods)
            {
                PopupXBoxGodSelectMsg box = getGodBox(item);
                box.gameObject.transform.SetParent(specific.gameObject.transform);
                box.ui = world.ui;
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

        public PopupXBoxGodSelectMsg getGodBox(God item)
        {
            GameObject obj = Instantiate(xBoxGodSelect) as GameObject;
            PopupXBoxGodSelectMsg specific = obj.GetComponent<PopupXBoxGodSelectMsg>();
            specific.setTo(ui.world, item);

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

        public PopupScrollSet getScrollSet(Ab_Soc_ProposeVote ab, Society soc, List<VoteIssue> issues)
        {
            PopupScrollSet specific = getInnerScrollSet();

            foreach (VoteIssue issue in issues)
            {
                PopupBoxVoteIssue box = getVoteIssueBox(ab, soc, issue);
                box.gameObject.transform.SetParent(specific.gameObject.transform);
                specific.scrollables.Add(box);
            }

            return specific;
        }
        public void popScrollSetSaves()
        {
            PopupScrollSet specific = getInnerScrollSet();

            var info = new DirectoryInfo(World.saveFolder);
            var fileInfo = info.GetFiles();
            List<FileInfo> files = new List<FileInfo>();
            List<string> versionNumber = new List<string>();

            foreach (FileInfo file in fileInfo)
            {
                if (file.Name.EndsWith(".sv"))
                {
                    //Here we're going to read in the header of the file only (plus a bit more, to account for us shoving more crud into the header at a future time).
                    //Saves loading the whole file just to check version number
                    string[] headerStrings = null;
                    using (FileStream fs = File.OpenRead(file.FullName))
                    {
                        byte[] b = new byte[1024 * 2];
                        UTF8Encoding temp = new UTF8Encoding(true);

                        fs.Read(b, 0, b.Length);//Read out enough to cover the entire header (regardless of how large it eventually gets)
                        headerStrings = temp.GetString(b).Split('\n');
                        for (int i = 0; i < headerStrings.Length; i++)
                        {
                            //World.log("Header " + i + " " + headerStrings[i]);
                        }
                        World.log("Read file: " + temp.GetString(b));
                        fs.Close();
                    }

                    if (headerStrings == null) { continue; }
                    if (headerStrings[0] != "Version;" + World.versionNumber + ";" + World.subversionNumber) { versionNumber.Add("" + headerStrings[0]); }//Old version, don't try to load
                    else { versionNumber.Add(null); }
                    files.Add(file);
                }
            }
            if (files.Count > 0)
            {
                //Give 'em the loadable ones first, then the rest of the garbage
                for (int i = 0; i < files.Count; i++)
                {
                    if (versionNumber[i] != null) { continue; }
                    var saveBox = getSaveBox();
                    saveBox.setTo(files[i], versionNumber[i]);
                    saveBox.gameObject.transform.SetParent(specific.gameObject.transform);
                    specific.scrollables.Add(saveBox);
                }
                for (int i = 0; i < files.Count; i++)
                {
                    if (versionNumber[i] == null) { continue; }
                    var saveBox = getSaveBox();
                    saveBox.setTo(files[i], versionNumber[i]);
                    saveBox.gameObject.transform.SetParent(specific.gameObject.transform);
                    specific.scrollables.Add(saveBox);
                }
            }
            else
            {
                popMsg("No save games found to load",true);
            }

            ui.addBlocker(specific.gameObject);
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

        private PopupBoxVoteIssue getVoteIssueBox(Ab_Soc_ProposeVote ab, Society soc, VoteIssue issue)
        {
            GameObject obj = Instantiate(popScrollVoteIssue) as GameObject;
            PopupBoxVoteIssue msg = obj.GetComponent<PopupBoxVoteIssue>();
            msg.setTo(ab, soc, issue);
            return msg;
        }
        private PopupBoxAgent getAgentBox(int index, Sprite icon, string title, string body)
        {
            GameObject obj = Instantiate(popAgentBox) as GameObject;
            PopupBoxAgent msg = obj.GetComponent<PopupBoxAgent>();
            msg.setTo(index, title, body, icon);
            return msg;
        }
        private PopupBoxText getScrollingTextBox(string body)
        {
            GameObject obj = Instantiate(popScrollingTextBox) as GameObject;
            PopupBoxText msg = obj.GetComponent<PopupBoxText>();
            msg.setTo(body);
            return msg;
        }

        public PopupScrollSet getScrollSetRelations(List<RelObj> rels)
        {
            PopupScrollSet specific = getInnerScrollSet();

            foreach (RelObj rel in rels)
            {
                PopupBoxPerson box = getPersonBox();
                box.gameObject.transform.SetParent(specific.gameObject.transform);
                specific.scrollables.Add(box);
                if (World.staticMap.persons[rel.them] == null) { continue; }
                box.setTo(World.staticMap.persons[rel.them]);
                box.body.text = "Liking: " + (int)rel.getLiking() + "\nSuspicion: " + (int)(rel.suspicion * 100) + "%";
                float scale = (float)rel.getLiking();
                if (scale > 1) { scale = 1; }
                else if (scale < -1) { scale = -1; }
                float r = 0;
                float g = 0;
                float b = 0;
                if (scale > 0) { g = scale; }
                else { r = -scale; }
                box.body.color = new Color(r, g, b);
            }

            return specific;
        }
        public PopupScrollSet getScrollSetRelationsReflexive(List<RelObj> rels)
        {
            PopupScrollSet specific = getInnerScrollSet();

            foreach (RelObj rel in rels)
            {
                PopupBoxPerson box = getPersonBox();
                box.gameObject.transform.SetParent(specific.gameObject.transform);
                specific.scrollables.Add(box);
                if (World.staticMap.persons[rel.them] == null) { continue; }
                box.setTo(World.staticMap.persons[rel.me.index]);
                box.body.text = "Their Liking: " + (int)rel.getLiking() + "\nTheir Suspicion: " + (int)(rel.suspicion * 100) + "%";
                float scale = (float)rel.getLiking();
                if (scale > 1) { scale = 1; }
                else if (scale < -1) { scale = -1; }
                float r = 0;
                float g = 0;
                float b = 0;
                if (scale > 0) { g = scale; }
                else { r = -scale; }
                box.body.color = new Color(r, g, b);
            }

            return specific;
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
        public PopupScrollSet getScrollSet(List<Ability> abilities, List<Ability> abilities_uncastable, Unit u)
        {
            PopupScrollSet specific = getInnerScrollSet();

            foreach (Ability b in abilities)
            {
                PopupBoxAbility box = getAbilityBox(b, u);
                box.gameObject.transform.SetParent(specific.gameObject.transform);
                specific.scrollables.Add(box);
            }
            foreach (Ability b in abilities_uncastable)
            {
                PopupBoxAbility box = getAbilityBox(b, u);
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
                PopupBoxAbility box = getAbilityBox(b, hex);
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
        public PopupScrollSet getScrollSet(VoteSession sess, List<VoteOption> votes)
        {
            PopupScrollSet specific = getInnerScrollSet();

            sess.assignVoters();
            foreach (VoteOption b in votes)
            {
                PopupBoxVote box = getVoteBox(sess, b);
                box.gameObject.transform.SetParent(specific.gameObject.transform);
                specific.scrollables.Add(box);
            }

            return specific;
        }
        public PopupBoxVote getVoteBox(VoteSession sess, VoteOption option)
        {
            GameObject obj = Instantiate(voteBox) as GameObject;
            PopupBoxVote specific = obj.GetComponent<PopupBoxVote>();
            specific.setTo(sess, option);

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
        public PopupBoxSavegame getSaveBox()
        {
            GameObject obj = Instantiate(saveBox) as GameObject;
            PopupBoxSavegame specific = obj.GetComponent<PopupBoxSavegame>();
            //specific.setTo(viewer, p, select);

            return specific;
        }
        public PopupBoxAbility getAbilityBox(Ability a, Hex hex)
        {
            GameObject obj = Instantiate(abilityBox) as GameObject;
            PopupBoxAbility specific = obj.GetComponent<PopupBoxAbility>();
            specific.setTo(a, hex);

            return specific;
        }
        public PopupBoxAbility getAbilityBox(Ability a, Unit u)
        {
            GameObject obj = Instantiate(abilityBox) as GameObject;
            PopupBoxAbility specific = obj.GetComponent<PopupBoxAbility>();
            specific.setTo(a, u);

            return specific;
        }
        public PopupBoxAbility getAbilityBox(Ability a, Person person)
        {
            GameObject obj = Instantiate(abilityBox) as GameObject;
            PopupBoxAbility specific = obj.GetComponent<PopupBoxAbility>();
            specific.setTo(a, person);

            return specific;
        }
        public void getGameOptionsPopup()
        {
            GameObject obj = Instantiate(prefabGameOptions) as GameObject;
            PopupGameOptions specific = obj.GetComponent<PopupGameOptions>();
            specific.ui = ui;
            specific.seedField.onEndEdit.AddListener(delegate { specific.onEditEnd(); });
            specific.suspicionGain.onEndEdit.AddListener(delegate { specific.onEditEnd(); });
            specific.awarenessGain.onEndEdit.AddListener(delegate { specific.onEditEnd(); });
            specific.investigatorPercentField.onEndEdit.AddListener(delegate { specific.onEditEnd(); });
            specific.powerGain.onEndEdit.AddListener(delegate { specific.onEditEnd(); });
            specific.sizeXField.onEndEdit.AddListener(delegate { specific.onEditEnd(); });
            specific.sizeYField.onEndEdit.AddListener(delegate { specific.onEditEnd(); });
            specific.historicalField.onEndEdit.AddListener(delegate { specific.onEditEnd(); });

            specific.bEasy.onClick.AddListener(delegate { specific.setEasy(); });
            specific.bMedium.onClick.AddListener(delegate { specific.setMedium(); });
            specific.bHard.onClick.AddListener(delegate { specific.setHard(); });

            specific.bSeedZero.onClick.AddListener(delegate { specific.setSeed0(); });
            specific.bSeedOne.onClick.AddListener(delegate { specific.setSeed1(); });

            specific.currentSeed = Eleven.random.Next();
            specific.setTextFieldsToCurrentValues();
            ui.addBlocker(specific.gameObject);
        }
        public void getGameOptionsPopupSimplified()
        {
            GameObject obj = Instantiate(prefabGameOptionsSimplified) as GameObject;
            PopupGameOptions specific = obj.GetComponent<PopupGameOptions>();
            specific.ui = ui;
            specific.seedField.onEndEdit.AddListener(delegate { specific.onEditEnd(); });
            specific.sizeXField.onEndEdit.AddListener(delegate { specific.onEditEnd(); });
            specific.sizeYField.onEndEdit.AddListener(delegate { specific.onEditEnd(); });
            specific.historicalField.onEndEdit.AddListener(delegate { specific.onEditEnd(); });

            specific.bEasy.onClick.AddListener(delegate { specific.setEasy(); });
            specific.bMedium.onClick.AddListener(delegate { specific.setMedium(); });
            specific.bHard.onClick.AddListener(delegate { specific.setHard(); });

            specific.bSeedZero.onClick.AddListener(delegate { specific.setSeed0(); });
            specific.bSeedOne.onClick.AddListener(delegate { specific.setSeed1(); });

            specific.nAgents += 1;
            specific.currentSeed = Eleven.random.Next();
            specific.setTextFieldsToCurrentValues();
            ui.addBlocker(specific.gameObject);
        }
        public void popGameOpts()
        {
            if (world.ui.blocker != null) { return; }
            if (world.displayMessages == false) { return; }

            GameObject obj = Instantiate(prefabIOOpts) as GameObject;
            PopupIOOptions specific = obj.GetComponent<PopupIOOptions>();
            specific.ui = ui;
            specific.map = ui.world.map;
            specific.bDismiss.onClick.AddListener(delegate { specific.dismiss(); });
            specific.bKeybinds.onClick.AddListener(delegate { popKeybinds(); });
            specific.bEdgeScroll.onClick.AddListener(delegate { specific.toggleEdgeScroll(); });
            specific.bSoundEffects.onClick.AddListener(delegate { specific.toggleSoundEffects(); });
            ui.addBlocker(specific.gameObject);
        }
        public void popKeybinds()
        {
            GameObject obj = Instantiate(prefabKeybinds) as GameObject;
            PopupKeybinds specific = obj.GetComponent<PopupKeybinds>();
            specific.ui = ui;
            specific.bDismiss.onClick.AddListener(delegate { specific.dismiss(); });

            ui.uiInputs.disable = true;
            ui.addBlockerOverride(specific.gameObject);
        }
        public void popMsg(string words,bool force=false)
        {
            if (!force && (world.displayMessages == false)) { return; }

            GameObject obj = Instantiate(prefabMsg) as GameObject;
            PopupMsg specific = obj.GetComponent<PopupMsg>();
            specific.ui = ui;
            specific.text.text = words;
            specific.bDismiss.onClick.AddListener(delegate { specific.dismiss(); });
            ui.addBlocker(specific.gameObject);
        }
        public void popMsgHint(string words, string titleWords,bool force = false)
        {
            if (!force && (world.displayMessages == false)) { return; }

            GameObject obj = Instantiate(prefabHint) as GameObject;
            PopupMsgHint specific = obj.GetComponent<PopupMsgHint>();
            specific.ui = ui;
            specific.text.text = words;
            specific.title.text = titleWords;
            specific.bDismiss.onClick.AddListener(delegate { specific.dismiss(); });
            ui.addBlocker(specific.gameObject);
        }
        public void popMsgAgent(Unit actor, Unit target, string words)
        {
            if (world.displayMessages == false) { return; }

            GameObject obj = Instantiate(prefabMsgAgents) as GameObject;
            PopupMsgAgents specific = obj.GetComponent<PopupMsgAgents>();
            specific.ui = ui;
            specific.setTo(actor, target);
            specific.text.text = words;
            specific.agentA = actor;
            specific.agentB = target;
            specific.bDismiss.onClick.AddListener(delegate { specific.dismiss(); });
            specific.bDismissAgentA.onClick.AddListener(delegate { specific.dismissAgentA(); });
            specific.bDismissAgentB.onClick.AddListener(delegate { specific.dismissAgentB(); });
            ui.addBlocker(specific.gameObject);
        }
        public void popMsgAgentDeath(Unit actor, string words)
        {
            if (world.displayMessages == false) { return; }

            GameObject obj = Instantiate(prefabMsgAgentsDeath) as GameObject;
            PopupMsgAgentsDeath specific = obj.GetComponent<PopupMsgAgentsDeath>();
            specific.ui = ui;
            specific.setTo(actor);
            specific.text.text = words;
            specific.agentA = actor;
            specific.bDismiss.onClick.AddListener(delegate { specific.dismiss(); });
            specific.bDismissAgentA.onClick.AddListener(delegate { specific.dismissAgentA(); });
            ui.addBlocker(specific.gameObject);
        }
        public void popCredits()
        {
            GameObject obj = Instantiate(prefabCredits) as GameObject;
            PopupMsg specific = obj.GetComponent<PopupMsg>();
            specific.ui = ui;
            specific.bDismiss.onClick.AddListener(delegate { specific.dismiss(); });
            ui.addBlocker(specific.gameObject);
        }
        public void popMsgTreeBackground(string words)
        {
            //if (world.displayMessages == false) { return; }

            GameObject obj = Instantiate(prefabMsgTreeBackground) as GameObject;
            PopupMsg specific = obj.GetComponent<PopupMsg>();
            specific.ui = ui;
            specific.text.text = words;
            specific.bDismiss.onClick.AddListener(delegate { specific.dismiss(); });
            ui.addBlocker(specific.gameObject);
        }
        public void popPumpkinVictory(string words)
        {
            if (world.displayMessages == false) { return; }

            GameObject obj = Instantiate(prefabPumpkinVictory) as GameObject;
            PopupPumpkinVictory specific = obj.GetComponent<PopupPumpkinVictory>();
            specific.ui = ui;
            specific.text.text = words;
            specific.bDismiss.onClick.AddListener(delegate { specific.dismiss(); });
            ui.addBlocker(specific.gameObject);
        }
        public void popAutosave()
        {
            if (world.displayMessages == false) { return; }

            GameObject obj = Instantiate(prefabAutosave) as GameObject;
            PopupAutosaveDialog specific = obj.GetComponent<PopupAutosaveDialog>();
            specific.ui = ui;
            specific.text.text = "Saving game...";
            specific.bDismiss.onClick.AddListener(delegate { specific.dismiss(); });
            specific.bAutoDismiss.onClick.AddListener(delegate { specific.autoDismiss(); });
            ui.addBlocker(specific.gameObject);
        }
        public void popSaveName()
        {
            //if (world.displayMessages == false) { return; }

            GameObject obj = Instantiate(prefabSaveName) as GameObject;
            PopupSaveDialog specific = obj.GetComponent<PopupSaveDialog>();
            specific.ui = ui;
            specific.bSave.onClick.AddListener(delegate { specific.save(); });
            specific.bCancel.onClick.AddListener(delegate { specific.cancel(); });
            ui.addBlocker(specific.gameObject);
        }
        public void popImgMsg(string body, string flavour, int img = 0)
        {
            if (world.displayMessages == false) { return; }

            GameObject obj = Instantiate(prefabImgMsg) as GameObject;
            PopupImgMsg specific = obj.GetComponent<PopupImgMsg>();
            specific.ui = ui;
            specific.textBody.text = body;
            specific.textFlavour.text = flavour;
            if (img == 0)
            {
                int q = Eleven.random.Next(4);
                if (q == 0) { specific.img.sprite = ui.world.textureStore.boxImg_blue; }
                if (q == 1) { specific.img.sprite = ui.world.textureStore.boxImg_thumb; }
                if (q == 2) { specific.img.sprite = ui.world.textureStore.boxImg_ship; }
                if (q == 3) { specific.img.sprite = ui.world.textureStore.boxImg_moon; }
            }
            else if (img == 1)
            {
                specific.img.sprite = ui.world.textureStore.boxImg_coins;
            }
            else if (img == 2)
            {
                specific.img.sprite = ui.world.textureStore.boxImg_seekerBook;
            }
            else if (img == 3)
            {
                specific.img.sprite = ui.world.textureStore.boxImg_pumpkin;
            }
            else if (img == 4)
            {
                specific.img.sprite = ui.world.textureStore.boxImg_cult;
            }
            else if (img == 5)
            {
                specific.img.sprite = ui.world.textureStore.boxImg_redDeath;
            }
            else if (img == 6)
            {
                specific.img.sprite = ui.world.textureStore.boxImg_fog;
            }
            else if (img == 7)
            {
                specific.img.sprite = ui.world.textureStore.boxImg_saviour;
            }
            specific.bDismiss.onClick.AddListener(delegate { specific.dismiss(); });
            ui.addBlocker(specific.gameObject);
        }
        public void popVoteMsg(string title, string subtitle, string body)
        {
            if (world.displayMessages == false) { return; }

            GameObject obj = Instantiate(prefabVoteMsg) as GameObject;
            PopupVotingMsg specific = obj.GetComponent<PopupVotingMsg>();
            specific.ui = ui;
            specific.title.text = title;
            specific.subtitle.text = subtitle;
            specific.textBody.text = body;
            specific.bDismiss.onClick.AddListener(delegate { specific.dismiss(); });
            ui.addBlocker(specific.gameObject);
        }
        public void popLightbringerMsg(Society society)
        {
            //if (world.displayMessages == false) { return; }

            GameObject obj = Instantiate(prefabLightbringerDiag) as GameObject;
            PopupLightbringer specific = obj.GetComponent<PopupLightbringer>();
            specific.ui = ui;
            specific.populate(society);
            specific.bDismiss.onClick.AddListener(delegate { specific.dismiss(); });
            specific.bDismissGoto.onClick.AddListener(delegate { specific.dismissGoto(); });
            ui.addBlocker(specific.gameObject);
        }
        public void popEvent(EventData d, EventContext ctx)
        {
            GameObject obj = Instantiate(prefabEvent) as GameObject;
            PopupEvent specific = obj.GetComponent<PopupEvent>();
            specific.ui = ui;
            specific.populate(d, ctx);
            ui.addBlocker(specific.gameObject);
        }


        internal GraphicalUnit getGraphicalUnit(Map map, Unit p)
        {
            GameObject obj = Instantiate(graphicalUnit) as GameObject;
            GraphicalUnit property = obj.GetComponent<GraphicalUnit>();
            property.setTo(p, world);
            property.borderLayer1.enabled = true;
            property.borderLayer2.enabled = true;
            return property;
        }
        internal GraphicalProperty getGraphicalProperty(Map map, Property p)
        {
            GameObject obj = Instantiate(graphicalProperty) as GameObject;
            GraphicalProperty property = obj.GetComponent<GraphicalProperty>();
            property.setTo(p, world);
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
        public void popDefeatBox()
        {
            //if (world.displayMessages == false) { return; }

            GameObject obj = Instantiate(prefabDefeatBox) as GameObject;
            PopupVictory specific = obj.GetComponent<PopupVictory>();
            specific.bDismiss.onClick.AddListener(delegate { specific.dismiss(); });
            specific.ui = ui;
            ui.addBlocker(specific.gameObject);
        }
        public void popEndgameCyclic()
        {
            //if (world.displayMessages == false) { return; }

            GameObject obj = Instantiate(prefabEndGameCyclic) as GameObject;
            PopupEndgameCyclic specific = obj.GetComponent<PopupEndgameCyclic>();
            specific.bDismiss.onClick.AddListener(delegate { specific.dismiss(); });
            specific.bDismissNextAge.onClick.AddListener(delegate { specific.dismissToNextAge(); });
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
