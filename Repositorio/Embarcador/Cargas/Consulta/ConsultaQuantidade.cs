using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Cargas
{
    sealed class ConsultaQuantidade : Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioQuantidade>
    {
        #region Construtores

        public ConsultaQuantidade() : base(tabela: "T_CARGA_JANELA_CARREGAMENTO as CargaJanelaCarregamento") { }

        #endregion

        #region Métodos Privados 

        private void SetarJoinsCarga(StringBuilder joins)
        {
            if (!joins.Contains(" Carga "))
                joins.Append("join T_CARGA Carga on Carga.CAR_CODIGO = CargaJanelaCarregamento.CAR_CODIGO ");
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

        private void SetarJoinsCargaJanelaCarregamentoGuarita(StringBuilder joins)
        {
            if (!joins.Contains(" Guarita "))
                joins.Append("left join T_CARGA_JANELA_CARREGAMENTO_GUARITA Guarita on Guarita.CJC_CODIGO = CargaJanelaCarregamento.CJC_CODIGO ");
        }

        private void SetarJoinsCargaOrdemEmbarqueIntegracao(StringBuilder joins)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" CargaOrdemEmbarqueIntegracao "))
                joins.Append("left join T_CARGA_ORDEM_EMBARQUE_INTEGRACAO CargaOrdemEmbarqueIntegracao on CargaOrdemEmbarqueIntegracao.CAR_CODIGO = Carga.CAR_CODIGO ");
        }

        private void SetarJoinsCarregamento(StringBuilder joins)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" Carregamento "))
                joins.Append("left join T_CARREGAMENTO Carregamento on Carregamento.CRG_CODIGO = Carga.CRG_CODIGO ");
        }

        private void SetarJoinsEmpresa(StringBuilder joins)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" Empresa "))
                joins.Append("left join T_EMPRESA Empresa on Empresa.EMP_CODIGO = Carga.EMP_CODIGO ");
        }

        private void SetarJoinsExcecaoCapacidadeCarregamento(StringBuilder joins)
        {
            if (!joins.Contains(" ExcecaoCapacidadeCarregamento "))
                joins.Append(@"
                    left join T_CENTRO_CARREGAMENTO_EXCECAO_CAPACIDADE ExcecaoCapacidadeCarregamento
                      on ExcecaoCapacidadeCarregamento.CEC_CODIGO = CargaJanelaCarregamento.CEC_CODIGO
                     and (
                             (ExcecaoCapacidadeCarregamento.CEX_TIPO_ABRANGENCIA = 0 and cast(CargaJanelaCarregamento.CJC_INICIO_CARREGAMENTO as date) = cast(ExcecaoCapacidadeCarregamento.CEX_DATA as date)) or 
                             (
                                 ExcecaoCapacidadeCarregamento.CEX_TIPO_ABRANGENCIA = 1 and
                                 cast(CargaJanelaCarregamento.CJC_INICIO_CARREGAMENTO as date) between cast(ExcecaoCapacidadeCarregamento.CEX_DATA as date) and cast(ExcecaoCapacidadeCarregamento.CEX_DATA_FINAL as date) and
			                     case datepart(weekday, CargaJanelaCarregamento.CJC_INICIO_CARREGAMENTO)
                                     when 1 then ExcecaoCapacidadeCarregamento.CEX_DISPONIVEL_DOMINGO
                                     when 2 then ExcecaoCapacidadeCarregamento.CEX_DISPONIVEL_SEGUNDA
                                     when 3 then ExcecaoCapacidadeCarregamento.CEX_DISPONIVEL_TERCA
                                     when 4 then ExcecaoCapacidadeCarregamento.CEX_DISPONIVEL_QUARTA
                                     when 5 then ExcecaoCapacidadeCarregamento.CEX_DISPONIVEL_QUINTA
                                     when 6 then ExcecaoCapacidadeCarregamento.CEX_DISPONIVEL_SEXTA
                                     else ExcecaoCapacidadeCarregamento.CEX_DISPONIVEL_SABADO
                                 end = 1
                             )
                         ) "
                );
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
            SetarJoinsVeiculo(joins);

            if (!joins.Contains(" PosicaoAtual "))
                joins.Append("left join T_POSICAO_ATUAL PosicaoAtual on Veiculo.VEI_CODIGO = PosicaoAtual.VEI_CODIGO ");
        }

        private void SetarJoinsFilial(StringBuilder joins)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" Filial "))
                joins.Append("left join T_FILIAL Filial on Filial.FIL_CODIGO = Carga.FIL_CODIGO ");
        }

        private void SetarJoinsContratoFreteTransportador(StringBuilder joins)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" ContratoFreteTransportador "))
                joins.Append("left join T_CONTRATO_FRETE_TRANSPORTADOR ContratoFreteTransportador on ContratoFreteTransportador.EMP_CODIGO = Carga.EMP_CODIGO ");
        }

        private void SetarJoinsTipoContratoFrete(StringBuilder joins)
        {
            SetarJoinsContratoFreteTransportador(joins);

            if (!joins.Contains(" TipoContratoFrete "))
                joins.Append("left join T_TIPO_CONTRATO_FRETE TipoContratoFrete on ContratoFreteTransportador.TCF_CODIGO = TipoContratoFrete.TCF_CODIGO ");
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioQuantidade filtroPesquisa)
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

                case "ClienteAdicional":
                    if (!select.Contains(" ClienteAdicional,"))
                    {
                        select.Append("substring(( ");
                        select.Append("    select distinct ', ' + ClienteAdicional.CLI_NOME + ' (' +  ");
                        select.Append("           case ");
                        select.Append(@"              when ClienteAdicional.CLI_FISJUR = 'J' then FORMAT(ClienteAdicional.CLI_CGCCPF, '00\.000\.000\/0000-00') ");
                        select.Append(@"              when ClienteAdicional.CLI_FISJUR = 'F' then FORMAT(ClienteAdicional.CLI_CGCCPF, '000\.000\.000-00') ");
                        select.Append("               else '' ");
                        select.Append("           end + ')' ");
                        select.Append("      from T_CARGA_PEDIDO CargaPedido ");
                        select.Append("      join T_PEDIDO Pedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO ");
                        select.Append("      join T_CLIENTE ClienteAdicional on ClienteAdicional.CLI_CGCCPF = Pedido.CLI_CODIGO_CLIENTE_ADICIONAL ");
                        select.Append("     where CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) ClienteAdicional, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");

                        SetarJoinsCarga(joins);
                    }
                    break;

                case "ClienteDonoContainer":
                    if (!select.Contains(" ClienteDonoContainer,"))
                    {
                        select.Append("substring(( ");
                        select.Append("    select distinct ', ' + ClienteDonoContainer.CLI_NOME + ' (' +  ");
                        select.Append("           case ");
                        select.Append(@"              when ClienteDonoContainer.CLI_FISJUR = 'J' then FORMAT(ClienteDonoContainer.CLI_CGCCPF, '00\.000\.000\/0000-00') ");
                        select.Append(@"              when ClienteDonoContainer.CLI_FISJUR = 'F' then FORMAT(ClienteDonoContainer.CLI_CGCCPF, '000\.000\.000-00') ");
                        select.Append("               else '' ");
                        select.Append("           end + ')' ");
                        select.Append("      from T_CARGA_PEDIDO CargaPedido ");
                        select.Append("      join T_PEDIDO Pedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO ");
                        select.Append("      join T_CLIENTE ClienteDonoContainer on ClienteDonoContainer.CLI_CGCCPF = Pedido.CLI_CODIGO_CLIENTE_CONTAINER ");
                        select.Append("     where CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) ClienteDonoContainer, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");

                        SetarJoinsCarga(joins);
                    }
                    break;

                case "DataCarregamento":
                    if (!select.Contains(" DataCarregamento,"))
                    {
                        select.Append("CargaJanelaCarregamento.CJC_INICIO_CARREGAMENTO DataCarregamento, ");

                        if (!groupBy.Contains("CargaJanelaCarregamento.CJC_INICIO_CARREGAMENTO,"))
                            groupBy.Append("CargaJanelaCarregamento.CJC_INICIO_CARREGAMENTO, ");
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

                case "DataInicioCarregamento":
                    if (!select.Contains(" DataInicioCarregamento,"))
                    {
                        select.Append("case when (isnull(CargaJanelaCarregamento.CJC_EXCEDENTE, 1) = 0 and CargaJanelaCarregamento.CEC_CODIGO is not null) then convert(nvarchar(10), CargaJanelaCarregamento.CJC_INICIO_CARREGAMENTO, 103) + ' ' + convert(nvarchar(5), CargaJanelaCarregamento.CJC_INICIO_CARREGAMENTO, 108) else '' end DataInicioCarregamento, ");
                        groupBy.Append("CargaJanelaCarregamento.CJC_EXCEDENTE, CargaJanelaCarregamento.CJC_INICIO_CARREGAMENTO, CargaJanelaCarregamento.CEC_CODIGO, ");
                    }
                    break;

                case "DataPrevisaoChegadaOrigem":
                    if (!select.Contains(" DataPrevisaoChegadaOrigem,"))
                    {
                        select.Append("case when (Carga.CAR_DATA_PREVISAO_CHEGADA_ORIGEM is not null) then convert(nvarchar(10), Carga.CAR_DATA_PREVISAO_CHEGADA_ORIGEM, 103) + ' ' + convert(nvarchar(5), Carga.CAR_DATA_PREVISAO_CHEGADA_ORIGEM, 108) else '' end DataPrevisaoChegadaOrigem, ");
                        groupBy.Append("Carga.CAR_DATA_PREVISAO_CHEGADA_ORIGEM, ");

                        SetarJoinsCarga(joins);
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
                        select.Append("CargaDadosSumarizados.CDS_DESTINATARIOS Destinatario, ");
                        groupBy.Append("CargaDadosSumarizados.CDS_DESTINATARIOS, ");

                        SetarJoinsCargaDadosSumarizados(joins);
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

                case "TipoOperacao":
                    if (!select.Contains(" TipoOperacao,"))
                    {
                        select.Append("CargaDadosSumarizados.CDA_TIPOS_DE_OPERACAO TipoOperacao, ");
                        groupBy.Append("CargaDadosSumarizados.CDA_TIPOS_DE_OPERACAO, ");

                        SetarJoinsCargaDadosSumarizados(joins);
                    }
                    break;

                case "DiasAtrazo":
                    if (!select.Contains(" DiasAtrazo,"))
                    {
                        select.Append("(case when datediff(day, CargaJanelaCarregamento.CJC_DATA_CARREGAMENTO_PROGRAMADA, CargaJanelaCarregamento.CJC_INICIO_CARREGAMENTO) > 0 then datediff(day, CargaJanelaCarregamento.CJC_DATA_CARREGAMENTO_PROGRAMADA, CargaJanelaCarregamento.CJC_INICIO_CARREGAMENTO) else 0 end) DiasAtrazo, ");
                        groupBy.Append("CargaJanelaCarregamento.CJC_DATA_CARREGAMENTO_PROGRAMADA, CargaJanelaCarregamento.CJC_INICIO_CARREGAMENTO, ");
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

                case "EXPAno":
                case "SubEXP":
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

                case "Observacao":
                    if (!select.Contains(" Observacao,"))
                    {
                        select.Append("CargaJanelaCarregamento.CJC_OBSERVACAO_TRANSPORTADOR Observacao, ");
                        groupBy.Append("CargaJanelaCarregamento.CJC_OBSERVACAO_TRANSPORTADOR, ");
                    }
                    break;

                case "ObservacaoCarregamento":
                    if (!select.Contains(" ObservacaoCarregamento,"))
                    {
                        select.Append("Carregamento.CRG_OBSERVACAO ObservacaoCarregamento, ");
                        groupBy.Append("Carregamento.CRG_OBSERVACAO, ");

                        SetarJoinsCarregamento(joins);
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

                case "SituacaoCargaJanelaCarregamento":
                case "SituacaoCargaJanelaCarregamentoDescricao":
                    if (!select.Contains(" SituacaoCargaJanelaCarregamento,"))
                    {
                        select.Append("CargaJanelaCarregamento.CJC_SITUACAO SituacaoCargaJanelaCarregamento, ");
                        groupBy.Append("CargaJanelaCarregamento.CJC_SITUACAO, ");
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
                        select.Append("(CASE ");
                        select.Append("     WHEN PosicaoAtual.POA_DATA_VEICULO IS NOT NULL ");
                        select.Append("     AND ");
                        select.Append("         PosicaoAtual.POA_DATA_VEICULO + '00:30:00' > CURRENT_TIMESTAMP ");
                        select.Append("     THEN ");
                        select.Append("         CAST(1 AS BIT) ");
                        select.Append("     ELSE ");
                        select.Append("CAST(0 AS BIT) ");
                        select.Append("END) Rastreador, ");
                        groupBy.Append("PosicaoAtual.POA_DATA_VEICULO, ");

                        SetarJoinsPosicaoAtual(joins);
                    }
                    break;

                case "Filial":
                    if (!select.Contains(" Filial,"))
                    {
                        select.Append("Filial.FIL_DESCRICAO Filial, ");
                        groupBy.Append("Filial.FIL_DESCRICAO, ");

                        SetarJoinsFilial(joins);
                    }
                    break;

                case "CNPJFilial":
                    if (!select.Contains(" CNPJFilial,"))
                    {
                        select.Append("Filial.FIL_CNPJ CNPJFilial, ");
                        groupBy.Append("Filial.FIL_CNPJ, ");

                        SetarJoinsFilial(joins);
                    }
                    break;

                case "ETANavioFormatada":
                    if (!select.Contains(" ETANavio,"))
                    {
                        select.Append("    (select top 1 + Pedido.PED_DATA_ETA ");
                        select.Append("      from T_CARGA_PEDIDO CargaPedido ");
                        select.Append("      join T_PEDIDO Pedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO ");
                        select.Append("     where CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append("       and isnull(Pedido.PED_DATA_ETA, '') <> '') ETANavio, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");

                        SetarJoinsCarga(joins);
                    }
                    break;

                case "ETSNavioFormatada":
                    if (!select.Contains(" ETSNavio,"))
                    {
                        select.Append("    (select top 1 Pedido.PED_DATA_ETS ");
                        select.Append("      from T_CARGA_PEDIDO CargaPedido ");
                        select.Append("      join T_PEDIDO Pedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO ");
                        select.Append("     where CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append("       and isnull(Pedido.PED_DATA_ETS, '') <> '') ETSNavio, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");

                        SetarJoinsCarga(joins);
                    }
                    break;

                case "TipoTomadorFormatada":
                    if (!select.Contains(" TipoTomador,"))
                    {
                        select.Append("    (select top (1) Pedido.PED_PAGAMENTO_MARITIMO ");
                        select.Append("      from T_CARGA_PEDIDO CargaPedido ");
                        select.Append("      join T_PEDIDO Pedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO ");
                        select.Append("     where CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO) TipoTomador, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");

                        SetarJoinsCarga(joins);
                    }
                    break;

                case "Temperatura":
                    if (!select.Contains(" Temperatura,"))
                    {
                        select.Append("    (select top (1) Pedido.PED_TEMPERATURA ");
                        select.Append("      from T_CARGA_PEDIDO CargaPedido ");
                        select.Append("      join T_PEDIDO Pedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO ");
                        select.Append("     where CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO) Temperatura, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");

                        SetarJoinsCarga(joins);
                    }
                    break;

                case "CodigoInland":
                    if (!select.Contains(" CodigoInland, "))
                    {
                        select.Append("    (select distinct top (1) isnull(Pedido.PED_INLAND_CODIGO, '') + ' ' + isnull(Pedido.PED_INLAND_DESCRICAO, '') ");
                        select.Append("      from T_CARGA_PEDIDO CargaPedido ");
                        select.Append("      join T_PEDIDO Pedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO ");
                        select.Append("     where CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO) CodigoInland, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");

                        SetarJoinsCarga(joins);
                    }
                    break;

                case "PaisDestino":
                    if (!select.Contains(" PaisDestino, "))
                    {
                        select.Append(" (SELECT top (1) Pedido.PED_PORTO_DESTINO_PAIS ");
                        select.Append("    FROM T_CARGA_PEDIDO CargaPedido ");
                        select.Append("      JOIN T_PEDIDO Pedido ON Pedido.PED_CODIGO = CargaPedido.PED_CODIGO ");
                        select.Append("      WHERE CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append("     AND (isnull(Pedido.PED_PORTO_DESTINO_CODIGO, '') <> '' ");
                        select.Append("       OR isnull(Pedido.PED_PORTO_DESTINO_DESCRICAO, '') <> '' ");
                        select.Append("       OR isnull(Pedido.PED_PORTO_DESTINO_PAIS, '') <> '') ) PaisDestino, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");

                        SetarJoinsCarga(joins);
                    }
                    break;

                case "PedidoOriginal":
                    if (!select.Contains(" PedidoOriginal, "))
                    {
                        select.Append("substring(( ");
                        select.Append("    select distinct ', ' + PedidoProvisorio.PED_NUMERO_PEDIDO_EMBARCADOR ");
                        select.Append("      from T_CARGA_PEDIDO CargaPedido ");
                        select.Append("      join T_PEDIDO Pedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO ");
                        select.Append("      join T_PEDIDO_TROCA PedidoTroca on PedidoTroca.PED_CODIGO_DEFINITIVO = Pedido.PED_CODIGO ");
                        select.Append("      join T_PEDIDO PedidoProvisorio on PedidoProvisorio.PED_CODIGO = PedidoTroca.PED_CODIGO_PROVISORIO ");

                        select.Append("      left join T_FILIAL Filial on Filial.FIL_CODIGO = PedidoProvisorio.FIL_CODIGO ");
                        select.Append("      left join T_CLIENTE Destinatario on Destinatario.CLI_CGCCPF = PedidoProvisorio.CLI_CODIGO, T_CONFIGURACAO_EMBARCADOR Configuracao ");

                        select.Append("     where CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append("         and not (CAST(Filial.FIL_CNPJ as float) = Destinatario.CLI_CGCCPF and Configuracao.CEM_TROCAR_PRE_CARGA_POR_CARGA = 0) ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) PedidoOriginal, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");

                        SetarJoinsCarga(joins);
                    }
                    break;

                case "ForaPeriodo":
                case "ForaPeriodoDescricao":
                    if (!select.Contains(" ForaPeriodo,"))
                    {
                        select.Append(@"
                            cast(isnull((
                                select top(1) 0
                                  from T_CENTRO_CARREGAMENTO_PERIODO_CARREGAMENTO PeriodoCarregamento
                                 where PeriodoCarregamento.PEC_HORA_INICIO <= cast(CargaJanelaCarregamento.CJC_INICIO_CARREGAMENTO as time)
                                   and PeriodoCarregamento.PEC_HORA_TERMINO >= cast(CargaJanelaCarregamento.CJC_INICIO_CARREGAMENTO as time)
                                   and (
                                           (ExcecaoCapacidadeCarregamento.CEX_CODIGO is not null and ExcecaoCapacidadeCarregamento.CEX_CODIGO = PeriodoCarregamento.CEX_CODIGO) or
                                           (ExcecaoCapacidadeCarregamento.CEX_CODIGO is null and PeriodoCarregamento.CEX_CODIGO is null and PeriodoCarregamento.CEC_CODIGO = CargaJanelaCarregamento.CEC_CODIGO and PeriodoCarregamento.PEC_DIA = datepart(weekday, CargaJanelaCarregamento.CJC_INICIO_CARREGAMENTO))	   
                                       )
                            ), 1) as bit) ForaPeriodo, "
                        );

                        groupBy.Append("ExcecaoCapacidadeCarregamento.CEX_CODIGO, CargaJanelaCarregamento.CEC_CODIGO, CargaJanelaCarregamento.CJC_INICIO_CARREGAMENTO, CargaJanelaCarregamento.CJC_TERMINO_CARREGAMENTO, ");

                        SetarJoinsExcecaoCapacidadeCarregamento(joins);
                    }
                    break;

                case "DataUltimaPosicaoFormatada":
                    if (!select.Contains(" DataUltimaPosicao, "))
                    {
                        select.Append("PosicaoAtual.POA_DATA_VEICULO DataUltimaPosicao, ");
                        groupBy.Append("PosicaoAtual.POA_DATA_VEICULO, ");

                        SetarJoinsPosicaoAtual(joins);
                    }
                    break;

                case "DivisoriaDescricao":
                    if (!select.Contains(" Divisoria,"))
                    {
                        select.Append("Carga.CAR_DIVISORIA_INTEGRACAO_LEILAO Divisoria, ");
                        groupBy.Append("Carga.CAR_DIVISORIA_INTEGRACAO_LEILAO, ");

                        SetarJoinsCarga(joins);
                    }
                    break;

                case "CargaPerigosaDescricao":
                    if (!select.Contains(" CargaPerigosa,"))
                    {
                        select.Append("Carga.CAR_CARGA_PERIGOSA_INTEGRACAO_LEILAO CargaPerigosa, ");
                        groupBy.Append("Carga.CAR_CARGA_PERIGOSA_INTEGRACAO_LEILAO, ");

                        SetarJoinsCarga(joins);
                    }
                    break;

                case "CustoPlanejado":
                    if (!select.Contains(" CustoPlanejado,"))
                    {
                        select.Append("Carga.CAR_CUSTO_PLANEJADO_INTEGRACAO_LEILAO CustoPlanejado, ");
                        groupBy.Append("Carga.CAR_CUSTO_PLANEJADO_INTEGRACAO_LEILAO, ");

                        SetarJoinsCarga(joins);
                    }
                    break;

                case "CustoAtual":
                    if (!select.Contains(" CustoAtual,"))
                    {
                        select.Append("Carga.CAR_CUSTO_ATUAL_INTEGRACAO_LEILAO CustoAtual, ");
                        groupBy.Append("Carga.CAR_CUSTO_ATUAL_INTEGRACAO_LEILAO, ");

                        SetarJoinsCarga(joins);
                    }
                    break;

                case "RazaoLeilao":
                    if (!select.Contains(" RazaoLeilao,"))
                    {
                        select.Append("Carga.CAR_RAZAO_INTEGRACAO_LEILAO RazaoLeilao, ");
                        groupBy.Append("Carga.CAR_RAZAO_INTEGRACAO_LEILAO, ");

                        SetarJoinsCarga(joins);
                    }
                    break;

                case "NumeroContrato":
                    if (!select.Contains(" NumeroContrato, "))
                    {
                        select.Append(@"(select TOP(1) CFT_NUMERO_EMBARCADOR from T_CONTRATO_FRETE_TRANSPORTADOR CFT 
                                            where Carga.EMP_CODIGO = CFT.EMP_CODIGO 
                                        and Carga.CAR_DATA_CARREGAMENTO between CFT.CFT_DATA_INICIAL and CFT.CFT_DATA_FINAL) NumeroContrato, ");

                        if (!groupBy.Contains("Carga.EMP_CODIGO,"))
                            groupBy.Append("Carga.EMP_CODIGO, ");

                        if (!groupBy.Contains("Carga.CAR_DATA_CARREGAMENTO,"))
                            groupBy.Append("Carga.CAR_DATA_CARREGAMENTO, ");

                        SetarJoinsCarga(joins);
                    }
                    break;

                case "DataInicialContratoFormatada":
                    if (!select.Contains(" DataInicialContrato, "))
                    {
                        select.Append(@"(select TOP(1) CFT_DATA_INICIAL from T_CONTRATO_FRETE_TRANSPORTADOR CFT 
							where Carga.EMP_CODIGO = CFT.EMP_CODIGO 
						and Carga.CAR_DATA_CARREGAMENTO between CFT.CFT_DATA_INICIAL and CFT.CFT_DATA_FINAL) DataInicialContrato, ");

                        if (!groupBy.Contains("Carga.EMP_CODIGO,"))
                            groupBy.Append("Carga.EMP_CODIGO, ");

                        if (!groupBy.Contains("Carga.CAR_DATA_CARREGAMENTO,"))
                            groupBy.Append("Carga.CAR_DATA_CARREGAMENTO, ");

                        SetarJoinsCarga(joins);
                    }
                    break;

                case "DataFinalContratoFormatada":
                    if (!select.Contains(" DataFinalContrato, "))
                    {
                        select.Append(@"(select TOP(1) CFT_DATA_FINAL from T_CONTRATO_FRETE_TRANSPORTADOR CFT 
							where Carga.EMP_CODIGO = CFT.EMP_CODIGO 
						and Carga.CAR_DATA_CARREGAMENTO between CFT.CFT_DATA_INICIAL and CFT.CFT_DATA_FINAL) DataFinalContrato, ");

                        if (!groupBy.Contains("Carga.EMP_CODIGO,"))
                            groupBy.Append("Carga.EMP_CODIGO, ");

                        if (!groupBy.Contains("Carga.CAR_DATA_CARREGAMENTO,"))
                            groupBy.Append("Carga.CAR_DATA_CARREGAMENTO, ");

                        SetarJoinsCarga(joins);
                    }
                    break;

                case "TipoContratoFreteDescricao":
                    if (!select.Contains(" TipoContratoFreteDescricao, "))
                    {
                        select.Append(@"(select TOP(1) TipoContratoFrete.TCF_DESCRICAO from T_CONTRATO_FRETE_TRANSPORTADOR CFT 
						 left join T_TIPO_CONTRATO_FRETE TipoContratoFrete on TipoContratoFrete.TCF_CODIGO = CFT.TCF_CODIGO
							where Carga.EMP_CODIGO = CFT.EMP_CODIGO 
						and Carga.CAR_DATA_CARREGAMENTO between CFT.CFT_DATA_INICIAL and CFT.CFT_DATA_FINAL) TipoContratoFreteDescricao, ");

                        if (!groupBy.Contains("Carga.EMP_CODIGO,"))
                            groupBy.Append("Carga.EMP_CODIGO, ");

                        if (!groupBy.Contains("Carga.CAR_DATA_CARREGAMENTO,"))
                            groupBy.Append("Carga.CAR_DATA_CARREGAMENTO, ");

                        SetarJoinsCarga(joins);
                    }
                    break;
            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioQuantidade filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            string pattern = "yyyy-MM-dd";

            SetarJoinsCarga(joins);

            where.Append(" and CargaJanelaCarregamento.CEC_CODIGO is not null ");
            where.Append(" and CargaJanelaCarregamento.CJC_EXCEDENTE = 0 ", (filtrosPesquisa.Tipo != TipoQuantidadeCarga.Faturada));
            where.Append(" and Carga.CAR_SITUACAO <> 13 ");
            where.Append(" and Carga.CAR_SITUACAO <> 18 ");

            List<TipoQuantidadeCarga> tiposFiltrarSemGuaritaFinalizada = new List<TipoQuantidadeCarga>()
            {
                TipoQuantidadeCarga.AgConfirmacaoTransportador,
                TipoQuantidadeCarga.AgLiberacaoParaTransportadores,
                TipoQuantidadeCarga.AguardandoCarregamento,
                TipoQuantidadeCarga.SemTransportador,
                TipoQuantidadeCarga.SemValorFrete
            };

            if (tiposFiltrarSemGuaritaFinalizada.Contains(filtrosPesquisa.Tipo))
            {
                where.Append(" and CargaJanelaCarregamento.CJC_CODIGO not in (  ");
                where.Append("         select cjc_codigo ");
                where.Append("           from T_CARGA_JANELA_CARREGAMENTO_GUARITA ");
                where.Append("          where CJC_DATA_ENTRADA_GUARITA is not null ");
                where.Append("            and CJC_DATA_FINAL_CARREGAMENTO is not null ");
                where.Append("            and CJC_LIBERACAO_VEICULO_CARREGAMENTO is not null ");
                where.Append("     ) ");
            }

            switch (filtrosPesquisa.Tipo)
            {
                case TipoQuantidadeCarga.AgConfirmacaoTransportador:
                    where.Append(" and CargaJanelaCarregamento.CJC_SITUACAO = 4 ");
                    break;

                case TipoQuantidadeCarga.AgLiberacaoParaTransportadores:
                    where.Append(" and CargaJanelaCarregamento.CJC_SITUACAO = 7 ");
                    break;

                case TipoQuantidadeCarga.AguardandoCarregamento:
                    where.Append(" and CargaJanelaCarregamento.CJC_SITUACAO = 5 ");
                    where.Append($" and Carga.CAR_SITUACAO IN({string.Join(", ", SituacaoCargaHelper.ObterSituacoesCargaNaoFaturada().Select(o => (int)o))}) ");
                    break;

                case TipoQuantidadeCarga.SemTransportador:
                    where.Append(" and CargaJanelaCarregamento.CJC_SITUACAO = 3 ");
                    break;

                case TipoQuantidadeCarga.SemValorFrete:
                    where.Append(" and CargaJanelaCarregamento.CJC_SITUACAO = 2 ");
                    break;

                case TipoQuantidadeCarga.EmAtraso:
                    where.Append(" and convert(date, CargaJanelaCarregamento.CJC_DATA_CARREGAMENTO_PROGRAMADA) < convert(date, CargaJanelaCarregamento.CJC_INICIO_CARREGAMENTO) ");

                    if (!groupBy.Contains("CargaJanelaCarregamento.CJC_DATA_CARREGAMENTO_PROGRAMADA,"))
                        groupBy.Append("CargaJanelaCarregamento.CJC_DATA_CARREGAMENTO_PROGRAMADA, ");

                    if (!groupBy.Contains("CargaJanelaCarregamento.CJC_INICIO_CARREGAMENTO,"))
                        groupBy.Append("CargaJanelaCarregamento.CJC_INICIO_CARREGAMENTO, ");
                    break;

                case TipoQuantidadeCarga.Faturada:
                    //where.Append(" and Guarita.JCG_CODIGO is not null ");
                    where.Append(" and Carga.CAR_DATA_FINALIZACAO_EMISSAO is not null ");
                    where.Append($" and NOT Carga.CAR_SITUACAO IN({string.Join(", ", SituacaoCargaHelper.ObterSituacoesCargaNaoFaturada().Select(o => (int)o))}) ");
                    SetarJoinsCargaJanelaCarregamentoGuarita(joins);
                    break;

                case TipoQuantidadeCarga.JaCarregada:
                    where.Append(" and Guarita.CJC_DATA_ENTRADA_GUARITA is not null ");
                    where.Append(" and Guarita.CJC_DATA_FINAL_CARREGAMENTO is not null ");
                    SetarJoinsCargaJanelaCarregamentoGuarita(joins);
                    break;

                case TipoQuantidadeCarga.ProntaParaCarregar:
                    where.Append(" and ( ");
                    where.Append("         (Guarita.CJC_DATA_ENTRADA_GUARITA is not null and Guarita.CJC_DATA_FINAL_CARREGAMENTO is not null) or ");
                    where.Append("         (CargaJanelaCarregamento.CJC_SITUACAO = 5 and (Guarita.CJC_DATA_ENTRADA_GUARITA is null or Guarita.CJC_DATA_FINAL_CARREGAMENTO is null or Guarita.CJC_LIBERACAO_VEICULO_CARREGAMENTO is null)) ");
                    where.Append("     )");
                    SetarJoinsCargaJanelaCarregamentoGuarita(joins);
                    break;
            }

            if (filtrosPesquisa.CodigoOperador > 0)
                where.Append($" and Carga.CAR_OPERADOR = {filtrosPesquisa.CodigoOperador} ");

            if (filtrosPesquisa.CodigosCentroCarregamento.Count > 0)
                where.Append($" and CargaJanelaCarregamento.CEC_CODIGO in({string.Join(",", filtrosPesquisa.CodigosCentroCarregamento)}) ");

            if (filtrosPesquisa.CodigoTransportador > 0)
                where.Append($" and Carga.EMP_CODIGO = {filtrosPesquisa.CodigoTransportador} ");

            if (filtrosPesquisa.CodigoRota > 0)
                where.Append($" and Carga.ROF_CODIGO = {filtrosPesquisa.CodigoRota} ");

            if (filtrosPesquisa.CodigosTipoCarga?.Count > 0)
                where.Append($" and (Carga.TCG_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosTipoCarga)}){(filtrosPesquisa.CodigosTipoCarga.Contains(-1) ? " or Carga.TCG_CODIGO is null" : "")})");

            if (filtrosPesquisa.CodigoModeloVeiculo > 0)
                where.Append($" and Carga.MVC_CODIGO = {filtrosPesquisa.CodigoModeloVeiculo} ");

            if (filtrosPesquisa.CpfCnpjDestinatario > 0d)
            {
                where.Append(" and exists ( ");
                where.Append("    select top (1) 1 ");
                where.Append("      from T_CARGA_PEDIDO CargaPedido ");
                where.Append("      join T_PEDIDO Pedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO ");
                where.Append("     where CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO ");
                where.Append($"      and Pedido.CLI_CODIGO = {filtrosPesquisa.CpfCnpjDestinatario.ToString("F0")} ");
                where.Append(" ) ");
            }

            if (filtrosPesquisa.DataInicial.HasValue)
                where.Append($" and CAST(CargaJanelaCarregamento.CJC_INICIO_CARREGAMENTO AS DATE) >= '{filtrosPesquisa.DataInicial.Value.ToString(pattern)}' ");

            if (filtrosPesquisa.DataFinal.HasValue)
                where.Append($" and CAST(CargaJanelaCarregamento.CJC_INICIO_CARREGAMENTO AS DATE) <= '{filtrosPesquisa.DataFinal.Value.ToString(pattern)}' ");

            if (filtrosPesquisa.CodigosFilial?.Count > 0)
                where.Append($" and Carga.FIL_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosFilial)})");

            if (filtrosPesquisa.CodigosTipoOperacao?.Count > 0)
                where.Append($" and (Carga.TOP_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosTipoOperacao)}){(filtrosPesquisa.CodigosTipoOperacao.Contains(-1) ? " or Carga.TOP_CODIGO is null" : "")})");

            if (filtrosPesquisa.CodigoVeiculo > 0)
                where.Append($" and (Carga.CAR_VEICULO = {filtrosPesquisa.CodigoVeiculo} or Carga.CAR_CODIGO in (select CAR_CODIGO from T_CARGA_VEICULOS_VINCULADOS where VEI_CODIGO = {filtrosPesquisa.CodigoVeiculo})) "); // SQL-INJECTION-SAFE

            if (filtrosPesquisa.CodigoTipoOperacao > 0)
                where.Append($" and Carga.TOP_CODIGO = {filtrosPesquisa.CodigoTipoOperacao} ");
        }

        #endregion
    }
}
