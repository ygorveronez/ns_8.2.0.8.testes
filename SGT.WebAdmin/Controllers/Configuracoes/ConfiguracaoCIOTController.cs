using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Configuracoes
{
    [CustomAuthorize("Configuracoes/ConfiguracaoCIOT")]
    public class ConfiguracaoCIOTController : BaseController
    {
        #region Construtores

        public ConfiguracaoCIOTController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> ObterConfiguracoesAtivas()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.CIOT.ConfiguracaoCIOT repConfiguracaoCIOT = new Repositorio.Embarcador.CIOT.ConfiguracaoCIOT(unidadeDeTrabalho);

                List<Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOT> configuracoesCIOT = repConfiguracaoCIOT.BuscarAtivas();

                return new JsonpResult(configuracoesCIOT.Select(o => new { o.Codigo, o.Descricao }).ToList());
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao buscar as configurações de CIOT disponíveis.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.CIOT.ConfiguracaoCIOT repConfiguracaoCIOT = new Repositorio.Embarcador.CIOT.ConfiguracaoCIOT(unitOfWork);

                Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOT configuracaoCIOT = new Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOT();

                unitOfWork.Start();

                PreencherEntidade(configuracaoCIOT, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();

                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao adicionar dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.CIOT.ConfiguracaoCIOT repConfiguracaoCIOT = new Repositorio.Embarcador.CIOT.ConfiguracaoCIOT(unitOfWork);

                Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOT configuracaoCIOT = repConfiguracaoCIOT.BuscarPorCodigo(codigo, true);

                if (configuracaoCIOT == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();

                PreencherEntidade(configuracaoCIOT, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();

                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao atualizar dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.CIOT.ConfiguracaoCIOT repConfiguracaoCIOT = new Repositorio.Embarcador.CIOT.ConfiguracaoCIOT(unitOfWork);
                Repositorio.Embarcador.CIOT.CIOTEFrete repCIOTEFrete = new Repositorio.Embarcador.CIOT.CIOTEFrete(unitOfWork);
                Repositorio.Embarcador.CIOT.CIOTPamcard repCIOTPamcard = new Repositorio.Embarcador.CIOT.CIOTPamcard(unitOfWork);
                Repositorio.Embarcador.CIOT.CIOTRepom repCIOTRepom = new Repositorio.Embarcador.CIOT.CIOTRepom(unitOfWork);
                Repositorio.Embarcador.CIOT.CIOTPagbem repCIOTPagbem = new Repositorio.Embarcador.CIOT.CIOTPagbem(unitOfWork);
                Repositorio.Embarcador.CIOT.CIOTTarget repCIOTTarget = new Repositorio.Embarcador.CIOT.CIOTTarget(unitOfWork);
                Repositorio.Embarcador.CIOT.CIOTExtratta repCIOTExtratta = new Repositorio.Embarcador.CIOT.CIOTExtratta(unitOfWork);
                Repositorio.Embarcador.CIOT.CIOTAmbipar repCIOTAmbipar = new Repositorio.Embarcador.CIOT.CIOTAmbipar(unitOfWork);
                Repositorio.Embarcador.CIOT.CIOTConfiguracaoFinanceira repositorioConfiguracaoFinanceira = new Repositorio.Embarcador.CIOT.CIOTConfiguracaoFinanceira(unitOfWork);
                Repositorio.Embarcador.CIOT.ConfiguracaoCIOTDataFixaVencimento repositorioDataFixaVencimentoCiot = new Repositorio.Embarcador.CIOT.ConfiguracaoCIOTDataFixaVencimento(unitOfWork);
                Repositorio.Embarcador.CIOT.CIOTRodocred repCIOTRodocred = new Repositorio.Embarcador.CIOT.CIOTRodocred(unitOfWork);
                Repositorio.Embarcador.CIOT.CIOTRepomFrete repCIOTRepomFrete = new Repositorio.Embarcador.CIOT.CIOTRepomFrete(unitOfWork);
                Repositorio.Embarcador.CIOT.CIOTTruckPad repCIOTTruckPad = new Repositorio.Embarcador.CIOT.CIOTTruckPad(unitOfWork);
                Repositorio.Embarcador.Financeiro.TipoMovimento repositorioTipoMovimento = new Repositorio.Embarcador.Financeiro.TipoMovimento(unitOfWork);


                Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOT configuracaoCIOT = repConfiguracaoCIOT.BuscarPorCodigo(codigo, false);

                if (configuracaoCIOT == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Dominio.Entidades.Embarcador.CIOT.CIOTEFrete ciotEFrete = null;
                Dominio.Entidades.Embarcador.CIOT.CIOTPamcard ciotPamcard = null;
                Dominio.Entidades.Embarcador.CIOT.CIOTRepom ciotRepom = null;
                Dominio.Entidades.Embarcador.CIOT.CIOTPagbem ciotPagbem = null;
                Dominio.Entidades.Embarcador.CIOT.CIOTTarget ciotTarget = null;
                Dominio.Entidades.Embarcador.CIOT.CIOTExtratta ciotExtratta = null;
                Dominio.Entidades.Embarcador.CIOT.CIOTAmbipar ciotAmbipar = null;
                Dominio.Entidades.Embarcador.CIOT.CIOTRodocred ciotRodocred = null;
                Dominio.Entidades.Embarcador.CIOT.CIOTRepomFrete ciotRepomFrete = null;
                Dominio.Entidades.Embarcador.CIOT.CIOTTruckPad ciotTruckPad = null;
                List<Dominio.Entidades.Embarcador.CIOT.CIOTConfiguracaoFinanceira> ciotConfiguracaoFinanceira = new List<Dominio.Entidades.Embarcador.CIOT.CIOTConfiguracaoFinanceira>();
                List<Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOTDataFixaVencimento> dataFixaVencimentoCiot = new List<Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOTDataFixaVencimento>();

                if (configuracaoCIOT.ConfiguracaoMovimentoFinanceiro)
                    ciotConfiguracaoFinanceira = repositorioConfiguracaoFinanceira.BuscarPorConfiguracaoCIOT(codigo);

                if (configuracaoCIOT.HabilitarDataFixaVencimentoCIOT)
                    dataFixaVencimentoCiot = repositorioDataFixaVencimentoCiot.BuscarPorConfiguracaoCIOT(codigo);


                switch (configuracaoCIOT.OperadoraCIOT)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT.eFrete:
                        ciotEFrete = repCIOTEFrete.BuscarPorConfiguracaoCIOT(configuracaoCIOT.Codigo);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT.Repom:
                        ciotRepom = repCIOTRepom.BuscarPorConfiguracaoCIOT(configuracaoCIOT.Codigo);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT.Pamcard:
                        ciotPamcard = repCIOTPamcard.BuscarPorConfiguracaoCIOT(configuracaoCIOT.Codigo);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT.Pagbem:
                        ciotPagbem = repCIOTPagbem.BuscarPorConfiguracaoCIOT(configuracaoCIOT.Codigo);
                        break;
                    case OperadoraCIOT.Target:
                        ciotTarget = repCIOTTarget.BuscarPorConfiguracaoCIOT(configuracaoCIOT.Codigo);
                        break;
                    case OperadoraCIOT.Extratta:
                        ciotExtratta = repCIOTExtratta.BuscarPorConfiguracaoCIOT(configuracaoCIOT.Codigo);
                        break;
                    case OperadoraCIOT.Ambipar:
                        ciotAmbipar = repCIOTAmbipar.BuscarPorConfiguracaoCIOT(configuracaoCIOT.Codigo);
                        break;
                    case OperadoraCIOT.Rodocred:
                        ciotRodocred = repCIOTRodocred.BuscarPorConfiguracaoCIOT(configuracaoCIOT.Codigo);
                        break;
                    case OperadoraCIOT.RepomFrete:
                        ciotRepomFrete = repCIOTRepomFrete.BuscarPorConfiguracaoCIOT(configuracaoCIOT.Codigo);
                        break;
                    case OperadoraCIOT.TruckPad:
                        ciotTruckPad = repCIOTTruckPad.BuscarPorConfiguracaoCIOT(configuracaoCIOT.Codigo);
                        break;
                    default:
                        break;
                }

                return new JsonpResult(new
                {
                    configuracaoCIOT.Codigo,
                    configuracaoCIOT.Descricao,
                    Situacao = configuracaoCIOT.Ativo,
                    configuracaoCIOT.Observacao,
                    configuracaoCIOT.AbrirCIOTAntesEmissaoCTe,
                    configuracaoCIOT.ConsultarFaturas,
                    configuracaoCIOT.ExigeRotaCadastrada,
                    configuracaoCIOT.GerarUmCIOTPorViagem,
                    configuracaoCIOT.IntegrarMotoristaNoCadastro,
                    configuracaoCIOT.IntegrarVeiculoNoCadastro,
                    configuracaoCIOT.OperadoraCIOT,
                    configuracaoCIOT.ValorPedagioRetornadoIntegradora,
                    configuracaoCIOT.TarifaSaque,
                    configuracaoCIOT.TarifaTransferencia,
                    configuracaoCIOT.CNPJOperadora,
                    configuracaoCIOT.EncerrarCIOTManualmente,
                    configuracaoCIOT.HabilitarConciliacaoFinanceira,
                    configuracaoCIOT.GerarTitulosContratoFrete,
                    configuracaoCIOT.PermiteVariosCIOTsAbertos,
                    configuracaoCIOT.UtilizarDataAtualComoInicioTerminoCIOT,
                    configuracaoCIOT.DiasTerminoCIOT,
                    configuracaoCIOT.ConfiguracaoMovimentoFinanceiro,
                    configuracaoCIOT.HabilitarDataFixaVencimentoCIOT,
                    configuracaoCIOT.HabilitarQuitacaoAutomaticaPagamentosPendentes,
                    PermiteVariosCIOTsAbertosTipoTerceiro = configuracaoCIOT.PermiteVariosCIOTsAbertosTipoTerceiro?.ToString("D") ?? "",
                    ConfiguracaoEFrete = new
                    {
                        Codigo = ciotEFrete?.Codigo ?? 0,
                        CodigoIntegradorEFrete = ciotEFrete?.CodigoIntegradorEFrete ?? string.Empty,
                        MatrizEFrete = new
                        {
                            Codigo = ciotEFrete?.MatrizEFrete?.Codigo ?? 0,
                            Descricao = ciotEFrete?.MatrizEFrete?.Descricao ?? string.Empty
                        },
                        SenhaEFrete = ciotEFrete?.SenhaEFrete ?? string.Empty,
                        UsuarioEFrete = ciotEFrete?.UsuarioEFrete ?? string.Empty,
                        CodigoTipoCarga = ciotEFrete?.CodigoTipoCarga ?? 1,
                        EmissaoGratuita = ciotEFrete?.EmissaoGratuita ?? false,
                        TipoPagamento = ciotEFrete?.TipoPagamento ?? null,
                    },
                    ConfiguracaoPamcard = new
                    {
                        Codigo = ciotPamcard?.Codigo,
                        Matriz = new
                        {
                            Codigo = ciotPamcard?.Matriz?.Codigo ?? 0,
                            Descricao = ciotPamcard?.Matriz?.Descricao ?? string.Empty
                        },
                        AjustarSaldoVencimentoDataEncerramento = ciotPamcard?.AjustarSaldoVencimentoDataEncerramento ?? false,
                        EnviarQuantidadesMaioresQueZero = ciotPamcard?.EnviarQuantidadesMaioresQueZero ?? false,
                        AssociarCartaoMotoristaTransportador = ciotPamcard?.AssociarCartaoMotoristaTransportador ?? false,
                        UtilizarDataAtualParaDefinirVencimentoSaldo = ciotPamcard?.UtilizarDataAtualParaDefinirVencimentoSaldo ?? false,
                        UtilizarDataAtualParaDefinirVencimentoAdiantamento = ciotPamcard?.UtilizarDataAtualParaDefinirVencimentoAdiantamento ?? false
                    },
                    ConfiguracaoRepom = new
                    {
                        Codigo = ciotRepom?.Codigo ?? 0,
                        CodigoCliente = ciotRepom?.CodigoCliente ?? string.Empty,
                        AssinaturaDigital = ciotRepom?.AssinaturaDigital ?? string.Empty,
                        CNPJIntegrador = ciotRepom?.CNPJIntegrador ?? string.Empty,
                        CodigoMovimentoINSS = ciotRepom?.CodigoMovimentoINSS ?? string.Empty,
                        CodigoMovimentoIR = ciotRepom?.CodigoMovimentoIR ?? string.Empty,
                        CodigoMovimentoSENAT = ciotRepom?.CodigoMovimentoSENAT ?? string.Empty,
                        CodigoMovimentoSEST = ciotRepom?.CodigoMovimentoSEST ?? string.Empty,
                    },
                    ConfiguracaoPagbem = new
                    {
                        Codigo = ciotPagbem?.Codigo,
                        URLPagbem = ciotPagbem?.URLPagbem ?? string.Empty,
                        UsuarioPagbem = ciotPagbem?.UsuarioPagbem ?? string.Empty,
                        SenhaPagbem = ciotPagbem?.SenhaPagbem ?? string.Empty,
                        TipoFilialContratantePagbem = ciotPagbem?.TipoFilialContratante ?? TipoFilialContratantePagbem.Empresa,
                        NaoIntegrarResponsavelCartaoPagbem = ciotPagbem?.NaoIntegrarResponsavelCartaoPagbem ?? false,
                        IntegrarNumeroRPSNFSE = ciotPagbem?.IntegrarNumeroRPSNFSE ?? false,
                        LiberarViagemManualmente = ciotPagbem?.LiberarViagemManualmente ?? false,
                        CNPJEmpresaContratante = ciotPagbem?.CNPJEmpresaContratante ?? string.Empty,
                        TipoTolerancia = ciotPagbem?.TipoTolerancia ?? string.Empty,
                        FreteTipoPeso = ciotPagbem?.FreteTipoPeso ?? string.Empty,
                        QuebraTipoCobranca = ciotPagbem?.QuebraTipoCobranca ?? string.Empty,
                        QuebraTolerancia = ciotPagbem?.QuebraTolerancia ?? 0m,
                        UtilizarCnpjContratanteIntegracao = ciotPagbem?.UtilizarCnpjContratanteIntegracao ?? false
                    },
                    ConfiguracaoTarget = new
                    {
                        Codigo = ciotTarget?.Codigo,
                        URLWebService = ciotTarget?.URLWebService ?? string.Empty,
                        Usuario = ciotTarget?.Usuario ?? string.Empty,
                        Senha = ciotTarget?.Senha ?? string.Empty,
                        Token = ciotTarget?.Token ?? string.Empty,
                        AssociarCartaoMotoristaTransportador = ciotTarget?.AssociarCartaoMotoristaTransportador ?? false,
                        ConsultarCartaoMotorista = ciotTarget?.ConsultarCartaoMotorista ?? false,
                        UtilizarCiotTarget = ciotTarget?.UtilizarCiotTarget ?? false,
                    },
                    ConfiguracaoExtratta = new
                    {
                        Codigo = ciotExtratta?.Codigo ?? 0,
                        URLAPI = ciotExtratta?.URLAPI ?? string.Empty,
                        CNPJAplicacao = ciotExtratta?.CNPJAplicacao ?? string.Empty,
                        Token = ciotExtratta?.Token ?? string.Empty,
                        UtilizarCNPJAplicacaoPreenchimentoCNPJEmpresa = ciotExtratta?.UtilizarCNPJAplicacaoPreenchimentoCNPJEmpresa ?? false,
                        PrefixoCampoNumeroControle = ciotExtratta?.PrefixoCampoNumeroControle ?? string.Empty,
                        ForcarCIOTNaoEquiparado = ciotExtratta?.ForcarCIOTNaoEquiparado ?? false,
                        UtilizarTipoGeracaoCIOTPreenchimentoHabilitarContratoCiotAgregado = ciotExtratta?.UtilizarTipoGeracaoCIOTPreenchimentoHabilitarContratoCiotAgregado ?? false,
                        EnviarQuantidadesMaioresQueZeroExtratta = ciotExtratta?.EnviarQuantidadesMaioresQueZero ?? false,
                        NaoRealizarQuitacaoViagemEncerramentoCIOT = ciotExtratta?.NaoRealizarQuitacaoViagemEncerramentoCIOT ?? false,
                        NomeUsuarioExtratta = ciotExtratta?.NomeUsuario ?? string.Empty,
                        DocumentoUsuarioExtratta = ciotExtratta?.DocumentoUsuario ?? string.Empty,
                        EnviarCarretaViagemV2 = ciotExtratta?.EnviarCarretaViagemV2 ?? false,
                    },
                    ConfiguracaoFinanceira = (from Dominio.Entidades.Embarcador.CIOT.CIOTConfiguracaoFinanceira item in ciotConfiguracaoFinanceira
                                              select new
                                              {
                                                  TipoPagamento = new
                                                  {
                                                      Codigo = item.Codigo,
                                                      CodigosConfiguracao = $"{item.TipoPagamento.ObterNumeradorTipoPagamento()},{item.TipoMovimentoParaUso.Codigo},{item.TipoMovimentoParaReversao.Codigo}",
                                                      TipoPagamento = $"{item.TipoPagamento}",
                                                      TipoMovimentoParaUso = item.TipoMovimentoParaUso.Descricao,
                                                      TipoMovimentoParaReversao = item.TipoMovimentoParaReversao.Descricao

                                                  }
                                              }).ToList(),
                    ConfiguracaoDataFixaVencimentoCiot = (from Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOTDataFixaVencimento item in dataFixaVencimentoCiot
                                                          select new
                                                          {
                                                              item.Codigo,
                                                              item.DiaInicialEmissao,
                                                              item.DiaFinalEmissao,
                                                              item.DiaVencimentoCIOT,
                                                          }).ToList(),
                    ConfiguracaoAmbipar = new
                    {
                        Codigo = ciotAmbipar?.Codigo ?? 0,
                        URL = ciotAmbipar?.URL ?? string.Empty,
                        Usuario = ciotAmbipar?.Usuario ?? string.Empty,
                        Senha = ciotAmbipar?.Senha ?? string.Empty,
                    },
                    ConfiguracaoRodocred = new
                    {
                        Codigo = ciotRodocred?.Codigo ?? 0,
                        URL = ciotRodocred?.URL ?? string.Empty,
                        Login = ciotRodocred?.Login ?? string.Empty,
                        ChaveAutenticacao = ciotRodocred?.ChaveAutenticacao ?? string.Empty,
                        IDCliente = ciotRodocred?.IDCliente ?? string.Empty,
                    },
                    ConfiguracaoRepomFrete = new
                    {
                        Codigo = ciotRepomFrete?.Codigo ?? 0,
                        URLRepomFrete = ciotRepomFrete?.URLRepomFrete ?? string.Empty,
                        UsuarioRepomFrete = ciotRepomFrete?.UsuarioRepomFrete ?? string.Empty,
                        SenhaRepomFrete = ciotRepomFrete?.SenhaRepomFrete ?? string.Empty,
                        PartnerRepomFrete = ciotRepomFrete?.PartnerRepomFrete ?? string.Empty,
                        UtilizarMetodosValidacaoCadastros = ciotRepomFrete?.UtilizarMetodosValidacaoCadastros ?? false,
                        RealizarEncerramentoAutorizacaoPagamentoSeparado = ciotRepomFrete?.RealizarEncerramentoAutorizacaoPagamentoSeparado ?? false,
                        RealizarCompraValePedagioIntegracaoCIOT = ciotRepomFrete?.RealizarCompraValePedagioIntegracaoCIOT ?? false,
                        EnviarQuantidadesMaioresQueZeroRepomFrete = ciotRepomFrete?.EnviarQuantidadesMaioresQueZero ?? false,
                        UsarDataPagamentoTransportadorTerceiro = ciotRepomFrete?.UsarDataPagamentoTransportadorTerceiro ?? false,
                        UtilizarDataPrevisaoEntregaPedidoParaExpectativaPagamentoSaldo = ciotRepomFrete?.UtilizarDataPrevisaoEntregaPedidoParaExpectativaPagamentoSaldo ?? false
                    },
                    ConfiguracaoTruckPad = new
                    {
                        Codigo = ciotTruckPad?.Codigo ?? 0,
                        URLTruckPadToken = ciotTruckPad?.URLTruckPadToken ?? string.Empty,
                        URLTruckPad = ciotTruckPad?.URLTruckPad ?? string.Empty,
                        UsuarioTruckPad = ciotTruckPad?.UsuarioTruckPad ?? string.Empty,
                        SenhaTruckPad = ciotTruckPad?.SenhaTruckPad ?? string.Empty,
                        OfficeID = ciotTruckPad?.OfficeID ?? string.Empty
                    }
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.CIOT.ConfiguracaoCIOT repConfiguracaoCIOT = new Repositorio.Embarcador.CIOT.ConfiguracaoCIOT(unitOfWork);
                Repositorio.Embarcador.CIOT.CIOTEFrete repCIOTEFrete = new Repositorio.Embarcador.CIOT.CIOTEFrete(unitOfWork);
                Repositorio.Embarcador.CIOT.CIOTPamcard repCIOTPamcard = new Repositorio.Embarcador.CIOT.CIOTPamcard(unitOfWork);
                Repositorio.Embarcador.CIOT.CIOTRepom repCIOTRepom = new Repositorio.Embarcador.CIOT.CIOTRepom(unitOfWork);
                Repositorio.Embarcador.CIOT.CIOTPagbem repCIOTPagbem = new Repositorio.Embarcador.CIOT.CIOTPagbem(unitOfWork);
                Repositorio.Embarcador.CIOT.CIOTTarget repCIOTTarget = new Repositorio.Embarcador.CIOT.CIOTTarget(unitOfWork);
                Repositorio.Embarcador.CIOT.CIOTExtratta repCIOTExtratta = new Repositorio.Embarcador.CIOT.CIOTExtratta(unitOfWork);
                Repositorio.Embarcador.CIOT.CIOTAmbipar repCIOTAmbipar = new Repositorio.Embarcador.CIOT.CIOTAmbipar(unitOfWork);
                Repositorio.Embarcador.CIOT.CIOTRodocred repCIOTRodocred = new Repositorio.Embarcador.CIOT.CIOTRodocred(unitOfWork);
                Repositorio.Embarcador.CIOT.CIOTRepomFrete repCIOTRepomFrete = new Repositorio.Embarcador.CIOT.CIOTRepomFrete(unitOfWork);
                Repositorio.Embarcador.CIOT.CIOTTruckPad repCIOTTruckPad = new Repositorio.Embarcador.CIOT.CIOTTruckPad(unitOfWork);

                Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOT configuracaoCIOT = repConfiguracaoCIOT.BuscarPorCodigo(codigo, true);

                if (configuracaoCIOT == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Dominio.Entidades.Embarcador.CIOT.CIOTEFrete ciotEFrete = repCIOTEFrete.BuscarPorConfiguracaoCIOT(configuracaoCIOT.Codigo); ;
                Dominio.Entidades.Embarcador.CIOT.CIOTPamcard ciotPamcard = repCIOTPamcard.BuscarPorConfiguracaoCIOT(configuracaoCIOT.Codigo);
                Dominio.Entidades.Embarcador.CIOT.CIOTRepom ciotRepom = repCIOTRepom.BuscarPorConfiguracaoCIOT(configuracaoCIOT.Codigo);
                Dominio.Entidades.Embarcador.CIOT.CIOTPagbem ciotPagbem = repCIOTPagbem.BuscarPorConfiguracaoCIOT(configuracaoCIOT.Codigo);
                Dominio.Entidades.Embarcador.CIOT.CIOTTarget ciotTarget = repCIOTTarget.BuscarPorConfiguracaoCIOT(configuracaoCIOT.Codigo);
                Dominio.Entidades.Embarcador.CIOT.CIOTExtratta ciotExtratta = repCIOTExtratta.BuscarPorConfiguracaoCIOT(configuracaoCIOT.Codigo);
                Dominio.Entidades.Embarcador.CIOT.CIOTAmbipar ciotAmbipar = repCIOTAmbipar.BuscarPorConfiguracaoCIOT(configuracaoCIOT.Codigo);
                Dominio.Entidades.Embarcador.CIOT.CIOTRodocred ciotRodocred = repCIOTRodocred.BuscarPorConfiguracaoCIOT(configuracaoCIOT.Codigo);
                Dominio.Entidades.Embarcador.CIOT.CIOTRepomFrete ciotRepomFrete = repCIOTRepomFrete.BuscarPorConfiguracaoCIOT(configuracaoCIOT.Codigo);
                Dominio.Entidades.Embarcador.CIOT.CIOTTruckPad ciotTruckPad = repCIOTTruckPad.BuscarPorConfiguracaoCIOT(configuracaoCIOT.Codigo);

                unitOfWork.Start();

                if (ciotRepom != null)
                    repCIOTRepom.Deletar(ciotRepom);

                if (ciotEFrete != null)
                    repCIOTEFrete.Deletar(ciotEFrete);

                if (ciotPamcard != null)
                    repCIOTPamcard.Deletar(ciotPamcard);

                if (ciotPagbem != null)
                    repCIOTPagbem.Deletar(ciotPagbem);

                if (ciotTarget != null)
                    repCIOTTarget.Deletar(ciotTarget);

                if (ciotExtratta != null)
                    repCIOTExtratta.Deletar(ciotExtratta);

                if (ciotAmbipar != null)
                    repCIOTAmbipar.Deletar(ciotAmbipar);

                if (ciotRodocred != null)
                    repCIOTRodocred.Deletar(ciotRodocred);

                if (ciotRepomFrete != null)
                    repCIOTRepomFrete.Deletar(ciotRepomFrete);

                if (ciotTruckPad != null)
                    repCIOTTruckPad.Deletar(ciotTruckPad);

                repConfiguracaoCIOT.Deletar(configuracaoCIOT, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao remover dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa()
        {
            try
            {
                var grid = ObterGridPesquisa();

                byte[] arquivoBinario = grid.GerarExcel();

                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", $"{grid.tituloExportacao}.{grid.extensaoCSV}");

                return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            try
            {
                return new JsonpResult(ObterGridPesquisa());
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
        }

        [AcceptVerbs("GET", "POST")]
        public async Task<IActionResult> DownloadMovimentoFinanceiro()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                DateTime dataMovimento = Request.GetDateTimeParam("DataMovimento");

                Servicos.Embarcador.CIOT.Repom svcRepom = new Servicos.Embarcador.CIOT.Repom();

                string caminho = svcRepom.ObterCaminhoConciliacaoFinanceira(dataMovimento, unitOfWork);

                if (!svcRepom.IntegrarConciliacaoFinanceira(dataMovimento, out string mensagemErro, unitOfWork))
                    return new JsonpResult(false, true, mensagemErro);

                return Arquivo(Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminho), "text/plain", "Movimento Financeiro " + dataMovimento.ToString("dd-MM-yyyy") + ".txt");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao remover dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("GET", "POST")]
        public async Task<IActionResult> DownloadMovimentoContabil()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                DateTime dataMovimento = Request.GetDateTimeParam("DataMovimento");

                Servicos.Embarcador.CIOT.Repom svcRepom = new Servicos.Embarcador.CIOT.Repom();

                string caminho = svcRepom.ObterCaminhoConciliacaoContabil(dataMovimento, unitOfWork);

                if (!svcRepom.IntegrarConciliacaoContabil(dataMovimento, out string mensagemErro, unitOfWork))
                    return new JsonpResult(false, true, mensagemErro);

                return Arquivo(Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminho), "text/plain", "Movimento Contábil " + dataMovimento.ToString("dd -MM-yyyy") + ".txt");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao remover dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void PreencherEntidade(Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOT configuracaoCIOT, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.CIOT.ConfiguracaoCIOT repConfiguracaoCIOT = new Repositorio.Embarcador.CIOT.ConfiguracaoCIOT(unitOfWork);

            bool ativo = Request.GetBoolParam("Situacao");
            bool abrirCIOTAntesEmissaoCTe = Request.GetBoolParam("AbrirCIOTAntesEmissaoCTe");
            bool consultarFaturas = Request.GetBoolParam("ConsultarFaturas");
            bool exigeRotaCadastrada = Request.GetBoolParam("ExigeRotaCadastrada");
            bool gerarUmCIOTPorViagem = Request.GetBoolParam("GerarUmCIOTPorViagem");
            bool integrarMotoristaNoCadastro = Request.GetBoolParam("IntegrarMotoristaNoCadastro");
            bool integrarVeiculoNoCadastro = Request.GetBoolParam("IntegrarVeiculoNoCadastro");
            bool valorPedagioRetornadoIntegradora = Request.GetBoolParam("ValorPedagioRetornadoIntegradora");
            bool encerrarCIOTManualmente = Request.GetBoolParam("EncerrarCIOTManualmente");
            bool configuracaoMovimentoFinanceiro = Request.GetBoolParam("ConfiguracaoMovimentoFinanceiro");
            bool habilitarDataFixaVencimentoCIOT = Request.GetBoolParam("habilitarDataFixaVencimentoCIOT");

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT operadoraCIOT = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT>("OperadoraCIOT");

            string descricao = Request.Params("Descricao");
            string observacao = Request.Params("Observacao");
            string cnpjOperadora = Request.GetStringParam("CNPJOperadora");

            decimal tarifaSaque = Request.GetDecimalParam("TarifaSaque");
            decimal tarifaTransferencia = Request.GetDecimalParam("TarifaTransferencia");

            configuracaoCIOT.Ativo = ativo;
            configuracaoCIOT.Descricao = descricao;
            configuracaoCIOT.Observacao = observacao;
            configuracaoCIOT.AbrirCIOTAntesEmissaoCTe = abrirCIOTAntesEmissaoCTe;
            configuracaoCIOT.ConsultarFaturas = consultarFaturas;
            configuracaoCIOT.ExigeRotaCadastrada = exigeRotaCadastrada;
            configuracaoCIOT.GerarUmCIOTPorViagem = gerarUmCIOTPorViagem;
            configuracaoCIOT.IntegrarMotoristaNoCadastro = integrarMotoristaNoCadastro;
            configuracaoCIOT.IntegrarVeiculoNoCadastro = integrarVeiculoNoCadastro;
            configuracaoCIOT.OperadoraCIOT = operadoraCIOT;
            configuracaoCIOT.ValorPedagioRetornadoIntegradora = valorPedagioRetornadoIntegradora;
            configuracaoCIOT.TarifaSaque = tarifaSaque;
            configuracaoCIOT.TarifaTransferencia = tarifaTransferencia;
            configuracaoCIOT.CNPJOperadora = cnpjOperadora;
            configuracaoCIOT.EncerrarCIOTManualmente = encerrarCIOTManualmente;
            configuracaoCIOT.HabilitarConciliacaoFinanceira = Request.GetBoolParam("HabilitarConciliacaoFinanceira");
            configuracaoCIOT.GerarTitulosContratoFrete = Request.GetBoolParam("GerarTitulosContratoFrete");
            configuracaoCIOT.UtilizarDataAtualComoInicioTerminoCIOT = Request.GetBoolParam("UtilizarDataAtualComoInicioTerminoCIOT");
            configuracaoCIOT.DiasTerminoCIOT = configuracaoCIOT.UtilizarDataAtualComoInicioTerminoCIOT ? Request.GetNullableIntParam("DiasTerminoCIOT") : null;
            configuracaoCIOT.ConfiguracaoMovimentoFinanceiro = configuracaoMovimentoFinanceiro;
            configuracaoCIOT.HabilitarDataFixaVencimentoCIOT = habilitarDataFixaVencimentoCIOT;
            configuracaoCIOT.HabilitarQuitacaoAutomaticaPagamentosPendentes = Request.GetBoolParam("HabilitarQuitacaoAutomaticaPagamentosPendentes");

            if (configuracaoCIOT.GerarUmCIOTPorViagem)
            {
                configuracaoCIOT.PermiteVariosCIOTsAbertos = Request.GetBoolParam("PermiteVariosCIOTsAbertos");
                configuracaoCIOT.PermiteVariosCIOTsAbertosTipoTerceiro = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoProprietarioVeiculo>("PermiteVariosCIOTsAbertosTipoTerceiro");
            }
            else
            {
                configuracaoCIOT.PermiteVariosCIOTsAbertos = false;
                configuracaoCIOT.PermiteVariosCIOTsAbertosTipoTerceiro = null;
            }

            Dominio.Entidades.Auditoria.HistoricoObjeto historicoObjeto = null;

            if (configuracaoCIOT.Codigo > 0)
                historicoObjeto = repConfiguracaoCIOT.Atualizar(configuracaoCIOT, Auditado);
            else
                repConfiguracaoCIOT.Inserir(configuracaoCIOT, Auditado);

            SetarConfiguracoesOperadoraCIOT(configuracaoCIOT, unitOfWork, historicoObjeto);
            SetarConfiguracaoMovimentoFinanceiro(configuracaoCIOT, unitOfWork);
            SetarConfiguracaoDataFixaVencimentoCiot(configuracaoCIOT, unitOfWork);
        }

        private void SetarConfiguracoesOperadoraCIOT(Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOT configuracaoCIOT, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Auditoria.HistoricoObjeto historicoObjeto)
        {
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Embarcador.CIOT.CIOTEFrete repCIOTEFrete = new Repositorio.Embarcador.CIOT.CIOTEFrete(unitOfWork);
            Repositorio.Embarcador.CIOT.CIOTPamcard repCIOTPamcard = new Repositorio.Embarcador.CIOT.CIOTPamcard(unitOfWork);
            Repositorio.Embarcador.CIOT.CIOTRepom repCIOTRepom = new Repositorio.Embarcador.CIOT.CIOTRepom(unitOfWork);
            Repositorio.Embarcador.CIOT.CIOTPagbem repCIOTPagbem = new Repositorio.Embarcador.CIOT.CIOTPagbem(unitOfWork);
            Repositorio.Embarcador.CIOT.CIOTTarget repCIOTTarget = new Repositorio.Embarcador.CIOT.CIOTTarget(unitOfWork);
            Repositorio.Embarcador.CIOT.CIOTExtratta repCIOTExtratta = new Repositorio.Embarcador.CIOT.CIOTExtratta(unitOfWork);
            Repositorio.Embarcador.CIOT.CIOTBBC repCIOTBBC = new Repositorio.Embarcador.CIOT.CIOTBBC(unitOfWork);
            Repositorio.Embarcador.CIOT.CIOTAmbipar repCIOTAmbipar = new Repositorio.Embarcador.CIOT.CIOTAmbipar(unitOfWork);
            Repositorio.Embarcador.CIOT.CIOTRodocred repCIOTRodocred = new Repositorio.Embarcador.CIOT.CIOTRodocred(unitOfWork);
            Repositorio.Embarcador.CIOT.CIOTRepomFrete repCIOTRepomFrete = new Repositorio.Embarcador.CIOT.CIOTRepomFrete(unitOfWork);
            Repositorio.Embarcador.CIOT.CIOTTruckPad repCIOTTruckPad = new Repositorio.Embarcador.CIOT.CIOTTruckPad(unitOfWork);


            Dominio.Entidades.Embarcador.CIOT.CIOTEFrete ciotEFrete = repCIOTEFrete.BuscarPorConfiguracaoCIOT(configuracaoCIOT.Codigo);
            Dominio.Entidades.Embarcador.CIOT.CIOTPamcard ciotPamcard = repCIOTPamcard.BuscarPorConfiguracaoCIOT(configuracaoCIOT.Codigo);
            Dominio.Entidades.Embarcador.CIOT.CIOTRepom ciotRepom = repCIOTRepom.BuscarPorConfiguracaoCIOT(configuracaoCIOT.Codigo);
            Dominio.Entidades.Embarcador.CIOT.CIOTPagbem ciotPagbem = repCIOTPagbem.BuscarPorConfiguracaoCIOT(configuracaoCIOT.Codigo);
            Dominio.Entidades.Embarcador.CIOT.CIOTTarget ciotTarget = repCIOTTarget.BuscarPorConfiguracaoCIOT(configuracaoCIOT.Codigo);
            Dominio.Entidades.Embarcador.CIOT.CIOTExtratta ciotExtratta = repCIOTExtratta.BuscarPorConfiguracaoCIOT(configuracaoCIOT.Codigo);
            Dominio.Entidades.Embarcador.CIOT.CIOTBBC ciotBBC = repCIOTBBC.BuscarPorConfiguracaoCIOT(configuracaoCIOT.Codigo);
            Dominio.Entidades.Embarcador.CIOT.CIOTAmbipar ciotAmbipar = repCIOTAmbipar.BuscarPorConfiguracaoCIOT(configuracaoCIOT.Codigo);
            Dominio.Entidades.Embarcador.CIOT.CIOTRodocred ciotRodocred = repCIOTRodocred.BuscarPorConfiguracaoCIOT(configuracaoCIOT.Codigo);
            Dominio.Entidades.Embarcador.CIOT.CIOTRepomFrete ciotRepomFrete = repCIOTRepomFrete.BuscarPorConfiguracaoCIOT(configuracaoCIOT.Codigo);
            Dominio.Entidades.Embarcador.CIOT.CIOTTruckPad ciotTruckPad = repCIOTTruckPad.BuscarPorConfiguracaoCIOT(configuracaoCIOT.Codigo);

            if (ciotEFrete != null)
                ciotEFrete.Initialize();

            if (ciotPamcard != null)
                ciotPamcard.Initialize();

            if (ciotRepom != null)
                ciotRepom.Initialize();

            if (ciotPagbem != null)
                ciotPagbem.Initialize();

            if (ciotTarget != null)
                ciotTarget.Initialize();

            if (ciotExtratta != null)
                ciotExtratta.Initialize();

            if (ciotBBC != null)
                ciotBBC.Initialize();

            if (ciotAmbipar != null)
                ciotAmbipar.Initialize();

            if (ciotRodocred != null)
                ciotRodocred.Initialize();

            if (ciotRepomFrete != null)
                ciotRepomFrete.Initialize();

            if (ciotTruckPad != null)
                ciotTruckPad.Initialize();

            RemoverConfiguracaoOutrasOperadoras(configuracaoCIOT.OperadoraCIOT, historicoObjeto, ciotEFrete, ciotPamcard, ciotRepom, ciotPagbem, ciotTarget, ciotExtratta, ciotBBC, ciotAmbipar, ciotRodocred, ciotRepomFrete, ciotTruckPad, unitOfWork);

            switch (configuracaoCIOT.OperadoraCIOT)
            {
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT.eFrete:

                    dynamic configuracaoEFrete = JsonConvert.DeserializeObject<dynamic>(Request.Params("ConfiguracaoEFrete"));

                    int codigoMatrizEFrete = (int)configuracaoEFrete.MatrizEFrete;

                    if (ciotEFrete == null)
                    {
                        ciotEFrete = new Dominio.Entidades.Embarcador.CIOT.CIOTEFrete();
                        ciotEFrete.ConfiguracaoCIOT = configuracaoCIOT;
                    }

                    ciotEFrete.CodigoIntegradorEFrete = (string)configuracaoEFrete.CodigoIntegradorEFrete;
                    ciotEFrete.SenhaEFrete = (string)configuracaoEFrete.SenhaEFrete;
                    ciotEFrete.UsuarioEFrete = (string)configuracaoEFrete.UsuarioEFrete;
                    ciotEFrete.MatrizEFrete = codigoMatrizEFrete > 0 ? repEmpresa.BuscarPorCodigo(codigoMatrizEFrete) : null;
                    ciotEFrete.CodigoTipoCarga = !string.IsNullOrWhiteSpace((string)configuracaoEFrete.CodigoTipoCarga) ? (int)configuracaoEFrete.CodigoTipoCarga : 0;
                    ciotEFrete.EmissaoGratuita = !string.IsNullOrWhiteSpace((string)configuracaoEFrete.EmissaoGratuita) ? (bool)configuracaoEFrete.EmissaoGratuita : false;
                    ciotEFrete.TipoPagamento = !string.IsNullOrWhiteSpace((string)configuracaoEFrete.TipoPagamento) ? (TipoPagamentoeFrete)configuracaoEFrete.TipoPagamento : 0;

                    if (ciotEFrete.Codigo > 0)
                        repCIOTEFrete.Atualizar(ciotEFrete, Auditado, historicoObjeto);
                    else
                        repCIOTEFrete.Inserir(ciotEFrete, Auditado, historicoObjeto);

                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT.Repom:

                    dynamic configuracaoRepom = JsonConvert.DeserializeObject<dynamic>(Request.Params("ConfiguracaoRepom"));

                    if (ciotRepom == null)
                    {
                        ciotRepom = new Dominio.Entidades.Embarcador.CIOT.CIOTRepom();
                        ciotRepom.ConfiguracaoCIOT = configuracaoCIOT;
                    }

                    ciotRepom.AssinaturaDigital = (string)configuracaoRepom.AssinaturaDigital;
                    ciotRepom.CodigoCliente = (string)configuracaoRepom.CodigoCliente;
                    ciotRepom.CNPJIntegrador = (string)configuracaoRepom.CNPJIntegrador;
                    ciotRepom.CodigoMovimentoINSS = (string)configuracaoRepom.CodigoMovimentoINSS;
                    ciotRepom.CodigoMovimentoIR = (string)configuracaoRepom.CodigoMovimentoIR;
                    ciotRepom.CodigoMovimentoSENAT = (string)configuracaoRepom.CodigoMovimentoSENAT;
                    ciotRepom.CodigoMovimentoSEST = (string)configuracaoRepom.CodigoMovimentoSEST;

                    if (ciotRepom.Codigo > 0)
                        repCIOTRepom.Atualizar(ciotRepom, Auditado, historicoObjeto);
                    else
                        repCIOTRepom.Inserir(ciotRepom, Auditado, historicoObjeto);

                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT.Pamcard:

                    dynamic configuracaoPamcard = JsonConvert.DeserializeObject<dynamic>(Request.Params("ConfiguracaoPamcard"));

                    int codigoMatrizPamcard = (int)configuracaoPamcard.Matriz;

                    if (ciotPamcard == null)
                    {
                        ciotPamcard = new Dominio.Entidades.Embarcador.CIOT.CIOTPamcard();
                        ciotPamcard.ConfiguracaoCIOT = configuracaoCIOT;
                    }

                    ciotPamcard.Matriz = codigoMatrizPamcard > 0 ? repEmpresa.BuscarPorCodigo(codigoMatrizPamcard) : null;
                    ciotPamcard.AjustarSaldoVencimentoDataEncerramento = (bool)configuracaoPamcard.AjustarSaldoVencimentoDataEncerramento;
                    ciotPamcard.EnviarQuantidadesMaioresQueZero = (bool)configuracaoPamcard.EnviarQuantidadesMaioresQueZero;
                    ciotPamcard.AssociarCartaoMotoristaTransportador = (bool)configuracaoPamcard.AssociarCartaoMotoristaTransportador;
                    ciotPamcard.UtilizarDataAtualParaDefinirVencimentoSaldo = (bool)configuracaoPamcard.UtilizarDataAtualParaDefinirVencimentoSaldo;
                    ciotPamcard.UtilizarDataAtualParaDefinirVencimentoAdiantamento = (bool)configuracaoPamcard.UtilizarDataAtualParaDefinirVencimentoAdiantamento;

                    if (ciotPamcard.Codigo > 0)
                        repCIOTPamcard.Atualizar(ciotPamcard, Auditado, historicoObjeto);
                    else
                        repCIOTPamcard.Inserir(ciotPamcard, Auditado, historicoObjeto);

                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT.Pagbem:

                    dynamic configuracaoPagbem = JsonConvert.DeserializeObject<dynamic>(Request.Params("ConfiguracaoPagbem"));

                    if (ciotPagbem == null)
                    {
                        ciotPagbem = new Dominio.Entidades.Embarcador.CIOT.CIOTPagbem();
                        ciotPagbem.ConfiguracaoCIOT = configuracaoCIOT;
                    }

                    ciotPagbem.URLPagbem = (string)configuracaoPagbem.URLPagbem;
                    ciotPagbem.UsuarioPagbem = (string)configuracaoPagbem.UsuarioPagbem;
                    ciotPagbem.SenhaPagbem = (string)configuracaoPagbem.SenhaPagbem;
                    ciotPagbem.CNPJEmpresaContratante = (string)configuracaoPagbem.CNPJEmpresaContratante;
                    ciotPagbem.TipoFilialContratante = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFilialContratantePagbem)configuracaoPagbem.TipoFilialContratantePagbem;
                    ciotPagbem.NaoIntegrarResponsavelCartaoPagbem = (bool)configuracaoPagbem.NaoIntegrarResponsavelCartaoPagbem;
                    ciotPagbem.IntegrarNumeroRPSNFSE = (bool)configuracaoPagbem.IntegrarNumeroRPSNFSE;
                    ciotPagbem.LiberarViagemManualmente = (bool)configuracaoPagbem.LiberarViagemManualmente;
                    ciotPagbem.UtilizarCnpjContratanteIntegracao = (bool)configuracaoPagbem.UtilizarCnpjContratanteIntegracao;
                    ciotPagbem.TipoTolerancia = (string)configuracaoPagbem.TipoTolerancia;
                    ciotPagbem.FreteTipoPeso = (string)configuracaoPagbem.FreteTipoPeso;
                    ciotPagbem.QuebraTipoCobranca = (string)configuracaoPagbem.QuebraTipoCobranca;
                    ciotPagbem.QuebraTolerancia = ((string)configuracaoPagbem.QuebraTolerancia).ToDecimal(0m);

                    if (ciotPagbem.Codigo > 0)
                        repCIOTPagbem.Atualizar(ciotPagbem, Auditado, historicoObjeto);
                    else
                        repCIOTPagbem.Inserir(ciotPagbem, Auditado, historicoObjeto);

                    break;

                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT.Target:

                    dynamic configuracaoTarget = JsonConvert.DeserializeObject<dynamic>(Request.Params("ConfiguracaoTarget"));

                    if (ciotTarget == null)
                    {
                        ciotTarget = new Dominio.Entidades.Embarcador.CIOT.CIOTTarget();
                        ciotTarget.ConfiguracaoCIOT = configuracaoCIOT;
                    }

                    ciotTarget.URLWebService = (string)configuracaoTarget.URLWebService;
                    ciotTarget.Usuario = (string)configuracaoTarget.Usuario;
                    ciotTarget.Senha = (string)configuracaoTarget.Senha;
                    ciotTarget.Token = (string)configuracaoTarget.Token;
                    ciotTarget.AssociarCartaoMotoristaTransportador = (bool)configuracaoTarget.AssociarCartaoMotoristaTransportador;
                    ciotTarget.ConsultarCartaoMotorista = (bool)configuracaoTarget.ConsultarCartaoMotorista;
                    ciotTarget.UtilizarCiotTarget = (bool)configuracaoTarget.UtilizarCiotTarget;

                    if (ciotTarget.Codigo > 0)
                        repCIOTTarget.Atualizar(ciotTarget, Auditado, historicoObjeto);
                    else
                        repCIOTTarget.Inserir(ciotTarget, Auditado, historicoObjeto);

                    break;

                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT.Extratta:

                    dynamic configuracaoExtratta = JsonConvert.DeserializeObject<dynamic>(Request.Params("ConfiguracaoExtratta"));

                    if (ciotExtratta == null)
                    {
                        ciotExtratta = new Dominio.Entidades.Embarcador.CIOT.CIOTExtratta();
                        ciotExtratta.ConfiguracaoCIOT = configuracaoCIOT;
                    }

                    ciotExtratta.URLAPI = (string)configuracaoExtratta.URLAPI;
                    ciotExtratta.CNPJAplicacao = (string)configuracaoExtratta.CNPJAplicacao;
                    ciotExtratta.Token = (string)configuracaoExtratta.Token;
                    ciotExtratta.UtilizarCNPJAplicacaoPreenchimentoCNPJEmpresa = (bool)configuracaoExtratta.UtilizarCNPJAplicacaoPreenchimentoCNPJEmpresa;
                    ciotExtratta.PrefixoCampoNumeroControle = (string)configuracaoExtratta.PrefixoCampoNumeroControle;
                    ciotExtratta.ForcarCIOTNaoEquiparado = (bool)configuracaoExtratta.ForcarCIOTNaoEquiparado;
                    ciotExtratta.UtilizarTipoGeracaoCIOTPreenchimentoHabilitarContratoCiotAgregado = (bool)configuracaoExtratta.UtilizarTipoGeracaoCIOTPreenchimentoHabilitarContratoCiotAgregado;
                    ciotExtratta.EnviarQuantidadesMaioresQueZero = (bool)configuracaoExtratta.EnviarQuantidadesMaioresQueZeroExtratta;
                    ciotExtratta.NaoRealizarQuitacaoViagemEncerramentoCIOT = (bool)configuracaoExtratta.NaoRealizarQuitacaoViagemEncerramentoCIOT;
                    ciotExtratta.NomeUsuario = (string)configuracaoExtratta.NomeUsuarioExtratta;
                    ciotExtratta.DocumentoUsuario = (string)configuracaoExtratta.DocumentoUsuarioExtratta;
                    ciotExtratta.EnviarCarretaViagemV2 = (bool)configuracaoExtratta.EnviarCarretaViagemV2;

                    if (ciotExtratta == null)
                        repCIOTExtratta.Atualizar(ciotExtratta, Auditado, historicoObjeto);
                    else
                        repCIOTExtratta.Inserir(ciotExtratta, Auditado, historicoObjeto);

                    break;

                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT.BBC:

                    dynamic configuracaoBBC = JsonConvert.DeserializeObject<dynamic>(Request.Params("ConfiguracaoBBC"));

                    if (ciotBBC == null)
                    {
                        ciotBBC = new Dominio.Entidades.Embarcador.CIOT.CIOTBBC();
                        ciotBBC.ConfiguracaoCIOT = configuracaoCIOT;
                    }
                    if (ciotBBC == null)
                        repCIOTBBC.Atualizar(ciotBBC, Auditado, historicoObjeto);
                    else
                        repCIOTBBC.Inserir(ciotBBC, Auditado, historicoObjeto);

                    break;

                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT.Ambipar:

                    dynamic configuracaoAmbipar = JsonConvert.DeserializeObject<dynamic>(Request.Params("ConfiguracaoAmbipar"));

                    if (ciotAmbipar == null)
                    {
                        ciotAmbipar = new Dominio.Entidades.Embarcador.CIOT.CIOTAmbipar();
                        ciotAmbipar.ConfiguracaoCIOT = configuracaoCIOT;
                    }

                    ciotAmbipar.URL = (string)configuracaoAmbipar.URL;
                    ciotAmbipar.Usuario = (string)configuracaoAmbipar.Usuario;
                    ciotAmbipar.Senha = (string)configuracaoAmbipar.Senha;

                    if (ciotAmbipar.Codigo > 0)
                        repCIOTAmbipar.Atualizar(ciotAmbipar, Auditado, historicoObjeto);
                    else
                        repCIOTAmbipar.Inserir(ciotAmbipar, Auditado, historicoObjeto);

                    break;

                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT.Rodocred:

                    dynamic configuracaoRodocred = JsonConvert.DeserializeObject<dynamic>(Request.Params("ConfiguracaoRodocred"));

                    if (ciotRodocred == null)
                    {
                        ciotRodocred = new Dominio.Entidades.Embarcador.CIOT.CIOTRodocred();
                        ciotRodocred.ConfiguracaoCIOT = configuracaoCIOT;
                    }

                    ciotRodocred.URL = (string)configuracaoRodocred.URL;
                    ciotRodocred.Login = (string)configuracaoRodocred.Login;
                    ciotRodocred.ChaveAutenticacao = (string)configuracaoRodocred.ChaveAutenticacao;
                    ciotRodocred.IDCliente = (string)configuracaoRodocred.IDCliente;

                    if (ciotRodocred.Codigo > 0)
                        repCIOTRodocred.Atualizar(ciotRodocred, Auditado, historicoObjeto);
                    else
                        repCIOTRodocred.Inserir(ciotRodocred, Auditado, historicoObjeto);

                    break;

                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT.RepomFrete:

                    dynamic configuracaoRepomFrete = JsonConvert.DeserializeObject<dynamic>(Request.Params("ConfiguracaoRepomFrete"));

                    if (ciotRepomFrete == null)
                    {
                        ciotRepomFrete = new Dominio.Entidades.Embarcador.CIOT.CIOTRepomFrete();
                        ciotRepomFrete.ConfiguracaoCIOT = configuracaoCIOT;
                    }

                    ciotRepomFrete.URLRepomFrete = (string)configuracaoRepomFrete.URLRepomFrete;
                    ciotRepomFrete.UsuarioRepomFrete = (string)configuracaoRepomFrete.UsuarioRepomFrete;
                    ciotRepomFrete.SenhaRepomFrete = (string)configuracaoRepomFrete.SenhaRepomFrete;
                    ciotRepomFrete.PartnerRepomFrete = (string)configuracaoRepomFrete.PartnerRepomFrete;
                    ciotRepomFrete.UtilizarMetodosValidacaoCadastros = (bool)configuracaoRepomFrete.UtilizarMetodosValidacaoCadastros;
                    ciotRepomFrete.RealizarEncerramentoAutorizacaoPagamentoSeparado = (bool)configuracaoRepomFrete.RealizarEncerramentoAutorizacaoPagamentoSeparado;
                    ciotRepomFrete.RealizarCompraValePedagioIntegracaoCIOT = (bool)configuracaoRepomFrete.RealizarCompraValePedagioIntegracaoCIOT;
                    ciotRepomFrete.EnviarQuantidadesMaioresQueZero = (bool)configuracaoRepomFrete.EnviarQuantidadesMaioresQueZeroRepomFrete;
                    ciotRepomFrete.UsarDataPagamentoTransportadorTerceiro = (bool)configuracaoRepomFrete.UsarDataPagamentoTransportadorTerceiro;
                    ciotRepomFrete.UtilizarDataPrevisaoEntregaPedidoParaExpectativaPagamentoSaldo = (bool)configuracaoRepomFrete.UtilizarDataPrevisaoEntregaPedidoParaExpectativaPagamentoSaldo;
                    if (ciotRepomFrete.Codigo > 0)
                        repCIOTRepomFrete.Atualizar(ciotRepomFrete, Auditado, historicoObjeto);
                    else
                        repCIOTRepomFrete.Inserir(ciotRepomFrete, Auditado, historicoObjeto);

                    break;

                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT.TruckPad:

                    dynamic configuracaoTruckPad = JsonConvert.DeserializeObject<dynamic>(Request.Params("ConfiguracaoTruckPad"));

                    if (ciotTruckPad == null)
                    {
                        ciotTruckPad = new Dominio.Entidades.Embarcador.CIOT.CIOTTruckPad();
                        ciotTruckPad.ConfiguracaoCIOT = configuracaoCIOT;
                    }

                    ciotTruckPad.URLTruckPadToken = (string)configuracaoTruckPad.URLTruckPadToken;
                    ciotTruckPad.URLTruckPad = (string)configuracaoTruckPad.URLTruckPad;
                    ciotTruckPad.UsuarioTruckPad = (string)configuracaoTruckPad.UsuarioTruckPad;
                    ciotTruckPad.SenhaTruckPad = (string)configuracaoTruckPad.SenhaTruckPad;
                    ciotTruckPad.OfficeID = (string)configuracaoTruckPad.OfficeID;

                    if (ciotTruckPad.Codigo > 0)
                        repCIOTTruckPad.Atualizar(ciotTruckPad, Auditado, historicoObjeto);
                    else
                        repCIOTTruckPad.Inserir(ciotTruckPad, Auditado, historicoObjeto);

                    break;
            }
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string descricao = Request.Params("Descricao");
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status = Request.GetEnumParam("Situacao", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("TipoOperadoraCIOT", false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Descricao, "Descricao", 45, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Pedidos.TipoOperacao.Operadora, "OperadoraCIOT", 15, Models.Grid.Align.left, true);

                if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho("Status", "DescricaoAtivo", 15, Models.Grid.Align.left, true);

                string propriedadeOrdenar = ObterPropriedadeOrdenar(grid.header[grid.indiceColunaOrdena].data);

                Repositorio.Embarcador.CIOT.ConfiguracaoCIOT repConfiguracaoCIOT = new Repositorio.Embarcador.CIOT.ConfiguracaoCIOT(unitOfWork);

                List<Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOT> listaConfiguracoesCIOT = repConfiguracaoCIOT.Consultar(descricao, status, propriedadeOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                int totalRegistros = repConfiguracaoCIOT.ContarConsulta(descricao, status);

                var retorno = (from config in listaConfiguracoesCIOT
                               select new
                               {
                                   config.Codigo,
                                   config.Descricao,
                                   config.DescricaoAtivo,
                                   TipoOperadoraCIOT = config.OperadoraCIOT,
                                   OperadoraCIOT = config.OperadoraCIOT.ObterDescricao()
                               }).ToList();

                grid.AdicionaRows(retorno);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "DescricaoAtivo")
                return "Ativo";

            return propriedadeOrdenar;
        }

        private void RemoverConfiguracaoOutrasOperadoras(Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT? operadoraSelecionada, Dominio.Entidades.Auditoria.HistoricoObjeto historicoObjeto, Dominio.Entidades.Embarcador.CIOT.CIOTEFrete ciotEFrete, Dominio.Entidades.Embarcador.CIOT.CIOTPamcard ciotPamcard, Dominio.Entidades.Embarcador.CIOT.CIOTRepom ciotRepom, Dominio.Entidades.Embarcador.CIOT.CIOTPagbem ciotPagbem, Dominio.Entidades.Embarcador.CIOT.CIOTTarget ciotTarget, Dominio.Entidades.Embarcador.CIOT.CIOTExtratta ciotExtratta, Dominio.Entidades.Embarcador.CIOT.CIOTBBC ciotBBC, Dominio.Entidades.Embarcador.CIOT.CIOTAmbipar ciotAmbipar, Dominio.Entidades.Embarcador.CIOT.CIOTRodocred ciotRodocred, Dominio.Entidades.Embarcador.CIOT.CIOTRepomFrete ciotRepomFrete, Dominio.Entidades.Embarcador.CIOT.CIOTTruckPad ciotTruckPad, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.CIOT.CIOTEFrete repCIOTEFrete = new Repositorio.Embarcador.CIOT.CIOTEFrete(unitOfWork);
            Repositorio.Embarcador.CIOT.CIOTPamcard repCIOTPamcard = new Repositorio.Embarcador.CIOT.CIOTPamcard(unitOfWork);
            Repositorio.Embarcador.CIOT.CIOTRepom repCIOTRepom = new Repositorio.Embarcador.CIOT.CIOTRepom(unitOfWork);
            Repositorio.Embarcador.CIOT.CIOTPagbem repCIOTPagbem = new Repositorio.Embarcador.CIOT.CIOTPagbem(unitOfWork);
            Repositorio.Embarcador.CIOT.CIOTTarget repCIOTTarget = new Repositorio.Embarcador.CIOT.CIOTTarget(unitOfWork);
            Repositorio.Embarcador.CIOT.CIOTExtratta repCIOTExtratta = new Repositorio.Embarcador.CIOT.CIOTExtratta(unitOfWork);
            Repositorio.Embarcador.CIOT.CIOTBBC repCIOTBBC = new Repositorio.Embarcador.CIOT.CIOTBBC(unitOfWork);
            Repositorio.Embarcador.CIOT.CIOTAmbipar repCIOTAmbipar = new Repositorio.Embarcador.CIOT.CIOTAmbipar(unitOfWork);
            Repositorio.Embarcador.CIOT.CIOTRodocred repCIOTRodocred = new Repositorio.Embarcador.CIOT.CIOTRodocred(unitOfWork);
            Repositorio.Embarcador.CIOT.CIOTRepomFrete repCIOTRepomFrete = new Repositorio.Embarcador.CIOT.CIOTRepomFrete(unitOfWork);
            Repositorio.Embarcador.CIOT.CIOTTruckPad repCIOTTruckPad = new Repositorio.Embarcador.CIOT.CIOTTruckPad(unitOfWork);

            if (operadoraSelecionada != OperadoraCIOT.eFrete && ciotEFrete != null)
                repCIOTEFrete.Deletar(ciotEFrete, Auditado, historicoObjeto);

            if (operadoraSelecionada != OperadoraCIOT.Pamcard && ciotPamcard != null)
                repCIOTPamcard.Deletar(ciotPamcard, Auditado, historicoObjeto);

            if (operadoraSelecionada != OperadoraCIOT.Repom && ciotRepom != null)
                repCIOTRepom.Deletar(ciotRepom, Auditado, historicoObjeto);

            if (operadoraSelecionada != OperadoraCIOT.Pagbem && ciotPagbem != null)
                repCIOTPagbem.Deletar(ciotPagbem, Auditado, historicoObjeto);

            if (operadoraSelecionada != OperadoraCIOT.Target && ciotTarget != null)
                repCIOTTarget.Deletar(ciotTarget, Auditado, historicoObjeto);

            if (operadoraSelecionada != OperadoraCIOT.Extratta && ciotExtratta != null)
                repCIOTExtratta.Deletar(ciotExtratta, Auditado, historicoObjeto);

            if (operadoraSelecionada != OperadoraCIOT.BBC && ciotBBC != null)
                repCIOTBBC.Deletar(ciotBBC, Auditado, historicoObjeto);

            if (operadoraSelecionada != OperadoraCIOT.Ambipar && ciotAmbipar != null)
                repCIOTAmbipar.Deletar(ciotAmbipar, Auditado, historicoObjeto);

            if (operadoraSelecionada != OperadoraCIOT.Rodocred && ciotRodocred != null)
                repCIOTRodocred.Deletar(ciotRodocred, Auditado, historicoObjeto);

            if (operadoraSelecionada != OperadoraCIOT.RepomFrete && ciotRepomFrete != null)
                repCIOTRepomFrete.Deletar(ciotRepomFrete, Auditado, historicoObjeto);

            if (operadoraSelecionada != OperadoraCIOT.TruckPad && ciotTruckPad != null)
                repCIOTTruckPad.Deletar(ciotTruckPad, Auditado, historicoObjeto);
        }

        private void SetarConfiguracaoMovimentoFinanceiro(Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOT configuracaoCIOT, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.CIOT.CIOTConfiguracaoFinanceira repositorioCIOTConfiguracaoFinanceira = new Repositorio.Embarcador.CIOT.CIOTConfiguracaoFinanceira(unitOfWork);
            List<Dominio.Entidades.Embarcador.CIOT.CIOTConfiguracaoFinanceira> existeConfiguracaoFinanceira = repositorioCIOTConfiguracaoFinanceira.BuscarPorConfiguracaoCIOT(configuracaoCIOT.Codigo);
            Repositorio.Embarcador.Financeiro.TipoMovimento repositorioTipoMovimento = new Repositorio.Embarcador.Financeiro.TipoMovimento(unitOfWork);

            dynamic configuracaoFinanceira = JsonConvert.DeserializeObject<dynamic>(Request.Params("ConfiguracaoFinanceira"));

            if (existeConfiguracaoFinanceira.Count > 0 && existeConfiguracaoFinanceira != null)
            {
                List<int> codigos = new List<int>();

                foreach (var tipoPagamento in configuracaoFinanceira)
                    codigos.Add((int)tipoPagamento.TipoPagamento.Codigo);

                List<Dominio.Entidades.Embarcador.CIOT.CIOTConfiguracaoFinanceira> listaRemoverConfiguracoes = existeConfiguracaoFinanceira.Where(c => !codigos.Contains(c.Codigo)).ToList();

                for (var i = 0; i < listaRemoverConfiguracoes.Count; i++)
                    repositorioCIOTConfiguracaoFinanceira.Deletar(listaRemoverConfiguracoes[i]);

            }

            foreach (var tipoPagamento in configuracaoFinanceira)
            {
                if (existeConfiguracaoFinanceira.Any(c => c.Codigo == (int)tipoPagamento.TipoPagamento.Codigo))
                    continue;


                var CodigosGrid = ((string)tipoPagamento.TipoPagamento.CodigosConfiguracao).ToString().Split(',').ToList();
                Dominio.Entidades.Embarcador.CIOT.CIOTConfiguracaoFinanceira novaConfiguracaoFinanceira = new Dominio.Entidades.Embarcador.CIOT.CIOTConfiguracaoFinanceira();
                novaConfiguracaoFinanceira.ConfiguracaoCIOT = configuracaoCIOT;
                novaConfiguracaoFinanceira.TipoPagamento = TipoPagamentoCIOTHelper.ObterTipoPagamento(CodigosGrid[0].ToInt());
                novaConfiguracaoFinanceira.TipoMovimentoParaUso = repositorioTipoMovimento.BuscarPorCodigo(CodigosGrid[1].ToInt());
                novaConfiguracaoFinanceira.TipoMovimentoParaReversao = repositorioTipoMovimento.BuscarPorCodigo(CodigosGrid[2].ToInt());
                repositorioCIOTConfiguracaoFinanceira.Inserir(novaConfiguracaoFinanceira);
            }

        }

        private void SetarConfiguracaoDataFixaVencimentoCiot(Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOT configuracaoCIOT, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.CIOT.ConfiguracaoCIOTDataFixaVencimento repositorioConfiguracaoCIOTDataFixaVencimento = new Repositorio.Embarcador.CIOT.ConfiguracaoCIOTDataFixaVencimento(unitOfWork);
            List<Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOTDataFixaVencimento> existeConfiguracaoCIOTDataFixaVencimento = repositorioConfiguracaoCIOTDataFixaVencimento.BuscarPorConfiguracaoCIOT(configuracaoCIOT.Codigo);

            dynamic configuracaoCIOTDataFixaVencimento = JsonConvert.DeserializeObject<dynamic>(Request.Params("ConfiguracaoDataFixaVencimentoCiot"));

            if (existeConfiguracaoCIOTDataFixaVencimento.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (var item in configuracaoCIOTDataFixaVencimento)
                {

                    int? codigo = ((string)item.Codigo).ToNullableInt();
                    if (codigo.HasValue)
                        codigos.Add((int)item.Codigo);
                }

                List<Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOTDataFixaVencimento> listaRemoverConfiguracoes = existeConfiguracaoCIOTDataFixaVencimento.Where(c => !codigos.Contains(c.Codigo)).ToList();

                for (var i = 0; i < listaRemoverConfiguracoes.Count; i++)
                    repositorioConfiguracaoCIOTDataFixaVencimento.Deletar(listaRemoverConfiguracoes[i]);

                foreach (var item in configuracaoCIOTDataFixaVencimento)
                {
                    if (((string)item.Codigo).ToNullableInt().HasValue && existeConfiguracaoCIOTDataFixaVencimento.Any(c => c.Codigo == (int)item.Codigo))
                    {
                        Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOTDataFixaVencimento dataFixaVencimento = repositorioConfiguracaoCIOTDataFixaVencimento.BuscarPorCodigo((int)item.Codigo) ?? throw new ControllerException("Não Foi Possivel Atualizar os Registros!");
                        dataFixaVencimento.DiaInicialEmissao = item.DiaInicialEmissao;
                        dataFixaVencimento.DiaFinalEmissao = item.DiaFinalEmissao;
                        dataFixaVencimento.DiaVencimentoCIOT = item.DiaVencimentoCIOT;
                        dataFixaVencimento.ConfiguracaoCIOT = configuracaoCIOT;
                        repositorioConfiguracaoCIOTDataFixaVencimento.Atualizar(dataFixaVencimento);
                    }
                    else
                    {
                        InserirDataFixaVencimentoCiot(item, repositorioConfiguracaoCIOTDataFixaVencimento, configuracaoCIOT);
                    }
                }
            }
            else
            {
                foreach (var item in configuracaoCIOTDataFixaVencimento)
                {
                    InserirDataFixaVencimentoCiot(item, repositorioConfiguracaoCIOTDataFixaVencimento, configuracaoCIOT);
                }
            }
        }

        private void InserirDataFixaVencimentoCiot(dynamic item, Repositorio.Embarcador.CIOT.ConfiguracaoCIOTDataFixaVencimento repositorioConfiguracaoCIOTDataFixaVencimento, Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOT configuracaoCIOT)
        {
            Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOTDataFixaVencimento dataFixaVencimento = new Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOTDataFixaVencimento()
            {
                DiaInicialEmissao = item.DiaInicialEmissao,
                DiaFinalEmissao = item.DiaFinalEmissao,
                DiaVencimentoCIOT = item.DiaVencimentoCIOT,
                ConfiguracaoCIOT = configuracaoCIOT
            };
            ValidaDataFixaVencimentoCIOT(dataFixaVencimento);
            repositorioConfiguracaoCIOTDataFixaVencimento.Inserir(dataFixaVencimento);
        }


        private void ValidaDataFixaVencimentoCIOT(Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOTDataFixaVencimento item)
        {
            if (!(0 < item.DiaInicialEmissao && item.DiaInicialEmissao < 32) || !(0 < item.DiaFinalEmissao && item.DiaFinalEmissao < 32) || !(0 < item.DiaVencimentoCIOT && item.DiaVencimentoCIOT < 32))
                throw new ControllerException("Dias Fora do período");
        }
        #endregion
    }
}
