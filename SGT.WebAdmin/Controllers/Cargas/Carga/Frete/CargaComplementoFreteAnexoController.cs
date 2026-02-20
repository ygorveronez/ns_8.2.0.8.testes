using SGT.WebAdmin.Controllers.Anexo;

namespace SGT.WebAdmin.Controllers.Cargas.Carga.Frete
{
    [CustomAuthorize("Cargas/CargaComplementoFreteAnexo", "Cargas/Carga", "Logistica/JanelaCarregamento")]
    public class CargaComplementoFreteAnexoController : AnexoController<Dominio.Entidades.Embarcador.Cargas.CargaComplementoFreteAnexo, Dominio.Entidades.Embarcador.Cargas.CargaComplementoFrete>
    {
		#region Construtores

		public CargaComplementoFreteAnexoController(Conexao conexao) : base(conexao) { }

		#endregion
	}
}