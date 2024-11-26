using Project.Gameplay.Player.Health;
using UnityEngine;

public class HealthAltEditorHelper : MonoBehaviour
{
     HealthAlt playerHealth;
     
     void Start()
     {
         playerHealth = FindObjectOfType<HealthAlt>();
     }


    public void DealDamageToPlayer(float damage)
    {
        if (playerHealth != null)
        {
            playerHealth.Damage(damage, null, 0f, 0f, Vector3.zero);
            Debug.Log($"Dealt {damage} damage to the player.");
        }
        else
        {
            Debug.LogWarning("Player health reference is missing.");
        }
    }
}
