using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Bidding.RFI
{
    public class RFIConviteAnexo : RepositorioBase<Dominio.Entidades.Embarcador.Bidding.RFI.RFIConviteAnexo>
    {
        public RFIConviteAnexo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Publicos

        public List<Dominio.Entidades.Embarcador.Bidding.RFI.RFIConviteAnexo> BuscarPorConvite(Dominio.Entidades.Embarcador.Bidding.RFI.RFIConvite RFIConvite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Bidding.RFI.RFIConviteAnexo>()
                .Where(o => o.EntidadeAnexo == RFIConvite);

            return query.ToList();
        }


        public void DeletarPorConvite(Dominio.Entidades.Embarcador.Bidding.RFI.RFIConvite RFIConvite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Bidding.RFI.RFIConviteAnexo>()
                            .Where(x => x.EntidadeAnexo == RFIConvite);

            foreach (var alternativa in query)
            {
                this.SessionNHiBernate.Delete(alternativa);
            }
        }

        #endregion Métodos Publicos
    }
}
