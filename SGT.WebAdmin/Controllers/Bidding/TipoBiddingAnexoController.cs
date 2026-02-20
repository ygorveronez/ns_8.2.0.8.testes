using SGT.WebAdmin.Controllers.Anexo;

namespace SGT.WebAdmin.Controllers.Bidding
{
    [CustomAuthorize("Bidding/TipoBidding")]
    public class TipoBiddingAnexoController : AnexoController<Dominio.Entidades.Embarcador.Bidding.TipoBiddingAnexo, Dominio.Entidades.Embarcador.Bidding.TipoBidding>
    {
		#region Construtores

		public TipoBiddingAnexoController(Conexao conexao) : base(conexao) { }

		#endregion
	}
}