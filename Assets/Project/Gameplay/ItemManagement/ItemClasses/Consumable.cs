using UnityEngine;
using UnityEngine.Serialization;

namespace Project.Gameplay.ItemManagement.ItemClasses
{
    public class Consumable : MonoBehaviour
    {
        [FormerlySerializedAs("ConsumableName")]
        public string consumableName;
        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
        }
    }
}
