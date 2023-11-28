using Kyna.Common.Abstractions;

namespace Kyna.ApplicationServices.Helpers;
public static class CliHelper
{
    public static class Keys
    {
        public const string Logs = "Logs";
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
}
