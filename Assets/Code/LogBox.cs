using UnityEngine;


namespace Assets.Code
{
    public class LogBox
    {
        public string path;
        public LogBox(string fileName)
        {
            path = fileName;
        }

        public LogBox(Person p)
        {
            path = "logging" + World.separator + "people" + World.separator + p.firstName + ".log";
            System.IO.File.WriteAllLines(path,new string[]{ "Log for " + p.firstName});
        }
        public LogBox(Society p)
        {
            path = "logging" + World.separator + "societies" + World.separator + p.getName() + ".log";
            System.IO.File.WriteAllLines(path, new string[] { "Log for " + p.getName() });
        }

        public void takeLine(string line)
        {
            /*
            //Don't use this as a field, or it'll get maximally shrivelled when serialising
            //Just don't log so much you faithless wretch
            System.IO.StreamWriter file;
            file = new System.IO.StreamWriter(path, true);
            file.WriteLine(line);
            file.Close();
            */
        }

    }
}