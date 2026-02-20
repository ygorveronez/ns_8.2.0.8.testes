using SGT.WebAdmin.Controllers.Anexo;

namespace SGT.WebAdmin.Controllers.Frota
{
    [CustomAuthorize("Frota/Infracao")]
    public class InfracaoAnexoController : AnexoController<Dominio.Entidades.Embarcador.Frota.InfracaoAnexo, Dominio.Entidades.Embarcador.Frota.Infracao>
    {
		#region Construtores

		public InfracaoAnexoController(Conexao conexao) : base(conexao) { }

		#endregion
	}
}