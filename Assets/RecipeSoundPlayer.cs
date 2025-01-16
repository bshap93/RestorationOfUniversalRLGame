using MoreMountains.Tools;
using Project.Core.Events;
using UnityEngine;

[RequireComponent(typeof(RecipeDisplay))]
public class RecipeSoundPlayer : MonoBehaviour, MMEventListener<RecipeEvent>
{
    public enum Modes
    {
        Direct,
        Event
    }

    [Header("Settings")] public Modes Mode = Modes.Direct;

    [Header("Sounds")] public AudioClip RecipeLearnedFx;
    public AudioClip RecipeSelectedFx;
    public AudioClip ErrorFx;
    public AudioClip OpenFx;
    public AudioClip CloseFx;

    AudioSource _audioSource;
    RecipeDisplay _recipeDisplay;

    protected virtual void Start()
    {
        SetupRecipeSoundPlayer();
        _audioSource = GetComponent<AudioSource>();
        _recipeDisplay = GetComponent<RecipeDisplay>();
    }

    protected virtual void OnEnable()
    {
        this.MMEventStartListening();

        PlayOpenSound();
    }

    protected virtual void OnDisable()
    {
        this.MMEventStopListening();

        PlayCloseSound();
    }

    public void OnMMEvent(RecipeEvent @event)
    {
        switch (@event.EventType)
        {
            case RecipeEventType.RecipeLearned:
                PlaySound(RecipeLearnedFx);
                break;
            case RecipeEventType.ShowRecipeDetails:
                PlaySound(RecipeSelectedFx);
                break;
            case RecipeEventType.Error:
                PlaySound(ErrorFx);
                break;
        }
    }

    public virtual void SetupRecipeSoundPlayer()
    {
        AddAudioSource();
    }

    protected virtual void AddAudioSource()
    {
        if (GetComponent<AudioSource>() == null) gameObject.AddComponent<AudioSource>();
    }

    public virtual void PlaySound(AudioClip soundFx, float volume = 1f)
    {
        if (soundFx == null) return;

        if (Mode == Modes.Direct)
            _audioSource.PlayOneShot(soundFx, volume);
        else
            MMSfxEvent.Trigger(soundFx, null, volume);
    }
    public void PlayOpenSound()
    {
        PlaySound(OpenFx);
    }

    public void PlayCloseSound()
    {
        PlaySound(CloseFx);
    }
}
