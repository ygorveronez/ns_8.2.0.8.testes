using SGT.WebAdmin.Controllers.Anexo;

namespace SGT.WebAdmin.Controllers.GestaoPatio
{
    [CustomAuthorize(new string[] { "GestaoPatio/FluxoPatio", "GestaoPatio/TravamentoChave" })]
    public class TravamentoChaveAnexoController : AnexoController<Dominio.Entidades.Embarcador.GestaoPatio.TravamentoChaveAnexo, Dominio.Entidades.Embarcador.GestaoPatio.TravamentoChave>
    {
		#region Construtores

		public TravamentoChaveAnexoController(Conexao conexao) : base(conexao) { }

		#endregion
	}
}