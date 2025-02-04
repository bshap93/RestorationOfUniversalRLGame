using UnityEngine;

namespace Core.SaveSystem
{
    /// <summary>
    ///     Central manager for all persistent objects in the game.
    ///     This should be the only object using DontDestroyOnLoad.
    /// </summary>
    public class PersistenceRoot : MonoBehaviour
    {
        static PersistenceRoot instance;
        public static PersistenceRoot Instance
        {
            get
            {
                if (instance == null)
                {
                    var go = new GameObject("PersistenceRoot");
                    instance = go.AddComponent<PersistenceRoot>();
                }

                return instance;
            }
        }

        void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        ///     Makes an object persistent between scenes by parenting it to the PersistenceRoot.
        /// </summary>
        public static void MakePersistent(GameObject go)
        {
            go.transform.SetParent(Instance.transform);
        }
    }

    /// <summary>
    ///     Base class for managers that need to persist between scenes.
    /// </summary>
    public abstract class PersistentManager : MonoBehaviour
    {
        protected virtual void Awake()
        {
            var existingManager = FindFirstObjectByType(GetType());
            if (existingManager != null && existingManager != this)
            {
                Destroy(gameObject);
                return;
            }

            PersistenceRoot.MakePersistent(gameObject);
            OnManagerAwake();
        }

        /// <summary>
        ///     Override this instead of Awake for initialization logic.
        /// </summary>
        protected virtual void OnManagerAwake()
        {
        }
    }
}
