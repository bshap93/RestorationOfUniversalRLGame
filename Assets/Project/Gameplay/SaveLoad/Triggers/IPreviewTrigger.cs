using MoreMountains.TopDownEngine;

namespace Project.Gameplay.ItemManagement.Triggers
{
    public interface IPreviewTrigger
    {
        void OnSelectedItem();
        void OnUnSelectedItem();

        public void OnMMEvent(MMCameraEvent cookingStationEvent);
    }
}
