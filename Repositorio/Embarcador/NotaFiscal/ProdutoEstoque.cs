using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.NotaFiscal
{
    public class ProdutoEstoque : RepositorioBase<Dominio.Entidades.Embarcador.NotaFiscal.ProdutoEstoque>
    {
        public ProdutoEstoque(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.NotaFiscal.ProdutoEstoque BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.ProdutoEstoque>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.NotaFiscal.ProdutoEstoque BuscarPorProduto(int codigo, int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.ProdutoEstoque>();
            var result = from obj in query where obj.Produto.Codigo == codigo select obj;
            if (codigoEmpresa > 0)
                result = result.Where(o => o.Empresa.Codigo == codigoEmpresa || o.Empresa == null);

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.NotaFiscal.ProdutoEstoque> BuscarTodosPorProduto(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.ProdutoEstoque>();
            var result = from obj in query where obj.Produto.Codigo == codigo select obj;

            return result.ToList();
        }

        public void DeletarEstoque(int codigoDeletar, int codigoPrincipal)
        {
            UnitOfWork.Sessao.CreateSQLQuery("UPDATE T_MERCADORIA SET PRE_CODIGO = :codigoPrincipal where PRE_CODIGO = :codigoDeletar").SetInt32("codigoPrincipal", codigoPrincipal).SetInt32("codigoDeletar", codigoDeletar).SetTimeout(6000).ExecuteUpdate();

            UnitOfWork.Sessao.CreateSQLQuery("delete from T_PRODUTO_ESTOQUE_HISTORICO where PRE_CODIGO = :codigoDeletar").SetInt32("codigoDeletar", codigoDeletar).SetTimeout(6000).ExecuteUpdate();
            UnitOfWork.Sessao.CreateSQLQuery("delete from T_PRODUTO_ESTOQUE where PRE_CODIGO = :codigoDeletar").SetInt32("codigoDeletar", codigoDeletar).SetTimeout(6000).ExecuteUpdate();
        }

        public Dominio.Entidades.Embarcador.NotaFiscal.ProdutoEstoque BuscarPorProduto(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.ProdutoEstoque>();
            var result = from obj in query where obj.Produto.Codigo == codigo select obj;
            result = result.Where(o => o.Empresa == null);

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.NotaFiscal.ProdutoEstoque BuscarPorProdutoECNPJFilial(int produto, string filial)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.ProdutoEstoque>();
            var result = from obj in query where obj.Produto.Codigo == produto select obj;

            if (!string.IsNullOrWhiteSpace(filial))
                result = result.Where(o => o.Empresa.CNPJ == filial);

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.NotaFiscal.ProdutoEstoque BuscarPorProdutoEFilial(int produto, int filial)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.ProdutoEstoque>();
            var result = from obj in query where obj.Produto.Codigo == produto select obj;

            if (filial > 0)
                result = result.Where(o => o.Empresa.Codigo == filial);

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.NotaFiscal.ProdutoEstoque> Consultar(int produto, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var result = _Consultar(produto);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsulta(int produto)
        {
            var result = _Consultar(produto);

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.NotaFiscal.ProdutoEstoque> BuscarEstoquesMinimosParaAlerta()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.ProdutoEstoque>();
            var result = from obj in query where obj.Quantidade <= obj.EstoqueMinimo && obj.EstoqueMinimo > 0 select obj;

            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.NotaFiscal.ProdutoEstoque BuscarPorProduto(int codigo, int? codigoEmpresa, int? codigoLocalArmazenamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.ProdutoEstoque>();
            var result = from obj in query where obj.Produto.Codigo == codigo select obj;

            if (codigoEmpresa.HasValue && codigoEmpresa > 0)
                result = result.Where(o => o.Empresa == null || o.Empresa.Codigo == codigoEmpresa);

            if (codigoLocalArmazenamento.HasValue && codigoLocalArmazenamento > 0)
                result = result.Where(o => o.LocalArmazenamento.Codigo == codigoLocalArmazenamento || o.LocalArmazenamento == null);

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.NotaFiscal.ProdutoEstoque BuscarPorProdutoEmpresaLocalArmazenamento(int produto, int codigoEmpresa, int codigoLocalArmazenamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.ProdutoEstoque>();
            var result = from obj in query where obj.Produto.Codigo == produto && obj.LocalArmazenamento.Codigo == codigoLocalArmazenamento select obj;

            if (codigoEmpresa > 0)
                result = result.Where(o => o.Empresa.Codigo == codigoEmpresa);

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.NotaFiscal.ProdutoEstoque BuscarLocalArmazenamentoEmOutroProduto(int codigo, int produto, int codigoEmpresa, int codigoLocalArmazenamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.ProdutoEstoque>();
            var result = from obj in query where obj.Produto.Codigo == produto && obj.LocalArmazenamento.Codigo == codigoLocalArmazenamento && obj.Codigo != codigo select obj;

            if (codigoEmpresa > 0)
                result = result.Where(o => o.Empresa.Codigo == codigoEmpresa);

            return result.FirstOrDefault();
        }

        public bool ExisteLocalArmazenamentoEmOutroProduto(int codigo, int produto, int codigoEmpresa, int codigoLocalArmazenamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.ProdutoEstoque>();
            var result = from obj in query where obj.Produto.Codigo == produto && obj.LocalArmazenamento.Codigo == codigoLocalArmazenamento && obj.Codigo != codigo select obj;

            if (codigoEmpresa > 0)
                result = result.Where(o => o.Empresa.Codigo == codigoEmpresa);

            return result.Count() > 0;
        }

        public decimal BuscarEstoquePosicao(int codigo, int codigoEmpresa, DateTime dataPosicaoEstoque)
        {
            string query = $@"SELECT ISNULL((SELECT SUM(CASE WHEN H.PEH_TIPO = 0 THEN H.PEH_QUANTIDADE ELSE (PEH_QUANTIDADE * -1) END) 
                                            FROM T_PRODUTO_ESTOQUE_HISTORICO H
                                            WHERE H.PRO_CODIGO = P.PRO_CODIGO 
                                            AND (H.PRE_CODIGO = E.PRE_CODIGO OR H.PRE_CODIGO IS NULL) 
                                            AND CAST(H.PEH_DATA AS DATE) <= '{dataPosicaoEstoque.ToString("yyyy-MM-dd")}' ), 0) QuantidadeEstoquePosicao
                            from T_PRODUTO p
                            join T_PRODUTO_ESTOQUE e on e.PRO_CODIGO = p.PRO_CODIGO
                            where p.PRO_CODIGO = {codigo}
                            and (e.LAP_CODIGO = 1 or e.LAP_CODIGO is null) ";

            if (codigoEmpresa != 0)
                query += $@" and e.EMP_CODIGO = {codigoEmpresa} ";

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            return nhQuery.UniqueResult<decimal>();
        }

        public List<Dominio.Entidades.Embarcador.NotaFiscal.ProdutoEstoque> ConsultaProdutoComEstoque(string codigoBarrasEAN, string descricao, string ncm, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, int codigoEmpresa, string codigoProdutoEmbarcador, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = _ConsultarProdutoComEstoque(codigoBarrasEAN, descricao, ncm, ativo, codigoEmpresa, codigoProdutoEmbarcador);

            query = query.Fetch(o => o.Produto);

            return ObterLista(query, propOrdenacao, dirOrdenacao, inicioRegistros, maximoRegistros);
        }

        public int ContaConsultaProdutoComEstoque(string codigoBarrasEAN, string descricao, string ncm, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, int codigoEmpresa, string codigoProdutoEmbarcador)
        {
            var query = _ConsultarProdutoComEstoque(codigoBarrasEAN, descricao, ncm, ativo, codigoEmpresa, codigoProdutoEmbarcador);

            return query.Count();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Produtos.ProdutoComEstoqueAgrupado> ConsultaProdutoComEstoqueAgrupado(string codigo, string codigoBarrasEAN, string descricao, string ncm, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, int codigoEmpresa, string codigoProdutoEmbarcador, bool IsCodigoDeBarras, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var consulta = this.SessionNHiBernate.CreateSQLQuery(QueryProdutoComEstoqueAgrupado(codigo, codigoBarrasEAN, descricao, ncm, codigoEmpresa, codigoProdutoEmbarcador, ativo, false, IsCodigoDeBarras, propOrdenacao, dirOrdenacao, inicioRegistros, maximoRegistros));

            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Produtos.ProdutoComEstoqueAgrupado)));

            return consulta.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Produtos.ProdutoComEstoqueAgrupado>();
        }

        private string QueryProdutoComEstoqueAgrupado(string codigo, string codigoBarrasEAN, string descricao, string ncm, int codigoEmpresa, string codigoProdutoEmbarcador, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, bool somenteContarNumeroRegistros, bool IsCodigoDeBarras, string propOrdenacao = "", string dirOrdenacao = "", int inicioRegistros = 0, int maximoRegistros = 0)
        {
            string sql;
            if (somenteContarNumeroRegistros)
                sql = "SELECT DISTINCT(COUNT(0) OVER ())";
            else
                sql = @"SELECT Produto.PRO_CODIGO Codigo,
                         Produto.PRO_DESCRICAO Descricao,
                         Produto.PRO_UNIDADE_MEDIDA UnidadeDeMedida,
                         Produto.PRO_COD_PRODUTO CodigoProduto,
                         Produto.PRO_COD_NCM CodigoNCM,
                         Produto.PRO_CODIGO_BARRAS_EAN CodigoBarrasEAN,
                         Produto.PRO_STATUS Status,
                        (select SUM(PRE_QUANTIDADE) from T_PRODUTO_ESTOQUE ProdutoEstoque where ProdutoEstoque.PRO_CODIGO = Produto.PRO_CODIGO group by PRO_CODIGO) as Estoque 
                        ";

            sql += @" FROM T_PRODUTO Produto WHERE 1 = 1";

            if (!string.IsNullOrWhiteSpace(codigo))
                sql += $" and Produto.PRO_CODIGO = '{codigo}'";

            if (!string.IsNullOrWhiteSpace(codigoBarrasEAN))
                sql += $" and Produto.PRO_CODIGO_EAN like '%{codigoBarrasEAN}%'";

            if (!string.IsNullOrWhiteSpace(descricao))
                sql += $" and Produto.PRO_DESCRICAO like '%{descricao}%'";

            if (!string.IsNullOrWhiteSpace(ncm))
                sql += $" and Produto.PRO_COD_NCM like '%{ncm}%'";

            if (!string.IsNullOrWhiteSpace(codigoProdutoEmbarcador))
            {
                sql += $" and Produto.PRO_COD_PRODUTO like '%{codigoProdutoEmbarcador}%'";
            }
            if (ativo == SituacaoAtivoPesquisa.Ativo)
                sql += $" and Produto.PRO_STATUS = 'A' ";
            else if (ativo == SituacaoAtivoPesquisa.Inativo)
                sql += $" and Produto.PRO_STATUS = 'I' ";

            if (!somenteContarNumeroRegistros && !string.IsNullOrWhiteSpace(propOrdenacao))
            {
                sql += $" order by {propOrdenacao} {dirOrdenacao}";

                if ((inicioRegistros > 0) || (maximoRegistros > 0))
                    sql += $" offset {inicioRegistros} rows fetch next {maximoRegistros} rows only;";
            }

            return sql;
        }

        public int ContaConsultaProdutoComEstoqueAgrupado(string codigo, string codigoBarrasEAN, string descricao, string ncm, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, int codigoEmpresa, string codigoProdutoEmbarcador, bool IsCodigoDeBarras)
        {
            var consulta = this.SessionNHiBernate.CreateSQLQuery(QueryProdutoComEstoqueAgrupado(codigo, codigoBarrasEAN, descricao, ncm, codigoEmpresa, codigoProdutoEmbarcador, ativo, true, IsCodigoDeBarras));

            return consulta.SetTimeout(600).UniqueResult<int>();
        }


        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.NotaFiscal.ProdutoEstoque> _Consultar(int produto)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.ProdutoEstoque>();

            var result = from obj in query where obj.Produto.Codigo == produto select obj;

            return result;
        }

        private IQueryable<Dominio.Entidades.Embarcador.NotaFiscal.ProdutoEstoque> _ConsultarProdutoComEstoque(string codigoBarrasEAN, string descricao, string ncm, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, int codigoEmpresa, string codigoProdutoEmbarcador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.ProdutoEstoque>();

            if (!string.IsNullOrWhiteSpace(descricao))
                query = query.Where(obj => obj.Produto.Descricao.Contains(descricao));

            if (!string.IsNullOrWhiteSpace(codigoBarrasEAN))
                query = query.Where(obj => obj.Produto.CodigoBarrasEAN.Contains(codigoBarrasEAN) || obj.Produto.CodigoEAN.Contains(codigoBarrasEAN));

            if (!string.IsNullOrWhiteSpace(ncm))
                query = query.Where(obj => obj.Produto.CodigoNCM.Contains(ncm));

            if (!string.IsNullOrWhiteSpace(codigoProdutoEmbarcador))
                query = query.Where(obj => obj.Produto.CodigoProduto.Contains(codigoProdutoEmbarcador) || obj.Produto.Codigo.ToString().Contains(codigoProdutoEmbarcador));

            if (ativo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
            {
                if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                    query = query.Where(obj => obj.Produto.Status.Equals("A"));
                else if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                    query = query.Where(obj => obj.Produto.Status.Equals("I"));
            }

            if (codigoEmpresa > 0)
                query = query.Where(obj => obj.Produto.Empresa.Codigo.Equals(codigoEmpresa));

            return query;
        }

        #endregion
    }
}
