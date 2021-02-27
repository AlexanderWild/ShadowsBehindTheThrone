using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine.UI;
using UnityEngine;
using FullSerializer;
using System.Threading;

namespace Assets.Code
{
    /*
     * World is your monobehaviour master. It calls the start function, triggering all loading and suchlikes and suchforths
     *
     * It has the ONLY reference to map. Map must be kept apart from any Unity gameObjects, so it can be serialised out
     * Obviously this is impossible, but every class which knows about its unity GameObject must be able to purge this reference
     *
     * It holds the references to the 'stores'. These are repositories which should not be serialised into the saved games
     */
    public class World : MonoBehaviour
    {
        public static bool logging   = false;

        public UIMaster ui;
        public TextureStore textureStore;
        public PrefabStore prefabStore;
        public AudioStore audioStore;
        public Camera outerCamera;
        public TextStore wordStore;
        public UIMusic musicPlayer;
        //public SoundObj soundSource;

        public Map map;
        public static Map staticMap;
        public bool displayMessages = false;
        public bool turnLock = false;
        public string pathPrefix = "";
        public static string separator = "";
        public bool isWindows = false;
        public static World self;
        public static bool automatic = false;

        public static int autosaveCount = 5;
        public static int autosavePeriod = 10;
        public static int autodismissAutosave = 0;

        public List<God> potentialGods = new List<God>();
        public List<God> chosenGods = new List<God>();
        public float lastFrame;

        public static bool advancedEdition = true;
        public static bool useHorseman = false;

        public static LogBox saveLog = new LogBox("saveLog.log");
        public static string saveFolderName = "ShadowsBehindTheThroneSavedGames";
        public static string saveHeader = "\nSAVEFILEDATAHEADER\n";
        public static int versionNumber = 17;
        public static int subversionNumber = 0;

        public static bool cheat_globalCooling = false;

        public static int musicVolume = 100;

        public static string saveFolder;

        public void Start()
        {
            //Screen.SetResolution(1920, 1080, true);

            if (Directory.Exists("advdata") == false) { advancedEdition = false; World.log("Setting to standard edition as not advanced data folder was detected"); }

            Log("User folder attempt: " + Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));

            specificStartup();
            ui.setToMainMenu();
            //Thread t = new Thread(musicPlayer.loadMusic);
            //t.Start();//Do not fuck me over here, Unity, I'm not feeling like spending a whole day debugging your shit
            musicPlayer.loadMusic(); //How did I know?

            potentialGods.Add(new God_Easy());
            potentialGods.Add(new God_Fog());
            potentialGods.Add(new God_MerchantOfNightmares());
            potentialGods.Add(new God_Flesh());
            potentialGods.Add(new God_WintersScythe());
            potentialGods.Add(new God_DeepOnes());
            potentialGods.Add(new God_Omni());

            AchievementManager.setup();
            musicPlayer.playTheme();
        }


        public void Update()
        {
            if (self == null) { self = this; }

            if (lastFrame == -1) {
                lastFrame = Time.realtimeSinceStartup;
                return;
            }
            else
            {
                float dT = Time.realtimeSinceStartup - lastFrame;
                float targetDT = 1 / 60;
                if (dT < targetDT)
                {
                    new WaitForSecondsRealtime(targetDT-dT);
                }
            }
        }

