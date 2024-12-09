using DunGen;
using DunGen.Adapters;
using Unity.AI.Navigation;
using UnityEngine;

namespace Project.Gameplay.Navigation
{
    [AddComponentMenu("DunGen/NavMesh/Custom NavMesh Adapter")]
    public class CustomNavMeshAdapter : UnityNavMeshAdapter
    {
        /// <summary>
        ///     Override the method to customize the baking logic for the full dungeon
        /// </summary>
        /// <param name="dungeon">The dungeon to bake</param>
        protected override void BakeFullDungeon(Dungeon dungeon)
        {
            Debug.Log("Custom BakeFullDungeon is being called!");

            // Destroy all old tile-based NavMesh surfaces before full bake
            foreach (var tile in dungeon.AllTiles)
            {
                var surfaces = tile.GetComponentsInChildren<NavMeshSurface>();
                foreach (var surface in surfaces) Destroy(surface); // Destroy all local tile surfaces
            }

            // Call base method to execute the default logic from UnityNavMeshAdapter
            base.BakeFullDungeon(dungeon);

            // Custom logic after the base logic has run
            Debug.Log("Custom logic for BakeFullDungeon has finished!");
        }

        /// <summary>
        ///     Optionally override the Generate method to control overall dungeon generation.
        /// </summary>
        /// <param name="dungeon">The dungeon to generate</param>
        public override void Generate(Dungeon dungeon)
        {
            Debug.Log("Custom Generate logic is running!");

            // Call the base implementation to use DunGen's default logic
            base.Generate(dungeon);

            // Add custom logic for after the dungeon generation
            Debug.Log("Custom Generate logic has finished!");
        }
    }
}
