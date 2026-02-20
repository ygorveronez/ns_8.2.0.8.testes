using SGT.WebAdmin.Controllers.Anexo;

namespace SGT.WebAdmin.Controllers.Chamados
{
    [CustomAuthorize("Chamados/ChamadoOcorrencia")]
    public class ChamadoAnaliseAnexoController : AnexoController<Dominio.Entidades.Embarcador.Chamados.ChamadoAnaliseAnexo, Dominio.Entidades.Embarcador.Chamados.ChamadoAnalise>
    {
		#region Construtores

		public ChamadoAnaliseAnexoController(Conexao conexao) : base(conexao) { }

		#endregion
	}
}