namespace backend;
public static class Utils
{

    // Safely to read environment variables, throws an exception if missing
    public static string GetEnv(string key)
    {
        string? value = Environment.GetEnvironmentVariable(key);
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidOperationException("Missing environment variables");

        return value;
    }

    // Safely reads an environment variable with passed key and splits it on comas, returning as an array if strings
    public static string[] GetEnvArray(string key)
    {
        string value = GetEnv(key);
        string[] values = value.Split(",");

        // trim all strings
        for(int i = 0; i < values.Length; i++)
        {
            values[i] = values[i].Trim();
        }

        return values;
    }
}