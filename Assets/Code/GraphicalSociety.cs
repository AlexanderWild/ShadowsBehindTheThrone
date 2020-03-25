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

        public enum viewState { UNLANDED, NEIGHBOR, HIERARCHY };
        public static viewState state = viewState.HIERARCHY;

        public static void setup(Society soc)
        {
            activeSociety = soc;
            foreach (Person p in activeSociety.people)
                world.prefabStore.getGraphicalSlot(p);
        }

        public static void clear()
        {
            foreach (GraphicalSlot s in loadedSlots)
            {
                s.gameObject.SetActive(false);

                s.upperRightText.text = "";
                s.lowerRightText.text = "";
            }
        }

        public static void resetHidden()
        {
            foreach (GraphicalSlot s in loadedSlots)
            {
                if (!s.gameObject.active)
                    s.recenter();
            }
        }

        public static void refreshHierarchy()
        {
            clear();

            Person ss = activeSociety.getSovreign();
            if (ss == null)
                return;

            focus = ss;

            var tree = new Dictionary<Person, List<Person>>();
            foreach (Person p in activeSociety.people)
            {
                Person sp = p.getDirectSuperiorIfAny();
                if (sp != null)
                {
                    // FIXME
                    if (sp == focus)
                        continue;

                    if (!tree.ContainsKey(sp))
                        tree.Add(sp, new List<Person>());

                    if (sp != focus || p.getDirectSubordinates().Count == 0)
                        tree[sp].Add(p);
                }
            }

            focus.outer.gameObject.SetActive(true);
            focus.outer.targetPosition = Vector3.zero;

            int n = tree.Count, i = 0;
            foreach (var pair in tree)
            {
                GraphicalSlot ds = pair.Key.outer;

                float radius = 2.0f;
                float angle  = 6.28f / n * i;

                float x = Mathf.Cos(angle) * radius;
                float y = Mathf.Sin(angle) * radius;

                ds.gameObject.SetActive(true);
                ds.connection = focus.outer;

                ds.targetPosition = new Vector3(x, y, 0.0f);
                ds.targetColor = ds.neutralColor;
                ds.targetColor.a = 0.5f;

                float n2 = pair.Value.Count, j = 0;
                foreach (Person pp in pair.Value)
                {
                    GraphicalSlot ds2 = pp.outer;

                    float radius2 = 2.0f;
                    float spread  = (n2 > 4) ? 3.5f : 2.5f;
                    float angle2  = (angle - 1.0f) + spread / n2 * j;

                    float x2 = Mathf.Cos(angle2) * radius2 + x;
                    float y2 = Mathf.Sin(angle2) * radius2 + y;

                    ds2.gameObject.SetActive(true);
                    ds2.connection = ds;

                    ds2.targetPosition = new Vector3(x2, y2, 0.0f);
                    ds2.targetColor = ds2.neutralColor;
                    ds2.targetColor.a = 0.25f;

                    j += 1;
                }

                i += 1;
            }

            state = viewState.HIERARCHY;
            resetHidden();
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
                if (p != focus && (p.title_land != null || p == originalFocus))
                    n += 1;
            }

            focus.outer.gameObject.SetActive(true);
            focus.outer.targetPosition = Vector3.zero;

            int i = 0;
            foreach (Person p in activeSociety.people)
            {
                if (p == focus || (p.title_land == null && p != originalFocus))
                    continue;

                GraphicalSlot ds = p.outer;

                float radius = (n > 12) ? 3.5f - 1.3f * (i % 2) : 3.2f;
                float angle  = 6.28f / n * i;

                float x = Mathf.Cos(angle) * radius;
                float y = Mathf.Sin(angle) * radius;

                ds.gameObject.SetActive(true);
                ds.connection = focus.outer;

                ds.targetPosition = new Vector3(x, y, 0.0f);
                ds.targetColor = ds.neutralColor;
                ds.targetColor.a = 0.5f;

                i += 1;
            }

            state = viewState.NEIGHBOR;
            resetHidden();
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

                float radius = 2.5f;
                float angle  = 6.28f / n * i;

                float x = Mathf.Cos(angle) * radius;
                float y = Mathf.Sin(angle) * radius;

                ds.gameObject.SetActive(true);
                ds.connection = focus.outer;

                ds.targetPosition = new Vector3(x, y, 0.0f);
                ds.targetColor = ds.neutralColor;
                ds.targetColor.a = 0.5f;

                i += 1;
            }

            state = viewState.UNLANDED;
            resetHidden();
        }

        public static void refresh(Person pf)
        {
            switch (state)
            {
                case viewState.UNLANDED: refreshUnlanded(pf); break;
                case viewState.NEIGHBOR: refreshNeighbor(pf); break;
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
