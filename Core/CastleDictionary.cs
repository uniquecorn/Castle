using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Castle
{
    [System.Serializable]
    public class CastleDictionary<K, V> : Dictionary<K, V>, ISerializationCallbackReceiver
    {
        [HideInInspector] public List<K> m_Keys = new List<K>();
        [SerializeReference, HideInInspector] public List<V> m_Values = new List<V>();

        public CastleDictionary() : base()
        {
        }

        public CastleDictionary(int capacity) : base(capacity)
        {
        }

        public virtual void OnBeforeSerialize()
        {
            m_Keys.Clear();
            m_Values.Clear();
            m_Keys.Capacity = m_Values.Capacity = Count;
            foreach (var kvp in this)
            {
                m_Keys.Add(kvp.Key);
                m_Values.Add(kvp.Value);
            }
        }

        public virtual void OnAfterDeserialize()
        {
            Clear();
            EnsureCapacity(m_Keys.Count);
            for (int i = 0; i < m_Keys.Count; i++)
            {
                TryAdd(m_Keys[i], m_Values[i]);
            }
            m_Keys.Clear();
            m_Values.Clear();
        }

        public List<T> GetValues<T>() where T : V
        {
            var list = new List<T>(Count);
            foreach (var kvp in this)
            {
                if(kvp.Value is not T item) continue;
                list.Add(item);
            }
            return list;
        }
    }

    [System.Serializable, InlineProperty, HideLabel]
    public class CastleTwoWayDictionary<K> : ISerializationCallbackReceiver
    {
        public enum KeyValueType
        {
            None,
            Key,
            Value
        }

        [SerializeReference, HideInInspector] public List<K> m_Keys = new List<K>();
        [SerializeReference, HideInInspector] public List<K> m_Values = new List<K>();

        [System.NonSerialized, ShowInInspector,OnValueChanged("RefreshSerialization",true)]
        public Dictionary<K, K> mainDictionary = new Dictionary<K, K>();

        [HideInInspector, System.NonSerialized]
        public Dictionary<K, K> secondDictionary = new Dictionary<K, K>();

        public bool TryAdd(K key, K value)
        {
            if (mainDictionary.ContainsKey(key) || secondDictionary.ContainsKey(value))
            {
                return false;
            }
            else
            {
                mainDictionary.TryAdd(key, value);
                secondDictionary.TryAdd(value, key);
                return true;
            }
        }

        public void RefreshSerialization()
        {
            m_Keys.Clear();
            m_Values.Clear();
            foreach (var kvp in mainDictionary)
            {
                m_Keys.Add(kvp.Key);
                m_Values.Add(kvp.Value);
            }
            mainDictionary.Clear();
            secondDictionary.Clear();
            for (int i = 0; i < m_Keys.Count; i++)
            {
                mainDictionary.TryAdd(m_Keys[i], m_Values[i]);
                secondDictionary.TryAdd(m_Values[i], m_Keys[i]);
            }
        }
        public virtual void OnBeforeSerialize()
        {
            m_Keys.Clear();
            m_Values.Clear();
            foreach (var kvp in mainDictionary)
            {
                m_Keys.Add(kvp.Key);
                m_Values.Add(kvp.Value);
            }
        }

        public virtual void OnAfterDeserialize()
        {
            mainDictionary.Clear();
            secondDictionary.Clear();
            for (int i = 0; i < m_Keys.Count; i++)
            {
                mainDictionary.TryAdd(m_Keys[i], m_Values[i]);
                secondDictionary.TryAdd(m_Values[i], m_Keys[i]);
            }

            m_Keys.Clear();
            m_Values.Clear();
        }

        public KeyValueType Get(K item, out K otherItem)
        {
            if (mainDictionary.TryGetValue(item, out otherItem))
            {
                return KeyValueType.Key;
            }
            else if (secondDictionary.TryGetValue(item, out otherItem))
            {
                return KeyValueType.Value;
            }

            otherItem = default;
            return KeyValueType.None;
        }
    }
}