using SGT.WebAdmin.Controllers.Anexo;

namespace SGT.WebAdmin.Controllers.Fretes
{
    [CustomAuthorize("Fretes/TabelaFreteAnexo", "Fretes/TabelaFrete")]
    public class TabelaFreteAnexoController : AnexoController<Dominio.Entidades.Embarcador.Frete.TabelaFreteAnexo, Dominio.Entidades.Embarcador.Frete.TabelaFrete>
    {
		#region Construtores

		public TabelaFreteAnexoController(Conexao conexao) : base(conexao) { }

		#endregion

		#region Métodos Protegidos Sobrescritos

		protected override bool IsPermitirAdicionarAnexo(Dominio.Entidades.Embarcador.Frete.TabelaFrete entidade)
        {
            return true;
        }

        protected override bool IsPermitirExcluirAnexo(Dominio.Entidades.Embarcador.Frete.TabelaFrete entidade)
        {
            return true;
        }

        #endregion
    }
}