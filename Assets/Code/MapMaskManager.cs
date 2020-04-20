using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code
{
    //[Serializable,HideInInspector]
    public class MapMaskManager
    {
        public enum maskType { NONE, NATION, PROVINCE, INFORMATION,VOTE_EFFECT,THREAT,LIKING_ME,LIKING_THEM, AWARENESS,SUSPICION, SUSPICION_FROM,TESTING };
        public maskType mask = maskType.NONE;
        public Map map;
        [NonSerialized]
        private static Color invis = new Color(0, 0, 0, 0);
        [NonSerialized]
        private static Color dark = new Color(0, 0, 0, 0.8f);

        public MapMaskManager(Map map)
        {
            this.map = map;
        }

        public bool applyMask(Hex hex)
        {
            return mask != maskType.NONE;
        }

        public string getTitleText()
        {
            if (mask == maskType.NATION)
            {
                return "Mask: Nation";
            }
            else if (mask == maskType.PROVINCE)
            {
                return "Mask: Province";
            }
            else if (mask == maskType.INFORMATION)
            {
                return "Mask: Information Availability";
            }
            else if (mask == maskType.VOTE_EFFECT)
            {
                return "Mask: Vote Effect";
            }
            else if (mask == maskType.THREAT)
            {
                return "Mask: Threat Perception";
            }
            else if (mask == maskType.LIKING_ME)
            {
                return "Mask: Selected Person's Liking";
            }
            else if (mask == maskType.LIKING_THEM)
            {
                return "Mask: Liking for Selected Person";
            }
            else if (mask == maskType.AWARENESS)
            {
                return "Mask: Awareness";
            }
            else if (mask == maskType.SUSPICION)
            {
                return "Mask: Suspicion";
            }
            else if (mask == maskType.SUSPICION_FROM)
            {
                return "Mask: Suspicion of Others";
            }
            return "";
        }
        public string getBodyText()
        {
            if (mask == maskType.LIKING_ME)
            {
                try
                {
                    Hex hex = GraphicalMap.getHexUnderMouse(Input.mousePosition).hex;
                    Person me = GraphicalMap.selectedHex.location.settlement.title.heldBy;
                    Person them = hex.location.settlement.title.heldBy;

                    if (me.state == Person.personState.enthralled)
                    {
                        return me.getFullName() + " is enthralled, so does not have normal likings.";
                    }
                    string words = me.getFullName() + " liking for " + them.getFullName();
                    RelObj rel = me.getRelation(them);
                    words += "\nAmount: " + ((int)rel.getLiking());

                    double sus = rel.getDislikingFromSuspicion();
                    if (sus != 0)
                    {
                        words += "\nFROM SUSPICION: " + (int)sus;
                    }

                    List<ReasonMsg> msgs = new List<ReasonMsg>();
                    RelObj.getLikingModifiers(me, them, msgs);
                    foreach (ReasonMsg msg in msgs)
                    {
                        words += "\n\n  -" + msg.msg + " " + ((int)msg.value);
                    }

                    List<RelEvent> sortedEvents = new List<RelEvent>();
                    sortedEvents.AddRange(rel.events);
                    sortedEvents.Sort();
                    int nPrinted = 0;
                    foreach (RelEvent ev in sortedEvents)
                    {
                        words += "\n\n  -" + ev.reason + " : " + ((int)ev.amount);
                        nPrinted += 1;
                        if (nPrinted > 6) { break; }
                    }
                    return words;
                }
                catch (Exception e)
                {
                    return "";
                }
            }
            else if (mask == maskType.LIKING_THEM)
            {
                try
                {
                    Hex hex = GraphicalMap.getHexUnderMouse(Input.mousePosition).hex;
                    Person them = GraphicalMap.selectedHex.location.settlement.title.heldBy;
                    Person me = hex.location.settlement.title.heldBy;

                    if (me.state == Person.personState.enthralled)
                    {
                        return me.getFullName() + " is enthralled, so does not have normal likings.";
                    }

                    string words = me.getFullName() + " liking for " + them.getFullName();
                    RelObj rel = me.getRelation(them);
                    words += "\nAmount: " + ((int)rel.getLiking());

                    double sus = rel.getDislikingFromSuspicion();
                    if (sus != 0)
                    {
                        words += "\nFROM SUSPICION: " + (int)sus;
                    }

                    List<ReasonMsg> msgs = new List<ReasonMsg>();
                    RelObj.getLikingModifiers(me, them, msgs);
                    foreach (ReasonMsg msg in msgs)
                    {
                        words += "\n\n  -" + msg.msg + " " + ((int)msg.value);
                    }

                    List<RelEvent> sortedEvents = new List<RelEvent>();
                    sortedEvents.AddRange(rel.events);
                    sortedEvents.Sort();
                    int nPrinted = 0;
                    foreach (RelEvent ev in sortedEvents)
                    {
                        words += "\n\n  -" + ev.reason + " : " + ((int)ev.amount);
                        nPrinted += 1;
                        if (nPrinted > 6) { break; }
                    }
                    return words;
                }
                catch (Exception e)
                {
                    return "";
                }
            }
            else if (mask == maskType.VOTE_EFFECT)
            {
                if (GraphicalMap.selectedHex != null && GraphicalMap.selectedHex.location != null && GraphicalMap.selectedHex.location.person() != null)
                {
                    Person voter = GraphicalMap.selectedHex.location.person();
                            Society soc = (Society)voter.society;
                            if (soc.voteSession == null) { return "No voting currently happening in " + voter.getFullName() + "'s society"; }
                            VoteOption opt = soc.voteSession.issue.options[0];
                            foreach (VoteOption o2 in soc.voteSession.issue.options)
                            {
                                if (o2.votesFor.Contains(voter))
                                {
                                    opt = o2;
                                    break;
                                }
                            }
                    return "The change in liking towards " + voter.getFullName() + " if they vote for their currently preferred option (" + opt.info(soc.voteSession.issue) + ").";
                        
                }
            }
            return "";
        }

        public Color getColor(Hex hex)
        {
            if (mask == maskType.NATION)
            {
                if (hex.owner != null)
                {
                    float r = hex.owner.color.r;
                    float g = hex.owner.color.g;
                    float b = hex.owner.color.b;
                    Color color = new Color(r, g, b, 0.8f);
                    return color;
                }
                else
                {
                    return new Color(0.5f, 0.5f, 0.5f, 0.5f);
                }
            }
            else if (mask == maskType.PROVINCE)
            {
                return new Color(hex.province.cr, hex.province.cg, hex.province.cb, 0.5f);
            }
            else if (mask == maskType.INFORMATION)
            {
                if (hex.location != null && GraphicalMap.selectedHex != null && GraphicalMap.selectedHex.location != null && GraphicalMap.selectedHex.location.soc != null)
                {
                    SocialGroup group = GraphicalMap.selectedHex.location.soc;

                    float mult = (float)map.getInformationAvailability(hex.location, group);
                    mult = Mathf.Max(0, mult);
                    mult = Mathf.Min(1, mult);
                    float r = mult;
                    float g = mult;
                    float b = mult;

                    //Color color = new Color(r, g, b, 0.8f);
                    Color color = new Color(0,0,0,(1-mult));
                    return color;
                }
                else if (hex.location != null)
                {
                    return new Color(0f, 0f, 0f, 1f);
                }
                else
                {
                    return new Color(0f, 0f, 0f, 0.75f);
                }
            }
            else if (mask == maskType.VOTE_EFFECT)
            {
                if (GraphicalMap.selectedHex != null && GraphicalMap.selectedHex.location != null && GraphicalMap.selectedHex.location.person() != null && hex.location != null && hex.location.soc != null)
                {
                    if (GraphicalMap.selectedHex == hex)
                    {
                        return new Color(1f, 1f, 1f, 0.5f);
                    }
                    Person voter = GraphicalMap.selectedHex.location.person();
                    if (hex.location.soc == voter.society) {
                        if (hex.location.person() != null && hex.location.soc is Society)
                        {
                            Society soc = (Society)hex.location.soc;
                            if (soc.voteSession == null) { return new Color(0f, 0f, 0f, 0.75f); }
                            VoteOption opt = null;
                            foreach (VoteOption o2 in soc.voteSession.issue.options)
                            {
                                if (o2.votesFor.Contains(voter))
                                {
                                    opt = o2;
                                    break;
                                }
                            }
                            if (opt == null)
                            {
                                soc.voteSession.assignVoters();
                            }
                            foreach (VoteOption o2 in soc.voteSession.issue.options)
                            {
                                if (o2.votesFor.Contains(voter))
                                {
                                    opt = o2;
                                    break;
                                }
                            }
                            if (soc.voteSession != null)
                            {
                                float delta = (float)soc.voteSession.issue.getLikingDelta(hex.location.person(), opt,soc.voteSession.issue);
                                delta /= 25;
                                if (delta > 1) { delta = 1f; }
                                if (delta < -1) { delta = -1f; }
                                if (delta >= 0)
                                {
                                    return new Color(0f,delta, 0f, 0.75f);
                                }
                                else
                                {
                                    return new Color(-delta, 0f, 0f, 0.75f);

                                }
                            }
                        }
                    }
                }
                return new Color(0f, 0f, 0f, 0.75f);
            }
            else if (mask == maskType.THREAT)
            {
                if (hex.location != null && GraphicalMap.selectedHex != null && GraphicalMap.selectedHex.location != null && GraphicalMap.selectedHex.location.settlement != null && GraphicalMap.selectedHex.location.settlement.title != null && GraphicalMap.selectedHex.location.settlement.title.heldBy != null)
                {
                    SocialGroup group = GraphicalMap.selectedHex.location.soc;

                    Person p = GraphicalMap.selectedHex.settlement.title.heldBy;
                    double highestThreat = 0;
                    foreach (ThreatItem item in p.threatEvaluations)
                    {
                        if (item.group != null && item.threat > highestThreat)
                        {
                            highestThreat = item.threat;
                        }
                    }
                    if (highestThreat != 0)
                    {
                        if (hex.location.soc == null)
                        {
                            return Color.black;
                        }
                        double threat = 0;
                        foreach (ThreatItem item in p.threatEvaluations)
                        {
                            if (item.group == hex.location.soc) { threat = item.threat/highestThreat;break; }
                        }
                        float mult = (float)threat;
                        mult = Mathf.Max(0, mult);
                        mult = Mathf.Min(1, mult);
                        float r = mult;
                        float g = mult;
                        float b = mult;

                        //Color color = new Color(r, g, b, 0.8f);
                        Color color = new Color(mult, 0, 0, 0.5f);
                        return color;
                    }
                    else
                    {
                        return Color.black;
                    }
                }
                else if (hex.location != null)
                {
                    return new Color(0f, 0f, 0f, 1f);
                }
                else
                {
                    return new Color(0f, 0f, 0f, 0.75f);
                }
            }

            else if (mask == maskType.LIKING_THEM)
            {
                Color c = new Color(0, 0, 0, 0.5f);
                try
                {
                    Person me = GraphicalMap.selectedHex.location.settlement.title.heldBy;
                    Person them = hex.location.settlement.title.heldBy;
                    float liking = (float)them.getRelation(me).getLiking();
                    if (liking > 0)
                    {
                        if (liking > 100) { liking = 100; }
                        liking /= 100;
                        c = new Color(0, liking, 0, 0.5f);
                    }
                    else
                    {
                        liking *= -1;
                        if (liking > 100) { liking = 100; }
                        liking /= 100;
                        c = new Color(liking, 0, 0, 0.5f);

                    }
                }
                catch (NullReferenceException e)
                {

                }
                catch (ArgumentNullException e)
                {

                }
                return c;
            }

            else if (mask == maskType.LIKING_ME)
            {
                Color c = new Color(0, 0, 0, 0.5f);
                try
                {
                    Person me = GraphicalMap.selectedHex.location.settlement.title.heldBy;
                    Person them = hex.location.settlement.title.heldBy;
                    float liking = (float)me.getRelation(them).getLiking();
                    if (liking > 0)
                    {
                        if (liking > 100) { liking = 100; }
                        liking /= 100;
                        c = new Color(0, liking, 0, 0.5f);
                    }
                    else
                    {
                        liking *= -1;
                        if (liking > 100) { liking = 100; }
                        liking /= 100;
                        c = new Color(liking, 0, 0, 0.5f);

                    }
                }
                catch (NullReferenceException e)
                {

                }
                catch (ArgumentNullException e)
                {

                }
                return c;
            }
            else if (mask == maskType.AWARENESS)
            {
                try
                {
                    Person them = hex.settlement.title.heldBy;

                    return new Color((float)them.awareness, 0, 0, 0.9f);
                }
                catch (Exception e)
                {
                    return new Color(0, 0, 0, 0.9f);
                }
            }
            else if (mask == maskType.SUSPICION)
            {
                try
                {
                    Person me = GraphicalMap.selectedHex.settlement.title.heldBy;
                    Person them = hex.settlement.title.heldBy;
                    RelObj rel = me.getRelation(them);

                    return new Color((float)rel.suspicion, 0, 0, 0.5f);
                }catch(Exception e)
                {
                    return new Color(0, 0, 0, 0.5f);
                }
            }
            else if (mask == maskType.SUSPICION_FROM)
            {
                try
                {
                    Person them = GraphicalMap.selectedHex.settlement.title.heldBy;
                    Person me = hex.settlement.title.heldBy;
                    RelObj rel = me.getRelation(them);

                    return new Color((float)rel.suspicion, 0, 0, 0.5f);
                }
                catch (Exception e)
                {
                    return new Color(0, 0, 0, 0.5f);
                }
            }
            else if (mask == maskType.TESTING)
            {
                if (hex.location != null)
                {
                    if (hex.location.debugVal == map.turn)
                    {
                        return new Color(1, 1, 1, 0.5f);
                    }
                    else
                    {
                        return new Color(0, 0, 0, 0.5f);

                    }
                }

                return new Color(0f, 0f, 0f, 0.75f);
            }
            else
            {
                return new Color(0, 0, 0, 0);
            }
        }

        public void setMask(maskType type)
        {
            mask = type;
        }
        public void toggleMask(maskType type)
        {
            if (mask == type) { mask = maskType.NONE; }
            else { mask = type; }
        }
    }
}
