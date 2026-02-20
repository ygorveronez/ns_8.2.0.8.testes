using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;
namespace Repositorio.Embarcador.ConfiguracaoContabil
{
    public class ConfiguracaoContaContabilProvisao : Repositorio.RepositorioBase<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabilProvisao>
    {
        public ConfiguracaoContaContabilProvisao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabilProvisao> BuscarPorConfiguracaoContabil(int configuracaoContabil)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabilProvisao>();
            var result = from obj in query where obj.ConfiguracaoContaContabil.Codigo == configuracaoContabil select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabilProvisao> BuscarTodas()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabilProvisao>();
            var result = from obj in query select obj;
            return result
                .Fetch(obj => obj.PlanoConta)
                .ToList();
        }
        public IList<Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabilProvisao> ConsultarConfiguracaoContaContabilProvisao()
        {
            var consulta = this.SessionNHiBernate.CreateSQLQuery(QueryConsultarConfiguracaoContaContabilProvisao());

            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabilProvisao)));

            return consulta.SetTimeout(600).List<Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabilProvisao>();
        }

        #region Métodos Privados

        private string QueryConsultarConfiguracaoContaContabilProvisao()
        {
            string sql;


            sql = @"select 
                        CCP_CODIGO as Codigo,
                        CCC_CODIGO as CodigoConfiguracaoContaContabil,
                        configuracaoContaContabilProvisao.PLA_CODIGO as CodigoPlanoConta,
                        CCP_TIPO_CONTA_CONTABIL as TipoContaContabil,
                        CCP_TIPO_CONTABILIZACAO as TipoContabilizacao,
                        configuracaoContaContabilProvisao.CCP_COMPONENTES_DE_FRETE_DO_TIPO_DESCONTO_NAO_DEVEM_SOMAR_NA_PROVISAO as ComponentesDeFreteDoTipoDescontoNaoDevemSomarNaProvisao";

            sql += @"   from T_CONFIGURACAO_CONTA_CONTABIL_PROVISAO as configuracaoContaContabilProvisao
                        left join T_PLANO_DE_CONTA planoConta on planoConta.PLA_CODIGO = configuracaoContaContabilProvisao.PLA_CODIGO";

            return sql;
        }

        #endregion

    }
}
