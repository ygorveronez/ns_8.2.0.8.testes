using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Financeiros
{
    public class FinanceirosController : BaseController
    {
		#region Construtores

		public FinanceirosController(Conexao conexao) : base(conexao) { }

		#endregion

        [CustomAuthorize("Financeiros/BaixaTituloReceber")]
        public async Task<IActionResult> BaixaTituloReceber()
        {
            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Financeiros/BaixaTituloReceber");
            ViewBag.PermissoesPersonalizadas = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadas);
            return View();
        }

        [CustomAuthorize("Financeiros/BaixaTituloReceberNovo")]
        public async Task<IActionResult> BaixaTituloReceberNovo()
        {
            //List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Financeiros/BaixaTituloReceberNovo");
            //ViewBag.PermissoesPersonalizadas = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadas);
            return View();
        }

        [CustomAuthorize("Financeiros/SituacaoLancamentoDocumentoEntrada")]
        public async Task<IActionResult> SituacaoLancamentoDocumentoEntrada()
        {
            //List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Financeiros/BaixaTituloReceberNovo");
            //ViewBag.PermissoesPersonalizadas = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadas);
            return View();
        }

        [CustomAuthorize("Financeiros/BaixaTituloPagar")]
        public async Task<IActionResult> BaixaTituloPagar()
        {
            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Financeiros/BaixaTituloPagar");
            ViewBag.PermissoesPersonalizadas = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadas);
            return View();
        }

        [CustomAuthorize("Financeiros/PlanoConta")]
        public async Task<IActionResult> PlanoConta()
        {
            return View();
        }

        [CustomAuthorize("Financeiros/Mensalidade")]
        public ActionResult Mensalidade()
        {
            return View();
        }

        [CustomAuthorize("Financeiros/CentroResultado")]
        public async Task<IActionResult> CentroResultado()
        {
            return View();
        }

        [CustomAuthorize("Financeiros/TipoMovimento")]
        public async Task<IActionResult> TipoMovimento()
        {
            return View();
        }

        [CustomAuthorize("Financeiros/MovimentoFinanceiro")]
        public async Task<IActionResult> MovimentoFinanceiro()
        {
            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Financeiros/MovimentoFinanceiro");
            ViewBag.PermissoesPersonalizadas = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadas);

            return View();
        }

        [CustomAuthorize("Financeiros/DocumentoEntrada")]
        public async Task<IActionResult> DocumentoEntrada()
        {
            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Financeiros/DocumentoEntrada");
            ViewBag.PermissoesPersonalizadas = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadas);

            return View();
        }

        [CustomAuthorize("Financeiros/GeracaoMovimentoLote")]
        public async Task<IActionResult> GeracaoMovimentoLote()
        {
            return View();
        }

        [CustomAuthorize("Financeiros/TituloFinanceiro")]
        public async Task<IActionResult> TituloFinanceiro()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro repConfiguracaoFinanceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro = repConfiguracaoFinanceiro.BuscarConfiguracaoPadrao();

                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Financeiros/TituloFinanceiro");

                var configuracaoTituloFinanceiro = new
                {
                    MovimentacaoFinanceiraParaTitulosDeProvisao = configuracaoFinanceiro?.MovimentacaoFinanceiraParaTitulosDeProvisao ?? false
                };

                ViewBag.PermissoesPersonalizadas = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadas);
                ViewBag.ConfiguracaoTituloFinanceiro = Newtonsoft.Json.JsonConvert.SerializeObject(configuracaoTituloFinanceiro);

                return View();
            }
        }

        [CustomAuthorize("Financeiros/CTeTituloReceber")]
        public async Task<IActionResult> CTeTituloReceber()
        {
            return View();
        }

        [CustomAuthorize("Financeiros/BoletoConfiguracao")]
        public async Task<IActionResult> BoletoConfiguracao()
        {
            return View();
        }

        [CustomAuthorize("Financeiros/BoletoGeracao")]
        public async Task<IActionResult> BoletoGeracao()
        {
            return View();
        }

        [CustomAuthorize("Financeiros/BoletoRemessa")]
        public async Task<IActionResult> BoletoRemessa()
        {
            return View();
        }

        [CustomAuthorize("Financeiros/BoletoImportarRetorno")]
        public async Task<IActionResult> BoletoImportarRetorno()
        {
            return View();
        }

        [CustomAuthorize("Financeiros/TipoPagamentoRecebimento")]
        public async Task<IActionResult> TipoPagamentoRecebimento()
        {
            return View();
        }

        [CustomAuthorize("Financeiros/FechamentoDiario")]
        public async Task<IActionResult> FechamentoDiario()
        {
            return View();
        }

        [CustomAuthorize("Financeiros/BancoTMS")]
        public async Task<IActionResult> BancoTMS()
        {
            return View();
        }

        [CustomAuthorize("Financeiros/DocumentoFaturamento")]
        public async Task<IActionResult> DocumentoFaturamento()
        {
            return View();
        }

        [CustomAuthorize("Financeiros/LancamentoConta")]
        public async Task<IActionResult> LancamentoConta()
        {
            return View();
        }

        [CustomAuthorize("Financeiros/Bordero")]
        public async Task<IActionResult> Bordero()
        {
            return View();
        }

        [CustomAuthorize("Financeiros/GeracaoTituloManual")]
        public async Task<IActionResult> GeracaoTituloManual()
        {
            return View();
        }

        [CustomAuthorize("Financeiros/CobrancaSimples")]
        public async Task<IActionResult> CobrancaSimples()
        {
            return View();
        }

        [CustomAuthorize("Financeiros/RegraEntradaDocumento")]
        public async Task<IActionResult> RegraEntradaDocumento()
        {
            return View();
        }

        [CustomAuthorize("Financeiros/BoletoAlteracao")]
        public async Task<IActionResult> BoletoAlteracao()
        {
            return View();
        }

        [CustomAuthorize("Financeiros/PlanoOrcamentario")]
        public async Task<IActionResult> PlanoOrcamentario()
        {
            return View();
        }

        [CustomAuthorize("Financeiros/PagamentoDigital")]
        public async Task<IActionResult> PagamentoDigital()
        {
            return View();
        }

        [CustomAuthorize("Financeiros/RemessaPagamento")]
        public async Task<IActionResult> RemessaPagamento()
        {
            return View();
        }

        [CustomAuthorize("Financeiros/RetornoPagamento")]
        public async Task<IActionResult> RetornoPagamento()
        {
            return View();
        }

        [CustomAuthorize("Financeiros/BoletoRetornoComando")]
        public async Task<IActionResult> BoletoRetornoComando()
        {
            return View();
        }

        [CustomAuthorize("Financeiros/GeracaoMovimentoLoteCentroResultado")]
        public async Task<IActionResult> GeracaoMovimentoLoteCentroResultado()
        {
            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Financeiros/MovimentoFinanceiro");
            ViewBag.PermissoesPersonalizadas = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadas);

            return View();
        }

        [CustomAuthorize("Financeiros/Cheque")]
        public async Task<IActionResult> Cheque()
        {
            return View();
        }

        [CustomAuthorize("Financeiros/ChequeAnexo")]
        public async Task<IActionResult> ChequeAnexo()
        {
            return View();
        }

        [CustomAuthorize("Financeiros/PagamentoEletronicoComandoRetorno")]
        public async Task<IActionResult> PagamentoEletronicoComandoRetorno()
        {
            return View();
        }

        [CustomAuthorize("Financeiros/TipoDespesaFinanceira")]
        public async Task<IActionResult> TipoDespesaFinanceira()
        {
            return View();
        }

        [CustomAuthorize("Financeiros/GrupoDespesaFinanceira")]
        public async Task<IActionResult> GrupoDespesaFinanceira()
        {
            return View();
        }

        [CustomAuthorize("Financeiros/RateioDespesaVeiculo")]
        public async Task<IActionResult> RateioDespesaVeiculo()
        {
            return View();
        }

        [CustomAuthorize("Financeiros/ExtratoBancario")]
        public async Task<IActionResult> ExtratoBancario()
        {
            return View();
        }

        [CustomAuthorize("Financeiros/ContratoFinanciamento")]
        public async Task<IActionResult> ContratoFinanciamento()
        {
            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Financeiros/MovimentoFinanceiro");
            ViewBag.PermissoesPersonalizadas = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadas);

            return View();
        }

        [CustomAuthorize("Financeiros/CaixaFuncionario")]
        public async Task<IActionResult> CaixaFuncionario()
        {
            return View();
        }

        [CustomAuthorize("Financeiros/DespesaMensal")]
        public async Task<IActionResult> DespesaMensal()
        {
            return View();
        }

        [CustomAuthorize("Financeiros/DespesaMensalProcessamento")]
        public async Task<IActionResult> DespesaMensalProcessamento()
        {
            return View();
        }

        [CustomAuthorize("Financeiros/TributoTipoDocumento")]
        public async Task<IActionResult> TributoTipoDocumento()
        {
            return View();
        }

        [CustomAuthorize("Financeiros/TributoCodigoReceita")]
        public async Task<IActionResult> TributoCodigoReceita()
        {
            return View();
        }

        [CustomAuthorize("Financeiros/TributoTipoImposto")]
        public async Task<IActionResult> TributoTipoImposto()
        {
            return View();
        }

        [CustomAuthorize("Financeiros/TributoVariacaoImposto")]
        public async Task<IActionResult> TributoVariacaoImposto()
        {
            return View();
        }

        [CustomAuthorize("Financeiros/ExtratoBancarioTipoLancamento")]
        public async Task<IActionResult> ExtratoBancarioTipoLancamento()
        {
            return View();
        }

        [CustomAuthorize("Financeiros/ConciliacaoBancaria")]
        public async Task<IActionResult> ConciliacaoBancaria()
        {
            return View();
        }

        [CustomAuthorize("Financeiros/AutorizacaoPagamentoTitulo")]
        public async Task<IActionResult> AutorizacaoPagamentoTitulo()
        {
            return View();
        }

        [CustomAuthorize("Financeiros/LoteContabilizacao")]
        public async Task<IActionResult> LoteContabilizacao()
        {
            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Financeiros/LoteContabilizacao");
            ViewBag.PermissoesPersonalizadas = Newtonsoft.Json.JsonConvert.SerializeObject(permissoesPersonalizadas);

            return View();
        }

        [CustomAuthorize("Financeiros/RegraAutorizacaoPagamentoEletronico")]
        public async Task<IActionResult> RegraAutorizacaoPagamentoEletronico()
        {
            return View();
        }

        [CustomAuthorize("Financeiros/AutorizacaoPagamentoEletronico")]
        public async Task<IActionResult> AutorizacaoPagamentoEletronico()
        {
            return View();
        }

        [CustomAuthorize("Financeiros/ConciliacaoTransportador")]
        public async Task<IActionResult> ConciliacaoTransportador()
        {
            return View();
        }

        [CustomAuthorize("Financeiros/DocumentosConciliacao")]
        public async Task<IActionResult> DocumentosConciliacao()
        {
            return View();
        }

        [CustomAuthorize("Financeiros/ModalidadeContratoFinanciamento")]
        public async Task<IActionResult> ModalidadeContratoFinanciamento()
        {
            return View();
        }

        [CustomAuthorize("Financeiros/AplicarAcrescimoDescontoNoTitulo")]
        public async Task<IActionResult> AplicarAcrescimoDescontoNoTitulo()
        {
            return View();
        }
        [CustomAuthorize("Financeiros/JustificativaCancelamentoFinanceiro")]
        public async Task<IActionResult> JustificativaCancelamentoFinanceiro()
        {
            return View();
        }

        [CustomAuthorize("Financeiros/ValeAvulso")]
        public async Task<IActionResult> ValeAvulso()
        {
            return View();
        }

        [CustomAuthorize("Financeiros/FichaCliente")]
        public async Task<IActionResult> FichaCliente()
        {
            return View();
        } 
        
        [CustomAuthorize("Financeiros/AcompanhamentoConta")]
        public async Task<IActionResult> AcompanhamentoConta()
        {
            return View();
        }    
        
        [CustomAuthorize("Financeiros/MovimentacaoContaPagar")]
        public async Task<IActionResult> MovimentacaoContaPagar()
        {
            return View();
        }

        [CustomAuthorize("Financeiros/MovimentacaoContasPagarTransportador")]
        public async Task<IActionResult> MovimentacaoContasPagarTransportador()
        {
            return View();
        }

        [CustomAuthorize("Financeiros/TermoQuitacao")]
        public async Task<IActionResult> TermoQuitacao()
        {
            return View();
        }

        [CustomAuthorize("Financeiros/TermoQuitacaoDocumento")]
        public async Task<IActionResult> TermoQuitacaoDocumento()
        {
            return View();
        }

        [AllowAuthenticate]
        [CustomAuthorize("Financeiros/TermoQuitacaoDocumento")]
        public async Task<IActionResult> RenderizarPDF()
        {
            ViewBag.Codigo = Request.GetIntParam("Codigo");

            return View();
        }

        [CustomAuthorize("Financeiros/AvisoPeriodico")]
        public async Task<IActionResult> AvisoPeriodico()
        {
            return View();
        }

        [CustomAuthorize("Financeiros/LiberacaoPagamentoProvedor")]
        public async Task<IActionResult> LiberacaoPagamentoProvedor()
        {
            return View();
        }

        [CustomAuthorize("Financeiros/FormasTitulo")]
        public async Task<IActionResult> FormasTitulo()
        {
            return View();
        }

        [CustomAuthorize("Financeiros/ContaBancaria")]
        public async Task<IActionResult> ContaBancaria()
        {
            return View();
        }
    }
}
