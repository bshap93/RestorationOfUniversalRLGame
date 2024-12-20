using System.Collections;
using CompassNavigatorPro;
using DunGen;
using UnityEngine;

namespace Project.Gameplay.Navigation
{
    public class StaticMapRoomCulling : AdjacentRoomCulling
    {
        [Header("Map Capture Settings")] public Camera topDownCamera; // Camera used to capture the dungeon layout
        public RenderTexture mapRenderTexture; // RenderTexture for capturing the dungeon map
        public CompassPro compassPro; // Reference to CompassPro to set the mini-map texture
        public bool captureStaticMapOnGenerationComplete = true; // Option to enable or disable map capture
        public bool disableCullingAfterMapCapture = true; // Option to disable the room culling after the map capture

        /// <summary>
        ///     Called when the dungeon generation status changes (inherited from AdjacentRoomCulling)
        /// </summary>
        protected override void OnDungeonGenerationStatusChanged(DungeonGenerator generator, GenerationStatus status)
        {
            base.OnDungeonGenerationStatusChanged(generator, status);

            if (status == GenerationStatus.Complete && captureStaticMapOnGenerationComplete)
                StartCoroutine(CaptureDungeonMap());
        }

        /// <summary>
        ///     Captures a static image of the dungeon and assigns it to Compass Navigator Pro as the mini-map.
        /// </summary>
        public IEnumerator CaptureDungeonMap()
        {
            if (!topDownCamera || !mapRenderTexture)
            {
                Debug.LogError("TopDownCamera or RenderTexture is not set. Cannot capture dungeon map.");
                yield break;
            }

            Debug.Log("Capturing static map of the dungeon...");

            // Step 1: Ensure all rooms are visible
            MakeAllTilesVisible();

            yield return null; // Wait for one frame to ensure everything is visible

            // Step 2: Set up the camera to capture the entire dungeon
            topDownCamera.targetTexture = mapRenderTexture;
            topDownCamera.Render();

            // Step 3: Convert the RenderTexture to a Texture2D
            var mapTexture = new Texture2D(mapRenderTexture.width, mapRenderTexture.height, TextureFormat.RGB24, false);
            var currentRT = RenderTexture.active;
            RenderTexture.active = mapRenderTexture;

            mapTexture.ReadPixels(new Rect(0, 0, mapRenderTexture.width, mapRenderTexture.height), 0, 0);
            mapTexture.Apply();

            RenderTexture.active = currentRT;

            Debug.Log("Map texture successfully created.");

            yield return null; // Wait for one frame to ensure Unity processes the texture

            // Step 4: Assign the texture to Compass Navigator Pro
            if (compassPro != null)
            {
                Debug.Log("Applying captured dungeon map texture to Compass Navigator Pro...");
                // Set the texture directly (this is the important change)
                compassPro.miniMapContentsTexture = mapTexture;

                // Set mini-map contents to "WorldMappedTexture"
                compassPro.miniMapContents = MiniMapContents.WorldMappedTexture;

                // Force CompassPro to update its mini-map contents
                compassPro.UpdateMiniMapContents();
            }

            // Step 5: Disable culling if required
            if (disableCullingAfterMapCapture) enabled = false; // Disable culling after the map is captured

            Debug.Log("Dungeon map successfully captured and set as a static map.");
        }

        /// <summary>
        ///     Ensures that all tiles in the dungeon are visible before capturing the mini-map.
        /// </summary>
        void MakeAllTilesVisible()
        {
            if (allTiles == null) return;

            foreach (var tile in allTiles) SetTileVisibility(tile, true);

            Debug.Log("All tiles have been made visible for the map capture.");
        }
    }
}
