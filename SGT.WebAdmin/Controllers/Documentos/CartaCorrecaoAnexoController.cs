using SGT.WebAdmin.Controllers.Anexo;


namespace SGT.WebAdmin.Controllers.Documentos
{
    [CustomAuthorize("Documentos/CartaCorrecaoAnexo", "Documentos/ControleDocumento")]
    public class CartaCorrecaoAnexoController : AnexoController<Dominio.Entidades.Embarcador.Documentos.CartaCorrecaoAnexo, Dominio.Entidades.Embarcador.Documentos.ControleDocumento>
    {
		#region Construtores

		public CartaCorrecaoAnexoController(Conexao conexao) : base(conexao) { }

		#endregion
	}
}