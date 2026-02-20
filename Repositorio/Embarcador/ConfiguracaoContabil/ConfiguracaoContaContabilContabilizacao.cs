using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
namespace Repositorio.Embarcador.ConfiguracaoContabil
{
    public class ConfiguracaoContaContabilContabilizacao : Repositorio.RepositorioBase<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabilContabilizacao>
    {
        public ConfiguracaoContaContabilContabilizacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabilContabilizacao> BuscarPorConfiguracaoContabil(int configuracaoContabil)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabilContabilizacao>();
            var result = from obj in query where obj.ConfiguracaoContaContabil.Codigo == configuracaoContabil select obj;
            return result.ToList();
        }
        public List<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabilContabilizacao> BuscarTodas()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabilContabilizacao>();
            var result = from obj in query select obj;
            return result
                .Fetch(obj => obj.PlanoConta)
                .Fetch(obj => obj.PlanoContaContraPartidaProvisao).ToList();
        }
        public IList<Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabilContabilizacao> ConsultarConfiguracaoContaContabilContabilizacao()
        {
            var consulta = this.SessionNHiBernate.CreateSQLQuery(QueryConsultarConfiguracaoContaContabilContabilizacao());

            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabilContabilizacao)));

            return consulta.SetTimeout(600).List<Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabilContabilizacao>();
        }

        #region Métodos Privados

        private string QueryConsultarConfiguracaoContaContabilContabilizacao()
        {
            string sql;


            sql = @"select 
                        CCT_CODIGO as Codigo,
                        CCC_CODIGO as CodigoConfiguracaoContaContabil,
                        configuracaoContaContabilContabilizacao.PLA_CODIGO as CodigoPlanoConta,
                        PLA_DESCRICAO as PlanoContaDescricao,
                        PLA_CODIGO_CONTRA_PARTIDA_PROVISAO as CodigoPlanoContaContraPartidaProvisao,
                        CCT_TIPO_CONTA_CONTABIL as TipoContaContabil,
                        CCT_TIPO_CONTABILIZACAO as TipoContabilizacao,
                        PLA_PLANO_CONTABILIDADE as PlanoContabilidade,
                        CCT_COMPONENTES_DE_FRETE_DO_TIPO_DESCONTO_NAO_DEVEM_SOMAR_NA_CONTABILIZACAO as ComponentesDeFreteDoTipoDescontoNaoDevemSomarNaContabilizacao";

            sql += @"   from T_CONFIGURACAO_CONTA_CONTABIL_CONTABILIZACAO as configuracaoContaContabilContabilizacao
                        left join T_PLANO_DE_CONTA planoConta on planoConta.PLA_CODIGO = configuracaoContaContabilContabilizacao.PLA_CODIGO";

            return sql;
        }

        #endregion

    }
}
