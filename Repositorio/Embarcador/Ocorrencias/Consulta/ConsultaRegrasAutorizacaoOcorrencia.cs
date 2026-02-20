using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Ocorrencias
{
    sealed class ConsultaRegrasAutorizacaoOcorrencia : Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaRelatorioRegrasAutorizacaoOcorrencia>
    {
        #region Construtores

        public ConsultaRegrasAutorizacaoOcorrencia() : base(tabela: "T_REGRAS_AUTORIZACAO_OCORRENCIA as Regra") { }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaRelatorioRegrasAutorizacaoOcorrencia filtroPesquisa)
        {
            switch (propriedade)
            {
                case "Codigo":
                    if (!select.Contains(" Codigo,"))
                    {
                        select.Append("Regra.RAO_CODIGO as Codigo, ");
                        groupBy.Append("Regra.RAO_CODIGO, ");
                    }
                    break;

                case "Ativo":
                case "AtivoDescricao":
                    if (!select.Contains(" Ativo,"))
                    {
                        select.Append("Regra.RAO_ATIVO as Ativo, ");
                        groupBy.Append("Regra.RAO_ATIVO, ");
                    }
                    break;

                case "DataVigencia":
                case "DataVigenciaFormatada":
                    if (!select.Contains(" DataVigencia,"))
                    {
                        select.Append("Regra.RAO_VIGENCIA as DataVigencia, ");
                        groupBy.Append("Regra.RAO_VIGENCIA, ");
                    }
                    break;

                case "Descricao":
                    if (!select.Contains(" Descricao,"))
                    {
                        select.Append("Regra.RAO_DESCRICAO as Descricao, ");
                        groupBy.Append("Regra.RAO_DESCRICAO, ");
                    }
                    break;

                case "DiasPrazoAprovacao":
                case "DiasPrazoAprovacaoFormatado":
                    if (!select.Contains(" DiasPrazoAprovacao,"))
                    {
                        select.Append("Regra.RAO_DIAS_PRAZO_APROVACAO as DiasPrazoAprovacao, ");
                        groupBy.Append("Regra.RAO_DIAS_PRAZO_APROVACAO, ");
                    }
                    break;

                case "EtapaAutorizacao":
                case "EtapaAutorizacaoDescricao":
                    if (!select.Contains(" EtapaAutorizacao,"))
                    {
                        select.Append("Regra.RAO_ETAPA_AUTORIZACAO as EtapaAutorizacao, ");
                        groupBy.Append("Regra.RAO_ETAPA_AUTORIZACAO, ");
                    }
                    break;

                case "NumeroAprovadores":
                    if (!select.Contains(" NumeroAprovadores, "))
                    {
                        select.Append("Regra.RAO_NUMERO_APROVADORES as NumeroAprovadores, ");
                        groupBy.Append("Regra.RAO_NUMERO_APROVADORES, ");
                    }
                    break;

                case "NumeroReprovadores":
                    if (!select.Contains(" NumeroReprovadores, "))
                    {
                        select.Append("Regra.RAO_NUMERO_REPROVADORES as NumeroReprovadores, ");
                        groupBy.Append("Regra.RAO_NUMERO_REPROVADORES, ");
                    }
                    break;

                case "PrioridadeAprovacao":
                    if (!select.Contains(" PrioridadeAprovacao,"))
                    {
                        select.Append("Regra.RAO_PRIORIDADE_APROVACAO as PrioridadeAprovacao, ");
                        groupBy.Append("Regra.RAO_PRIORIDADE_APROVACAO, ");
                    }
                    break;

                case "Observacao":
                    if (!select.Contains(" Observacao,"))
                    {
                        select.Append("Regra.RAO_OBSERVACOES as Observacao, ");
                        groupBy.Append("Regra.RAO_OBSERVACOES, ");
                    }
                    break;

                case "TipoOcorrencia":
                    if (!select.Contains(" TipoOcorrencia, "))
                    {
                        select.Append(@"RTRIM(LTRIM(SUBSTRING((SELECT DISTINCT ' / ' +  ocorrencia.OCO_DESCRICAO
	                                               FROM T_REGRA_OCORRENCIA_TIPO_OCORRENCIA regraTipoOcorrencia
		                                            JOIN T_OCORRENCIA ocorrencia ON ocorrencia.OCO_CODIGO = regraTipoOcorrencia.OCO_CODIGO
	                                               WHERE regraTipoOcorrencia.RAO_CODIGO = Regra.RAO_CODIGO FOR XML PATH('')), 3, 1000))) TipoOcorrencia, ");

                        if (!groupBy.Contains("Regra.RAO_CODIGO,"))
                            groupBy.Append("Regra.RAO_CODIGO, ");
                    }
                    break;

                case "ValorOcorrencia":
                    if (!select.Contains(" ValorOcorrencia, "))
                    {
                        select.Append(@"RTRIM(LTRIM(SUBSTRING((SELECT DISTINCT ' / ' +  FORMAT(regraValor.RVO_VALOR, 'N2', 'pt-BR')
	                                               FROM T_REGRA_OCORRENCIA_VALOR_OCORRENCIA regraValor
	                                               WHERE regraValor.RAO_CODIGO = Regra.RAO_CODIGO FOR XML PATH('')), 3, 1000))) ValorOcorrencia, ");

                        if (!groupBy.Contains("Regra.RAO_CODIGO,"))
                            groupBy.Append("Regra.RAO_CODIGO, ");
                    }
                    break;

                case "TipoOperacao":
                    if (!select.Contains(" TipoOperacao, "))
                    {
                        select.Append(@"RTRIM(LTRIM(SUBSTRING((SELECT DISTINCT ' / ' +  tipoOperacao.TOP_DESCRICAO
	                                               FROM T_REGRA_OCORRENCIA_TIPO_OPERACAO regraTipoOperacao
		                                            JOIN T_TIPO_OPERACAO tipoOperacao ON tipoOperacao.TOP_CODIGO = regraTipoOperacao.TOP_CODIGO
	                                               WHERE regraTipoOperacao.RAO_CODIGO = Regra.RAO_CODIGO FOR XML PATH('')), 3, 1000))) TipoOperacao, ");

                        if (!groupBy.Contains("Regra.RAO_CODIGO,"))
                            groupBy.Append("Regra.RAO_CODIGO, ");
                    }
                    break;

                case "Aprovadores":
                    if (!select.Contains(" Aprovadores, "))
                    {
                        select.Append(@"RTRIM(LTRIM(SUBSTRING((SELECT DISTINCT ' / ' +  funcionario.FUN_NOME
	                                               FROM T_REGRAS_OCORRENCIA_FUNCIONARIOS regraFuncionario
		                                            JOIN T_FUNCIONARIO funcionario ON funcionario.FUN_CODIGO = regraFuncionario.FUN_CODIGO
	                                               WHERE regraFuncionario.RAO_CODIGO = Regra.RAO_CODIGO FOR XML PATH('')), 3, 1000))) Aprovadores, ");

                        if (!groupBy.Contains("Regra.RAO_CODIGO,"))
                            groupBy.Append("Regra.RAO_CODIGO, ");
                    }
                    break;
            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaRelatorioRegrasAutorizacaoOcorrencia filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            string pattern = "yyyy-MM-dd";

            if (filtrosPesquisa.DataVgenciaInicial.HasValue)
                where.Append($" and CAST(Regra.RAO_VIGENCIA AS DATE) >= '{filtrosPesquisa.DataVgenciaInicial.Value.ToString(pattern)}'");

            if (filtrosPesquisa.DataVigenciaLimite.HasValue)
                where.Append($" and CAST(Regra.RAO_VIGENCIA AS DATE) <= '{filtrosPesquisa.DataVigenciaLimite.Value.ToString(pattern)}'");

            if (filtrosPesquisa.EtapaAutorizacao.HasValue)
                where.Append($" and Regra.RAO_ETAPA_AUTORIZACAO = {(int)filtrosPesquisa.EtapaAutorizacao.Value}");

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Descricao))
                where.Append($" and Regra.RAO_DESCRICAO like '%{filtrosPesquisa.Descricao}%'");

            if (filtrosPesquisa.Ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                where.Append($" and Regra.RAO_ATIVO = 1");
            else if (filtrosPesquisa.Ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                where.Append($" and Regra.RAO_ATIVO = 0");

            if (filtrosPesquisa.CodigoAprovador > 0)
            {
                where.Append(" and exists (");
                where.Append("         select 1 ");
                where.Append("           from T_REGRAS_OCORRENCIA_FUNCIONARIOS Aprovador ");
                where.Append("          where Aprovador.RAO_CODIGO = Regra.RAO_CODIGO ");
                where.Append($"           and Aprovador.FUN_CODIGO = {filtrosPesquisa.CodigoAprovador} ");
                where.Append("     ) ");
            }
        }

        #endregion

        #region Métodos Públicos

        public string ObterSqlPesquisaAlcada(Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaRelatorioRegrasAutorizacaoOcorrencia filtrosPesquisa)
        {
            StringBuilder groupBy = new StringBuilder();
            StringBuilder joins = new StringBuilder();
            StringBuilder sql = new StringBuilder();
            StringBuilder where = new StringBuilder();

            SetarWhere(filtrosPesquisa, where, joins, groupBy);

            sql.Append("select Alcada.CodigoRegra, ");
            sql.Append("       Alcada.Tipo, ");
            sql.Append("	      case ");
            sql.Append("           when Alcada.Condicao = 1 then 'Igual a (==)' ");
            sql.Append("           when Alcada.Condicao = 2 then 'Diferente de (!=)' ");
            sql.Append("           when Alcada.Condicao = 3 then 'Maior que (>)' ");
            sql.Append("           when Alcada.Condicao = 4 then 'Maior ou igual que (>=)' ");
            sql.Append("           when Alcada.Condicao = 5 then 'Menor que (<)' ");
            sql.Append("           when Alcada.Condicao = 6 then 'Menor ou igual que (<=)' ");
            sql.Append("           else '' ");
            sql.Append("       end as Condicao, ");
            sql.Append("       case ");
            sql.Append("           when Alcada.Juncao = 1 then 'E (Todas verdadeiras)' ");
            sql.Append("           when Alcada.Juncao = 2 then 'Ou (Apenas uma verdadeira)' ");
            sql.Append("           else '' ");
            sql.Append("       end as Juncao, ");
            sql.Append("       Alcada.Propriedade ");
            sql.Append("  from ( ");
            sql.Append("           select RegraPorTipoOcorrencia.RAO_CODIGO as CodigoRegra, ");
            sql.Append("                  'Regra por Tipo de Ocorrência' as Tipo, ");
            sql.Append("                  RegraPorTipoOcorrencia.RTO_CONDICAO as Condicao, ");
            sql.Append("                  RegraPorTipoOcorrencia.RTO_JUNCAO as Juncao, ");
            sql.Append("                  TipoOcorrencia.OCO_DESCRICAO as Propriedade ");
            sql.Append("             from T_REGRA_OCORRENCIA_TIPO_OCORRENCIA RegraPorTipoOcorrencia ");
            sql.Append("             join T_OCORRENCIA TipoOcorrencia ");
            sql.Append("               on RegraPorTipoOcorrencia.OCO_CODIGO = TipoOcorrencia.OCO_CODIGO ");
            sql.Append("             join T_REGRAS_AUTORIZACAO_OCORRENCIA Regra ");
            sql.Append("               on RegraPorTipoOcorrencia.RAO_CODIGO = Regra.RAO_CODIGO ");
            sql.Append("            where Regra.RAO_TIPO_OCORRENCIA = 1 ");
            sql.Append("            union ");
            sql.Append("           select RegraPorComponenteFrete.RAO_CODIGO as CodigoRegra, ");
            sql.Append("                  'Regra por Componente de Frete' as Tipo, ");
            sql.Append("                  RegraPorComponenteFrete.RCF_CONDICAO as Condicao, ");
            sql.Append("                  RegraPorComponenteFrete.RCF_JUNCAO as Juncao, ");
            sql.Append("                  ComponenteFrete.CFR_DESCRICAO as Propriedade ");
            sql.Append("             from T_REGRA_OCORRENCIA_COMPONENTE_FRETE RegraPorComponenteFrete ");
            sql.Append("             join T_COMPONENTE_FRETE ComponenteFrete ");
            sql.Append("               on RegraPorComponenteFrete.CFR_CODIGO = ComponenteFrete.CFR_CODIGO ");
            sql.Append("             join T_REGRAS_AUTORIZACAO_OCORRENCIA Regra ");
            sql.Append("               on RegraPorComponenteFrete.RAO_CODIGO = Regra.RAO_CODIGO ");
            sql.Append("            where Regra.RAO_COMPONENTE_FRETE = 1 ");
            sql.Append("            union ");
            sql.Append("           select RegraPorFilialEmissao.RAO_CODIGO as CodigoRegra, ");
            sql.Append("                  'Regra por Filial de Emissão' as Tipo, ");
            sql.Append("                  RegraPorFilialEmissao.RFE_CONDICAO as Condicao, ");
            sql.Append("                  RegraPorFilialEmissao.RFE_JUNCAO as Juncao, ");
            sql.Append("                  FilialEmissao.FIL_DESCRICAO as Propriedade ");
            sql.Append("             from T_REGRA_OCORRENCIA_FILIAL_EMISSAO RegraPorFilialEmissao ");
            sql.Append("             join T_FILIAL FilialEmissao ");
            sql.Append("               on RegraPorFilialEmissao.FIL_CODIGO = FilialEmissao.FIL_CODIGO ");
            sql.Append("             join T_REGRAS_AUTORIZACAO_OCORRENCIA Regra ");
            sql.Append("               on RegraPorFilialEmissao.RAO_CODIGO = Regra.RAO_CODIGO ");
            sql.Append("            where Regra.RAO_FILIAL_EMISSAO = 1 ");
            sql.Append("            union ");
            sql.Append("           select RegraPorTomadorOcorrencia.RAO_CODIGO as CodigoRegra, ");
            sql.Append("                  'Regra por Tomador da Ocorrência' as Tipo, ");
            sql.Append("                  RegraPorTomadorOcorrencia.RTO_CONDICAO as Condicao, ");
            sql.Append("                  RegraPorTomadorOcorrencia.RTO_JUNCAO as Juncao, ");
            sql.Append("                  TomadorOcorrencia.CLI_NOME as Propriedade ");
            sql.Append("             from T_REGRA_OCORRENCIA_TOMADOR_OCORRENCIA RegraPorTomadorOcorrencia ");
            sql.Append("             join T_CLIENTE TomadorOcorrencia ");
            sql.Append("               on RegraPorTomadorOcorrencia.CLI_CGCCPF = TomadorOcorrencia.CLI_CGCCPF ");
            sql.Append("             join T_REGRAS_AUTORIZACAO_OCORRENCIA Regra ");
            sql.Append("               on RegraPorTomadorOcorrencia.RAO_CODIGO = Regra.RAO_CODIGO ");
            sql.Append("            where Regra.RAO_TOMADOR_OCORRENCIA = 1 ");
            sql.Append("            union ");
            sql.Append("           select RegraPorValorOcorrencia.RAO_CODIGO as CodigoRegra, ");
            sql.Append("                  'Regra por Valor da Ocorrência' as Tipo, ");
            sql.Append("                  RegraPorValorOcorrencia.RVO_CONDICAO as Condicao, ");
            sql.Append("                  RegraPorValorOcorrencia.RVO_JUNCAO as Juncao, ");
            sql.Append("                  FORMAT(RegraPorValorOcorrencia.RVO_VALOR, 'N2', 'pt-BR') as Propriedade ");
            sql.Append("             from T_REGRA_OCORRENCIA_VALOR_OCORRENCIA RegraPorValorOcorrencia ");
            sql.Append("             join T_REGRAS_AUTORIZACAO_OCORRENCIA Regra ");
            sql.Append("               on RegraPorValorOcorrencia.RAO_CODIGO = Regra.RAO_CODIGO ");
            sql.Append("            where Regra.RAO_VALOR_OCORRENCIA = 1 ");
            sql.Append("            union ");
            sql.Append("           select RegraPorTipoOperacao.RAO_CODIGO as CodigoRegra, ");
            sql.Append("                  'Regra por Tipo da Operação' as Tipo, ");
            sql.Append("                  RegraPorTipoOperacao.RTP_CONDICAO as Condicao, ");
            sql.Append("                  RegraPorTipoOperacao.RTP_JUNCAO as Juncao, ");
            sql.Append("                  TipoOperacao.TOP_DESCRICAO as Propriedade ");
            sql.Append("             from T_REGRA_OCORRENCIA_TIPO_OPERACAO RegraPorTipoOperacao ");
            sql.Append("             join T_TIPO_OPERACAO TipoOperacao ");
            sql.Append("               on RegraPorTipoOperacao.TOP_CODIGO = TipoOperacao.TOP_CODIGO ");
            sql.Append("             join T_REGRAS_AUTORIZACAO_OCORRENCIA Regra ");
            sql.Append("               on RegraPorTipoOperacao.RAO_CODIGO = Regra.RAO_CODIGO ");
            sql.Append("            where Regra.RAO_TIPO_OPERACAO = 1 ");
            sql.Append("       ) as Alcada ");
            sql.Append("  join T_REGRAS_AUTORIZACAO_OCORRENCIA Regra ");
            sql.Append("    on Alcada.CodigoRegra = Regra.RAO_CODIGO ");

            if (where.Length > 0)
                sql.Append($" where {where.ToString().Trim().Substring(3)} ");

            return sql.ToString();
        }

        #endregion
    }
}
