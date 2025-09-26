Shader "Custom/Fade"
{
     Properties
    {
        _Color("Fade Color", Color) = (0,0,0,1)  // Цвет затемнения
        _Intensity("Fade Intensity", Range(0,1)) = 0  // Интенсивность (0 - прозрачно, 1 - полностью непрозрачно)
    }

    SubShader
    {
        Tags 
        { 
            "Queue" = "Transparent+1000"  // Рендерим поверх всех объектов
            "RenderType" = "Transparent"
            "IgnoreProjector" = "True"    // Игнорируем проекторы
        }

        Blend SrcAlpha OneMinusSrcAlpha  // Стандартный режим смешивания для прозрачности
        ZWrite Off  // Отключаем запись в буфер глубины
        Cull Off    // Отображаем обе стороны

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
            };

            fixed4 _Color;
            float _Intensity;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Инвертируем интенсивность: 1 = полностью прозрачно, 0 = непрозрачно
                float alpha = 1.0 - _Intensity;
                return fixed4(_Color.rgb, alpha);
            }
            ENDCG
        }
    }
}
