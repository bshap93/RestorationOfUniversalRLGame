using System.Numerics;

namespace Project.Gameplay.DungeonGeneration.Rooms.Models
{
    public class DoorPoint
    {
        public enum DoorDirection
        {
            North,
            East,
            South,
            West
        }

        public DoorDirection Direction;
        public bool IsOpen;
        public Vector3 Position;
        
    }
}
