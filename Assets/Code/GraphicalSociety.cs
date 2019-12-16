using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

namespace Assets.Code
{
    public class GraphicalSociety
    {
        public static World world;

        public static Society activeSociety;
        public static Person focus;
        public static List<GraphicalSlot> loadedSlots = new List<GraphicalSlot>();

        public static void setup(Society soc)
        {
            activeSociety = soc;
            foreach (Person p in activeSociety.people)
            {
                GraphicalSlot slot = world.prefabStore.getGraphicalSlot(p);
            }

            if (loadedSlots.Count != 0)
                refresh(activeSociety.people[0]);
        }

        public static void refresh(Person pf)
        {
            focus = pf;

            int tn = activeSociety.people.Count - 1;
            int i = 0;

            foreach (GraphicalSlot s in loadedSlots)
            {
                if (s.inner == focus)
                {
                    s.targetPosition = Vector3.zero;

                    s.subtitle.text = "";
                    s.lowerRightText.text = "";
                }
                else
                {
                    int n, ring;
                    if (i < 12)
                    {
                        n = (tn < 12) ? tn : 12;
                        ring = 0;
                    }
                    else
                    {
                        n = (tn - 12 < 52) ? tn - 12 : 52;
                        ring = 1;
                    }

                    RelObj rel = focus.getRelation(s.inner);
                    float liking = (float)rel.getLiking() / 100;
                    if (i == 0)
                        Debug.Log(rel.suspicion.ToString());

                    float radius = (1.75f * ring + 3.0f);
                    float x = Mathf.Cos(6.28f / n * i + ring * 0.1f) * radius;
                    float y = Mathf.Sin(6.28f / n * i + ring * 0.1f) * radius;

                    s.targetPosition = new Vector3(x, y, 0.0f);
                    if (liking < 0)
                        s.targetColor = Color.Lerp(s.neutralColor, s.badColor, -liking);
                    else
                        s.targetColor = Color.Lerp(s.neutralColor, s.goodColor, liking);
                    s.targetColor.a = 0.5f;
                    //s.targetColor.a = 0.1f + ((float)rel.suspicion / 100);

                    s.subtitle.text = "Relationship with " + focus.firstName;
                    s.lowerRightText.text  = "Liked by: " + focus.getRelation(s.inner).getLiking().ToString("N0") + "%";
                    s.lowerRightText.text += ", Likes: " + s.inner.getRelation(focus).getLiking().ToString("N0") + "%";
                    s.lowerRightText.text += "\nSuspected by: " + focus.getRelation(s.inner).suspicion.ToString("N0") + "%";
                    s.lowerRightText.text += ", Suspects: " + s.inner.getRelation(focus).suspicion.ToString("N0") + "%";

                    i += 1;
                }
            }
        }

        public static void showHover(Person p)
        {
            //
        }

        public static void purge()
        {
            foreach (SocialGroup sg in World.staticMap.socialGroups)
            {
                if (sg is Society)
                {
                    Society soc = (Society)sg;
                    foreach (Person p in soc.people){
                        p.outer = null;
                    }
                }
            }
            foreach (GraphicalSlot sl in loadedSlots)
            {
                sl.inner.outer = null;
            }
            loadedSlots.Clear();
        }
    }
}
