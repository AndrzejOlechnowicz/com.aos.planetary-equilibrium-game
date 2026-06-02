// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/StylizedPlanet_Unlit"
{
    Properties
    {
        _HeightMap ("Height Map", 2D) = "white" {}
        _NoiseTile ("Noise Tile", 2D) = "white" {}
        _MoistureMap ("Moisture Map", 2D) = "white" {}

        _OceanDeep ("Ocean Deep", Color) = (0.105,0.227,0.424,1)
        _OceanShallow ("Ocean Shallow", Color) = (0.180,0.435,0.686,1)
        _LandBase ("Land Base", Color) = (0.788,0.659,0.420,1)
        _FloraColor ("Flora Color", Color) = (0.227,0.651,0.333,1)
        _IceColor ("Ice Color", Color) = (1,1,1,1)

        _WaterLevel ("Water Level", Range(0,1)) = 0.5
        _FloraLevel ("Flora Level", Range(0,1)) = 0.8
        _Temperature ("Temperature", Range(0,1)) = 0.5

        _NoiseTiling ("Noise Tiling", Float) = 8.0
        _StylizeFlatness ("Stylize Flatness", Range(0,1)) = 0.6

        _IceStartLatitude ("Ice Start Latitude", Range(0,1)) = 0.7
        _IceFalloff ("Ice Falloff", Range(0.01,0.5)) = 0.12
        _IceRimWidth ("Ice Rim Width", Range(0.015, 0.25)) = 0.1

    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        LOD 100

        Pass
        {
            Name "UNLIT"
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

            #include "UnityCG.cginc"

            sampler2D _HeightMap;
            float4 _HeightMap_TexelSize;
            sampler2D _NoiseTile;
            sampler2D _MoistureMap;


            float4 _OceanDeep;
            float4 _OceanShallow;
            float4 _LandBase;
            float4 _FloraColor;
            float4 _IceColor;

            float _WaterLevel;
            float _FloraLevel;
            float _Temperature;

            float _NoiseTiling;
            float _StylizeFlatness;

            float _IceStartLatitude;
            float _IceFalloff;
            float _IceRimWidth;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldNormal : TEXCOORD0;
                float2 uv : TEXCOORD1;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                float3 worldN = mul((float3x3)unity_ObjectToWorld, v.normal);
                o.worldNormal = normalize(worldN);
                o.uv = v.uv;
                return o;
            }

            static inline float SampleR(sampler2D s, float2 uv) { return tex2D(s, uv).r; }

            float4 frag(v2f IN) : SV_Target
            {
                float2 uv = IN.uv;
                float height = saturate(SampleR(_HeightMap, uv)); // normalized 0..1

                // land mask: 1 = visible land, 0 = underwater
                float landMask = step(_WaterLevel, height);

                // ocean color: shallow -> deep
                float oceanDepth = 0.0;
                if (_WaterLevel > 0.0001) oceanDepth = saturate((_WaterLevel - height) / _WaterLevel);
                float4 oceanColor = lerp(_OceanShallow, _OceanDeep, oceanDepth);

                // noise
                float2 noiseUV = uv * _NoiseTiling;
                float noise = tex2D(_NoiseTile, noiseUV).r; // 0..1
                float n = noise * 2.0 - 1.0; // -1..1

                // noise amplitude controlled by StylizeFlatness
                float noiseAmp = lerp(0.02, 0.28, _StylizeFlatness);
                float4 landColor = _LandBase;
                landColor.rgb += n * noiseAmp;

                // --- FLORA SMOOTH SIZE PATCH START ---
                
                // sample moisture map
                //float moisture = tex2D(_MoistureMap, uv).r;


                // sample flora noise (tile + offset)
                //float floraNoiseRaw = tex2D(_NoiseTile, uv * (_NoiseTiling) + float2(12.3,45.6)).r; // 0..1
                
                // contrast the noise so patches are clear
                //float floraNoise = 1;//pow(saturate((floraNoiseRaw - 0.25) / 0.75), 1.9); // 0..1
                
                // height above water (0..1)
                float heightAboveWater = 0.0;
                if (height > _WaterLevel)
                    heightAboveWater = saturate((height - _WaterLevel) / max(0.0001, 1.0 - _WaterLevel));
                
                // elevation bias so flora prefers real land
                float elevationBias = smoothstep(0.005, 0.95, heightAboveWater);
                
                // per-pixel flora signal
                float floraSignal = elevationBias;// * moisture;//floraNoise * elevationBias; // 0..1
                
                // Map global _FloraLevel to a threshold range
                // When _FloraLevel==0 threshold is high (almost no pixels pass).
                // When _FloraLevel==1 threshold is low (most pixels pass).
                // We use an exponential mapping for perceptual control.
                float tMin = 1.001; // threshold when FloraLevel==0 (very few pixels)
                float tMax = 0.05; // threshold when FloraLevel==1 (many pixels)
                float mapped = pow(saturate(_FloraLevel), 1.2); // slight easing

                float threshold = lerp(tMin, tMax, mapped);
                
                // crisp per-pixel mask using step
                float floraMask = step(threshold, floraSignal);
                
                // ensure flora only on visible land
                floraMask *= landMask;
                
                // apply flora as full replacement where mask==1
                float4 landWithFlora = lerp(landColor, _FloraColor, floraMask);
                
                

                // --- ICECAP LOGIC (HEIGHT + SLOPE EROSION, OPAQUE, CUSTOM RIM) ---

                // latitude 0..1 (0 = equator, 1 = pole)
                float lat = abs(IN.worldNormal.y);
                
                // base ice boundary
                float baseEdge = _IceStartLatitude;
                
                // temperature shrinks icecaps toward the poles
                float tempShrink = saturate(1.0 - _Temperature);
                
                // shift boundary toward the pole
                float iceBoundary = lerp(1.0, baseEdge, tempShrink);
                
                // --- HEIGHT-BASED EROSION (primary) ---
                float heightErode = 1.0 - height; // lowlands erode more
                
                // --- SLOPE EROSION (secondary, subtle) ---
                float2 texel = _HeightMap_TexelSize.xy;
                
                float hC = height;
                float hR = tex2D(_HeightMap, uv + float2(texel.x, 0)).r;
                float hU = tex2D(_HeightMap, uv + float2(0, texel.y)).r;
                
                float slope = abs(hC - hR) + abs(hC - hU);
                slope = saturate(slope * 10.0); // subtle, not dominant
                
                // --- COMBINE EROSION (mostly height, some slope) ---
                float erosion = lerp(heightErode, slope, 0.25); // 75% height, 25% slope
                
                // erosion strength based on your falloff
                float erosionStrength = _IceFalloff * 1.8;
                
                // eroded boundary
                float erodedBoundary = iceBoundary + (erosion - 0.5) * erosionStrength;
                
                // --- HARD MASK (no fade) ---
                float iceMask = step(erodedBoundary, lat); // 1 = ice, 0 = land
                
                // ensure ice only on land / ocean
                float landIceMask = iceMask * landMask;
                float oceanIceMask = iceMask * (1.0 - landMask);
                
                // --- RIM EFFECT (using _IceRimWidth + your coloring) ---
                
                // bell-shaped rim mask: 0 (inside) → 1 (edge) → 0 (outside)
                float rimCore = smoothstep(erodedBoundary - _IceRimWidth,
                                           erodedBoundary + _IceRimWidth,
                                           lat);
                float rim = 1.0 - abs(rimCore * 2.0 - 1.0);
                
                // your desired rim coloring
                float3 rimmedIce = lerp(_IceColor.rgb, 
                                        _IceColor.rgb * 0.5,   
                                        rim);
                
                // --- APPLY ICE AS FULL REPLACEMENT ---
                float4 landWithIce = lerp(landWithFlora, float4(rimmedIce, 1.0), landIceMask);
                float4 oceanWithIce = lerp(oceanColor, float4(rimmedIce, 1.0), oceanIceMask);
                
                // final base color
                float4 baseColor = lerp(oceanWithIce, landWithIce, landMask);




                return baseColor;
            }
            ENDCG
        }
    }

    FallBack "Unlit/Color"
}
