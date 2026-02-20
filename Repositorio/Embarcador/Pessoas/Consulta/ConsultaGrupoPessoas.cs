using Dominio.ObjetosDeValor.Embarcador.Pessoas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Pessoas
{
    sealed class ConsultaGrupoPessoas : Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Pessoas.FiltroPesquisaRelatorioGrupoPessoas>
    {
        #region Construtores
        
        public ConsultaGrupoPessoas() : base(tabela: "T_GRUPO_PESSOAS as GrupoPessoas") { }

        #endregion

        #region MétodosPrivados

        private void SetarJoinsPedidoTipoPagamento(StringBuilder joins)
        {
            if (!joins.Contains(" PedidoTipoPagamento "))
                joins.Append(" LEFT JOIN T_PEDIDO_TIPO_PAGAMENTO PedidoTipoPagamento ON PedidoTipoPagamento.PTP_CODIGO = GrupoPessoas.PTP_CODIGO ");
        }
        private void SetarJoinsVendedor(StringBuilder joins)
        {
            if (!joins.Contains(" Vendedor "))
                joins.Append(" LEFT JOIN t_grupo_pessoas_funcionario AS grupopessoasfunc ON grupopessoas.GRP_CODIGO = grupopessoasfunc.GRP_CODIGO LEFT JOIN t_funcionario AS funcionario ON grupopessoasfunc.FUN_CODIGO = funcionario.FUN_CODIGO ");
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, FiltroPesquisaRelatorioGrupoPessoas filtrosPesquisa)
        {
            switch (propriedade)
            {
                case "Descricao":
                    if (!select.Contains(" Descricao, "))
                    {
                        select.Append("GrupoPessoas.GRP_DESCRICAO Descricao, ");

                        if (!groupBy.Contains("GrupoPessoas.GRP_DESCRICAO"))
                            groupBy.Append("GrupoPessoas.GRP_DESCRICAO, ");
                    }
                    break;

                case "AtivoDescricao":
                    if (!select.Contains(" Ativo, "))
                    {
                        select.Append("GrupoPessoas.GRP_ATIVO Ativo, ");

                        if (!groupBy.Contains("GrupoPessoas.GRP_ATIVO"))
                            groupBy.Append("GrupoPessoas.GRP_ATIVO, ");
                    }
                    break;

                case "TipoGrupoDescricao":
                    if (!select.Contains(" TipoGrupo, "))
                    {
                        select.Append("GrupoPessoas.GRP_TIPO_GRUPO_PESSOAS TipoGrupo, ");

                        if (!groupBy.Contains("GrupoPessoas.GRP_TIPO_GRUPO_PESSOAS"))
                            groupBy.Append("GrupoPessoas.GRP_TIPO_GRUPO_PESSOAS, ");
                    }
                    break;

                case "Email":
                    if (!select.Contains(" Email, "))
                    {
                        select.Append("GrupoPessoas.GRP_EMAIL Email, ");

                        if (!groupBy.Contains("GrupoPessoas.GRP_EMAIL"))
                            groupBy.Append("GrupoPessoas.GRP_EMAIL, ");
                    }
                    break;

                case "CondicaoPedido":
                    if (select.Contains(" CondicaoPedido, "))
                    {
                        select.Append("PedidoTipoPagamento.PTP_DESCRICAO CondicaoPedido, ");

                        if (!groupBy.Contains("PedidoTipoPagamento.PTP_DESCRICAO"))
                            groupBy.Append("PedidoTipoPagamento.PTP_DESCRICAO, ");

                        SetarJoinsPedidoTipoPagamento(joins);
                    }
                    break;

                case "SituacaoFinanceiraDescricao":
                    if (!select.Contains(" SituacaoFinanceira, "))
                    {
                        select.Append("GrupoPessoas.GRP_SITUACAO_FINANCEIRA SituacaoFinanceira, ");

                        if (!groupBy.Contains("GrupoPessoas.GRP_SITUACAO_FINANCEIRA"))
                            groupBy.Append("GrupoPessoas.GRP_SITUACAO_FINANCEIRA, ");
                    }
                    break;

                case "ValorLimiteFaturamento":
                    if (!select.Contains(" ValorLimiteFaturamento, "))
                    {
                        select.Append("GrupoPessoas.GRP_VALOR_LIMITE_FATURAMENTO ValorLimiteFaturamento, ");

                        if (!groupBy.Contains("GrupoPessoas.GRP_VALOR_LIMITE_FATURAMENTO"))
                            groupBy.Append("GrupoPessoas.GRP_VALOR_LIMITE_FATURAMENTO, ");
                    }
                    break;

                case "DiasEmAbertoAposVencimento":
                    if (!select.Contains(" DiasEmAbertoAposVencimento, "))
                    {
                        select.Append("GrupoPessoas.GRP_DIA_EM_ABERTO_APOS_VENCIMENTO DiasEmAbertoAposVencimento, ");

                        if (!groupBy.Contains("GrupoPessoas.GRP_DIA_EM_ABERTO_APOS_VENCIMENTO"))
                            groupBy.Append("GrupoPessoas.GRP_DIA_EM_ABERTO_APOS_VENCIMENTO, ");
                    }
                    break;

                case "DiasDePrazoFatura":
                    if (!select.Contains(" DiasDePrazoFatura, "))
                    {
                        select.Append("GrupoPessoas.GRP_DIA_DE_PRAZO_FATURA DiasDePrazoFatura, ");

                        if (!groupBy.Contains("GrupoPessoas.GRP_DIA_DE_PRAZO_FATURA"))
                            groupBy.Append("GrupoPessoas.GRP_DIA_DE_PRAZO_FATURA, ");
                    }
                    break;

                case "BloqueadoDescricao":
                    if (!select.Contains(" Bloqueado, "))
                    {
                        select.Append("GrupoPessoas.GRP_BLOQUEADO Bloqueado, ");

                        if (!groupBy.Contains("GrupoPessoas.GRP_BLOQUEADO"))
                            groupBy.Append("GrupoPessoas.GRP_BLOQUEADO, ");
                    }
                    break;


                case "ExigeCanhotoFisico":
                case "ExigeCanhotoFisicoDescricao":
                    if (!select.Contains(" ExigeCanhotoFisico, "))
                    {
                        select.Append("GrupoPessoas.GRP_EXIGE_CANHOTO_FISICO_FATURA ExigeCanhotoFisico, ");

                        if (!groupBy.Contains("GrupoPessoas.GRP_EXIGE_CANHOTO_FISICO_FATURA"))
                            groupBy.Append("GrupoPessoas.GRP_EXIGE_CANHOTO_FISICO_FATURA, ");
                    }
                    break;

                case "Vendedor":
                    if (!select.Contains(" Vendedor, "))
                    {
                        select.Append("STRING_AGG(funcionario.FUN_NOME, ', ') as Vendedor, ");

                        if (!groupBy.Contains("funcionario.FUN_NOME"))
                            SetarJoinsVendedor(joins);
                    }
                    break;
            }
        }

        protected override void SetarWhere(FiltroPesquisaRelatorioGrupoPessoas filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            if (filtrosPesquisa.TipoGrupo != null && filtrosPesquisa.TipoGrupo.Count > 0)
                where.Append($" AND GrupoPessoas.GRP_TIPO_GRUPO_PESSOAS IN ({string.Join(", ", filtrosPesquisa.TipoGrupo.Select(o => (int)o))}) ");

            if (filtrosPesquisa.Ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                where.Append(" AND GrupoPessoas.GRP_ATIVO = 1 ");
            else if (filtrosPesquisa.Ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                where.Append(" AND GrupoPessoas.GRP_ATIVO = 0 ");

            if (filtrosPesquisa.Bloqueado != 9)
                where.Append($" AND GrupoPessoas.GRP_BLOQUEADO = {filtrosPesquisa.Bloqueado} ");
        }

        #endregion
    }
}
