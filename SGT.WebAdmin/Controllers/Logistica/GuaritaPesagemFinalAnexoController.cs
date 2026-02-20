using SGT.WebAdmin.Controllers.Anexo;

namespace SGT.WebAdmin.Controllers.Logistica
{
    [CustomAuthorize("GestaoPatio/FluxoPatio")]
    public class GuaritaPesagemFinalAnexoController : AnexoController<Dominio.Entidades.Embarcador.GestaoPatio.GuaritaEntradaPesagemFinalAnexo, Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita>
    {
		#region Construtores

		public GuaritaPesagemFinalAnexoController(Conexao conexao) : base(conexao) { }

		#endregion
	}
}