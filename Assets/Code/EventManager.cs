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

        public static EventData.Outcome chooseOutcome(EventData.Choice c, EventContext ctx)
        {
            int sum = 0;
            foreach (var o in c.outcomes)
            {
                if (o.weight == 0)
                    throw new Exception("found unweighted outcome in event choice.");
                else
                    sum += o.weight;
            }

            int rand = Eleven.random.Next(0, sum);
            foreach (var o in c.outcomes)
            {
                if (rand >= o.weight)
                {
                    rand -= o.weight;
                    continue;
                }

                ctx.updateEnvironment(o.environment);
                foreach (var e in o.effects)
                    EventRuntime.evaluate(e, ctx);

                return o;
            }

            throw new Exception("unable to choose event outcome.");
        }

        static bool tryEvent(ActiveEvent e, EventContext ctx)
        {
            if (Eleven.random.NextDouble() >= e.data.probability)
                return false;
            if (!EventRuntime.evaluate(e.conditionalRoot, ctx))
                return false;

            ctx.map.world.prefabStore.popEvent(e.data, ctx);
            return true;
        }
    }
}
