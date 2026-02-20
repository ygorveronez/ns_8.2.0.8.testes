using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Configuracoes
{
    public class ConfiguracaoFinanceiroIntervaloTempoDocumentoEmitidoEscrituracao : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiroIntervaloTempoDocumentoEmitidoEscrituracao>
    {
        public ConfiguracaoFinanceiroIntervaloTempoDocumentoEmitidoEscrituracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiroIntervaloTempoDocumentoEmitidoEscrituracao> BuscarPorCondiguracaoFinanceira(int codigoConfiguracaoFinanceiro)
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiroIntervaloTempoDocumentoEmitidoEscrituracao> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiroIntervaloTempoDocumentoEmitidoEscrituracao>();

            query = query.Where(x => x.ConfiguracaoFinanceiro.Codigo == codigoConfiguracaoFinanceiro);

            return query.ToList();
        }
    }
}
