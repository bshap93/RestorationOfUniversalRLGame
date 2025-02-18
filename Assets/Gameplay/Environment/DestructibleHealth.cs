using MoreMountains.TopDownEngine;
using PixelCrushers.DialogueSystem;
using UnityEngine;

namespace Gameplay.Environment
{
    public class DestructibleHealth : Health
    {
        PersistentDestructible _persistentDestructible;
        protected override void Awake()
        {
            base.Awake();

            _persistentDestructible = GetComponent<PersistentDestructible>();
        }

        protected override void DestroyObject()
        {
            // If we have save components, make sure to record destruction before deactivating
            if (_persistentDestructible != null)
            {
                Debug.Log("DestructibleHealth.DestroyObject: Recording destruction");
                _persistentDestructible.OnDestroy();
            }

            if (_autoRespawn == null)
            {
                Debug.Log("Reached autospawn true");
                if (DestroyOnDeath)
                {
                    Debug.Log("Reached destroy on death true");
                    if (_character != null)
                        Destroy(_character.gameObject);
                    else
                        Destroy(gameObject);
                }
            }
            else
            {
                _autoRespawn.Kill();
            }
        }
    }
}
