using UnityEngine;
using UnityEngine.Serialization;

public class LevelInformation : MonoBehaviour
{
    [FormerlySerializedAs("_locationData")] [SerializeField]
    LocationData locationData;
}
