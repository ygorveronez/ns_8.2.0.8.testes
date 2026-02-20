using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio
{
    sealed class ConsultaManifestoEletronicoDeDocumentosFiscais : Embarcador.Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.MDFe.FiltroPesquisaMdfeRelatorio>
    {
        #region Construtores

        public ConsultaManifestoEletronicoDeDocumentosFiscais() : base(tabela: "T_MDFE as Mdfe") { }

        #endregion

        #region Métodos Privados

        private void SetarJoinsEmpresa(StringBuilder joins)
        {
            if (!joins.Contains(" Empresa "))
                joins.Append("join T_EMPRESA Empresa on Empresa.EMP_CODIGO = Mdfe.EMP_CODIGO ");
        }

        private void SetarJoinsMunicipios(StringBuilder joins)
        {
            if (!joins.Contains(" MunicipioDescarregamentoMDFe "))
                joins.Append("join T_MDFE_MUNICIPIO_DESCARREGAMENTO MunicipioDescarregamentoMDFe on MunicipioDescarregamentoMDFe.MDF_CODIGO = Mdfe.MDF_CODIGO ");
        }

        private void SetarJoinsEmpresaSerie(StringBuilder joins)
        {
            if (!joins.Contains(" EmpresaSerie "))
                joins.Append("join T_EMPRESA_SERIE EmpresaSerie on EmpresaSerie.ESE_CODIGO = Mdfe.ESE_CODIGO ");
        }

        private void SetarJoinsCargaMDFe(StringBuilder joins)
        {
            if (!joins.Contains(" CargaMDFe "))
                joins.Append("left join T_CARGA_MDFE CargaMDFe on Mdfe.MDF_CODIGO = CargaMDFe.CMD_CODIGO ");
        }

        private void SetarJoinsCarga(StringBuilder joins)
        {
            SetarJoinsCargaMDFe(joins);

            if (!joins.Contains(" Carga "))
                joins.Append("join T_CARGA Carga on CargaMDFe.CAR_CODIGO = Carga.CAR_CODIGO ");
        }
        private void SetarJoinsMDFeMunicipioDescarregamento(StringBuilder joins)
        {
            if (!joins.Contains(" MDFeMunicipioDescarregamento "))
                joins.Append("left join T_MDFE_MUNICIPIO_DESCARREGAMENTO MDFeMunicipioDescarregamento on MDFeMunicipioDescarregamento.MDF_CODIGO = Mdfe.MDF_CODIGO ");
        }

        private void SetarJoinsMDFeMunicipioDescarregamentoDoc(StringBuilder joins)
        {
            if (!joins.Contains(" MDFeMunicipioDescarregamentoDoc "))
                joins.Append("left join T_MDFE_MUNICIPIO_DESCARREGAMENTO_DOC MDFeMunicipioDescarregamentoDoc on MDFeMunicipioDescarregamentoDoc.MDD_CODIGO = MDFeMunicipioDescarregamento.MDD_CODIGO ");
        }

        private void SetarJoinsCTe(StringBuilder joins)
        {
            if (!joins.Contains(" Cte "))
                joins.Append("left join T_CTE Cte on CTe.CON_CODIGO = MDFeMunicipioDescarregamentoDoc.CON_CODIGO ");
        }

        private void SetarJoinsTomadorPagadorCTe(StringBuilder joins)
        {
            if(!joins.Contains(" TomadorPagadorCTe "))
                joins.Append("left join T_CTE_PARTICIPANTE TomadorPagadorCTe ON TomadorPagadorCTe.PCT_CODIGO = Cte.CON_TOMADOR_PAGADOR_CTE ");   
        }

        private void SetarJoinsTomador(StringBuilder joins)
        {
            SetarJoinsMDFeMunicipioDescarregamento(joins);
            SetarJoinsMDFeMunicipioDescarregamentoDoc(joins);
            SetarJoinsCTe(joins);
            SetarJoinsTomadorPagadorCTe(joins);
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.MDFe.FiltroPesquisaMdfeRelatorio filtroPesquisa)
        {
            switch (propriedade)
            {   
                case "ChaveAcesso":
                    if (!select.Contains(" ChaveAcesso,"))
                    {
                        select.Append("Mdfe.MDF_CHAVE as ChaveAcesso, ");
                        groupBy.Append("Mdfe.MDF_CHAVE, ");
                    }
                    break;

                case "CnpjEmpresa":
                    if (!select.Contains(" CnpjEmpresa"))
                    {
                        select.Append("Empresa.EMP_CNPJ as CnpjEmpresa, ");
                        groupBy.Append("Empresa.EMP_CNPJ, ");

                        SetarJoinsEmpresa(joins);
                    }
                    break;

                case "Codigo":
                    if (!select.Contains(" Codigo,"))
                    {
                        select.Append("Mdfe.MDF_CODIGO as Codigo, ");
                        groupBy.Append("Mdfe.MDF_CODIGO, ");
                    }
                    break;

                case "DataAutorizacao":
                case "DataAutorizacaoFormatada":
                    if (!select.Contains(" DataAutorizacao"))
                    {
                        select.Append("Mdfe.MDF_DATA_AUTORIZACAO as DataAutorizacao, ");
                        groupBy.Append("Mdfe.MDF_DATA_AUTORIZACAO, ");
                    }
                    break;

                case "DataCancelamento":
                case "DataCancelamentoFormatada":
                    if (!select.Contains(" DataCancelamento"))
                    {
                        select.Append("Mdfe.MDF_DATA_CANCELAMENTO as DataCancelamento, ");
                        groupBy.Append("Mdfe.MDF_DATA_CANCELAMENTO, ");
                    }
                    break;

                case "DataEmissao":
                case "DataEmissaoFormatada":
                    if (!select.Contains(" DataEmissao"))
                    {
                        select.Append("Mdfe.MDF_DATA_EMISSAO as DataEmissao, ");
                        groupBy.Append("Mdfe.MDF_DATA_EMISSAO, ");
                    }
                    break;

                case "DataEncerramento":
                case "DataEncerramentoFormatada":
                    if (!select.Contains(" DataEncerramento"))
                    {
                        select.Append("Mdfe.MDF_DATA_ENCERRAMENTO as DataEncerramento, ");
                        groupBy.Append("Mdfe.MDF_DATA_ENCERRAMENTO, ");
                    }
                    break;

                case "JustificativaCancelamento":
                    if (!select.Contains(" JustificativaCancelamento,"))
                    {
                        select.Append("Mdfe.MDF_JUSTIFICATIVA_CANCELAMENTO as JustificativaCancelamento, ");
                        groupBy.Append("Mdfe.MDF_JUSTIFICATIVA_CANCELAMENTO, ");
                    }
                    break;

                case "MensagemRetornoSefaz":
                    if (!select.Contains(" MensagemRetornoSefaz,"))
                    {
                        select.Append("Mdfe.MDF_MENSAGEM_RETORNO_SEFAZ as MensagemRetornoSefaz, ");
                        groupBy.Append("Mdfe.MDF_MENSAGEM_RETORNO_SEFAZ, ");
                    }
                    break;

                case "Motoristas":
                    if (!select.Contains(" Motoristas"))
                    {
                        select.Append("isnull(SUBSTRING(( ");
                        select.Append("    select ', ' + Motorista.MDM_NOME ");
                        select.Append("      from T_MDFE_MOTORISTA Motorista ");
                        select.Append("     where Motorista.MDF_CODIGO = Mdfe.MDF_CODIGO ");
                        select.Append("       for XML PATH('') ");
                        select.Append("), 3, 1000), '') as Motoristas, ");

                        groupBy.Append("Mdfe.MDF_CODIGO, ");
                    }
                    break;

                case "Numero":
                    if (!select.Contains(" Numero,"))
                    {
                        select.Append("Mdfe.MDF_NUMERO as Numero, ");
                        groupBy.Append("Mdfe.MDF_NUMERO, ");
                    }
                    break;

                case "PesoBrutoMercadoria":
                case "PesoBrutoMercadoriaFormatado":
                    if (!select.Contains(" PesoBrutoMercadoria,"))
                    {
                        select.Append("Mdfe.MDF_PESO_BRUTO as PesoBrutoMercadoria, ");
                        select.Append("Mdfe.MDF_UNIDADE_MEDIDA as UnidadeMedidaMercadoria, ");

                        groupBy.Append("Mdfe.MDF_PESO_BRUTO, ");
                        groupBy.Append("Mdfe.MDF_UNIDADE_MEDIDA, ");
                    }
                    break;

                case "ProtocoloAutorizacao":
                    if (!select.Contains(" ProtocoloAutorizacao,"))
                    {
                        select.Append("Mdfe.MDF_PROTOCOLO as ProtocoloAutorizacao, ");
                        groupBy.Append("Mdfe.MDF_PROTOCOLO, ");
                    }
                    break;

                case "ProtocoloCancelamento":
                    if (!select.Contains(" ProtocoloCancelamento,"))
                    {
                        select.Append("Mdfe.MDF_PROTOCOLO_CANCELAMENTO as ProtocoloCancelamento, ");
                        groupBy.Append("Mdfe.MDF_PROTOCOLO_CANCELAMENTO, ");
                    }
                    break;

                case "ProtocoloEncerramento":
                    if (!select.Contains(" ProtocoloEncerramento,"))
                    {
                        select.Append("Mdfe.MDF_PROTOCOLO_ENCERRAMENTO as ProtocoloEncerramento, ");
                        groupBy.Append("Mdfe.MDF_PROTOCOLO_ENCERRAMENTO, ");
                    }
                    break;

                case "RazaoSocialEmpresa":
                    if (!select.Contains(" RazaoSocialEmpresa"))
                    {
                        select.Append("Empresa.EMP_RAZAO as RazaoSocialEmpresa, ");
                        groupBy.Append("Empresa.EMP_RAZAO, ");

                        SetarJoinsEmpresa(joins);
                    }
                    break;

                case "Serie":
                    if (!select.Contains(" Serie"))
                    {
                        select.Append("EmpresaSerie.ESE_NUMERO as Serie, ");
                        groupBy.Append("EmpresaSerie.ESE_NUMERO, ");

                        SetarJoinsEmpresaSerie(joins);
                    }
                    break;

                case "StatusMdfe":
                case "StatusMdfeDescricao":
                    if (!select.Contains(" StatusMdfe"))
                    {
                        select.Append("Mdfe.MDF_STATUS as StatusMdfe, ");
                        groupBy.Append("Mdfe.MDF_STATUS, ");
                    }
                    break;

                case "UfCarregamento":
                    if (!select.Contains(" UfCarregamento,"))
                    {
                        select.Append("Mdfe.UF_CARREGAMENTO as UfCarregamento, ");
                        groupBy.Append("Mdfe.UF_CARREGAMENTO, ");
                    }
                    break;

                case "UfDescarregamento":
                    if (!select.Contains(" UfDescarregamento,"))
                    {
                        select.Append("Mdfe.UF_DESCARREGAMENTO as UfDescarregamento, ");
                        groupBy.Append("Mdfe.UF_DESCARREGAMENTO, ");
                    }
                    break;

                case "ValorTotalMercadoria":
                    if (!select.Contains(" ValorTotalMercadoria,"))
                    {
                        select.Append("Mdfe.MDF_VALOR_TOTAL as ValorTotalMercadoria, ");
                        groupBy.Append("Mdfe.MDF_VALOR_TOTAL, ");
                    }
                    break;

                case "Veiculos":
                    if (!select.Contains(" Veiculos"))
                    {
                        select.Append("isnull(SUBSTRING(( ");
                        select.Append("    select ', ' + Veiculo.MDV_PLACA ");
                        select.Append("      from T_MDFE_VEICULO Veiculo ");
                        select.Append("     where Veiculo.MDF_CODIGO = Mdfe.MDF_CODIGO ");
                        select.Append("       for XML PATH('') ");
                        select.Append("), 3, 1000), '') as Veiculos, ");

                        if (!groupBy.Contains("Mdfe.MDF_CODIGO"))
                            groupBy.Append("Mdfe.MDF_CODIGO, ");
                    }
                    break;

                case "TipoOperacao":
                    if (!select.Contains(" TipoOperacao,"))
                    {
                        select.Append("isnull(SUBSTRING(( ");
                        select.Append("    select ', ' + TipoOperacao.TOP_DESCRICAO ");
                        select.Append("      from T_CARGA_MDFE CargaMDFe ");
                        select.Append("      JOIN T_CARGA Carga ON Carga.CAR_CODIGO = CargaMDFe.CAR_CODIGO ");
                        select.Append("      JOIN T_TIPO_OPERACAO TipoOperacao ON TipoOperacao.TOP_CODIGO = Carga.TOP_CODIGO ");
                        select.Append("     where CargaMDFe.MDF_CODIGO = Mdfe.MDF_CODIGO ");
                        select.Append("       for XML PATH('') ");
                        select.Append("), 3, 1000), '') as TipoOperacao, ");

                        if (!groupBy.Contains("Mdfe.MDF_CODIGO"))
                            groupBy.Append("Mdfe.MDF_CODIGO, ");
                    }
                    break;

                case "NumeroCarga":
                    if (!select.Contains(" NumeroCarga,"))
                    {
                        select.Append("isnull(SUBSTRING(( ");
                        select.Append("    select ', ' + Carga.CAR_CODIGO_CARGA_EMBARCADOR ");
                        select.Append("      from T_CARGA_MDFE CargaMDFe ");
                        select.Append("      JOIN T_CARGA Carga ON Carga.CAR_CODIGO = CargaMDFe.CAR_CODIGO ");
                        select.Append("     where CargaMDFe.MDF_CODIGO = Mdfe.MDF_CODIGO ");
                        select.Append("       for XML PATH('') ");
                        select.Append("), 3, 1000), '') as NumeroCarga, ");

                        if (!groupBy.Contains("Mdfe.MDF_CODIGO"))
                            groupBy.Append("Mdfe.MDF_CODIGO, ");
                    }
                    break;

                case "CTes":
                    if (!select.Contains(" CTes, "))
                    {
                        select.Append("isnull(SUBSTRING(( ");
                        select.Append("    select ', ' + CAST(CTe.CON_NUM AS VARCHAR(11)) ");
                        select.Append("      from T_CTE CTe ");
                        select.Append("      JOIN T_MDFE_MUNICIPIO_DESCARREGAMENTO_DOC MunicipioDescarregamentoDocMDFe ON MunicipioDescarregamentoDocMDFe.CON_CODIGO = CTe.CON_CODIGO ");
                        select.Append("      JOIN T_MDFE_MUNICIPIO_DESCARREGAMENTO MunicipioDescarregamentoMDFe ON MunicipioDescarregamentoMDFe.MDD_CODIGO = MunicipioDescarregamentoDocMDFe.MDD_CODIGO ");
                        select.Append("     where MunicipioDescarregamentoMDFe.MDF_CODIGO = Mdfe.MDF_CODIGO ");
                        select.Append("       for XML PATH('') ");
                        select.Append("), 3, 1000), '') as CTes, ");

                        if (!groupBy.Contains("Mdfe.MDF_CODIGO"))
                            groupBy.Append("Mdfe.MDF_CODIGO, ");
                    }
                    break;

                case "ValorFrete":
                    if (!select.Contains(" ValorFrete, "))
                    {
                        select.Append("(SELECT SUM(CTe.CON_VALOR_FRETE) FROM T_CTE CTe inner join T_MDFE_MUNICIPIO_DESCARREGAMENTO_DOC MDFeMunicipioDescarregamentoDoc on MDFeMunicipioDescarregamentoDoc.CON_CODIGO = CTe.CON_CODIGO inner join T_MDFE_MUNICIPIO_DESCARREGAMENTO MDFeMunicipioDescarregamento on MDFeMunicipioDescarregamento.MDD_CODIGO = MDFeMunicipioDescarregamentoDoc.MDD_CODIGO WHERE MDFeMunicipioDescarregamento.MDF_CODIGO = Mdfe.MDF_CODIGO) ValorFrete, ");

                        if (!groupBy.Contains("Mdfe.MDF_CODIGO"))
                            groupBy.Append("Mdfe.MDF_CODIGO, ");
                    }
                    break;

                case "ValorReceber":
                    if (!select.Contains(" ValorReceber, "))
                    {
                        select.Append("(SELECT SUM(CTe.CON_VALOR_RECEBER) FROM T_CTE CTe inner join T_MDFE_MUNICIPIO_DESCARREGAMENTO_DOC MDFeMunicipioDescarregamentoDoc on MDFeMunicipioDescarregamentoDoc.CON_CODIGO = CTe.CON_CODIGO inner join T_MDFE_MUNICIPIO_DESCARREGAMENTO MDFeMunicipioDescarregamento on MDFeMunicipioDescarregamento.MDD_CODIGO = MDFeMunicipioDescarregamentoDoc.MDD_CODIGO WHERE MDFeMunicipioDescarregamento.MDF_CODIGO = Mdfe.MDF_CODIGO) ValorReceber, ");

                        if (!groupBy.Contains("Mdfe.MDF_CODIGO"))
                            groupBy.Append("Mdfe.MDF_CODIGO, ");
                    }
                    break;


                case "MunicipioDescarregamento":
                    if (!select.Contains(" MunicipioDescarregamento, "))
                    {
                        select.Append("SUBSTRING( ");
                        select.Append("    (SELECT ', ' + Localidade.LOC_DESCRICAO ");
                        select.Append("      FROM T_LOCALIDADES Localidade INNER JOIN T_MDFE_MUNICIPIO_DESCARREGAMENTO MDFeMunicipioDescarregamento ");
                        select.Append("      ON MDFeMunicipioDescarregamento.LOC_CODIGO = Localidade.LOC_CODIGO ");
                        select.Append("      WHERE MDFeMunicipioDescarregamento.MDF_CODIGO = Mdfe.MDF_CODIGO ");
                        select.Append("      FOR XML PATH('')), 3, 1000) MunicipioDescarregamento, ");

                        groupBy.Append("Mdfe.MDF_CODIGO, ");
                    }
                    break;

                case "CPFMotoristaFormatado":
                    if (!select.Contains(" CPFMotorista"))
                    {
                        select.Append("isnull(SUBSTRING(( ");
                        select.Append("    select ', ' + CPFMotorista.MDM_CPF ");
                        select.Append("      from T_MDFE_MOTORISTA CPFMotorista ");
                        select.Append("     where CPFMotorista.MDF_CODIGO = Mdfe.MDF_CODIGO ");
                        select.Append("       for XML PATH('') ");
                        select.Append("), 3, 1000), '') as CPFMotorista, ");

                        groupBy.Append("Mdfe.MDF_CODIGO, ");
                    }
                    break;

                case "CPFCNPJTomadorFormatado":
                    if (!select.Contains(" CPFCNPJTomador, "))
                    {
                        select.Append("TomadorPagadorCTe.PCT_CPF_CNPJ CPFCNPJTomador, ");

                        if (!groupBy.Contains("TomadorPagadorCTe.PCT_CPF_CNPJ"))
                            groupBy.Append("TomadorPagadorCTe.PCT_CPF_CNPJ, ");

                        SetarJoinsTomador(joins);
                    }
                    break;

                case "Tomador":
                    if (!select.Contains(" Tomador, "))
                    {
                        select.Append("TomadorPagadorCTe.PCT_NOME Tomador, ");

                        if (!groupBy.Contains("TomadorPagadorCTe.PCT_NOME"))
                            groupBy.Append("TomadorPagadorCTe.PCT_NOME, ");

                        SetarJoinsTomador(joins);
                    }
                    break;
            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.MDFe.FiltroPesquisaMdfeRelatorio filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            string pattern = "yyyy-MM-dd";

            if (filtrosPesquisa.CodigoEmpresa > 0)
                where.Append($" and Mdfe.EMP_CODIGO = {filtrosPesquisa.CodigoEmpresa}");

            if (filtrosPesquisa.CodigoSerie > 0)
                where.Append($" and Mdfe.ESE_CODIGO = {filtrosPesquisa.CodigoSerie}");

            if (filtrosPesquisa.DataAutorizacaoInicial.HasValue)
                where.Append($" and CAST(Mdfe.MDF_DATA_AUTORIZACAO AS DATE) >= '{filtrosPesquisa.DataAutorizacaoInicial.Value.Date.ToString(pattern)}'");

            if (filtrosPesquisa.DataAutorizacaoLimite.HasValue)
                where.Append($" and CAST(Mdfe.MDF_DATA_AUTORIZACAO AS DATE) <= '{filtrosPesquisa.DataAutorizacaoLimite.Value.ToString(pattern)}'");

            if (filtrosPesquisa.DataCancelamentoInicial.HasValue)
                where.Append($" and CAST(Mdfe.MDF_DATA_CANCELAMENTO AS DATE) >= '{filtrosPesquisa.DataCancelamentoInicial.Value.Date.ToString(pattern)}'");

            if (filtrosPesquisa.DataCancelamentoLimite.HasValue)
                where.Append($" and CAST(Mdfe.MDF_DATA_CANCELAMENTO AS DATE) <= '{filtrosPesquisa.DataCancelamentoLimite.Value.ToString(pattern)}'");

            if (filtrosPesquisa.DataEmissaoInicial.HasValue)
                where.Append($" and CAST(Mdfe.MDF_DATA_EMISSAO AS DATE) >= '{filtrosPesquisa.DataEmissaoInicial.Value.Date.ToString(pattern)}'");

            if (filtrosPesquisa.DataEmissaoLimite.HasValue)
                where.Append($" and CAST(Mdfe.MDF_DATA_EMISSAO AS DATE) <= '{filtrosPesquisa.DataEmissaoLimite.Value.ToString(pattern)}'");

            if (filtrosPesquisa.DataEncerramentoInicial.HasValue)
                where.Append($" and CAST(Mdfe.MDF_DATA_ENCERRAMENTO AS DATE) >= '{filtrosPesquisa.DataEncerramentoInicial.Value.Date.ToString(pattern)}'");

            if (filtrosPesquisa.DataEncerramentoLimite.HasValue)
                where.Append($" and CAST(Mdfe.MDF_DATA_ENCERRAMENTO AS DATE) <= '{filtrosPesquisa.DataEncerramentoLimite.Value.ToString(pattern)}'");

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.EstadoCarregamento))
                where.Append($" and Mdfe.UF_CARREGAMENTO = '{filtrosPesquisa.EstadoCarregamento}'");

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.EstadoDescarregamento))
                where.Append($" and Mdfe.UF_DESCARREGAMENTO = '{filtrosPesquisa.EstadoDescarregamento}'");

            if (filtrosPesquisa.NumeroInicial > 0)
                where.Append($" and Mdfe.MDF_NUMERO >= {filtrosPesquisa.NumeroInicial}");

            if (filtrosPesquisa.NumeroLimite > 0)
                where.Append($" and Mdfe.MDF_NUMERO <= {filtrosPesquisa.NumeroLimite}");

            if (filtrosPesquisa.ListaStatusMdfe?.Count() > 0)
                where.Append($" and Mdfe.MDF_STATUS in ({string.Join(", ", (from status in filtrosPesquisa.ListaStatusMdfe select (int)status))})");

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.CpfMotorista))
            {
                where.Append(" and exists( ");
                where.Append("     select 1 ");
                where.Append("       from T_MDFE_MOTORISTA Motorista ");
                where.Append("      where Motorista.MDF_CODIGO = Mdfe.MDF_CODIGO ");
                where.Append($"       and Motorista.MDM_CPF = '{filtrosPesquisa.CpfMotorista}' ");
                where.Append(" ) ");
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.PlacaVeiculo))
            {
                where.Append(" and exists( ");
                where.Append("     select 1 ");
                where.Append("       from T_MDFE_VEICULO Veiculo ");
                where.Append("      where Veiculo.MDF_CODIGO = Mdfe.MDF_CODIGO ");
                where.Append($"       and Veiculo.MDV_PLACA = '{filtrosPesquisa.PlacaVeiculo}' ");
                where.Append(" ) ");
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroCarga))
            {
                where.Append($" and Mdfe.MDF_CODIGO in (SELECT DISTINCT CargaMDFe.MDF_CODIGO " +
                                $"  FROM T_CARGA_MDFE CargaMDFe" +
                                $"  JOIN T_CARGA Carga ON Carga.CAR_CODIGO = CargaMDFe.CAR_CODIGO" +
                                $"  WHERE Carga.CAR_CODIGO_CARGA_EMBARCADOR = '{filtrosPesquisa.NumeroCarga}')");
            }

            if (filtrosPesquisa.NumeroCTe > 0)
            {
                where.Append($" and Mdfe.MDF_CODIGO in (SELECT DISTINCT MunicipioDescarregamentoMDFe.MDF_CODIGO " +
                                $"  FROM T_MDFE_MUNICIPIO_DESCARREGAMENTO MunicipioDescarregamentoMDFe" +
                                $"  JOIN T_MDFE_MUNICIPIO_DESCARREGAMENTO_DOC MunicipioDescarregamentoDocMDFe ON MunicipioDescarregamentoDocMDFe.MDD_CODIGO = MunicipioDescarregamentoMDFe.MDD_CODIGO" +
                                $"  JOIN T_CTE CTe ON CTe.CON_CODIGO = MunicipioDescarregamentoDocMDFe.CON_CODIGO" +
                                $"  WHERE CTe.CON_NUM = '{filtrosPesquisa.NumeroCTe}')");
            }

            if (filtrosPesquisa.TipoOperacao > 0)
            {
                where.Append($" and Mdfe.MDF_CODIGO in (SELECT DISTINCT CargaMDFe.MDF_CODIGO " +
                                $"  FROM T_CARGA_MDFE CargaMDFe" +
                                $"  JOIN T_CARGA Carga ON Carga.CAR_CODIGO = CargaMDFe.CAR_CODIGO" +
                                $"  JOIN T_TIPO_OPERACAO TipoOperacao ON TipoOperacao.TOP_CODIGO = Carga.TOP_CODIGO" +
                                $"  WHERE TipoOperacao.TOP_CODIGO = '{filtrosPesquisa.TipoOperacao}')");
            }

            if (filtrosPesquisa.MDFeVinculadoACarga.HasValue)
                where.Append("  and Mdfe.MDF_CODIGO " + (filtrosPesquisa.MDFeVinculadoACarga.Value ? "in" : "not in") +
                    " (select CargaMDFe.MDF_CODIGO from T_CARGA_MDFE CargaMDFe where CargaMDFe.MDF_CODIGO = Mdfe.MDF_CODIGO) ");

            if (filtrosPesquisa.MunicipioDescarregamento > 0)
            {
                where.Append($" and MunicipioDescarregamentoMDFe.LOC_CODIGO = {filtrosPesquisa.MunicipioDescarregamento.ToString()}");

                SetarJoinsMunicipios(joins);
            }

            if (filtrosPesquisa.CodigosFiliais.Any(codigo => codigo == -1))
            {
                SetarJoinsCarga(joins);

                where.Append($@" and (Carga.FIL_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosFiliais)}) OR EXISTS (   SELECT _cargaPedidoRecebedor.CAR_CODIGO 
                                                                                                                       FROM T_CARGA_PEDIDO _cargaPedidoRecebedor 
                                                                                                                       LEFT JOIN T_PEDIDO _pedido ON _pedido.PED_CODIGO = _cargaPedidoRecebedor.PED_CODIGO
                                                                                                                       WHERE Carga.CAR_CODIGO = _cargaPedidoRecebedor.CAR_CODIGO
                                                                                                                       AND _pedido.CLI_CODIGO_RECEBEDOR IN ({string.Join(",", filtrosPesquisa.CodigosRecebedores)})))");
            }
        }

        #endregion

        #region Métodos Públicos

        public string ObterSqlPesquisaConhecimentosDeFrete(Dominio.ObjetosDeValor.Embarcador.MDFe.FiltroPesquisaMdfeRelatorio filtrosPesquisa)
        {
            StringBuilder groupBy = new StringBuilder();
            StringBuilder joins = new StringBuilder();
            StringBuilder sql = new StringBuilder();
            StringBuilder where = new StringBuilder();

            SetarWhere(filtrosPesquisa, where, joins, groupBy);

            sql.Append(@"SELECT         Mdfe.MDF_CODIGO CodigoMDFe,         
			                            CON_NUM Numero,
			                            ESE_NUMERO Serie,
			                            CON_DATAHORAEMISSAO DataHoraEmissao,
			                            CON_CHAVECTE Chave,
			                            CON_STATUS Status,
			                            CON_VALOR_RECEBER ValorReceber
                        FROM   T_MDFE Mdfe
                                JOIN T_MDFE_MUNICIPIO_DESCARREGAMENTO MunicipioDescarregamentoMDFe ON Mdfe.MDF_CODIGO = MunicipioDescarregamentoMDFe.MDF_CODIGO
                                JOIN T_MDFE_MUNICIPIO_DESCARREGAMENTO_DOC MunicipioDescarregamentoDocMDFe ON MunicipioDescarregamentoMDFe.MDD_CODIGO = MunicipioDescarregamentoDocMDFe.MDD_CODIGO 
                                JOIN T_CTE CTe ON MunicipioDescarregamentoDocMDFe.CON_CODIGO = CTe.CON_CODIGO 
	                            JOIN T_EMPRESA_SERIE EmpresaSerie ON CTe.CON_SERIE = EmpresaSerie.ESE_CODIGO");
            sql.Append(joins.ToString());

            if (where.Length > 0)
                sql.Append($" where {where.ToString().Trim().Substring(3)} ");

            return sql.ToString();
        }

        #endregion
    }
}