        public void specificStartup()
        {
            if (SystemInfo.operatingSystemFamily == OperatingSystemFamily.Windows)
            {
                Log("Windows! A windows OS");
                isWindows = true;
                pathPrefix = "";
                separator = "\\";
                string[] decomp = Application.dataPath.Split('/');
                for (int i = 0; i < decomp.Length - 1; i++)
                {
                    pathPrefix += decomp[i] + "/";
                }

                textureStore.world = this;
                textureStore.load();
                wordStore = new TextStore();
                wordStore.load();
            }
            else //if (SystemInfo.operatingSystemFamily == OperatingSystemFamily.Linux)
            {
                Log("The operating system is Linux based");
                pathPrefix = "";
                separator = "/";
                string[] decomp = Application.dataPath.Split('/');
                for (int i = 0; i < decomp.Length - 1; i++)
                {
                    pathPrefix += decomp[i] + "/";
                }
                textureStore.world = this;
                textureStore.loadLinux();
                wordStore = new TextStore();
                wordStore.loadLinux(this);
            }

            //wordStore = new TextStore();
            //wordStore.load();
            Application.targetFrameRate = 60;
            GraphicalMap.world = this;
            //Activity.load();

            GraphicalSociety.world = this;

            if (logging)
            {
                foreach (string f in Directory.GetFiles("logging" + separator + "people"))
                {
                    if (f.Contains(".log"))
                    {
                        File.Delete(f);
                    }
                }
                foreach (string f in Directory.GetFiles("logging" + separator + "societies"))
                {
                    if (f.Contains(".log"))
                    {
                        File.Delete(f);
                    }
                }
            }

            saveFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + World.separator + World.saveFolderName + World.separator;
        }
        public void startup(PopupGameOptions opts)
        {
            Log("Called startup");
            specificStartup();//Will overwrite a bunch of stuff, this is by design to keep seeds valid

            Params param = new Params();
            //param.loadFromFile();

            //Apply the choices the user made in the choice screen
            param.overmind_powerRegen *= opts.powerGainPercent / 100f;
            param.person_suspicionPerEvidence *= opts.susGainPercent / 100f;
            param.awareness_master_speed *= opts.awarenessGainSpeed / 100f;
            param.unit_investigatorsPerProvince *= opts.investigatorCount / 100f;
            param.mapGen_sizeX = opts.sizeX;
            param.mapGen_sizeY = opts.sizeY;
            param.mapGen_burnInSteps = opts.burnIn;
            param.unit_investigatorsSeeEnthralled = opts.investigatorsSee ? 1 : 0;
            param.useLightbringer = opts.allowLightbringer ? 1 : 0;
            param.useAwareness = opts.useAwareness ? 1 : 0;
            param.usePaladins = opts.usePaladins ? 1 : 0;
            param.overmind_maxEnthralled = opts.nAgents;
            param.overmind_allowDirectEnthralling = opts.politicalStart ? 1:0;
            param.unit_armyHPMult = opts.armyHPMult/100d;

            World.log("Opts use awareness " + opts.useAwareness);

            map = new Map(param);
            GraphicalMap.map = map;
            GraphicalMap.world = this;

            map.seed = opts.currentSeed;
            map.automatic = automatic;

            if (opts.useSimplified)
            {
                map.simplified = true;
                map.param.overmind_allowDirectEnthralling = 0;
                map.agentsOnly = true;
                if (opts.difficultySetting > 0)
                {
                    map.param.usePaladins = 1;
                }
            }
            else
            {
                map.agentsOnly = false;
            }
            if (param.overmind_allowDirectEnthralling == 0) { param.flashEnthrallables = false; }

            Property_Prototype.loadProperties(map);
            EconTrait.loadTraits(map);
            staticMap = map;
            map.world = this;
            map.globalist.buildBasicElements();
            Eleven.random = new System.Random(opts.currentSeed);
            map.gen();
            if (advancedEdition)
            {
                map.param.option_useAdvancedGraphics = 1;
            }
            PopupIOOptions.load(map);
            if (!advancedEdition)//Force it down, even if the IO options deceive you, if you are not actually the advanced edition
            {
                map.param.option_useAdvancedGraphics = 0;
            }



            //ui.setToWorld();
            displayMessages = !automatic;
            Log("Got to end of initial startup");
            ui.checkData();

            ui.setToVoting();
            ui.setToWorld();
            //bQuicksave();

            musicPlayer.stopTheme();

            if (map.simplified)
            {
                printSimplifiedMessage();
            }
            else if (map.automatic)
            {
                map.overmind.autoAI.popAIModeMessage();
            }
        }

        private void printSimplifiedMessage()
        {
            String msg = "Welcome to Shadows Behind the Throne";
            msg += "\n\nThe game revolves around stealth and covert action. The nations of humankind are powerful, and huge, but slow to react and often self-destructive, due to the political desires of their nobles.";
            msg += "\n\nTo get started, use your power to create agents (or select a human one and take them over), then begin your work against your chosen target. " +
            "Infiltrating a few locations is a good first step (the vampire or merchant work well here), even if the agent you used dies, the infiltration level will benefit your other agents.";
            msg += "\n\nIf you wish to use direct attacks on the nations, try to cause civil wars before you begin, for example by enshadowing some nobles, or causing madness, they will turn on each other, giving you an opening.";
            prefabStore.popMsgTreeBackground(msg);
        }

        public void bUseHorsemanToggle()
        {
            useHorseman = !useHorseman;
        }
        /*
        public void startup()
        {
            Log("Called startup");
            Params param = new Params();
            param.loadFromFile();
            map = new Map(param);
            GraphicalMap.map = map;
            GraphicalMap.world = this;

            Property_Prototype.loadProperties(map);
            EconTrait.loadTraits(map);
            staticMap = map;
            map.world = this;
            map.globalist.buildBasicElements();
            map.gen();


            //ui.setToWorld();
            displayMessages = true;
            Log("Got to end of initial startup");
            ui.checkData();

            //bQuicksave();
        }
        */

