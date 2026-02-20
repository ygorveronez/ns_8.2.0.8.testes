using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Cargas.ControleEntrega
{
    public class CargaEntregaEvento : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaEvento>
    {
        #region Construtores

        public CargaEntregaEvento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public bool ExistePorTipoOcorrenciaCargaEntrega(int codigoTipoOcorrencia, int codigoCarga, int codigoCargaEntrega)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaEvento>()
                .Where(o => o.TipoDeOcorrencia.Codigo == codigoTipoOcorrencia);

            if (codigoCargaEntrega > 0)
                query = query.Where(obj => obj.CargaEntrega.Codigo == codigoCargaEntrega);
            else
                query = query.Where(obj => obj.Carga.Codigo == codigoCarga);

            return query.Any();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaEvento> BuscarPorCargaEntrega(int codigoCargaEntrega)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaEvento> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaEvento>()
                .Where(o => o.CargaEntrega.Codigo == codigoCargaEntrega);

            return query.OrderBy(cargaEntregaEvento => cargaEntregaEvento.Codigo).ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaEvento BuscarUltimoPorCargaEntrega(int codigoCargaEntrega)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaEvento>()
                .Where(o => o.CargaEntrega.Codigo == codigoCargaEntrega);

            return query.OrderByDescending(o => o.Codigo).FirstOrDefault();
        }

        #endregion
    }
}
