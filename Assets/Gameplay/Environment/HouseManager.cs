using System.Collections.Generic;
using UnityEngine;

public class HouseManager : MonoBehaviour
{
    public List<GameObject> houseParts;
    public List<HouseCollider> houseColliders;


    readonly List<HouseCollider> _houseCollidersPlayerIsIn = new();
    readonly Dictionary<string, int> _housePartHidden = new();

    readonly Dictionary<string, GameObject> _housePartsDict = new();

    void Start()
    {
        foreach (var housePart in houseParts)
        {
            _housePartsDict.Add(housePart.name, housePart);
            _housePartHidden.Add(housePart.name, 0);
        }
    }


    public void AddHouseColliderPlayerIsIn(HouseCollider houseCollider)
    {
        _houseCollidersPlayerIsIn.Add(houseCollider);

        _housePartHidden[houseCollider.housePartThisHides.name]++;

        houseCollider.housePartThisHides.SetActive(false);
    }

    public void RemoveHouseColliderPlayerIsIn(HouseCollider houseCollider)
    {
        if (_houseCollidersPlayerIsIn.Contains(houseCollider))
        {
            _houseCollidersPlayerIsIn.Remove(houseCollider);
            _housePartHidden[houseCollider.housePartThisHides.name]--;
        }

        if (_housePartHidden[houseCollider.housePartThisHides.name] == 0)
            houseCollider.housePartThisHides.SetActive(true);
        else
            Debug.Log(
                _housePartHidden[houseCollider.housePartThisHides.name] + " house colliders are hiding " +
                houseCollider.housePartThisHides.name);
    }
}
