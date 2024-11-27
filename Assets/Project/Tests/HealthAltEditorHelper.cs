using Project.Gameplay.Player.Health;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Project.Tests
{
    public class OdinDamageButtonWithInstigator : MonoBehaviour
    {
        [Title("Player Settings")] [Required] [Tooltip("Reference to the player's HealthAlt component.")]
        public HealthAlt playerHealth;

        [Title("Instigator Settings")] [Required] [Tooltip("The GameObject that will act as the instigator of the damage.")]
        public GameObject instigator;

        [Title("Damage Settings")] [Tooltip("Amount of damage to deal to the player.")]
        public float damageAmount = 10f;

        [Button("Deal Damage to Player", ButtonSizes.Large)]
        public void DealDamageToPlayer()
        {
            if (playerHealth != null)
            {
                if (instigator == null)
                {
                    Debug.LogWarning("Instigator is not assigned.");
                    return;
                }

                var damageDirection = (playerHealth.transform.position - instigator.transform.position).normalized;

                // Deal damage to the player
                playerHealth.Damage(damageAmount, instigator, 0f, 0f, damageDirection);

                Debug.Log($"Dealt {damageAmount} damage to the player from {instigator.name}.");
            }
            else
            {
                Debug.LogWarning("Player health component is not assigned.");
            }
        }
    }
}
