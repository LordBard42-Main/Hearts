using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GenericDictionary <KEY,VALUE>
{
    [SerializeField] private List<Entry<KEY, VALUE>> dictionary;

    public VALUE GetByKey(KEY key)
    {
        foreach(Entry<KEY, VALUE> entry in dictionary)
        {
            if(key.Equals(entry.Key))
            {
                return entry.Value;
            }
        }

        return default(VALUE);
    }

}

[System.Serializable]
public class Entry<KEY, VALUE>
{

    [SerializeField] private KEY key;
    [SerializeField]  private VALUE value;


    public KEY Key { get => key; set => key = value; }
    public VALUE Value { get => value; set => this.value = value; }
}

