using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code
{
	[Serializable]
    public class EventData
    {
		[Serializable]
		public class Effect
		{
			public enum Type
			{
				ADD_POWER,
				WRITE
			}

			public string type;
			public string value;
			
			//
		}

		[Serializable]
		public class Outcome
		{
			public string name;
			public string description;

			public List<Effect> effects;

			//
		}

		public enum Type { LOCATION, UNIT, WORLD }
		public string type;
	
        public string conditional;
		public double probability;

		public string id;
		public string modCredit;
		public string imgCredit;

		public List<Outcome> outcomes;

		//
    }
}
