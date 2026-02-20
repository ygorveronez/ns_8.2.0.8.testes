using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Financeiro
{
    sealed class ConsultaCondicoesPagamentoTransportador : Embarcador.Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaCondicoesPagamentoTransportador>
    {
        #region Construtores

        public ConsultaCondicoesPagamentoTransportador() : base(tabela: "T_CONDICAO_PAGAMENTO_TRANSPORTADOR as CondicaoPagamentoTransportador") { }

        #endregion

        #region Métodos Privados

        private void SetarJoinsEmpresa(StringBuilder joins)
        {
            if (!joins.Contains(" Empresa "))
                joins.Append("left join T_EMPRESA Empresa on Empresa.EMP_CODIGO = CondicaoPagamentoTransportador.EMP_CODIGO ");
        }

        private void SetarJoinsLocalidade(StringBuilder joins)
        {
            if (!joins.Contains(" Localidades "))
                joins.Append("left join T_LOCALIDADES Localidades on Localidades.LOC_CODIGO = Empresa.LOC_CODIGO ");
        }
        private void SetarJoinsTipoCarga(StringBuilder joins)
        {
            if (!joins.Contains(" TipoCarga "))
                joins.Append("left join T_TIPO_DE_CARGA TipoCarga on TipoCarga.TCG_CODIGO = CondicaoPagamentoTransportador.TCG_CODIGO ");
        }

        private void SetarJoinsTipoOperacao(StringBuilder joins)
        {
            if (!joins.Contains(" TipoOperacao "))
                joins.Append("left join T_TIPO_OPERACAO TipoOperacao on TipoOperacao.TOP_CODIGO = CondicaoPagamentoTransportador.TOP_CODIGO ");
        }


        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaCondicoesPagamentoTransportador filtroPesquisa)
        {
            switch (propriedade)
            {
                case "CodigoCPT":
                    if (!select.Contains(" CodigoCPT, "))
                    {
                        select.Append("CondicaoPagamentoTransportador.CPT_CODIGO as CodigoCPT, ");
                        groupBy.Append("CondicaoPagamentoTransportador.CPT_CODIGO, ");
                    }
                    break;

                case "RazaoSocial":
                    if (!select.Contains(" RazaoSocial, "))
                    {
                        select.Append("Empresa.EMP_RAZAO as RazaoSocial, ");
                        groupBy.Append("Empresa.EMP_RAZAO, ");

                        SetarJoinsEmpresa(joins);
                    }
                    break;

                case "CodigoIntegracao":
                    if (!select.Contains(" CodigoIntegracao, "))
                    {
                        select.Append("Empresa.EMP_CODIGO_INTEGRACAO as CodigoIntegracao, ");
                        groupBy.Append("Empresa.EMP_CODIGO_INTEGRACAO, ");

                        SetarJoinsEmpresa(joins);
                    }
                    break;

                case "CNPJ":
                case "CNPJFormatado":
                    if (!select.Contains(" CNPJ, "))
                    {
                        select.Append("Empresa.EMP_CNPJ as CNPJ, ");
                        groupBy.Append("Empresa.EMP_CNPJ, ");

                        SetarJoinsEmpresa(joins);
                    }
                    break;

                case "SiglaUF":
                    if (!select.Contains(" SiglaUF, "))
                    {
                        select.Append("Localidades.UF_SIGLA as SiglaUF, ");
                        groupBy.Append("Localidades.UF_SIGLA, ");

                        SetarJoinsEmpresa(joins);
                        SetarJoinsLocalidade(joins);
                    }
                    break;

                case "DiaEmissaoLimite":
                case "DiaEmissaoLimiteFormatado":
                    if (!select.Contains(" DiaEmissaoLimite, "))
                    {
                        select.Append("CondicaoPagamentoTransportador.CPT_DIA_EMISSAO_LIMITE as DiaEmissaoLimite, ");
                        groupBy.Append("CondicaoPagamentoTransportador.CPT_DIA_EMISSAO_LIMITE, ");
                    }
                    break;

                case "DiaMes":
                case "DiaMesFormatado":
                    if (!select.Contains(" DiaMes, "))
                    {
                        select.Append("CondicaoPagamentoTransportador.CPT_DIA_MES as DiaMes, ");
                        groupBy.Append("CondicaoPagamentoTransportador.CPT_DIA_MES, ");
                    }
                    break;

                case "DiasDePrazoPagamento":
                    if (!select.Contains(" DiasDePrazoPagamento, "))
                    {
                        select.Append("CondicaoPagamentoTransportador.CPT_DIAS_DE_PRAZO_PAGAMENTO as DiasDePrazoPagamento, ");
                        groupBy.Append("CondicaoPagamentoTransportador.CPT_DIAS_DE_PRAZO_PAGAMENTO, ");
                    }
                    break;

                case "DiaSemana":
                case "DiaSemanaDescricao":
                    if (!select.Contains(" DiaSemana, "))
                    {
                        select.Append("CondicaoPagamentoTransportador.CPT_DIA_SEMANA as DiaSemana, ");
                        groupBy.Append("CondicaoPagamentoTransportador.CPT_DIA_SEMANA, ");
                    }
                    break;

                case "TipoPrazoPagamento":
                case "TipoPrazoPagamentoDescricao":
                    if (!select.Contains(" TipoPrazoPagamento, "))
                    {
                        select.Append("CondicaoPagamentoTransportador.CPT_TIPO_PRAZO_PAGAMENTO as TipoPrazoPagamento, ");
                        groupBy.Append("CondicaoPagamentoTransportador.CPT_TIPO_PRAZO_PAGAMENTO, ");
                    }
                    break;

                case "VencimentoForaMes":
                case "VencimentoForaMesFormatado":
                    if (!select.Contains(" VencimentoForaMes, "))
                    {
                        select.Append("CondicaoPagamentoTransportador.CPT_VENCIMENTO_FORA_MES as VencimentoForaMes, ");
                        groupBy.Append("CondicaoPagamentoTransportador.CPT_VENCIMENTO_FORA_MES, ");
                    }
                    break;

                case "TipoCargaDescricao":
                    if (!select.Contains(" TipoCargaDescricao, "))
                    {
                        select.Append("TipoCarga.TCG_DESCRICAO as TipoCargaDescricao, ");
                        groupBy.Append("TipoCarga.TCG_DESCRICAO, ");

                        SetarJoinsTipoCarga(joins);
                    }
                    break;

                case "TipoOperacaoDescricao":
                    if (!select.Contains(" TipoOperacaoDescricao, "))
                    {
                        select.Append("TipoOperacao.TOP_DESCRICAO as TipoOperacaoDescricao, ");
                        groupBy.Append("TipoOperacao.TOP_DESCRICAO, ");

                        SetarJoinsTipoOperacao(joins);
                    }
                    break;
            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaCondicoesPagamentoTransportador filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.CodigoIntegracao))
                where.Append($" and Empresa.EMP_CODIGO_INTEGRACAO = '{filtrosPesquisa.CodigoIntegracao}'");

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Estado) && filtrosPesquisa.Estado != "0")
                where.Append($" and Localidades.UF_SIGLA = '{filtrosPesquisa.Estado}'");

            if (filtrosPesquisa.CodigoEmpresa > 0)
                where.Append($" and CondicaoPagamentoTransportador.EMP_CODIGO = {filtrosPesquisa.CodigoEmpresa}");

            if (filtrosPesquisa.CodigoTipoCarga > 0)
                where.Append($" and CondicaoPagamentoTransportador.TCG_CODIGO = {filtrosPesquisa.CodigoTipoCarga}");

            if (filtrosPesquisa.CodigoTipoOperacao > 0)
                where.Append($" and CondicaoPagamentoTransportador.TOP_CODIGO = {filtrosPesquisa.CodigoTipoOperacao}");

        }

        #endregion
    }
}
