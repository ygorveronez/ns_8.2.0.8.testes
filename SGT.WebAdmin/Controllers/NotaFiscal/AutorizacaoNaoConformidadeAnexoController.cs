using SGT.WebAdmin.Controllers.Anexo;

namespace SGT.WebAdmin.Controllers.NotaFiscal
{
    [CustomAuthorize("NotasFiscais/AutorizacaoNaoConformidade")]
    public class AutorizacaoNaoConformidadeAnexoController : AnexoController<Dominio.Entidades.Embarcador.NotaFiscal.NaoConformidadeAnexo, Dominio.Entidades.Embarcador.NotaFiscal.NaoConformidade>
    {
		#region Construtores

		public AutorizacaoNaoConformidadeAnexoController(Conexao conexao) : base(conexao) { }

		#endregion
	}
}