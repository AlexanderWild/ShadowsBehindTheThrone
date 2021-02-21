using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code
{
    public class PopupLightbringer: MonoBehaviour
    {
        public Text title;
        public Text body;
        public Text locations;
        public Button bDismiss;
        public Button bDismissGoto;
        public Society society;
        public UIMaster ui;

        public void populate(Society society)
        {
            title.text = society.getName() + " has begun the Lightbringer Ritual. If this Ritual is successful, they will drive the Darkness back, causing you to suffer a defeat in this age.";
            body.text = "They will be employing " + society.map.overmind.lightbringerLocations.Count() + " locations which they believe hold the secret to driving back the dark. If they hold at least half of these " +
                " when the ritual ends, you will be defeated in this age, and humanity will survive. If you can remove enough them for the society's control (by destroying them or having them invaded) " +
                "or if you can corrupt them (placing a broken, enthralled noble in the location), the ritual will fail."
                + "\n\nThese locations are marked by flashing signs above them on the map";
            locations.text = "They currently hold " + society.map.overmind.computeLightbringerHeldLocations() + "/" + society.map.overmind.lightbringerLocations.Count + " locations.";
            this.society = society;
        }

        public void dismiss()
        {
            ui.removeBlocker(this.gameObject);
        }


        public void dismissGoto()
        {
            GraphicalMap.panTo(society.getCapital().hex.x, society.getCapital().hex.y);
            ui.removeBlocker(this.gameObject);
        }
    }
}
