using System.Collections.Generic;
using Project.Gameplay.DungeonGeneration.Rooms.Models;
using UnityEngine;

namespace Project.Gameplay.DungeonGeneration.Generators
{
public enum RoomType
{
    Start,
    End,
    Normal
}


public class RoomTemplate
{
    public string ID;
    public GameObject Prefab;
    public RoomType Type;
    public List<DoorPoint> DoorPoints;
}

public class DungeonData
{
    public int Seed;
    public Vector2Int Size;
    public List<RoomData> Rooms;
}

public class RoomData
{
    public string TemplateId;
    public Vector2Int Position;
    public bool IsCleared;
}
}
