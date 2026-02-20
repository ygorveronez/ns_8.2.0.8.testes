using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Pessoas
{
    public class PessoasController : BaseController
    {
		#region Construtores

		public PessoasController(Conexao conexao) : base(conexao) { }

		#endregion

        [CustomAuthorize("Pessoas/GrupoPessoas")]
        public async Task<IActionResult> GrupoPessoas()
        {
            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Pessoas/GrupoPessoas");
            ViewBag.PermissoesPersonalizadas = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadas);
            return View();
        }

        [CustomAuthorize("Pessoas/GrupoPessoas")]
        public async Task<IActionResult> GrupoPessoasModeloVeicularEmbarcador()
        {
            return View();
        }

        [CustomAuthorize("Pessoas/Banco")]
        public async Task<IActionResult> Banco()
        {
            return View();
        }

        [CustomAuthorize("Pessoas/Pessoa")]
        public async Task<IActionResult> Pessoa()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {

                Repositorio.Embarcador.Configuracoes.IntegracaoSIC repositorioIntegracaoSIC = new Repositorio.Embarcador.Configuracoes.IntegracaoSIC(unitOfWork);
                Repositorio.Embarcador.Configuracoes.IntegracaoKMM repositorioIntegracaoKMM = new Repositorio.Embarcador.Configuracoes.IntegracaoKMM(unitOfWork);
                Repositorio.Embarcador.Configuracoes.IntegracaoGlobus repositorioIntegracaoGlobus = new Repositorio.Embarcador.Configuracoes.IntegracaoGlobus(unitOfWork);
                Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntregacao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService repositorioConfiguracaoWebService = new Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService(unitOfWork);
                Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unitOfWork);

                Repositorio.Embarcador.Configuracoes.ConfiguracaoContratoFreteTerceiro repConfiguracaoContratoFreteTerceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoContratoFreteTerceiro(unitOfWork);
                Repositorio.Embarcador.CIOT.ConfiguracaoGeralCIOT repConfiguracaoGeralCIOT = new Repositorio.Embarcador.CIOT.ConfiguracaoGeralCIOT(unitOfWork);
                Repositorio.Embarcador.CIOT.ConfiguracaoGeralTipoPagamentoCIOT repConfiguracaoGeralTipoPagamentoCIOT = new Repositorio.Embarcador.CIOT.ConfiguracaoGeralTipoPagamentoCIOT(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configuracaoWebService = repositorioConfiguracaoWebService.BuscarPrimeiroRegistro();

                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSIC configuracaoIntegracaoSIC = repositorioIntegracaoSIC.BuscarPrimeiroRegistro();
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoKMM configuracaoIntegracaoKMM = repositorioIntegracaoKMM.BuscarPrimeiroRegistro();
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGlobus configuracaoIntegracaoGlobus = repositorioIntegracaoGlobus.BuscarPrimeiroRegistro();

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoContratoFreteTerceiro configuracaoContratoFreteTerceiro  = repConfiguracaoContratoFreteTerceiro.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.CIOT.ConfiguracaoGeralCIOT configuracaoGeralCIOT = repConfiguracaoGeralCIOT.BuscarConfiguracaoPadrao();
                List<Dominio.Entidades.Embarcador.CIOT.ConfiguracaoGeralTipoPagamentoCIOT> configuracaoGeralTipoPagamentoCIOTs = repConfiguracaoGeralTipoPagamentoCIOT.BuscarConfiguracaoPadrao().ToList();

                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Pessoas/Pessoa");
                ViewBag.PermissoesPersonalizadas = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadas);
                ViewBag.ConfiguracoesPessoa = Newtonsoft.Json.JsonConvert.SerializeObject(new
                {
                    ConfiguracaoEmbarcador.UtilizarComponentesCliente,
                    utilizaIntegracaoPessoas = (configuracaoIntegracaoSIC?.RealizarIntegracaoNovosCadastrosPessoaSIC ?? false) || (configuracaoIntegracaoKMM?.PossuiIntegracao ?? false) || (configuracaoIntegracaoGlobus?.PossuiIntegracao ?? false),
                    PossuiIntegracaoFilaH = repTipoIntregacao.ExistePorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FilaH),
                    TornarCampoINSSeReterImpostoTrazerComoSim = (configuracaoWebService?.TornarCampoINSSeReterImpostoTrazerComoSim ?? false),
                    HabilitarFluxoPedidoEcommerce = (configuracaoWebService?.HabilitarFluxoPedidoEcommerce ?? false),
                    PossuiAreaRedex = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador && repositorioCliente.PossuiClientesRedex(),
                    configuracaoContratoFreteTerceiro.GerarCIOTMarcadoAoCadastrarTransportadorTerceiro,
                    configuracaoGeralCIOT,
                    configuracaoGeralTipoPagamentoCIOTs = new
                    {
                        configuracaoGeralTipoPagamentoCIOTs = (from obj in configuracaoGeralTipoPagamentoCIOTs
                                                       select new
                                                       {
                                                           obj.Codigo,
                                                           obj.TipoPagamentoCIOT,
                                                           OperadoraCIOT = obj.Operadora,
                                                           DescricaoTipoPagamentoCIOT = obj.TipoPagamentoCIOT.ObterDescricao(),
                                                           DescricaoOperadoraCIOT = obj.Operadora.ObterDescricao(),
                                                       }).ToList(),
                    },
                });


            }

            return View();
        }

        [CustomAuthorize("Pessoas/Usuario")]
        public async Task<IActionResult> Usuario()
        {

            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Repositorio.Embarcador.Configuracoes.IntegracaoIntercab repositorioConfiguracaoIntegracaoIntercab = new Repositorio.Embarcador.Configuracoes.IntegracaoIntercab(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral repConfiguracaoGeral = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeral configuracaoGeral = repConfiguracaoGeral.BuscarConfiguracaoPadrao();
                var configuracaoIntegracaoIntercab = repositorioConfiguracaoIntegracaoIntercab.BuscarIntegracao();

                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Pessoas/Usuario");
                ViewBag.PermissoesPersonalizadas = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadas);
                ViewBag.ExigeContraSenha = (IsHomologacao ? Cliente.ClienteConfiguracaoHomologacao : Cliente.ClienteConfiguracao)?.ExigeContraSenha ?? false;
                ViewBag.VisualizarDashRegiao = Newtonsoft.Json.JsonConvert.SerializeObject(configuracaoIntegracaoIntercab?.AtivarControleDashRegiaoOperador ?? false);
                ViewBag.HabilitarFuncionalidadesProjetoGollum = (configuracaoGeral?.HabilitarFuncionalidadesProjetoGollum ?? false) ? "true" : "false";
            }

            return View();
        }

        [CustomAuthorize("Pessoas/DescargaPessoa")]
        public async Task<IActionResult> DescargaPessoa()
        {
            return View();
        }

        [CustomAuthorize("Pessoas/PerfilAcesso")]
        public async Task<IActionResult> PerfilAcesso()
        {
            return View();
        }

        [CustomAuthorize("Pessoas/PoliticaSenha")]
        public async Task<IActionResult> PoliticaSenha()
        {
            return View();
        }

        [CustomAuthorize("Pessoas/RestricaoEntrega")]
        public async Task<IActionResult> RestricaoEntrega()
        {
            return View();
        }

        [CustomAuthorize("Pessoas/Representante")]
        public async Task<IActionResult> Representante()
        {
            return View();
        }

        [CustomAuthorize("Pessoas/SetorFuncionario")]
        public async Task<IActionResult> SetorFuncionario()
        {
            return View();
        }

        [CustomAuthorize("Pessoas/CategoriaPessoa")]
        public async Task<IActionResult> CategoriaPessoa()
        {
            return View();
        }

        [AllowAnonymous]
        public async Task<IActionResult> AlterarSenha()
        {
            return View();
        }

        [CustomAuthorize("Pessoas/FuncionarioMeta")]
        public async Task<IActionResult> FuncionarioMeta()
        {
            return View();
        }

        [CustomAuthorize("Pessoas/RegraFuncionarioComissao")]
        public async Task<IActionResult> RegraFuncionarioComissao()
        {
            return View();
        }

        [CustomAuthorize("Pessoas/AutorizacaoFuncionarioComissao")]
        public async Task<IActionResult> AutorizacaoFuncionarioComissao()
        {
            return View();
        }

        [CustomAuthorize("Pessoas/FuncionarioComissao")]
        public async Task<IActionResult> FuncionarioComissao()
        {
            return View();
        }

        [CustomAuthorize("Pessoas/ColaboradorSituacao")]
        public async Task<IActionResult> ColaboradorSituacao()
        {
            return View();
        }

        [CustomAuthorize("Pessoas/ColaboradorSituacaoLancamento")]
        public async Task<IActionResult> ColaboradorSituacaoLancamento()
        {
            return View();
        }

        [CustomAuthorize("Pessoas/PerfilAcessoMobile")]
        public async Task<IActionResult> PerfilAcessoMobile()
        {
            return View();
        }

        [CustomAuthorize("Pessoas/ContatoGrupoPessoa")]
        public async Task<IActionResult> ContatoGrupoPessoa()
        {
            return View();
        }

        [CustomAuthorize("Pessoas/PessoaClassificacao")]
        public async Task<IActionResult> PessoaClassificacao()
        {
            return View();
        }

        [CustomAuthorize("Pessoas/PessoaCampoObrigatorio")]
        public async Task<IActionResult> PessoaCampoObrigatorio()
        {
            return View();
        }

        [CustomAuthorize("Pessoas/ConfiguracaoBloqueioFinanceiro")]
        public async Task<IActionResult> ConfiguracaoBloqueioFinanceiro()
        {
            return View();
        }

        [CustomAuthorize("Pessoas/AlteracaoFormaPagamento")]
        public async Task<IActionResult> AlteracaoFormaPagamento()
        {
            return View();
        }

        [CustomAuthorize("Pessoas/Cargo")]
        public async Task<IActionResult> Cargo()
        {
            return View();
        }

        [CustomAuthorize("Pessoas/MarcaEPI")]
        public async Task<IActionResult> MarcaEPI()
        {
            return View();
        }

        [CustomAuthorize("Pessoas/TipoEPI")]
        public async Task<IActionResult> TipoEPI()
        {
            return View();
        }

        [CustomAuthorize("Pessoas/EPI")]
        public async Task<IActionResult> EPI()
        {
            return View();
        }

        [CustomAuthorize("Pessoas/TipoTerceiro")]
        public async Task<IActionResult> TipoTerceiro()
        {
            return View();
        }


        [CustomAuthorize("Pessoas/ClienteBuscaAutomatica")]
        public async Task<IActionResult> ClienteBuscaAutomatica()
        {
            return View();
        }
    }
}
