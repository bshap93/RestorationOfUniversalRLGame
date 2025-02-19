using System.Collections;
using MoreMountains.InventoryEngine;
using MoreMountains.TopDownEngine;
using PixelCrushers;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Core.Events
{
    public static class TdePauseUtility
    {
        static int pauseDepth;
        static bool prevSendNavEvents;

#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void InitStaticVariables()
        {
            pauseDepth = 0;
            prevSendNavEvents = false;
        }
#endif

        public static void Pause(bool pause, bool disableInput,
            string[] floatAnimatorParametersToStop,
            string[] boolAnimatorParametersToStop)
        {
            // In case we get multiple requests to pause before unpause, only unpause after last call to Unpause:
            if (pauseDepth == 0)
            {
                if (pause)
                {
                    GameManager.Instance.Pause(PauseMethods.PauseMenu, false);
                    GUIManager.Instance.PauseScreen.SetActive(false);
                }

                if (disableInput) prevSendNavEvents = EventSystem.current.sendNavigationEvents;
            }

            pauseDepth++;
            if (disableInput) SetTopDownInput(false);
            SetPlayerControl(false, floatAnimatorParametersToStop, boolAnimatorParametersToStop);
            EventSystem.current.sendNavigationEvents = true;
        }

        public static void Unpause(bool pause, bool disableInput,
            string[] floatAnimatorParametersToStop,
            string[] boolAnimatorParametersToStop)
        {
            pauseDepth--;
            if (pauseDepth == 0)
                GameManager.Instance.StartCoroutine(
                    UnpauseAtEndOfFrame(
                        pause, disableInput, floatAnimatorParametersToStop, boolAnimatorParametersToStop));
        }

        static IEnumerator UnpauseAtEndOfFrame(bool pause, bool disableInput,
            string[] floatAnimatorParametersToStop,
            string[] boolAnimatorParametersToStop)
        {
            yield return new WaitForEndOfFrame();
            if (pause)
            {
                GameManager.Instance.UnPause();
                GUIManager.Instance.PauseScreen.SetActive(false);
            }

            if (disableInput)
            {
                SetTopDownInput(true);
                EventSystem.current.sendNavigationEvents = prevSendNavEvents;
            }

            SetPlayerControl(true, floatAnimatorParametersToStop, boolAnimatorParametersToStop);
        }

        static void SetTopDownInput(bool value)
        {
            SetAllInputManagers(value);
        }

        static void SetAllInputManagers(bool value)
        {
            // Enable/disable the TDE input managers:
            foreach (var inputManager in GameObjectUtility.FindObjectsByType<InputManager>())
                inputManager.InputDetectionActive = value;

            // Enable/disable the Inventory Engine input managers:
            foreach (var inputManager in GameObjectUtility.FindObjectsByType<InventoryInputManager>())
                inputManager.enabled = value;
        }

        static void SetPlayerControl(bool value, string[] floatAnimatorParametersToStop,
            string[] boolAnimatorParametersToStop)
        {
            // Freeze or unfreeze characters, including stopping movements, shooting, and walk particles.
            if (value)
            {
                LevelManager.Instance.UnFreezeCharacters();
            }
            else
            {
                LevelManager.Instance.FreezeCharacters();
                GameManager.Instance.StartCoroutine(
                    StopAnimators(floatAnimatorParametersToStop, boolAnimatorParametersToStop));
            }

            foreach (var player in LevelManager.Instance.Players)
            {
                player.LinkedInputManager.RunButton.TriggerButtonUp();
                var characterRun = player.GetComponent<CharacterRun>();
                if (characterRun != null) characterRun.RunStop();
                player.MovementState.ChangeState(CharacterStates.MovementStates.Idle);

                var characterMovement = player.GetComponent<CharacterMovement>();
                if (characterMovement != null)
                {
                    characterMovement.PermitAbility(value);
                    characterMovement.MovementForbidden = !value;
                }

                foreach (var characterHandleWeapon in player.GetComponents<CharacterHandleWeapon>())
                    characterHandleWeapon.ShootStop();
            }
        }

        static IEnumerator StopAnimators
        (string[] floatAnimatorParametersToStop,
            string[] boolAnimatorParametersToStop)
        {
            yield return null;
            foreach (var player in LevelManager.Instance.Players)
            {
                var animator = player.GetComponent<Character>()._animator;
                foreach (var floatParameter in floatAnimatorParametersToStop) animator.SetFloat(floatParameter, 0);
                foreach (var boolParameter in boolAnimatorParametersToStop) animator.SetBool(boolParameter, false);
            }
        }
    }
}
