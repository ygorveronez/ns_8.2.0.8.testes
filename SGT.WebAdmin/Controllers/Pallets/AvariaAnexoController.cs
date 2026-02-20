using SGT.WebAdmin.Controllers.Anexo;

namespace SGT.WebAdmin.Controllers.Pallets
{
    [CustomAuthorize("Pallets/Avaria")]
    public class AvariaAnexoController : AnexoController<Dominio.Entidades.Embarcador.Pallets.Avaria.AvariaPalletAnexo, Dominio.Entidades.Embarcador.Pallets.Avaria.AvariaPallet>
    {
		#region Construtores

		public AvariaAnexoController(Conexao conexao) : base(conexao) { }

		#endregion
	}
}