using System.Collections.Generic;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

namespace Project.AI.Scripts
{
	public class AIActionAlertNearbyEnemies : AIAction
	{
		// Radius within which to alert other enemies
		public float AlertRadius = 5.0f;

		public string TargetState;

		// LayerMask for the layers you want to alert
		public LayerMask AlertableLayers;

		// Dictionary to keep track of alerted enemies
		private Dictionary<AIBrain, bool> alertedEnemies = new Dictionary<AIBrain, bool>();

		public override void PerformAction()
		{
			AlertNearbyEnemies();
		}

		protected void AlertNearbyEnemies()
		{
			Collider[] hitColliders = Physics.OverlapSphere(this.gameObject.transform.position, AlertRadius, AlertableLayers);
			foreach (var hitCollider in hitColliders)
			{
				// Exclude the alerting enemy
				if (hitCollider.gameObject != this.gameObject && (hitCollider.transform.parent == null || hitCollider.transform.parent.gameObject != this.gameObject))
				{
					var enemyBrain = hitCollider.GetComponentInChildren<AIBrain>();
					if (enemyBrain != null && LevelManager.HasInstance && LevelManager.Instance.Players != null && LevelManager.Instance.Players[0] != null)
					{
						// Only perform the state transition if the enemy is not the alerting enemy
						if (enemyBrain.gameObject != this.gameObject)
						{
							// Only perform the state transition if the enemy hasn't been alerted yet
							if (!alertedEnemies.ContainsKey(enemyBrain) || !alertedEnemies[enemyBrain])
							{
								enemyBrain.Target = LevelManager.Instance.Players[0].transform;
								// Change the state of the enemy's brain to the target state if it's provided
								if (!string.IsNullOrEmpty(TargetState) && enemyBrain.CurrentState.StateName != TargetState)
								{
									enemyBrain.TransitionToState(TargetState);
									// Mark the enemy as alerted
									alertedEnemies[enemyBrain] = true;
								}
							}
						}
					}
				}
			}
		}

		// Draw a red sphere in the Scene view at the alert radius when this object is selected
		protected virtual void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(this.gameObject.transform.position, AlertRadius);
		}
	}
}