        public void bStartGameAutomatic()
        {
            World.automatic = true;
            audioStore.playClick();
            ui.addBlocker(ui.world.prefabStore.getScrollSetGods(ui.world.potentialGods).gameObject);
        }
        public void bStartGameOptions()
        {
            audioStore.playClick();
            ui.addBlocker(ui.world.prefabStore.getScrollSetGods(ui.world.potentialGods).gameObject);
        }
        public void bStartGameOptionsStreamlined()
        {
            audioStore.playClick();
            ui.world.prefabStore.getGameOptionsPopupSimplified();
        }

        public void bFlashEnthrallables()
        {
            audioStore.playClick();

            map.param.flashEnthrallables = !map.param.flashEnthrallables;
            GraphicalMap.checkData();
        }

        public void bQuit()
        {
            if (SteamManager.s_EverInitialized)
            {
                SteamManager.shutdownSteamAPI();
            }

            Application.Quit();
        }
        public void bViewPlayback()
        {
            audioStore.playClick();
            ui.addBlocker(prefabStore.getPlayback(this, map).gameObject);
        }
        public void bStartGame(int seed, PopupGameOptions opts)
        {
            audioStore.playActivate();

            Eleven.random = new System.Random(seed);
            startup(opts);
            for (int i = 0; i < map.param.mapGen_burnInSteps; i++)
            {
                map.turnTick();
            }
            map.overmind.namesChosen.AddRange(chosenGods);
            foreach (God chosenGod in chosenGods){
                chosenGod.onStart(map);
            }
            map.burnInComplete = true;
            map.overmind.addDefaultAbilities();
            chosenGods.Clear();//Just in case this fucks with something
            ui.setToWorld();
            map.overmind.startedComplete();
        }

        public void bContinue()
        {
            audioStore.playClick();
            ui.setToWorld();
        }

        /*
         *Not used by button. Used by cheats and suchlike if need be
        */
        public void bEndTurn()
        {
            if (turnLock) { return; }
            if (ui.blocker != null) { return; }

            audioStore.playClick();

            turnLock = true;
            if (map != null) {
                map.turnTick();
            }

            turnLock = false;
            ui.checkData();
        }
        public void b10Turns()
        {
            if (turnLock) { return; }
            if (ui.blocker != null) { return; }

            turnLock = true;
            if (map != null)
            {
                for (int i = 0; i < 10; i++)
                {
                    map.turnTick();
                }
            }

            turnLock = false;
            ui.checkData();
        }

        public void b100Turns()
        {
            if (turnLock) { return; }
            if (ui.blocker != null) { return; }

            turnLock = true;
            if (map != null)
            {
                for (int i = 0; i < 100; i++)
                {
                    map.turnTick();
                }
            }

            turnLock = false;
            ui.checkData();
        }
        public static void Log(string str)
        {
            Debug.Log(str);
        }
        public static void log(string str)
        {
            if (staticMap != null)
            {
                Debug.Log("Turn: " + staticMap.turn + ": " + str);
            }
            else
            {
                Debug.Log(str);
            }
        }


        public void testXScroll()
        {

            List<ThreatItem> threats = new List<ThreatItem>();
            for (int i = 0; i < 5; i++)
            {
                Person p = new Person((Society)map.socialGroups[0]);
                ThreatItem t = new ThreatItem(map, p);
                threats.Add(t);
            }
            ui.addBlocker(prefabStore.getScrollSetThreats(threats).gameObject);
        }

        public void testYScroll()
        {

            List<ThreatItem> threats = new List<ThreatItem>();
            for (int i = 0; i < 5; i++)
            {
                Person p = new Person((Society)map.socialGroups[0]);
                ThreatItem t = new ThreatItem(map, p);
                threats.Add(t);
            }
            ui.addBlocker(prefabStore.getScrollSetThreats(threats).gameObject);
        }

