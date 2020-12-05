using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.IO;


namespace Assets.Code
{
    public class TextureStore : MonoBehaviour
    {
        public World world;

        public Sprite person_lightbringer;
        public Sprite person_dark;
        public Sprite person_halfDark;
        public Sprite person_basic;

        public Sprite person_back_vampire;
        public Sprite person_back_eyes;
        public Sprite person_back_shadow;
        public Sprite person_pumpkin;
        public Sprite person_advEnthralled;
        public Sprite person_advClear;
        public Sprite person_advBroken;

        public Sprite slotKing;
        public Sprite slotDuke;
        public Sprite slotCount;
        public Sprite slotBasic;

        public Sprite boxImg_coins;
        public Sprite boxImg_ship;
        public Sprite boxImg_thumb;
        public Sprite boxImg_blue;
        public Sprite boxImg_seekerBook;
        public Sprite boxImg_redDeath;
        public Sprite boxImg_pumpkin;
        public Sprite boxImg_cult;
        public Sprite boxImg_moon;

        public Sprite hex_base;
        public Sprite hex_simplified;
        public Sprite hex_terrain_sea;
        public Sprite hex_terrain_grass;
        public Sprite hex_terrain_path;
        public Sprite hex_terrain_forest;
        public Sprite hex_terrain_snowforest;
        public Sprite hex_terrain_swamp;
        public Sprite hex_terrain_damp;
        public Sprite hex_terrain_dry;
        public Sprite hex_terrain_desert;
        public Sprite hex_terrain_tundra;
        public Sprite hex_terrain_snow;
        public Sprite hex_terrain_mountain;

        public Sprite hex_special_flesh;
        public Sprite hex_special_flesh2;

        public Sprite loc_empty;
        public Sprite loc_green;
        public Sprite loc_city_roman;
        public Sprite loc_city_light;
        public Sprite loc_city_metropole;
        public Sprite loc_minor_green;
        public Sprite loc_minor_emptyGrass;
        public Sprite loc_minor_emptySnow;
        public Sprite loc_minor_emptyDesert;
        public Sprite loc_minor_emptyOcean;
        public Sprite loc_minor_ritualCircle;
        public Sprite loc_minor_fort;
        public Sprite loc_minor_university;
        public Sprite loc_minor_church;
        public Sprite loc_minor_ruins;
        public Sprite loc_minor_farm;
        public Sprite loc_town;
        public Sprite loc_worm_nest;
        public Sprite loc_flesh;
        public Sprite loc_corpseroot;

        public Sprite icon_enshadow;
        public Sprite icon_convert;
        public Sprite icon_mask;
        public Sprite icon_ghoul;
        public Sprite icon_axes;
        public Sprite icon_shield;
        public Sprite icon_fishman;
        public Sprite icon_eyes;
        public Sprite icon_moon;
        public Sprite icon_fog;
        public Sprite icon_vampire;
        public Sprite icon_coins;
        public Sprite icon_doctor;
        public Sprite icon_seeker;
        public Sprite icon_corpseroot;
        public Sprite icon_pumpkin;
        public Sprite icon_heirophant;

        public Sprite icon_combat;

        public Sprite questionMark;

        public Sprite emptyDukeSlot;

        public Sprite flora_forest;
        public Sprite flora_forestSnow;
        public Sprite flora_flesh;
        public Sprite flora_corpseroot;

        public Sprite unit_default;
        public Sprite unit_lookingGlass;
        public Sprite unit_ghoul;
        public Sprite unit_fishmen;
        public Sprite unit_deadFishmen;
        public Sprite unit_army;
        public Sprite unit_battleIcon;
        public Sprite unit_supply;
        public Sprite unit_enthralled;
        public Sprite unit_trader;
        public Sprite unit_doctor;
        public Sprite unit_letter;
        public Sprite unit_letterAlert;
        public Sprite unit_nomadicTribe;
        public Sprite unit_seeker;
        public Sprite unit_worm;
        public Sprite unit_ghast;
        public Sprite unit_refugees;
        public Sprite unit_cyclops;
        public Sprite unit_vampire;
        public Sprite unit_fleshmaw;
        public Sprite unit_armyOfDead;
        public Sprite unit_paladin;
        public Sprite unit_pumpkin;
        public Sprite unit_heirophant;

        public Sprite property_noDead;
        public Sprite property_forgottenSecret;
        public Sprite property_securityMinor;
        public Sprite property_securityMajor;
        public Sprite property_securityLockdown;
        public Sprite property_pumpkin;

