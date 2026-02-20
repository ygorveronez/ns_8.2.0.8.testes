
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;

namespace Repositorio.WebService
{
    public class MetodosRest : RepositorioBase<Dominio.Entidades.WebService.MetodosRest>
    {
        #region Constructores
        public MetodosRest(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }
        #endregion

        #region Metodos Publicos
        public List<Dominio.Entidades.WebService.MetodosRest> Buscar(string nomeMetodo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.WebService.MetodosRest>();

            if (!string.IsNullOrEmpty(nomeMetodo))
                query = query.Where(m => m.NomeMetodo.Contains(nomeMetodo));

            return query.ToList();
        }

        public Dominio.Entidades.WebService.MetodosRest BuscarPorNome(string nomeMetodo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.WebService.MetodosRest>();
            return query.Where( m => m.NomeMetodo == nomeMetodo).FirstOrDefault();
        }
        public bool ExiteMetodoSalvoNoBanco(string nomeMetodo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.WebService.MetodosRest>();
            return query.Where(m => m.NomeMetodo == nomeMetodo).FirstOrDefault() != null;
        }

        #endregion
    }
}
