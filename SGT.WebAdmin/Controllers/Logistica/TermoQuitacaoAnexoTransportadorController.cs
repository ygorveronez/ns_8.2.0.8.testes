using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGT.WebAdmin.Controllers.Anexo;

namespace SGT.WebAdmin.Controllers.Logistica
{
    [CustomAuthorize("Logistica/TermoQuitacaoAnexoTransportador")]
    public class TermoQuitacaoAnexoTransportadorController : AnexoController<Dominio.Entidades.Embarcador.Logistica.TermoQuitacao.TermoQuitacaoAnexoTransportador, Dominio.Entidades.Embarcador.Logistica.TermoQuitacao.TermoQuitacao>
    {
		#region Construtores

		public TermoQuitacaoAnexoTransportadorController(Conexao conexao) : base(conexao) { }

		#endregion

		#region Métodos Privados

		private bool IsPermitirAdicionarOuExcluirAnexo(Dominio.Entidades.Embarcador.Logistica.TermoQuitacao.TermoQuitacao termoQuitacao)
        {
            if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                return false;

            return (
                (termoQuitacao.Situacao == SituacaoTermoQuitacao.AguardandoAceiteTransportador) ||
                (termoQuitacao.Situacao == SituacaoTermoQuitacao.AprovacaoRejeitada)
            );
        }

        #endregion

        #region Métodos Protegidos com Permissão de Sobrescrita

        protected override bool IsPermitirAdicionarAnexo(Dominio.Entidades.Embarcador.Logistica.TermoQuitacao.TermoQuitacao termoQuitacao)
        {
            return IsPermitirAdicionarOuExcluirAnexo(termoQuitacao);
        }

        protected override bool IsPermitirExcluirAnexo(Dominio.Entidades.Embarcador.Logistica.TermoQuitacao.TermoQuitacao termoQuitacao)
        {
            return IsPermitirAdicionarOuExcluirAnexo(termoQuitacao);
        }

        #endregion
    }
}