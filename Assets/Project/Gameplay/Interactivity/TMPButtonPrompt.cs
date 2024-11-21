using System.Collections;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using TMPro;
using UnityEngine;

namespace Project.Gameplay.Interactivity
{
    [AddComponentMenu("TopDown Engine/GUI/TextMeshPro Button Prompt")]
    public class TMPButtonPrompt : ButtonPrompt
    {
        [Header("TextMeshPro Settings")]
        /// The TextMeshPro component for the prompt
        [Tooltip("The TextMeshPro component for the prompt")]
        public TextMeshProUGUI TMPPromptText;

        public override void Initialization()
        {
            base.Initialization();
            if (TMPPromptText != null)
            {
                TMPPromptText.text = string.Empty;
                TMPPromptText.alpha = 0f;
            }
        }

        public override void SetText(string newText)
        {
            if (TMPPromptText != null) TMPPromptText.text = newText;
        }

        public override void SetTextColor(Color newColor)
        {
            if (TMPPromptText != null) TMPPromptText.color = newColor;
        }

        public override void Show()
        {
            gameObject.SetActive(true);
            if (_hideCoroutine != null) StopCoroutine(_hideCoroutine);
            ContainerCanvasGroup.alpha = 0f;
            StartCoroutine(MMFade.FadeCanvasGroup(ContainerCanvasGroup, FadeInDuration, 1f));

            if (TMPPromptText != null) TMPPromptText.alpha = 1f;
        }

        public override void Hide()
        {
            if (!gameObject.activeInHierarchy) return;
            _hideCoroutine = StartCoroutine(HideCo());
        }

        protected override IEnumerator HideCo()
        {
            ContainerCanvasGroup.alpha = 1f;
            StartCoroutine(MMFade.FadeCanvasGroup(ContainerCanvasGroup, FadeOutDuration, 0f));
            if (TMPPromptText != null) TMPPromptText.alpha = 0f;
            yield return new WaitForSeconds(FadeOutDuration);
            gameObject.SetActive(false);
        }
    }
}
