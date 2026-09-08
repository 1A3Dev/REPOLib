using HarmonyLib;
using REPOLib.Modules;
using TMPro;
using UnityEngine;

namespace REPOLib.Patches;

[HarmonyPatch(typeof(MaterialReference))]
internal static class MaterialReferencePatch
{
    [HarmonyPatch(MethodType.Constructor, typeof(int), typeof(TMP_FontAsset), typeof(TMP_SpriteAsset), typeof(Material), typeof(float))]
    [HarmonyPrefix]
    private static void ConstructorPrefix(TMP_FontAsset fontAsset, TMP_SpriteAsset spriteAsset, ref Material material)
    {
        if (material != null) return;

        if (fontAsset != null)
        {
            if (fontAsset.material != null)
            {
                material = fontAsset.material;
                return;
            }

            var defaultMaterial = TMP_Settings.defaultFontAsset?.material;
            if (defaultMaterial == null) return;

            var fallback = new Material(defaultMaterial)
            {
                name = $"{fontAsset.name} (REPOLib Fallback Material)"
            };

            if (fontAsset.atlasTexture != null)
            {
                fallback.mainTexture = fontAsset.atlasTexture;
            }

            fontAsset.material = fallback;
            material = fallback;
            return;
        }

        if (spriteAsset != null)
        {
            if (spriteAsset.material != null)
            {
                material = spriteAsset.material;
                return;
            }

            var fallback = TMP_Settings.defaultSpriteAsset?.material;
            if (fallback == null) return;

            spriteAsset.material = fallback;
            material = fallback;
            return;
        }

        var fallbackMaterial = TMP_Settings.defaultFontAsset?.material;
        if (fallbackMaterial == null) return;

        material = fallbackMaterial;
    }
}
