using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Cargas
{
    sealed class ConsultaQuantidadeJanelaDescarregamento : Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioQuantidadeDescarga>
    {
        #region Construtores

        public ConsultaQuantidadeJanelaDescarregamento() : base(tabela: "T_CARGA_JANELA_DESCARREGAMENTO as CargaJanelaDescarregamento") { }

        #endregion

        #region Métodos Privados 

        private void SetarJoinsCarga(StringBuilder joins)
        {
            if (!joins.Contains(" Carga "))
                joins.Append("join T_CARGA Carga on Carga.CAR_CODIGO = CargaJanelaDescarregamento.CAR_CODIGO ");
        }

        private void SetarJoinsCargaLicenca(StringBuilder joins)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" CargaLicenca "))
                joins.Append("left join T_CARGA_LICENCA CargaLicenca on CargaLicenca.CAR_CODIGO = Carga.CAR_CODIGO ");
        }

        private void SetarJoinsCargaDadosSumarizados(StringBuilder joins)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" CargaDadosSumarizados "))
                joins.Append("left join T_CARGA_DADOS_SUMARIZADOS CargaDadosSumarizados on CargaDadosSumarizados.CDS_CODIGO = Carga.CDS_CODIGO ");
        }

        private void SetarJoinsCargaOrdemEmbarqueIntegracao(StringBuilder joins)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" CargaOrdemEmbarqueIntegracao "))
                joins.Append("left join T_CARGA_ORDEM_EMBARQUE_INTEGRACAO CargaOrdemEmbarqueIntegracao on CargaOrdemEmbarqueIntegracao.CAR_CODIGO = Carga.CAR_CODIGO ");
        }

        private void SetarJoinsCargaCentroDescarregamento(StringBuilder joins)
        {
            if (!joins.Contains(" CentroDescarregamento "))
                joins.Append("left join T_CENTRO_DESCARREGAMENTO CentroDescarregamento on CentroDescarregamento.CED_CODIGO = CargaJanelaDescarregamento.CED_CODIGO ");
        }

        private void SetarJoinsCargaCentroDescarregamentoDestinatario(StringBuilder joins)
        {
            if (!joins.Contains(" CentroDescarregamentoDestinatario "))
                joins.Append("left join T_CLIENTE Destinatario on CentroDescarregamento.CLI_CGCCPF_DESTINATARIO = Destinatario.CLI_CGCCPF ");
        }

        private void SetarJoinsEmpresa(StringBuilder joins)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" Empresa "))
                joins.Append("left join T_EMPRESA Empresa on Empresa.EMP_CODIGO = Carga.EMP_CODIGO ");
        }

        private void SetarJoinsExcecaoCapacidadeDescarregamento(StringBuilder joins)
        {
            if (!joins.Contains(" ExcecaoCapacidadeDescarregamento "))
                joins.Append(" left join T_CENTRO_DESCARREGAMENTO_EXCECAO_CAPACIDADE ExcecaoCapacidadeDescarregamento on ExcecaoCapacidadeDescarregamento.CED_CODIGO = CargaJanelaDescarregamento.CED_CODIGO and cast(CargaJanelaDescarregamento.CJD_INICIO_DESCARREGAMENTO as date) between cast(ExcecaoCapacidadeDescarregamento.CEX_DATA as date) and cast(ExcecaoCapacidadeDescarregamento.CEX_DATA_FINAL as date) ");
        }

        private void SetarJoinsModeloVeicularCarga(StringBuilder joins)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" ModeloVeicularCarga "))
                joins.Append("left join T_MODELO_VEICULAR_CARGA ModeloVeicularCarga on ModeloVeicularCarga.MVC_CODIGO = Carga.MVC_CODIGO ");
        }

        private void SetarJoinsOperadorCarga(StringBuilder joins)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" OperadorCarga "))
                joins.Append("left join T_FUNCIONARIO OperadorCarga on OperadorCarga.FUN_CODIGO = Carga.CAR_OPERADOR ");
        }

        private void SetarJoinsRotaCarga(StringBuilder joins)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" RotaCarga "))
                joins.Append("left join T_ROTA_FRETE RotaCarga on RotaCarga.ROF_CODIGO = Carga.ROF_CODIGO ");
        }

        private void SetarJoinsTipoCarga(StringBuilder joins)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" TipoCarga "))
                joins.Append("left join T_TIPO_DE_CARGA TipoCarga on TipoCarga.TCG_CODIGO = Carga.TCG_CODIGO ");
        }

        private void SetarJoinsVeiculo(StringBuilder joins)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" Veiculo "))
                joins.Append("left join T_VEICULO Veiculo on Veiculo.VEI_CODIGO = Carga.CAR_VEICULO ");
        }

        private void SetarJoinsPosicaoAtual(StringBuilder joins)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" PosicaoAtual "))
                joins.Append("left join T_POSICAO_ATUAL PosicaoAtual on PosicaoAtual.VEI_CODIGO = Carga.CAR_VEICULO ");
        }

        private void SetarJoinsFilial(StringBuilder joins)
        {
            SetarJoinsCarga(joins);

            if ((!joins.Contains(" Filial ")) || (!joins.Contains(" CNPJFilial ")))
            {
                joins.Append("left join T_FILIAL Filial on Filial.FIL_CODIGO = Carga.FIL_CODIGO ");
            }
        }

        private void SetarJoinsCargaJanelaCarregamento(StringBuilder joins)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" CargaJanelaCarregamento "))
                joins.Append(" LEFT JOIN T_CARGA_JANELA_CARREGAMENTO CargaJanelaCarregamento ON CargaJanelaCarregamento.CAR_CODIGO = Carga.CAR_CODIGO ");
        }

        private void SetarJoinsMonitoramento(StringBuilder joins)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" Monitoramento "))
            {
                joins.Append($" left join (select max(_monitoramento.MON_CODIGO) MON_CODIGO, _monitoramento.CAR_CODIGO from T_MONITORAMENTO _monitoramento where _monitoramento.MON_STATUS != {(int)MonitoramentoStatus.Cancelado} group by _monitoramento.CAR_CODIGO) UltimoMonitormentoCarga on UltimoMonitormentoCarga.CAR_CODIGO = Carga.CAR_CODIGO "); // SQL-INJECTION-SAFE
                joins.Append($" left join T_MONITORAMENTO Monitoramento on Monitoramento.MON_CODIGO = UltimoMonitormentoCarga.MON_CODIGO ");
            }
        }

        private void SetarJoinsMonitoriamentoStatusViagem(StringBuilder joins)
        {
            SetarJoinsMonitoramento(joins);

            if (!joins.Contains(" MonitoramentoStatusViagem "))
                joins.Append(" LEFT JOIN T_MONITORAMENTO_STATUS_VIAGEM MonitoramentoStatusViagem on MonitoramentoStatusViagem.MSV_CODIGO = Monitoramento.MSV_CODIGO ");
        }

        private void SetarJoinsCargaEntrega(StringBuilder joins)
        {
            SetarJoinsCargaCentroDescarregamento(joins);

            if (!joins.Contains(" CargaEntrega "))
                joins.Append(" LEFT JOIN T_CARGA_ENTREGA CargaEntrega ON CargaEntrega.CAR_CODIGO = CargaJanelaDescarregamento.CAR_CODIGO and CargaEntrega.CLI_CODIGO_ENTREGA = CentroDescarregamento.CLI_CGCCPF_DESTINATARIO and CargaEntrega.CEN_COLETA = 0 ");
        }

        private void SetarJoinsFaixaTemperatura(StringBuilder joins)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" FaixaTemperatura "))
                joins.Append(" LEFT JOIN T_FAIXA_TEMPERATURA FaixaTemperatura ON FaixaTemperatura.FTE_CODIGO = Carga.FTE_CODIGO");
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioQuantidadeDescarga filtroPesquisa)
        {
            switch (propriedade)
            {
                case "CidadeExpedidor":
                    if (!select.Contains(" CidadeExpedidor,"))
                    {
                        select.Append("substring(( ");
                        select.Append("    select distinct ', ' + Localidade.LOC_DESCRICAO + ' - ' + Localidade.UF_SIGLA ");
                        select.Append("      from T_CARGA_PEDIDO CargaPedido ");
                        select.Append("      join T_CLIENTE Expedidor on Expedidor.CLI_CGCCPF = CargaPedido.CLI_CODIGO_EXPEDIDOR ");
                        select.Append("      join T_LOCALIDADES Localidade on Localidade.LOC_CODIGO = Expedidor.LOC_CODIGO ");
                        select.Append("     where CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) CidadeExpedidor, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");

                        SetarJoinsCarga(joins);
                    }
                    break;

                case "CidadeRemetente":
                    if (!select.Contains(" CidadeRemetente,"))
                    {
                        select.Append("substring(( ");
                        select.Append("    select distinct ', ' + Localidade.LOC_DESCRICAO + ' - ' + Localidade.UF_SIGLA ");
                        select.Append("      from T_CARGA_PEDIDO CargaPedido ");
                        select.Append("      join T_PEDIDO Pedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO ");
                        select.Append("      join T_CLIENTE Remetente on Remetente.CLI_CGCCPF = Pedido.CLI_CODIGO_REMETENTE ");
                        select.Append("      join T_LOCALIDADES Localidade on Localidade.LOC_CODIGO = Remetente.LOC_CODIGO ");
                        select.Append("     where CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) CidadeRemetente, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");

                        SetarJoinsCarga(joins);
                    }
                    break;

                case "DataDescarregamento":
                    if (!select.Contains(" DataDescarregamento,"))
                    {
                        select.Append("CargaJanelaDescarregamento.CJD_INICIO_DESCARREGAMENTO DataDescarregamento, ");

                        if (!groupBy.Contains("CargaJanelaDescarregamento.CJD_INICIO_DESCARREGAMENTO,"))
                            groupBy.Append("CargaJanelaDescarregamento.CJD_INICIO_DESCARREGAMENTO, ");
                    }
                    break;

                case "DataDescarregamentoProgramada":
                    if (!select.Contains(" DataDescarregamentoProgramada,"))
                    {
                        select.Append("CargaJanelaDescarregamento.CJD_DATA_DESCARREGAMENTO_PROGRAMADA DataDescarregamentoProgramada, ");

                        if (!groupBy.Contains("CargaJanelaDescarregamento.CJD_DATA_DESCARREGAMENTO_PROGRAMADA,"))
                            groupBy.Append("CargaJanelaDescarregamento.CJD_DATA_DESCARREGAMENTO_PROGRAMADA, ");
                    }
                    break;

                case "DataDeadLCargaNavioViagem":
                    if (!select.Contains(" DataDeadLCargaNavioViagem,"))
                    {
                        select.Append("isnull(( ");
                        select.Append("    select top (1) convert(nvarchar(10), Pedido.PED_NAVIO_VIAGEM_DATA_DEADLCARGA, 103) + ' ' + convert(nvarchar(5), Pedido.PED_NAVIO_VIAGEM_DATA_DEADLCARGA, 108) ");
                        select.Append("      from T_CARGA_PEDIDO CargaPedido ");
                        select.Append("      join T_PEDIDO Pedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO ");
                        select.Append("     where CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append("       and Pedido.PED_NAVIO_VIAGEM_DATA_DEADLCARGA is not null ");
                        select.Append("), '') DataDeadLCargaNavioViagem, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");

                        SetarJoinsCarga(joins);
                    }
                    break;

                case "DataDeadLineNavioViagem":
                    if (!select.Contains(" DataDeadLineNavioViagem,"))
                    {
                        select.Append("isnull(( ");
                        select.Append("    select top (1) + convert(nvarchar(10), Pedido.PED_NAVIO_VIAGEM_DATA_DEADLINE, 103) + ' ' + convert(nvarchar(5), Pedido.PED_NAVIO_VIAGEM_DATA_DEADLINE, 108) ");
                        select.Append("      from T_CARGA_PEDIDO CargaPedido ");
                        select.Append("      join T_PEDIDO Pedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO ");
                        select.Append("     where CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append("       and Pedido.PED_NAVIO_VIAGEM_DATA_DEADLINE is not null ");
                        select.Append("), '') DataDeadLineNavioViagem, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");

                        SetarJoinsCarga(joins);
                    }
                    break;

                case "InicioDescarregamento":
                    if (!select.Contains(" InicioDescarregamento,"))
                    {
                        select.Append("convert(nvarchar(10), CargaJanelaDescarregamento.CJD_INICIO_DESCARREGAMENTO, 103) + ' ' + convert(nvarchar(5), CargaJanelaDescarregamento.CJD_INICIO_DESCARREGAMENTO, 108) InicioDescarregamento, ");

                        if (!groupBy.Contains("CargaJanelaDescarregamento.CJD_INICIO_DESCARREGAMENTO,"))
                            groupBy.Append("CargaJanelaDescarregamento.CJD_INICIO_DESCARREGAMENTO, ");
                    }
                    break;

                case "DataPrevisaoDescarregamento":
                    if (!select.Contains(" DataPrevisaoDescarregamento,"))
                    {
                        select.Append("CargaJanelaDescarregamento.CJD_DATA_DESCARREGAMENTO_PROGRAMADA DataCarregamento, ");

                        if (!groupBy.Contains("CargaJanelaDescarregamento.CJD_DATA_DESCARREGAMENTO_PROGRAMADA,"))
                            groupBy.Append("CargaJanelaDescarregamento.CJD_DATA_DESCARREGAMENTO_PROGRAMADA, ");
                    }
                    break;

                case "Despachante":
                    if (!select.Contains(" Despachante,"))
                    {
                        select.Append("substring(( ");
                        select.Append("    select distinct ', ' + isnull(Pedido.PED_DESPACHANTE_CODIGO, '') + ");
                        select.Append("           case ");
                        select.Append("               when (isnull(Pedido.PED_DESPACHANTE_CODIGO, '') <> '' and isnull(Pedido.PED_DESPACHANTE_DESCRICAO, '') <> '') then ' - ' ");
                        select.Append("               else '' ");
                        select.Append("           end + ");
                        select.Append("           isnull(Pedido.PED_DESPACHANTE_DESCRICAO, '') ");
                        select.Append("      from T_CARGA_PEDIDO CargaPedido ");
                        select.Append("      join T_PEDIDO Pedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO ");
                        select.Append("     where CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append("       and (isnull(Pedido.PED_DESPACHANTE_CODIGO, '') <> '' or isnull(Pedido.PED_DESPACHANTE_DESCRICAO, '') <> '') ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) Despachante, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");

                        SetarJoinsCarga(joins);
                    }
                    break;

                case "Destinatario":
                    if (!select.Contains(" Destinatario,"))
                    {
                        select.Append("Destinatario.CLI_NOME Destinatario, ");
                        groupBy.Append("Destinatario.CLI_NOME, ");

                        SetarJoinsCargaCentroDescarregamento(joins);
                        SetarJoinsCargaCentroDescarregamentoDestinatario(joins);
                    }
                    break;

                case "Destino":
                    if (!select.Contains(" Destino,"))
                    {
                        select.Append("CargaDadosSumarizados.CDS_DESTINOS Destino, ");
                        groupBy.Append("CargaDadosSumarizados.CDS_DESTINOS, ");

                        SetarJoinsCargaDadosSumarizados(joins);
                    }
                    break;

                case "DiasAtrazo":
                    if (!select.Contains(" DiasAtrazo,"))
                    {
                        select.Append("(case when datediff(day, CargaJanelaDescarregamento.CJD_DATA_DESCARREGAMENTO_PROGRAMADA, CargaJanelaDescarregamento.CJD_INICIO_DESCARREGAMENTO) > 0 then datediff(day, CargaJanelaDescarregamento.CJD_DATA_DESCARREGAMENTO_PROGRAMADA, CargaJanelaDescarregamento.CJD_INICIO_DESCARREGAMENTO) else 0 end) DiasAtrazo, ");
                        groupBy.Append("CargaJanelaDescarregamento.CJD_DATA_DESCARREGAMENTO_PROGRAMADA, CargaJanelaDescarregamento.CJD_INICIO_DESCARREGAMENTO, ");
                    }
                    break;

                case "Expedidor":
                    if (!select.Contains(" Expedidor,"))
                    {
                        select.Append("CargaDadosSumarizados.CDS_EXPEDIDORES Expedidor, ");
                        groupBy.Append("CargaDadosSumarizados.CDS_EXPEDIDORES, ");

                        SetarJoinsCargaDadosSumarizados(joins);
                    }
                    break;

                case "ModeloVeiculo":
                    if (!select.Contains(" ModeloVeiculo,"))
                    {
                        select.Append("ModeloVeicularCarga.MVC_DESCRICAO ModeloVeiculo, ");
                        groupBy.Append("ModeloVeicularCarga.MVC_DESCRICAO, ");

                        SetarJoinsModeloVeicularCarga(joins);
                    }
                    break;

                case "NavioViagem":
                    if (!select.Contains(" NavioViagem,"))
                    {
                        select.Append("substring(( ");
                        select.Append("    select distinct ', ' + isnull(Pedido.PED_NAVIO_VIAGEM_CODIGO, '') + ");
                        select.Append("           case ");
                        select.Append("               when (isnull(Pedido.PED_NAVIO_VIAGEM_CODIGO, '') <> '' and isnull(Pedido.PED_NAVIO_VIAGEM_NOME, '') <> '') then ' - ' ");
                        select.Append("               else '' ");
                        select.Append("           end + ");
                        select.Append("           isnull(Pedido.PED_NAVIO_VIAGEM_NOME, '') ");
                        select.Append("      from T_CARGA_PEDIDO CargaPedido ");
                        select.Append("      join T_PEDIDO Pedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO ");
                        select.Append("     where CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append("       and (isnull(Pedido.PED_NAVIO_VIAGEM_CODIGO, '') <> '' or isnull(Pedido.PED_NAVIO_VIAGEM_NOME, '') <> '') ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) NavioViagem, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");

                        SetarJoinsCarga(joins);
                    }
                    break;

                case "NumeroBooking":
                    if (!select.Contains(" NumeroBooking,"))
                    {
                        select.Append("substring(( ");
                        select.Append("    select distinct ', ' + Pedido.PED_NUMERO_BOOKING ");
                        select.Append("      from T_CARGA_PEDIDO CargaPedido ");
                        select.Append("      join T_PEDIDO Pedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO ");
                        select.Append("     where CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append("       and isnull(Pedido.PED_NUMERO_BOOKING, '') <> '' ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) NumeroBooking, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");

                        SetarJoinsCarga(joins);
                    }
                    break;

                case "NumeroCarga":
                    if (!select.Contains(" NumeroCarga,"))
                    {
                        select.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR NumeroCarga, ");
                        groupBy.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR, ");

                        SetarJoinsCarga(joins);
                    }
                    break;

                case "NumeroEntregas":
                    if (!select.Contains(" NumeroEntregas,"))
                    {
                        select.Append("CargaDadosSumarizados.CDS_NUMERO_ENTREGAS NumeroEntregas, ");
                        groupBy.Append("CargaDadosSumarizados.CDS_NUMERO_ENTREGAS, ");
                        SetarJoinsCargaDadosSumarizados(joins);
                    }
                    break;

                case "NumeroEXP":
                    if (!select.Contains(" NumeroEXP,"))
                    {
                        select.Append("substring(( ");
                        select.Append("    select distinct ', ' + Pedido.PED_NUMERO_EXP ");
                        select.Append("      from T_CARGA_PEDIDO CargaPedido ");
                        select.Append("      join T_PEDIDO Pedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO ");
                        select.Append("     where CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append("       and isnull(Pedido.PED_NUMERO_EXP, '') <> '' ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) NumeroEXP, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");

                        SetarJoinsCarga(joins);
                    }
                    break;

                case "NumeroPedidoEmbarcador":
                    if (!select.Contains(" NumeroPedidoEmbarcador,"))
                    {
                        select.Append("substring(( ");
                        select.Append("    select distinct ', ' + Pedido.PED_NUMERO_PEDIDO_EMBARCADOR ");
                        select.Append("      from T_CARGA_PEDIDO CargaPedido ");
                        select.Append("      join T_PEDIDO Pedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO ");
                        select.Append("     where CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append("       and isnull(Pedido.PED_NUMERO_PEDIDO_EMBARCADOR, '') <> '' ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) NumeroPedidoEmbarcador, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");

                        SetarJoinsCarga(joins);
                    }
                    break;

                case "NumeroPedidoProvisorio":
                    if (!select.Contains(" NumeroPedidoProvisorio,"))
                    {
                        select.Append("substring(( ");
                        select.Append("    select distinct ', ' + Pedido.PED_NUMERO_PEDIDO_PROVISORIO ");
                        select.Append("      from T_CARGA_PEDIDO CargaPedido ");
                        select.Append("      join T_PEDIDO Pedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO ");
                        select.Append("     where CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append("       and isnull(Pedido.PED_NUMERO_PEDIDO_PROVISORIO, '') <> '' ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) NumeroPedidoProvisorio, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");

                        SetarJoinsCarga(joins);
                    }
                    break;

                case "Operador":
                    if (!select.Contains(" Operador,"))
                    {
                        select.Append("OperadorCarga.FUN_NOME Operador, ");
                        groupBy.Append("OperadorCarga.FUN_NOME, ");

                        SetarJoinsOperadorCarga(joins);
                    }
                    break;

                case "TempoDescarregamento":
                    if (!select.Contains(" TempoDescarregamento,"))
                    {
                        select.Append("CargaJanelaDescarregamento.CJD_TEMPO_DESCARREGAMENTO TempoDescarregamento, ");

                        if (!groupBy.Contains("CargaJanelaDescarregamento.CJD_TEMPO_DESCARREGAMENTO,"))
                            groupBy.Append("CargaJanelaDescarregamento.CJD_TEMPO_DESCARREGAMENTO, ");
                    }
                    break;

                case "OrdemEmbarque":
                    if (!select.Contains(" OrdemEmbarque,"))
                    {
                        select.Append("substring(( ");
                        select.Append("    select distinct ', ' + OrdemEmbarque.OEM_NUMERO ");
                        select.Append("      from T_ORDEM_EMBARQUE OrdemEmbarque ");
                        select.Append("     where OrdemEmbarque.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) OrdemEmbarque, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");

                        SetarJoinsCarga(joins);
                    }
                    break;

                case "PesoCarga":
                    if (!select.Contains(" PesoCarga,"))
                    {
                        select.Append("CargaDadosSumarizados.CDS_PESO_TOTAL PesoCarga, ");
                        groupBy.Append("CargaDadosSumarizados.CDS_PESO_TOTAL, ");

                        SetarJoinsCargaDadosSumarizados(joins);
                    }
                    break;

                case "PortoViagemDestino":
                    if (!select.Contains(" PortoViagemDestino,"))
                    {
                        select.Append("substring(( ");
                        select.Append("    select distinct ', ' + isnull(Pedido.PED_PORTO_DESTINO_CODIGO, '') + ");
                        select.Append("           case ");
                        select.Append("               when (isnull(Pedido.PED_PORTO_DESTINO_CODIGO, '') <> '' and isnull(Pedido.PED_PORTO_DESTINO_DESCRICAO, '') <> '') then ' - ' ");
                        select.Append("               else '' ");
                        select.Append("           end + ");
                        select.Append("           isnull(Pedido.PED_PORTO_DESTINO_DESCRICAO, '') + ");
                        select.Append("           case ");
                        select.Append("               when (isnull(Pedido.PED_PORTO_DESTINO_DESCRICAO, '') <> '' and isnull(Pedido.PED_PORTO_DESTINO_PAIS, '') <> '') then ' - ' ");
                        select.Append("               else '' ");
                        select.Append("           end + ");
                        select.Append("           isnull(Pedido.PED_PORTO_DESTINO_PAIS, '') ");
                        select.Append("      from T_CARGA_PEDIDO CargaPedido ");
                        select.Append("      join T_PEDIDO Pedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO ");
                        select.Append("     where CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append("       and (isnull(Pedido.PED_PORTO_DESTINO_CODIGO, '') <> '' or isnull(Pedido.PED_PORTO_DESTINO_DESCRICAO, '') <> '' or isnull(Pedido.PED_PORTO_DESTINO_PAIS, '') <> '') ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) PortoViagemDestino, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");

                        SetarJoinsCarga(joins);
                    }
                    break;

                case "PortoViagemOrigem":
                    if (!select.Contains(" PortoViagemOrigem,"))
                    {
                        select.Append("substring(( ");
                        select.Append("    select distinct ', ' + isnull(Pedido.PED_PORTO_ORIGEM_CODIGO, '') + ");
                        select.Append("           case ");
                        select.Append("               when (isnull(Pedido.PED_PORTO_ORIGEM_CODIGO, '') <> '' and isnull(Pedido.PED_PORTO_ORIGEM_DESCRICAO, '') <> '') then ' - ' ");
                        select.Append("               else '' ");
                        select.Append("           end + ");
                        select.Append("           isnull(Pedido.PED_PORTO_ORIGEM_DESCRICAO, '') + ");
                        select.Append("           case ");
                        select.Append("               when (isnull(Pedido.PED_PORTO_ORIGEM_DESCRICAO, '') <> '' and isnull(Pedido.PED_PORTO_ORIGEM_PAIS, '') <> '') then ' - ' ");
                        select.Append("               else '' ");
                        select.Append("           end + ");
                        select.Append("           isnull(Pedido.PED_PORTO_ORIGEM_PAIS, '') ");
                        select.Append("      from T_CARGA_PEDIDO CargaPedido ");
                        select.Append("      join T_PEDIDO Pedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO ");
                        select.Append("     where CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append("       and (isnull(Pedido.PED_PORTO_ORIGEM_CODIGO, '') <> '' or isnull(Pedido.PED_PORTO_ORIGEM_DESCRICAO, '') <> '' or isnull(Pedido.PED_PORTO_ORIGEM_PAIS, '') <> '') ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) PortoViagemOrigem, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");

                        SetarJoinsCarga(joins);
                    }
                    break;

                case "PossuiGenset":
                    if (!select.Contains(" PossuiGenset,"))
                    {
                        select.Append("isnull(( ");
                        select.Append("    select top (1) 'Sim' ");
                        select.Append("      from T_CARGA_PEDIDO CargaPedido ");
                        select.Append("      join T_PEDIDO Pedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO ");
                        select.Append("     where CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append("       and Pedido.PED_POSSUI_GENSET = 1 ");
                        select.Append("), 'Não') PossuiGenset, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");

                        SetarJoinsCarga(joins);
                    }
                    break;

                case "Remetente":
                    if (!select.Contains(" Remetente,"))
                    {
                        select.Append("CargaDadosSumarizados.CDS_REMETENTES Remetente, ");
                        groupBy.Append("CargaDadosSumarizados.CDS_REMETENTES, ");

                        SetarJoinsCargaDadosSumarizados(joins);
                    }
                    break;

                case "Rota":
                    if (!select.Contains(" Rota,"))
                    {
                        select.Append("RotaCarga.ROF_DESCRICAO Rota, ");
                        groupBy.Append("RotaCarga.ROF_DESCRICAO, ");

                        SetarJoinsRotaCarga(joins);
                    }
                    break;

                case "SituacaoCargaJanelaDescarregamento":
                case "SituacaoCargaJanelaDescarregamentoDescricao":
                    if (!select.Contains(" SituacaoCargaJanelaDescarregamento,"))
                    {
                        select.Append("CargaJanelaDescarregamento.CJD_SITUACAO SituacaoCargaJanelaDescarregamento, ");
                        groupBy.Append("CargaJanelaDescarregamento.CJD_SITUACAO, ");
                    }
                    break;

                case "CentroDescarregamento":
                case "CentroDescarregamentoDescricao":
                    if (!select.Contains(" CentroDescarregamentoDescricao,"))
                    {
                        select.Append("CentroDescarregamento.CED_DESCRICAO CentroDescarregamentoDescricao, ");
                        groupBy.Append("CentroDescarregamento.CED_DESCRICAO, ");

                        SetarJoinsCargaCentroDescarregamento(joins);
                    }
                    break;

                case "SituacaoOrdemEmbarque":
                case "SituacaoOrdemEmbarqueDescricao":
                    if (!select.Contains(" SituacaoOrdemEmbarque,"))
                    {
                        select.Append("CargaOrdemEmbarqueIntegracao.INT_SITUACAO_INTEGRACAO SituacaoOrdemEmbarque, ");
                        groupBy.Append("CargaOrdemEmbarqueIntegracao.INT_SITUACAO_INTEGRACAO, ");

                        SetarJoinsCargaOrdemEmbarqueIntegracao(joins);
                    }
                    break;

                case "StatusGr":
                    if (!select.Contains(" StatusGr,"))
                    {
                        select.Append("case when (Carga.CAR_VEICULO is not null and Carga.CAR_PROBLEMA_INTEGRACAO_MOTORISTA_TELERISCO = 1) then 'Não OK' else 'OK' end StatusGr, ");
                        groupBy.Append("Carga.CAR_VEICULO, Carga.CAR_PROBLEMA_INTEGRACAO_MOTORISTA_TELERISCO, ");

                        SetarJoinsCarga(joins);
                    }
                    break;

                case "TipoCarga":
                    if (!select.Contains(" TipoCarga,"))
                    {
                        select.Append("TipoCarga.TCG_DESCRICAO TipoCarga, ");
                        groupBy.Append("TipoCarga.TCG_DESCRICAO, ");

                        SetarJoinsTipoCarga(joins);
                    }
                    break;

                case "TipoProbe":
                case "TipoProbeDescricao":
                    if (!select.Contains(" TipoProbe,"))
                    {
                        select.Append("( ");
                        select.Append("    select top (1) Pedido.PED_TIPO_PROBE ");
                        select.Append("      from T_CARGA_PEDIDO CargaPedido ");
                        select.Append("      join T_PEDIDO Pedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO ");
                        select.Append("     where CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append("       and Pedido.PED_TIPO_PROBE is not null ");
                        select.Append(") TipoProbe, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");

                        SetarJoinsCarga(joins);
                    }
                    break;

                case "Transportador":
                    if (!select.Contains(" Transportador,"))
                    {
                        select.Append("Empresa.EMP_RAZAO Transportador, ");
                        groupBy.Append("Empresa.EMP_RAZAO, ");

                        SetarJoinsEmpresa(joins);
                    }
                    break;

                case "ValorFrete":
                    if (!select.Contains(" ValorFrete,"))
                    {
                        select.Append("SUM(Carga.CAR_VALOR_FRETE_PAGAR) + SUM(Carga.CAR_VALOR_ICMS) ValorFrete, ");

                        SetarJoinsCarga(joins);
                    }
                    break;

                case "Veiculos":
                    if (!select.Contains(" Veiculos,"))
                    {
                        select.Append("(Veiculo.VEI_PLACA + isnull(( ");
                        select.Append("    select ', ' + Reboque.VEI_PLACA ");
                        select.Append("      from T_CARGA_VEICULOS_VINCULADOS VeiculoVinculado ");
                        select.Append("      join T_VEICULO Reboque on VeiculoVinculado.VEI_CODIGO = Reboque.VEI_CODIGO ");
                        select.Append("     where VeiculoVinculado.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append("       for xml path('') ");
                        select.Append("), '')) Veiculos, ");

                        groupBy.Append("Veiculo.VEI_PLACA, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");

                        SetarJoinsVeiculo(joins);
                    }
                    break;

                case "ViaTransporte":
                    if (!select.Contains(" ViaTransporte,"))
                    {
                        select.Append("substring(( ");
                        select.Append("    select distinct ', ' + ViaTransporte.TVT_DESCRICAO ");
                        select.Append("      from T_CARGA_PEDIDO CargaPedido ");
                        select.Append("      join T_PEDIDO Pedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO ");
                        select.Append("      join T_VIA_TRANSPORTE ViaTransporte on ViaTransporte.TVT_CODIGO = Pedido.TVT_CODIGO ");
                        select.Append("     where CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) ViaTransporte, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");

                        SetarJoinsCarga(joins);
                    }
                    break;

                case "CnpjTransportador":
                    if (!select.Contains(" CnpjTransportador,"))
                    {
                        select.Append("Empresa.EMP_CNPJ CnpjTransportador, ");
                        groupBy.Append("Empresa.EMP_CNPJ, ");

                        SetarJoinsEmpresa(joins);
                    }
                    break;

                case "Licenca":
                case "LicencaDescricao":
                    if (!select.Contains(" Licenca,"))
                    {
                        select.Append("CargaLicenca.TCL_SITUACAO Licenca, ");
                        groupBy.Append("CargaLicenca.TCL_SITUACAO, ");

                        SetarJoinsCargaLicenca(joins);
                    }
                    break;

                case "Rastreador":
                    if (!select.Contains(" Rastreador,"))
                    {
                        select.AppendLine($"cast((case when (PosicaoAtual.POA_DATA_VEICULO is not null and dateadd(minute, {filtroPesquisa.TempoSemPosicaoParaVeiculoPerderSinal}, PosicaoAtual.POA_DATA_VEICULO) < getdate()) then 1 else 0 end) as bit) as Rastreador, ");
                        groupBy.Append("PosicaoAtual.POA_DATA_VEICULO, ");

                        SetarJoinsPosicaoAtual(joins);
                    }
                    break;

                case "Filial":
                    if (!select.Contains(" Filial,"))
                    {
                        select.Append("Filial.FIL_DESCRICAO Filial, ");
                        groupBy.Append("Filial.FIL_DESCRICAO, ");

                        if (!select.Contains("Filial.FIL_CNPJ"))
                            SetarJoinsFilial(joins);
                    }
                    break;

                case "CNPJFilial":
                    if (!select.Contains(" CNPJFilial,"))
                    {
                        select.Append("Filial.FIL_CNPJ CNPJFilial, ");
                        groupBy.Append("Filial.FIL_CNPJ, ");

                        if (!select.Contains("Filial.FIL_DESCRICAO"))
                            SetarJoinsFilial(joins);
                    }
                    break;

                case "ForaPeriodo":
                case "ForaPeriodoDescricao":
                    if (!select.Contains(" ForaPeriodo,"))
                    {
                        select.Append(@"
                            cast(isnull((
                               select top(1) 0
                                 from T_CENTRO_DESCARREGAMENTO_PERIODO_DESCARREGAMENTO PeriodoDescarregamento
                                where PeriodoDescarregamento.PED_HORA_INICIO <= cast(CargaJanelaDescarregamento.CJD_INICIO_DESCARREGAMENTO as time)
                                  and PeriodoDescarregamento.PED_HORA_TERMINO >= cast(CargaJanelaDescarregamento.CJD_TERMINO_DESCARREGAMENTO as time)
                                  and (
                                          (ExcecaoCapacidadeDescarregamento.CEX_CODIGO is not null and ExcecaoCapacidadeDescarregamento.CEX_CODIGO = PeriodoDescarregamento.CEX_CODIGO) or
                                          (ExcecaoCapacidadeDescarregamento.CEX_CODIGO is null and PeriodoDescarregamento.CEX_CODIGO is null and PeriodoDescarregamento.CED_CODIGO = CargaJanelaDescarregamento.CED_CODIGO and PeriodoDescarregamento.PED_DIA = datepart(weekday, CargaJanelaDescarregamento.CJD_INICIO_DESCARREGAMENTO))	   
                                      )
                           ), 1) as bit) ForaPeriodo, "
                        );

                        groupBy.Append("ExcecaoCapacidadeDescarregamento.CEX_CODIGO, CargaJanelaDescarregamento.CED_CODIGO, CargaJanelaDescarregamento.CJD_INICIO_DESCARREGAMENTO, CargaJanelaDescarregamento.CJD_TERMINO_DESCARREGAMENTO, ");

                        SetarJoinsExcecaoCapacidadeDescarregamento(joins);
                    }
                    break;

                case "DataCarregamentoProgramada":
                case "DataCarregamentoProgramadaFormatada":
                    if (!select.Contains(" DataCarregamentoProgramada, "))
                    {
                        select.Append("CargaJanelaCarregamento.CJC_INICIO_CARREGAMENTO DataCarregamentoProgramada, ");
                        groupBy.Append("CargaJanelaCarregamento.CJC_INICIO_CARREGAMENTO, ");

                        SetarJoinsCargaJanelaCarregamento(joins);
                    }
                    break;

                case "DataPrevisaoEntregaPedido":
                case "DataPrevisaoEntregaPedidoFormatada":
                    if (!select.Contains(" DataPrevisaoEntregaPedido, "))
                    {
                        select.Append(@"(
                            SELECT max(Pedido.PED_PREVISAO_ENTREGA)
	                        FROM T_CARGA_PEDIDO CargaPedido 
		                        JOIN T_PEDIDO Pedido ON Pedido.PED_CODIGO = CargaPedido.PED_CODIGO
		                        JOIN ( 
			                        SELECT TOP 1 CLI_CODIGO_ENTREGA AS CLI_CODIGO
			                        FROM T_CARGA_ENTREGA CargaEntrega 
			                        WHERE CargaEntrega.CAR_CODIGO = CargaJanelaDescarregamento.CAR_CODIGO
			                        AND CargaEntrega.CEN_SITUACAO != 2 
			                        AND CargaEntrega.CEN_COLETA = 0
			                        ORDER BY CEN_ORDEM
		                        ) ClienteEntrega on ClienteEntrega.CLI_CODIGO = Pedido.CLI_CODIGO
	                        WHERE CargaPedido.CAR_CODIGO = CargaJanelaDescarregamento.CAR_CODIGO) DataPrevisaoEntregaPedido, ");

                        if (!groupBy.Contains("CargaJanelaDescarregamento.CAR_CODIGO"))
                            groupBy.Append("CargaJanelaDescarregamento.CAR_CODIGO, ");
                    }
                    break;

                case "DataEntregaPlanejadaProximaEntrega":
                case "DataEntregaPlanejadaProximaEntregaFormatada":
                    if (!select.Contains(" DataEntregaPlanejadaProximaEntrega, "))
                    {
                        select.Append(@"(
                            SELECT 
		                        min(CargaEntrega.CEN_DATA_ENTREGA_PREVISTA) 
	                        FROM T_CARGA_ENTREGA CargaEntrega 
	                        WHERE 
		                        CargaEntrega.CAR_CODIGO = CargaJanelaDescarregamento.CAR_CODIGO and 
		                        CargaEntrega.CEN_SITUACAO != 2 and 
		                        CargaEntrega.CEN_COLETA = 0
                            ) DataEntregaPlanejadaProximaEntrega, ");

                        if (!groupBy.Contains("CargaJanelaDescarregamento.CAR_CODIGO"))
                            groupBy.Append("CargaJanelaDescarregamento.CAR_CODIGO, ");
                    }
                    break;

                case "StatusViagem":
                    if (!select.Contains(" StatusViagem, "))
                    {
                        select.Append("MonitoramentoStatusViagem.MSV_DESCRICAO StatusViagem, ");
                        groupBy.Append("MonitoramentoStatusViagem.MSV_DESCRICAO, ");

                        SetarJoinsMonitoriamentoStatusViagem(joins);
                    }
                    break;

                case "DataEntradaRaio":
                case "DataEntradaRaioFormatada":
                    if (!select.Contains(" DataEntradaRaio, "))
                    {
                        select.Append("CargaEntrega.CEN_DATA_ENTRADA_RAIO DataEntradaRaio, ");
                        groupBy.Append("CargaEntrega.CEN_DATA_ENTRADA_RAIO, ");

                        SetarJoinsCargaEntrega(joins);
                    }
                    break;

                case "KmAteDestino":
                    if (!select.Contains(" KmAteDestino, "))
                    {
                        select.Append("CargaEntrega.CEN_DISTANCIA_ATE_DESTINO KmAteDestino, ");
                        groupBy.Append("CargaEntrega.CEN_DISTANCIA_ATE_DESTINO, ");

                        SetarJoinsCargaEntrega(joins);
                    }
                    break;

                case "NotasFiscais":
                    if (!select.Contains(" NotasFiscais "))
                    {
                        select.Append("SUBSTRING(( ");
                        select.Append("    SELECT ', ' + CAST(NotaFiscal.NF_NUMERO AS VARCHAR(20)) ");
                        select.Append("    FROM T_CARGA_PEDIDO CargaPedido ");
                        select.Append("       LEFT JOIN T_PEDIDO_XML_NOTA_FISCAL PedidoNF ON PedidoNF.CPE_CODIGO = CargaPedido.CPE_CODIGO ");
                        select.Append("       LEFT JOIN T_XML_NOTA_FISCAL NotaFiscal ON NotaFiscal.NFX_CODIGO = PedidoNF.NFX_CODIGO ");
                        select.Append("    WHERE CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append("    FOR XML PATH('') ");
                        select.Append("), 3, 1000) NotasFiscais, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "DataUltimaPosicao":
                case "DataUltimaPosicaoFormatada":
                    if (!select.Contains(" DataUltimaPosicao, "))
                    {
                        select.Append("PosicaoAtual.POA_DATA DataUltimaPosicao, ");
                        groupBy.Append("PosicaoAtual.POA_DATA, ");

                        SetarJoinsPosicaoAtual(joins);
                    }
                    break;

                case "SetPointTransp":
                    if (!select.Contains(" SetPointTransp, "))
                    {
                        select.Append("Carga.CAR_SET_POINT_VEICULO SetPointTransp, ");
                        groupBy.Append("Carga.CAR_SET_POINT_VEICULO, ");

                        SetarJoinsCarga(joins);
                    }
                    break;

                case "RangeTempTransp":
                    if (!select.Contains(" RangeTempTransp, "))
                    {
                        select.Append("CONCAT(FaixaTemperatura.FTE_FAIXA_FINAL, (CASE WHEN FaixaTemperatura.FTE_FAIXA_FINAL is not null or FaixaTemperatura.FTE_FAIXA_INICIAL is not null then ' até ' end), FaixaTemperatura.FTE_FAIXA_FINAL) RangeTempTransp, ");
                        groupBy.Append("FaixaTemperatura.FTE_FAIXA_FINAL, ");
                        groupBy.Append("FaixaTemperatura.FTE_FAIXA_INICIAL, ");

                        SetarJoinsFaixaTemperatura(joins);
                    }
                    break;

                case "TipoCargaTaura":
                    if (!select.Contains(" TipoCargaTaura, "))
                    {
                        select.Append("Carga.CAR_CATEGORIA_CARGA_EMBARCADOR TipoCargaTaura, ");
                        groupBy.Append("Carga.CAR_CATEGORIA_CARGA_EMBARCADOR, ");

                        SetarJoinsCarga(joins);
                    }
                    break;
                case "ChegadaPlanejada":
                case "ChegadaPlanejadaFormatada":
                    if (!select.Contains(" ChegadaPlanejada, "))
                    {
                        select.Append("Carga.CAR_DATA_FIM_VIAGEM_REPROGRAMADA ChegadaPlanejada, ");
                        groupBy.Append("Carga.CAR_DATA_FIM_VIAGEM_REPROGRAMADA, ");

                        SetarJoinsCarga(joins);
                    }
                    break;
            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioQuantidadeDescarga filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            SetarJoinsCarga(joins);

            where.Append(" and CargaJanelaDescarregamento.CJD_EXCEDENTE = 0 ");
            where.Append(" and isnull(CargaJanelaDescarregamento.CJD_CANCELADA, 0) = 0 ");

            where.Append($@"
                and (
                        (Carga.CAR_SITUACAO <> {(int)SituacaoCarga.Cancelada} and Carga.CAR_SITUACAO <> {(int)SituacaoCarga.Anulada}) or
                        (
                            CargaJanelaDescarregamento.CJD_SITUACAO = {(int)SituacaoCargaJanelaDescarregamento.CargaDevolvida} or
                            CargaJanelaDescarregamento.CJD_SITUACAO = {(int)SituacaoCargaJanelaDescarregamento.NaoComparecimento} or
                            CargaJanelaDescarregamento.CJD_SITUACAO = {(int)SituacaoCargaJanelaDescarregamento.NaoComparecimentoConfirmadoPeloFornecedor}
                        )
                    ) "
            );

            where.Append(" and exists ( ");
            where.Append("         select top(1) 1 ");
            where.Append("           from T_CARGA_PEDIDO _cargaPedido ");
            where.Append("           join T_PEDIDO _pedido on _pedido.PED_CODIGO = _cargaPedido.PED_CODIGO ");
            where.Append("          where _cargaPedido.CAR_CODIGO = CargaJanelaDescarregamento.CAR_CODIGO ");
            where.Append($"           and _cargaPedido.PED_TIPO_CARREGAMENTO_PEDIDO <> {(int)TipoCarregamentoPedido.TrocaNota} ");
            where.Append("            and isnull(_cargaPedido.CLI_CODIGO_RECEBEDOR, _pedido.CLI_CODIGO) = CentroDescarregamento.CLI_CGCCPF_DESTINATARIO ");
            where.Append("     ) ");

            if (filtrosPesquisa.Situacao?.Count > 0)
                where.Append($" and CargaJanelaDescarregamento.CJD_SITUACAO in ({string.Join(", ", filtrosPesquisa.Situacao.Select(o => o.ToString("d")))}) ");

            if (filtrosPesquisa.CodigoOperador > 0)
                where.Append($" and Carga.CAR_OPERADOR = {filtrosPesquisa.CodigoOperador} ");

            if (filtrosPesquisa.CodigosCentroDescarregamento.Count > 0)
                where.Append($" and CargaJanelaDescarregamento.CED_CODIGO in({string.Join(",", filtrosPesquisa.CodigosCentroDescarregamento)}) ");

            if (filtrosPesquisa.CodigoTransportador > 0)
                where.Append($" and Carga.EMP_CODIGO = {filtrosPesquisa.CodigoTransportador} ");

            if (filtrosPesquisa.CodigoRota > 0)
                where.Append($" and Carga.ROF_CODIGO = {filtrosPesquisa.CodigoRota} ");

            if (filtrosPesquisa.CodigosTipoCarga?.Count > 0)
                where.Append($" and (Carga.TCG_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosTipoCarga)}){(filtrosPesquisa.CodigosTipoCarga.Contains(-1) ? " or Carga.TCG_CODIGO is null" : "")})");

            if (filtrosPesquisa.CodigoModeloVeiculo > 0)
                where.Append($" and Carga.MVC_CODIGO = {filtrosPesquisa.CodigoModeloVeiculo} ");

            if (filtrosPesquisa.CpfCnpjDestinatario > 0d)
                where.Append($" and Destinatario.CLI_CGCCPF = {filtrosPesquisa.CpfCnpjDestinatario.ToString("F0")} ");

            if (filtrosPesquisa.DataInicial.HasValue)
                where.Append($" and CargaJanelaDescarregamento.CJD_INICIO_DESCARREGAMENTO >= '{filtrosPesquisa.DataInicial.Value.ToString("MM/dd/yyyy")}' ");

            if (filtrosPesquisa.DataFinal.HasValue)
                where.Append($" and CargaJanelaDescarregamento.CJD_INICIO_DESCARREGAMENTO < '{filtrosPesquisa.DataFinal.Value.AddDays(1).ToString("MM/dd/yyyy")}' ");

            if (filtrosPesquisa.CodigosFilial?.Count > 0)
                where.Append($" and Carga.FIL_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosFilial)})");

            if (filtrosPesquisa.CodigosTipoOperacao?.Count > 0)
                where.Append($" and (Carga.TOP_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosTipoOperacao)}){(filtrosPesquisa.CodigosTipoOperacao.Contains(-1) ? " or Carga.TOP_CODIGO is null" : "")})");

            if (filtrosPesquisa.CodigoVeiculo > 0)
                where.Append($" and (Carga.CAR_VEICULO = {filtrosPesquisa.CodigoVeiculo} or Carga.CAR_CODIGO in (select CAR_CODIGO from T_CARGA_VEICULOS_VINCULADOS where VEI_CODIGO = {filtrosPesquisa.CodigoVeiculo})) "); // SQL-INJECTION-SAFE
        }

        #endregion
    }
}
