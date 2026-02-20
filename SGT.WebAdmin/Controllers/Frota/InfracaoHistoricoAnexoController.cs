using SGT.WebAdmin.Controllers.Anexo;

namespace SGT.WebAdmin.Controllers.Frota
{
    [CustomAuthorize("Frota/InfracaoHistoricoAnexo", "Frota/Infracao")]
    public class InfracaoHistoricoAnexoController : AnexoController<Dominio.Entidades.Embarcador.Frota.InfracaoHistoricoAnexo, Dominio.Entidades.Embarcador.Frota.InfracaoHistorico>
    {
		#region Construtores

		public InfracaoHistoricoAnexoController(Conexao conexao) : base(conexao) { }

		#endregion

		#region Métodos Protegidos Sobrescritos

		protected override bool IsPermitirAdicionarAnexo(Dominio.Entidades.Embarcador.Frota.InfracaoHistorico entidade)
        {
            return entidade.Infracao.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoInfracao.Cancelada;
        }

        protected override bool IsPermitirExcluirAnexo(Dominio.Entidades.Embarcador.Frota.InfracaoHistorico entidade)
        {
            return entidade.Infracao.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoInfracao.Cancelada;
        }

        #endregion
    }
}