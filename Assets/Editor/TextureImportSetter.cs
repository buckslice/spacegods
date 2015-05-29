using UnityEngine;
using UnityEditor;

public class TextureImportSetter : AssetPostprocessor {

    // auto imports level textures correctly
    void OnPreprocessTexture() {
        if (assetPath.Contains("Textures")) {
            TextureImporter ti = (TextureImporter)assetImporter;
            ti.filterMode = FilterMode.Bilinear;
            ti.mipmapEnabled = false;
        }
    }
}
