using SGT.WebAdmin.Controllers.Anexo;

namespace SGT.WebAdmin.Controllers.Bidding
{
    [CustomAuthorize("Bidding/BiddingAceitacao")]
    public class BiddingAceitamentoAnexoController : AnexoController<Dominio.Entidades.Embarcador.Bidding.BiddingAceitamentoQuestionarioAnexo, Dominio.Entidades.Embarcador.Bidding.BiddingAceitamentoQuestaoResposta>
    {
		#region Construtores

		public BiddingAceitamentoAnexoController(Conexao conexao) : base(conexao) { }

		#endregion
	}
}