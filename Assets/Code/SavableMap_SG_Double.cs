using UnityEngine;

using System.Collections.Generic;
using System;

namespace Assets.Code
{
    public class SavableMap_SG_Double
    {
        public List<SocialGroup> keys = new List<SocialGroup>();
        public List<double> values = new List<double>();

        public double lookup(SocialGroup key){
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

        public void Add(SocialGroup key, double value)
        {
            add(key, value);
        }
        public void add(SocialGroup key, double value)
        {
            keys.Add(key);
            values.Add(value);
        }

        public bool ContainsKey(SocialGroup key)
        {
            return keys.Contains(key);
        }

        internal void set(SocialGroup key, double value)
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