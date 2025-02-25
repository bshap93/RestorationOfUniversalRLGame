namespace Gameplay.Character.Attributes
{
    public interface IPlayerAttributeManager
    {
        public int GetAttributeValue();
        void Awake();
        void Start();

        public void LoadPlayerAttribute();

        public bool HasSavedData();
    }
}
