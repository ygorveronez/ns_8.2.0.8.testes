using SGT.WebAdmin.Controllers.Anexo;

namespace SGT.WebAdmin.Controllers.Logistica
{
    [CustomAuthorize("Logistica/AgendamentoColetaAnexo", "Logistica/AgendamentoColeta")]
    public class AgendamentoColetaAnexoController : AnexoController<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaAnexo, Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta>
    {
		#region Construtores

		public AgendamentoColetaAnexoController(Conexao conexao) : base(conexao) { }

		#endregion
	}
}