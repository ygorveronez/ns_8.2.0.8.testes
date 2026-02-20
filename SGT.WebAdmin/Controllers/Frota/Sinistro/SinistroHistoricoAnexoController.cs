using SGT.WebAdmin.Controllers.Anexo;

namespace SGT.WebAdmin.Controllers.Frota.Sinistro
{
    [CustomAuthorize("Frota/Sinistro")]
    public class SinistroHistoricoAnexoController : AnexoController<Dominio.Entidades.Embarcador.Frota.SinistroHistoricoAnexo, Dominio.Entidades.Embarcador.Frota.SinistroHistorico>
    {
		#region Construtores

		public SinistroHistoricoAnexoController(Conexao conexao) : base(conexao) { }

		#endregion
	}
}