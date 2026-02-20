using SGT.WebAdmin.Controllers.Anexo;

namespace SGT.WebAdmin.Controllers.GestaoPallet
{
    [CustomAuthorize("GestaoPallet/ControlePallet", "GestaoPallet/ControleSaldoPallet")]
    public class ControlePalletAnexoController : AnexoController<Dominio.Entidades.Embarcador.GestaoPallet.MovimentacaoPalletAnexo, Dominio.Entidades.Embarcador.GestaoPallet.MovimentacaoPallet>
    {
        #region Construtores

        public ControlePalletAnexoController(Conexao conexao) : base(conexao) { }

        #endregion
    }
}