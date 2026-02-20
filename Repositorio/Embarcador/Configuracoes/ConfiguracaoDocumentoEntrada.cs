using System.Linq;

namespace Repositorio.Embarcador.Configuracoes
{
    public class ConfiguracaoDocumentoEntrada : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoDocumentoEntrada>
    {
        public ConfiguracaoDocumentoEntrada(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoDocumentoEntrada BuscarConfiguracaoPadrao()
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoDocumentoEntrada> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoDocumentoEntrada>();

            return query.FirstOrDefault();
        }
    }
}
