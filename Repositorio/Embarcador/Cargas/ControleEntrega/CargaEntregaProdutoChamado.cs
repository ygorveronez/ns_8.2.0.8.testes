using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Cargas.ControleEntrega
{
    public class CargaEntregaProdutoChamado : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaProdutoChamado>
    {
        public CargaEntregaProdutoChamado(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos
        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaProdutoChamado> BuscarPorChamado(int codigo)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaProdutoChamado>();
            var result = query.Where(obj => obj.Chamado.Codigo == codigo);
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaProdutoChamado> BuscarPorChamados(List<int> codigos)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaProdutoChamado>();
            var result = query.Where(obj => codigos.Contains(obj.Chamado.Codigo));
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaProdutoChamado> BuscarPorCargaEntregaCodigoCarga(int codigoCarga)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaProdutoChamado>();
            query = query.Where(obj => obj.CargaEntrega.Carga.Codigo == codigoCarga);
            return query.Fetch(obj=> obj.CargaEntrega).ToList();
        }
        #endregion
    }
}
