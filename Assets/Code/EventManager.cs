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

        public static Dictionary<string, Sprite> loadedModImages = new Dictionary<string, Sprite>();

        public class ActiveEvent
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

        public static Dictionary<string, ActiveEvent> events = new Dictionary<string, ActiveEvent>();

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
            foreach (var kv in events)
            {
                var e = kv.Value;

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
                    World.log("Found a narr event to trigger");
                    m.world.prefabStore.popEvent(e.data, ctx);
                    break;
                }
            }
        }

        public static EventData.Outcome chooseOutcome(EventData.Choice c, EventContext ctx)
        {
            double sum = 0;
            foreach (var o in c.outcomes)
            {
                sum += o.weight;
            }

            double rand = Eleven.random.NextDouble()*sum;
            foreach (var o in c.outcomes)
            {
                rand -= o.weight;
                if (rand <= 0)
                {
                    ctx.updateEnvironment(o.environment);
                    foreach (var e in o.effects)
                        EventRuntime.evaluate(e, ctx);

                    return o;
                }
            }

            throw new Exception("unable to choose event outcome. ");
        }

        static void loadMod(string mod)
        {
            foreach (var path in Directory.EnumerateFiles(mod, "*.json"))
            {
                try
                {
                    string data = File.ReadAllText(path);
                    EventData ev = JsonUtility.FromJson<EventData>(data);

                    if (!events.ContainsKey(ev.id))
                        events.Add(ev.id, new ActiveEvent(ev));
                    else
                        World.Log("Ignoring duplicate event " + ev.id);
                }
                catch (Exception e)
                {
                    string msg = "[" + path + "] could not load event: " + e.Message;
                    World.Log(msg);
                    World.self.ui.uiMainMenu.errorMessage.text = msg;
                    //World.self.prefabStore.popMsg("[" + path + "] could not load event: " + e.Message);
                }
            }
            foreach (var imgpath in Directory.EnumerateFiles(mod, "*.jpg"))
            {
                try
                {
                    World.log("Narr Event img found " + imgpath);
                    string[] split = imgpath.Split(World.separator[0]);
                    string imgName = split[split.Length - 2] + "." + split[split.Length - 1];
                    //Sprite loaded = TextureStore.LoadPNG(imgpath);
                    LoadImage(imgpath, imgName);
                    World.log("Narr Event Saving loaded image " + imgName);
                }
                catch (Exception e)
                {
                    string msg = "[" + imgpath + "] could not load img: " + e.Message;
                    World.Log(msg);
                    World.self.ui.uiMainMenu.errorMessage.text = msg;
                    //World.self.prefabStore.popMsg("[" + path + "] could not load event: " + e.Message);
                }
            }
        }

        public static void LoadImage(string filePath,string imgName)
        {

            Texture2D tex = null;
            byte[] fileData;

            Sprite response = null;
            if (File.Exists(filePath))
            {
                fileData = File.ReadAllBytes(filePath);
                tex = new Texture2D(2, 2);
                tex.LoadImage(fileData);
                response = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
                World.log("Narr event " + response.rect.width + ", " + response.rect.height);
            }
            else
            {
                World.log("Unable to find: " + filePath);
            }

            loadedModImages.Add(imgName, response);
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
