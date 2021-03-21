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
		public class Variable
		{
			public string key;
			public string value;
		}

		[Serializable]
		public class Effect
		{
			public string command;
			public string argument;
		}

		[Serializable]
		public class Outcome
		{
			public string name;
			public string description;

			public List<Variable> environment;
			public List<Effect> effects;
		}

		public enum Type { LOCATION, UNIT, WORLD }
		public string type;

		public string id;
		public string modCredit;
		public string imgCredit;

		public string conditional;
		public double probability;

		public string name;
		public string description;

		public List<Outcome> outcomes;
    }
}
