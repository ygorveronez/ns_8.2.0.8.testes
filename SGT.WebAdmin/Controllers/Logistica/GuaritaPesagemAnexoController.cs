using SGT.WebAdmin.Controllers.Anexo;

namespace SGT.WebAdmin.Controllers.Logistica
{
    [CustomAuthorize("GestaoPatio/FluxoPatio")]
    public class GuaritaPesagemAnexoController : AnexoController<Dominio.Entidades.Embarcador.GestaoPatio.GuaritaEntradaPesagemAnexo, Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita>
    {
		#region Construtores

		public GuaritaPesagemAnexoController(Conexao conexao) : base(conexao) { }

		#endregion
	}
}