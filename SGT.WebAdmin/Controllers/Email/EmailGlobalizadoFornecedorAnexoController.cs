using SGT.WebAdmin.Controllers.Anexo;

namespace SGT.WebAdmin.Controllers.Fretes
{
    [CustomAuthorize("Email/EmailGlobalizadoFornecedorAnexo", "Email/EmailGlobalizadoFornecedor")]
    public class EmailGlobalizadoFornecedorAnexoController : AnexoController<Dominio.Entidades.Embarcador.Email.EmailGlobalizadoFornecedorAnexo, Dominio.Entidades.Embarcador.Email.EmailGlobalizadoFornecedor>
    {
		#region Construtores

		public EmailGlobalizadoFornecedorAnexoController(Conexao conexao) : base(conexao) { }

		#endregion
	}
}