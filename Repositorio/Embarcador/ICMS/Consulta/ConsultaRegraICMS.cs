using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.ICMS
{
    sealed class ConsultaRegraICMS : Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.ICMS.FiltroPesquisaRelatorioRegraICMS>
    {
        #region Construtores

        public ConsultaRegraICMS() : base(tabela: "T_REGRA_ICMS as RegraICMS ") { }

        #endregion

        #region Métodos Privados

        private void SetarJoinsCFOP(StringBuilder joins)
        {
            if (!joins.Contains(" CFOP "))
                joins.Append(" left join T_CFOP CFOP ON CFOP.CFO_CODIGO = RegraICMS.CFO_CODIGO ");
        }

        private void SetarJoinsEmpresa(StringBuilder joins)
        {
            if (!joins.Contains(" Empresa "))
                joins.Append(" left join T_EMPRESA Empresa on Empresa.EMP_codigo = RegraICMS.EMP_CODIGO  ");
        }

        private void SetarJoinsSetor(StringBuilder joins)
        {
            if (!joins.Contains(" Setor "))
                joins.Append(" LEFT JOIN T_SETOR Setor ON Setor.SET_CODIGO = RegraICMS.RIC_SETOR_EMPRESA ");
        }

        private void SetarJoinsClienteRemetente(StringBuilder joins)
        {

            if (!joins.Contains(" Remetente "))
                joins.Append(" left join T_CLIENTE Remetente on RegraICMS.CLI_REMETENTE = Remetente.CLI_CGCCPF ");
        }

        private void SetarJoinsClienteDestinatario(StringBuilder joins)
        {

            if (!joins.Contains(" Destinatario "))
                joins.Append(" left join T_CLIENTE Destinatario on RegraICMS.CLI_DESTINATARIO = Destinatario.CLI_CGCCPF ");
        }

        private void SetarJoinsClienteTomador(StringBuilder joins)
        {

            if (!joins.Contains(" Tomador "))
                joins.Append(" left join T_CLIENTE Tomador on RegraICMS.CLI_TOMADOR = Tomador.CLI_CGCCPF ");
        }

        private void SetarJoinsGrupoPessoasRemetente(StringBuilder joins)
        {
            if (!joins.Contains(" GrupoPessoasRemetente "))
                joins.Append(" left join T_GRUPO_PESSOAS GrupoPessoasRemetente on RegraICMS.GRP_REMETENTE = GrupoPessoasRemetente.GRP_CODIGO ");
        }

        private void SetarJoinsGrupoPessoasDestinatario(StringBuilder joins)
        {
            if (!joins.Contains(" GrupoPessoasDestinatario "))
                joins.Append(" left join T_GRUPO_PESSOAS GrupoPessoasDestinatario on RegraICMS.GRP_DESTINATARIO = GrupoPessoasDestinatario.GRP_CODIGO ");
        }

        private void SetarJoinsGrupoPessoasTomador(StringBuilder joins)
        {
            if (!joins.Contains(" GrupoPessoasTomador "))
                joins.Append(" left join T_GRUPO_PESSOAS GrupoPessoasTomador on RegraICMS.GRP_TOMADOR = GrupoPessoasTomador.GRP_CODIGO ");
        }

        private void SetarJoinsAtividadesRemetente(StringBuilder joins)
        {

            if (!joins.Contains(" AtividadesRemetente "))
                joins.Append(" left join T_ATIVIDADES AtividadesRemetente on AtividadesRemetente.ATI_CODIGO = RegraICMS.ATI_CODIGO_REMETENTE ");
        }

        private void SetarJoinsAtividadesDestinatario(StringBuilder joins)
        {

            if (!joins.Contains(" AtividadesDestinatario "))
                joins.Append(" left join T_ATIVIDADES AtividadesDestinatario on AtividadesDestinatario.ATI_CODIGO = RegraICMS.ATI_CODIGO_DESTINATARIO ");
        }

        private void SetarJoinsAtividadesTomador(StringBuilder joins)
        {

            if (!joins.Contains(" AtividadesTomador "))
                joins.Append(" left join T_ATIVIDADES AtividadesTomador on AtividadesTomador.ATI_CODIGO = RegraICMS.ATI_CODIGO_TOMADOR ");
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.ICMS.FiltroPesquisaRelatorioRegraICMS filtrosPesquisa)
        {
            switch (propriedade)
            {
                case "Modal":
                    if (!select.Contains(" Modal, "))
                    {
                        select.Append(@"case when RIC_TIPO_MODAL = 1 THEN 'Rodoviário' 
                                        when RIC_TIPO_MODAL = 2 THEN 'Aereo'
                                        when RIC_TIPO_MODAL = 3 THEN 'Aquaviario'
                                        when RIC_TIPO_MODAL = 4 THEN 'Ferroviario'
                                        when RIC_TIPO_MODAL = 5 THEN 'Dutoviario'
                                        when RIC_TIPO_MODAL = 6 THEN 'Multimodal'
                                        else 'Todos' end Modal, ");
                        groupBy.Append("RegraICMS.RIC_TIPO_MODAL, ");
                    }
                    break;

                case "EstadoEmitente":
                    if (!select.Contains(" EstadoEmitente, "))
                    {
                        select.Append("RegraICMS.UF_SIGLA_EMITENTE EstadoEmitente, ");
                        groupBy.Append("RegraICMS.UF_SIGLA_EMITENTE, ");
                    }
                    break;

                case "EstadoEmitenteDiferente":
                    if (!select.Contains(" EstadoEmitenteDiferente, "))
                    {
                        select.Append("RegraICMS.UF_SIGLA_EMITENTE_DEFERENTE_DE EstadoEmitenteDiferente, ");
                        groupBy.Append("RegraICMS.UF_SIGLA_EMITENTE_DEFERENTE_DE, ");
                    }
                    break;

                case "EstadoOrigem":
                    if (!select.Contains(" EstadoOrigem, "))
                    {
                        select.Append("CASE WHEN RegraICMS.RIC_UF_ORIGEM_DIFERENTE = 0 THEN RegraICMS.UF_SIGLA_ORIGEM ELSE ''  END EstadoOrigem, ");
                        groupBy.Append("RegraICMS.RIC_UF_ORIGEM_DIFERENTE, RegraICMS.UF_SIGLA_ORIGEM, ");
                    }
                    break;

                case "EstadoOrigemDiferente":
                    if (!select.Contains(" EstadoOrigemDiferente, "))
                    {
                        select.Append("CASE WHEN RegraICMS.RIC_UF_ORIGEM_DIFERENTE = 0 THEN '' ELSE RegraICMS.UF_SIGLA_ORIGEM  END EstadoOrigemDiferente, ");
                        groupBy.Append("RegraICMS.RIC_UF_ORIGEM_DIFERENTE, RegraICMS.UF_SIGLA_ORIGEM, ");
                    }
                    break;

                case "EstadoDestino":
                    if (!select.Contains(" EstadoDestino, "))
                    {
                        select.Append("CASE WHEN RegraICMS.RIC_UF_DESTINO_DIFERENTE = 0 THEN UF_SIGLA_DESTINO ELSE '' END EstadoDestino, ");
                        groupBy.Append("RegraICMS.RIC_UF_DESTINO_DIFERENTE, RegraICMS.UF_SIGLA_DESTINO, ");
                    }
                    break;

                case "EstadoDestinoDiferente":
                    if (!select.Contains(" EstadoDestinoDiferente, "))
                    {
                        select.Append(" CASE WHEN RegraICMS.RIC_UF_DESTINO_DIFERENTE = 0 THEN '' ELSE RegraICMS.UF_SIGLA_DESTINO END EstadoDestinoDiferente, ");
                        groupBy.Append("RegraICMS.RIC_UF_DESTINO_DIFERENTE, RegraICMS.UF_SIGLA_DESTINO, ");
                    }
                    break;

                case "EstadoTomador":
                    if (!select.Contains(" EstadoTomador, "))
                    {
                        select.Append(" CASE WHEN RegraICMS.RIC_UF_TOMADOR_DIFERENTE = 0 THEN RegraICMS.UF_SIGLA_TOMADOR ELSE '' END EstadoTomador, ");
                        groupBy.Append("RegraICMS.RIC_UF_TOMADOR_DIFERENTE, RegraICMS.UF_SIGLA_TOMADOR, ");
                    }
                    break;

                case "EstadoTomadorDiferente":
                    if (!select.Contains(" EstadoTomadorDiferente, "))
                    {
                        select.Append(" CASE WHEN RegraICMS.RIC_UF_TOMADOR_DIFERENTE = 0 THEN '' ELSE RegraICMS.UF_SIGLA_TOMADOR END EstadoTomadorDiferente, ");
                        groupBy.Append("RegraICMS.UF_SIGLA_TOMADOR, RegraICMS.UF_SIGLA_TOMADOR, ");
                    }
                    break;

                case "DataInicialFormatada":
                    if (!select.Contains(" DataInicial, "))
                    {
                        select.Append("RegraICMS.RIC_VIGENCIA_INICIO DataInicial, ");
                        groupBy.Append("RegraICMS.RIC_VIGENCIA_INICIO, ");
                    }
                    break;

                case "DataFinalFormatada":
                    if (!select.Contains(" DataFinal, "))
                    {
                        select.Append("RegraICMS.RIC_VIGENCIA_FIM DataFinal, ");
                        groupBy.Append("RegraICMS.RIC_VIGENCIA_FIM, ");
                    }
                    break;

                case "Empresa":
                    if (!select.Contains(" Empresa, "))
                    {
                        select.Append("Empresa.emp_cnpj + ' ' + Empresa.emp_razao Empresa, ");
                        groupBy.Append("Empresa.emp_cnpj, Empresa.emp_razao, ");

                        SetarJoinsEmpresa(joins);
                    }
                    break;

                case "Setor":
                    if (!select.Contains(" Setor, "))
                    {
                        select.Append("Setor.SET_DESCRICAO Setor, ");
                        groupBy.Append("Setor.SET_DESCRICAO, ");

                        SetarJoinsSetor(joins);
                    }
                    break;

                case "Remetente":
                    if (!select.Contains(" Remetente, "))
                    {
                        select.Append("Remetente.CLI_NOME Remetente, ");
                        groupBy.Append("Remetente.CLI_NOME, ");

                        SetarJoinsClienteRemetente(joins);
                    }
                    break;

                case "GrupoPessoasRemetente":
                    if (!select.Contains(" Remetente, "))
                    {
                        select.Append("GrupoPessoasRemetente.GRP_DESCRICAO GrupoPessoasRemetente, ");
                        groupBy.Append("GrupoPessoasRemetente.GRP_DESCRICAO, ");

                        SetarJoinsGrupoPessoasRemetente(joins);
                    }
                    break;

                case "AtividadeRemetente":
                    if (!select.Contains(" AtividadeRemetente, "))
                    {
                        select.Append("AtividadesRemetente.ATI_DESCRICAO AtividadeRemetente, ");
                        groupBy.Append("AtividadesRemetente.ATI_DESCRICAO, ");

                        SetarJoinsAtividadesRemetente(joins);
                    }
                    break;

                case "Destinatario":
                    if (!select.Contains(" Destinatario, "))
                    {
                        select.Append("Destinatario.CLI_NOME Destinatario, ");
                        groupBy.Append("Destinatario.CLI_NOME, ");

                        SetarJoinsClienteDestinatario(joins);
                    }
                    break;

                case "GrupoPessoasDestinatario":
                    if (!select.Contains(" Destinatario, "))
                    {
                        select.Append("GrupoPessoasDestinatario.GRP_DESCRICAO GrupoPessoasDestinatario, ");
                        groupBy.Append("GrupoPessoasDestinatario.GRP_DESCRICAO, ");

                        SetarJoinsGrupoPessoasDestinatario(joins);
                    }
                    break;

                case "AtividadesDestinatario":
                    if (!select.Contains(" AtividadesDestinatario, "))
                    {
                        select.Append("AtividadesDestinatario.ATI_DESCRICAO AtividadesDestinatario, ");
                        groupBy.Append("RegraICMS.ATI_DESCRICAO, ");

                        SetarJoinsAtividadesDestinatario(joins);
                    }
                    break;

                case "Tomador":
                    if (!select.Contains(" Tomador, "))
                    {
                        select.Append("Tomador.CLI_NOME Tomador, ");
                        groupBy.Append("Tomador.CLI_NOME, ");

                        SetarJoinsClienteTomador(joins);
                    }
                    break;

                case "GrupoPessoasTomador":
                    if (!select.Contains(" GrupoPessoasTomador, "))
                    {
                        select.Append("GrupoPessoasTomador.GRP_DESCRICAO GrupoPessoasTomador, ");
                        groupBy.Append("GrupoPessoasTomador.GRP_DESCRICAO, ");

                        SetarJoinsGrupoPessoasTomador(joins);
                    }
                    break;

                case "AtividadeTomadorDiferenteDe":
                    if (!select.Contains(" AtividadeTomadorDiferenteDe, "))
                    {
                        select.Append("RegraICMS.RIC_ATIVIDADE_TOMADOR_DIFERENTE AtividadeTomadorDiferenteDe, ");
                        groupBy.Append("RegraICMS.RIC_ATIVIDADE_TOMADOR_DIFERENTE, ");
                    }
                    break;

                case "AtividadesTomador":
                    if (!select.Contains(" AtividadesTomador, "))
                    {
                        select.Append("AtividadesTomador.ATI_DESCRICAO AtividadesTomador, ");
                        groupBy.Append("AtividadesTomador.ATI_DESCRICAO, ");

                        SetarJoinsAtividadesTomador(joins);
                    }
                    break;

                case "TipoServico":
                    if (!select.Contains(" TipoServico, "))
                    {
                        select.Append(@"case when RIC_TIPO_SERVICO = 0 then 'Normal'
                                         when RIC_TIPO_SERVICO = 1 then 'SubContratacao'
                                         when RIC_TIPO_SERVICO = 2 then 'Redespacho'
                                         when RIC_TIPO_SERVICO = 3 then 'RedIntermediario'
                                         when RIC_TIPO_SERVICO = 4 then 'ServVinculadoMultimodal'
                                         when RIC_TIPO_SERVICO = 6 then 'TransporteDePessoas'
                                         when RIC_TIPO_SERVICO = 7 then 'TransporteDeValores'
                                         when RIC_TIPO_SERVICO = 8 then 'ExcessoDeBagagem'
                                        else 'Todos' end TipoServico, ");
                        groupBy.Append("RegraICMS.RIC_TIPO_SERVICO, ");
                    }
                    break;

                case "TipoPagamento":
                    if (!select.Contains(" TipoPagamento, "))
                    {
                        select.Append(@"case when RIC_PAGOAPAGAR = 0 then 'Pago'
                                         when RIC_PAGOAPAGAR = 0 then 'A Pagar'
                                         when RIC_PAGOAPAGAR = 0 then 'Outros'
                                         else 'Todos' end TipoPagamento, ");
                        groupBy.Append("RegraICMS.RIC_PAGOAPAGAR, ");
                    }
                    break;

                case "NumeroProposta":
                    if (!select.Contains(" NumeroProposta, "))
                    {
                        select.Append("RegraICMS.RIC_NUMERO_PROPOSTA NumeroProposta, ");
                        groupBy.Append("RegraICMS.RIC_NUMERO_PROPOSTA, ");
                    }
                    break;

                case "UFdeOrigemIgualaUFTomadorFormatada":
                    if (!select.Contains(" UFdeOrigemIgualaUFTomador, "))
                    {
                        select.Append("RegraICMS.RIC_UF_ORIGEM_IGUAL_UF_TOMADOR UFdeOrigemIgualaUFTomador, ");
                        groupBy.Append("RegraICMS.RIC_UF_ORIGEM_IGUAL_UF_TOMADOR, ");
                    }
                    break;

                case "TipoOperacao":
                    if (!select.Contains(" TipoOperacao, "))
                    {
                        select.Append(@"SUBSTRING((SELECT DISTINCT ', ' + Operacao.TOP_DESCRICAO
                                        FROM T_TIPO_OPERACAO Operacao
                                        join T_REGRA_ICMS_TIPO_OPERACAO rt on rt.TOP_CODIGO = Operacao.TOP_CODIGO
                                        WHERE rt.RIC_CODIGO = RegraICMS.RIC_CODIGO  FOR XML PATH('')), 3, 1000) TipoOperacao, ");
                        if (!groupBy.Contains("RegraICMS.RIC_CODIGO"))
                            groupBy.Append("RegraICMS.RIC_CODIGO, ");
                    }
                    break;

                case "Produtos":
                    if (!select.Contains(" Produtos, "))
                    {
                        select.Append(@"SUBSTRING((SELECT DISTINCT ', ' + Operacao.GRP_DESCRICAO
                                        FROM T_PRODUTO_EMBARCADOR Operacao
                                        join T_REGRA_ICMS_PRODUTO_EMBARCADOR rt on rt.PRO_CODIGO = Operacao.PRO_CODIGO
                                        WHERE rt.RIC_CODIGO = RegraICMS.RIC_CODIGO  FOR XML PATH('')), 3, 1000) Produtos, ");
                        if (!groupBy.Contains("RegraICMS.RIC_CODIGO"))
                            groupBy.Append("RegraICMS.RIC_CODIGO, ");
                    }
                    break;

                case "CST":
                    if (!select.Contains(" CST, "))
                    {
                        select.Append("RegraICMS.RIC_CST CST, ");
                        groupBy.Append("RegraICMS.RIC_CST, ");
                    }
                    break;

                case "CFOP":
                    if (!select.Contains(" CFOP, "))
                    {
                        select.Append("CFOP.CFO_CFOP CFOP, ");
                        groupBy.Append("CFOP.CFO_CFOP, ");

                        SetarJoinsCFOP(joins);
                    }
                    break;

                case "Aliquota":
                    if (!select.Contains(" Aliquota, "))
                    {
                        select.Append("RegraICMS.RIC_ALIQUOTA Aliquota, ");
                        groupBy.Append("RegraICMS.RIC_ALIQUOTA, ");
                    }
                    break;

                case "Situacao":
                    if (!select.Contains(" Situacao, "))
                    {
                        select.Append("case when RegraICMS.RIC_ATIVO = 1 then 'Ativo' else 'Inativo' end Situacao, ");
                        groupBy.Append("RegraICMS.RIC_ATIVO, ");
                    }
                    break;

                case "PercentualReducaoBaseCalculo":
                    if (!select.Contains(" PercentualReducaoBaseCalculo, "))
                    {
                        select.Append("RegraICMS.RIC_PERCENTUAL_REDUCAO_BC PercentualReducaoBaseCalculo, ");
                        groupBy.Append("RegraICMS.RIC_PERCENTUAL_REDUCAO_BC, ");
                    }
                    break;

                case "PercentualCreditoPresumido":
                    if (!select.Contains(" PercentualCreditoPresumido, "))
                    {
                        select.Append("RegraICMS.RIC_PERCENTUAL_CREDITO_PRESUMIDO PercentualCreditoPresumido, ");
                        groupBy.Append("RegraICMS.RIC_PERCENTUAL_CREDITO_PRESUMIDO, ");
                    }
                    break;

                case "Descricao":
                    if (!select.Contains(" Descricao, "))
                    {
                        select.Append("RegraICMS.RIC_DESCRICAO Descricao, ");
                        groupBy.Append("RegraICMS.RIC_DESCRICAO, ");
                    }
                    break;

                case "ImprimirLeiFormatada":
                    if (!select.Contains(" ImprimirLei, "))
                    {
                        select.Append("RIC_IMPRIMME_LEI_CTE ImprimirLei, ");
                        groupBy.Append("RegraICMS.RIC_IMPRIMME_LEI_CTE, ");
                    }
                    break;

                case "ZerarBaseCalculoFormatada":
                    if (!select.Contains(" ZerarBaseCalculo, "))
                    {
                        select.Append("RIC_ZERAR_VALOR_ICMS ZerarBaseCalculo, ");
                        groupBy.Append("RegraICMS.RIC_ZERAR_VALOR_ICMS, ");
                    }
                    break;

                case "NaoReduzirRetencaoFormatada":
                    if (!select.Contains(" NaoReduzirRetencao, "))
                    {
                        select.Append("RIC_NAO_REDUZIR_RETENCAO_ICMS_DO_VALOR_DA_PRESTACAO NaoReduzirRetencao, ");
                        groupBy.Append("RegraICMS.RIC_NAO_REDUZIR_RETENCAO_ICMS_DO_VALOR_DA_PRESTACAO, ");
                    }
                    break;

                case "NaoImprimirImpostosFormatada":
                    if (!select.Contains(" NaoImprimirImpostos, "))
                    {
                        select.Append("RIC_NAO_IMPRIMIR_IMPOSTOS_DACTE NaoImprimirImpostos, ");
                        groupBy.Append("RegraICMS.RIC_NAO_IMPRIMIR_IMPOSTOS_DACTE, ");
                    }
                    break;

                case "RegimeTomadorDiferente":
                    if (!select.Contains(" RegimeTomadorDiferente, "))
                    {
                        select.Append("case when RegraICMS.RIC_REGIME_TRIBUTARIO_TOMADOR_DIFERENTE = 1 then (CASE WHEN RegraICMS.RIC_REGIME_TRIBUTARIO_TOMADOR = 1 THEN 'Simples Nacional' else '' end) else '' end RegimeTomadorDiferente, ");
                        groupBy.Append("RegraICMS.RIC_REGIME_TRIBUTARIO_TOMADOR, RegraICMS.RIC_REGIME_TRIBUTARIO_TOMADOR_DIFERENTE, ");
                    }
                    break;

                case "SomenteOptanteSimplesNacionalFormatada":
                    if (!select.Contains(" SomenteOptanteSimplesNacional, "))
                    {
                        select.Append("RegraICMS.RIC_SOMENTE_OPTANTE_SIMPLES_NACIONAL SomenteOptanteSimplesNacional, ");
                        groupBy.Append("RegraICMS.RIC_SOMENTE_OPTANTE_SIMPLES_NACIONAL, ");
                    }
                    break;

                case "DescontarICMSSTQuandoICMSNaoInclusoFormatada":
                    if (!select.Contains(" DescontarICMSSTQuandoICMSNaoIncluso, "))
                    {
                        select.Append("RegraICMS.RIC_DESCONTAR_ICMS_ST_QUANDO_ICMS_NAO_INCLUSO DescontarICMSSTQuandoICMSNaoIncluso, ");
                        groupBy.Append("RegraICMS.RIC_DESCONTAR_ICMS_ST_QUANDO_ICMS_NAO_INCLUSO, ");
                    }
                    break;

                case "IncluirPISeCOFINSnaBCFormatada":
                    if (!select.Contains(" IncluirPISeCOFINSnaBC, "))
                    {
                        select.Append("RegraICMS.RIC_INCLUIR_PIS_CONFIS_NA_BC IncluirPISeCOFINSnaBC, ");
                        groupBy.Append("RegraICMS.RIC_INCLUIR_PIS_CONFIS_NA_BC, ");
                    }
                    break;

                case "NaoIncluirPISeCOFINSnaBCparaComplementosFormatada":
                    if (!select.Contains(" NaoIncluirPISeCOFINSnaBCparaComplementos, "))
                    {
                        select.Append("RegraICMS.RIC_NAO_INCLUIR_PIS_CONFIS_NA_BC_PARA_COMPLEMENTOS NaoIncluirPISeCOFINSnaBCparaComplementos, ");
                        groupBy.Append("RegraICMS.RIC_NAO_INCLUIR_PIS_CONFIS_NA_BC_PARA_COMPLEMENTOS, ");
                    }
                    break;
            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.ICMS.FiltroPesquisaRelatorioRegraICMS filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            string pattern = "yyyy-MM-dd";

            where.Append(" and (RegraICMS.RIC_TIPO is null or RegraICMS.RIC_TIPO = 0)");

            if (filtrosPesquisa.DataInicial != DateTime.MinValue)
                where.Append(" and CAST(RegraICMS.RIC_VIGENCIA_INICIO AS DATE) >= '" + filtrosPesquisa.DataInicial.ToString(pattern) + "'");

            if (filtrosPesquisa.DataFinal != DateTime.MinValue)
                where.Append(" and CAST(RegraICMS.RIC_VIGENCIA_FIM AS DATE) <= '" + filtrosPesquisa.DataFinal.ToString(pattern) + "'");

            if (filtrosPesquisa.Ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                where.Append($" and RegraICMS.ric_ativo = 1");
            else if (filtrosPesquisa.Ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                where.Append($" and (RegraICMS.ric_ativo = 0 or RegraICMS.ric_ativo IS NULL)");

        }

        #endregion
    }
}