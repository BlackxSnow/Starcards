using System;

namespace Utility
{
    public static class Easings
    {
        public static float EaseInOutCirc(float t)
        {
            return MathF.Sqrt(1 - MathF.Pow(t - 1, 2));
        }
        public static float EaseInOutEtpo(float t)
        {
            return t == 0
              ? 0
              : t == 1
              ? 1
              : t < 0.5 ? MathF.Pow(2, 20 * t - 10) / 2
              : (2 - MathF.Pow(2, -20 * t + 10)) / 2;
        }
        public static float EaseInOutQuad(float t)
        {
            return t < 0.5f ? 2 * t * t : 1 - MathF.Pow(-2 * t + 2, 2) / 2;
        }
        public static float EaseOutBounce(float t)
        {
            const float n1 = 7.5625f;
            const float d1 = 2.75f;

            if (t < 1 / d1)
            {
                return n1 * t * t;
            }
            else if (t < 2 / d1)
            {
                return n1 * (t -= 1.5f / d1) * t + 0.75f;
            }
            else if (t < 2.5 / d1)
            {
                return n1 * (t -= 2.25f / d1) * t + 0.9375f;
            }
            else
            {
                return n1 * (t -= 2.625f / d1) * t + 0.984375f;
            }
        }
        public static float EaseInOutBounce(float t)
        {
            return t < 0.5 ? (1 - EaseOutBounce(1 - 2 * t)) / 2
                : (1 + EaseOutBounce(2 * t - 1)) / 2;
        }
    }
}
