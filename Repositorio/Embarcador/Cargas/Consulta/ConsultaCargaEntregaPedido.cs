using Dominio.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Cargas
{
    sealed class ConsultaCargaEntregaPedido : Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioCargaEntregaPedido>
    {
        #region Construtores

        public ConsultaCargaEntregaPedido() : base(tabela: "T_CARGA Carga") { }

        #endregion

        #region Métodos Privados

        private void SetarJoinsCargaEntrega(StringBuilder joins)
        {
            if (!joins.Contains(" CargaEntrega "))
                joins.Append(" join T_CARGA_ENTREGA CargaEntrega on Carga.CAR_CODIGO = CargaEntrega.CAR_CODIGO ");
        }

        private void SetarJoinsCargaEntregaPedido(StringBuilder joins)
        {
            SetarJoinsCargaEntrega(joins);

            if (!joins.Contains(" CargaEntregaPedido "))
                joins.Append(" left join T_CARGA_ENTREGA_PEDIDO CargaEntregaPedido on CargaEntrega.CEN_CODIGO = CargaEntregaPedido.CEN_CODIGO ");
        }

        private void SetarJoinsCargaJanelaCarregamento(StringBuilder joins)
        {
            if (!joins.Contains(" CargaJanelaCarregamento "))
                joins.Append(" left join T_CARGA_JANELA_CARREGAMENTO CargaJanelaCarregamento on Carga.CAR_CODIGO = CargaJanelaCarregamento.CAR_CODIGO ");
        }

        private void SetarJoinsCargaPedido(StringBuilder joins)
        {
            SetarJoinsCargaEntregaPedido(joins);

            if (!joins.Contains(" CargaPedido "))
                joins.Append(" left join T_CARGA_PEDIDO CargaPedido on CargaEntregaPedido.CPE_CODIGO = CargaPedido.CPE_CODIGO ");
        }

        private void SetarJoinsCliente(StringBuilder joins)
        {
            SetarJoinsPedido(joins);

            if (!joins.Contains(" Cliente "))
                joins.Append(" left join T_CLIENTE Cliente on Pedido.CLI_CODIGO = Cliente.CLI_CGCCPF ");
        }

        private void SetarJoinsFilial(StringBuilder joins)
        {
            if (!joins.Contains(" Filial "))
                joins.Append(" join T_FILIAL Filial on Carga.FIL_CODIGO = Filial.FIL_CODIGO ");
        }

        private void SetarJoinsPedido(StringBuilder joins)
        {
            SetarJoinsCargaPedido(joins);

            if (!joins.Contains(" Pedido "))
                joins.Append(" left join T_PEDIDO Pedido on CargaPedido.PED_CODIGO = Pedido.PED_CODIGO ");
        }

        private void SetarJoinsRedespacho(StringBuilder joins)
        {
            if (!joins.Contains(" Redespacho "))
                joins.Append(" left join T_REDESPACHO Redespacho on Redespacho.RED_CODIGO = Carga.RED_CODIGO ");
        }
        private void SetarJoinsMonitoramento(StringBuilder joins)
        {

            if (!joins.Contains(" Monitoramento "))
            {
                joins.Append($" left join (select max(_monitoramento.MON_CODIGO) MON_CODIGO, _monitoramento.CAR_CODIGO from T_MONITORAMENTO _monitoramento group by _monitoramento.CAR_CODIGO) UltimoMonitormentoCarga on UltimoMonitormentoCarga.CAR_CODIGO = Carga.CAR_CODIGO ");
                joins.Append($" left join T_MONITORAMENTO Monitoramento on Monitoramento.MON_CODIGO = UltimoMonitormentoCarga.MON_CODIGO ");
            }
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioCargaEntregaPedido filtroPesquisa)
        {
            switch (propriedade)
            {
                case "Filial":
                    if (!select.Contains(" Filial, "))
                    {
                        select.Append("Filial.FIL_DESCRICAO as Filial, ");
                        groupBy.Append("Filial.FIL_DESCRICAO, ");

                        SetarJoinsFilial(joins);
                    }
                    break;

                case "GradeCarga":
                    if (!select.Contains(" GradeCarga, "))
                    {
                        select.Append("(SELECT CEC_DESCRICAO FROM T_CENTRO_CARREGAMENTO CC WHERE CC.CEC_CODIGO = " +
                            "(SELECT CEC_CODIGO FROM T_CARGA_JANELA_CARREGAMENTO J WHERE j.CAR_CODIGO = Carga.CAR_CODIGO)) as GradeCarga, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }

                    break;

                case "NomeCliente":
                    if (!select.Contains(" Cliente, "))
                    {
                        select.Append("Cliente.CLI_NOME as NomeCliente, ");
                        groupBy.Append("Cliente.CLI_NOME, ");

                        SetarJoinsCliente(joins);
                    }
                    break;

                case "CNPJCliente":
                case "CNPJClienteFormatado":
                    if (!select.Contains(" CNPJCliente, "))
                    {
                        select.Append("Cliente.CLI_CGCCPF as CNPJCliente, Cliente.CLI_FISJUR as TipoCliente, ");
                        groupBy.Append("Cliente.CLI_CGCCPF, Cliente.CLI_FISJUR, ");

                        SetarJoinsCliente(joins);
                    }
                    break;

                case "DataInicioCarregamento":
                case "DataInicioCarregamentoFormatada":
                    if (!select.Contains(" DataInicioCarregamento, "))
                    {
                        select.Append("CargaJanelaCarregamento.CJC_INICIO_CARREGAMENTO as DataInicioCarregamento, ");
                        groupBy.Append("CargaJanelaCarregamento.CJC_INICIO_CARREGAMENTO, ");

                        SetarJoinsCargaJanelaCarregamento(joins);
                    }
                    break;

                case "DataAgendamento":
                case "DataAgendamentoFormatada":
                    if (!select.Contains("DataAgendamento, "))
                    {
                        select.Append("MAX(CargaEntrega.CEN_DATA_AGENDAMENTO) as DataAgendamento, ");
                       
                        SetarJoinsCargaEntrega(joins);
                    }
                    break;

                case "DataChegadaCliente":
                case "DataChegadaClienteFormatada":
                    if (!select.Contains(" DataChegadaCliente, "))
                    {
                        select.Append("CargaEntrega.CEN_DATA_ENTRADA_RAIO as DataChegadaCliente, ");
                        groupBy.Append("CargaEntrega.CEN_DATA_ENTRADA_RAIO, ");

                        SetarJoinsCargaEntrega(joins);
                    }
                    break;

                case "NumeroPedido":
                    if (!select.Contains(" NumeroPedido,"))
                    {
                        select.Append("Pedido.PED_NUMERO_PEDIDO_EMBARCADOR as NumeroPedido, ");
                        groupBy.Append("Pedido.PED_NUMERO_PEDIDO_EMBARCADOR, ");

                        SetarJoinsPedido(joins);
                    }
                    break;

                case "NumeroCarga":
                    if (!select.Contains(" NumeroCarga, "))
                    {
                        select.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR as NumeroCarga, ");
                        groupBy.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR, ");
                    }
                    break;

                case "NumeroEXP":
                    if (!select.Contains(" NumeroEXP, "))
                    {
                        select.Append("Pedido.PED_NUMERO_EXP as NumeroEXP, ");
                        groupBy.Append("Pedido.PED_NUMERO_EXP, ");

                        SetarJoinsPedido(joins);
                    }
                    break;

                case "NumeroRedespacho":
                    if (!select.Contains(" NumeroRedespacho, "))
                    {
                        select.Append("(case when Redespacho.RED_NUMERO is null then '' else cast(Redespacho.RED_NUMERO as varchar(20)) end) as NumeroRedespacho, ");
                        groupBy.Append("Redespacho.RED_NUMERO, ");

                        SetarJoinsRedespacho(joins);
                    }
                    break;

                case "PrevisaoEntrega":
                case "PrevisaoEntregaFormatada":
                    if (!select.Contains(" PrevisaoEntrega, "))
                    {
                        select.Append("Pedido.PED_PREVISAO_ENTREGA as PrevisaoEntrega, ");
                        groupBy.Append("Pedido.PED_PREVISAO_ENTREGA, ");

                        SetarJoinsPedido(joins);
                    }
                    break;

                case "MonitoramentoStatus":
                case "MonitoramentoStatusDescricao":
                    if (!select.Contains(" MonitoramentoStatus, "))
                    {
                        select.Append("Monitoramento.MON_STATUS as MonitoramentoStatus, ");
                        groupBy.Append("Monitoramento.MON_STATUS, ");

                        SetarJoinsMonitoramento(joins);
                    }
                    break;

                case "EntregaRealizadaNoPrazoDescricao":
                case "EntregaRealizadaNoPrazo":
                    if (!select.Contains(" EntregaRealizadaNoPrazo, "))
                    {
                        select.Append("CargaEntrega.CEN_REALIZADA_NO_PRAZO as EntregaRealizadaNoPrazo, ");
                        groupBy.Append("CargaEntrega.CEN_REALIZADA_NO_PRAZO, ");

                        if(!joins.Contains("CargaEntrega"))
                            SetarJoinsCargaEntrega(joins);
                    }
                    break;

                default:
                    break;
            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioCargaEntregaPedido filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            SetarJoinsCargaEntrega(joins);

            if (filtrosPesquisa.DataInicialCriacao != DateTime.MinValue || filtrosPesquisa.DataFinalCriacao != DateTime.MinValue)
            {
                if (filtrosPesquisa.DataInicialCriacao != DateTime.MinValue)
                    where.Append($" AND Carga.CAR_DATA_CRIACAO >= '" + filtrosPesquisa.DataInicialCriacao.ToString("yyyy-MM-dd") + "'");

                if (filtrosPesquisa.DataFinalCriacao != DateTime.MinValue)
                    where.Append($" AND Carga.CAR_DATA_CRIACAO <= '" + filtrosPesquisa.DataFinalCriacao.ToString("yyyy-MM-dd") + " 23:59:59'");
            }

            if (filtrosPesquisa.DataInicialEntrega != DateTime.MinValue || filtrosPesquisa.DataFinalEntrega != DateTime.MinValue)
            {
                if (filtrosPesquisa.DataInicialEntrega != DateTime.MinValue)
                    where.Append($" AND Pedido.PED_PREVISAO_ENTREGA >= '" + filtrosPesquisa.DataInicialEntrega.ToString("yyyy-MM-dd") + "'");

                if (filtrosPesquisa.DataFinalEntrega != DateTime.MinValue)
                    where.Append($" AND Pedido.PED_PREVISAO_ENTREGA <= '" + filtrosPesquisa.DataFinalEntrega.ToString("yyyy-MM-dd") + " 23:59:59'");
            }
            if (filtrosPesquisa.CodigosFilial.Contains(-1))
            {
                where.Append($@" AND ( Carga.FIL_CODIGO IN ({string.Join(", ", filtrosPesquisa.CodigosFilial)}) OR EXISTS (SELECT _cargaPedidoRecebedor.CAR_CODIGO 
                                                                                                                       FROM T_CARGA_PEDIDO _cargaPedidoRecebedor 
                                                                                                                       LEFT JOIN T_PEDIDO _pedido ON _pedido.PED_CODIGO = _cargaPedidoRecebedor.PED_CODIGO
                                                                                                                       WHERE Carga.CAR_CODIGO = _cargaPedidoRecebedor.CAR_CODIGO
                                                                                                                       AND _pedido.CLI_CODIGO_RECEBEDOR IN ({string.Join(",", filtrosPesquisa.CodigosRecebedor)})))");
            }
            else if (filtrosPesquisa.CodigosFilial?.Count > 0)
                where.Append($" AND Carga.FIL_CODIGO IN ({string.Join(", ", filtrosPesquisa.CodigosFilial)})");

            if (filtrosPesquisa.CodigosTransportadora?.Count > 0)
                where.Append($" AND Carga.EMP_CODIGO IN ({string.Join(", ", filtrosPesquisa.CodigosTransportadora)})");

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.TipoDestino))
            {
                if (filtrosPesquisa.TipoDestino.Equals("C"))
                    where.Append($" AND CargaEntrega.CEN_COLETA = 1 ");
                else if (filtrosPesquisa.TipoDestino.Equals("E"))
                    where.Append($" AND CargaEntrega.CEN_COLETA = 0 ");
            }

            if (filtrosPesquisa.CargaAgendada.HasValue)
            {
                if (filtrosPesquisa?.CargaAgendada ?? false)
                    where.Append($" AND EXISTS (SELECT CJC_CODIGO FROM T_CARGA_JANELA_CARREGAMENTO _janelaCarregamento join T_CARGA _carga ON _janelaCarregamento.CAR_CODIGO = _carga.CAR_CODIGO where _janelaCarregamento.CAR_CODIGO = Carga.CAR_CODIGO) ");
                else
                    where.Append($" AND NOT EXISTS (SELECT CJC_CODIGO FROM T_CARGA_JANELA_CARREGAMENTO _janelaCarregamento join T_CARGA _carga ON _janelaCarregamento.CAR_CODIGO = _carga.CAR_CODIGO where _janelaCarregamento.CAR_CODIGO = Carga.CAR_CODIGO) ");
            }

            if (filtrosPesquisa.SituacaoCargas != null && filtrosPesquisa.SituacaoCargas.Count > 0)
            {
                string situacoes = string.Join(", ", from situacao in filtrosPesquisa.SituacaoCargas select situacao.ToString("d"));

                where.Append($" and Carga.CAR_SITUACAO IN ({situacoes})");
            }

            if (filtrosPesquisa.PossuiRedespacho == OpcaoSimNaoPesquisa.Sim)
                where.Append($" and Carga.RED_CODIGO is not null ");
            else if (filtrosPesquisa.PossuiRedespacho == OpcaoSimNaoPesquisa.Nao)
                where.Append($" and Carga.RED_CODIGO is null ");

            if (filtrosPesquisa.PrevisaoEntregaPlanejadaInicio != DateTime.MinValue)
                where.Append($" AND CargaEntrega.CEN_DATA_ENTREGA_PREVISTA >= '" + filtrosPesquisa.PrevisaoEntregaPlanejadaInicio.ToString("yyyy-MM-dd") + "'");

            if (filtrosPesquisa.PrevisaoEntregaPlanejadaFinal != DateTime.MinValue)
                where.Append($" AND CargaEntrega.CEN_DATA_ENTREGA_PREVISTA <= '" + filtrosPesquisa.PrevisaoEntregaPlanejadaFinal.AddDays(1).ToString("yyyy-MM-dd") + "'");

            if (filtrosPesquisa.StatusMonitoramento.Count > 0){
                SetarJoinsMonitoramento(joins);
                where.Append($" AND Monitoramento.MON_STATUS IN ({string.Join(", ", (from obj in filtrosPesquisa.StatusMonitoramento select (int)obj).ToList())})");
            }

        }

        #endregion
    }
}
