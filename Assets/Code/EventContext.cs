using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code
{
    public struct EventContext
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

		public Society society
		{
			get
			{
				// if (_location != null && _location.soc is Society)
				// 	return (_location.soc as Society);
				if (_person != null)
					return _person.society;
				else if (_unit != null && _unit.society is Society)
					return (_unit.society as Society);

				throw new Exception("event society not in current context.");
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

		public static EventContext withLocation(Map m, Location l)
		{
			return new EventContext(m, l, null, null);
		}

		public static EventContext withPerson(Map m, Person p)
		{
			return new EventContext(m, null, p, null);
		}

		public static EventContext withUnit(Map m, Unit u)
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
				writeEnvironment(v.key, res, v.local);
			}
		}

		public string readEnvironment(string key)
		{
			State localState = getLocalState();
			if (localState != null && localState.environment.ContainsKey(key))
				return localState.environment[key];

			if (!map.eventState.environment.ContainsKey(key))
				return "";
			else
				return map.eventState.environment[key];
		}

		public void writeEnvironment(string key, string value, bool local)
		{
			State state = map.eventState;
			if (local)
			{
				state = getLocalState();
				if (state == null)
					throw new Exception("local event state is not available in current context.");
			}

			if (value == "")
				state.environment.Remove(key);
			else
				state.environment[key] = value;
		}

		State getLocalState()
		{
			if (_location != null)
				return _location.eventState;
			if (_person != null)
				return _person.eventState;
			if (_unit != null)
				return _unit.eventState;

			return null;
		}
    }
}
