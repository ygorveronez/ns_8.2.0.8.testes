using SGT.WebAdmin.Controllers.Anexo;

namespace SGT.WebAdmin.Controllers.Transportadores
{
    [CustomAuthorize("Transportadores/Transportador")]
    public class TransportadorAnexoController : AnexoController<Dominio.Entidades.EmpresaAnexo, Dominio.Entidades.Empresa>
    {
		#region Construtores

		public TransportadorAnexoController(Conexao conexao) : base(conexao) { }

		#endregion
	}
}