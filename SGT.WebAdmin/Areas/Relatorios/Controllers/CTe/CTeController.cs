using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;


namespace SGT.WebAdmin.Areas.Relatorios.Controllers.CTe
{
    [Area("Relatorios")]
    public class CTeController : BaseController
    {
        #region Construtores

        public CTeController(Conexao conexao) : base(conexao) { }

        #endregion

        [CustomAuthorize("Relatorios/CTe/CTeEmitido")]
        public async Task<IActionResult> CTeEmitido()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/CTe/CTes")]
        public async Task<IActionResult> CTes()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/CTe/FaturamentoPorGrupoPessoas")]
        public async Task<IActionResult> FaturamentoPorGrupoPessoas()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/CTe/ComparativoMensalFaturamentoGrupoPessoas")]
        public async Task<IActionResult> ComparativoMensalFaturamentoGrupoPessoas()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/CTe/MagazineLuiza")]
        public async Task<IActionResult> MagazineLuiza()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/CTe/FalhaNumeracaoCTe")]
        public async Task<IActionResult> FalhaNumeracaoCTe()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/CTe/PosicaoCTe")]
        public async Task<IActionResult> PosicaoCTe()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/CTe/CTesSubcontratados")]
        public async Task<IActionResult> CTesSubcontratados()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/CTe/Tomador")]
        public async Task<IActionResult> Tomador()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/CTe/CargaCTeIntegracao")]
        public async Task<IActionResult> CargaCTeIntegracao()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/CTe/ComponenteFreteCTe")]
        public async Task<IActionResult> ComponenteFreteCTe()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/CTe/AFRMMControl")]
        public async Task<IActionResult> AFRMMControl()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/CTe/Container")]
        public async Task<IActionResult> Container()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/CTe/ApuracaoICMS")]
        public async Task<IActionResult> ApuracaoICMS()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/CTe/Subcontratacao")]
        public async Task<IActionResult> Subcontratacao()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/CTe/FaturamentoPorCTe")]
        public async Task<IActionResult> FaturamentoPorCTe()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/CTe/NFeCTeContainer")]
        public async Task<IActionResult> NFeCTeContainer()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/CTe/AFRMMControlMercante")]
        public async Task<IActionResult> AFRMMControlMercante()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/CTe/ValePedagio")]
        public async Task<IActionResult> ValePedagio(CancellationToken cancellationToken)
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoRelatorio repConfiguracaoRelatorio = new Repositorio.Embarcador.Configuracoes.ConfiguracaoRelatorio(unitOfWork, cancellationToken);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoRelatorio configuracaoRelatorio = await repConfiguracaoRelatorio.BuscarConfiguracaoPadraoAsync();

                ViewBag.ConfiguracaoRelatorio = Newtonsoft.Json.JsonConvert.SerializeObject(new
                {
                    ExibirTodasCargasNoRelatorioDeValePedagio = configuracaoRelatorio?.ExibirTodasCargasNoRelatorioDeValePedagio ?? false,
                });
            }
            return View();
        }

        [CustomAuthorize("Relatorios/CTe/TakeOrPay")]
        public async Task<IActionResult> TakeOrPay()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/CTe/AuditoriaCTe")]
        public async Task<IActionResult> AuditoriaCTe()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/CTe/ComissaoVendedorCTe")]
        public async Task<IActionResult> ComissaoVendedorCTe()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/CTe/CustoRentabilidadeCteCrt")]
        public async Task<IActionResult> CustoRentabilidadeCteCrt()
        {
            return View();
        }
    }
}
