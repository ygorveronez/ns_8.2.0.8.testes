using System.Linq;
using System.Threading;

namespace Repositorio.Embarcador.Configuracoes
{
    public class ConfiguracaoContratoFreteTerceiro : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoContratoFreteTerceiro>
    {
        public ConfiguracaoContratoFreteTerceiro(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public ConfiguracaoContratoFreteTerceiro(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoContratoFreteTerceiro BuscarConfiguracaoPadrao()
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoContratoFreteTerceiro> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoContratoFreteTerceiro>();

            return query.FirstOrDefault();
        }
    }
}
