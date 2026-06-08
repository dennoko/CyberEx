//----------------------------------------------------------------------------------------------------------------------
// CyberEx - custom.hlsl
// 変数定義・ジオメトリシェーダー準備・v2f拡張
// PC専用（ジオメトリシェーダー使用のため Quest/Mobile 非対応）

//----------------------------------------------------------------------------------------------------------------------
// ジオメトリシェーダー使用のため vertCustom / appdataCopy を有効化
#define LIL_CUSTOM_VERT_COPY

//----------------------------------------------------------------------------------------------------------------------
// 頂点入力セマンティクス
#define LIL_REQUIRE_APP_POSITION
#define LIL_REQUIRE_APP_TEXCOORD0
#define LIL_REQUIRE_APP_TEXCOORD1
#define LIL_REQUIRE_APP_NORMAL
#define LIL_REQUIRE_APP_TANGENT
#define LIL_REQUIRE_APP_COLOR

//----------------------------------------------------------------------------------------------------------------------
// v2f 拡張: ジオメトリシェーダー → フラグメントへのデータ転送
//   cyberData.x : 1.0 = ゴーストコピー / 0.0 = オリジナルポリゴン
//   cyberData.y : プリミティブごとのランダム値（ノイズオフセット用）
#define LIL_CUSTOM_V2F_MEMBER(id0,id1,id2,id3) \
    float4 cyberData : TEXCOORD##id0;

// フラグメントシェーダーでワールド座標・法線を常に有効化
#define LIL_V2F_FORCE_POSITION_WS
#define LIL_V2F_FORCE_NORMAL

//----------------------------------------------------------------------------------------------------------------------
// カスタム変数（float / vector）
// NOTE: テクスチャは LIL_CUSTOM_TEXTURES に記述すること
#define LIL_CUSTOM_PROPERTIES \
    float  _CyberEnabled;           \
    float  _CyberIntensity;         \
    float  _GlitchStrength;         \
    float  _GlitchSpeed;            \
    float  _GlitchBlockScale;       \
    float  _RGBSplitStrength;       \
    float  _HoloAlpha;              \
    float  _ScanLineSpeed;          \
    float  _ScanLineDensity;        \
    float  _ScanLineWidth;          \
    float  _FlickerSpeed;           \
    float  _FlickerStrength;        \
    float4 _HoloEmissionColor;      \
    float  _GeoBugOffset;           \
    float  _GeoBugBlurAmount;       \
    float  _GeoBugBlurSpeed;        \
    float  _GeoBugThreshold;        \
    float  _CutoutNoiseScale;       \
    float  _CutoutThreshold;        \
    float  _CutoutSharpness;

// カスタムテクスチャ（共有サンプラー sampler_linear_repeat を使用してサンプラー数節約）
#define LIL_CUSTOM_TEXTURES \
    TEXTURE2D(_CyberMaskTex);       \
    TEXTURE2D(_CyberNoiseTex);
