using Dominio.ObjetosDeValor.Embarcador.Financeiro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Financeiro
{
    sealed class ConsultaConferenciaFiscal : Embarcador.Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioConferenciaFiscal>
    {
        #region Construtor

        public ConsultaConferenciaFiscal() : base(tabela: "T_CIOT as CIOT") { }

        #endregion

        #region Métodos Privados

        public void SetarJoinsContratante(StringBuilder joins)
        {
            if (!joins.Contains(" Contratante "))
                joins.Append(" JOIN T_EMPRESA Contratante on Contratante.EMP_CODIGO = CIOT.EMP_CODIGO ");
        }
        
        public void SetarJoinsContratado(StringBuilder joins)
        {
            if (!joins.Contains(" Contratado "))
                joins.Append(" JOIN T_CLIENTE Contratado on Contratado.CLI_CGCCPF = CIOT.CLI_CGCCPF ");
        }
        
        public void SetarJoinsLocalidadeContratado(StringBuilder joins)
        {
            SetarJoinsContratado(joins);

            if (!joins.Contains(" LocalidadeContratado "))
                joins.Append(" JOIN T_LOCALIDADES LocalidadeContratado on LocalidadeContratado.LOC_CODIGO = Contratado.LOC_CODIGO ");
        }
        
        public void SetarJoinsCIOTCTe(StringBuilder joins)
        {
            if (!joins.Contains(" CIOTCTe "))
                joins.Append(" JOIN T_CIOT_CTE CIOTCTe on CIOTCTe.CIO_CODIGO = CIOT.CIO_CODIGO ");
        }

        public void SetarJoinsCargaCTe(StringBuilder joins)
        {
            SetarJoinsCIOTCTe(joins);

            if (!joins.Contains(" CargaCTe "))
                joins.Append(" JOIN T_CARGA_CTE CargaCTe on CargaCTe.CCT_CODIGO = CIOTCTe.CCT_CODIGO ");
        }
        
        public void SetarJoinsCarga(StringBuilder joins)
        {
            SetarJoinsCargaCTe(joins);

            if (!joins.Contains(" Carga "))
                joins.Append(" JOIN T_CARGA Carga on Carga.CAR_CODIGO = CargaCTe.CAR_CODIGO ");
        }

        public void SetarJoinsContrato(StringBuilder joins)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" Contrato "))
                joins.Append(" JOIN T_CONTRATO_FRETE_TERCEIRO Contrato on Contrato.CAR_CODIGO = Carga.CAR_CODIGO ");
        }
        
        public void SetarJoinsCTe(StringBuilder joins)
        {
            SetarJoinsCargaCTe(joins);

            if (!joins.Contains(" CTe "))
                joins.Append(" JOIN T_CTE CTe on CTe.CON_CODIGO = CargaCTe.CON_CODIGO ");
        }
        
        public void SetarJoinsSerie(StringBuilder joins)
        {
            SetarJoinsCTe(joins);

            if (!joins.Contains(" Serie "))
                joins.Append(" JOIN T_EMPRESA_SERIE Serie on Serie.ESE_CODIGO = CTe.CON_SERIE ");
        }

        public void SetarJoinsCFOP(StringBuilder joins)
        {
            SetarJoinsCTe(joins);

            if (!joins.Contains(" CFOP "))
                joins.Append(" JOIN T_CFOP CFOP on CFOP.CFO_CODIGO = CTe.CFO_CODIGO ");
        }

        public void SetarJoinsEmitente(StringBuilder joins)
        {
            SetarJoinsCTe(joins);

            if (!joins.Contains(" Emitente "))
                joins.Append(" JOIN T_EMPRESA Emitente on Emitente.EMP_CODIGO = CTe.EMP_CODIGO ");
        }

        public void SetarJoinsLocalidadeEmitente(StringBuilder joins)
        {
            SetarJoinsEmitente(joins);

            if (!joins.Contains(" LocalidadeEmitente "))
                joins.Append(" JOIN T_LOCALIDADES LocalidadeEmitente on LocalidadeEmitente.LOC_CODIGO = Emitente.LOC_CODIGO ");
        }

        public void SetarJoinsTomador(StringBuilder joins)
        {
            SetarJoinsCTe(joins);

            if (!joins.Contains(" Tomador "))
                joins.Append(" JOIN T_CTE_PARTICIPANTE Tomador on Tomador.PCT_CODIGO = CTe.CON_TOMADOR_PAGADOR_CTE ");
        }

        public void SetarJoinsLocalidadeTomador(StringBuilder joins)
        {
            SetarJoinsTomador(joins);

            if (!joins.Contains(" LocalidadeTomador "))
                joins.Append(" JOIN T_LOCALIDADES LocalidadeTomador on LocalidadeTomador.LOC_CODIGO = Tomador.LOC_CODIGO ");
        }

        public void SetarJoinsRemetente(StringBuilder joins)
        {
            SetarJoinsCTe(joins);

            if (!joins.Contains(" Remetente "))
                joins.Append(" JOIN T_CTE_PARTICIPANTE Remetente on Remetente.PCT_CODIGO = CTe.CON_REMETENTE_CTE ");
        }

        public void SetarJoinsLocalidadeRemetente(StringBuilder joins)
        {
            SetarJoinsRemetente(joins);

            if (!joins.Contains(" LocalidadeRemetente "))
                joins.Append(" JOIN T_LOCALIDADES LocalidadeRemetente on LocalidadeRemetente.LOC_CODIGO = Remetente.LOC_CODIGO ");
        }

        public void SetarJoinsDestinatario(StringBuilder joins)
        {
            SetarJoinsCTe(joins);

            if (!joins.Contains(" Destinatario "))
                joins.Append(" JOIN T_CTE_PARTICIPANTE Destinatario on Destinatario.PCT_CODIGO = CTe.CON_DESTINATARIO_CTE ");
        }

        public void SetarJoinsLocalidadeDestinatario(StringBuilder joins)
        {
            SetarJoinsDestinatario(joins);

            if (!joins.Contains(" LocalidadeDestinatario "))
                joins.Append(" JOIN T_LOCALIDADES LocalidadeDestinatario on LocalidadeDestinatario.LOC_CODIGO = Destinatario.LOC_CODIGO ");
        }

        public void SetarJoinsExpedidor(StringBuilder joins)
        {
            SetarJoinsCTe(joins);

            if (!joins.Contains(" Expedidor "))
                joins.Append(" LEFT OUTER JOIN T_CTE_PARTICIPANTE Expedidor on Expedidor.PCT_CODIGO = CTe.CON_EXPEDIDOR_CTE ");
        }

        public void SetarJoinsLocalidadeExpedidor(StringBuilder joins)
        {
            SetarJoinsExpedidor(joins);

            if (!joins.Contains(" LocalidadeExpedidor "))
                joins.Append(" LEFT OUTER JOIN T_LOCALIDADES LocalidadeExpedidor on LocalidadeExpedidor.LOC_CODIGO = Expedidor.LOC_CODIGO ");
        }

        public void SetarJoinsRecebedor(StringBuilder joins)
        {
            SetarJoinsCTe(joins);

            if (!joins.Contains(" Recebedor "))
                joins.Append(" LEFT OUTER JOIN T_CTE_PARTICIPANTE Recebedor on Recebedor.PCT_CODIGO = CTe.CON_RECEBEDOR_CTE ");
        }

        public void SetarJoinsLocalidadeRecebedor(StringBuilder joins)
        {
            SetarJoinsRecebedor(joins);

            if (!joins.Contains(" LocalidadeRecebedor "))
                joins.Append(" LEFT OUTER JOIN T_LOCALIDADES LocalidadeRecebedor on LocalidadeRecebedor.LOC_CODIGO = Recebedor.LOC_CODIGO ");
        }

        public void SetarJoinsInicio(StringBuilder joins)
        {
            SetarJoinsCTe(joins);

            if (!joins.Contains(" Inicio "))
                joins.Append(" JOIN T_LOCALIDADES Inicio on Inicio.LOC_CODIGO = CTe.CON_LOCINICIOPRESTACAO ");
        }

        public void SetarJoinsFim(StringBuilder joins)
        {
            SetarJoinsCTe(joins);

            if (!joins.Contains(" Fim "))
                joins.Append(" JOIN T_LOCALIDADES Fim on Fim.LOC_CODIGO = CTe.CON_LOCTERMINOPRESTACAO ");
        }

        public void SetarJoinsInfCTe(StringBuilder joins)
        {
            SetarJoinsCTe(joins);

            if (!joins.Contains(" InfCTe "))
                joins.Append(" LEFT OUTER JOIN T_CTE_INF_CARGA InfCTe on InfCTe.CON_CODIGO = CTe.CON_CODIGO and InfCTe.ICA_UN = '01' ");
        }

        public void SetarJoinsCTeNotas(StringBuilder joins)
        {
            SetarJoinsCTe(joins);

            if (!joins.Contains(" CTeNotas "))
                joins.Append(" JOIN T_CTE_XML_NOTAS_FISCAIS CTeNotas on CTeNotas.CON_CODIGO = CTe.CON_CODIGO ");
        }

        public void SetarJoinsNota(StringBuilder joins)
        {
            SetarJoinsCTeNotas(joins);

            if (!joins.Contains(" Nota "))
                joins.Append(" JOIN T_XML_NOTA_FISCAL Nota on Nota.NFX_CODIGO = CTeNotas.NFX_CODIGO ");
        }

        public void SetarJoinsEmitenteNota(StringBuilder joins)
        {
            SetarJoinsNota(joins);

            if (!joins.Contains(" EmitenteNota "))
                joins.Append(" JOIN T_CLIENTE EmitenteNota on EmitenteNota.CLI_CGCCPF = Nota.CLI_CODIGO_REMETENTE ");
        }
        
        public void SetarJoinsDestinatarioNota(StringBuilder joins)
        {
            SetarJoinsNota(joins);

            if (!joins.Contains(" DestinatarioNota "))
                joins.Append(" JOIN T_CLIENTE DestinatarioNota on DestinatarioNota.CLI_CGCCPF = Nota.CLI_CODIGO_DESTINATARIO ");
        }
        
        public void SetarJoinsVeiculo(StringBuilder joins)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" Veiculo "))
                joins.Append(" left outer join T_VEICULO Veiculo on Veiculo.VEI_CODIGO = Carga.CAR_VEICULO ");
        }

        public void SetarJoinsModeloVeicular(StringBuilder joins)
        {
            SetarJoinsVeiculo(joins);

            if (!joins.Contains(" ModeloVeicular "))
                joins.Append(" left outer join T_MODELO_VEICULAR_CARGA ModeloVeicular on ModeloVeicular.MVC_CODIGO = Veiculo.MVC_CODIGO ");
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, FiltroPesquisaRelatorioConferenciaFiscal filtrosPesquisa)
        {
            switch (propriedade)
            {
                case "DataEmissaoCIOT":
                case "DataEmissaoCIOTFormatada":
                    if (!select.Contains(" DataEmissaoCIOT, "))
                    {
                        select.Append("CIOT.CIO_DATA_ABERTURA DataEmissaoCIOT, ");
                        groupBy.Append("CIOT.CIO_DATA_ABERTURA, ");
                    }
                    break;

                case "NumeroCIOT":
                    if (!select.Contains(" NumeroCIOT, "))
                    {
                        select.Append("CIOT.CIO_NUMERO NumeroCIOT, ");
                        groupBy.Append("CIOT.CIO_NUMERO, ");
                    }
                    break;

                case "Contratante":
                    if (!select.Contains(" Contratante, "))
                    {
                        select.Append("Contratante.EMP_RAZAO Contratante, ");
                        groupBy.Append("Contratante.EMP_RAZAO, ");

                        SetarJoinsContratante(joins);
                    }
                    break;

                case "CNPJContratante":
                case "CNPJContratanteFormatado":
                    if (!select.Contains(" CNPJContratante, "))
                    {
                        select.Append("Contratante.EMP_CNPJ CNPJContratante, ");
                        groupBy.Append("Contratante.EMP_CNPJ, ");

                        SetarJoinsContratante(joins);
                    }
                    break;

                case "Contratado":
                    if (!select.Contains(" Contratado, "))
                    {
                        select.Append("Contratado.CLI_NOME Contratado, ");
                        groupBy.Append("Contratado.CLI_NOME, ");

                        SetarJoinsContratado(joins);
                    }
                    break;

                case "TipoContratado":
                    if (!select.Contains(" TipoContratado, "))
                    {
                        select.Append("Contratado.CLI_FISJUR TipoContratado, ");
                        groupBy.Append("Contratado.CLI_FISJUR, ");

                        SetarJoinsContratado(joins);
                    }
                    break;

                case "CPFCNPJContratado":
                case "CPFCNPJContratadoFormatado":
                    SetarSelect("TipoContratado", 0, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    if (!select.Contains(" CPFCNPJContratado, "))
                    {
                        select.Append("Contratado.CLI_CGCCPF CPFCNPJContratado, ");
                        groupBy.Append("Contratado.CLI_CGCCPF, ");

                        SetarJoinsContratado(joins);
                    }
                    break;

                case "RegimeTributarioTerceiro":
                case "RegimeTributarioTerceiroFormatado":
                    if (!select.Contains(" RegimeTributarioTerceiro, "))
                    {
                        select.Append("Contratado.CLI_REGIME_TRIBUTARIO RegimeTributarioTerceiro, ");
                        groupBy.Append("Contratado.CLI_REGIME_TRIBUTARIO, ");

                        SetarJoinsContratado(joins);
                    }
                    break;

                case "UFContratado":
                    if (!select.Contains(" UFContratado, "))
                    {
                        select.Append("LocalidadeContratado.UF_SIGLA UFContratado, ");
                        groupBy.Append("LocalidadeContratado.UF_SIGLA, ");

                        SetarJoinsLocalidadeContratado(joins);
                    }
                    break;

                case "MunicipioContratado":
                    if (!select.Contains(" MunicipioContratado, "))
                    {
                        select.Append("LocalidadeContratado.LOC_DESCRICAO MunicipioContratado, ");
                        groupBy.Append("LocalidadeContratado.LOC_DESCRICAO, ");

                        SetarJoinsLocalidadeContratado(joins);
                    }
                    break;

                case "ValorFreteContratado":
                    if (!select.Contains(" ValorFreteContratado, "))
                    {
                        select.Append("Contrato.CFT_VALOR_FRETE_SUB_CONTRATACAO ValorFreteContratado, ");
                        groupBy.Append("Contrato.CFT_VALOR_FRETE_SUB_CONTRATACAO, ");

                        SetarJoinsContrato(joins);
                    }
                    break;

                case "AliquotaPIS":
                    if (!select.Contains(" AliquotaPIS, "))
                    {
                        select.Append("Contrato.CFT_ALIQUOTA_PIS AliquotaPIS, ");
                        
                        if (!groupBy.Contains("Contrato.CFT_ALIQUOTA_PIS"))
                            groupBy.Append("Contrato.CFT_ALIQUOTA_PIS, ");

                        SetarJoinsContratado(joins);
                    }
                    break;

                case "AliquotaCOFINS":
                    if (!select.Contains(" AliquotaCOFINS, "))
                    {
                        select.Append("Contrato.CFT_ALIQUOTA_COFINS AliquotaCOFINS, ");

                        if (!groupBy.Contains("Contrato.CFT_ALIQUOTA_COFINS"))
                            groupBy.Append("Contrato.CFT_ALIQUOTA_COFINS, ");

                        SetarJoinsContratado(joins);
                    }
                    break;

                case "ValorPIS":
                    SetarSelect("AliquotaPIS", 0, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    SetarSelect("ValorFreteContratado", 0, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    break;

                case "ValorCOFINS":
                    SetarSelect("AliquotaCOFINS", 0, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    SetarSelect("ValorFreteContratado", 0, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    break;

                case "DataEmissaoCTe":
                case "DataEmissaoCTeFormatada":
                    if (!select.Contains(" DataEmissaoCTe, "))
                    {
                        select.Append("CTe.CON_DATAHORAEMISSAO DataEmissaoCTe, ");
                        groupBy.Append("CTe.CON_DATAHORAEMISSAO, ");

                        SetarJoinsCTe(joins);
                    }
                    break;

                case "ChaveCTe":
                    if (!select.Contains(" ChaveCTe, "))
                    {
                        select.Append("CTe.CON_CHAVECTE ChaveCTe, ");

                        if (!groupBy.Contains("CTe.CON_CHAVECTE"))
                            groupBy.Append("CTe.CON_CHAVECTE, ");

                        SetarJoinsCTe(joins);
                    }
                    break;

                case "NumeroCTe":
                    if (!select.Contains(" NumeroCTe, "))
                    {
                        select.Append("CTe.CON_NUM NumeroCTe, ");
                        groupBy.Append("CTe.CON_NUM, ");

                        SetarJoinsCTe(joins);
                    }
                    break;

                case "SerieCTe":
                    if (!select.Contains(" SerieCTe, "))
                    {
                        select.Append("Serie.ESE_NUMERO SerieCTe, ");
                        groupBy.Append("Serie.ESE_NUMERO, ");

                        SetarJoinsSerie(joins);
                    }
                    break;

                case "Finalidade":
                case "FinalidadeFormatada":
                    if (!select.Contains(" Finalidade, "))
                    {
                        select.Append("CTe.CON_TIPO_CTE Finalidade, ");

                        if (!groupBy.Contains("CTe.CON_TIPO_CTE"))
                            groupBy.Append("CTe.CON_TIPO_CTE, ");

                        SetarJoinsCTe(joins);
                    }
                    break;

                case "Forma":
                case "FormaFormatada":
                    if (!select.Contains(" Forma, "))
                    {
                        select.Append("CTe.CON_TIPO_CTE Forma, ");

                        if (!groupBy.Contains("CTe.CON_TIPO_CTE"))
                            groupBy.Append("CTe.CON_TIPO_CTE, ");

                        SetarJoinsCTe(joins);
                    }
                    break;

                case "CFOPCTe":
                    if (!select.Contains(" CFOPCTe, "))
                    {
                        select.Append("CFOP.CFO_CFOP CFOPCTe, ");
                        groupBy.Append("CFOP.CFO_CFOP, ");

                        SetarJoinsCFOP(joins);
                    }
                    break;

                case "StatusCTe":
                case "DescricaoStatusCTe":
                    if (!select.Contains(" StatusCTe, "))
                    {
                        select.Append("CTe.CON_STATUS StatusCTe, ");
                        groupBy.Append("CTe.CON_STATUS, ");

                        SetarJoinsCTe(joins);
                    }
                    break;

                case "DataCancelamentoCTe":
                case "DataCancelamentoCTeFormatada":
                    if (!select.Contains(" DataCancelamentoCTe, "))
                    {
                        select.Append("CTe.CON_DATA_CANCELAMENTO DataCancelamentoCTe, ");
                        groupBy.Append("CTe.CON_DATA_CANCELAMENTO, ");

                        SetarJoinsCTe(joins);
                    }
                    break;


                case "ValorTotalCTe":
                    if (!select.Contains(" ValorTotalCTe, "))
                    {
                        select.Append("CTe.CON_VALOR_RECEBER ValorTotalCTe, ");
                        groupBy.Append("CTe.CON_VALOR_RECEBER, ");

                        SetarJoinsCTe(joins);
                    }
                    break;

                case "BaseICMS":
                    if (!select.Contains(" BaseICMS, "))
                    {
                        select.Append("CTe.CON_BC_ICMS BaseICMS, ");
                        groupBy.Append("CTe.CON_BC_ICMS, ");

                        SetarJoinsCTe(joins);
                    }
                    break;

                case "AliquotaICMS":
                    if (!select.Contains(" AliquotaICMS, "))
                    {
                        select.Append("CTe.CON_ALIQ_ICMS AliquotaICMS, ");
                        groupBy.Append("CTe.CON_ALIQ_ICMS, ");

                        SetarJoinsCTe(joins);
                    }
                    break;

                case "ValorICMS":
                    if (!select.Contains(" ValorICMS, "))
                    {
                        select.Append("CTe.CON_VAL_ICMS ValorICMS, ");
                        groupBy.Append("CTe.CON_VAL_ICMS, ");

                        SetarJoinsCTe(joins);
                    }
                    break;

                case "CSTICMS":
                    if (!select.Contains(" CSTICMS, "))
                    {
                        select.Append("CTe.CON_CST CSTICMS, ");
                        groupBy.Append("CTe.CON_CST, ");

                        SetarJoinsCTe(joins);
                    }
                    break;

                case "ChaveCTeComplementar":
                    if (!select.Contains(" ChaveCTeComplementar, "))
                    {
                        select.Append("(SELECT top(1) CTeComp.CON_CHAVECTE FROM T_CTE CTeComp where CTeComp.CON_CHAVE_CTE_SUB_COMP = CTe.CON_CHAVECTE ORDER BY CTeComp.CON_CODIGO DESC) ChaveCTeComplementar, ");

                        if (!groupBy.Contains("CTe.CON_CHAVECTE"))
                            groupBy.Append("CTe.CON_CHAVECTE, ");

                        SetarJoinsCTe(joins);
                    }
                    break;

                case "Emitente":
                    if (!select.Contains(" Emitente, "))
                    {
                        select.Append("Emitente.EMP_RAZAO Emitente, ");
                        groupBy.Append("Emitente.EMP_RAZAO, ");

                        SetarJoinsEmitente(joins);
                    }
                    break;

                case "CPFCNPJEmitente":
                case "CPFCNPJEmitenteFormatado":
                    if (!select.Contains(" CPFCNPJEmitente, "))
                    {
                        select.Append("Emitente.EMP_CNPJ CPFCNPJEmitente, ");
                        groupBy.Append("Emitente.EMP_CNPJ, ");

                        SetarJoinsEmitente(joins);
                    }
                    break;

                case "UFEmitente":
                    if (!select.Contains(" UFEmitente, "))
                    {
                        select.Append("LocalidadeEmitente.UF_SIGLA UFEmitente, ");
                        groupBy.Append("LocalidadeEmitente.UF_SIGLA, ");

                        SetarJoinsLocalidadeEmitente(joins);
                    }
                    break;

                case "Tomador":
                    if (!select.Contains(" Tomador, "))
                    {
                        select.Append("Tomador.PCT_NOME Tomador, ");
                        groupBy.Append("Tomador.PCT_NOME, ");

                        SetarJoinsTomador(joins);
                    }
                    break;

                case "CPFCNPJTomador":
                case "CPFCNPJTomadorFormatado":
                    if (!select.Contains(" CPFCNPJTomador, "))
                    {
                        select.Append("Tomador.PCT_CPF_CNPJ CPFCNPJTomador, ");
                        groupBy.Append("Tomador.PCT_CPF_CNPJ, ");

                        SetarJoinsTomador(joins);
                    }
                    break;

                case "UFTomador":
                    if (!select.Contains(" UFTomador, "))
                    {
                        select.Append("LocalidadeTomador.UF_SIGLA UFTomador, ");
                        groupBy.Append("LocalidadeTomador.UF_SIGLA, ");

                        SetarJoinsLocalidadeTomador(joins);
                    }
                    break;

                case "Remetente":
                    if (!select.Contains(" Remetente, "))
                    {
                        select.Append("Remetente.PCT_NOME Remetente, ");
                        groupBy.Append("Remetente.PCT_NOME, ");

                        SetarJoinsRemetente(joins);
                    }
                    break;

                case "CPFCNPJRemetente":
                case "CPFCNPJRemetenteFormatado":
                    if (!select.Contains(" CPFCNPJRemetente, "))
                    {
                        select.Append("Remetente.PCT_CPF_CNPJ CPFCNPJRemetente, ");
                        groupBy.Append("Remetente.PCT_CPF_CNPJ, ");

                        SetarJoinsRemetente(joins);
                    }
                    break;

                case "UFRemetente":
                    if (!select.Contains(" UFRemetente, "))
                    {
                        select.Append("LocalidadeRemetente.UF_SIGLA UFRemetente, ");
                        groupBy.Append("LocalidadeRemetente.UF_SIGLA, ");

                        SetarJoinsLocalidadeRemetente(joins);
                    }
                    break;

                case "Expedidor":
                    if (!select.Contains(" Expedidor, "))
                    {
                        select.Append("Expedidor.PCT_NOME Expedidor, ");
                        groupBy.Append("Expedidor.PCT_NOME, ");

                        SetarJoinsExpedidor(joins);
                    }
                    break;

                case "CPFCNPJExpedidor":
                case "CPFCNPJExpedidorFormatado":
                    if (!select.Contains(" CPFCNPJExpedidor, "))
                    {
                        select.Append("Expedidor.PCT_CPF_CNPJ CPFCNPJExpedidor, ");
                        groupBy.Append("Expedidor.PCT_CPF_CNPJ, ");

                        SetarJoinsExpedidor(joins);
                    }
                    break;

                case "UFExpedidor":
                    if (!select.Contains(" UFExpedidor, "))
                    {
                        select.Append("LocalidadeExpedidor.UF_SIGLA UFExpedidor, ");
                        groupBy.Append("LocalidadeExpedidor.UF_SIGLA, ");

                        SetarJoinsLocalidadeExpedidor(joins);
                    }
                    break;

                case "Destinatario":
                    if (!select.Contains(" Destinatario, "))
                    {
                        select.Append("Destinatario.PCT_NOME Destinatario, ");
                        groupBy.Append("Destinatario.PCT_NOME, ");

                        SetarJoinsDestinatario(joins);
                    }
                    break;

                case "CPFCNPJDestinatario":
                case "CPFCNPJDestinatarioFormatado":
                    if (!select.Contains(" CPFCNPJDestinatario, "))
                    {
                        select.Append("Destinatario.PCT_CPF_CNPJ CPFCNPJDestinatario, ");
                        groupBy.Append("Destinatario.PCT_CPF_CNPJ, ");

                        SetarJoinsDestinatario(joins);
                    }
                    break;

                case "UFDestinatario":
                    if (!select.Contains(" UFDestinatario, "))
                    {
                        select.Append("LocalidadeDestinatario.UF_SIGLA UFDestinatario, ");
                        groupBy.Append("LocalidadeDestinatario.UF_SIGLA, ");

                        SetarJoinsLocalidadeDestinatario(joins);
                    }
                    break;

                case "Recebedor":
                    if (!select.Contains(" Recebedor, "))
                    {
                        select.Append("Recebedor.PCT_NOME Recebedor, ");
                        groupBy.Append("Recebedor.PCT_NOME, ");

                        SetarJoinsRecebedor(joins);
                    }
                    break;

                case "CPFCNPJRecebedor":
                case "CPFCNPJRecebedorFormatado":
                    if (!select.Contains(" CPFCNPJRecebedor, "))
                    {
                        select.Append("Recebedor.PCT_CPF_CNPJ CPFCNPJRecebedor, ");
                        groupBy.Append("Recebedor.PCT_CPF_CNPJ, ");

                        SetarJoinsRecebedor(joins);
                    }
                    break;

                case "UFRecebedor":
                    if (!select.Contains(" UFRecebedor, "))
                    {
                        select.Append("LocalidadeRecebedor.UF_SIGLA UFRecebedor, ");
                        groupBy.Append("LocalidadeRecebedor.UF_SIGLA, ");

                        SetarJoinsLocalidadeRecebedor(joins);
                    }
                    break;

                case "UFInicioPrestacao":
                    if (!select.Contains(" UFInicioPrestacao, "))
                    {
                        select.Append("Inicio.UF_SIGLA UFInicioPrestacao, ");
                        groupBy.Append("Inicio.UF_SIGLA, ");

                        SetarJoinsInicio(joins);
                    }
                    break;

                case "UFFimPrestacao":
                    if (!select.Contains(" UFFimPrestacao, "))
                    {
                        select.Append("Fim.UF_SIGLA UFFimPrestacao, ");
                        groupBy.Append("Fim.UF_SIGLA, ");

                        SetarJoinsFim(joins);
                    }
                    break;

                case "MunicipioInicioPrestacao":
                    if (!select.Contains(" MunicipioInicioPrestacao, "))
                    {
                        select.Append("Inicio.LOC_DESCRICAO MunicipioInicioPrestacao, ");
                        groupBy.Append("Inicio.LOC_DESCRICAO, ");

                        SetarJoinsInicio(joins);
                    }
                    break;

                case "MunicipioFimPrestacao":
                    if (!select.Contains(" MunicipioFimPrestacao, "))
                    {
                        select.Append("Fim.LOC_DESCRICAO MunicipioFimPrestacao, ");
                        groupBy.Append("Fim.LOC_DESCRICAO, ");

                        SetarJoinsFim(joins);
                    }
                    break;

                case "ProdutoPredominante":
                    if (!select.Contains(" ProdutoPredominante, "))
                    {
                        select.Append("CTe.CON_PRODUTO_PRED ProdutoPredominante, ");
                        groupBy.Append("CTe.CON_PRODUTO_PRED, ");

                        SetarJoinsCTe(joins);
                    }
                    break;

                case "Quantidade":
                    if (!select.Contains(" Quantidade, "))
                    {
                        select.Append("InfCTe.ICA_QTD Quantidade, ");
                        groupBy.Append("InfCTe.ICA_QTD, ");

                        SetarJoinsInfCTe(joins);
                    }
                    break;

                case "UnidadeMedida":
                case "DescricaoUnidadeMedida":
                    if (!select.Contains(" UnidadeMedida, "))
                    {
                        select.Append("InfCTe.ICA_TIPO UnidadeMedida, ");
                        groupBy.Append("InfCTe.ICA_TIPO, ");

                        SetarJoinsInfCTe(joins);
                    }
                    break;

                case "ChaveNFeReferenciada":
                    if (!select.Contains(" ChaveNFeReferenciada, "))
                    {
                        select.Append("Nota.NF_CHAVE ChaveNFeReferenciada, ");
                        groupBy.Append("Nota.NF_CHAVE, ");

                        SetarJoinsNota(joins);
                    }
                    break;

                case "NumeroNFeReferenciada":
                    if (!select.Contains(" NumeroNFeReferenciada, "))
                    {
                        select.Append("Nota.NF_NUMERO NumeroNFeReferenciada, ");
                        groupBy.Append("Nota.NF_NUMERO, ");

                        SetarJoinsNota(joins);
                    }
                    break;

                case "DataEmissaoNFe":
                case "DataEmissaoNFeFormatada":
                    if (!select.Contains(" DataEmissaoNFe, "))
                    {
                        select.Append("Nota.NF_DATA_EMISSAO DataEmissaoNFe, ");
                        groupBy.Append("Nota.NF_DATA_EMISSAO, ");

                        SetarJoinsNota(joins);
                    }
                    break;

                case "ValorNFe":
                    if (!select.Contains(" ValorNFe, "))
                    {
                        select.Append("Nota.NF_VALOR ValorNFe, ");
                        groupBy.Append("Nota.NF_VALOR, ");

                        SetarJoinsNota(joins);
                    }
                    break;

                case "CFOPNFe":
                    if (!select.Contains(" CFOPNFe, "))
                    {
                        select.Append("Nota.NF_CFOP CFOPNFe, ");
                        groupBy.Append("Nota.NF_CFOP, ");

                        SetarJoinsNota(joins);
                    }
                    break;

                case "BaseICMSNFe":
                    if (!select.Contains(" BaseICMSNFe, "))
                    {
                        select.Append("Nota.NF_BASE_CALCULO_ICMS BaseICMSNFe, ");
                        groupBy.Append("Nota.NF_BASE_CALCULO_ICMS, ");

                        SetarJoinsNota(joins);
                    }
                    break;

                case "ValorICMSNFe":
                    if (!select.Contains(" ValorICMSNFe, "))
                    {
                        select.Append("Nota.NF_VALOR_ICMS ValorICMSNFe, ");
                        groupBy.Append("Nota.NF_VALOR_ICMS, ");

                        SetarJoinsNota(joins);
                    }
                    break;

                case "AliquotaICMSNFe":
                    SetarSelect("BaseICMSNFe", 0, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    SetarSelect("ValorICMSNFe", 0, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    break;

                case "EmitenteNota":
                    if (!select.Contains(" EmitenteNota, "))
                    {
                        select.Append("EmitenteNota.CLI_NOME EmitenteNota, ");
                        groupBy.Append("EmitenteNota.CLI_NOME, ");

                        SetarJoinsEmitenteNota(joins);
                    }
                    break;

                case "TipoEmitenteNota":
                    if (!select.Contains(" TipoEmitenteNota, "))
                    {
                        select.Append("EmitenteNota.CLI_FISJUR TipoEmitenteNota, ");
                        groupBy.Append("EmitenteNota.CLI_FISJUR, ");

                        SetarJoinsEmitenteNota(joins);
                    }
                    break;

                case "CPFCNPJEmitenteNota":
                case "CPFCNPJEmitenteNotaFormatado":
                    SetarSelect("TipoEmitenteNota", 0, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    if (!select.Contains(" CPFCNPJEmitenteNota, "))
                    {
                        select.Append("EmitenteNota.CLI_CGCCPF CPFCNPJEmitenteNota, ");
                        groupBy.Append("EmitenteNota.CLI_CGCCPF, ");

                        SetarJoinsEmitenteNota(joins);
                    }
                    break;

                case "DestinatarioNota":
                    if (!select.Contains(" DestinatarioNota, "))
                    {
                        select.Append("DestinatarioNota.CLI_NOME DestinatarioNota, ");
                        groupBy.Append("DestinatarioNota.CLI_NOME, ");

                        SetarJoinsDestinatarioNota(joins);
                    }
                    break;

                case "TipoDestinatarioNota":
                    if (!select.Contains(" TipoDestinatarioNota, "))
                    {
                        select.Append("DestinatarioNota.CLI_FISJUR TipoDestinatarioNota, ");
                        groupBy.Append("DestinatarioNota.CLI_FISJUR, ");

                        SetarJoinsDestinatarioNota(joins);
                    }
                    break;

                case "CPFCNPJDestinatarioNota":
                case "CPFCNPJDestinatarioNotaFormatado":
                    SetarSelect("TipoDestinatarioNota", 0, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    if (!select.Contains(" CPFCNPJDestinatarioNota, "))
                    {
                        select.Append("DestinatarioNota.CLI_CGCCPF CPFCNPJDestinatarioNota, ");
                        groupBy.Append("DestinatarioNota.CLI_CGCCPF, ");

                        SetarJoinsDestinatarioNota(joins);
                    }
                    break;
            }
        }

        protected override void SetarWhere(FiltroPesquisaRelatorioConferenciaFiscal filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            string pattern = "yyyy-MM-dd";

            if (filtrosPesquisa.DataAberturaCIOTInicial != DateTime.MinValue)
                where.Append($" and CIOT.CIO_DATA_ABERTURA >= '{filtrosPesquisa.DataAberturaCIOTInicial.ToString(pattern)}'");

            if (filtrosPesquisa.DataAberturaCIOTFinal != DateTime.MinValue)
                where.Append($" and CIOT.CIO_DATA_ABERTURA < '{filtrosPesquisa.DataAberturaCIOTFinal.AddDays(1).ToString(pattern)}'");

            if (filtrosPesquisa.DataEncerramentoCIOTInicial != DateTime.MinValue)
                where.Append($" and CIOT.CIO_DATA_ENCERRAMENTO >= '{filtrosPesquisa.DataEncerramentoCIOTInicial.ToString(pattern)}'");

            if (filtrosPesquisa.DataEncerramentoCIOTFinal != DateTime.MinValue)
                where.Append($" and CIOT.CIO_DATA_ENCERRAMENTO < '{filtrosPesquisa.DataEncerramentoCIOTFinal.AddDays(1).ToString(pattern)}'");

            if (filtrosPesquisa.NumeroCTe > 0)
            {
                where.Append($" and CTe.CON_NUM = {filtrosPesquisa.NumeroCTe}");
                SetarJoinsCTe(joins);
            }

            if (filtrosPesquisa.StatusCTe != string.Empty)
            {
                where.Append($" and CTe.CON_STATUS = :CTE_CON_STATUS"); 
                parametros.Add(new Embarcador.Consulta.ParametroSQL("CTE_CON_STATUS", filtrosPesquisa.StatusCTe));
                SetarJoinsCTe(joins);
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroCarga))
            {
                where.Append($" and Carga.CAR_CODIGO_CARGA_EMBARCADOR = :CARGA_CAR_CODIGO_CARGA_EMBARCADOR");
                parametros.Add(new Embarcador.Consulta.ParametroSQL("CARGA_CAR_CODIGO_CARGA_EMBARCADOR", filtrosPesquisa.NumeroCarga));
                SetarJoinsCarga(joins);
            }

            if (filtrosPesquisa.TipoOperacao > 0)
            {
                where.Append($" and Carga.TOP_CODIGO = {filtrosPesquisa.TipoOperacao}");
                SetarJoinsCarga(joins);
            }

            if (filtrosPesquisa.Empresa > 0)
            {
                where.Append($" and Carga.EMP_CODIGO = {filtrosPesquisa.Empresa} ");
                SetarJoinsCarga(joins);
            }

            if (filtrosPesquisa.Veiculo > 0)
            {
                where.Append($" and (Carga.CAR_VEICULO = {filtrosPesquisa.Veiculo} or exists (select VEI_CODIGO from T_CARGA_VEICULOS_VINCULADOS where CAR_CODIGO = Carga.CAR_CODIGO and VEI_CODIGO = {filtrosPesquisa.Veiculo}))"); // SQL-INJECTION-SAFE
                SetarJoinsCarga(joins);
            }

            if (filtrosPesquisa.ModeloVeicular > 0)
            {
                where.Append($" and (ModeloVeicular.MVC_CODIGO = {filtrosPesquisa.ModeloVeicular} or exists (select VeiculoVinculado.VEI_CODIGO from T_CARGA_VEICULOS_VINCULADOS VeiculoVinculado INNER JOIN T_VEICULO Veiculo ON VeiculoVinculado.VEI_CODIGO = Veiculo.VEI_CODIGO INNER JOIN T_MODELO_VEICULAR_CARGA ModeloVeicular ON Veiculo.MVC_CODIGO = ModeloVeicular.MVC_CODIGO where VeiculoVinculado.CAR_CODIGO = Carga.CAR_CODIGO and ModeloVeicular.MVC_CODIGO = {filtrosPesquisa.ModeloVeicular}))"); // SQL-INJECTION-SAFE
                SetarJoinsModeloVeicular(joins);
            }

            if (filtrosPesquisa.CpfCnpjTerceiros.Count > 0)
            {
                where.Append($" and Contrato.CLI_CGCCPF_TERCEIRO in({string.Join(",", filtrosPesquisa.CpfCnpjTerceiros)})");
                SetarJoinsContrato(joins);
            }

            if (filtrosPesquisa.DataEmissaoContratoInicial != DateTime.MinValue)
            {
                where.Append($" and Contrato.CFT_DATA_EMISSAO_CONTRATO >= '{filtrosPesquisa.DataEmissaoContratoInicial.ToString(pattern)}'");
                SetarJoinsContrato(joins);
            }

            if (filtrosPesquisa.DataEmissaoContratoFinal != DateTime.MinValue)
            {
                where.Append($" and Contrato.CFT_DATA_EMISSAO_CONTRATO < '{filtrosPesquisa.DataEmissaoContratoFinal.AddDays(1).ToString(pattern)}'");
                SetarJoinsContrato(joins);
            }

            if (filtrosPesquisa.NumeroContrato > 0)
            {
                where.Append($" and Contrato.CFT_NUMERO_CONTRATO = {filtrosPesquisa.NumeroContrato}");
                SetarJoinsContrato(joins);
            }

            if (filtrosPesquisa.Situacao?.Count > 0)
            {
                where.Append($" and Contrato.CFT_CONTRATO_FRETE in ({string.Join(",", filtrosPesquisa.Situacao.Select(o => o.ToString("D")))})");
                SetarJoinsContrato(joins);
            }

            if (filtrosPesquisa.DataAprovacaoInicial != DateTime.MinValue)
            {
                where.Append($" and Contrato.CFT_CODIGO in (SELECT CFT_CODIGO FROM T_AUTORIZACAO_ALCADA_CONTRATO_FRETE_TERCEIRO WHERE AAC_DATA >= '{filtrosPesquisa.DataAprovacaoInicial.ToString(pattern)}')"); // SQL-INJECTION-SAFE
                SetarJoinsContrato(joins);
            }

            if (filtrosPesquisa.DataAprovacaoFinal != DateTime.MinValue)
            {
                where.Append($" and Contrato.CFT_CODIGO in (SELECT CFT_CODIGO FROM T_AUTORIZACAO_ALCADA_CONTRATO_FRETE_TERCEIRO WHERE AAC_DATA < '{filtrosPesquisa.DataAprovacaoFinal.AddDays(1).ToString(pattern)}')"); // SQL-INJECTION-SAFE
                SetarJoinsContrato(joins);
            }

            if (filtrosPesquisa.DataEncerramentoInicial != DateTime.MinValue)
            {
                where.Append($" and Contrato.CFT_DATA_ENCERRAMENTO_CONTRATO >= '{filtrosPesquisa.DataEncerramentoInicial.ToString(pattern)}'");
                SetarJoinsContrato(joins);
            }

            if (filtrosPesquisa.DataEncerramentoFinal != DateTime.MinValue)
            {
                where.Append($" and Contrato.CFT_DATA_ENCERRAMENTO_CONTRATO < '{filtrosPesquisa.DataEncerramentoFinal.AddDays(1).ToString(pattern)}'");
                SetarJoinsContrato(joins);
            }                
        }

        #endregion

        
    }
}
