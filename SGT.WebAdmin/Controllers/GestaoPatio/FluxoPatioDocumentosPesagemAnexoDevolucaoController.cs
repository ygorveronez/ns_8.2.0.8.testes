using SGT.WebAdmin.Controllers.Anexo;

namespace SGT.WebAdmin.Controllers.GestaoPatio
{
    [CustomAuthorize("GestaoPatio/FluxoPatio")]
    public class FluxoPatioDocumentosPesagemAnexoDevolucaoController : AnexoController<Dominio.Entidades.Embarcador.GestaoPatio.FluxoPatioDocumentosPesagemDevolucaoAnexo, Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio>
    {
		#region Construtores

		public FluxoPatioDocumentosPesagemAnexoDevolucaoController(Conexao conexao) : base(conexao) { }

		#endregion

		protected virtual bool IsPermitirAdicionarAnexo(Dominio.Entidades.Embarcador.GestaoPatio.FluxoPatioDocumentosPesagemDevolucaoAnexo entidade)
        {
            return (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS);
        }

        protected virtual bool IsPermitirExcluirAnexo(Dominio.Entidades.Embarcador.GestaoPatio.FluxoPatioDocumentosPesagemDevolucaoAnexo entidade)
        {
            return (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS);
        }
    }
}