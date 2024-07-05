using Interfaces;
using UnityEngine;

namespace Utilities
{
    public class GameStorage : IGameStorage
    {
        public void SaveInt(string key, int value)
        {
            PlayerPrefs.SetInt(key, value);
        }

        public int LoadInt(string key, int defaultValue = 0)
        {
            return PlayerPrefs.GetInt(key);
        }

        public void SaveFloat(string key, float value)
        {
            throw new System.NotImplementedException();
        }

        public int LoadFloat(string key, float defaultValue = 0)
        {
            throw new System.NotImplementedException();
        }

        public void SaveString(string key, string value)
        {
            throw new System.NotImplementedException();
        }

        public int LoadString(string key, string defaultValue = "")
        {
            throw new System.NotImplementedException();
        }
    }
}