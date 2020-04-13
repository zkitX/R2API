﻿using BepInEx;
using R2API.Utils;
using System.Collections.Generic;
using System.IO;

namespace R2API {
    /// <summary>
    /// API for replacing the ingame font
    /// </summary>
    [R2APISubmodule]
    public static class FontAPI {
        [R2APISubmoduleInit(Stage = InitStage.SetHooks)]
        internal static void FontAwake() {
            var fontFiles = Directory.GetFiles(Paths.PluginPath, "*.font", SearchOption.AllDirectories);

            foreach (var fontFile in fontFiles) {
                Fonts.Add(fontFile);
            }

            On.RoR2.UI.HGTextMeshProUGUI.OnCurrentLanguageChanged += HGTextMeshProUGUI_OnCurrentLanguageChanged;
            On.RoR2.Language.SetCurrentLanguage += Language_SetCurrentLanguage;
        }

        private static void Language_SetCurrentLanguage(On.RoR2.Language.orig_SetCurrentLanguage orig, string language) {
            orig(language);
            if (Fonts._fontAsset) {
                RoR2.UI.HGTextMeshProUGUI.defaultLanguageFont = Fonts._fontAsset;
            }
        }

        private static void HGTextMeshProUGUI_OnCurrentLanguageChanged(On.RoR2.UI.HGTextMeshProUGUI.orig_OnCurrentLanguageChanged orig) {
            if (Fonts._fontAsset) {
                RoR2.UI.HGTextMeshProUGUI.defaultLanguageFont = Fonts._fontAsset;
            }
        }

        /// <summary>
        /// use this class to add fonts
        /// </summary>
        public static class Fonts {
            /// <summary>
            /// for adding an TMP_FontAsset inside an seperate assetbundle (.font is loaded automatically)
            /// </summary>
            /// <param name="path">absolute path to the assetbundle</param>
            public static void Add(string path) {
                var fontBundle = UnityEngine.AssetBundle.LoadFromFile(path);
                var fonts = fontBundle.LoadAllAssets<TMPro.TMP_FontAsset>();
                foreach (var font in fonts) {
                    Add(font);
                }
            }

            /// <summary>
            /// for adding an TMP_FontAsset while it is still in an assetbundle
            /// </summary>
            /// <param name="fontFile">the assetbundle file</param>
            public static void Add(byte[] fontFile) {
                var fonts = UnityEngine.AssetBundle.LoadFromMemory(fontFile).LoadAllAssets<TMPro.TMP_FontAsset>();
                foreach (var font in fonts) {
                    Fonts.Add(font);
                }
            }

            /// <summary>
            /// for adding an TMP_FontAsset directly
            /// </summary>
            /// <param name="fontAsset">The loaded fontasset</param>
            public static void Add(TMPro.TMP_FontAsset fontAsset) {
                if (fontAsset) {
                    R2API.Logger.LogWarning("multiple fonts as custom font, loading the first added");
                    return;
                }
                _fontAsset = fontAsset;
            }

            internal static TMPro.TMP_FontAsset _fontAsset = null;
        }
    }
}
