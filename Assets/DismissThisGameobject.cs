using UnityEngine;

public class DismissThisGameobject : MonoBehaviour
{
    public static void Dismiss()
    {
        Destroy(GameObject.Find("DismissThisGameobject"));
    }
}
