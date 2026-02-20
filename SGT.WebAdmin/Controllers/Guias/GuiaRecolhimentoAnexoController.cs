using SGT.WebAdmin.Controllers.Anexo;

namespace SGT.WebAdmin.Controllers.Guias
{
    [CustomAuthorize("Guias/VincularGuia", "Guias/GuiaRecolhimento")]
    public class GuiaRecolhimentoAnexoController : AnexoController<Dominio.Entidades.Embarcador.Guias.GuiaRecolhimentoAnexo, Dominio.Entidades.Embarcador.CTe.GuiaNacionalRecolhimentoTributoEstadual>
    {
		#region Construtores

		public GuiaRecolhimentoAnexoController(Conexao conexao) : base(conexao) { }

		#endregion

		#region Métodos Protegidos Sobrescritos

		protected override bool IsPermitirAdicionarAnexo(Dominio.Entidades.Embarcador.CTe.GuiaNacionalRecolhimentoTributoEstadual guia)
        {
            return false;
        }

        protected override bool IsPermitirExcluirAnexo(Dominio.Entidades.Embarcador.CTe.GuiaNacionalRecolhimentoTributoEstadual guia)
        {
            return false;
        }

        #endregion
    }
}