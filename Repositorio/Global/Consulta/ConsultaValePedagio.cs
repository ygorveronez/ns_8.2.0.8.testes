using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Global.Consulta
{
    sealed class ConsultaValePedagio : Embarcador.Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaValePedagioRelatorio>
    {
        #region Construtores

        public ConsultaValePedagio() : base(tabela: "T_CARGA_INTEGRACAO_VALE_PEDAGIO as CargaIntegracao") { }

        #endregion

        #region Métodos Privados

        private void SetarJoinCarga(StringBuilder joins)
        {
            if (!joins.Contains(" Carga "))
                joins.Append(" JOIN T_CARGA Carga ON Carga.CAR_CODIGO = CargaIntegracao.CAR_CODIGO ");
        }

        private void SetarJoinFilial(StringBuilder joins)
        {
            SetarJoinCarga(joins);

            if (!joins.Contains(" Filial "))
                joins.Append(" JOIN T_FILIAL Filial ON Carga.FIL_CODIGO = Filial.FIL_CODIGO ");
        }

        private void SetarJoinModeloVeicular(StringBuilder joins)
        {
            SetarJoinCarga(joins);

            if (!joins.Contains(" ModeloVeicular "))
                joins.Append(" JOIN T_MODELO_VEICULAR_CARGA ModeloVeicular ON Carga.MVC_CODIGO = ModeloVeicular.MVC_CODIGO ");
        }

        private void SetarJoinEmpresa(StringBuilder joins)
        {
            SetarJoinCarga(joins);
            if (!joins.Contains(" Empresa "))
                joins.Append(" JOIN T_EMPRESA Empresa ON Carga.EMP_CODIGO = Empresa.EMP_CODIGO ");
        }

        private void SetarJoinTipoCarga(StringBuilder joins)
        {
            SetarJoinCarga(joins);

            if (!joins.Contains(" TipoCarga "))
                joins.Append(" LEFT JOIN T_TIPO_DE_CARGA TipoCarga ON Carga.TCG_CODIGO = TipoCarga.TCG_CODIGO ");
        }

        private void SetarJoinCargaDadosSumarizados(StringBuilder joins)
        {
            SetarJoinCarga(joins);

            if (!joins.Contains(" CargaDadosSumarizados "))
                joins.Append(" JOIN T_CARGA_DADOS_SUMARIZADOS CargaDadosSumarizados ON carga.CDS_CODIGO = CargaDadosSumarizados.CDS_CODIGO ");
        }

        private void SetarJoinTipoOperacao(StringBuilder joins)
        {
            SetarJoinCarga(joins);

            if (!joins.Contains(" TipoOperacao "))
                joins.Append(" JOIN T_TIPO_OPERACAO TipoOperacao ON Carga.TOP_CODIGO = TipoOperacao.TOP_CODIGO ");
        }

        private void SetarJoinsCargaAgrupada(StringBuilder joins)
        {
            if (!joins.Contains(" CargaAgrupada "))
                joins.Append(" left join T_CARGA CargaAgrupada on CargaAgrupada.CAR_CODIGO = carga.CAR_CODIGO_AGRUPAMENTO ");
        }

        private void SetarJoinCargaIntegracaoValePedagioCargaAgrupada(StringBuilder joins)
        {
            SetarJoinsCargaAgrupada(joins);

            if (!joins.Contains(" CargaAgrupadaIntegracao "))
                joins.Append(" left join T_CARGA_INTEGRACAO_VALE_PEDAGIO CargaAgrupadaIntegracao on CargaAgrupada.CAR_CODIGO = CargaAgrupadaIntegracao.CAR_CODIGO ");
        }

        private void SetarJoinCargaIntegracaoValePedagioCarga(StringBuilder joins)
        {
            SetarJoinCarga(joins);

            if (!joins.Contains(" CargaIntegracaoValePedagio "))
                joins.Append(" left join T_CARGA_INTEGRACAO_VALE_PEDAGIO CargaIntegracaoValePedagio on Carga.CAR_CODIGO = CargaIntegracaoValePedagio.CAR_CODIGO ");
        }

        private void SetarJoinCargaIntegracaoValePedagioRotaFreteCarga(StringBuilder joins)
        {
            SetarJoinCargaIntegracaoValePedagioCarga(joins);

            if (!joins.Contains(" RotaFrete "))
                joins.Append(" left join T_ROTA_FRETE RotaFrete on RotaFrete.ROF_CODIGO = CargaIntegracaoValePedagio.ROF_CODIGO ");
        }

        private void SetarJoinsTipoIntegracao(StringBuilder joins)
        {
            if (!joins.Contains(" TipoIntegracao "))
                joins.Append(" left join T_TIPO_INTEGRACAO TipoIntegracao on TipoIntegracao.TPI_CODIGO = CargaIntegracao.TPI_CODIGO ");
        }
        private void SetarJoinsTransportadora(StringBuilder joins)
        {
            if (!joins.Contains(" Transportador "))
                joins.Append(" left join T_EMPRESA Transportador on Transportador.EMP_CODIGO = Carga.EMP_CODIGO ");
        }
        private void SetarJoinsMotorista(StringBuilder joins)
        {
            if (!joins.Contains(" Motorista "))
                joins.Append(" left join T_FUNCIONARIO Motorista on Motorista.FUN_CODIGO = Vinculo.FUN_CODIGO_MOTORISTA ");
        }
        private void SetarJoinsVeiculo(StringBuilder joins)
        {
            if (!joins.Contains(" Veiculo "))
                joins.Append(" JOIN T_VEICULO Veiculo ON Veiculo.VEI_CODIGO = Carga.CAR_VEICULO ");
        }
        private void SetarJoinsCargaDadosSumarizados(StringBuilder joins)
        {
            if (!joins.Contains(" DadosSumarizados "))
                joins.Append(" left join T_CARGA_DADOS_SUMARIZADOS DadosSumarizados ON DadosSumarizados.CDS_CODIGO = Carga.CDS_CODIGO ");
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaValePedagioRelatorio filtrosPesquisa)
        {
            switch (propriedade)
            {
                case "Motoristas":
                    if (!select.Contains(" Motoristas,"))
                    {
                        select.Append(
                            @"SUBSTRING((
                            SELECT DISTINCT ', ' + CAST((
                                    SUBSTRING(Motorista.FUN_CPF, 1, 3) + '.' +
                                    SUBSTRING(Motorista.FUN_CPF, 4, 3) + '.' +
                                    SUBSTRING(Motorista.FUN_CPF, 7, 3) + '-' +
                                    SUBSTRING(Motorista.FUN_CPF, 10, 3) + ' - ' +
                                    Motorista.FUN_NOME
                                ) AS NVARCHAR(4000))
                                FROM T_CARGA_MOTORISTA CargaMotorista
                                JOIN T_FUNCIONARIO Motorista ON CargaMotorista.CAR_MOTORISTA = Motorista.FUN_CODIGO
                                WHERE CargaMotorista.CAR_CODIGO = Carga.CAR_CODIGO FOR XML PATH('')), 3, 4000) Motoristas, ");

                        groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "NumeroCarga":
                    if (!select.Contains("NumeroCarga"))
                    {
                        select.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR NumeroCarga, ");
                        SetarJoinCarga(joins);
                        groupBy.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR, ");
                    }
                    break;

                case "NumeroCargaAgrupada":
                    if (!select.Contains(" NumeroCargaAgrupada "))
                    {
                        select.Append("CargaAgrupada.CAR_CODIGO_CARGA_EMBARCADOR NumeroCargaAgrupada, ");
                        groupBy.Append("CargaAgrupada.CAR_CODIGO_CARGA_EMBARCADOR, ");

                        SetarJoinsCargaAgrupada(joins);
                    }
                    break;

                case "PesoCarga":
                    if (!select.Contains("PesoCarga"))
                    {
                        select.Append("( ");
                        select.Append("    select sum(_cargapedido.PED_PESO) ");
                        select.Append("      from T_CARGA_PEDIDO _cargapedido ");
                        select.Append("     where (CASE WHEN Carga.CAR_CODIGO_AGRUPAMENTO > 0 THEN Carga.CAR_CODIGO_AGRUPAMENTO ELSE Carga.CAR_CODIGO END) = _cargapedido.CAR_CODIGO ");
                        select.Append(") PesoCarga, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO_AGRUPAMENTO, "))
                            groupBy.Append("Carga.CAR_CODIGO_AGRUPAMENTO, ");

                        if (!groupBy.Contains("Carga.CAR_CARGA_AGRUPADA, "))
                            groupBy.Append("Carga.CAR_CARGA_AGRUPADA, ");
                    }
                    break;

                case "Filial":
                    if (!select.Contains("Filial"))
                    {
                        select.Append(" Filial.FIL_DESCRICAO Filial, ");
                        SetarJoinFilial(joins);
                        groupBy.Append("Filial.FIL_DESCRICAO, ");
                    }
                    break;

                case "DataCargaFormatada":
                    if (!select.Contains("DataCargaFormatada"))
                    {
                        select.Append(" Carga.CAR_DATA_CRIACAO DataCarga, ");
                        SetarJoinCarga(joins);
                        groupBy.Append("Carga.CAR_DATA_CRIACAO, ");
                    }
                    break;

                case "Origem":
                    if (!select.Contains("Origem"))
                    {
                        select.Append(" CargaDadosSumarizados.CDS_ORIGENS Origem, ");
                        SetarJoinCargaDadosSumarizados(joins);
                        groupBy.Append("CargaDadosSumarizados.CDS_ORIGENS, ");

                    }
                    break;

                case "Destino":
                    if (!select.Contains("Destino"))
                    {
                        select.Append(" CargaDadosSumarizados.CDS_DESTINOS Destino, ");
                        SetarJoinCargaDadosSumarizados(joins);
                        groupBy.Append("CargaDadosSumarizados.CDS_DESTINOS, ");

                    }
                    break;

                case "TipoCarga":
                    if (!select.Contains("TipoCarga"))
                    {
                        select.Append(" TipoCarga.TCG_DESCRICAO TipoCarga, ");
                        SetarJoinTipoCarga(joins);
                        groupBy.Append("TipoCarga.TCG_DESCRICAO, ");

                    }
                    break;

                case "ModeloVeicular":
                    if (!select.Contains("ModeloVeicular"))
                    {
                        select.Append(" ModeloVeicular.MVC_DESCRICAO ModeloVeicular, ");
                        SetarJoinModeloVeicular(joins);
                        groupBy.Append("ModeloVeicular.MVC_DESCRICAO, ");

                    }
                    break;

                case "TipoOperacao":
                    if (!select.Contains("TipoOperacao"))
                    {
                        select.Append(" TipoOperacao.TOP_DESCRICAO TipoOperacao, ");
                        SetarJoinTipoOperacao(joins);
                        groupBy.Append("TipoOperacao.TOP_DESCRICAO, ");

                    }
                    break;

                case "Transportador":
                    if (!select.Contains("Transportador"))
                    {
                        select.Append(" Empresa.EMP_FANTASIA Transportador, ");
                        SetarJoinEmpresa(joins);
                        groupBy.Append("Empresa.EMP_FANTASIA, ");

                    }
                    break;

                case "NumeroValePedagio":
                    if (!select.Contains("NumeroValePedagio"))
                    {
                        select.Append(" (CASE WHEN Carga.CAR_CODIGO_AGRUPAMENTO > 0 THEN CargaAgrupadaIntegracao.CVP_NUMERO_VALE_PEDAGIO ELSE CargaIntegracao.CVP_NUMERO_VALE_PEDAGIO END) NumeroValePedagio, ");
                        groupBy.Append("CargaIntegracao.CVP_NUMERO_VALE_PEDAGIO, ");
                        groupBy.Append("CargaAgrupadaIntegracao.CVP_NUMERO_VALE_PEDAGIO, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO_AGRUPAMENTO, "))
                            groupBy.Append("Carga.CAR_CODIGO_AGRUPAMENTO, ");

                        SetarJoinCarga(joins);
                        SetarJoinCargaIntegracaoValePedagioCargaAgrupada(joins);
                    }
                    break;

                case "SituacaoValePedagioDescricao":
                    if (!select.Contains("SituacaoValePedagioDescricao"))
                    {
                        select.Append(" (CASE WHEN Carga.CAR_CODIGO_AGRUPAMENTO > 0 THEN CargaAgrupadaIntegracao.CVP_SITUACAO_VALE_PEDAGIO ELSE CargaIntegracao.CVP_SITUACAO_VALE_PEDAGIO END) SituacaoValePedagio, ");
                        groupBy.Append("CargaIntegracao.CVP_SITUACAO_VALE_PEDAGIO, ");
                        groupBy.Append("CargaAgrupadaIntegracao.CVP_SITUACAO_VALE_PEDAGIO, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO_AGRUPAMENTO, "))
                            groupBy.Append("Carga.CAR_CODIGO_AGRUPAMENTO, ");

                        SetarJoinCarga(joins);
                        SetarJoinCargaIntegracaoValePedagioCargaAgrupada(joins);
                    }
                    break;

                case "ValorValePedagio":
                    if (!select.Contains("ValorValePedagio"))
                    {
                        select.Append(" (CASE WHEN Carga.CAR_CODIGO_AGRUPAMENTO > 0 THEN CargaAgrupadaIntegracao.CVP_VALOR_VALE_PEDAGIO ELSE CargaIntegracao.CVP_VALOR_VALE_PEDAGIO END) ValorValePedagio, ");
                        groupBy.Append("CargaIntegracao.CVP_VALOR_VALE_PEDAGIO, ");
                        groupBy.Append("CargaAgrupadaIntegracao.CVP_VALOR_VALE_PEDAGIO, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO_AGRUPAMENTO, "))
                            groupBy.Append("Carga.CAR_CODIGO_AGRUPAMENTO, ");

                        SetarJoinCarga(joins);
                        SetarJoinCargaIntegracaoValePedagioCargaAgrupada(joins);
                    }
                    break;

                case "SituacaoIntegracaoValePedagioDescricao":
                    if (!select.Contains("SituacaoIntegracaoValePedagioDescricao"))
                    {
                        select.Append(" (CASE WHEN Carga.CAR_CODIGO_AGRUPAMENTO > 0 THEN CargaAgrupadaIntegracao.INT_SITUACAO_INTEGRACAO ELSE CargaIntegracao.INT_SITUACAO_INTEGRACAO END) SituacaoIntegracaoValePedagio, ");
                        groupBy.Append("CargaIntegracao.INT_SITUACAO_INTEGRACAO, ");
                        groupBy.Append("CargaAgrupadaIntegracao.INT_SITUACAO_INTEGRACAO, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO_AGRUPAMENTO, "))
                            groupBy.Append("Carga.CAR_CODIGO_AGRUPAMENTO, ");

                        SetarJoinCarga(joins);
                        SetarJoinCargaIntegracaoValePedagioCargaAgrupada(joins);
                    }
                    break;

                case "DataRetornoValePedagioFormatada":
                    if (!select.Contains("DataRetornoValePedagioFormatada"))
                    {
                        select.Append(" CargaIntegracao.INT_DATA_INTEGRACAO DataRetornoValePedagio, ");
                        groupBy.Append("CargaIntegracao.INT_DATA_INTEGRACAO, ");
                    }
                    break;

                case "RetornoIntegracao":
                    if (!select.Contains("RetornoIntegracao"))
                    {
                        select.Append(" (CASE WHEN Carga.CAR_CODIGO_AGRUPAMENTO > 0 THEN CargaAgrupadaIntegracao.INT_PROBLEMA_INTEGRACAO ELSE CargaIntegracao.INT_PROBLEMA_INTEGRACAO END) RetornoIntegracao, ");
                        groupBy.Append("CargaIntegracao.INT_PROBLEMA_INTEGRACAO, ");
                        groupBy.Append("CargaAgrupadaIntegracao.INT_PROBLEMA_INTEGRACAO, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO_AGRUPAMENTO, "))
                            groupBy.Append("Carga.CAR_CODIGO_AGRUPAMENTO, ");

                        SetarJoinCarga(joins);
                        SetarJoinCargaIntegracaoValePedagioCargaAgrupada(joins);
                    }
                    break;

                case "VeiculosCarga":
                    if (!select.Contains("VeiculosCarga"))
                    {
                        select.Append("substring( ");
                        select.Append("     (select Veiculo.VEI_PLACA from T_VEICULO Veiculo ");
                        select.Append("     where Veiculo.VEI_CODIGO = Carga.CAR_VEICULO) + ISNULL((SELECT ', ' + veiculo1.VEI_PLACA ");
                        select.Append("     FROM T_CARGA_VEICULOS_VINCULADOS veiculoVinculadoCarga1 ");
                        select.Append("     INNER JOIN T_VEICULO veiculo1 ON veiculoVinculadoCarga1.VEI_CODIGO = veiculo1.VEI_CODIGO ");
                        select.Append("     WHERE veiculoVinculadoCarga1.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append("     FOR XML PATH('')), ''), 0, 100) VeiculosCarga, ");

                        SetarJoinCarga(joins);
                        if (!groupBy.Contains("Carga.CAR_VEICULO, "))
                            groupBy.Append("Carga.CAR_VEICULO, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO, "))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "SituacaoCarga":
                case "SituacaoCargaDescricao":
                    if (!select.Contains("SituacaoCarga"))
                    {
                        select.Append(" Carga.CAR_SITUACAO SituacaoCarga, ");

                        if (!groupBy.Contains("Carga.CAR_SITUACAO, "))
                            groupBy.Append("Carga.CAR_SITUACAO, ");

                        SetarJoinCarga(joins);
                    }
                    break;

                case "CNPJFilialFormatado":
                    if (!select.Contains("CNPJFilial"))
                    {
                        select.Append(" Filial.FIL_CNPJ CNPJFilial, ");

                        if (!groupBy.Contains("Filial.FIL_CNPJ, "))
                            groupBy.Append("Filial.FIL_CNPJ, ");

                        SetarJoinFilial(joins);
                    }
                    break;

                case "CNPJTransportadorFormatado":
                    if (!select.Contains("CNPJTransportador"))
                    {
                        select.Append(" Empresa.EMP_CNPJ CNPJTransportador, ");

                        if (!groupBy.Contains("Empresa.EMP_CNPJ, "))
                            groupBy.Append("Empresa.EMP_CNPJ, ");

                        SetarJoinEmpresa(joins);
                    }
                    break;

                case "NumValePedagio":
                    if (!select.Contains("NumValePedagio"))
                    {
                        select.Append(" CargaIntegracao.CVP_NUMERO_VALE_PEDAGIO NumValePedagio, ");

                        if (!groupBy.Contains("CargaIntegracao.CVP_NUMERO_VALE_PEDAGIO, "))
                            groupBy.Append("CargaIntegracao.CVP_NUMERO_VALE_PEDAGIO, ");
                    }
                    break;

                case "TipoIntegracaoDescricao":
                    if (!select.Contains("Integradora"))
                    {
                        select.Append(" TipoIntegracao.TPI_TIPO Integradora, ");

                        if (!groupBy.Contains("TipoIntegracao.TPI_TIPO, "))
                            groupBy.Append("TipoIntegracao.TPI_TIPO, ");

                        SetarJoinsTipoIntegracao(joins);
                    }
                    break;

                case "TipoCompraValePedagioDescricao":
                    if (!select.Contains("TipoCompraValePedagio"))
                    {
                        select.Append(" CargaIntegracao.CVP_TIPO_COMPRA TipoCompraValePedagio, ");

                        if (!groupBy.Contains("CargaIntegracao.CVP_TIPO_COMPRA, "))
                            groupBy.Append("CargaIntegracao.CVP_TIPO_COMPRA, ");
                    }
                    break;

                case "TipoPercursoVPDescricao":
                    if (!select.Contains("TipoPercursoVP"))
                    {
                        select.Append(" isnull(CargaIntegracao.CVP_TIPO_PERCURSO_VP, -1) TipoPercursoVP, ");

                        if (!groupBy.Contains("CargaIntegracao.CVP_TIPO_PERCURSO_VP, "))
                            groupBy.Append("CargaIntegracao.CVP_TIPO_PERCURSO_VP, ");
                    }
                    break;
                case "Expedidor":
                    if (!select.Contains(" Expedidor"))
                    {
                        select.Append("DadosSumarizados.CDS_EXPEDIDORES Expedidor, ");
                        groupBy.Append("DadosSumarizados.CDS_EXPEDIDORES, ");

                        SetarJoinsCargaDadosSumarizados(joins);
                    }
                    break;

                case "Recebedor":
                    if (!select.Contains(" Recebedor, "))
                    {
                        select.Append("DadosSumarizados.CDS_RECEBEDORES Recebedor, ");
                        groupBy.Append("DadosSumarizados.CDS_RECEBEDORES, ");

                        SetarJoinsCargaDadosSumarizados(joins);
                    }
                    break;

                case "RotaFrete":
                    if (!select.Contains(" RotaFrete "))
                    {
                        select.Append("RotaFrete.ROF_DESCRICAO RotaFrete, ");
                        groupBy.Append("RotaFrete.ROF_DESCRICAO, ");

                        SetarJoinCargaIntegracaoValePedagioRotaFreteCarga(joins);
                    }
                    break;
                case "ModoCompraValePedagioTargetDescricao":
                    if (!select.Contains(" ModoCompraValePedagioTarget, "))
                    {

                        select.Append("Veiculo.VEI_MEIO_COMPRA_VALE_PEDAGIO_TARGET ModoCompraValePedagioTarget, ");
                        groupBy.Append("Veiculo.VEI_MEIO_COMPRA_VALE_PEDAGIO_TARGET, ");
                        SetarJoinsVeiculo(joins);
                    }
                    break;
            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaValePedagioRelatorio filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            string pattern = "yyyy-MM-dd";

            if (filtrosPesquisa.ExibirCargasAgrupadas)
                where.Append(" and Carga.CAR_CARGA_FECHADA = 1 ");
            else
            {
                where.Append(" and ((Carga.CAR_CARGA_FECHADA = 1 ");

                if (!filtrosPesquisa.ExibirTodasCargasPorPadrao)
                    where.Append("and Carga.CAR_CARGA_AGRUPADA = 0 ");

                where.Append(") or (Carga.CAR_CARGA_FECHADA = 0 and Carga.CAR_CODIGO_AGRUPAMENTO is not null))");
            }

            if (filtrosPesquisa.CodigoCarga > 0)
            {
                where.Append($" and (Carga.CAR_CODIGO = {filtrosPesquisa.CodigoCarga} or CargaAgrupada.CAR_CODIGO = {filtrosPesquisa.CodigoCarga})");

                SetarJoinsCargaAgrupada(joins);
            }

            if (filtrosPesquisa.DataCargaInicial != DateTime.MinValue)
                where.Append($" AND Carga.CAR_DATA_CRIACAO >= '{filtrosPesquisa.DataCargaInicial.Date.ToString(pattern)}'");

            if (filtrosPesquisa.DataCargaFinal != DateTime.MinValue)
                where.Append($" AND Carga.CAR_DATA_CRIACAO < '{filtrosPesquisa.DataCargaFinal.Date.AddDays(1).ToString(pattern)}'");

            if (filtrosPesquisa.CodigoTipoOperacao > 0)
                where.Append(" AND TipoOperacao.TOP_CODIGO = " + filtrosPesquisa.CodigoTipoOperacao);

            if (filtrosPesquisa.CodigosFiliais.Any(codigo => codigo == -1))
            {
                where.Append($@" and (Filial.FIL_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosFiliais)}) OR EXISTS (   SELECT _cargaPedidoRecebedor.CAR_CODIGO 
                                                                                                                       FROM T_CARGA_PEDIDO _cargaPedidoRecebedor 
                                                                                                                       LEFT JOIN T_PEDIDO _pedido ON _pedido.PED_CODIGO = _cargaPedidoRecebedor.PED_CODIGO
                                                                                                                       WHERE Carga.CAR_CODIGO = _cargaPedidoRecebedor.CAR_CODIGO
                                                                                                                       AND _pedido.CLI_CODIGO_RECEBEDOR IN ({string.Join(",", filtrosPesquisa.CodigosRecebedores)})))");
                SetarJoinFilial(joins);
            }
            else if (filtrosPesquisa.CodigosFiliais.Count() > 0)
            {
                where.Append($" and (Filial.FIL_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosFiliais)})) ");
                SetarJoinFilial(joins);
            }

            if (filtrosPesquisa.NumeroValePedagio.Count > 0)
                where.Append($" AND CargaIntegracao.CVP_CODIGO IN ({string.Join(", ", filtrosPesquisa.NumeroValePedagio)}) ");

            if (filtrosPesquisa.SituacaoValePedagio.Count > 0)
                where.Append($" AND CargaIntegracao.CVP_SITUACAO_VALE_PEDAGIO in ({string.Join(", ", filtrosPesquisa.SituacaoValePedagio.Select(o => o.ToString("D")))}) ");

            if (filtrosPesquisa.SituacaoIntegracaoValePedagio.Count > 0)
                where.Append($" AND CargaIntegracao.INT_SITUACAO_INTEGRACAO in ({string.Join(", ", filtrosPesquisa.SituacaoIntegracaoValePedagio.Select(o => o.ToString("D")))}) ");

            if (filtrosPesquisa.DataCompraVPRInicial != DateTime.MinValue)
                where.Append($" AND CargaIntegracao.INT_DATA_INTEGRACAO >= '{filtrosPesquisa.DataCompraVPRInicial.ToString(pattern)}'");

            if (filtrosPesquisa.DataCompraVPRFinal != DateTime.MinValue)
                where.Append($" AND CargaIntegracao.INT_DATA_INTEGRACAO < '{filtrosPesquisa.DataCompraVPRFinal.AddDays(1).ToString(pattern)}'");

            if (filtrosPesquisa.Transportador > 0)
                where.Append($" AND Carga.EMP_CODIGO = '" + filtrosPesquisa.Transportador + "'");
            if (filtrosPesquisa.Expedidor > 0)
                where.Append($" AND Carga.CLI_CODIGO_EXPEDIDOR = {filtrosPesquisa.Expedidor} ");

            if (filtrosPesquisa.Recebedor > 0)
                where.Append($" AND Carga.CLI_CODIGO_RECEBEDOR = {filtrosPesquisa.Recebedor} ");

            if (filtrosPesquisa.Veiculo > 0)
            {
                where.Append(" and ( ");
                where.Append($" Carga.CAR_VEICULO = {filtrosPesquisa.Veiculo} or ");
                where.Append("  Carga.CAR_CODIGO in ( ");
                where.Append("  select _cargaveiculos.CAR_CODIGO ");
                where.Append("  from T_CARGA_VEICULOS_VINCULADOS _cargaveiculos ");
                where.Append($" WHERE _cargaveiculos.VEI_CODIGO = {filtrosPesquisa.Veiculo} ");
                where.Append(") ");
                where.Append("     )");
            }

            if (filtrosPesquisa.Motorista > 0)
            {
                where.Append("  and Carga.CAR_CODIGO in ( ");
                where.Append("  select _cargamotorista.CAR_CODIGO ");
                where.Append("  from T_CARGA_MOTORISTA _cargamotorista ");
                where.Append($" where _cargamotorista.CAR_MOTORISTA = {filtrosPesquisa.Motorista} ");
                where.Append("   )");
            }
        }

        #endregion
    }
}
