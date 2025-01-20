using Project.Gameplay.Player.Stats;
using UnityEngine;

namespace Project.Gameplay.Enemy
{
    public class ExtendsEnemy : MonoBehaviour
    {
        [SerializeField] int xpAmount = 10; // Amount of XP to give to the player on death
        [SerializeField] XpManager XpManager;

        /// <summary>
        ///     This method should be called when the enemy dies
        /// </summary>
        public void Die()
        {
            // Trigger MMGameEvent to notify listeners that XP should be awarded
            XpManager.AddExperience(xpAmount);

            Debug.Log("Enemy died.");
        }
    }
}
