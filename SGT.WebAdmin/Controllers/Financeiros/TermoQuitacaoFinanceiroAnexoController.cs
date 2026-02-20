using SGT.WebAdmin.Controllers.Anexo;

namespace SGT.WebAdmin.Controllers.Financeiros
{
    [CustomAuthorize("Financeiros/TermoQuitacao")]
    public class TermoQuitacaoFinanceiroAnexoController : AnexoController<Dominio.Entidades.Embarcador.Financeiro.TermoQuitacaoFinanceiroAnexo, Dominio.Entidades.Embarcador.Financeiro.TermoQuitacaoFinanceiro>
    {
		#region Construtores

		public TermoQuitacaoFinanceiroAnexoController(Conexao conexao) : base(conexao) { }

		#endregion
	}
}