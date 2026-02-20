using SGT.WebAdmin.Controllers.Anexo;

namespace SGT.WebAdmin.Controllers.Pallets
{
    [CustomAuthorize("Pallets/Devolucao")]
    public class DevolucaoAnexoController : AnexoController<Dominio.Entidades.Embarcador.Pallets.DevolucaoPalletAnexo, Dominio.Entidades.Embarcador.Pallets.DevolucaoPallet>
    {
		#region Construtores

		public DevolucaoAnexoController(Conexao conexao) : base(conexao) { }

		#endregion
	}
}