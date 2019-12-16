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


        public Sprite slotKing;
        public Sprite slotDuke;
        public Sprite slotCount;
        public Sprite slotBasic;

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

        public Sprite loc_empty;
        public Sprite loc_green;
        public Sprite loc_city_roman;
        public Sprite loc_city_light;
        public Sprite loc_minor_green;
        public Sprite loc_minor_emptyGrass;
        public Sprite loc_minor_emptySnow;
        public Sprite loc_minor_emptyDesert;
        public Sprite loc_minor_emptyOcean;
        public Sprite loc_minor_ritualCircle;
        public Sprite loc_minor_fort;
        public Sprite loc_minor_church;
        public Sprite loc_minor_ruins;
        public Sprite loc_minor_farm;
        public Sprite loc_worm_nest;
        public Sprite loc_flesh;

        public Sprite icon_enshadow;
        public Sprite icon_convert;
        public Sprite icon_mask;
        public Sprite icon_ghoul;
        public Sprite icon_axes;
        public Sprite icon_shield;
        public Sprite icon_fishman;
        public Sprite icon_eyes;

        public Sprite icon_combat;

        public Sprite questionMark;

        public Sprite flora_forest;
        public Sprite flora_forestSnow;
        public Sprite flora_flesh;

        public Sprite unit_default;
        public Sprite unit_lookingGlass;
        public Sprite unit_ghoul;
        public Sprite unit_fishmen;
        public Sprite unit_deadFishmen;
        public Sprite unit_army;
        public Sprite unit_supply;
        public Sprite unit_enthralled;
        public Sprite unit_trader;
        public Sprite unit_doctor;
        public Sprite unit_letter;
        public Sprite unit_letterAlert;
        public Sprite unit_nomadicTribe;
        public Sprite unit_witchHunter;
        public Sprite unit_worm;
        public Sprite unit_ghast;
        public Sprite unit_refugees;
        public Sprite unit_cyclops;
        public Sprite unit_fleshmaw;

        public Sprite tutorial1;
        public Sprite tutorial2;
        public Sprite tutorial3;
        public Sprite tutorial4;
        public Sprite tutorial5;

        public Sprite cloud_plague;

        public Dictionary<string, Sprite> sprites;
        public Dictionary<string, Sprite> images;
        public List<Sprite> layerFore = new List<Sprite>();
        public List<Sprite> layerBack = new List<Sprite>();

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
            return response;
        }
    }
}
