using SGT.WebAdmin.Controllers.Anexo;

namespace SGT.WebAdmin.Controllers.Cargas.Cancelamento
{
    [CustomAuthorize("Cargas/CancelamentoCarga")]
    public class CancelamentoCargaAnexoController : AnexoController<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoAnexo, Dominio.Entidades.Embarcador.Cargas.CargaCancelamento>
    {
		#region Construtores

		public CancelamentoCargaAnexoController(Conexao conexao) : base(conexao) { }

		#endregion
	}
}