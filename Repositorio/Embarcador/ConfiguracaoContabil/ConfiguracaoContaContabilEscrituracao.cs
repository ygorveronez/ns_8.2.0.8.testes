using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
namespace Repositorio.Embarcador.ConfiguracaoContabil
{
    public class ConfiguracaoContaContabilEscrituracao : Repositorio.RepositorioBase<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabilEscrituracao>
    {
        public ConfiguracaoContaContabilEscrituracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabilEscrituracao> BuscarPorConfiguracaoContabil(int configuracaoContabil)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabilEscrituracao>();
            var result = from obj in query where obj.ConfiguracaoContaContabil.Codigo == configuracaoContabil select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabilEscrituracao> BuscarTodas()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabilEscrituracao>();
            var result = from obj in query select obj;
            return result
                .Fetch(obj => obj.PlanoConta)
                .ToList();
        }
        public IList<Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabilEscrituracao> ConsultarConfiguracaoContaContabilEscrituracao()
        {
            var consulta = this.SessionNHiBernate.CreateSQLQuery(QueryConsultarConfiguracaoContaContabilEscrituracao());

            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabilEscrituracao)));

            return consulta.SetTimeout(600).List<Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabilEscrituracao>();
        }

        #region Métodos Privados

        private string QueryConsultarConfiguracaoContaContabilEscrituracao()
        {
            string sql;


            sql = @"select 
                        CCE_CODIGO as Codigo,
                        CCC_CODIGO as CodigoConfiguracaoContaContabil,
                        CCE_TIPO_CONTA_CONTABIL as TipoContaContabil,
                        DCB_SEMPRE_GERAR_REGISTRO as SempreGerarRegistro";

            sql += @"   from T_CONFIGURACAO_CONTA_CONTABIL_ESCRITURACAO as configuracaoContaContabilEscrituracao";

            return sql;
        }

        #endregion

    }
}