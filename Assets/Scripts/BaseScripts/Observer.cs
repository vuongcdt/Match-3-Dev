using System;
using System.Collections.Generic;

namespace BaseScripts
{
    public class Observer : Singleton<Observer>
    {
#nullable enable
        private Dictionary<string, List<Action<object?>>> _listeners = new();

        public bool AddListener(string key, Action<object?> action)
#nullable disable
        {
            if (!_listeners.ContainsKey(key))
            {
                try
                {
                    _listeners.TryAdd(key, new List<Action<object>>());
                }
                catch (Exception e)
                {
                    print($"Add Listener Fail: {e}");
                }
            }

            _listeners[key].Add(action);
            return true;
        }

        public void Notify(string key, object value = null)
        {
            if (_listeners.TryGetValue(key, out var listener))
            {
                foreach (var action in listener)
                {
                    try
                    {
                        action?.Invoke(value);
                    }
                    catch (Exception e)
                    {
                        print($"Action fail: {e}");
                    }
                }
            }
        }
    }
}