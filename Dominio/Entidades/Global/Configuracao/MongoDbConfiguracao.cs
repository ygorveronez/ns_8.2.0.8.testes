namespace Dominio.Entidades.Global.Configuracao
{
    public class MongoDbConfiguracao
    {
        public string ConnectionString { get; set; }

        public string Database { get; set; }

        public bool IsSSL { get; set; }
    }
}
