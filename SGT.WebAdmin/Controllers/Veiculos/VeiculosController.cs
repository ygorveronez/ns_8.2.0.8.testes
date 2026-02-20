using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Utilidades.Extensions;

namespace SGT.WebAdmin.Controllers.Veiculos
{
    public class VeiculosController : BaseController
    {
		#region Construtores

		public VeiculosController(Conexao conexao) : base(conexao) { }

		#endregion

        [CustomAuthorize("Veiculos/Veiculo")]
        public async Task<IActionResult> Veiculo()
        {
            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Veiculos/Veiculo");
            ViewBag.PermissoesPersonalizadas = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadas);
            ViewBag.UtilizarAlcadaAprovacaoVeiculo = ConfiguracaoEmbarcador.UtilizarAlcadaAprovacaoVeiculo;

            using (var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Repositorio.Embarcador.Frotas.GeracaoFrotaAutomatizada repoConfiguracaoFrota = new Repositorio.Embarcador.Frotas.GeracaoFrotaAutomatizada(unitOfWork);
                ViewBag.MostrarPaletizadoGeracaoFrota = repoConfiguracaoFrota.ContarTodos() > 0;

                Repositorio.Embarcador.Veiculos.AlcadasCadastroVeiculo.AprovacaoAlcadaCadastroVeiculo repositorioAprovacaoAlcadaCadastroVeiculo = new Repositorio.Embarcador.Veiculos.AlcadasCadastroVeiculo.AprovacaoAlcadaCadastroVeiculo(unitOfWork);
                ViewBag.ExisteRegraAlcadaComFilial = repositorioAprovacaoAlcadaCadastroVeiculo.BuscarSeExisteRegraPorFilial();

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoVeiculo configuracaoVeiculo = new Repositorio.Embarcador.Configuracoes.ConfiguracaoVeiculo(unitOfWork).BuscarConfiguracaoPadrao();

                var configuracoesVeiculo = new
                {
                    configuracaoVeiculo.ObrigarANTTVeiculoValidarSalvarDadosTransporte
                };

                ViewBag.ConfiguracaoVeiculo = configuracoesVeiculo.ToJson();
            }

            return View();
        }

        [CustomAuthorize("Veiculos/MarcaVeiculo")]
        public async Task<IActionResult> MarcaVeiculo()
        {
            return View();
        }

        [CustomAuthorize("Veiculos/MarcaEquipamento")]
        public async Task<IActionResult> MarcaEquipamento()
        {
            return View();
        }

        [CustomAuthorize("Veiculos/Equipamento")]
        public async Task<IActionResult> Equipamento()
        {
            return View();
        }

        [CustomAuthorize("Veiculos/ModeloCarroceria")]
        public async Task<IActionResult> ModeloCarroceria()
        {
            return View();
        }

        [CustomAuthorize("Veiculos/SegmentoVeiculo")]
        public async Task<IActionResult> SegmentoVeiculo()
        {
            return View();
        }

        [CustomAuthorize("Veiculos/TipoPlotagem")]
        public async Task<IActionResult> TipoPlotagem()
        {
            return View();
        }

        [CustomAuthorize("Veiculos/Macro")]
        public async Task<IActionResult> Macro()
        {
            return View();
        }

        [CustomAuthorize("Veiculos/PainelVeiculo")]
        public async Task<IActionResult> PainelVeiculo()
        {
            return View();
        }

        [CustomAuthorize("Veiculos/TipoComunicacaoRastreador")]
        public async Task<IActionResult> TipoComunicacaoRastreador()
        {
            return View();
        }

        [CustomAuthorize("Veiculos/TecnologiaRastreador")]
        public async Task<IActionResult> TecnologiaRastreador()
        {
            return View();
        }

        [CustomAuthorize("Veiculos/ResponsavelVeiculo")]
        public async Task<IActionResult> ResponsavelVeiculo()
        {
            return View();
        }

        [CustomAuthorize("Veiculos/VeiculoMonitoramento")]
        public async Task<IActionResult> VeiculoMonitoramento()
        {
            return View();
        }

        [CustomAuthorize("Veiculos/ModeloEquipamento")]
        public async Task<IActionResult> ModeloEquipamento()
        {
            return View();
        }

        [CustomAuthorize("Veiculos/VeiculoLicenca")]
        public async Task<IActionResult> VeiculoLicenca()
        {
            return View();
        }

        [CustomAuthorize("Veiculos/RegraAutorizacaoCadastroVeiculo")]
        public async Task<IActionResult> RegraAutorizacaoCadastroVeiculo()
        {
            return View();
        }

        [CustomAuthorize("Veiculos/AutorizacaoCadastroVeiculo")]
        public async Task<IActionResult> AutorizacaoCadastroVeiculo()
        {
            return View();
        }

        [CustomAuthorize("Veiculos/ManutencaoCentroResultado")]
        public async Task<IActionResult> ManutencaoCentroResultado()
        {
            return View();
        }
        [CustomAuthorize("Veiculos/CorVeiculo")]
        public async Task<IActionResult> CorVeiculo()
        {
            return View();
        }

        [CustomAuthorize("Veiculos/TabelaMediaPorSegmento")]
        public async Task<IActionResult> TabelaMediaPorSegmento()
        {
            return View();
        }
    }
}
