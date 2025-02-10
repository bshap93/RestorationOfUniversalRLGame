using System;
using Project.Core.CharacterCreation;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


// Update AttributeRowUI to include the stat type
public class AttributeRowUI : MonoBehaviour
{
    [SerializeField] Button decrementButton;
    [SerializeField] Button incrementButton;
    [SerializeField] public TMP_Text statNameText;
    [SerializeField] StatType statType; // Add this
    [SerializeField] TMP_Text valueText;

    public Action<int> OnPointsChanged; // Modified to pass point change
    public int CurrentPoints { get; private set; }

    public void Initialize(Action<int> onPointsChanged)
    {
        OnPointsChanged = onPointsChanged;
        incrementButton.onClick.AddListener(OnIncrease);
        decrementButton.onClick.AddListener(OnDecrease);
        statNameText.text = statType.ToString();
        UpdateUI();
    }

    void OnIncrease()
    {
        CurrentPoints++;
        UpdateUI();
        OnPointsChanged?.Invoke(1); // Spent 1 point
    }

    void OnDecrease()
    {
        if (CurrentPoints > 0)
        {
            CurrentPoints--;
            UpdateUI();
            OnPointsChanged?.Invoke(-1); // Refunded 1 point
        }
    }

    public void SetIncrementButtonState(bool enabled)
    {
        if (incrementButton != null) incrementButton.interactable = enabled;
    }

    void UpdateUI()
    {
        valueText.text = CurrentPoints.ToString();
        decrementButton.interactable = CurrentPoints > 0;
    }
}
