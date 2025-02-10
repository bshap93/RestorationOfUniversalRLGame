using UnityEngine;

public class HouseCollider : MonoBehaviour
{
    public HouseManager houseManager;
    public GameObject housePartThisHides;
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered house collider");
            houseManager.AddHouseColliderPlayerIsIn(this);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player exited house collider");
            houseManager.RemoveHouseColliderPlayerIsIn(this);
        }
    }
}
