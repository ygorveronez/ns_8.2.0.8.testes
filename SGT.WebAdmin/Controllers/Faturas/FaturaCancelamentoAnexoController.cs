using SGT.WebAdmin.Controllers.Anexo;

namespace SGT.WebAdmin.Controllers.Faturas
{
    [CustomAuthorize("Faturas/Fatura", "Cargas/Carga", "SAC/AtendimentoCliente")]
    public class FaturaCancelamentoAnexoController : AnexoController<Dominio.Entidades.Embarcador.Fatura.FaturaCancelamentoAnexo, Dominio.Entidades.Embarcador.Fatura.Fatura>
    {
		#region Construtores

		public FaturaCancelamentoAnexoController(Conexao conexao) : base(conexao) { }

		#endregion
	}
}