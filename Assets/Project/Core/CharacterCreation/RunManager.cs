using System.Collections.Generic;
using UnityEngine;

namespace Project.Core.CharacterCreation
{
    public class RunManager : MonoBehaviour
    {
        [Header("Configuration")] [SerializeField]
        int startingAttributePoints = 10;
        [SerializeField] List<CharacterTrait> availableTraits;
        [SerializeField] List<StartingClass> availableClasses;
        [SerializeField] string selectedClassName;
        [SerializeField] List<string> selectedTraitNames;
        public static RunManager Instance { get; private set; }

        public RunConfig CurrentRun { get; }
        public int StartingAttributePoints => startingAttributePoints;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                LoadTraitsAndClasses();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void LoadTraitsAndClasses()
        {
            // Load trait and class definitions from ScriptableObjects
            availableTraits = new List<CharacterTrait>(Resources.LoadAll<CharacterTrait>("Traits"));
            availableClasses = new List<StartingClass>(Resources.LoadAll<StartingClass>("Classes"));
        }

        public List<CharacterTrait> GetAvailableTraits()
        {
            return availableTraits;
        }

        public List<StartingClass> GetAvailableClasses()
        {
            return availableClasses;
        }
    }
}
