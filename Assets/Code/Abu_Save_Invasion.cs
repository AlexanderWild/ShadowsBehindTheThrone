using UnityEngine;
using System.Collections.Generic;

namespace Assets.Code
{
    public class Abu_Save_Invasion: AbilityUnit
    {

        public override void castInner(Map map, Unit u)
        {
            Society soc = (Society)u.location.soc;

            SocialGroup enemy = null;
            foreach (SocialGroup sg in map.socialGroups)
            {
                if (sg.getRel(u.location.soc).state == DipRel.dipState.war)
                {
                    bool controlled = false;
                    if (sg is Society)
                    {
                        Society sgSoc = (Society)sg;
                        if (sgSoc.isDarkEmpire) { controlled = true; }
                    }
                    else
                    {
                        controlled = sg.isDark();
                    }
                    if (controlled && sg.currentMilitary > u.location.soc.currentMilitary)
                    {
                        enemy = sg;
                    }
                }
            }
            if (enemy == null) { map.world.prefabStore.popMsg(soc.getName() + " is not at war with a superior military you control."); return; }

                HashSet<Person> targets = new HashSet<Person>();
            foreach (Person p in soc.people) { targets.Add(p); }
            int nSaved = 0;
            foreach (Location loc in map.locations)
            {
                if (loc.soc == soc)
                {
                    foreach (Location l2 in loc.getNeighbours())
                    {
                        if (l2.soc != soc)
                        {
                            if (l2.person() != null) { 
                                targets.Add(l2.person()); }
                        }
                    }
                    nSaved += 1;
                }
            }
            if (nSaved >= 10)
            {
                AchievementManager.unlockAchievement(SteamManager.achievement_key.SAVIOUR);
            }

            foreach (Person p in targets)
            {
                double boost = 100;
                //Let's not overcomplicate this one
                //foreach (ThreatItem item in p.threatEvaluations)
                //{
                //    if (item.group == enemy)
                //    {
                //        boost = item.threat;
                //    }
                //}
                //if (boost > 150) { boost = 150; }
                //if (boost < 25) { boost = 25; }
                p.getRelation(u.person).addLiking(boost, "Our Saviour!", map.turn,RelObj.STACK_REPLACE,true);
            }

            DipRel rel = soc.getRel(enemy);
            if (rel.war != null)
            {
                rel.war.startTurn = map.turn;
            }
            List<Unit> rems = new List<Unit>();
            foreach (Unit u2 in map.units)
            {
                if (u2.society == enemy && u2.isMilitary)
                {
                    rems.Add(u2);
                }
                else if (u2.society == soc && u2 is Unit_Army)
                {
                    u2.task = null;//Retask, to drop the 'defend the homeland' defensive task and go on the offensive
                }
            }
            foreach (Unit u2 in rems)
            {
                u2.die(map, "Killed by The Saviour");
            }
            soc.posture = Society.militaryPosture.offensive;//Flip to assault mode to ruin the dark forces

            Evidence e2 = new Evidence(map.turn);
            e2.pointsTo = u;
            e2.weight = u.location.map.param.unit_majorEvidence;
            u.location.evidence.Add(e2);

            u.location.map.world.prefabStore.popImgMsg(u.getName() + " saves " + soc.getName() + " from invasion by " + enemy.getName()+ "."
                + "\nNobles within the nation, and those adjacent to it gain liking for " + soc.getName() + ". " + targets.Count + " nobles affected. " + rems.Count + " units killed."
                + soc.getName() + " will now wage this war with renewed vigour, driving back their dark enemy",
                u.location.map.world.wordStore.lookup("ABILITY_SAVIOUR_INVASION"),7);

        }
        public override bool castable(Map map, Unit u)
        {
            if (u.location.soc == null) { return false; }
            if (u.location.soc is Society == false) { return false; }
            foreach (SocialGroup sg in map.socialGroups)
            {
                if (sg.getRel(u.location.soc).state == DipRel.dipState.war)
                {
                    bool controlled = false;
                    if (sg is Society)
                    {
                        Society sgSoc = (Society)sg;
                        if (sgSoc.isDarkEmpire) { controlled = true; }
                    }
                    else
                    {
                        controlled = sg.isDark();
                    }
                    if (controlled && sg.currentMilitary > u.location.soc.currentMilitary)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public override void cast(Map map, Hex hex) { }
        public override bool castable(Map map, Hex hex)
        {
            return false;
        }

        public override int getCost()
        {
            return 0;
        }

        public override int getCooldown()
        {
            return 0;
        }

        public override string specialCost()
        {
            return "Major Evidence";
        }
        public override string getDesc()
        {
            return "If you are attacking a weaker nation with forces under your control (such as Deep Ones, Unholy Flesh or a Dark Empire) you may sabotage it, " +
                "losing a major portion of your military forces in order to gain a major liking boost from nearby nobles."
                + "\n[Requires a society at war by militarily larger group you control]";
        }

        public override string getName()
        {
            return "Drive back the Monsters";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_saviour;
        }
    }
}