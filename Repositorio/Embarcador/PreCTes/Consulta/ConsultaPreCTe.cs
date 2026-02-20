using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.PreCTes.Consulta
{
    sealed class ConsultaPreCTe : Embarcador.Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.PreCTes.FiltroPesquisaRelatorioPreCTe>
    {
        #region Construtores

        public ConsultaPreCTe() : base(tabela: "T_PRE_CTE as PreCTe") { }

        #endregion

        #region Métodos Privados

        private void SetarJoinsCargaCTe(StringBuilder joins)
        {
            if (!joins.Contains(" CargaCTe "))
                joins.Append(" left join T_CARGA_CTE CargaCTe on CargaCTe.PCO_CODIGO = PreCTe.PCO_CODIGO ");
        }

        private void SetarJoinsCTe(StringBuilder joins)
        {
            SetarJoinsCargaCTe(joins);

            if (!joins.Contains(" CTe "))
                joins.Append(" left join T_CTE CTe on CTe.CON_CODIGO = CargaCTe.CON_CODIGO ");
        }

        private void SetarJoinsCarga(StringBuilder joins)
        {
            SetarJoinsCargaCTe(joins);

            if (!joins.Contains(" Carga "))
                joins.Append(" left JOIN T_CARGA Carga on Carga.CAR_CODIGO = CargaCTe.CAR_CODIGO ");
        }

        private void SetarJoinsTipoOperacao(StringBuilder joins)
        {

            SetarJoinsCarga(joins);

            if (!joins.Contains(" TipoOperacao "))
                joins.Append(" left JOIN T_TIPO_OPERACAO TipoOperacao on Carga.TOP_CODIGO = TipoOperacao.TOP_CODIGO ");
        }

        private void SetarJoinsCfop(StringBuilder joins)
        {
            if (!joins.Contains(" CFOP "))
                joins.Append(" left join T_CFOP CFOP on CFOP.CFO_CODIGO = PreCTe.CFO_CODIGO ");
        }

        private void SetarJoinsRemetente(StringBuilder joins)
        {
            if (!joins.Contains(" RemetentePreCTe "))
                joins.Append(" left join T_CTE_PARTICIPANTE RemetentePreCTe on PreCTe.PCO_REMETENTE_CTE = RemetentePreCTe.PCT_CODIGO ");
        }

        private void SetarJoinsRemetenteCliente(StringBuilder joins)
        {
            SetarJoinsRemetente(joins);

            if (!joins.Contains(" ClienteRemetente "))
                joins.Append(" left join T_CLIENTE ClienteRemetente on ClienteRemetente.CLI_CGCCPF = RemetentePreCTe.CLI_CODIGO ");
        }

        private void SetarJoinsRemetenteLocalidade(StringBuilder joins)
        {
            SetarJoinsRemetente(joins);

            if (!joins.Contains(" LocalidadeRemetente "))
                joins.Append(" left join T_LOCALIDADES LocalidadeRemetente on RemetentePreCTe.LOC_CODIGO = LocalidadeRemetente.LOC_CODIGO ");
        }

        private void SetarJoinsRecebedor(StringBuilder joins)
        {
            if (!joins.Contains(" RecebedorPreCTe "))
                joins.Append(" left join T_CTE_PARTICIPANTE RecebedorPreCTe on PreCTe.PCO_RECEBEDOR_CTE = RecebedorPreCTe.PCT_CODIGO ");
        }

        private void SetarJoinsRecebedorCliente(StringBuilder joins)
        {
            SetarJoinsRecebedor(joins);

            if (!joins.Contains(" ClienteRecebedor "))
                joins.Append(" left join T_CLIENTE ClienteRecebedor on ClienteRecebedor.CLI_CGCCPF = RecebedorPreCTe.CLI_CODIGO ");
        }

        private void SetarJoinsRecebedorLocalidade(StringBuilder joins)
        {
            SetarJoinsRecebedor(joins);

            if (!joins.Contains(" LocalidadeRecebedor "))
                joins.Append(" left join T_LOCALIDADES LocalidadeRecebedor on RecebedorPreCTe.LOC_CODIGO = LocalidadeRecebedor.LOC_CODIGO ");
        }

        private void SetarJoinsDestinatario(StringBuilder joins)
        {
            if (!joins.Contains(" DestinatarioPreCTe "))
                joins.Append(" left join T_CTE_PARTICIPANTE DestinatarioPreCTe on PreCTe.PCO_DESTINATARIO_CTE = DestinatarioPreCTe.PCT_CODIGO ");
        }

        private void SetarJoinsDestinatarioCliente(StringBuilder joins)
        {
            SetarJoinsDestinatario(joins);

            if (!joins.Contains("ClienteDestinatario"))
                joins.Append(" left join T_CLIENTE ClienteDestinatario on ClienteDestinatario.CLI_CGCCPF = DestinatarioPreCTe.CLI_CODIGO ");
        }

        private void SetarJoinsDestinatarioLocalidade(StringBuilder joins)
        {
            SetarJoinsDestinatario(joins);

            if (!joins.Contains(" LocalidadeDestinatario "))
                joins.Append(" left join T_LOCALIDADES LocalidadeDestinatario on DestinatarioPreCTe.LOC_CODIGO = LocalidadeDestinatario.LOC_CODIGO ");
        }

        private void SetarJoinsExpedidor(StringBuilder joins)
        {
            if (!joins.Contains(" ExpedidorPreCTe "))
                joins.Append(" left join T_CTE_PARTICIPANTE ExpedidorPreCTe on PreCTe.PCO_EXPEDIDOR_CTE = ExpedidorPreCTe.PCT_CODIGO ");
        }

        private void SetarJoinsExpedidorCliente(StringBuilder joins)
        {
            SetarJoinsExpedidor(joins);

            if (!joins.Contains(" ClienteExpedidor "))
                joins.Append(" left join T_CLIENTE ClienteExpedidor on ClienteExpedidor.CLI_CGCCPF = ExpedidorPreCTe.CLI_CODIGO ");
        }

        private void SetarJoinsExpedidorLocalidade(StringBuilder joins)
        {
            SetarJoinsExpedidor(joins);

            if (!joins.Contains(" LocalidadeExpedidor "))
                joins.Append(" left join T_LOCALIDADES LocalidadeExpedidor on ExpedidorPreCTe.LOC_CODIGO = LocalidadeExpedidor.LOC_CODIGO ");
        }

        private void SetarJoinsTomador(StringBuilder joins)
        {
            if (!joins.Contains(" TomadorPagadorPreCTe "))
                joins.Append(" left join T_CTE_PARTICIPANTE TomadorPagadorPreCTe on PreCTe.PCO_TOMADOR = TomadorPagadorPreCTe.PCT_CODIGO ");
        }

        private void SetarJoinsTomadorCliente(StringBuilder joins)
        {
            SetarJoinsTomador(joins);

            if (!joins.Contains(" ClienteTomador "))
                joins.Append(" left join T_CLIENTE ClienteTomador on ClienteTomador.CLI_CGCCPF = TomadorPagadorPreCTe.CLI_CODIGO ");
        }

        private void SetarJoinsTomadorLocalidade(StringBuilder joins)
        {
            SetarJoinsTomador(joins);

            if (!joins.Contains(" LocalidadeTomador "))
                joins.Append(" left join T_LOCALIDADES LocalidadeTomador on TomadorPagadorPreCTe.LOC_CODIGO = LocalidadeTomador.LOC_CODIGO ");
        }

        private void SetarJoinsComplemento(StringBuilder joins)
        {
            if (!joins.Contains(" ComplementoInfo "))
                joins.Append(" left join T_CARGA_CTE_COMPLEMENTO_INFO ComplementoInfo on ComplementoInfo.PCO_CODIGO = PreCTe.PCO_CODIGO ");
        }

        private void SetarJoinsOcorrencia(StringBuilder joins)
        {
            SetarJoinsComplemento(joins);

            if (!joins.Contains(" Ocorrencia "))
                joins.Append(" left join T_CARGA_OCORRENCIA Ocorrencia on Ocorrencia.COC_CODIGO = ComplementoInfo.COC_CODIGO ");
        }

        private void SetarJoinsTipoOcorrencia(StringBuilder joins)
        {
            SetarJoinsOcorrencia(joins);

            if (!joins.Contains(" TipoOcorrencia "))
                joins.Append(" left join T_OCORRENCIA TipoOcorrencia on TipoOcorrencia.OCO_CODIGO = Ocorrencia.OCO_CODIGO ");
        }

        private void SetarJoinsTransportador(StringBuilder joins)
        {
            if (!joins.Contains(" TransportadorPreCTe "))
                joins.Append(" inner join T_EMPRESA TransportadorPreCTe on PreCTe.EMP_CODIGO = TransportadorPreCTe.EMP_CODIGO ");
        }

        private void SetarJoinsTransportadorPai(StringBuilder joins)
        {
            SetarJoinsTransportador(joins);

            if (!joins.Contains(" TransportadorPaiPreCTe "))
                joins.Append(" left join T_EMPRESA TransportadorPaiPreCTe on TransportadorPreCTe.EMP_EMPRESA = TransportadorPaiPreCTe.EMP_CODIGO ");
        }

        private void SetarJoinsTransportadorConfiguracao(StringBuilder joins)
        {
            SetarJoinsTransportador(joins);

            if (!joins.Contains(" TransportadorConfiguracao "))
                joins.Append(" left join T_CONFIG TransportadorConfiguracao ON TransportadorConfiguracao.COF_CODIGO = TransportadorPreCTe.COF_CODIGO");
        }
        private void SetarJoinsTransportadorPaiConfiguracao(StringBuilder joins)
        {
            SetarJoinsTransportadorPai(joins);

            if (!joins.Contains(" TransportadorPaiConfiguracao "))
                joins.Append(" left join T_CONFIG TransportadorPaiConfiguracao ON TransportadorPaiConfiguracao.COF_CODIGO = TransportadorPaiPreCTe.COF_CODIGO");
        }

        private void SetarJoinsTransportadorLocalidade(StringBuilder joins)
        {
            SetarJoinsTransportador(joins);

            if (!joins.Contains(" LocalidadeTransportador "))
                joins.Append(" left join T_LOCALIDADES LocalidadeTransportador on TransportadorPreCTe.LOC_CODIGO = LocalidadeTransportador.LOC_CODIGO ");
        }

        private void SetarJoinsLocalidadeFimPrestacao(StringBuilder joins)
        {
            if (!joins.Contains(" FimPrestacaoPreCTe "))
                joins.Append(" left join T_LOCALIDADES FimPrestacaoPreCTe on PreCTe.PCO_LOCTERMINOPRESTACAO = FimPrestacaoPreCTe.LOC_CODIGO ");
        }

        private void SetarJoinsLocalidadeInicioPrestacao(StringBuilder joins)
        {
            if (!joins.Contains(" InicioPrestacaoPreCTe "))
                joins.Append(" left join T_LOCALIDADES InicioPrestacaoPreCTe on PreCTe.PCO_LOCINICIOPRESTACAO = InicioPrestacaoPreCTe.LOC_CODIGO ");
        }

        private void SetarJoinsCTeVeiculo(StringBuilder joins)
        {
            SetarJoinsCTe(joins);

            if (!joins.Contains(" CteVeiculo "))
                joins.Append(" LEFT JOIN T_CTE_VEICULO CteVeiculo on CteVeiculo.CON_CODIGO = CTe.CON_CODIGO ");
        }

        private void SetarJoinsVeiculo(StringBuilder joins)
        {
            SetarJoinsCTeVeiculo(joins);

            if (!joins.Contains(" Veiculo "))
                joins.Append(" LEFT JOIN T_VEICULO Veiculo on Veiculo.VEI_CODIGO = CteVeiculo.VEI_CODIGO ");
        }

        private void SetarJoinsModeloVeiculo(StringBuilder joins)
        {
            SetarJoinsVeiculo(joins);

            if (!joins.Contains(" VeiculoModelo "))
                joins.Append(" LEFT JOIN T_VEICULO_MODELO VeiculoModelo on VeiculoModelo.VMO_CODIGO = Veiculo.VMO_CODIGO ");
        }


        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.PreCTes.FiltroPesquisaRelatorioPreCTe filtroPesquisa)
        {
            switch (propriedade)
            {
                case "Codigo":
                    if (!select.Contains(" Codigo, "))
                    {
                        select.Append("PreCTe.PCO_CODIGO as Codigo, ");
                        groupBy.Append("PreCTe.PCO_CODIGO, ");
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

                case "NumeroCTe":
                    if (!select.Contains(" NumeroCTe, "))
                    {
                        select.Append(@"(SELECT _cte.CON_NUM FROM T_CARGA_CTE _cargaCte 
                                        JOIN T_CTE _cte on _cte.CON_CODIGO = _cargaCte.CON_CODIGO WHERE _cargaCte.PCO_CODIGO = PreCTe.PCO_CODIGO) NumeroCTe, ");

                        groupBy.Append("PreCTe.PCO_CODIGO, ");

                        SetarJoinsTipoOperacao(joins);
                    }
                    break;


                case "TipoDeCarga":
                    if (!select.Contains("TipoDeCarga"))
                    {
                        select.Append("substring((select distinct ', ' + TipoDeCarga.TCG_DESCRICAO from T_CARGA_CTE CargaCTe inner join T_CARGA Carga on Carga.CAR_CODIGO = CargaCTe.CAR_CODIGO inner join T_TIPO_DE_CARGA TipoDeCarga on TipoDeCarga.TCG_CODIGO = Carga.TCG_CODIGO where CargaCTe.PCO_CODIGO = PreCTe.PCO_CODIGO for xml path('')), 3, 200) TipoDeCarga, ");

                        if (!groupBy.Contains("PreCTe.PCO_CODIGO"))
                            groupBy.Append("PreCTe.PCO_CODIGO, ");
                    }
                    break;

                case "PesoLiquidoKg":
                    if (!select.Contains("PesoLiquidoKg"))
                    {
                        select.Append("(select sum(_cargaPedido.PED_PESO_LIQUIDO) from T_CARGA_CTE _cargaCte left join T_CARGA_PEDIDO _cargaPedido on _cargaPedido.CAR_CODIGO = _cargaCte.CAR_CODIGO WHERE PreCTe.PCO_CODIGO = _cargaCte.PCO_CODIGO) PesoLiquidoKg, ");

                        if (!groupBy.Contains("PreCTe.PCO_CODIGO"))
                            groupBy.Append("PreCTe.PCO_CODIGO, ");
                    }
                    break;


                case "PesoKg":
                    if (!select.Contains("PesoKg"))
                    {
                        select.Append("(select sum(_cargaPedido.PED_PESO) from T_CARGA_CTE _cargaCte left join T_CARGA_PEDIDO _cargaPedido on _cargaPedido.CAR_CODIGO = _cargaCte.CAR_CODIGO WHERE PreCTe.PCO_CODIGO = _cargaCte.PCO_CODIGO) PesoKg, ");

                        if (!groupBy.Contains("PreCTe.PCO_CODIGO"))
                            groupBy.Append("PreCTe.PCO_CODIGO, ");
                    }
                    break;

                case "ModeloVeicular":
                    if (!select.Contains("ModeloVeicular"))
                    {
                        select.Append("substring((select distinct ', ' + Modelo.MVC_DESCRICAO from T_CARGA_CTE CargaCTe inner join T_CARGA Carga on Carga.CAR_CODIGO = CargaCTe.CAR_CODIGO_ORIGEM join T_MODELO_VEICULAR_CARGA Modelo on Modelo.MVC_CODIGO = Carga.MVC_CODIGO where CargaCTe.PCO_CODIGO = PreCTe.PCO_CODIGO for xml path('')), 3, 200) ModeloVeicular, ");

                        if (!groupBy.Contains("PreCTe.PCO_CODIGO"))
                            groupBy.Append("PreCTe.PCO_CODIGO, ");
                    }
                    break;

                case "ModeloVeiculoCarga":
                    if (!select.Contains(" ModeloVeiculoCarga "))
                    {
                        select.Append(@"substring((SELECT ', ' + mov.MVC_DESCRICAO FROM T_CARGA_CTE cargaCte  left join T_CTE cte on cte.CON_CODIGO = cargaCte.CON_CODIGO 
                                        left join T_CTE_VEICULO ctevei on ctevei.CON_CODIGO = cte.CON_CODIGO 
                                        LEFT JOIN T_VEICULO veiculo on ctevei.VEI_CODIGO = veiculo.VEI_CODIGO
                                        left join T_MODELO_VEICULAR_CARGA mov on mov.MVC_CODIGO = veiculo.MVC_CODIGO
                                        where ctevei.CON_CODIGO = cte.CON_CODIGO and cargaCte.PCO_CODIGO = PreCTe.PCO_CODIGO for xml path('')),3, 1000) ModeloVeiculoCarga, ");

                        if (!groupBy.Contains("PreCTe.PCO_CODIGO"))
                            groupBy.Append("PreCTe.PCO_CODIGO, ");


                    }
                    break;

                case "DataEmissaoFormatada":
                case "DataEmissao":
                    if (!select.Contains(" DataEmissao, "))
                    {
                        select.Append("PreCTe.PCO_DATAHORAEMISSAO DataEmissao, ");
                        groupBy.Append("PreCTe.PCO_DATAHORAEMISSAO, ");
                    }
                    break;

                case "CNPJFilial":
                    if (!select.Contains(" CNPJFilial, "))
                    {
                        select.Append(@"
                            SUBSTRING((
                                select distinct ', ' + _filial.FIL_CNPJ
                                from T_CARGA_CTE _cargaCTe 
                                inner join T_CARGA _carga ON _carga.CAR_CODIGO = _cargaCTe.CAR_CODIGO_ORIGEM 
                                inner join T_FILIAL _filial on _carga.FIL_CODIGO = _filial.FIL_CODIGO
                                where _cargaCTe.PCO_CODIGO = PreCTe.PCO_CODIGO for xml path('')), 3, 1000) CNPJFilial, ");

                        groupBy.Append("PreCTe.PCO_CODIGO, ");
                    }
                    break;

                case "FilialDescricao":
                    if (!select.Contains(" FilialDescricao, "))
                    {
                        select.Append(@"
                            SUBSTRING((
                                select distinct ', ' + _filial.FIL_DESCRICAO
                                from T_CARGA_CTE _cargaCTe 
                                inner join T_CARGA _carga ON _carga.CAR_CODIGO = _cargaCTe.CAR_CODIGO_ORIGEM 
                                inner join T_FILIAL _filial on _carga.FIL_CODIGO = _filial.FIL_CODIGO
                                where _cargaCTe.PCO_CODIGO = PreCTe.PCO_CODIGO for xml path('')), 3, 1000) FilialDescricao, ");

                        groupBy.Append("PreCTe.PCO_CODIGO, ");
                    }
                    break;

                case "DescricaoTipoTomador":
                case "TipoTomador":
                    if (!select.Contains(" TipoTomador, "))
                    {
                        select.Append("PreCTe.PCO_TOMADOR TipoTomador,");
                        groupBy.Append("PreCTe.PCO_TOMADOR, ");
                    }
                    break;

                case "DescricaoTipoPagamento":
                    if (!select.Contains(" TipoPagamento, "))
                    {
                        select.Append("PreCTe.PCO_PAGOAPAGAR as TipoPagamento, ");

                        if (!groupBy.Contains("PreCTe.PCO_PAGOAPAGAR"))
                            groupBy.Append("PreCTe.PCO_PAGOAPAGAR, ");
                    }
                    break;


                case "NumeroCarga":
                    if (!select.Contains(" NumeroCarga, "))
                    {
                        select.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR NumeroCarga, ");
                        groupBy.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR, ");

                        SetarJoinsCarga(joins);
                    }
                    break;

                case "ValorPrestacaoServico":
                    if (!select.Contains(" ValorPrestacaoServico, "))
                    {
                        select.Append("PreCTe.PCO_VALOR_PREST_SERVICO ValorPrestacaoServico, ");
                        groupBy.Append("PreCTe.PCO_VALOR_PREST_SERVICO, ");
                    }
                    break;

                case "TipoOcorrencia":
                    if (!select.Contains(" TipoOcorrencia, "))
                    {
                        select.Append("TipoOcorrencia.OCO_DESCRICAO TipoOcorrencia, ");
                        groupBy.Append("TipoOcorrencia.OCO_DESCRICAO, ");

                        SetarJoinsTipoOcorrencia(joins);

                    }
                    break;

                case "Ocorrencia":
                    if (!select.Contains(" Ocorrencia, "))
                    {
                        select.Append("CONVERT(varchar(100), Ocorrencia.COC_NUMERO_CONTRATO) Ocorrencia, ");
                        groupBy.Append("Ocorrencia.COC_NUMERO_CONTRATO, ");

                        SetarJoinsOcorrencia(joins);
                    }
                    break;

                case "ValorFrete":
                    if (!select.Contains("ValorFrete"))
                    {
                        select.Append("PreCTe.PCO_VALOR_FRETE ValorFrete, ");

                        if (!groupBy.Contains("PreCTe.PCO_VALOR_FRETE"))
                            groupBy.Append("PreCTe.PCO_VALOR_FRETE, ");
                    }
                    break;

                case "CSTFormatada":
                    if (!select.Contains("CST"))
                    {
                        select.Append("PreCTe.PCO_CST CST, ");
                        groupBy.Append("PreCTe.PCO_CST, ");
                    }
                    break;

                case "BaseCalculoICMS":
                    if (!select.Contains("BaseCalculoICMS"))
                        select.Append("SUM(PreCTe.PCO_BC_ICMS) BaseCalculoICMS, ");
                    break;

                case "AliquotaICMS":
                    if (!select.Contains("AliquotaICMS"))
                    {
                        select.Append("PreCTe.PCO_ALIQ_ICMS AliquotaICMS, ");

                        if (!groupBy.Contains("PreCTe.PCO_ALIQ_ICMS"))
                            groupBy.Append("PreCTe.PCO_ALIQ_ICMS, ");
                    }
                    break;

                case "ValorICMS":
                    if (!select.Contains("ValorICMS"))
                        select.Append("SUM(PreCTe.PCO_VAL_ICMS) ValorICMS, ");
                    break;


                case "AliquotaISS":
                    if (!select.Contains("AliquotaISS"))
                    {
                        select.Append("PreCTe.PCO_ALIQUOTA_ISS AliquotaISS, ");

                        if (!groupBy.Contains("PreCTe.PCO_ALIQUOTA_ISS"))
                            groupBy.Append("PreCTe.PCO_ALIQUOTA_ISS, ");
                    }
                    break;

                case "ValorISS":
                    if (!select.Contains("ValorISS"))
                        select.Append("SUM(PreCTe.PCO_VALOR_ISS) ValorISS, ");
                    break;

                case "AliquotaPIS":
                    if (!select.Contains(" AliquotaPIS,"))
                    {
                        select.Append("coalesce(TransportadorConfiguracao.COF_ALIQUOTA_PIS, TransportadorPaiConfiguracao.COF_ALIQUOTA_PIS, 0) AliquotaPIS, ");

                        groupBy.Append("TransportadorConfiguracao.COF_ALIQUOTA_PIS, ");
                        groupBy.Append("TransportadorPaiConfiguracao.COF_ALIQUOTA_PIS, ");

                        SetarJoinsTransportadorConfiguracao(joins);
                        SetarJoinsTransportadorPaiConfiguracao(joins);
                    }
                    break;

                case "AliquotaCOFINS":
                    if (!select.Contains(" AliquotaCOFINS,"))
                    {
                        select.Append("coalesce(TransportadorConfiguracao.COF_ALIQUOTA_COFINS, TransportadorPaiConfiguracao.COF_ALIQUOTA_COFINS, 0) AliquotaCOFINS, ");

                        groupBy.Append("TransportadorConfiguracao.COF_ALIQUOTA_COFINS, ");
                        groupBy.Append("TransportadorPaiConfiguracao.COF_ALIQUOTA_COFINS, ");

                        SetarJoinsTransportadorConfiguracao(joins);
                        SetarJoinsTransportadorPaiConfiguracao(joins);
                    }
                    break;


                case "ValorCOFINS":
                    SetarSelect("AliquotaCOFINS", 0, select, joins, groupBy, somenteContarNumeroRegistros, filtroPesquisa);
                    SetarSelect("BaseCalculoICMS", 0, select, joins, groupBy, somenteContarNumeroRegistros, filtroPesquisa);
                    break;

                case "ValorPIS":
                    SetarSelect("AliquotaPIS", 0, select, joins, groupBy, somenteContarNumeroRegistros, filtroPesquisa);
                    SetarSelect("BaseCalculoICMS", 0, select, joins, groupBy, somenteContarNumeroRegistros, filtroPesquisa);
                    break;



                case "ValorMercadoria":
                    if (!select.Contains("ValorMercadoria"))
                        select.Append("SUM(PreCTe.PCO_VALOR_TOTAL_MERC) ValorMercadoria, ");
                    break;

                case "TabelaFrete":
                    if (!select.Contains(" TabelaFrete, "))
                    {
                        select.Append(
                            @"SUBSTRING((
                                SELECT DISTINCT ', ' + TabelaFrete.TBF_DESCRICAO
                                        from T_TABELA_FRETE TabelaFrete
                                        inner join T_CARGA Carga on Carga.TBF_CODIGO = TabelaFrete.TBF_CODIGO
                                        inner join T_CARGA_CTE CargaCTe on CargaCTe.CAR_CODIGO = Carga.CAR_CODIGO 
                                 WHERE CargaCTe.PCO_CODIGO = PreCTe.PCO_CODIGO for xml path('')), 3, 1000) TabelaFrete, "
                        );

                        if (!groupBy.Contains("PreCTe.PCO_CODIGO"))
                            groupBy.Append(" PreCTe.PCO_CODIGO, ");
                    }
                    break;


                case "CodigoTabelaFreteCliente":
                    if (!select.Contains(" CodigoTabelaFreteCliente, "))
                    {
                        select.Append(
                                @"SUBSTRING((
                                SELECT DISTINCT ', ' + 
                                            (CASE WHEN TabelaFrete.TBF_TIPO_CALCULO = 0 THEN (SUBSTRING((SELECT DISTINCT ', ' + 
		                                            TFC_CODIGO_INTEGRACAO 
			                                        FROM T_TABELA_FRETE_CLIENTE TabelaFreteCliente
			                                        JOIN T_CARGA_TABELA_FRETE_CLIENTE CargaTabelaFreteCliente on CargaTabelaFreteCliente.TFC_CODIGO = TabelaFreteCliente.TFC_CODIGO
			                                        WHERE CargaTabelaFreteCliente.CAR_CODIGO = Carga.CAR_CODIGO FOR XML PATH('')), 3, 3000)) --PorCarga
                                              --WHEN TabelaFrete.TBF_TIPO_CALCULO = 1 THEN () --PorPedido
                                              ELSE '' END)
                                        from T_TABELA_FRETE TabelaFrete
                                        inner join T_CARGA Carga on Carga.TBF_CODIGO = TabelaFrete.TBF_CODIGO
                                        inner join T_CARGA_CTE CargaCTe on CargaCTe.CAR_CODIGO = Carga.CAR_CODIGO 
                                 WHERE CargaCTe.PCO_CODIGO = PreCTe.PCO_CODIGO for xml path('')), 3, 5000) CodigoTabelaFreteCliente, "
                            );

                        if (!groupBy.Contains("PreCTe.PCO_CODIGO"))
                            groupBy.Append(" PreCTe.PCO_CODIGO, ");
                    }
                    break;

                case "TabelaFreteCliente":
                    if (!select.Contains(" TabelaFreteCliente, "))
                    {
                        select.Append(
                                @"SUBSTRING((S
                                SELECT DISTINCT ', ' + 
                                            (CASE WHEN TabelaFrete.TBF_TIPO_CALCULO = 0 THEN (SUBSTRING((SELECT DISTINCT ', ' + 
		                                            CAST((TFC_CODIGO_INTEGRACAO + ' - ' + TFC_DESCRICAO_ORIGEM + ' até ' + TFC_DESCRICAO_DESTINO) AS NVARCHAR(500))
			                                        FROM T_TABELA_FRETE_CLIENTE TabelaFreteCliente
			                                        JOIN T_CARGA_TABELA_FRETE_CLIENTE CargaTabelaFreteCliente on CargaTabelaFreteCliente.TFC_CODIGO = TabelaFreteCliente.TFC_CODIGO
			                                        WHERE CargaTabelaFreteCliente.CAR_CODIGO = Carga.CAR_CODIGO FOR XML PATH('')), 3, 3000)) --PorCarga
                                              --WHEN TabelaFrete.TBF_TIPO_CALCULO = 1 THEN () --PorPedido
                                              ELSE '' END)
                                        from T_TABELA_FRETE TabelaFrete
                                        inner join T_CARGA Carga on Carga.TBF_CODIGO = TabelaFrete.TBF_CODIGO
                                        inner join T_CARGA_CTE CargaCTe on CargaCTe.CAR_CODIGO = Carga.CAR_CODIGO 
                                 WHERE CargaCTe.PCO_CODIGO = PreCTe.PCO_CODIGO for xml path('')), 3, 5000) TabelaFreteCliente, "
                            );

                        if (!groupBy.Contains("PreCTe.PCO_CODIGO"))
                            groupBy.Append(" PreCTe.PCO_CODIGO, ");
                    }
                    break;


                case "ProdutoPredominante":
                    if (!select.Contains(" ProdutoPredominante, "))
                    {
                        select.Append("PreCTe.PCO_PRODUTO_PRED ProdutoPredominante, ");
                        groupBy.Append("PreCTe.PCO_PRODUTO_PRED, ");
                    }
                    break;

                case "NomeFantasiaTransportador":
                    if (!select.Contains("TransportadorPreCTe.EMP_FANTASIA"))
                    {
                        select.Append("TransportadorPreCTe.EMP_FANTASIA NomeFantasiaTransportador, ");
                        groupBy.Append("TransportadorPreCTe.EMP_FANTASIA, ");

                        SetarJoinsTransportador(joins);
                    }
                    break;

                case "RazaoSocialTransportador":
                    if (!select.Contains("TransportadorPreCTe.EMP_RAZAO"))
                    {
                        select.Append("TransportadorPreCTe.EMP_RAZAO RazaoSocialTransportador, ");
                        groupBy.Append("TransportadorPreCTe.EMP_RAZAO, ");

                        SetarJoinsTransportador(joins);
                    }
                    break;

                case "CNPJTransportadorFormatado":
                    if (!select.Contains("TransportadorPreCTe.EMP_CNPJ"))
                    {
                        select.Append("TransportadorPreCTe.EMP_CNPJ CNPJTransportador, ");
                        groupBy.Append("TransportadorPreCTe.EMP_CNPJ, ");

                        SetarJoinsTransportador(joins);
                    }
                    break;

                case "UFTransportador":
                    if (!select.Contains(" UFTransportador, "))
                    {
                        select.Append("LocalidadeTransportador.UF_SIGLA UFTransportador, ");

                        if (!groupBy.Contains("LocalidadeTransportador.UF_SIGLA"))
                            groupBy.Append("LocalidadeTransportador.UF_SIGLA, ");

                        SetarJoinsTransportadorLocalidade(joins);
                    }
                    break;

                case "Motorista":
                    if (!select.Contains("Motorista"))
                    {
                        select.Append(@"substring((select ', ' + funcionario.FUN_NOME from T_CARGA_CTE cargaCte 
                                        left join T_CARGA_MOTORISTA cargaMotorista on cargaMotorista.CAR_CODIGO = cargaCte.CAR_CODIGO 
                                        left join T_FUNCIONARIO funcionario on funcionario.FUN_CODIGO = cargaMotorista.CAR_MOTORISTA 
                                        where cargaCTe.PCO_CODIGO = PreCTe.PCO_CODIGO for xml path('')), 3, 1000) Motorista, ");

                        if (!groupBy.Contains("PreCTe.PCO_CODIGO"))
                            groupBy.Append("PreCTe.PCO_CODIGO, ");
                    }
                    break;

                case "CPFMotorista":
                    if (!select.Contains("CPFMotorista"))
                    {
                        select.Append(@"substring((select ', ' + funcionario.FUN_CPF from T_CARGA_CTE cargaCte 
                                        left join T_CARGA_MOTORISTA cargaMotorista on cargaMotorista.CAR_CODIGO = cargaCte.CAR_CODIGO 
                                        left join T_FUNCIONARIO funcionario on funcionario.FUN_CODIGO = cargaMotorista.CAR_MOTORISTA 
                                        where cargaCTe.PCO_CODIGO = PreCTe.PCO_CODIGO for xml path('')), 3, 1000) CPFMotorista, ");

                        if (!groupBy.Contains("PreCTe.PCO_CODIGO"))
                            groupBy.Append("PreCTe.PCO_CODIGO, ");
                    }
                    break;

                case "NumeroNotaFiscal":
                    if (!select.Contains("NumeroNotaFiscal"))
                    {
                        select.Append("substring((select DISTINCT ', ' + notaFiscal1.PNF_NUMERO from T_PRE_CTE_DOCS notaFiscal1 join T_CARGA_CTE _cargaCte on _cargaCte.PCO_CODIGO = notaFiscal1.PCO_CODIGO LEFT OUTER JOIN T_CANHOTO_NOTA_FISCAL _canhoto ON _canhoto.CAR_CODIGO = _cargaCte.CAR_CODIGO where _cargaCte.PCO_CODIGO = PreCTe.PCO_CODIGO for xml path('')), 3, 100000) NumeroNotaFiscal, ");

                        if (!groupBy.Contains("PreCTe.PCO_CODIGO"))
                            groupBy.Append("PreCTe.PCO_CODIGO, ");
                    }
                    break;

                case "ChaveNotaFiscal":
                    if (!select.Contains("ChaveNotaFiscal"))
                    {
                        select.Append("substring((select ', ' + notaFiscal2.PNF_CHAVENFE from T_PRE_CTE_DOCS notaFiscal2 where notaFiscal2.PCO_CODIGO = PreCTe.PCO_CODIGO for xml path('')), 3, 15000) ChaveNotaFiscal, ");

                        if (!groupBy.Contains("PreCTe.PCO_CODIGO"))
                            groupBy.Append("PreCTe.PCO_CODIGO, ");
                    }
                    break;

                case "CFOP":
                    if (!select.Contains(" CFOP"))
                    {
                        select.Append("CFOP.CFO_CFOP CFOP, ");
                        groupBy.Append("CFOP.CFO_CFOP, ");

                        SetarJoinsCfop(joins);
                    }
                    break;

                case "LocalidadeRemetente":
                    if (!select.Contains(" LocalidadeRemetente, "))
                    {
                        select.Append("LocalidadeRemetente.LOC_DESCRICAO + '-' + LocalidadeRemetente.UF_SIGLA LocalidadeRemetente, ");

                        if (!groupBy.Contains("LocalidadeRemetente.LOC_DESCRICAO"))
                            groupBy.Append("LocalidadeRemetente.LOC_DESCRICAO, ");

                        if (!groupBy.Contains("LocalidadeRemetente.UF_SIGLA"))
                            groupBy.Append("LocalidadeRemetente.UF_SIGLA, ");

                        SetarJoinsRemetenteLocalidade(joins);
                    }
                    break;

                case "CodigoRemetente":
                    if (!select.Contains(" CodigoRemetente, "))
                    {
                        select.Append("ClienteRemetente.CLI_CODIGO_INTEGRACAO CodigoRemetente, ");

                        if (!groupBy.Contains("ClienteRemetente.CLI_CODIGO_INTEGRACAO"))
                            groupBy.Append("ClienteRemetente.CLI_CODIGO_INTEGRACAO, ");

                        SetarJoinsRemetenteCliente(joins);
                    }
                    break;

                case "CPFCNPJRemetente":
                    if (!select.Contains(" CPFCNPJRemetente, "))
                    {
                        select.Append("RemetentePreCTe.PCT_CPF_CNPJ CPFCNPJRemetente, ");

                        if (!groupBy.Contains("RemetentePreCTe.PCT_CPF_CNPJ"))
                            groupBy.Append("RemetentePreCTe.PCT_CPF_CNPJ, ");

                        SetarJoinsRemetente(joins);
                    }
                    break;

                case "IERemetente":
                    if (!select.Contains(" IERemetente, "))
                    {
                        select.Append("RemetentePreCTe.PCT_IERG IERemetente, ");
                        groupBy.Append("RemetentePreCTe.PCT_IERG, ");

                        SetarJoinsRemetente(joins);
                    }
                    break;

                case "Remetente":
                    if (!select.Contains(" Remetente, "))
                    {
                        select.Append("RemetentePreCTe.PCT_NOME Remetente, ");

                        if (!groupBy.Contains("RemetentePreCTe.PCT_NOME"))
                            groupBy.Append("RemetentePreCTe.PCT_NOME, ");

                        SetarJoinsRemetente(joins);
                    }
                    break;

                case "CodigoDocumentoRemetente":
                    if (!select.Contains(" CodigoDocumentoRemetente, "))
                    {
                        select.Append("ClienteRemetente.CLI_CODIGO_DOCUMENTO CodigoDocumentoRemetente, ");

                        if (!groupBy.Contains("ClienteRemetente.CLI_CODIGO_DOCUMENTO"))
                            groupBy.Append("ClienteRemetente.CLI_CODIGO_DOCUMENTO, ");

                        SetarJoinsRemetenteCliente(joins);
                    }
                    break;

                case "CodigoRecebedor":
                    if (!select.Contains(" CodigoRecebedor, "))
                    {
                        select.Append("ClienteRecebedor.CLI_CODIGO_INTEGRACAO CodigoRecebedor, ");

                        if (!groupBy.Contains("ClienteRecebedor.CLI_CODIGO_INTEGRACAO"))
                            groupBy.Append("ClienteRecebedor.CLI_CODIGO_INTEGRACAO, ");

                        SetarJoinsRecebedorCliente(joins);
                    }
                    break;

                case "CPFCNPJRecebedor":
                    if (!select.Contains(" CPFCNPJRecebedor, "))
                    {
                        select.Append("RecebedorPreCTe.PCT_CPF_CNPJ CPFCNPJRecebedor, ");

                        if (!groupBy.Contains("RecebedorPreCTe.PCT_CPF_CNPJ"))
                            groupBy.Append("RecebedorPreCTe.PCT_CPF_CNPJ, ");

                        SetarJoinsRecebedor(joins);
                    }
                    break;

                case "IERecebedor":
                    if (!select.Contains(" IERecebedor, "))
                    {
                        select.Append("RecebedorPreCTe.PCT_IERG IERecebedor, ");
                        groupBy.Append("RecebedorPreCTe.PCT_IERG, ");

                        SetarJoinsRecebedor(joins);
                    }
                    break;

                case "Recebedor":
                    if (!select.Contains(" Recebedor, "))
                    {
                        select.Append("RecebedorPreCTe.PCT_NOME Recebedor, ");

                        if (!groupBy.Contains("RecebedorPreCTe.PCT_NOME"))
                            groupBy.Append("RecebedorPreCTe.PCT_NOME, ");

                        SetarJoinsRecebedor(joins);
                    }
                    break;

                case "UFRecebedor":
                    if (!select.Contains(" UFRecebedor, "))
                    {
                        select.Append("LocalidadeRecebedor.UF_SIGLA UFRecebedor, ");

                        if (!groupBy.Contains("LocalidadeRecebedor.UF_SIGLA"))
                            groupBy.Append("LocalidadeRecebedor.UF_SIGLA, ");

                        SetarJoinsRecebedorLocalidade(joins);
                    }
                    break;

                case "CodigoDocumentoRecebedor":
                    if (!select.Contains(" CodigoDocumentoRecebedor, "))
                    {
                        select.Append("ClienteRecebedor.CLI_CODIGO_DOCUMENTO CodigoDocumentoRecebedor, ");

                        if (!groupBy.Contains("ClienteRecebedor.CLI_CODIGO_DOCUMENTO"))
                            groupBy.Append("ClienteRecebedor.CLI_CODIGO_DOCUMENTO, ");

                        SetarJoinsRecebedorCliente(joins);
                    }
                    break;

                case "LocalidadeRecebedor":
                    if (!select.Contains(" LocalidadeRecebedor, "))
                    {
                        select.Append("LocalidadeRecebedor.LOC_DESCRICAO + '-' + LocalidadeRecebedor.UF_SIGLA LocalidadeRecebedor, ");

                        if (!groupBy.Contains("LocalidadeRecebedor.LOC_DESCRICAO"))
                            groupBy.Append("LocalidadeRecebedor.LOC_DESCRICAO, ");

                        if (!groupBy.Contains("LocalidadeRecebedor.UF_SIGLA"))
                            groupBy.Append("LocalidadeRecebedor.UF_SIGLA, ");

                        SetarJoinsRecebedorLocalidade(joins);
                    }
                    break;

                case "CodigoDestinatario":
                    if (!select.Contains(" CodigoDestinatario, "))
                    {
                        select.Append("ClienteDestinatario.CLI_CODIGO_INTEGRACAO CodigoDestinatario, ");

                        if (!groupBy.Contains("ClienteDestinatario.CLI_CODIGO_INTEGRACAO"))
                            groupBy.Append("ClienteDestinatario.CLI_CODIGO_INTEGRACAO, ");

                        SetarJoinsDestinatarioCliente(joins);
                    }
                    break;

                case "CPFCNPJDestinatario":
                    if (!select.Contains(" CPFCNPJDestinatario, "))
                    {
                        select.Append("DestinatarioPreCTe.PCT_CPF_CNPJ CPFCNPJDestinatario, ");

                        if (!groupBy.Contains("DestinatarioPreCTe.PCT_CPF_CNPJ"))
                            groupBy.Append("DestinatarioPreCTe.PCT_CPF_CNPJ, ");

                        SetarJoinsDestinatario(joins);
                    }
                    break;

                case "IEDestinatario":
                    if (!select.Contains(" IEDestinatario, "))
                    {
                        select.Append("DestinatarioPreCTe.PCT_IERG IEDestinatario, ");
                        groupBy.Append("DestinatarioPreCTe.PCT_IERG, ");

                        SetarJoinsDestinatario(joins);
                    }
                    break;

                case "Destinatario":
                    if (!select.Contains(" Destinatario, "))
                    {
                        select.Append("DestinatarioPreCTe.PCT_NOME Destinatario, ");

                        if (!groupBy.Contains("DestinatarioPreCTe.PCT_NOME"))
                            groupBy.Append("DestinatarioPreCTe.PCT_NOME, ");

                        SetarJoinsDestinatario(joins);
                    }
                    break;


                case "LocalidadeDestinatario":
                    if (!select.Contains(" LocalidadeDestinatario, "))
                    {
                        select.Append("LocalidadeDestinatario.LOC_DESCRICAO + '-' + LocalidadeDestinatario.UF_SIGLA LocalidadeDestinatario, ");

                        if (!groupBy.Contains("LocalidadeDestinatario.LOC_DESCRICAO"))
                            groupBy.Append("LocalidadeDestinatario.LOC_DESCRICAO, ");

                        if (!groupBy.Contains("LocalidadeDestinatario.UF_SIGLA"))
                            groupBy.Append("LocalidadeDestinatario.UF_SIGLA, ");

                        SetarJoinsDestinatarioLocalidade(joins);
                    }
                    break;

                case "UFDestinatario":
                    if (!select.Contains(" UFDestinatario, "))
                    {
                        select.Append("LocalidadeDestinatario.UF_SIGLA UFDestinatario, ");

                        if (!groupBy.Contains("LocalidadeDestinatario.UF_SIGLA"))
                            groupBy.Append("LocalidadeDestinatario.UF_SIGLA, ");

                        SetarJoinsDestinatarioLocalidade(joins);
                    }
                    break;

                case "CodigoExpedidor":
                    if (!select.Contains(" CodigoExpedidor, "))
                    {
                        select.Append("ClienteExpedidor.CLI_CODIGO_INTEGRACAO CodigoExpedidor, ");

                        if (!groupBy.Contains("ClienteExpedidor.CLI_CODIGO_INTEGRACAO"))
                            groupBy.Append("ClienteExpedidor.CLI_CODIGO_INTEGRACAO, ");

                        SetarJoinsExpedidorCliente(joins);
                    }
                    break;

                case "CPFCNPJExpedidor":
                    if (!select.Contains(" CPFCNPJExpedidor, "))
                    {
                        select.Append("ExpedidorPreCTe.PCT_CPF_CNPJ CPFCNPJExpedidor, ");

                        if (!groupBy.Contains("ExpedidorPreCTe.PCT_CPF_CNPJ"))
                            groupBy.Append("ExpedidorPreCTe.PCT_CPF_CNPJ, ");

                        SetarJoinsExpedidor(joins);
                    }
                    break;

                case "IEExpedidor":
                    if (!select.Contains(" IEExpedidor, "))
                    {
                        select.Append("ExpedidorPreCTe.PCT_IERG IEExpedidor, ");
                        groupBy.Append("ExpedidorPreCTe.PCT_IERG, ");

                        SetarJoinsExpedidor(joins);
                    }
                    break;

                case "Expedidor":
                    if (!select.Contains(" Expedidor, "))
                    {
                        select.Append("ExpedidorPreCTe.PCT_NOME Expedidor, ");

                        if (!groupBy.Contains("ExpedidorPreCTe.PCT_NOME"))
                            groupBy.Append("ExpedidorPreCTe.PCT_NOME, ");

                        SetarJoinsExpedidor(joins);
                    }
                    break;

                case "LocalidadeExpedidor":
                    if (!select.Contains(" LocalidadeExpedidor, "))
                    {
                        select.Append("LocalidadeExpedidor.LOC_DESCRICAO + '-' + LocalidadeExpedidor.UF_SIGLA LocalidadeExpedidor, ");

                        if (!groupBy.Contains("LocalidadeExpedidor.LOC_DESCRICAO"))
                            groupBy.Append("LocalidadeExpedidor.LOC_DESCRICAO, ");

                        if (!groupBy.Contains("LocalidadeExpedidor.UF_SIGLA"))
                            groupBy.Append("LocalidadeExpedidor.UF_SIGLA, ");

                        SetarJoinsExpedidorLocalidade(joins);
                    }
                    break;

                case "UFExpedidor":
                    if (!select.Contains(" UFExpedidor, "))
                    {
                        select.Append("LocalidadeExpedidor.UF_SIGLA UFExpedidor, ");

                        if (!groupBy.Contains("LocalidadeExpedidor.UF_SIGLA"))
                            groupBy.Append("LocalidadeExpedidor.UF_SIGLA, ");

                        SetarJoinsExpedidorLocalidade(joins);
                    }
                    break;

                case "CodigoDocumentoExpedidor":
                    if (!select.Contains(" CodigoDocumentoExpedidor, "))
                    {
                        select.Append("ClienteExpedidor.CLI_CODIGO_DOCUMENTO CodigoDocumentoExpedidor, ");

                        if (!groupBy.Contains("ClienteExpedidor.CLI_CODIGO_DOCUMENTO"))
                            groupBy.Append("ClienteExpedidor.CLI_CODIGO_DOCUMENTO, ");

                        SetarJoinsExpedidorCliente(joins);
                    }
                    break;

                case "CPFCNPJTomador":
                    if (!select.Contains(" CPFCNPJTomador, "))
                    {
                        select.Append("TomadorPagadorPreCTe.PCT_CPF_CNPJ CPFCNPJTomador, ");

                        if (!groupBy.Contains("TomadorPagadorPreCTe.PCT_CPF_CNPJ"))
                            groupBy.Append("TomadorPagadorPreCTe.PCT_CPF_CNPJ, ");

                        SetarJoinsTomador(joins);
                    }
                    break;

                case "CodigoTomador":
                    if (!select.Contains(" CodigoTomador, "))
                    {
                        select.Append("TomadorPagadorPreCTe.PCT_CODIGO_INTEGRACAO CodigoTomador, ");

                        if (!groupBy.Contains("TomadorPagadorPreCTe.PCT_CODIGO_INTEGRACAO"))
                            groupBy.Append("TomadorPagadorPreCTe.PCT_CODIGO_INTEGRACAO, ");

                        SetarJoinsTomador(joins);
                    }
                    break;

                case "IETomador":
                    if (!select.Contains(" IETomador, "))
                    {
                        select.Append("TomadorPagadorPreCTe.PCT_IERG IETomador, ");
                        groupBy.Append("TomadorPagadorPreCTe.PCT_IERG, ");

                        SetarJoinsTomador(joins);
                    }
                    break;

                case "Tomador":
                    if (!select.Contains(" Tomador, "))
                    {
                        select.Append("TomadorPagadorPreCTe.PCT_NOME Tomador, ");

                        if (!groupBy.Contains("PreCTe.PCO_TOMADOR"))
                            groupBy.Append("PreCTe.PCO_TOMADOR, ");

                        if (!groupBy.Contains("TomadorPagadorPreCTe.PCT_NOME"))
                            groupBy.Append("TomadorPagadorPreCTe.PCT_NOME, ");

                        SetarJoinsTomador(joins);
                    }
                    break;

                case "UFTomador":
                    if (!select.Contains(" UFTomador, "))
                    {
                        select.Append("LocalidadeTomador.UF_SIGLA UFTomador, ");

                        if (!groupBy.Contains("LocalidadeTomador.UF_SIGLA"))
                            groupBy.Append("LocalidadeTomador.UF_SIGLA, ");

                        SetarJoinsTomadorLocalidade(joins);
                    }
                    break;

                case "CodigoDocumentoTomador":
                    if (!select.Contains(" CodigoDocumentoTomador, "))
                    {
                        select.Append("ClienteTomador.CLI_CODIGO_DOCUMENTO CodigoDocumentoTomador, ");

                        if (!groupBy.Contains("ClienteTomador.CLI_CODIGO_DOCUMENTO"))
                            groupBy.Append("ClienteTomador.CLI_CODIGO_DOCUMENTO, ");

                        SetarJoinsTomadorCliente(joins);
                    }
                    break;

                case "IBGEFimPrestacao":
                    if (!select.Contains(" IBGEFimPrestacao,"))
                    {
                        select.Append(" FimPrestacaoPreCTe.LOC_IBGE IBGEFimPrestacao, ");
                        groupBy.Append("FimPrestacaoPreCTe.LOC_IBGE, ");

                        SetarJoinsLocalidadeFimPrestacao(joins);
                    }
                    break;

                case "FimPrestacao":
                    if (!select.Contains(" FimPrestacao,"))
                    {
                        select.Append(" FimPrestacaoPreCTe.LOC_DESCRICAO + '-' + FimPrestacaoPreCTe.UF_SIGLA FimPrestacao, ");
                        groupBy.Append("FimPrestacaoPreCTe.LOC_DESCRICAO, ");

                        if (!groupBy.Contains("FimPrestacaoPreCTe.UF_SIGLA"))
                            groupBy.Append("FimPrestacaoPreCTe.UF_SIGLA, ");

                        SetarJoinsLocalidadeFimPrestacao(joins);
                    }
                    break;

                case "UFFimPrestacao":
                    if (!select.Contains("UFFimPrestacao"))
                    {
                        select.Append(" FimPrestacaoPreCTe.UF_SIGLA UFFimPrestacao, ");

                        if (!groupBy.Contains("FimPrestacaoPreCTe.UF_SIGLA"))
                            groupBy.Append(" FimPrestacaoPreCTe.UF_SIGLA, ");

                        SetarJoinsLocalidadeFimPrestacao(joins);
                    }
                    break;


                case "IBGEInicioPrestacao":
                    if (!select.Contains(" IBGEInicioPrestacao,"))
                    {
                        select.Append(" InicioPrestacaoPreCTe.LOC_IBGE IBGEInicioPrestacao, ");
                        groupBy.Append("InicioPrestacaoPreCTe.LOC_IBGE, ");

                        SetarJoinsLocalidadeInicioPrestacao(joins);
                    }
                    break;

                case "InicioPrestacao":
                    if (!select.Contains(" InicioPrestacao,"))
                    {
                        select.Append(" InicioPrestacaoPreCTe.LOC_DESCRICAO + '-' + InicioPrestacaoPreCTe.UF_SIGLA InicioPrestacao, ");
                        groupBy.Append("InicioPrestacaoPreCTe.LOC_DESCRICAO, ");

                        if (!groupBy.Contains("InicioPrestacaoPreCTe.UF_SIGLA"))
                            groupBy.Append("InicioPrestacaoPreCTe.UF_SIGLA, ");

                        SetarJoinsLocalidadeInicioPrestacao(joins);
                    }
                    break;

                case "UFInicioPrestacao":
                    if (!select.Contains("UFInicioPrestacao"))
                    {
                        select.Append(" InicioPrestacaoPreCTe.UF_SIGLA UFInicioPrestacao, ");

                        if (!groupBy.Contains("InicioPrestacaoPreCTe.UF_SIGLA"))
                            groupBy.Append("InicioPrestacaoPreCTe.UF_SIGLA, ");

                        SetarJoinsLocalidadeInicioPrestacao(joins);
                    }
                    break;

                case "PesoPedido":
                    if (!select.Contains("PesoPedido"))
                    {
                        select.Append(@"(
                                            SELECT SUM(_cargaPedido.PED_PESO)
                                            FROM
                                                T_CARGA_CTE CargaCTe 
                                                JOIN T_CARGA_PEDIDO_XML_NOTA_FISCAL_CTE _cargaPedidoCte ON _cargaPedidoCte.CCT_CODIGO = CargaCTe.CCT_CODIGO 
                                                JOIN T_PEDIDO_XML_NOTA_FISCAL _pedidoXmlNotaFiscal ON _pedidoXmlNotaFiscal.PNF_CODIGO = _cargaPedidoCte.PNF_CODIGO 
                                                JOIN T_CARGA_PEDIDO _cargaPedido ON _cargaPedido.CPE_CODIGO = _pedidoXmlNotaFiscal.CPE_CODIGO 
                                            WHERE 
                                                CargaCTe.PCO_CODIGO = PreCTe.PCO_CODIGO
                                        ) PesoPedido, ");

                        if (!groupBy.Contains("PreCTe.PCO_CODIGO"))
                            groupBy.Append("PreCTe.PCO_CODIGO, ");
                    }
                    break;

                case "NumeroFolha":
                    if (!select.Contains("NumeroFolha,"))
                    {
                        select.Append(@"
                            case
                                when PreCTe.PCO_TIPO_CTE = 1
                                    then 
                                        substring((
                                    select distinct ', ' + stageComplementar.STA_NUMERO_FOLHA 

                                      from T_CARGA_CTE CargaCTe 
                                      join T_CARGA_PEDIDO_XML_NOTA_FISCAL_CTE CargaPedidoXMLNotaFiscalCTe on CargaPedidoXMLNotaFiscalCTe.CCT_CODIGO = CargaCTe.CCT_CODIGO 
                                      join T_PEDIDO_XML_NOTA_FISCAL PedidoXMLNotaFiscal on PedidoXMLNotaFiscal.PNF_CODIGO = CargaPedidoXMLNotaFiscalCTe.PNF_CODIGO 
                                      join T_CARGA_PEDIDO CargaPedido on CargaPedido.CPE_CODIGO = PedidoXMLNotaFiscal.CPE_CODIGO
                                      join T_STAGE _Stage on _Stage.STA_CODIGO = CargaPedido.STA_CODIGO_RELEVANTE_CUSTO
                                      join T_STAGE_COMPLEMENTO stageComplemento on stageComplemento.STA_CODIGO_COMPLEMENTADA = _Stage.STA_CODIGO
                                      join T_STAGE stageComplementar on stageComplementar.STA_CODIGO = stageComplemento.STA_CODIGO_COMPLEMENTAR

                                     where 
                                        CargaCTe.PCO_CODIGO = PreCTe.PCO_CODIGO
                                        and stageComplemento.COC_CODIGO = Ocorrencia.COC_CODIGO
                                           for xml path('')
                                    ), 3, 200)
                                else 
                                    substring((
                                    select distinct ', ' + _Stage.STA_NUMERO_FOLHA 
                                      from T_CARGA_CTE CargaCTe 
                                      join T_CARGA_PEDIDO_XML_NOTA_FISCAL_CTE CargaPedidoXMLNotaFiscalCTe on CargaPedidoXMLNotaFiscalCTe.CCT_CODIGO = CargaCTe.CCT_CODIGO 
                                      join T_PEDIDO_XML_NOTA_FISCAL PedidoXMLNotaFiscal on PedidoXMLNotaFiscal.PNF_CODIGO = CargaPedidoXMLNotaFiscalCTe.PNF_CODIGO 
                                      join T_CARGA_PEDIDO CargaPedido on CargaPedido.CPE_CODIGO = PedidoXMLNotaFiscal.CPE_CODIGO 
                                      join T_PEDIDO_STAGE PedidoStage on PedidoStage.PED_CODIGO = CargaPedido.PED_CODIGO 
                                      join T_STAGE _Stage on _Stage.STA_CODIGO = PedidoStage.STA_CODIGO 
                                     where CargaCTe.PCO_CODIGO = PreCTe.PCO_CODIGO
                                       for xml path('')
                                ), 3, 200)
                            end NumeroFolha, "
                        );

                        if (!groupBy.Contains("PreCTe.PCO_CODIGO"))
                            groupBy.Append(" PreCTe.PCO_CODIGO, ");

                        if (!groupBy.Contains("PreCTe.PCO_TIPO_CTE"))
                            groupBy.Append(" PreCTe.PCO_TIPO_CTE, ");

                        if (!groupBy.Contains("Ocorrencia.COC_CODIGO"))
                            groupBy.Append(" Ocorrencia.COC_CODIGO, ");

                        SetarJoinsOcorrencia(joins);
                    }
                    break;

                case "DataFolhaFormatada":
                case "DataFolha":
                    if (!select.Contains("DataFolha,"))
                    {
                        select.Append(@"
                            case
                                when PreCTe.PCO_TIPO_CTE = 1
                                    then 
                                        substring((
                                    select distinct ', ' + convert(varchar(10), stageComplementar.STA_DATA_FOLHA, 101) 

                                      from T_CARGA_CTE CargaCTe 
                                      join T_CARGA_PEDIDO_XML_NOTA_FISCAL_CTE CargaPedidoXMLNotaFiscalCTe on CargaPedidoXMLNotaFiscalCTe.CCT_CODIGO = CargaCTe.CCT_CODIGO 
                                      join T_PEDIDO_XML_NOTA_FISCAL PedidoXMLNotaFiscal on PedidoXMLNotaFiscal.PNF_CODIGO = CargaPedidoXMLNotaFiscalCTe.PNF_CODIGO 
                                      join T_CARGA_PEDIDO CargaPedido on CargaPedido.CPE_CODIGO = PedidoXMLNotaFiscal.CPE_CODIGO
                                      join T_STAGE _Stage on _Stage.STA_CODIGO = CargaPedido.STA_CODIGO_RELEVANTE_CUSTO
                                      join T_STAGE_COMPLEMENTO stageComplemento on stageComplemento.STA_CODIGO_COMPLEMENTADA = _Stage.STA_CODIGO
                                      join T_STAGE stageComplementar on stageComplementar.STA_CODIGO = stageComplemento.STA_CODIGO_COMPLEMENTAR

                                     where  
                                        CargaCTe.PCO_CODIGO = PreCTe.PCO_CODIGO
                                        and stageComplemento.COC_CODIGO = Ocorrencia.COC_CODIGO
                                           for xml path('')
                                    ), 3, 200)
                                else substring((
                                    select distinct ', ' + convert(varchar(10), _Stage.STA_DATA_FOLHA, 101)
                                      from T_CARGA_CTE CargaCTe 
                                      join T_CARGA_PEDIDO_XML_NOTA_FISCAL_CTE CargaPedidoXMLNotaFiscalCTe on CargaPedidoXMLNotaFiscalCTe.CCT_CODIGO = CargaCTe.CCT_CODIGO 
                                      join T_PEDIDO_XML_NOTA_FISCAL PedidoXMLNotaFiscal on PedidoXMLNotaFiscal.PNF_CODIGO = CargaPedidoXMLNotaFiscalCTe.PNF_CODIGO 
                                      join T_CARGA_PEDIDO CargaPedido on CargaPedido.CPE_CODIGO = PedidoXMLNotaFiscal.CPE_CODIGO 
                                      join T_PEDIDO_STAGE PedidoStage on PedidoStage.PED_CODIGO = CargaPedido.PED_CODIGO 
                                      join T_STAGE _Stage on _Stage.STA_CODIGO = PedidoStage.STA_CODIGO 
                                     where CargaCTe.PCO_CODIGO = PreCTe.PCO_CODIGO
                                       for xml path('')
                                ), 3, 200)
                            end DataFolha, "
                        );

                        if (!groupBy.Contains("PreCTe.PCO_CODIGO"))
                            groupBy.Append(" PreCTe.PCO_CODIGO, ");

                        if (!groupBy.Contains("PreCTe.PCO_CODIGO"))
                            groupBy.Append(" PreCTe.PCO_CODIGO, ");

                        if (!groupBy.Contains("Ocorrencia.COC_CODIGO"))
                            groupBy.Append(" Ocorrencia.COC_CODIGO, ");

                        SetarJoinsOcorrencia(joins);
                    }
                    break;

                case "FolhaCalculada":
                    if (!select.Contains("FolhaCalculada,"))
                    {
                        select.Append(@"
                            case
                                when PreCTe.PCO_TIPO_CTE = 1
                                    then 
                                        substring((
                                    select distinct ', ' + stageComplementar.STA_CALCULO 

                                      from T_CARGA_CTE CargaCTe 
                                      join T_CARGA_PEDIDO_XML_NOTA_FISCAL_CTE CargaPedidoXMLNotaFiscalCTe on CargaPedidoXMLNotaFiscalCTe.CCT_CODIGO = CargaCTe.CCT_CODIGO 
                                      join T_PEDIDO_XML_NOTA_FISCAL PedidoXMLNotaFiscal on PedidoXMLNotaFiscal.PNF_CODIGO = CargaPedidoXMLNotaFiscalCTe.PNF_CODIGO 
                                      join T_CARGA_PEDIDO CargaPedido on CargaPedido.CPE_CODIGO = PedidoXMLNotaFiscal.CPE_CODIGO
                                      join T_STAGE _Stage on _Stage.STA_CODIGO = CargaPedido.STA_CODIGO_RELEVANTE_CUSTO
                                      join T_STAGE_COMPLEMENTO stageComplemento on stageComplemento.STA_CODIGO_COMPLEMENTADA = _Stage.STA_CODIGO
                                      join T_STAGE stageComplementar on stageComplementar.STA_CODIGO = stageComplemento.STA_CODIGO_COMPLEMENTAR

                                     where  
                                        CargaCTe.PCO_CODIGO = PreCTe.PCO_CODIGO
                                        and stageComplemento.COC_CODIGO = Ocorrencia.COC_CODIGO
                                           for xml path('')
                                    ), 3, 200)
                                else substring((
                                    select distinct ', ' + _Stage.STA_CALCULO 
                                      from T_CARGA_CTE CargaCTe 
                                      join T_CARGA_PEDIDO_XML_NOTA_FISCAL_CTE CargaPedidoXMLNotaFiscalCTe on CargaPedidoXMLNotaFiscalCTe.CCT_CODIGO = CargaCTe.CCT_CODIGO 
                                      join T_PEDIDO_XML_NOTA_FISCAL PedidoXMLNotaFiscal on PedidoXMLNotaFiscal.PNF_CODIGO = CargaPedidoXMLNotaFiscalCTe.PNF_CODIGO 
                                      join T_CARGA_PEDIDO CargaPedido on CargaPedido.CPE_CODIGO = PedidoXMLNotaFiscal.CPE_CODIGO 
                                      join T_PEDIDO_STAGE PedidoStage on PedidoStage.PED_CODIGO = CargaPedido.PED_CODIGO 
                                      join T_STAGE _Stage on _Stage.STA_CODIGO = PedidoStage.STA_CODIGO 
                                     where CargaCTe.PCO_CODIGO = PreCTe.PCO_CODIGO
                                       for xml path('')
                                ), 3, 200)
                            end FolhaCalculada, "
                        );

                        if (!groupBy.Contains("PreCTe.PCO_CODIGO"))
                            groupBy.Append(" PreCTe.PCO_CODIGO, ");

                        if (!groupBy.Contains("PreCTe.PCO_TIPO_CTE"))
                            groupBy.Append(" PreCTe.PCO_TIPO_CTE, ");

                        if (!groupBy.Contains("Ocorrencia.COC_CODIGO"))
                            groupBy.Append(" Ocorrencia.COC_CODIGO, ");

                        SetarJoinsOcorrencia(joins);
                    }
                    break;

                case "FolhaAtribuida":
                    if (!select.Contains("FolhaAtribuida,"))
                    {
                        select.Append(@"
                            case
                                when PreCTe.PCO_TIPO_CTE = 1
                                    then 
                                        substring((
                                    select distinct ', ' + stageComplementar.STA_ATRIBUIDO 

                                      from T_CARGA_CTE CargaCTe 
                                      join T_CARGA_PEDIDO_XML_NOTA_FISCAL_CTE CargaPedidoXMLNotaFiscalCTe on CargaPedidoXMLNotaFiscalCTe.CCT_CODIGO = CargaCTe.CCT_CODIGO 
                                      join T_PEDIDO_XML_NOTA_FISCAL PedidoXMLNotaFiscal on PedidoXMLNotaFiscal.PNF_CODIGO = CargaPedidoXMLNotaFiscalCTe.PNF_CODIGO 
                                      join T_CARGA_PEDIDO CargaPedido on CargaPedido.CPE_CODIGO = PedidoXMLNotaFiscal.CPE_CODIGO
                                      join T_STAGE _Stage on _Stage.STA_CODIGO = CargaPedido.STA_CODIGO_RELEVANTE_CUSTO
                                      join T_STAGE_COMPLEMENTO stageComplemento on stageComplemento.STA_CODIGO_COMPLEMENTADA = _Stage.STA_CODIGO
                                      join T_STAGE stageComplementar on stageComplementar.STA_CODIGO = stageComplemento.STA_CODIGO_COMPLEMENTAR

                                     where  
                                        CargaCTe.PCO_CODIGO = PreCTe.PCO_CODIGO
                                        and stageComplemento.COC_CODIGO = Ocorrencia.COC_CODIGO
                                           for xml path('')
                                    ), 3, 200)
                                else substring((
                                    select distinct ', ' + _Stage.STA_ATRIBUIDO 
                                      from T_CARGA_CTE CargaCTe 
                                      join T_CARGA_PEDIDO_XML_NOTA_FISCAL_CTE CargaPedidoXMLNotaFiscalCTe on CargaPedidoXMLNotaFiscalCTe.CCT_CODIGO = CargaCTe.CCT_CODIGO 
                                      join T_PEDIDO_XML_NOTA_FISCAL PedidoXMLNotaFiscal on PedidoXMLNotaFiscal.PNF_CODIGO = CargaPedidoXMLNotaFiscalCTe.PNF_CODIGO 
                                      join T_CARGA_PEDIDO CargaPedido on CargaPedido.CPE_CODIGO = PedidoXMLNotaFiscal.CPE_CODIGO 
                                      join T_PEDIDO_STAGE PedidoStage on PedidoStage.PED_CODIGO = CargaPedido.PED_CODIGO 
                                      join T_STAGE _Stage on _Stage.STA_CODIGO = PedidoStage.STA_CODIGO 
                                     where CargaCTe.PCO_CODIGO = PreCTe.PCO_CODIGO
                                       for xml path('')
                                ), 3, 200)
                            end FolhaAtribuida, "
                        );

                        if (!groupBy.Contains("PreCTe.PCO_CODIGO"))
                            groupBy.Append(" PreCTe.PCO_CODIGO, ");

                        if (!groupBy.Contains("PreCTe.PCO_TIPO_CTE"))
                            groupBy.Append(" PreCTe.PCO_TIPO_CTE, ");

                        if (!groupBy.Contains("Ocorrencia.COC_CODIGO"))
                            groupBy.Append(" Ocorrencia.COC_CODIGO, ");

                        SetarJoinsOcorrencia(joins);
                    }
                    break;

                case "FolhaTransferida":
                    if (!select.Contains("FolhaTransferida,"))
                    {
                        select.Append(@"
                            case
                                when PreCTe.PCO_TIPO_CTE = 1
                                    then 
                                        substring((
                                    select distinct ', ' + stageComplementar.STA_TRANSFERIDO 

                                      from T_CARGA_CTE CargaCTe 
                                      join T_CARGA_PEDIDO_XML_NOTA_FISCAL_CTE CargaPedidoXMLNotaFiscalCTe on CargaPedidoXMLNotaFiscalCTe.CCT_CODIGO = CargaCTe.CCT_CODIGO 
                                      join T_PEDIDO_XML_NOTA_FISCAL PedidoXMLNotaFiscal on PedidoXMLNotaFiscal.PNF_CODIGO = CargaPedidoXMLNotaFiscalCTe.PNF_CODIGO 
                                      join T_CARGA_PEDIDO CargaPedido on CargaPedido.CPE_CODIGO = PedidoXMLNotaFiscal.CPE_CODIGO
                                      join T_STAGE _Stage on _Stage.STA_CODIGO = CargaPedido.STA_CODIGO_RELEVANTE_CUSTO
                                      join T_STAGE_COMPLEMENTO stageComplemento on stageComplemento.STA_CODIGO_COMPLEMENTADA = _Stage.STA_CODIGO
                                      join T_STAGE stageComplementar on stageComplementar.STA_CODIGO = stageComplemento.STA_CODIGO_COMPLEMENTAR

                                     where  
                                        CargaCTe.PCO_CODIGO = PreCTe.PCO_CODIGO
                                        and stageComplemento.COC_CODIGO = Ocorrencia.COC_CODIGO
                                           for xml path('')
                                    ), 3, 200)
                                else substring((
                                    select distinct ', ' + _Stage.STA_TRANSFERIDO 
                                      from T_CARGA_CTE CargaCTe 
                                      join T_CARGA_PEDIDO_XML_NOTA_FISCAL_CTE CargaPedidoXMLNotaFiscalCTe on CargaPedidoXMLNotaFiscalCTe.CCT_CODIGO = CargaCTe.CCT_CODIGO 
                                      join T_PEDIDO_XML_NOTA_FISCAL PedidoXMLNotaFiscal on PedidoXMLNotaFiscal.PNF_CODIGO = CargaPedidoXMLNotaFiscalCTe.PNF_CODIGO 
                                      join T_CARGA_PEDIDO CargaPedido on CargaPedido.CPE_CODIGO = PedidoXMLNotaFiscal.CPE_CODIGO 
                                      join T_PEDIDO_STAGE PedidoStage on PedidoStage.PED_CODIGO = CargaPedido.PED_CODIGO 
                                      join T_STAGE _Stage on _Stage.STA_CODIGO = PedidoStage.STA_CODIGO 
                                     where CargaCTe.PCO_CODIGO = PreCTe.PCO_CODIGO
                                       for xml path('')
                                ), 3, 200)
                            end FolhaTransferida, "
                        );

                        if (!groupBy.Contains("PreCTe.PCO_CODIGO"))
                            groupBy.Append(" PreCTe.PCO_CODIGO, ");

                        if (!groupBy.Contains("PreCTe.PCO_TIPO_CTE"))
                            groupBy.Append(" PreCTe.PCO_TIPO_CTE, ");

                        if (!groupBy.Contains("Ocorrencia.COC_CODIGO"))
                            groupBy.Append(" Ocorrencia.COC_CODIGO, ");

                        SetarJoinsOcorrencia(joins);
                    }
                    break;

                case "FolhaCancelada":
                case "FolhaCanceladaFormatada":
                    if (!select.Contains("FolhaCancelada,"))
                    {
                        select.Append(@"
                            case
                                when PreCTe.PCO_TIPO_CTE = 1
                                    then 
                                        substring((
                                    select distinct ', ' + (case when stageComplementar.STA_CANCELADO = 1 then 'Sim' else 'Não' end) 

                                      from T_CARGA_CTE CargaCTe 
                                      join T_CARGA_PEDIDO_XML_NOTA_FISCAL_CTE CargaPedidoXMLNotaFiscalCTe on CargaPedidoXMLNotaFiscalCTe.CCT_CODIGO = CargaCTe.CCT_CODIGO 
                                      join T_PEDIDO_XML_NOTA_FISCAL PedidoXMLNotaFiscal on PedidoXMLNotaFiscal.PNF_CODIGO = CargaPedidoXMLNotaFiscalCTe.PNF_CODIGO 
                                      join T_CARGA_PEDIDO CargaPedido on CargaPedido.CPE_CODIGO = PedidoXMLNotaFiscal.CPE_CODIGO
                                      join T_STAGE _Stage on _Stage.STA_CODIGO = CargaPedido.STA_CODIGO_RELEVANTE_CUSTO
                                      join T_STAGE_COMPLEMENTO stageComplemento on stageComplemento.STA_CODIGO_COMPLEMENTADA = _Stage.STA_CODIGO
                                      join T_STAGE stageComplementar on stageComplementar.STA_CODIGO = stageComplemento.STA_CODIGO_COMPLEMENTAR

                                     where  
                                        CargaCTe.PCO_CODIGO = PreCTe.PCO_CODIGO
                                        and stageComplemento.COC_CODIGO = Ocorrencia.COC_CODIGO
                                           for xml path('')
                                    ), 3, 200)
                                else substring((
                                     select distinct ', ' + (case when _Stage.STA_CANCELADO = 1 then 'Sim' else 'Não' end)  
                                      from T_CARGA_CTE CargaCTe 
                                      join T_CARGA_PEDIDO_XML_NOTA_FISCAL_CTE CargaPedidoXMLNotaFiscalCTe on CargaPedidoXMLNotaFiscalCTe.CCT_CODIGO = CargaCTe.CCT_CODIGO 
                                      join T_PEDIDO_XML_NOTA_FISCAL PedidoXMLNotaFiscal on PedidoXMLNotaFiscal.PNF_CODIGO = CargaPedidoXMLNotaFiscalCTe.PNF_CODIGO 
                                      join T_CARGA_PEDIDO CargaPedido on CargaPedido.CPE_CODIGO = PedidoXMLNotaFiscal.CPE_CODIGO 
                                      join T_PEDIDO_STAGE PedidoStage on PedidoStage.PED_CODIGO = CargaPedido.PED_CODIGO 
                                      join T_STAGE _Stage on _Stage.STA_CODIGO = PedidoStage.STA_CODIGO 
                                     where CargaCTe.PCO_CODIGO = PreCTe.PCO_CODIGO
                                       for xml path('')
                                ), 3, 200)
                            end FolhaCancelada, "
                        );

                        if (!groupBy.Contains("PreCTe.PCO_CODIGO"))
                            groupBy.Append(" PreCTe.PCO_CODIGO, ");

                        if (!groupBy.Contains("PreCTe.PCO_TIPO_CTE"))
                            groupBy.Append(" PreCTe.PCO_TIPO_CTE, ");

                        if (!groupBy.Contains("Ocorrencia.COC_CODIGO"))
                            groupBy.Append(" Ocorrencia.COC_CODIGO, ");

                        SetarJoinsOcorrencia(joins);
                    }
                    break;

                case "FolhaInconsistente":
                case "FolhaInconsistenteFormatada":
                    if (!select.Contains("FolhaInconsistente,"))
                    {
                        select.Append(@"
                            case
                                when PreCTe.PCO_TIPO_CTE = 1
                                    then 
                                        substring((
                                    select distinct ', ' +(case when stageComplementar.STA_INCONSISTENTE = 1 then 'Sim' else 'Não' end) 

                                      from T_CARGA_CTE CargaCTe 
                                      join T_CARGA_PEDIDO_XML_NOTA_FISCAL_CTE CargaPedidoXMLNotaFiscalCTe on CargaPedidoXMLNotaFiscalCTe.CCT_CODIGO = CargaCTe.CCT_CODIGO 
                                      join T_PEDIDO_XML_NOTA_FISCAL PedidoXMLNotaFiscal on PedidoXMLNotaFiscal.PNF_CODIGO = CargaPedidoXMLNotaFiscalCTe.PNF_CODIGO 
                                      join T_CARGA_PEDIDO CargaPedido on CargaPedido.CPE_CODIGO = PedidoXMLNotaFiscal.CPE_CODIGO
                                      join T_STAGE _Stage on _Stage.STA_CODIGO = CargaPedido.STA_CODIGO_RELEVANTE_CUSTO
                                      join T_STAGE_COMPLEMENTO stageComplemento on stageComplemento.STA_CODIGO_COMPLEMENTADA = _Stage.STA_CODIGO
                                      join T_STAGE stageComplementar on stageComplementar.STA_CODIGO = stageComplemento.STA_CODIGO_COMPLEMENTAR

                                     where  
                                        CargaCTe.PCO_CODIGO = PreCTe.PCO_CODIGO
                                        and stageComplemento.COC_CODIGO = Ocorrencia.COC_CODIGO
                                           for xml path('')
                                    ), 3, 200)
                                else substring((
                                    select distinct ', ' + (case when _Stage.STA_INCONSISTENTE = 1 then 'Sim' else 'Não' end) 
                                      from T_CARGA_CTE CargaCTe 
                                      join T_CARGA_PEDIDO_XML_NOTA_FISCAL_CTE CargaPedidoXMLNotaFiscalCTe on CargaPedidoXMLNotaFiscalCTe.CCT_CODIGO = CargaCTe.CCT_CODIGO 
                                      join T_PEDIDO_XML_NOTA_FISCAL PedidoXMLNotaFiscal on PedidoXMLNotaFiscal.PNF_CODIGO = CargaPedidoXMLNotaFiscalCTe.PNF_CODIGO 
                                      join T_CARGA_PEDIDO CargaPedido on CargaPedido.CPE_CODIGO = PedidoXMLNotaFiscal.CPE_CODIGO 
                                      join T_PEDIDO_STAGE PedidoStage on PedidoStage.PED_CODIGO = CargaPedido.PED_CODIGO 
                                      join T_STAGE _Stage on _Stage.STA_CODIGO = PedidoStage.STA_CODIGO 
                                     where CargaCTe.PCO_CODIGO = PreCTe.PCO_CODIGO
                                       for xml path('')
                                ), 3, 200)
                            end FolhaInconsistente, "
                        );

                        if (!groupBy.Contains("PreCTe.PCO_CODIGO"))
                            groupBy.Append(" PreCTe.PCO_CODIGO, ");

                        if (!groupBy.Contains("PreCTe.PCO_TIPO_CTE"))
                            groupBy.Append(" PreCTe.PCO_TIPO_CTE, ");

                        if (!groupBy.Contains("Ocorrencia.COC_CODIGO"))
                            groupBy.Append(" Ocorrencia.COC_CODIGO, ");

                        SetarJoinsOcorrencia(joins);
                    }
                    break;

                case "InconsistenciaFolha":
                    if (!select.Contains("InconsistenciaFolha,"))
                    {
                        select.Append(@"
                            case
                                when PreCTe.PCO_TIPO_CTE = 1
                                    then 
                                        substring((
                                    select distinct ', ' + stageComplementar.STA_MENSAGEM_RETORNO_ETAPA 

                                      from T_CARGA_CTE CargaCTe 
                                      join T_CARGA_PEDIDO_XML_NOTA_FISCAL_CTE CargaPedidoXMLNotaFiscalCTe on CargaPedidoXMLNotaFiscalCTe.CCT_CODIGO = CargaCTe.CCT_CODIGO 
                                      join T_PEDIDO_XML_NOTA_FISCAL PedidoXMLNotaFiscal on PedidoXMLNotaFiscal.PNF_CODIGO = CargaPedidoXMLNotaFiscalCTe.PNF_CODIGO 
                                      join T_CARGA_PEDIDO CargaPedido on CargaPedido.CPE_CODIGO = PedidoXMLNotaFiscal.CPE_CODIGO
                                      join T_STAGE _Stage on _Stage.STA_CODIGO = CargaPedido.STA_CODIGO_RELEVANTE_CUSTO
                                      join T_STAGE_COMPLEMENTO stageComplemento on stageComplemento.STA_CODIGO_COMPLEMENTADA = _Stage.STA_CODIGO
                                      join T_STAGE stageComplementar on stageComplementar.STA_CODIGO = stageComplemento.STA_CODIGO_COMPLEMENTAR

                                     where  
                                        CargaCTe.PCO_CODIGO = PreCTe.PCO_CODIGO
                                        and stageComplemento.COC_CODIGO = Ocorrencia.COC_CODIGO
                                           for xml path('')
                                    ), 3, 200)
                                else substring((
                                    select distinct ', ' + _Stage.STA_MENSAGEM_RETORNO_ETAPA
                                      from T_CARGA_CTE CargaCTe 
                                      join T_CARGA_PEDIDO_XML_NOTA_FISCAL_CTE CargaPedidoXMLNotaFiscalCTe on CargaPedidoXMLNotaFiscalCTe.CCT_CODIGO = CargaCTe.CCT_CODIGO 
                                      join T_PEDIDO_XML_NOTA_FISCAL PedidoXMLNotaFiscal on PedidoXMLNotaFiscal.PNF_CODIGO = CargaPedidoXMLNotaFiscalCTe.PNF_CODIGO 
                                      join T_CARGA_PEDIDO CargaPedido on CargaPedido.CPE_CODIGO = PedidoXMLNotaFiscal.CPE_CODIGO 
                                      join T_PEDIDO_STAGE PedidoStage on PedidoStage.PED_CODIGO = CargaPedido.PED_CODIGO 
                                      join T_STAGE _Stage on _Stage.STA_CODIGO = PedidoStage.STA_CODIGO 
                                     where CargaCTe.PCO_CODIGO = PreCTe.PCO_CODIGO
                                       for xml path('')
                                ), 3, 200)
                            end InconsistenciaFolha, "
                        );


                        if (!groupBy.Contains("PreCTe.PCO_CODIGO"))
                            groupBy.Append(" PreCTe.PCO_CODIGO, ");

                        if (!groupBy.Contains("PreCTe.PCO_TIPO_CTE"))
                            groupBy.Append(" PreCTe.PCO_TIPO_CTE, ");

                        if (!groupBy.Contains("Ocorrencia.COC_CODIGO"))
                            groupBy.Append(" Ocorrencia.COC_CODIGO, ");

                        SetarJoinsOcorrencia(joins);
                    }
                    break;

                case "SituacaoPreCTeDescricao":
                case "SituacaoPreCTe":
                    if (!select.Contains(" SituacaoPreCTe, "))
                    {
                        select.Append("(SELECT CASE WHEN (_cargaCte.PCO_CODIGO IS NOT NULL AND _cargaCte.CON_CODIGO IS NOT NULL) THEN 2 ELSE 1 END AS SituacaoPreCTe from T_CARGA_CTE _cargaCte WHERE _cargaCte.PCO_CODIGO IS NOT NULL AND _cargaCte.PCO_CODIGO = PreCTe.PCO_CODIGO) SituacaoPreCTe, ");

                        if (!groupBy.Contains("PreCTe.PCO_CODIGO"))
                            groupBy.Append(" PreCTe.PCO_CODIGO, ");
                    }
                    break;



                default:
                    if (!somenteContarNumeroRegistros && propriedade.Contains("ValorComponente"))
                    {
                        select.Append("(select SUM(CargaCTeComponenteFrete.CCC_VALOR_COMPONENTE) from T_CARGA_CTE_COMPONENTES_FRETE CargaCTeComponenteFrete inner join T_CARGA_CTE CargaCTe on CargaCTe.CCT_CODIGO = CargaCTeComponenteFrete.CCT_CODIGO where CargaCTe.PCO_CODIGO = PreCTe.PCO_CODIGO and CargaCTeComponenteFrete.CFR_CODIGO = " + codigoDinamico + ") " + propriedade + ", "); 

                        if (!groupBy.Contains("PreCTe.PCO_CODIGO"))
                            groupBy.Append("PreCTe.PCO_CODIGO, ");
                    }
                    break;


            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.PreCTes.FiltroPesquisaRelatorioPreCTe filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            string pattern = "yyyy-MM-dd";


            if (filtrosPesquisa.CodigosTiposOperacao?.Count > 0)
                where.Append($" and exists (select CargaCTe.PCO_CODIGO from T_CARGA_CTE CargaCTe inner join T_CARGA Carga on CargaCTe.CAR_CODIGO_ORIGEM = Carga.CAR_CODIGO where PreCTe.PCO_CODIGO = CargaCTe.PCO_CODIGO AND Carga.TOP_CODIGO in " +
                    $"({string.Join(", ", filtrosPesquisa.CodigosTiposOperacao)}){(filtrosPesquisa.CodigosTiposOperacao.Contains(-1) ? " or Carga.TOP_CODIGO is null" : "")})");

            if (filtrosPesquisa.DataEmissaoInicial.HasValue)
                where.Append("  and PreCTe.PCO_DATAHORAEMISSAO >= '" + filtrosPesquisa.DataEmissaoInicial.Value.ToString(pattern) + "'");

            if (filtrosPesquisa.DataEmissaoFinal.HasValue)
                where.Append("  and PreCTe.PCO_DATAHORAEMISSAO < '" + filtrosPesquisa.DataEmissaoFinal.Value.AddDays(1).ToString(pattern) + "'");

            if (filtrosPesquisa.PossuiFRS.HasValue)
            {
                string whereFRS;

                if (filtrosPesquisa.PossuiFRS.Value)
                {
                    whereFRS = "IS NOT NULL";
                }
                else
                {
                    whereFRS = "IS NULL";
                }

                string subQuery = @$"
                    (CASE WHEN PreCTe.PCO_TIPO_CTE = 1 
	                    THEN
                            (SELECT distinct CargaCTe.PCO_CODIGO from T_CARGA_CTE CargaCTe 
		                    join T_CARGA_PEDIDO_XML_NOTA_FISCAL_CTE CargaPedidoXMLNotaFiscalCTe on CargaPedidoXMLNotaFiscalCTe.CCT_CODIGO = CargaCTe.CCT_CODIGO 
		                    join T_PEDIDO_XML_NOTA_FISCAL PedidoXMLNotaFiscal on PedidoXMLNotaFiscal.PNF_CODIGO = CargaPedidoXMLNotaFiscalCTe.PNF_CODIGO 
		                    join T_CARGA_PEDIDO CargaPedido on CargaPedido.CPE_CODIGO = PedidoXMLNotaFiscal.CPE_CODIGO
		                    join T_STAGE _Stage on _Stage.STA_CODIGO = CargaPedido.STA_CODIGO_RELEVANTE_CUSTO
		                    join T_STAGE_COMPLEMENTO stageComplemento on stageComplemento.STA_CODIGO_COMPLEMENTADA = _Stage.STA_CODIGO
		                    join T_STAGE stageComplementar on stageComplementar.STA_CODIGO = stageComplemento.STA_CODIGO_COMPLEMENTAR
		 
		                      WHERE stageComplementar.STA_CANCELADO = 0 AND stageComplementar.STA_NUMERO_FOLHA {whereFRS} and CargaCTe.PCO_CODIGO = PreCTe.PCO_CODIGO AND stageComplemento.COC_CODIGO = Ocorrencia.COC_CODIGO) 
		  
	                    ELSE 
		                    (SELECT distinct CargaCTe.PCO_CODIGO from T_CARGA_CTE CargaCTe 
		                    join T_CARGA_PEDIDO_XML_NOTA_FISCAL_CTE CargaPedidoXMLNotaFiscalCTe on CargaPedidoXMLNotaFiscalCTe.CCT_CODIGO = CargaCTe.CCT_CODIGO 
		                    join T_PEDIDO_XML_NOTA_FISCAL PedidoXMLNotaFiscal on PedidoXMLNotaFiscal.PNF_CODIGO = CargaPedidoXMLNotaFiscalCTe.PNF_CODIGO 
		                    join T_CARGA_PEDIDO CargaPedido on CargaPedido.CPE_CODIGO = PedidoXMLNotaFiscal.CPE_CODIGO 
		                    join T_PEDIDO_STAGE PedidoStage on PedidoStage.PED_CODIGO = CargaPedido.PED_CODIGO 
		                    join T_STAGE _Stage on _Stage.STA_CODIGO = PedidoStage.STA_CODIGO
		
		                     WHERE _Stage.STA_CANCELADO = 0 AND _Stage.STA_NUMERO_FOLHA {whereFRS} and CargaCTe.PCO_CODIGO = PreCTe.PCO_CODIGO)

	                END)";

                where.Append(@$"AND {subQuery} is not null AND {subQuery} != ''");

                SetarJoinsOcorrencia(joins);
            }


            if (filtrosPesquisa.CodigoOrigem > 0)
                where.Append("  and PreCTe.PCO_LOCINICIOPRESTACAO = " + filtrosPesquisa.CodigoOrigem.ToString());

            if (filtrosPesquisa.CodigoDestino > 0)
                where.Append("  and PreCTe.PCO_LOCTERMINOPRESTACAO = " + filtrosPesquisa.CodigoDestino.ToString());

            if (filtrosPesquisa.CodigosRemetentes?.Count > 0)
            {
                where.Append($"  and ClienteRemetente.CLI_CGCCPF in ( {string.Join(", ", filtrosPesquisa.CodigosRemetentes.Select(x => x.ToString("F0")))} ) ");

                SetarJoinsRemetenteCliente(joins);
            }

            if (filtrosPesquisa.CodigosRecebedores?.Count > 0)
            {
                where.Append($"  and ClienteRecebedor.CLI_CGCCPF in ( {string.Join(", ", filtrosPesquisa.CodigosRecebedores.Select(x => x.ToString("F0")))} ) ");

                SetarJoinsRecebedorCliente(joins);
            }

            if (filtrosPesquisa.CodigosModelosVeiculos?.Count > 0)
            {
                where.Append($" and VeiculoModelo.VMO_CODIGO = " + filtrosPesquisa.CodigosModelosVeiculos.ToString());
                SetarJoinsModeloVeiculo(joins);
            }


            if (filtrosPesquisa.CodigosDestinatarios?.Count > 0)
            {
                where.Append($"  and ClienteDestinatario.CLI_CGCCPF in ( {string.Join(", ", filtrosPesquisa.CodigosDestinatarios.Select(x => x.ToString("F0")))} ) ");

                SetarJoinsDestinatarioCliente(joins);
            }

            if (filtrosPesquisa.CodigosExpedidores?.Count > 0)
            {
                where.Append($"  and ClienteExpedidor.CLI_CGCCPF in ( {string.Join(", ", filtrosPesquisa.CodigosExpedidores.Select(x => x.ToString("F0")))} ) ");

                SetarJoinsExpedidorCliente(joins);
            }

            if (filtrosPesquisa.CodigosTomadores?.Count > 0)
            {
                where.Append($"  and ClienteTomador.CLI_CGCCPF in ( {string.Join(", ", filtrosPesquisa.CodigosTomadores.Select(x => x.ToString("F0")))} ) ");

                SetarJoinsTomadorCliente(joins);
            }

            if (filtrosPesquisa.CodigosCargas?.Count > 0)
                where.Append("  and exists (select CargaCTe.PCO_CODIGO from T_CARGA_CTE CargaCTe where PreCTe.PCO_CODIGO = CargaCTe.PCO_CODIGO " +
                    $"AND (CargaCTe.CAR_CODIGO_ORIGEM in " +
                    $"({string.Join(", ", filtrosPesquisa.CodigosCargas)})" +
                    " or CargaCTe.CAR_CODIGO in " +
                    $"({string.Join(", ", filtrosPesquisa.CodigosCargas)})))");

            if (filtrosPesquisa.CodigosFiliais?.Count > 0)
            {
                where.Append($" and exists (select CargaCTe.PCO_CODIGO from T_CARGA_CTE CargaCTe inner join T_CARGA Carga on CargaCTe.CAR_CODIGO_ORIGEM = Carga.CAR_CODIGO where PreCTe.PCO_CODIGO = CargaCTe.PCO_CODIGO AND Carga.FIL_CODIGO in " +
                    $"({string.Join(",", filtrosPesquisa.CodigosFiliais)}))");
            }

            if (filtrosPesquisa.CodigosTransportadores?.Count > 0)
                where.Append($"  and PreCTe.EMP_CODIGO IN ({string.Join(", ", filtrosPesquisa.CodigosTransportadores)})");


            if (filtrosPesquisa.TipoTomador != null && filtrosPesquisa.TipoTomador.Count > 0)
            {
                where.Append("  and PreCTe.PCO_TOMADOR in ('" + string.Join("', '", filtrosPesquisa.TipoTomador.Select(x => (int)x)) + "')");
            }

            if (filtrosPesquisa.CodigosTiposOcorrencia?.Count > 0)
                where.Append($" and exists (select ComplementoInfo.PCO_CODIGO from T_CARGA_CTE_COMPLEMENTO_INFO ComplementoInfo left join T_CARGA_OCORRENCIA CargaOcorrencia on CargaOcorrencia.COC_CODIGO = ComplementoInfo.COC_CODIGO WHERE PreCTe.PCO_CODIGO = ComplementoInfo.PCO_CODIGO AND CargaOcorrencia.OCO_CODIGO in ({string.Join(",", filtrosPesquisa.CodigosTiposOcorrencia)} ))"); // SQL-INJECTION-SAFE

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.CodigoEstadoOrigem) && filtrosPesquisa.CodigoEstadoOrigem != "0")
            {
                where.Append("  and InicioPrestacaoPreCTe.UF_SIGLA = '" + filtrosPesquisa.CodigoEstadoOrigem + "'");

                SetarJoinsLocalidadeInicioPrestacao(joins);
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.CodigoEstadoDestino) && filtrosPesquisa.CodigoEstadoDestino != "0") 
            {
                where.Append("  and FimPrestacaoPreCTe.UF_SIGLA = :FIMPRESTACAOPRECTE_UF_SIGLA");
                parametros.Add(new Embarcador.Consulta.ParametroSQL("FIMPRESTACAOPRECTE_UF_SIGLA", filtrosPesquisa.CodigoEstadoDestino));

                SetarJoinsLocalidadeFimPrestacao(joins);
            }

            if (filtrosPesquisa.CodigosTiposDeCarga?.Count > 0)
                where.Append($" and exists (select CargaCTe.PCO_CODIGO from T_CARGA_CTE CargaCTe inner join T_CARGA Carga on CargaCTe.CAR_CODIGO_ORIGEM = Carga.CAR_CODIGO where PreCTe.PCO_CODIGO = CargaCTe.PCO_CODIGO AND (Carga.TCG_CODIGO in ({string.Join(",", filtrosPesquisa.CodigosTiposDeCarga)}){(filtrosPesquisa.CodigosTiposDeCarga.Contains(-1) ? " or Carga.TCG_CODIGO is null)" : ")")})"); // SQL-INJECTION-SAFE

            if (filtrosPesquisa.Situacao.HasValue)
            {
                if (filtrosPesquisa.Situacao.Value == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRelatorioPreCTe.CTeRecebido)
                {
                    where.Append(" and exists (SELECT _cargaCte.PCO_CODIGO FROM T_CARGA_CTE _cargaCte WHERE _cargaCte.PCO_CODIGO IS NOT NULL AND _cargaCte.CON_CODIGO is not null AND _cargaCte.PCO_CODIGO = PreCTe.PCO_CODIGO ) ");
                }
                else if (filtrosPesquisa.Situacao.Value == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRelatorioPreCTe.Pendente)
                {
                    where.Append(" and exists (SELECT _cargaCte.PCO_CODIGO FROM T_CARGA_CTE _cargaCte WHERE _cargaCte.PCO_CODIGO IS NOT NULL AND _cargaCte.CON_CODIGO IS NULL AND _cargaCte.PCO_CODIGO = PreCTe.PCO_CODIGO ) ");
                }
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroNFe))
            {
                where.Append("  and exists (SELECT _notafiscal.PCO_CODIGO FROM T_PRE_CTE_DOCS _notafiscal WHERE PreCTe.PCO_CODIGO = _notafiscal.PCO_CODIGO AND _notafiscal.PNF_NUMERO LIKE :NOTAFISCAL_PNF_NUMERO) ");
                parametros.Add(new Embarcador.Consulta.ParametroSQL("NOTAFISCAL_PNF_NUMERO", $"%{filtrosPesquisa.NumeroNFe}%"));
            }



        }

        #endregion
    }
}
