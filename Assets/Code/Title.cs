﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Assets.Code{
    public abstract class Title
    {
        public Society society;
        public Person heldBy;
        public int turnLastAssigned = 0;

        public string nameM = "DEFAULTTITLE_M";
        public string nameF = "DEFAULTTITLE_F";

        public Title(Society soc)
        {
            this.society = soc;
        }

        public virtual void turnTick() { }
        public abstract string getName();
        public abstract double getPrestige();

        public virtual List<Person> getEligibleHolders(Society soc)
        {
            return soc.people;
        }
    }
}
