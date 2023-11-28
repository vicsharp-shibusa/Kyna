namespace Kyna.Common.Abstractions;

public interface IAppConfig
{
    string AppName { get; }
    string AppVersion { get; }
    bool Verbose { get; set; }
    bool ShowHelp { get; set; }
}
