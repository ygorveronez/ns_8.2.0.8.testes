using SGT.WebAdmin.Controllers.Anexo;

namespace SGT.WebAdmin.Controllers.Financeiros
{
    [CustomAuthorize("Financeiros/TituloFinanceiro", "Cargas/Carga")]
    public class TituloCancelamentoAnexoController : AnexoController<Dominio.Entidades.Embarcador.Financeiro.TituloCancelamentoAnexo, Dominio.Entidades.Embarcador.Financeiro.Titulo>
    {
		#region Construtores

		public TituloCancelamentoAnexoController(Conexao conexao) : base(conexao) { }

		#endregion
	}
}