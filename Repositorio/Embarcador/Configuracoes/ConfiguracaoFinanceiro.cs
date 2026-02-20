using System.Linq;

namespace Repositorio.Embarcador.Configuracoes
{
    public class ConfiguracaoFinanceiro : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro>
    {
        public ConfiguracaoFinanceiro(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro BuscarConfiguracaoPadrao()
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro>();

            return query.FirstOrDefault();
        }

        public bool ExisteManterValorMoedaConfirmarDocumentosFatura()
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro>();

            return query.Select(o => (bool?)o.ManterValorMoedaConfirmarDocumentosFatura).FirstOrDefault() ?? false;
        }
    }
}
