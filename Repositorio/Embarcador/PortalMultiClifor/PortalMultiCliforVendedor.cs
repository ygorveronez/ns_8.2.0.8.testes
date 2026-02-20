using System.Linq;
using System.Linq.Dynamic.Core;
namespace Repositorio.Embarcador.PortalMultiClifor
{
    public class PortalMultiCliforVendedor : RepositorioBase<Dominio.Entidades.Embarcador.PortalMultiClifor.PortalMultiCliforVendedor>
    {
        #region Construtores

        public PortalMultiCliforVendedor(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos
        public Dominio.Entidades.Embarcador.PortalMultiClifor.PortalMultiCliforVendedor BuscarPorUsuarioAcessoPortal(string usuarioAcessoPortal)
        {
            var consulta = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PortalMultiClifor.PortalMultiCliforVendedor>()
                .Where(o => o.UsuarioAcessoPortal == usuarioAcessoPortal);

            return consulta.FirstOrDefault();
        }
        #endregion
    }
}
