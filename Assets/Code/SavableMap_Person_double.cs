using UnityEngine;
using System.Collections.Generic;
using System;

namespace Assets.Code
{
    public class SavableMap_Person_Double
    {
        public List<Person> keys = new List<Person>();
        public List<double> values = new List<double>();

        public double lookup(Person key){
            for (int i = 0; i < keys.Count; i++)
            {
                if (keys[i].Equals(key))
                {
                    return values[i];
                }
            }
            return 0;
        }

        internal void Clear()
        {
            keys.Clear();
            values.Clear();
        }

        public void Add(Person key, double value)
        {
            add(key, value);
        }
        public void add(Person key, double value)
        {
            keys.Add(key);
            values.Add(value);
        }

        public bool ContainsKey(Person key)
        {
            return keys.Contains(key);
        }

        internal void set(Person key, double value)
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