using MoreMountains.Tools;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecipeHeader : MonoBehaviour, MMEventListener<MMGameEvent>
{
    public Image recipeImage;
    // Start is called before the first frame update
    public TMP_Text recipeName;


    void Start()
    {
        recipeName = GetComponentInChildren<TMP_Text>();
        recipeName.text = "Recipe Name";
    }

    void OnEnable()
    {
        this.MMEventStartListening();
    }

    void OnDisable()
    {
        this.MMEventStopListening();
    }

    public void OnMMEvent(MMGameEvent eventType)
    {
    }
}