        public static bool hasWritePermission(string folderPath)
        {
            try
            {

                //Because Unity isn't re-implementing the correct libraries, we're doing it this way. Apologies to the future, we make war with the army with have not the one we want
                File.WriteAllLines("trialWrite.tmp", new string[] { "trialForPermissionCheck" });
                File.Delete("trialWrite.tmp");
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool checkSaveFolder() {
            if (Directory.Exists(saveFolder) == false) {
                try
                {
                    Directory.CreateDirectory(saveFolder);
                }catch(Exception e)
                {
                    World.autosavePeriod = -1;
                    return false;
                }
            }

            return true;
        }

        public void save(string filename,bool popMsg=true)
        {
            Map rescueMap = World.staticMap;

            if (checkSaveFolder() == false) {
                prefabStore.popMsg("Unable to locate directory " + saveFolder + ". Saving cannot proceed without folder access. Aborting save.");
                return;
            }
            if (!hasWritePermission(saveFolder))
            {
                prefabStore.popMsg("Unable to write to directory " + saveFolder + ". Saving cannot proceed without folder access. Aborting save.");
                World.autosavePeriod = -1;
                return;
            }

            try
            {


                World world = this;
                // world.ui.setToMainMenu();
                GraphicalMap.purge();
                GraphicalSociety.purge();
                map.compressForSave();
                world.map.world = null;


                //foreach (SocialGroup sg in map.socialGroups)
                //{
                //    if (sg is Society)
                //    {
                //        Society soc = (Society)sg;
                //        soc.voteSession = null;
                //    }
                //}

                fsSerializer _serializer = new fsSerializer();
                fsData data;
                _serializer.TrySerialize(typeof(Map), map, out data).AssertSuccessWithoutWarnings();

                // emit the data via JSON
                string saveString = fsJsonPrinter.CompressedJson(data);
                World.Log("Save data exit point");

                string catSaveString = "Version;" + World.versionNumber + ";" + World.subversionNumber;
                catSaveString += saveHeader;
                catSaveString += saveString;

                if (File.Exists(filename))
                {
                    World.Log("Overwriting old save: " + filename);
                    File.Delete(filename);
                }
                File.WriteAllLines(filename, new string[] { catSaveString });//Do it all on one line, to avoid faff wrt line endings

                world.map.world = world;
                staticMap = map;
                map.decompressFromSave();

                if (popMsg)
                {
                    world.prefabStore.popMsg("Game saved as: " + filename);
                }

                //// step 1: parse the JSON data
                //fsData data = fsJsonParser.Parse(serializedState);

                //// step 2: deserialize the data
                //object deserialized = null;
                //_serializer.TryDeserialize(data, type, ref deserialized).AssertSuccessWithoutWarnings();
            }
            catch(Exception e)
            {
                World.log(e.Message);
                World.log(e.StackTrace);
                prefabStore.popMsg("Failure to save");
                prefabStore.popMsg("Exception: " + e.StackTrace);

                map = rescueMap;
                map.world = this;
                staticMap = map;
            }
        }

        public void bQuicksave()
        {
            audioStore.playClick();
            // save("quicksave.sv");

            prefabStore.popSaveName();
        }
        public void bQuickload()
        {
            audioStore.playClick();

            World.Log("load clicked");
            load("quicksave.sv");
        }
        public void bLoad()
        {
            audioStore.playClick();

            World.Log("load clicked");
            prefabStore.popScrollSetSaves();
        }

        public void bDumpData()
        {
            World.log("nSocieties " + map.socialGroups.Count);
            World.log("Map size: " + map.grid.Length + " / " + map.grid[0].Length);

        }
        public void load(string filename)
        {
            try
            {
                if (map != null)
                {
                    GraphicalMap.purge();
                    GraphicalSociety.purge();
                    map.world = null;
                    map = null;
                }
                filename = saveFolder + filename;
                string fullFile = File.ReadAllText(filename);
                //string serializedState = fullFile.Substring(fullFile.IndexOf("\n"), fullFile.Length);
                int startIndex = fullFile.IndexOf(saveHeader) + saveHeader.Length;
                int endIndex = fullFile.Length - startIndex;
                string serializedState = fullFile.Substring(startIndex,endIndex);
                fsSerializer _serializer = new fsSerializer();
                fsData data = fsJsonParser.Parse(serializedState);
                World.Log("Data parsed");
                object deserialized = null;
                _serializer.TryDeserialize(data, typeof(Map), ref deserialized).AssertSuccessWithoutWarnings();
                World.saveLog.takeLine("Finished deserial");
                map = (Map)deserialized;
                map.world = this;
                staticMap = map;
                World.self.displayMessages = true;
                GraphicalMap.map = map;
                //ui.setToMainMenu();
                //GraphicalMap.checkLoaded();
                //GraphicalMap.checkData();
                //graphicalMap.loadArea(0, 0);
                map.decompressFromSave();
                prefabStore.popMsg("Loaded file: " + filename);
                World.Log("reached end of loading code");
                // prefabStore.popMsg("Load may well have succeeded.");
            }
            catch (FileLoadException e)
            {
                Debug.Log(e);
                World.log(e.StackTrace);
                prefabStore.popMsg("Exception: " + e.StackTrace);
            }
            catch (Exception e2)
            {
                Debug.Log(e2);
                World.log(e2.StackTrace);
                prefabStore.popMsg("Exception: " + e2.StackTrace);
            }
        }
    }

}
