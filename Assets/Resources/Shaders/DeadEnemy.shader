Shader "Custom/DeadEnemy"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _FlySpeed ("Fly Speed", Float) = 1.0
        _RotateSpeed ("Rotate Speed", Float) = 1.0
        _FlyDirection ("Fly Direction", Vector) = (0,1,0,0)
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            fixed4 _Color;
            float _FlySpeed;
            float _RotateSpeed;
            float4 _FlyDirection;

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                
                // Apply flying effect
                float3 flyOffset = _FlyDirection.xyz * _FlySpeed * _Time.y;
                IN.vertex.xyz += flyOffset;
                
                // Apply rotation
                float angle = _Time.y * _RotateSpeed;
                float sina, cosa;
                sincos(angle, sina, cosa);
                float2x2 rotationMatrix = float2x2(cosa, -sina, sina, cosa);
                
                IN.vertex.xy = mul(rotationMatrix, IN.vertex.xy);
                
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color * _Color;
                
                return OUT;
            }

            sampler2D _MainTex;

            fixed4 frag(v2f IN) : SV_Target
            {
                fixed4 c = tex2D(_MainTex, IN.texcoord) * IN.color;
                c.rgb *= c.a;
                return c;
            }
            ENDCG
        }
    }
}