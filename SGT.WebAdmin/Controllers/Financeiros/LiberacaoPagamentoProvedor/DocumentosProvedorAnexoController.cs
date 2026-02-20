using SGT.WebAdmin.Controllers.Anexo;

namespace SGT.WebAdmin.Controllers.Financeiros.LiberacaoPagamentoProvedor
{
    [CustomAuthorize("Financeiros/LiberacaoPagamentoProvedor")]
    public class DocumentosProvedorAnexoController : AnexoController<Dominio.Entidades.Embarcador.Financeiro.DocumentosProvedorAnexo, Dominio.Entidades.Embarcador.Financeiro.PagamentoProvedor>
    {
		#region Construtores

		public DocumentosProvedorAnexoController(Conexao conexao) : base(conexao) { }

		#endregion
	}
}