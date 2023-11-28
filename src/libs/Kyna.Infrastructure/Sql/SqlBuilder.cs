namespace Kyna.Infrastructure.Sql;

public enum DatabaseEngine
{
    None = 0,
    PostgreSql,
    MsSqlServer
}

internal partial class SqlBuilder
{
    private const string NotImplemented = "Selected database engine not yet implemented; see argument to SqlBuilder constructor.";

    private readonly DatabaseEngine engine;

    public SqlBuilder(DatabaseEngine engine = DatabaseEngine.PostgreSql)
    {
        this.engine = engine;
    }
}
