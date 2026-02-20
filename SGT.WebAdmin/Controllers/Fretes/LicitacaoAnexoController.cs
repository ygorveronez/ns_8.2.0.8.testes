using SGT.WebAdmin.Controllers.Anexo;

namespace SGT.WebAdmin.Controllers.Fretes
{
    [CustomAuthorize("Fretes/LicitacaoAnexo", "Fretes/Licitacao")]
    public class LicitacaoAnexoController : AnexoController<Dominio.Entidades.Embarcador.Frete.LicitacaoAnexo, Dominio.Entidades.Embarcador.Frete.Licitacao>
    {
		#region Construtores

		public LicitacaoAnexoController(Conexao conexao) : base(conexao) { }

		#endregion
	}
}