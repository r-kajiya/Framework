using UnityEngine;

namespace Framework
{
    public static class ColorUtility
    {
        public static Color RedHA = new Color(Color.red.r, Color.red.g, Color.red.b, 0.5f);
        public static Color GreenHA = new Color(Color.green.r, Color.green.g, Color.green.b, 0.5f);
        public static Color BlueHR = new Color(Color.blue.r, Color.blue.g, Color.blue.b, 0.5f);
        public static Color CyanHR = new Color(Color.cyan.r, Color.cyan.g, Color.cyan.b, 0.5f);
        public static Color GrayHR = new Color(Color.gray.r, Color.gray.g, Color.gray.b, 0.5f);
        public static Color GreenHR = new Color(Color.green.r, Color.green.g, Color.green.b, 0.5f);
        public static Color MagentaHR = new Color(Color.magenta.r, Color.magenta.g, Color.magenta.b, 0.5f);
        public static Color WhiteHR = new Color(Color.white.r, Color.white.g, Color.white.b, 0.5f);
        public static Color YellowHR = new Color(Color.yellow.r, Color.yellow.g, Color.yellow.b, 0.5f);
        public static Color BlackHR = new Color(Color.black.r, Color.black.g, Color.black.b, 0.5f);
        public static Color None = new Color(0f, 0f, 0f, 0f);
        
        public static Color HexToColor(this string self)
        {
            if (!UnityEngine.ColorUtility.TryParseHtmlString(self, out var color)) {
                Debug.LogWarning("Unknown color code... " + self);
            }
            return color;
        }
    }
}