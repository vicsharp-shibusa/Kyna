using Kyna.ApplicationServices.Helpers;

namespace Kyna.ApplicationServices.Tests;

public class CliHelperTests
{
    [Fact]
    public void FormatArguments_Pretty()
    {
        CliArg[] args = {
            new CliArg(new string[] {"-?","?","-h","--help" },new string[0],false,"Show this help."),
            new CliArg(new string[] {"-v","--verbose" },new string[0],false,"Turn on verbose communication."),
            new CliArg(new string[] {"-c","--connection-string" },new string[] { "connection string"},true,"Set connection string."),
        };

        // required items come to the top.
        // optional items have the [] notation.
        // columns line up.
        // args are sorted by length and then alphabetically
        // (e.g., {"-?","?","-h","--help" } becomes [?|-?|-h|--help]).
        var expected = @"-c|--connection-string <connection string> 	Set connection string.
[?|-?|-h|--help]                           	Show this help.
[-v|--verbose]                             	Turn on verbose communication.
";

        var sut = CliHelper.FormatArguments(args);

        Assert.NotNull(sut);
        Assert.Equal(expected, sut);
    }
}
