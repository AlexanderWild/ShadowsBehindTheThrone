using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.IO;

namespace Assets.Code
{
    public class TextStore
    {
        public static SerialMap<string, List<string>> flavourLists = new SerialMap<string, List<string>>();
        //public static string[] firstNamesM;
        //public static string[] firstNamesF;
        //public static string[] lastNames;

        public static MarkovModel model;
        public static MarkovModel amerimodel;
        public static MarkovModel frenchModel;
        public static HashSet<string> usedCityNames = new HashSet<string>();
        public static HashSet<string> usedLocNames = new HashSet<string>();
        
        public static HashSet<string> verboten = new HashSet<string>();

        public static void buildVerboten()
        {
            verboten.Add("dong");
            verboten.Add("wang");
            verboten.Add("mong");
            verboten.Add("dick");
            verboten.Add("fuck");
            verboten.Add("cunt");
            verboten.Add("shit");
            verboten.Add("crap");

            MarkovModel capper = new MarkovModel();
            HashSet<string> lowerV = new HashSet<string>();
            foreach (string s in verboten)
            {
                lowerV.Add(s);
            }
            foreach (string s in lowerV)
            {
                verboten.Add(capper.capFirst(s));
            }
        }

        public static string getFlavour(string key)
        {
            if (flavourLists.ContainsKey(key))
            {
                List<string> opts = flavourLists.get(key);
                int q = Eleven.random.Next(opts.Count);
                return opts[q];
            }
            else
            {
                return key;
            }
        }

        public static string getName(bool male)
        {
            for (int i = 0; i < 100; i++)
            {
                string word = amerimodel.capFirst(amerimodel.getWord());
                if (verboten.Contains(word)) { continue; }
                return word;
            }
            return "Alhazred";
        }

        public static string getCityName()
        {
            for (int t = 0; t < 100; t++)
            {
                string reply = model.capFirst(model.getWord());
                if (verboten.Contains(reply))
                {
                    //World.Log("Caught verboten word: " + reply);
                    continue;
                }
                if (usedCityNames.Contains(reply))
                {
                    continue;
                }
                usedCityNames.Add(reply);
                return reply;
            }
            return model.capFirst(model.getWord());
        }

        public static string getLocName()
        {
            for (int t = 0; t < 100; t++)
            {
                string reply = model.capFirst(frenchModel.getWord());
                if (verboten.Contains(reply))
                {
                    //World.Log("Caught verboten word: " + reply);
                    continue;
                }
                if (usedLocNames.Contains(reply))
                {
                    continue;
                }
                usedLocNames.Add(reply);
                return reply;
            }
            return model.capFirst(model.getWord());
        }
        public void loadLinux(World world)
        {

            loadFlavourLinux(world);
            string[] markovFood = System.IO.File.ReadAllLines(world.pathPrefix + "/data/words/chineseCities.txt");
            string[] markovFood2 = System.IO.File.ReadAllLines(world.pathPrefix + "/data/words/frenchCities.txt");
            string[] markovFood3 = System.IO.File.ReadAllLines(world.pathPrefix + "/data/words/french.txt");

            buildVerboten();
            model = new MarkovModel();
            model.buildModel(markovFood);
            amerimodel = new MarkovModel();
            amerimodel.buildModel(markovFood2);
            frenchModel = new MarkovModel();
            frenchModel.buildModel(markovFood3);

            string key;
            string value;
            key = "VICTORY";
            value = "Another day ends, another sun sets.";
            key = "VICTORY";
            value = "\"The heavens are mourning the death of the sun.\"";
            put(key, value);
            key = "DEFEAT";
            value = "The sun shines brighter, the night seems shorter.";
            put(key, value);
        }

        public void load()
        {
            loadFlavour();
            string[] markovFood = System.IO.File.ReadAllLines(".\\data\\words\\chineseCities.txt");
            string[] markovFood2 = System.IO.File.ReadAllLines(".\\data\\words\\frenchCities.txt");
            string[] markovFood3 = System.IO.File.ReadAllLines(".\\data\\words\\french.txt");

            buildVerboten();
            model = new MarkovModel();
            model.buildModel(markovFood);
            amerimodel = new MarkovModel();
            amerimodel.buildModel(markovFood2);
            frenchModel = new MarkovModel();
            frenchModel.buildModel(markovFood3);
        }

        public void loadFlavour()
        {
            string[] filePaths = Directory.GetFiles(".\\data\\words\\flavour");
            foreach (string fileName in filePaths)
            {

                if (fileName.EndsWith(".txt"))
                {
                    //World.log("Flavour seen: " + fileName);
                    string[] words = System.IO.File.ReadAllLines(fileName);
                    string title = words[0];
                    string body = "";
                    for (int i = 1; i < words.Length; i++)
                    {
                        body += words[i] + "\n";
                    }
                    //World.log("Flavour read. Title: " + title + " body: " + body);
                    put(title, body);
                }
            }
        }
        public void loadFlavourLinux(World world)
        {
            string[] filePaths = Directory.GetFiles(world.pathPrefix + "/data/words/flavour");
            foreach (string fileName in filePaths)
            {

                if (fileName.EndsWith(".txt"))
                {
                    //World.log("Flavour seen: " + fileName);
                    string[] words = System.IO.File.ReadAllLines(fileName);
                    string title = words[0];
                    string body = "";
                    for (int i = 1; i < words.Length; i++)
                    {
                        body += words[i] + "\n";
                    }
                    //World.log("Flavour read. Title: " + title + " body: " + body);
                    put(title, body);
                }
            }
        }

        public void put(string key, string val)
        {
            List<string> existing = new List<string>();
            if (flavourLists.ContainsKey(key))
            {
                existing = flavourLists.get(key);
            }
            else
            {
                flavourLists.put(key, existing);
            }
            existing.Add(val);
        }

        public string lookup(string key)
        {
            if (flavourLists.ContainsKey(key))
            {
                List<string> opts = flavourLists.get(key);
                int q = Eleven.random.Next(opts.Count);
                return opts[q];
            }
            return key;
        }

        public bool hasLookupKey(string key)
        {
            return flavourLists.ContainsKey(key);
        }
    }
}
