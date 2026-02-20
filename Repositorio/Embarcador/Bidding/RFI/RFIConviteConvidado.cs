using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Bidding.RFI
{
    public class RFIConviteConvidado : RepositorioBase<Dominio.Entidades.Embarcador.Bidding.RFI.RFIConviteConvidado>
    {
        public RFIConviteConvidado(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Publicos

        public List<Dominio.Entidades.Embarcador.Bidding.RFI.RFIConviteConvidado> BuscarConvidados(Dominio.Entidades.Embarcador.Bidding.RFI.RFIConvite RFIConvite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Bidding.RFI.RFIConviteConvidado>()
                .Where(o => o.RFIConvite == RFIConvite);

            return query
                .Fetch(o => o.Convidado)
                .ToList();
        }

        public Dominio.Entidades.Embarcador.Bidding.RFI.RFIConviteConvidado BuscarConvidado(Dominio.Entidades.Embarcador.Bidding.RFI.RFIConvite RFIConvite, int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Bidding.RFI.RFIConviteConvidado>()
                .Where(o => o.RFIConvite == RFIConvite && o.Convidado.Codigo == codigoEmpresa);

            return query
                .Fetch(o => o.RFIConvite)
                .FirstOrDefault();
        }

        public void DeletarPorConviteComExcecao(Dominio.Entidades.Embarcador.Bidding.RFI.RFIConvite RFIConvite, List<int> codigosConvidados)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Bidding.RFI.RFIConviteConvidado>()
                            .Where(x => !codigosConvidados.Contains(x.Codigo) && x.RFIConvite == RFIConvite);

            foreach (var alternativa in query)
            {
                this.SessionNHiBernate.Delete(alternativa);
            }
        }

        #endregion Métodos Publicos
    }
}
