using SGT.WebAdmin.Controllers.Anexo;

namespace SGT.WebAdmin.Controllers.Frota.Sinistro
{
    [CustomAuthorize("Frota/Sinistro")]
    public class SinistroIndicacaoPagadorAnexoController : AnexoController<Dominio.Entidades.Embarcador.Frota.SinistroIndicacaoPagadorAnexo, Dominio.Entidades.Embarcador.Frota.SinistroDados>
    {
		#region Construtores

		public SinistroIndicacaoPagadorAnexoController(Conexao conexao) : base(conexao) { }

		#endregion
	}
}