using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Logistica.Consulta
{
    sealed class ConsultaAgendaCancelada : Embarcador.Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioAgendaCancelada>
    {
        #region Construtores

        public ConsultaAgendaCancelada() : base(tabela: "T_CARGA_CANCELAMENTO as CargaCancelamento") { }

        #endregion

        #region Métodos Privados 

        private void SetarJoinsAgendamentoColeta(StringBuilder joins)
        {
            if (!joins.Contains(" AgendamentoColeta "))
                joins.Append("JOIN T_AGENDAMENTO_COLETA AgendamentoColeta on AgendamentoColeta.CAR_CODIGO = CargaCancelamento.CAR_CODIGO ");
        }

        private void SetarJoinsSolicitante(StringBuilder joins)
        {
            if (!joins.Contains(" Solicitante "))
                joins.Append("LEFT JOIN T_FUNCIONARIO Solicitante on Solicitante.FUN_CODIGO = CargaCancelamento.FUN_CODIGO ");
        }

        private void SetarJoinsCarga(StringBuilder joins)
        {
            SetarJoinsAgendamentoColeta(joins);
            if (!joins.Contains(" Carga "))
                joins.Append("LEFT JOIN T_CARGA Carga ON Carga.CAR_CODIGO = AgendamentoColeta.CAR_CODIGO ");
        }

        private void SetarJoinsFilial(StringBuilder joins)
        {
            SetarJoinsCarga(joins);
            if (!joins.Contains(" Filial "))
                joins.Append("LEFT JOIN T_FILIAL Filial on Filial.FIL_CODIGO = Carga.FIL_CODIGO ");
        }

        private void SetarJoinsRemetente(StringBuilder joins)
        {
            SetarJoinsAgendamentoColeta(joins);
            if (!joins.Contains(" Remetente "))
                joins.Append("LEFT JOIN T_CLIENTE Remetente on Remetente.CLI_CGCCPF = AgendamentoColeta.REM_CODIGO ");
        }

        private void SetarJoinsCargaJanelaDescarregamento(StringBuilder joins)
        {
            SetarJoinsCarga(joins);
            if (!joins.Contains(" CargaJanelaDescarregamento "))
                joins.Append("JOIN T_CARGA_JANELA_DESCARREGAMENTO CargaJanelaDescarregamento on CargaJanelaDescarregamento.CAR_CODIGO = Carga.CAR_CODIGO and isnull(CargaJanelaDescarregamento.CJD_CANCELADA, 0) = 0 ");
        }

        private void SetarJoinsCentroDescarregamento(StringBuilder joins)
        {
            SetarJoinsCargaJanelaDescarregamento(joins);
            if (!joins.Contains(" CentroDescarregamento "))
                joins.Append("JOIN T_CENTRO_DESCARREGAMENTO CentroDescarregamento on CargaJanelaDescarregamento.CED_CODIGO = CentroDescarregamento.CED_CODIGO ");
        }

        private void SetarJoinsDestinatario(StringBuilder joins)
        {
            SetarJoinsCentroDescarregamento(joins);
            if (!joins.Contains(" Destinatario "))
                joins.Append("JOIN T_CLIENTE Destinatario on Destinatario.CLI_CGCCPF = CentroDescarregamento.CLI_CGCCPF_DESTINATARIO ");
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioAgendaCancelada filtroPesquisa)
        {
            switch (propriedade)
            {
                case "Codigo":
                    if (!select.Contains(" Codigo, "))
                    {
                        select.Append("CargaCancelamento.CAC_CODIGO as Codigo, ");
                        groupBy.Append("CargaCancelamento.CAC_CODIGO, ");
                    }
                    break;

                case "DataAgenda":
                    if (!select.Contains(" DataAgenda, "))
                    {
                        select.Append("CargaJanelaDescarregamento.CJD_INICIO_DESCARREGAMENTO as DataAgenda, ");
                        groupBy.Append("CargaJanelaDescarregamento.CJD_INICIO_DESCARREGAMENTO, ");

                        SetarJoinsCargaJanelaDescarregamento(joins);
                    }
                    break;

                case "DataCancelamento":
                    if (!select.Contains(" DataCancelamento, "))
                    {
                        select.Append("CargaCancelamento.CAC_DATA_CANCELAMENTO as DataCancelamento, ");
                        groupBy.Append("CargaCancelamento.CAC_DATA_CANCELAMENTO, ");
                    }
                    break;

                case "Senha":
                    if (!select.Contains(" Senha, "))
                    {
                        select.Append("AgendamentoColeta.ACO_SENHA as Senha, ");
                        groupBy.Append("AgendamentoColeta.ACO_SENHA, ");

                        SetarJoinsAgendamentoColeta(joins);
                    }
                    break;

                case "Fornecedor":
                    if (!select.Contains(" Fornecedor, "))
                    {
                        select.Append("Remetente.CLI_NOME as Fornecedor, ");
                        groupBy.Append("Remetente.CLI_NOME, ");

                        SetarJoinsRemetente(joins);
                    }
                    break;

                case "Destinatario":
                    if (!select.Contains(" Destinatario, "))
                    {
                        select.Append("Destinatario.CLI_NOME + ' (' + CAST(CAST(Destinatario.CLI_CGCCPF AS bigint) as nvarchar(18)) + ')' as Destinatario, ");
                        groupBy.Append("Destinatario.CLI_NOME, Destinatario.CLI_CGCCPF, ");

                        SetarJoinsDestinatario(joins);
                    }
                    break;

                case "Solicitante":
                    if (!select.Contains(" Solicitante, "))
                    {
                        select.Append("Solicitante.FUN_NOME as Solicitante, ");
                        groupBy.Append("Solicitante.FUN_NOME, ");

                        SetarJoinsSolicitante(joins);
                    }
                    break;

                case "NumeroPedido":
                    if (!select.Contains(" NumeroPedido, "))
                    {
                        select.Append(@"substring(
                                          (select distinct ', ' + _Pedido.PED_NUMERO_PEDIDO_EMBARCADOR from T_PEDIDO _Pedido 
                                           inner join T_AGENDAMENTO_COLETA_PEDIDO _AgendamentoPedido on _AgendamentoPedido.PED_CODIGO = _Pedido.PED_CODIGO 
                                           where _AgendamentoPedido.ACO_CODIGO = AgendamentoColeta.ACO_CODIGO
                                           for xml path('')), 3, 200
                                          ) NumeroPedido, ");
                        groupBy.Append("AgendamentoColeta.ACO_CODIGO, ");

                        SetarJoinsAgendamentoColeta(joins);
                    }
                    break;

                case "QuantidadeCaixas":
                    if (!select.Contains(" QuantidadeCaixas, "))
                    {
                        select.Append(@"(SELECT SUM(_AgendamentoPedido.ACP_VOLUMES_ENVIAR) FROM T_AGENDAMENTO_COLETA_PEDIDO _AgendamentoPedido where _AgendamentoPedido.ACO_CODIGO = AgendamentoColeta.ACO_CODIGO) QuantidadeCaixas, ");
                        groupBy.Append("AgendamentoColeta.ACO_CODIGO, ");

                        SetarJoinsAgendamentoColeta(joins);
                    }
                    break;

                case "Carga":
                    if (!select.Contains(" Carga, "))
                    {
                        select.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR Carga, ");
                        groupBy.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR, ");

                        SetarJoinsCarga(joins);
                    }
                    break;

                case "MotivoCancelamento":
                    if (!select.Contains(" MotivoCancelamento, "))
                    {
                        select.Append("CargaCancelamento.CAC_MOTIVO_CANCELAMENTO as MotivoCancelamento, ");
                        groupBy.Append("CargaCancelamento.CAC_MOTIVO_CANCELAMENTO, ");
                    }
                    break;

                case "SituacaoJanelaFormatado":
                case "SituacaoJanela":
                    if (!select.Contains(" SituacaoJanela, "))
                    {
                        select.Append("CargaJanelaDescarregamento.CJD_SITUACAO as SituacaoJanela, ");
                        groupBy.Append("CargaJanelaDescarregamento.CJD_SITUACAO, ");

                        SetarJoinsCargaJanelaDescarregamento(joins);
                    }
                    break;

                case "DescricaoSituacaoAgendamento":
                case "SituacaoAgendamento":
                    if (!select.Contains(" SituacaoAgendamento, "))
                    {
                        select.Append("AgendamentoColeta.ACO_SITUACAO as SituacaoAgendamento, ");
                        groupBy.Append("AgendamentoColeta.ACO_SITUACAO, ");

                        SetarJoinsAgendamentoColeta(joins);
                    }
                    break;
            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioAgendaCancelada filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            SetarJoinsCargaJanelaDescarregamento(joins);
            SetarJoinsCarga(joins);
            SetarJoinsAgendamentoColeta(joins);

            if (filtrosPesquisa.DataInicio != DateTime.MinValue)
                where.Append($" and CargaJanelaDescarregamento.CJD_INICIO_DESCARREGAMENTO >= '{filtrosPesquisa.DataInicio.ToString("yyyyMMdd HH:mm:ss")}'");

            if (filtrosPesquisa.DataFim != DateTime.MinValue)
                where.Append($" and CargaJanelaDescarregamento.CJD_INICIO_DESCARREGAMENTO <= '{filtrosPesquisa.DataFim.ToString("yyyyMMdd HH:mm:ss")}'");

            if (filtrosPesquisa.Destinatario > 0)
                where.Append($" and CentroDescarregamento.CLI_CGCCPF_DESTINATARIO = {filtrosPesquisa.Destinatario}");

            if (filtrosPesquisa.Fornecedor > 0)
                where.Append($" and AgendamentoColeta.REM_CODIGO = {filtrosPesquisa.Fornecedor}");

            if (filtrosPesquisa.TipoDeCarga > 0)
                where.Append($" and AgendamentoColeta.TCC_CODIGO = {filtrosPesquisa.TipoDeCarga}");

            if (filtrosPesquisa.Filial > 0)
                where.Append($" and Carga.FIL_CODIGO = {filtrosPesquisa.Filial}");

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroCarga))
                where.Append($" and Carga.CAR_CODIGO_CARGA_EMBARCADOR = '{filtrosPesquisa.NumeroCarga}'");

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Senha))
                where.Append($" and AgendamentoColeta.ACO_SENHA = '{filtrosPesquisa.Senha}'");

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Pedido))
                where.Append($@" and AgendamentoColeta.ACO_CODIGO IN 
                                (SELECT _a.ACO_CODIGO FROM T_AGENDAMENTO_COLETA _a 
                                JOIN T_AGENDAMENTO_COLETA_PEDIDO _ap on _ap.ACO_CODIGO = _a.ACO_CODIGO
                                JOIN T_PEDIDO _ped on _ped.PED_CODIGO = _ap.PED_CODIGO
                                WHERE _ped.PED_NUMERO_PEDIDO_EMBARCADOR = '{filtrosPesquisa.Pedido}')");

            if (filtrosPesquisa.SituacaoAgendamento?.Count > 0)
                where.Append($" and AgendamentoColeta.ACO_SITUACAO in ({string.Join(", ", filtrosPesquisa.SituacaoAgendamento.Select(o => o.ToString("d")))}) ");
        }

        #endregion
    }
}

