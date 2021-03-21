using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code
{
    public class EventContext
    {
		public class State
		{
			public Dictionary<string, string> environment = new Dictionary<string, string>();
		}

        public Map map;

		private Location _location;
		public Location location
		{
			get
			{
				if (_location == null)
					throw new Exception("event location not in current context.");
				
				return _location;
			}
		}

		private Person _person;
		public Person person
		{
			get
			{
				if (_person == null)
					throw new Exception("event person not in current context.");

				return _person;
			}
		}

		private Unit _unit;
		public Unit unit
		{
			get
			{
				if (_unit == null)
					throw new Exception("event unit not in current context.");

				return _unit;
			}
		}

		EventContext(Map m, Location l, Person p, Unit u)
		{
			map = m;

			_location = l;
			_person = p;
			_unit = u;
		}

		public EventContext(Map m) : this(m, null, null, null) {}

		static EventContext withLocation(Map m, Location l)
		{
			return new EventContext(m, l, null, null);
		}

		static EventContext withPerson(Map m, Person p)
		{
			return new EventContext(m, null, p, null);
		}

		static EventContext withUnit(Map m, Unit u)
		{
			return new EventContext(m, null, null, u);
		}

		public void updateEnvironment(List<EventData.Variable> vs)
		{
			// Just parse enviroment expressions on the fly.
			// Only one event can occur every turn, so performance will be OK.
			foreach (var v in vs)
			{
				var tokens = EventParser.tokenize(v.value);
				var syntax = EventParser.parse(tokens);

				string res = EventRuntime.evaluateAny(syntax, this);
				writeEnvironment(v.key, res);
			}
		}

		public string readEnvironment(string key)
		{
			if (!map.eventState.environment.ContainsKey(key))
				return "";
			else
				return map.eventState.environment[key];
		}

		public void writeEnvironment(string key, string value)
		{
			map.eventState.environment[key] = value;
		}
    }
}
