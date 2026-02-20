using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Cargas.ControleEntrega
{
    public class CargaEntregaNotaFiscalChamado : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscalChamado>
    {
        public CargaEntregaNotaFiscalChamado(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos
        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscalChamado> BuscarPorCodigoChamado(int codigo)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscalChamado>();
            var result = query.Where(obj => obj.Chamado.Codigo == codigo);
            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscalChamado BuscarPorChamadoECargaEntregaNotaFiscal(int codigoChamado, int codigoEntregaNotaFiscal)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscalChamado>();
            var result = query.Where(obj => obj.Chamado.Codigo == codigoChamado && obj.CargaEntregaNotaFiscal.Codigo == codigoEntregaNotaFiscal);
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscalChamado> BuscarPorCargaEntregaCodigoCarga(int codigoCarga)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscalChamado>();
            query = query.Where(obj => obj.CargaEntregaNotaFiscal.CargaEntrega.Carga.Codigo == codigoCarga);
            return query.Fetch(obj => obj.CargaEntregaNotaFiscal).ThenFetch(nf => nf.CargaEntrega).ToList();
        }
        #endregion
    }
}
