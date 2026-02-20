using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio
{
    public class FiltroPesquisa : RepositorioBase<Dominio.Entidades.Global.FiltroPesquisa>
    {
        public FiltroPesquisa(UnitOfWork unitOfWork) : base(unitOfWork) { }


        public Dominio.Entidades.Global.FiltroPesquisa BuscarFiltroPesquisa(Dominio.ObjetosDeValor.Enumerador.FiltroPesquisa codigoFiltroPesquisa, int codigoUsuario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Global.FiltroPesquisa>();
            return query.Where(o => o.Usuario.Codigo == codigoUsuario && o.CodigoFiltroPesquisa == codigoFiltroPesquisa).FirstOrDefault();
            
        }

        public List<Dominio.Entidades.Global.FiltroPesquisa> BuscarFiltrosPesquisa(Dominio.ObjetosDeValor.Enumerador.FiltroPesquisa codigoFiltroPesquisa, int codigoUsuario, string filtro)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Global.FiltroPesquisa>().Where(o => o.CodigoFiltroPesquisa == codigoFiltroPesquisa);

            if (codigoUsuario > 0)
                query = query.Where(x => x.Usuario.Codigo == codigoUsuario);

            if (!string.IsNullOrEmpty(filtro))
                query = query.Where(x => x.Modelo.Descricao.Contains(filtro));

            return query.OrderByDescending(x=> x.Codigo).ToList();
        }

        public List<Dominio.Entidades.Global.FiltroPesquisa> BuscarFiltrosPesquisaModelo(Dominio.ObjetosDeValor.Enumerador.FiltroPesquisa codigoFiltroPesquisa, int codigoUsuario, string descricaoModelo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Global.FiltroPesquisa>().Where(o => o.CodigoFiltroPesquisa == codigoFiltroPesquisa);

            if (codigoUsuario > 0)
                query = query.Where(x => (x.Usuario.Codigo == codigoUsuario && x.Modelo.ModeloExclusivoUsuario) || !x.Modelo.ModeloExclusivoUsuario);
            else
                query = query.Where(x => !x.Modelo.ModeloExclusivoUsuario);

            if (!string.IsNullOrEmpty(descricaoModelo))
                query = query.Where(x => x.Modelo.Descricao.Contains(descricaoModelo));

            return query.OrderByDescending(x => x.Codigo).ToList();
        }

        public Dominio.Entidades.Global.FiltroPesquisa BuscarFiltroPesquisaPadrao(Dominio.ObjetosDeValor.Enumerador.FiltroPesquisa codigoFiltroPesquisa, int codigoUsuario, bool exclusivo)
		{
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Global.FiltroPesquisa>().Where(o => o.CodigoFiltroPesquisa == codigoFiltroPesquisa && o.Modelo.ModeloPadrao);

            if (codigoUsuario > 0 && exclusivo)
                query = query.Where(x => x.Usuario.Codigo == codigoUsuario && x.Modelo.ModeloExclusivoUsuario);
            else
                query = query.Where(x => !x.Modelo.ModeloExclusivoUsuario);

            return query.OrderByDescending(x => x.Codigo).FirstOrDefault();
        }
    }
}
