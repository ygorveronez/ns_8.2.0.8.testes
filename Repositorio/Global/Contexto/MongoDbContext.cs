using Dominio.Interfaces.Database;
using Dominio.Interfaces.Repositorios;
using MongoDB.Driver;
using System;

namespace Repositorio.Global.Contexto
{
    public class MongoDbContext : IMongoDbContext
    {
        #region Propriedades Privadas

        private readonly IMongoDatabase _database;
        private readonly IMongoDatabase _databaseAdmin;

        #endregion Propriedades Privadas

        #region Construtores

        public MongoDbContext(ITenantService tenantService, MongoClient mongoClient)
        {
            try
            {
                var config = tenantService.ObterMongoDbConfiguracao();

                _database = mongoClient.GetDatabase(config.DatabaseName);
                _databaseAdmin = mongoClient.GetDatabase("AdminMultisoftware");
            }
            catch (Exception ex)
            {
                Infrastructure.Services.Logging.Logger.Current.Error($"[Arquitetura-CatchNoAction] Erro ao inicializar contexto MongoDB: {ex}", "CatchNoAction");
            }
        }

        #endregion Construtores

        public IMongoDatabase ObterCollection() => _database;
        public IMongoDatabase ObterCollectionAdmin() => _databaseAdmin;
    }
}
