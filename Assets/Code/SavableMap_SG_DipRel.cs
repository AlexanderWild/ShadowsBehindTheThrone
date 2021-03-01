using UnityEngine;

using System.Collections.Generic;
using System;

namespace Assets.Code
{
    public class SavableMap_SG_DipRel
    {
        public List<double> keys = new List<double>();
        public List<DipRel> values = new List<DipRel>();

        public DipRel lookup(SocialGroup key){
            for (int i = 0; i < keys.Count; i++)
            {
                if (keys[i].Equals(key.randID))
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

        public void Add(SocialGroup key, DipRel value)
        {
            add(key, value);
        }
        public void add(SocialGroup key, DipRel value)
        {
            keys.Add(key.randID);
            values.Add(value);
        }

        public bool ContainsKey(SocialGroup key)
        {
            return keys.Contains(key.randID);
        }

        internal void set(SocialGroup key, DipRel value)
        {
            for (int i = 0; i < keys.Count; i++)
            {
                if (keys[i].Equals(key.randID))
                {
                    values[i] = value;
                    return;
                }
            }
            add(key, value);
        }
    }
}