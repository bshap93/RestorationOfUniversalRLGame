using System.Collections.Generic;
using MoreMountains.Tools;
using Project.Core.CharacterCreation;
using Project.Core.SaveSystem;
using Project.UI.CharacterCreation.UIElements.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Project.UI.CharacterCreation.Scripts
{
    public class CharacterCreationUI : MonoBehaviour
    {
        [Header("Panels")] [SerializeField] GameObject classSelectionPanel;
        [SerializeField] GameObject attributePanel;
        [SerializeField] GameObject traitsPanel;
        [SerializeField] TraitsPanel traitsPanelScript;
        [SerializeField] ConfirmationPanel confirmationPanel;

        [Header("Class Selection")] [SerializeField]
        ClassSelectionPrefab classButton1;
        [SerializeField] ClassSelectionPrefab classButton2;
        [SerializeField] VerticalLayoutGroup detailsPanel;
        [SerializeField] TextMeshProUGUI classNameText;
        [SerializeField] TextMeshProUGUI classDescriptionText;
        [SerializeField] Image selectedClassIcon;

        [Header("Attribute Elements")] [SerializeField]
        TextMeshProUGUI pointsRemainingText;
        [SerializeField] List<AttributeRowUI> attributeRows;
        [SerializeField] int startingPoints = 10;


        [Header("Confirmation")] [SerializeField]
        TMP_InputField characterNameInput;
        [SerializeField] Button confirmButton;

        [Header("Navigation")] [SerializeField]
        Button nextButton;
        [SerializeField] Button backButton;

        [FormerlySerializedAs("FirstSceneName")]
        public string firstSceneName = "AboveGroundStagingArea";


        CharacterCreationData _currentConfig;
        CreationStep _currentStep = CreationStep.ClassSelection;
        int _remainingPoints;
        StartingClass _selectedClass;

        void Start()
        {
            var availableClasses = RunManager.Instance.GetAvailableClasses();
            if (availableClasses.Count >= 2)
            {
                // Assign the first class to button1, and the second class to button2
                classButton1.Setup(availableClasses[0], OnClassButtonClicked);
                classButton2.Setup(availableClasses[1], OnClassButtonClicked);
            }

            _currentConfig = new CharacterCreationData();
            _remainingPoints = startingPoints;
            InitializeUI();
            ShowCurrentStep();
        }

        void InitializeUI()
        {
            characterNameInput.onValueChanged.AddListener(OnCharacterNameChanged);

            nextButton.onClick.AddListener(OnNextClicked);
            backButton.onClick.AddListener(OnBackClicked);
            confirmButton.onClick.AddListener(OnConfirmClicked);

            foreach (var row in attributeRows) row.Initialize(OnAttributePointChanged);

            UpdatePointsDisplay();
        }

        void OnCharacterNameChanged(string playerCharacterName)
        {
            confirmButton.interactable = !string.IsNullOrWhiteSpace(name);
        }


        void OnClassButtonClicked(StartingClass classData)
        {
            if (classData == null) return;

            // Deselect both buttons
            classButton1.SetSelected(false);
            classButton2.SetSelected(false);

            _selectedClass = classData;

            // Update UI
            if (detailsPanel != null) detailsPanel.gameObject.SetActive(true);

            if (classNameText != null) classNameText.text = classData.className;
            if (classDescriptionText != null) classDescriptionText.text = classData.description;

            if (selectedClassIcon != null)
            {
                selectedClassIcon.sprite = classData.classIcon;
                selectedClassIcon.preserveAspect = true;
            }

            nextButton.interactable = true;
        }

        void OnAttributePointChanged(int pointChange)
        {
            _remainingPoints -= pointChange;
            UpdatePointsDisplay();

            nextButton.interactable = true;
        }

        void UpdatePointsDisplay()
        {
            if (pointsRemainingText != null)
                pointsRemainingText.text = $"{_remainingPoints}";

            foreach (var row in attributeRows)
                if (row != null)
                    row.SetIncrementButtonState(_remainingPoints > 0);
        }

        void OnNextClicked()
        {
            switch (_currentStep)
            {
                case CreationStep.ClassSelection:
                    if (_selectedClass != null) _currentStep = CreationStep.Attributes;
                    break;
                case CreationStep.Attributes:


                    _currentStep = CreationStep.Confirmation;
                    ShowCharacterSummary();
                    // traitsPanelScript.Initialize(
                    //     RunManager.Instance.GetAvailableTraits(), _selectedClass.ClassType);

                    break;
                case CreationStep.Traits:
                    if (traitsPanelScript.HasRequiredTraits())
                    {
                        _currentStep = CreationStep.Confirmation;
                        ShowCharacterSummary();
                    }
                    else
                    {
                        Debug.Log("Please select exactly 2 traits before continuing.");
                    }

                    break;
            }

            ShowCurrentStep();
        }

        void ShowCharacterSummary()
        {
            _currentConfig.selectedClassName = _selectedClass.className;
            _currentConfig.attributes = GatherAttributeData();
            // _currentConfig.selectedTraitNames = traitsPanelScript.GetSelectedTraits().ConvertAll(t => t.traitName);
            _currentConfig.selectedTraitNames = new List<string> { "None" };


            confirmationPanel.DisplayCharacterSummary(_currentConfig);
        }

        CharacterStats GatherAttributeData()
        {
            var stats = new CharacterStats();
            foreach (var row in attributeRows)
                switch (row.statNameText.text)
                {
                    case "Strength":
                        stats.strength = row.CurrentPoints;
                        break;
                    case "Agility":
                        stats.agility = row.CurrentPoints;
                        break;
                    case "Endurance":
                        stats.endurance = row.CurrentPoints;
                        break;
                    case "Intelligence":
                        stats.intelligence = row.CurrentPoints;
                        break;
                    case "Intuition":
                        stats.intuition = row.CurrentPoints;
                        break;
                }

            return stats;
        }

        void ShowCurrentStep()
        {
            classSelectionPanel.SetActive(false);
            attributePanel.SetActive(false);
            traitsPanel.SetActive(false);
            confirmationPanel.gameObject.SetActive(false);

            switch (_currentStep)
            {
                case CreationStep.ClassSelection:
                    classSelectionPanel.SetActive(true);
                    break;
                case CreationStep.Attributes:
                    attributePanel.SetActive(true);
                    break;
                case CreationStep.Traits:
                    traitsPanel.SetActive(true);
                    break;
                case CreationStep.Confirmation:
                    confirmationPanel.gameObject.SetActive(true);
                    break;
            }

            backButton.gameObject.SetActive(_currentStep != CreationStep.ClassSelection);
            nextButton.gameObject.SetActive(_currentStep != CreationStep.Confirmation);
            confirmButton.gameObject.SetActive(_currentStep == CreationStep.Confirmation);

            nextButton.interactable = _currentStep switch
            {
                CreationStep.ClassSelection => _selectedClass != null,
                CreationStep.Attributes => true,
                CreationStep.Traits => true,
                _ => false
            };
        }
        void OnBackClicked()
        {
            _currentStep = _currentStep switch
            {
                CreationStep.Attributes => CreationStep.ClassSelection,
                CreationStep.Traits => CreationStep.Attributes,
                CreationStep.Confirmation => CreationStep.Traits,
                _ => _currentStep
            };

            ShowCurrentStep();
        }

        void OnConfirmClicked()
        {
            // Save the final character configuration
            SaveCharacterStats();

            // Could load the game scene here
            // SceneManager.LoadScene("GameScene");
            Debug.Log("Character Creation Completed!");
        }


        void SaveCharacterStats()
        {
            var saveData = NewSaveManager.Instance.CurrentSave;

            saveData.characterCreationData = new CharacterCreationData
            {
                characterName = characterNameInput.text,
                selectedClassName = _selectedClass.className,
                attributes = GatherAttributeData(),
                selectedTraitNames = traitsPanelScript.GetSelectedTraits().ConvertAll(t => t.traitName),
                remainingPoints = _remainingPoints
            };

            Debug.Log(
                $"Saving Character Data: Class - {saveData.characterCreationData.selectedClassName}, " +
                $"Traits - {string.Join(", ", saveData.characterCreationData.selectedTraitNames)}");

            NewSaveManager.Instance.SaveGame();
            MMSceneLoadingManager.LoadScene(firstSceneName);
        }


        enum CreationStep
        {
            ClassSelection,
            Attributes,
            Traits,
            Confirmation
        }
    }
}
