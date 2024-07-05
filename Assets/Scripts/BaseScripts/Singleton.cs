using UnityEngine;

namespace BaseScripts
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;
        public static T Instance => _instance;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = GetComponent<T>();
            }
            else if (_instance.gameObject.GetInstanceID() != this.gameObject.GetInstanceID())
            {
                Destroy(gameObject);
            }
        }
    }
}