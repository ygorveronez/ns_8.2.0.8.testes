using System.Linq;

namespace Repositorio.Embarcador.Configuracoes
{
    public class ConfiguracaoEncerramentoAutomaticoMDFe : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoEncerramentoAutomaticoMDFe>
    {
        public ConfiguracaoEncerramentoAutomaticoMDFe(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoEncerramentoAutomaticoMDFe BuscarConfiguracaoPadrao()
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoEncerramentoAutomaticoMDFe> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoEncerramentoAutomaticoMDFe>();

            return query.FirstOrDefault();
        }
    }
}
