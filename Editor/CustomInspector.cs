#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace dennokoworks
{
    public class CyberExInspector : lilToon.lilToonInspector
    {
        private const string shaderName = "dennokoworks/CyberEx";

        // --- Master ---
        MaterialProperty _CyberEnabled;
        MaterialProperty _CyberIntensity;

        // --- Mask / Noise ---
        MaterialProperty _CyberMaskTex;
        MaterialProperty _CyberNoiseTex;

        // --- A. Glitch ---
        MaterialProperty _GlitchStrength;
        MaterialProperty _GlitchSpeed;
        MaterialProperty _GlitchBlockScale;
        MaterialProperty _RGBSplitStrength;

        // --- B. Hologram ---
        MaterialProperty _HoloAlpha;
        MaterialProperty _ScanLineSpeed;
        MaterialProperty _ScanLineDensity;
        MaterialProperty _ScanLineWidth;
        MaterialProperty _FlickerSpeed;
        MaterialProperty _FlickerStrength;
        MaterialProperty _HoloEmissionColor;

        // --- C. Geometry Bug ---
        MaterialProperty _GeoBugOffset;
        MaterialProperty _GeoBugBlurAmount;
        MaterialProperty _GeoBugBlurSpeed;
        MaterialProperty _GeoBugThreshold;
        MaterialProperty _CutoutNoiseScale;
        MaterialProperty _CutoutThreshold;
        MaterialProperty _CutoutSharpness;

        private static bool isShowCyberMaster     = true;
        private static bool isShowCyberGlitch      = true;
        private static bool isShowCyberHologram    = true;
        private static bool isShowCyberGeoBug      = true;

        protected override void LoadCustomProperties(MaterialProperty[] props, Material material)
        {
            isCustomShader = true;
            ReplaceToCustomShaders();
            isShowRenderMode = !material.shader.name.Contains("Optional");

            _CyberEnabled       = FindProperty("_CyberEnabled",       props);
            _CyberIntensity     = FindProperty("_CyberIntensity",     props);
            _CyberMaskTex       = FindProperty("_CyberMaskTex",       props);
            _CyberNoiseTex      = FindProperty("_CyberNoiseTex",      props);

            _GlitchStrength     = FindProperty("_GlitchStrength",     props);
            _GlitchSpeed        = FindProperty("_GlitchSpeed",        props);
            _GlitchBlockScale   = FindProperty("_GlitchBlockScale",   props);
            _RGBSplitStrength   = FindProperty("_RGBSplitStrength",   props);

            _HoloAlpha          = FindProperty("_HoloAlpha",          props);
            _ScanLineSpeed      = FindProperty("_ScanLineSpeed",      props);
            _ScanLineDensity    = FindProperty("_ScanLineDensity",    props);
            _ScanLineWidth      = FindProperty("_ScanLineWidth",      props);
            _FlickerSpeed       = FindProperty("_FlickerSpeed",       props);
            _FlickerStrength    = FindProperty("_FlickerStrength",    props);
            _HoloEmissionColor  = FindProperty("_HoloEmissionColor",  props);

            _GeoBugOffset       = FindProperty("_GeoBugOffset",       props);
            _GeoBugBlurAmount   = FindProperty("_GeoBugBlurAmount",   props);
            _GeoBugBlurSpeed    = FindProperty("_GeoBugBlurSpeed",    props);
            _GeoBugThreshold    = FindProperty("_GeoBugThreshold",    props);
            _CutoutNoiseScale   = FindProperty("_CutoutNoiseScale",   props);
            _CutoutThreshold    = FindProperty("_CutoutThreshold",    props);
            _CutoutSharpness    = FindProperty("_CutoutSharpness",    props);
        }

        protected override void DrawCustomProperties(Material material)
        {
            // ---- Master ----
            isShowCyberMaster = Foldout("CyberEx - Master", "CyberEx_Master", isShowCyberMaster);
            if (isShowCyberMaster)
            {
                EditorGUILayout.BeginVertical(boxOuter);
                EditorGUILayout.LabelField("CyberEx Master", customToggleFont);
                EditorGUILayout.BeginVertical(boxInnerHalf);
                m_MaterialEditor.ShaderProperty(_CyberEnabled,   new GUIContent("Cyber Effects Enabled", "サイバーエフェクト全体の有効/無効を切り替えます"));
                m_MaterialEditor.ShaderProperty(_CyberIntensity, new GUIContent("Intensity",             "エフェクト全体の強度を調整します（0=無効、1=最大）"));
                EditorGUILayout.Space(4);
                m_MaterialEditor.TexturePropertySingleLine(
                    new GUIContent("Cyber Mask (R=Glitch G=Holo B=GeoBug A=Cutout)",
                        "R=グリッチ / G=ホログラム / B=ジオメトリバグ / A=カットアウト のマスクテクスチャ。\n各チャンネルの輝度でエフェクトの影響範囲を制御します。"),
                    _CyberMaskTex);
                m_MaterialEditor.TexturePropertySingleLine(
                    new GUIContent("Noise Texture",
                        "各エフェクトのノイズ生成に使用するテクスチャ。\nタイリングさせたグレースケールノイズ画像を推奨します。"),
                    _CyberNoiseTex);
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndVertical();
            }

            // ---- A. Glitch ----
            isShowCyberGlitch = Foldout("CyberEx - A. Glitch & RGB Split", "CyberEx_Glitch", isShowCyberGlitch);
            if (isShowCyberGlitch)
            {
                EditorGUILayout.BeginVertical(boxOuter);
                EditorGUILayout.LabelField("Glitch & RGB Split", customToggleFont);
                EditorGUILayout.BeginVertical(boxInnerHalf);
                m_MaterialEditor.ShaderProperty(_GlitchStrength,   new GUIContent("UV Glitch Strength", "UVがずれるグリッチの強度。値が大きいほど激しくずれます。"));
                m_MaterialEditor.ShaderProperty(_GlitchSpeed,      new GUIContent("Glitch Speed",        "グリッチアニメーションの速度。値が大きいほど早く切り替わります。"));
                m_MaterialEditor.ShaderProperty(_GlitchBlockScale, new GUIContent("Block Noise Scale",   "ブロックノイズのスケール。値が大きいほど粗いブロックになります。"));
                m_MaterialEditor.ShaderProperty(_RGBSplitStrength, new GUIContent("RGB Split Strength",  "RGBチャンネルを個別にずらす色収差エフェクトの強度。"));
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndVertical();
            }

            // ---- B. Hologram ----
            isShowCyberHologram = Foldout("CyberEx - B. Hologram", "CyberEx_Holo", isShowCyberHologram);
            if (isShowCyberHologram)
            {
                EditorGUILayout.BeginVertical(boxOuter);
                EditorGUILayout.LabelField("Hologram & Scanline", customToggleFont);
                EditorGUILayout.BeginVertical(boxInnerHalf);
                m_MaterialEditor.ShaderProperty(_HoloAlpha,         new GUIContent("Holo Alpha",          "ホログラム全体の透明度。0で完全透明、1で不透明になります。"));
                m_MaterialEditor.ShaderProperty(_HoloEmissionColor, new GUIContent("Holo Emission Color", "ホログラムの発光カラー。HDR値でブルームとの組み合わせが映えます。"));
                EditorGUILayout.Space(4);
                m_MaterialEditor.ShaderProperty(_ScanLineDensity, new GUIContent("Scan Line Density", "スキャンラインの密度。値が大きいほど細かい縞模様になります。"));
                m_MaterialEditor.ShaderProperty(_ScanLineWidth,   new GUIContent("Scan Line Width",   "スキャンラインの線幅。0に近いほど細い線になります。"));
                m_MaterialEditor.ShaderProperty(_ScanLineSpeed,   new GUIContent("Scan Line Speed",   "スキャンラインが流れる速度。負の値で逆方向になります。"));
                EditorGUILayout.Space(4);
                m_MaterialEditor.ShaderProperty(_FlickerStrength, new GUIContent("Flicker Strength", "ちらつきエフェクトの強度。0で無効、1で最大のちらつきになります。"));
                m_MaterialEditor.ShaderProperty(_FlickerSpeed,    new GUIContent("Flicker Speed",    "ちらつきの周期速度。値が大きいほど高速でちらつきます。"));
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndVertical();
            }

            // ---- C. Geometry Bug ----
            isShowCyberGeoBug = Foldout("CyberEx - C. Geometry Bug (PC Only)", "CyberEx_GeoBug", isShowCyberGeoBug);
            if (isShowCyberGeoBug)
            {
                EditorGUILayout.BeginVertical(boxOuter);
                EditorGUILayout.LabelField("Geometry Bug / Ghost Duplicate", customToggleFont);
                EditorGUILayout.BeginVertical(boxInnerHalf);
                EditorGUILayout.HelpBox(
                    "Geometry Shader: PC / Windows only. Not available on Quest/Mobile.",
                    MessageType.Info);
                EditorGUILayout.Space(2);
                m_MaterialEditor.ShaderProperty(_GeoBugThreshold,  new GUIContent("Geo Bug Mask Threshold", "ジオメトリバグが発生するCyber Maskの閾値（Bチャンネル）。この値以上のピクセルでバグが現れます。"));
                m_MaterialEditor.ShaderProperty(_GeoBugOffset,     new GUIContent("Normal Offset",          "ゴースト複製を法線方向へずらすオフセット量。値が大きいほど大きくずれます。"));
                m_MaterialEditor.ShaderProperty(_GeoBugBlurAmount, new GUIContent("Lateral Blur Amount",    "横方向のブラー（広がり）の量。ゴーストをぼかして幽霊感を演出します。"));
                m_MaterialEditor.ShaderProperty(_GeoBugBlurSpeed,  new GUIContent("Blur Speed",             "ブラーのアニメーション速度。値が大きいほど早く揺れます。"));
                EditorGUILayout.Space(4);
                EditorGUILayout.LabelField("Cutout Pattern", EditorStyles.boldLabel);
                m_MaterialEditor.ShaderProperty(_CutoutNoiseScale, new GUIContent("Noise Scale",      "カットアウトパターンのノイズスケール。値が小さいほど大きな塊でカットされます。"));
                m_MaterialEditor.ShaderProperty(_CutoutThreshold,  new GUIContent("Cutout Threshold", "カットアウトの閾値。高いほど多くの面積が消えます（Cyber MaskのAチャンネルと連動）。"));
                m_MaterialEditor.ShaderProperty(_CutoutSharpness,  new GUIContent("Cutout Sharpness", "カットアウトエッジのシャープさ。値が大きいほど境界がくっきりします。"));
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndVertical();
            }
        }

        protected override void ReplaceToCustomShaders()
        {
            lts         = Shader.Find(shaderName + "/lilToon");
            ltsc        = Shader.Find("Hidden/" + shaderName + "/Cutout");
            ltst        = Shader.Find("Hidden/" + shaderName + "/Transparent");
            ltsot       = Shader.Find("Hidden/" + shaderName + "/OnePassTransparent");
            ltstt       = Shader.Find("Hidden/" + shaderName + "/TwoPassTransparent");

            ltso        = Shader.Find("Hidden/" + shaderName + "/OpaqueOutline");
            ltsco       = Shader.Find("Hidden/" + shaderName + "/CutoutOutline");
            ltsto       = Shader.Find("Hidden/" + shaderName + "/TransparentOutline");
            ltsoto      = Shader.Find("Hidden/" + shaderName + "/OnePassTransparentOutline");
            ltstto      = Shader.Find("Hidden/" + shaderName + "/TwoPassTransparentOutline");

            ltsoo       = Shader.Find(shaderName + "/[Optional] OutlineOnly/Opaque");
            ltscoo      = Shader.Find(shaderName + "/[Optional] OutlineOnly/Cutout");
            ltstoo      = Shader.Find(shaderName + "/[Optional] OutlineOnly/Transparent");

            ltstess     = Shader.Find("Hidden/" + shaderName + "/Tessellation/Opaque");
            ltstessc    = Shader.Find("Hidden/" + shaderName + "/Tessellation/Cutout");
            ltstesst    = Shader.Find("Hidden/" + shaderName + "/Tessellation/Transparent");
            ltstessot   = Shader.Find("Hidden/" + shaderName + "/Tessellation/OnePassTransparent");
            ltstesstt   = Shader.Find("Hidden/" + shaderName + "/Tessellation/TwoPassTransparent");

            ltstesso    = Shader.Find("Hidden/" + shaderName + "/Tessellation/OpaqueOutline");
            ltstessco   = Shader.Find("Hidden/" + shaderName + "/Tessellation/CutoutOutline");
            ltstessto   = Shader.Find("Hidden/" + shaderName + "/Tessellation/TransparentOutline");
            ltstessoto  = Shader.Find("Hidden/" + shaderName + "/Tessellation/OnePassTransparentOutline");
            ltstesstto  = Shader.Find("Hidden/" + shaderName + "/Tessellation/TwoPassTransparentOutline");

            ltsl        = Shader.Find(shaderName + "/lilToonLite");
            ltslc       = Shader.Find("Hidden/" + shaderName + "/Lite/Cutout");
            ltslt       = Shader.Find("Hidden/" + shaderName + "/Lite/Transparent");
            ltslot      = Shader.Find("Hidden/" + shaderName + "/Lite/OnePassTransparent");
            ltsltt      = Shader.Find("Hidden/" + shaderName + "/Lite/TwoPassTransparent");

            ltslo       = Shader.Find("Hidden/" + shaderName + "/Lite/OpaqueOutline");
            ltslco      = Shader.Find("Hidden/" + shaderName + "/Lite/CutoutOutline");
            ltslto      = Shader.Find("Hidden/" + shaderName + "/Lite/TransparentOutline");
            ltsloto     = Shader.Find("Hidden/" + shaderName + "/Lite/OnePassTransparentOutline");
            ltsltto     = Shader.Find("Hidden/" + shaderName + "/Lite/TwoPassTransparentOutline");

            ltsref      = Shader.Find("Hidden/" + shaderName + "/Refraction");
            ltsrefb     = Shader.Find("Hidden/" + shaderName + "/RefractionBlur");
            ltsfur      = Shader.Find("Hidden/" + shaderName + "/Fur");
            ltsfurc     = Shader.Find("Hidden/" + shaderName + "/FurCutout");
            ltsfurtwo   = Shader.Find("Hidden/" + shaderName + "/FurTwoPass");
            ltsfuro     = Shader.Find(shaderName + "/[Optional] FurOnly/Transparent");
            ltsfuroc    = Shader.Find(shaderName + "/[Optional] FurOnly/Cutout");
            ltsfurotwo  = Shader.Find(shaderName + "/[Optional] FurOnly/TwoPass");
            ltsgem      = Shader.Find("Hidden/" + shaderName + "/Gem");
            ltsfs       = Shader.Find(shaderName + "/[Optional] FakeShadow");

            ltsover     = Shader.Find(shaderName + "/[Optional] Overlay");
            ltsoover    = Shader.Find(shaderName + "/[Optional] OverlayOnePass");
            ltslover    = Shader.Find(shaderName + "/[Optional] LiteOverlay");
            ltsloover   = Shader.Find(shaderName + "/[Optional] LiteOverlayOnePass");

            ltsm        = Shader.Find(shaderName + "/lilToonMulti");
            ltsmo       = Shader.Find("Hidden/" + shaderName + "/MultiOutline");
            ltsmref     = Shader.Find("Hidden/" + shaderName + "/MultiRefraction");
            ltsmfur     = Shader.Find("Hidden/" + shaderName + "/MultiFur");
            ltsmgem     = Shader.Find("Hidden/" + shaderName + "/MultiGem");
        }
    }
}
#endif
