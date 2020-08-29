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
        public static Person originalFocus;
        public static List<GraphicalSlot> loadedSlots = new List<GraphicalSlot>();
        public static Dictionary<Province, GraphicalSlot> loadedPlaceholders = new Dictionary<Province, GraphicalSlot>();
        public static float zoom = 1;
        public static float offX = 0;
        public static float offY = 0;

        public enum viewState { UNLANDED, NEIGHBOR, HIERARCHY, DYNAMIC };
        public static viewState state = viewState.HIERARCHY;
        public static NewtonSolver<GraphicalSlot> solver = null;

        private static Vector3 originalScale;

        private class AverageLikingMetric : NewtonSolver<GraphicalSlot>.IDistanceMetric
        {
            public double getDistance(GraphicalSlot a, GraphicalSlot b)
            {
                RelObj ra = a.inner.getRelation(b.inner);
                RelObj rb = b.inner.getRelation(a.inner);
                double liking = ra.getLiking() + rb.getLiking();

                return (liking + 100) / 40;
            }
        }

        public static void setup(Society soc)
        {
            activeSociety = soc;
            //Person ss = activeSociety.getSovreign();

            foreach (Person p in activeSociety.people)
            {
                world.prefabStore.getGraphicalSlot(p);
                if (p.getLocation() == null)
                    continue;
            }

            foreach (Person p in activeSociety.people)
            {
                Province pp = p.getLocation().province;
                if (!loadedPlaceholders.ContainsKey(pp))
                {
                    GraphicalSlot ds = world.prefabStore.getGraphicalSlotPlaceholder(pp.name);
                    originalScale = ds.gameObject.transform.localScale;

                    //if (ss != null && ss.getLocation().province.name == pp.name)
                    //{
                        //ds.layerBack.enabled = false;
                        //ds.border.enabled = false;
                    //}

                    ds.border.enabled = false;
                    ds.layerBack.sprite = world.textureStore.emptyDukeSlot;
                    loadedPlaceholders.Add(pp, ds);
                }
            }
        }

        public static void update()
        {
            if (state == viewState.DYNAMIC && solver != null)
                solver.solve();
        }

        public static void clear()
        {
            foreach (GraphicalSlot s in loadedSlots)
            {
                s.targetEnabled = true;
                s.gameObject.transform.localScale = originalScale;
                s.gameObject.SetActive(false);

                s.connection = null;

                s.upperRightText.text = "";
                s.lowerRightText.text = "";
                
            }
            foreach (var pair in loadedPlaceholders)
            {
                pair.Value.gameObject.SetActive(false);
            }
        }

        public static void resetHidden()
        {
            foreach (GraphicalSlot s in loadedSlots)
            {
                if (!s.gameObject.activeInHierarchy)
                    s.recenter();
            }
            foreach (var pair in loadedPlaceholders)
            {
                if (!pair.Value.gameObject.activeInHierarchy)
                    pair.Value.recenter();
            }
        }

        public static void refreshOffset()
        {
            foreach (GraphicalSlot loaded in loadedSlots)
            {
                loaded.offset = new Vector3(offX, offY, 0);
            }
            foreach (GraphicalSlot loaded in loadedPlaceholders.Values)
            {
                loaded.offset = new Vector3(offX, offY, 0);
            }
        }
        public static void refreshHierarchy(Person nfocus)
        {
            clear();

            Person ss = activeSociety.getSovreign();
            //if (ss == null)
            //    return;

            focus = (state == viewState.HIERARCHY && nfocus != null) ? nfocus : ss;

            var tree = new Dictionary<GraphicalSlot, List<GraphicalSlot>>();
            foreach (Person p in activeSociety.people)
            {
                GraphicalSlot ds = null;
                if (p == ss || p.title_land == null)
                    continue;

                Person sp = p.getDirectSuperiorIfAny();
                if (sp != null)
                {
                    if (sp == ss)
                    {
                        if (!p.getIsProvinceRuler())
                            ds = loadedPlaceholders[p.getLocation().province];
                    }
                    else
                    {
                        ds = sp.outer;
                    }
                }
                else
                {
                    ds = loadedPlaceholders[p.getLocation().province];
                }

                if (ds == null)
                    continue;

                if (!tree.ContainsKey(ds))
                    tree.Add(ds, new List<GraphicalSlot>());

                tree[ds].Add(p.outer);
            }

            ss.outer.gameObject.SetActive(true);
            focus.outer.targetPosition = Vector3.zero;

            int n = tree.Count, i = 0;
            foreach (var pair in tree)
            {
                GraphicalSlot ds = pair.Key;

                float radius = 2.0f*zoom;
                float angle  = 6.28f / n * i;

                float x = Mathf.Cos(angle) * radius;
                float y = Mathf.Sin(angle) * radius;
                

                ds.gameObject.SetActive(true);
                ds.connection = ss.outer;

                ds.targetPosition = new Vector3(x, y, 0.0f);
                ds.targetStartColor = ds.targetEndColor = ds.neutralColor;
                ds.targetStartColor.a = ds.targetEndColor.a = 0.5f;

                float n2 = pair.Value.Count, j = 0;
                foreach (GraphicalSlot ds2 in pair.Value)
                {
                    float radius2 = 1.5f*zoom;
                    float spread  = (n2 > 4) ? 3.5f : 2.5f;
                    float angle2  = (angle - spread / 2) + spread / n2 * (j + 0.5f);

                    float x2 = Mathf.Cos(angle2) * radius2 + x;
                    float y2 = Mathf.Sin(angle2) * radius2 + y;

                    ds2.gameObject.SetActive(true);
                    ds2.gameObject.transform.localScale = originalScale * 0.75f;
                    ds2.connection = ds;

                    ds2.targetPosition = new Vector3(x2, y2, 0.0f);
                    ds2.targetStartColor = ds2.targetEndColor = ds2.neutralColor;
                    ds2.targetStartColor.a = ds2.targetEndColor.a = 0.25f;

                    j += 1;
                }

                i += 1;
            }

            state = viewState.HIERARCHY;
            resetHidden();
            refreshOffset();
        }

        public static void refreshNeighbor(Person nfocus)
        {
            clear();

            focus = (nfocus != null) ? nfocus : activeSociety.getSovreign();
            if (state != viewState.NEIGHBOR)
                originalFocus = focus;

            int n = 0;
            foreach (Person p in activeSociety.people)
            {
                //if (p != focus && (p.title_land != null || p == originalFocus))
                if (p != focus || p == originalFocus)
                    n += 1;
            }

            focus.outer.gameObject.SetActive(true);
            focus.outer.targetPosition = Vector3.zero;

            int i = 0;
            foreach (Person p in activeSociety.people)
            {
                //if (p == focus || (p.title_land == null && p != originalFocus))
                if (p == focus)
                    continue;

                GraphicalSlot ds = p.outer;

                float radius = (n > 12) ? 3.5f - 1.3f * (i % 2) : 3.2f;
                float angle  = 6.28f / n * i;

                radius *= zoom;

                float x = Mathf.Cos(angle) * radius;
                float y = Mathf.Sin(angle) * radius;
                

                ds.gameObject.SetActive(true);
                ds.connection = focus.outer;

                RelObj rto   = focus.getRelation(p);
                RelObj rfrom = p.getRelation(focus);

                float to   = (float)rto.getLiking();
                float from = (float)rfrom.getLiking();

                ds.targetPosition = new Vector3(x, y, 0.0f);
                if (from < 0)
                    ds.targetStartColor = Color.Lerp(ds.neutralColor, ds.badColor, -from / 100);
                else
                    ds.targetStartColor = Color.Lerp(ds.neutralColor, ds.goodColor, from / 100);
                if (to < 0)
                    ds.targetEndColor = Color.Lerp(ds.neutralColor, ds.badColor, -to / 100);
                else
                    ds.targetEndColor = Color.Lerp(ds.neutralColor, ds.goodColor, to / 100);
                ds.targetStartColor.a = ds.targetEndColor.a = 0.5f;

                ds.upperRightText.text  = "liking for center: " + from.ToString("N0") + "%";
                ds.upperRightText.text += "\nliking from center: " + to.ToString("N0") + "%";
                ds.lowerRightText.text  = "suspicion for center: " + rfrom.suspicion.ToString("N0") + "%";
                ds.lowerRightText.text += "\nsuspicion from center: " + rto.suspicion.ToString("N0") + "%";

                i += 1;
            }

            state = viewState.NEIGHBOR;
            resetHidden();
            refreshOffset();
        }

        public static void refreshUnlanded(Person nfocus)
        {
            clear();

            focus = (nfocus != null) ? nfocus : activeSociety.getSovreign();
            if (state != viewState.UNLANDED)
                originalFocus = focus;

            int n = 0;
            foreach (Person p in activeSociety.people)
            {
                if (p != focus && (p.title_land == null || p == originalFocus))
                    n += 1;
            }

            focus.outer.gameObject.SetActive(true);
            focus.outer.targetPosition = Vector3.zero;

            int i = 0;
            foreach (Person p in activeSociety.people)
            {
                if (p == focus || (p.title_land != null && p != originalFocus))
                    continue;

                GraphicalSlot ds = p.outer;

                float radius = 2.5f*zoom;
                float angle  = 6.28f / n * i;

                float x = Mathf.Cos(angle) * radius;
                float y = Mathf.Sin(angle) * radius;

                ds.gameObject.SetActive(true);
                ds.connection = focus.outer;

                RelObj rto   = focus.getRelation(p);
                RelObj rfrom = p.getRelation(focus);

                float to   = (float)rto.getLiking();
                float from = (float)rfrom.getLiking();

                ds.targetPosition = new Vector3(x, y, 0.0f);
                if (from < 0)
                    ds.targetStartColor = Color.Lerp(ds.neutralColor, ds.badColor, -from / 100);
                else
                    ds.targetStartColor = Color.Lerp(ds.neutralColor, ds.goodColor, from / 100);
                if (to < 0)
                    ds.targetEndColor = Color.Lerp(ds.neutralColor, ds.badColor, -to / 100);
                else
                    ds.targetEndColor = Color.Lerp(ds.neutralColor, ds.goodColor, to / 100);
                ds.targetStartColor.a = ds.targetEndColor.a = 0.5f;

                ds.upperRightText.text  = "liking for center: " + from.ToString("N0") + "%";
                ds.upperRightText.text += "\nliking from center: " + to.ToString("N0") + "%";
                ds.lowerRightText.text  = "suspicion for center: " + rfrom.suspicion.ToString("N0") + "%";
                ds.lowerRightText.text += "\nsuspicion from center: " + rto.suspicion.ToString("N0") + "%";

                i += 1;
            }

            state = viewState.UNLANDED;
            resetHidden();
            refreshOffset();
        }

        public static void refreshDynamic(Person nfocus)
        {
            clear();

            List<GraphicalSlot> nodes = new List<GraphicalSlot>();
            foreach (Person p in activeSociety.people)
            {
                GraphicalSlot ds = p.outer;
                ds.targetEnabled = false;

                ds.gameObject.SetActive(true);
                nodes.Add(ds);
            }

            solver = new NewtonSolver<GraphicalSlot>(nodes, new AverageLikingMetric());

            state = viewState.DYNAMIC;
            resetHidden();
            refreshOffset();
        }

        public static void refresh(Person pf)
        {
            switch (state)
            {
                case viewState.HIERARCHY: refreshHierarchy(pf); break;
                case viewState.UNLANDED: refreshUnlanded(pf); break;
                case viewState.NEIGHBOR: refreshNeighbor(pf); break;
                case viewState.DYNAMIC: refreshDynamic(pf); break;
            }
        }

        public static void showHover(Person p)
        {
            //
        }

        public static void purge()
        {
            zoom = 1;
            offX = 0;
            offY = 0;

            // FIXME: is this still needed?
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
            loadedPlaceholders.Clear();
        }
    }
}
