using UnityEngine;


public static class UIHelper
{
    public static bool ValidateInputAsInt(string value, out int result)
    {
        result = 0;
        return !string.IsNullOrWhiteSpace(value) && int.TryParse(value, out result);
    }
    
    public static bool ValidateInputAsFloat(string value, out float result)
    {
        result = 0f;
        return !string.IsNullOrWhiteSpace(value) && float.TryParse(value, out result);
    }
}