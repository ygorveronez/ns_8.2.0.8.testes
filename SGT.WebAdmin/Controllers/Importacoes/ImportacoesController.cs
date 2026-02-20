using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Importacoes
{
    public class ImportacoesController : BaseController
    {
        #region Construtores

        public ImportacoesController(Conexao conexao) : base(conexao) { }

        #endregion

        [CustomAuthorize("Importacoes/ImportacaoTabelaFrete")]
        public async Task<IActionResult> ImportacaoTabelaFrete()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            Repositorio.Embarcador.Cargas.TipoIntegracao reTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = await reTipoIntegracao.BuscarPorTipoAsync(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.LBC);

            var configuracoesTabelaFrete = new
            {
                PossuiIntegracaoLBC = tipoIntegracao?.Ativo ?? false,
                UtilizarLayoutImportacaoGPA = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().UtilizarLayoutImportacaoTabelaFreteGPA ?? false,
                UtilizarMetodoImportacaoPorServico = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().UtilizarMetodoImportacaoTabelaFretePorServico ?? false
            };

            ViewBag.ConfiguracoesImportacaoTabelaFrete = Newtonsoft.Json.JsonConvert.SerializeObject(configuracoesTabelaFrete);

            return View();
        }

        [CustomAuthorize("Importacoes/ImportacaoRPS")]
        public async Task<IActionResult> ImportacaoRPS()
        {
            return View();
        }

        [CustomAuthorize("Importacoes/ImportacaoHierarquia")]
        public async Task<IActionResult> ImportacaoHierarquia()
        {
            return View();
        }

        [CustomAuthorize("Importacoes/ImportacaoArquivoTabelaFrete")]
        public async Task<IActionResult> ImportacaoArquivoTabelaFrete()
        {
            return View();
        }
    }
}
