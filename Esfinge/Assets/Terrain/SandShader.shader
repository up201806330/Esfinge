Shader "Custom/SandShader"
{
    Properties
    {
        _SandColor ("Color", Color) = (1,1,1,1)
        _SandTex ("Sand noise", 2D) = "white" {}
        _SteepTex ("Steep texture", 2D) = "bump" {}
        _ShallowTex ("Shallow texture", 2D) = "bump" {}
        _SandStrength ("Strength", Float) = 0.5
        _SteepnessSharpnessPower ("Steepness Power", Float) = 1.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM   
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        float _SandStrength;
        fixed4 _SandColor;
        float _SteepnessSharpnessPower;
 
        float3 nlerp(float3 n1, float3 n2, float t) {
            return normalize(lerp(n1, n2, t));
        }

        struct Input
        {
            float3 vertNormal;
            float2 uv_SandTex;
            float3 worldNormal;
            INTERNAL_DATA
        };
        
        sampler2D _SandTex;
        sampler2D _ShallowTex;
        sampler2D _SteepTex;

        float3 SandNormal (float2 uv, float3 N) {
            // Random vector
            float3 random = tex2D(_SandTex, uv);
            // Random direction
            // [0,1]->[-1,+1]
            float3 S = normalize(random * 2 - 1);
            // Rotates N towards Ns based on _SandStrength
            float3 Ns = nlerp(N, S, _SandStrength);
            return Ns;
        }

        float4 _ShallowTex_ST;
        float4 _SteepTex_ST;
        float3 WavesNormal(float2 uv, float3 N, float steepness){
            // [0,1]->[-1,+1]
            float3 shallow = UnpackNormal(tex2D(_ShallowTex, TRANSFORM_TEX(uv, _ShallowTex)));
            float3 steep   = UnpackNormal(tex2D(_SteepTex,   TRANSFORM_TEX(uv, _SteepTex  )));
 
            // Steepness normal
            float3 S = nlerp(steep, shallow, steepness);
            return S;
		}

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void vert (inout appdata_full v, out Input o) {
            UNITY_INITIALIZE_OUTPUT(Input,o);
            o.vertNormal = v.normal;
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            
            float3 N_WORLD = WorldNormalVector(IN, o.Normal);
            float3 UP_WORLD = float3(0, 1, 0);
 
            // Calculates "steepness"
            // => 0: steep (90 degrees surface)
            //  => 1: shallow (flat surface)
            float steepness = saturate(dot(N_WORLD, UP_WORLD));  
            steepness = pow(steepness, _SteepnessSharpnessPower) * 0.7;

            o.Albedo = _SandColor;
            o.Alpha = 1;
            float3 N = IN.vertNormal;
            N = WavesNormal(IN.uv_SandTex.xy, N, steepness);
            N = SandNormal (IN.uv_SandTex, N);

         
            o.Normal = N;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
