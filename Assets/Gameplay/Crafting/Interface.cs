namespace Gameplay.Crafting
{
    public interface ICraftingStation
    {
        void ShowStationChoicePanel();
        void HideStationChoicePanel();

        void OnSelectedItem();
        void OnUnSelectedItem();

        void OnValidate();
    }
}
