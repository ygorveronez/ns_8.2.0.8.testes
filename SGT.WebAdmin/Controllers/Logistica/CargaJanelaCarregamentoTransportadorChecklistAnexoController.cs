using SGT.WebAdmin.Controllers.Anexo;

namespace SGT.WebAdmin.Controllers.Logistica
{
    [CustomAuthorize("Logistica/JanelaCarregamentoTransportador")]
    public class CargaJanelaCarregamentoTransportadorChecklistAnexoController : AnexoController<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorChecklistAnexo, Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorChecklist>
    {
        #region Construtores

        public CargaJanelaCarregamentoTransportadorChecklistAnexoController(Conexao conexao) : base(conexao) { }

        #endregion
    }
}