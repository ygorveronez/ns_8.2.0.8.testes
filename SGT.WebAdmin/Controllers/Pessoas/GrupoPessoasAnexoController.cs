using SGT.WebAdmin.Controllers.Anexo;

namespace SGT.WebAdmin.Controllers.Pessoas
{
    [CustomAuthorize("Pessoas/GrupoPessoas")]
    public class GrupoPessoasAnexoController : AnexoController<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasAnexo, Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas>
    {
		#region Construtores

		public GrupoPessoasAnexoController(Conexao conexao) : base(conexao) { }

		#endregion
	}
}