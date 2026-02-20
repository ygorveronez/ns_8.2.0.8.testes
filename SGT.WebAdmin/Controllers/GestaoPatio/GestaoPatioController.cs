using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.GestaoPatio
{
    public class GestaoPatioController : BaseController
    {
        #region Construtores

        public GestaoPatioController(Conexao conexao) : base(conexao) { }

        #endregion

        [CustomAuthorize("GestaoPatio/CheckListOpcoes")]
        public async Task<IActionResult> CheckListOpcoes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            List<Dominio.Entidades.Embarcador.GestaoPatio.ChecklistOpcoesRelacaoCampo> relacoesCampo = new Repositorio.Embarcador.GestaoPatio.CheckListOpcoes(unitOfWork).BuscarRelacoesCampo();
            dynamic relacoesCampoRetornar = (from o in relacoesCampo select new { o.Codigo, Descricao = o.CheckListOpcaoRelacaoCampo.ObterDescricao() });
            ViewBag.OpcoesRelacaoCampo = Newtonsoft.Json.JsonConvert.SerializeObject(relacoesCampoRetornar);


            Servicos.Embarcador.Integracao.BalancaKIKI.IntegracaoBalancaKIKI servicoIntegracaoBalancaKIKI = new Servicos.Embarcador.Integracao.BalancaKIKI.IntegracaoBalancaKIKI(unitOfWork);
            dynamic OpcoesTagIntegracao = (from o in servicoIntegracaoBalancaKIKI.GetTagsDeIntegracaoCheckListPerguntas(relacoesCampo) select new { Codigo = o, Descricao = o });
            ViewBag.OpcoesTagIntegracao = Newtonsoft.Json.JsonConvert.SerializeObject(OpcoesTagIntegracao);
            return View();
        }

        [CustomAuthorize("GestaoPatio/CheckListObservacao")]
        public async Task<IActionResult> CheckListObservacao()
        {
            return View();
        }

        [CustomAuthorize("GestaoPatio/CheckList")]
        public async Task<IActionResult> CheckList()
        {
            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadasFluxoPatio = ObterPermissoesPersonalizadas("GestaoPatio/FluxoPatio");
            ViewBag.PermissoesPersonalizadasFluxoPatio = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadasFluxoPatio);

            return View();
        }

        [CustomAuthorize("GestaoPatio/DocaCarregamento")]
        public async Task<IActionResult> DocaCarregamento()
        {
            return View();
        }

        [CustomAuthorize("GestaoPatio/TravamentoChave")]
        public async Task<IActionResult> TravamentoChave()
        {
            return View();
        }

        [CustomAuthorize("GestaoPatio/AnexosProdutor")]
        public async Task<IActionResult> AnexosProdutor()
        {
            return View();
        }

        [CustomAuthorize("GestaoPatio/FluxoPatio")]
        public async Task<IActionResult> FluxoPatio()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            Repositorio.Embarcador.Filiais.ConfiguracaoGestaoPatio repConfiguracaoGestaoPatio = new Repositorio.Embarcador.Filiais.ConfiguracaoGestaoPatio(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS config = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
            ViewBag.FluxoDePatioComoMonitoramento = config.FluxoDePatioComoMonitoramento;
            ViewBag.InformarDadosChegadaVeiculoNoFluxoPatio = config.InformarDadosChegadaVeiculoNoFluxoPatio ? "true" : "false";
            bool fluxoPatioTabelado = repConfiguracaoGestaoPatio.BuscarConfiguracao()?.ViewFluxoPatioTabelado ?? false;

            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga");
            ViewBag.PermissoesPersonalizadasCarga = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadas);

            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadasFluxoPatio = ObterPermissoesPersonalizadas("GestaoPatio/FluxoPatio");
            ViewBag.PermissoesPersonalizadasFluxoPatio = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadasFluxoPatio);

            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadasOcorrencia = ObterPermissoesPersonalizadas("Ocorrencias/Ocorrencia");

            ViewBag.PermissoesPersonalizadasOcorrencia = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadasOcorrencia);

            bool permissaoDelegar = false;

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                permissaoDelegar = UsuarioPossuiPermissao("Chamados/RegrasAnaliseChamados");

            ViewBag.PermissaoDelegar = permissaoDelegar ? "true" : "false";

            string view = "~/Views/GestaoPatio/FluxoPatio.cshtml";
            if (fluxoPatioTabelado)
                view = "~/Views/GestaoPatio/FluxoPatioTable.cshtml";

            return View(view);
        }

        [CustomAuthorize("GestaoPatio/GuaritaCheckList")]
        public async Task<IActionResult> GuaritaCheckList()
        {
            return View();
        }

        [CustomAuthorize("GestaoPatio/DisponibilidadeVeiculo")]
        public async Task<IActionResult> DisponibilidadeVeiculo()
        {
            return View();
        }

        [CustomAuthorize("GestaoPatio/CheckListTipo")]
        public async Task<IActionResult> CheckListTipo()
        {
            return View();
        }

        [CustomAuthorize("GestaoPatio/ControleVisita")]
        public async Task<IActionResult> ControleVisita()
        {
            return View();
        }

        [CustomAuthorize("GestaoPatio/OcorrenciaPatio")]
        public async Task<IActionResult> OcorrenciaPatio()
        {
            return View();
        }

        [CustomAuthorize("GestaoPatio/OcorrenciaPatioTipo")]
        public async Task<IActionResult> OcorrenciaPatioTipo()
        {
            return View();
        }

        [CustomAuthorize("GestaoPatio/SolicitacaoVeiculo")]
        public async Task<IActionResult> SolicitacaoVeiculo()
        {
            return View();
        }

        [CustomAuthorize("GestaoPatio/AdicionarCargaFluxoPatio")]
        public async Task<IActionResult> AdicionarCargaFluxoPatio()
        {
            return View();
        }

        [CustomAuthorize("GestaoPatio/MontagemCargaPatio")]
        public async Task<IActionResult> MontagemCargaPatio()
        {
            return View();
        }

        [CustomAuthorize("GestaoPatio/SeparacaoMercadoria")]
        public async Task<IActionResult> SeparacaoMercadoria()
        {
            return View();
        }

        [CustomAuthorize("GestaoPatio/RegrasMultaAtrasoRetirada")]
        public async Task<IActionResult> RegrasMultaAtrasoRetirada()
        {
            return View();
        }

        [CustomAuthorize("GestaoPatio/AvaliacaoDescarga")]
        public async Task<IActionResult> AvaliacaoDescarga()
        {
            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadasFluxoPatio = ObterPermissoesPersonalizadas("GestaoPatio/FluxoPatio");
            ViewBag.PermissoesPersonalizadasFluxoPatio = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadasFluxoPatio);

            return View();
        }

        [CustomAuthorize("GestaoPatio/FluxoPatioIntegracao")]
        public async Task<IActionResult> FluxoPatioIntegracao()
        {
            return View();
        }

        [CustomAuthorize("GestaoPatio/RegrasAutorizacaoPesagem")]
        public async Task<IActionResult> RegrasAutorizacaoPesagem()
        {
            return View();
        }

        [CustomAuthorize("GestaoPatio/ConfiguracaoToleranciaPesagem")]
        public async Task<IActionResult> ConfiguracaoToleranciaPesagem()
        {
            return View();
        }

        [CustomAuthorize("GestaoPatio/AutorizacaoPesagem")]
        public async Task<IActionResult> AutorizacaoPesagem()
        {
            return View();
        }

        [CustomAuthorize("GestaoPatio/CheckListVigencia")]
        public async Task<IActionResult> CheckListVigencia()
        {
            return View();
        }
    }
}
