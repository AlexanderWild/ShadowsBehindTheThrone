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
        static List<ActiveEvent> personEvents = new List<ActiveEvent>();
        static List<ActiveEvent> unitEvents = new List<ActiveEvent>();
        static List<ActiveEvent> worldEvents = new List<ActiveEvent>();

        public static void load(string modPath)
        {
            foreach (var mod in Directory.EnumerateDirectories(modPath))
            {
                foreach (var path in Directory.EnumerateFiles(mod, "*.json"))
                {
                    try
                    {
                        string data = File.ReadAllText(path);
                        EventData ev = JsonUtility.FromJson<EventData>(data);

                        ActiveEvent res = new ActiveEvent(ev);
                        switch (ev.getType())
                        {
                            case EventData.Type.LOCATION: locationEvents.Add(res); break;
                            case EventData.Type.PERSON:   personEvents.Add(res);   break;
                            case EventData.Type.UNIT:     unitEvents.Add(res);     break;
                            case EventData.Type.WORLD:    worldEvents.Add(res);    break;
                        }
                    }
                    catch (Exception e)
                    {
                        World.Log("[" + path + "] could not load event: " + e.Message);
                    }
                }
            }
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
