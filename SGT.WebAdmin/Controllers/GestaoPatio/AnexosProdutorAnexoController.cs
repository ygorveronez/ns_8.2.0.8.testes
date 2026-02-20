using SGT.WebAdmin.Controllers.Anexo;

namespace SGT.WebAdmin.Controllers.Logistica
{
    [CustomAuthorize("GestaoPatio/AnexosProdutor", "GestaoPatio/AnexosProdutor")]
    public class AnexosProdutorAnexoController : AnexoController<Dominio.Entidades.Embarcador.GestaoPatio.AnexoProdutor, Dominio.Entidades.Embarcador.Cargas.Carga>
    {
		#region Construtores

		public AnexosProdutorAnexoController(Conexao conexao) : base(conexao) { }

		#endregion

		protected virtual bool IsPermitirAdicionarAnexo(Dominio.Entidades.Embarcador.GestaoPatio.AnexoProdutor entidade)
        {
            return TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor;
        }
        
        protected virtual bool IsPermitirExcluirAnexo(Dominio.Entidades.Embarcador.GestaoPatio.AnexoProdutor entidade)
        {
            return TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor;
        }
    }
}