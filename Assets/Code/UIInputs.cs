using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Code
{
    public class UIInputs : MonoBehaviour
    {
        public World world;
        bool leftClickDown = false;
        bool rightClickDown = false;
        public float scrollSpeed = 0.4f;
        public float socScrollSpeed = 0.3f;
        public float lastAutoturn = 0;
        public long scrollwheelTracking = 0;

        public bool disable = false;

        public void Update()
        {
            if (world.map == null) { return; }
            if (disable) { return; }
            
            mouseClicks();
            scrollKeys();
            scaling();
            hotkeys();
            automatic();
        }

        public void automatic()
        {
            if (world.map.automatic == false) { return; }

            float dT = Time.time - lastAutoturn;
            if ((dT > 0.5 && Input.GetKey(KeyCode.LeftControl)) || Input.GetKey(KeyCode.LeftAlt)){
                world.bEndTurn();
                lastAutoturn = Time.time;
            }
        }

        public void hotkeys()
        {
            if (Input.GetKeyDown(KeyCode.Backspace) && Input.GetKey(KeyCode.LeftShift))
            {
                world.ui.uiMidTop.cheatField.gameObject.SetActive(!world.ui.uiMidTop.cheatField.gameObject.activeSelf);
            }

            if (world.ui.blocker != null) { return; }


            if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.Tab))
            {
            }
            else if (Input.GetKeyDown(KeyCode.LeftControl))
            {
            }
            else if (Input.GetKeyDown(KeyCode.Q))
            {
                //try
                //{
                //    Property.addProperty(World.staticMap, GraphicalMap.selectedHex.location, "Blizzard");
                //}
                //catch  { }
            }
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (world.ui.state == UIMaster.uiState.VOTING)
                {
                    world.ui.uiVoting.bDismiss();
                }
                world.ui.setToMainMenu();
            }

            if (world.ui.state == UIMaster.uiState.WORLD || world.ui.state == UIMaster.uiState.BACKGROUND)
            {
                if (UIKeybinds.getKey(UIKeybinds.Action.MASK_CLEAR_MASKS))
                {
                    world.map.masker.mask = MapMaskManager.maskType.NONE;
                    GraphicalMap.checkData();
                }
                else if (UIKeybinds.getKey(UIKeybinds.Action.MASK_NATION))
                {
                    if (world.map.masker.mask == MapMaskManager.maskType.NATION)
                    {
                        world.map.masker.mask = MapMaskManager.maskType.NONE;
                    }
                    else
                    {
                        World.log("Set to nation");
                        world.map.masker.mask = MapMaskManager.maskType.NATION;
                    }
                    GraphicalMap.checkData();
                }
                else if (UIKeybinds.getKey(UIKeybinds.Action.MASK_PROVINCE))
                {
                    if (world.map.masker.mask == MapMaskManager.maskType.PROVINCE)
                    {
                        world.map.masker.mask = MapMaskManager.maskType.NONE;
                    }
                    else
                    {
                        world.map.masker.mask = MapMaskManager.maskType.PROVINCE;
                    }
                    GraphicalMap.checkData();
                }
                else if (UIKeybinds.getKey(UIKeybinds.Action.MASK_VOTE_EFFECT))
                {
                    if (world.map.masker.mask == MapMaskManager.maskType.VOTE_EFFECT)
                    {
                        world.map.masker.mask = MapMaskManager.maskType.NONE;
                    }
                    else
                    {
                        world.map.masker.mask = MapMaskManager.maskType.VOTE_EFFECT;
                    }
                    GraphicalMap.checkData();
                }
                else if (UIKeybinds.getKey(UIKeybinds.Action.MASK_INFILTRATION))
                {
                    if (world.map.masker.mask == MapMaskManager.maskType.INFILTRATION)
                    {
                        world.map.masker.mask = MapMaskManager.maskType.NONE;
                    }
                    else
                    {
                        world.map.masker.mask = MapMaskManager.maskType.INFILTRATION;
                    }
                    GraphicalMap.checkData();
                }
                else if (UIKeybinds.getKey(UIKeybinds.Action.MASK_MY_LIKING))
                {
                    if (world.map.masker.mask == MapMaskManager.maskType.LIKING_ME)
                    {
                        world.map.masker.mask = MapMaskManager.maskType.NONE;
                    }
                    else
                    {
                        world.map.masker.mask = MapMaskManager.maskType.LIKING_ME;
                    }
                    GraphicalMap.checkData();
                }
                else if (UIKeybinds.getKey(UIKeybinds.Action.MASK_THEIR_LIKING))
                {
                    if (world.map.masker.mask == MapMaskManager.maskType.LIKING_THEM)
                    {
                        world.map.masker.mask = MapMaskManager.maskType.NONE;
                    }
                    else
                    {
                        world.map.masker.mask = MapMaskManager.maskType.LIKING_THEM;
                    }
                    GraphicalMap.checkData();
                }
                else if (UIKeybinds.getKey(UIKeybinds.Action.MASK_AWARENESS))
                {
                    if (world.map.masker.mask == MapMaskManager.maskType.AWARENESS)
                    {
                        world.map.masker.mask = MapMaskManager.maskType.NONE;
                    }
                    else
                    {
                        world.map.masker.mask = MapMaskManager.maskType.AWARENESS;
                    }
                    GraphicalMap.checkData();
                }
                else if (UIKeybinds.getKey(UIKeybinds.Action.MASK_MY_SUSPICION))
                {
                    if (world.map.masker.mask == MapMaskManager.maskType.SUSPICION)
                    {
                        world.map.masker.mask = MapMaskManager.maskType.NONE;
                    }
                    else
                    {
                        world.map.masker.mask = MapMaskManager.maskType.SUSPICION;
                    }
                    GraphicalMap.checkData();
                }
                else if (UIKeybinds.getKey(UIKeybinds.Action.MASK_THEIR_SUSPICION))
                {
                    if (world.map.masker.mask == MapMaskManager.maskType.SUSPICION_FROM)
                    {
                        world.map.masker.mask = MapMaskManager.maskType.NONE;
                    }
                    else
                    {
                        world.map.masker.mask = MapMaskManager.maskType.SUSPICION_FROM;
                    }
                    GraphicalMap.checkData();
                }
                else if (Input.GetKeyDown(KeyCode.End))
                {
                    if (world.map.masker.mask == MapMaskManager.maskType.TESTING)
                    {
                        world.map.masker.mask = MapMaskManager.maskType.NONE;
                    }
                    else
                    {
                        world.map.masker.mask = MapMaskManager.maskType.TESTING;
                    }
                    GraphicalMap.checkData();
                }
            }

            if (world.ui.state == UIMaster.uiState.WORLD && world.ui.blocker == null)
            {
                if (Input.GetKeyDown(KeyCode.PageDown))
                {
                    world.bEndTurn();
                }
            }
        }

        public void scaling()
        {
            //Anyone who wants to read can just pull this value and see if it has changed since they last polled
            if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                scrollwheelTracking += 1;
                //World.log("mouse wheel pos");
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                scrollwheelTracking -= 1;
                //World.log("mouse wheel neg");
            }
            
            if (EventSystem.current.IsPointerOverGameObject())
            {
                if (EventSystem.current.currentSelectedGameObject != null)
                {
                    return;
                }
            }

            if (world.ui.state == UIMaster.uiState.WORLD && world.ui.blocker == null)
            {
                if (UIKeybinds.getKey(UIKeybinds.Action.ZOOM_IN) || Input.GetAxis("Mouse ScrollWheel") > 0)
                {
                    GraphicalMap.lastMapChange += 1;
                    GraphicalMap.scale = GraphicalMap.scale * 1.1f;
                    if (GraphicalMap.scale > GraphicalMap.maxScale) { GraphicalMap.scale = GraphicalMap.maxScale; }
                    GraphicalMap.checkData();
                }
                else if (UIKeybinds.getKey(UIKeybinds.Action.ZOOM_OUT) || Input.GetAxis("Mouse ScrollWheel") < 0)
                {
                    GraphicalMap.lastMapChange += 1;
                    GraphicalMap.scale = GraphicalMap.scale / 1.1f;
                    if (GraphicalMap.scale < GraphicalMap.minScale) { GraphicalMap.scale = GraphicalMap.minScale; }
                    GraphicalMap.checkData();
                }
            }
            if (world.ui.state == UIMaster.uiState.SOCIETY && world.ui.blocker == null)
            {
                if (UIKeybinds.getKey(UIKeybinds.Action.ZOOM_IN) || Input.GetAxis("Mouse ScrollWheel") > 0)
                {
                    if (GraphicalSociety.focus != null)
                    {
                        GraphicalSociety.zoom += 0.1f;
                        GraphicalSociety.refresh(GraphicalSociety.focus);
                    }
                }
                else if (UIKeybinds.getKey(UIKeybinds.Action.ZOOM_OUT) || Input.GetAxis("Mouse ScrollWheel") < 0)
                {
                    if (GraphicalSociety.focus != null && GraphicalSociety.zoom > 1)
                    {
                        GraphicalSociety.zoom -= 0.1f;
                        GraphicalSociety.refresh(GraphicalSociety.focus);
                    }
                }
            }
        }
        public void scrollKeys()
        {

            if (world.map.param.option_edgeScroll == 1)
            {

                if (Input.mousePosition.y >= Screen.height - 5)
                {
                    //GraphicalMap.panStepsToTake = 0;
                    GraphicalMap.lastMapChange += 1;
                    GraphicalMap.y -= scrollSpeed;
                    if (GraphicalMap.y < 0) { GraphicalMap.y = 0; }
                }
                else if (Input.mousePosition.y <= 2)
                {
                    //GraphicalMap.panStepsToTake = 0;
                    GraphicalMap.lastMapChange += 1;
                    GraphicalMap.y += scrollSpeed;
                    if (GraphicalMap.y > world.map.sy) { GraphicalMap.y = world.map.sy; }
                }
                if ( Input.mousePosition.x <= 0)
                {
                    //GraphicalMap.panStepsToTake = 0;
                    GraphicalMap.lastMapChange += 1;
                    GraphicalMap.x += scrollSpeed;
                    if (GraphicalMap.x > world.map.sx) { GraphicalMap.x = world.map.sx; }
                }
                else if (Input.mousePosition.x >= Screen.width - 5)
                {
                    //GraphicalMap.panStepsToTake = 0;
                    GraphicalMap.lastMapChange += 1;
                    GraphicalMap.x -= scrollSpeed;
                    if (GraphicalMap.x < 0) { GraphicalMap.x = 0; }
                }
            }
            if (UIKeybinds.getKey(UIKeybinds.Action.PAN_UP, true))
            {
                if (world.ui.state == UIMaster.uiState.SOCIETY) {
                    if (GraphicalSociety.focus != null)
                    {
                        GraphicalSociety.offY -= socScrollSpeed;
                        GraphicalSociety.refreshOffset();
                    }
                }
                else
                {
                    //GraphicalMap.panStepsToTake = 0;
                    GraphicalMap.lastMapChange += 1;
                    GraphicalMap.y -= scrollSpeed;
                    if (GraphicalMap.y < 0) { GraphicalMap.y = 0; }
                }
            }
            else if (UIKeybinds.getKey(UIKeybinds.Action.PAN_DOWN, true))
            {
                if (world.ui.state == UIMaster.uiState.SOCIETY)
                {
                    if (GraphicalSociety.focus != null)
                    {
                        GraphicalSociety.offY += socScrollSpeed;
                        GraphicalSociety.refreshOffset();
                    }
                }
                else
                {
                    //GraphicalMap.panStepsToTake = 0;
                    GraphicalMap.lastMapChange += 1;
                    GraphicalMap.y += scrollSpeed;
                    if (GraphicalMap.y > world.map.sy) { GraphicalMap.y = world.map.sy; }
                }
            }
            if (UIKeybinds.getKey(UIKeybinds.Action.PAN_LEFT, true))
            {
                if (world.ui.state == UIMaster.uiState.SOCIETY)
                {
                    if (GraphicalSociety.focus != null)
                    {
                        GraphicalSociety.offX += socScrollSpeed;
                        GraphicalSociety.refreshOffset();
                    }
                }
                else
                {
                    //GraphicalMap.panStepsToTake = 0;
                    GraphicalMap.lastMapChange += 1;
                    GraphicalMap.x += scrollSpeed;
                    if (GraphicalMap.x > world.map.sx) { GraphicalMap.x = world.map.sx; }


                }
            }
            else if (UIKeybinds.getKey(UIKeybinds.Action.PAN_RIGHT, true))
            {
                if (world.ui.state == UIMaster.uiState.SOCIETY)
                {
                    if (GraphicalSociety.focus != null)
                    {
                        GraphicalSociety.offX -= socScrollSpeed;
                        GraphicalSociety.refreshOffset();
                    }
                }
                else
                {
                    //GraphicalMap.panStepsToTake = 0;
                    GraphicalMap.lastMapChange += 1;
                    GraphicalMap.x -= scrollSpeed;
                    if (GraphicalMap.x < 0) { GraphicalMap.x = 0; }


                }
            }

        }

        public void mouseClicks()
        {
            if (world.ui.blocker != null) { return; }//Blocker on screen takes precedence

            if (Input.GetMouseButtonDown(0) && !leftClickDown) { leftClick(); }
            else if (Input.GetMouseButtonDown(1) && !rightClickDown) { rightClick(); }
            leftClickDown = Input.GetMouseButtonDown(0);
            rightClickDown = Input.GetMouseButtonDown(1);
        }

        public void leftClick()
        {
            if (world.ui.state == UIMaster.uiState.WORLD)
            {
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    return;
                }
                clickOnHex();
            }
            if (world.ui.state == UIMaster.uiState.SOCIETY)
            {
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    return;
                }
                clickOnSociety();
            }
            world.ui.checkData();
        }

        public void clickOnSociety()
        {
            Vector3 pos = Input.mousePosition;
            double dist = 0;
            GraphicalSlot best = null;
            foreach (GraphicalSlot slot in GraphicalSociety.loadedSlots)
            {
                Vector3 slotLoc = world.outerCamera.WorldToScreenPoint(slot.transform.position);
                double d = (slotLoc - pos).sqrMagnitude;
                if (best == null || d < dist)
                {
                    dist = d;
                    best = slot;
                }
            }
            //GraphicalSociety.focus = best.inner;
            GraphicalSociety.refresh(best.inner);
        }

        public void rightClick()
        {
            if (world.ui.state == UIMaster.uiState.WORLD)
            {
                rightClickOnHex();
            }
            world.ui.checkData();

        }

        public void clickOnHex()
        {
            Hex clickedHex = GraphicalMap.getHexUnderMouse(Input.mousePosition).hex;



            if (Input.GetKey(KeyCode.LeftControl))
            {
                GraphicalMap.selectedSelectable = null;
                GraphicalMap.selectedHex = clickedHex;
                world.audioStore.playClick();
                return;
            }
            world.ui.checkData();


            if (clickedHex.location != null) {
                List<object> selectables = new List<object>();
                foreach (object o in clickedHex.location.units)
                {
                    selectables.Add(o);
                }
                foreach (object o in clickedHex.location.properties)
                {
                    selectables.Add(o);
                }
                int index = -1;
                if (GraphicalMap.selectedSelectable != null)
                {
                    index = selectables.IndexOf(GraphicalMap.selectedSelectable);
                }
                if (index == -1)//Nothing from this loc selected yet
                {
                    index = 0;
                }
                else
                {
                    index += 1;
                }
                //See if we're out of selectables (including if there were none)
                if (index >= selectables.Count)
                {
                    GraphicalMap.selectedSelectable = null;
                    GraphicalMap.selectedHex = clickedHex;
                    world.audioStore.playClick();
                    world.ui.checkData();
                    return;
                }
                else
                {
                    GraphicalMap.selectedSelectable = selectables[index];
                    GraphicalMap.selectedHex = clickedHex;
                    world.audioStore.playClick();
                    world.ui.checkData();
                    return;

                }
                
            }

            GraphicalMap.selectedSelectable = null;
            GraphicalMap.selectedHex = clickedHex;
            world.audioStore.playClick();
            world.ui.checkData();
        }

        public void rightClickOnHex()
        {

            Hex clickedHex = GraphicalMap.getHexUnderMouse(Input.mousePosition).hex;
            if (clickedHex.location != null)
            {
                if (GraphicalMap.selectedSelectable != null && GraphicalMap.selectedSelectable is Unit && ((Unit)GraphicalMap.selectedSelectable).isEnthralled() && (!((Unit)GraphicalMap.selectedSelectable).automated))
                {
                    Unit u = (Unit)GraphicalMap.selectedSelectable;
                    if (u.location.getNeighbours().Contains(clickedHex.location)){
                        if (u.movesTaken == 0)
                        {
                            u.location.map.adjacentMoveTo(u,clickedHex.location);
                            u.movesTaken += 1;
                            u.location.map.world.audioStore.playClickSelect();
                            u.task = null;
                        }
                    }
                }
            }
        }
    }
}
