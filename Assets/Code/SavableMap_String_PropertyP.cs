using UnityEngine;

using System.Collections.Generic;
using System;

namespace Assets.Code
{
    public class SavableMap_String_PropertyP
    {
        public List<string> keys = new List<string>();
        public List<Property_Prototype> values = new List<Property_Prototype>();

        public Property_Prototype lookup(string key){
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

        public void Add(string key, Property_Prototype value)
        {
            add(key, value);
        }
        public void add(string key, Property_Prototype value)
        {
            keys.Add(key);
            values.Add(value);
        }

        public bool ContainsKey(string key)
        {
            return keys.Contains(key);
        }

        internal void set(string key, Property_Prototype value)
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