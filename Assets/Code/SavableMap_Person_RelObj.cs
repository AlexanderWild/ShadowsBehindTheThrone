using UnityEngine;

using System.Collections.Generic;
using System;

namespace Assets.Code
{
    public class SavableMap_Person_RelObj
    {
        public List<Person> keys = new List<Person>();
        public List<RelObj> values = new List<RelObj>();

        public RelObj lookup(Person key){
            for (int i = 0; i < keys.Count; i++)
            {
                if (keys[i].Equals(key))
                {
                    return values[i];
                }
            }
            return null;
        }

        internal void Clear()
        {
            keys.Clear();
            values.Clear();
        }

        public void Add(Person key, RelObj value)
        {
            add(key, value);
        }
        public void add(Person key, RelObj value)
        {
            keys.Add(key);
            values.Add(value);
        }

        public bool ContainsKey(Person key)
        {
            return keys.Contains(key);
        }

        internal void set(Person key, RelObj value)
        {
            for (int i = 0; i < keys.Count; i++)
            {
                if (keys[i].Equals(key))
                {
                    values[i] = value;
                    return;
                }
            }
            add(key, value);
        }
    }
}