        public List<Sprite> tutorialImages;

        public Sprite cloud_plague;

        public Sprite painting_nightMoon;
        public Sprite painting_monasteryRuins;
        public Sprite painting_deathAndGravedigger;
        public Sprite painting_ettyStorm;
        public Sprite painting_deathAndConflagaration;
        public Sprite painting_monkAndOldWoman;

        public Sprite special_horsemanLayer0;
        public Sprite special_horsemanLayer1;
        public Sprite special_horsemanLayer2;

        public Dictionary<string, Sprite> sprites;
        public Dictionary<string, Sprite> images;
        public List<Sprite> layerFore = new List<Sprite>();
        public List<Sprite> layerBack = new List<Sprite>();
        public List<GraphicalCulture> cultures = new List<GraphicalCulture>();

        public void load()
        {
            sprites = new Dictionary<string, Sprite>();
            images = new Dictionary<string, Sprite>();

            string[] dirPaths = Directory.GetDirectories(".\\data");
            foreach (string d in dirPaths)
            {
                string[] subDirectories = Directory.GetDirectories(d);
                List<string> allPaths = new List<string>();
                foreach (string subD in subDirectories)
                {
                    string[] filePathsInner = Directory.GetFiles(subD);
                    foreach (string s in filePathsInner)
                    {
                        if (s.EndsWith(".png"))
                        {
                            allPaths.Add(s);
                        }
                    }
                }

                string[] filePaths = Directory.GetFiles(d);
                foreach (string s in filePaths)
                {
                    if (s.EndsWith(".png"))
                    {
                        allPaths.Add(s);
                    }
                }

                foreach (string s in allPaths)
                {
                    if (s.EndsWith(".png"))
                    {
                        string path = s.Replace('\\', '-');
                        path = path.Substring(2);
                        //World.log(path);
                        string[] codeArray = path.Split('-');
                        string code = codeArray[codeArray.Length - 2] + "-" + codeArray[codeArray.Length - 1];
                        //World.log("Code: " + code);

                        if (sprites.ContainsKey(path)) { continue; }

                        Sprite test = LoadPNG(s);
                        sprites.Add(code, test);
                        if (path.Contains("profileLayerBack"))
                        {
                            layerBack.Add(test);
                        }
                        else if (path.Contains("profileLayerFore"))
                        {
                            layerFore.Add(test);
                        }
                        else if (path.Contains("boxImages"))
                        {
                            string[] subs = path.Split('-');
                            string subpath = subs[subs.Length - 1].Substring(0, subs[subs.Length - 1].Length - 4);
                            //World.log("Image saved as " + subpath);
                            images.Add(subpath, test);
                        }
                    }
                }
            }

            if (World.advancedEdition)
            {
                foreach (string s in Directory.GetDirectories(".\\advdata%cultures".Replace("%", World.separator)))
                {
                    if (s.Contains("CultureDef") && Directory.Exists(s))
                    {
                        World.log("DIRECTORY " + s);
                        GraphicalCulture cult = new GraphicalCulture();
                        foreach (string fileName in Directory.GetFiles(s + World.separator + "f_eyes"))
                        {

                            if (fileName.EndsWith(".png"))
                            {
                                Sprite loadedImg = LoadPNG(fileName);
                                cult.f_eyes.Add(loadedImg);
                            }
                        }
                        foreach (string fileName in Directory.GetFiles(s + World.separator + "f_mouths"))
                        {

                            if (fileName.EndsWith(".png"))
                            {
                                Sprite loadedImg = LoadPNG(fileName);
                                cult.f_mouths.Add(loadedImg);
                            }
                        }
                        foreach (string fileName in Directory.GetFiles(s + World.separator + "f_faces"))
                        {

                            if (fileName.EndsWith(".png"))
                            {
                                Sprite loadedImg = LoadPNG(fileName);
                                cult.f_faces.Add(loadedImg);
                            }
                        }
                        cult.f_jewels.Add(person_advClear);
                        foreach (string fileName in Directory.GetFiles(s + World.separator + "f_jewels"))
                        {

                            if (fileName.EndsWith(".png"))
                            {
                                Sprite loadedImg = LoadPNG(fileName);
                                cult.f_jewels.Add(loadedImg);
                            }
                        }
                        foreach (string fileName in Directory.GetFiles(s + World.separator + "f_hair"))
                        {

                            if (fileName.EndsWith(".png"))
                            {
                                Sprite loadedImg = LoadPNG(fileName);
                                cult.f_hair.Add(loadedImg);
                            }
                        }
                        foreach (string fileName in Directory.GetFiles(s + World.separator + "m_eyes"))
                        {

                            if (fileName.EndsWith(".png"))
                            {
                                Sprite loadedImg = LoadPNG(fileName);
                                cult.m_eyes.Add(loadedImg);
                            }
                        }
                        foreach (string fileName in Directory.GetFiles(s + World.separator + "m_mouths"))
                        {

                            if (fileName.EndsWith(".png"))
                            {
                                Sprite loadedImg = LoadPNG(fileName);
                                cult.m_mouths.Add(loadedImg);
                            }
                        }
                        foreach (string fileName in Directory.GetFiles(s + World.separator + "m_faces"))
                        {

                            if (fileName.EndsWith(".png"))
                            {
                                Sprite loadedImg = LoadPNG(fileName);
                                cult.m_faces.Add(loadedImg);
                            }
                        }
                        cult.m_jewels.Add(person_advClear);
                        foreach (string fileName in Directory.GetFiles(s + World.separator + "m_jewels"))
                        {

                            if (fileName.EndsWith(".png"))
                            {
                                Sprite loadedImg = LoadPNG(fileName);
                                cult.m_jewels.Add(loadedImg);
                            }
                        }
                        foreach (string fileName in Directory.GetFiles(s + World.separator + "m_hair"))
                        {

                            if (fileName.EndsWith(".png"))
                            {
                                Sprite loadedImg = LoadPNG(fileName);
                                cult.m_hair.Add(loadedImg);
                            }
                        }
                        World.log("Loaded culture. nFEyes: " + cult.f_eyes.Count);
                        cultures.Add(cult);
                    }
                }
            }
        }

