namespace SGT.WebAdmin.Controllers.Logistica
{
    [CustomAuthorize("Logistica/PosicaoFrota2")]
    public class PosicaoFrota3Controller : PosicaoFrotaController
    {
		#region Construtores

		public PosicaoFrota3Controller(Conexao conexao) : base(conexao) { }

		#endregion
	}
}