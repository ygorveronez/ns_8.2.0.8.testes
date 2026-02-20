using SGT.WebAdmin.Controllers.Anexo;

namespace SGT.WebAdmin.Controllers.Notificacoes
{
    [CustomAuthorize("Notificacoes/MensagemAviso")]
    public class MensagemAvisoAnexoController : AnexoController<Dominio.Entidades.MensagemAvisoAnexo, Dominio.Entidades.MensagemAviso>
    {
		#region Construtores

		public MensagemAvisoAnexoController(Conexao conexao) : base(conexao) { }

		#endregion
	}
}