using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code
{
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
			public int weight;
			public string description;

			public List<Variable> environment;
			public List<Effect> effects;
		}

		[Serializable]
		public class Choice
		{
			public string name;
			public List<Outcome> outcomes;
		}

		public enum Type { LOCATION, PERSON, UNIT, WORLD }
		[SerializeField]
		private string type;

		public string id;
		public string modCredit;
		public string imgCredit;

		public string conditional;
		public double probability;

		public string name;
		public string description;

		public List<Choice> choices;

		public Type getType()
		{
			return (Type)Enum.Parse(typeof(Type), type);
		}
    }
}
