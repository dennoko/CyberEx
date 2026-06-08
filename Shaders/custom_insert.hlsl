// CyberEx - custom_insert.hlsl
// Unity ライブラリ #include 直後に挿入（lilSubShaderInsert）
// フォワードパスのみ: グリッチUV / ホログラム走査線 / RGB収差 / ゴーストカットアウト

#if defined(LIL_PASS_FORWARD) || defined(LIL_PASS_FORWARDADD)

//----------------------------------------------------------------------------------------------------------------------
// BEFORE_MAIN: グリッチUVディストーション + マスクサンプリング
//   - ブロックノイズで fd.uvMain を歪める（メインテクスチャ読み込み前に適用）
//   - 以降のマクロで使う _cyberGlitchMask / _cyberHoloMask を宣言
//----------------------------------------------------------------------------------------------------------------------
#define BEFORE_MAIN \
    float4 _cyberMask = LIL_SAMPLE_2D(_CyberMaskTex, sampler_linear_repeat, fd.uv0); \
    float  _cyberGlitchMask = _cyberMask.r * _CyberEnabled * _CyberIntensity; \
    float  _cyberHoloMask   = _cyberMask.g * _CyberEnabled * _CyberIntensity; \
    float  _cyberCutoutMask = _cyberMask.a; \
    { \
        float2 _blockUV    = floor(fd.uvMain * _GlitchBlockScale) / _GlitchBlockScale; \
        float4 _blockNoise = LIL_SAMPLE_2D(_CyberNoiseTex, sampler_linear_repeat, \
                                 _blockUV + LIL_TIME * _GlitchSpeed * 0.05); \
        float  _glitchBurst = step(0.7, _blockNoise.b); \
        float2 _glitchOff   = (_blockNoise.rg * 2.0 - 1.0) * _GlitchStrength \
                               * _glitchBurst * _cyberGlitchMask; \
        fd.uvMain += _glitchOff; \
    }

//----------------------------------------------------------------------------------------------------------------------
// BEFORE_EMISSION_1ST: ホログラム走査線 + フリッカー
//   - positionWS.y ベースのスクロール走査線を発光として加算
//   - 2つのサイン波の積で不規則なチラつきを生成
//----------------------------------------------------------------------------------------------------------------------
#define BEFORE_EMISSION_1ST \
    if (_cyberHoloMask > 0.001) { \
        float _scanPhase = frac(fd.positionWS.y * _ScanLineDensity - LIL_TIME * _ScanLineSpeed); \
        float _scanLine  = smoothstep(1.0 - _ScanLineWidth, 1.0 - _ScanLineWidth * 0.1, _scanPhase); \
        fd.emissionColor.rgb += _HoloEmissionColor.rgb * _scanLine * _cyberHoloMask; \
        float _fA      = sin(LIL_TIME * _FlickerSpeed * 1.3); \
        float _fB      = sin(LIL_TIME * _FlickerSpeed * 0.71 + 1.57); \
        float _flicker = 1.0 - _FlickerStrength * (0.5 - 0.5 * _fA * _fB); \
        fd.emissionColor.rgb *= lerp(1.0, _flicker, _cyberHoloMask); \
    }

//----------------------------------------------------------------------------------------------------------------------
// BEFORE_OUTPUT: RGB収差 / ホログラム半透明 / ゴーストコピーのカットアウト
//   - RGB収差: R・B チャンネルを位置ベースのバーストで逆方向にシフト
//   - ホログラム: マスク強度に応じて alpha を低下させる
//   - ゴーストコピー: cyberData.x==1 のポリゴンをノイズ閾値でくり抜く
//----------------------------------------------------------------------------------------------------------------------
#define BEFORE_OUTPUT \
    { \
        float _rgbTime  = LIL_TIME * _GlitchSpeed * 2.5; \
        float _rgbBurst = step(0.82, frac(sin(_rgbTime * 1.37 + floor(fd.positionWS.y * 40.0)) * 43.758)); \
        float _rgbShift = _RGBSplitStrength * _cyberGlitchMask * _rgbBurst; \
        float _rgbSin   = sin(fd.positionWS.y * 90.0 + _rgbTime); \
        fd.col.r = saturate(fd.col.r + _rgbSin * _rgbShift * 2.0); \
        fd.col.b = saturate(fd.col.b - _rgbSin * _rgbShift * 2.0); \
        fd.col.a = lerp(fd.col.a, fd.col.a * _HoloAlpha, _cyberHoloMask); \
        float _isGhost   = input.cyberData.x; \
        float _ghostRand = input.cyberData.y; \
        if (_isGhost > 0.5) { \
            float2 _noiseUV = fd.uv0 * _CutoutNoiseScale \
                              + float2(_ghostRand * 3.73, _ghostRand * 2.19) \
                              + LIL_TIME * 0.15; \
            float _noiseVal  = LIL_SAMPLE_2D(_CyberNoiseTex, sampler_linear_repeat, _noiseUV).r; \
            float _cutoutVal = _noiseVal + _cyberCutoutMask * 0.3; \
            clip((_cutoutVal - _CutoutThreshold) / max(0.001, _CutoutSharpness)); \
            fd.col.rgb = lerp(fd.col.rgb, _HoloEmissionColor.rgb, 0.35); \
            fd.col.a  *= _HoloAlpha; \
        } \
    }

#endif // LIL_PASS_FORWARD || LIL_PASS_FORWARDADD
