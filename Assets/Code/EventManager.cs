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
            public EventData.Type type;

            public EventParser.SyntaxNode conditionalRoot;

            public ActiveEvent(EventData d)
            {
                data = d;
                type = (EventData.Type)Enum.Parse(typeof(EventData.Type), data.type);
                
                var tokens = EventParser.tokenize(data.conditional);
                conditionalRoot = EventParser.parse(tokens);
            }

            public bool willTrigger(EventContext ctx)
            {
                if (Eleven.random.NextDouble() >= data.probability)
                    return false;
                if (!EventRuntime.evaluate(conditionalRoot, ctx))
                    return false;

                return true;
            }
        }

        static List<ActiveEvent> events = new List<ActiveEvent>();

        public static void load(string modPath)
        {
            foreach (var mod in Directory.EnumerateDirectories(modPath))
                loadMod(mod);

            World.log("NarrEvents loaded " + events.Count);

            // TODO: load user mod folder.
        }

        public static void turnTick(Map m)
        {
            World.Log("narrEvents to check " + events.Count);
            foreach (var e in events)
            {
                EventContext? nctx = null;
                switch (e.type)
                {
                    case EventData.Type.LOCATION:
                        nctx = chooseContext(e, nextLocation(m));
                        break;
                    case EventData.Type.PERSON:
                        nctx = chooseContext(e, nextPerson(m));
                        break;
                    case EventData.Type.UNIT:
                        nctx = chooseContext(e, nextUnit(m));
                        break;
                    case EventData.Type.WORLD:
                        nctx = chooseContext(e, nextWorld(m));
                        break;
                }

                if (nctx is EventContext ctx)
                {
                    m.world.prefabStore.popEvent(e.data, ctx);
                    break;
                }
            }
        }

        public static EventData.Outcome chooseOutcome(EventData.Choice c, EventContext ctx)
        {
            int sum = 0;
            foreach (var o in c.outcomes)
            {
                sum += o.weight;
            }

            int rand = Eleven.random.Next(sum);
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

        static void loadMod(string mod)
        {
            foreach (var path in Directory.EnumerateFiles(mod, "*.json"))
            {
                try
                {
                    string data = File.ReadAllText(path);
                    EventData ev = JsonUtility.FromJson<EventData>(data);

                    events.Add(new ActiveEvent(ev));
                }
                catch (Exception e)
                {
                    World.Log("[" + path + "] could not load event: " + e.Message);
                    //World.self.prefabStore.popMsg("[" + path + "] could not load event: " + e.Message);
                }
            }
        }

        static EventContext? chooseContext(ActiveEvent e, IEnumerable<EventContext> next)
        {
            EventContext? res = null;
            int c = 0;

            foreach (var ctx in next)
            {
                if (!e.willTrigger(ctx))
                    continue;

                if (Eleven.random.Next(c) == 0)
                    res = ctx;

                c += 1;
            }

            return res;
        }

        static IEnumerable<EventContext> nextLocation(Map m)
        {
            foreach (var l in m.majorLocations)
                yield return EventContext.withLocation(m, l);
        }

        static IEnumerable<EventContext> nextPerson(Map m)
        {
            foreach (var p in m.persons)
            {
                if (p != null)
                {
                    yield return EventContext.withPerson(m, p);
                }
            }
        }

        static IEnumerable<EventContext> nextUnit(Map m)
        {
            foreach (var u in m.units)
            {
                if (u != null && !u.isMilitary && u.society is Society)
                    yield return EventContext.withUnit(m, u);
            }
        }

        static IEnumerable<EventContext> nextWorld(Map m)
        {
            yield return new EventContext(m);
        }
    }
}
