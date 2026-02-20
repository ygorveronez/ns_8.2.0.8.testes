using SGT.WebAdmin.Controllers.Anexo;

namespace SGT.WebAdmin.Controllers.GestaoPatio
{
    [CustomAuthorize(new string[] { "GestaoPatio/FluxoPatio", "GestaoPatio/CheckList" })]
    public class CheckListAnexoController : AnexoController<Dominio.Entidades.Embarcador.GestaoPatio.CheckListAnexo, Dominio.Entidades.Embarcador.GestaoPatio.CheckListCarga>
    {
		#region Construtores

		public CheckListAnexoController(Conexao conexao) : base(conexao) { }

		#endregion
	}
}