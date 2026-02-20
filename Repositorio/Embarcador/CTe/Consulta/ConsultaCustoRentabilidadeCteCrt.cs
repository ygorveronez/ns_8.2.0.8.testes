using Dominio.ObjetosDeValor.Embarcador.Consulta;
using Dominio.ObjetosDeValor.Embarcador.CTe;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using Repositorio.Embarcador.Consulta;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.CTe;

public sealed class ConsultaCustoRentabilidadeCteCrt : Consulta.Consulta<FiltroPesquisaRelatorioCustoRentabilidadeCteCrt>
{
    #region Construtores

    public ConsultaCustoRentabilidadeCteCrt() : base(tabela: "T_CARGA AS Carga ") { }

    #endregion

    #region Atributos Privados

    private readonly StringBuilder withSelect = new StringBuilder();
    private readonly StringBuilder withJoins = new StringBuilder();

    #endregion Atributos Privados

    #region Métodos Privados

    private void SetarJoinsWithCargaCTe(StringBuilder joins)
    {
        if (!joins.Contains(" CargaCTe "))
            joins.Append(" LEFT JOIN T_CARGA_CTE CargaCTe on CargaCTe.CCT_CODIGO = GestaoDocumento.CCT_CODIGO ");
    }

    private void SetarJoinsWithCarga(StringBuilder joins)
    {
        SetarJoinsWithCargaCTe(joins);

        if (!joins.Contains(" Carga "))
            joins.Append(" LEFT JOIN T_CARGA Carga ON Carga.CAR_CODIGO = CargaCTe.CAR_CODIGO ");
    }

    private void SetarJoinsWithPreCTe(StringBuilder joins)
    {
        SetarJoinsWithCargaCTe(joins);

        if (!joins.Contains(" PreCTe "))
            joins.Append(" LEFT JOIN T_PRE_CTE PreCTe ON PreCTe.PCO_CODIGO = CargaCTe.PCO_CODIGO ");
    }

    private void SetarJoinsWithCTeEspelho(StringBuilder joins)
    {
        SetarJoinsWithCargaCTe(joins);

        if (!joins.Contains(" CTeEspelho "))
            joins.Append(" LEFT JOIN T_CTE CTeEspelho ON CTeEspelho.CON_CODIGO = CargaCte.CON_CODIGO ");
    }

    private void SetarJoinsWithEmpresaEspelho(StringBuilder joins)
    {
        SetarJoinsWithCTeEspelho(joins);

        if (!joins.Contains(" EmpresaEspelho "))
            joins.Append(" LEFT JOIN T_EMPRESA EmpresaEspelho ON EmpresaEspelho.EMP_CODIGO = CteEspelho.EMP_CODIGO ");
    }

    private void SetarJoinsCargaCTe(StringBuilder joins)
    {
        if (!joins.Contains(" CargaCTe "))
            joins.Append(" JOIN T_CARGA_CTE CargaCTe ON CargaCTe.CAR_CODIGO = Carga.CAR_CODIGO ");
    }

    private void SetarJoinsFilial(StringBuilder joins)
    {
        if (!joins.Contains(" Filial "))
            joins.Append(" LEFT JOIN T_FILIAL Filial ON Filial.FIL_CODIGO = Carga.FIL_CODIGO ");
    }

    private void SetarJoinsCTe(StringBuilder joins)
    {
        SetarJoinsCargaCTe(joins);

        if (!joins.Contains(" CTe "))
            joins.Append(" LEFT JOIN T_CTE CTe ON CTe.CON_CODIGO = CargaCTe.CON_CODIGO ");
    }

    private void SetarJoinsSerie(StringBuilder joins)
    {
        SetarJoinsCTe(joins);

        if (!joins.Contains(" Serie "))
            joins.Append(" LEFT JOIN T_EMPRESA_SERIE Serie on CTe.CON_SERIE = Serie.ESE_CODIGO ");
    }

    private void SetarJoinsTomadorCTe(StringBuilder joins)
    {
        SetarJoinsCTe(joins);

        if (!joins.Contains(" TomadorCTe "))
            joins.Append(" LEFT JOIN T_CTE_PARTICIPANTE TomadorCTe on TomadorCTe.PCT_CODIGO = CTe.CON_TOMADOR_PAGADOR_CTE ");
    }

    private void SetarJoinsTomadorCliente(StringBuilder joins)
    {
        SetarJoinsTomadorCTe(joins);

        if (!joins.Contains(" ClienteTomador "))
            joins.Append(" LEFT JOIN T_CLIENTE ClienteTomador on ClienteTomador.CLI_CGCCPF = TomadorCTe.CLI_CODIGO ");
    }

