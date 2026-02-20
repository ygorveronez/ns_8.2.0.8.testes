using Dominio.Excecoes.Embarcador;
using SGT.WebAdmin.Controllers.Anexo;

namespace SGT.WebAdmin.Controllers.Cargas.Carga.DadosTransporte
{
    [CustomAuthorize("Cargas/Carga", "Logistica/JanelaCarregamento")]
    public class CargaVeiculoContainerAnexoController : AnexoController<Dominio.Entidades.Embarcador.Cargas.CargaVeiculoContainerAnexo, Dominio.Entidades.Embarcador.Cargas.CargaVeiculoContainer>
    {
		#region Construtores

		public CargaVeiculoContainerAnexoController(Conexao conexao) : base(conexao) { }

		#endregion

		#region Métodos Privados

		private bool IsPermitirAdicionarOuExcluirAnexo(Dominio.Entidades.Embarcador.Cargas.CargaVeiculoContainer cargaVeiculoContainer)
        {
            return true;
        }

        #endregion

        #region Métodos Protegidos com Permissão de Sobrescrita

        protected override bool IsPermitirAdicionarAnexo(Dominio.Entidades.Embarcador.Cargas.CargaVeiculoContainer cargaVeiculoContainer)
        {
            return IsPermitirAdicionarOuExcluirAnexo(cargaVeiculoContainer);
        }

        protected override bool IsPermitirExcluirAnexo(Dominio.Entidades.Embarcador.Cargas.CargaVeiculoContainer cargaVeiculoContainer)
        {
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                return false;

            return IsPermitirAdicionarOuExcluirAnexo(cargaVeiculoContainer);
        }

        protected override void PreecherInformacoesAdicionais(Dominio.Entidades.Embarcador.Cargas.CargaVeiculoContainerAnexo cargaAnexo, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.TipoAnexo repositorioTipoAnexo = new Repositorio.Embarcador.Cargas.TipoAnexo(unitOfWork);
            int codigoTipoAnexo = Request.GetIntParam("TipoAnexo");

            Dominio.Entidades.Embarcador.Cargas.TipoAnexo tipoAnexo = repositorioTipoAnexo.BuscarPorCodigo(codigoTipoAnexo, false);

            if (tipoAnexo == null)
                throw new ControllerException("Tipo anexo não encontrado");

            cargaAnexo.TipoAnexo = tipoAnexo;

        }
        #endregion
    }
}