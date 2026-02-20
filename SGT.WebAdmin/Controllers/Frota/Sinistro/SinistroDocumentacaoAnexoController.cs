using SGT.WebAdmin.Controllers.Anexo;

namespace SGT.WebAdmin.Controllers.Frota.Sinistro
{
    [CustomAuthorize("Frota/Sinistro")]
    public class SinistroDocumentacaoAnexoController : AnexoController<Dominio.Entidades.Embarcador.Frota.SinistroDocumentacaoAnexo, Dominio.Entidades.Embarcador.Frota.SinistroDados>
    {
		#region Construtores

		public SinistroDocumentacaoAnexoController(Conexao conexao) : base(conexao) { }

		#endregion
	}
}