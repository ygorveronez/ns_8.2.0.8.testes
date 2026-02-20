using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace SGT.WebAdmin.Controllers.Ocorrencias
{
    public class OcorrenciasController : BaseController
    {
        #region Construtores

        public OcorrenciasController(Conexao conexao) : base(conexao) { }

        #endregion

        [CustomAuthorize("Ocorrencias/Ocorrencia")]
        public async Task<IActionResult> Ocorrencia(string TokenAcesso)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Ocorrencias/Ocorrencia");

            Repositorio.Embarcador.Configuracoes.ConfiguracaoOcorrencia repConfiguracaoOcorrencia = new Repositorio.Embarcador.Configuracoes.ConfiguracaoOcorrencia(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoOcorrencia configuracaoOcorrencia = repConfiguracaoOcorrencia.BuscarConfiguracaoPadrao();

            ViewBag.PermissoesPersonalizadasOcorrencia = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadas);
            ViewBag.PermiteBaixarAnexoOcorrenciaEmLote = ConfiguracaoEmbarcador.MaxDownloadsPorVez > 0 ? "true" : "false";
            ViewBag.TokenAcesso = !string.IsNullOrWhiteSpace(TokenAcesso) ? TokenAcesso : "-1";
            ViewBag.InduzirTransportadorSelecionarApenasUmComplementoSolicitacaoComplementos = Newtonsoft.Json.JsonConvert.SerializeObject(configuracaoOcorrencia.InduzirTransportadorSelecionarApenasUmComplementoSolicitacaoComplementos);
            ViewBag.PermitirIncluirOcorrenciaPorSelecaoNotasFiscaisCTe = Newtonsoft.Json.JsonConvert.SerializeObject(configuracaoOcorrencia.PermitirIncluirOcorrenciaPorSelecaoNotasFiscaisCTe);

            var configuracoesOcorrencia = new
            {
                configuracaoOcorrencia.TrazerCentroResultadoOcorrencia
            };

            ViewBag.ConfiguracoesOcorrencia = Newtonsoft.Json.JsonConvert.SerializeObject(configuracoesOcorrencia);

            return View();
        }


        [CustomAuthorize("Ocorrencias/TipoOcorrencia")]
        public async Task<IActionResult> TipoOcorrencia()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);


            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoTrizy repositorioIntegracaoTrizy = new Repositorio.Embarcador.Configuracoes.IntegracaoTrizy(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTrizy configuracaoIntegracaoTrizy = repositorioIntegracaoTrizy.BuscarPrimeiroRegistro();
            ViewBag.ConfiguracoesTipoOcorrencia = Newtonsoft.Json.JsonConvert.SerializeObject(new
            {
                PossuiIntegracaoDiageo = repTipoIntegracao.ExistePorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Diageo),
                VersaoSuperAppV3 = configuracaoIntegracaoTrizy?.VersaoIntegracao == VersaoIntegracaoTrizy.Versao3
            });

            return View();
        }

        [CustomAuthorize("Ocorrencias/GrupoTipoDeOcorrencia")]
        public async Task<IActionResult> GrupoTipoDeOcorrencia()
        {
            return View();
        }

        [CustomAuthorize("Ocorrencias/ParametroOcorrencia")]
        public async Task<IActionResult> ParametroOcorrencia()
        {
            return View();
        }

        [CustomAuthorize("Ocorrencias/GestaoOcorrencia")]
        public async Task<IActionResult> GestaoOcorrencia()
        {
            return View();
        }

        [CustomAuthorize("Ocorrencias/RegrasAutorizacaoOcorrencia")]
        public async Task<IActionResult> RegrasAutorizacaoOcorrencia()
        {
            return View();
        }

        [CustomAuthorize("Ocorrencias/AutorizacaoOcorrencia")]
        public async Task<IActionResult> AutorizacaoOcorrencia()
        {
            return View();
        }

        [CustomAuthorize("Ocorrencias/MotivoRejeicaoOcorrencia")]
        public async Task<IActionResult> MotivoRejeicaoOcorrencia()
        {
            return View();
        }

        [CustomAuthorize("Ocorrencias/OcorrenciaCancelamento")]
        public async Task<IActionResult> OcorrenciaCancelamento()
        {
            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Ocorrencias/OcorrenciaCancelamento");
            ViewBag.PermissoesPersonalizadasOcorrenciaCancelamento = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadas);
            return View();
        }

        [CustomAuthorize("Ocorrencias/ValorParametroOcorrencia")]
        public async Task<IActionResult> ValorParametroOcorrencia()
        {
            return View();
        }

        [CustomAuthorize("Ocorrencias/OcorrenciaParametroOcorrencia")]
        public async Task<IActionResult> OcorrenciaParametroOcorrencia()
        {
            return View();
        }

        [CustomAuthorize("Ocorrencias/AceiteDebito")]
        public async Task<IActionResult> AceiteDebito()
        {
            return View();
        }

        [CustomAuthorize("Ocorrencias/LogLeituraArquivoOcorrencia")]
        public async Task<IActionResult> LogLeituraArquivoOcorrencia()
        {
            return View();
        }

        [CustomAuthorize("Ocorrencias/RegraParcelamentoOcorrencia")]
        public async Task<IActionResult> RegraParcelamentoOcorrencia()
        {
            return View();
        }

        [CustomAuthorize("Ocorrencias/ImportarOcorrencia")]
        public async Task<IActionResult> ImportarOcorrencia()
        {
            return View();
        }

        [CustomAuthorize("Ocorrencias/OcorrenciaLote")]
        public async Task<IActionResult> OcorrenciaLote()
        {
            return View();
        }

        [CustomAuthorize("Ocorrencias/JustificativaOcorrencia")]
        public async Task<IActionResult> JustificativaOcorrencia()
        {
            return View();
        }

        [CustomAuthorize("Ocorrencias/TiposCausadoresOcorrencia")]
        public async Task<IActionResult> TiposCausadoresOcorrencia()
        {
            return View();
        }

    }
}
