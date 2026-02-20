using SGT.WebAdmin.Controllers.Anexo;

namespace SGT.WebAdmin.Controllers.Cargas.Carga.Frete
{
    [CustomAuthorize("Cargas/CargaSolicitacaoFreteAnexo", "Cargas/Carga", "Logistica/JanelaCarregamento")]
    public class CargaSolicitacaoFreteAnexoController : AnexoController<Dominio.Entidades.Embarcador.Cargas.CargaSolicitacaoFreteAnexo, Dominio.Entidades.Embarcador.Cargas.Carga>
    {
		#region Construtores

		public CargaSolicitacaoFreteAnexoController(Conexao conexao) : base(conexao) { }

		#endregion
	}
}