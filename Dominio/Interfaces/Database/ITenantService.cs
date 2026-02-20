using MongoDB.Driver;

namespace Dominio.Interfaces.Database
{
    public interface ITenantService
    {
        MongoUrlBuilder ObterMongoDbConfiguracao();      

        string AdminStringConexao();

        string StringConexao();

        AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware ObterTipoServicoMultisoftware();

        AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente ObterCliente();

        Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado ObterAuditado(Dominio.ObjetosDeValor.Enumerador.OrigemAuditado origemAuditado = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.Sistema);

    }
}
