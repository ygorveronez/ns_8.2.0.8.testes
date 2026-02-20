using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Cargas
{
    public class TipoAnexo : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.TipoAnexo>
    {
        #region Constructor
        public  TipoAnexo (Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }
        #endregion

        #region Metodos Privados
        private IQueryable<Dominio.Entidades.Embarcador.Cargas.TipoAnexo> Consulta(string descricao, bool status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.TipoAnexo>();
            query = query.Where(t => t.Status == status);

            if (!string.IsNullOrEmpty(descricao))
                query = query.Where(t => t.Descricao.Contains(descricao));

            return query;
        }

        #endregion

        #region Metodos Publicos
        public List<Dominio.Entidades.Embarcador.Cargas.TipoAnexo> Consultar(string descricao, bool status, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta ) {
        
            var consulta = Consulta(descricao, status);
            return ObterLista(consulta, parametroConsulta);
        }

        public int ContarConsulta(string descricao, bool status)
        {
            var consulta = Consulta(descricao, status);
            return consulta.Count();
        }
        #endregion
    }
}
