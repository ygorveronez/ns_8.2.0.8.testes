using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Configuracoes
{
    public class ConfiguracaoFinanceiraContratoFreteTerceirosTipoOperacao : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraContratoFreteTerceirosTipoOperacao>
    {
        public ConfiguracaoFinanceiraContratoFreteTerceirosTipoOperacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraContratoFreteTerceirosTipoOperacao BuscarPrimeiraConfiguracao()
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraContratoFreteTerceirosTipoOperacao> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraContratoFreteTerceirosTipoOperacao>();
            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraContratoFreteTerceirosTipoOperacao> BuscarPorTipoOperacao(int codigoConfiguracaoFinanceira, int codigoTipoOperacao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraContratoFreteTerceirosTipoOperacao> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraContratoFreteTerceirosTipoOperacao>();

            query = query.Where(o => o.ConfiguracaoFinanceiraContratoFreteTerceiros.Codigo == codigoConfiguracaoFinanceira);

            if (codigoTipoOperacao > 0)
                query = query.Where(o => o.TipoOperacao == null || o.TipoOperacao.Codigo == codigoTipoOperacao);
            else
                query = query.Where(o => o.TipoOperacao == null);

            return query.Fetch(o => o.TipoMovimentoReversaoValorAbastecimento)
                        .Fetch(o => o.TipoMovimentoReversaoValorAdiantamento)
                        .Fetch(o => o.TipoMovimentoReversaoValorINSS)
                        .Fetch(o => o.TipoMovimentoReversaoValorIRRF)
                        .Fetch(o => o.TipoMovimentoReversaoValorLiquido)
                        .Fetch(o => o.TipoMovimentoReversaoValorSaldo)
                        .Fetch(o => o.TipoMovimentoReversaoValorSENAT)
                        .Fetch(o => o.TipoMovimentoReversaoValorSEST)
                        .Fetch(o => o.TipoMovimentoReversaoValorTarifaSaque)
                        .Fetch(o => o.TipoMovimentoReversaoValorTarifaTransferencia)
                        .Fetch(o => o.TipoMovimentoReversaoValorTotal)
                        .Fetch(o => o.TipoMovimentoValorAbastecimento)
                        .Fetch(o => o.TipoMovimentoValorAdiantamento)
                        .Fetch(o => o.TipoMovimentoValorINSS)
                        .Fetch(o => o.TipoMovimentoValorIRRF)
                        .Fetch(o => o.TipoMovimentoValorLiquido)
                        .Fetch(o => o.TipoMovimentoValorSaldo)
                        .Fetch(o => o.TipoMovimentoValorSENAT)
                        .Fetch(o => o.TipoMovimentoValorSEST)
                        .Fetch(o => o.TipoMovimentoValorTarifaSaque)
                        .Fetch(o => o.TipoMovimentoValorTarifaTransferencia)
                        .Fetch(o => o.TipoMovimentoValorTotal)
                        .Fetch(o => o.TipoOperacao).ToList();
        }
    }
}
