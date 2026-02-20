using SGT.WebAdmin.Controllers.Anexo;

namespace SGT.WebAdmin.Controllers.Bidding
{
    [CustomAuthorize("Bidding/BiddingChecklistAnexoPadrao", "Bidding/BiddingChecklistQuestionarioPadrao")]
    public class BiddingChecklistAnexoPadraoController : AnexoController<Dominio.Entidades.Embarcador.Bidding.BiddingChecklistQuestionarioAnexoPadrao, Dominio.Entidades.Embarcador.Bidding.BiddingChecklistQuestionarioPadrao>
    {
        #region Construtores

        public BiddingChecklistAnexoPadraoController(Conexao conexao) : base(conexao) { }

        #endregion
    }
}