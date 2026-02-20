using System.Linq;

namespace Repositorio.Embarcador.Configuracoes
{
    public class ConfiguracaoAtendimentoAutomatico : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoAtendimentoAutomatico>
    {
        public ConfiguracaoAtendimentoAutomatico(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoAtendimentoAutomatico BuscarConfiguracaoPadrao()
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoAtendimentoAutomatico> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoAtendimentoAutomatico>();

            return query.FirstOrDefault();
        }
    }
}
