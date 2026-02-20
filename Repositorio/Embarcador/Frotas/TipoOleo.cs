using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;

namespace Repositorio.Embarcador.Frotas
{
    public class TipoOleo : RepositorioBase<Dominio.Entidades.Embarcador.Frotas.TipoOleo>
    {
        public TipoOleo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Frotas.TipoOleo BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frotas.TipoOleo>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }
        public Dominio.Entidades.Embarcador.Frotas.TipoOleo BuscarPorCodigoIntegracao(string codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frotas.TipoOleo>();
            var result = from obj in query where obj.CodigoIntegracao == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Frotas.TipoOleo> Consultar(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaTipoOleo filtrosPesquisa, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            IQueryable<Dominio.Entidades.Embarcador.Frotas.TipoOleo> result = Consultar(filtrosPesquisa);

            result
                .Fetch(o => o.Produto);

            return ObterLista(result, propOrdenacao, dirOrdenacao, inicioRegistros, maximoRegistros);
        }

        private IQueryable<Dominio.Entidades.Embarcador.Frotas.TipoOleo> Consultar(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaTipoOleo filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frotas.TipoOleo>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Descricao))
                result = result.Where(obj => obj.Descricao.Contains(filtrosPesquisa.Descricao));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.TipoDeOleo))
                result = result.Where(obj => obj.TipoDeOleo.Contains(filtrosPesquisa.TipoDeOleo));

            if (filtrosPesquisa.Produto > 0)
                result = result.Where(obj => obj.Produto.Codigo == filtrosPesquisa.Produto);

            return result;
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaTipoOleo filtrosPesquisa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Frotas.TipoOleo> result = Consultar(filtrosPesquisa);

            return result.Count();
        }
    }
}