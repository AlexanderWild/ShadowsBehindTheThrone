using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace Assets.Code
{
    public class GraphicalHex : MonoBehaviour
    {
        public bool needed;
        public SpriteRenderer terrainLayer;
        public SpriteRenderer locLayer;
        public SpriteRenderer locBorderLayer;
        public SpriteRenderer cloudLayer;
        public SpriteRenderer floraLayer;
        public SpriteRenderer evidenceLayer;
        public SpriteRenderer mask;
        public GameObject flag;
        public PopupNameTag nameTag;
        public PopupNameTag defenceTag;
        public PopupNameTag societyNameTag;
        public Map map;
        public Color invisible = new Color(0, 0, 0, 0);
        public Color pale = new Color(1, 1, 1, 0.2f);
        public float lastScale = 1;
        public List<GraphicalLink> linksTo = new List<GraphicalLink>();

        private Hex hexHidden;
        public World world;
        public GameObject[] borders;
        public GameObject[] borders2;
        public Vector3 loc;


        public Hex hex
        {
            get { return hexHidden; }
            set { hexHidden = value; }
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (needed == false)
            {

                try
                {
                    if (hex.location != null)
                    {
                        foreach (Property p in hex.location.properties)
                        {
                            if (p.outer != null)
                            {
                                Destroy(p.outer.gameObject);
                            }
                        }
                    }
                    Destroy(this.gameObject);
                }
                catch (Exception e)
                {
                    World.log(e.ToString());
                }
                return;
            }
            transform.localPosition = GraphicalMap.getLoc(this);

            if (this.societyNameTag != null)
            {
                this.societyNameTag.gameObject.SetActive((GraphicalMap.scale != GraphicalMap.minScale) == Input.GetKey(KeyCode.LeftControl));
            }

            if (map.param.flashEnthrallables && map.overmind.enthralled == null && map.overmind.hasEnthrallAbilities)
            {
                if (hex.location != null && hex.location.person() != null)
                {
                    if (hex.location.person().enthrallable())
                    {
                        float c = Mathf.Sin(Time.fixedTime*4);
                        terrainLayer.color = new Color(1, c, 1);
                    }
                }
            }
        }

        public void checkData()
        {
            map = hex.map;
            gameObject.transform.localScale = new Vector3(GraphicalMap.scale, GraphicalMap.scale, 1);

            float dark = 0.1f + (0.55f * hex.purity);
            float floraDark = 0.3f + (+0.7f * hex.purity);
            Color colour = new Color(dark, dark, dark);

            if (hex.terrain == Hex.terrainType.SEA)
            {
                terrainLayer.sprite = world.textureStore.hex_terrain_sea;
            }
            else if (hex.terrain == Hex.terrainType.GRASS)
            {
                terrainLayer.sprite = world.textureStore.hex_terrain_grass;
            }
            else if (hex.terrain == Hex.terrainType.PATH)
            {
                terrainLayer.sprite = world.textureStore.hex_terrain_path;
            }
            //else if (hex.terrain == Hex.terrainType.FOREST)
            //{
            //    if (map.tempMap[hex.x][ hex.y] < 0.3f)
            //    {
            //        terrainLayer.sprite = world.textureStore.hex_terrain_snowforest;
            //    }
            //    else
            //    {
            //        terrainLayer.sprite = world.textureStore.hex_terrain_forest;
            //    }
            //}
            else if (hex.terrain == Hex.terrainType.SWAMP)
            {
                terrainLayer.sprite = world.textureStore.hex_terrain_swamp;
            }
            else if (hex.terrain == Hex.terrainType.MUD)
            {
                terrainLayer.sprite = world.textureStore.hex_terrain_path;
            }
            else if (hex.terrain == Hex.terrainType.DESERT)
            {
                terrainLayer.sprite = world.textureStore.hex_terrain_desert;
            }
            else if (hex.terrain == Hex.terrainType.DRY)
            {
                terrainLayer.sprite = world.textureStore.hex_terrain_dry;
            }
            else if (hex.terrain == Hex.terrainType.WETLAND)
            {
                terrainLayer.sprite = world.textureStore.hex_terrain_damp;
            }
            else if (hex.terrain == Hex.terrainType.SNOW)
            {
                terrainLayer.sprite = world.textureStore.hex_terrain_snow;
            }
            else if (hex.terrain == Hex.terrainType.TUNDRA)
            {
                terrainLayer.sprite = world.textureStore.hex_terrain_tundra;
            }
            else if (hex.terrain == Hex.terrainType.MOUNTAIN)
            {
                terrainLayer.sprite = world.textureStore.hex_terrain_mountain;
            }
            else
            {
                terrainLayer.sprite = world.textureStore.hex_base;
            }
            terrainLayer.color = colour;


            //setTerrainGraphicsSimplified();


            if (hex.location != null)
            {
                locLayer.enabled = true;
                locLayer.sprite = hex.location.getSprite();
            }
            //else if (hex.terrain == Hex.terrainType.SEA && hex.amphibPoint)
            //{
            //    locLayer.enabled = true;
            //    locLayer.sprite = world.textureStore.hex_loc_lighthouse;
            //}
            else
            {
                locLayer.enabled = false;
            }



            if (hex.location != null)
            {
                if (nameTag == null)
                {
                    nameTag = world.prefabStore.getNameTag(hex.location.name, Color.white);
                    nameTag.gameObject.transform.SetParent(transform);
                    nameTag.gameObject.transform.localPosition = new Vector3(0, -0.5f, -3.02f);
                    nameTag.gameObject.transform.localScale = new Vector3(0.015f, 0.015f, 1);//This is apparently their default scale
                }
                else
                {
                    nameTag.gameObject.SetActive(true);
                    nameTag.gameObject.transform.localScale = new Vector3(0.015f, 0.015f, 1);//This is apparently their default scale
                }
                //Name tag now must exist, so can be given a name and color
                if (hex.location.settlement != null)
                {
                    nameTag.words.text = hex.location.settlement.name;
                    if (hex.location.soc == null)
                    {
                        nameTag.words.color = new Color(0.7f, 0.7f, 0.5f);
                    }
                    else
                    {
                        float brightness = hex.location.soc.color.r;
                        brightness += hex.location.soc.color.b;
                        brightness += hex.location.soc.color.g;
                        if (brightness > 0.5f)
                        {
                            nameTag.words.color = hex.location.soc.color;
                        }
                        else
                        {
                            nameTag.words.color = new Color(0.7f, 0.7f, 0.5f);

                        }
                    }
                }
                else
                {
                    nameTag.words.text = hex.location.name;
                    nameTag.words.color = new Color(0.7f, 0.7f, 0.5f);
                }

                if (hex.location.soc != null)
                {

                    if (defenceTag == null)
                    {
                        defenceTag = world.prefabStore.getNameTag(hex.location.name, Color.white);
                        defenceTag.gameObject.transform.SetParent(transform);
                        defenceTag.gameObject.transform.localPosition = new Vector3(0, -0.75f, -3.02f);
                        defenceTag.gameObject.transform.localScale = new Vector3(0.015f, 0.015f, 1);//This is apparently their default scale
                    }
                    else
                    {
                        defenceTag.gameObject.SetActive(true);
                        defenceTag.gameObject.transform.localScale = new Vector3(0.015f, 0.015f, 1);//This is apparently their default scale
                    }
                    string strDef = "" + ((int)(hex.location.soc.currentMilitary + 0.5) + "/" + (int)(hex.location.soc.maxMilitary + 0.5));
                    if (hex.location.getMaxMilitaryDefence() > 0)
                    {
                        strDef += " +DEF " + (int)(hex.location.getMilitaryDefence()+0.5) + "/" + (int)(0.5+hex.location.getMaxMilitaryDefence());
                    }
                    defenceTag.words.text = strDef;
                }
                else
                {
                    if (defenceTag != null)
                    {
                        defenceTag.gameObject.SetActive(false);
                    }
                }

                /*

                if (hex.location.cloud == null)
                {
                    cloudLayer.sprite = null;
                }
                else
                {
                    cloudLayer.sprite = hex.location.cloud.getSprite(map);
                }
                */
            }

            /*
            if (hex.territoryOf != null && hex.territoryOf.soc != null && hex.territoryOf.soc.centralHex == hex)
            {
                if (societyNameTag == null)
                {
                    societyNameTag = world.prefabStore.getSocietyName(hex.territoryOf.soc.getName());
                    societyNameTag.gameObject.transform.SetParent(transform);
                    societyNameTag.gameObject.transform.localPosition = new Vector3(0, 0, -9.02f);
                }
                float scale = 1 + hex.territoryOf.soc.centralHexWidth;
                societyNameTag.gameObject.transform.localScale = new Vector3(scale, scale, 1);
            }
            else if (societyNameTag != null)
            {
                Destroy(societyNameTag.gameObject);
            }
            */


            if (hex.flora != null)
            {
                floraLayer.enabled = true;
                floraLayer.sprite = hex.flora.getSprite();
                floraLayer.color = new Color(floraDark, floraDark, floraDark);
            }
            else
            {
                floraLayer.enabled = false;
            }

            for (int i = 0; i < 6; i++)
            {

                bool shouldHaveDivider = true;
                bool shouldHaveBorder = true;
                if (hex.owner == null) { shouldHaveBorder = false; }//No need for borders at all

                bool right = false;
                int y = 0;
                if (i == 0) { right = false; y = 0; }
                else if (i == 1) { right = false; y = -1; }
                else if (i == 2) { right = true; y = -1; }
                else if (i == 3) { right = true; y = 0; }
                else if (i == 4) { right = true; y = 1; }
                else if (i == 5) { right = false; y = 1; }

                Hex neighbour = map.getNeighbourRelative(hex, y, right);
                if (neighbour == null) { shouldHaveBorder = false; shouldHaveDivider = false; }
                if (neighbour != null && neighbour.owner == hex.owner) { shouldHaveBorder = false; }
                if (neighbour != null && neighbour.territoryOf == hex.territoryOf) { shouldHaveDivider = false; }
                if (neighbour != null && neighbour.terrain == Hex.terrainType.SEA) { shouldHaveDivider = false; }
                if (hex.terrain == Hex.terrainType.SEA) { shouldHaveDivider = false; }
                if (neighbour != null && neighbour.terrain == Hex.terrainType.SEA)
                {
                    if (neighbour.owner == null) { shouldHaveBorder = false; }
                }


                //Temporarily disabling these altogether, to see how it looks
                shouldHaveDivider = false;

                if (shouldHaveBorder || shouldHaveDivider)
                {
                    if (borders[i] == null)
                    {
                        borders[i] = world.prefabStore.getHexEdgeSprite();
                        borders[i].transform.SetParent(transform);
                        borders[i].transform.localPosition = new Vector3(0, 0, -0.01f);
                        borders[i].transform.Rotate(new Vector3(0, 0, 60 * (i + 3)));
                    }
                    if (shouldHaveBorder && borders2[i] == null)
                    {
                        borders2[i] = world.prefabStore.getHexEdge2Sprite();
                        borders2[i].transform.SetParent(transform);
                        borders2[i].transform.localPosition = new Vector3(0, 0, -0.011f);
                        borders2[i].transform.Rotate(new Vector3(0, 0, 60 * (i + 3)));

                    }
                }

                if (shouldHaveBorder)
                {
                    borders[i].SetActive(true);
                    if (borders[i] != null) { borders[i].GetComponent<SpriteRenderer>().color = hex.owner.color; }

                    borders2[i].SetActive(true);
                    borders2[i].GetComponent<SpriteRenderer>().color = hex.owner.color2;
                }
                else if (!shouldHaveBorder && shouldHaveDivider)
                {
                    borders[i].SetActive(true);
                    if (borders[i] != null) { borders[i].GetComponent<SpriteRenderer>().color = pale; }

                    if (borders2[i] != null)
                    {
                        borders2[i].SetActive(false);
                    }
                }
                else
                {
                    if (borders[i] != null)
                    {
                        borders[i].SetActive(false);
                    }
                    if (borders2[i] != null)
                    {
                        borders2[i].SetActive(false);
                    }
                }
            }

            if (map != null && map.masker.applyMask(this.hex))
            {
                mask.color = map.masker.getColor(hex);
                mask.enabled = true;
            }
            else
            {
                mask.enabled = false;
            }


            //Reset all these, because they can spawn wrong somehow
            for (int i = 0; i < 6; i++)
            {
                if (borders[i] != null)
                {
                    borders[i].gameObject.transform.localScale = new Vector3(1, 1, 1);
                }
                if (borders2[i] != null)
                {
                    borders2[i].gameObject.transform.localScale = new Vector3(1, 1, 1);
                }
            }

            if (hex.location != null)
            {
                foreach (Property p in hex.location.properties)
                {
                    if (p.outer == null)
                    {
                        p.outer = map.world.prefabStore.getGraphicalProperty(map, p);
                    }
                }
            }
        }
    }
}
