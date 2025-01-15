using UnityEngine;
using PixelCrushers.TopDownEngineSupport;

namespace PixelCrushers.QuestMachine.TopDownEngineSupport
{

    /// <summary>
    /// Pauses TopDown and/or disables player input when receiving a 'Pause Player'
    /// message from the quest journal or quest dialogue UI. Resumes when receiving 
    /// 'Unpause Player'. You can also manually call Pause() and Unpause().
    /// </summary>
    [AddComponentMenu("Pixel Crushers/Quest Machine/Third Party/TopDown Engine/Pause TopDown")]
    public class PauseTopDown : MonoBehaviour, IMessageHandler
    {
        [Tooltip("Pause Topdown when Quest Machine sends Pause Player message.")]
        public bool pauseTopDownDuringPause = true;

        [Tooltip("Disable TopDown player input when Quest Machine sends Pause Player message.")]
        public bool disableTopDownInputDuringPause = true;
        public string[] floatAnimatorParametersToStop = new string[] { "Speed" };
        public string[] boolAnimatorParametersToStop = new string[] { "Walking", "Running", "Jumping" };

        protected virtual void OnEnable()
        {
            MessageSystem.AddListener(this, "Pause Player", string.Empty);
            MessageSystem.AddListener(this, "Unpause Player", string.Empty);
        }

        protected virtual void OnDisable()
        {
            MessageSystem.RemoveListener(this);
        }

        public void OnMessage(MessageArgs messageArgs)
        {
            switch (messageArgs.message)
            {
                case "Pause Player":
                    Pause();
                    break;
                case "Unpause Player":
                    Unpause();
                    break;
            }
        }

        public virtual void Pause()
        {
            TDEPauseUtility.Pause(pauseTopDownDuringPause, disableTopDownInputDuringPause,
                floatAnimatorParametersToStop, boolAnimatorParametersToStop);
        }

        public virtual void Unpause()
        {
            TDEPauseUtility.Unpause(pauseTopDownDuringPause, disableTopDownInputDuringPause,
                floatAnimatorParametersToStop, boolAnimatorParametersToStop);
        }

    }
}
