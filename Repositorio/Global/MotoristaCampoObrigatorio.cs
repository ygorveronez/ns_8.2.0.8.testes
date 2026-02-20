using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio
{
    public class MotoristaCampoObrigatorio : RepositorioBase<Dominio.Entidades.MotoristaCampoObrigatorio>
    {
        public MotoristaCampoObrigatorio(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Globais

        public Dominio.Entidades.MotoristaCampoObrigatorio ObterMotoristaCampoObrigatorio()
        {
            IQueryable<Dominio.Entidades.MotoristaCampoObrigatorio> query = this.SessionNHiBernate.Query<Dominio.Entidades.MotoristaCampoObrigatorio>();
            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.MotoristaCampoObrigatorio> Consultar(int codigoTipoOperacao, bool? ativo, string propOrdenar, string dirOrdenar, int inicio, int limite)
        {
            IQueryable<Dominio.Entidades.MotoristaCampoObrigatorio> query = ObterQueryConsulta(propOrdenar, dirOrdenar, inicio, limite);

            return query.ToList();
        }

        public int ContarConsulta(int codigoTipoOperacao, bool? ativo)
        {
            IQueryable<Dominio.Entidades.MotoristaCampoObrigatorio> query = ObterQueryConsulta();

            return query.Count();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.MotoristaCampoObrigatorio> ObterQueryConsulta(string propOrdenar = "", string dirOrdenar = "", int inicio = 0, int limite = 0)
        {
            IQueryable<Dominio.Entidades.MotoristaCampoObrigatorio> query = this.SessionNHiBernate.Query<Dominio.Entidades.MotoristaCampoObrigatorio>();

            if (!string.IsNullOrWhiteSpace(propOrdenar) && !string.IsNullOrWhiteSpace(dirOrdenar))
                query = query.OrderBy(propOrdenar + " " + dirOrdenar);

            if (inicio > 0 || limite > 0)
                query = query.Skip(inicio).Take(limite);

            return query;
        }

        #endregion
    }
}
