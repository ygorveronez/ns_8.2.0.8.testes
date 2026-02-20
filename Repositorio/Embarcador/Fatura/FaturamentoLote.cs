using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Fatura
{
    public class FaturamentoLote : RepositorioBase<Dominio.Entidades.Embarcador.Fatura.FaturamentoLote>
    {
        public FaturamentoLote(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Fatura.FaturamentoLote BuscarPorCodigo(long codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturamentoLote>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public bool ExisteFaturamentoLotePorViagem(int codigoViagem)
        {
            IQueryable<Dominio.Entidades.Embarcador.Fatura.FaturamentoLote> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturamentoLote>();

            query = query.Where(o => o.PedidoViagemNavio.Codigo == codigoViagem);

            return query.Any();
        }

        public List<long> BuscarCodigosPorSituacao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFaturamentoLote tipo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEnvioDocumentacaoLote situacao, string propOrdenar, string dirOrdenar, int inicio, int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturamentoLote>();

            query = query.Where(o => o.Situacao == situacao && o.Tipo == tipo);

            return query.Select(o => o.Codigo).Skip(inicio).Take(limite).ToList();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Fatura.FaturamentoAutomatico> BuscarFaturamentoAutomaticoPendente(DateTime dataHoraAtual, int? delay)
        {
            string sql = $@"select Schedule.PVS_CODIGO Codigo, Schedule.PVN_CODIGO Viagem, Schedule.POT_CODIGO_ATRACACAO Porto, Schedule.TTI_CODIGO_ATRACACAO Terminal, Porto.POT_QUANTIDADE_HORAS_FATURAMENTO_AUTOMATICO QuantidadeHoras,
                dateadd(HOUR, Porto.POT_QUANTIDADE_HORAS_FATURAMENTO_AUTOMATICO, Schedule.PVS_DATA_PREVISAO_SAIDA_NAVIO) DataAFaturar, Empresa.EMP_FUSO_HORARIO Fuso,
                Schedule.PVS_DATA_PREVISAO_SAIDA_NAVIO DataSaidaNavio
                from T_PEDIDO_VIAGEM_NAVIO_SCHEDULE Schedule
                join T_PORTO Porto on Porto.POT_CODIGO = Schedule.POT_CODIGO_ATRACACAO and Porto.POT_QUANTIDADE_HORAS_FATURAMENTO_AUTOMATICO > 0 and Porto.POT_QUANTIDADE_HORAS_FATURAMENTO_AUTOMATICO is not null
                join T_EMPRESA Empresa on Empresa.EMP_CODIGO = Porto.EMP_CODIGO
                where (Schedule.PVS_GEROU_FATURAMENTO_AUTOMATICO = 0 or Schedule.PVS_GEROU_FATURAMENTO_AUTOMATICO is null)                
                and Schedule.PVS_DATA_PREVISAO_SAIDA_NAVIO is not null
                and Schedule.PVS_STATUS = 1
                and Schedule.PVS_ETS_CONFIRMADO = 1
                and dateadd(Minute, {delay ?? 0}, dateadd(HOUR, Porto.POT_QUANTIDADE_HORAS_FATURAMENTO_AUTOMATICO, Schedule.PVS_DATA_PREVISAO_SAIDA_NAVIO)) <= '{dataHoraAtual.ToString("yyyy-MM-dd HH:mm:sss")}'";

            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Fatura.FaturamentoAutomatico)));

            return query.SetTimeout(6000).List<Dominio.ObjetosDeValor.Embarcador.Fatura.FaturamentoAutomatico>();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Fatura.FaturamentoLoteNotificacao> BuscarFaturamentoAutomaticoPendenteNotificacao(bool envioAgrupado)
        {
            string sql = @"select distinct lote.fal_codigo codigo, fatura.fat_codigo fatura, fatura.cli_cgccpf cliente, fatura.grp_codigo grupo
                from t_faturamento_lote lote
                join t_faturamento_lote_fatura faturas on faturas.fal_codigo = lote.fal_codigo
                join t_fatura fatura on fatura.fat_codigo = faturas.fat_codigo
                join T_FATURA_DOCUMENTO docs on docs.FAT_CODIGO = fatura.FAT_CODIGO
                left outer join t_grupo_pessoas grupo on grupo.grp_codigo = fatura.grp_codigo
                left outer join t_cliente cliente on cliente.cli_cgccpf = fatura.cli_cgccpf
                left outer join t_acordo_faturamento_cliente acordogrupo on acordogrupo.grp_codigo = grupo.grp_codigo
				left outer join t_acordo_faturamento_cliente acordocliente on acordocliente.cli_cgccpf = cliente.cli_cgccpf  
                where lote.fal_situacao = 2 and (lote.fal_notificado_operador = 0 or lote.fal_notificado_operador is null)
                and fatura.fat_situacao = 2
                and fatura.fat_codigo not in (select integracao.fat_codigo from t_fatura_integracao integracao
                join t_faturamento_lote_fatura faturas on faturas.fat_codigo = integracao.fat_codigo
                where integracao.tpi_codigo = 5
                and faturas.fal_codigo = lote.fal_codigo
                and integracao.fai_situacao in (0,2,3))";

            if (envioAgrupado)
                sql += " and (acordogrupo.afc_cabotagem_nao_enviar_email_fatura_automaticamente = 1 or acordocliente.afc_cabotagem_nao_enviar_email_fatura_automaticamente = 1) ";
            //else
            //    sql += " and (acordogrupo.afc_cabotagem_nao_enviar_email_fatura_automaticamente = 0 or acordogrupo.afc_cabotagem_nao_enviar_email_fatura_automaticamente is null) and (acordocliente.afc_cabotagem_nao_enviar_email_fatura_automaticamente = 0 or acordocliente.afc_cabotagem_nao_enviar_email_fatura_automaticamente is null) ";

            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Fatura.FaturamentoLoteNotificacao)));

            return query.SetTimeout(6000).List<Dominio.ObjetosDeValor.Embarcador.Fatura.FaturamentoLoteNotificacao>();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Fatura.FaturamentoLoteNotificacao> BuscarCancelamentoFaturamentoPendenteNotificacao()
        {
            string sql = @"select Lote.FAL_CODIGO Codigo, Fatura.FAT_CODIGO Fatura, Fatura.CLI_CGCCPF Cliente, Fatura.GRP_CODIGO Grupo
                from T_FATURAMENTO_LOTE Lote
                JOIN T_FATURAMENTO_LOTE_FATURA Faturas on Faturas.FAL_CODIGO = Lote.FAL_CODIGO
                JOIN T_FATURA Fatura on Fatura.FAT_CODIGO = Faturas.FAT_CODIGO
                LEFT OUTER JOIN T_GRUPO_PESSOAS Grupo on Grupo.GRP_CODIGO = Fatura.GRP_CODIGO
                LEFT OUTER JOIN T_CLIENTE Cliente on Cliente.CLI_CGCCPF = Fatura.CLI_CGCCPF                
                WHERE Lote.FAL_SITUACAO = 2 AND (Lote.FAL_NOTIFICADO_OPERADOR = 0 OR Lote.FAL_NOTIFICADO_OPERADOR IS NULL)
                AND Fatura.FAT_SITUACAO = 3
				AND Lote.FAL_CODIGO not in 
				(SELECT EmCancelamento.FAL_CODIGO FROM T_FATURAMENTO_LOTE_FATURA EmCancelamento
					JOIN T_FATURAMENTO_LOTE LoteCancelamento on LoteCancelamento.FAL_CODIGO = EmCancelamento.FAL_CODIGO
					JOIN T_FATURA FaturaEmCancelamento on FaturaEmCancelamento.FAT_CODIGO = EmCancelamento.FAT_CODIGO
					where FaturaEmCancelamento.FAT_SITUACAO = 6)";

            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Fatura.FaturamentoLoteNotificacao)));

            return query.SetTimeout(6000).List<Dominio.ObjetosDeValor.Embarcador.Fatura.FaturamentoLoteNotificacao>();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Fatura.FaturamentoLoteNotificacao> BuscarCancelamentoFaturamentoManualPendenteNotificacao()
        {
            string sql = @"select 0 Codigo, Fatura.FAT_CODIGO Fatura, Fatura.CLI_CGCCPF Cliente, Fatura.GRP_CODIGO Grupo
                from T_FATURA Fatura 
                LEFT OUTER JOIN T_GRUPO_PESSOAS Grupo on Grupo.GRP_CODIGO = Fatura.GRP_CODIGO
                LEFT OUTER JOIN T_CLIENTE Cliente on Cliente.CLI_CGCCPF = Fatura.CLI_CGCCPF                
                WHERE Fatura.FAT_SITUACAO = 3 AND (Fatura.FAT_NOTIFICADO_OPERADOR = 0 OR Fatura.FAT_NOTIFICADO_OPERADOR IS NULL)
				AND Fatura.FUN_CODIGO_CANCELAMENTO is not null";

            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Fatura.FaturamentoLoteNotificacao)));

            return query.SetTimeout(6000).List<Dominio.ObjetosDeValor.Embarcador.Fatura.FaturamentoLoteNotificacao>();
        }

    }
}
