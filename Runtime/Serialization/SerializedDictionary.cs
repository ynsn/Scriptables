using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace StackMedia.Scriptables
{
    [Serializable]
    public class SerializedPair<TKey, TValue> : IComparable<SerializedPair<TKey, TValue>>
    {
        [SerializeField] public TKey key;

        [SerializeField] public TValue value;

        [SerializeField] public bool duplicateKey;

        public SerializedPair(TKey key, TValue value)
        {
            this.key = key;
            this.value = value;
        }

        public void Deconstruct(out TKey outKey, out TValue outValue)
        {
            outKey = key;
            outValue = value;
        }
        public int CompareTo(SerializedPair<TKey, TValue> other)
        {
            if (key is IComparable<TKey> comparableKey)
            {
                return comparableKey.CompareTo(other.key);
            }
            else if (key is IComparable comparable)
            {
                return comparable.CompareTo(other.key);
            }
            else
            {
                throw new InvalidOperationException($"Cannot compare {typeof(TKey)}");
            }
        }
    }

    /// <summary>
    /// A serializable dictionary implementation that works with Unity's serialization system.
    /// Allows dictionaries to be serialized and displayed in the Unity Inspector.
    /// </summary>
    /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
    [Serializable]
    public class SerializedDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField] private List<SerializedPair<TKey, TValue>> items = new List<SerializedPair<TKey, TValue>>();

        [NonSerialized] private Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();


        #region ISerializationCallbackReceiver

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            var duplicateKeys = items.Select((m, i) => (i, m)).Where(m => m.m.duplicateKey).ToArray();
            items.Clear();
            items.AddRange(dictionary.Select(pair => new SerializedPair<TKey, TValue>(pair.Key, pair.Value)));
            foreach (var pair in duplicateKeys) items.Insert(pair.i, pair.m);
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            dictionary.Clear();
            foreach (var pair in items)
            {
                TKey key = pair.key;
                var add = key != null && !dictionary.ContainsKey(key);
                if (add) dictionary.Add(key, pair.value);
                pair.duplicateKey = !add;
            }
        }

        #endregion

        public void Add(TKey key, TValue value) => (dictionary as IDictionary<TKey, TValue>).Add(key, value);

        public bool ContainsKey(TKey key) => (dictionary as IDictionary<TKey, TValue>).ContainsKey(key);

        public bool Remove(TKey key) => (dictionary as IDictionary<TKey, TValue>).Remove(key);

        public bool TryGetValue(TKey key, out TValue value) => (dictionary as IDictionary<TKey, TValue>).TryGetValue(key, out value);

        public TValue this[TKey key]
        {
            get => (dictionary as IDictionary<TKey, TValue>)[key];
            set => (dictionary as IDictionary<TKey, TValue>)[key] = value;
        }

        public ICollection<TKey> Keys => (dictionary as IDictionary<TKey, TValue>).Keys;

        public ICollection<TValue> Values => (dictionary as IDictionary<TKey, TValue>).Values;


        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => dictionary.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public void Add(KeyValuePair<TKey, TValue> item) => (dictionary as IDictionary<TKey, TValue>).Add(item);
        public void Clear() => (dictionary as IDictionary<TKey, TValue>).Clear();
        public bool Contains(KeyValuePair<TKey, TValue> item) => (dictionary as IDictionary<TKey, TValue>).Contains(item);
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => (dictionary as IDictionary<TKey, TValue>).CopyTo(array, arrayIndex);
        public bool Remove(KeyValuePair<TKey, TValue> item) => (dictionary as IDictionary<TKey, TValue>).Remove(item);

        public int Count => (dictionary as IDictionary<TKey, TValue>).Count;

        public bool IsReadOnly => (dictionary as IDictionary<TKey, TValue>).IsReadOnly;
    }
}
