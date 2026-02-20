using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Filiais
{
    public sealed class ProdutoEmbarcadorEstoqueArmazem : RepositorioBase<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorEstoqueArmazem>
    {
        #region Construtores

        public ProdutoEmbarcadorEstoqueArmazem(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public ProdutoEmbarcadorEstoqueArmazem(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion

        #region Métodos Públicos

        public IList<Dominio.ObjetosDeValor.Embarcador.Filial.GestaoArmazem> ConsultarGestaoArmazem(Dominio.ObjetosDeValor.Embarcador.Filial.FiltroPesquisaGestaoArmazem filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            var consulta = this.SessionNHiBernate.CreateSQLQuery(QueryConsultarGestaoArmazem(filtrosPesquisa, false, parametroConsulta));

            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Filial.GestaoArmazem)));

            return consulta.SetTimeout(600).List<Dominio.ObjetosDeValor.Embarcador.Filial.GestaoArmazem>();
        }

        public Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorEstoqueArmazem BuscarPorFilialProdutoArmazem(int codigoFilial, int codigoProdutoEmbarador, int codigoFilialArmazem)
        {
            var consultaProdutoEmbarcadorEstoqueArmazem = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorEstoqueArmazem>()
                    .Where(o => (o.Filial.Codigo == codigoFilial) && (o.Produto.Codigo == codigoProdutoEmbarador) && (o.Armazem.Codigo == codigoFilialArmazem));

            return consultaProdutoEmbarcadorEstoqueArmazem.FirstOrDefault();
        }

        public Task<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorEstoqueArmazem> BuscarPorFilialProdutoArmazemAsync(int codigoFilial, int codigoProdutoEmbarador, int codigoFilialArmazem)
        {
            var consultaProdutoEmbarcadorEstoqueArmazem = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorEstoqueArmazem>()
                    .Where(o => (o.Filial.Codigo == codigoFilial) && (o.Produto.Codigo == codigoProdutoEmbarador) && (o.Armazem.Codigo == codigoFilialArmazem));

            return consultaProdutoEmbarcadorEstoqueArmazem.FirstOrDefaultAsync(CancellationToken);
        }

        public List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorEstoqueArmazem> BuscarPorFilialEFilialArmazem(int codigoFilial, int codigoFilialArmazem)
        {
            var consultaProdutoEmbarcadorEstoqueArmazem = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorEstoqueArmazem>()
                    .Where(o => o.Filial.Codigo == codigoFilial && o.Armazem.Codigo == codigoFilialArmazem);

            return consultaProdutoEmbarcadorEstoqueArmazem.ToList();
        }

        public int ContarGestaoArmazem(Dominio.ObjetosDeValor.Embarcador.Filial.FiltroPesquisaGestaoArmazem filtrosPesquisa)
        {
            var consulta = this.SessionNHiBernate.CreateSQLQuery(QueryConsultarGestaoArmazem(filtrosPesquisa, true));

            return consulta.SetTimeout(600).UniqueResult<int>();
        }

        public List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorEstoqueArmazem> BuscarPorCodigosProduto(List<int> codigosProdutoEmbarador)
        {
            var consultaProdutoEmbarcadorEstoqueArmazem = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorEstoqueArmazem>()
                    .Where(o => codigosProdutoEmbarador.Contains(o.Produto.Codigo));

            return consultaProdutoEmbarcadorEstoqueArmazem.ToList();
        }

        public decimal BuscarEstoqueDisponivelPorFilialProdutoArmazem(int codigoFilial, int codigoProdutoEmbarador, int codigoFilialArmazem)
        {
            var consultaProdutoEmbarcadorEstoqueArmazem = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorEstoqueArmazem>()
                    .Where(o => (o.Filial.Codigo == codigoFilial) && (o.Produto.Codigo == codigoProdutoEmbarador) && (o.Armazem.Codigo == codigoFilialArmazem));

            return consultaProdutoEmbarcadorEstoqueArmazem.Select(o => (decimal?)o.EstoqueDisponivel).FirstOrDefault() ?? 0;
        }

        #endregion

        #region Métodos Privados

        private string QueryConsultarGestaoArmazem(Dominio.ObjetosDeValor.Embarcador.Filial.FiltroPesquisaGestaoArmazem filtrosPesquisa, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta = null)
        {
            string sql = "";

            if (somenteContarNumeroRegistros)
                sql = "select distinct(count(0) over ()) ";
            else
                sql = @"select  
                                ProdutoEmbarcadorEstoqueArmazem.PEA_CODIGO Codigo,
	                            Filial.FIL_CODIGO_FILIAL_EMBARCADOR CodigoFilial,
	                            Filial.FIL_DESCRICAO DescricaoFilial,
	                            ProdutoEmbarcador.PRO_CODIGO_PRODUTO_EMBARCADOR CodigoProduto,
	                            ProdutoEmbarcador.GRP_DESCRICAO DescricaoProduto,
                                FilialArmazem.FIA_CODIGO_INTEGRACAO CodigoArmazem,
	                            ProdutoEmbarcadorEstoqueArmazem.PEA_ESTOQUE_DISPONIVEL EstoqueDisponivel,
	                            ProdutoEmbarcadorEstoqueArmazem.PEA_ESTOQUE_SESSAO_ROTERIZACAO EstoqueSessaoRoterizacao ";

            sql += @"from 
                            T_PRODUTO_EMBARCADOR_ESTOQUE_ARMAZEM ProdutoEmbarcadorEstoqueArmazem
	                        join T_FILIAL Filial on ProdutoEmbarcadorEstoqueArmazem.FIL_CODIGO = Filial.FIL_CODIGO
	                        join T_FILIAL_ARMAZEM FilialArmazem on ProdutoEmbarcadorEstoqueArmazem.FIA_CODIGO = FilialArmazem.FIA_CODIGO
	                        join T_PRODUTO_EMBARCADOR ProdutoEmbarcador on ProdutoEmbarcadorEstoqueArmazem.PRO_CODIGO = ProdutoEmbarcador.PRO_CODIGO ";

            sql += " where 1=1 ";

            if (filtrosPesquisa.CodigosArmazem.Count > 0)
                sql += $" and FilialArmazem.FIA_CODIGO in (" + string.Join(",", filtrosPesquisa.CodigosArmazem) + ')';

            if (filtrosPesquisa.CodigosFilial.Count > 0)
                sql += $" and Filial.FIL_CODIGO in (" + string.Join(",", filtrosPesquisa.CodigosFilial) + ')';

            if (filtrosPesquisa.CodigosProdutoEmbarcador.Count > 0)
                sql += $" and ProdutoEmbarcador.PRO_CODIGO in (" + string.Join(",", filtrosPesquisa.CodigosProdutoEmbarcador) + ')';

            if (!somenteContarNumeroRegistros && !string.IsNullOrWhiteSpace(parametroConsulta.PropriedadeOrdenar))
            {
                sql += $" order by {parametroConsulta.PropriedadeOrdenar} {parametroConsulta.DirecaoOrdenar}";

                if ((parametroConsulta.InicioRegistros > 0) || (parametroConsulta.LimiteRegistros > 0))
                    sql += $" offset {parametroConsulta.InicioRegistros} rows fetch next {parametroConsulta.LimiteRegistros} rows only;";
            }

            return sql;
        }
        #endregion
    }
}
