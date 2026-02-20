using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;


namespace Repositorio.Embarcador.Documentos
{
    public class EnvioDocumentacaoLote : RepositorioBase<Dominio.Entidades.Embarcador.Documentos.EnvioDocumentacaoLote>
    {
        public EnvioDocumentacaoLote(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Documentos.EnvioDocumentacaoLote BuscarPorCodigo(long codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.EnvioDocumentacaoLote>();

            query = query.Where(o => o.Codigo == codigo);

            return query.FirstOrDefault();
        }

        public bool ExisteDocumentacaoLotePorViagem(int codigoViagem)
        {
            IQueryable<Dominio.Entidades.Embarcador.Documentos.EnvioDocumentacaoLote> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.EnvioDocumentacaoLote>();

            query = query.Where(o => o.PedidoViagemNavio.Codigo == codigoViagem);

            return query.Any();
        }

        public List<long> BuscarCodigosPorSituacao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoImpressaoLote tipoImpressaoLote, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEnvioDocumentacaoLote situacao, string propOrdenar, string dirOrdenar, int inicio, int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.EnvioDocumentacaoLote>();

            query = query.Where(o => o.Situacao == situacao && o.TipoImpressaoLote == tipoImpressaoLote);

            return query.Select(o => o.Codigo).Skip(inicio).Take(limite).ToList();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Documentos.EnvioAutomaticoDocumentacao> BuscarDocumentacaoAutomaticaPendente()
        {
            string sql = @"SELECT Schedule.PVS_CODIGO CodigoSchedule, Schedule.PVN_CODIGO CodigoViagem, Terminal.TTI_CODIGO CodigoTerminal
                FROM T_TIPO_TERMINAL_IMPORTACAO Terminal
                JOIN T_PEDIDO_VIAGEM_NAVIO_SCHEDULE Schedule on Schedule.TTI_CODIGO_ATRACACAO = Terminal.TTI_CODIGO
                where Terminal.TTI_QUANTIDADE_DIAS_ENVIO_DOCUMENTACAO > 0 and Terminal.TTI_QUANTIDADE_DIAS_ENVIO_DOCUMENTACAO is not null
                and (Schedule.PVS_ENVIOU_DOCUMENTACAO_AUTOMATICA = 0 OR Schedule.PVS_ENVIOU_DOCUMENTACAO_AUTOMATICA IS NULL)
                AND Schedule.PVS_DATA_PREVISAO_CHEGADA_NAVIO IS NOT NULL 
                AND DATEDIFF(DAY, '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:sss") + "', Schedule.PVS_DATA_PREVISAO_CHEGADA_NAVIO) <= Terminal.TTI_QUANTIDADE_DIAS_ENVIO_DOCUMENTACAO";
            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Documentos.EnvioAutomaticoDocumentacao)));

            return query.SetTimeout(6000).List<Dominio.ObjetosDeValor.Embarcador.Documentos.EnvioAutomaticoDocumentacao>();
        }

    }
}
