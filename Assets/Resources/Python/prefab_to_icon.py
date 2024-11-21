import UnityEngine as ue
import os
from UnityEditor import AssetDatabase


def create_icon_from_prefab(prefab_path, output_path, icon_size=(256, 256)):
    # Load the prefab
    prefab = AssetDatabase.LoadAssetAtPath(prefab_path, ue.GameObject)
    if prefab is None:
        print("Prefab not found at path:", prefab_path)
        return

    # Instantiate prefab in the scene
    instance = ue.Object.Instantiate(prefab)

    # Set up a camera for rendering
    cam = ue.Camera.main
    if cam is None:
        cam = ue.GameObject("IconCamera", ue.Camera).GetComponent(ue.Camera)

    cam.transform.position = ue.Vector3(0, 0, -10)  # Position camera appropriately
    cam.targetTexture = ue.RenderTexture(icon_size[0], icon_size[1], 24)

    # Render to texture
    cam.Render()

    # Save the render texture to a PNG file
    render_texture = cam.targetTexture
    ue.Texture2D
    icon_texture = ue.Texture2D(render_texture.width, render_texture.height, ue.TextureFormat.RGB24, False)
    ue.RenderTexture.active = render_texture
    icon_texture.ReadPixels(ue.Rect(0, 0, render_texture.width, render_texture.height), 0, 0)
    icon_texture.Apply()

    # Save the texture as PNG
    output_icon_path = os.path.join(output_path, "prefab_icon.png")
    with open(output_icon_path, "wb") as f:
        f.write(icon_texture.EncodeToPNG())

    # Clean up
    ue.Object.Destroy(instance)
    ue.RenderTexture.active = None
    cam.targetTexture = None
    print("Icon created at:", output_icon_path)
