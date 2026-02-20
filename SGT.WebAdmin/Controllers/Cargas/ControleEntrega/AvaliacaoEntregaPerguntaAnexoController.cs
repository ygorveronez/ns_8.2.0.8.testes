using SGT.WebAdmin.Controllers.Anexo;

namespace SGT.WebAdmin.Controllers.Cargas.ControleEntrega
{
    [CustomAuthorize("Cargas/AvaliacaoEntrega")]
    public class AvaliacaoEntregaPerguntaAnexoController : AnexoController<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaAvaliacaoAnexo, Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaAvaliacao>
    {
		#region Construtores

		public AvaliacaoEntregaPerguntaAnexoController(Conexao conexao) : base(conexao) { }

		#endregion
	}
}