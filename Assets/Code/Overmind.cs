using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Code
{
    public class Overmind
    {
        public float power;
        public bool hasTakenAction;

        public List<Ability> abilities = new List<Ability>();
        public List<Ability> powers = new List<Ability>();
        public Map map;
        public Person enthralled;
        public bool victoryAchieved = false;

        public Overmind(Map map)
        {
            this.map = map;
            powers.Add(new Ab_Enth_Enthrall());
            powers.Add(new Ab_Enth_DarkEmpire());
            //abilities.Add(new Ab_TestAddShadow());
            powers.Add(new Ab_Fishman_Lair());
            powers.Add(new Ab_Fishman_CultOfTheDeep());
            //powers.Add(new Ab_Fishman_Call());
            powers.Add(new Ab_Fishman_Attack());
            powers.Add(new Ab_Enth_MiliaryAid());
            powers.Add(new Ab_Enth_TrustingFool());
            powers.Add(new Ab_Enth_Enshadow());
            powers.Add(new Ab_Enth_Apoptosis());
            powers.Add(new Ab_UnholyFlesh_Seed());
            powers.Add(new Ab_UnholyFlesh_Screetching());
            powers.Add(new Ab_UnholyFlesh_Attack());
            powers.Add(new Ab_UnholyFlesh_Defend());
            powers.Add(new Ab_UnholyFlesh_Grow());
            powers.Add(new Ab_Over_CancelVote());
            powers.Add(new Ab_Over_InformationBlackout());
            powers.Add(new Ab_Over_SowDissent());
            powers.Add(new Ab_Over_UncannyGlamour());

            abilities.Add(new Ab_Soc_Vote());
            abilities.Add(new Ab_Soc_ProposeVote());
            abilities.Add(new Ab_Soc_SharedGlory());
            abilities.Add(new Ab_Soc_JoinRebels());
            abilities.Add(new Ab_Soc_JoinLoyalists());
            abilities.Add(new Ab_Soc_ShareEvidence());
            abilities.Add(new Ab_Soc_BoycottVote());
            abilities.Add(new Ab_Soc_Fearmonger());
            abilities.Add(new Ab_Soc_DenounceOther());
            abilities.Add(new Ab_Soc_SwitchVote());
        }

        public void turnTick()
        {
            hasTakenAction = false;
            power += map.param.overmind_powerRegen;
            power = Math.Min(power, map.param.overmind_maxPower);

            processEnthralled();
            int count = 0;
            double sum = 0;
            foreach (Location loc in map.locations)
            {
                if (loc.person() != null) { sum += loc.person().shadow;count += 1; }
            }
            if (count == 0) { map.data_avrgEnshadowment = 0; }
            else { map.data_avrgEnshadowment = sum / count; }
            if ((!victoryAchieved) && map.data_avrgEnshadowment > map.param.victory_targetEnshadowmentAvrg)
            {
                victory();
            }
        }
        public void victory()
        {
            victoryAchieved = true;
            World.log("VICTORY DETECTED");
            map.world.prefabStore.popVictoryBox();
        }

        public void processEnthralled()
        {
            if (enthralled == null) { return; }

            if (enthralled.isDead) { enthralled = null; }
        }
        public int countAvailableAbilities(Hex hex)
        {
            if (hex == null) { return 0; }
            if (hex.location == null) { return 0; }
            int n = 0;
            foreach (Ability a in abilities)
            {
                if (a.castable(map, hex))
                {
                    n += 1;
                }
            }
            return n;
        }
        public int countAvailablePowers(Hex hex)
        {
            if (hex == null) { return 0; }
            if (hex.location == null) { return 0; }
            int n = 0;
            foreach (Ability a in powers)
            {
                if (a.castable(map, hex))
                {
                    n += 1;
                }
            }
            return n;
        }
        public List<Ability> getAvailableAbilities(Hex hex)
        {
            if (hex == null) { return new List<Ability>(); }
            if (hex.location == null) { return new List<Ability>(); }
            List<Ability> reply = new List<Ability>();
            foreach (Ability a in abilities)
            {
                if (a.castable(map, hex))
                {
                    reply.Add(a);
                }
            }
            return reply;
        }
        public List<Ability> getAvailablePowers(Hex hex)
        {
            if (hex == null) { return new List<Ability>(); }
            if (hex.location == null) { return new List<Ability>(); }
            List<Ability> reply = new List<Ability>();
            foreach (Ability a in powers)
            {
                if (a.castable(map, hex))
                {
                    reply.Add(a);
                }
            }
            return reply;
        }
        public int countAvailableAbilities(Person p)
        {
            if (p == null) { return 0; }
            int n = 0;
            foreach (Ability a in abilities)
            {
                if (a.castable(map, p))
                {
                    n += 1;
                }
            }
            return n;
        }
        public int countAvailablePowers(Person p)
        {
            if (p == null) { return 0; }
            int n = 0;
            foreach (Ability a in powers)
            {
                if (a.castable(map, p))
                {
                    n += 1;
                }
            }
            return n;
        }
        public List<Ability> getAvailableAbilities(Person p)
        {
            if (p == null) { return new List<Ability>(); }
            List<Ability> reply = new List<Ability>();
            foreach (Ability a in abilities)
            {
                if (a.castable(map, p))
                {
                    reply.Add(a);
                }
            }
            return reply;
        }
        public List<Ability> getAvailablePowers(Person p)
        {
            if (p == null) { return new List<Ability>(); }
            List<Ability> reply = new List<Ability>();
            foreach (Ability a in powers)
            {
                if (a.castable(map, p))
                {
                    reply.Add(a);
                }
            }
            return reply;
        }
    }
}
