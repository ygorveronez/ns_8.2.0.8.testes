namespace System.Collections.Generic;

public static class DictionaryExtensions
{
    public static T GetValue<T>(this Dictionary<string, string> dict, string key)
    {
        try
        {
            if (dict.TryGetValue(key, out string content))
            {
                return (T)Convert.ChangeType(content, typeof(T));
            }
            return default(T);
        }
        catch (Exception e)
        {
            //TODO LOG
            return default(T);
        }
    }
}