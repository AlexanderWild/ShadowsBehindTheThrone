using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine.UI;
using UnityEngine;
using FullSerializer;

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
        public static bool developer = false;

        public UIMaster ui;
        public TextureStore textureStore;
        public PrefabStore prefabStore;
        public Camera outerCamera;
        public TextStore wordStore;
        //public SoundObj soundSource;

        public Map map;
        public static Map staticMap;
        public bool displayMessages = false;
        public bool turnLock = false;
        public string pathPrefix = "";
        public static string separator = "";
        public bool isWindows = false;

        public List<God> potentialGods = new List<God>();
        public List<God> chosenGods = new List<God>();
        public float lastFrame;

        public static LogBox saveLog = new LogBox("saveLog.log");

        public void Start()
        {
            Screen.SetResolution(1920, 1080, true);


            specificStartup();
            if (developer)
            {
                Eleven.random = new System.Random(2);
                startup();
                ui.setToWorld();
            }
            else
            {
                ui.setToMainMenu();
            }

            potentialGods.Add(new God_LordOfBroken());
            potentialGods.Add(new God_Easy());
            potentialGods.Add(new God_DeepOnes());
            potentialGods.Add(new God_WintersScythe());
            potentialGods.Add(new God_Flesh());
            potentialGods.Add(new God_Omni());
        }


        public void Update()
        {
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
                separator = "/";
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
        }
        public void startup(PopupGameOptions opts)
        {
            Log("Called startup");
            Params param = new Params();
            param.loadFromFile();

            //Apply the choices the user made in the choice screen
            param.overmind_powerRegen *= opts.powerGainPercent / 100f;
            param.person_suspicionPerEvidence *= opts.susGainPercent / 100f;
            param.mapGen_sizeX = opts.sizeX;
            param.mapGen_sizeY = opts.sizeY;
            param.mapGen_burnInSteps = opts.burnIn;

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

        public void bStartGameOptions()
        {
            ui.addBlocker(ui.world.prefabStore.getScrollSetGods(ui.world.potentialGods).gameObject);
        }

        public void bFlashEnthrallables()
        {
            map.param.flashEnthrallables = !map.param.flashEnthrallables;
            GraphicalMap.checkData();
        }

        public void bQuit()
        {
            Application.Quit();
        }
        public void bViewPlayback()
        {
            ui.addBlocker(prefabStore.getPlayback(this, map).gameObject);
        }
        public void bStartGameSeeded(int seed, PopupGameOptions opts)
        {
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
            chosenGods = null;//Just in case this fucks with something
            ui.setToWorld();
        }

        public void bContinue()
        {
            ui.setToWorld();
        }

        public void bEndTurn()
        {
            if (turnLock) { return; }
            if (ui.blocker != null) { return; }

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

        public void save(string filename)
        {
            try
            {
                World world = this;
                // world.ui.setToMainMenu();
                GraphicalMap.purge();
                GraphicalSociety.purge();
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
                World.Log("Save exit point");

                if (File.Exists(filename))
                {
                    World.Log("Overwriting old save: " + filename);
                    File.Delete(filename);
                }
                File.WriteAllLines(filename, new string[] { saveString });

                world.map.world = world;
                staticMap = map;

                world.prefabStore.popMsg("Game saved as: " + filename);

                //// step 1: parse the JSON data
                //fsData data = fsJsonParser.Parse(serializedState);

                //// step 2: deserialize the data
                //object deserialized = null;
                //_serializer.TryDeserialize(data, type, ref deserialized).AssertSuccessWithoutWarnings();
            }catch(Exception e)
            {
                World.log(e.Message);
                World.log(e.StackTrace);
                prefabStore.popMsg("Failure to save");
                prefabStore.popMsg("Exception: " + e.StackTrace);
            }
        }

        public void bQuicksave()
        {
            save("quicksave.sv");
        }
        public void bQuickload()
        {
            World.Log("load clicked");
            load("quicksave.sv");
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

                string serializedState = File.ReadAllText(filename);
                fsSerializer _serializer = new fsSerializer();
                fsData data = fsJsonParser.Parse(serializedState);
                World.Log("Data parsed");
                object deserialized = null;
                _serializer.TryDeserialize(data, typeof(Map), ref deserialized).AssertSuccessWithoutWarnings();
                World.saveLog.takeLine("Finished deserial");
                map = (Map)deserialized;
                map.world = this;
                staticMap = map;
                GraphicalMap.map = map;
                //ui.setToMainMenu();
                //GraphicalMap.checkLoaded();
                //GraphicalMap.checkData();
                //graphicalMap.loadArea(0, 0);
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
