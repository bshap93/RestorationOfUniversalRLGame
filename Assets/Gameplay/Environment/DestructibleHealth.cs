using MoreMountains.TopDownEngine;
using PixelCrushers;
using UnityEngine;

namespace Gameplay.Environment
{
    public class DestructibleHealth : Health
    {
        DestructibleSaver _destructibleSaver;
        protected override void Awake()
        {
            base.Awake();

            _destructibleSaver = GetComponent<DestructibleSaver>();
        }

        protected override void DestroyObject()
        {
            // If we have save components, make sure to record destruction before deactivating
            if (_destructibleSaver != null)
            {
                Debug.Log("DestructibleHealth.DestroyObject: Recording destruction");
                _destructibleSaver.RecordDestruction();
            }

            if (_autoRespawn == null)
            {
                if (DestroyOnDeath)
                {
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
