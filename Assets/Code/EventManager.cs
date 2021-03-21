using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;


namespace Assets.Code
{
    public class EventManager
    {
        class ActiveEvent
        {
            public EventData data;
            public EventParser.SyntaxNode conditionalRoot;

            public ActiveEvent(EventData d)
            {
                data = d;
                
                var tokens = EventParser.tokenize(data.conditional);
                conditionalRoot = EventParser.parse(tokens);
            }
        }

        static List<ActiveEvent> locationEvents = new List<ActiveEvent>();
        static List<ActiveEvent> unitEvents = new List<ActiveEvent>();
        static List<ActiveEvent> worldEvents = new List<ActiveEvent>();

        public static void load(string modPath)
        {
            // FIXME: temporary loading
            string data = File.ReadAllText(modPath + "/test.json");
            worldEvents.Add(new ActiveEvent(JsonUtility.FromJson<EventData>(data)));
        }

        public static void turnTick(Map m)
        {
            if (m.eventsDisabled) { return; }
            EventContext ctx = new EventContext(m);
            foreach (var we in worldEvents)
            {
                if (tryEvent(we, ctx))
                    break;
            }

            // TODO: loop over other event types.
        }

        static bool tryEvent(ActiveEvent e, EventContext ctx)
        {
            if (Eleven.random.NextDouble() >= e.data.probability)
                return false;
            if (!EventRuntime.evaluate(e.conditionalRoot, ctx))
                return false;

            World.Log(e.data.id + " event ocurring...");
            ctx.map.world.prefabStore.popEvent(e.data);

            return true;
        }
    }
}
