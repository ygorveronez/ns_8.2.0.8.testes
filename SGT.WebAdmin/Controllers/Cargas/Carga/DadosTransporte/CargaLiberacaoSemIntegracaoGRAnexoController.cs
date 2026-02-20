using SGT.WebAdmin.Controllers.Anexo;

namespace SGT.WebAdmin.Controllers.Cargas.Carga.DadosTransporte
{
    [CustomAuthorize("Cargas/Carga")]
    public class CargaLiberacaoSemIntegracaoGRAnexoController : AnexoController<Dominio.Entidades.Embarcador.Cargas.CargaLiberacaoSemIntegracaoGRAnexo, Dominio.Entidades.Embarcador.Cargas.Carga>
    {
		#region Construtores

		public CargaLiberacaoSemIntegracaoGRAnexoController(Conexao conexao) : base(conexao) { }

		#endregion
	}
}