﻿using MoreMountains.Feedbacks;
using UnityEngine;

namespace Gameplay.Dialogue
{
    public class DialogueSoundTrigger : MonoBehaviour
    {
        public MMF_Player feedbackPlayer;

        public void PlayFeedback()
        {
            if (feedbackPlayer != null)
            {
                feedbackPlayer.PlayFeedbacks();
            }
        }
    }
}
