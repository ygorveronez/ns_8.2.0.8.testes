using SGT.WebAdmin.Controllers.Anexo;

namespace SGT.WebAdmin.Controllers.Dica
{
    [CustomAuthorize("Dica/Dica")]
    public class DicaAnexoController : AnexoController<Dominio.Entidades.DicaAnexo, Dominio.Entidades.Dica>
    {
		#region Construtores

		public DicaAnexoController(Conexao conexao) : base(conexao) { }

		#endregion
	}
}