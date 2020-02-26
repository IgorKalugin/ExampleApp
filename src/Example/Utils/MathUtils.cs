using Xamarin.Forms;

namespace Example.Utils
{
    public static class MathUtils
    {
        /// <summary>
        /// Performs linear interpolation from first value to second based on alpha
        /// </summary>
        /// <param name="first">First value</param>
        /// <param name="second">Second value</param>
        /// <param name="alpha">Should be in range from 0 to 1 including</param>
        public static double Lerp(double first, double second, double alpha)
        {
            return first * (1 - alpha) + second * alpha;
        }
        
        /// <summary>
        /// Performs linear interpolation from first color to second color based on alpha
        /// </summary>
        /// <param name="first">First color</param>
        /// <param name="second">Second color</param>
        /// <param name="alpha">Should be in range from 0 to 1 including</param>
        public static Color Lerp(Color first, Color second, double alpha)
        {
            var r = Lerp(first.R, second.R, alpha);
            var g = Lerp(first.G, second.G, alpha);
            var b = Lerp(first.B, second.B, alpha);
            var a = Lerp(first.A, second.A, alpha);
            return new Color(r, g, b, a);
        }
    }
}