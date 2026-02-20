using SGT.WebAdmin.Controllers.Anexo;

namespace SGT.WebAdmin.Controllers.PagamentoMotorista
{
    [CustomAuthorize("PagamentosMotoristas/PagamentoMotoristaTMS", "PagamentosMotoristas/AutorizacaoPagamentoMotorista")]
    public class PagamentoMotoristaTMSAnexoController : AnexoController<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMSAnexo, Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS>
    {
		#region Construtores

		public PagamentoMotoristaTMSAnexoController(Conexao conexao) : base(conexao) { }

		#endregion
	}
}