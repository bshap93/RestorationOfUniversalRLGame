using MoreMountains.Tools;
using UnityEngine;

public class RevertTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    public static void Revert()
    {
        MMGameEvent.Trigger("Revert");
    }
}
