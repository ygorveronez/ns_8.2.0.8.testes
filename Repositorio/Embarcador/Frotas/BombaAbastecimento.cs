using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;

namespace Repositorio.Embarcador.Frotas
{
    public class BombaAbastecimento : RepositorioBase<Dominio.Entidades.Embarcador.Frotas.BombaAbastecimento>
    {
        public BombaAbastecimento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Frotas.BombaAbastecimento> Consultar(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaBombaAbastecimento filtrosPesquisa, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            IQueryable<Dominio.Entidades.Embarcador.Frotas.BombaAbastecimento> result = Consultar(filtrosPesquisa);

            result.Fetch(o => o.LocalArmazenamentoProduto);

            return ObterLista(result, propOrdenacao, dirOrdenacao, inicioRegistros, maximoRegistros);
        }

        private IQueryable<Dominio.Entidades.Embarcador.Frotas.BombaAbastecimento> Consultar(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaBombaAbastecimento filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frotas.BombaAbastecimento>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Descricao))
                result = result.Where(obj => obj.Descricao.Contains(filtrosPesquisa.Descricao));

            if (filtrosPesquisa.LocalArmazenamento > 0)
                result = result.Where(obj => obj.LocalArmazenamentoProduto.Codigo == filtrosPesquisa.LocalArmazenamento);

            return result;
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaBombaAbastecimento filtrosPesquisa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Frotas.BombaAbastecimento> result = Consultar(filtrosPesquisa);

            return result.Count();
        }

        public Dominio.Entidades.Embarcador.Frotas.BombaAbastecimento BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frotas.BombaAbastecimento>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }
    }
}