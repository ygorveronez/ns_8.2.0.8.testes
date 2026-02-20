using SGT.WebAdmin.Controllers.Anexo;

namespace SGT.WebAdmin.Controllers.RFI
{
    [CustomAuthorize("Bidding/RFIConvite")]
    public class RFIConviteAnexoController : AnexoController<Dominio.Entidades.Embarcador.Bidding.RFI.RFIConviteAnexo, Dominio.Entidades.Embarcador.Bidding.RFI.RFIConvite>
    {
		#region Construtores

		public RFIConviteAnexoController(Conexao conexao) : base(conexao) { }

		#endregion
	}
}