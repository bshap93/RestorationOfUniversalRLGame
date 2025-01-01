using System.Collections.Generic;
using UnityEngine;

namespace Project.Gameplay.Transitions
{
    public class Floor : MonoBehaviour
    {
        public int floorLevel; // 0 for ground floor, 1 for first floor, etc.
        private List<Renderer> floorRenderers = new List<Renderer>();
        private Dictionary<Renderer, Color> originalColors = new Dictionary<Renderer, Color>();
        static readonly int Mode = Shader.PropertyToID("_Mode");
        static readonly int SrcBlend = Shader.PropertyToID("_SrcBlend");
        static readonly int DstBlend = Shader.PropertyToID("_DstBlend");

        private void Awake()
        {
            // Automatically find all child renderers in the hierarchy
            floorRenderers.AddRange(GetComponentsInChildren<Renderer>());

            // Cache the original colors of materials for restoration later
            foreach (Renderer renderer in floorRenderers)
            {
                if (renderer != null)
                {
                    originalColors[renderer] = renderer.material.color;
                }
            }
        }

        public void SetTransparency(float alpha)
        {
            foreach (Renderer renderer in floorRenderers)
            {
                if (renderer != null && originalColors.ContainsKey(renderer))
                {
                    // Update alpha of the existing material without instantiating a new one
                    Color originalColor = originalColors[renderer];
                    Color newColor = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
                    Material material = renderer.material;
                    material.color = newColor;

                    // Ensure proper rendering for transparency
                    if (alpha < 1f)
                    {
                        material.SetFloat(Mode, 3); // Transparent mode
                        material.SetInt(SrcBlend, (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                        material.SetInt(DstBlend, (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                        material.EnableKeyword("_ALPHABLEND_ON");
                        material.renderQueue = 3000;
                    }
                    else
                    {
                        material.SetFloat(Mode, 0); // Opaque mode
                        material.SetInt(SrcBlend, (int)UnityEngine.Rendering.BlendMode.One);
                        material.SetInt(DstBlend, (int)UnityEngine.Rendering.BlendMode.Zero);
                        material.DisableKeyword("_ALPHABLEND_ON");
                        material.renderQueue = -1;
                    }
                }
            }
        }

        public void ResetMaterials()
        {
            foreach (Renderer renderer in floorRenderers)
            {
                if (renderer != null && originalColors.ContainsKey(renderer))
                {
                    // Restore the original color of the material
                    Material material = renderer.material;
                    Color originalColor = originalColors[renderer];
                    material.color = originalColor;

                    // Reset to opaque mode
                    material.SetFloat(Mode, 0);
                    material.SetInt(SrcBlend, (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt(DstBlend, (int)UnityEngine.Rendering.BlendMode.Zero);
                    material.DisableKeyword("_ALPHABLEND_ON");
                    material.renderQueue = -1;
                }
            }
        }
    }
}
