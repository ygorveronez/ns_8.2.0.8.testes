using SGT.WebAdmin.Controllers.Anexo;

namespace SGT.WebAdmin.Controllers.NFS
{
    [CustomAuthorize("NFS/AnexoLancamentoNFSManual", "NFS/NFSManual")]
    public class AnexoLancamentoNFSManualController : AnexoController<Dominio.Entidades.Embarcador.NFS.AnexoLancamentoNFSManual, Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual>
    {
		#region Construtores

		public AnexoLancamentoNFSManualController(Conexao conexao) : base(conexao) { }

		#endregion
	}
}