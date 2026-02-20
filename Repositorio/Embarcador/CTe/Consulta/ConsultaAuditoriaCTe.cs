using Dominio.ObjetosDeValor.Embarcador.CTe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.CTe
{
    sealed class ConsultaAuditoriaCTe : Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioAuditoriaCTe>
    {
        #region Construtor
        
        public ConsultaAuditoriaCTe() : base(tabela: "T_CTE as CTe") { }

        #endregion

        #region Métodos Privados

        private void SetarJoinsTomador(StringBuilder joins)
        {
            if (!joins.Contains(" Tomador "))
                joins.Append(" left join T_CTE_PARTICIPANTE Tomador ON Tomador.PCT_CODIGO = CTe.CON_TOMADOR_PAGADOR_CTE");
        }

        private void SetarJoinsLocalidadeTomador(StringBuilder joins)
        {
            SetarJoinsTomador(joins);

            if (!joins.Contains(" LocalidadeTomador "))
                joins.Append(" left join T_LOCALIDADES LocalidadeTomador ON LocalidadeTomador.LOC_CODIGO = Tomador.LOC_CODIGO");
        }

        private void SetarJoinsClienteTomador(StringBuilder joins)
        {
            SetarJoinsTomador(joins);

            if (!joins.Contains(" ClienteTomador "))
                joins.Append(" left join T_CLIENTE ClienteTomador on ClienteTomador.CLI_CGCCPF = Tomador.CLI_CODIGO ");
        }

        private void SetarJoinsRemetente(StringBuilder joins)
        {
            if (!joins.Contains(" Remetente "))
                joins.Append(" left join T_CTE_PARTICIPANTE Remetente ON Remetente.PCT_CODIGO = CTe.CON_REMETENTE_CTE");
        }

        private void SetarJoinsLocalidadeRemetente(StringBuilder joins)
        {
            SetarJoinsRemetente(joins);

            if (!joins.Contains(" LocalidadeRemetente "))
                joins.Append(" left join T_LOCALIDADES LocalidadeRemetente ON LocalidadeRemetente.LOC_CODIGO = Remetente.LOC_CODIGO");
        }

        private void SetarJoinsExpedidor(StringBuilder joins)
        {
            if (!joins.Contains(" Expedidor "))
                joins.Append(" left join T_CTE_PARTICIPANTE Expedidor ON Expedidor.PCT_CODIGO = CTe.CON_EXPEDIDOR_CTE");
        }

        private void SetarJoinsLocalidadeExpedidor(StringBuilder joins)
        {
            SetarJoinsExpedidor(joins);

            if (!joins.Contains(" LocalidadeExpedidor "))
                joins.Append(" left join T_LOCALIDADES LocalidadeExpedidor ON LocalidadeExpedidor.LOC_CODIGO = Expedidor.LOC_CODIGO");
        }

        private void SetarJoinsRecebedor(StringBuilder joins)
        {
            if (!joins.Contains(" Recebedor "))
                joins.Append(" left join T_CTE_PARTICIPANTE Recebedor ON Recebedor.PCT_CODIGO = CTe.CON_RECEBEDOR_CTE");
        }

        private void SetarJoinsLocalidadeRecebedor(StringBuilder joins)
        {
            SetarJoinsRecebedor(joins);

            if (!joins.Contains(" LocalidadeRecebedor "))
                joins.Append(" left join T_LOCALIDADES LocalidadeRecebedor ON LocalidadeRecebedor.LOC_CODIGO = Recebedor.LOC_CODIGO");
        }

        private void SetarJoinsDestinatario(StringBuilder joins)
        {
            if (!joins.Contains(" Destinatario "))
                joins.Append(" left join T_CTE_PARTICIPANTE Destinatario ON Destinatario.PCT_CODIGO = CTe.CON_DESTINATARIO_CTE");
        }

        private void SetarJoinsLocalidadeDestinatario(StringBuilder joins)
        {
            SetarJoinsDestinatario(joins);

            if (!joins.Contains(" LocalidadeDestinatario "))
                joins.Append(" left join T_LOCALIDADES LocalidadeDestinatario ON LocalidadeDestinatario.LOC_CODIGO = Destinatario.LOC_CODIGO");
        }

        private void SetarJoinsCargaCte(StringBuilder joins)
        {
            if (!joins.Contains(" CargaCTe "))
                joins.Append(" left join T_CARGA_CTE CargaCTe ON CargaCTe.CON_CODIGO = CTe.CON_CODIGO");
        }

        private void SetarJoinsCarga(StringBuilder joins)
        {
            SetarJoinsCargaCte(joins);

            if (!joins.Contains(" Carga "))
                joins.Append(" left join T_CARGA Carga ON Carga.CAR_CODIGO = CargaCTe.CAR_CODIGO AND Carga.CAR_CARGA_TRANSBORDO = 0");
        }

        private void SetarJoinsOperador(StringBuilder joins)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" Operador "))
                joins.Append(" left join T_FUNCIONARIO Operador ON Operador.FUN_CODIGO = Carga.CAR_OPERADOR");
        }

        private void SetarJoinsTabelaFrete(StringBuilder joins)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" TabelaFrete "))
                joins.Append(" left join T_TABELA_FRETE TabelaFrete ON TabelaFrete.TBF_CODIGO = Carga.TBF_CODIGO");
        }

        private void SetarJoinsTransportador(StringBuilder joins)
        {
            if (!joins.Contains(" TransportadorCTe "))
                joins.Append(" inner join T_EMPRESA TransportadorCTe on CTe.EMP_CODIGO = TransportadorCTe.EMP_CODIGO ");
        }

        private void SetarJoinsTransportadorConfiguracao(StringBuilder joins)
        {
            SetarJoinsTransportador(joins);

            if (!joins.Contains(" TransportadorConfiguracao "))
                joins.Append(" left join T_CONFIG TransportadorConfiguracao ON TransportadorConfiguracao.COF_CODIGO = TransportadorCTe.COF_CODIGO");
        }

        private void SetarJoinsTransportadorPai(StringBuilder joins)
        {
            SetarJoinsTransportador(joins);

            if (!joins.Contains(" TransportadorPaiCTe "))
                joins.Append(" left join T_EMPRESA TransportadorPaiCTe on TransportadorCTe.EMP_EMPRESA = TransportadorPaiCTe.EMP_CODIGO ");
        }

        private void SetarJoinsTransportadorPaiConfiguracao(StringBuilder joins)
        {
            SetarJoinsTransportadorPai(joins);

            if (!joins.Contains(" TransportadorPaiConfiguracao "))
                joins.Append(" left join T_CONFIG TransportadorPaiConfiguracao ON TransportadorPaiConfiguracao.COF_CODIGO = TransportadorPaiCTe.COF_CODIGO");
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, FiltroPesquisaRelatorioAuditoriaCTe filtrosPesquisa)
        {
            switch (propriedade)
            {
                case "MunicipioCobranca":
                    if (!select.Contains(" MunicipioCobranca, "))
                    {
                        select.Append("COALESCE(LocalidadeTomador.LOC_DESCRICAO, Tomador.PCT_CIDADE, '') MunicipioCobranca, ");

                        if (!groupBy.Contains("LocalidadeTomador.LOC_DESCRICAO"))
                            groupBy.Append("LocalidadeTomador.LOC_DESCRICAO, ");
                        if (!groupBy.Contains("Tomador.PCT_CIDADE"))
                            groupBy.Append("Tomador.PCT_CIDADE, ");

                        SetarJoinsLocalidadeTomador(joins);
                    }
                    break;

                case "DataEmissaoCTeFormatada":
                case "DataEmissaoCTe":
                case "AnoEmissaoCTe":
                case "MesEmissaoCTe":
                    if (!select.Contains(" DataEmissaoCTe, "))
                    {
                        select.Append("CTe.CON_DATAHORAEMISSAO DataEmissaoCTe, ");
                        groupBy.Append("CTe.CON_DATAHORAEMISSAO, ");
                    }
                    break;

                case "NumeroRomaneio":
                    if (!select.Contains(" NumeroRomaneio, "))
                    {
                        select.Append(
                            "SUBSTRING((SELECT ', ' +  " +
                            "   CARGA.CAR_CODIGO_CARGA_EMBARCADOR " +
                            "FROM T_CARGA_CTE CargaCTe " +
                            "   INNER JOIN T_CARGA Carga ON Carga.CAR_CODIGO = CargaCTe.CAR_CODIGO " +
                            "WHERE CargaCTe.CON_CODIGO = CTe.CON_CODIGO " +
                            "FOR XML PATH('')), 3, 1000) NumeroRomaneio, ");

                        if (!groupBy.Contains("CTe.CON_CODIGO,"))
                            groupBy.Append("CTe.CON_CODIGO, ");
                    }
                    break;

                case "KilometragemRota":
                    if (!select.Contains(" KilometragemRota,"))
                    {
                        select.Append(
                            @"(
                                select SUM(DadosSumarizados.CDS_DISTANCIA) 
                                  from T_CARGA_CTE CargaCTe 
                                  join T_CARGA Carga on CargaCTe.CAR_CODIGO = Carga.CAR_CODIGO 
                                  join T_CARGA_DADOS_SUMARIZADOS DadosSumarizados ON DadosSumarizados.CDS_CODIGO = Carga.CDS_CODIGO 
                                 where CargaCTe.CON_CODIGO = CTe.CON_CODIGO 
                            ) KilometragemRota, "
                        );

                        if (!groupBy.Contains("CTe.CON_CODIGO,"))
                            groupBy.Append("CTe.CON_CODIGO, ");
                    }
                    break;

                case "NumeroCTe":
                    if (!select.Contains(" NumeroCTe, "))
                    {
                        select.Append("CTe.CON_NUM NumeroCTe, ");
                        groupBy.Append("CTe.CON_NUM, ");
                    }
                    break;

                case "ChaveCTe":
                    if (!select.Contains(" ChaveCTe, "))
                    {
                        select.Append("CON_CHAVECTE ChaveCTe, ");
                        groupBy.Append("CON_CHAVECTE, ");
                    }
                    break;

                case "TotalReceberCTe":
                    if (!select.Contains(" TotalReceberCTe, "))
                    {
                        select.Append("CTe.CON_VALOR_RECEBER TotalReceberCTe, ");
                        groupBy.Append("CTe.CON_VALOR_RECEBER, ");
                    }
                    break;

                case "ValorTotalMercadoria":
                    if (!select.Contains(" ValorTotalMercadoria, "))
                    {
                        select.Append("CTe.CON_VALOR_TOTAL_MERC ValorTotalMercadoria, ");
                        groupBy.Append("CTe.CON_VALOR_TOTAL_MERC, ");
                    }
                    break;

                case "NotasFiscais":
                    if (!select.Contains(" NotasFiscais, "))
                    {
                        select.Append("SUBSTRING((SELECT '/ ' + CAST(cteDocs.NFC_NUMERO AS NVARCHAR(20)) FROM T_CTE_DOCS cteDocs WHERE cteDocs.CON_CODIGO = CTe.CON_CODIGO FOR XML PATH('')), 3, 1000) NotasFiscais, ");

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append("CTe.CON_CODIGO, ");
                    }
                    break;

                case "FreteUnitario":
                    if (!select.Contains(" FreteUnitario, "))
                    {
                        select.Append("CTe.CON_VALOR_FRETE FreteUnitario, ");

                        if (!groupBy.Contains("CTe.CON_VALOR_FRETE"))
                            groupBy.Append("CTe.CON_VALOR_FRETE, ");
                    }
                    break;

                case "PesoCarregado":
                    if (!select.Contains(" PesoCarregado, "))
                    {
                        select.Append("CTe.CON_PESO PesoCarregado, ");

                        if (!groupBy.Contains("CTe.CON_PESO"))
                            groupBy.Append("CTe.CON_PESO, ");
                    }
                    break;

                case "PesoFrete":
                    if (!select.Contains(" PesoFrete, "))
                    {
                        select.Append("CTe.CON_PESO PesoFrete, ");

                        if (!groupBy.Contains("CTe.CON_PESO"))
                            groupBy.Append("CTe.CON_PESO, ");
                    }
                    break;

                case "TabelaFreteCarga":
                    if (!select.Contains(" TabelaFreteCarga, "))
                    {
                        select.Append("TabelaFrete.TBF_DESCRICAO TabelaFreteCarga, ");
                        groupBy.Append("TabelaFrete.TBF_DESCRICAO, ");

                        SetarJoinsTabelaFrete(joins);
                    }
                    break;

                case "ValorFrete":
                    if (!select.Contains(" ValorFrete, "))
                    {
                        select.Append("CTe.CON_VALOR_FRETE ValorFrete, ");

                        if (!groupBy.Contains("CTe.CON_VALOR_FRETE"))
                            groupBy.Append("CTe.CON_VALOR_FRETE, ");
                    }
                    break;

                case "ValorPedagio":
                    if (!select.Contains(" ValorPedagio, "))
                    {
                        select.Append(
                            @"( 
		                        select SUM(CPT_VALOR) 
		                        FROM T_CTE_COMP_PREST ComponentePrestacaoCTe 
			                        INNER JOIN T_COMPONENTE_FRETE ComponenteFrete ON ComponenteFrete.CFR_CODIGO = ComponentePrestacaoCTe.CFR_CODIGO 
		                        WHERE ComponenteFrete.CFR_TIPO_COMPONENTE_FRETE = 2 AND ComponentePrestacaoCTe.CON_CODIGO = CTe.CON_CODIGO 
	                        ) ValorPedagio, ");

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append("CTe.CON_CODIGO, ");
                    }
                    break;

                case "ValorICMS":
                    if (!select.Contains(" ValorICSM, "))
                    {
                        select.Append("CTe.CON_VAL_ICMS ValorICMS, ");
                        groupBy.Append("CTe.CON_VAL_ICMS, ");
                    }
                    break;

                case "BaseCalculoICMS":
                    if (!somenteContarNumeroRegistros && !select.Contains(" BaseCalculoICMS, "))
                    {
                        select.Append("CTe.CON_BC_ICMS BaseCalculoICMS, ");
                        groupBy.Append("CTe.CON_BC_ICMS, ");
                    }
                    break;

                case "AliquotaCOFINS":
                    if (!select.Contains(" AliquotaCOFINS, "))
                    {
                        select.Append("coalesce(TransportadorConfiguracao.COF_ALIQUOTA_COFINS, TransportadorPaiConfiguracao.COF_ALIQUOTA_COFINS, 0) AliquotaCOFINS, ");

                        groupBy.Append("TransportadorConfiguracao.COF_ALIQUOTA_COFINS, ");
                        groupBy.Append("TransportadorPaiConfiguracao.COF_ALIQUOTA_COFINS, ");

                        SetarJoinsTransportadorConfiguracao(joins);
                        SetarJoinsTransportadorPaiConfiguracao(joins);
                    }
                    break;

                case "AliquotaPIS":
                    if (!select.Contains(" AliquotaPIS, "))
                    {
                        select.Append("coalesce(TransportadorConfiguracao.COF_ALIQUOTA_PIS, TransportadorPaiConfiguracao.COF_ALIQUOTA_PIS, 0) AliquotaPIS, ");

                        groupBy.Append("TransportadorConfiguracao.COF_ALIQUOTA_PIS, ");
                        groupBy.Append("TransportadorPaiConfiguracao.COF_ALIQUOTA_PIS, ");

                        SetarJoinsTransportadorConfiguracao(joins);
                        SetarJoinsTransportadorPaiConfiguracao(joins);
                    }
                    break;

                case "ValorPIS":
                    SetarSelect("AliquotaPIS", 0, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    SetarSelect("BaseCalculoICMS", 0, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    break;

                case "ValorCOFINS":
                    SetarSelect("AliquotaCOFINS", 0, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    SetarSelect("BaseCalculoICMS", 0, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    break;

                //case "ValorISS":
                //    if (!select.Contains(" ValorISS, "))
                //    {
                //        select.Append("CTe.CON_VALOR_ISS ValorISS, ");
                //        groupBy.Append("CTe.CON_VALOR_ISS, ");
                //    }
                //    break;

                case "Peso":
                    if (!select.Contains(" Peso, "))
                    {
                        select.Append("CTe.CON_PESO Peso, ");

                        if (!groupBy.Contains("CTe.CON_PESO"))
                            groupBy.Append("CTe.CON_PESO, ");
                    }
                    break;

                case "Mercadoria":
                    if (!select.Contains(" Mercadoria, "))
                    {
                        select.Append("CTe.CON_PRODUTO_PRED Mercadoria, ");
                        groupBy.Append("CTe.CON_PRODUTO_PRED, ");
                    }
                    break;

                case "PlacaTracao":
                    if (!select.Contains(" PlacaTracao,"))
                    {
                        select.Append(
                            @"substring(
		                        (
			                        select ', ' + VeiculoCTe.CVE_PLACA
			                        from T_CTE_VEICULO VeiculoCTe
			                        where VeiculoCTe.CON_CODIGO = CTe.CON_CODIGO and VeiculoCTe.CVE_TIPO_VEICULO = 0
			                        for xml path('')
		                        ), 3, 1000
	                        ) PlacaTracao, ");

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append("CTe.CON_CODIGO, ");
                    }
                    break;

                case "ModalidadePlacaTracao":
                    if (!select.Contains(" ModalidadePlacaTracao, "))
                    {
                        select.Append(
                            @"SUBSTRING( 
		                        (SELECT ', ' + ModeloVeicular.MVC_DESCRICAO 
			                    FROM T_CTE_VEICULO VeiculoCTe 
				                    INNER JOIN T_VEICULO Veiculo ON Veiculo.VEI_CODIGO = VeiculoCTe.VEI_CODIGO 
				                    INNER JOIN T_MODELO_VEICULAR_CARGA ModeloVeicular ON ModeloVeicular.MVC_CODIGO = Veiculo.MVC_CODIGO 
			                    WHERE VeiculoCTe.CON_CODIGO = CTe.CON_CODIGO AND VeiculoCTe.CVE_TIPO_VEICULO = 0 
			                    FOR XML PATH('') 
		                        ), 3, 1000 
	                        ) ModalidadePlacaTracao, ");

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append("CTe.CON_CODIGO, ");
                    }
                    break;

                case "Operacao":
                    if (!select.Contains(" Operacao, "))
                    {
                        select.Append(
                            "SUBSTRING((SELECT ', ' +  " +
                            "   TipoOperacao.TOP_DESCRICAO " +
                            "FROM T_CARGA_CTE CargaCTe " +
                            "   INNER JOIN T_CARGA Carga ON Carga.CAR_CODIGO = CargaCTe.CAR_CODIGO " +
                            "   INNER JOIN T_TIPO_OPERACAO TipoOperacao ON TipoOperacao.TOP_CODIGO = Carga.TOP_CODIGO " +
                            "WHERE CargaCTe.CON_CODIGO = CTe.CON_CODIGO " +
                            "FOR XML PATH('')), 3, 1000) Operacao, ");
                    }
                    break;

                case "RazaoSocialRemetente":
                    if (!select.Contains(" RazaoSocialRemetente, "))
                    {
                        select.Append("Remetente.PCT_NOME RazaoSocialRemetente, ");

                        if (!groupBy.Contains("Remetente.PCT_NOME"))
                            groupBy.Append("Remetente.PCT_NOME, ");

                        SetarJoinsRemetente(joins);
                    }
                    break;

                case "MunicipioRemetente":
                    if (!select.Contains(" MunicipioRemetente, "))
                    {
                        select.Append("COALESCE(LocalidadeRemetente.LOC_DESCRICAO, Remetente.PCT_CIDADE, '') MunicipioRemetente, ");

                        if (!groupBy.Contains("LocalidadeRemetente.LOC_DESCRICAO"))
                            groupBy.Append("LocalidadeRemetente.LOC_DESCRICAO, ");
                        if (!groupBy.Contains("Remetente.PCT_CIDADE"))
                            groupBy.Append("Remetente.PCT_CIDADE, ");

                        SetarJoinsLocalidadeRemetente(joins);
                    }
                    break;

                case "UFRemetente":
                    if (!select.Contains(" UFRemetente, "))
                    {
                        select.Append("LocalidadeRemetente.UF_SIGLA UFRemetente, ");

                        if (!groupBy.Contains("LocalidadeRemetente.UF_SIGLA"))
                            groupBy.Append("LocalidadeRemetente.UF_SIGLA, ");

                        SetarJoinsLocalidadeRemetente(joins);
                    }
                    break;

                case "RazaoSocialColeta":
                    if (!select.Contains(" RazaoSocialColeta, "))
                    {
                        select.Append("Expedidor.PCT_NOME RazaoSocialColeta, ");

                        if (!groupBy.Contains("Expedidor.PCT_NOME"))
                            groupBy.Append("Expedidor.PCT_NOME, ");

                        SetarJoinsExpedidor(joins);
                    }
                    break;

                case "UFColeta":
                    if (!select.Contains(" UFColeta, "))
                    {
                        select.Append("LocalidadeExpedidor.UF_SIGLA UFColeta, ");

                        if (!groupBy.Contains("LocalidadeExpedidor.UF_SIGLA"))
                            groupBy.Append("LocalidadeExpedidor.UF_SIGLA, ");

                        SetarJoinsLocalidadeExpedidor(joins);
                    }
                    break;

                case "RazaoSocialRedespacho":
                    if (!select.Contains(" RazaoSocialRedespacho, "))
                    {
                        select.Append("Recebedor.PCT_NOME RazaoSocialRedespacho, ");

                        if (!groupBy.Contains("Recebedor.PCT_NOME"))
                            groupBy.Append("Recebedor.PCT_NOME, ");

                        SetarJoinsRecebedor(joins);
                    }
                    break;

                case "MunicipioRedespacho":
                    if (!select.Contains(" MunicipioRedespacho, "))
                    {
                        select.Append("COALESCE(LocalidadeRecebedor.LOC_DESCRICAO, Recebedor.PCT_CIDADE, '') MunicipioRedespacho, ");

                        if (!groupBy.Contains("LocalidadeRecebedor.LOC_DESCRICAO"))
                            groupBy.Append("LocalidadeRecebedor.LOC_DESCRICAO, ");
                        if (!groupBy.Contains("Recebedor.PCT_CIDADE"))
                            groupBy.Append("Recebedor.PCT_CIDADE, ");

                        SetarJoinsLocalidadeRecebedor(joins);
                    }
                    break;

                case "UFRedespacho":
                    if (!select.Contains(" UFRedespacho, "))
                    {
                        select.Append("LocalidadeRecebedor.UF_SIGLA UFRedespacho, ");

                        if (!groupBy.Contains("LocalidadeRecebedor.UF_SIGLA"))
                            groupBy.Append("LocalidadeRecebedor.UF_SIGLA, ");

                        SetarJoinsLocalidadeRecebedor(joins);
                    }
                    break;

                case "RazaoSocialDestinatario":
                    if (!select.Contains(" RazaoSocialDestinatario, "))
                    {
                        select.Append("Destinatario.PCT_NOME RazaoSocialDestinatario, ");

                        if (!groupBy.Contains("Destinatario.PCT_NOME"))
                            groupBy.Append("Destinatario.PCT_NOME, ");

                        SetarJoinsDestinatario(joins);
                    }
                    break;

                case "MunicipioDestinatario":
                    if (!select.Contains(" MunicipioDestinatario, "))
                    {
                        select.Append("COALESCE(LocalidadeDestinatario.LOC_DESCRICAO, Destinatario.PCT_CIDADE, '') MunicipioDestinatario, ");

                        if (!groupBy.Contains("LocalidadeDestinatario.LOC_DESCRICAO"))
                            groupBy.Append("LocalidadeDestinatario.LOC_DESCRICAO, ");
                        if (!groupBy.Contains("Destinatario.PCT_CIDADE"))
                            groupBy.Append("Destinatario.PCT_CIDADE, ");

                        SetarJoinsLocalidadeDestinatario(joins);
                    }
                    break;

                case "UFDestinatario":
                    if (!select.Contains(" UFDestinatario, "))
                    {
                        select.Append("LocalidadeDestinatario.UF_SIGLA UFDestinatario, ");

                        if (!groupBy.Contains("LocalidadeDestinatario.UF_SIGLA"))
                            groupBy.Append("LocalidadeDestinatario.UF_SIGLA, ");

                        SetarJoinsLocalidadeDestinatario(joins);
                    }
                    break;

                case "Observacao":
                    if (!select.Contains(" Observacao, "))
                    {
                        select.Append("CTe.CON_OBSGERAIS Observacao, ");
                        groupBy.Append("CTe.CON_OBSGERAIS, ");
                    }
                    break;

                case "TipoConhecimento":
                    if (!select.Contains(" TipoConhecimento, "))
                    {
                        select.Append("CASE " +
                            "   WHEN CTe.CON_TIPO_CTE = 0 THEN 'Normal' " +
                            "   WHEN CTe.CON_TIPO_CTE = 1 THEN 'Complemento' " +
                            "   WHEN CTe.CON_TIPO_CTE = 2 THEN 'Anulação' " +
                            "   WHEN CTe.CON_TIPO_CTE = 3 THEN 'Substituto' " +
                            "   ELSE '' " +
                            "END TipoConhecimento, ");

                        groupBy.Append("CTe.CON_TIPO_CTE, ");
                    }
                    break;

                case "InseridoPor":
                    if (!select.Contains(" InseridoPor, "))
                    {
                        select.Append("Operador.FUN_NOME InseridoPor, ");
                        groupBy.Append("Operador.FUN_NOME, ");

                        SetarJoinsOperador(joins);
                    }
                    break;

                //case "PesoCobrado":
                //case "TotalImpostos":
                //    if (!select.Contains($" {propriedade}, "))
                //        select.Append($"0.00 {propriedade}, ");
                //    break;

                //case "SecCat":
                //case "DespachoCATITR":
                case "MercadoriaGrupo":
                case "EspecieDescricaoFrete":
                //case "TipoFrete":
                //case "DataEntrega":
                //case "TipoConhecimento":
                    if (!select.Contains($" {propriedade}, "))
                        select.Append($"'' {propriedade}, ");
                    break;
            }
        }

        protected override void SetarWhere(FiltroPesquisaRelatorioAuditoriaCTe filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            string pattern = "yyyy-MM-dd";

            if (filtrosPesquisa.DataEmissaoInicial != DateTime.MinValue)
                where.Append($" and CTe.CON_DATAHORAEMISSAO >= '{filtrosPesquisa.DataEmissaoInicial.ToString(pattern)}'");

            if (filtrosPesquisa.DataEmissaoFinal != DateTime.MinValue)
                where.Append($" and CTe.CON_DATAHORAEMISSAO < '{filtrosPesquisa.DataEmissaoFinal.AddDays(1).ToString(pattern)}'");

            if (filtrosPesquisa.NumeroDocumentoInicial > 0)
                where.Append($" and CTe.CON_NUM >= {filtrosPesquisa.NumeroDocumentoInicial}");

            if (filtrosPesquisa.NumeroDocumentoFinal > 0)
                where.Append($" and CTe.CON_NUM <= {filtrosPesquisa.NumeroDocumentoFinal}");

            if (filtrosPesquisa.CodigoCarga > 0)
                where.Append($" and CTe.CON_CODIGO in (select CargaCTe.CON_CODIGO from T_CARGA_CTE CargaCTe where CargaCTe.CAR_CODIGO_ORIGEM = {filtrosPesquisa.CodigoCarga} or CargaCTe.CAR_CODIGO = {filtrosPesquisa.CodigoCarga})"); // SQL-INJECTION-SAFE

            if (filtrosPesquisa.CpfCnpjTomador > 0d)
            {
                where.Append($" and ClienteTomador.CLI_CGCCPF = {filtrosPesquisa.CpfCnpjTomador.ToString("F0")}");

                SetarJoinsClienteTomador(joins);
            }

            if (filtrosPesquisa.codigoFilial > 0)
                where.Append($" and CTe.CON_CODIGO in (select CargaCTe.CON_CODIGO from T_CARGA_CTE CargaCTe inner join T_CARGA Carga on CargaCTe.CAR_CODIGO_ORIGEM = Carga.CAR_CODIGO where Carga.FIL_CODIGO = {filtrosPesquisa.codigoFilial})"); // SQL-INJECTION-SAFE

            if (filtrosPesquisa.grupoPessoas > 0)
            {
                where.Append($" and ClienteTomador.GRP_CODIGO = {filtrosPesquisa.grupoPessoas}");
                
                SetarJoinsClienteTomador(joins);
            }

            if (filtrosPesquisa.statusCTe != null && filtrosPesquisa.statusCTe.Count > 0)
            {
                if (filtrosPesquisa.statusCTe.Count == 1)
                    where.Append(" and CTe.CON_STATUS = '" + filtrosPesquisa.statusCTe[0] + "'");
                else
                    where.Append(" and CTe.CON_STATUS in ('" + string.Join("', '", filtrosPesquisa.statusCTe) + "')");
            }

            if (filtrosPesquisa.modeloDocumento > 0)
                where.Append($" and CTe.CON_MODELODOC = {filtrosPesquisa.modeloDocumento}");

            if (filtrosPesquisa.codigoTransportador > 0)
                where.Append($" and CTe.EMP_CODIGO = {filtrosPesquisa.codigoTransportador}");
        }

        #endregion
    }
}
