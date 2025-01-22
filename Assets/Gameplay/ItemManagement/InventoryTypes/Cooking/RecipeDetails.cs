using System;
using MoreMountains.Tools;
using Project.Core.Events;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Project.Gameplay.ItemManagement.InventoryTypes.Cooking
{
    public class RecipeDetails : MonoBehaviour, MMEventListener<RecipeEvent>
    {
        /// the reference inventory from which we'll display item details
        [MMInformation(
            "Specify the name of the recipe whose details you want to display in this Details panel. ",
            MMInformationAttribute.InformationType.Info, false)]
        public string PlayerID = "Player1";
        /// if you make this panel global, it'll ignore
        public bool Global;

        [Header("Default")]
        [MMInformation(
            "Here you can set default values for all fields of the details panel. These values will be displayed when no item is selected (and if you've chosen not to hide the panel in that case).",
            MMInformationAttribute.InformationType.Info, false)]
        /// the title to display when none is provided
        public string DefaultTitle;
        /// the short description to display when none is provided
        public string DefaultShortDescription;
        /// the icon to display when none is provided
        [FormerlySerializedAs("DefaultIcon")] public Sprite defaultRecipeProductSprite;

        [Header("Behaviour")]
        [MMInformation(
            "Here you can decide whether or not to hide the details panel on start.",
            MMInformationAttribute.InformationType.Info, false)]
        /// whether or not to hide the details panel at start
        public bool HideOnStart = true;

        [Header("Components")]
        [MMInformation(
            "Here you need to bind the panel components.", MMInformationAttribute.InformationType.Info, false)]
        /// the icon container object
        public Image Icon;
        /// the title container object
        public Text Title;
        /// the short description container object
        public Text ShortDescription;


        public JournalPersistenceManager journalPersistenceManager;
        protected CanvasGroup _canvasGroup;


        protected float _fadeDelay = 0.2f;

        /// whether the details are currently hidden or not
        public virtual bool Hidden { get; protected set; }

        /// <summary>
        ///     On Start, we grab and store the canvas group and determine our current Hidden status
        /// </summary>
        protected virtual void Start()
        {
            _canvasGroup = GetComponent<CanvasGroup>();

            if (HideOnStart) _canvasGroup.alpha = 0;

            if (_canvasGroup.alpha == 0)
                Hidden = true;
            else
                Hidden = false;
        }
        void OnEnable()
        {
            this.MMEventStartListening();
        }

        void OnDisable()
        {
            this.MMEventStopListening();
        }
        public void OnMMEvent(RecipeEvent mmEvent)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Fills all the detail fields with the specified recipe's values
        /// </summary>
        /// <param name="cookingRecipe"></param>
        public virtual void DisplayDetails(CookingRecipe cookingRecipe)
        {
            if (journalPersistenceManager.JournalData.knownRecipes.Contains(cookingRecipe))
            {
                if (!Hidden)
                {
                    StartCoroutine(MMFade.FadeCanvasGroup(_canvasGroup, _fadeDelay, 0f));
                    Hidden = true;
                }

                StartCoroutine(FillDetailFieldsWithDefaults(0));
            }
            else
            {
                StartCoroutine(FillDetailFields(cookingRecipe, 0f));

                if (Hidden)
                {
                    StartCoroutine(MMFade.FadeCanvasGroup(_canvasGroup, _fadeDelay, 1f));
                    Hidden = false;
                }
            }
        }
        string FillDetailFields(CookingRecipe cookingRecipe, float f)
        {
            throw new NotImplementedException();
        }
        string FillDetailFieldsWithDefaults(int i)
        {
            throw new NotImplementedException();
        }
    }
}
