using UnityEngine;

[CreateAssetMenu(fileName = "Location", menuName = "Location/LocationData", order = 1)]
public class LocationData : ScriptableObject
{
    public string locationName;
    public int locationID;
    public string locationDescription;


    public Vector3Int locationCoordinates;
}