    private void SetarJoinsEmpresa(StringBuilder joins)
    {
        SetarJoinsCTe(joins);

        if (!joins.Contains(" Empresa "))
            joins.Append(" LEFT JOIN T_EMPRESA Empresa ON Empresa.EMP_CODIGO = CTe.EMP_CODIGO ");
    }

    private void SetarJoinsRemetente(StringBuilder joins)
    {
        if (!joins.Contains(" RemetenteCTe "))
            joins.Append(" left join T_CTE_PARTICIPANTE RemetenteCTe on CTe.CON_REMETENTE_CTE = RemetenteCTe.PCT_CODIGO ");
    }

    private void SetarJoinsRemetenteCliente(StringBuilder joins)
    {
        SetarJoinsRemetente(joins);

        if (!joins.Contains(" ClienteRemetente "))
            joins.Append(" left join T_CLIENTE ClienteRemetente on ClienteRemetente.CLI_CGCCPF = RemetenteCTe.CLI_CODIGO ");
    }

    private void SetarJoinsDestinatario(StringBuilder joins)
    {
        if (!joins.Contains(" DestinatarioCTe "))
            joins.Append(" left join T_CTE_PARTICIPANTE DestinatarioCTe on CTe.CON_DESTINATARIO_CTE = DestinatarioCTe.PCT_CODIGO ");
    }

    private void SetarJoinsDestinatarioCliente(StringBuilder joins)
    {
        SetarJoinsDestinatario(joins);

        if (!joins.Contains("ClienteDestinatario"))
            joins.Append(" left join T_CLIENTE ClienteDestinatario on ClienteDestinatario.CLI_CGCCPF = DestinatarioCTe.CLI_CODIGO ");
    }

    private void SetarJoinsCTE_GestaoDocumento(StringBuilder joins)
    {
        if (!joins.Contains(" CTE_GestaoDocumento "))
            joins.Append(" LEFT JOIN CTE_GestaoDocumento ON CTE_GestaoDocumento.CodigoCargaOriginal = Carga.CAR_CODIGO ");
    }

    #endregion

    #region Métodos Protegidos Sobrescritos

    protected override SQLDinamico ObterSql(FiltroPesquisaRelatorioCustoRentabilidadeCteCrt filtrosPesquisa, ParametroConsulta parametrosConsulta, List<PropriedadeAgrupamento> propriedades, bool somenteContarNumeroRegistros)
    {
        var sqlDinamico = base.ObterSql(filtrosPesquisa, parametrosConsulta, propriedades, somenteContarNumeroRegistros);

        var sql = sqlDinamico.StringQuery;

        string campos = withSelect.ToString().Trim();

        StringBuilder withSql = new StringBuilder();
        withSql.AppendLine("WITH CTE_GestaoDocumento AS (");
        withSql.AppendLine($" SELECT {(campos.Length > 0 ? campos.Substring(0, campos.Length - 1) : "")}");
        withSql.AppendLine(" FROM T_GESTAO_DOCUMENTO gestaoDocumento");
        withSql.AppendLine(withJoins.ToString());
        withSql.AppendLine(")");

        withSql.AppendLine(sql);

        return new SQLDinamico(withSql.ToString(), sqlDinamico.Parametros);
    }

    protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, FiltroPesquisaRelatorioCustoRentabilidadeCteCrt filtrosPesquisa)
    {
        switch (propriedade)
        {
            case "CodigoCargaOriginal":
                if (!withSelect.Contains("CodigoCargaOriginal, "))
                {
                    withSelect.Append("carga.CAR_CARGA_ESPELHO AS CodigoCargaOriginal, ");

                    SetarJoinsWithCarga(withJoins);
                    SetarJoinsCTE_GestaoDocumento(joins);
                }
                break;

            case "NotasFiscais":
                if (!select.Contains(" NotasFiscais, "))
                {
                    select.Append(@"SUBSTRING((
                                            SELECT DISTINCT 
                                                ', ' + CAST(_notaFiscal.NF_NUMERO AS VARCHAR)
                                            FROM 
                                                T_XML_NOTA_FISCAL _notaFiscal
                                                JOIN T_PEDIDO_XML_NOTA_FISCAL _pedidoNotaFiscal ON _pedidoNotaFiscal.NFX_CODIGO = _notaFiscal.NFX_CODIGO
			                                    JOIN T_CARGA_PEDIDO _cargaPedido ON _cargaPedido.CPE_CODIGO = _pedidoNotaFiscal.CPE_CODIGO
                                            WHERE 
                                                _cargaPedido.CAR_CODIGO = Carga.CAR_CODIGO
                                                AND _notaFiscal.NF_FATURA = 0
                                            FOR XML PATH('')
                                        ), 3, 100000) NotasFiscais, "
                    );

                    if (!groupBy.Contains("Carga.CAR_CODIGO"))
                        groupBy.Append(" Carga.CAR_CODIGO, ");
                }
                break;

            case "Tomador":
                if (!select.Contains(" Tomador, "))
                {
                    select.Append("TomadorCTe.PCT_NOME Tomador, ");

                    if (!groupBy.Contains("TomadorCTe.PCT_NOME"))
                        groupBy.Append(" TomadorCTe.PCT_NOME, ");

                    SetarJoinsTomadorCTe(joins);
                }
                break;

            case "CargaVenda":
                if (!select.Contains(" CargaVenda, "))
                {
                    select.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR CargaVenda, ");

                    if (!groupBy.Contains("Carga.CAR_CODIGO_CARGA_EMBARCADOR"))
                        groupBy.Append(" Carga.CAR_CODIGO_CARGA_EMBARCADOR, ");
                }
                break;

            case "NumeroDocumentoCTe":
                if (!select.Contains(" NumeroDocumentoCTe, "))
                {
                    select.Append("CAST(ISNULL(CTe.CON_NUMERO_CRT, CTe.CON_NUM) AS VARCHAR) NumeroDocumentoCTe, ");

                    if (!groupBy.Contains("CTe.CON_NUM"))
                        groupBy.Append(" CTe.CON_NUM, ");

                    if (!groupBy.Contains("CTe.CON_NUMERO_CRT"))
                        groupBy.Append(" CTe.CON_NUMERO_CRT, ");

                    SetarJoinsCTe(joins);
                }
                break;

            case "CodigoIntegracaoFilial":
                if (!select.Contains(" CodigoIntegracaoFilial, "))
                {
                    select.Append("Filial.FIL_CODIGO_FILIAL_EMBARCADOR CodigoIntegracaoFilial, ");

                    if (!groupBy.Contains("Filial.FIL_CODIGO_FILIAL_EMBARCADOR"))
                        groupBy.Append(" Filial.FIL_CODIGO_FILIAL_EMBARCADOR, ");

                    SetarJoinsFilial(joins);
                }
                break;

            case "DataEmissaoOriginal":
                if (!select.Contains(" DataEmissaoOriginal, "))
                {
                    select.Append("cte.CON_DATA_AUTORIZACAO DataEmissaoOriginal, ");

                    if (!groupBy.Contains("cte.CON_DATA_AUTORIZACAO"))
                        groupBy.Append(" cte.CON_DATA_AUTORIZACAO, ");

                    SetarJoinsCTe(joins);
                }
                break;

            case "ValorBrutoOriginal":
                if (!select.Contains(" ValorBrutoOriginal, "))
                {
                    select.Append("cte.CON_VALOR_RECEBER ValorBrutoOriginal, ");

                    if (!groupBy.Contains("cte.CON_VALOR_RECEBER"))
                        groupBy.Append(" cte.CON_VALOR_RECEBER, ");

                    SetarJoinsCTe(joins);
                }
                break;

            case "ValorPisOriginal":
                if (!select.Contains(" ValorPisOriginal, "))
                {
                    select.Append("cte.CON_VALOR_PIS ValorPisOriginal, ");

                    if (!groupBy.Contains("cte.CON_VALOR_PIS"))
                        groupBy.Append(" cte.CON_VALOR_PIS, ");

                    SetarJoinsCTe(joins);
                }
                break;

            case "ValorCofinsOriginal":
                if (!select.Contains(" ValorCofinsOriginal, "))
                {
                    select.Append("cte.CON_VALOR_COFINS ValorCofinsOriginal, ");

                    if (!groupBy.Contains("cte.CON_VALOR_COFINS"))
                        groupBy.Append(" cte.CON_VALOR_COFINS, ");

                    SetarJoinsCTe(joins);
                }
                break;

            case "ValorIssNfOriginal":
                if (!select.Contains(" ValorIssNfOriginal, "))
                {
                    select.Append("cte.CON_VALOR_ISS_RETIDO ValorIssNfOriginal, ");

                    if (!groupBy.Contains("cte.CON_VALOR_ISS_RETIDO"))
                        groupBy.Append(" cte.CON_VALOR_ISS_RETIDO, ");

                    SetarJoinsCTe(joins);
                }
                break;

            case "ValorIcmsOriginal":
                if (!select.Contains(" ValorIcmsOriginal, "))
                {
                    select.Append("cte.CON_VAL_ICMS ValorIcmsOriginal, ");

                    if (!groupBy.Contains("cte.CON_VAL_ICMS"))
                        groupBy.Append(" cte.CON_VAL_ICMS, ");

                    SetarJoinsCTe(joins);
                }
                break;

            case "ValorLiquidoOriginal":
                if (!select.Contains(" ValorLiquidoOriginal, "))
                {
                    select.Append("cte.CON_VALOR_FRETE ValorLiquidoOriginal, ");

                    if (!groupBy.Contains("cte.CON_VALOR_FRETE"))
                        groupBy.Append(" cte.CON_VALOR_FRETE, ");

                    SetarJoinsCTe(joins);
                }
                break;

            case "TransportadoraOriginal":
                if (!select.Contains(" TransportadoraOriginal, "))
                {
                    select.Append("empresa.EMP_RAZAO TransportadoraOriginal, ");

                    if (!groupBy.Contains("empresa.EMP_RAZAO"))
                        groupBy.Append(" empresa.EMP_RAZAO, ");

                    SetarJoinsEmpresa(joins);
                }
                break;

            case "CnpjTransportadoraOriginal":
            case "CnpjTransportadoraOriginalFormatado":
                if (!select.Contains(" CnpjTransportadoraOriginal, "))
                {
                    select.Append("empresa.EMP_CNPJ CnpjTransportadoraOriginal, ");

                    if (!groupBy.Contains("empresa.EMP_CNPJ"))
                        groupBy.Append(" empresa.EMP_CNPJ, ");

                    SetarJoinsEmpresa(joins);
                }
                break;

            case "NumeroProvisao":
                if (!select.Contains(" NumeroProvisao, "))
                {
                    select.Append("CTE_GestaoDocumento.NumeroProvisao NumeroProvisao, ");
                    withSelect.Append("(Select CAST(ISNULL(_cte.CON_NUMERO_CRT, _cte.CON_NUM) AS VARCHAR) from T_CTE _cte where _cte.CON_CODIGO = gestaoDocumento.CON_CODIGO) NumeroProvisao, ");

                    if (!groupBy.Contains("CTE_GestaoDocumento.NumeroProvisao"))
                        groupBy.Append(" CTE_GestaoDocumento.NumeroProvisao, ");

                    SetarJoinsCTE_GestaoDocumento(joins);
                }
                break;

            case "DataEmissaoProvisao":
                if (!select.Contains(" DataEmissaoProvisao, "))
                {
                    select.Append("CTE_GestaoDocumento.DataEmissaoProvisao DataEmissaoProvisao, ");
                    withSelect.Append("preCte.PCO_DATAHORAEMISSAO DataEmissaoProvisao, ");

                    if (!groupBy.Contains("CTE_GestaoDocumento.DataEmissaoProvisao"))
                        groupBy.Append(" CTE_GestaoDocumento.DataEmissaoProvisao, ");

                    SetarJoinsCTE_GestaoDocumento(joins);
                    SetarJoinsWithPreCTe(withJoins);
                }
                break;

            case "ValorBrutoProvisao":
                if (!select.Contains(" ValorBrutoProvisao, "))
                {
                    select.Append("CTE_GestaoDocumento.ValorBrutoProvisao ValorBrutoProvisao, ");
                    withSelect.Append("preCte.PCO_VALOR_RECEBER ValorBrutoProvisao, ");

                    if (!groupBy.Contains("CTE_GestaoDocumento.ValorBrutoProvisao"))
                        groupBy.Append(" CTE_GestaoDocumento.ValorBrutoProvisao, ");

                    SetarJoinsCTE_GestaoDocumento(joins);
                    SetarJoinsWithPreCTe(withJoins);
                }
                break;

            case "ValorPisProvisao":
                if (!select.Contains(" ValorPisProvisao, "))
                {
                    select.Append("CTE_GestaoDocumento.ValorPisProvisao ValorPisProvisao, ");
                    withSelect.Append("preCte.PCO_VALOR_PIS ValorPisProvisao, ");

                    if (!groupBy.Contains("CTE_GestaoDocumento.ValorPisProvisao"))
                        groupBy.Append(" CTE_GestaoDocumento.ValorPisProvisao, ");

                    SetarJoinsCTE_GestaoDocumento(joins);
                    SetarJoinsWithPreCTe(withJoins);
                }
                break;

            case "ValorCofinsProvisao":
                if (!select.Contains(" ValorCofinsProvisao, "))
                {
                    select.Append("CTE_GestaoDocumento.ValorCofinsProvisao ValorCofinsProvisao, ");
                    withSelect.Append("preCte.PCO_VALOR_COFINS ValorCofinsProvisao, ");

                    if (!groupBy.Contains("CTE_GestaoDocumento.ValorCofinsProvisao"))
                        groupBy.Append(" CTE_GestaoDocumento.ValorCofinsProvisao, ");

                    SetarJoinsCTE_GestaoDocumento(joins);
                    SetarJoinsWithPreCTe(withJoins);
                }
                break;

            case "ValorImpostoProvisao":
                if (!select.Contains(" ValorImpostoProvisao, "))
                {
                    select.Append("CTE_GestaoDocumento.ValorImpostoProvisao ValorImpostoProvisao, ");
                    withSelect.Append("preCte.PCO_VALOR_ISS_RETIDO ValorImpostoProvisao, ");

                    if (!groupBy.Contains("CTE_GestaoDocumento.ValorImpostoProvisao"))
                        groupBy.Append(" CTE_GestaoDocumento.ValorImpostoProvisao, ");

                    SetarJoinsCTE_GestaoDocumento(joins);
                    SetarJoinsWithPreCTe(withJoins);
                }
                break;

            case "ValorIcmsProvisao":
                if (!select.Contains(" ValorIcmsProvisao, "))
                {
                    select.Append("CTE_GestaoDocumento.ValorIcmsProvisao ValorIcmsProvisao, ");
                    withSelect.Append("preCte.PCO_VAL_ICMS ValorIcmsProvisao, ");

                    if (!groupBy.Contains("CTE_GestaoDocumento.ValorIcmsProvisao"))
                        groupBy.Append(" CTE_GestaoDocumento.ValorIcmsProvisao, ");

                    SetarJoinsCTE_GestaoDocumento(joins);
                    SetarJoinsWithPreCTe(withJoins);
                }
                break;

            case "ValorLiquidoProvisao":
                if (!select.Contains(" ValorLiquidoProvisao, "))
                {
                    select.Append("CTE_GestaoDocumento.ValorLiquidoProvisao ValorLiquidoProvisao, ");
                    withSelect.Append("preCte.PCO_VALOR_FRETE ValorLiquidoProvisao, ");

                    if (!groupBy.Contains("CTE_GestaoDocumento.ValorLiquidoProvisao"))
                        groupBy.Append(" CTE_GestaoDocumento.ValorLiquidoProvisao, ");

                    SetarJoinsCTE_GestaoDocumento(joins);
                    SetarJoinsWithPreCTe(withJoins);
                }
                break;

            case "DataEmissaoCTeEspelho":
                if (!select.Contains(" DataEmissaoCTeEspelho, "))
                {
                    select.Append("CTE_GestaoDocumento.DataEmissaoCTeEspelho DataEmissaoCTeEspelho, ");
                    withSelect.Append("cteEspelho.CON_DATA_AUTORIZACAO DataEmissaoCTeEspelho, ");

                    if (!groupBy.Contains("CTE_GestaoDocumento.DataEmissaoCTeEspelho"))
                        groupBy.Append(" CTE_GestaoDocumento.DataEmissaoCTeEspelho, ");

                    SetarJoinsCTE_GestaoDocumento(joins);
                    SetarJoinsWithCTeEspelho(withJoins);
                }
                break;

            case "ValorBrutoCTeEspelho":
                if (!select.Contains(" ValorBrutoCTeEspelho, "))
                {
                    select.Append("CTE_GestaoDocumento.ValorBrutoCTeEspelho ValorBrutoCTeEspelho, ");
                    withSelect.Append("cteEspelho.CON_VALOR_RECEBER ValorBrutoCTeEspelho, ");

                    if (!groupBy.Contains("CTE_GestaoDocumento.ValorBrutoCTeEspelho"))
                        groupBy.Append(" CTE_GestaoDocumento.ValorBrutoCTeEspelho, ");

                    SetarJoinsCTE_GestaoDocumento(joins);
                    SetarJoinsWithCTeEspelho(withJoins);
                }
                break;

            case "ValorPisCTeEspelho":
                if (!select.Contains(" ValorPisCTeEspelho, "))
                {
                    select.Append("CTE_GestaoDocumento.ValorPisCTeEspelho ValorPisCTeEspelho, ");
                    withSelect.Append("cteEspelho.CON_VALOR_PIS ValorPisCTeEspelho, ");

                    if (!groupBy.Contains("CTE_GestaoDocumento.ValorPisCTeEspelho"))
                        groupBy.Append(" CTE_GestaoDocumento.ValorPisCTeEspelho, ");

                    SetarJoinsCTE_GestaoDocumento(joins);
                    SetarJoinsWithCTeEspelho(withJoins);
                }
                break;

            case "ValorCofinsCTeEspelho":
                if (!select.Contains(" ValorCofinsCTeEspelho, "))
                {
                    select.Append("CTE_GestaoDocumento.ValorCofinsCTeEspelho ValorCofinsCTeEspelho, ");
                    withSelect.Append("cteEspelho.CON_VALOR_COFINS ValorCofinsCTeEspelho, ");

                    if (!groupBy.Contains("CTE_GestaoDocumento.ValorCofinsCTeEspelho"))
                        groupBy.Append(" CTE_GestaoDocumento.ValorCofinsCTeEspelho, ");

                    SetarJoinsCTE_GestaoDocumento(joins);
                    SetarJoinsWithCTeEspelho(withJoins);
                }
                break;

            case "ValorIssNfEspelho":
                if (!select.Contains(" ValorIssNfEspelho, "))
                {
                    select.Append("CTE_GestaoDocumento.ValorIssNfEspelho ValorIssNfEspelho, ");
                    withSelect.Append("cteEspelho.CON_VALOR_ISS_RETIDO ValorIssNfEspelho, ");

                    if (!groupBy.Contains("CTE_GestaoDocumento.ValorIssNfEspelho"))
                        groupBy.Append(" CTE_GestaoDocumento.ValorIssNfEspelho, ");

                    SetarJoinsCTE_GestaoDocumento(joins);
                    SetarJoinsWithCTeEspelho(withJoins);
                }
                break;

            case "ValorIcmsCTeEspelho":
                if (!select.Contains(" ValorIcmsCTeEspelho, "))
                {
                    select.Append("CTE_GestaoDocumento.ValorIcmsCTeEspelho ValorIcmsCTeEspelho, ");
                    withSelect.Append("cteEspelho.CON_VAL_ICMS ValorIcmsCTeEspelho, ");

                    if (!groupBy.Contains("CTE_GestaoDocumento.ValorIcmsCTeEspelho"))
                        groupBy.Append(" CTE_GestaoDocumento.ValorIcmsCTeEspelho, ");

                    SetarJoinsCTE_GestaoDocumento(joins);
                    SetarJoinsWithCTeEspelho(withJoins);
                }
                break;

            case "ValorLiquidoCTeEspelho":
                if (!select.Contains(" ValorLiquidoCTeEspelho, "))
                {
                    select.Append("CTE_GestaoDocumento.ValorLiquidoCTeEspelho ValorLiquidoCTeEspelho, ");
                    withSelect.Append("cteEspelho.CON_VALOR_FRETE ValorLiquidoCTeEspelho, ");

                    if (!groupBy.Contains("CTE_GestaoDocumento.ValorLiquidoCTeEspelho"))
                        groupBy.Append(" CTE_GestaoDocumento.ValorLiquidoCTeEspelho, ");

                    SetarJoinsCTE_GestaoDocumento(joins);
                    SetarJoinsWithCTeEspelho(withJoins);
                }
                break;

            case "TransportadoraEspelho":
                if (!select.Contains(" TransportadoraEspelho, "))
                {
                    select.Append("CTE_GestaoDocumento.TransportadoraEspelho TransportadoraEspelho, ");
                    withSelect.Append("empresaEspelho.EMP_RAZAO TransportadoraEspelho, ");

                    if (!groupBy.Contains("CTE_GestaoDocumento.TransportadoraEspelho"))
                        groupBy.Append(" cte_gestaodocumento.transportadoraespelho, ");

                    SetarJoinsCTE_GestaoDocumento(joins);
                    SetarJoinsWithEmpresaEspelho(withJoins);
                }
                break;

            case "CnpjTransportadoraEspelho":
            case "CnpjTransportadoraEspelhoFormatado":
                if (!select.Contains(" CnpjTransportadoraEspelho, "))
                {
                    select.Append("CTE_GestaoDocumento.CnpjTransportadoraEspelho CnpjTransportadoraEspelho, ");
                    withSelect.Append("empresaEspelho.EMP_CNPJ CnpjTransportadoraEspelho, ");

                    if (!groupBy.Contains("CTE_GestaoDocumento.CnpjTransportadoraEspelho"))
                        groupBy.Append(" CTE_GestaoDocumento.CnpjTransportadoraEspelho, ");

                    SetarJoinsCTE_GestaoDocumento(joins);
                    SetarJoinsWithEmpresaEspelho(withJoins);
                }
                break;

            case "Rentabilidade":
                SetarSelect("ValorLiquidoCTeEspelho", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                SetarSelect("ValorLiquidoProvisao", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                SetarSelect("ValorLiquidoOriginal", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                break;

            case "RentabilidadeDolar":
                SetarSelect("ReceitaDolar", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                SetarSelect("ProvisaoDolar", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                SetarSelect("DespesaDolar", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                break;

            case "ReceitaDolar":
                if (!select.Contains(" ReceitaDolar, "))
                {
                    select.Append("Carga.CAR_VALOR_TOTAL_MOEDA ReceitaDolar,");

                    if (!groupBy.Contains("Carga.CAR_VALOR_TOTAL_MOEDA"))
                        groupBy.Append(" Carga.CAR_VALOR_TOTAL_MOEDA, ");
                }
                break;

            case "ProvisaoDolar":
                if (!select.Contains(" ProvisaoDolar, "))
                {
                    select.Append("CTE_GestaoDocumento.ProvisaoDolar ProvisaoDolar, ");
                    withSelect.Append("carga.CAR_VALOR_TOTAL_MOEDA ProvisaoDolar, ");

                    if (!groupBy.Contains("CTE_GestaoDocumento.ProvisaoDolar"))
                        groupBy.Append(" CTE_GestaoDocumento.ProvisaoDolar, ");
                }
                break;

            case "DespesaDolar":
                if (!select.Contains(" DespesaDolar, "))
                {
                    select.Append("CTE_GestaoDocumento.DespesaDolar DespesaDolar, ");
                    withSelect.Append("cteEspelho.CON_VALOR_TOTAL_MOEDA DespesaDolar, ");

                    if (!groupBy.Contains("CTE_GestaoDocumento.DespesaDolar"))
                        groupBy.Append(" CTE_GestaoDocumento.DespesaDolar, ");
                }
                break;

            case "TaxaConversaoMoeda":
                if (!select.Contains(" TaxaConversaoMoeda, "))
                {
                    select.Append("Carga.CAR_VALOR_COTACAO_MOEDA TaxaConversaoMoeda,");

                    if (!groupBy.Contains("Carga.CAR_VALOR_COTACAO_MOEDA"))
                        groupBy.Append(" Carga.CAR_VALOR_COTACAO_MOEDA, ");
                }
                break;
        }
    }

    protected override void SetarWhere(FiltroPesquisaRelatorioCustoRentabilidadeCteCrt filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
    {
        string pattern = "yyyy-MM-dd";

        where.Append($"AND carga.CAR_SITUACAO not in ({(int)SituacaoCarga.Cancelada}, {(int)SituacaoCarga.Anulada}) ");

        if (filtrosPesquisa.DataInicialEmissao.HasValue)
        {
            where.Append($" AND cte.CON_DATA_AUTORIZACAO >= '{filtrosPesquisa.DataInicialEmissao.Value.ToString(pattern)}' ");
            SetarJoinsCTe(joins);
        }

        if (filtrosPesquisa.DataFinalEmissao.HasValue)
        {
            where.Append($" AND cte.CON_DATA_AUTORIZACAO <= '{filtrosPesquisa.DataFinalEmissao.Value.ToString(pattern)}' ");
            SetarJoinsCTe(joins);
        }

        if (filtrosPesquisa.NumeroInicial > 0)
        {
            where.Append($" AND cte.CON_NUM >= {filtrosPesquisa.NumeroInicial} ");
            SetarJoinsCTe(joins);
        }

        if (filtrosPesquisa.NumeroFinal > 0)
        {
            where.Append($" AND cte.CON_NUM <= {filtrosPesquisa.NumeroFinal} ");
            SetarJoinsCTe(joins);
        }

        if (!string.IsNullOrEmpty(filtrosPesquisa.Pedido))
        {
            where.Append(@$" AND exists (select CargaCTe.CON_CODIGO from T_CARGA_CTE CargaCTe 
                            inner join T_CARGA_PEDIDO_XML_NOTA_FISCAL_CTE CargaPedidoXMLNotaFiscalCTe on CargaPedidoXMLNotaFiscalCTe.CCT_CODIGO = CargaCTe.CCT_CODIGO  
                            inner join T_PEDIDO_XML_NOTA_FISCAL PedidoXMLNotaFiscal on PedidoXMLNotaFiscal.PNF_CODIGO = CargaPedidoXMLNotaFiscalCTe.PNF_CODIGO
                            inner join T_CARGA_PEDIDO CargaPedido on CargaPedido.CPE_CODIGO = PedidoXMLNotaFiscal.CPE_CODIGO
                            inner join T_PEDIDO Pedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO
                            where CTe.CON_CODIGO = CargaCTe.CON_CODIGO AND Pedido.PED_NUMERO_PEDIDO_EMBARCADOR = :PEDIDO_PED_NUMERO_PEDIDO_EMBARCADOR)"); 
            parametros.Add(new ParametroSQL("PEDIDO_PED_NUMERO_PEDIDO_EMBARCADOR", filtrosPesquisa.Pedido));

            SetarJoinsCTe(joins);
        }

        if (!string.IsNullOrEmpty(filtrosPesquisa.NotaFiscal))
        {
            where.Append($"  AND exists (SELECT _notafiscal.CON_CODIGO FROM T_CTE_DOCS _notafiscal WHERE CTe.CON_CODIGO = _notafiscal.CON_CODIGO AND _notafiscal.NFC_NUMERO LIKE :NOTAFISCAL_NFC_NUMERO) "); 
            parametros.Add(new ParametroSQL("NOTAFISCAL_NFC_NUMERO", $"%{filtrosPesquisa.NotaFiscal}%"));

            SetarJoinsCTe(joins);
        }

        if (filtrosPesquisa.Serie > 0)
        {
            where.Append($"  AND Serie.ESE_NUMERO = {filtrosPesquisa.Serie}");
            SetarJoinsSerie(joins);
        }

        if (filtrosPesquisa.TipoServico.Count > 0)
        {
            where.Append($"  and CTe.CON_TIPO_SERVICO in ('" + string.Join("', '", filtrosPesquisa.TipoServico) + "')");
            SetarJoinsCTe(joins);
        }

        if (filtrosPesquisa.Situacao.Count > 0)
        {
            if (filtrosPesquisa.Situacao.Contains("G"))
                where.Append("  and (CTe.CON_STATUS in ('" + string.Join("', '", filtrosPesquisa.Situacao) + "') OR CTe.CON_ANULADO_GERENCIALMENTE = 1) ");
            else
                where.Append("  and (CTe.CON_STATUS in ('" + string.Join("', '", filtrosPesquisa.Situacao) + "') AND (CTe.CON_ANULADO_GERENCIALMENTE IS NULL OR CTe.CON_ANULADO_GERENCIALMENTE = 0)) ");

            SetarJoinsCTe(joins);
        }

        if (filtrosPesquisa.CTeVinculadoACarga.HasValue)
            where.Append("  and " + (filtrosPesquisa.CTeVinculadoACarga.Value ? "exists" : "not exists") + " (select _cargaCTe.CON_CODIGO from T_CARGA_CTE _cargaCTe WHERE CTe.CON_CODIGO = _cargaCTe.CON_CODIGO) ");

        if (filtrosPesquisa.CpfCnpjRemetente > 0)
        {
            where.Append("  and ClienteRemetente.CLI_CGCCPF = " + filtrosPesquisa.CpfCnpjRemetente.ToString("F0"));

            SetarJoinsRemetenteCliente(joins);
        }

        if (filtrosPesquisa.CpfCnpjDestinatarios.Count > 0)
        {
            where.Append("  and ClienteDestinatario.CLI_CGCCPF in (" + string.Join(",", filtrosPesquisa.CpfCnpjDestinatarios) + ')');

            SetarJoinsDestinatarioCliente(joins);
        }

        if (filtrosPesquisa.CpfCnpjTomadores.Count > 0)
        {
            where.Append("  and ClienteTomador.CLI_CGCCPF in (" + string.Join(",", filtrosPesquisa.CpfCnpjTomadores) + ')');

            SetarJoinsTomadorCliente(joins);
        }

        if (filtrosPesquisa.CodigosVeiculo.Count > 0)
        {
            where.Append(" and exists (select vei_codigo from t_cte_veiculo vei where vei.CON_CODIGO = CTe.CON_CODIGO and vei.VEI_CODIGO ");

            where.Append($"in ({string.Join(",", filtrosPesquisa.CodigosVeiculo)}))");
        }

        if (filtrosPesquisa.CodigosCarga.Count > 0)
            where.Append($" AND Carga.CAR_CODIGO in ({string.Join(",", filtrosPesquisa.CodigosCarga)})");

        if (filtrosPesquisa.CodigoFilial > 0)
            where.Append($" AND Carga.FIL_CODIGO = {filtrosPesquisa.CodigoFilial}");

        if (filtrosPesquisa.CodigosTransportador.Count > 0)
            where.Append($" AND Carga.EMP_CODIGO in ({string.Join(",", filtrosPesquisa.CodigosTransportador)})");

        if (filtrosPesquisa.CodigosTipoOperacao.Count > 0)
            where.Append($" AND Carga.TOP_CODIGO in ({string.Join(",", filtrosPesquisa.CodigosTipoOperacao)})");

        if (filtrosPesquisa.CodigosCTe.Count > 0)
        {
            where.Append($"AND CTe.CON_CODIGO IN ({string.Join(",", filtrosPesquisa.CodigosCTe)}) ");
            SetarJoinsCTe(joins);
        }

        if (filtrosPesquisa.CodigosModeloDocumento.Count > 0)
        {
            where.Append($"AND CTe.CON_MODELODOC IN ({string.Join(",", filtrosPesquisa.CodigosModeloDocumento)}) ");
            SetarJoinsCTe(joins);
        }
    }
    #endregion
}