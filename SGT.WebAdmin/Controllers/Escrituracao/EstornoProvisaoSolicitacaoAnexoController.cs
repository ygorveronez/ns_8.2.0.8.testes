using SGT.WebAdmin.Controllers.Anexo;

namespace SGT.WebAdmin.Controllers.Escrituracao
{
    [CustomAuthorize("Escrituracao/AutorizacaoEstornoProvisao")]
    public class EstornoProvisaoSolicitacaoAnexoController : AnexoController<Dominio.Entidades.Embarcador.Escrituracao.EstornoProvisaoSolicitacaoAnexo, Dominio.Entidades.Embarcador.Escrituracao.EstornoProvisaoSolicitacao>
    {
		#region Construtores

		public EstornoProvisaoSolicitacaoAnexoController(Conexao conexao) : base(conexao) { }

		#endregion

		#region Métodos Protegidos Sobrescritos

		protected override bool IsPermitirAdicionarAnexo(Dominio.Entidades.Embarcador.Escrituracao.EstornoProvisaoSolicitacao ocorrencia)
        {
            return true;
        }

        protected override bool IsPermitirExcluirAnexo(Dominio.Entidades.Embarcador.Escrituracao.EstornoProvisaoSolicitacao ocorrencia)
        {
            return true;
        }

        #endregion
    }
}