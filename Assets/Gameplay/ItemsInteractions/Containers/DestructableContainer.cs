namespace Gameplay.ItemsInteractions.Containers
{
    public class DestructableContainer : BaseDestructable
    {
        protected override void Awake()
        {
            base.Awake();
            if (Health != null) Health.OnDeath += OnDeath;
        }
        void OnDestroy()
        {
            if (Health != null) Health.OnDeath -= OnDeath;
        }
    }
}
