namespace SGT.BackgroundWorkers.Utils
{ 
public enum EnvironmentType
{
    Homolog = 0,
    Production = 1
}

    public class CompanySettings
    {
        public string DatabaseConnectionString { get; private set; }
        public EnvironmentType Environment { get; private set; }
        public string RedisConnectionString { get; private set; }

        public void SetRedisConnectionString(string redisConnectionString)
        {
            if (string.IsNullOrWhiteSpace(RedisConnectionString))
                RedisConnectionString = redisConnectionString;
        }
        public void SetDatabaseConnectionString(string databaseConnectionString)
        {
            if (string.IsNullOrWhiteSpace(DatabaseConnectionString))
                DatabaseConnectionString = databaseConnectionString;
        }

        public void SetEnvironment(EnvironmentType environment)
        {
            Environment = environment;
        }

    }
}