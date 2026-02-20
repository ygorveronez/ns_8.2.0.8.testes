using MongoDB.Driver;

namespace Dominio.Interfaces.Repositorios
{
    public interface IMongoDbContext
    {
        IMongoDatabase ObterCollection();
        IMongoDatabase ObterCollectionAdmin();
    }
}
