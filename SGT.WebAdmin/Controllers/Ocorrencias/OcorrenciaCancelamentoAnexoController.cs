using SGT.WebAdmin.Controllers.Anexo;

namespace SGT.WebAdmin.Controllers.Ocorrencias
{
    [CustomAuthorize("Ocorrencias/OcorrenciaCancelamento")]
    public class OcorrenciaCancelamentoAnexoController : AnexoController<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCancelamentoAnexo, Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCancelamento>
    {
		#region Construtores

		public OcorrenciaCancelamentoAnexoController(Conexao conexao) : base(conexao) { }

		#endregion
	}
}