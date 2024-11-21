using UnityEngine;
using UnityEngine.Serialization;

namespace Project.Gameplay.ItemManagement.ItemClasses
{
    public class Collectable : MonoBehaviour
    {
        [FormerlySerializedAs("CollectableName")]
        public string collectableName;
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
