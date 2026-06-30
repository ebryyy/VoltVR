using UnityEngine;

namespace VoltVR.Core
{
    // Bu sýnýf, kendisinden türeyen sýnýflarý sahnede "Tekil/Singleton" yapar.
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<T>();
                    if (_instance == null)
                    {
                        GameObject obj = new GameObject(typeof(T).Name);
                        _instance = obj.AddComponent<T>();
                    }
                }
                return _instance;
            }
        }

        protected virtual void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject); // Sahnede kazara 2 tane varsa eskisini siler.
            }
            else
            {
                _instance = this as T;
            }
        }
    }
}