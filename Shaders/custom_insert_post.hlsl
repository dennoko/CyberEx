// CyberEx - custom_insert_post.hlsl
// vert() 定義直後に挿入（lilSubShaderInsertPost）
// vertCustom: ジオメトリシェーダーへの橋渡し頂点シェーダー
// geomCustom: ポリゴン複製・浮遊・左右ブレ・cyberData セット

#if !defined(LIL_PASS_FORWARD_FUR_INCLUDED)

//----------------------------------------------------------------------------------------------------------------------
// vertCustom: appdata を appdataCopy として返すラッパー
//----------------------------------------------------------------------------------------------------------------------
appdataCopy vertCustom(appdata i)
{
    return appdataOriginalToCopy(i);
}

//----------------------------------------------------------------------------------------------------------------------
// ヘルパー: v2f に cyberData をセットして TriangleStream へ追記
//----------------------------------------------------------------------------------------------------------------------
void AppendTriangle(
    inout TriangleStream<v2f> outStream,
    appdata a0, appdata a1, appdata a2,
    float4 cd)
{
    v2f o0 = vert(a0);
    v2f o1 = vert(a1);
    v2f o2 = vert(a2);
    o0.cyberData = cd;
    o1.cyberData = cd;
    o2.cyberData = cd;
    outStream.Append(o0);
    outStream.Append(o1);
    outStream.Append(o2);
    outStream.RestartStrip();
}

//----------------------------------------------------------------------------------------------------------------------
// geomCustom: ジオメトリシェーダー本体
//   入力 : 三角形1枚 (3頂点)
//   出力 : オリジナル1枚 + ゴーストコピー最大2枚 = 最大9頂点
//
//   ゴーストコピーの動き:
//     Ghost A : 法線方向オフセット + タンジェント方向サイン波ブレ
//     Ghost B : 法線方向オフセット×1.6 + バイタンジェント方向逆位相ブレ
//----------------------------------------------------------------------------------------------------------------------
[maxvertexcount(9)]
void geomCustom(
    triangle appdataCopy ic[3],
    uint primitiveID : SV_PrimitiveID,
    inout TriangleStream<v2f> outStream)
{
    if (_Invisible) return;

    // appdataCopy → appdata（lilToon の逆変換関数を使用）
    appdata i0 = appdataCopyToOriginal(ic[0]);
    appdata i1 = appdataCopyToOriginal(ic[1]);
    appdata i2 = appdataCopyToOriginal(ic[2]);
    LIL_SETUP_INSTANCE_ID(i0);

    //----------------------------------------------------------
    // 三角形の平均TBN・UV を計算
    //----------------------------------------------------------
    float3 normalOS    = normalize(i0.normalOS   + i1.normalOS   + i2.normalOS);
    float3 tangentOS   = normalize(i0.tangentOS.xyz + i1.tangentOS.xyz + i2.tangentOS.xyz);
    float3 bitangentOS = normalize(cross(normalOS, tangentOS) * i0.tangentOS.w);
    float2 avgUV       = (i0.uv0 + i1.uv0 + i2.uv0) * 0.333333;

    //----------------------------------------------------------
    // ジオメトリバグマスク (B チャンネル) をサンプリング
    //----------------------------------------------------------
    float4 geoMask = LIL_SAMPLE_2D_LOD(_CyberMaskTex, sampler_linear_repeat, avgUV, 0);
    float  bugMask = geoMask.b * _CyberEnabled * _CyberIntensity;

    //----------------------------------------------------------
    // オリジナル三角形を出力（cyberData.x = 0）
    //----------------------------------------------------------
    AppendTriangle(outStream, i0, i1, i2, float4(0, 0, 0, 0));

    // マスクが閾値未満なら以降のゴーストコピーはスキップ
    if (bugMask < _GeoBugThreshold) return;

    //----------------------------------------------------------
    // プリミティブ固有のランダム値（各三角形に固有の動き）
    //----------------------------------------------------------
    float randVal = frac(sin(float(primitiveID) * 12.9898) * 43758.5453);
    float phase   = randVal * 6.2832; // 0 〜 2PI
    float t       = LIL_TIME * _GeoBugBlurSpeed;

    //----------------------------------------------------------
    // Ghost A: 法線 + タンジェント方向ブレ
    //----------------------------------------------------------
    {
        float3 offsetA = normalOS  * _GeoBugOffset
                       + tangentOS * sin(t + phase) * _GeoBugBlurAmount;

        appdata ga0 = i0;
        appdata ga1 = i1;
        appdata ga2 = i2;
        ga0.positionOS.xyz += offsetA;
        ga1.positionOS.xyz += offsetA;
        ga2.positionOS.xyz += offsetA;

        AppendTriangle(outStream, ga0, ga1, ga2, float4(1.0, randVal, 0, 0));
    }

    //----------------------------------------------------------
    // Ghost B: 法線×1.6 + バイタンジェント方向ブレ（位相 PI ずれ）
    //----------------------------------------------------------
    {
        float3 offsetB = normalOS    * _GeoBugOffset * 1.6
                       + bitangentOS * sin(t + phase + 3.1416) * _GeoBugBlurAmount;

        appdata gb0 = i0;
        appdata gb1 = i1;
        appdata gb2 = i2;
        gb0.positionOS.xyz += offsetB;
        gb1.positionOS.xyz += offsetB;
        gb2.positionOS.xyz += offsetB;

        AppendTriangle(outStream, gb0, gb1, gb2, float4(1.0, frac(randVal + 0.5), 0, 0));
    }
}

#endif // !LIL_PASS_FORWARD_FUR_INCLUDED
