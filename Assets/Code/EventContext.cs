using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code
{
    public class EventContext
    {
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
    }
}
