using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using LinqKit;

namespace Repositorio.Embarcador.Pedidos
{
    public class ImportarEDI : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.ImportarEDI>
    {
        public ImportarEDI(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Pedidos.ImportarEDI BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.ImportarEDI>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.ImportarEDI> Consultar(Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaImportarEDI filtroPesquisa, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var result = Consultar(filtroPesquisa);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaImportarEDI filtroPesquisa)
        {
            var result = Consultar(filtroPesquisa);

            return result.Count();
        }

        private IQueryable<Dominio.Entidades.Embarcador.Pedidos.ImportarEDI> Consultar(Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaImportarEDI filtroPesquisa)
        {

            var filtro = PredicateBuilder.True<Dominio.Entidades.Embarcador.Pedidos.ImportarEDI>();
            var consulta = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.ImportarEDI>();

            if (filtroPesquisa.GrupoPessoa > 0)
                filtro = filtro.And(obj => obj.GrupoPessoas.Codigo == filtroPesquisa.GrupoPessoa);
            
            if (filtroPesquisa.TipoOperacao > 0)
                filtro = filtro.And(obj => obj.TipoOperacao.Codigo == filtroPesquisa.TipoOperacao);

            consulta = consulta.Where(filtro);

            return consulta;
        }
    }
}
