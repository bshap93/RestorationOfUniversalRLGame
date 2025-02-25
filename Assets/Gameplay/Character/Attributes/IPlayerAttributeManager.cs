namespace Gameplay.Character.Attributes
{
    public interface IPlayerAttributeManager
    {
        public int GetAttributeLevelValue();
        void Awake();
        void Start();

        public void LoadPlayerAttribute();

        public bool HasSavedData();
    }
}
