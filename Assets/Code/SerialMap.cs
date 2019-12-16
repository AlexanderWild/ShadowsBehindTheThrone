using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code
{
    //[Serializable,HideInInspector]
    public class SerialMap<TKey, TValue>
    {
        public HashSet<TKey> existing = new HashSet<TKey>();
        public List<TKey> keys = new List<TKey>();
        public List<TValue> values = new List<TValue>();

        public SerialMap()
        {

        }

        public void put(TKey key, TValue value)
        {
            if (existing.Contains(key))
            {
                for (int i = 0; i < keys.Count; i++)
                {
                    if (keys[i].Equals(key))
                    {
                        values[i] = value;
                        return;
                    }
                }
                throw new Exception("Failed to find entry in serialMap");
            }
            else
            {
                keys.Add(key);
                values.Add(value);
                existing.Add(key);
            }
        }

        public TValue get(TKey key)
        {
            if (existing.Contains(key) == false) { return default(TValue); }
            for (int i = 0; i < keys.Count; i++)
            {
                if (keys[i].Equals(key))
                {
                    return values[i];
                }
            }
            return default(TValue);
        }

        public bool ContainsKey(TKey key)
        {
            return existing.Contains(key);
        }
    }
}
