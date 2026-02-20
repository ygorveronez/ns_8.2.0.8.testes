using SGT.WebAdmin.Controllers.Anexo;

namespace SGT.WebAdmin.Controllers.Pallets
{
    [CustomAuthorize("Pallets/LiquidacaoValePallet")]
    public class LiquidacaoValePalletController : AnexoController<Dominio.Entidades.Embarcador.Pallets.LiquidacaoAnexo, Dominio.Entidades.Embarcador.Pallets.ValePallet>
    {
		#region Construtores

		public LiquidacaoValePalletController(Conexao conexao) : base(conexao) { }

		#endregion
	}
}
