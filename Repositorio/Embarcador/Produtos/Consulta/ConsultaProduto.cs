using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Produtos.Consulta
{
    sealed class ConsultaProduto : Embarcador.Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Produtos.FiltroPesquisaRelatorioProduto>
    {
        #region Construtores

        public ConsultaProduto() : base(tabela: "T_PRODUTO as Produto") { }

        #endregion

        #region Métodos Privados

        private void SetarJoinsGrupoProdutoTMS(StringBuilder joins)
        {
            if (!joins.Contains(" GrupoProdutoTMS "))
                joins.Append(" left join T_GRUPO_PRODUTO_TMS GrupoProdutoTMS on GrupoProdutoTMS.GPR_CODIGO = Produto.GPR_CODIGO ");
        }

        private void SetarJoinsLocalArmazenamentoProduto(StringBuilder joins)
        {
            if (!joins.Contains(" LocalArmazenamentoProduto "))
                joins.Append(" left join T_LOCAL_ARMAZENAMENTO_PRODUTO LocalArmazenamentoProduto on LocalArmazenamentoProduto.LAP_CODIGO = Produto.LAP_CODIGO ");
        }

        private void SetarJoinsMarcaProduto(StringBuilder joins)
        {
            if (!joins.Contains(" MarcaProduto "))
                joins.Append(" left join T_MARCA_PRODUTO MarcaProduto on MarcaProduto.MAP_CODIGO = Produto.MAP_CODIGO ");
        }

        private void SetarJoinsGrupoImposto(StringBuilder joins)
        {
            if (!joins.Contains(" GrupoImposto "))
                joins.Append(" left join T_GRUPO_IMPOSTO GrupoImposto on GrupoImposto.GRI_CODIGO = Produto.GRI_CODIGO ");
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Produtos.FiltroPesquisaRelatorioProduto filtroPesquisa)
        {
            switch (propriedade)
            {
                case "Codigo":
                    if (!select.Contains(" Codigo, "))
                    {
                        select.Append("Produto.PRO_CODIGO as Codigo, ");
                        groupBy.Append("Produto.PRO_CODIGO, ");
                    }
                    break;

                case "Descricao":
                    if (!select.Contains(" Descricao, "))
                    {
                        select.Append("Produto.PRO_DESCRICAO as Descricao, ");
                        groupBy.Append("Produto.PRO_DESCRICAO, ");
                    }
                    break;

                case "DescricaoNotaFiscal":
                    if (!select.Contains(" DescricaoNotaFiscal, "))
                    {
                        select.Append("Produto.PRO_DESCRICAO_NOTA_FISCAL as DescricaoNotaFiscal, ");
                        groupBy.Append("Produto.PRO_DESCRICAO_NOTA_FISCAL, ");
                    }
                    break;

                case "UnidadeMedidaFormatada":
                    if (!select.Contains(" UnidadeDeMedida, "))
                    {
                        select.Append("Produto.PRO_UNIDADE_MEDIDA as UnidadeDeMedida, ");
                        groupBy.Append("Produto.PRO_UNIDADE_MEDIDA, ");
                    }
                    break;

                case "CodigoProduto":
                    if (!select.Contains(" CodigoProduto, "))
                    {
                        select.Append("Produto.PRO_COD_PRODUTO as CodigoProduto, ");
                        groupBy.Append("Produto.PRO_COD_PRODUTO, ");
                    }
                    break;

                case "CodigoNCM":
                    if (!select.Contains(" CodigoNCM, "))
                    {
                        select.Append("Produto.PRO_COD_NCM as CodigoNCM, ");
                        groupBy.Append("Produto.PRO_COD_NCM, ");
                    }
                    break;

                case "CodigoCEST":
                    if (!select.Contains(" CodigoCEST, "))
                    {
                        select.Append("Produto.PRO_COD_CEST as CodigoCEST, ");
                        groupBy.Append("Produto.PRO_COD_CEST, ");
                    }
                    break;

                case "CodigoBarrasEAN":
                    if (!select.Contains(" CodigoBarrasEAN, "))
                    {
                        select.Append("Produto.PRO_CODIGO_BARRAS_EAN as CodigoBarrasEAN, ");
                        groupBy.Append("Produto.PRO_CODIGO_BARRAS_EAN, ");
                    }
                    break;

                case "CodigoBarras":
                    if (!select.Contains(" CodigoBarras, "))
                    {
                        select.Append("Produto.PRO_CODIGO_EAN as CodigoBarras, ");
                        groupBy.Append("Produto.PRO_CODIGO_EAN, ");
                    }
                    break;

                case "CodigoAnvisa":
                    if (!select.Contains(" CodigoAnvisa, "))
                    {
                        select.Append("Produto.PRO_CODIGO_ANVISA as CodigoAnvisa, ");
                        groupBy.Append("Produto.PRO_CODIGO_ANVISA, ");
                    }
                    break;

                case "CodigoANP":
                    if (!select.Contains(" CodigoANP, "))
                    {
                        select.Append("Produto.PRO_CODIGO_ANP as CodigoANP, ");
                        groupBy.Append("Produto.PRO_CODIGO_ANP, ");
                    }
                    break;

                case "Status":
                    if (!select.Contains(" Status, "))
                    {
                        select.Append(@"   CASE WHEN Produto.PRO_STATUS = 'A' THEN 'Ativo' 
                                                WHEN Produto.PRO_STATUS = 'I' THEN 'Inativo' 
                                            ELSE 'Não Definido' 
                                            END as Status, ");
                        groupBy.Append("Produto.PRO_STATUS, ");
                    }
                    break;

                case "CategoriaFormatada":
                    if (!select.Contains(" Categoria, "))
                    {
                        select.Append("Produto.PRO_CATEGORIA_PRODUTO as Categoria, ");
                        groupBy.Append("Produto.PRO_CATEGORIA_PRODUTO, ");
                    }
                    break;

                case "OrigemMercadoriaFormatada":
                    if (!select.Contains(" OrigemMercadoria, "))
                    {
                        select.Append("Produto.PRO_ORIGEM_MERCADORIA as OrigemMercadoria, ");
                        groupBy.Append("Produto.PRO_ORIGEM_MERCADORIA, ");
                    }
                    break;

                case "GeneroProdutoFormatado":
                    if (!select.Contains(" GeneroProduto, "))
                    {
                        select.Append("Produto.PRO_GENERO_PRODUTO as GeneroProduto, ");
                        groupBy.Append("Produto.PRO_GENERO_PRODUTO, ");
                    }
                    break;

                case "UltimoCusto":
                    if (!select.Contains(" UltimoCusto, "))
                    {
                        select.Append("Produto.PRO_ULTIMO_CUSTO as UltimoCusto, ");
                        groupBy.Append("Produto.PRO_ULTIMO_CUSTO, ");
                    }
                    break;

                case "CustoMedio":
                    if (!select.Contains(" CustoMedio, "))
                    {
                        select.Append("Produto.PRO_CUSTO_MEDIO as CustoMedio, ");
                        groupBy.Append("Produto.PRO_CUSTO_MEDIO, ");
                    }
                    break;

                case "MargemLucro":
                    if (!select.Contains(" MargemLucro, "))
                    {
                        select.Append("Produto.PRO_MARGEM_LUCRO as MargemLucro, ");
                        groupBy.Append("Produto.PRO_MARGEM_LUCRO, ");
                    }
                    break;

                case "ValorVenda":
                    if (!select.Contains(" ValorVenda, "))
                    {
                        select.Append("Produto.PRO_VALOR_VENDA as ValorVenda, ");
                        groupBy.Append("Produto.PRO_VALOR_VENDA, ");
                    }
                    break;

                case "ValorMinimoVenda":
                    if (!select.Contains(" ValorMinimoVenda, "))
                    {
                        select.Append("Produto.PRO_VALOR_MINIMO_VENDA as ValorMinimoVenda, ");
                        groupBy.Append("Produto.PRO_VALOR_MINIMO_VENDA, ");
                    }
                    break;

                case "PesoBruto":
                    if (!select.Contains(" PesoBruto, "))
                    {
                        select.Append("Produto.PRO_PESO_BRUTO as PesoBruto, ");
                        groupBy.Append("Produto.PRO_PESO_BRUTO, ");
                    }
                    break;

                case "PesoLiquido":
                    if (!select.Contains(" PesoLiquido, "))
                    {
                        select.Append("Produto.PRO_PESO_LIQUIDO as PesoLiquido, ");
                        groupBy.Append("Produto.PRO_PESO_LIQUIDO, ");
                    }
                    break;

                case "GrupoProduto":
                    if (!select.Contains(" GrupoProduto, "))
                    {
                        select.Append("GrupoProdutoTMS.GRP_DESCRICAO as GrupoProduto, ");
                        groupBy.Append("GrupoProdutoTMS.GRP_DESCRICAO, ");

                        SetarJoinsGrupoProdutoTMS(joins);
                    }
                    break;

                case "Marca":
                    if (!select.Contains(" Marca, "))
                    {
                        select.Append("MarcaProduto.MAP_DESCRICAO as Marca, ");
                        groupBy.Append("MarcaProduto.MAP_DESCRICAO, ");

                        SetarJoinsMarcaProduto(joins);
                    }
                    break;

                case "LocalArmazenamento":
                    if (!select.Contains(" LocalArmazenamento, "))
                    {
                        select.Append("LocalArmazenamentoProduto.LAP_DESCRICAO as LocalArmazenamento, ");
                        groupBy.Append("LocalArmazenamentoProduto.LAP_DESCRICAO, ");

                        SetarJoinsLocalArmazenamentoProduto(joins);
                    }
                    break;

                case "GrupoImposto":
                    if (!select.Contains(" GrupoImposto, "))
                    {
                        select.Append("GrupoImposto.GRI_DESCRICAO as GrupoImposto, ");
                        groupBy.Append("GrupoImposto.GRI_DESCRICAO, ");

                        SetarJoinsGrupoImposto(joins);
                    }
                    break;

            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.Produtos.FiltroPesquisaRelatorioProduto filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.CodigoNCM))
                where.Append($" and Produto.PRO_COD_NCM like '{filtrosPesquisa.CodigoNCM}%'");

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.CodigoCEST))
                where.Append($" and Produto.PRO_COD_CEST like '{filtrosPesquisa.CodigoCEST}%'");

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.CodigoBarrasEAN))
                where.Append($" and Produto.PRO_CODIGO_BARRAS_EAN like '%{filtrosPesquisa.CodigoBarrasEAN}%'");

            if (filtrosPesquisa.CodigoProduto > 0)
                where.Append($" and Produto.PRO_CODIGO = {filtrosPesquisa.CodigoProduto}");

            if (filtrosPesquisa.CodigoGrupo > 0)
                where.Append($" and Produto.GPR_CODIGO = {filtrosPesquisa.CodigoGrupo}");

            if (filtrosPesquisa.CodigoMarca > 0)
                where.Append($" and Produto.MAP_CODIGO = {filtrosPesquisa.CodigoMarca}");

            if (filtrosPesquisa.CodigoLocalArmazenamento > 0)
                where.Append($" and Produto.LAP_CODIGO = {filtrosPesquisa.CodigoLocalArmazenamento}");

            if (filtrosPesquisa.CodigoGrupoImposto > 0)
                where.Append($" and Produto.GRI_CODIGO = {filtrosPesquisa.CodigoGrupoImposto}");

            if (filtrosPesquisa.CodigoEmpresa > 0)
                where.Append($" and Produto.EMP_CODIGO = {filtrosPesquisa.CodigoEmpresa}");

            if (filtrosPesquisa.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                where.Append($" and Produto.PRO_STATUS = 'A'");
            else if (filtrosPesquisa.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                where.Append($" and Produto.PRO_STATUS = 'I'");

            if (filtrosPesquisa.CategoriaProduto != null)
                where.Append($" and Produto.PRO_CATEGORIA_PRODUTO = {filtrosPesquisa.CategoriaProduto.Value.ToString("d")}");
        }

        #endregion
    }
}
