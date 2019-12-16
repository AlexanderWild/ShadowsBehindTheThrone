using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Code
{
    public class ZeitOpt
    {
        public Zeit zeit;
        public string desc;
        public Map map;
        public static int DISSENT_NONE = 0;
        public static int DISSENT_KING = 1;
        public static int DISSENT_RULERS = 2;
        public static int DISSENT_EVERYONE = 3;


        //Effects
        public float[] politicalDeltas;
        public int dissentEffect = 0;
        public bool amplifyTargetThreat;
        public bool reduceTargetThreat;
        public bool reduceOtherThreats;

        public ZeitOpt(Zeit zeit, Map map)
        {
            this.map = map;
            this.zeit = zeit;
            //politicalDeltas = new float[Person.politicNames.Length];
        }

        public void implement(Person caster)
        {
            /*
            List<Person> rulers = caster.society.getRulers();

            foreach (Slot slot in caster.slot.heirarchy.allSlots)
            {
                if (slot.person == null) { continue; }
                float liking = slot.person.getLiking(caster);
                if (liking < 0) { liking = 0; }
                liking /= 100;
                if (liking > 1) { liking = 1; }

                for (int i = 0; i < politicalDeltas.Length; i++)
                {
                    float add = politicalDeltas[i] * liking * zeit.weight * map.para.society_zeitBaseAmount;
                    slot.person.politics[i] += add;
                    if (slot.person.politics[i] > 1) { slot.person.politics[i] = 1; }
                }

                if (this.amplifyTargetThreat)
                {
                    foreach (ThreatItem item in slot.person.threatFears)
                    {
                        if (this.zeit.threatItem.isSame(item))
                        {
                            item.zeitgeist *= (float)Math.Pow(2f,zeit.weight);
                        }
                    }
                }
                if (this.reduceTargetThreat)
                {
                    foreach (ThreatItem item in slot.person.threatFears)
                    {
                        if (this.zeit.threatItem.isSame(item))
                        {
                            item.zeitgeist *= (float)Math.Pow(0.5f, zeit.weight);
                        }
                    }
                }
                if (this.reduceOtherThreats)
                {
                    foreach (ThreatItem item in slot.person.threatFears)
                    {
                        if (zeit.threatItem.isSame(item) == false)
                        {
                            item.zeitgeist *= (float)Math.Pow(0.5f, zeit.weight);
                        }
                    }
                }

                //Dissent makes you dislike your leaders
                float dissentDelta = map.para.society_zeitDislikeRulersAmount * liking * zeit.weight;
                if (dissentEffect == DISSENT_KING)
                {
                    if (slot == caster.society.command.top)
                    {
                        slot.person.getRel(caster).addLiking(map.para.society_zeitDislikeRulersAmount);
                    }
                    else
                    {
                        if (caster.society.command.top.person != null)
                        {
                            slot.person.getRel(caster.society.command.top.person).addLiking(dissentDelta);
                        }
                    }
                }
                else if (dissentEffect == DISSENT_RULERS)
                {
                    if (rulers.Contains(slot.person))
                    {
                        slot.person.getRel(caster).addLiking(map.para.society_zeitDislikeRulersAmount);

                    }
                    else
                    {
                        foreach (Person p in rulers)
                        {
                            slot.person.getRel(p).addLiking(dissentDelta);
                        }
                    }
                }
                else if (dissentEffect == DISSENT_EVERYONE)
                {
                    foreach (Person p in caster.society.people)
                    {
                        foreach (Person p2 in caster.society.people)
                        {
                            if (p == p2) { continue; }
                            p.getRel(p2).addLiking(map.para.society_zeitDislikeEveryoneAmount);
                        }
                    }
                }
            }
            */
        }

        public void implement(Location castOn, Society society)
        {
            /*
            List<Person> rulers = society.getRulers();
            
            foreach (Slot slot in society.command.allSlots)
            {
                if (slot.person == null) { continue; }
                if (slot.domain == null) { continue; }
                float power = 1;


                float dist = (float)map.getDist(slot.domain.loc.hex, castOn.hex);//From 0 to infinity
                //Want 1 at 0, 0.5f at 4, don't care past there
                float div = 1 + (dist / 4);//2 at 3
                power = 1 / div;

                for (int i = 0; i < politicalDeltas.Length; i++)
                {
                    float add = politicalDeltas[i] * power * zeit.weight * map.para.society_zeitBaseAmount;
                    slot.person.politics[i] += add;
                    if (slot.person.politics[i] > 1) { slot.person.politics[i] = 1; }
                }

                //Dissent makes you dislike your leaders
                float dissentDelta = map.para.society_zeitDislikeRulersAmount * power * zeit.weight;
                if (dissentEffect == DISSENT_KING)
                {
                    if (slot == society.command.top)
                    {
                        //Do nothing, since you can't target the caster
                    }
                    else
                    {
                        if (society.command.top.person != null)
                        {
                            slot.person.getRel(society.command.top.person).addLiking(dissentDelta);
                        }
                    }
                }
                else if (dissentEffect == DISSENT_RULERS)
                {
                    if (rulers.Contains(slot.person))
                    {
                        //Do nothing, since you can't target the caster
                    }
                    else
                    {
                        foreach (Person p in rulers)
                        {
                            slot.person.getRel(p).addLiking(dissentDelta);
                        }
                    }
                }
                else if (dissentEffect == DISSENT_EVERYONE)
                {
                    if (slot.person != null)
                    {
                        Person p = slot.person;
                        //foreach (Person p in society.people)
                        //{
                        foreach (Person p2 in society.people)
                        {
                            if (p == p2) { continue; }
                            p.getRel(p2).addLiking(map.para.society_zeitDislikeEveryoneAmount);
                        }
                        //}
                    }
                }
            }
            */
        }

        public string getEffectsDesc()
        {
            string reply = "" + desc + "\n";
            /*
            for (int i = 0; i < Person.politicNames.Length; i++)
            {
                if (politicalDeltas[i] > 0)
                {
                    reply += "Increase: " + Person.politicNames[i] + "  ";
                }
                else if (politicalDeltas[i] < 0)
                {
                    reply += "Decreases: " + Person.politicNames[i] + "  ";
                }
            }
            */

            if (this.reduceTargetThreat)
            {
                reply += "Reduces estimate of this threat. ";
            }
            if (amplifyTargetThreat)
            {
                reply += "Increases estimate of this threat. ";
            }
            if (reduceOtherThreats)
            {
                reply += "Decreases other threat-estimates. ";
            }


            if (dissentEffect == DISSENT_KING)
            {
                reply += "Blames Leader ";
            }
            else if (dissentEffect == DISSENT_RULERS)
            {
                reply += "Blames High-Ranking Nobles ";
            }
            else if (dissentEffect == DISSENT_EVERYONE)
            {
                reply += "Blames Everyone ";
            }
            return reply;
        }
    }
}
