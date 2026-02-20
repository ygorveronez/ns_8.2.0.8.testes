using SGT.WebAdmin.Controllers.Anexo;

namespace SGT.WebAdmin.Controllers.GestaoPatio
{
    [CustomAuthorize("GestaoPatio/FluxoPatio")]
    public class FluxoPatioDocumentosPesagemAnexoNotaFiscalComplementarController : AnexoController<Dominio.Entidades.Embarcador.GestaoPatio.FluxoPatioDocumentosPesagemNotaFiscalComplementarAnexo, Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio>
    {
		#region Construtores

		public FluxoPatioDocumentosPesagemAnexoNotaFiscalComplementarController(Conexao conexao) : base(conexao) { }

		#endregion

		protected virtual bool IsPermitirAdicionarAnexo(Dominio.Entidades.Embarcador.GestaoPatio.FluxoPatioDocumentosPesagemNotaFiscalComplementarAnexo entidade)
        {
            return (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS);
        }

        protected virtual bool IsPermitirExcluirAnexo(Dominio.Entidades.Embarcador.GestaoPatio.FluxoPatioDocumentosPesagemNotaFiscalComplementarAnexo entidade)
        {
            return (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS);
        }
    }
}