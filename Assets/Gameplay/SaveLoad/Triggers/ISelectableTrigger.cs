using MoreMountains.TopDownEngine;

namespace Project.Gameplay.SaveLoad.Triggers
{
    public interface ISelectableTrigger
    {
        void OnSelectedItem();
        void OnUnSelectedItem();
    }
}
