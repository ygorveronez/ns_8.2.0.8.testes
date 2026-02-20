using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Frete
{
    public class TabelaFreteIntegrarAlteracaoDados
    {
        public int CodigoTabelaFrete { get; set; }

        public SituacaoTabelaFreteIntegrarAlteracao Situacao { get; set; }
    }

    public class TabelaFreteIntegrarAlteracao : RepositorioBase<Dominio.Entidades.Embarcador.Frete.TabelaFreteIntegrarAlteracao>
    {
        #region Construtores

        public TabelaFreteIntegrarAlteracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Construtores

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Frete.TabelaFreteIntegrarAlteracao BuscarPendentePorTabelaFrete(int codigoTabelaFrete)
        {
            var consultaTabelaFreteIntegrarAlteracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteIntegrarAlteracao>()
                .Where(tabelaFreteAlteracao => tabelaFreteAlteracao.TabelaFrete.Codigo == codigoTabelaFrete && tabelaFreteAlteracao.Situacao == SituacaoTabelaFreteIntegrarAlteracao.PendenteIntegracao)
                .OrderByDescending(tabelaFreteAlteracao => tabelaFreteAlteracao.Numero);

            return consultaTabelaFreteIntegrarAlteracao.FirstOrDefault();
        }

        public int BuscarProximoNumeroPorTabelaFrete(int codigoTabelaFrete)
        {
            var consultaTabelaFreteIntegrarAlteracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteIntegrarAlteracao>()
                .Where(tabelaFreteAlteracao => tabelaFreteAlteracao.TabelaFrete.Codigo == codigoTabelaFrete);

            int? ultimoNumero = consultaTabelaFreteIntegrarAlteracao.Max(tabelaFreteAlteracao => (int?)tabelaFreteAlteracao.Numero);

            return ultimoNumero.HasValue ? ultimoNumero.Value + 1 : 1;
        }

        public IList<(int CodigoTabelaFrete, SituacaoTabelaFreteIntegrarAlteracao Situacao)> BuscarSituacoesIntegracaoAlteracao(List<int> codigosTabelaFrete)
        {
            string sql = $@"
                select IntegracaoAlteracao.TBF_CODIGO CodigoTabelaFrete, IntegracaoAlteracao.TIA_SITUACAO Situacao
                  from T_TABELA_FRETE_INTEGRAR_ALTERACAO IntegracaoAlteracao
                 where IntegracaoAlteracao.TBF_CODIGO in ({string.Join(", ", codigosTabelaFrete)})
                   and IntegracaoAlteracao.TIA_NUMERO = (
                           select max(_integracaoAlteracao.TIA_NUMERO)
                             from T_TABELA_FRETE_INTEGRAR_ALTERACAO _integracaoAlteracao
                            where _integracaoAlteracao.TBF_CODIGO = IntegracaoAlteracao.TBF_CODIGO
                       )";

            var consultaTabelaFreteIntegrarAlteracao = this.SessionNHiBernate.CreateSQLQuery(sql);

            consultaTabelaFreteIntegrarAlteracao.SetResultTransformer(new NHibernate.Transform.AliasToBeanConstructorResultTransformer(typeof((int CodigoTabelaFrete, SituacaoTabelaFreteIntegrarAlteracao Situacao)).GetConstructors().FirstOrDefault()));

            return consultaTabelaFreteIntegrarAlteracao.SetTimeout(600).List<(int CodigoTabelaFrete, SituacaoTabelaFreteIntegrarAlteracao Situacao)>();
        }

        #endregion Métodos Públicos
    }
}
