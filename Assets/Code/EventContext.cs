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

		EventContext(Map m, Location l, Unit u)
		{
			map = m;
			_location = l;
			_unit = u;
		}

		public EventContext(Map m) : this(m, null, null) {}

		static EventContext WithLocation(Map m, Location l)
		{
			return new EventContext(m, l, null);
		}

		static EventContext WithUnit(Map m, Unit u)
		{
			return new EventContext(m, null, u);
		}
    }
}
