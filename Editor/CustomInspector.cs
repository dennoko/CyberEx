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
                m_MaterialEditor.ShaderProperty(_CyberEnabled,   "Cyber Effects Enabled");
                m_MaterialEditor.ShaderProperty(_CyberIntensity, "Intensity");
                EditorGUILayout.Space(4);
                m_MaterialEditor.TexturePropertySingleLine(
                    new GUIContent("Cyber Mask (R=Glitch G=Holo B=GeoBug A=Cutout)"), _CyberMaskTex);
                m_MaterialEditor.TexturePropertySingleLine(
                    new GUIContent("Noise Texture"), _CyberNoiseTex);
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
                m_MaterialEditor.ShaderProperty(_GlitchStrength,   "UV Glitch Strength");
                m_MaterialEditor.ShaderProperty(_GlitchSpeed,      "Glitch Speed");
                m_MaterialEditor.ShaderProperty(_GlitchBlockScale, "Block Noise Scale");
                m_MaterialEditor.ShaderProperty(_RGBSplitStrength, "RGB Split Strength");
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
                m_MaterialEditor.ShaderProperty(_HoloAlpha,         "Holo Alpha");
                m_MaterialEditor.ShaderProperty(_HoloEmissionColor, "Holo Emission Color");
                EditorGUILayout.Space(4);
                m_MaterialEditor.ShaderProperty(_ScanLineDensity, "Scan Line Density");
                m_MaterialEditor.ShaderProperty(_ScanLineWidth,   "Scan Line Width");
                m_MaterialEditor.ShaderProperty(_ScanLineSpeed,   "Scan Line Speed");
                EditorGUILayout.Space(4);
                m_MaterialEditor.ShaderProperty(_FlickerStrength, "Flicker Strength");
                m_MaterialEditor.ShaderProperty(_FlickerSpeed,    "Flicker Speed");
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
                m_MaterialEditor.ShaderProperty(_GeoBugThreshold,  "Geo Bug Mask Threshold");
                m_MaterialEditor.ShaderProperty(_GeoBugOffset,     "Normal Offset");
                m_MaterialEditor.ShaderProperty(_GeoBugBlurAmount, "Lateral Blur Amount");
                m_MaterialEditor.ShaderProperty(_GeoBugBlurSpeed,  "Blur Speed");
                EditorGUILayout.Space(4);
                EditorGUILayout.LabelField("Cutout Pattern", EditorStyles.boldLabel);
                m_MaterialEditor.ShaderProperty(_CutoutNoiseScale, "Noise Scale");
                m_MaterialEditor.ShaderProperty(_CutoutThreshold,  "Cutout Threshold");
                m_MaterialEditor.ShaderProperty(_CutoutSharpness,  "Cutout Sharpness");
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
