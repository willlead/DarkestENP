using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class PrefabMgr
{
    static private Dictionary<string, GameObject> _cache = new Dictionary<string, GameObject>();

    static public void Load(string subfolder, string subName)
    {
        if (_cache.Count > 0) return;
        object[] t0 = Resources.LoadAll(subfolder);
        for (int i = 0; i < t0.Length; i++)
        {
            GameObject t1 = (GameObject)(t0[i]);
            _cache[t1.name] = t1;
        }
    }

    static public GameObject Get(string key)
    {
        return _cache[key];
    }

    static public GameObject Instance ( string key, string name, float x, float y, float z)
    {
        GameObject t0 = (GameObject)(GameObject.Instantiate(_cache[key], new Vector3(x, y, z), _cache[key].transform.rotation));
        t0.name = name;
        return t0;
    }

    static public void Remove(params string[] arg)
    {
        for (int i = 0; i < arg.Length; i++)
        {
            string key = arg[i];
            _cache.Remove(key);
        }
    }
}

