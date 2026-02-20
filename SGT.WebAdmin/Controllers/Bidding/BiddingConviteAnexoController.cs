using SGT.WebAdmin.Controllers.Anexo;

namespace SGT.WebAdmin.Controllers.Bidding
{
    [CustomAuthorize("Bidding/BiddingConvite")]
    public class BiddingConviteAnexoController : AnexoController<Dominio.Entidades.Embarcador.Bidding.BiddingConviteAnexo, Dominio.Entidades.Embarcador.Bidding.BiddingConvite>
    {
		#region Construtores

		public BiddingConviteAnexoController(Conexao conexao) : base(conexao) { }

		#endregion
	}
}