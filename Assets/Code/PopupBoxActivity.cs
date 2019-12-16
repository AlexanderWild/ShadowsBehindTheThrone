using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code
{
    public class PopupBoxActivity : MonoBehaviour,PopupScrollable
    {
        public Text title;
        public Text body;
        public Text stats;
        public Text powers;
        //public Activity activity;
        public GameObject mover;
        public float targetY;
        public Person person;

        public void Update()
        {
            Vector3 loc = new Vector3(mover.transform.position.x, targetY, mover.transform.position.z);
            Vector3 delta = loc - mover.transform.position;
            if (delta.magnitude > 0.02f)
            {
                delta *= 0.075f;
            }
            mover.transform.Translate(delta);
        }

        /*
        public void setTo(Activity act,Person person)
        {
            if (act.useable(person) == false)
            {
                Color half = new Color(0.5f, 0.5f, 0.5f);
                title.color = half;
                body.color = half;
                stats.color = half;
                powers.color = half;
            }
            this.person = person;
            this.activity = act;
            title.text = act.displayName;
            body.text = act.desc;

            stats.text = "Cost: " + activity.cost
                + "\nBase Prestige Gain: " + activity.prestigeGainHost
                //+ "\nTime to set up: " + activity.prepTime
                + "\nMax Attendees: " + activity.nGuestsMax;

            powers.text = "";
            if (act.powers.Count > 0)
            {
                string pow = "Powers Enabled:";
                foreach (string s in act.powers)
                {
                    pow += "\n-" + s;
                }
                powers.text = pow;
            }
        }
        */

        public float ySize()
        {
            return 200;
        }

        public void setTargetY(float y)
        {
            targetY = y;
        }
        public void clicked(Map map)
        {
            /*
            if (activity.useable(person) == false)
            {
                map.world.prefabStore.popMsg("You cannot host this activity. Check if you can afford it and meet any conditions.");
                map.world.soundSource.failure();
                return;
            }
            int min = ((map.turn / 5) * 5);

            int date = -1;
            for (int i = 5; i < 20; i += 5)
            {
                if (this.person.activityDiary.ContainsKey(i + min)) { continue; }
                //Date is free, set it 
                date = i + min + 1;
                break;
            }

            if (date != -1)
            {
                this.person.ongoingActivity = new ActivityInstance(map, this.person, activity, this.person.domain.loc);
                this.person.cash -= activity.cost;
                this.person.activityDiary.Add(date, this.person.ongoingActivity);
                World.log("Activity on date: " + person.activityDiary[date]);
                this.person.ongoingActivity.guests.Add(this.person);
                this.person.ongoingActivity.date = date;

                string msg = "You will host " + activity.displayName + " on turn " + (date) + ". You may now begin to invite guests from either your social circle or entire society, based on those permitted by the activity.";
                map.world.prefabStore.popMsg(msg);

                List<Invite> rems = new List<Invite>();
                foreach (Invite inv in this.person.invites)
                {
                    if (this.person.activityDiary.ContainsKey(inv.activity.date))
                    {
                        rems.Add(inv);
                    }
                }
                foreach (Invite inv in rems) { this.person.invites.Remove(inv); }
            }
            else
            {
                map.world.prefabStore.popMsg("No free date could be found to host this activity on.");
                map.world.soundSource.failure();
            }
            */
            World.log("Acitvity box clicked");
        }

        public string getTitle()
        {
            return "DEFAULT TITLE";
        }

        public string getBody()
        {
            return "Hit select to pick this activity to host. You will then need to invite guests."
                + "\n\nYou gain prestige by hosting an activity, but also gain prestige from the prestige of your guests. Inviting high-prestige nobles will boost your own prestige."
                + "\n\nShadow is spread between nobles by the host to the guests, who will then spread it to their city's population."
                + "\n\nActivities can also lead to certain powers being made available to you, if you have sufficient power reserves at the time.";
        }

        public bool overwriteSidebar()
        {
            return true;
        }

        public bool selectable()
        {
            throw new NotImplementedException();
        }
    }
}
