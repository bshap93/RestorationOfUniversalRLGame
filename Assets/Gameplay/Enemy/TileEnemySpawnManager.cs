using DunGen;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using Plugins.DunGen.Code;
using UnityEngine;

// Namespace from DungenCharacter

namespace Project.Gameplay.Enemy
{
    public class TileEnemySpawnerManager : MonoBehaviour, MMEventListener<MMCameraEvent>
    {
        void OnEnable()
        {
            this.MMEventStartListening();
        }

        void OnDisable()
        {
            this.MMEventStopListening();
        }

        public void OnMMEvent(MMCameraEvent itemEvent)
        {
            if (itemEvent.EventType == MMCameraEventTypes.SetTargetCharacter)
            {
                var character = FindObjectOfType<DungenCharacter>();
                character.OnTileChanged += OnPlayerTileChanged;
            }
        }
        /// <summary>
        ///     Called whenever the player enters a new tile
        /// </summary>
        void OnPlayerTileChanged(DungenCharacter character, Tile previousTile, Tile newTile)
        {
            if (newTile == null) return;

            // Check if the new tile has a SpawnEnemiesInTile component
        }
    }
}
