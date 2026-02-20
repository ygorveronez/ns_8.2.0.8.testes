using System;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Filiais
{
    public class FiliaisController : BaseController
    {
		#region Construtores

		public FiliaisController(Conexao conexao) : base(conexao) { }

		#endregion

        [CustomAuthorize("Filiais/ConfiguracaoGestaoPatio")]
        public async Task<IActionResult> ConfiguracaoGestaoPatio()
        {
            return View();
        }

        [CustomAuthorize("Filiais/Filial")]
        public async Task<IActionResult> Filial()
        {
            bool PossuiValorDescargaCliente = false;
            bool PossuiTipoIntegracaoVtex = false;
            bool HabilitarCadastroArmazem = false;

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Pessoas.ClienteDescarga repClienteDescarga = new Repositorio.Embarcador.Pessoas.ClienteDescarga(unitOfWork);
                Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral repConfiguracaoGeral = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeral configuracaoGeral = repConfiguracaoGeral.BuscarConfiguracaoPadrao();

                PossuiValorDescargaCliente = repClienteDescarga.VerificarSeExisteValorDescargaCliente();
                PossuiTipoIntegracaoVtex = repositorioTipoIntegracao.ExistePorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.VTEX);
                HabilitarCadastroArmazem = configuracaoGeral?.HabilitarCadastroArmazem ?? false;
            }
            catch (Exception)
            {
            }
            finally
            {
                unitOfWork.Dispose();
            }

            ViewBag.PossuiValorDescargaCliente = Newtonsoft.Json.JsonConvert.SerializeObject(PossuiValorDescargaCliente);
            ViewBag.PossuiIntegracaoVtex = Newtonsoft.Json.JsonConvert.SerializeObject(PossuiTipoIntegracaoVtex);
            ViewBag.HabilitarCadastroArmazem = Newtonsoft.Json.JsonConvert.SerializeObject(HabilitarCadastroArmazem);

            return View();
        }

        [CustomAuthorize("Filiais/Turno")]
        public async Task<IActionResult> Turno()
        {
            return View();
        }

        [CustomAuthorize("Filiais/SequenciaGestaoPatio")]
        public async Task<IActionResult> SequenciaGestaoPatio()
        {
            return View();
        }

        [CustomAuthorize("Filiais/GestaoArmazem")]
        public async Task<IActionResult> GestaoArmazem()
        {
            return View();
        }
    }
}
