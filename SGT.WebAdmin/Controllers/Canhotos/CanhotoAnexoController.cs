using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGT.WebAdmin.Controllers.Anexo;

namespace SGT.WebAdmin.Controllers.Canhotos
{
    [CustomAuthorize("Canhotos/CanhotoAnexo", "Canhotos/Canhoto")]
    public class CanhotoAnexoController : AnexoController<Dominio.Entidades.Embarcador.Canhotos.CanhotoAnexo, Dominio.Entidades.Embarcador.Canhotos.Canhoto>
    {
		#region Construtores

		public CanhotoAnexoController(Conexao conexao) : base(conexao) { }

		#endregion

		#region Métodos Protegidos Sobrescritos

		protected override bool IsPermitirAdicionarAnexo(Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto)
        {
            return canhoto.SituacaoDigitalizacaoCanhoto != SituacaoDigitalizacaoCanhoto.Digitalizado;
        }

        protected override bool IsPermitirExcluirAnexo(Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto)
        {
            return canhoto.SituacaoDigitalizacaoCanhoto != SituacaoDigitalizacaoCanhoto.Digitalizado;
        }

        #endregion
    }
}