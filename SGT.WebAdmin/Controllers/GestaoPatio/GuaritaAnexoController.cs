using SGT.WebAdmin.Controllers.Anexo;

namespace SGT.WebAdmin.Controllers.GestaoPatio
{
    [CustomAuthorize(new string[] { "GestaoPatio/FluxoPatio", "Cargas/CargaJanelaCarregamentoGuarita" })]
    public class GuaritaAnexoController : AnexoController<Dominio.Entidades.Embarcador.GestaoPatio.GuaritaAnexo, Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita>
    {
		#region Construtores

		public GuaritaAnexoController(Conexao conexao) : base(conexao) { }

		#endregion
	}
}