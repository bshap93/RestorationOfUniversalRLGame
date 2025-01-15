using MoreMountains.Tools;
using UnityEngine;

namespace Project.Tests
{
    public class MagicSystemTester : MonoBehaviour
    {
        public MMProgressBar HealthBar;
        public MMProgressBar KinemaBar;
        public MMProgressBar FavourBar;

        public float Health = 100f;
        public float MaxHealth = 100f;
        public float Kinema = 50f;
        public float MaxKinema = 100f;
        public float Favour = 30f;
        public float MaxFavour = 100f;

        public float TestChangeRate = 10f;

        void Update()
        {
            // Simulate changes in resources
            if (Input.GetKey(KeyCode.H))
            {
                // Decrease Health 
                Health = Mathf.Max(Health - TestChangeRate * Time.deltaTime, 0);
                UpdateResourceBars();
            }

            if (Input.GetKey(KeyCode.J)) // Increase Health
                Health = Mathf.Min(Health + TestChangeRate * Time.deltaTime, MaxHealth);

            if (Input.GetKey(KeyCode.K)) // Decrease Kinema
                Kinema = Mathf.Max(Kinema - TestChangeRate * Time.deltaTime, 0);

            if (Input.GetKey(KeyCode.L)) // Increase Kinema
                Kinema = Mathf.Min(Kinema + TestChangeRate * Time.deltaTime, MaxKinema);

            if (Input.GetKey(KeyCode.N)) // Decrease Favour
                Favour = Mathf.Max(Favour - TestChangeRate * Time.deltaTime, 0);

            if (Input.GetKey(KeyCode.M))
            {
                Favour = Mathf.Min(Favour + TestChangeRate * Time.deltaTime, MaxFavour);
                UpdateResourceBars();
            } // Increase Favour
        }
        void UpdateResourceBars()
        {
            // Update Progress Bars
            UpdateBar(HealthBar, Health, MaxHealth);
            UpdateBar(KinemaBar, Kinema, MaxKinema);
            UpdateBar(FavourBar, Favour, MaxFavour);
        }

        void UpdateBar(MMProgressBar bar, float currentValue, float maxValue)
        {
            if (bar != null) bar.UpdateBar(currentValue, 0f, maxValue);
        }
    }
}
