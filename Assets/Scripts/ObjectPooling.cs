using System.Collections.Generic;
using UnityEngine;

public class ObjectPooling : Singleton<ObjectPooling>
{
    private Dictionary<GameObject, List<GameObject>> _allPoolObjects = new();
    private Dictionary<MonoBehaviour, List<MonoBehaviour>> _allPoolScripts = new();

    public GameObject GetObject(GameObject prefab, Vector3 pos, Quaternion qua, Transform parent)
    {
        if (!_allPoolObjects.ContainsKey(prefab))
        {
            _allPoolObjects.Add(prefab, new List<GameObject>());
        }

        foreach (var gameObj in _allPoolObjects[prefab])
        {
            if (gameObj.activeSelf)
            {
                continue;
            }

            return gameObj;
        }

        var newGameObj = Instantiate(prefab, pos, qua, parent);
        _allPoolObjects[prefab].Add(newGameObj);
        return newGameObj;
    }

    public T GetScript<T>(T scriptPrefab, Vector3 pos, Quaternion qua, Transform parent) where T : MonoBehaviour
    {
        if (!_allPoolScripts.ContainsKey(scriptPrefab))
        {
            _allPoolScripts.Add(scriptPrefab, new List<MonoBehaviour>());
        }

        foreach (var script in _allPoolScripts[scriptPrefab])
        {
            if (script.gameObject.activeSelf)
            {
                continue;
            }

            return (T)script;
        }

        var newScript = Instantiate(scriptPrefab, pos, qua, parent);
        _allPoolScripts[scriptPrefab].Add(newScript);
        return newScript;
    }
}