using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.RH.Consulta
{
    sealed class ConsultaComissaoFuncionario : Embarcador.Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.RH.FiltroPesquisaDocumenotosComissaoFuncionario>
    {
        #region Construtores

        public ConsultaComissaoFuncionario() : base(tabela: "T_COMISSAO_FUNCIONARIO_MOTORISTA_DOCUMENTO ComissaoDocumento") { }

        #endregion

        #region Métodos Privados

        private void SetarJoinsMotorista(StringBuilder joins) {

            SetarJoinsComissaoMotorista(joins);
            if (!joins.Contains(" Motorista "))
                joins.Append(" join T_FUNCIONARIO Motorista on Motorista.FUN_CODIGO = ComissaoMotorista.FUN_CODIGO_MOTORISTA  ");
        }

        private void SetarJoinsComissaoMotorista(StringBuilder joins)
        {
            if (!joins.Contains(" ComissaoMotorista "))
                joins.Append(" join T_COMISSAO_FUNCIONARIO_MOTORISTA ComissaoMotorista on ComissaoMotorista.CFM_CODIGO = ComissaoDocumento.CFM_CODIGO");
        }        
        
        private void SetarJoinsCargaSimarizada(StringBuilder joins)
        {
            if (!joins.Contains(" CargaSimarizada "))
                joins.Append(" left join T_CARGA_DADOS_SUMARIZADOS CargaSimarizada on CargaSimarizada.CDS_CODIGO = ComissaoDocumento.CDS_CODIGO");
        }        
        
        private void SetarJoinsConfiguracaoComissao(StringBuilder joins)
        {
            if (!joins.Contains(" ConfiguracaoComissao "))
                joins.Append(" cross join T_CONFIGURACAO_COMISSAO_MOTORISTA ConfiguracaoComissao");
        }       
        
        private void SetarJoinsOcorrencia(StringBuilder joins)
        {
            if (!joins.Contains(" Ocorrencia "))
                joins.Append(" left join T_CARGA_OCORRENCIA Ocorrencia on Ocorrencia.COC_CODIGO = ComissaoDocumento.COC_CODIGO");
        }

        private void SetarJoinsComissao(StringBuilder joins)
        {
            SetarJoinsComissaoMotorista(joins);
            if (!joins.Contains(" Comissao "))
                joins.Append(" join T_COMISSAO_FUNCIONARIO Comissao on Comissao.CMF_CODIGO = ComissaoMotorista.CMF_CODIGO  ");
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.RH.FiltroPesquisaDocumenotosComissaoFuncionario filtrosPesquisa)
        {
            switch (propriedade)
            {

                case "Numero":
                    if (!select.Contains(" Numero, "))
                    {
                        select.Append("ComissaoDocumento.CFM_NUMERO as Numero, ");
                        if (!groupBy.Contains("ComissaoDocumento.CFM_NUMERO"))
                            groupBy.Append("ComissaoDocumento.CFM_NUMERO, ");
                    }
                    break;
                case "Motorista":
                    if (!select.Contains(" Motorista, "))
                    {
                        select.Append("Motorista.FUN_NOME as Motorista, ");
                        if (!groupBy.Contains("Motorista.FUN_NOME"))
                            groupBy.Append("Motorista.FUN_NOME, ");
                        SetarJoinsMotorista(joins);
                    }
                    break;
                case "CPF":
                    if (!select.Contains(" CPF, "))
                    {
                        select.Append("Motorista.FUN_CPF as CPF, ");
                        if (!groupBy.Contains("Motorista.FUN_CPF"))
                            groupBy.Append("Motorista.FUN_CPF, ");
                        SetarJoinsMotorista(joins);
                    }
                    break;               
                case "Destino":
                    if (!select.Contains(" Destino, "))
                    {
                        select.Append("CargaSimarizada.CDS_ORIGENS as Destino, ");
                        if (!groupBy.Contains("CargaSimarizada.CDS_ORIGENS"))
                            groupBy.Append("CargaSimarizada.CDS_ORIGENS, ");
                        SetarJoinsCargaSimarizada(joins);
                    }
                    break;              
                case "Origem":
                    if (!select.Contains(" Origem, "))
                    {
                        select.Append("CargaSimarizada.CDS_DESTINOS as Origem, ");
                        if (!groupBy.Contains("CargaSimarizada.CDS_DESTINOS"))
                            groupBy.Append("CargaSimarizada.CDS_DESTINOS, ");
                        SetarJoinsCargaSimarizada(joins);
                    }
                    break;
                case "ValoFreteLiquido":
                    if (!select.Contains(" ValoFreteLiquido, "))
                    {
                        select.Append("ComissaoDocumento.CMD_VALOR_FRETE_LIQUIDO as ValoFreteLiquido, ");
                        if (!groupBy.Contains("ComissaoDocumento.CMD_VALOR_FRETE_LIQUIDO"))
                            groupBy.Append("ComissaoDocumento.CMD_VALOR_FRETE_LIQUIDO, ");                        
                    }
                    break;
                case "ValorComissao":
                    if (!select.Contains(" ValorComissao, "))
                    {
                        select.Append("ComissaoDocumento.CMD_VALOR_COMISSAO as ValorComissao, ");
                        if (!groupBy.Contains("ComissaoDocumento.CMD_VALOR_COMISSAO"))
                            groupBy.Append("ComissaoDocumento.CMD_VALOR_COMISSAO, ");
                    }
                    break;
                case "Periodo":
                    if (!select.Contains(" Periodo, "))
                    {
                        select.Append(@"concat(FORMAT(Comissao.CMF_DATA_INICIO, 'dd/MM/yyyy'),' - ', FORMAT(Comissao.CMF_DATA_FIM, 'dd/MM/yyyy'))as Periodo, ");
                        if (!groupBy.Contains("Comissao.CMF_DATA_INICIO"))
                            groupBy.Append("Comissao.CMF_DATA_INICIO, ");
                        if (!groupBy.Contains("Comissao.CMF_DATA_FIM"))
                            groupBy.Append("Comissao.CMF_DATA_FIM, ");
                        SetarJoinsComissao(joins);
                    }
                    break;
                case "DataCarregaementoEmissao":
                    if (!select.Contains(" DataCarregaementoEmissao, "))
                    {
                        select.Append(@"(case when 
	                                       ConfiguracaoComissao.CCM_DATA_BASE = 2 
		                                 then (select FORMAT(ca.CAR_DATA_CARREGAMENTO, 'dd/MM/yyyy') from T_CARGA ca where ca.CAR_CODIGO = coalesce(ComissaoDocumento.CAR_CODIGO, Ocorrencia.CAR_CODIGO))
		                                 else (select FORMAT(max(cte.CON_DATAHORAEMISSAO), 'dd/MM/yyyy')
				                                 from T_CARGA_CTE ccet 
				                                inner join T_CTE cte
				                                   on cte.CON_CODIGO = ccet.CON_CODIGO 
				                                where ccet.CAR_CODIGO = coalesce(ComissaoDocumento.CAR_CODIGO, Ocorrencia.CAR_CODIGO))
		                                 end) as DataCarregaementoEmissao   , ");

                        if (!groupBy.Contains("ConfiguracaoComissao.CCM_DATA_BASE"))
                            groupBy.Append("ConfiguracaoComissao.CCM_DATA_BASE, ");
                        if (!groupBy.Contains("ComissaoDocumento.CAR_CODIGO"))
                            groupBy.Append("ComissaoDocumento.CAR_CODIGO, ");
                        if (!groupBy.Contains("Ocorrencia.CAR_CODIGO"))
                            groupBy.Append("Ocorrencia.CAR_CODIGO, ");
                        SetarJoinsConfiguracaoComissao(joins);
                        SetarJoinsOcorrencia(joins);
                    }
                    break;
                case "Documentos":
                    if (!select.Contains(" Documentos, "))
                    {
                        select.Append(@"(select STRING_AGG(coalesce(cte.CON_NUMERO_CRT, cte.CON_NUM), ', ') as numero
		                                   from T_CARGA_CTE ccet 
		                                  inner join T_CTE cte
			                                 on cte.CON_CODIGO = ccet.CON_CODIGO 
		                                  where ccet.CAR_CODIGO = coalesce(ComissaoDocumento.CAR_CODIGO, Ocorrencia.CAR_CODIGO)) as Documentos, ");

                        if (!groupBy.Contains("ComissaoDocumento.CAR_CODIGO"))
                            groupBy.Append("ComissaoDocumento.CAR_CODIGO, ");
                        if (!groupBy.Contains("Ocorrencia.CAR_CODIGO"))
                            groupBy.Append("Ocorrencia.CAR_CODIGO, ");
                        SetarJoinsOcorrencia(joins);
                    }
                    break;

            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.RH.FiltroPesquisaDocumenotosComissaoFuncionario filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            string pattern = "yyyy-MM-dd";


            if (filtrosPesquisa.Motorista.Count > 0)
            {
                where.Append($" and Motorista.FUN_CODIGO in( {String.Join(",", filtrosPesquisa.Motorista)} )");
                SetarJoinsMotorista(joins);
            }

            if (filtrosPesquisa.DataInicial != DateTime.MinValue)
            {
                where.Append($" and CAST(Comissao.CMF_DATA_INICIO AS DATE) >= '{filtrosPesquisa.DataInicial.ToString(pattern)}' ");
                SetarJoinsComissao(joins);
            }

            if (filtrosPesquisa.DataFinal != DateTime.MinValue)
            {
                where.Append($" and CAST(Comissao.CMF_DATA_FIM AS DATE) <= '{filtrosPesquisa.DataFinal.ToString(pattern)}' ");
                SetarJoinsComissao(joins);
            }
        }

        #endregion
    }
}
