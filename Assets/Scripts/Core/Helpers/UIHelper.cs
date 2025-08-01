using System;
using System.Collections.Generic;
using TMPro;

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

    public static bool ValidateMinMaxAsFloat((string min, string max) value, out (float min, float max) result)
    {
        result = default;
        if (string.IsNullOrWhiteSpace(value.min) || string.IsNullOrWhiteSpace(value.max))
            return false;

        float.TryParse(value.min, out float min);
        float.TryParse(value.min, out float max);
        
        result = (min, max);
        return true;
    }


    public static List<TMP_Dropdown.OptionData> CreateOptionListFromEnum<T>() where T : Enum
    {
        var options = new List<TMP_Dropdown.OptionData>();

        foreach (var item in Enum.GetValues(typeof(T)))
        {
            options.Add(new TMP_Dropdown.OptionData(item.ToString()));
        }

        return options;
    }

    public static T GetEnumValueFromOption<T>(int option) where T : Enum
    {
        foreach (var item in Enum.GetValues(typeof(T)))
        {
            if (Convert.ToInt32(item) == option)
                return (T)item;
        }

        return default;
    }
}