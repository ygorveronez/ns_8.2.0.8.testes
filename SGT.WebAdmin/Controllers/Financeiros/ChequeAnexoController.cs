using SGT.WebAdmin.Controllers.Anexo;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace SGT.WebAdmin.Controllers.Financeiros
{
    [CustomAuthorize("Financeiros/ChequeAnexo", "Financeiros/Cheque")]
    public class ChequeAnexoController : AnexoController<Dominio.Entidades.Embarcador.Financeiro.ChequeAnexo, Dominio.Entidades.Embarcador.Financeiro.Cheque>
    {
		#region Construtores

		public ChequeAnexoController(Conexao conexao) : base(conexao) { }

		#endregion

		#region Métodos Protegidos Sobrescritos

		protected override bool IsPermitirAdicionarAnexo(Dominio.Entidades.Embarcador.Financeiro.Cheque entidade)
        {
            return (entidade.Status != StatusCheque.Cancelado) && (entidade.Status != StatusCheque.Compensado);
        }

        protected override bool IsPermitirExcluirAnexo(Dominio.Entidades.Embarcador.Financeiro.Cheque entidade)
        {
            return (entidade.Status != StatusCheque.Cancelado) && (entidade.Status != StatusCheque.Compensado); ;
        }

        #endregion
    }
}