using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace XposeCraft.Collections
{
    /// <summary>
    /// Represents a generic collection of key/value pairs.
    /// Can be used for serialization purposes, which implies that it can be used during a hot-swap in Unity Editor.
    /// Source: http://stackoverflow.com/questions/36194178/unity-serialized-dictionary-index-out-of-range-after-12-items
    /// </summary>
    /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
    [System.Serializable]
    public class SerializableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField] private List<TKey> _keys = new List<TKey>();

        [SerializeField] private List<TValue> _values = new List<TValue>();

        private Dictionary<TKey, TValue> _dictionary = new Dictionary<TKey, TValue>();

        public ICollection<TKey> Keys
        {
            get { return ((IDictionary<TKey, TValue>) _dictionary).Keys; }
        }

        public ICollection<TValue> Values
        {
            get { return ((IDictionary<TKey, TValue>) _dictionary).Values; }
        }

        public int Count
        {
            get { return ((IDictionary<TKey, TValue>) _dictionary).Count; }
        }

        public bool IsReadOnly
        {
            get { return ((IDictionary<TKey, TValue>) _dictionary).IsReadOnly; }
        }

        public TValue this[TKey key]
        {
            get { return ((IDictionary<TKey, TValue>) _dictionary)[key]; }

            set { ((IDictionary<TKey, TValue>) _dictionary)[key] = value; }
        }

        public void OnBeforeSerialize()
        {
            _keys.Clear();
            _values.Clear();
            foreach (KeyValuePair<TKey, TValue> pair in this)
            {
                _keys.Add(pair.Key);
                _values.Add(pair.Value);
            }
        }

        public void OnAfterDeserialize()
        {
            _dictionary = new Dictionary<TKey, TValue>();

            if (_keys.Count != _values.Count)
            {
                throw new System.Exception(string.Format(
                    "there are {0} keys and {1} values after deserialization. Make sure that both key and value types are serializable.",
                    _keys.Count, _values.Count));
            }
            for (int i = 0; i < _keys.Count; i++)
            {
                Add(_keys[i], _values[i]);
            }
        }

        public void Add(TKey key, TValue value)
        {
            ((IDictionary<TKey, TValue>) _dictionary).Add(key, value);
        }

        public bool ContainsKey(TKey key)
        {
            return ((IDictionary<TKey, TValue>) _dictionary).ContainsKey(key);
        }

        public bool Remove(TKey key)
        {
            return ((IDictionary<TKey, TValue>) _dictionary).Remove(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return ((IDictionary<TKey, TValue>) _dictionary).TryGetValue(key, out value);
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            ((IDictionary<TKey, TValue>) _dictionary).Add(item);
        }

        public void Clear()
        {
            ((IDictionary<TKey, TValue>) _dictionary).Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return ((IDictionary<TKey, TValue>) _dictionary).Contains(item);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            ((IDictionary<TKey, TValue>) _dictionary).CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return ((IDictionary<TKey, TValue>) _dictionary).Remove(item);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return ((IDictionary<TKey, TValue>) _dictionary).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IDictionary<TKey, TValue>) _dictionary).GetEnumerator();
        }
    }
}
