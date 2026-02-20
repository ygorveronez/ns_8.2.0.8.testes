using SGT.WebAdmin.Controllers.Anexo;

namespace SGT.WebAdmin.Controllers.Pessoas
{
    [CustomAuthorize("Pessoas/AnexoGrupoPessoasLayoutEDI")]
    public class AnexoGrupoPessoasLayoutEDIController : AnexoController<Dominio.Entidades.Embarcador.Pessoas.AnexoGrupoPessoasLayoutEDI, Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI>
    {
		#region Construtores

		public AnexoGrupoPessoasLayoutEDIController(Conexao conexao) : base(conexao) { }

		#endregion
	}
}