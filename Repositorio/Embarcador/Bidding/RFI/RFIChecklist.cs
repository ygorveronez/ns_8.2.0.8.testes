using System.Linq;

namespace Repositorio.Embarcador.Bidding.RFI
{
    public class RFIChecklist : RepositorioBase<Dominio.Entidades.Embarcador.Bidding.RFI.RFIChecklist>
    {
        public RFIChecklist(UnitOfWork unitOfWork) : base(unitOfWork) { }
        #region Métodos Publicos
        public Dominio.Entidades.Embarcador.Bidding.RFI.RFIChecklist BuscarChecklistPorRFIConviteCodigo(int RFIConvitecodigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Bidding.RFI.RFIChecklist>()
                .Where(o => o.RFIConvite.Codigo == RFIConvitecodigo);

            return query.FirstOrDefault();
        }
        #endregion
    }
}
