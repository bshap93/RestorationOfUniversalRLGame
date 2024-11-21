using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Project.Gameplay.DungeonGeneration.Generators
{
    public class NewDungeonGenerator : MonoBehaviour
    {
        private NewDungeonGenerator generator;

        private void Awake()
        {
            // Add DungeonGenerator component if it doesn't exist
            generator = gameObject.GetComponent<NewDungeonGenerator>();
            if (generator == null)
            {
                generator = gameObject.AddComponent<NewDungeonGenerator>();
            }
        }

        public Task GenerateNewDungeon(int seed)
        {
            try 
            {
                // var dungeonData = await generator.GenerateDungeon(seed);
            
                // TODO: Implement these methods
                // SpawnPlayer();
                // PopulateRooms();
                // InitializeEnemies();
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to generate dungeon: {e.Message}");
                throw;
            }

            return Task.CompletedTask;
        }

        public void LoadDungeon(DungeonData data)
        {
            // TODO: Implement
            throw new System.NotImplementedException();
        }
    }
}