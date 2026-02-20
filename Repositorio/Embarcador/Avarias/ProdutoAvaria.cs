using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Avarias
{
    public class ProdutoAvaria : RepositorioBase<Dominio.Entidades.Embarcador.Avarias.ProdutoAvaria>
    {

        public ProdutoAvaria(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Avarias.ProdutoAvaria BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.ProdutoAvaria>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Avarias.ProdutoAvaria BuscarPorProduto(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.ProdutoAvaria>();
            var result = from obj in query where obj.ProdutoEmbarcador.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public bool ValidaPorProduto(int codigoProduto)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.ProdutoAvaria>();
            var result = from obj in query where obj.ProdutoEmbarcador.Codigo == codigoProduto select obj;
            return result.Count() == 0;
        }

        public bool ValidaPorProduto(int codigo, int codigoProduto)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.ProdutoAvaria>();
            var result = from obj in query
                         where 
                            obj.ProdutoEmbarcador.Codigo == codigoProduto &&
                            obj.Codigo != codigo
                         select obj;
            return result.Count() == 0;
        }

        private IQueryable<Dominio.Entidades.Embarcador.Avarias.ProdutoAvaria> _Consultar(Dominio.ObjetosDeValor.Embarcador.Avarias.FiltroPesquisaProdutoAvaria filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.ProdutoAvaria>();

            // Filtros
            if (filtrosPesquisa.CodigoProduto > 0)
                query = query.Where(o => o.ProdutoEmbarcador.Codigo == filtrosPesquisa.CodigoProduto);

            if (filtrosPesquisa.CodigoGrupoProduto > 0)
                query = query.Where(o => o.ProdutoEmbarcador.GrupoProduto.Codigo == filtrosPesquisa.CodigoGrupoProduto);

            if (filtrosPesquisa.DataInicial != DateTime.MinValue && filtrosPesquisa.DataFinal != DateTime.MinValue)
                query = query.Where(o => o.DataCadastro > filtrosPesquisa.DataInicial.Date && o.DataCadastro < filtrosPesquisa.DataFinal.AddDays(1).Date);
            else if (filtrosPesquisa.DataInicial != DateTime.MinValue)
                query = query.Where(o => o.DataCadastro > filtrosPesquisa.DataInicial.Date);
            else if (filtrosPesquisa.DataFinal != DateTime.MinValue)
                query = query.Where(o => o.DataCadastro < filtrosPesquisa.DataFinal.AddDays(1).Date);
            
            return query;
        }

        public List<Dominio.Entidades.Embarcador.Avarias.ProdutoAvaria> Consultar(Dominio.ObjetosDeValor.Embarcador.Avarias.FiltroPesquisaProdutoAvaria filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            var result = _Consultar(filtrosPesquisa);
            
            return ObterLista(result, parametroConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Avarias.FiltroPesquisaProdutoAvaria filtrosPesquisa)
        {
            var result = _Consultar(filtrosPesquisa);

            return result.Count();
        }
    }
}
