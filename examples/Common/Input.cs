using System.Text.Json;
using System.Text.Json.Serialization;
using System.Diagnostics.CodeAnalysis;

namespace HybridExamples;

public static class Input
{
    public static T Read<T>(string[] args)
    {
        if (args.Length > 1) throw new ArgumentException("Usage: example [input.json]");
        string path = args.Length == 1 ? args[0] : Path.Combine(AppContext.BaseDirectory, "input.json");
        return JsonSerializer.Deserialize<T>(File.ReadAllText(path), new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow
        }) ?? throw new ArgumentException("Input is empty.");
    }

    public static void Require([DoesNotReturnIf(false)] bool condition, string message)
    {
        if (!condition) throw new ArgumentException(message);
    }

    public static void Assert(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException("Check failed: " + message);
    }

    public static bool FiniteNonnegative(double value) => double.IsFinite(value) && value >= 0;

    public static int Run(Action action)
    {
        try { action(); return 0; }
        catch (Exception ex) when (ex.GetType().FullName != "Gurobi.GRBException")
        { Console.Error.WriteLine("ERROR: " + ex.Message); return 1; }
    }
}