        public void loadLinux()
        {
            World.log("Logging for Linux texture loading");
            sprites = new Dictionary<string, Sprite>();
            images = new Dictionary<string, Sprite>();

            string[] dirPaths = Directory.GetDirectories(world.pathPrefix + "/data");
            //string[] dirPaths = Directory.GetDirectories(".\\data");
            foreach (string d in dirPaths)
            {
                //World.log("Directory path found: " + d);
                string[] subDirectories = Directory.GetDirectories(d);
                List<string> allPaths = new List<string>();
                foreach (string subD in subDirectories)
                {
                    //World.log("Sub Directory path found: " + subD);
                    string[] filePathsInner = Directory.GetFiles(subD);
                    foreach (string s in filePathsInner)
                    {
                        World.log("File: " + subD);
                        if (s.EndsWith(".png"))
                        {
                            allPaths.Add(s);
                        }
                    }
                }

                string[] filePaths = Directory.GetFiles(d);
                foreach (string s in filePaths)
                {
                    if (s.EndsWith(".png"))
                    {
                        allPaths.Add(s);
                    }
                }

                foreach (string s in allPaths)
                {
                    if (s.EndsWith(".png"))
                    {
                        string path = s.Replace('\\', '-');
                        path = s.Replace('/', '-');
                        path = path.Substring(2);
                        //World.log(path);
                        string[] codeArray = path.Split('-');
                        string code = codeArray[codeArray.Length - 2] + "-" + codeArray[codeArray.Length - 1];
                        //World.log("Code: " + code);

                        if (sprites.ContainsKey(path)) { continue; }

                        Sprite test = LoadPNG(s);
                        sprites.Add(code, test);
                        //World.log("Image saved as " + path);
                        if (path.Contains("profileLayerBack"))
                        {
                            layerBack.Add(test);
                        }
                        else if (path.Contains("profileLayerFore"))
                        {
                            layerFore.Add(test);
                        }
                        else if (path.Contains("boxImages"))
                        {
                            string[] subs = path.Split('-');
                            string subpath = subs[subs.Length - 1].Substring(0, subs[subs.Length - 1].Length - 4);
                            //World.log("Box Image saved as " + subpath);
                            images.Add(subpath, test);
                        }
                    }
                }
            }
        }
        public static Sprite LoadPNG(string filePath)
        {

            Texture2D tex = null;
            byte[] fileData;

            Sprite response = null;
            if (File.Exists(filePath))
            {
                fileData = File.ReadAllBytes(filePath);
                tex = new Texture2D(2, 2);
                tex.LoadImage(fileData);
                response = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
            }
            else
            {
                World.log("Unable to find: " + filePath);
            }
            return response;
        }
    }
}
