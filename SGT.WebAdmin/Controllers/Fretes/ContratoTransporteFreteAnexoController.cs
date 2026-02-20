using SGT.WebAdmin.Controllers.Anexo;

namespace SGT.WebAdmin.Controllers.Fretes
{
    [CustomAuthorize("Fretes/ContratoTransporteFreteAnexo", "Fretes/ContratoTransporteFrete", "Fretes/ContratoTransportadorFrete")]
    public class ContratoTransporteFreteAnexoController : AnexoController<Dominio.Entidades.Embarcador.Frete.ContratoTransporteFreteAnexo, Dominio.Entidades.Embarcador.Frete.ContratoTransporteFrete>
    {
		#region Construtores

		public ContratoTransporteFreteAnexoController(Conexao conexao) : base(conexao) { }

		#endregion
	}
}