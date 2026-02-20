using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Logistica.Consulta
{
    sealed class ConsultaMonitoramentoTempoVeiculo : Embarcador.Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioMonitoramentoTempoVeiculo>
    {
        #region Construtores

        public ConsultaMonitoramentoTempoVeiculo() : base(tabela: "T_CARGA as Carga") { }

        #endregion

        #region Métodos Privados

        private void SetarJoinsFilial(StringBuilder joins)
        {
            if (!joins.Contains(" Filial "))
                joins.Append(" left join T_FILIAL Filial on Filial.FIL_CODIGO = Carga.FIL_CODIGO ");
        }

        private void SetarJoinsTipoCarga(StringBuilder joins)
        {
            if (!joins.Contains(" TipoCarga "))
                joins.Append(" left join T_TIPO_DE_CARGA TipoCarga on TipoCarga.TCG_CODIGO = Carga.TCG_CODIGO ");
        }

        private void SetarJoinsTipoOperacao(StringBuilder joins)
        {
            if (!joins.Contains(" TipoOperacao "))
                joins.Append(" left join T_TIPO_OPERACAO TipoOperacao on TipoOperacao.TOP_CODIGO = Carga.TOP_CODIGO ");
        }

        private void SetarJoinsTransportador(StringBuilder joins)
        {
            if (!joins.Contains(" Transportador "))
                joins.Append(" left join T_EMPRESA Transportador on Transportador.EMP_CODIGO = Carga.EMP_CODIGO ");
        }

        private void SetarJoinsVeiculo(StringBuilder joins)
        {
            if (!joins.Contains(" Veiculo "))
                joins.Append(" left join T_VEICULO Veiculo on Veiculo.VEI_CODIGO = Carga.CAR_VEICULO ");
        }

        private void SetarJoinsModeloVeicularCarga(StringBuilder joins)
        {
            if (!joins.Contains(" ModeloVeiculo "))
                joins.Append(" left join T_MODELO_VEICULAR_CARGA ModeloVeiculo on ModeloVeiculo.MVC_CODIGO = Carga.MVC_CODIGO ");
        }

        private void SetarJoinsCargaDadosSumarizados(StringBuilder joins)
        {
            if (!joins.Contains(" DadosSumarizados "))
                joins.Append(" left join T_CARGA_DADOS_SUMARIZADOS DadosSumarizados ON DadosSumarizados.CDS_CODIGO = Carga.CDS_CODIGO ");
        }

        private void SetarJoinsMonitoramento(StringBuilder joins)
        {
            if (!joins.Contains(" Monitoramento "))
                joins.Append(" left join T_MONITORAMENTO Monitoramento on Carga.CAR_CODIGO = Monitoramento.CAR_CODIGO ");
        }

        private void SetarJoinsCargaCTe(StringBuilder joins)
        {
            if (!joins.Contains(" CargaCTe "))
                joins.Append(" left join T_CARGA_CTE CargaCTe on Carga.CAR_CODIGO = CargaCTe.CAR_CODIGO ");
        }

        private void SetarJoinsOcorrenciaCTeIntegracao(StringBuilder joins)
        {
            SetarJoinsCargaCTe(joins);

            if (!joins.Contains(" OcorrenciaCTeIntegracao "))
                joins.Append("LEFT JOIN T_OCORRENCIA_CTE_INTEGRACAO OcorrenciaCTeIntegracao ON OcorrenciaCTeIntegracao.CCT_CODIGO = CargaCTe.CCT_CODIGO ");
        }

        private void SetarJoinsTabelaFrete(StringBuilder joins)
        {
            if (!joins.Contains(" TabelaFrete "))
                joins.Append(" left join T_TABELA_FRETE TabelaFrete on TabelaFrete.TBF_CODIGO = Carga.TBF_CODIGO ");
        }

        private void SetarJoinsCargaEntrega(StringBuilder joins)
        {
            if (!joins.Contains(" CargaEntrega "))
                joins.Append(" left join T_CARGA_ENTREGA CargaEntrega on CargaEntrega.CAR_CODIGO = Carga.CAR_CODIGO ");
        }

        private void SetarJoinsClienteCargaEntrega(StringBuilder joins)
        {
            SetarJoinsCargaEntrega(joins);

            if (!joins.Contains(" ClienteCargaEntrega "))
                joins.Append(" left join T_CLIENTE ClienteCargaEntrega on ClienteCargaEntrega.CLI_CGCCPF = CargaEntrega.CLI_CODIGO_ENTREGA ");
        }

        private void SetarJoinsLocalidadeClienteCargaEntrega(StringBuilder joins)
        {
            SetarJoinsClienteCargaEntrega(joins);

            if (!joins.Contains(" LocalidadeClienteCargaEntrega "))
                joins.Append(" left join T_LOCALIDADES LocalidadeClienteCargaEntrega on LocalidadeClienteCargaEntrega.LOC_CODIGO = ClienteCargaEntrega.LOC_CODIGO ");
        }

        private void SetarJoinsPaisLocalidadeClienteCargaEntrega(StringBuilder joins)
        {
            SetarJoinsLocalidadeClienteCargaEntrega(joins);

            if (!joins.Contains(" PaisLocalidadeClienteCargaEntrega "))
                joins.Append(" left join T_PAIS PaisLocalidadeClienteCargaEntrega on PaisLocalidadeClienteCargaEntrega.PAI_CODIGO = LocalidadeClienteCargaEntrega.PAI_CODIGO ");
        }

        private void SetarJoinsCidadeOrigemPedido(StringBuilder joins)
        {
            if (!joins.Contains(" CidadeOrigemPedido "))
                joins.Append(@" OUTER APPLY (SELECT TOP 1 CidadeOrigemPedido.LOC_DESCRICAO
                                             FROM T_CARGA_ENTREGA_PEDIDO AS cep
                                             INNER JOIN T_CARGA_PEDIDO AS cargapedido ON cargapedido.CPE_CODIGO = cep.CPE_CODIGO 
                                             INNER JOIN T_PEDIDO AS Pedido ON Pedido.PED_CODIGO = CargaPedido.PED_CODIGO
                                             INNER JOIN T_LOCALIDADES AS CidadeOrigemPedido ON Pedido.LOC_CODIGO_ORIGEM = CidadeOrigemPedido.LOC_CODIGO
                                             WHERE cep.CEN_CODIGO = CargaEntrega.CEN_CODIGO) AS CidadeOrigemPedido ");
        }

        private void SetarJoinsCidadeDestinoPedido(StringBuilder joins)
        {
            if (!joins.Contains(" CidadeDestinoPedido "))
                joins.Append(@" OUTER APPLY (SELECT TOP 1 CidadeDestinoPedido.LOC_DESCRICAO
                                             FROM T_CARGA_ENTREGA_PEDIDO AS cep
                                             INNER JOIN T_CARGA_PEDIDO AS cargapedido ON cargapedido.CPE_CODIGO = cep.CPE_CODIGO 
                                             INNER JOIN T_PEDIDO AS Pedido ON Pedido.PED_CODIGO = CargaPedido.PED_CODIGO
                                             INNER JOIN T_LOCALIDADES AS CidadeDestinoPedido ON Pedido.LOC_CODIGO_DESTINO = CidadeDestinoPedido.LOC_CODIGO
                                             WHERE cep.CEN_CODIGO = CargaEntrega.CEN_CODIGO) AS CidadeDestinoPedido ");
        }

        private void SetarJoinsClienteOrigemPedido(StringBuilder joins)
        {
            if (!joins.Contains(" ClienteOrigemPedido "))
                joins.Append(@"OUTER APPLY (SELECT TOP 1
			                                CASE WHEN CargaPedido.CLI_CODIGO_EXPEDIDOR IS NOT NULL
			                                THEN ClienteCargaPedido.CLI_NOME ELSE ClientePedido.CLI_NOME
			                                END AS CLI_NOME
			                                FROM T_CARGA_ENTREGA_PEDIDO AS cep
			                                INNER JOIN T_CARGA_PEDIDO AS CargaPedido ON CargaPedido.CPE_CODIGO = cep.CPE_CODIGO
			                                INNER JOIN T_PEDIDO AS Pedido ON Pedido.PED_CODIGO = CargaPedido.PED_CODIGO
			                                LEFT JOIN T_CLIENTE AS ClienteCargaPedido ON CargaPedido.CLI_CODIGO_EXPEDIDOR = ClienteCargaPedido.CLI_CGCCPF
			                                LEFT JOIN T_CLIENTE AS ClientePedido ON Pedido.CLI_CODIGO_REMETENTE = ClientePedido.CLI_CGCCPF
			                                WHERE cep.CEN_CODIGO = CargaEntrega.CEN_CODIGO) AS ClienteOrigemPedido ");
        }

        private void SetarJoinsClienteDestinoPedido(StringBuilder joins)
        {
            if (!joins.Contains(" ClienteDestinoPedido "))
                joins.Append(@" OUTER APPLY (SELECT TOP 1
			                                 CASE WHEN CargaPedido.CLI_CODIGO_RECEBEDOR IS NOT NULL
			                                 THEN ClienteCargaPedido.CLI_NOME ELSE ClientePedido.CLI_NOME
			                                 END AS CLI_NOME
			                                 FROM T_CARGA_ENTREGA_PEDIDO AS cep
			                                 INNER JOIN T_CARGA_PEDIDO AS CargaPedido ON CargaPedido.CPE_CODIGO = cep.CPE_CODIGO
			                                 INNER JOIN T_PEDIDO AS Pedido ON Pedido.PED_CODIGO = CargaPedido.PED_CODIGO			 
			                                 LEFT JOIN T_CLIENTE AS ClienteCargaPedido ON CargaPedido.CLI_CODIGO_RECEBEDOR = ClienteCargaPedido.CLI_CGCCPF
			                                 LEFT JOIN T_CLIENTE AS ClientePedido ON Pedido.CLI_CODIGO_RECEBEDOR = ClientePedido.CLI_CGCCPF
			                                 WHERE cep.CEN_CODIGO = CargaEntrega.CEN_CODIGO) AS ClienteDestinoPedido ");
        }

        private void SetarJoinsGrupoPessoasTomador(StringBuilder joins)
        {
            if (!joins.Contains(" GrupoPessoasTomador "))
                joins.Append(@" OUTER APPLY(SELECT TOP 1 GrupoPessoas.[GRP_DESCRICAO]
			                                FROM T_CARGA_ENTREGA_PEDIDO AS cep
			                                INNER JOIN T_CARGA_PEDIDO AS CargaPedido ON CargaPedido.CPE_CODIGO = cep.CPE_CODIGO
			                                INNER JOIN [T_CLIENTE] as TomadorCargaPedido ON CargaPedido.[CLI_CODIGO_TOMADOR] = TomadorCargaPedido.[CLI_CGCCPF]
			                                LEFT JOIN [T_CLIENTE_GRUPO_PESSOAS] as TomadorGrupoPessoas ON TomadorCargaPedido.CLI_CGCCPF = TomadorGrupoPessoas.[CLI_CGCCPF]
			                                LEFT JOIN [T_GRUPO_PESSOAS] as GrupoPessoas ON TomadorGrupoPessoas.[GRP_CODIGO] = GrupoPessoas.[GRP_CODIGO]
			                                WHERE cep.CEN_CODIGO = CargaEntrega.CEN_CODIGO) AS GrupoPessoasTomador ");
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioMonitoramentoTempoVeiculo filtroPesquisa)
        {
            switch (propriedade)
            {
                case "Codigo":
                    if (!select.Contains(" Codigo, "))
                    {
                        select.Append("Carga.CAR_CODIGO as Codigo, ");
                        groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "NumeroPedidoProvisorio":
                    if (!select.Contains(" NumeroPedidoProvisorio, "))
                    {
                        select.Append("substring(( ");
                        select.Append("    select distinct ', ' + Pedido.PED_NUMERO_PEDIDO_PROVISORIO ");
                        select.Append("      from T_CARGA_PEDIDO CargaPedido ");
                        select.Append("      join T_PEDIDO Pedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO ");
                        select.Append("     where CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append("       and isnull(Pedido.PED_NUMERO_PEDIDO_PROVISORIO, '') <> '' ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) NumeroPedidoProvisorio, ");
                    }
                    break;

                case "NumeroCarga":
                    if (!select.Contains("NumeroCarga, "))
                    {
                        select.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR NumeroCarga, ");
                        groupBy.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR, ");
                    }
                    break;

                case "Filial":
                    if (!select.Contains(" Filial, "))
                    {
                        select.Append("Filial.FIL_DESCRICAO Filial, ");
                        groupBy.Append("Filial.FIL_DESCRICAO, ");

                        SetarJoinsFilial(joins);
                    }
                    break;

                case "TipoOperacao":
                    if (!select.Contains(" TipoOperacao, "))
                    {
                        select.Append("TipoOperacao.TOP_DESCRICAO TipoOperacao, ");
                        groupBy.Append("TipoOperacao.TOP_DESCRICAO, ");

                        SetarJoinsTipoOperacao(joins);
                    }
                    break;

                case "Natureza":
                    if (!select.Contains(" Natureza, "))
                    {
                        select.Append(@"(CASE WHEN CargaEntrega.CEN_COLETA = 1 THEN 'Coleta' ELSE 'Entrega' END) Natureza, ");
                        groupBy.Append("CargaEntrega.CEN_COLETA, ");

                        SetarJoinsCargaEntrega(joins);
                    }
                    break;

                case "TipoCarga":
                    if (!select.Contains(" TipoCarga, "))
                    {
                        select.Append("TipoCarga.TCG_DESCRICAO TipoCarga, ");
                        groupBy.Append("TipoCarga.TCG_DESCRICAO, ");

                        SetarJoinsTipoCarga(joins);
                    }
                    break;

                case "Pedido":
                    if (!select.Contains(" Pedido, "))
                    {
                        select.Append(@"SUBSTRING((select distinct ', ' + pedido.PED_NUMERO_PEDIDO_EMBARCADOR 
                                            FROM T_CARGA_ENTREGA_PEDIDO cep
                                            inner join T_CARGA_PEDIDO cargapedido on cargapedido.CPE_CODIGO = cep.CPE_CODIGO 
                                            inner join T_PEDIDO as Pedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO
      									    WHERE cep.CEN_CODIGO = CargaEntrega.CEN_CODIGO
                                            for xml path('')), 3, 1000) Pedido, ");

                        if (!groupBy.Contains("CargaEntrega.CEN_CODIGO,"))
                            groupBy.Append("CargaEntrega.CEN_CODIGO, ");

                        SetarJoinsCargaEntrega(joins);
                    }
                    break;

                case "NumeroEXP":
                    if (!select.Contains(" NumeroEXP, "))
                    {
                        select.Append(@"SUBSTRING((select distinct ', ' + pedido.PED_NUMERO_EXP 
                                            FROM T_CARGA_PEDIDO cargapedido
                                            inner join T_PEDIDO as Pedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO
      									    WHERE Carga.CAR_CODIGO  = CargaPedido.CAR_CODIGO
                                            for xml path('')), 3, 1000) NumeroEXP, ");
                    }
                    break;

                case "Cliente":
                    if (!select.Contains(" Cliente, "))
                    {
                        select.Append("ClienteCargaEntrega.CLI_NOME Cliente, ");
                        groupBy.Append("ClienteCargaEntrega.CLI_NOME, ");

                        SetarJoinsClienteCargaEntrega(joins);
                    }
                    break;

                case "UFDestino":
                    if (!select.Contains(" UFDestino, "))
                    {
                        select.Append("LocalidadeClienteCargaEntrega.UF_SIGLA UFDestino, ");
                        groupBy.Append("LocalidadeClienteCargaEntrega.UF_SIGLA, ");

                        SetarJoinsLocalidadeClienteCargaEntrega(joins);
                    }
                    break;

                case "PaisDestino":
                    if (!select.Contains(" PaisDestino, "))
                    {
                        select.Append("PaisLocalidadeClienteCargaEntrega.PAI_NOME PaisDestino, ");
                        groupBy.Append("PaisLocalidadeClienteCargaEntrega.PAI_NOME, ");

                        SetarJoinsPaisLocalidadeClienteCargaEntrega(joins);
                    }
                    break;

                case "DataColeta":
                    if (!select.Contains(" DataColeta, "))
                    {
                        select.Append(@"(select TOP 1 CONVERT(NVARCHAR(10), Pedido.PED_DATA_INICIAL_COLETA, 103) + ' ' + CONVERT(NVARCHAR(5), Pedido.PED_DATA_INICIAL_COLETA, 108) 
                                            from T_PEDIDO Pedido 
                                            inner join T_CARGA_PEDIDO CargaPedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO 
                                            where CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO) DataColeta, ");
                    }
                    break;

                case "Transportador":
                    if (!select.Contains("Transportador, "))
                    {
                        select.Append("Transportador.EMP_RAZAO Transportador, ");
                        groupBy.Append("Transportador.EMP_RAZAO, ");

                        SetarJoinsTransportador(joins);
                    }
                    break;

                case "Veiculo":
                    if (!select.Contains(" Veiculo, "))
                    {
                        select.Append("Veiculo.VEI_PLACA as Veiculo, ");
                        groupBy.Append("Veiculo.VEI_PLACA, ");

                        SetarJoinsVeiculo(joins);
                    }
                    break;

                case "Reboque":
                    if (!select.Contains(" Reboque, "))
                    {
                        select.Append(@"ISNULL(substring((SELECT distinct ', ' + case when veiculo1.VEI_TIPOVEICULO = '1' then veiculo1.VEI_PLACA end
		                                            FROM T_CARGA_VEICULOS_VINCULADOS veiculoVinculadoCarga1 
		                                            INNER JOIN T_VEICULO veiculo1 ON veiculoVinculadoCarga1.VEI_CODIGO = veiculo1.VEI_CODIGO
				                                    WHERE veiculoVinculadoCarga1.CAR_CODIGO = Carga.CAR_CODIGO
                                        FOR XML PATH('')), 3, 100), '') Reboque, ");
                    }
                    break;

                case "ModeloReboque":
                    if (!select.Contains(" ModeloReboque, "))
                    {
                        select.Append(@"ISNULL(substring((SELECT distinct ', ' + mvcr.MVC_DESCRICAO
	                                              FROM T_CARGA_VEICULOS_VINCULADOS veiculoVinculadoCarga1 
		                                          INNER JOIN T_VEICULO veiculo1 ON veiculoVinculadoCarga1.VEI_CODIGO = veiculo1.VEI_CODIGO
		                                          INNER JOIN T_MODELO_VEICULAR_CARGA mvcr ON mvcr.MVC_CODIGO = veiculo1.MVC_CODIGO
				                                  WHERE veiculoVinculadoCarga1.CAR_CODIGO = Carga.CAR_CODIGO
                                       FOR XML PATH('')), 3, 100), '') ModeloReboque, ");
                    }
                    break;

                case "ModeloVeiculo":
                    if (!select.Contains(" ModeloVeiculo, "))
                    {
                        select.Append("ModeloVeiculo.MVC_DESCRICAO ModeloVeiculo, ");
                        groupBy.Append("ModeloVeiculo.MVC_DESCRICAO, ");

                        SetarJoinsModeloVeicularCarga(joins);
                    }
                    break;

                case "DataPrevisaoChegadaOrigemFormatada":
                    if (!select.Contains(" DataPrevisaoChegadaOrigem, "))
                    {
                        select.Append("Carga.CAR_DATA_PREVISAO_CHEGADA_ORIGEM DataPrevisaoChegadaOrigem, ");
                        groupBy.Append("Carga.CAR_DATA_PREVISAO_CHEGADA_ORIGEM, ");
                    }
                    break;

                case "DataInicioEntregaReprogramada":
                    if (!select.Contains(" DataInicioEntregaReprogramada, "))
                    {
                        select.Append("(CONVERT(NVARCHAR(10), CargaEntrega.CEN_DATA_ENTREGA_REPROGRAMADA, 103) + ' ' + CONVERT(NVARCHAR(5), CargaEntrega.CEN_DATA_ENTREGA_REPROGRAMADA, 108)) DataInicioEntregaReprogramada, ");

                        if (!groupBy.Contains("CargaEntrega.CEN_DATA_ENTREGA_REPROGRAMADA,"))
                            groupBy.Append("CargaEntrega.CEN_DATA_ENTREGA_REPROGRAMADA, ");

                        SetarJoinsCargaEntrega(joins);
                    }
                    break;

                case "DataInicioEntrega":
                    if (!select.Contains(" DataInicioEntrega, "))
                    {
                        select.Append("(CONVERT(NVARCHAR(10), CargaEntrega.CEN_DATA_INICIO_ENTREGA, 103) + ' ' + CONVERT(NVARCHAR(5), CargaEntrega.CEN_DATA_INICIO_ENTREGA, 108)) DataInicioEntrega, ");
                        groupBy.Append("CargaEntrega.CEN_DATA_INICIO_ENTREGA, ");

                        SetarJoinsCargaEntrega(joins);
                    }
                    break;

                case "DataEntrega":
                    if (!select.Contains(" DataEntrega, "))
                    {
                        select.Append("(CONVERT(NVARCHAR(10), CargaEntrega.CEN_DATA_FIM_ENTREGA, 103) + ' ' + CONVERT(NVARCHAR(5), CargaEntrega.CEN_DATA_FIM_ENTREGA, 108)) DataEntrega, ");
                        groupBy.Append("CargaEntrega.CEN_DATA_FIM_ENTREGA, ");

                        SetarJoinsCargaEntrega(joins);
                    }
                    break;

                case "StatusViagem":
                    if (!select.Contains(" StatusViagem, "))
                    {
                        select.Append(@"(select top 1 StatusViagem.MSV_DESCRICAO from T_MONITORAMENTO_HISTORICO_STATUS_VIAGEM Historico
                                            join T_MONITORAMENTO_STATUS_VIAGEM StatusViagem on StatusViagem.MSV_CODIGO = Historico.MSV_CODIGO
                                            where StatusViagem.MSV_TIPO_REGRA in (5,6,7,8,9,10) and Historico.MON_CODIGO = Monitoramento.MON_CODIGO
                                            order by Historico.MHS_DATA_FIM desc) StatusViagem, ");

                        if (!groupBy.Contains("Monitoramento.MON_CODIGO,"))
                            groupBy.Append("Monitoramento.MON_CODIGO, ");

                        SetarJoinsMonitoramento(joins);
                    }
                    break;

                case "DataInicioViagem":
                    select.Append("(CONVERT(NVARCHAR(10), Carga.CAR_DATA_INICIO_VIAGEM, 103) + ' ' + CONVERT(NVARCHAR(5), Carga.CAR_DATA_INICIO_VIAGEM, 108)) DataInicioViagem, ");
                    groupBy.Append("Carga.CAR_DATA_INICIO_VIAGEM, ");

                    break;

                case "DataPrevisaoEntrega":
                    if (!select.Contains(" DataPrevisaoEntrega, "))
                    {
                        select.Append(@"SUBSTRING((select distinct ', ' + CONVERT(NVARCHAR(10), Pedido.PED_PREVISAO_ENTREGA, 103) + ' ' + CONVERT(NVARCHAR(5), Pedido.PED_PREVISAO_ENTREGA, 108) 
                                                        from T_PEDIDO Pedido 
                                                        inner join T_CARGA_PEDIDO CargaPedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO 
                                                        where CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO 
                                                        for xml path('')), 3, 1000) DataPrevisaoEntrega, ");
                    }
                    break;

                case "DataEntregaRecalculada":
                    if (!select.Contains(" DataEntregaRecalculada, "))
                    {
                        select.Append(@"(CONVERT(NVARCHAR(10), isnull(CargaEntrega.CEN_DATA_ENTREGA_REPROGRAMADA, CargaEntrega.CEN_DATA_ENTREGA_PREVISTA), 103)
                                 + ' ' + CONVERT(NVARCHAR(5), isnull(CargaEntrega.CEN_DATA_ENTREGA_REPROGRAMADA, CargaEntrega.CEN_DATA_ENTREGA_PREVISTA), 108)) DataEntregaRecalculada, ");

                        groupBy.Append("CargaEntrega.CEN_DATA_ENTREGA_PREVISTA, ");
                        if (!groupBy.Contains("CargaEntrega.CEN_DATA_ENTREGA_REPROGRAMADA,"))
                            groupBy.Append("CargaEntrega.CEN_DATA_ENTREGA_REPROGRAMADA, ");

                        SetarJoinsCargaEntrega(joins);
                    }
                    break;

                case "DataEntradaRaio":
                    if (!select.Contains(" DataEntradaRaio, "))
                    {
                        select.Append("(CONVERT(NVARCHAR(10), CargaEntrega.CEN_DATA_ENTRADA_RAIO, 103) + ' ' + CONVERT(NVARCHAR(5), CargaEntrega.CEN_DATA_ENTRADA_RAIO, 108)) DataEntradaRaio, ");
                        groupBy.Append("CargaEntrega.CEN_DATA_ENTRADA_RAIO, ");

                        SetarJoinsCargaEntrega(joins);
                    }
                    break;

                case "DataSaidaRaio":
                    if (!select.Contains(" DataSaidaRaio, "))
                    {
                        select.Append("(CONVERT(NVARCHAR(10), CargaEntrega.CEN_DATA_SAIDA_RAIO, 103) + ' ' + CONVERT(NVARCHAR(5), CargaEntrega.CEN_DATA_SAIDA_RAIO, 108)) DataSaidaRaio, ");
                        groupBy.Append("CargaEntrega.CEN_DATA_SAIDA_RAIO, ");

                        SetarJoinsCargaEntrega(joins);
                    }
                    break;

                case "SituacaoEntregaFormatada":
                    if (!select.Contains(" SituacaoEntrega, "))
                    {
                        select.Append("CAST(CargaEntrega.CEN_SITUACAO AS VARCHAR(2)) SituacaoEntrega, ");
                        groupBy.Append("CargaEntrega.CEN_SITUACAO, ");

                        SetarJoinsCargaEntrega(joins);
                    }
                    break;

                case "PercentualViagem":
                    if (!select.Contains(" PercentualViagem, "))
                    {
                        select.Append("(Monitoramento.MON_PERCENTUAL_VIAGEM/100) PercentualViagem, ");

                        if (!groupBy.Contains("Monitoramento.MON_PERCENTUAL_VIAGEM,"))
                            groupBy.Append("Monitoramento.MON_PERCENTUAL_VIAGEM, ");

                        SetarJoinsMonitoramento(joins);
                    }
                    break;

                case "Distancia":
                    if (!select.Contains(" Distancia, "))
                    {
                        select.Append("DadosSumarizados.CDS_DISTANCIA Distancia, ");
                        groupBy.Append("DadosSumarizados.CDS_DISTANCIA, ");

                        SetarJoinsCargaDadosSumarizados(joins);
                    }
                    break;

                case "KMRestante":
                    if (!select.Contains(" KMRestante, "))
                    {
                        select.Append("(Monitoramento.MON_DISTANCIA_PREVISTA * (100 - Monitoramento.MON_PERCENTUAL_VIAGEM)/100) KMRestante, ");

                        groupBy.Append("Monitoramento.MON_DISTANCIA_PREVISTA, ");
                        if (!groupBy.Contains("Monitoramento.MON_PERCENTUAL_VIAGEM,"))
                            groupBy.Append("Monitoramento.MON_PERCENTUAL_VIAGEM, ");

                        SetarJoinsMonitoramento(joins);
                    }
                    break;

                case "PesoTotal":
                    if (!select.Contains(" PesoTotal, "))
                    {
                        select.Append("DadosSumarizados.CDS_PESO_TOTAL PesoTotal, ");
                        groupBy.Append("DadosSumarizados.CDS_PESO_TOTAL, ");

                        SetarJoinsCargaDadosSumarizados(joins);
                    }
                    break;

                case "NumeroNotas":
                    if (!select.Contains(" NumeroNotas, "))
                    {
                        select.Append(@"SUBSTRING((select distinct ', ' + cast(nf.NF_NUMERO as varchar(30)) FROM T_CARGA_ENTREGA_PEDIDO cep
                                             inner join T_CARGA_PEDIDO cargapedido on cargapedido.CPE_CODIGO = cep.CPE_CODIGO 
 											 inner join T_PEDIDO_XML_NOTA_FISCAL pnf on pnf.CPE_CODIGO = cargapedido.CPE_CODIGO
 											 inner join T_XML_NOTA_FISCAL nf on nf.NFX_CODIGO = pnf.NFX_CODIGO
											 WHERE cep.CEN_CODIGO = CargaEntrega.CEN_CODIGO
                                            for xml path('')), 3, 1000) NumeroNotas, ");

                        if (!groupBy.Contains("CargaEntrega.CEN_CODIGO,"))
                            groupBy.Append("CargaEntrega.CEN_CODIGO, ");

                        SetarJoinsCargaEntrega(joins);
                    }
                    break;

                case "ValorNotas":
                    if (!select.Contains(" ValorNotas, "))
                    {
                        select.Append(@"ISNULL((select sum(nf.NF_VALOR) FROM T_CARGA_ENTREGA_PEDIDO cep
                                             inner join T_CARGA_PEDIDO cargapedido on cargapedido.CPE_CODIGO = cep.CPE_CODIGO 
 											 inner join T_PEDIDO_XML_NOTA_FISCAL pnf on pnf.CPE_CODIGO = cargapedido.CPE_CODIGO
 											 inner join T_XML_NOTA_FISCAL nf on nf.NFX_CODIGO = pnf.NFX_CODIGO
											 WHERE cep.CEN_CODIGO = CargaEntrega.CEN_CODIGO), 0.0) ValorNotas, ");

                        if (!groupBy.Contains("CargaEntrega.CEN_CODIGO,"))
                            groupBy.Append("CargaEntrega.CEN_CODIGO, ");

                        SetarJoinsCargaEntrega(joins);
                    }
                    break;

                case "ValorFreteAPagar":
                    if (!select.Contains(" ValorFreteAPagar, "))
                    {
                        select.Append("Carga.CAR_VALOR_FRETE_PAGAR ValorFreteAPagar, ");
                        groupBy.Append("Carga.CAR_VALOR_FRETE_PAGAR, ");
                    }
                    break;

                case "DataDeadLCargaNavioViagem":
                    if (!select.Contains(" DataDeadLCargaNavioViagem, "))
                    {
                        select.Append("isnull(( ");
                        select.Append("    select top (1) convert(nvarchar(10), Pedido.PED_NAVIO_VIAGEM_DATA_DEADLCARGA, 103) + ' ' + convert(nvarchar(5), Pedido.PED_NAVIO_VIAGEM_DATA_DEADLCARGA, 108) ");
                        select.Append("      from T_CARGA_PEDIDO CargaPedido ");
                        select.Append("      join T_PEDIDO Pedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO ");
                        select.Append("     where CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append("       and Pedido.PED_NAVIO_VIAGEM_DATA_DEADLCARGA is not null ");
                        select.Append("), '') DataDeadLCargaNavioViagem, ");
                    }
                    break;

                case "DataDeadLineNavioViagem":
                    if (!select.Contains(" DataDeadLineNavioViagem, "))
                    {
                        select.Append("isnull(( ");
                        select.Append("    select top (1) + convert(nvarchar(10), Pedido.PED_NAVIO_VIAGEM_DATA_DEADLINE, 103) + ' ' + convert(nvarchar(5), Pedido.PED_NAVIO_VIAGEM_DATA_DEADLINE, 108) ");
                        select.Append("      from T_CARGA_PEDIDO CargaPedido ");
                        select.Append("      join T_PEDIDO Pedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO ");
                        select.Append("     where CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append("       and Pedido.PED_NAVIO_VIAGEM_DATA_DEADLINE is not null ");
                        select.Append("), '') DataDeadLineNavioViagem, ");
                    }
                    break;

                case "TaxaOcupacaoCarregado":
                    if (!select.Contains(" TaxaOcupacaoCarregado, "))
                    {
                        select.Append(@"ISNULL((select sum(CASE WHEN ModeloVeicular.MVC_CAPACIDADE_PESO_TRANSPORTE > 0 THEN ((Pedido.PED_PESO_TOTAL_CARGA * 100) / ModeloVeicular.MVC_CAPACIDADE_PESO_TRANSPORTE) ELSE 0.0 END)
                                                        from T_PEDIDO Pedido 
                                                        inner join T_MODELO_VEICULAR_CARGA ModeloVeicular on ModeloVeicular.MVC_CODIGO = Pedido.MVC_CODIGO
                                                        inner join T_CARGA_PEDIDO CargaPedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO 
                                                        where CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO), 0.0) TaxaOcupacaoCarregado, ");
                    }
                    break;

                case "TabelaFrete":
                    if (!select.Contains(" TabelaFrete, "))
                    {
                        select.Append("TabelaFrete.TBF_DESCRICAO TabelaFrete, ");
                        groupBy.Append("TabelaFrete.TBF_DESCRICAO, ");

                        SetarJoinsTabelaFrete(joins);
                    }
                    break;

                case "TempoDeslocamentoParaPlanta":
                    if (!select.Contains(" TempoDeslocamentoParaPlanta, "))
                    {
                        select.Append(@"(select isnull(sum(Historico.MHS_TEMPO_SEGUNDOS), 0) from T_MONITORAMENTO_HISTORICO_STATUS_VIAGEM Historico
                                            join T_MONITORAMENTO_STATUS_VIAGEM StatusViagem on StatusViagem.MSV_CODIGO = Historico.MSV_CODIGO
                                            where StatusViagem.MSV_TIPO_REGRA = 5 and Historico.MON_CODIGO = Monitoramento.MON_CODIGO) TempoDeslocamentoParaPlanta, ");

                        if (!groupBy.Contains("Monitoramento.MON_CODIGO,"))
                            groupBy.Append("Monitoramento.MON_CODIGO, ");

                        SetarJoinsMonitoramento(joins);
                    }
                    break;

                case "TempoAguardandoHorarioCarregamento":
                    if (!select.Contains(" TempoAguardandoHorarioCarregamento, "))
                    {
                        select.Append(@"(select isnull(sum(Historico.MHS_TEMPO_SEGUNDOS), 0) from T_MONITORAMENTO_HISTORICO_STATUS_VIAGEM Historico
                                            join T_MONITORAMENTO_STATUS_VIAGEM StatusViagem on StatusViagem.MSV_CODIGO = Historico.MSV_CODIGO
                                            where StatusViagem.MSV_TIPO_REGRA = 6 and Historico.MON_CODIGO = Monitoramento.MON_CODIGO) TempoAguardandoHorarioCarregamento, ");

                        if (!groupBy.Contains("Monitoramento.MON_CODIGO,"))
                            groupBy.Append("Monitoramento.MON_CODIGO, ");

                        SetarJoinsMonitoramento(joins);
                    }
                    break;

                case "TempoAguardandoCarregamento":
                    if (!select.Contains(" TempoAguardandoCarregamento, "))
                    {
                        select.Append(@"(select isnull(sum(Historico.MHS_TEMPO_SEGUNDOS), 0) from T_MONITORAMENTO_HISTORICO_STATUS_VIAGEM Historico
                                            join T_MONITORAMENTO_STATUS_VIAGEM StatusViagem on StatusViagem.MSV_CODIGO = Historico.MSV_CODIGO
                                            where StatusViagem.MSV_TIPO_REGRA = 7 and Historico.MON_CODIGO = Monitoramento.MON_CODIGO) TempoAguardandoCarregamento, ");

                        if (!groupBy.Contains("Monitoramento.MON_CODIGO,"))
                            groupBy.Append("Monitoramento.MON_CODIGO, ");

                        SetarJoinsMonitoramento(joins);
                    }
                    break;

                case "TempoEmCarregamento":
                    if (!select.Contains(" TempoEmCarregamento, "))
                    {
                        select.Append(@"(select isnull(sum(Historico.MHS_TEMPO_SEGUNDOS), 0) from T_MONITORAMENTO_HISTORICO_STATUS_VIAGEM Historico
                                            join T_MONITORAMENTO_STATUS_VIAGEM StatusViagem on StatusViagem.MSV_CODIGO = Historico.MSV_CODIGO
                                            where StatusViagem.MSV_TIPO_REGRA = 8 and Historico.MON_CODIGO = Monitoramento.MON_CODIGO) TempoEmCarregamento, ");

                        if (!groupBy.Contains("Monitoramento.MON_CODIGO,"))
                            groupBy.Append("Monitoramento.MON_CODIGO, ");

                        SetarJoinsMonitoramento(joins);
                    }
                    break;

                case "TempoEmLiberacao":
                    if (!select.Contains(" TempoEmLiberacao, "))
                    {
                        select.Append(@"(select isnull(sum(Historico.MHS_TEMPO_SEGUNDOS), 0) from T_MONITORAMENTO_HISTORICO_STATUS_VIAGEM Historico
                                            join T_MONITORAMENTO_STATUS_VIAGEM StatusViagem on StatusViagem.MSV_CODIGO = Historico.MSV_CODIGO
                                            where StatusViagem.MSV_TIPO_REGRA = 9 and Historico.MON_CODIGO = Monitoramento.MON_CODIGO) TempoEmLiberacao, ");

                        if (!groupBy.Contains("Monitoramento.MON_CODIGO,"))
                            groupBy.Append("Monitoramento.MON_CODIGO, ");

                        SetarJoinsMonitoramento(joins);
                    }
                    break;

                case "TempoTransito":
                    if (!select.Contains(" TempoTransito, "))
                    {
                        select.Append(@"(select isnull(sum(Historico.MHS_TEMPO_SEGUNDOS), 0) from T_MONITORAMENTO_HISTORICO_STATUS_VIAGEM Historico
                                            join T_MONITORAMENTO_STATUS_VIAGEM StatusViagem on StatusViagem.MSV_CODIGO = Historico.MSV_CODIGO
                                            where StatusViagem.MSV_TIPO_REGRA = 10 and Historico.MON_CODIGO = Monitoramento.MON_CODIGO) TempoTransito, ");

                        if (!groupBy.Contains("Monitoramento.MON_CODIGO,"))
                            groupBy.Append("Monitoramento.MON_CODIGO, ");

                        SetarJoinsMonitoramento(joins);
                    }
                    break;

                case "TempoAguardandoHorarioDescarga":
                    if (!select.Contains(" TempoAguardandoHorarioDescarga, "))
                    {
                        select.Append(@"(select isnull(sum(Historico.MHS_TEMPO_SEGUNDOS), 0) from T_MONITORAMENTO_HISTORICO_STATUS_VIAGEM Historico
                                            join T_MONITORAMENTO_STATUS_VIAGEM StatusViagem on StatusViagem.MSV_CODIGO = Historico.MSV_CODIGO
                                            where StatusViagem.MSV_TIPO_REGRA = 11 and Historico.MON_CODIGO = Monitoramento.MON_CODIGO) TempoAguardandoHorarioDescarga, ");

                        if (!groupBy.Contains("Monitoramento.MON_CODIGO,"))
                            groupBy.Append("Monitoramento.MON_CODIGO, ");

                        SetarJoinsMonitoramento(joins);
                    }
                    break;

                case "TempoAguardandoDescarga":
                    if (!select.Contains(" TempoAguardandoDescarga, "))
                    {
                        select.Append(@"(select isnull(sum(Historico.MHS_TEMPO_SEGUNDOS), 0) from T_MONITORAMENTO_HISTORICO_STATUS_VIAGEM Historico
                                            join T_MONITORAMENTO_STATUS_VIAGEM StatusViagem on StatusViagem.MSV_CODIGO = Historico.MSV_CODIGO
                                            where StatusViagem.MSV_TIPO_REGRA = 12 and Historico.MON_CODIGO = Monitoramento.MON_CODIGO) TempoAguardandoDescarga, ");

                        if (!groupBy.Contains("Monitoramento.MON_CODIGO,"))
                            groupBy.Append("Monitoramento.MON_CODIGO, ");

                        SetarJoinsMonitoramento(joins);
                    }
                    break;

                case "TempoDescarga":
                    if (!select.Contains(" TempoDescarga, "))
                    {
                        select.Append(@"(select isnull(sum(Historico.MHS_TEMPO_SEGUNDOS), 0) from T_MONITORAMENTO_HISTORICO_STATUS_VIAGEM Historico
                                            join T_MONITORAMENTO_STATUS_VIAGEM StatusViagem on StatusViagem.MSV_CODIGO = Historico.MSV_CODIGO
                                            where StatusViagem.MSV_TIPO_REGRA = 13 and Historico.MON_CODIGO = Monitoramento.MON_CODIGO) TempoDescarga, ");

                        if (!groupBy.Contains("Monitoramento.MON_CODIGO,"))
                            groupBy.Append("Monitoramento.MON_CODIGO, ");

                        SetarJoinsMonitoramento(joins);
                    }
                    break;

                case "CapacidadeKGVeiculo":
                    if (!select.Contains(" CapacidadeKGVeiculo, "))
                    {
                        select.Append("Veiculo.VEI_CAP_KG as CapacidadeKGVeiculo, ");
                        groupBy.Append("Veiculo.VEI_CAP_KG, ");

                        SetarJoinsVeiculo(joins);
                    }
                    break;

                case "DataCriacaoCarga":
                case "DataCriacaoCargaFormatada":
                    if (!select.Contains(" DataCriacaoCarga, "))
                    {
                        select.Append("Carga.CAR_DATA_CRIACAO as DataCriacaoCarga, ");
                        groupBy.Append("Carga.CAR_DATA_CRIACAO, ");
                    }
                    break;

                case "DataSalvamentoDadosTransporte":
                case "DataSalvamentoDadosTransporteFormatada":
                    if (!select.Contains(" DataSalvamentoDadosTransporte, "))
                    {
                        select.Append("Carga.CAR_DATA_SALVAMENTO_DADOS_TRANSPORTE as DataSalvamentoDadosTransporte, ");
                        groupBy.Append("Carga.CAR_DATA_SALVAMENTO_DADOS_TRANSPORTE, ");
                    }
                    break;

                case "DataConfirmacaoEnvioDocumentos":
                case "DataConfirmacaoEnvioDocumentosFormatada":
                    if (!select.Contains(" DataConfirmacaoEnvioDocumentos, "))
                    {
                        select.Append("Carga.CAR_DATA_CONFIRMACAO_DOCUMENTOS_FISCAIS as DataConfirmacaoEnvioDocumentos, ");
                        groupBy.Append("Carga.CAR_DATA_CONFIRMACAO_DOCUMENTOS_FISCAIS, ");
                    }
                    break;

                case "DataConfirmacaoValorFrete":
                case "DataConfirmacaoValorFreteFormatada":
                    if (!select.Contains(" DataConfirmacaoValorFrete, "))
                    {
                        select.Append("Carga.CAR_DATA_CONFIRMACAO_VALOR_FRETE as DataConfirmacaoValorFrete, ");
                        groupBy.Append("Carga.CAR_DATA_CONFIRMACAO_VALOR_FRETE, ");
                    }
                    break;

                case "DataInicioEmissaoDocumentos":
                case "DataInicioEmissaoDocumentosFormatada":
                    if (!select.Contains(" DataInicioEmissaoDocumentos, "))
                    {
                        select.Append("Carga.CAR_DATA_INICIO_EMISSAO_DOCUMENTOS as DataInicioEmissaoDocumentos, ");
                        groupBy.Append("Carga.CAR_DATA_INICIO_EMISSAO_DOCUMENTOS, ");
                    }
                    break;

                case "DataFimEmissaoDocumentos":
                case "DataFimEmissaoDocumentosFormatada":
                    if (!select.Contains(" DataFimEmissaoDocumentos, "))
                    {
                        select.Append("Carga.CAR_DATA_FINALIZACAO_EMISSAO as DataFimEmissaoDocumentos, ");
                        groupBy.Append("Carga.CAR_DATA_FINALIZACAO_EMISSAO, ");
                    }
                    break;

                case "DataEnvioCTeOcorrencia":
                case "DataEnvioCTeOcorrenciaFormatada":
                    if (!select.Contains(" DataEnvioCTeOcorrencia, "))
                    {
                        select.Append("OcorrenciaCTeIntegracao.INT_DATA_INTEGRACAO DataEnvioCTeOcorrencia, ");
                        groupBy.Append("OcorrenciaCTeIntegracao.INT_DATA_INTEGRACAO, ");

                        SetarJoinsOcorrenciaCTeIntegracao(joins);
                    }
                    break;

                case "DataPrevisaoEntregaPedidoRecalculada":
                case "DataPrevisaoEntregaPedidoRecalculadaFormatada":
                    if (!select.Contains(" DataPrevisaoEntregaPedidoRecalculada, "))
                    {
                        select.Append("CargaEntrega.CEN_DATA_ENTREGA_REPROGRAMADA DataPrevisaoEntregaPedidoRecalculada, ");
                        groupBy.Append("CargaEntrega.CEN_DATA_ENTREGA_REPROGRAMADA, ");

                        SetarJoinsCargaEntrega(joins);
                    }
                    break;

                case "CidadeOrigemPedido":
                    if (!select.Contains(" CidadeOrigemPedido, "))
                    {
                        select.Append(@"CidadeOrigemPedido.LOC_DESCRICAO AS CidadeOrigemPedido, ");
                        SetarJoinsCidadeOrigemPedido(joins);
                        groupBy.Append("CidadeOrigemPedido.LOC_DESCRICAO, ");
                    }
                    break;

                case "CidadeDestinoPedido":
                    if (!select.Contains(" CidadeDestinoPedido, "))
                    {
                        select.Append(@"CidadeDestinoPedido.LOC_DESCRICAO AS CidadeDestinoPedido, ");
                        SetarJoinsCidadeDestinoPedido(joins);
                        groupBy.Append("CidadeDestinoPedido.LOC_DESCRICAO, ");
                    }
                    break;

                case "ClienteOrigemPedido":
                    if (!select.Contains(" ClienteOrigemPedido, "))
                    {
                        select.Append(@"ClienteOrigemPedido.CLI_NOME AS ClienteOrigemPedido, ");
                        SetarJoinsClienteOrigemPedido(joins);
                        groupBy.Append("ClienteOrigemPedido.CLI_NOME, ");
                    }
                    break;

                case "ClienteDestinoPedido":
                    if (!select.Contains(" ClienteDestinoPedido, "))
                    {
                        select.Append(@"ClienteDestinoPedido.CLI_NOME AS ClienteDestinoPedido, ");
                        SetarJoinsClienteDestinoPedido(joins);
                        groupBy.Append("ClienteDestinoPedido.CLI_NOME, ");
                    }
                    break;

                case "GrupoPessoasTomador":
                    if (!select.Contains(" GrupoPessoasTomador, "))
                    {
                        select.Append(@"GrupoPessoasTomador.GRP_DESCRICAO AS GrupoPessoasTomador, ");
                        SetarJoinsGrupoPessoasTomador(joins);
                        groupBy.Append("GrupoPessoasTomador.GRP_DESCRICAO, ");
                    }
                    break;
            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioMonitoramentoTempoVeiculo filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            string pattern = "yyyy-MM-dd";

            groupBy.Append("Carga.CAR_CODIGO, ");

            SetarJoinsCargaEntrega(joins);

            if (filtrosPesquisa.DataInicioEntregaInicial != DateTime.MinValue)
                where.Append($" and CAST(CargaEntrega.CEN_DATA_INICIO_ENTREGA AS DATE) >= '{filtrosPesquisa.DataInicioEntregaInicial.ToString(pattern)}'");

            if (filtrosPesquisa.DataInicioEntregaFinal != DateTime.MinValue)
                where.Append($" and CAST(CargaEntrega.CEN_DATA_INICIO_ENTREGA AS DATE) <= '{filtrosPesquisa.DataInicioEntregaFinal.ToString(pattern)}'");

            if (filtrosPesquisa.DataEntregaInicial != DateTime.MinValue)
                where.Append($" and CAST(CargaEntrega.CEN_DATA_FIM_ENTREGA AS DATE) >= '{filtrosPesquisa.DataEntregaInicial.ToString(pattern)}'");

            if (filtrosPesquisa.DataEntregaFinal != DateTime.MinValue)
                where.Append($" and CAST(CargaEntrega.CEN_DATA_FIM_ENTREGA AS DATE) <= '{filtrosPesquisa.DataEntregaFinal.ToString(pattern)}'");

            if (filtrosPesquisa.DataEmissaoInicial != DateTime.MinValue)
                where.Append($" and CAST(Carga.CAR_DATA_INICIO_EMISSAO_DOCUMENTOS AS DATE) >= '{filtrosPesquisa.DataEmissaoInicial.ToString(pattern)}'");

            if (filtrosPesquisa.DataEmissaoFinal != DateTime.MinValue)
                where.Append($" and CAST(Carga.CAR_DATA_FINALIZACAO_EMISSAO AS DATE) <= '{filtrosPesquisa.DataEmissaoFinal.ToString(pattern)}'");

            if (filtrosPesquisa.CodigosFilial.Any(codigo => codigo == -1))
            {
                where.Append($@" and (Filial.FIL_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosFilial)}) OR EXISTS(   SELECT _cargaPedidoRecebedor.CAR_CODIGO 
                                                                                                                       FROM T_CARGA_PEDIDO _cargaPedidoRecebedor 
                                                                                                                       LEFT JOIN T_PEDIDO _pedido ON _pedido.PED_CODIGO = _cargaPedidoRecebedor.PED_CODIGO
                                                                                                                       WHERE Carga.CAR_CODIGO = _cargaPedidoRecebedor.CAR_CODIGO
                                                                                                                       AND _pedido.FIL_CODIGO IN ({string.Join(",", filtrosPesquisa.Recebedores)}) ) )");
                SetarJoinsFilial(joins);
            }
            else if (filtrosPesquisa.CodigosFilial.Count > 0)
            {
                where.Append($" and Filial.FIL_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosFilial)})");
                SetarJoinsFilial(joins);
            }

            if (filtrosPesquisa.CodigosTransportador.Count > 0)
                where.Append($" and Carga.EMP_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosTransportador)})");

            if (filtrosPesquisa.CodigosClienteEntrega.Count > 0)
            {
                where.Append($" and ClienteCargaEntrega.CLI_CGCCPF in ({string.Join(", ", filtrosPesquisa.CodigosClienteEntrega)})");
                SetarJoinsClienteCargaEntrega(joins);
            }

            if (filtrosPesquisa.CodigosCarga.Count > 0)
                where.Append($" and Carga.CAR_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosCarga)})");

            //Filtros no Pedido
            StringBuilder wherePedido = new StringBuilder();

            if (filtrosPesquisa.CodigoOrigem > 0)
                wherePedido.Append($" and _pedido.LOC_CODIGO_ORIGEM = {filtrosPesquisa.CodigoOrigem} ");

            if (filtrosPesquisa.CodigoDestino > 0)
                wherePedido.Append($" and _pedido.LOC_CODIGO_DESTINO = {filtrosPesquisa.CodigoDestino} ");

            if (wherePedido.Length > 0)
            {
                where.Append(" and Carga.CAR_CODIGO in (");
                where.Append("         select _cargapedido.CAR_CODIGO ");
                where.Append("           from T_CARGA_PEDIDO _cargapedido");
                where.Append("           join T_PEDIDO _pedido on _pedido.PED_CODIGO = _cargapedido.PED_CODIGO ");
                where.Append($"         where {wherePedido.ToString().Trim().Substring(3)} ");
                where.Append("     ) ");
            }
        }

        #endregion
    }
}
