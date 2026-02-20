using SGT.WebAdmin.Controllers.Anexo;

namespace SGT.WebAdmin.Controllers.Financeiros
{
    [CustomAuthorize("Financeiros/TermoQuitacaoDocumentoAssinado", "Financeiros/TermoQuitacaoDocumento")]
    public class TermoQuitacaoDocumentoAssinadoController : AnexoController<Dominio.Entidades.Embarcador.Financeiro.TermoQuitacaoControleDocumentoAssinado, Dominio.Entidades.Embarcador.Financeiro.TermoQuitacaoFinanceiro>
    {
		#region Construtores

		public TermoQuitacaoDocumentoAssinadoController(Conexao conexao) : base(conexao) { }

		#endregion
	}
}