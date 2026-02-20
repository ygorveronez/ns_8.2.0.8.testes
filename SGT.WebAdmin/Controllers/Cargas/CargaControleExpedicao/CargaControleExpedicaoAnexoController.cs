using SGT.WebAdmin.Controllers.Anexo;

namespace SGT.WebAdmin.Controllers.Cargas.CargaControleExpedicao
{
    [CustomAuthorize("Cargas/CargaControleExpedicao")]
    public class CargaControleExpedicaoAnexoController : AnexoController<Dominio.Entidades.Embarcador.Cargas.CargaControleExpedicaoAnexo, Dominio.Entidades.Embarcador.Cargas.CargaControleExpedicao>
	{
		#region Construtores

		public CargaControleExpedicaoAnexoController(Conexao conexao) : base(conexao) { }

		#endregion
	}
}