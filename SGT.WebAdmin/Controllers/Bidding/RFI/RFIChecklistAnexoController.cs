using SGT.WebAdmin.Controllers.Anexo;

namespace SGT.WebAdmin.Controllers.Bidding
{
    [CustomAuthorize("Bidding/RFIChecklistAnexo")]
    public class RFIChecklistAnexoController : AnexoController<Dominio.Entidades.Embarcador.Bidding.RFI.RFIChecklistQuestionarioAnexo, Dominio.Entidades.Embarcador.Bidding.RFI.RFIChecklistQuestionario>
    {
		#region Construtores

		public RFIChecklistAnexoController(Conexao conexao) : base(conexao) { }

		#endregion
	}
}