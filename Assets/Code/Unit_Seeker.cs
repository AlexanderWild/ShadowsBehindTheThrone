using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code
{
    public class Unit_Seeker : Unit
    {
        public bool knowsTruth = false;
        public int reqSecrets = 14;
        public int secrets = 0;

        
        public Unit_Seeker(Location loc,SocialGroup soc) : base(loc,soc)
        {
            maxHp = 3;
            hp = 3;

            reqSecrets = loc.map.param.unit_seeker_nReqSecrets;

            abilities.Add(new Abu_Seeker_SpreadTruth());
            abilities.Add(new Abu_Seeker_LearnTruth());
            abilities.Add(new Abu_Seeker_AccessLibrary());
            abilities.Add(new Abu_Seeker_UncoverSecret());
            abilities.Add(new Abu_Seeker_ChangeDirection());
            abilities.Add(new Abu_Base_Recruit());
            abilities.Add(new Abu_Base_ChangeIdentity());
            abilities.Add(new Abu_Base_SetLoose());
            abilities.Add(new Abu_Base_Apoptosis());
        }

        public override void turnTickInner(Map map)
        {
        }

        public override bool checkForDisband(Map map)
        {
            return false;
        }
        public override void turnTickAI(Map map)
        {
        }

        public override bool definesBackground()
        {
            return knowsTruth;
        }

        public override Sprite getPortraitBackground()
        {
            return World.staticMap.world.textureStore.person_back_eyes;
        }


        public override Sprite getSprite(World world)
        {
            return world.textureStore.unit_seeker;
        }

        public override bool hasSpecialInfo()
        {
            return true;
        }

        public override Color specialInfoColour()
        {
            return new Color(0.8f, 00.8f, 0.4f);
        }

        public override string specialInfo()
        {
            return "Secrets " + secrets + "/" + reqSecrets + "\n" + getSecretLocationString(); ;
        }

        public string getSecretLocationString()
        {

            double bestDist = -1;
            Location bestLoc = null;
            foreach (Location loc in this.location.map.locations)
            {
                foreach (Property p in loc.properties)
                {
                    if (p.proto is Pr_ForgottenSecret)
                    {
                        double dist = Math.Abs(loc.hex.x - this.location.hex.x) + Math.Abs(loc.hex.y - this.location.hex.y);
                        if (dist < bestDist || bestDist == -1)
                        {
                            bestLoc = loc;
                            bestDist = dist;
                        }
                    }
                }
            }
            string add = "";
            if (bestLoc != null)
            {
                add += "Closest: " + bestLoc.getName() + " (";
                bool dir = false;
                if (bestLoc.hex.y > location.hex.y) { add += "South "; dir = true; }
                if (bestLoc.hex.y < location.hex.y) { add += "North "; dir = true; }
                if (bestLoc.hex.x > location.hex.x) { add += "West"; dir = true; }
                if (bestLoc.hex.x < location.hex.x) { add += "East"; dir = true; }
                if (!dir) { add += "Here"; }
                add += ")";
            }
            else
            {
                add = "No secrets remain undiscovered";
            }
            return add;
        }

        public override string specialInfoLong()
        {
            return "Seekers search for Forgotten Secrets in libraries and buried beneath the earth. If they gather enough, they can learn dark truths and gain terrible power."
                + "\n\n" + getSecretLocationString();
        }

        public override string getTitleM()
        {
            return "Seeker";
        }

        public override string getTitleF()
        {
            return "Seeker";
        }

        public override string getDesc()
        {
            return "Seekers must search for 'Forgotten Secrets' across the world, in order to learn lost powers.";
        }

        public static void addForgottenSecrets(Map map)
        {

            int nSecrets = 0;
            int nReq = map.param.unit_seeker_nCreatedSecrets;
            List<Location> available = new List<Location>();
            foreach (Location loc in map.locations)
            {
                if (loc.settlement != null && (loc.settlement is Set_University || loc.settlement is Set_Abbey))
                {
                    available.Add(loc);
                }
            }

            //From these, pick at most nReq
            //Gonna do it badly, because I don't have a random.shuffle(list) implementation at hand
            while (available.Count > 0 && nSecrets < nReq)
            {
                int q = Eleven.random.Next(available.Count);
                Location chosen = available[q];
                available.RemoveAt(q);
                nSecrets += 1;
                Property.addProperty(map, chosen, "Forgotten Secret");
            }

            if (nSecrets < nReq)
            {
                available = new List<Location>();
                foreach (Location loc in map.locations)
                {
                    if (loc.settlement == null && (loc.isOcean == false))
                    {
                        available.Add(loc);
                    }
                }
                while (available.Count > 0 && nSecrets < nReq)
                {
                    int q = Eleven.random.Next(available.Count);
                    Location chosen = available[q];
                    available.RemoveAt(q);
                    nSecrets += 1;
                    Property.addProperty(map, chosen, "Forgotten Secret");
                }
            }

            //Emergency fallback, make everywhere acceptable
            if (nSecrets < nReq)
            {
                available = new List<Location>();
                foreach (Location loc in map.locations)
                {
                    available.Add(loc);
                }
                while (available.Count > 0 && nSecrets < nReq)
                {
                    int q = Eleven.random.Next(available.Count);
                    Location chosen = available[q];
                    available.RemoveAt(q);
                    nSecrets += 1;
                    Property.addProperty(map, chosen, "Forgotten Secret");
                }
            }


        }
    }
}
