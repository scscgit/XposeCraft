using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

namespace XposeCraft.Collections
{
    /// <summary>
    /// Represents a generic collection of key/value pairs.
    /// To use this in a class:
    /// Inherit your generic class to a non-generic version (this needs not be public).
    /// Mark it as Serializable.
    /// Source: http://schemingdeveloper.com/2014/11/21/iserializationcallbackreceiver/
    /// </summary>
    /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
    public class SerializableDictionary2<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField, HideInInspector] private List<TKey> _serializedKeys = new List<TKey>();
        [SerializeField, HideInInspector] private List<TValue> _serializedValues = new List<TValue>();
        [SerializeField, HideInInspector] private int _serializedCount;

        [Obsolete("Manual call not supported.", true)]
        public void OnBeforeSerialize()
        {
            _serializedKeys.Clear();
            _serializedKeys.Capacity = Count;
            _serializedValues.Clear();
            _serializedValues.Capacity = Count;

            foreach (var item in this)
            {
                _serializedKeys.Add(item.Key);
                _serializedValues.Add(item.Value);
            }

            _serializedCount = Count;
        }

        [Obsolete("Manual call not supported.", true)]
        public void OnAfterDeserialize()
        {
            Clear();

            if (_serializedCount != _serializedKeys.Count)
            {
                throw new SerializationException(string.Format("{0} failed to serialize.", typeof(TKey).Name));
            }
            if (_serializedCount != _serializedValues.Count)
            {
                throw new SerializationException(string.Format("{0} failed to serialize.", typeof(TValue).Name));
            }

            for (var i = 0; i < _serializedCount; ++i)
            {
                Add(_serializedKeys[i], _serializedValues[i]);
            }

            _serializedKeys.Clear();
            _serializedValues.Clear();
        }
    }
}
