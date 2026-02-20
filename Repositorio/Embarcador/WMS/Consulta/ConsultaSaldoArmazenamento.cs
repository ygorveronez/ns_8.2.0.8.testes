using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.WMS
{
    sealed class ConsultaSaldoArmazenamento : Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.WMS.FiltroPesquisaRelatorioSaldoArmazenamento>
    {
        #region Construtores

        public ConsultaSaldoArmazenamento() : base(tabela: "T_PRODUTO_EMBARCADOR_LOTE as ProdutoEmbarcadorLote") { }

        #endregion

        #region Métodos Privados

        private void SetarJoinsProdutoEmbarcador(StringBuilder joins)
        {
            if (!joins.Contains(" ProdutoEmbarcador "))
                joins.Append(" left join T_PRODUTO_EMBARCADOR ProdutoEmbarcador on ProdutoEmbarcador.PRO_CODIGO = ProdutoEmbarcadorLote.PRO_CODIGO ");
        }

        private void SetarJoinsUnidadeMedida(StringBuilder joins)
        {
            SetarJoinsProdutoEmbarcador(joins);

            if (!joins.Contains(" UnidadeMedida "))
                joins.Append(" left join T_UNIDADE_MEDIDA UnidadeMedida on UnidadeMedida.UNI_CODIGO = ProdutoEmbarcador.UNI_CODIGO ");
        }

        private void SetarJoinsDepositoPosicao(StringBuilder joins)
        {
            if (!joins.Contains(" DepositoPosicao "))
                joins.Append(" left join T_DEPOSITO_POSICAO DepositoPosicao on DepositoPosicao.DPO_CODIGO = ProdutoEmbarcadorLote.DPO_CODIGO ");
        }

        private void SetarJoinsDepositoBloco(StringBuilder joins)
        {
            SetarJoinsDepositoPosicao(joins);

            if (!joins.Contains(" DepositoBloco "))
                joins.Append(" left join T_DEPOSITO_BLOCO DepositoBloco on DepositoBloco.DEB_CODIGO = DepositoPosicao.DEB_CODIGO ");
        }

        private void SetarJoinsDepositoRua(StringBuilder joins)
        {
            SetarJoinsDepositoBloco(joins);

            if (!joins.Contains(" DepositoRua "))
                joins.Append(" left join T_DEPOSITO_RUA DepositoRua on DepositoRua.DER_CODIGO = DepositoBloco.DER_CODIGO ");
        }

        private void SetarJoinsDeposito(StringBuilder joins)
        {
            SetarJoinsDepositoRua(joins);

            if (!joins.Contains(" Deposito "))
                joins.Append(" left join T_DEPOSITO Deposito on Deposito.DEP_CODIGO = DepositoRua.DEP_CODIGO ");
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.WMS.FiltroPesquisaRelatorioSaldoArmazenamento filtroPesquisa)
        {
            switch (propriedade)
            {
                case "ProdutoEmbarcador":
                    if (!select.Contains(" ProdutoEmbarcador, "))
                    {
                        select.Append("ProdutoEmbarcador.GRP_DESCRICAO as ProdutoEmbarcador, ");
                        groupBy.Append("ProdutoEmbarcador.GRP_DESCRICAO, ");

                        SetarJoinsProdutoEmbarcador(joins);
                    }
                    break;

                case "Deposito":
                    if (!select.Contains(" Deposito, "))
                    {
                        select.Append("Deposito.DEP_DESCRICAO as Deposito, ");
                        groupBy.Append("Deposito.DEP_DESCRICAO, ");

                        SetarJoinsDeposito(joins);
                    }
                    break;

                case "Bloco":
                    if (!select.Contains(" Bloco, "))
                    {
                        select.Append("DepositoBloco.DEB_DESCRICAO as Bloco, ");
                        groupBy.Append("DepositoBloco.DEB_DESCRICAO, ");

                        SetarJoinsDepositoBloco(joins);
                    }
                    break;

                case "Posicao":
                    if (!select.Contains(" Posicao, "))
                    {
                        select.Append("DepositoPosicao.DPO_DESCRICAO as Posicao, ");
                        groupBy.Append("DepositoPosicao.DPO_DESCRICAO, ");

                        SetarJoinsDepositoPosicao(joins);
                    }
                    break;

                case "Rua":
                    if (!select.Contains(" Rua, "))
                    {
                        select.Append("DepositoRua.DER_DESCRICAO as Rua, ");
                        groupBy.Append("DepositoRua.DER_DESCRICAO, ");

                        SetarJoinsDepositoRua(joins);
                    }
                    break;

                case "Abreviacao":
                    if (!select.Contains(" Abreviacao, "))
                    {
                        select.Append("DepositoPosicao.DPO_ABREVIACAO as Abreviacao, ");
                        groupBy.Append("DepositoPosicao.DPO_ABREVIACAO, ");

                        SetarJoinsDepositoPosicao(joins);
                    }
                    break;

                case "CodigoBarras":
                    if (!select.Contains(" CodigoBarras, "))
                    {
                        select.Append("ProdutoEmbarcadorLote.PEL_CODIGO_BARRAS as CodigoBarras, ");
                        groupBy.Append("ProdutoEmbarcadorLote.PEL_CODIGO_BARRAS, ");
                    }
                    break;

                case "Descricao":
                    if (!select.Contains(" Descricao, "))
                    {
                        select.Append("ProdutoEmbarcadorLote.PEL_DESCRICAO as Descricao, ");
                        groupBy.Append("ProdutoEmbarcadorLote.PEL_DESCRICAO, ");
                    }
                    break;

                case "DataVencimentoFormatada":
                    if (!select.Contains(" DataVencimento, "))
                    {
                        select.Append("ProdutoEmbarcadorLote.PEL_DATA_VENCIMENTO as DataVencimento, ");
                        groupBy.Append("ProdutoEmbarcadorLote.PEL_DATA_VENCIMENTO, ");
                    }
                    break;

                case "TipoRecebimento":
                    if (!select.Contains(" TipoRecebimento, "))
                    {
                        select.Append(@"   CASE WHEN ProdutoEmbarcadorLote.PEL_TIPO_RECEBIMENTO = 2 THEN 'Volume' 
                                            ELSE 'Mercadoria' 
                                            END as TipoRecebimento, ");
                        groupBy.Append("ProdutoEmbarcadorLote.PEL_TIPO_RECEBIMENTO, ");
                    }
                    break;

                case "QuantidadeLote":
                    if (!select.Contains(" QuantidadeLote, "))
                    {
                        select.Append("sum(ProdutoEmbarcadorLote.PEL_QUANTIDADE_LOTE) as QuantidadeLote, ");
                    }
                    break;

                case "QuantidadeAtual":
                    if (!select.Contains(" QuantidadeAtual, "))
                    {
                        select.Append("sum(ProdutoEmbarcadorLote.PEL_QUANTIDADE_ATUAL) as QuantidadeAtual, ");
                    }
                    break;

                case "UnidadeMedida":
                    if (!select.Contains(" UnidadeMedida, "))
                    {
                        select.Append("UnidadeMedida.UNI_SIGLA as UnidadeMedida, ");
                        groupBy.Append("UnidadeMedida.UNI_SIGLA, ");

                        SetarJoinsUnidadeMedida(joins);
                    }
                    break;
            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.WMS.FiltroPesquisaRelatorioSaldoArmazenamento filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            string pattern = "yyyy-MM-dd";

            if (filtrosPesquisa.TipoRecebimento.HasValue)
            {
                if (filtrosPesquisa.TipoRecebimento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRecebimentoMercadoria.Mercadoria)
                    where.Append(" AND (ProdutoEmbarcadorLote.PEL_TIPO_RECEBIMENTO = 1 OR ProdutoEmbarcadorLote.PEL_TIPO_RECEBIMENTO IS NULL) ");
                else
                    where.Append(" AND (ProdutoEmbarcadorLote.PEL_TIPO_RECEBIMENTO = 2) ");
            }

            if (filtrosPesquisa.SaldoDisponivel)
                where.Append(" AND ISNULL(ProdutoEmbarcadorLote.PEL_QUANTIDADE_ATUAL, 0) > 0");

            if (filtrosPesquisa.CodigoProdutoEmbarcador > 0)
                where.Append(" AND ProdutoEmbarcador.PRO_CODIGO = " + filtrosPesquisa.CodigoProdutoEmbarcador.ToString());

            if (filtrosPesquisa.CodigoDeposito > 0)
            {
                where.Append(" AND Deposito.DEP_CODIGO = " + filtrosPesquisa.CodigoDeposito.ToString());
                SetarJoinsDeposito(joins);
            }

            if (filtrosPesquisa.CodigoBloco > 0)
            {
                where.Append(" AND DepositoBloco.DEB_CODIGO = " + filtrosPesquisa.CodigoBloco.ToString());
                SetarJoinsDepositoBloco(joins);
            }

            if (filtrosPesquisa.CodigoPosicao > 0)
            {
                where.Append(" AND DepositoPosicao.DPO_CODIGO = " + filtrosPesquisa.CodigoPosicao.ToString());
                SetarJoinsDepositoPosicao(joins);
            }

            if (filtrosPesquisa.CodigoRua > 0)
            {
                where.Append(" AND DepositoRua.DER_CODIGO = " + filtrosPesquisa.CodigoRua.ToString());
                SetarJoinsDepositoRua(joins);
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.CodigoBarras))
                where.Append(" AND ProdutoEmbarcadorLote.PEL_CODIGO_BARRAS = '" + filtrosPesquisa.CodigoBarras + "'");

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Descricao))
                where.Append(" AND ProdutoEmbarcadorLote.PEL_DESCRICAO = '" + filtrosPesquisa.Descricao + "'");

            if (filtrosPesquisa.DataVencimentoInicial != DateTime.MinValue)
                where.Append(" and CAST(ProdutoEmbarcadorLote.PEL_DATA_VENCIMENTO AS DATE) >= '" + filtrosPesquisa.DataVencimentoInicial.ToString(pattern) + "' ");

            if (filtrosPesquisa.DataVencimentoFinal != DateTime.MinValue)
                where.Append(" and CAST(ProdutoEmbarcadorLote.PEL_DATA_VENCIMENTO AS DATE) <= '" + filtrosPesquisa.DataVencimentoFinal.ToString(pattern) + "' ");
        }

        #endregion
    }
}