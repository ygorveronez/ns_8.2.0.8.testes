using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Transportadores
{
    public class TransportadoresController : BaseController
    {
		#region Construtores

		public TransportadoresController(Conexao conexao) : base(conexao) { }

		#endregion

        [CustomAuthorize("Transportadores/Transportador")]
        public async Task<IActionResult> Transportador()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                List<TipoIntegracao> tipos = new List<TipoIntegracao>() { TipoIntegracao.OpenTech };
                Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoTransportador repConfiguracaoTransportador = new Repositorio.Embarcador.Configuracoes.ConfiguracaoTransportador(unitOfWork);
                
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTransportador configuracaoTransportador = repConfiguracaoTransportador.BuscarConfiguracaoPadrao();

                bool possuiIntegracaoGerenciadoraRisco = repositorioTipoIntegracao.ExistePorTipo(tipos);

                var configuracoesTransportador = new
                {
                    configuracaoEmbarcador.ExigirEmailPrincipalCadastroTransportador,
                    SistemaEstrangeiro = configuracaoEmbarcador.Pais != TipoPais.Brasil,
                    configuracaoTransportador.PermitirCadastrarTransportadorInformacoesMinimas,
                    configuracaoTransportador.PermitirInformarEmpresaFavorecidaNosDadosBancarios,
                    configuracaoTransportador.ExisteTransportadorPadraoContratacao,
                    TransportadoraPadraoContratacao = new
                    {
                        Codigo = configuracaoTransportador.TransportadorPadraoContratacao?.Codigo ?? 0,
                        Descricao = configuracaoTransportador.TransportadorPadraoContratacao?.Descricao ?? string.Empty
                    },

                    PossuiIntegracaoCorreios = repositorioTipoIntegracao.ExistePorTipo(TipoIntegracao.Correios),
                    PossuiIntegracaoUnilever = repositorioTipoIntegracao.ExistePorTipo(TipoIntegracao.Unilever)
            };

                ViewBag.ConfiguracoesTransportador = Newtonsoft.Json.JsonConvert.SerializeObject(configuracoesTransportador);
                ViewBag.PossuiIntegracaoGerenciadoraRisco = Newtonsoft.Json.JsonConvert.SerializeObject(possuiIntegracaoGerenciadoraRisco);
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Transportadores/Transportador");
                ViewBag.PermissoesPersonalizadas = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadas);
            }

            return View();
        }

        [CustomAuthorize("Transportadores/Motorista")]
        public async Task<IActionResult> Motorista()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoMotorista repConfiguracaoMotorista = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMotorista(unitOfWork);
            
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMotorista configuracaoMotorista = repConfiguracaoMotorista.BuscarConfiguracaoPadrao();
            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Transportadores/Motorista");
            ViewBag.PermissoesPersonalizadas = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadas);
            ViewBag.ExigeContraSenha = (IsHomologacao ? Cliente.ClienteConfiguracaoHomologacao : Cliente.ClienteConfiguracao)?.ExigeContraSenha ?? false;
            ViewBag.PermiteInformarAjudanteNaCarga = repTipoOperacao.PossuiMotoristaAjudante();
            ViewBag.PermiteDuplicarCadastroMotorista = (configuracaoMotorista?.PermiteDuplicarCadastroMotorista ?? false);

            return View();
        }

        [CustomAuthorize("Transportadores/GerenciarTransportadores")]
        public async Task<IActionResult> GerenciarTransportadores()
        {
            return View();
        }

        [CustomAuthorize("Transportadores/UsuarioTransportador")]
        public async Task<IActionResult> Usuario()
        {
            return View();
        }

        [CustomAuthorize("Transportadores/ImportacaoMotorista")]
        public async Task<IActionResult> ImportacaoMotorista()
        {
            return View();
        }

        [CustomAuthorize("Transportadores/ImportacaoVeiculo")]
        public async Task<IActionResult> ImportacaoVeiculo()
        {
            return View();
        }

        [CustomAuthorize("Transportadores/PerfilAcessoTransportador")]
        public async Task<IActionResult> PerfilAcessoTransportador()
        {
            return View();
        }

        [CustomAuthorize("Transportadores/AlteracaoMassa")]
        public async Task<IActionResult> AlteracaoMassa()
        {
            return View();
        }

        [CustomAuthorize("Transportadores/ContaTransportador")]
        public async Task<IActionResult> ContaTransportador()
        {
            return View();
        }

        [CustomAuthorize("Transportadores/MotoristaCampoObrigatorio")]
        public async Task<IActionResult> MotoristaCampoObrigatorio()
        {
            return View();
        }

        [CustomAuthorize("Transportadores/GrupoTransportador")]
        public async Task<IActionResult> GrupoTransportador()
        {
            return View();
        }

        [CustomAuthorize("Transportadores/TransportadorIntegracao")]
        public async Task<IActionResult> TransportadorIntegracao()
        {
            return View();
        }

        [CustomAuthorize("Transportadores/DadosTransportador")]
        public async Task<IActionResult> DadosTransportador()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoTransportador repConfiguracaoTransportador = new Repositorio.Embarcador.Configuracoes.ConfiguracaoTransportador(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTransportador configuracaoTransportador = repConfiguracaoTransportador.BuscarConfiguracaoPadrao();

                var configuracoesTransportador = new
                {
                    configuracaoEmbarcador.ExigirEmailPrincipalCadastroTransportador,
                    SistemaEstrangeiro = configuracaoEmbarcador.Pais != TipoPais.Brasil,
                    configuracaoTransportador.PermitirCadastrarTransportadorInformacoesMinimas,
                    configuracaoTransportador.PermitirInformarEmpresaFavorecidaNosDadosBancarios,
                    configuracaoTransportador.ExisteTransportadorPadraoContratacao,
                    TransportadoraPadraoContratacao = new
                    {
                        Codigo = configuracaoTransportador.TransportadorPadraoContratacao?.Codigo ?? 0,
                        Descricao = configuracaoTransportador.TransportadorPadraoContratacao?.Descricao ?? string.Empty
                    },

                    PossuiIntegracaoCorreios = repositorioTipoIntegracao.ExistePorTipo(TipoIntegracao.Correios),
                    PossuiIntegracaoUnilever = repositorioTipoIntegracao.ExistePorTipo(TipoIntegracao.Unilever)
                };

                ViewBag.ConfiguracoesTransportador = Newtonsoft.Json.JsonConvert.SerializeObject(configuracoesTransportador);
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Transportadores/Transportador");
                ViewBag.PermissoesPersonalizadas = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadas);
            }

            return View();
        }

        [CustomAuthorize("Transportadores/TransportadorCertificado")]
        public async Task<IActionResult> TransportadorCertificado()
        {
            return View();
        }

        [CustomAuthorize("Transportadores/GestaoTransportador")]
        public async Task<IActionResult> GestaoTransportador()
        {
            return View();
        }


        [CustomAuthorize("Transportadores/SolicitacaoToken")]
        public async Task<IActionResult> SolicitacaoToken()
        {
            return View();
        }

        [CustomAuthorize("Transportadores/RegrasAutorizacaoToken")]
        public async Task<IActionResult> RegrasAutorizacaoToken()
        {
            return View();
        }

        [CustomAuthorize("Transportadores/AutorizacaoToken")]
        public async Task<IActionResult> AutorizacaoToken()
        {
            return View();
        }
    }
}
