using SGT.WebAdmin.Controllers.Anexo;

namespace SGT.WebAdmin.Controllers.Terceiros.ContratoFreteAcrescimoDesconto
{
    [CustomAuthorize("Terceiros/ContratoFreteAcrescimoDesconto")]
    public class ContratoFreteAcrescimoDescontoAnexoController : AnexoController<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDescontoAnexo, Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto>
    {
		#region Construtores

		public ContratoFreteAcrescimoDescontoAnexoController(Conexao conexao) : base(conexao) { }

		#endregion
	}
}