using Kyna.Common.Abstractions;
using System.Text;

namespace Kyna.ApplicationServices.Helpers;

public static class CliHelper
{
    public static class Keys
    {
        public const string Logs = "Logs";
    }

    public static CliArg[] GetDefaultArgDescriptions()
    {
        return new CliArg[] {
            new CliArg(new string[] {"-?","?","-h","--help" },new string[0],false,"Show this help."),
            new CliArg(new string[] {"-v","--verbose" },new string[0],false,"Turn on verbose communication.")
        };
    }

    public static string[] HydrateDefaultAppConfig(string[] args, IAppConfig config)
    {
        List<string> remainingArgs = new List<string>(args.Length + 1);

        for (int i = 0; i < args.Length; i++)
        {
            string argument = args[i].ToLower();

            switch (argument)
            {
                case "?":
                case "-?":
                case "--help":
                    config.ShowHelp = true;
                    break;
                case "-v":
                case "--verbose":
                    config.Verbose = true;
                    break;
                default:
                    remainingArgs.Add(args[i]);
                    break;
            }
        }

        return remainingArgs.ToArray();
    }

    public static string FormatArguments(CliArg[] cliArgs)
    {
        List<KeyValuePair<string, string>> args = new(cliArgs.Length + 1);

        var requiredArgs = cliArgs.Where(a => a.Required).ToArray();
        var optionalArgs = cliArgs.Except(requiredArgs).ToArray();

        foreach (var arg in requiredArgs)
        {
            args.Add(arg.AsKeyValuePair());
        }

        foreach (var arg in optionalArgs)
        {
            args.Add(arg.AsKeyValuePair());
        }

        var keyWidth = 1 + args.MaxBy(a => a.Key.Length).Key.Length;

        StringBuilder result = new();

        foreach (var arg in args)
        {
            result.AppendLine($"{arg.Key.PadRight(keyWidth)}\t{arg.Value}");
        }

        return result.ToString();
    }
}

public struct CliArg
{
    public string[] Args;
    public string[] SubArgs;
    public bool Required;
    public string Description;

    public CliArg(string[] args, string[] subArgs, bool required, string description)
    {
        Args = args.OrderBy(a => a.Length).ThenBy(a => a).ToArray();
        SubArgs = subArgs;
        Required = required;
        Description = description;
    }

    public KeyValuePair<string, string> AsKeyValuePair()
    {
        string args = string.Join('|', Args);
        string subArgs = SubArgs.Any() ? string.Join(' ', SubArgs.Select(a => $"<{a}>")) : string.Empty;

        string exp = $"{args} {subArgs}".Trim();
        if (!Required)
        {
            exp = $"[{exp}]";
        }

        return new KeyValuePair<string, string>(exp, Description);
    }
}
