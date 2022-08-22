using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace Utils
{
    [Serializable]
    public struct SerialKeyValuePair<K, V>
    {
        public K key;
        public V val;
    }
    public class HideInNormalInspectorAttribute : PropertyAttribute { }
}
