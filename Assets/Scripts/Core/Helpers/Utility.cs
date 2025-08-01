using UnityEngine;

public static class ColorUtils
{
    public static bool GetColorFromHex(string hex, out Color color)
    {
        return ColorUtility.TryParseHtmlString(hex, out color);
    }
}
