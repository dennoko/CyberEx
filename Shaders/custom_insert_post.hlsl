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
// geomCustom: ジオメトリシェーダー本体
//   入力 : 三角形1枚 (3頂点)
//   出力 : オリジナル1枚 + ゴーストコピー最大2枚 = 最大9頂点
//
//   ゴーストコピーの動き:
//     Ghost A : 法線方向オフセット + タンジェント方向サイン波ブレ
//     Ghost B : 法線方向オフセット + バイタンジェント方向サイン波ブレ（位相ずれ）
//----------------------------------------------------------------------------------------------------------------------
[maxvertexcount(9)]
void geomCustom(
    triangle appdataCopy ic[3],
    uint primitiveID : SV_PrimitiveID,
    inout TriangleStream<v2f> outStream)
{
    if (_Invisible) return;

    // appdataCopy → appdata
    appdata i[3] = {
        appdataOriginalToCopy(ic[0]),
        appdataOriginalToCopy(ic[1]),
        appdataOriginalToCopy(ic[2])
    };
    LIL_SETUP_INSTANCE_ID(i[0]);

    //----------------------------------------------------------
    // 三角形の重心・TBN・UV を計算
    //----------------------------------------------------------
    float3 normalOS   = normalize(i[0].normalOS  + i[1].normalOS  + i[2].normalOS);
    float3 tangentOS  = normalize(i[0].tangentOS.xyz + i[1].tangentOS.xyz + i[2].tangentOS.xyz);
    float3 bitangentOS = normalize(cross(normalOS, tangentOS) * i[0].tangentOS.w);
    float2 avgUV      = (i[0].uv0 + i[1].uv0 + i[2].uv0) * 0.333333;

    //----------------------------------------------------------
    // ジオメトリバグマスク (B チャンネル) をサンプリング
    //----------------------------------------------------------
    float4 geoMask = LIL_SAMPLE_2D_LOD(_CyberMaskTex, sampler_linear_repeat, avgUV, 0);
    float  bugMask = geoMask.b * _CyberEnabled * _CyberIntensity;

    //----------------------------------------------------------
    // オリジナル三角形を出力（cyberData.x = 0）
    //----------------------------------------------------------
    v2f base[3] = { vert(i[0]), vert(i[1]), vert(i[2]) };
    base[0].cyberData = float4(0, 0, 0, 0);
    base[1].cyberData = float4(0, 0, 0, 0);
    base[2].cyberData = float4(0, 0, 0, 0);
    outStream.Append(base[0]);
    outStream.Append(base[1]);
    outStream.Append(base[2]);
    outStream.RestartStrip();

    // マスクが閾値未満なら以降のゴーストコピーは生成しない
    if (bugMask < _GeoBugThreshold) return;

    //----------------------------------------------------------
    // プリミティブ固有のランダム値（各三角形に固有の動き）
    //----------------------------------------------------------
    float randVal = frac(sin(float(primitiveID) * 12.9898) * 43758.5453);
    float phase   = randVal * 6.2832; // 0 〜 2PI

    float t = LIL_TIME * _GeoBugBlurSpeed;

    //----------------------------------------------------------
    // Ghost A: 法線 + タンジェント方向ブレ
    //   オフセット = normalOS * offset + tangentOS * sin(t + phase) * blur
    //----------------------------------------------------------
    {
        float sinA = sin(t + phase);
        float3 offsetA = normalOS   * _GeoBugOffset
                       + tangentOS  * sinA * _GeoBugBlurAmount;

        appdata ia[3] = i;
        ia[0].positionOS.xyz += offsetA;
        ia[1].positionOS.xyz += offsetA;
        ia[2].positionOS.xyz += offsetA;

        v2f gA[3] = { vert(ia[0]), vert(ia[1]), vert(ia[2]) };
        float4 cdA = float4(1.0, randVal, 0, 0);
        gA[0].cyberData = cdA;
        gA[1].cyberData = cdA;
        gA[2].cyberData = cdA;
        outStream.Append(gA[0]);
        outStream.Append(gA[1]);
        outStream.Append(gA[2]);
        outStream.RestartStrip();
    }

    //----------------------------------------------------------
    // Ghost B: 法線 + バイタンジェント方向ブレ（位相 PI ずれ）
    //   より遠くに浮かせて奥行き感を出す
    //----------------------------------------------------------
    {
        float sinB = sin(t + phase + 3.1416);
        float3 offsetB = normalOS    * _GeoBugOffset * 1.6
                       + bitangentOS * sinB * _GeoBugBlurAmount;

        appdata ib[3] = i;
        ib[0].positionOS.xyz += offsetB;
        ib[1].positionOS.xyz += offsetB;
        ib[2].positionOS.xyz += offsetB;

        v2f gB[3] = { vert(ib[0]), vert(ib[1]), vert(ib[2]) };
        float4 cdB = float4(1.0, frac(randVal + 0.5), 0, 0);
        gB[0].cyberData = cdB;
        gB[1].cyberData = cdB;
        gB[2].cyberData = cdB;
        outStream.Append(gB[0]);
        outStream.Append(gB[1]);
        outStream.Append(gB[2]);
        outStream.RestartStrip();
    }
}

#endif // !LIL_PASS_FORWARD_FUR_INCLUDED
