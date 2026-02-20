using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Localization.Resources.Consultas;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Pedidos
{
    [CustomAuthorize(new string[] { "BuscarIntegracoes", "VerificarSeExiste", "ObterConfiguracoesIntegracoes" }, "Pedidos/TipoOperacao")]
    public class TipoOperacaoController : BaseController
    {
        #region Construtores

        public TipoOperacaoController(Conexao conexao) : base(conexao) { }

        #endregion

        //LER AS INFORMA��ES ABAIXO:

        //FAVOR ADICIONAREM AS NOVAS CONFIGURA��ES EM TABELAS RELACIONADAS, J� TEM V�RIAS CRIADAS POR ABA, SEGUIR EXEMPLO CASO AINDA N�O TENHA
        //N�O DEVE MAIS ADICIONAR NA TABELA PRINCIPAL T_TIPO_OPERACAO

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            try
            {
                return new JsonpResult(ObterGridPesquisa());
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
        }


        public async Task<IActionResult> Adicionar(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unidadeDeTrabalho);

                unidadeDeTrabalho.Start();

                Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = new Dominio.Entidades.Embarcador.Pedidos.TipoOperacao();

                if (Request.GetBoolParam("UsarComoPadraoQuandoCargaForDevolucaoDoTipoColeta"))
                {
                    Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacaoConfiguracaoDevolucaoCompra = repTipoOperacao.BuscarTipoOperacaoPadraoGeracaoCargaDevolucao();

                    if (tipoOperacaoConfiguracaoDevolucaoCompra != null)

                        throw new ControllerException(string.Format(Localization.Resources.Pedidos.TipoOperacao.NaoFoiPossivelCadastrarMaisDeUmaOperacaoComConfiguracaoPadraoCargaDevolucao, tipoOperacaoConfiguracaoDevolucaoCompra.Descricao));
                }

                PreencherTipoOperacao(tipoOperacao, unidadeDeTrabalho);

                bool novo = true;
                if (!string.IsNullOrWhiteSpace(tipoOperacao.CodigoIntegracao))
                {
                    Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacaoExiste = repTipoOperacao.BuscarPorCodigoIntegracao(tipoOperacao.CodigoIntegracao);
                    if (tipoOperacaoExiste != null)
                        novo = false;
                }
                else
                    tipoOperacao.CodigoIntegracao = Guid.NewGuid().ToString().Replace("-", "");

                Dominio.Entidades.Embarcador.Pedidos.TipoOperacao existeTipoOpercaoForaDoPais = repTipoOperacao.BuscarTipoOperacaoPadraoQuandoForadoPais();
                Dominio.Entidades.Embarcador.Pedidos.TipoOperacao existeTipoOpercaoDentroDoPais = repTipoOperacao.BuscarTipoOperacaoPadraoQuandoDentrodoPais();

                if ((existeTipoOpercaoForaDoPais?.UsarComoPadraoParaFretesForaDoPais ?? false) && tipoOperacao.UsarComoPadraoParaFretesForaDoPais)
                {
                    throw new ControllerException(Localization.Resources.Pedidos.TipoOperacao.JaExisteUmTipoDeOperacaoPadraoParaFretesForaDoPaisAtivada);
                }
                if ((existeTipoOpercaoDentroDoPais?.UsarComoPadraoParaFretesDentroDoPais ?? false) && tipoOperacao.UsarComoPadraoParaFretesDentroDoPais)
                {
                    throw new ControllerException(Localization.Resources.Pedidos.TipoOperacao.JaExisteUmTipoDeOperacaoComConfiguracaoFreteDoPaisAtivada);
                }

                SalvarIntegracaoMultiEmbarcador(tipoOperacao);
                PreencherConfiguracaoLicenca(tipoOperacao);
                PreencherConfiguracaoAgendamentoColeta(tipoOperacao, unidadeDeTrabalho);
                PreencherConfiguracaoCarga(tipoOperacao);
                PreencherConfiguracaoTransportador(tipoOperacao);
                SetarDadosPagbem(tipoOperacao);

                if (!novo)
                    throw new ControllerException(Localization.Resources.Pedidos.TipoOperacao.JaExisteUmaOperacaoParaCodigoDeIntegracaoInformado);

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeDeTrabalho);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarPrimeiroRegistro();

                if (tipoOperacao.CriarNovoPedidoAoVincularNotaFiscalComDiferentesParticipantes)
                    configuracaoTMS.NaoValidarRemetenteNotaComRemetentePedido = true;

                Dominio.Entidades.Auditoria.HistoricoObjeto historicoConfiguracao = repConfiguracaoTMS.Atualizar(configuracaoTMS, Auditado);
                repTipoOperacao.Inserir(tipoOperacao, Auditado);

                dynamic configuracao = SalvarConfiguracaoEmissaoCTe(tipoOperacao, unidadeDeTrabalho);
                SalvarLayoutsEDI(tipoOperacao, unidadeDeTrabalho);
                SalvarConfiguracaoCIOTPamcard(tipoOperacao, unidadeDeTrabalho);
                SalvarProdutosPadroes(tipoOperacao, unidadeDeTrabalho);
                SalvarTiposOcorrencia(tipoOperacao, unidadeDeTrabalho);
                SalvarGrupoTomadoresBloqueados(tipoOperacao, unidadeDeTrabalho);
                SalvarTipoComprovantes(tipoOperacao, unidadeDeTrabalho);

                AtualizarIntegracoes(tipoOperacao, unidadeDeTrabalho);
                AtualizarConfiguracoesMobile(tipoOperacao, null, unidadeDeTrabalho);
                AtualizarTransportadores(tipoOperacao, null, unidadeDeTrabalho);

                AtualizarConfiguracoesEmissao(tipoOperacao, configuracao, null, unidadeDeTrabalho);
                SalvarConfiguracaoFatura(tipoOperacao, unidadeDeTrabalho);
                AtualizarConfiguracoesCalculoFrete(tipoOperacao, null, unidadeDeTrabalho);
                AtualizarConfiguracoesEmissaoDocumento(tipoOperacao, null, unidadeDeTrabalho);
                AtualizarConfiguracoesIntegracao(tipoOperacao, null, unidadeDeTrabalho);
                AtualizarConfiguracoesCarga(tipoOperacao, null, unidadeDeTrabalho);
                AtualizarConfiguracoesFreeTime(tipoOperacao, null, unidadeDeTrabalho);
                AtualizarConfiguracoesTerceiro(tipoOperacao, null, unidadeDeTrabalho);
                AtualizarConfiguracoesImpressao(tipoOperacao, null, unidadeDeTrabalho);
                AtualizarConfiguracoesPagamentos(tipoOperacao, null, unidadeDeTrabalho);
                AtualizarConfiguracoesIntercab(tipoOperacao, null, unidadeDeTrabalho);
                AtualizarConfiguracoesTransportador(tipoOperacao, null, unidadeDeTrabalho);
                AtualizarConfiguracoesDocumentoEmissao(tipoOperacao, null, unidadeDeTrabalho);
                AtualizarConfiguracoesCanhoto(tipoOperacao, null, unidadeDeTrabalho);
                AtualizarConfiguracoesControleEntrega(tipoOperacao, null, unidadeDeTrabalho);
                AtualizarConfiguracaoChamado(tipoOperacao, null, unidadeDeTrabalho);
                AtualizarConfiguracoesLicenca(tipoOperacao, null, unidadeDeTrabalho);
                AtualizarConfiguracoesJanelaCarregamento(tipoOperacao, null, unidadeDeTrabalho);
                AtualizarConfiguracoesMontagemCarga(tipoOperacao, null, unidadeDeTrabalho);
                AtualizarConfiguracoesIntegracaoDiageo(tipoOperacao, null, unidadeDeTrabalho);
                AtualizarConfiguracoesEMP(tipoOperacao, null, unidadeDeTrabalho);
                await AtualizarConfiguracoesTrizyAsync(tipoOperacao, null, unidadeDeTrabalho, cancellationToken);
                AtualizarConfiguracoesPedido(tipoOperacao, null, unidadeDeTrabalho);
                AtualizarNotificacoesAppTrizy(tipoOperacao, unidadeDeTrabalho);
                await AtualizarEventosSuperAppAsync(tipoOperacao, unidadeDeTrabalho, cancellationToken);
                AtualizarConfiguracoesIntegracaoTransSat(tipoOperacao, null, unidadeDeTrabalho);
                AtualizarConfiguracoesTipoPropriedadeVeiculo(tipoOperacao, null, unidadeDeTrabalho);
                AtualizarConfiguracoesCotacaoPedido(tipoOperacao, unidadeDeTrabalho);
                AtualizarConfiguracoesContainer(tipoOperacao, unidadeDeTrabalho);

                SalvarExcecoesAngelLira(tipoOperacao, unidadeDeTrabalho);

                AtualizarGatilhosGeracaoAutomaticaPedidoOcorrenciaColetaEntrega(tipoOperacao, null, unidadeDeTrabalho);
                SalvarTiposCargas(tipoOperacao, unidadeDeTrabalho);
                SalvarTiposCargasEmissao(tipoOperacao, unidadeDeTrabalho);
                SalvarConfiguracaoVendedores(tipoOperacao, unidadeDeTrabalho);
                SalvarConfiguracaoPesoConsideradoCarga(tipoOperacao, unidadeDeTrabalho);
                SalvarTipoOperacaoFilialMotoristaGenerico(tipoOperacao, unidadeDeTrabalho);
                await SalvarCodigosIntegracaoAsync(tipoOperacao, unidadeDeTrabalho, cancellationToken);

                await unidadeDeTrabalho.CommitChangesAsync();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                await unidadeDeTrabalho.RollbackAsync();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                await unidadeDeTrabalho.RollbackAsync();
                return new JsonpResult(false, Localization.Resources.Pedidos.TipoOperacao.OcorreuUmaFalhaAoAdicionar);
            }
            finally
            {
                await unidadeDeTrabalho.DisposeAsync();
            }
        }


        public async Task<IActionResult> Atualizar(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unidadeDeTrabalho);

                unidadeDeTrabalho.Start();

                if (Request.GetBoolParam("UsarComoPadraoQuandoCargaForDevolucaoDoTipoColeta"))
                {
                    Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacaoConfiguracaoDevolucaoCompra = repTipoOperacao.BuscarTipoOperacaoPadraoGeracaoCargaDevolucao();

                    if (tipoOperacaoConfiguracaoDevolucaoCompra != null && tipoOperacaoConfiguracaoDevolucaoCompra.Codigo != Request.GetIntParam("Codigo"))
                        throw new ControllerException(string.Format(Localization.Resources.Pedidos.TipoOperacao.NaoFoiPossivelCadastrarMaisDeUmaOperacaoComConfiguracaoPadraoCargaDevolucao, tipoOperacaoConfiguracaoDevolucaoCompra.Descricao));
                }

                Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = repTipoOperacao.BuscarPorCodigo(Request.GetIntParam("Codigo"), true);

                if (tipoOperacao == null)
                    throw new ControllerException(Localization.Resources.Pedidos.TipoOperacao.NaoFoiPossivelEncontrarRegistro);

                PreencherTipoOperacao(tipoOperacao, unidadeDeTrabalho);

                bool novo = true;
                Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacaoExiste = repTipoOperacao.BuscarPorCodigoIntegracao(tipoOperacao.CodigoIntegracao);
                if (tipoOperacaoExiste != null && tipoOperacao.Codigo != tipoOperacaoExiste.Codigo)
                    novo = false;

                if (!novo)
                    throw new ControllerException(Localization.Resources.Pedidos.TipoOperacao.JaExisteUmaOperacaoParaCodigoDeIntegracaoInformado);

                dynamic configuracao = SalvarConfiguracaoEmissaoCTe(tipoOperacao, unidadeDeTrabalho);
                SalvarIntegracaoMultiEmbarcador(tipoOperacao);
                PreencherConfiguracaoLicenca(tipoOperacao);
                PreencherConfiguracaoAgendamentoColeta(tipoOperacao, unidadeDeTrabalho);
                PreencherConfiguracaoCarga(tipoOperacao);
                PreencherConfiguracaoTransportador(tipoOperacao);
                SetarDadosPagbem(tipoOperacao);

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeDeTrabalho);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarPrimeiroRegistro();

                if (tipoOperacao.CriarNovoPedidoAoVincularNotaFiscalComDiferentesParticipantes)
                    configuracaoTMS.NaoValidarRemetenteNotaComRemetentePedido = true;

                Dominio.Entidades.Auditoria.HistoricoObjeto historicoConfiguracao = repConfiguracaoTMS.Atualizar(configuracaoTMS, Auditado);
                Dominio.Entidades.Auditoria.HistoricoObjeto historico = repTipoOperacao.Atualizar(tipoOperacao, Auditado);

                SalvarLayoutsEDI(tipoOperacao, unidadeDeTrabalho);
                SalvarConfiguracaoCIOTPamcard(tipoOperacao, unidadeDeTrabalho);
                SalvarProdutosPadroes(tipoOperacao, unidadeDeTrabalho);
                SalvarTiposOcorrencia(tipoOperacao, unidadeDeTrabalho);
                SalvarGrupoTomadoresBloqueados(tipoOperacao, unidadeDeTrabalho);
                SalvarTipoComprovantes(tipoOperacao, unidadeDeTrabalho);

                AtualizarIntegracoes(tipoOperacao, unidadeDeTrabalho);
                AtualizarConfiguracoesMobile(tipoOperacao, historico, unidadeDeTrabalho);
                AtualizarTransportadores(tipoOperacao, historico, unidadeDeTrabalho);

                AtualizarConfiguracoesEmissao(tipoOperacao, configuracao, historico, unidadeDeTrabalho);
                SalvarConfiguracaoFatura(tipoOperacao, unidadeDeTrabalho);
                SalvarConfiguracaoVendedores(tipoOperacao, unidadeDeTrabalho);
                AtualizarConfiguracoesCalculoFrete(tipoOperacao, historico, unidadeDeTrabalho);
                AtualizarConfiguracoesEmissaoDocumento(tipoOperacao, historico, unidadeDeTrabalho);
                AtualizarConfiguracoesIntegracao(tipoOperacao, historico, unidadeDeTrabalho);
                AtualizarConfiguracoesCarga(tipoOperacao, historico, unidadeDeTrabalho);
                AtualizarConfiguracoesFreeTime(tipoOperacao, historico, unidadeDeTrabalho);
                AtualizarConfiguracoesTerceiro(tipoOperacao, historico, unidadeDeTrabalho);
                AtualizarConfiguracoesImpressao(tipoOperacao, historico, unidadeDeTrabalho);
                AtualizarConfiguracoesPagamentos(tipoOperacao, historico, unidadeDeTrabalho);
                AtualizarConfiguracoesIntercab(tipoOperacao, historico, unidadeDeTrabalho);
                AtualizarConfiguracoesTransportador(tipoOperacao, historico, unidadeDeTrabalho);
                AtualizarConfiguracoesDocumentoEmissao(tipoOperacao, historico, unidadeDeTrabalho);
                AtualizarConfiguracoesCanhoto(tipoOperacao, historico, unidadeDeTrabalho);
                AtualizarConfiguracoesControleEntrega(tipoOperacao, historico, unidadeDeTrabalho);
                AtualizarConfiguracaoChamado(tipoOperacao, historico, unidadeDeTrabalho);
                AtualizarConfiguracoesLicenca(tipoOperacao, historico, unidadeDeTrabalho);
                AtualizarConfiguracoesJanelaCarregamento(tipoOperacao, historico, unidadeDeTrabalho);
                AtualizarConfiguracoesMontagemCarga(tipoOperacao, historico, unidadeDeTrabalho);
                AtualizarConfiguracoesIntegracaoDiageo(tipoOperacao, historico, unidadeDeTrabalho);
                AtualizarConfiguracoesEMP(tipoOperacao, historico, unidadeDeTrabalho);
                await AtualizarConfiguracoesTrizyAsync(tipoOperacao, historico, unidadeDeTrabalho, cancellationToken);
                AtualizarConfiguracoesPedido(tipoOperacao, historico, unidadeDeTrabalho);
                AtualizarNotificacoesAppTrizy(tipoOperacao, unidadeDeTrabalho);
                await AtualizarEventosSuperAppAsync(tipoOperacao, unidadeDeTrabalho, cancellationToken);
                AtualizarConfiguracoesGestaoDevolucao(tipoOperacao, unidadeDeTrabalho);
                AtualizarConfiguracoesIntegracaoTransSat(tipoOperacao, historico, unidadeDeTrabalho);
                AtualizarConfiguracoesTipoPropriedadeVeiculo(tipoOperacao, historico, unidadeDeTrabalho);
                AtualizarConfiguracoesCotacaoPedido(tipoOperacao, unidadeDeTrabalho);
                AtualizarConfiguracoesContainer(tipoOperacao, unidadeDeTrabalho);

                SalvarExcecoesAngelLira(tipoOperacao, unidadeDeTrabalho);

                AtualizarGatilhosGeracaoAutomaticaPedidoOcorrenciaColetaEntrega(tipoOperacao, historico, unidadeDeTrabalho);
                SalvarTiposCargas(tipoOperacao, unidadeDeTrabalho);
                SalvarTiposCargasEmissao(tipoOperacao, unidadeDeTrabalho);
                SalvarConfiguracaoPesoConsideradoCarga(tipoOperacao, unidadeDeTrabalho);
                SalvarTipoOperacaoFilialMotoristaGenerico(tipoOperacao, unidadeDeTrabalho);
                await SalvarCodigosIntegracaoAsync(tipoOperacao, unidadeDeTrabalho, cancellationToken);

                await unidadeDeTrabalho.CommitChangesAsync();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                await unidadeDeTrabalho.RollbackAsync();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                await unidadeDeTrabalho.RollbackAsync();
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAtualizar);
            }
            finally
            {
                await unidadeDeTrabalho.DisposeAsync();
            }
        }

        public async Task<IActionResult> BuscarPorCodigo(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                bool duplicar = Request.GetBoolParam("Duplicar");

                Repositorio.Embarcador.Pedidos.TipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unidadeDeTrabalho);
                Repositorio.Embarcador.Pedidos.TipoOperacaoTipoCarga repositorioTipoOperacaoTipoCarga = new Repositorio.Embarcador.Pedidos.TipoOperacaoTipoCarga(unidadeDeTrabalho);
                Repositorio.Embarcador.Pedidos.TipoOperacaoTipoCargaEmissao repositorioTipoOperacaoTipoCargaEmissao = new Repositorio.Embarcador.Pedidos.TipoOperacaoTipoCargaEmissao(unidadeDeTrabalho);
                Repositorio.Embarcador.Pedidos.TipoOperacaoAnexo repositorioTipoOperacaoAnexo = new Repositorio.Embarcador.Pedidos.TipoOperacaoAnexo(unidadeDeTrabalho);
                Repositorio.Embarcador.Pedidos.TipoOperacaoEventoSuperApp repositorioTipoOperacaoEventoSuperApp = new Repositorio.Embarcador.Pedidos.TipoOperacaoEventoSuperApp(unidadeDeTrabalho, cancellationToken);
                Repositorio.Embarcador.Pedidos.TipoOperacaoNotificacaoApp repositorioTipoOperacaoNotificacaoApp = new Repositorio.Embarcador.Pedidos.TipoOperacaoNotificacaoApp(unidadeDeTrabalho);
                Repositorio.Embarcador.Pedidos.TipoOperacaoVendedores repTipoOperacaoVendedores = new Repositorio.Embarcador.Pedidos.TipoOperacaoVendedores(unidadeDeTrabalho);
                Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoTrizyInformacaoAdicionalRelatorio repositorioConfiguracaoTipoOperacaoTrizyInformacaoAdicionalRelatorio = new Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoTrizyInformacaoAdicionalRelatorio(unidadeDeTrabalho, cancellationToken);

                Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = repositorioTipoOperacao.BuscarPorCodigo(codigo);
                List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoAnexo> anexos = repositorioTipoOperacaoAnexo.BuscarPorCodigoTipoOperacao(codigo);
                List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoEventoSuperApp> listaEventosSuperApp = await repositorioTipoOperacaoEventoSuperApp.BuscarPorTipoOperacaoAsync(codigo);
                List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoNotificacaoApp> listaNotificacoes = repositorioTipoOperacaoNotificacaoApp.BuscarPorTipoOperacao(codigo);


                if (tipoOperacao == null)
                    return new JsonpResult(false, true, Localization.Resources.Pedidos.TipoOperacao.NaoFoiPossivelEncontrarRegistro);

                List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoTipoCarga> tipoOperacaoTiposCargas = repositorioTipoOperacaoTipoCarga.BuscarPorTipoOperacao(tipoOperacao.Codigo);
                List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoTipoCargaEmissao> tipoOperacaoTiposCargasEmissao = repositorioTipoOperacaoTipoCargaEmissao.BuscarPorTipoOperacao(tipoOperacao);
                List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoVendedores> tipoOperacaoVendedores = repTipoOperacaoVendedores.BuscarPorCodigoTipoOperacao(tipoOperacao.Codigo);
                List<Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoTrizyInformacaoAdicionalRelatorio> listaConfiguracaoTipoOperacaoTrizyInformacaoAdicionalRelatorio = await repositorioConfiguracaoTipoOperacaoTrizyInformacaoAdicionalRelatorio.BuscarPorConfiguracaoTipoOperacaoTrizyAsync(tipoOperacao.ConfiguracaoTrizy?.Codigo ?? 0);

                var retorno = new
                {
                    Codigo = duplicar ? 0 : tipoOperacao.Codigo,
                    tipoOperacao.InserirDadosContabeisXCampoXTextCTe,
                    tipoOperacao.UtilizarValorFreteNotasFiscais,
                    tipoOperacao.NaoGerarContratoFreteTerceiro,
                    NaoGerarCIOT = tipoOperacao.NaoGerarContratoFreteTerceiro,
                    tipoOperacao.EnviarNumeroPedidoEmbarcadorNoCodigoControleBrasilRisk,
                    tipoOperacao.NaoEnviarOrigemComoUltimoPontoRota,
                    tipoOperacao.PossuiIntegracaoRaster,
                    tipoOperacao.CodigoFilialRaster,
                    tipoOperacao.CodigoPerfilSegurancaRaster,
                    tipoOperacao.PesoMaximo,
                    tipoOperacao.PesoMinimo,
                    tipoOperacao.RemessaSAP,
                    tipoOperacao.CodigoIntegracaoRepom,
                    tipoOperacao.RemetenteDoCTeSeraODestinatarioDoPedido,
                    tipoOperacao.PossuiIntegracaoAngelLira,
                    tipoOperacao.PossuiIntegracaoTrafegus,
                    tipoOperacao.PermiteConsultarPorPacotesLoggi,
                    tipoOperacao.TipoOperacaoExibeValorUnitarioDoProduto,
                    tipoOperacao.PossuiIntegracaoANTT,
                    tipoOperacao.PossuiIntegracaoIntercab,
                    tipoOperacao.PermitirAdicionarNaJanelaDescarregamento,
                    tipoOperacao.BloquearAdicaoNaJanelaDescarregamentoAutomaticamente,
                    tipoOperacao.PossuiIntegracaoGoldenService,
                    tipoOperacao.ColetaEmProdutorRural,
                    tipoOperacao.TipoPagamentoContratoFreteTerceiro,
                    tipoOperacao.PercentualAdiantamentoFreteTerceiro,
                    tipoOperacao.PercentualAbastecimentoFreteTerceiro,
                    tipoOperacao.DiasVencimentoSaldoContratoFrete,
                    tipoOperacao.DiasVencimentoAdiantamentoContratoFrete,
                    tipoOperacao.PercentualCobrancaPadraoTerceiros,
                    tipoOperacao.UtilizarConfiguracaoTerceiroComoPadrao,
                    tipoOperacao.UtilizarConfiguracaoTerceiro,
                    tipoOperacao.GerarDiariaMotoristaProprio,
                    tipoOperacao.PossuiIntegracaoLogiun,
                    tipoOperacao.PossuiIntegracaoMundialRisk,
                    tipoOperacao.CodigoIntegracaoGoldenService,
                    tipoOperacao.UtilizarDadosPedidoParaNotasExterior,
                    tipoOperacao.Ativo,
                    tipoOperacao.CodigoIntegracaoGerenciadoraRisco,
                    tipoOperacao.Descricao,
                    tipoOperacao.ExigeNotaFiscalParaCalcularFrete,
                    tipoOperacao.UtilizarExpedidorComoTransportador,
                    tipoOperacao.EmitirDocumentosRetroativamente,
                    tipoOperacao.NaoExigeVeiculoParaEmissao,
                    tipoOperacao.TipoPessoa,
                    tipoOperacao.FretePorContadoCliente,
                    tipoOperacao.PermitirQualquerModeloVeicular,
                    tipoOperacao.PermitirTransbordarNotasDeOutrasCargas,
                    tipoOperacao.EmiteCTeFilialEmissora,
                    tipoOperacao.ExigeProcImportacaoPedido,
                    tipoOperacao.AverbarDocumentoDaSubcontratacao,
                    tipoOperacao.CalculaFretePorTabelaFreteFilialEmissora,
                    tipoOperacao.HabilitarGestaoPatio,
                    tipoOperacao.HabilitarGestaoPatioDestino,
                    tipoOperacao.OperacaoRecolhimentoTroca,
                    tipoOperacao.ExigePercursoEntreCNPJ,
                    tipoOperacao.ExigeConformacaoFreteAntesEmissao,
                    tipoOperacao.DocumentoXCampo,
                    tipoOperacao.DocumentoXTexto,
                    tipoOperacao.UtilizarXCampoSomenteNoRedespacho,
                    tipoOperacao.NaoExigeConformacaoDasNotasEmissao,
                    tipoOperacao.LiberarCargaSemNFeAutomaticamenteAposLiberarFaturamento,
                    tipoOperacao.GerarDocumentoPadraoParaCadaPedidoCarga,
                    tipoOperacao.LiberarCargaAutomaticamenteParaFaturamentoAposPrazoEsgotadoJanelaCarregamento,
                    tipoOperacao.ExigePlacaTracao,
                    tipoOperacao.NaoValidarTransportadorImportacaoDocumento,
                    tipoOperacao.EmissaoDocumentosForaDoSistema,
                    tipoOperacao.CompraValePedagioDocsEmitidosFora,
                    tipoOperacao.NaoPermiteAgruparCargas,
                    tipoOperacao.ManterUnicaCargaNoAgrupamento,
                    tipoOperacao.GerarCargaViaMontagemDoTipoPreCarga,
                    tipoOperacao.UsarRecebedorComoPontoPartidaCarga,
                    tipoOperacao.ProdutoPredominanteOperacao,
                    tipoOperacao.ExigeRecebedor,
                    tipoOperacao.NaoGerarFaturamento,
                    tipoOperacao.ExigeQueVeiculoIgualModeloVeicularDaCarga,
                    tipoOperacao.IndicadorGlobalizadoRemetente,
                    tipoOperacao.IndicadorGlobalizadoDestinatario,
                    tipoOperacao.SempreUsarIndicadorGlobalizadoDestinatario,
                    tipoOperacao.NotificarCasoNumeroPedidoForExistente,
                    tipoOperacao.IndicadorGlobalizadoDestinatarioNFSe,
                    tipoOperacao.ValidarNotaFiscalPeloDestinatario,
                    tipoOperacao.NaoGerarTituloGNREAutomatico,
                    tipoOperacao.PermiteImportarDocumentosManualmente,
                    tipoOperacao.NaoValidarNotaFiscalExistente,
                    tipoOperacao.NaoValidarNotasFiscaisComDiferentesPortos,
                    tipoOperacao.UsarConfiguracaoEmissao,
                    tipoOperacao.UsarConfiguracaoFaturaPorTipoOperacao,
                    tipoOperacao.EmissaoMDFeManual,
                    tipoOperacao.OperacaoDeRedespacho,
                    tipoOperacao.UsaJanelaCarregamentoPorEscala,
                    tipoOperacao.PermiteTransportadorAvancarEtapaEmissao,
                    tipoOperacao.UtilizarTipoOperacaoPreCargaAoGerarCarga,
                    tipoOperacao.PermiteUtilizarEmContratoFrete,
                    tipoOperacao.OperacaoDeImportacaoExportacao,
                    tipoOperacao.GeraCargaAutomaticamente,
                    tipoOperacao.TipoOperacaoUtilizaCentroDeCustoPEP,
                    tipoOperacao.TipoOperacaoUtilizaContaRazao,
                    tipoOperacao.PercentualCobrarDesistenciaCarga,
                    tipoOperacao.PercentualCobrarDesistenciaCarregamento,
                    tipoOperacao.PermiteDesistenciaCarga,
                    tipoOperacao.PermiteDesistenciaCarregamento,
                    tipoOperacao.AtualizarProdutosPorXmlNotaFiscal,
                    tipoOperacao.AtualizarSaldoPedidoProdutosPorXmlNotaFiscal,
                    tipoOperacao.CobrarDesistenciaCargaAposHorario,
                    tipoOperacao.CodigoModeloContratacao,
                    tipoOperacao.IntegracaoProcedimentoEmbarque,
                    tipoOperacao.ExigirConfirmacaoDadosTransportadorAvancarCarga,
                    tipoOperacao.GerarPedidoNoRecebientoNFe,
                    tipoOperacao.UtilizarDataNFeEmissaoDocumentos,
                    tipoOperacao.ObrigarRotaNaMontagemDeCarga,
                    tipoOperacao.ExigirDataRetiradaCtrnJanelaCarregamentoTransportador,
                    tipoOperacao.ExigirMaxGrossJanelaCarregamentoTransportador,
                    tipoOperacao.ExigirNumeroContainerJanelaCarregamentoTransportador,
                    tipoOperacao.ExigirTaraContainerJanelaCarregamentoTransportador,
                    tipoOperacao.CorJanelaCarregamento,
                    UtilizarCorJanelaCarregamento = !string.IsNullOrWhiteSpace(tipoOperacao.CorJanelaCarregamento),
                    tipoOperacao.ObrigatorioInformarAnexoSolicitacaoFrete,
                    HoraCobrarDesistenciaCarga = tipoOperacao.HoraCobrarDesistenciaCarga.HasValue ? tipoOperacao.HoraCobrarDesistenciaCarga.Value.ToString(@"hh\:mm") : string.Empty,
                    TipoDeCargaPadraoOperacao = new { Codigo = tipoOperacao.TipoDeCargaPadraoOperacao != null ? tipoOperacao.TipoDeCargaPadraoOperacao.Codigo : 0, Descricao = tipoOperacao.TipoDeCargaPadraoOperacao != null ? tipoOperacao.TipoDeCargaPadraoOperacao.Descricao : "" },
                    UtilizarTipoCargaPadrao = tipoOperacao.TipoDeCargaPadraoOperacao != null ? true : false,
                    GrupoTomador = new { Codigo = tipoOperacao.GrupoTomador?.Codigo ?? 0, Descricao = tipoOperacao.GrupoTomador?.Descricao ?? "" },
                    UtilizarGrupoTomador = tipoOperacao.GrupoTomador != null,
                    InformarProdutoPredominanteOperacao = !string.IsNullOrWhiteSpace(tipoOperacao.ProdutoPredominanteOperacao) ? true : false,
                    tipoOperacao.Observacao,
                    CodigoIntegracao = duplicar ? string.Empty : tipoOperacao.CodigoIntegracao,
                    tipoOperacao.TipoImpressao,
                    tipoOperacao.TipoImpressaoDiarioBordo,
                    tipoOperacao.UtilizarFatorCubagem,
                    tipoOperacao.FatorCubagem,
                    tipoOperacao.TipoUsoFatorCubagem,
                    tipoOperacao.UtilizarPaletizacao,
                    tipoOperacao.NaoPermitirAlterarValorFreteNaCarga,
                    tipoOperacao.PesoPorPallet,
                    tipoOperacao.ObrigatorioPassagemExpedicao,
                    tipoOperacao.CentroCustoBrasilRisk,
                    tipoOperacao.CNPJClienteBrasilRisk,
                    tipoOperacao.CNPJTransportadoraBrasilRisk,
                    tipoOperacao.ProdutoBrasilRisk,
                    tipoOperacao.PossuiIntegracaoBrasilRisk,
                    tipoOperacao.PedidoLogisticoBrasilRisk,
                    tipoOperacao.CentroCustoMundialRisk,
                    tipoOperacao.TipoPlanoInfolog,
                    tipoOperacao.CNPJClienteMundialRisk,
                    tipoOperacao.CNPJTransportadoraMundialRisk,
                    tipoOperacao.ProdutoMundialRisk,
                    tipoOperacao.ProdutoLogiun,
                    tipoOperacao.CNPJClienteLogiun,
                    tipoOperacao.CNPJTransportadoraLogiun,
                    tipoOperacao.CentroCustoLogiun,
                    tipoOperacao.VincularMotoristaFilaCarregamentoManualmente,
                    tipoOperacao.PermitirGerarRedespacho,
                    tipoOperacao.PermitirGerarRecorrenciaRedespacho,
                    tipoOperacao.NaoUtilizaJanelaCarregamento,
                    tipoOperacao.PermitirMultiplosDestinatariosPedido,
                    tipoOperacao.PermitirMultiplosRemetentesPedido,
                    tipoOperacao.UtilizarDeslocamentoPedido,
                    tipoOperacao.TipoUltimoPontoRoteirizacao,
                    tipoOperacao.EixosSuspenso,
                    tipoOperacao.TipoCarregamento,
                    tipoOperacao.TipoObrigacaoUsoTerminal,
                    tipoOperacao.OperacaoTrocaNota,
                    tipoOperacao.PermitirTrocaNota,
                    tipoOperacao.OperacaoExigeInformarCargaRetorno,
                    tipoOperacao.GerarCTeComplementarNaCarga,
                    tipoOperacao.ExclusivaDeSubcontratacaoOuRedespacho,
                    tipoOperacao.AdicionarOuRemoverPedidosReplicaAosTrechosAnteriores,
                    tipoOperacao.SempreEmitirSubcontratacao,
                    tipoOperacao.ExigirValorFreteAvancarCarga,
                    tipoOperacao.PermiteGerarPedidoSemDestinatario,
                    tipoOperacao.PermiteReordenarEntregasCarga,
                    tipoOperacao.ExigeInformarIscaNaCarga,
                    tipoOperacao.GerarCargaFinalizada,
                    tipoOperacao.PermiteInformarIscaNaCarga,
                    tipoOperacao.NaoIncluirICMSFrete,
                    tipoOperacao.ExigirInformarPeso,
                    tipoOperacao.ExigirInformarQuantidadeMercadoria,
                    tipoOperacao.PermiteEmitirCargaDiferentesOrigensParcialmente,
                    tipoOperacao.ExigirInformarValorNota,
                    tipoOperacao.ExigirInformarValorMercadoria,
                    tipoOperacao.ExigirInformarNCM,
                    tipoOperacao.ExigeProdutoEmbarcadorPedido,
                    tipoOperacao.BloquearEmisssaoComMesmoLocalDeOrigemEDestino,
                    tipoOperacao.DisponibilizarPedidosParaSeparacaoAposEmissaoDocumentos,
                    tipoOperacao.DisponibilizarPedidosComRecebedorParaSeparacaoAposEmissaoDocumentos,
                    tipoOperacao.TransbordoRodoviario,
                    tipoOperacao.LogisticaReversa,
                    tipoOperacao.ExigirInformarCFOP,
                    tipoOperacao.ExigirInformarM3,
                    tipoOperacao.NaoExigeRotaRoteirizada,
                    tipoOperacao.RoteirizarPorLocalidade,
                    tipoOperacao.NaoComprarValePedagio,
                    tipoOperacao.PermitirUtilizarPlacaContrato,
                    tipoOperacao.NaoGerarControleColetaEntrega,
                    tipoOperacao.GerarControleColetaEntregaAposEmissaoDocumentos,
                    tipoOperacao.AgendamentoGeraApenasPedido,
                    tipoOperacao.PermitirVeiculoDiferenteMontagemCarga,
                    tipoOperacao.TipoOperacaoAgendamento,
                    tipoOperacao.NaoPermiteRejeitarEntrega,
                    tipoOperacao.UtilizarRecebedorApenasComoParticipante,
                    tipoOperacao.ConfiguracaoTabelaFretePorPedido,
                    tipoOperacao.TempoEntregaAngelLira,
                    tipoOperacao.NaoEnviarDataInicioETerminoViagemAngelLira,
                    tipoOperacao.IntegrarPreSMAngelLira,
                    tipoOperacao.ReintegrarSMCargaAngelLira,
                    tipoOperacao.GerarComissaoParcialMotorista,
                    tipoOperacao.PercentualComissaoParcialMotorista,
                    tipoOperacao.NaoEmitirCargaComValorZerado,
                    tipoOperacao.NaoPermitirValorFreteLiquidoZerado,
                    tipoOperacao.AvancarCargaAutomaticaAposMontagem,
                    tipoOperacao.AvancarEtapaFreteAutomaticamente,
                    tipoOperacao.ValidarTomadorDoPedidoDiferenteDaCarga,
                    tipoOperacao.CamposSecundariosObrigatoriosPedido,
                    tipoOperacao.ImportarTerminalOrigemComoExpedidor,
                    tipoOperacao.ImportarTerminalDestinoComoRecebedor,
                    tipoOperacao.LiberarAutomaticamentePagamento,
                    tipoOperacao.CriarNovoPedidoAoVincularNotaFiscalComDiferentesParticipantes,
                    tipoOperacao.GerarRedespachoAutomaticamente,
                    tipoOperacao.GerarRedespachoAutomaticamentePorPedidoAposEmissaoDocumentos,
                    tipoOperacao.GerarRedespachoParaOutrasEtapasCarregamento,
                    tipoOperacao.Reentrega,
                    tipoOperacao.RetornoVazio,
                    tipoOperacao.PermitirSolicitarNotasFiscaisSemEncerrarMDFeAnterior,
                    tipoOperacao.DeslocamentoVazio,
                    tipoOperacao.EncerrarMDFeManualmente,
                    tipoOperacao.CargaPropria,
                    tipoOperacao.PermitirSelecionarNotasCompativeis,
                    tipoOperacao.PermitirTransportadorInformeNotasCompativeis,
                    tipoOperacao.UtilizarNomeDestinatarioNotaFiscalParaEmitirCTe,
                    tipoOperacao.EmissaoAutomaticaCTe,
                    tipoOperacao.PermitirAdicionarRemoverPedidosEtapa1,
                    tipoOperacao.TomadorCTeSubcontratacaoDeveSerDoCTeOriginal,
                    tipoOperacao.FixarValorFreteNegociadoRateioPedidos,
                    tipoOperacao.DataEntregaPagbem,
                    tipoOperacao.PesoPagbem,
                    tipoOperacao.TicketBalancaPagbem,
                    tipoOperacao.AvariaPagbem,
                    tipoOperacao.CanhotoNFePagbem,
                    tipoOperacao.ComprovantePedagioPagbem,
                    tipoOperacao.DACTEPagbem,
                    tipoOperacao.ContratoTransportePagbem,
                    tipoOperacao.DataDesembarquePagbem,
                    tipoOperacao.FreteTipoPesoPagBem,
                    tipoOperacao.RelatorioInspecaoDesembarquePagbem,
                    tipoOperacao.UtilizarValorFreteOriginalSubcontratacao,
                    UtilizarMoedaEstrangeira = tipoOperacao.UtilizarMoedaEstrangeira ?? false,
                    Moeda = tipoOperacao.Moeda ?? MoedaCotacaoBancoCentral.Real,
                    tipoOperacao.PermitirExpedidorRecebedorIgualRemetenteDestinatario,
                    tipoOperacao.AlterarRemetentePedidoConformeNotaFiscal,
                    tipoOperacao.RetornarCanhotoQuandoTodasNotasDoCTeEstiveremConformadasPagamento,
                    tipoOperacao.RetornarCarregamentoPendenteAposEtapaCTE,
                    tipoOperacao.NaoIntegrarOpentech,
                    tipoOperacao.IntegrarPedidosNaIntegracaoOpentech,
                    tipoOperacao.ValorMinimoMercadoriaOpenTech,
                    tipoOperacao.NotificarRemetentePorEmailAoSolicitarNotas,
                    tipoOperacao.ValidarMotoristaTeleriscoAoConfirmarTransportador,
                    tipoOperacao.ValidarMotoristaVeiculoBrasilRiskAoConfirmarTransportador,
                    tipoOperacao.IntegrarDadosTransporteBrasilRiskAoAtualizarVeiculoMotorista,
                    tipoOperacao.ValidarMotoristaVeiculoAdagioAoConfirmarTransportador,
                    tipoOperacao.ValidarMotoristaBuonnyAoConfirmarTransportador,
                    tipoOperacao.NaoGerarCanhoto,
                    tipoOperacao.UtilizarMaiorDistanciaPedidoNaMontagemCarga,
                    tipoOperacao.EmitirNFeRemessa,
                    tipoOperacao.PossuiIntegracaoNOX,
                    tipoOperacao.ValorMinimoMercadoriaNOX,
                    tipoOperacao.IntegrarPreSMNOX,
                    tipoOperacao.UtilizarTipoCargaPedidoCalculoFrete,
                    tipoOperacao.PermiteDividirPedidoEmCargasDiferentes,
                    tipoOperacao.NaoProcessarTrocaAlvoViaMonitoramento,
                    tipoOperacao.PermitirCargaSemAverbacao,
                    tipoOperacao.ExigeChaveVendaAntesConfirmarNotas,
                    tipoOperacao.GerarNovoNumeroCargaNoRedespacho,
                    tipoOperacao.PossuiIntegracaoA52,
                    tipoOperacao.TempoEntregaA52,
                    tipoOperacao.TipoA52,
                    tipoOperacao.TipoCargaA52,
                    tipoOperacao.TipoOperacaoA52,
                    IntegrarPedidosA52 = tipoOperacao?.IntegrarPedidoA52 ?? false,
                    tipoOperacao.ValidarLicencaVeiculo,
                    tipoOperacao.ValidarLicencaVeiculoPorCarga,
                    tipoOperacao.PermitirAvancarEtapaComLicencaInvalida,
                    tipoOperacao.SolicitarNotasFiscaisAoSalvarDadosTransportador,
                    tipoOperacao.PermitirValorFreteInformadoPeloEmbarcadorZerado,
                    tipoOperacao.PermitirImportarCTeComChaveNFeDiferenteNoPreCTe,
                    tipoOperacao.SelecionarRetiradaProduto,
                    tipoOperacao.BloquearAlteracaoHorarioCarregamentoCarga,
                    tipoOperacao.ImprimirRelatorioRomaneioEtapaImpressaoCarga,
                    tipoOperacao.NaoExibirDetalhesDoFretePortalTransportador,
                    tipoOperacao.EmailAgendamentoColeta,
                    tipoOperacao.BloquearMontagemCargaSemNotaFiscal,
                    tipoOperacao.PermiteInformarRecebedorAgendamento,
                    tipoOperacao.PermitidoSelecionarTipoDeOperacaoNaMontagemDaCarga,
                    tipoOperacao.PermiteInformarPesoCubadoNaMontagemDaCarga,
                    tipoOperacao.NaoExigeRoteirizacaoMontagemCarga,
                    tipoOperacao.PermiteAdicionarPedidoCargaFechada,
                    tipoOperacao.NaoDisponibilizarCargaParaIntegracaoERP,
                    tipoOperacao.NaoPermiteAvancarCargaSemDataPrevisaoDeEntrega,
                    tipoOperacao.GerarPedidoAoReceberCarga,
                    tipoOperacao.PermitirEnviarImagemParaMultiplosCanhotos,
                    tipoOperacao.PermitirInformarDataEntregaParaMultiplosCanhotos,
                    tipoOperacao.PermiteAlterarDataInicioCarregamentoNoControleEntrega,
                    tipoOperacao.PermiteAlterarHorarioCarregamentoCargasFaturadas,
                    tipoOperacao.AlterarDataJanelaCarregamentoAoAtualizarDataAgendamentoEntregaPedido,
                    tipoOperacao.BloquearLiberacaoCargasComPedidosDatasAgendadasDivergentes,
                    tipoOperacao.GerarLoteEntregaAutomaticamente,
                    tipoOperacao.PedidoColetaEntrega,
                    tipoOperacao.UtilizarRecebedorPedidoParaSVM,
                    tipoOperacao.EnviarComprovantesDaCarga,
                    tipoOperacao.MonitorarRetornoCargaBuonny,
                    tipoOperacao.NaoPermitirGerarComissaoMotorista,
                    tipoOperacao.NaoPermitirEmitirCargaComMesmoNumeroOutroDocumento,
                    tipoOperacao.PermitirDataInicioViagemAnteriorDataCarregamento,
                    tipoOperacao.NaoAguardarImportacaoDoCTeParaAvancar,
                    tipoOperacao.NaoRealizarIntegracaoComAX,
                    tipoOperacao.RealizarIntegracaoComMicDta,
                    tipoOperacao.IntegrarMICDTAComSiscomex,
                    tipoOperacao.PermitirConsultaDeValoresPedagioSemParar,
                    ConsultaDeValoresPedagioAdicionarComponenteFrete = tipoOperacao.ConsultaDeValoresPedagioAdicionarComponenteFrete ?? (tipoOperacao.PermitirConsultaDeValoresPedagioSemParar ? (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS ? false : true) : false),
                    tipoOperacao.PermitirAgendarDescargaAposDataEntregaSugerida,
                    tipoOperacao.NaoExigirQueEntregasSejamAgendadasComCliente,
                    tipoOperacao.PermitirAlterarVolumesNaCarga,
                    tipoOperacao.PossibilitarInicioViagemViaGuarita,
                    tipoOperacao.BloquearAvancoCargaVolumesZerados,
                    tipoOperacao.AtualizarRotaRealizadaDoMonitoramento,
                    tipoOperacao.NaoGerarMonitoramento,
                    tipoOperacao.NaoUtilizarRecebedorDaNotaFiscal,
                    tipoOperacao.ExigirTermoAceite,
                    tipoOperacao.TermoAceite,
                    tipoOperacao.OperacaoInsumos,
                    tipoOperacao.InformarDadosNotaCte,
                    tipoOperacao.ManterOrdemAoRoteirizarAgendaEntrega,
                    tipoOperacao.HabilitarTipoPagamentoValePedagio,
                    tipoOperacao.TipoPagamentoValePedagio,
                    tipoOperacao.QuandoProcessarMonitoramento,
                    tipoOperacao.ExigeSenhaConfirmacaoEntrega,
                    tipoOperacao.NumeroTentativaSenhaConfirmacaoEntrega,
                    tipoOperacao.EnviarCTesPorWebService,
                    tipoOperacao.EnviarSeguroAverbacaoPorWebService,
                    tipoOperacao.ReceberCTesAverbacaoPorWebService,
                    tipoOperacao.OperacaoDestinadaCTeComplementar,
                    tipoOperacao.PermitirInformarRecebedorMontagemCarga,
                    tipoOperacao.OperacaoTransferenciaContainer,
                    tipoOperacao.NaoPermitirFinalizarEntregaRejeitada,
                    tipoOperacao.IniciarMonitoramentoAutomaticamenteDataCarregamento,
                    tipoOperacao.PermitirAlterarDataChegadaVeiculo,
                    tipoOperacao.EnviarPesoLiquidoLinkNotas,
                    TipoOperacaoPadraoParaFretesForaDoPais = tipoOperacao.UsarComoPadraoParaFretesForaDoPais,
                    TipoOperacaoPadraoParaFretesDentroDoPais = tipoOperacao.UsarComoPadraoParaFretesDentroDoPais,
                    tipoOperacao.TipoConsolidacao,
                    TipoOperacaoPrecheckin = new
                    {
                        Codigo = tipoOperacao.TipoOperacaoPrecheckin?.Codigo ?? 0,
                        Descricao = tipoOperacao.TipoOperacaoPrecheckin?.Descricao ?? string.Empty
                    },
                    TipoOperacaoPrecheckinTransferencia = new
                    {
                        Codigo = tipoOperacao.TipoOperacaoPrecheckinTransferencia?.Codigo ?? 0,
                        Descricao = tipoOperacao.TipoOperacaoPrecheckinTransferencia?.Descricao ?? string.Empty
                    },
                    tipoOperacao.ModalCarga,
                    tipoOperacao.PrazoSolicitacaoOcorrencia,
                    tipoOperacao.NaoUtilizarConfiguracoesDeComprovantesDoGrupoPessoa,

                    Pessoa = new
                    {
                        Codigo = tipoOperacao.Pessoa?.CPF_CNPJ ?? 0D,
                        Descricao = tipoOperacao.Pessoa?.Nome ?? string.Empty
                    },
                    Expedidor = new
                    {
                        Codigo = tipoOperacao.Expedidor?.CPF_CNPJ ?? 0D,
                        Descricao = tipoOperacao.Expedidor?.Nome ?? string.Empty
                    },
                    Recebedor = new
                    {
                        Codigo = tipoOperacao.Recebedor?.CPF_CNPJ ?? 0D,
                        Descricao = tipoOperacao.Recebedor?.Nome ?? string.Empty
                    },
                    PagamentoMotoristaTipo = new
                    {
                        Codigo = tipoOperacao.PagamentoMotoristaTipo?.Codigo ?? 0,
                        Descricao = tipoOperacao.PagamentoMotoristaTipo?.Descricao ?? string.Empty
                    },
                    GrupoPessoas = new
                    {
                        Codigo = tipoOperacao.GrupoPessoas?.Codigo ?? 0,
                        Descricao = tipoOperacao.GrupoPessoas?.Descricao ?? string.Empty
                    },
                    TipoOperacaoRedespacho = new
                    {
                        Codigo = tipoOperacao.TipoOperacaoRedespacho?.Codigo ?? 0,
                        Descricao = tipoOperacao.TipoOperacaoRedespacho?.Descricao ?? string.Empty
                    },
                    ConfiguracaoLayoutEDI = ObterLayoutEDI(tipoOperacao),
                    ConfiguracaoEmissaoCTe = ObterConfiguracaoEmissaoCTe(tipoOperacao, unidadeDeTrabalho),
                    Integracoes = ObterIntegracoes(tipoOperacao),
                    ConfiguracaoCIOT = new
                    {
                        Codigo = tipoOperacao.ConfiguracaoCIOT?.Codigo ?? 0,
                        Descricao = tipoOperacao.ConfiguracaoCIOT?.Descricao ?? string.Empty
                    },
                    GrupoTipoOperacao = new
                    {
                        Codigo = tipoOperacao.GrupoTipoOperacao?.Codigo ?? 0D,
                        Descricao = tipoOperacao.GrupoTipoOperacao?.Descricao ?? string.Empty
                    },
                    ListaTransportador = ObterTransportadores(tipoOperacao, unidadeDeTrabalho, duplicar),
                    ListaGatilhoGeracaoAutomaticaPedidoOcorrenciaColetaEntrega = ObterGatilhoGeracaoAutomaticaPedidoOcorrenciaColetaEntrega(tipoOperacao, unidadeDeTrabalho, duplicar),
                    Produtos = ObterProdutosPadroes(tipoOperacao, unidadeDeTrabalho, duplicar),
                    TiposOcorrencia = ObterTiposOcorrencia(tipoOperacao, unidadeDeTrabalho, duplicar),
                    GrupoTomadoresBloqueados = ObterGrupoTomadoresBloqueados(tipoOperacao, unidadeDeTrabalho, duplicar),
                    CheckListDesembarque = new
                    {
                        Codigo = tipoOperacao.CheckListDesembarque?.Codigo ?? 0D,
                        Descricao = tipoOperacao.CheckListDesembarque?.Descricao ?? string.Empty
                    },
                    CheckListEntrega = new
                    {
                        Codigo = tipoOperacao.CheckListEntrega?.Codigo ?? 0D,
                        Descricao = tipoOperacao.CheckListEntrega?.Descricao ?? string.Empty
                    },
                    CheckListColeta = new
                    {
                        Codigo = tipoOperacao.CheckListColeta?.Codigo ?? 0D,
                        Descricao = tipoOperacao.CheckListColeta?.Descricao ?? string.Empty
                    },
                    ExcecoesAngelLira = ObterExcecoesAngelLira(tipoOperacao, unidadeDeTrabalho, duplicar),
                    ControleEntrega = ObterControleEntrega(tipoOperacao),
                    CalculoFrete = ObterCalculoFrete(tipoOperacao),
                    Mobile = ObterMobile(tipoOperacao),
                    ConfiguracaoCIOTPamcard = ObterConfiguracaoCIOTPamcard(tipoOperacao, unidadeDeTrabalho),
                    EmissaoDocumentos = ObterEmissaoDocumentos(tipoOperacao),
                    ConfiguracaoFatura = ObterConfiguracaoFatura(tipoOperacao, unidadeDeTrabalho),
                    FreeTime = ObterFreeTime(tipoOperacao),
                    ConfiguracaoCanhoto = ObterConfiguracaoCanhoto(tipoOperacao),
                    Integracao = ObterIntegracao(tipoOperacao),
                    CIOT = ObterCIOT(tipoOperacao),
                    MultiEmbarcador = ObterMultiEmbarcador(tipoOperacao),
                    ConfiguracaoCarga = ObterConfiguracaoCarga(tipoOperacao, unidadeDeTrabalho),
                    Paletes = ObterPaletes(tipoOperacao),
                    ConfiguracaoTerceiro = ObterConfiguracaoTerceiro(tipoOperacao),
                    ConfiguracaoAgendamentoColetaEntrega = ObterConfiguracaoAgendamentoColetaEntrega(tipoOperacao),
                    ConfiguracaoImpressao = ObterConfiguracaoImpressao(tipoOperacao),
                    ConfiguracaoTransportador = ObterConfiguracaoTransportador(tipoOperacao),
                    ConfiguracaoDocumentoEmissao = ObterConfiguracaoDocumentoEmissao(tipoOperacao),
                    ConfiguracaoControleEntrega = ObterConfiguracaoControleEntrega(tipoOperacao, unidadeDeTrabalho),
                    ConfiguracaoAtendimento = ObterConfiguracaoChamado(tipoOperacao, unidadeDeTrabalho),
                    ConfiguracaoMotivosChamados = ObterConfiguracaoChamadoMotivosChamados(tipoOperacao, unidadeDeTrabalho),
                    ConfiguracaoChamadoTransportador = ObterConfiguracaoChamadoTransportador(tipoOperacao, unidadeDeTrabalho),
                    ConfiguracaoLicenca = ObterConfiguracaoLicenca(tipoOperacao),
                    ConfiguracaoJanelaCarregamento = ObterConfiguracaoJanelaCarregamento(tipoOperacao),
                    ConfiguracaoMontagem = ObterConfiguracaomontagemCarga(tipoOperacao),
                    ConfiguracaoGestaoDevolucao = ObterConfiguracaoGestaoDevolucao(tipoOperacao),
                    ConfiguracaoTipoPropriedadeVeiculo = ObterConfiguracaoTipoPropriedadeVeiculo(tipoOperacao),
                    ConfiguracaoCotacaoPedido = ObterConfiguracaoCotacaoPedido(tipoOperacao),
                    Comprovantes = ObterTiposComprovantes(tipoOperacao),
                    ConfiguracaoIntegracaoDiageo = ObterConfiguracaoIntegracaoDieageo(tipoOperacao),
                    ConfiguracaoIntegracaoTransSat = ObterConfiguracaoIntegracaoTransSat(tipoOperacao),
                    ConfiguracaoPagamentos = ObterConfiguracaoPagamentos(tipoOperacao),
                    ConfiguracaoIntercab = ObterConfiguracaoIntercab(tipoOperacao),
                    ConfiguracaoEMP = ObterConfiguracaoEMP(tipoOperacao),
                    ConfiguracaoTrizy = ObterConfiguracaoTrizy(tipoOperacao, listaConfiguracaoTipoOperacaoTrizyInformacaoAdicionalRelatorio),
                    ConfiguracaoPedido = ObterConfiguracaoPedido(tipoOperacao),
                    ConfiguracaoPesoConsideradoCarga = ObterConfiguracaoPesoConsideradoCarga(tipoOperacao, unidadeDeTrabalho),
                    ConfiguracaoContainer = ObterConfiguracaoContainer(tipoOperacao),
                    TiposCargas = tipoOperacaoTiposCargas.Select(obj => new
                    {
                        Codigo = obj.TipoCarga.Codigo,
                        Descricao = obj.TipoCarga.Descricao,
                    }).ToList(),

                    ListVendedores = tipoOperacaoVendedores.Select(obj => new
                    {
                        obj.Codigo,
                        Funcionario = new { Codigo = obj.Funcionario.Codigo, Descricao = obj.Funcionario.Nome },
                        PercentualComissao = obj.PercentualComissao.ToString("n5"),
                        DataInicioVigencia = obj.DataInicioVigencia?.ToString("dd/MM/yyyy") ?? string.Empty,
                        DataFimVigencia = obj.DataFimVigencia?.ToString("dd/MM/yyyy") ?? string.Empty,
                    }).ToList(),

                    TiposCargasEmissao = tipoOperacaoTiposCargasEmissao.Select(obj => new
                    {
                        Codigo = obj.TipoCarga.Codigo,
                        Descricao = obj.TipoCarga.Descricao,
                    }).ToList(),
                    TipoRetornoCarga = new
                    {
                        Codigo = tipoOperacao.TipoRetornoCarga?.Codigo ?? 0D,
                        Descricao = tipoOperacao.TipoRetornoCarga?.Descricao ?? string.Empty
                    },
                    Anexos = (
                        from obj in anexos
                        select new
                        {
                            obj.Codigo,
                            obj.Descricao,
                            obj.NomeArquivo,
                        }
                    ).ToList(),
                    NotificacoesAppTrizy = ObterNotificacoesAppTrizy(listaNotificacoes, duplicar),
                    EventosSuperApp = ObterEventosSuperApp(listaEventosSuperApp, duplicar),
                    tipoOperacao.HabilitarOutraConfiguracaoOpenTech,
                    tipoOperacao.UsuarioOpenTech,
                    tipoOperacao.SenhaOpenTech,
                    tipoOperacao.DominioOpenTech,
                    tipoOperacao.CodigoClienteOpenTech,
                    tipoOperacao.CodigoPASOpenTech,
                    tipoOperacao.URLOpenTech,
                    tipoOperacao.CodigoProdutoPadraoOpentech,
                    tipoOperacao.CodigoTransportadorOpenTech,
                    tipoOperacao.ConsiderarTomadorPedido,
                    FiliaisMotoristasGenericos = ObterTipoOperacaoFilialMotoristaGenerico(tipoOperacao, unidadeDeTrabalho),
                    tipoOperacao.ConfiguracaoCarga?.RemarkSped,
                    tipoOperacao.HabilitarAppTrizy,
                    CodigosIntegracao = await ObterCodigosIntegracaoAsync(tipoOperacao.Codigo, unidadeDeTrabalho, cancellationToken),
                };

                return new JsonpResult(retorno);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Pedidos.TipoOperacao.OcorreuUmaFalhaAoBuscarPorCodigo);
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unidadeDeTrabalho.Start();

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = repTipoOperacao.BuscarPorCodigo(codigo, true);

                if (tipoOperacao == null)
                    return new JsonpResult(false, true, Localization.Resources.Pedidos.TipoOperacao.NaoFoiPossivelEncontrarRegistro);

                repTipoOperacao.Deletar(tipoOperacao, Auditado);

                unidadeDeTrabalho.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();
                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, true, Localization.Resources.Pedidos.TipoOperacao.NaoFoiPossivelExcluirRegistroPoisMesmoJaPossuiVinculoComOutrosRecursosDoSistemaRecomendamosQueVoceInativeRegistroCasoNaoDesejaMaisUtilizaLo);
                else
                {
                    Servicos.Log.TratarErro(ex);
                    return new JsonpResult(false, Localization.Resources.Pedidos.TipoOperacao.OcorreuUmaFalhaAoExcluir);
                }
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaSeguros()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia Repositorios
                Repositorio.Embarcador.Pedidos.TipoOperacaoApoliceSeguro repTipoOperacaoApoliceSeguro = new Repositorio.Embarcador.Pedidos.TipoOperacaoApoliceSeguro(unitOfWork);

                // Manipula grids
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();

                // Cabecalhos grid
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Pedidos.TipoOperacao.Apolice, "Apolice", 40, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Pedidos.TipoOperacao.Desconto, "Desconto", 40, Models.Grid.Align.left, true);

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                if (propOrdenar == "Apolice") propOrdenar = "ApoliceSeguro.NumeroApolice";

                // Dados do filtro
                int tipoOperacao = 0;
                int.TryParse(Request.Params("TipoOperacao"), out tipoOperacao);

                // Consulta
                List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoApoliceSeguro> listaGrid = repTipoOperacaoApoliceSeguro.Consultar(tipoOperacao, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                int totalRegistros = repTipoOperacaoApoliceSeguro.ContarConsulta(tipoOperacao);

                var lista = from obj in listaGrid
                            select new
                            {
                                Codigo = obj.Codigo,
                                Apolice = obj.ApoliceSeguro.DescricaoComSeguradora,
                                Desconto = obj.Desconto?.ToString("n4") ?? "-",
                            };

                // Seta valores na grid
                grid.AdicionaRows(lista.ToList());
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AdicionarSeguro()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Inicia transacao
                unitOfWork.Start();

                // Instancia repositorios
                Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
                Repositorio.Embarcador.Seguros.ApoliceSeguro repApoliceSeguro = new Repositorio.Embarcador.Seguros.ApoliceSeguro(unitOfWork);
                Repositorio.Embarcador.Pedidos.TipoOperacaoApoliceSeguro repTipoOperacaoApoliceSeguro = new Repositorio.Embarcador.Pedidos.TipoOperacaoApoliceSeguro(unitOfWork);

                // Converte parametros
                int codigoTipoOperacao = 0;
                int.TryParse(Request.Params("TipoOperacao"), out codigoTipoOperacao);

                int codigoApoliceSeguro = 0;
                int.TryParse(Request.Params("Apolice"), out codigoApoliceSeguro);

                decimal? desconto = null;
                decimal descontoAux = 0m;
                if (decimal.TryParse(Request.Params("Desconto"), out descontoAux))
                    desconto = descontoAux;

                // Busca informacoes
                Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro apoliceSeguro = repApoliceSeguro.BuscarPorCodigo(codigoApoliceSeguro);
                Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = repTipoOperacao.BuscarPorCodigo(codigoTipoOperacao);

                // Valida 
                if (tipoOperacao == null)
                    return new JsonpResult(false, true, Localization.Resources.Pedidos.TipoOperacao.NaoFoiPossivelEncontrarTipoDeOperacao);

                if (apoliceSeguro == null)
                    return new JsonpResult(false, true, Localization.Resources.Pedidos.TipoOperacao.NaoFoiPossivelEncontrarApoliceDeSeguro);

                if (repTipoOperacaoApoliceSeguro.TipoOperacaoApoliceSeguroJaExiste(codigoTipoOperacao, codigoApoliceSeguro))
                    return new JsonpResult(false, true, Localization.Resources.Pedidos.TipoOperacao.JaExisteUmaApoliceDeSeguroParaEsseTipoDeOperacao);

                // Vincula dados
                Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoApoliceSeguro tipoOperacaoSeguro = new Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoApoliceSeguro
                {
                    ApoliceSeguro = apoliceSeguro,
                    TipoOperacao = tipoOperacao,
                    Desconto = desconto
                };

                // Persiste dados
                repTipoOperacaoApoliceSeguro.Inserir(tipoOperacaoSeguro, Auditado);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, tipoOperacaoSeguro.TipoOperacao, null, Localization.Resources.Pedidos.TipoOperacao.AdionouApolice + tipoOperacaoSeguro.Descricao + ".", unitOfWork);
                unitOfWork.CommitChanges();

                // Retorna sucesso
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Pedidos.TipoOperacao.OcorreuUmaFalhaAoAdicionarDados);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ExcluirSeguroPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Inicia transacao
                unitOfWork.Start();

                // Instancia repositorios
                Repositorio.Embarcador.Pedidos.TipoOperacaoApoliceSeguro repTipoOperacaoApoliceSeguro = new Repositorio.Embarcador.Pedidos.TipoOperacaoApoliceSeguro(unitOfWork);

                // Parametros
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoApoliceSeguro tipoOperacaoSeguro = repTipoOperacaoApoliceSeguro.BuscarPorCodigo(codigo);

                // Valida
                if (tipoOperacaoSeguro == null)
                    return new JsonpResult(false, true, Localization.Resources.Pedidos.TipoOperacao.NaoFoiPossivelEncontrarRegistro);

                // Persiste dados
                repTipoOperacaoApoliceSeguro.Deletar(tipoOperacaoSeguro, Auditado);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, tipoOperacaoSeguro.TipoOperacao, null, Localization.Resources.Pedidos.TipoOperacao.RemoveuApolice + tipoOperacaoSeguro.Descricao + ".", unitOfWork);
                unitOfWork.CommitChanges();

                // Retorna informacoes
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Pedidos.TipoOperacao.OcorreuUmaFalhaAoRemoverDados);
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
                Models.Grid.Grid grid = ObterGridPesquisa();

                byte[] arquivoBinario = grid.GerarExcel();

                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", $"{grid.tituloExportacao}.{grid.extensaoCSV}");

                return new JsonpResult(false, Localization.Resources.Pedidos.TipoOperacao.OcorreuUmaFalhaAoGerarArquivo);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, Localization.Resources.Pedidos.TipoOperacao.OcorreuUmaFalhaAoExportar);
            }
        }

        public async Task<IActionResult> BuscarIntegracoes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {

                Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);

                List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> tipoIntegracaos = await repTipoIntegracao.BuscarTodosAsync();

                var retornoIntegracoes = (
                        from obj in tipoIntegracaos
                        select new
                        {
                            obj.Codigo,
                            obj.Tipo,
                            Descricao = obj.Tipo.ObterDescricao(),
                        }
                    ).ToList();
                return new JsonpResult(new
                {
                    Integracoes = retornoIntegracoes
                });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Pedidos.TipoOperacao.OcorreuUmaFalhaAoBuscarIntegracoes);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterConfiguracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {

                Repositorio.Embarcador.Configuracoes.IntegracaoIntercab repConfiguracaoIntercab = new Repositorio.Embarcador.Configuracoes.IntegracaoIntercab(unitOfWork);

                return new JsonpResult(new
                {
                    TemIntegracaoIntercab = repConfiguracaoIntercab.PossuiIntegracaoIntercab(),
                }); ;
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, false, Localization.Resources.Transportadores.Transportador.OcorreuUmaFalhaAoObterAsConfiguracoesDeIntegracao);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterTodos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaTipoOperacao filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);

                Repositorio.Embarcador.Pedidos.TipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);

                List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> tiposOperacoes = repositorioTipoOperacao.Consultar(filtrosPesquisa, null);

                var retorno = (from obj in tiposOperacoes select new { obj.Codigo, obj.Descricao }).ToList();

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ObterConfiguracoesIntegracoes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Configuracoes.IntegracaoUnilever repositorioConfiguracaoIntegracaoUnilever = new Repositorio.Embarcador.Configuracoes.IntegracaoUnilever(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoUnilever configuracaoIntegracaoUnilever = repositorioConfiguracaoIntegracaoUnilever.Buscar();

                var retorno = new
                {
                    IntegracaoUnilever = configuracaoIntegracaoUnilever?.IntegrarAvancoParaEmissao ?? false
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> VerificarSeExiste()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Logistica.CentroDistribuicao repositorioCentroDistribuicao = new Repositorio.Embarcador.Logistica.CentroDistribuicao(unitOfWork);
                Repositorio.Embarcador.Configuracoes.IntegracaoUnilever repositorioIntegracaoUnilever = new Repositorio.Embarcador.Configuracoes.IntegracaoUnilever(unitOfWork);
                Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoUnilever integracaoUnilever = repositorioIntegracaoUnilever.BuscarPrimeiroRegistro();

                var retorno = new
                {
                    CentroDistribuicao = repositorioCentroDistribuicao.VerificarExiste(),
                    ExibirPreCalculo = integracaoUnilever != null ? integracaoUnilever.IntegrarValorPreCalculo : false,
                    ExibirTipoOperacaoModalFerroviario = repositorioEmpresa.ExisteTransportadorFerroviario()
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AvancarCargasComDocumentosVinculadosPorTipoOperacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoTipoOpeacao = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido repositorioConfiguracaoPedido = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = repConfiguracaoGeralCarga.BuscarPrimeiroRegistro();
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarPrimeiroRegistro();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido = repositorioConfiguracaoPedido.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = repTipoOperacao.BuscarPorCodigo(codigoTipoOpeacao);

                if (!configuracaoGeralCarga.PermitirAvancarCargasEmitidasEmbarcadorPorTipoOperacao || !tipoOperacao.CTeEmitidoNoEmbarcador)
                    return new JsonpResult(false, true, Localization.Resources.Pedidos.TipoOperacao.NaoPermitidoAvancarCargas);

                int quantidadeCargasAvancadas = 0;
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = repCarga.BuscarCargasPorTipoOperacaoComCTeEmitidoEmbarcador(codigoTipoOpeacao);

                if (cargas.Count <= 0)
                    return new JsonpResult(false, true, Localization.Resources.Pedidos.TipoOperacao.NenhumaCargaDisponivelParaAvancar);

                foreach (Dominio.Entidades.Embarcador.Cargas.Carga carga in cargas)
                {
                    unitOfWork.Start();

                    bool cargaDeRetorno = carga.CargaCTes.Count > 0 && carga.CargaCTes.All(o => o.CTe.TipoCTE == Dominio.Enumeradores.TipoCTE.Complemento);

                    string mensagemErro = Servicos.Embarcador.Carga.CargaImportacaoEmbarcador.AvancarEtapaCargaAutomaticamente(carga, carga.Pedidos.FirstOrDefault(), cargaDeRetorno, false, configuracaoTMS, TipoServicoMultisoftware, unitOfWork, configuracaoPedido);

                    if (string.IsNullOrWhiteSpace(mensagemErro))
                    {
                        quantidadeCargasAvancadas++;
                        unitOfWork.CommitChanges();
                    }
                }

                return new JsonpResult(true, true, string.Format(Localization.Resources.Pedidos.TipoOperacao.ForamAvancadasCargas, quantidadeCargasAvancadas));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return new JsonpResult(false, Localization.Resources.Pedidos.TipoOperacao.OcorreuFalhaAvancarCargas);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ObterConfiguracaoTMSGeral()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {

                Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral repConfiguracaoGeral = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeral configuracaoGeral = repConfiguracaoGeral.BuscarConfiguracaoPadrao();

                var dynConfiguracaoTMSGeral = new
                {
                    HabilitarEnvioPorSMSDeDocumentos = configuracaoGeral?.HabilitarEnvioPorSMSDeDocumentos ?? false,
                };

                return new JsonpResult(dynConfiguracaoTMSGeral);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao obter as Configurações Gerais do Sistema.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ObterConfiguracaoTipoOperacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Pedidos.CamposObrigatorios.TipoOperacaoValorPadrao repConfiguracaoTipoOperacaoValorPadrao = new Repositorio.Embarcador.Pedidos.CamposObrigatorios.TipoOperacaoValorPadrao(unitOfWork);

                List<Dominio.Entidades.Embarcador.Pedidos.CamposObrigatorios.TipoOperacaoValorPadrao> configuracaoTipoOperacaoValorPadrao = repConfiguracaoTipoOperacaoValorPadrao.ConsultarTodos();
                var resultado = configuracaoTipoOperacaoValorPadrao.Select(item => new
                {
                    Codigo = item.Codigo,
                    Descricao = item.Campo.Descricao,
                    Habilitar = item.Habilitar
                }).ToList();

                return new JsonpResult(resultado);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao obter as Configurações Tipo Operacao Valor Padrao.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
        #endregion

        #region Métodos Privados

        private dynamic ObterMultiEmbarcador(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao)
        {
            return new
            {
                UtilizaMultiEmbarcador = tipoOperacao.GrupoPessoas?.UtilizaMultiEmbarcador ?? false,
                HabilitarIntegracaoMultiEmbarcador = tipoOperacao.HabilitarIntegracaoMultiEmbarcador ?? false,
                IntegrarCIOTMultiEmbarcador = tipoOperacao.IntegrarCIOTMultiEmbarcador ?? false,
                IntegrarCargasMultiEmbarcador = tipoOperacao.IntegrarCargasMultiEmbarcador ?? false,
                URLIntegracaoMultiEmbarcador = tipoOperacao.URLIntegracaoMultiEmbarcador ?? string.Empty,
                TokenIntegracaoMultiEmbarcador = tipoOperacao.TokenIntegracaoMultiEmbarcador ?? string.Empty,
                DataInicialCargasMultiEmbarcador = tipoOperacao.DataInicialCargasMultiEmbarcador?.ToString("dd/MM/yyyy") ?? string.Empty,
                NaoImportarCargasComplementaresMultiEmbarcador = tipoOperacao.NaoImportarCargasComplementaresMultiEmbarcador ?? false,
                NaoGerarCargaMultiEmbarcador = tipoOperacao.NaoGerarCargaMultiEmbarcador ?? false,
                NaoIntegrarCancelamentoMultiEmbarcadorComDadosInvalidos = tipoOperacao.NaoIntegrarCancelamentoMultiEmbarcadorComDadosInvalidos ?? false,
                VincularDocumentosAutomaticamenteEmCargaExistenteMultiEmbarcador = tipoOperacao.VincularDocumentosAutomaticamenteEmCargaExistenteMultiEmbarcador ?? false,
                UtilizarGeracaoDeNFSeAvancada = tipoOperacao.UtilizarGeracaoDeNFSeAvancada ?? false,
                ExpressaoRegularNumeroPedidoObservacaoCTeMultiEmbarcador = tipoOperacao.ExpressaoRegularNumeroPedidoObservacaoCTeMultiEmbarcador ?? string.Empty,
                PermiteUtilizarEmContratoFrete = tipoOperacao.PermiteUtilizarEmContratoFrete ?? false
            };
        }

        private dynamic ObterCIOT(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao)
        {
            return new
            {
                TipoOperadoraCIOT = tipoOperacao.ConfiguracaoCIOT?.OperadoraCIOT,
            };
        }

        private dynamic ObterIntegracao(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao)
        {
            return new
            {
                IntegracaoUtilizarTipoIntegracaoGrupoPessoas = tipoOperacao?.ConfiguracaoIntegracao?.UtilizarTipoIntegracaoGrupoPessoas ?? false,
                AtivarRegraCancelamentoDosPedidosMichelin = tipoOperacao?.ConfiguracaoIntegracao?.AtivarRegraCancelamentoDosPedidosMichelin ?? false,
                HorasParaCalculoCancelamento = tipoOperacao?.ConfiguracaoIntegracao?.HorasParaCalculoCancelamento ?? 0,
                HabilitarIntegracaoAvancoParaEmissao = tipoOperacao?.ConfiguracaoIntegracao?.HabilitarIntegracaoAvancoParaEmissao ?? false,
                CalcularGerarGNRE = tipoOperacao?.ConfiguracaoIntegracao?.CalcularGerarGNRE ?? false,
                IntegrarCargasGeradasMultiEmbarcador = tipoOperacao?.ConfiguracaoIntegracao?.IntegrarCargasGeradasMultiEmbarcador ?? false,
                EnviarApenasPrimeiroPedidoNaOpentech = tipoOperacao?.ConfiguracaoIntegracao?.EnviarApenasPrimeiroPedidoNaOpentech ?? false,
                EnviarInformacoesTotaisDaCargaNaOpentech = tipoOperacao?.ConfiguracaoIntegracao?.EnviarInformacoesTotaisDaCargaNaOpentech ?? false,
                ValidarSomenteVeiculoMotoristaOpentech = tipoOperacao?.ConfiguracaoIntegracao?.ValidarSomenteVeiculoMotoristaOpentech ?? false,
                DefinirParaNaoMonitorarRetornoIntegracaoBounny = tipoOperacao?.ConfiguracaoIntegracao?.DefinirParaNaoMonitorarRetornoIntegracaoBounny ?? false,
                ManterIntegracoesReferenciandoCargaDeOrigemDoRedespacho = tipoOperacao?.ConfiguracaoIntegracao?.ManterIntegracoesReferenciandoCargaDeOrigemDoRedespacho ?? false,
                IntegrarDadosTransporte = tipoOperacao?.ConfiguracaoIntegracao?.IntegrarDadosTransporte ?? false,
                IntegrarDocumentos = tipoOperacao?.ConfiguracaoIntegracao?.IntegrarDocumentos ?? false,
                GerarIntegracaoKlios = tipoOperacao?.ConfiguracaoIntegracao?.GerarIntegracaoKlios ?? false,
                EnviarTagsIntegracaoMarfrigComTomadorServico = tipoOperacao?.ConfiguracaoIntegracao?.EnviarTagsIntegracaoMarfrigComTomadorServico ?? false,
                PossuiTempoEnvioIntegracaoDocumentosCarga = tipoOperacao?.ConfiguracaoIntegracao?.PossuiTempoEnvioIntegracaoDocumentosCarga ?? false,
                ConsultarTaxasKMM = tipoOperacao?.ConfiguracaoIntegracao?.ConsultarTaxasKMM ?? false,
                NaoGerarIntegracaoRetornoConfirmacaoColeta = tipoOperacao?.ConfiguracaoIntegracao?.NaoGerarIntegracaoRetornoConfirmacaoColeta ?? false,
                NaoIntegrarEtapa1Opentech = tipoOperacao?.ConfiguracaoIntegracao?.NaoIntegrarEtapa1Opentech ?? false,
                TiposTerceiros = tipoOperacao?.ConfiguracaoIntegracao?.TiposTerceiros?.Select(t => new
                {
                    t.Codigo,
                    t.Descricao,
                    t.DescricaoSituacao
                }).ToList() ?? null,
            };
        }

        private dynamic ObterConfiguracaoCarga(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            List<Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoCargaEstado> configuracoesEstaduais = new List<Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoCargaEstado>();
            if (tipoOperacao?.ConfiguracaoCarga != null)
            {
                Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoCargaEstado repositorioConfiguracaoTipoOperacaoCargaEstado = new Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoCargaEstado(unidadeDeTrabalho);
                configuracoesEstaduais = repositorioConfiguracaoTipoOperacaoCargaEstado.BuscarPorConfiguracao(tipoOperacao.ConfiguracaoCarga.Codigo);
            }
            return new
            {
                tipoOperacao.EmailDisponibilidadeCarga,
                tipoOperacao.ExigirVeiculoComRastreador,
                tipoOperacao.CargaTipoConsolidacao,
                tipoOperacao.TipoOperacaoMercosul,
                tipoOperacao.CalcularPautaFiscal,
                tipoOperacao.NaoPermitirAvancarCargaSemRegraICMS,
                tipoOperacao.ObrigatorioVincularContainerCarga,
                tipoOperacao.ObrigatorioRealizarConferenciaContainerCarga,
                tipoOperacao.ValidarSeCargaPossuiVinculoComPreCarga,
                tipoOperacao.ObrigarInformarRICnaColetaDeConteiner,
                tipoOperacao.ExigirCargaRoteirizada,
                tipoOperacao.NaoCriarAprovacaoCargaConfirmarDocumento,
                NaoPermitirInformarMotoristaComCNHVencida = tipoOperacao?.ConfiguracaoCarga?.NaoPermitirInformarMotoristaComCNHVencida ?? false,
                AdicionarBLComoOutroDocumentoAutomaticamenteNaCarga = tipoOperacao?.ConfiguracaoCarga?.AdicionarBLComoOutroDocumentoAutomaticamenteNaCarga ?? false,
                PermiteAdicionarAnexosGuarita = tipoOperacao?.ConfiguracaoCarga?.PermiteAdicionarAnexosGuarita ?? false,
                MultiplicarQuantidadeProdutoPorCaixaPelaQuantidadeCaixa = tipoOperacao?.ConfiguracaoCarga?.MultiplicarQuantidadeProdutoPorCaixaPelaQuantidadeCaixa ?? false,
                EnviarEmailAoGerarCargaPlanejamentoPedidosDetalhado = tipoOperacao?.ConfiguracaoCarga?.EnviarEmailAoGerarCargaPlanejamentoPedidosDetalhado ?? false,
                PermitirVisualizarOrdenarAsZonasDeTransporte = tipoOperacao?.ConfiguracaoCarga?.PermitirVisualizarOrdenarAsZonasDeTransporte ?? false,
                PermitirAdicionarObservacaoNaEtapaUmDaCarga = tipoOperacao?.ConfiguracaoCarga?.PermitirAdicionarObservacaoNaEtapaUmDaCarga ?? false,
                ExigeInformarIscaNaCargaComValorMaiorQue = tipoOperacao?.ConfiguracaoCarga?.ExigeInformarIscaNaCargaComValorMaiorQue.ToString("n2") ?? "0",
                ValorLimiteNaCarga = tipoOperacao?.ConfiguracaoCarga?.ValorLimiteNaCarga.ToString("n2") ?? "0",
                ExibirOperadorInsercaoCargaNoPortalTransportador = tipoOperacao?.ConfiguracaoCarga?.ExibirOperadorInsercaoCargaNoPortalTransportador ?? false,
                ExibirNumeroPedidoNosDetalhesDaCarga = tipoOperacao?.ConfiguracaoCarga?.ExibirNumeroPedidoNosDetalhesDaCarga ?? false,
                ExibirFiltroDePedidosEtapaNotaFiscal = tipoOperacao?.ConfiguracaoCarga?.ExibirFiltroDePedidosEtapaNotaFiscal ?? false,
                PermitirAlterarDataRetornoCDCarga = tipoOperacao?.ConfiguracaoCarga?.PermitirAlterarDataRetornoCDCarga ?? false,
                IgnorarRateioConfiguradoPorto = tipoOperacao?.ConfiguracaoCarga?.IgnorarRateioConfiguradoPorto ?? false,
                ConfiguracoesCargaEstado = (from obj in configuracoesEstaduais
                                            select new
                                            {
                                                obj.Codigo,
                                                CodigoEstado = obj.Estado?.Sigla ?? "",
                                                DescricaoEstado = obj.Estado?.Descricao ?? string.Empty,
                                                ExigeInformarIscaNaCargaComValorMaiorQue = obj.ExigeInformarIscaNaCargaComValorMaiorQue.ToString("n2")
                                            }).ToList(),
                TempoParaAlertarPorEmailResponsavelDaFilialDaCargaQueAindaNaoTeveOsDadosDeTransporteInformados = tipoOperacao.ConfiguracaoCarga?.TempoParaAlertarPorEmailResponsavelDaFilialDaCargaQueAindaNaoTeveOsDadosDeTransporteInformados,
                TempoParaRecebimentoDosPacotes = tipoOperacao.ConfiguracaoCarga?.TempoParaRecebimentoDosPacotes,
                PercentualToleranciaParaEmissaoEntreCTesRecebidosVersusPacotes = tipoOperacao.ConfiguracaoCarga?.PercentualToleranciaParaEmissaoEntreCTesRecebidosVersusPacotes,
                QuantidadeDiasValidacaoNFeDataCarregamento = tipoOperacao.ConfiguracaoCarga?.QuantidadeDiasValidacaoNFeDataCarregamento,
                TipoRotaCarga = tipoOperacao.ConfiguracaoCarga?.TipoRotaCarga,
                ExigeNotaFiscalTenhaTagRetirada = tipoOperacao.ConfiguracaoCarga?.ExigeNotaFiscalTenhaTagRetirada,
                tipoOperacao.ExpressaoRegularNumeroBookingObservacaoCTe,
                tipoOperacao.ExpressaoRegularNumeroContainerObservacaoCTe,
                tipoOperacao.ExpressaoBooking,
                tipoOperacao.ExpressaoContainer,
                tipoOperacao.CargaBloqueadaParaEdicaoIntegracao,
                ValidarLoteProdutoVersusLoteNotaFiscal = tipoOperacao.ConfiguracaoCarga?.ValidarLoteProdutoVersusLoteNotaFiscal ?? false,
                ExecutarCalculoRelevanciaDeCustoNFePorCFOP = tipoOperacao.ConfiguracaoCarga?.ExecutarCalculoRelevanciaDeCustoNFePorCFOP ?? false,
                AguardarRecebimentoProdutoParaProvisionar = tipoOperacao.ConfiguracaoCarga?.AguardarRecebimentoProdutoParaProvisionar ?? false,
                AlertarAlteracoesPedidoNoFluxoPatio = tipoOperacao.ConfiguracaoCarga?.AlertarAlteracoesPedidoNoFluxoPatio ?? false,
                AtivoModuloNaoConformidades = tipoOperacao.ConfiguracaoCarga?.AtivoModuloNaoConformidades ?? false,
                HerdarDadosDeTransporteCargaPrimeiroTrecho = tipoOperacao.ConfiguracaoCarga?.HerdarDadosDeTransporteCargaPrimeiroTrecho ?? false,
                ConsiderarKMRecibidoDoEmbarcador = tipoOperacao.ConfiguracaoCarga?.ConsiderarKMRecibidoDoEmbarcador ?? false,
                PermitirAdicionarNovosPedidosPorNotasAvulsas = tipoOperacao.ConfiguracaoCarga?.PermitirAdicionarNovosPedidosPorNotasAvulsas ?? false,
                GerarRetornoAutomaticoMomento = tipoOperacao.ConfiguracaoCarga?.GerarRetornoAutomaticoMomento ?? GerarRetornoAutomaticoMomento.Nenhum,
                ObrigarInformarValePedagio = tipoOperacao.ConfiguracaoCarga?.ObrigarInformarValePedagio ?? false,
                PermitirRelacionarOutrasCargas = tipoOperacao.ConfiguracaoCarga?.PermitirRelacionarOutrasCargas ?? false,
                VincularPedidoDeAcordoComNumeroOrdem = tipoOperacao.ConfiguracaoCarga?.VincularPedidoDeAcordoComNumeroOrdem ?? false,
                ConsiderarKMDaRotaFrete = tipoOperacao.ConfiguracaoCarga?.ConsiderarKMDaRotaFrete ?? false,
                DeixarPedidosDisponiveisParaMontegemCarga = tipoOperacao.ConfiguracaoCarga?.DeixarPedidosDisponiveisParaMontegemCarga ?? false,
                PrecisaEsperarNotasFilhaParaGerarPagamento = tipoOperacao.ConfiguracaoCarga?.PrecisaEsperarNotasFilhaParaGerarPagamento ?? false,
                PrecisaEsperarNotaTransferenciaParaGeraPagamento = tipoOperacao.ConfiguracaoCarga?.PrecisaEsperarNotaTransferenciaParaGeraPagamento ?? false,
                ValidarPesoDasNotasRelevantes = tipoOperacao.ConfiguracaoCarga?.ValidarPesoDasNotasRelevantes ?? false,
                GerarCargaEspelhoAoConfirmarEntrega = tipoOperacao.ConfiguracaoCarga?.GerarCargaEspelhoAoConfirmarEntrega ?? false,
                UtilizarRotaFreteInformadoPedido = tipoOperacao.ConfiguracaoCarga?.UtilizarRotaFreteInformadoPedido ?? false,
                BloquearInclusaoArquivosXMLDeNFeCarga = tipoOperacao.ConfiguracaoCarga?.BloquearInclusaoArquivosXMLDeNFeCarga ?? false,
                PermitirIntegrarPacotes = tipoOperacao.ConfiguracaoCarga?.PermitirIntegrarPacotes ?? false,
                TipoDeCancelamentoDaCarga = tipoOperacao.ConfiguracaoCarga?.TipoCancelamentoCargaDocumento ?? TipoCancelamentoCargaDocumento.Carga,
                NaoPermitirUsoNotasQueEstaoEmOutraCarga = tipoOperacao.ConfiguracaoCarga?.NaoPermitirUsoNotasQueEstaoEmOutraCarga ?? false,
                InformarLacreNosDadosTransporte = tipoOperacao.ConfiguracaoCarga?.InformarLacreNosDadosTransporte ?? false,
                TipoOperacaoCargaEspelho = new { Codigo = tipoOperacao?.ConfiguracaoCarga?.TipoOperacaoCargaEspelho?.Codigo ?? 0, Descricao = tipoOperacao?.ConfiguracaoCarga?.TipoOperacaoCargaEspelho?.Descricao ?? string.Empty },
                TipoOperacaoCargaRetornoColetasRejeitadas = new { Codigo = tipoOperacao?.ConfiguracaoCarga?.TipoOperacaoCargaRetornoColetasRejeitadas?.Codigo ?? 0, Descricao = tipoOperacao?.ConfiguracaoCarga?.TipoOperacaoCargaRetornoColetasRejeitadas?.Descricao ?? string.Empty },
                NaoPermitirEditarPesoBrutoDaNotaFiscalEletronica = tipoOperacao.ConfiguracaoCarga?.NaoPermitirEditarPesoBrutoDaNotaFiscalEletronica ?? false,
                HabilitarConsultaContainerEMP = tipoOperacao.ConfiguracaoCarga?.HabilitarConsultaContainerEMP ?? false,
                TipoDeEnvioPorSMSDeDocumentos = tipoOperacao?.ConfiguracaoCarga?.TipoDeEnvioPorSMSDeDocumentos ?? TipoDeEnvioPorSMSDeDocumentos.Nenhum,
                UtilizarDistribuidorPorRegiaoNaRegiaoDestino = tipoOperacao.ConfiguracaoCarga?.UtilizarDistribuidorPorRegiaoNaRegiaoDestino ?? false,
                GerarCargaRetornoRejeitarTodasColetas = tipoOperacao.ConfiguracaoCarga?.GerarCargaRetornoRejeitarTodasColetas ?? false,
                ValidaValorPreCalculoValorFrete = tipoOperacao.ConfiguracaoCarga?.ValidaValorPreCalculoValorFrete ?? false,
                PermitirInformarAjudantesNaCarga = tipoOperacao.ConfiguracaoCarga?.PermitirInformarAjudantesNaCarga ?? false,
                ValidarValorMinimoCarga = tipoOperacao.ConfiguracaoCarga?.ValidarValorMinimoCarga ?? false,
                ValorMinimoCarga = tipoOperacao.ConfiguracaoCarga?.ValorMinimoCarga.ToString("n2") ?? "0",
                PermiteInformarTransportadorEtapaUmQuandoNotaFiscalNaoRecebidaAntesCalculoFrete = tipoOperacao.ConfiguracaoCarga?.PermiteInformarTransportadorEtapaUmQuandoNotaFiscalNaoRecebidaAntesCalculoFrete ?? false,
                AvancarCargaAutomaticamenteAoReceberIntegracaoNotasWS = tipoOperacao.ConfiguracaoCarga?.AvancarCargaAutomaticamenteAoReceberIntegracaoNotasWS ?? false,
                TipoOperacaoPreCarga = tipoOperacao.ConfiguracaoCarga?.TipoOperacaoPreCarga ?? false,
                PermitirSelecionarPreCargaNaCarga = tipoOperacao.ConfiguracaoCarga?.PermitirSelecionarPreCargaNaCarga ?? false,
                obrigatorioInformarAliquotaImpostoSuspensoeValor = tipoOperacao.ConfiguracaoCarga?.ObrigatorioInformarAliquotaImpostoSuspensoeValor ?? false,
                LiberarCargaSemPlanejamento = tipoOperacao.ConfiguracaoCarga?.LiberarCargaSemPlanejamento ?? false,
                HorasManterCargaNaEtapaUmAteXHorasAntesDaDataDeCarregamento = tipoOperacao.ConfiguracaoCarga?.HorasManterCargaNaEtapaUmAteXHorasAntesDaDataDeCarregamento ?? 0,
                ManterCargaNaEtapaUmAteXHorasAntesDaDataDeCarregamento = tipoOperacao.ConfiguracaoCarga?.ManterCargaNaEtapaUmAteXHorasAntesDaDataDeCarregamento ?? false,
                NaoAvancarEtapaSePlacaEstiverEmMonitoramento = tipoOperacao.ConfiguracaoCarga?.NaoAvancarEtapaSePlacaEstiverEmMonitoramento ?? false,
                IncrementaCodigoPorTipoOperacao = tipoOperacao.ConfiguracaoCarga?.IncrementaCodigoPorTipoOperacao ?? false,
                AdicionaPrefixoCodigoCarga = tipoOperacao.ConfiguracaoCarga?.AdicionaPrefixoCodigoCarga ?? string.Empty,
                ReceberAtualizacaoDeTransporteEmQualquerSituacaoCarga = tipoOperacao.ConfiguracaoCarga?.ReceberAtualizacaoDeTransporteEmQualquerSituacaoCarga ?? false,
                UtilizarDirecionamentoCustoExtra = tipoOperacao.ConfiguracaoCarga?.UtilizarDirecionamentoCustoExtra ?? false,
                DirecionamentoCustoExtra = tipoOperacao.ConfiguracaoCarga?.DirecionamentoCustoExtra ?? TipoDirecionamentoCustoExtra.Nenhum,
                ObrigatorioJustificarCustoExtra = tipoOperacao.ConfiguracaoCarga?.ObrigatorioJustificarCustoExtra ?? false,
                UtilizaIntegracaoOKColeta = tipoOperacao.ConfiguracaoCarga?.UtilizaIntegracaoOKColeta ?? false,
                BuscarDocumentosEAverbacaoPelaOSMae = tipoOperacao.ConfiguracaoCarga?.BuscarDocumentosEAverbacaoPelaOSMae ?? false,
                AcordoFaturamento = tipoOperacao.ConfiguracaoCarga?.AcordoFaturamento ?? TipoAcordoFaturamento.NaoInformado,
                TipoDocumentoProvedor = tipoOperacao.ConfiguracaoCarga?.DocumentoProvedor ?? TipoDocumentoProvedor.Nenhum,
                GerarRedespachoAutomaticamenteAposEmissaoDocumentos = tipoOperacao.ConfiguracaoCarga?.GerarRedespachoAutomaticamenteAposEmissaoDocumentos ?? false,
                InformarTransportadorSubcontratadoEtapaUm = tipoOperacao.ConfiguracaoCarga?.InformarTransportadorSubcontratadoEtapaUm ?? false,
                GerarCargaEspelhoAutomaticamenteAoFinalizarCarga = tipoOperacao.ConfiguracaoCarga?.GerarCargaEspelhoAutomaticamenteAoFinalizarCarga ?? false,
                NecessitaInformarPlacaCarregamento = tipoOperacao.ConfiguracaoCarga?.NecessitaInformarPlacaCarregamento ?? false,
                DisponibilizarNotaFiscalNoPedidoAoFinalizarCarga = tipoOperacao.ConfiguracaoCarga?.DisponibilizarNotaFiscalNoPedidoAoFinalizarCarga ?? false,
                HabilitarVinculoMotoristaGenericoCarga = tipoOperacao.ConfiguracaoCarga?.HabilitarVinculoMotoristaGenericoCarga ?? false,
                AvancarCargaQuandoPedidoZeroPacotes = tipoOperacao.ConfiguracaoCarga?.AvancarCargaQuandoPedidoZeroPacotes ?? false,
                LiberarPedidoComRecebedorParaMontagemCarga = tipoOperacao.ConfiguracaoCarga?.LiberarPedidoComRecebedorParaMontagemCarga ?? false,
                BuscarPacoteMesmoCNPJAdicionarCargaAposConsulta = tipoOperacao.ConfiguracaoCarga?.BuscarPacoteMesmoCNPJAdicionarCargaAposConsulta ?? false,
                GerarMDFeParaRecebedorDaCarga = tipoOperacao.ConfiguracaoCarga?.GerarMDFeParaRecebedorDaCarga ?? false,
                TipoOperacaoInternacional = tipoOperacao.ConfiguracaoCarga?.TipoOperacaoInternacional ?? false,
                LayoutEmailTipoPropostaTipoOperacao = tipoOperacao.ConfiguracaoCarga?.LayoutEmailTipoPropostaTipoOperacao ?? false,
                NaoPermitirAvancarCargaComTracaoSemReboque = tipoOperacao.ConfiguracaoCarga?.NaoPermitirAvancarCargaComTracaoSemReboque ?? false,
                RoteirizarCargaEtapaNotaFiscal = tipoOperacao.ConfiguracaoCarga?.RoteirizarCargaEtapaNotaFiscal ?? false,
                GerarCargaDeRedespachoVinculandoApenasUmaNotaPorEntrega = tipoOperacao.ConfiguracaoCarga?.GerarCargaDeRedespachoVinculandoApenasUmaNotaPorEntrega ?? false,
                PermitirIncluirCTesDeDiferentesUFsEmMDFeUnico = tipoOperacao.ConfiguracaoCarga?.PermitirIncluirCTesDeDiferentesUFsEmMDFeUnico ?? false,
                PermiteInformarMotoristasSomenteEtapaUmQuandoNotasFiscaisNaoSaoRecebidasAntesCalcularFrete = tipoOperacao.ConfiguracaoCarga?.PermiteInformarMotoristasSomenteEtapaUmQuandoNotasFiscaisNaoSaoRecebidasAntesCalcularFrete ?? false,
            };
        }

        private dynamic ObterConfiguracaoImpressao(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao)
        {
            return new
            {
                tipoOperacao.NaoNecessarioConfirmarImpressaoDocumentos,
                tipoOperacao.PermiteRealizarImpressaoCarga,
                tipoOperacao.ImprimirCRT,
                tipoOperacao.PermitirTransportadorInformarObservacaoImpressaoCarga,
                tipoOperacao.UtilizarPlanoViagem,
                tipoOperacao.EnviarEmailPlanoViagemSolicitarNotasCarga,
                tipoOperacao.EnviarEmailPlanoViagemFinalizarCarga,
                PermiteBaixarComprovanteColeta = tipoOperacao.ConfiguracaoImpressao?.PermiteBaixarComprovanteColeta ?? false,
                EnviarEmailPlanoViagemTransportador = tipoOperacao.ConfiguracaoImpressao?.EnviarPlanoViagemTransportador ?? false,
                OcultarQuantidadeValoresOrdemColeta = tipoOperacao.ConfiguracaoImpressao?.OcultarQuantidadeValoresOrdemColeta ?? false,
                ImprimirMinuta = tipoOperacao.ConfiguracaoImpressao?.ImprimirMinuta ?? false,
                AlterarLayoutDaFaturaIncluirTipoServico = tipoOperacao.ConfiguracaoImpressao?.AlterarLayoutDaFaturaIncluirTipoServico ?? false
            };
        }

        private dynamic ObterConfiguracaoPagamentos(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao)
        {
            return new
            {
                TipoLiberacaoPagamento = tipoOperacao.ConfiguracaoPagamentos?.TipoLiberacaoPagamento ?? TipoLiberacaoPagamento.Nenhum,
                CentroDeResultado = new { Codigo = tipoOperacao.ConfiguracaoPagamentos?.CentroResultado?.Codigo ?? 0, Descricao = tipoOperacao.ConfiguracaoPagamentos?.CentroResultado?.Descricao ?? "" },
            };
        }

        private dynamic ObterConfiguracaoPedido(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao)
        {
            return new
            {
                BloquearInclusaoAlteracaoPedidosNaoTenhamTabelaFreteConfigurada = tipoOperacao.ConfiguracaoPedido?.BloquearInclusaoAlteracaoPedidosNaoTenhamTabelaFreteConfigurada ?? false,
                FiltrarPedidosPorRemetenteRetiradaProduto = tipoOperacao.ConfiguracaoPedido?.FiltrarPedidosPorRemetenteRetiradaProduto ?? false,
                EnviarPedidoReentregaAutomaticamenteRoteirizar = tipoOperacao.ConfiguracaoPedido?.EnviarPedidoReentregaAutomaticamenteRoteirizar ?? false
            };
        }

        private dynamic ObterConfiguracaoIntercab(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao)
        {
            return new
            {
                Tomador = tipoOperacao?.ConfiguracaoIntercab?.Tomador ?? TipoTomadorCabotagem.Outros,
                ModalProposta = tipoOperacao?.ConfiguracaoIntercab?.ModalProposta ?? TipoModalPropostaCabotagem.Outros,
                TipoProposta = tipoOperacao?.ConfiguracaoIntercab?.TipoProposta ?? TipoPropostaCabotagem.Outros
            };
        }

        private dynamic ObterConfiguracaoEMP(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao)
        {
            return new
            {
                AtivarIntegracaoComSIL = tipoOperacao?.ConfiguracaoEMP?.AtivarIntegracaoComSIL ?? false,
            };
        }

        private dynamic ObterConfiguracaoTrizy(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, List<Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoTrizyInformacaoAdicionalRelatorio> listaConfiguracaoTipoOperacaoTrizyInformacaoAdicionalRelatorio)
        {
            ICollection<InformacoesAdicionaisEntregaTrizy> informacoesAdicionaisEntrega = null;
            if (tipoOperacao != null && tipoOperacao.ConfiguracaoTrizy != null && tipoOperacao.ConfiguracaoTrizy.InformacoesAdicionaisEntrega != null)
                informacoesAdicionaisEntrega = tipoOperacao.ConfiguracaoTrizy.InformacoesAdicionaisEntrega;

            ICollection<EnumContatosInformacoesEntregaTrizy> contatosInformacoesEntrega = null;
            if (tipoOperacao != null && tipoOperacao.ConfiguracaoTrizy != null && tipoOperacao.ConfiguracaoTrizy.ContatosInformacoesEntrega != null)
                contatosInformacoesEntrega = tipoOperacao.ConfiguracaoTrizy.ContatosInformacoesEntrega;

            var informacoesAdicionaisRelatorio = listaConfiguracaoTipoOperacaoTrizyInformacaoAdicionalRelatorio
                                    .Select(x => new
                                    {
                                        Rotulo = x.Rotulo,
                                        Descricao = x.Descricao,
                                        Codigo = x.Codigo
                                    })
                                    .ToList();

            return new
            {
                EnviarEstouIndoColeta = tipoOperacao?.ConfiguracaoTrizy?.EnviarEstouIndoColeta ?? true,
                EnviarEstouIndoEntrega = tipoOperacao?.ConfiguracaoTrizy?.EnviarEstouIndoEntrega ?? true,
                SolicitarComprovanteColetaEntrega = tipoOperacao?.ConfiguracaoTrizy?.SolicitarComprovanteColetaEntrega ?? true,
                EnviarPrimeiraColetaComoOrigemNoLugarDoRemetente = tipoOperacao?.ConfiguracaoTrizy?.EnviarPrimeiraColetaComoOrigemNoLugarDoRemetente ?? false,
                HabilitarChat = tipoOperacao?.ConfiguracaoTrizy?.HabilitarChat ?? true,
                EnviarInicioViagemColeta = tipoOperacao?.ConfiguracaoTrizy?.EnviarInicioViagemColeta ?? true,
                EnviarInicioViagemEntrega = tipoOperacao?.ConfiguracaoTrizy?.EnviarInicioViagemEntrega ?? true,
                EnviarMensagemAlertaPreTrip = tipoOperacao?.ConfiguracaoTrizy?.EnviarMensagemAlertaPreTrip ?? false,
                EnviarPreTripJuntoAoNumeroCarga = tipoOperacao?.ConfiguracaoTrizy?.EnviarPreTripJuntoAoNumeroCarga ?? false,
                EnviarIniciarViagemColetaPreTrip = tipoOperacao?.ConfiguracaoTrizy?.EnviarIniciarViagemColetaPreTrip ?? false,
                EnviarIniciarViagemEntregaPreTrip = tipoOperacao?.ConfiguracaoTrizy?.EnviarIniciarViagemEntregaPreTrip ?? false,
                EnviarEstouIndoColetaPreTrip = tipoOperacao?.ConfiguracaoTrizy?.EnviarEstouIndoColetaPreTrip ?? false,
                EnviarEstouIndoEntregaPreTrip = tipoOperacao?.ConfiguracaoTrizy?.EnviarEstouIndoEntregaPreTrip ?? false,
                EnviarPrimeiraColetaComoOrigemNoLugarDoRemetentePreTrip = tipoOperacao?.ConfiguracaoTrizy?.EnviarPrimeiraColetaComoOrigemNoLugarDoRemetentePreTrip ?? false,
                EnviarChegueiParaCarregar = tipoOperacao?.ConfiguracaoTrizy?.EnviarChegueiParaCarregar ?? false,
                EnviarChegueiParaDescarregar = tipoOperacao?.ConfiguracaoTrizy?.EnviarChegueiParaDescarregar ?? false,
                EnviarChegueiParaCarregarPreTrip = tipoOperacao?.ConfiguracaoTrizy?.EnviarChegueiParaCarregarPreTrip ?? false,
                EnviarChegueiParaDescarregarPreTrip = tipoOperacao?.ConfiguracaoTrizy?.EnviarChegueiParaDescarregarPreTrip ?? false,
                NaoFinalizarPreTrip = tipoOperacao?.ConfiguracaoTrizy?.NaoFinalizarPreTrip ?? false,
                ExigirEnvioFotosDasNotasNaOrigemPreTrip = tipoOperacao?.ConfiguracaoTrizy?.ExigirEnvioFotosDasNotasNaOrigemPreTrip ?? false,
                SolicitarComprovanteEntregaSemOCRPreTrip = tipoOperacao?.ConfiguracaoTrizy?.SolicitarComprovanteEntregaSemOCRPreTrip ?? false,
                SolicitarDataeHoraDoCanhoto = tipoOperacao?.ConfiguracaoTrizy?.SolicitarDataeHoraDoCanhoto ?? false,
                NaoEnviarEventosViagemColetaEntregaSolicitarApenasCanhotos = tipoOperacao?.ConfiguracaoTrizy?.NaoEnviarEventosViagemColetaEntregaSolicitarApenasCanhotos ?? false,
                NaoEnviarEventosNaOrigem = tipoOperacao?.ConfiguracaoTrizy?.NaoEnviarEventosNaOrigem ?? false,
                VincularDataEHoraSolicitadaNoCanhoto = tipoOperacao?.ConfiguracaoTrizy?.VincularDataEHoraSolicitadaNoCanhoto ?? false,
                NaoEnviarDocumentosFiscais = tipoOperacao?.ConfiguracaoTrizy?.NaoEnviarDocumentosFiscais ?? false,
                NaoEnviarTagValidacao = tipoOperacao?.ConfiguracaoTrizy?.NaoEnviarTagValidacao ?? false,
                SolicitarAssinaturaNaConfirmacaoDeColetaEntrega = tipoOperacao?.ConfiguracaoTrizy?.SolicitarAssinaturaNaConfirmacaoDeColetaEntrega ?? false,
                TituloInformacaoAdicional = tipoOperacao?.ConfiguracaoTrizy?.TituloInformacaoAdicional ?? string.Empty,
                SolicitarFotoComoEvidenciaObrigatoria = tipoOperacao?.ConfiguracaoTrizy?.SolicitarFotoComoEvidenciaObrigatoria ?? false,
                SolicitarFotoComoEvidenciaOpcional = tipoOperacao?.ConfiguracaoTrizy?.SolicitarFotoComoEvidenciaOpcional ?? false,
                EnviarEventoIniciarViagemComoOpcional = tipoOperacao?.ConfiguracaoTrizy?.EnviarEventoIniciarViagemComoOpcional ?? false,
                EnviarEventoIniciarViagemComoOpcionalPreTrip = tipoOperacao?.ConfiguracaoTrizy?.EnviarEventoIniciarViagemComoOpcionalPreTrip ?? false,
                DataEsperadaParaColetas = tipoOperacao?.ConfiguracaoTrizy?.DataEsperadaParaColetas ?? DataEsperadaColetaEntregaTrizy.DataPrevisao,
                DataEsperadaParaEntregas = tipoOperacao?.ConfiguracaoTrizy?.DataEsperadaParaEntregas ?? DataEsperadaColetaEntregaTrizy.DataPrevisao,
                EnviarInformacoesAdicionaisEntrega = tipoOperacao?.ConfiguracaoTrizy?.EnviarInformacoesAdicionaisEntrega ?? false,
                InformacoesAdicionaisEntrega = informacoesAdicionaisEntrega != null ? informacoesAdicionaisEntrega.Select(e => (int)e).ToArray() : new int[0],
                EnviarQuantidadeDeCaixasNoLugarDoPesoDosProdutos = tipoOperacao?.ConfiguracaoTrizy?.EnviarQuantidadeDeCaixasNoLugarDoPesoDosProdutos ?? false,
                EnviarDadosEmpresaGR = tipoOperacao?.ConfiguracaoTrizy?.EnviarDadosEmpresaGR ?? false,
                CNPJEmpresaGR = tipoOperacao?.ConfiguracaoTrizy?.CNPJEmpresaGR ?? "",
                DescricaoEmpresaGR = tipoOperacao?.ConfiguracaoTrizy?.DescricaoEmpresaGR ?? "",
                SolicitarNomeRecebedorNaConfirmacaoDeColetaEntrega = tipoOperacao?.ConfiguracaoTrizy?.SolicitarNomeRecebedorNaConfirmacaoDeColetaEntrega ?? false,
                SolicitarDocumentoNaConfirmacaoDeColetaEntrega = tipoOperacao?.ConfiguracaoTrizy?.SolicitarDocumentoNaConfirmacaoDeColetaEntrega ?? false,
                IdentificarNotaDeMercadoriaENotaDePallet = tipoOperacao?.ConfiguracaoTrizy?.IdentificarNotaDeMercadoriaENotaDePallet ?? false,
                ContatosInformacoesEntrega = contatosInformacoesEntrega != null ? contatosInformacoesEntrega.Select(e => (int)e).ToArray() : new int[0],
                EnviarContatoInformacoesEntrega = tipoOperacao?.ConfiguracaoTrizy?.EnviarContatoInformacoesEntrega ?? false,
                ConverterCanhotoParaPretoeBrancoERotacionarAutomaticamente = tipoOperacao?.ConfiguracaoTrizy?.ConverterCanhotoParaPretoeBrancoERotacionarAutomaticamente ?? false,
                EnviarMascaraFixaParaoCanhoto = tipoOperacao?.ConfiguracaoTrizy?.EnviarMascaraFixaParaoCanhoto ?? false,
                EnviarMascaraDinamicaParaoCanhoto = tipoOperacao?.ConfiguracaoTrizy?.EnviarMascaraDinamicaParaoCanhoto ?? false,
                NaoPermitirVincularFotosDaGaleriaParaCanhotos = tipoOperacao?.ConfiguracaoTrizy?.NaoPermitirVincularFotosDaGaleriaParaCanhotos ?? false,
                HabilitarEnvioRelatorio = tipoOperacao?.ConfiguracaoTrizy?.HabilitarEnvioRelatorio ?? false,
                TituloRelatorioViagem = tipoOperacao.ConfiguracaoTrizy?.TituloRelatorioViagem ?? "",
                TituloReciboViagem = tipoOperacao.ConfiguracaoTrizy?.TituloReciboViagem ?? "",
                InformacoesAdicionaisRelatorio = informacoesAdicionaisRelatorio,
                NaoEnviarPolilinha = tipoOperacao.ConfiguracaoTrizy?.NaoEnviarPolilinha ?? false,
                HabilitarDevolucao = tipoOperacao.ConfiguracaoTrizy?.HabilitarDevolucao ?? false,
                HabilitarDevolucaoParcial = tipoOperacao.ConfiguracaoTrizy?.HabilitarDevolucaoParcial ?? false,
                VersaoIntegracaoTrizy = tipoOperacao.ConfiguracaoTrizy?.VersaoIntegracao ?? VersaoIntegracaoTrizy.Versao1
            };
        }

        private dynamic ObterConfiguracaoDocumentoEmissao(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao)
        {
            return new
            {
                NaoAlterarTomadorCargaPedidoImportacaoCTeSubcontratacao = tipoOperacao.ConfiguracaoDocumentoEmissao?.NaoAlterarTomadorCargaPedidoImportacaoCTeSubcontratacao ?? false,
                ImportarCTeSempreComoSubcontratacao = tipoOperacao.ConfiguracaoDocumentoEmissao?.ImportarCTeSempreComoSubcontratacao ?? false,
                PossuiNotaOrdemVenda = tipoOperacao.ConfiguracaoDocumentoEmissao?.PossuiNotaOrdemVenda ?? false,
                UtilizaNotaVendaObjetoCTE = tipoOperacao.ConfiguracaoDocumentoEmissao?.UtilizaNotaVendaObjetoCTE ?? false,
                MinutosAvancarParaEmissaoseInformadosDadosTransporte = tipoOperacao.ConfiguracaoDocumentoEmissao?.MinutosAvancarParaEmissaoseInformadosDadosTransporte ?? 0,
                NaoUtilizaNotaVendaObjetoCTE = tipoOperacao.ConfiguracaoDocumentoEmissao?.NaoUtilizaNotaVendaObjetoCTE ?? false,
                EmitirCTENotaRemessa = tipoOperacao.ConfiguracaoDocumentoEmissao?.EmitirCTENotaRemessa ?? false,
                DesconsiderarNotaPalletEmissaoCTE = tipoOperacao.ConfiguracaoDocumentoEmissao?.DesconsiderarNotaPalletEmissaoCTE ?? false,
            };
        }

        private dynamic ObterConfiguracaoTransportador(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao)
        {
            return new
            {
                tipoOperacao.OcultarCargasComEsseTipoOperacaoNoPortalTransportador,
                tipoOperacao.PermitirTransportadorEnviarNotasFiscais,
                PermitirTransportadorSolicitarNotasFiscais = tipoOperacao?.ConfiguracaoTransportador?.PermitirTransportadorSolicitarNotasFiscais ?? false,
                PermitirEnvioImagemMultiplosCanhotos = tipoOperacao?.ConfiguracaoTransportador?.PermitirEnvioImagemMultiplosCanhotos ?? false,
                PermitirTransportadorAjusteCargaSegundoTrecho = tipoOperacao?.ConfiguracaoTransportador?.PermitirTransportadorAjusteCargaSegundoTrecho ?? false,
                TipoOperacaoPadraoFerroviario = new { Codigo = tipoOperacao?.ConfiguracaoTransportador?.TipoOperacaoModalFerroviario?.Codigo ?? 0, Descricao = tipoOperacao?.ConfiguracaoTransportador?.TipoOperacaoModalFerroviario?.Descricao ?? string.Empty },
                PermitirRetornarEtapa = tipoOperacao?.ConfiguracaoTransportador?.PermitirRetornarEtapa ?? false,
                BloquearTransportadorNaoIMOAptoCargasPerigosas = tipoOperacao?.ConfiguracaoTransportador?.BloquearTransportadorNaoIMOAptoCargasPerigosas ?? false,
                AlertarTransportadorNaoIMOCargasPerigosas = tipoOperacao?.ConfiguracaoTransportador?.AlertarTransportadorNaoIMOCargasPerigosas ?? false,
                BloquearVeiculoSemEspelhamento = tipoOperacao?.ConfiguracaoTransportador?.BloquearVeiculoSemEspelhamento ?? false,
                BloquearVeiculoSemEspelhamentoJanela = tipoOperacao?.ConfiguracaoTransportador?.BloquearVeiculoSemEspelhamentoJanela ?? false,
            };
        }

        private dynamic ObterFreeTime(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao)
        {
            return new
            {
                TipoFreeTime = tipoOperacao?.ConfiguracaoFreeTime?.TipoFreeTime ?? TipoFreeTime.PorParada,
                TempoColetas = tipoOperacao?.ConfiguracaoFreeTime?.TempoColetas ?? 0,
                TempoFronteiras = tipoOperacao?.ConfiguracaoFreeTime?.TempoFronteiras ?? 0,
                TempoEntregas = tipoOperacao?.ConfiguracaoFreeTime?.TempoEntregas ?? 0,
                TempoTotalViagem = tipoOperacao?.ConfiguracaoFreeTime?.TempoTotalViagem ?? 0
            };
        }

        private dynamic ObterConfiguracaoCanhoto(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao)
        {
            return new
            {
                NotificarCanhotosPendentes = tipoOperacao?.ConfiguracaoCanhoto?.NotificarCanhotosPendentes ?? false,
                NotificarCanhotosPendentesDiariamente = tipoOperacao?.ConfiguracaoCanhoto?.NotificarCanhotosPendentesDiariamente ?? false,
                NotificarCanhotosRejeitadosDiariamente = tipoOperacao?.ConfiguracaoCanhoto?.NotificarCanhotosRejeitadosDiariamente ?? false,
                PrazoAposDataEmissaoCanhoto = tipoOperacao?.ConfiguracaoCanhoto?.PrazoAposDataEmissaoCanhoto ?? 0,
                DiaSemanaNotificarCanhotosPendentes = tipoOperacao?.ConfiguracaoCanhoto?.DiaSemanaNotificarCanhotosPendentes,
                NotificarCanhotosRejeitados = tipoOperacao?.ConfiguracaoCanhoto?.NotificarCanhotosRejeitados ?? false,
                NaoPermiteUploadDeCanhotosComCTeNaoAutorizado = tipoOperacao?.ConfiguracaoCanhoto?.NaoPermiteUploadDeCanhotosComCTeNaoAutorizado ?? false,
                NaoGerarCanhotoAvulsoEmCargasComAoMenosUmRecebedor = tipoOperacao?.ConfiguracaoCanhoto?.NaoGerarCanhotoAvulsoEmCargasComAoMenosUmRecebedor ?? false
            };
        }

        private dynamic ObterConfiguracaoTerceiro(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao)
        {
            return new
            {
                NaoSomarValorPedagioContratoFrete = tipoOperacao?.ConfiguracaoTerceiro?.NaoSomarValorPedagioContratoFrete ?? false,
                NaoSubtrairValePedagioDoContrato = tipoOperacao?.ConfiguracaoTerceiro?.NaoSubtrairValePedagioDoContrato ?? false,
                NaoUtilizarOperadoraConfiguradaNoTransportadorTerceiro = tipoOperacao?.ConfiguracaoTerceiro?.NaoUtilizarOperadoraConfiguradaNoTransportadorTerceiro ?? false,
                AdicionarValorContratoFreteComoAcrescimoDescontoProximoContrato = tipoOperacao?.ConfiguracaoTerceiro?.AdicionarValorContratoFreteComoAcrescimoDescontoProximoContrato ?? false,
                JustificativaAdicionarValorContratoFreteComoAcrescimoDescontoProximoContrato = new
                {
                    Codigo = tipoOperacao?.ConfiguracaoTerceiro?.JustificativaAdicionarValorContratoFreteComoAcrescimoDescontoProximoContrato?.Codigo ?? 0,
                    Descricao = tipoOperacao?.ConfiguracaoTerceiro?.JustificativaAdicionarValorContratoFreteComoAcrescimoDescontoProximoContrato?.Descricao ?? string.Empty
                }
            };
        }

        private dynamic ObterConfiguracaoAgendamentoColetaEntrega(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao)
        {
            return new
            {
                UtilizarDataSaidaGuaritaComoTerminoCarregamento = tipoOperacao.ConfiguracaoAgendamentoColetaEntrega?.UtilizarDataSaidaGuaritaComoTerminoCarregamento ?? false,
                ObrigarInformarCTePortalFornecedor = tipoOperacao.ConfiguracaoAgendamentoColetaEntrega?.ObrigarInformarCTePortalFornecedor ?? false,
                ExigirNumeroIsisReturnParaAgendarEntrega = tipoOperacao.ConfiguracaoAgendamentoColetaEntrega?.ExigirNumeroIsisReturnParaAgendarEntrega ?? false,
                RemoverEtapaAgendamentoDoAgendamentoColeta = tipoOperacao.ConfiguracaoAgendamentoColetaEntrega?.RemoverEtapaAgendamentoDoAgendamentoColeta ?? false,
                NaoObrigarInformarModeloVeicularAgendamento = tipoOperacao.ConfiguracaoAgendamentoColetaEntrega?.NaoObrigarInformarModeloVeicularAgendamento ?? false,
                NaoObrigarInformarTransportadorAgendamento = tipoOperacao.ConfiguracaoAgendamentoColetaEntrega?.NaoObrigarInformarTransportadorAgendamento ?? false,
                AvancarEtapaNFeCargaAoConfirmarAgendamentoRetira = tipoOperacao.ConfiguracaoAgendamentoColetaEntrega?.AvancarEtapaNFeCargaAoConfirmarAgendamentoRetira ?? false,
                ExigirQueCDDestinoSejaInformadoAgendamento = tipoOperacao.ConfiguracaoAgendamentoColetaEntrega?.ExigirQueCDDestinoSejaInformadoAgendamento ?? false,
                EnviarEmailAoClienteComLinkDeAgendamentoQuandoGerarACarga = tipoOperacao.ConfiguracaoAgendamentoColetaEntrega?.EnviarEmailAoCLienteComLinkDeAgendamentoQuandoGerarCarga ?? false,
                ConsiderarDataEntregaComoInicioDoFluxoPatio = tipoOperacao.ConfiguracaoAgendamentoColetaEntrega?.ConsiderarDataEntregaComoInicioDoFluxoPatio ?? false,
            };
        }

        private dynamic ObterEmissaoDocumentos(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao)
        {
            return new
            {
                EmissaoDocumentoFinalizarCargaAutomaticamente = tipoOperacao?.ConfiguracaoEmissaoDocumento?.FinalizarCargaAutomaticamente ?? false,
                EmissaoDocumentoUtilizarExpedidorRecebedorPedidoSubcontratacao = tipoOperacao?.ConfiguracaoEmissaoDocumento?.UtilizarExpedidorRecebedorPedidoSubcontratacao ?? false,
                EmissaoDocumentoNaoPermitirAcessarDocumentosAntesCargaEmTransporte = tipoOperacao?.ConfiguracaoEmissaoDocumento?.NaoPermitirAcessarDocumentosAntesCargaEmTransporte ?? false,
                EmissaoDocumentoDescricaoUnidadeMedidaPesoModeloVeicularRateado = tipoOperacao?.ConfiguracaoEmissaoDocumento?.DescricaoUnidadeMedidaPesoModeloVeicularRateado ?? string.Empty,
                EmissaoDocumentoRatearPesoModeloVeicularEntreCTes = tipoOperacao?.ConfiguracaoEmissaoDocumento?.RatearPesoModeloVeicularEntreCTes ?? false,
                EmissaoDocumentoObrigatorioAprovarCtesImportados = tipoOperacao?.ConfiguracaoEmissaoDocumento?.ObrigatorioAprovarCtesImportados ?? false,
                TipoConhecimentoProceda = tipoOperacao?.ConfiguracaoEmissaoDocumento?.TipoConhecimentoProceda ?? "",
                NaoPermitirLiberarSemValePedagio = tipoOperacao?.ConfiguracaoEmissaoDocumento?.NaoPermitirLiberarSemValePedagio ?? false,
                TipoDeEmitente = tipoOperacao?.ConfiguracaoEmissaoDocumento?.TipoDeEmitente,
                NaoPermitirEmissaoComMesmaOrigemEDestino = tipoOperacao?.ConfiguracaoEmissaoDocumento?.NaoPermitirEmissaoComMesmaOrigemEDestino ?? false,
                ValidarRelevanciaNotasPrechekin = tipoOperacao?.ConfiguracaoEmissaoDocumento?.ValidarRelevanciaNotasPrechekin ?? false,
                EmitirDocumentoSempreOrigemDestinoPedido = tipoOperacao?.ConfiguracaoEmissaoDocumento?.EmitirDocumentoSempreOrigemDestinoPedido ?? false,
                GerarCTeSimplificadoQuandoCompativel = tipoOperacao?.ConfiguracaoEmissaoDocumento?.GerarCTeSimplificadoQuandoCompativel ?? false,
                ClassificacaoNFeRemessaVenda = tipoOperacao?.ConfiguracaoEmissaoDocumento?.ClassificacaoNFeRemessaVenda ?? false,
                EnviarParaObservacaoCTeNFeRemessa = tipoOperacao?.ConfiguracaoEmissaoDocumento?.EnviarParaObservacaoCTeNFeRemessa ?? false,
                EnviarParaObservacaoCTeNFeVenda = tipoOperacao?.ConfiguracaoEmissaoDocumento?.EnviarParaObservacaoCTeNFeVenda ?? false,
                AverbarContainerComAverbacaoCarga = tipoOperacao?.ConfiguracaoEmissaoDocumento?.AverbarContainerComAverbacaoCarga ?? false,
                ValorContainerAverbacao = tipoOperacao?.ConfiguracaoEmissaoDocumento?.ValorContainerAverbacao ?? 0,
                UtilizarOutroEnderecoPedidoMesmoSePossuirRecebedor = tipoOperacao?.ConfiguracaoEmissaoDocumento?.UtilizarOutroEnderecoPedidoMesmoSePossuirRecebedor ?? false
            };
        }

        private dynamic ObterConfiguracaoFatura(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacaoParam, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoFatura tipoOperacao = tipoOperacaoParam?.ConfiguracaoTipoOperacaoFatura ?? new Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoFatura();

            Repositorio.Embarcador.Pedidos.TipoOperacaoFaturaVencimento repTipoOperacaoFaturaVencimento = new Repositorio.Embarcador.Pedidos.TipoOperacaoFaturaVencimento(unidadeDeTrabalho);
            List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoFaturaVencimento> tipoOperacaoFaturaVencimentos = repTipoOperacaoFaturaVencimento.BuscarPorTipoOperacao(tipoOperacaoParam.Codigo);


            return new
            {
                PermiteFinalSemana = tipoOperacao.PermiteFinalDeSemana,
                tipoOperacao.TipoPrazoFaturamento,
                tipoOperacao.FormaGeracaoTituloFatura,
                tipoOperacao.DiasDePrazoFatura,
                tipoOperacao.ExigeCanhotoFisico,
                ArmazenaCanhotoFisicoCTe = tipoOperacao.ArmazenaCanhotoFisicoCTe ?? false,
                tipoOperacao.SomenteOcorrenciasFinalizadoras,
                tipoOperacao.FaturarSomenteOcorrenciasFinalizadoras,
                tipoOperacao.NaoGerarFaturaAteReceberCanhotos,
                Banco = new
                {
                    Codigo = tipoOperacao.Banco?.Codigo ?? 0,
                    Descricao = tipoOperacao.Banco?.Descricao ?? string.Empty
                },
                tipoOperacao.Agencia,
                Digito = tipoOperacao.DigitoAgencia,
                tipoOperacao.NumeroConta,
                TipoConta = tipoOperacao.TipoContaBanco,
                TomadorFatura = new
                {
                    Codigo = tipoOperacao.ClienteTomadorFatura?.CPF_CNPJ ?? 0d,
                    Descricao = tipoOperacao.ClienteTomadorFatura?.Nome ?? string.Empty
                },
                tipoOperacao.ObservacaoFatura,
                FormaPagamento = new
                {
                    Codigo = tipoOperacao.FormaPagamento?.Codigo ?? 0,
                    Descricao = tipoOperacao.FormaPagamento?.Descricao ?? string.Empty
                },
                tipoOperacao.GerarTituloPorDocumentoFiscal,
                BoletoConfiguracao = new
                {
                    Codigo = tipoOperacao.BoletoConfiguracao?.Codigo ?? 0,
                    Descricao = tipoOperacao.BoletoConfiguracao?.Descricao ?? string.Empty
                },
                tipoOperacao.EnviarBoletoPorEmailAutomaticamente,
                tipoOperacao.EnviarDocumentacaoFaturamentoCTe,
                tipoOperacao.GerarTituloAutomaticamente,
                tipoOperacao.GerarFaturaAutomaticaCte,
                tipoOperacao.GerarFaturamentoAVista,
                tipoOperacao.AssuntoEmailFatura,
                tipoOperacao.CorpoEmailFatura,
                tipoOperacao.GerarBoletoAutomaticamente,
                tipoOperacao.EnviarArquivosDescompactados,
                tipoOperacao.NaoEnviarEmailFaturaAutomaticamente,
                tipoOperacao.TipoEnvioFatura,
                tipoOperacao.TipoAgrupamentoFatura,
                tipoOperacao.FormaTitulo,
                DiasSemanaFatura = tipoOperacao.DiasSemanaFatura?.Select(o => o).ToList() ?? new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana>(),
                DiasMesFatura = tipoOperacao.DiasMesFatura?.Select(o => o).ToList() ?? new List<int>(),
                tipoOperacao.EmailEnvioDocumentacao,
                tipoOperacao.TipoAgrupamentoEnvioDocumentacao,
                tipoOperacao.AssuntoDocumentacao,
                tipoOperacao.CorpoEmailDocumentacao,
                tipoOperacao.EmailFatura,
                tipoOperacao.HabilitarPeriodoVencimentoEspecifico,
                InformarEmailEnvioDocumentacao = !string.IsNullOrWhiteSpace(tipoOperacao.EmailEnvioDocumentacao),
                tipoOperacao.FormaEnvioDocumentacao,
                tipoOperacao.EmailEnvioDocumentacaoPorta,
                tipoOperacao.TipoAgrupamentoEnvioDocumentacaoPorta,
                tipoOperacao.AssuntoDocumentacaoPorta,
                tipoOperacao.CorpoEmailDocumentacaoPorta,
                tipoOperacao.GerarFaturamentoMultiplaParcela,
                tipoOperacao.QuantidadeParcelasFaturamento,
                tipoOperacao.AvisoVencimetoHabilitarConfiguracaoPersonalizada,
                tipoOperacao.AvisoVencimetoQunatidadeDias,
                tipoOperacao.AvisoVencimetoEnviarDiariamente,
                tipoOperacao.CobrancaHabilitarConfiguracaoPersonalizada,
                tipoOperacao.CobrancaQunatidadeDias,
                tipoOperacao.AvisoVencimetoNaoEnviarEmail,
                tipoOperacao.CobrancaNaoEnviarEmail,
                InformarEmailEnvioDocumentacaoPorta = !string.IsNullOrWhiteSpace(tipoOperacao.EmailEnvioDocumentacaoPorta),
                tipoOperacao.FormaEnvioDocumentacaoPorta,

                FaturaVencimentos = (from vencimento in tipoOperacaoFaturaVencimentos
                                     select new
                                     {
                                         vencimento.Codigo,
                                         vencimento.DiaInicial,
                                         vencimento.DiaFinal,
                                         vencimento.DiaVencimento
                                     }).ToList(),

                tipoOperacao.GerarTituloAutomaticamenteComAdiantamentoSaldo,
                tipoOperacao.NaoValidarPossuiAcordoFaturamentoAvancoCarga,
                PercentualAdiantamentoTituloAutomatico = tipoOperacao.PercentualAdiantamentoTituloAutomatico.ToString("n2") ?? string.Empty,
                PrazoAdiantamentoEmDiasTituloAutomatico = tipoOperacao.PrazoAdiantamentoEmDiasTituloAutomatico.ToString("n0") ?? string.Empty,
                PercentualSaldoTituloAutomatico = tipoOperacao.PercentualSaldoTituloAutomatico.ToString("n2") ?? string.Empty,
                PrazoSaldoEmDiasTituloAutomatico = tipoOperacao.PrazoSaldoEmDiasTituloAutomatico.ToString("n0") ?? string.Empty,
            };
        }

        private dynamic ObterConfiguracaoCIOTPamcard(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.TipoOperacaoConfiguracaoCIOTPamcard repTipoOperacaoConfiguracaoCIOTPamcard = new Repositorio.Embarcador.Pedidos.TipoOperacaoConfiguracaoCIOTPamcard(unitOfWork);
            Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoConfiguracaoCIOTPamcard tipoOperacaoConfiguracaoCIOTPamcard = tipoOperacao.ConfiguracaoCIOT?.OperadoraCIOT == OperadoraCIOT.Pamcard ? repTipoOperacaoConfiguracaoCIOTPamcard.BuscarPorTipoOperacao(tipoOperacao) : null;

            return new
            {
                UtilizarConfiguracaoPersonalizadaParcelasPamcard = tipoOperacaoConfiguracaoCIOTPamcard?.UtilizarConfiguracaoPersonalizadaParcelas ?? false,
                EfetivacaoAbastecimentoPamcard = tipoOperacaoConfiguracaoCIOTPamcard?.EfetivacaoAbastecimento,
                StatusAbastecimentoPamcard = tipoOperacaoConfiguracaoCIOTPamcard?.StatusAbastecimento,
                EfetivacaoAdiantamentoPamcard = tipoOperacaoConfiguracaoCIOTPamcard?.EfetivacaoAdiantamento,
                StatusAdiantamentoPamcard = tipoOperacaoConfiguracaoCIOTPamcard?.StatusAdiantamento,
                EfetivacaoSaldoPamcard = tipoOperacaoConfiguracaoCIOTPamcard?.EfetivacaoSaldo,
                StatusSaldoPamcard = tipoOperacaoConfiguracaoCIOTPamcard?.StatusSaldo,
            };
        }

        private dynamic ObterMobile(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao)
        {
            return new
            {
                MobilePermiteFotosEntrega = tipoOperacao.ConfiguracaoMobile?.PermiteFotosEntrega ?? false,
                MobileQuantidadeMinimasFotosEntrega = tipoOperacao.ConfiguracaoMobile?.QuantidadeMinimasFotosEntrega ?? 0,
                MobilePermiteConfirmarChegadaEntrega = tipoOperacao.ConfiguracaoMobile?.PermiteConfirmarChegadaEntrega ?? false,
                MobilePermiteConfirmarChegadaColeta = tipoOperacao.ConfiguracaoMobile?.PermiteConfirmarChegadaColeta ?? false,
                MobileControlarTempoColeta = tipoOperacao.ConfiguracaoMobile?.ControlarTempoColeta ?? false,
                MobileNaoUtilizarProdutosNaColeta = tipoOperacao.ConfiguracaoMobile?.NaoUtilizarProdutosNaColeta ?? false,
                MobilePermitirEscanearChavesNfe = tipoOperacao.ConfiguracaoMobile?.PermitirEscanearChavesNfe ?? false,
                MobileObrigarEscanearChavesNfe = tipoOperacao.ConfiguracaoMobile?.ObrigarEscanearChavesNfe ?? false,
                MobilePermitirVisualizarProgramacaoAntesViagem = tipoOperacao.ConfiguracaoMobile?.PermitirVisualizarProgramacaoAntesViagem ?? false,
                MobileExibirEntregaAntesEtapaTransporte = tipoOperacao.ConfiguracaoMobile?.ExibirEntregaAntesEtapaTransporte ?? false,
                ExibirEntregaEtapaEmissaoDocumentos = tipoOperacao.ConfiguracaoMobile?.ExibirEntregaEtapaEmissaoDocumentos ?? false,
                MobileSolicitarJustificativaRegistroForaRaio = tipoOperacao.ConfiguracaoMobile?.SolicitarJustificativaRegistroForaRaio ?? false,
                MobilePermiteEventos = tipoOperacao.ConfiguracaoMobile?.PermiteEventos ?? false,
                MobilePermiteChat = tipoOperacao.ConfiguracaoMobile?.PermiteChat ?? false,
                MobilePermiteSAC = tipoOperacao.ConfiguracaoMobile?.PermiteSAC ?? false,
                MobilePermiteConfirmarEntrega = tipoOperacao.ConfiguracaoMobile?.PermiteConfirmarEntrega ?? false,
                MobileBloquearRastreamento = tipoOperacao.ConfiguracaoMobile?.BloquearRastreamento ?? false,
                MobilePermiteCanhotoModoManual = tipoOperacao.ConfiguracaoMobile?.PermiteCanhotoModoManual ?? false,
                MobilePermiteEntregaParcial = tipoOperacao.ConfiguracaoMobile?.PermiteEntregaParcial ?? false,
                MobileControlarTempoEntrega = tipoOperacao.ConfiguracaoMobile?.ControlarTempoEntrega ?? false,
                MobileExibirRelatorio = tipoOperacao.ConfiguracaoMobile?.ExibirRelatorio ?? false,
                MobileNaoRetornarColetas = tipoOperacao.ConfiguracaoMobile?.NaoRetornarColetas ?? false,
                MobileObrigarAssinaturaProdutor = tipoOperacao.ConfiguracaoMobile?.ObrigarAssinaturaProdutor ?? false,
                MobileForcarPreenchimentoSequencial = tipoOperacao.ConfiguracaoMobile?.ForcarPreenchimentoSequencial ?? false,
                MobileObrigarFotoCanhoto = tipoOperacao.ConfiguracaoMobile?.ObrigarFotoCanhoto ?? false,
                MobileObrigarAssinatura = tipoOperacao.ConfiguracaoMobile?.ObrigarAssinaturaEntrega ?? false,
                MobileObrigarDadosRecebedor = tipoOperacao.ConfiguracaoMobile?.ObrigarDadosRecebedor ?? false,
                ExibirAvaliacaoNaAssintura = tipoOperacao.ConfiguracaoMobile?.ExibirAvaliacaoNaAssintura ?? false,
                PermiteBaixarOsDocumentosDeTransporte = tipoOperacao.ConfiguracaoMobile?.PermiteBaixarOsDocumentosDeTransporte ?? false,
                TempoLimiteConfirmacaoMotorista = tipoOperacao.ConfiguracaoMobile?.TempoLimiteConfirmacaoMotorista,
                NecessarioConfirmacaoMotorista = tipoOperacao.ConfiguracaoMobile?.NecessarioConfirmacaoMotorista ?? false,
                IniciarViagemNoControleDePatioAoIniciarViagemNoApp = tipoOperacao.ConfiguracaoMobile?.IniciarViagemNoControleDePatioAoIniciarViagemNoApp ?? false,
                SolicitarReconhecimentoFacialDoRecebedor = tipoOperacao.ConfiguracaoMobile?.SolicitarReconhecimentoFacialDoRecebedor ?? false,
                MobilePermiteFotosColeta = tipoOperacao.ConfiguracaoMobile?.PermiteFotosColeta ?? false,
                MobileQuantidadeMinimasFotosColeta = tipoOperacao.ConfiguracaoMobile?.QuantidadeMinimasFotosColeta ?? 0,
                NaoListarProdutosColetaEntrega = tipoOperacao.ConfiguracaoMobile?.NaoListarProdutosColetaEntrega ?? false,
                NaoApresentarDataInicioViagem = tipoOperacao.ConfiguracaoMobile?.NaoApresentarDataInicioViagem ?? false,
                ReplicarDataDigitalizacaoCanhotoDataEntregaCliente = tipoOperacao.ConfiguracaoMobile?.ReplicarDataDigitalizacaoCanhotoDataEntregaCliente ?? false,
            };
        }

        private dynamic ObterCalculoFrete(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao)
        {
            return new
            {
                CalculoFreteMesclarValorEmbarcadorComTabelaFrete = tipoOperacao.ConfiguracaoCalculoFrete?.MesclarValorEmbarcadorComTabelaFrete ?? false,
                CalculoFreteBloquearAjusteConfiguracoesFreteCarga = tipoOperacao.ConfiguracaoCalculoFrete?.BloquearAjusteConfiguracoesFreteCarga ?? false,
                CalculoFreteTipoCotacao = tipoOperacao.ConfiguracaoCalculoFrete?.TipoCotacao ?? TipoCotacaoFreteInternacional.CotacaoCorrente,
                CalculoFreteValorMoedaCotacao = (tipoOperacao.ConfiguracaoCalculoFrete?.ValorMoedaCotacao ?? 0m).ToString("n10"),
                CalculoFretePermiteInformarQuantidadePaletes = tipoOperacao.ConfiguracaoCalculoFrete?.PermiteInformarQuantidadePaletes ?? false,
                NaoAlterarTipoPagamentoTomadorValoresInformadosManualmente = tipoOperacao.ConfiguracaoCalculoFrete?.NaoAlterarTipoPagamentoTomadorValoresInformadosManualmente ?? false,
                CalculoFreteMesclarComponentesManuaisPedidoComTabelaFrete = tipoOperacao.ConfiguracaoCalculoFrete?.MesclarComponentesManuaisPedidoComTabelaFrete ?? false,
                ExecutarPreCalculoFrete = tipoOperacao.ConfiguracaoCalculoFrete?.ExecutarPreCalculoFrete ?? false,
                RatearValorFreteEntrePedidosAposReceberDocumentos = tipoOperacao.ConfiguracaoCalculoFrete?.RatearValorFreteEntrePedidosAposReceberDocumentos ?? false,
                CalcularFretePeloBIDPedidoOrigem = tipoOperacao.ConfiguracaoCalculoFrete?.CalcularFretePeloBIDPedidoOrigem ?? false,
                NaoIncluirValorICMSBasecalculoQuandoValorFreteInformadoOperador = tipoOperacao.ConfiguracaoCalculoFrete?.NaoIncluirValorICMSBasecalculoQuandoValorFreteInformadoOperador ?? false,
                InformarValorFreteTerceiroManualmente = tipoOperacao.ConfiguracaoCalculoFrete?.InformarValorFreteTerceiroManualmente ?? false,
                ConsiderarInclusaoIcmsCargaPedidoNaEmissaoCteComplementar = tipoOperacao.ConfiguracaoCalculoFrete?.ConsiderarInclusaoIcmsCargaPedidoNaEmissaoCteComplementar ?? false,
                PermiteEscolherDestinacaoDoComplementoDeFrete = tipoOperacao.ConfiguracaoCalculoFrete?.PermiteEscolherDestinacaoDoComplementoDeFrete ?? false,
                ValorMaximoCalculoFrete = tipoOperacao.ConfiguracaoCalculoFrete?.ValorMaximoCalculoFrete ?? 0m,
                NecessarioAguardarVinculoNotadeRemessaIndustrializador = tipoOperacao.ConfiguracaoCalculoFrete?.NecessarioAguardarVinculoNotadeRemessaIndustrializador ?? false,
                ImportarVeiculoMDFEEmbarcador = tipoOperacao.ConfiguracaoCalculoFrete?.ImportarVeiculoMDFEEmbarcador ?? false,
                ExigirComprovantesLiberacaoPagamentoContratoFrete = tipoOperacao.ConfiguracaoCalculoFrete?.ExigirComprovantesLiberacaoPagamentoContratoFrete ?? false,
                UtilizarContratoFreteCliente = tipoOperacao.ConfiguracaoCalculoFrete?.UtilizarContratoFreteCliente ?? false,
                RatearValorFreteInformadoEmbarcador = tipoOperacao.ConfiguracaoCalculoFrete?.RatearValorFreteInformadoEmbarcador ?? false,
                UtilizarCoberturaDeCarga = tipoOperacao.ConfiguracaoCalculoFrete?.UtilizarCoberturaDeCarga ?? false,
            };
        }

        private dynamic ObterControleEntrega(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao)
        {
            return new
            {
                tipoOperacao.DevolucaoProdutosPorPeso,
                tipoOperacao.PermitirTransportadorConfirmarRejeitarEntrega,
                tipoOperacao.NotificarTransportadorAoAgendarEntrega,
                tipoOperacao.PermitirAgendarEntregaSomenteAposInicioViagemCarga,
                tipoOperacao.ConsiderarApenasDiasUteisNaPrevisaoDeEntrega,
                tipoOperacao.EnviarLinkAcompanhamentoParaClienteEntrega,
                tipoOperacao.NaoAtualizarDataReprogramadaAposEntradaRaio,
                tipoOperacao.RealizarBaixaEntradaNoRaio,
                tipoOperacao.PermitirAtualizarEntregasCargasFinalizadas,
                tipoOperacao.GerarEntregaPorNotaFiscalCarga,
                tipoOperacao.PermiteEnviarNotasComplementaresAposEmissaoDocumentosTransporte,
                tipoOperacao.PermitirAdicionarPedidoReentregaAposInicioViagem,
                tipoOperacao.GerarControleColeta,
                tipoOperacao.PermiteAdicionarColeta,
                tipoOperacao.PermiteImpressaoMobile,
                tipoOperacao.PermiteRetificarMobile,
                tipoOperacao.ExibirCalculadoraMobile,
                tipoOperacao.EnviarComprovanteEntregaAposFinalizacaoEntrega,
                tipoOperacao.IniciarViagemPeloStatusViagem,
                tipoOperacao.InicioViagemPorCargaGerada,
                tipoOperacao.HabilitarCobrancaEstadiaAutomaticaPeloTracking,
                tipoOperacao.TempoMinimoCobrancaEstadia,
                tipoOperacao.GerarOcorrenciaPedidoEntregueForaPrazo,
                ComponenteFrete = new { Codigo = tipoOperacao.ComponenteFrete?.Codigo ?? 0, Descricao = tipoOperacao.ComponenteFrete?.Descricao ?? "" },
                TipoOcorrencia = new { Codigo = tipoOperacao.TipoOcorrencia?.Codigo ?? 0, Descricao = tipoOperacao.TipoOcorrencia?.Descricao ?? "" },
                TipoOcorrenciaPedidoEntregueForaPrazo = new { Codigo = tipoOperacao.TipoOcorrenciaPedidoEntregueForaPrazo?.Codigo ?? 0, Descricao = tipoOperacao.TipoOcorrenciaPedidoEntregueForaPrazo?.Descricao ?? "" },
                tipoOperacao.ModeloCobrancaEstadiaTracking,
                tipoOperacao.CodigoCondicaoPagamento,
                ConsiderarDatasDePrevisaoDoPedidoParaEstadia = tipoOperacao.ConfiguracaoFreeTime?.ConsiderarDatasDePrevisaoDoPedidoParaEstadia ?? false,
                tipoOperacao.VincularApenasUmaNotaPorEntrega
            };
        }

        private dynamic ObterPaletes(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao)
        {
            return new
            {
                tipoOperacao.NaoGerarControlePaletes,
            };
        }

        private dynamic ObterConfiguracaoControleEntrega(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.TipoOperacaoControleEntregaSetor repTipoOperacaoControleEntregaSetor = new Repositorio.Embarcador.Pedidos.TipoOperacaoControleEntregaSetor(unitOfWork);
            List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoControleEntregaSetor> listaSetores = repTipoOperacaoControleEntregaSetor.BuscarPorConfiguracaoTipoOperacaoControleEntrega(tipoOperacao.ConfiguracaoControleEntrega?.Codigo ?? 0);

            return new
            {
                LocalDeParqueamentoCliente = new
                {
                    Codigo = tipoOperacao.ConfiguracaoControleEntrega?.LocalDeParqueamentoCliente?.CPF_CNPJ ?? 0,
                    Descricao = tipoOperacao.ConfiguracaoControleEntrega?.LocalDeParqueamentoCliente?.Descricao ?? string.Empty
                },
                ExigirConferenciaProdutosAoConfirmarEntrega = tipoOperacao.ConfiguracaoControleEntrega?.ExigirConferenciaProdutosAoConfirmarEntrega ?? false,
                GerarEventoColetaEntregaUnicoParaTodosTrechos = tipoOperacao.ConfiguracaoControleEntrega?.GerarEventoColetaEntregaUnicoParaTodosTrechos ?? false,
                ExigirJustificativaParaEncerramentoManualViagem = tipoOperacao?.ExigirJustificativaParaEncerramentoManualViagem ?? false,
                EnviarBoletimViagemAoFinalizarViagem = tipoOperacao.ConfiguracaoControleEntrega?.EnviarBoletimViagemAoFinalizarViagem ?? false,
                EnviarBoletimViagemAoFinalizarViagemParaRemetente = tipoOperacao.ConfiguracaoControleEntrega?.EnviarBoletimViagemAoFinalizarViagemParaRemetente ?? false,
                EnviarBoletimViagemAoFinalizarViagemParaTransportador = tipoOperacao.ConfiguracaoControleEntrega?.EnviarBoletimViagemAoFinalizarViagemParaTransportador ?? false,
                AlterarSituacaoEntregaNFeParaDevolvidaAoConfirmarEntrega = tipoOperacao.ConfiguracaoControleEntrega?.AlterarSituacaoEntregaNFeParaDevolvidaAoConfirmarEntrega ?? false,
                PermitirInformarNotasFiscaisNoControleEntrega = tipoOperacao.ConfiguracaoControleEntrega?.PermitirInformarNotasFiscaisNoControleEntrega ?? false,
                ExigirInformarNumeroPacotesNaColetaTrizy = tipoOperacao.ConfiguracaoControleEntrega?.ExigirInformarNumeroPacotesNaColetaTrizy ?? false,
                FinalizarControleEntregaAoFinalizarMonitoramentoCarga = tipoOperacao.ConfiguracaoControleEntrega?.FinalizarControleEntregaAoFinalizarMonitoramentoCarga ?? false,
                RecriarControleDeEntregasAoConfirmarEnvioDocumentos = tipoOperacao.ConfiguracaoControleEntrega?.RecriarControleDeEntregasAoConfirmarEnvioDocumentos ?? false,
                ConfirmarColetasQuandoTodasAsEntregasDaCargaForemConcluidas = tipoOperacao.ConfiguracaoControleEntrega?.ConfirmarColetasQuandoTodasAsEntregasDaCargaForemConcluidas ?? false,
                DisponibilizarFotoDaColetaNaTelaDeAprovacaoDeNotaFiscal = tipoOperacao.ConfiguracaoControleEntrega?.DisponibilizarFotoDaColetaNaTelaDeAprovacaoDeNotaFiscal ?? false,
                Setores = (
                    from setor in listaSetores
                    select new
                    {
                        Codigo = setor.Setor.Codigo,
                        Descricao = setor.Setor.Descricao,
                    }
                ).ToList(),

                NaoFinalizarEntregasPorTrackingMonitoramento = tipoOperacao.ConfiguracaoControleEntrega?.NaoFinalizarEntregasPorTrackingMonitoramento ?? false,
                SobrescreverDataEntradaSaidaAlvo = tipoOperacao.ConfiguracaoControleEntrega?.SobrescreverDataEntradaSaidaAlvo ?? false,
                NaoFinalizarColetasPorTrackingMonitoramento = tipoOperacao.ConfiguracaoControleEntrega?.NaoFinalizarColetasPorTrackingMonitoramento ?? false,
                GerarControleEntregaSemRota = tipoOperacao.ConfiguracaoControleEntrega?.GerarControleEntregaSemRota ?? false,
                OrdenarColetasPorDataCarregamento = tipoOperacao.ConfiguracaoControleEntrega?.OrdenarColetasPorDataCarregamento ?? false,
                BloquearInicioeFimDeViagemPeloTransportadorEmCargaNaoEmitida = tipoOperacao.ConfiguracaoControleEntrega?.BloquearInicioeFimDeViagemPeloTransportadorEmCargaNaoEmitida ?? false,

                ConsiderarConfiguracoesPrevisaoEntregaTipoOperacao = tipoOperacao.ConfiguracaoControleEntrega?.ConsiderarConfiguracoesPrevisaoEntregaTipoOperacao ?? false,
                DesconsiderarSabadosCalculoPrevisao = tipoOperacao.ConfiguracaoControleEntrega?.DesconsiderarSabadosCalculoPrevisao ?? false,
                DesconsiderarFeriadosCalculoPrevisao = tipoOperacao.ConfiguracaoControleEntrega?.DesconsiderarFeriadosCalculoPrevisao ?? false,
                DesconsiderarDomingosCalculoPrevisao = tipoOperacao.ConfiguracaoControleEntrega?.DesconsiderarDomingosCalculoPrevisao ?? false,
                ConsiderarJornadaMotorista = tipoOperacao.ConfiguracaoControleEntrega?.ConsiderarJornadaMotorista ?? false,
                HorarioInicialAlmoco = tipoOperacao.ConfiguracaoControleEntrega?.HorarioInicialAlmoco ?? new TimeSpan(),
                MinutosIntervalo = tipoOperacao.ConfiguracaoControleEntrega?.MinutosIntervalo ?? 0,
                DesconsiderarHorariosParaPrazoEntrega = tipoOperacao.ConfiguracaoControleEntrega?.DesconsiderarHorariosParaPrazoEntrega ?? false,
                DataRealizacaoDoEvento = tipoOperacao.ConfiguracaoControleEntrega?.DataRealizacaoDoEvento ?? TipoDataCalculoParadaNoPrazo.DataConfirmacao,
                DataPrevistaDoEvento = tipoOperacao.ConfiguracaoControleEntrega?.DataPrevistaDoEvento ?? TipoDataCalculoParadaNoPrazo.DataPrevista,
            };
        }

        private dynamic ObterConfiguracaoLicenca(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao)
        {
            return new
            {
                ValidarLicencaMotorista = tipoOperacao.ConfiguracaoTipoOperacaoLicenca?.ValidarLicencaMotorista ?? false
            };
        }

        private void PreencherTipoOperacao(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.GestaoPatio.CheckListTipo repCheckListTipo = new Repositorio.Embarcador.GestaoPatio.CheckListTipo(unitOfWork);
            Repositorio.Embarcador.CIOT.ConfiguracaoCIOT repConfiguracaoCIOT = new Repositorio.Embarcador.CIOT.ConfiguracaoCIOT(unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
            Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Cargas.Retornos.TipoRetornoCarga repTipoRetornoCarga = new Repositorio.Embarcador.Cargas.Retornos.TipoRetornoCarga(unitOfWork);

            bool.TryParse(Request.Params("PermiteDesistenciaCarga"), out bool permiteDesistenciaCarga);
            bool.TryParse(Request.Params("CobrarDesistenciaCargaAposHorario"), out bool cobrarDesistenciaCargaAposHorario);
            bool.TryParse(Request.Params("PermiteDesistenciaCarregamento"), out bool permiteDesistenciaCarregamento);
            bool.TryParse(Request.Params("AtualizarProdutosPorXmlNotaFiscal"), out bool atualizarProdutosPorXmlNotaFiscal);
            bool.TryParse(Request.Params("AtualizarSaldoPedidoProdutosPorXmlNotaFiscal"), out bool atualizarSaldoPedidoProdutosPorXmlNotaFiscal);
            bool.TryParse(Request.Params("UtilizarFatorCubagem"), out bool utilizarFatorCubagem);
            bool.TryParse(Request.Params("UtilizarPaletizacao"), out bool utilizarPaletizacao);
            bool.TryParse(Request.Params("NaoPermitirAlterarValorFreteNaCarga"), out bool naoPermitirAlterarValorFreteNaCarga);
            bool.TryParse(Request.Params("UtilizarDadosPedidoParaNotasExterior"), out bool utilizarDadosPedidoParaNotasExterior);
            bool.TryParse(Request.Params("UtilizarConfiguracaoTerceiro"), out bool utilizarConfiguracaoTerceiro);
            bool.TryParse(Request.Params("GerarDiariaMotoristaProprio"), out bool gerarDiariaMotoristaProprio);
            bool.TryParse(Request.Params("PossuiIntegracaoLogiun"), out bool possuiIntegracaoLogiun);
            bool.TryParse(Request.Params("PossuiIntegracaoMundialRisk"), out bool possuiIntegracaoMundialRisk);
            bool.TryParse(Request.Params("ColetaEmProdutorRural"), out bool coletaEmProdutorRural);
            bool.TryParse(Request.Params("PossuiIntegracaoAngelLira"), out bool possuiIntegracaoAngelLira);
            bool.TryParse(Request.Params("ConsiderarTomadorPedido"), out bool considerarTomadorPedido);
            bool remetenteDoCTeSeraODestinatarioDoPedido = Request.GetBoolParam("RemetenteDoCTeSeraODestinatarioDoPedido");
            bool possuiIntegracaoGoldenService = Request.GetBoolParam("PossuiIntegracaoGoldenService");
            bool configuracaoTabelaFretePorPedido = Request.GetBoolParam("ConfiguracaoTabelaFretePorPedido");

            decimal.TryParse(Request.Params("PercentualCobrarDesistenciaCarregamento"), out decimal percentualCobrarDesistenciaCarregamento);
            decimal.TryParse(Request.Params("PercentualCobrarDesistenciaCarga"), out decimal percentualCobrarDesistenciaCarga);
            decimal.TryParse(Request.Params("FatorCubagem"), out decimal fatorCubagem);
            decimal.TryParse(Request.Params("PesoPorPallet"), out decimal pesoPorPallet);
            decimal.TryParse(Request.Params("PercentualAdiantamentoFreteTerceiro"), out decimal percentualAdiantamentoFreteTerceiro);

            decimal pesoMinimo = Request.GetDecimalParam("PesoMinimo");
            decimal pesoMaximo = Request.GetDecimalParam("PesoMaximo");

            TimeSpan.TryParseExact(Request.Params("HoraCobrarDesistenciaCarga"), @"hh\:mm", null, out TimeSpan horaCobrarDesistenciaCarga);

            int codigoPagamentoMotoristaTipo = Request.GetIntParam("PagamentoMotoristaTipo");
            int grupoPessoas = Request.GetIntParam("GrupoPessoas");
            int tipoCarga = Request.GetIntParam("TipoDeCargaPadraoOperacao");
            int codigoModeloContratacao = Request.GetIntParam("CodigoModeloContratacao");
            int integracaoProcedimentoEmbarque = Request.GetIntParam("IntegracaoProcedimentoEmbarque");
            int codigoGrupoTomador = Request.GetIntParam("GrupoTomador");
            int codigoConfiguracaoCIOT = Request.GetIntParam("ConfiguracaoCIOT");


            int grupoTipoOperacao = Request.GetIntParam("GrupoTipoOperacao");
            int componenteFrete = Request.GetIntParam("ComponenteFrete");
            int tipoOcorrencia = Request.GetIntParam("TipoOcorrencia");
            int tipoOcorrenciaPedidoEntregueForaPrazo = Request.GetIntParam("TipoOcorrenciaPedidoEntregueForaPrazo");
            int remessaSAP = Request.GetIntParam("RemessaSAP");
            int tipoOperacaoRedespacho = Request.GetIntParam("TipoOperacaoRedespacho");
            int tipoOperacaoPreCheking = Request.GetIntParam("TipoOperacaoPrecheckin");
            int tipoOperacaoPreChekingTransferencia = Request.GetIntParam("TipoOperacaoPrecheckinTransferencia");

            double pessoa = Request.GetDoubleParam("Pessoa");
            double cpfCnpjExpedidor = Request.GetDoubleParam("Expedidor");
            double cpfCnpjRecebedor = Request.GetDoubleParam("Recebedor");

            Enum.TryParse(Request.Params("TipoUsoFatorCubagem"), out TipoUsoFatorCubagem tipoUsoFatorCubagem);

            string codigoIntegracaoRepom = Request.Params("CodigoIntegracaoRepom");
            string centroCustoBrasilRisk = Request.Params("CentroCustoBrasilRisk");
            string cnpjTransportadoraBrasilRisk = Request.Params("CNPJTransportadoraBrasilRisk");
            string cnpjClienteBrasilRisk = Request.Params("CNPJClienteBrasilRisk");
            string produtoBrasilRisk = Request.Params("ProdutoBrasilRisk");

            string centroCustoMundialRisk = Request.Params("CentroCustoMundialRisk");
            string cnpjTransportadoraMundialRisk = Request.Params("CNPJTransportadoraMundialRisk");
            string cnpjClienteMundialRisk = Request.Params("CNPJClienteMundialRisk");
            string produtoMundialRisk = Request.Params("ProdutoMundialRisk");

            string centroCustoLogiun = Request.Params("CentroCustoLogiun");
            string cnpjTransportadoraLogiun = Request.Params("CNPJTransportadoraLogiun");
            string cnpjClienteLogiun = Request.Params("CNPJClienteLogiun");
            string produtoLogiun = Request.Params("ProdutoLogiun");

            string codigoIntegracaoGoldenService = Request.Params("CodigoIntegracaoGoldenService");

            string codigoIntegracaoGerenciadoraRisco = Request.Params("CodigoIntegracaoGerenciadoraRisco");

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                tipoOperacao.NaoGerarContratoFreteTerceiro = Request.GetBoolParam("NaoGerarContratoFreteTerceiro");
            else
                tipoOperacao.NaoGerarContratoFreteTerceiro = Request.GetBoolParam("NaoGerarCIOT");

            tipoOperacao.NaoPermitirCTesParaTransbordoComDestinoDiferenteDoPedido = Request.GetBoolParam("NaoPermitirCTesParaTransbordoComDestinoDiferenteDoPedido");
            tipoOperacao.RemessaSAP = remessaSAP;
            tipoOperacao.UtilizarTipoCargaPedidoCalculoFrete = Request.GetBoolParam("UtilizarTipoCargaPedidoCalculoFrete");
            tipoOperacao.PermiteDividirPedidoEmCargasDiferentes = Request.GetBoolParam("PermiteDividirPedidoEmCargasDiferentes");
            tipoOperacao.PossuiIntegracaoNOX = Request.GetBoolParam("PossuiIntegracaoNOX");
            tipoOperacao.ValorMinimoMercadoriaNOX = Request.GetDecimalParam("ValorMinimoMercadoriaNOX");
            tipoOperacao.IntegrarPreSMNOX = Request.GetBoolParam("IntegrarPreSMNOX");
            tipoOperacao.UtilizarNomeDestinatarioNotaFiscalParaEmitirCTe = Request.GetBoolParam("UtilizarNomeDestinatarioNotaFiscalParaEmitirCTe");
            tipoOperacao.ConfiguracaoCIOT = codigoConfiguracaoCIOT > 0 ? repConfiguracaoCIOT.BuscarPorCodigo(codigoConfiguracaoCIOT) : null;
            tipoOperacao.PesoMinimo = pesoMinimo;
            tipoOperacao.PesoMaximo = pesoMaximo;
            tipoOperacao.ConfiguracaoTabelaFretePorPedido = configuracaoTabelaFretePorPedido;
            tipoOperacao.CodigoIntegracaoRepom = codigoIntegracaoRepom;
            tipoOperacao.PermitirAdicionarNaJanelaDescarregamento = Request.GetBoolParam("PermitirAdicionarNaJanelaDescarregamento");
            tipoOperacao.BloquearAdicaoNaJanelaDescarregamentoAutomaticamente = Request.GetBoolParam("BloquearAdicaoNaJanelaDescarregamentoAutomaticamente");
            tipoOperacao.PossuiIntegracaoAngelLira = possuiIntegracaoAngelLira;
            tipoOperacao.ConsiderarTomadorPedido = considerarTomadorPedido;
            tipoOperacao.PossuiIntegracaoTrafegus = Request.GetBoolParam("PossuiIntegracaoTrafegus");
            tipoOperacao.PermiteConsultarPorPacotesLoggi = Request.GetBoolParam("PermiteConsultarPorPacotesLoggi");
            tipoOperacao.TipoOperacaoExibeValorUnitarioDoProduto = Request.GetBoolParam("TipoOperacaoExibeValorUnitarioDoProduto");
            tipoOperacao.PossuiIntegracaoANTT = Request.GetBoolParam("PossuiIntegracaoANTT");
            tipoOperacao.PossuiIntegracaoIntercab = Request.GetBoolParam("PossuiIntegracaoIntercab");
            tipoOperacao.PossuiIntegracaoGoldenService = possuiIntegracaoGoldenService;
            tipoOperacao.ColetaEmProdutorRural = coletaEmProdutorRural;
            tipoOperacao.RemetenteDoCTeSeraODestinatarioDoPedido = remetenteDoCTeSeraODestinatarioDoPedido;
            tipoOperacao.CodigoIntegracaoGoldenService = codigoIntegracaoGoldenService;
            tipoOperacao.CodigoIntegracaoGerenciadoraRisco = codigoIntegracaoGerenciadoraRisco;
            tipoOperacao.ProdutoBrasilRisk = produtoBrasilRisk;
            tipoOperacao.CNPJClienteBrasilRisk = cnpjClienteBrasilRisk;
            tipoOperacao.CNPJTransportadoraBrasilRisk = cnpjTransportadoraBrasilRisk;
            tipoOperacao.CentroCustoBrasilRisk = centroCustoBrasilRisk;
            tipoOperacao.PossuiIntegracaoBrasilRisk = Request.GetBoolParam("PossuiIntegracaoBrasilRisk");
            tipoOperacao.PedidoLogisticoBrasilRisk = Request.GetBoolParam("PedidoLogisticoBrasilRisk");
            tipoOperacao.EnviarNumeroPedidoEmbarcadorNoCodigoControleBrasilRisk = Request.GetBoolParam("EnviarNumeroPedidoEmbarcadorNoCodigoControleBrasilRisk");
            tipoOperacao.NaoEnviarOrigemComoUltimoPontoRota = Request.GetBoolParam("NaoEnviarOrigemComoUltimoPontoRota");
            tipoOperacao.ProdutoMundialRisk = produtoMundialRisk;
            tipoOperacao.CNPJClienteMundialRisk = cnpjClienteMundialRisk;
            tipoOperacao.CNPJTransportadoraMundialRisk = cnpjTransportadoraMundialRisk;
            tipoOperacao.CentroCustoMundialRisk = centroCustoMundialRisk;
            tipoOperacao.TipoPlanoInfolog = Request.GetStringParam("TipoPlanoInfolog");
            tipoOperacao.ProdutoLogiun = produtoLogiun;
            tipoOperacao.CNPJClienteLogiun = cnpjClienteLogiun;
            tipoOperacao.CNPJTransportadoraLogiun = cnpjTransportadoraLogiun;
            tipoOperacao.CentroCustoLogiun = centroCustoLogiun;
            tipoOperacao.Ativo = bool.Parse(Request.Params("Ativo"));
            tipoOperacao.Descricao = Request.Params("Descricao");
            tipoOperacao.Observacao = Request.Params("Observacao");
            tipoOperacao.CodigoIntegracao = Request.Params("CodigoIntegracao");
            tipoOperacao.ExigeNotaFiscalParaCalcularFrete = (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS ? true : bool.Parse(Request.Params("ExigeNotaFiscalParaCalcularFrete")));
            tipoOperacao.UtilizarExpedidorComoTransportador = bool.Parse(Request.Params("UtilizarExpedidorComoTransportador"));
            tipoOperacao.EmitirDocumentosRetroativamente = bool.Parse(Request.Params("EmitirDocumentosRetroativamente"));
            tipoOperacao.NaoExigeVeiculoParaEmissao = bool.Parse(Request.Params("NaoExigeVeiculoParaEmissao"));
            tipoOperacao.FretePorContadoCliente = bool.Parse(Request.Params("FretePorContadoCliente"));
            tipoOperacao.PermitirQualquerModeloVeicular = bool.Parse(Request.Params("PermitirQualquerModeloVeicular"));
            tipoOperacao.PermitirTransbordarNotasDeOutrasCargas = bool.Parse(Request.Params("PermitirTransbordarNotasDeOutrasCargas"));
            tipoOperacao.EmiteCTeFilialEmissora = bool.Parse(Request.Params("EmiteCTeFilialEmissora"));
            tipoOperacao.ExigeProcImportacaoPedido = bool.Parse(Request.Params("ExigeProcImportacaoPedido"));
            tipoOperacao.AverbarDocumentoDaSubcontratacao = Request.GetBoolParam("AverbarDocumentoDaSubcontratacao");
            tipoOperacao.CalculaFretePorTabelaFreteFilialEmissora = bool.Parse(Request.Params("CalculaFretePorTabelaFreteFilialEmissora"));
            tipoOperacao.HabilitarGestaoPatio = Request.GetBoolParam("HabilitarGestaoPatio");
            tipoOperacao.HabilitarGestaoPatioDestino = Request.GetBoolParam("HabilitarGestaoPatioDestino");
            tipoOperacao.OperacaoRecolhimentoTroca = Request.GetBoolParam("OperacaoRecolhimentoTroca");
            tipoOperacao.ExigePercursoEntreCNPJ = bool.Parse(Request.Params("ExigePercursoEntreCNPJ"));
            tipoOperacao.ExigeConformacaoFreteAntesEmissao = bool.Parse(Request.Params("ExigeConformacaoFreteAntesEmissao"));
            tipoOperacao.NaoExigeConformacaoDasNotasEmissao = bool.Parse(Request.Params("NaoExigeConformacaoDasNotasEmissao"));
            tipoOperacao.LiberarCargaSemNFeAutomaticamenteAposLiberarFaturamento = Request.GetBoolParam("LiberarCargaSemNFeAutomaticamenteAposLiberarFaturamento");
            tipoOperacao.GerarDocumentoPadraoParaCadaPedidoCarga = Request.GetBoolParam("GerarDocumentoPadraoParaCadaPedidoCarga");
            tipoOperacao.LiberarCargaAutomaticamenteParaFaturamentoAposPrazoEsgotadoJanelaCarregamento = Request.GetBoolParam("LiberarCargaAutomaticamenteParaFaturamentoAposPrazoEsgotadoJanelaCarregamento");
            tipoOperacao.PermitirMultiplosDestinatariosPedido = Request.GetBoolParam("PermitirMultiplosDestinatariosPedido");
            tipoOperacao.PermitirMultiplosRemetentesPedido = Request.GetBoolParam("PermitirMultiplosRemetentesPedido");
            tipoOperacao.UtilizarDeslocamentoPedido = Request.GetBoolParam("UtilizarDeslocamentoPedido");
            tipoOperacao.OperacaoTrocaNota = Request.GetBoolParam("OperacaoTrocaNota");
            tipoOperacao.PermitirTrocaNota = Request.GetBoolParam("PermitirTrocaNota");
            tipoOperacao.OperacaoExigeInformarCargaRetorno = Request.GetBoolParam("OperacaoExigeInformarCargaRetorno");
            tipoOperacao.GerarCTeComplementarNaCarga = Request.GetBoolParam("GerarCTeComplementarNaCarga");
            tipoOperacao.ExclusivaDeSubcontratacaoOuRedespacho = Request.GetBoolParam("ExclusivaDeSubcontratacaoOuRedespacho");
            tipoOperacao.AdicionarOuRemoverPedidosReplicaAosTrechosAnteriores = Request.GetBoolParam("AdicionarOuRemoverPedidosReplicaAosTrechosAnteriores");
            tipoOperacao.SempreEmitirSubcontratacao = Request.GetBoolParam("SempreEmitirSubcontratacao");
            tipoOperacao.PermitirUtilizarPlacaContrato = Request.GetBoolParam("PermitirUtilizarPlacaContrato");
            tipoOperacao.NaoGerarControleColetaEntrega = Request.GetBoolParam("NaoGerarControleColetaEntrega");
            tipoOperacao.GerarControleColetaEntregaAposEmissaoDocumentos = Request.GetBoolParam("GerarControleColetaEntregaAposEmissaoDocumentos");
            tipoOperacao.AgendamentoGeraApenasPedido = Request.GetBoolParam("AgendamentoGeraApenasPedido");
            tipoOperacao.PermitirVeiculoDiferenteMontagemCarga = Request.GetBoolParam("PermitirVeiculoDiferenteMontagemCarga");
            tipoOperacao.TipoOperacaoAgendamento = Request.GetBoolParam("TipoOperacaoAgendamento");
            tipoOperacao.GerarControleColeta = Request.GetBoolParam("GerarControleColeta");
            tipoOperacao.PermiteAdicionarColeta = Request.GetBoolParam("PermiteAdicionarColeta");
            tipoOperacao.PermiteImpressaoMobile = Request.GetBoolParam("PermiteImpressaoMobile");
            tipoOperacao.PermiteRetificarMobile = Request.GetBoolParam("PermiteRetificarMobile");
            tipoOperacao.ExibirCalculadoraMobile = Request.GetBoolParam("ExibirCalculadoraMobile");
            tipoOperacao.NaoPermiteRejeitarEntrega = Request.GetBoolParam("NaoPermiteRejeitarEntrega");
            tipoOperacao.ExigirValorFreteAvancarCarga = Request.GetBoolParam("ExigirValorFreteAvancarCarga");
            tipoOperacao.PermiteGerarPedidoSemDestinatario = Request.GetBoolParam("PermiteGerarPedidoSemDestinatario");
            tipoOperacao.PermiteReordenarEntregasCarga = Request.GetBoolParam("PermiteReordenarEntregasCarga");
            tipoOperacao.PermiteInformarIscaNaCarga = Request.GetBoolParam("PermiteInformarIscaNaCarga");
            tipoOperacao.ExigeInformarIscaNaCarga = Request.GetBoolParam("ExigeInformarIscaNaCarga");
            tipoOperacao.GerarCargaFinalizada = Request.GetBoolParam("GerarCargaFinalizada");
            tipoOperacao.NaoIncluirICMSFrete = Request.GetBoolParam("NaoIncluirICMSFrete");

            tipoOperacao.ExigirInformarPeso = Request.GetBoolParam("ExigirInformarPeso");
            tipoOperacao.PermiteEmitirCargaDiferentesOrigensParcialmente = Request.GetBoolParam("PermiteEmitirCargaDiferentesOrigensParcialmente");
            tipoOperacao.ExigirInformarQuantidadeMercadoria = Request.GetBoolParam("ExigirInformarQuantidadeMercadoria");
            tipoOperacao.ExigirInformarValorNota = Request.GetBoolParam("ExigirInformarValorNota");
            tipoOperacao.ExigirInformarValorMercadoria = Request.GetBoolParam("ExigirInformarValorMercadoria");
            tipoOperacao.ExigeProdutoEmbarcadorPedido = Request.GetBoolParam("ExigeProdutoEmbarcadorPedido");
            tipoOperacao.BloquearEmisssaoComMesmoLocalDeOrigemEDestino = Request.GetBoolParam("BloquearEmisssaoComMesmoLocalDeOrigemEDestino");
            tipoOperacao.DisponibilizarPedidosParaSeparacaoAposEmissaoDocumentos = Request.GetBoolParam("DisponibilizarPedidosParaSeparacaoAposEmissaoDocumentos");
            tipoOperacao.DisponibilizarPedidosComRecebedorParaSeparacaoAposEmissaoDocumentos = Request.GetBoolParam("DisponibilizarPedidosComRecebedorParaSeparacaoAposEmissaoDocumentos");
            tipoOperacao.TransbordoRodoviario = Request.GetBoolParam("TransbordoRodoviario");
            tipoOperacao.LogisticaReversa = Request.GetBoolParam("LogisticaReversa");
            tipoOperacao.ExigirInformarNCM = Request.GetBoolParam("ExigirInformarNCM");
            tipoOperacao.ExigirInformarCFOP = Request.GetBoolParam("ExigirInformarCFOP");
            tipoOperacao.ExigirInformarM3 = Request.GetBoolParam("ExigirInformarM3");

            tipoOperacao.NaoExigeRotaRoteirizada = Request.GetBoolParam("NaoExigeRotaRoteirizada");
            tipoOperacao.RoteirizarPorLocalidade = Request.GetBoolParam("RoteirizarPorLocalidade");
            tipoOperacao.NaoComprarValePedagio = Request.GetBoolParam("NaoComprarValePedagio");
            tipoOperacao.ExigirConfirmacaoDadosTransportadorAvancarCarga = Request.GetBoolParam("ExigirConfirmacaoDadosTransportadorAvancarCarga");
            tipoOperacao.GerarPedidoNoRecebientoNFe = Request.GetBoolParam("GerarPedidoNoRecebientoNFe");
            tipoOperacao.ObrigarRotaNaMontagemDeCarga = Request.GetBoolParam("ObrigarRotaNaMontagemDeCarga");
            tipoOperacao.UtilizarDataNFeEmissaoDocumentos = Request.GetBoolParam("UtilizarDataNFeEmissaoDocumentos");
            tipoOperacao.ExigirDataRetiradaCtrnJanelaCarregamentoTransportador = Request.GetBoolParam("ExigirDataRetiradaCtrnJanelaCarregamentoTransportador");
            tipoOperacao.ExigirMaxGrossJanelaCarregamentoTransportador = Request.GetBoolParam("ExigirMaxGrossJanelaCarregamentoTransportador");
            tipoOperacao.ExigirNumeroContainerJanelaCarregamentoTransportador = Request.GetBoolParam("ExigirNumeroContainerJanelaCarregamentoTransportador");
            tipoOperacao.ExigirTaraContainerJanelaCarregamentoTransportador = Request.GetBoolParam("ExigirTaraContainerJanelaCarregamentoTransportador");
            tipoOperacao.ObrigatorioInformarAnexoSolicitacaoFrete = Request.GetBoolParam("ObrigatorioInformarAnexoSolicitacaoFrete");
            tipoOperacao.CorJanelaCarregamento = Request.GetBoolParam("UtilizarCorJanelaCarregamento") ? Request.GetStringParam("CorJanelaCarregamento") : string.Empty;
            //tipoOperacao.BloquearFreteZerado = Request.GetBoolParam("BloquearFreteZerado");

            tipoOperacao.PermitirDataInicioViagemAnteriorDataCarregamento = Request.GetBoolParam("PermitirDataInicioViagemAnteriorDataCarregamento");
            tipoOperacao.NaoAguardarImportacaoDoCTeParaAvancar = Request.GetBoolParam("NaoAguardarImportacaoDoCTeParaAvancar");
            tipoOperacao.OperacaoDeImportacaoExportacao = bool.Parse(Request.Params("OperacaoDeImportacaoExportacao"));
            tipoOperacao.GeraCargaAutomaticamente = bool.Parse(Request.Params("GeraCargaAutomaticamente"));
            tipoOperacao.TipoOperacaoUtilizaCentroDeCustoPEP = bool.Parse(Request.Params("TipoOperacaoUtilizaCentroDeCustoPEP"));
            tipoOperacao.TipoOperacaoUtilizaContaRazao = bool.Parse(Request.Params("TipoOperacaoUtilizaContaRazao"));

            tipoOperacao.ExigePlacaTracao = bool.Parse(Request.Params("ExigePlacaTracao"));
            tipoOperacao.NaoValidarTransportadorImportacaoDocumento = bool.Parse(Request.Params("NaoValidarTransportadorImportacaoDocumento"));

            tipoOperacao.EmissaoDocumentosForaDoSistema = bool.Parse(Request.Params("EmissaoDocumentosForaDoSistema"));
            tipoOperacao.CompraValePedagioDocsEmitidosFora = Request.GetBoolParam("CompraValePedagioDocsEmitidosFora");

            tipoOperacao.NaoPermiteAgruparCargas = bool.Parse(Request.Params("NaoPermiteAgruparCargas"));
            tipoOperacao.GerarCargaViaMontagemDoTipoPreCarga = bool.Parse(Request.Params("GerarCargaViaMontagemDoTipoPreCarga"));
            tipoOperacao.UsarRecebedorComoPontoPartidaCarga = bool.Parse(Request.Params("UsarRecebedorComoPontoPartidaCarga"));
            tipoOperacao.ManterUnicaCargaNoAgrupamento = bool.Parse(Request.Params("ManterUnicaCargaNoAgrupamento"));

            tipoOperacao.TipoImpressao = (TipoImpressao)int.Parse(Request.Params("TipoImpressao"));
            tipoOperacao.TipoImpressaoDiarioBordo = Request.GetEnumParam<TipoImpressaoDiarioBordo>("TipoImpressaoDiarioBordo");
            tipoOperacao.ExigeRecebedor = bool.Parse(Request.Params("ExigeRecebedor"));
            tipoOperacao.NaoGerarFaturamento = Request.GetBoolParam("NaoGerarFaturamento");
            tipoOperacao.ExigeQueVeiculoIgualModeloVeicularDaCarga = bool.Parse(Request.Params("ExigeQueVeiculoIgualModeloVeicularDaCarga"));
            tipoOperacao.IndicadorGlobalizadoRemetente = bool.Parse(Request.Params("IndicadorGlobalizadoRemetente"));
            tipoOperacao.PermiteImportarDocumentosManualmente = bool.Parse(Request.Params("PermiteImportarDocumentosManualmente"));
            tipoOperacao.EmissaoMDFeManual = bool.Parse(Request.Params("EmissaoMDFeManual"));
            tipoOperacao.ValidarNotaFiscalPeloDestinatario = bool.Parse(Request.Params("ValidarNotaFiscalPeloDestinatario"));
            tipoOperacao.NaoGerarTituloGNREAutomatico = bool.Parse(Request.Params("NaoGerarTituloGNREAutomatico"));
            tipoOperacao.OperacaoDeRedespacho = bool.Parse(Request.Params("OperacaoDeRedespacho"));
            tipoOperacao.UsaJanelaCarregamentoPorEscala = bool.Parse(Request.Params("UsaJanelaCarregamentoPorEscala"));
            tipoOperacao.PermiteTransportadorAvancarEtapaEmissao = Request.GetBoolParam("PermiteTransportadorAvancarEtapaEmissao");
            tipoOperacao.UtilizarTipoOperacaoPreCargaAoGerarCarga = Request.GetBoolParam("UtilizarTipoOperacaoPreCargaAoGerarCarga");
            tipoOperacao.PermiteUtilizarEmContratoFrete = bool.Parse(Request.Params("PermiteUtilizarEmContratoFrete"));
            tipoOperacao.ObrigatorioPassagemExpedicao = bool.Parse(Request.Params("ObrigatorioPassagemExpedicao"));
            tipoOperacao.UtilizarRecebedorApenasComoParticipante = Request.GetBoolParam("UtilizarRecebedorApenasComoParticipante");
            tipoOperacao.IndicadorGlobalizadoDestinatario = bool.Parse(Request.Params("IndicadorGlobalizadoDestinatario"));
            tipoOperacao.NotificarCasoNumeroPedidoForExistente = bool.Parse(Request.Params("NotificarCasoNumeroPedidoForExistente"));
            tipoOperacao.IndicadorGlobalizadoDestinatarioNFSe = bool.Parse(Request.Params("IndicadorGlobalizadoDestinatarioNFSe"));
            tipoOperacao.SempreUsarIndicadorGlobalizadoDestinatario = bool.Parse(Request.Params("SempreUsarIndicadorGlobalizadoDestinatario"));

            tipoOperacao.PermitirTransportadorInformarObservacaoImpressaoCarga = Request.GetBoolParam("PermitirTransportadorInformarObservacaoImpressaoCarga");
            tipoOperacao.PermitirAgendarDescargaAposDataEntregaSugerida = Request.GetBoolParam("PermitirAgendarDescargaAposDataEntregaSugerida");
            tipoOperacao.PermiteDesistenciaCarga = permiteDesistenciaCarga;
            tipoOperacao.PermiteDesistenciaCarregamento = permiteDesistenciaCarregamento;
            tipoOperacao.AtualizarProdutosPorXmlNotaFiscal = atualizarProdutosPorXmlNotaFiscal;
            tipoOperacao.AtualizarSaldoPedidoProdutosPorXmlNotaFiscal = atualizarSaldoPedidoProdutosPorXmlNotaFiscal;
            tipoOperacao.CobrarDesistenciaCargaAposHorario = cobrarDesistenciaCargaAposHorario;
            tipoOperacao.PercentualCobrarDesistenciaCarga = percentualCobrarDesistenciaCarga;
            tipoOperacao.PercentualCobrarDesistenciaCarregamento = percentualCobrarDesistenciaCarregamento;
            tipoOperacao.UtilizarDadosPedidoParaNotasExterior = utilizarDadosPedidoParaNotasExterior;
            tipoOperacao.UtilizarConfiguracaoTerceiro = utilizarConfiguracaoTerceiro;
            tipoOperacao.GerarDiariaMotoristaProprio = gerarDiariaMotoristaProprio;
            tipoOperacao.PossuiIntegracaoMundialRisk = possuiIntegracaoMundialRisk;
            tipoOperacao.PossuiIntegracaoLogiun = possuiIntegracaoLogiun;
            tipoOperacao.PercentualAdiantamentoFreteTerceiro = percentualAdiantamentoFreteTerceiro;
            tipoOperacao.PercentualAbastecimentoFreteTerceiro = Request.GetDecimalParam("PercentualAbastecimentoFreteTerceiro");
            tipoOperacao.PercentualCobrancaPadraoTerceiros = Request.GetDecimalParam("PercentualCobrancaPadraoTerceiros");
            tipoOperacao.UtilizarConfiguracaoTerceiroComoPadrao = Request.GetBoolParam("UtilizarConfiguracaoTerceiroComoPadrao");
            tipoOperacao.TipoPagamentoContratoFreteTerceiro = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoContratoFrete>("TipoPagamentoContratoFreteTerceiro");
            tipoOperacao.DiasVencimentoAdiantamentoContratoFrete = Request.GetIntParam("DiasVencimentoAdiantamentoContratoFrete");
            tipoOperacao.DiasVencimentoSaldoContratoFrete = Request.GetIntParam("DiasVencimentoSaldoContratoFrete");
            tipoOperacao.PrazoSolicitacaoOcorrencia = Request.GetIntParam("PrazoSolicitacaoOcorrencia");

            tipoOperacao.CodigoModeloContratacao = codigoModeloContratacao;
            tipoOperacao.IntegracaoProcedimentoEmbarque = integracaoProcedimentoEmbarque;

            tipoOperacao.TipoUltimoPontoRoteirizacao = Request.GetNullableEnumParam<TipoUltimoPontoRoteirizacao>("TipoUltimoPontoRoteirizacao");
            tipoOperacao.EixosSuspenso = Request.GetNullableEnumParam<EixosSuspenso>("EixosSuspenso");
            tipoOperacao.TipoCarregamento = Request.GetNullableEnumParam<RetornoCargaTipo>("TipoCarregamento");
            tipoOperacao.TipoObrigacaoUsoTerminal = Request.GetNullableEnumParam<TipoObrigacaoUsoTerminal>("TipoObrigacaoUsoTerminal");
            tipoOperacao.TipoConsolidacao = Request.GetEnumParam<EnumTipoConsolidacao>("TipoConsolidacao", EnumTipoConsolidacao.NaoConsolida);
            tipoOperacao.ModalCarga = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal>("ModalCarga", Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Rodoviario);

            if (ConfiguracaoEmbarcador.UtilizaMoedaEstrangeira)
            {
                tipoOperacao.UtilizarMoedaEstrangeira = Request.GetNullableBoolParam("UtilizarMoedaEstrangeira");
                tipoOperacao.Moeda = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral>("Moeda");
            }
            else
            {
                tipoOperacao.UtilizarMoedaEstrangeira = null;
                tipoOperacao.Moeda = null;
            }

            if (cobrarDesistenciaCargaAposHorario)
                tipoOperacao.HoraCobrarDesistenciaCarga = horaCobrarDesistenciaCarga;
            else
                tipoOperacao.HoraCobrarDesistenciaCarga = null;

            tipoOperacao.UtilizarFatorCubagem = utilizarFatorCubagem;

            if (tipoOperacao.UtilizarFatorCubagem)
            {
                tipoOperacao.TipoUsoFatorCubagem = tipoUsoFatorCubagem;
                tipoOperacao.FatorCubagem = fatorCubagem;
            }
            else
            {
                tipoOperacao.TipoUsoFatorCubagem = null;
                tipoOperacao.FatorCubagem = null;
            }

            tipoOperacao.UtilizarPaletizacao = utilizarPaletizacao;
            tipoOperacao.NaoPermitirAlterarValorFreteNaCarga = naoPermitirAlterarValorFreteNaCarga;

            if (tipoOperacao.UtilizarPaletizacao)
                tipoOperacao.PesoPorPallet = pesoPorPallet;
            else
                tipoOperacao.PesoPorPallet = null;

            bool usarConfiguracaoEmissao = bool.Parse(Request.Params("UsarConfiguracaoEmissao"));
            tipoOperacao.UsarConfiguracaoFaturaPorTipoOperacao = bool.Parse(Request.Params("UsarConfiguracaoFaturaPorTipoOperacao"));

            TipoPessoa tipoPessoa;
            Enum.TryParse(Request.Params("TipoPessoa"), out tipoPessoa);
            tipoOperacao.TipoPessoa = tipoPessoa;

            tipoOperacao.UsarConfiguracaoEmissao = usarConfiguracaoEmissao;

            tipoOperacao.ProdutoPredominanteOperacao = Request.Params("ProdutoPredominanteOperacao");
            if (tipoCarga > 0)
                tipoOperacao.TipoDeCargaPadraoOperacao = new Dominio.Entidades.Embarcador.Cargas.TipoDeCarga() { Codigo = tipoCarga };
            else
                tipoOperacao.TipoDeCargaPadraoOperacao = null;

            if (codigoGrupoTomador > 0)
            {
                Repositorio.Embarcador.Pessoas.GrupoPessoas repositorioGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);
                tipoOperacao.GrupoTomador = repositorioGrupoPessoas.BuscarPorCodigo(codigoGrupoTomador);
            }
            else
                tipoOperacao.GrupoTomador = null;

            tipoOperacao.CodigoModeloContratacao = codigoModeloContratacao;
            tipoOperacao.IntegracaoProcedimentoEmbarque = integracaoProcedimentoEmbarque;

            tipoOperacao.PagamentoMotoristaTipo = codigoPagamentoMotoristaTipo > 0 ? new Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTipo() { Codigo = codigoPagamentoMotoristaTipo } : null;
            tipoOperacao.GrupoPessoas = grupoPessoas > 0 ? new Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas() { Codigo = grupoPessoas } : null;
            tipoOperacao.GrupoTipoOperacao = grupoTipoOperacao > 0 ? new Dominio.Entidades.Embarcador.Pedidos.GrupoTipoOperacao() { Codigo = grupoTipoOperacao } : null;
            tipoOperacao.Pessoa = grupoPessoas <= 0 && pessoa > 0d ? new Dominio.Entidades.Cliente() { CPF_CNPJ = pessoa } : null;

            tipoOperacao.IntegrarPreSMAngelLira = Request.GetBoolParam("IntegrarPreSMAngelLira");
            tipoOperacao.ReintegrarSMCargaAngelLira = Request.GetBoolParam("ReintegrarSMCargaAngelLira");
            tipoOperacao.TempoEntregaAngelLira = Request.GetIntParam("TempoEntregaAngelLira");
            tipoOperacao.NaoEnviarDataInicioETerminoViagemAngelLira = Request.GetBoolParam("NaoEnviarDataInicioETerminoViagemAngelLira");
            tipoOperacao.PossuiIntegracaoRaster = Request.GetBoolParam("PossuiIntegracaoRaster");
            tipoOperacao.CodigoPerfilSegurancaRaster = Request.GetNullableIntParam("CodigoPerfilSegurancaRaster");
            tipoOperacao.CodigoFilialRaster = Request.GetNullableIntParam("CodigoFilialRaster");
            tipoOperacao.GerarComissaoParcialMotorista = Request.GetBoolParam("GerarComissaoParcialMotorista");
            tipoOperacao.PercentualComissaoParcialMotorista = Request.GetDecimalParam("PercentualComissaoParcialMotorista");
            tipoOperacao.CamposSecundariosObrigatoriosPedido = Request.GetBoolParam("CamposSecundariosObrigatoriosPedido");
            tipoOperacao.NaoEmitirCargaComValorZerado = Request.GetBoolParam("NaoEmitirCargaComValorZerado");
            tipoOperacao.NaoPermitirValorFreteLiquidoZerado = Request.GetBoolParam("NaoPermitirValorFreteLiquidoZerado");
            tipoOperacao.NaoAtualizarDataReprogramadaAposEntradaRaio = Request.GetBoolParam("NaoAtualizarDataReprogramadaAposEntradaRaio");
            tipoOperacao.RealizarBaixaEntradaNoRaio = Request.GetBoolParam("RealizarBaixaEntradaNoRaio");
            tipoOperacao.AvancarCargaAutomaticaAposMontagem = Request.GetBoolParam("AvancarCargaAutomaticaAposMontagem");
            tipoOperacao.AvancarEtapaFreteAutomaticamente = Request.GetBoolParam("AvancarEtapaFreteAutomaticamente");
            tipoOperacao.ValidarTomadorDoPedidoDiferenteDaCarga = Request.GetBoolParam("ValidarTomadorDoPedidoDiferenteDaCarga");
            tipoOperacao.ImportarTerminalOrigemComoExpedidor = Request.GetBoolParam("ImportarTerminalOrigemComoExpedidor");
            tipoOperacao.ImportarTerminalDestinoComoRecebedor = Request.GetBoolParam("ImportarTerminalDestinoComoRecebedor");
            tipoOperacao.LiberarAutomaticamentePagamento = Request.GetBoolParam("LiberarAutomaticamentePagamento");
            tipoOperacao.CriarNovoPedidoAoVincularNotaFiscalComDiferentesParticipantes = Request.GetBoolParam("CriarNovoPedidoAoVincularNotaFiscalComDiferentesParticipantes");
            tipoOperacao.GerarRedespachoParaOutrasEtapasCarregamento = Request.GetBoolParam("GerarRedespachoParaOutrasEtapasCarregamento");
            tipoOperacao.GerarRedespachoAutomaticamentePorPedidoAposEmissaoDocumentos = Request.GetBoolParam("GerarRedespachoAutomaticamentePorPedidoAposEmissaoDocumentos");
            tipoOperacao.GerarRedespachoAutomaticamente = Request.GetBoolParam("GerarRedespachoAutomaticamente");
            tipoOperacao.Reentrega = Request.GetBoolParam("Reentrega");
            tipoOperacao.RetornoVazio = Request.GetBoolParam("RetornoVazio");
            tipoOperacao.NaoProcessarTrocaAlvoViaMonitoramento = Request.GetBoolParam("NaoProcessarTrocaAlvoViaMonitoramento");
            tipoOperacao.PossuiIntegracaoA52 = Request.GetBoolParam("PossuiIntegracaoA52");
            tipoOperacao.TempoEntregaA52 = Request.GetIntParam("TempoEntregaA52");
            tipoOperacao.TipoA52 = Request.GetIntParam("TipoA52");
            tipoOperacao.TipoCargaA52 = Request.GetIntParam("TipoCargaA52");
            tipoOperacao.TipoOperacaoA52 = Request.GetIntParam("TipoOperacaoA52");
            tipoOperacao.IntegrarPedidoA52 = Request.GetBoolParam("IntegrarPedidosA52");
            tipoOperacao.PedidoColetaEntrega = Request.GetBoolParam("PedidoColetaEntrega");
            tipoOperacao.UtilizarRecebedorPedidoParaSVM = Request.GetBoolParam("UtilizarRecebedorPedidoParaSVM");
            tipoOperacao.EnviarComprovantesDaCarga = Request.GetBoolParam("EnviarComprovantesDaCarga");
            tipoOperacao.MonitorarRetornoCargaBuonny = Request.GetBoolParam("MonitorarRetornoCargaBuonny");
            tipoOperacao.PermitirAlterarVolumesNaCarga = Request.GetBoolParam("PermitirAlterarVolumesNaCarga");
            tipoOperacao.VincularMotoristaFilaCarregamentoManualmente = Request.GetBoolParam("VincularMotoristaFilaCarregamentoManualmente");
            tipoOperacao.PermitirGerarRedespacho = Request.GetBoolParam("PermitirGerarRedespacho");
            tipoOperacao.PermitirGerarRecorrenciaRedespacho = Request.GetBoolParam("PermitirGerarRecorrenciaRedespacho");
            tipoOperacao.NaoUtilizaJanelaCarregamento = Request.GetBoolParam("NaoUtilizaJanelaCarregamento");
            tipoOperacao.PermitirSolicitarNotasFiscaisSemEncerrarMDFeAnterior = Request.GetBoolParam("PermitirSolicitarNotasFiscaisSemEncerrarMDFeAnterior");
            tipoOperacao.DeslocamentoVazio = Request.GetBoolParam("DeslocamentoVazio");
            tipoOperacao.EncerrarMDFeManualmente = Request.GetBoolParam("EncerrarMDFeManualmente");
            tipoOperacao.CargaPropria = Request.GetBoolParam("CargaPropria");
            tipoOperacao.PermitirSelecionarNotasCompativeis = Request.GetBoolParam("PermitirSelecionarNotasCompativeis");
            tipoOperacao.PermitirTransportadorInformeNotasCompativeis = Request.GetBoolParam("PermitirTransportadorInformeNotasCompativeis");
            tipoOperacao.EmissaoAutomaticaCTe = Request.GetBoolParam("EmissaoAutomaticaCTe");
            tipoOperacao.PermitirAdicionarRemoverPedidosEtapa1 = Request.GetBoolParam("PermitirAdicionarRemoverPedidosEtapa1");
            tipoOperacao.TomadorCTeSubcontratacaoDeveSerDoCTeOriginal = Request.GetBoolParam("TomadorCTeSubcontratacaoDeveSerDoCTeOriginal");
            tipoOperacao.FixarValorFreteNegociadoRateioPedidos = Request.GetBoolParam("FixarValorFreteNegociadoRateioPedidos");
            tipoOperacao.UtilizarValorFreteOriginalSubcontratacao = Request.GetBoolParam("UtilizarValorFreteOriginalSubcontratacao");
            tipoOperacao.PermitirExpedidorRecebedorIgualRemetenteDestinatario = Request.GetBoolParam("PermitirExpedidorRecebedorIgualRemetenteDestinatario");
            tipoOperacao.AlterarRemetentePedidoConformeNotaFiscal = Request.GetBoolParam("AlterarRemetentePedidoConformeNotaFiscal");
            tipoOperacao.RetornarCanhotoQuandoTodasNotasDoCTeEstiveremConformadasPagamento = Request.GetBoolParam("RetornarCanhotoQuandoTodasNotasDoCTeEstiveremConformadasPagamento");
            tipoOperacao.RetornarCarregamentoPendenteAposEtapaCTE = Request.GetBoolParam("RetornarCarregamentoPendenteAposEtapaCTE");
            tipoOperacao.NaoIntegrarOpentech = Request.GetBoolParam("NaoIntegrarOpentech");
            tipoOperacao.IntegrarPedidosNaIntegracaoOpentech = Request.GetBoolParam("IntegrarPedidosNaIntegracaoOpentech");
            tipoOperacao.ValorMinimoMercadoriaOpenTech = Request.GetDecimalParam("ValorMinimoMercadoriaOpenTech");
            tipoOperacao.NotificarRemetentePorEmailAoSolicitarNotas = Request.GetBoolParam("NotificarRemetentePorEmailAoSolicitarNotas");
            tipoOperacao.ValidarMotoristaTeleriscoAoConfirmarTransportador = Request.GetBoolParam("ValidarMotoristaTeleriscoAoConfirmarTransportador");
            tipoOperacao.ValidarMotoristaVeiculoBrasilRiskAoConfirmarTransportador = Request.GetBoolParam("ValidarMotoristaVeiculoBrasilRiskAoConfirmarTransportador");
            tipoOperacao.IntegrarDadosTransporteBrasilRiskAoAtualizarVeiculoMotorista = Request.GetBoolParam("IntegrarDadosTransporteBrasilRiskAoAtualizarVeiculoMotorista");
            tipoOperacao.ValidarMotoristaVeiculoAdagioAoConfirmarTransportador = Request.GetBoolParam("ValidarMotoristaVeiculoAdagioAoConfirmarTransportador");
            tipoOperacao.ValidarMotoristaBuonnyAoConfirmarTransportador = Request.GetBoolParam("ValidarMotoristaBuonnyAoConfirmarTransportador");
            tipoOperacao.NaoGerarCanhoto = Request.GetBoolParam("NaoGerarCanhoto");
            tipoOperacao.UtilizarMaiorDistanciaPedidoNaMontagemCarga = Request.GetBoolParam("UtilizarMaiorDistanciaPedidoNaMontagemCarga");
            tipoOperacao.EmitirNFeRemessa = Request.GetBoolParam("EmitirNFeRemessa");
            tipoOperacao.PermitirCargaSemAverbacao = Request.GetBoolParam("PermitirCargaSemAverbacao");
            tipoOperacao.ExigeChaveVendaAntesConfirmarNotas = Request.GetBoolParam("ExigeChaveVendaAntesConfirmarNotas");
            tipoOperacao.GerarNovoNumeroCargaNoRedespacho = Request.GetBoolParam("GerarNovoNumeroCargaNoRedespacho");
            tipoOperacao.SolicitarNotasFiscaisAoSalvarDadosTransportador = Request.GetBoolParam("SolicitarNotasFiscaisAoSalvarDadosTransportador");
            tipoOperacao.SelecionarRetiradaProduto = Request.GetBoolParam("SelecionarRetiradaProduto");
            tipoOperacao.BloquearAlteracaoHorarioCarregamentoCarga = Request.GetBoolParam("BloquearAlteracaoHorarioCarregamentoCarga");
            tipoOperacao.PermitirValorFreteInformadoPeloEmbarcadorZerado = Request.GetBoolParam("PermitirValorFreteInformadoPeloEmbarcadorZerado");
            tipoOperacao.PermitirImportarCTeComChaveNFeDiferenteNoPreCTe = Request.GetBoolParam("PermitirImportarCTeComChaveNFeDiferenteNoPreCTe");
            tipoOperacao.ImprimirRelatorioRomaneioEtapaImpressaoCarga = Request.GetBoolParam("ImprimirRelatorioRomaneioEtapaImpressaoCarga");
            tipoOperacao.NaoExibirDetalhesDoFretePortalTransportador = Request.GetBoolParam("NaoExibirDetalhesDoFretePortalTransportador");
            tipoOperacao.PermitidoSelecionarTipoDeOperacaoNaMontagemDaCarga = Request.GetBoolParam("PermitidoSelecionarTipoDeOperacaoNaMontagemDaCarga");
            tipoOperacao.DevolucaoProdutosPorPeso = Request.GetBoolParam("DevolucaoProdutosPorPeso");
            tipoOperacao.PermiteInformarPesoCubadoNaMontagemDaCarga = Request.GetBoolParam("PermiteInformarPesoCubadoNaMontagemDaCarga");
            tipoOperacao.NaoExigeRoteirizacaoMontagemCarga = Request.GetBoolParam("NaoExigeRoteirizacaoMontagemCarga");
            tipoOperacao.NaoDisponibilizarCargaParaIntegracaoERP = Request.GetBoolParam("NaoDisponibilizarCargaParaIntegracaoERP");
            tipoOperacao.PermiteAdicionarPedidoCargaFechada = Request.GetBoolParam("PermiteAdicionarPedidoCargaFechada");
            tipoOperacao.NaoPermiteAvancarCargaSemDataPrevisaoDeEntrega = Request.GetBoolParam("NaoPermiteAvancarCargaSemDataPrevisaoDeEntrega");
            tipoOperacao.GerarPedidoAoReceberCarga = Request.GetBoolParam("GerarPedidoAoReceberCarga");
            tipoOperacao.UtilizarValorFreteNotasFiscais = Request.GetBoolParam("UtilizarValorFreteNotasFiscais");
            tipoOperacao.InserirDadosContabeisXCampoXTextCTe = Request.GetBoolParam("InserirDadosContabeisXCampoXTextCTe");
            tipoOperacao.PermitirEnviarImagemParaMultiplosCanhotos = Request.GetBoolParam("PermitirEnviarImagemParaMultiplosCanhotos");
            tipoOperacao.PermitirInformarDataEntregaParaMultiplosCanhotos = Request.GetBoolParam("PermitirInformarDataEntregaParaMultiplosCanhotos");
            tipoOperacao.UtilizarPlanoViagem = Request.GetBoolParam("UtilizarPlanoViagem");
            tipoOperacao.EnviarEmailPlanoViagemSolicitarNotasCarga = Request.GetBoolParam("EnviarEmailPlanoViagemSolicitarNotasCarga");
            tipoOperacao.EnviarEmailPlanoViagemFinalizarCarga = Request.GetBoolParam("EnviarEmailPlanoViagemFinalizarCarga");
            tipoOperacao.NaoNecessarioConfirmarImpressaoDocumentos = Request.GetBoolParam("NaoNecessarioConfirmarImpressaoDocumentos");
            tipoOperacao.PermiteAlterarDataInicioCarregamentoNoControleEntrega = Request.GetBoolParam("PermiteAlterarDataInicioCarregamentoNoControleEntrega");
            tipoOperacao.PermiteAlterarHorarioCarregamentoCargasFaturadas = Request.GetBoolParam("PermiteAlterarHorarioCarregamentoCargasFaturadas");
            tipoOperacao.AlterarDataJanelaCarregamentoAoAtualizarDataAgendamentoEntregaPedido = Request.GetBoolParam("AlterarDataJanelaCarregamentoAoAtualizarDataAgendamentoEntregaPedido");
            tipoOperacao.BloquearLiberacaoCargasComPedidosDatasAgendadasDivergentes = Request.GetBoolParam("BloquearLiberacaoCargasComPedidosDatasAgendadasDivergentes");
            tipoOperacao.GerarLoteEntregaAutomaticamente = Request.GetBoolParam("GerarLoteEntregaAutomaticamente");
            tipoOperacao.PermiteRealizarImpressaoCarga = Request.GetBoolParam("PermiteRealizarImpressaoCarga");
            tipoOperacao.ImprimirCRT = Request.GetBoolParam("ImprimirCRT");
            tipoOperacao.PermitirTransportadorConfirmarRejeitarEntrega = Request.GetBoolParam("PermitirTransportadorConfirmarRejeitarEntrega");
            tipoOperacao.NotificarTransportadorAoAgendarEntrega = Request.GetBoolParam("NotificarTransportadorAoAgendarEntrega");
            tipoOperacao.PermitirAgendarEntregaSomenteAposInicioViagemCarga = Request.GetBoolParam("PermitirAgendarEntregaSomenteAposInicioViagemCarga");
            tipoOperacao.NaoPermitirGerarComissaoMotorista = Request.GetBoolParam("NaoPermitirGerarComissaoMotorista");
            tipoOperacao.NaoPermitirEmitirCargaComMesmoNumeroOutroDocumento = Request.GetBoolParam("NaoPermitirEmitirCargaComMesmoNumeroOutroDocumento");
            tipoOperacao.OcultarCargasComEsseTipoOperacaoNoPortalTransportador = Request.GetBoolParam("OcultarCargasComEsseTipoOperacaoNoPortalTransportador");
            tipoOperacao.ConsiderarApenasDiasUteisNaPrevisaoDeEntrega = Request.GetBoolParam("ConsiderarApenasDiasUteisNaPrevisaoDeEntrega");
            tipoOperacao.EnviarLinkAcompanhamentoParaClienteEntrega = Request.GetBoolParam("EnviarLinkAcompanhamentoParaClienteEntrega");
            tipoOperacao.DocumentoXCampo = Request.GetStringParam("DocumentoXCampo");
            tipoOperacao.DocumentoXTexto = Request.GetStringParam("DocumentoXTexto");
            tipoOperacao.UtilizarXCampoSomenteNoRedespacho = Request.GetBoolParam("UtilizarXCampoSomenteNoRedespacho");
            tipoOperacao.NaoExigirQueEntregasSejamAgendadasComCliente = Request.GetBoolParam("NaoExigirQueEntregasSejamAgendadasComCliente");
            tipoOperacao.PossibilitarInicioViagemViaGuarita = Request.GetBoolParam("PossibilitarInicioViagemViaGuarita");
            tipoOperacao.BloquearAvancoCargaVolumesZerados = Request.GetBoolParam("BloquearAvancoCargaVolumesZerados");
            tipoOperacao.PermitirAtualizarEntregasCargasFinalizadas = Request.GetBoolParam("PermitirAtualizarEntregasCargasFinalizadas");
            tipoOperacao.HabilitarCobrancaEstadiaAutomaticaPeloTracking = Request.GetBoolParam("HabilitarCobrancaEstadiaAutomaticaPeloTracking");
            tipoOperacao.ComponenteFrete = componenteFrete > 0 ? new Dominio.Entidades.Embarcador.Frete.ComponenteFrete() { Codigo = componenteFrete } : null;
            tipoOperacao.TipoOcorrencia = tipoOcorrencia > 0 ? new Dominio.Entidades.TipoDeOcorrenciaDeCTe() { Codigo = tipoOcorrencia } : null;
            tipoOperacao.VincularApenasUmaNotaPorEntrega = Request.GetBoolParam("VincularApenasUmaNotaPorEntrega");

            tipoOperacao.TempoMinimoCobrancaEstadia = Request.GetIntParam("TempoMinimoCobrancaEstadia");
            tipoOperacao.ModeloCobrancaEstadiaTracking = Request.GetEnumParam<ModeloCobrancaEstadiaTracking>("ModeloCobrancaEstadiaTracking");
            tipoOperacao.PermiteEnviarNotasComplementaresAposEmissaoDocumentosTransporte = Request.GetBoolParam("PermiteEnviarNotasComplementaresAposEmissaoDocumentosTransporte");
            tipoOperacao.PermitirAdicionarPedidoReentregaAposInicioViagem = Request.GetBoolParam("PermitirAdicionarPedidoReentregaAposInicioViagem");
            tipoOperacao.AtualizarRotaRealizadaDoMonitoramento = Request.GetBoolParam("AtualizarRotaRealizadaDoMonitoramento");
            tipoOperacao.NaoGerarMonitoramento = Request.GetBoolParam("NaoGerarMonitoramento");
            tipoOperacao.NaoUtilizarRecebedorDaNotaFiscal = Request.GetBoolParam("NaoUtilizarRecebedorDaNotaFiscal");
            tipoOperacao.OperacaoInsumos = Request.GetBoolParam("OperacaoInsumos");
            tipoOperacao.InformarDadosNotaCte = Request.GetBoolParam("InformarDadosNotaCte");
            tipoOperacao.ManterOrdemAoRoteirizarAgendaEntrega = Request.GetBoolParam("ManterOrdemAoRoteirizarAgendaEntrega");
            tipoOperacao.HabilitarTipoPagamentoValePedagio = Request.GetBoolParam("HabilitarTipoPagamentoValePedagio");
            tipoOperacao.TipoPagamentoValePedagio = Request.GetEnumParam<Dominio.Enumeradores.TipoPagamentoValePedagio>("TipoPagamentoValePedagio");
            tipoOperacao.GerarEntregaPorNotaFiscalCarga = Request.GetBoolParam("GerarEntregaPorNotaFiscalCarga");
            tipoOperacao.QuandoProcessarMonitoramento = Request.GetEnumParam<QuandoProcessarMonitoramento>("QuandoProcessarMonitoramento");
            tipoOperacao.NaoRealizarIntegracaoComAX = Request.GetNullableBoolParam("NaoRealizarIntegracaoComAX");
            tipoOperacao.RealizarIntegracaoComMicDta = Request.GetBoolParam("RealizarIntegracaoComMicDta");
            tipoOperacao.IntegrarMICDTAComSiscomex = Request.GetBoolParam("IntegrarMICDTAComSiscomex");
            tipoOperacao.PermitirConsultaDeValoresPedagioSemParar = Request.GetBoolParam("PermitirConsultaDeValoresPedagioSemParar");
            tipoOperacao.ConsultaDeValoresPedagioAdicionarComponenteFrete = Request.GetBoolParam("ConsultaDeValoresPedagioAdicionarComponenteFrete");
            tipoOperacao.ExigeSenhaConfirmacaoEntrega = Request.GetBoolParam("ExigeSenhaConfirmacaoEntrega");
            tipoOperacao.NumeroTentativaSenhaConfirmacaoEntrega = tipoOperacao.ExigeSenhaConfirmacaoEntrega ? Request.GetIntParam("NumeroTentativaSenhaConfirmacaoEntrega") : 0;
            tipoOperacao.EnviarComprovanteEntregaAposFinalizacaoEntrega = Request.GetBoolParam("EnviarComprovanteEntregaAposFinalizacaoEntrega");
            tipoOperacao.IniciarViagemPeloStatusViagem = Request.GetBoolParam("IniciarViagemPeloStatusViagem");
            tipoOperacao.InicioViagemPorCargaGerada = Request.GetBoolParam("InicioViagemPorCargaGerada");

            tipoOperacao.CodigoCondicaoPagamento = Request.GetStringParam("CodigoCondicaoPagamento");

            tipoOperacao.GerarOcorrenciaPedidoEntregueForaPrazo = Request.GetBoolParam("GerarOcorrenciaPedidoEntregueForaPrazo");
            tipoOperacao.TipoOcorrenciaPedidoEntregueForaPrazo = Request.GetBoolParam("GerarOcorrenciaPedidoEntregueForaPrazo") && tipoOcorrenciaPedidoEntregueForaPrazo > 0 ? new Dominio.Entidades.TipoDeOcorrenciaDeCTe() { Codigo = tipoOcorrenciaPedidoEntregueForaPrazo } : null;

            tipoOperacao.ExigirTermoAceite = Request.GetBoolParam("ExigirTermoAceite");
            tipoOperacao.TermoAceite = Request.GetStringParam("TermoAceite");

            tipoOperacao.NaoGerarControlePaletes = Request.GetBoolParam("NaoGerarControlePaletes");
            tipoOperacao.EnviarCTesPorWebService = Request.GetBoolParam("EnviarCTesPorWebService");
            tipoOperacao.EnviarSeguroAverbacaoPorWebService = Request.GetBoolParam("EnviarSeguroAverbacaoPorWebService");
            tipoOperacao.ReceberCTesAverbacaoPorWebService = Request.GetBoolParam("ReceberCTesAverbacaoPorWebService");
            tipoOperacao.OperacaoDestinadaCTeComplementar = Request.GetBoolParam("OperacaoDestinadaCTeComplementar");
            tipoOperacao.PermitirInformarRecebedorMontagemCarga = Request.GetBoolParam("PermitirInformarRecebedorMontagemCarga");
            tipoOperacao.OperacaoTransferenciaContainer = Request.GetBoolParam("OperacaoTransferenciaContainer");
            tipoOperacao.NaoPermitirFinalizarEntregaRejeitada = Request.GetBoolParam("NaoPermitirFinalizarEntregaRejeitada");
            tipoOperacao.IniciarMonitoramentoAutomaticamenteDataCarregamento = Request.GetBoolParam("IniciarMonitoramentoAutomaticamenteDataCarregamento");

            tipoOperacao.UsarComoPadraoParaFretesForaDoPais = Request.GetBoolParam("TipoOperacaoPadraoParaFretesForaDoPais");
            tipoOperacao.UsarComoPadraoParaFretesDentroDoPais = Request.GetBoolParam("TipoOperacaoPadraoParaFretesDentroDoPais");

            tipoOperacao.PermitirAlterarDataChegadaVeiculo = Request.GetBoolParam("PermitirAlterarDataChegadaVeiculo");
            tipoOperacao.ExigirJustificativaParaEncerramentoManualViagem = Request.GetBoolParam("ExigirJustificativaParaEncerramentoManualViagem");
            tipoOperacao.PermiteImprimirOrdemColetaNaGuarita = Request.GetBoolParam("PermiteImprimirOrdemColetaNaGuarita");
            tipoOperacao.NaoUtilizarConfiguracoesDeComprovantesDoGrupoPessoa = Request.GetBoolParam("NaoUtilizarConfiguracoesDeComprovantesDoGrupoPessoa");


            tipoOperacao.HabilitarOutraConfiguracaoOpenTech = Request.GetBoolParam("HabilitarOutraConfiguracaoOpenTech");
            tipoOperacao.UsuarioOpenTech = Request.GetStringParam("UsuarioOpenTech");
            tipoOperacao.SenhaOpenTech = Request.GetStringParam("SenhaOpenTech");
            tipoOperacao.DominioOpenTech = Request.GetStringParam("DominioOpenTech");
            tipoOperacao.CodigoClienteOpenTech = Request.GetIntParam("CodigoClienteOpenTech");
            tipoOperacao.CodigoPASOpenTech = Request.GetIntParam("CodigoPASOpenTech");
            tipoOperacao.URLOpenTech = Request.GetStringParam("URLOpenTech");
            tipoOperacao.CodigoProdutoPadraoOpentech = Request.GetIntParam("CodigoProdutoPadraoOpentech");
            tipoOperacao.CodigoTransportadorOpenTech = Request.GetIntParam("CodigoTransportadorOpenTech");

            tipoOperacao.EnviarPesoLiquidoLinkNotas = Request.GetBoolParam("EnviarPesoLiquidoLinkNotas");
            tipoOperacao.HabilitarAppTrizy = Request.GetBoolParam("HabilitarAppTrizy");


            // Checklists
            int codigoChecklistDesembarque = Request.GetIntParam("CheckListDesembarque");
            Dominio.Entidades.Embarcador.GestaoPatio.CheckListTipo checkListDesembarque = codigoChecklistDesembarque > 0 ? repCheckListTipo.BuscarPorCodigo(codigoChecklistDesembarque) : null;
            tipoOperacao.CheckListDesembarque = checkListDesembarque;

            int codigoChecklistEntrega = Request.GetIntParam("CheckListEntrega");
            Dominio.Entidades.Embarcador.GestaoPatio.CheckListTipo checkListEntrega = codigoChecklistEntrega > 0 ? repCheckListTipo.BuscarPorCodigo(codigoChecklistEntrega) : null;
            tipoOperacao.CheckListEntrega = checkListEntrega;

            int codigoChecklistColeta = Request.GetIntParam("CheckListColeta");
            Dominio.Entidades.Embarcador.GestaoPatio.CheckListTipo checkListColeta = codigoChecklistColeta > 0 ? repCheckListTipo.BuscarPorCodigo(codigoChecklistColeta) : null;
            tipoOperacao.CheckListColeta = checkListColeta;

            int codigoTipoRetornoCarga = Request.GetIntParam("TipoRetornoCarga");
            Dominio.Entidades.Embarcador.Cargas.Retornos.TipoRetornoCarga tipoRetornoCarga = codigoTipoRetornoCarga > 0 ? repTipoRetornoCarga.BuscarPorCodigo(codigoTipoRetornoCarga) : null;
            tipoOperacao.TipoRetornoCarga = tipoRetornoCarga;

            tipoOperacao.Expedidor = cpfCnpjExpedidor > 0 ? repositorioCliente.BuscarPorCPFCNPJ(cpfCnpjExpedidor) : null;
            tipoOperacao.Recebedor = cpfCnpjRecebedor > 0 ? repositorioCliente.BuscarPorCPFCNPJ(cpfCnpjRecebedor) : null;
            tipoOperacao.TipoOperacaoRedespacho = tipoOperacaoRedespacho > 0 ? repTipoOperacao.BuscarPorCodigo(tipoOperacaoRedespacho) : null;
            tipoOperacao.TipoOperacaoPrecheckin = tipoOperacaoPreCheking > 0 ? repTipoOperacao.BuscarPorCodigo(tipoOperacaoPreCheking) : null;
            tipoOperacao.TipoOperacaoPrecheckinTransferencia = tipoOperacaoPreChekingTransferencia > 0 ? repTipoOperacao.BuscarPorCodigo(tipoOperacaoPreChekingTransferencia) : null;

        }

        private dynamic ObterProdutosPadroes(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, Repositorio.UnitOfWork unitOfWork, bool duplicar)
        {
            Repositorio.Embarcador.Pedidos.TipoOperacaoProdutosPadrao repTipoOperacaoProdutoPadrao = new Repositorio.Embarcador.Pedidos.TipoOperacaoProdutosPadrao(unitOfWork);

            List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador> produtos = repTipoOperacaoProdutoPadrao.BuscarProdutosPorTipoOperacao(tipoOperacao.Codigo);
            return produtos.Select(obj => new
            {
                Codigo = duplicar ? 0 : obj.Codigo,
                Descricao = obj.Descricao,
            }).ToList();
        }

        private dynamic ObterTipoOperacaoFilialMotoristaGenerico(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.TipoOperacaoFilialMotoristaGenerico repositorioTipoOperacaoFilialMotoristaGenerico = new Repositorio.Embarcador.Pedidos.TipoOperacaoFilialMotoristaGenerico(unitOfWork);

            List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoFilialMotoristaGenerico> tiposOperacaoFilialMotoristaGenericos = repositorioTipoOperacaoFilialMotoristaGenerico.BuscarFiliaisMotoristasPorTipoOperacao(tipoOperacao.Codigo);
            return tiposOperacaoFilialMotoristaGenericos.Select(obj => new
            {
                Codigo = obj.Filial.Codigo,
                Descricao = obj.Filial.Descricao,
                MotoristaCodigo = obj.Motorista.Codigo,
                MotoristaDescricao = obj.Motorista.Descricao
            }).ToList();
        }

        private dynamic ObterTiposOcorrencia(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, Repositorio.UnitOfWork unitOfWork, bool duplicar)
        {
            return tipoOperacao.TiposOcorrencia.Select(obj => new
            {
                Codigo = duplicar ? 0 : obj.Codigo,
                obj.Descricao,
            }).ToList();
        }

        private dynamic ObterGrupoTomadoresBloqueados(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, Repositorio.UnitOfWork unitOfWork, bool duplicar)
        {
            Repositorio.Embarcador.Pedidos.TipoOperacaoGrupoTomadorBloqueado repTipoOperacaoGrupoTomadorBloqueado = new Repositorio.Embarcador.Pedidos.TipoOperacaoGrupoTomadorBloqueado(unitOfWork);
            List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoGrupoTomadorBloqueado> grupoTomadoresBloqueados = repTipoOperacaoGrupoTomadorBloqueado.BuscarPorTipoOperacao(tipoOperacao.Codigo);

            return grupoTomadoresBloqueados
                .Select(obj => new
                {
                    obj.GrupoPessoas.Codigo,
                    obj.GrupoPessoas.Descricao,
                }).ToList();
        }

        private dynamic ObterLayoutEDI(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao)
        {
            return tipoOperacao.LayoutsEDI.OrderBy(o => o.LayoutEDI.Descricao).Select(obj => new
            {
                obj.Codigo,
                CodigoLayoutEDI = obj.LayoutEDI.Codigo,
                DescricaoLayoutEDI = obj.LayoutEDI.Descricao,
                TipoIntegracao = obj.TipoIntegracao.Tipo,
                DescricaoTipoIntegracao = obj.TipoIntegracao.Descricao,
                obj.Diretorio,
                obj.Emails,
                obj.EnderecoFTP,
                obj.Passivo,
                obj.UtilizarSFTP,
                obj.SSL,
                obj.Porta,
                obj.Senha,
                obj.Usuario,
                obj.CriarComNomeTemporaraio
            }).ToList();
        }

        private dynamic ObterApolicesSeguro(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao)
        {
            return tipoOperacao.ApolicesSeguro.Select(obj => new
            {
                obj.ApoliceSeguro.Codigo,
                Seguradora = obj.ApoliceSeguro.Seguradora.Nome,
                obj.ApoliceSeguro.NumeroApolice,
                obj.ApoliceSeguro.NumeroAverbacao,
                Responsavel = obj.ApoliceSeguro.DescricaoResponsavel,
                Vigencia = obj.ApoliceSeguro.InicioVigencia.ToString("dd/MM/yyyy") + Localization.Resources.Pedidos.TipoOperacao.Ate + obj.ApoliceSeguro.FimVigencia.ToString("dd/MM/yyyy")
            }).ToList();
        }

        private dynamic ObterComponentesFrete(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao)
        {
            return tipoOperacao.TipoOperacaoConfiguracoesComponentes.Select(obj => new
            {
                Codigo = obj.ComponenteFrete.Codigo,
                ComponenteFrete = new { obj.ComponenteFrete.Codigo, obj.ComponenteFrete.Descricao },
                CobrarOutroDocumento = obj.ModeloDocumentoFiscal != null ? true : false,
                ModeloDocumentoFiscal = obj.ModeloDocumentoFiscal != null ? new { obj.ModeloDocumentoFiscal.Codigo, obj.ModeloDocumentoFiscal.Descricao } : new { Codigo = 0, Descricao = "" },
                ImprimirOutraDescricaoCTe = !string.IsNullOrEmpty(obj.OutraDescricaoCTe) ? true : false,
                DescricaoCTe = obj.OutraDescricaoCTe,
                UsarOutraFormulaRateio = obj.RateioFormula != null ? true : false,
                FormulaRateioFrete = obj.RateioFormula != null ? new { obj.RateioFormula.Codigo, obj.RateioFormula.Descricao } : new { Codigo = 0, Descricao = "" },
                obj.IncluirICMS,
                obj.IncluirIntegralmenteContratoFreteTerceiro
            }).ToList();
        }

        private dynamic ObterClientesBloqueados(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao)
        {
            return tipoOperacao.ClientesBloquearEmissaoDosDestinatario.Select(obj => new
            {
                obj.Codigo,
                CPF_CNPJ = obj.CPF_CNPJ,
                obj.Nome
            }).ToList();
        }

        private dynamic ObterConfiguracaoEmissaoCTe(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.TipoOperacaoIntegracaoFTP repositorioTipoOperacaoIntegracaoFTP = new Repositorio.Embarcador.Pedidos.TipoOperacaoIntegracaoFTP(unitOfWork);
            Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoIntegracaoFTP tipoOperacaoIntegracaoFTP = repositorioTipoOperacaoIntegracaoFTP.BuscarPorTipoOperacaoAsync(tipoOperacao.Codigo, default).GetAwaiter().GetResult();

            return new
            {
                TipoIntegracaoMercadoLivre = tipoOperacao.TipoIntegracaoMercadoLivre ?? TipoIntegracaoMercadoLivre.HandlingUnit,
                tipoOperacao.IntegracaoMercadoLivreRealizarConsultaRotaEFacilityAutomaticamente,
                tipoOperacao.IntegracaoMercadoLivreAvancarCargaEtapaNFeAutomaticamente,
                tipoOperacao.TipoTempoAcrescimoDecrescimoDataPrevisaoSaida,
                TempoAcrescimoDecrescimoDataPrevisaoSaida = $"{(int)tipoOperacao.TempoAcrescimoDecrescimoDataPrevisaoSaida.TotalHours:d3}:{tipoOperacao.TempoAcrescimoDecrescimoDataPrevisaoSaida:mm}",
                tipoOperacao.ValorFreteLiquidoDeveSerValorAReceber,
                tipoOperacao.ValorFreteLiquidoDeveSerValorAReceberSemICMS,
                tipoOperacao.GerarCIOTParaTodasAsCargas,
                ValorMaximoEmissaoPendentePagamento = (tipoOperacao.ValorMaximoEmissaoPendentePagamento ?? 0m).ToString("n2"),
                TipoEnvioEmail = tipoOperacao.TipoEnvioEmail ?? TipoEnvioEmailCTe.Normal,
                tipoOperacao.ObservacaoEmissaoCarga,
                tipoOperacao.DescricaoItemPesoCTeSubcontratacao,
                tipoOperacao.CaracteristicaTransporteCTe,
                tipoOperacao.ImportarRedespachoIntermediario,
                EmitenteImportacaoRedespachoIntermediario = new
                {
                    Codigo = tipoOperacao.EmitenteImportacaoRedespachoIntermediario?.CPF_CNPJ_SemFormato ?? string.Empty,
                    Descricao = tipoOperacao.EmitenteImportacaoRedespachoIntermediario?.Descricao ?? string.Empty
                },
                ExpedidorImportacaoRedespachoIntermediario = new
                {
                    Codigo = tipoOperacao.ExpedidorImportacaoRedespachoIntermediario?.CPF_CNPJ_SemFormato ?? string.Empty,
                    Descricao = tipoOperacao.ExpedidorImportacaoRedespachoIntermediario?.Descricao ?? string.Empty
                },
                RecebedorImportacaoRedespachoIntermediario = new
                {
                    Codigo = tipoOperacao.RecebedorImportacaoRedespachoIntermediario?.CPF_CNPJ_SemFormato ?? string.Empty,
                    Descricao = tipoOperacao.RecebedorImportacaoRedespachoIntermediario?.Descricao ?? string.Empty
                },
                tipoOperacao.GerarMDFeTransbordoSemConsiderarOrigem,
                tipoOperacao.BloquearDiferencaValorFreteEmbarcador,
                PercentualBloquearDiferencaValorFreteEmbarcador = tipoOperacao.PercentualBloquearDiferencaValorFreteEmbarcador.ToString("n2"),
                tipoOperacao.EmitirComplementoDiferencaFreteEmbarcador,
                TipoOcorrenciaComplementoDiferencaFreteEmbarcador = new
                {
                    Descricao = tipoOperacao.TipoOcorrenciaComplementoDiferencaFreteEmbarcador?.Descricao ?? string.Empty,
                    Codigo = tipoOperacao.TipoOcorrenciaComplementoDiferencaFreteEmbarcador?.Codigo ?? 0
                },
                tipoOperacao.GerarOcorrenciaSemTabelaFrete,
                TipoOcorrenciaSemTabelaFrete = new
                {
                    Descricao = tipoOperacao.TipoOcorrenciaSemTabelaFrete?.Descricao ?? string.Empty,
                    Codigo = tipoOperacao.TipoOcorrenciaSemTabelaFrete?.Codigo ?? 0
                },
                TipoOcorrenciaCTeEmitidoEmbarcador = new
                {
                    Descricao = tipoOperacao.TipoOcorrenciaCTeEmitidoEmbarcador?.Descricao ?? string.Empty,
                    Codigo = tipoOperacao.TipoOcorrenciaCTeEmitidoEmbarcador?.Codigo ?? 0
                },
                NaoValidarNotaFiscalExistente = tipoOperacao.NaoValidarNotaFiscalExistente,
                NaoValidarNotasFiscaisComDiferentesPortos = tipoOperacao.NaoValidarNotasFiscaisComDiferentesPortos,
                AgruparMovimentoFinanceiroPorPedido = tipoOperacao.AgruparMovimentoFinanceiroPorPedido,
                tipoOperacao.ValePedagioObrigatorio,
                Diretorio = tipoOperacaoIntegracaoFTP?.Diretorio ?? string.Empty,
                EnderecoFTP = tipoOperacaoIntegracaoFTP?.EnderecoFTP ?? string.Empty,
                Passivo = tipoOperacaoIntegracaoFTP?.Passivo ?? false,
                Porta = tipoOperacaoIntegracaoFTP?.Porta ?? string.Empty,
                Senha = tipoOperacaoIntegracaoFTP?.Senha ?? string.Empty,
                UtilizarSFTP = tipoOperacaoIntegracaoFTP?.UtilizarSFTP ?? false,
                SSL = tipoOperacaoIntegracaoFTP?.SSL ?? false,
                Usuario = tipoOperacaoIntegracaoFTP?.Usuario ?? string.Empty,
                NomenclaturaArquivo = tipoOperacaoIntegracaoFTP?.NomenclaturaArquivo ?? string.Empty,
                ModeloDocumentoFiscal = tipoOperacao.ModeloDocumentoFiscal != null ? new { tipoOperacao.ModeloDocumentoFiscal.Codigo, tipoOperacao.ModeloDocumentoFiscal.Descricao } : new { Codigo = 0, Descricao = "" },
                DisponibilizarDocumentosParaNFsManual = tipoOperacao.DisponibilizarDocumentosParaNFsManual,
                EmpresaEmissora = tipoOperacao.EmpresaEmissora != null ? new { tipoOperacao.EmpresaEmissora.Codigo, Descricao = tipoOperacao.EmpresaEmissora.RazaoSocial + " (" + tipoOperacao.EmpresaEmissora.Localidade.DescricaoCidadeEstado + ")" } : new { Codigo = 0, Descricao = "" },
                EmitirEmpresaFixa = tipoOperacao.EmpresaEmissora != null ? true : false,
                CobrarOutroDocumento = tipoOperacao.ModeloDocumentoFiscal != null ? true : false,
                TipoRateioDocumentos = tipoOperacao.TipoEmissaoCTeDocumentos,
                tipoOperacao.CTeEmitidoNoEmbarcador,
                tipoOperacao.ExigirNumeroPedido,
                tipoOperacao.RegexValidacaoNumeroPedidoEmbarcador,
                tipoOperacao.TipoEmissaoCTeParticipantes,
                tipoOperacao.TipoEmissaoIntramunicipal,
                tipoOperacao.DescricaoComponenteFreteEmbarcador,
                tipoOperacao.UtilizarOutroModeloDocumentoEmissaoMunicipal,
                tipoOperacao.NaoEmitirMDFe,
                tipoOperacao.ProvisionarDocumentos,
                tipoOperacao.DisponibilizarDocumentosParaLoteEscrituracao,
                tipoOperacao.DisponibilizarDocumentosParaLoteEscrituracaoCancelamento,
                tipoOperacao.DisponibilizarDocumentosParaPagamento,
                tipoOperacao.QuitarDocumentoAutomaticamenteAoGerarLote,
                tipoOperacao.EscriturarSomenteDocumentosEmitidosParaNFe,
                ObrigatorioInformarMDFeEmitidoPeloEmbarcador = tipoOperacao?.ObrigatorioInformarMDFeEmitidoPeloEmbarcador ?? false,
                ModeloDocumentoFiscalEmissaoMunicipal = new
                {
                    Codigo = tipoOperacao.ModeloDocumentoFiscalEmissaoMunicipal?.Codigo ?? 0,
                    Descricao = tipoOperacao.ModeloDocumentoFiscalEmissaoMunicipal?.Descricao ?? string.Empty
                },
                ArquivoImportacaoNotasFiscais = new
                {
                    Codigo = tipoOperacao.ArquivoImportacaoNotaFiscal?.Codigo ?? 0,
                    Descricao = tipoOperacao.ArquivoImportacaoNotaFiscal?.Descricao ?? string.Empty
                },
                FormulaRateioFrete = new
                {
                    Codigo = tipoOperacao.RateioFormula?.Codigo ?? 0,
                    Descricao = tipoOperacao.RateioFormula?.Descricao ?? string.Empty,
                    ParametroRateioFormula = tipoOperacao.RateioFormula?.ParametroRateioFormula ?? null
                },
                TipoIntegracao = tipoOperacao.TipoIntegracao?.Tipo ?? TipoIntegracao.NaoInformada,
                ApolicesSeguro = ObterApolicesSeguro(tipoOperacao),
                ComponentesFrete = ObterComponentesFrete(tipoOperacao),
                ClientesBloqueados = ObterClientesBloqueados(tipoOperacao),
                tipoOperacao.TipoPropostaMultimodal,
                tipoOperacao.TipoServicoMultimodal,
                tipoOperacao.ModalPropostaMultimodal,
                tipoOperacao.TipoCobrancaMultimodal,
                tipoOperacao.BloquearEmissaoDeEntidadeSemCadastro,
                tipoOperacao.BloquearEmissaoDosDestinatario,
                Observacao = tipoOperacao.ObservacaoCTe,
                ObservacaoTerceiro = tipoOperacao.ObservacaoCTeTerceiro,
                tipoOperacao.NaoPermitirVincularCTeComplementarEmCarga,
                tipoOperacao.GerarOcorrenciaComplementoSubcontratacao,
                TempoCarregamento = $"{(int)tipoOperacao.TempoCarregamento.TotalHours:d3}:{tipoOperacao.TempoCarregamento:mm}",
                TempoDescarregamento = $"{(int)tipoOperacao.TempoDescarregamento.TotalHours:d3}:{tipoOperacao.TempoDescarregamento:mm}",
                TipoOcorrenciaComplementoSubcontratacao = new
                {
                    Codigo = tipoOperacao.TipoOcorrenciaComplementoSubcontratacao?.Codigo ?? 0,
                    Descricao = tipoOperacao.TipoOcorrenciaComplementoSubcontratacao?.Descricao
                },
                UtilizarPrimeiraUnidadeMedidaPesoCTeSubcontratacao = tipoOperacao.ConfiguracaoEmissao?.UtilizarPrimeiraUnidadeMedidaPesoCTeSubcontratacao ?? false,
                LiberarDocumentosEmitidosQuandoEntregaForConfirmada = tipoOperacao.ConfiguracaoEmissao?.LiberarDocumentosEmitidosQuandoEntregaForConfirmada ?? false,
                DisponibilizarComposicaoRateioCarga = tipoOperacao.ConfiguracaoEmissao?.DisponibilizarComposicaoRateioCarga ?? false,
                AverbarCTeImportadoDoEmbarcador = tipoOperacao.ConfiguracaoEmissao?.AverbarCTeImportadoDoEmbarcador,
                TipoUsoFatorCubagemRateioFormula = tipoOperacao.ConfiguracaoEmissao?.TipoUsoFatorCubagemRateioFormula ?? null,
                FatorCubagemRateioFormula = tipoOperacao.ConfiguracaoEmissao?.FatorCubagemRateioFormula?.ToString("n2") ?? "",
                TipoReceita = tipoOperacao.ConfiguracaoEmissao?.TipoReceita ?? null
            };
        }

        private dynamic ObterIntegracoes(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao)
        {
            return tipoOperacao.Integracoes.Select(o => new
            {
                o.Codigo,
                Tipo = o.Tipo,
                Descricao = o.Tipo.ObterDescricao()
            }).ToList();
        }

        private dynamic ObterTransportadores(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, Repositorio.UnitOfWork unitOfWork, bool duplicar)
        {
            Repositorio.Embarcador.Pedidos.TipoOperacaoTransportador repositorioTipoOperacaoTransportador = new Repositorio.Embarcador.Pedidos.TipoOperacaoTransportador(unitOfWork);

            List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoTransportador> transportadores = repositorioTipoOperacaoTransportador.BuscarPorTipoOperacao(tipoOperacao.Codigo);

            return transportadores.Select(o => new
            {
                Codigo = duplicar ? 0 : o.Codigo,
                o.TipoUltimoPontoRoteirizacao,
                Transportador = new { o.Transportador.Codigo, o.Transportador.Descricao }
            }).ToList();
        }

        private dynamic ObterGatilhoGeracaoAutomaticaPedidoOcorrenciaColetaEntrega(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, Repositorio.UnitOfWork unitOfWork, bool duplicar)
        {
            Repositorio.Embarcador.Pedidos.GatilhoGeracaoAutomaticaPedidoOcorrenciaColetaEntrega repositorioGatilhoGeracaoAutomaticaPedidoOcorrenciaColetaEntrega = new Repositorio.Embarcador.Pedidos.GatilhoGeracaoAutomaticaPedidoOcorrenciaColetaEntrega(unitOfWork);

            List<Dominio.Entidades.Embarcador.Pedidos.GatilhoGeracaoAutomaticaPedidoOcorrenciaColetaEntrega> gatilhosGeracaoAutomaticaPedidoOcorrenciaColetaEntrega = repositorioGatilhoGeracaoAutomaticaPedidoOcorrenciaColetaEntrega.BuscarPorTipoOperacao(tipoOperacao.Codigo);

            return gatilhosGeracaoAutomaticaPedidoOcorrenciaColetaEntrega.Select(o => new
            {
                Codigo = duplicar ? 0 : o.Codigo,
                o.Gatilho,
                o.Observacao,
                TipoOcorrencia = new
                {
                    o.TipoOcorrencia.Codigo,
                    o.TipoOcorrencia.Descricao
                }
            }).ToList();
        }

        private dynamic ObterConfiguracaoChamado(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, Repositorio.UnitOfWork unitOfWork)
        {
            return new
            {
                NaoPermitirGerarAtendimento = tipoOperacao.ConfiguracaoTipoOperacaoChamado?.NaoPermitirGerarAtendimento ?? false,
                PermitirSelecionarApenasAlgunsMotivosAtendimento = tipoOperacao.ConfiguracaoTipoOperacaoChamado?.PermitirSelecionarApenasAlgunsMotivosAtendimento ?? false,
                NaoValidarRetornoGeradoParaFinalizacaoAtendimento = tipoOperacao.ConfiguracaoTipoOperacaoChamado?.NaoValidarRetornoGeradoParaFinalizacaoAtendimento ?? false,
                FinalizarAutomaticamenteAtendimentoNfeEntregue = tipoOperacao.ConfiguracaoTipoOperacaoChamado?.FinalizarAutomaticamenteAtendimentoNfeEntregue ?? false,
            };
        }

        private dynamic ObterConfiguracaoChamadoMotivosChamados(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, Repositorio.UnitOfWork unitOfWork)
        {
            if (tipoOperacao.ConfiguracaoTipoOperacaoChamado?.MotivosChamados == null)
                return new List<dynamic>();

            return tipoOperacao.ConfiguracaoTipoOperacaoChamado.MotivosChamados.Select(obj => new
            {
                obj.Codigo,
                obj.Descricao,
            }).ToList();
        }

        private dynamic ObterConfiguracaoChamadoTransportador(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, Repositorio.UnitOfWork unitOfWork)
        {
            if (tipoOperacao.ConfiguracaoTipoOperacaoChamado?.Transportadores == null)
                return new List<dynamic>();

            return tipoOperacao.ConfiguracaoTipoOperacaoChamado.Transportadores.Select(obj => new
            {
                obj.Codigo,
                obj.Descricao,
            }).ToList();
        }


        private dynamic ObterConfiguracaoJanelaCarregamento(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao)
        {
            return new
            {
                PermitirRejeitarCargaJanelaCarregamentoTransportador = tipoOperacao.ConfiguracaoJanelaCarregamento?.PermitirRejeitarCargaJanelaCarregamentoTransportador ?? false,
                PermiteImprimirOrdemColetaNaGuarita = tipoOperacao?.PermiteImprimirOrdemColetaNaGuarita ?? false,
                PermitirLiberarCargaComTipoCondicaoPagamentoFOBParaTransportadores = tipoOperacao.ConfiguracaoJanelaCarregamento?.PermitirLiberarCargaComTipoCondicaoPagamentoFOBParaTransportadores ?? false,
                LayoutImpressaoOrdemColeta = tipoOperacao.ConfiguracaoJanelaCarregamento?.LayoutImpressaoOrdemColeta ?? LayoutImpressaoOrdemColeta.LayoutPadrao,
            };
        }

        private dynamic ObterConfiguracaomontagemCarga(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao)
        {
            if (tipoOperacao.ConfiguracaoMontagemCarga == null)
                tipoOperacao.ConfiguracaoMontagemCarga = new Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoMontagemCarga();

            if (tipoOperacao.ConfiguracaoMontagemCarga?.TransportadoresMontagemCarga == null)
                tipoOperacao.ConfiguracaoMontagemCarga.TransportadoresMontagemCarga = new List<Dominio.Entidades.Empresa>();

            return new
            {
                DisponibilizarPedidosMontagemAoFinalizarTransporte = tipoOperacao.ConfiguracaoMontagemCarga?.DisponibilizarPedidosMontagemAoFinalizarTransporte ?? false,
                ExibirPedidosMontagemIntegracao = tipoOperacao.ConfiguracaoMontagemCarga?.ExibirPedidosMontagemIntegracao ?? false,
                DisponibilizarPedidosMontagemDeterminadosTransportadores = tipoOperacao.ConfiguracaoMontagemCarga?.DisponibilizarPedidosMontagemDeterminadosTransportadores ?? false,
                OcultarTipoDeOperacaoNaMontagemDaCarga = tipoOperacao.ConfiguracaoMontagemCarga?.OcultarTipoDeOperacaoNaMontagemDaCarga ?? false,
                ControlarCapacidadePorUnidade = tipoOperacao.ConfiguracaoMontagemCarga?.ControlarCapacidadePorUnidade ?? false,
                ExigirInformarDataPrevisaoInicioViagem = tipoOperacao.ConfiguracaoMontagemCarga?.ExigirInformarDataPrevisaoInicioViagem ?? false,
                RoteirizarNovamenteAoConfirmarDocumentos = tipoOperacao.ConfiguracaoMontagemCarga?.RoteirizarNovamenteAoConfirmarDocumentos ?? false,
                MontagemComRecebedorNaoGerarCargaComoColeta = tipoOperacao.ConfiguracaoMontagemCarga?.MontagemComRecebedorNaoGerarCargaComoColeta ?? false,
                TransportadoresMontagem = (
                    from transp in tipoOperacao.ConfiguracaoMontagemCarga.TransportadoresMontagemCarga
                    select new
                    {
                        Codigo = transp.Codigo,
                        Descricao = transp.Descricao,
                    }
                ).ToList()
            };
        }

        private dynamic ObterConfiguracaoIntegracaoDieageo(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao)
        {
            return new
            {
                PossuiIntegracaoDiageo = tipoOperacao.ConfiguracaoTipoOperacaoIntegracaoDiageo?.PossuiIntegracaoDiageo ?? false,
            };
        }

        private dynamic ObterTiposComprovantes(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao)
        {
            if (tipoOperacao.TiposComprovante?.Count() <= 0)
                return new List<dynamic>();

            return tipoOperacao.TiposComprovante.Select(obj => new
            {
                Codigo = obj.Codigo,
                Descricao = obj.Descricao,
            }).ToList();

        }

        private dynamic ObterEventosSuperApp(List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoEventoSuperApp> listaTipoOperacaoEventos, bool duplicar)
        {
            List<dynamic> listaEventos = new List<dynamic>();
            foreach (Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoEventoSuperApp tipoOperacaoEventoSuperApp in listaTipoOperacaoEventos)
            {
                listaEventos.Add(new
                {
                    Codigo = duplicar ? 0 : tipoOperacaoEventoSuperApp.EventoSuperApp.Codigo,
                    Tipo = (int)tipoOperacaoEventoSuperApp.EventoSuperApp.Tipo,
                    TipoDescricao = tipoOperacaoEventoSuperApp.EventoSuperApp.Tipo.ObterDescricao(),
                    Titulo = tipoOperacaoEventoSuperApp.EventoSuperApp.Titulo,
                    Obrigatorio = tipoOperacaoEventoSuperApp.EventoSuperApp.Obrigatorio,
                    ObrigatorioDescricao = tipoOperacaoEventoSuperApp.EventoSuperApp.Obrigatorio ? "Sim" : "Não",
                    Ordem = tipoOperacaoEventoSuperApp.EventoSuperApp.Ordem,
                    TipoEventoCustomizado = (int)tipoOperacaoEventoSuperApp.EventoSuperApp.TipoEventoCustomizado,
                    TipoEventoCustomizadoDescricao = tipoOperacaoEventoSuperApp.EventoSuperApp.TipoEventoCustomizado.ObterDescricao(),
                    TipoParada = (int)tipoOperacaoEventoSuperApp.EventoSuperApp.TipoParada,
                    TipoParadaDescricao = tipoOperacaoEventoSuperApp.EventoSuperApp.TipoParada.ObterDescricao(),
                    ChecklistSuperApp = new
                    {
                        Codigo = tipoOperacaoEventoSuperApp.EventoSuperApp?.ChecklistSuperApp?.Codigo ?? 0D,
                        Descricao = tipoOperacaoEventoSuperApp.EventoSuperApp?.ChecklistSuperApp?.Descricao ?? string.Empty,
                    }
                });
            }
            return listaEventos;
        }

        private dynamic ObterNotificacoesAppTrizy(List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoNotificacaoApp> listaTipoOperacaoNotificacoes, bool duplicar)
        {
            List<dynamic> listaNotificacoes = new List<dynamic>();
            foreach (Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoNotificacaoApp tipoOperacaoNotificacaoApp in listaTipoOperacaoNotificacoes)
            {
                listaNotificacoes.Add(new
                {
                    Codigo = duplicar ? 0 : tipoOperacaoNotificacaoApp.NotificacaoApp.Codigo,
                    Tipo = (int)tipoOperacaoNotificacaoApp.NotificacaoApp.Tipo,
                    TipoDescricao = tipoOperacaoNotificacaoApp.NotificacaoApp.Tipo.ObterDescricao(),
                    Titulo = tipoOperacaoNotificacaoApp.NotificacaoApp.Titulo,
                    Mensagem = tipoOperacaoNotificacaoApp.NotificacaoApp.Mensagem
                });
            }

            return listaNotificacoes;
        }

        private dynamic ObterConfiguracaoIntegracaoTransSat(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao)
        {
            return new
            {
                PossuiIntegracaoTransSat = tipoOperacao.ConfiguracaoTipoOperacaoIntegracaoTransSat?.PossuiIntegracaoTransSat ?? false,
            };
        }

        private dynamic ObterConfiguracaoGestaoDevolucao(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao)
        {
            if (tipoOperacao.ConfiguracaoGestaoDevolucao == null)
                tipoOperacao.ConfiguracaoGestaoDevolucao = new Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoGestaoDevolucao();

            return new
            {
                UsarComoPadraoQuandoCargaForDevolucaoDoTipoColeta = tipoOperacao.ConfiguracaoGestaoDevolucao.UsarComoPadraoQuandoCargaForDevolucaoDoTipoColeta
            };
        }

        private dynamic ObterConfiguracaoTipoPropriedadeVeiculo(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao)
        {
            if (tipoOperacao.ConfiguracaoTipoPropriedadeVeiculo == null)
                tipoOperacao.ConfiguracaoTipoPropriedadeVeiculo = new Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoTipoPropriedadeVeiculo();

            return new
            {
                TipoPropriedadeVeiculo = tipoOperacao.ConfiguracaoTipoPropriedadeVeiculo?.TipoPropriedadeVeiculo ?? TipoPropriedadeVeiculo.Ambos,
                TipoProprietarioVeiculo = tipoOperacao.ConfiguracaoTipoPropriedadeVeiculo?.TipoProprietarioVeiculo ?? Dominio.Enumeradores.TipoProprietarioVeiculo.Todos,
                TiposTerceirosPropriedadeVeiculo = tipoOperacao.ConfiguracaoTipoPropriedadeVeiculo.TiposTerceiros?.Select(t => new
                {
                    t.Codigo,
                    t.Descricao,
                    t.DescricaoSituacao
                }).ToList() ?? null,
            };
        }

        private dynamic ObterConfiguracaoCotacaoPedido(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao)
        {
            if (tipoOperacao.ConfiguracaoCotacaoPedido == null)
                tipoOperacao.ConfiguracaoCotacaoPedido = new Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoCotacaoPedido();

            return new
            {
                HabilitaInformarDadosDosPedidosNaCotacao = tipoOperacao.ConfiguracaoCotacaoPedido.HabilitaInformarDadosDosPedidosNaCotacao
            };
        }
        private dynamic ObterConfiguracaoContainer(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao)
        {
            if (tipoOperacao.ConfiguracaoContainer == null)
                tipoOperacao.ConfiguracaoContainer = new Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoContainer();

            return new
            {
                GestaoViagemContainerFluxoUnico = tipoOperacao.ConfiguracaoContainer.GestaoViagemContainerFluxoUnico,
                NaoPermitirAlterarMotoristaAposAverbacaoContainer = tipoOperacao.ConfiguracaoContainer.NaoPermitirAlterarMotoristaAposAverbacaoContainer,
                ExigirComprovanteColetaContainerParaSeguir = tipoOperacao.ConfiguracaoContainer.ExigirComprovanteColetaContainerParaSeguir,
                GerarPagamentoMotoristaAutomaticamenteComValorAdiantamentoContainer = tipoOperacao.ConfiguracaoContainer.GerarPagamentoMotoristaAutomaticamenteComValorAdiantamentoContainer,
                ComprarValePedagioEtapaContainer = tipoOperacao.ConfiguracaoContainer.ComprarValePedagioEtapaContainer,
                TipoPagamentoAdiantamentoContainer = new { Codigo = tipoOperacao.ConfiguracaoContainer?.PagamentoMotoristaTipo?.Codigo ?? 0, Descricao = tipoOperacao.ConfiguracaoContainer?.PagamentoMotoristaTipo?.Descricao ?? "" },
                ModeloDocumentoDocumentoContainer = new { Codigo = tipoOperacao.ConfiguracaoContainer?.ModeloDocumentoContainer?.Codigo ?? 0, Descricao = tipoOperacao.ConfiguracaoContainer?.ModeloDocumentoContainer?.Descricao ?? "" },
                TipoComprovanteColetaContainer = new { Codigo = tipoOperacao.ConfiguracaoContainer?.TipoComprovanteColetaContainer?.Codigo ?? 0, Descricao = tipoOperacao.ConfiguracaoContainer?.TipoComprovanteColetaContainer?.Descricao ?? "" }

            };
        }
        private dynamic ObterConfiguracaoPesoConsideradoCarga(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoPesoConsideradoCarga repositorioConfiguracaoTipoOperacaoPesoConsideradoCarga = new Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoPesoConsideradoCarga(unitOfWork);
            Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoPesoConsideradoCarga configuracaoTipoOperacaoPesoConsideradoCarga = repositorioConfiguracaoTipoOperacaoPesoConsideradoCarga.BuscarPorTipoOperacao(tipoOperacao);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarPrimeiroRegistro();

            return new
            {
                PesoConsideradoNaCarga = configuracaoTipoOperacaoPesoConsideradoCarga?.PesoConsideradoNaCarga ?? (configuracaoTMS.UtilizarPesoLiquidoNFeParaCTeMDFe ? EnumPesoConsideradoCarga.PesoLiquido : EnumPesoConsideradoCarga.PesoBruto)
            };
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaTipoOperacao filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Descricao, "Descricao", 55, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.CodigoIntegracao, "CodigoIntegracao", 20, Models.Grid.Align.left, true);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    grid.AdicionarCabecalho(Localization.Resources.Consultas.TipoOperacao.Embarcador, "Embarcador", 35, Models.Grid.Align.left, false);

                if (filtrosPesquisa.Ativo == SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Situacao, "DescricaoAtivo", 10, Models.Grid.Align.center, false);

                grid.AdicionarCabecalho("NaoExigeVeiculoParaEmissao", false);
                grid.AdicionarCabecalho("ColetaEmProdutorRural", false);
                grid.AdicionarCabecalho("OperacaoDeImportacaoExportacao", false);
                grid.AdicionarCabecalho("PermiteUtilizarEmContratoFrete", false);
                grid.AdicionarCabecalho("PermitirMultiplosDestinatariosPedido", false);
                grid.AdicionarCabecalho("PermitirMultiplosRemetentesPedido", false);
                grid.AdicionarCabecalho("RecriarControleDeEntregasAoConfirmarEnvioDocumentos", false);
                grid.AdicionarCabecalho("ConfirmarColetasQuandoTodasAsEntregasDaCargaForemConcluidas", false);
                grid.AdicionarCabecalho("DisponibilizarFotoDaColetaNaTelaDeAprovacaoDeNotaFiscal", false);
                grid.AdicionarCabecalho("UtilizarDeslocamentoPedido", false);
                grid.AdicionarCabecalho("TipoUltimoPontoRoteirizacao", false);
                grid.AdicionarCabecalho("PermiteGerarPedidoSemDestinatario", false);
                grid.AdicionarCabecalho("CamposSecundariosObrigatoriosPedido", false);
                grid.AdicionarCabecalho("TipoServicoMultimodal", false);
                grid.AdicionarCabecalho("ObrigarRotaNaMontagemDeCarga", false);
                grid.AdicionarCabecalho("ExigePlacaTracao", false);
                grid.AdicionarCabecalho("PermitirVeiculoDiferenteMontagemCarga", false);
                grid.AdicionarCabecalho("TipoObrigacaoUsoTerminal", false);
                grid.AdicionarCabecalho("TransbordoRodoviario", false);
                grid.AdicionarCabecalho("PermitirEnviarImagemParaMultiplosCanhotos", false);
                grid.AdicionarCabecalho("PermitirInformarDataEntregaParaMultiplosCanhotos", false);
                grid.AdicionarCabecalho("Reentrega", false);
                grid.AdicionarCabecalho("TempoCarregamento", false);
                grid.AdicionarCabecalho("TempoDescarregamento", false);
                grid.AdicionarCabecalho("ApresentarSaldoProduto", false);
                grid.AdicionarCabecalho("NecessarioConfirmacaoMotorista", false);
                grid.AdicionarCabecalho("TempoLimiteConfirmacaoMotorista", false);
                grid.AdicionarCabecalho("TipoDeCargaPadraoOperacao", false);
                grid.AdicionarCabecalho("Expedidor", false);
                grid.AdicionarCabecalho("CodigoExpedidor", false);
                grid.AdicionarCabecalho("PermitirInformarRecebedorMontagemCarga", false);
                grid.AdicionarCabecalho("InformarDadosNotaCte", false);
                grid.AdicionarCabecalho("ControlarCapacidadePorUnidade", false);
                grid.AdicionarCabecalho("PermitirInformarAjudantesNaCarga", false);
                grid.AdicionarCabecalho("ExigirInformarDataPrevisaoInicioViagem", false);
                grid.AdicionarCabecalho("ObrigatorioJustificarCustoExtra", false);
                grid.AdicionarCabecalho("LiberarCargaSemPlanejamento", false);
                grid.AdicionarCabecalho("NecessitaInformarPlacaCarregamento", false);
                grid.AdicionarCabecalho("NaoExigeRoteirizacaoMontagemCarga", false);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                Repositorio.Embarcador.Pedidos.TipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
                int totalRegistros = repositorioTipoOperacao.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> listaTipoOperacao = totalRegistros > 0 ? repositorioTipoOperacao.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>();

                var retorno = (
                    from obj in listaTipoOperacao
                    select new
                    {
                        obj.Codigo,
                        obj.NaoExigeVeiculoParaEmissao,
                        obj.CamposSecundariosObrigatoriosPedido,
                        obj.ColetaEmProdutorRural,
                        obj.OperacaoDeImportacaoExportacao,
                        obj.PermiteUtilizarEmContratoFrete,
                        obj.PermitirMultiplosDestinatariosPedido,
                        obj.PermitirMultiplosRemetentesPedido,
                        obj.UtilizarDeslocamentoPedido,
                        obj.TipoServicoMultimodal,
                        obj.Descricao,
                        obj.DescricaoAtivo,
                        obj.TipoUltimoPontoRoteirizacao,
                        obj.PermiteGerarPedidoSemDestinatario,
                        Embarcador = obj.Pessoa != null ? obj.Pessoa.Nome + "(" + obj.Pessoa.CPF_CNPJ_Formatado + ")" : obj.GrupoPessoas != null ? obj.GrupoPessoas.Descricao : "",
                        obj.CodigoIntegracao,
                        obj.TipoObrigacaoUsoTerminal,
                        obj.ObrigarRotaNaMontagemDeCarga,
                        obj.ExigePlacaTracao,
                        obj.TransbordoRodoviario,
                        obj.PermitirVeiculoDiferenteMontagemCarga,
                        PermitirEnviarImagemParaMultiplosCanhotos = obj.PermitirEnviarImagemParaMultiplosCanhotos || (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe && (obj.ConfiguracaoTransportador?.PermitirEnvioImagemMultiplosCanhotos ?? false)),
                        obj.PermitirInformarDataEntregaParaMultiplosCanhotos,
                        obj.Reentrega,
                        Expedidor = obj.Expedidor?.Nome ?? "",
                        CodigoExpedidor = obj.Expedidor?.Codigo ?? 0,
                        TempoCarregamento = obj.TempoCarregamentoTicks > 0 ? $"{(int)obj.TempoCarregamento.TotalHours:d3}:{obj.TempoCarregamento:mm}" : "",
                        TempoDescarregamento = obj.TempoDescarregamentoTicks > 0 ? $"{(int)obj.TempoDescarregamento.TotalHours:d3}:{obj.TempoDescarregamento:mm}" : "",
                        ApresentarSaldoProduto = Servicos.Embarcador.Carga.MontagemCarga.SaldoPedidoProduto.ApresentarSaldoProdutoGridPedido(null, obj),
                        NecessarioConfirmacaoMotorista = obj.ConfiguracaoMobile?.NecessarioConfirmacaoMotorista ?? false,
                        TempoLimiteConfirmacaoMotorista = obj.ConfiguracaoMobile?.TempoLimiteConfirmacaoMotorista,
                        TipoDeCargaPadraoOperacao = obj.TipoDeCargaPadraoOperacao != null ? obj.TipoDeCargaPadraoOperacao.Codigo : 0,
                        PermitirInformarRecebedorMontagemCarga = obj.PermitirInformarRecebedorMontagemCarga,
                        obj.InformarDadosNotaCte,
                        CodigoTipoCarga = obj.TipoDeCargaPadraoOperacao?.Codigo ?? 0,
                        ControlarCapacidadePorUnidade = obj.ConfiguracaoMontagemCarga?.ControlarCapacidadePorUnidade ?? false,
                        PermitirInformarAjudantesNaCarga = obj.ConfiguracaoCarga?.PermitirInformarAjudantesNaCarga ?? false,
                        ObrigatorioJustificarCustoExtra = obj.ConfiguracaoCarga?.ObrigatorioJustificarCustoExtra ?? false,
                        LiberarCargaSemPlanejamento = obj.ConfiguracaoCarga?.LiberarCargaSemPlanejamento ?? false,
                        ExigirInformarDataPrevisaoInicioViagem = obj.ConfiguracaoMontagemCarga?.ExigirInformarDataPrevisaoInicioViagem ?? false,
                        RecriarControleDeEntregasAoConfirmarEnvioDocumentos = obj.ConfiguracaoControleEntrega?.RecriarControleDeEntregasAoConfirmarEnvioDocumentos ?? false,
                        ConfirmarColetasQuandoTodasAsEntregasDaCargaForemConcluidas = obj.ConfiguracaoControleEntrega?.ConfirmarColetasQuandoTodasAsEntregasDaCargaForemConcluidas ?? false,
                        DisponibilizarFotoDaColetaNaTelaDeAprovacaoDeNotaFiscal = obj.ConfiguracaoControleEntrega?.DisponibilizarFotoDaColetaNaTelaDeAprovacaoDeNotaFiscal ?? false,
                        NecessitaInformarPlacaCarregamento = obj.ConfiguracaoCarga?.NecessitaInformarPlacaCarregamento ?? false,
                        obj.NaoExigeRoteirizacaoMontagemCarga
                    }
                ).ToList();

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

        private void SalvarProdutosPadroes(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.TipoDeOcorrenciaDeCTe repTipoDeOcorrencia = new Repositorio.TipoDeOcorrenciaDeCTe(unitOfWork);

            if (string.IsNullOrWhiteSpace(Request.Params("TiposOcorrencia")))
                return;

            tipoOperacao.TiposOcorrencia = new List<Dominio.Entidades.TipoDeOcorrenciaDeCTe>();

            dynamic dynTiposOcorrencia = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("TiposOcorrencia"));
            foreach (dynamic dynTipoOcorrencia in dynTiposOcorrencia)
            {
                Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoOcorrencia = repTipoDeOcorrencia.BuscarPorCodigo(((string)dynTipoOcorrencia.Codigo).ToInt());
                if (tipoOcorrencia == null)
                    continue;

                tipoOperacao.TiposOcorrencia.Add(tipoOcorrencia);
            }
        }

        private void SalvarGrupoTomadoresBloqueados(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, Repositorio.UnitOfWork unitOfWork)
        {
            if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                return;

            Repositorio.Embarcador.Pedidos.TipoOperacaoGrupoTomadorBloqueado repTipoOperacaoGrupoTomadorBloqueado = new Repositorio.Embarcador.Pedidos.TipoOperacaoGrupoTomadorBloqueado(unitOfWork);
            Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);

            dynamic dynGrupoTomadoresBloqueados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("GrupoTomadoresBloqueados"));

            List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoGrupoTomadorBloqueado> grupoTomadoresBloqueados = repTipoOperacaoGrupoTomadorBloqueado.BuscarPorTipoOperacao(tipoOperacao.Codigo);

            if (grupoTomadoresBloqueados.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (dynamic grupoTomador in dynGrupoTomadoresBloqueados)
                {
                    int codigoGrupoTomador = ((string)grupoTomador.Codigo).ToInt();
                    if (codigoGrupoTomador > 0)
                        codigos.Add(codigoGrupoTomador);
                }

                List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoGrupoTomadorBloqueado> deletar = (from obj in grupoTomadoresBloqueados where !codigos.Contains(obj.GrupoPessoas.Codigo) select obj).ToList();

                for (int i = 0; i < deletar.Count; i++)
                    repTipoOperacaoGrupoTomadorBloqueado.Deletar(deletar[i]);
            }

            foreach (dynamic grupoTomador in dynGrupoTomadoresBloqueados)
            {
                int codigoGrupoTomador = ((string)grupoTomador.Codigo).ToInt();

                Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoGrupoTomadorBloqueado tipoOperacaoGrupoTomadorBloqueado = repTipoOperacaoGrupoTomadorBloqueado.BuscarPorGrupoETipoOperacao(tipoOperacao.Codigo, codigoGrupoTomador);

                if (tipoOperacaoGrupoTomadorBloqueado == null)
                {
                    tipoOperacaoGrupoTomadorBloqueado = new Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoGrupoTomadorBloqueado();

                    tipoOperacaoGrupoTomadorBloqueado.TipoOperacao = tipoOperacao;
                    tipoOperacaoGrupoTomadorBloqueado.GrupoPessoas = repGrupoPessoas.BuscarPorCodigo(codigoGrupoTomador);

                    repTipoOperacaoGrupoTomadorBloqueado.Inserir(tipoOperacaoGrupoTomadorBloqueado);
                }
            }
        }

        private void SalvarTiposOcorrencia(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.TipoOperacaoProdutosPadrao repTipoOperacaoProdutosPadrao = new Repositorio.Embarcador.Pedidos.TipoOperacaoProdutosPadrao(unitOfWork);
            Repositorio.Embarcador.Produtos.ProdutoEmbarcador repProduto = new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(unitOfWork);
            if (!string.IsNullOrWhiteSpace(Request.Params("Produtos")))
            {
                dynamic dynProdutos = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Produtos"));

                repTipoOperacaoProdutosPadrao.DeletarPorTipoOperacao(tipoOperacao.Codigo);

                foreach (dynamic dynProduto in dynProdutos)
                {
                    Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador produtoAdd = repProduto.BuscarPorCodigo(((string)dynProduto.Codigo).ToInt());

                    Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoProdutosPadrao tipoOperacaoProduto = new Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoProdutosPadrao();
                    tipoOperacaoProduto.Produto = produtoAdd;
                    tipoOperacaoProduto.TipoOperacao = tipoOperacao;
                    repTipoOperacaoProdutosPadrao.Inserir(tipoOperacaoProduto);

                }
            }
        }

        private void SalvarConfiguracaoCIOTPamcard(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.TipoOperacaoConfiguracaoCIOTPamcard repTipoOperacaoConfiguracaoCIOTPamcard = new Repositorio.Embarcador.Pedidos.TipoOperacaoConfiguracaoCIOTPamcard(unitOfWork);
            Repositorio.Auditoria.HistoricoObjeto repHistoricoObjeto = new Repositorio.Auditoria.HistoricoObjeto(unitOfWork);

            Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoConfiguracaoCIOTPamcard tipoOperacaoConfiguracaoCIOTPamcard = repTipoOperacaoConfiguracaoCIOTPamcard.BuscarPorTipoOperacao(tipoOperacao);

            if (tipoOperacao.ConfiguracaoCIOT == null || tipoOperacao.ConfiguracaoCIOT.OperadoraCIOT != OperadoraCIOT.Pamcard)
            {
                if (tipoOperacaoConfiguracaoCIOTPamcard != null)
                    repTipoOperacaoConfiguracaoCIOTPamcard.Deletar(tipoOperacaoConfiguracaoCIOTPamcard);

                return;
            }

            if (tipoOperacaoConfiguracaoCIOTPamcard == null)
            {
                tipoOperacaoConfiguracaoCIOTPamcard = new Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoConfiguracaoCIOTPamcard();
                tipoOperacaoConfiguracaoCIOTPamcard.TipoOperacao = tipoOperacao;
            }
            else
                tipoOperacaoConfiguracaoCIOTPamcard.Initialize();


            tipoOperacaoConfiguracaoCIOTPamcard.UtilizarConfiguracaoPersonalizadaParcelas = Request.GetBoolParam("UtilizarConfiguracaoPersonalizadaParcelasPamcard");

            tipoOperacaoConfiguracaoCIOTPamcard.EfetivacaoAbastecimento = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.PamcardParcelaTipoEfetivacao>("EfetivacaoAbastecimentoPamcard");
            tipoOperacaoConfiguracaoCIOTPamcard.StatusAbastecimento = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.PamcardParcelaStatus>("StatusAbastecimentoPamcard");

            tipoOperacaoConfiguracaoCIOTPamcard.EfetivacaoAdiantamento = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.PamcardParcelaTipoEfetivacao>("EfetivacaoAdiantamentoPamcard");
            tipoOperacaoConfiguracaoCIOTPamcard.StatusAdiantamento = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.PamcardParcelaStatus>("StatusAdiantamentoPamcard");

            tipoOperacaoConfiguracaoCIOTPamcard.EfetivacaoSaldo = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.PamcardParcelaTipoEfetivacao>("EfetivacaoSaldoPamcard");
            tipoOperacaoConfiguracaoCIOTPamcard.StatusSaldo = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.PamcardParcelaStatus>("StatusSaldoPamcard");

            if (tipoOperacaoConfiguracaoCIOTPamcard.Codigo > 0)
            {
                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = tipoOperacaoConfiguracaoCIOTPamcard.GetChanges();

                repTipoOperacaoConfiguracaoCIOTPamcard.Atualizar(tipoOperacaoConfiguracaoCIOTPamcard);

                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, tipoOperacao, alteracoes, Localization.Resources.Pedidos.TipoOperacao.AlterouConfiguracaoDoCIOTPamcard, unitOfWork);
            }
            else
            {
                repTipoOperacaoConfiguracaoCIOTPamcard.Inserir(tipoOperacaoConfiguracaoCIOTPamcard);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, tipoOperacao, Localization.Resources.Pedidos.TipoOperacao.AdicionouConfiguracaoDoCIOTPamcard, unitOfWork);
            }
        }

        private void SalvarIntegracaoMultiEmbarcador(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao)
        {
            bool? habilitarIntegracaoMultiEmbarcador = Request.GetNullableBoolParam("HabilitarIntegracaoMultiEmbarcador");

            if (habilitarIntegracaoMultiEmbarcador == true)
            {
                tipoOperacao.HabilitarIntegracaoMultiEmbarcador = habilitarIntegracaoMultiEmbarcador;
                tipoOperacao.IntegrarCIOTMultiEmbarcador = Request.GetNullableBoolParam("IntegrarCIOTMultiEmbarcador");
                tipoOperacao.TokenIntegracaoMultiEmbarcador = Request.GetNullableStringParam("TokenIntegracaoMultiEmbarcador");
                tipoOperacao.URLIntegracaoMultiEmbarcador = Request.GetNullableStringParam("URLIntegracaoMultiEmbarcador");
                tipoOperacao.PermiteUtilizarEmContratoFrete = Request.GetNullableBoolParam("PermiteUtilizarEmContratoFrete");
                tipoOperacao.IntegrarCargasMultiEmbarcador = Request.GetNullableBoolParam("IntegrarCargasMultiEmbarcador");
                tipoOperacao.DataInicialCargasMultiEmbarcador = Request.GetNullableDateTimeParam("DataInicialCargasMultiEmbarcador");
                tipoOperacao.NaoImportarCargasComplementaresMultiEmbarcador = Request.GetNullableBoolParam("NaoImportarCargasComplementaresMultiEmbarcador");
                tipoOperacao.NaoGerarCargaMultiEmbarcador = Request.GetNullableBoolParam("NaoGerarCargaMultiEmbarcador");
                tipoOperacao.NaoIntegrarCancelamentoMultiEmbarcadorComDadosInvalidos = Request.GetNullableBoolParam("NaoIntegrarCancelamentoMultiEmbarcadorComDadosInvalidos");
                tipoOperacao.ExpressaoRegularNumeroPedidoObservacaoCTeMultiEmbarcador = Request.GetStringParam("ExpressaoRegularNumeroPedidoObservacaoCTeMultiEmbarcador");
                tipoOperacao.VincularDocumentosAutomaticamenteEmCargaExistenteMultiEmbarcador = Request.GetBoolParam("VincularDocumentosAutomaticamenteEmCargaExistenteMultiEmbarcador");
                tipoOperacao.UtilizarGeracaoDeNFSeAvancada = Request.GetBoolParam("UtilizarGeracaoDeNFSeAvancada");
            }
            else
            {
                tipoOperacao.HabilitarIntegracaoMultiEmbarcador = null;
                tipoOperacao.IntegrarCIOTMultiEmbarcador = null;
                tipoOperacao.TokenIntegracaoMultiEmbarcador = null;
                tipoOperacao.URLIntegracaoMultiEmbarcador = null;
                tipoOperacao.IntegrarCargasMultiEmbarcador = null;
                tipoOperacao.PermiteUtilizarEmContratoFrete = null;
                tipoOperacao.DataInicialCargasMultiEmbarcador = null;
                tipoOperacao.NaoImportarCargasComplementaresMultiEmbarcador = null;
                tipoOperacao.NaoGerarCargaMultiEmbarcador = null;
                tipoOperacao.NaoIntegrarCancelamentoMultiEmbarcadorComDadosInvalidos = null;
                tipoOperacao.ExpressaoRegularNumeroPedidoObservacaoCTeMultiEmbarcador = null;
                tipoOperacao.VincularDocumentosAutomaticamenteEmCargaExistenteMultiEmbarcador = null;
                tipoOperacao.UtilizarGeracaoDeNFSeAvancada = null;
            }
        }

        private void SalvarTipoOperacaoFilialMotoristaGenerico(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, Repositorio.UnitOfWork unitOfWork)
        {
            if (!tipoOperacao.ConfiguracaoCarga?.HabilitarVinculoMotoristaGenericoCarga ?? false)
                return;

            Repositorio.Embarcador.Pedidos.TipoOperacaoFilialMotoristaGenerico repositorioTipoOperacaoFilialMotoristaGenerico = new Repositorio.Embarcador.Pedidos.TipoOperacaoFilialMotoristaGenerico(unitOfWork);
            Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
            Repositorio.Usuario repositorioMotorista = new Repositorio.Usuario(unitOfWork);

            dynamic dynTiposOperacaoFilialMotoristaGenerico = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("FiliaisMotoristasGenericos"));

            var tiposOperacaoFilialMotoristaGenericosExistentes = repositorioTipoOperacaoFilialMotoristaGenerico.BuscarFiliaisMotoristasPorTipoOperacao(tipoOperacao.Codigo) ?? new List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoFilialMotoristaGenerico>();

            List<int> novosCodigos = new List<int>();
            List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoFilialMotoristaGenerico> novosRegistros = new List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoFilialMotoristaGenerico>();

            foreach (dynamic dynItem in dynTiposOperacaoFilialMotoristaGenerico)
            {
                int codigoFilial = dynItem.Codigo;
                int codigoMotorista = dynItem.MotoristaCodigo;

                novosCodigos.Add(codigoFilial);

                // Verifica se já existe
                Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoFilialMotoristaGenerico registroExistente = tiposOperacaoFilialMotoristaGenericosExistentes
                    .FirstOrDefault(x => x.Filial.Codigo == codigoFilial && x.Motorista.Codigo == codigoMotorista);

                if (registroExistente == null)
                {
                    Dominio.Entidades.Embarcador.Filiais.Filial filial = repositorioFilial.BuscarPorCodigo(codigoFilial);
                    Dominio.Entidades.Usuario motorista = repositorioMotorista.BuscarPorCodigo(codigoMotorista);

                    if (tiposOperacaoFilialMotoristaGenericosExistentes.Any(x => x.Filial.Codigo == codigoFilial))
                    {
                        throw new ControllerException("Já existe um motorista cadastrado para essa filial");
                    }

                    novosRegistros.Add(new Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoFilialMotoristaGenerico
                    {
                        TipoOperacao = tipoOperacao,
                        Motorista = motorista,
                        Filial = filial
                    });
                }
            }

            // Identifica registros que precisam ser removidos (não estão na nova lista)
            var registrosParaRemover = tiposOperacaoFilialMotoristaGenericosExistentes
                .Where(x => !novosCodigos.Contains(x.Filial.Codigo))
                .ToList();

            foreach (var registro in registrosParaRemover)
            {
                repositorioTipoOperacaoFilialMotoristaGenerico.Deletar(registro);
            }

            // Adiciona apenas os novos registros
            foreach (var novoRegistro in novosRegistros)
            {
                repositorioTipoOperacaoFilialMotoristaGenerico.Inserir(novoRegistro);
            }
        }


        private void AtualizarConfiguracoesMobile(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, Dominio.Entidades.Auditoria.HistoricoObjeto historico, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unidadeDeTrabalho);
            Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoMobile repConfiguracaoTipoOperacaoMobile = new Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoMobile(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoMobile configuracao = tipoOperacao.ConfiguracaoMobile ?? new Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoMobile();

            if (configuracao.Codigo > 0)
                configuracao.Initialize();

            configuracao.TipoOperacao = tipoOperacao;
            configuracao.PermiteFotosEntrega = Request.GetBoolParam("MobilePermiteFotosEntrega");
            configuracao.QuantidadeMinimasFotosEntrega = Request.GetIntParam("MobileQuantidadeMinimasFotosEntrega");
            configuracao.PermiteConfirmarChegadaEntrega = Request.GetBoolParam("MobilePermiteConfirmarChegadaEntrega");
            configuracao.PermiteConfirmarChegadaColeta = Request.GetBoolParam("MobilePermiteConfirmarChegadaColeta");
            configuracao.ControlarTempoColeta = Request.GetBoolParam("MobileControlarTempoColeta");
            configuracao.NaoUtilizarProdutosNaColeta = Request.GetBoolParam("MobileNaoUtilizarProdutosNaColeta");
            configuracao.PermitirEscanearChavesNfe = Request.GetBoolParam("MobilePermitirEscanearChavesNfe");
            configuracao.ObrigarEscanearChavesNfe = Request.GetBoolParam("MobileObrigarEscanearChavesNfe");
            configuracao.PermitirVisualizarProgramacaoAntesViagem = Request.GetBoolParam("MobilePermitirVisualizarProgramacaoAntesViagem");
            configuracao.ExibirEntregaAntesEtapaTransporte = Request.GetBoolParam("MobileExibirEntregaAntesEtapaTransporte");
            configuracao.ExibirEntregaEtapaEmissaoDocumentos = Request.GetBoolParam("ExibirEntregaEtapaEmissaoDocumentos");
            configuracao.SolicitarJustificativaRegistroForaRaio = Request.GetBoolParam("MobileSolicitarJustificativaRegistroForaRaio");
            configuracao.PermiteEventos = Request.GetBoolParam("MobilePermiteEventos");
            configuracao.PermiteChat = Request.GetBoolParam("MobilePermiteChat");
            configuracao.PermiteSAC = Request.GetBoolParam("MobilePermiteSAC");
            configuracao.PermiteCanhotoModoManual = Request.GetBoolParam("MobilePermiteCanhotoModoManual");
            configuracao.PermiteConfirmarEntrega = Request.GetBoolParam("MobilePermiteConfirmarEntrega");
            configuracao.BloquearRastreamento = Request.GetBoolParam("MobileBloquearRastreamento");
            configuracao.PermiteEntregaParcial = Request.GetBoolParam("MobilePermiteEntregaParcial");
            configuracao.ControlarTempoEntrega = Request.GetBoolParam("MobileControlarTempoEntrega");
            configuracao.ExibirRelatorio = Request.GetBoolParam("MobileExibirRelatorio");
            configuracao.NaoRetornarColetas = Request.GetBoolParam("MobileNaoRetornarColetas");
            configuracao.ObrigarAssinaturaProdutor = Request.GetBoolParam("MobileObrigarAssinaturaProdutor");
            configuracao.NecessarioConfirmacaoMotorista = Request.GetBoolParam("NecessarioConfirmacaoMotorista");
            configuracao.IniciarViagemNoControleDePatioAoIniciarViagemNoApp = Request.GetBoolParam("IniciarViagemNoControleDePatioAoIniciarViagemNoApp");
            configuracao.TempoLimiteConfirmacaoMotorista = Request.GetTimeParam("TempoLimiteConfirmacaoMotorista");
            configuracao.ExibirAvaliacaoNaAssintura = Request.GetBoolParam("ExibirAvaliacaoNaAssintura");
            configuracao.PermiteBaixarOsDocumentosDeTransporte = Request.GetBoolParam("PermiteBaixarOsDocumentosDeTransporte");
            configuracao.ForcarPreenchimentoSequencial = Request.GetBoolParam("MobileForcarPreenchimentoSequencial");
            configuracao.ObrigarFotoCanhoto = Request.GetBoolParam("MobileObrigarFotoCanhoto");
            configuracao.ObrigarAssinaturaEntrega = Request.GetBoolParam("MobileObrigarAssinatura");
            configuracao.ObrigarDadosRecebedor = Request.GetBoolParam("MobileObrigarDadosRecebedor");
            configuracao.SolicitarReconhecimentoFacialDoRecebedor = Request.GetBoolParam("SolicitarReconhecimentoFacialDoRecebedor");
            configuracao.PermiteFotosColeta = Request.GetBoolParam("MobilePermiteFotosColeta");
            configuracao.QuantidadeMinimasFotosColeta = Request.GetIntParam("MobileQuantidadeMinimasFotosColeta");
            configuracao.NaoListarProdutosColetaEntrega = Request.GetBoolParam("NaoListarProdutosColetaEntrega");
            configuracao.NaoApresentarDataInicioViagem = Request.GetBoolParam("NaoApresentarDataInicioViagem");
            configuracao.ReplicarDataDigitalizacaoCanhotoDataEntregaCliente = Request.GetBoolParam("ReplicarDataDigitalizacaoCanhotoDataEntregaCliente");

            if (configuracao.Codigo == 0)
                repConfiguracaoTipoOperacaoMobile.Inserir(configuracao);
            else
                repConfiguracaoTipoOperacaoMobile.Atualizar(configuracao, Auditado, historico);

            tipoOperacao.ConfiguracaoMobile = configuracao;
            repTipoOperacao.Atualizar(tipoOperacao);
        }

        private void AtualizarConfiguracoesCalculoFrete(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, Dominio.Entidades.Auditoria.HistoricoObjeto historico, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unidadeDeTrabalho);
            Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoCalculoFrete repConfiguracaoTipoOperacaoCalculoFrete = new Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoCalculoFrete(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoCalculoFrete configuracaoCalculoFrete = tipoOperacao.ConfiguracaoCalculoFrete ?? new Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoCalculoFrete();

            if (configuracaoCalculoFrete.Codigo > 0)
                configuracaoCalculoFrete.Initialize();

            configuracaoCalculoFrete.MesclarValorEmbarcadorComTabelaFrete = Request.GetBoolParam("CalculoFreteMesclarValorEmbarcadorComTabelaFrete");
            configuracaoCalculoFrete.BloquearAjusteConfiguracoesFreteCarga = Request.GetBoolParam("CalculoFreteBloquearAjusteConfiguracoesFreteCarga");
            configuracaoCalculoFrete.TipoCotacao = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCotacaoFreteInternacional>("CalculoFreteTipoCotacao");
            configuracaoCalculoFrete.ValorMoedaCotacao = Request.GetNullableDecimalParam("CalculoFreteValorMoedaCotacao");
            configuracaoCalculoFrete.PermiteInformarQuantidadePaletes = Request.GetBoolParam("CalculoFretePermiteInformarQuantidadePaletes");
            configuracaoCalculoFrete.NaoAlterarTipoPagamentoTomadorValoresInformadosManualmente = Request.GetBoolParam("NaoAlterarTipoPagamentoTomadorValoresInformadosManualmente");
            configuracaoCalculoFrete.ExecutarPreCalculoFrete = Request.GetBoolParam("ExecutarPreCalculoFrete");
            configuracaoCalculoFrete.RatearValorFreteEntrePedidosAposReceberDocumentos = Request.GetBoolParam("RatearValorFreteEntrePedidosAposReceberDocumentos");
            configuracaoCalculoFrete.CalcularFretePeloBIDPedidoOrigem = Request.GetBoolParam("CalcularFretePeloBIDPedidoOrigem");
            configuracaoCalculoFrete.NaoIncluirValorICMSBasecalculoQuandoValorFreteInformadoOperador = Request.GetBoolParam("NaoIncluirValorICMSBasecalculoQuandoValorFreteInformadoOperador");
            configuracaoCalculoFrete.InformarValorFreteTerceiroManualmente = Request.GetBoolParam("InformarValorFreteTerceiroManualmente");
            configuracaoCalculoFrete.ConsiderarInclusaoIcmsCargaPedidoNaEmissaoCteComplementar = Request.GetBoolParam("ConsiderarInclusaoIcmsCargaPedidoNaEmissaoCteComplementar");
            configuracaoCalculoFrete.PermiteEscolherDestinacaoDoComplementoDeFrete = Request.GetBoolParam("PermiteEscolherDestinacaoDoComplementoDeFrete");
            configuracaoCalculoFrete.MesclarComponentesManuaisPedidoComTabelaFrete = Request.GetBoolParam("CalculoFreteMesclarComponentesManuaisPedidoComTabelaFrete");
            configuracaoCalculoFrete.ValorMaximoCalculoFrete = Request.GetDecimalParam("ValorMaximoCalculoFrete");
            configuracaoCalculoFrete.NecessarioAguardarVinculoNotadeRemessaIndustrializador = Request.GetBoolParam("NecessarioAguardarVinculoNotadeRemessaIndustrializador");
            configuracaoCalculoFrete.ImportarVeiculoMDFEEmbarcador = Request.GetBoolParam("ImportarVeiculoMDFEEmbarcador");
            configuracaoCalculoFrete.UtilizarContratoFreteCliente = Request.GetBoolParam("UtilizarContratoFreteCliente");
            configuracaoCalculoFrete.RatearValorFreteInformadoEmbarcador = Request.GetBoolParam("RatearValorFreteInformadoEmbarcador");
            configuracaoCalculoFrete.UtilizarCoberturaDeCarga = Request.GetBoolParam("UtilizarCoberturaDeCarga");
            configuracaoCalculoFrete.ExigirComprovantesLiberacaoPagamentoContratoFrete = Request.GetBoolParam("ExigirComprovantesLiberacaoPagamentoContratoFrete");

            if (configuracaoCalculoFrete.Codigo == 0)
                repConfiguracaoTipoOperacaoCalculoFrete.Inserir(configuracaoCalculoFrete);
            else
                repConfiguracaoTipoOperacaoCalculoFrete.Atualizar(configuracaoCalculoFrete, Auditado, historico);

            tipoOperacao.ConfiguracaoCalculoFrete = configuracaoCalculoFrete;

            repTipoOperacao.Atualizar(tipoOperacao);
        }

        private void AtualizarConfiguracoesEmissaoDocumento(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, Dominio.Entidades.Auditoria.HistoricoObjeto historico, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unidadeDeTrabalho);
            Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoEmissaoDocumento repConfiguracaoTipoOperacaoEmissaoDocumento = new Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoEmissaoDocumento(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoEmissaoDocumento configuracaoTipoOperacaoEmissaoDocumento = tipoOperacao.ConfiguracaoEmissaoDocumento ?? new Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoEmissaoDocumento();

            if (configuracaoTipoOperacaoEmissaoDocumento.Codigo > 0)
                configuracaoTipoOperacaoEmissaoDocumento.Initialize();

            configuracaoTipoOperacaoEmissaoDocumento.FinalizarCargaAutomaticamente = Request.GetBoolParam("EmissaoDocumentoFinalizarCargaAutomaticamente");
            configuracaoTipoOperacaoEmissaoDocumento.UtilizarExpedidorRecebedorPedidoSubcontratacao = Request.GetBoolParam("EmissaoDocumentoUtilizarExpedidorRecebedorPedidoSubcontratacao");
            configuracaoTipoOperacaoEmissaoDocumento.NaoPermitirAcessarDocumentosAntesCargaEmTransporte = Request.GetBoolParam("EmissaoDocumentoNaoPermitirAcessarDocumentosAntesCargaEmTransporte");
            configuracaoTipoOperacaoEmissaoDocumento.RatearPesoModeloVeicularEntreCTes = Request.GetBoolParam("EmissaoDocumentoRatearPesoModeloVeicularEntreCTes");
            configuracaoTipoOperacaoEmissaoDocumento.TipoConhecimentoProceda = Request.GetStringParam("TipoConhecimentoProceda");
            configuracaoTipoOperacaoEmissaoDocumento.DescricaoUnidadeMedidaPesoModeloVeicularRateado = configuracaoTipoOperacaoEmissaoDocumento.RatearPesoModeloVeicularEntreCTes ? Request.GetStringParam("EmissaoDocumentoDescricaoUnidadeMedidaPesoModeloVeicularRateado") : null;
            configuracaoTipoOperacaoEmissaoDocumento.ObrigatorioAprovarCtesImportados = Request.GetBoolParam("EmissaoDocumentoObrigatorioAprovarCtesImportados");
            configuracaoTipoOperacaoEmissaoDocumento.NaoPermitirLiberarSemValePedagio = Request.GetBoolParam("NaoPermitirLiberarSemValePedagio");
            configuracaoTipoOperacaoEmissaoDocumento.TipoDeEmitente = Request.GetNullableEnumParam<Dominio.Enumeradores.TipoEmitenteMDFe>("TipoDeEmitente");
            configuracaoTipoOperacaoEmissaoDocumento.NaoPermitirEmissaoComMesmaOrigemEDestino = Request.GetBoolParam("NaoPermitirEmissaoComMesmaOrigemEDestino");
            configuracaoTipoOperacaoEmissaoDocumento.ValidarRelevanciaNotasPrechekin = Request.GetBoolParam("ValidarRelevanciaNotasPrechekin");
            configuracaoTipoOperacaoEmissaoDocumento.EmitirDocumentoSempreOrigemDestinoPedido = Request.GetBoolParam("EmitirDocumentoSempreOrigemDestinoPedido");
            configuracaoTipoOperacaoEmissaoDocumento.GerarCTeSimplificadoQuandoCompativel = Request.GetBoolParam("GerarCTeSimplificadoQuandoCompativel");
            configuracaoTipoOperacaoEmissaoDocumento.ClassificacaoNFeRemessaVenda = Request.GetBoolParam("ClassificacaoNFeRemessaVenda");
            configuracaoTipoOperacaoEmissaoDocumento.EnviarParaObservacaoCTeNFeRemessa = Request.GetBoolParam("EnviarParaObservacaoCTeNFeRemessa");
            configuracaoTipoOperacaoEmissaoDocumento.EnviarParaObservacaoCTeNFeVenda = Request.GetBoolParam("EnviarParaObservacaoCTeNFeVenda");
            configuracaoTipoOperacaoEmissaoDocumento.ValorContainerAverbacao = Request.GetDecimalParam("ValorContainerAverbacao");
            configuracaoTipoOperacaoEmissaoDocumento.AverbarContainerComAverbacaoCarga = Request.GetBoolParam("AverbarContainerComAverbacaoCarga");
            configuracaoTipoOperacaoEmissaoDocumento.UtilizarOutroEnderecoPedidoMesmoSePossuirRecebedor = Request.GetBoolParam("UtilizarOutroEnderecoPedidoMesmoSePossuirRecebedor");
            configuracaoTipoOperacaoEmissaoDocumento.ChargeCodeVinculado = Request.GetIntParam("AcordoFaturamento");

            if (configuracaoTipoOperacaoEmissaoDocumento.Codigo == 0)
                repConfiguracaoTipoOperacaoEmissaoDocumento.Inserir(configuracaoTipoOperacaoEmissaoDocumento);
            else
                repConfiguracaoTipoOperacaoEmissaoDocumento.Atualizar(configuracaoTipoOperacaoEmissaoDocumento, Auditado, historico);

            tipoOperacao.ConfiguracaoEmissaoDocumento = configuracaoTipoOperacaoEmissaoDocumento;

            repTipoOperacao.Atualizar(tipoOperacao);
        }

        private void AtualizarConfiguracoesIntegracao(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, Dominio.Entidades.Auditoria.HistoricoObjeto historico, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unidadeDeTrabalho);
            Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoIntegracao repConfiguracaoTipoOperacaoIntegracao = new Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoIntegracao(unidadeDeTrabalho);
            Repositorio.Embarcador.Pessoas.TipoTerceiro repTipoTerceiro = new Repositorio.Embarcador.Pessoas.TipoTerceiro(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoIntegracao configuracaoTipoOperacaoIntegracao = tipoOperacao.ConfiguracaoIntegracao ?? new Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoIntegracao();

            if (configuracaoTipoOperacaoIntegracao.Codigo > 0)
                configuracaoTipoOperacaoIntegracao.Initialize();

            configuracaoTipoOperacaoIntegracao.UtilizarTipoIntegracaoGrupoPessoas = Request.GetBoolParam("IntegracaoUtilizarTipoIntegracaoGrupoPessoas");
            configuracaoTipoOperacaoIntegracao.AtivarRegraCancelamentoDosPedidosMichelin = Request.GetBoolParam("AtivarRegraCancelamentoDosPedidosMichelin");
            configuracaoTipoOperacaoIntegracao.HorasParaCalculoCancelamento = Request.GetIntParam("HorasParaCalculoCancelamento");
            configuracaoTipoOperacaoIntegracao.HabilitarIntegracaoAvancoParaEmissao = Request.GetBoolParam("HabilitarIntegracaoAvancoParaEmissao");
            configuracaoTipoOperacaoIntegracao.ManterIntegracoesReferenciandoCargaDeOrigemDoRedespacho = Request.GetBoolParam("ManterIntegracoesReferenciandoCargaDeOrigemDoRedespacho");
            configuracaoTipoOperacaoIntegracao.CalcularGerarGNRE = Request.GetBoolParam("CalcularGerarGNRE");
            configuracaoTipoOperacaoIntegracao.IntegrarCargasGeradasMultiEmbarcador = Request.GetBoolParam("IntegrarCargasGeradasMultiEmbarcador");
            configuracaoTipoOperacaoIntegracao.EnviarApenasPrimeiroPedidoNaOpentech = Request.GetBoolParam("EnviarApenasPrimeiroPedidoNaOpentech");
            configuracaoTipoOperacaoIntegracao.EnviarInformacoesTotaisDaCargaNaOpentech = Request.GetBoolParam("EnviarInformacoesTotaisDaCargaNaOpentech");
            configuracaoTipoOperacaoIntegracao.ValidarSomenteVeiculoMotoristaOpentech = Request.GetBoolParam("ValidarSomenteVeiculoMotoristaOpentech");
            configuracaoTipoOperacaoIntegracao.DefinirParaNaoMonitorarRetornoIntegracaoBounny = Request.GetBoolParam("DefinirParaNaoMonitorarRetornoIntegracaoBounny");
            configuracaoTipoOperacaoIntegracao.IntegrarDocumentos = Request.GetBoolParam("IntegrarDocumentos");
            configuracaoTipoOperacaoIntegracao.IntegrarDadosTransporte = Request.GetBoolParam("IntegrarDadosTransporte");
            configuracaoTipoOperacaoIntegracao.GerarIntegracaoKlios = Request.GetBoolParam("GerarIntegracaoKlios");
            configuracaoTipoOperacaoIntegracao.EnviarTagsIntegracaoMarfrigComTomadorServico = Request.GetBoolParam("EnviarTagsIntegracaoMarfrigComTomadorServico");
            configuracaoTipoOperacaoIntegracao.PossuiTempoEnvioIntegracaoDocumentosCarga = Request.GetBoolParam("PossuiTempoEnvioIntegracaoDocumentosCarga");
            configuracaoTipoOperacaoIntegracao.ConsultarTaxasKMM = Request.GetBoolParam("ConsultarTaxasKMM");
            configuracaoTipoOperacaoIntegracao.NaoGerarIntegracaoRetornoConfirmacaoColeta = Request.GetBoolParam("NaoGerarIntegracaoRetornoConfirmacaoColeta");
            configuracaoTipoOperacaoIntegracao.NaoIntegrarEtapa1Opentech = Request.GetBoolParam("NaoIntegrarEtapa1Opentech");

            if (configuracaoTipoOperacaoIntegracao.ConsultarTaxasKMM)
            {
                if (configuracaoTipoOperacaoIntegracao.TiposTerceiros == null)
                    configuracaoTipoOperacaoIntegracao.TiposTerceiros = new List<Dominio.Entidades.Embarcador.Pessoas.TipoTerceiro>();

                var jsonString = Request.GetStringParam("TiposTerceiros");

                if (jsonString.StartsWith("\"") && jsonString.EndsWith("\""))
                    jsonString = jsonString[1..^1];
                jsonString = jsonString.Replace("\\", "");
                var tipoTerceiroLista = Newtonsoft.Json.JsonConvert.DeserializeObject<List<dynamic>>(jsonString);

                configuracaoTipoOperacaoIntegracao.TiposTerceiros.Clear();
                foreach (var item in tipoTerceiroLista)
                {
                    var codigoTerceiro = (int)item.Terceiro.Codigo;
                    var tipoTerceiro = repTipoTerceiro.BuscarPorCodigo(codigoTerceiro);
                    if (tipoTerceiro != null)
                        configuracaoTipoOperacaoIntegracao.TiposTerceiros.Add(tipoTerceiro);
                }
            }

            if (configuracaoTipoOperacaoIntegracao.Codigo == 0)
                repConfiguracaoTipoOperacaoIntegracao.Inserir(configuracaoTipoOperacaoIntegracao);
            else
                repConfiguracaoTipoOperacaoIntegracao.Atualizar(configuracaoTipoOperacaoIntegracao, Auditado, historico);

            tipoOperacao.ConfiguracaoIntegracao = configuracaoTipoOperacaoIntegracao;
            repTipoOperacao.Atualizar(tipoOperacao);
        }

        private void AtualizarConfiguracoesCarga(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, Dominio.Entidades.Auditoria.HistoricoObjeto historico, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unidadeDeTrabalho);
            Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoCarga repConfiguracaoTipoOperacaoCarga = new Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoCarga(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoCarga configuracaoTipoOperacaoCarga = tipoOperacao.ConfiguracaoCarga ?? new Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoCarga();

            if (configuracaoTipoOperacaoCarga.Codigo > 0)
                configuracaoTipoOperacaoCarga.Initialize();

            configuracaoTipoOperacaoCarga.NaoPermitirInformarMotoristaComCNHVencida = Request.GetBoolParam("NaoPermitirInformarMotoristaComCNHVencida");
            configuracaoTipoOperacaoCarga.MultiplicarQuantidadeProdutoPorCaixaPelaQuantidadeCaixa = Request.GetBoolParam("MultiplicarQuantidadeProdutoPorCaixaPelaQuantidadeCaixa");
            configuracaoTipoOperacaoCarga.ExigeInformarIscaNaCargaComValorMaiorQue = Request.GetDecimalParam("ExigeInformarIscaNaCargaComValorMaiorQue");
            configuracaoTipoOperacaoCarga.ValorLimiteNaCarga = Request.GetDecimalParam("ValorLimiteNaCarga");
            configuracaoTipoOperacaoCarga.AdicionarBLComoOutroDocumentoAutomaticamenteNaCarga = Request.GetBoolParam("AdicionarBLComoOutroDocumentoAutomaticamenteNaCarga");
            configuracaoTipoOperacaoCarga.PermiteAdicionarAnexosGuarita = Request.GetBoolParam("PermiteAdicionarAnexosGuarita");
            configuracaoTipoOperacaoCarga.EnviarEmailAoGerarCargaPlanejamentoPedidosDetalhado = Request.GetBoolParam("EnviarEmailAoGerarCargaPlanejamentoPedidosDetalhado");
            configuracaoTipoOperacaoCarga.PermitirVisualizarOrdenarAsZonasDeTransporte = Request.GetBoolParam("PermitirVisualizarOrdenarAsZonasDeTransporte");
            configuracaoTipoOperacaoCarga.ExibirOperadorInsercaoCargaNoPortalTransportador = Request.GetBoolParam("ExibirOperadorInsercaoCargaNoPortalTransportador");
            configuracaoTipoOperacaoCarga.PermitirAdicionarObservacaoNaEtapaUmDaCarga = Request.GetBoolParam("PermitirAdicionarObservacaoNaEtapaUmDaCarga");
            configuracaoTipoOperacaoCarga.ExibirNumeroPedidoNosDetalhesDaCarga = Request.GetBoolParam("ExibirNumeroPedidoNosDetalhesDaCarga");
            configuracaoTipoOperacaoCarga.TempoParaAlertarPorEmailResponsavelDaFilialDaCargaQueAindaNaoTeveOsDadosDeTransporteInformados = Request.GetIntParam("TempoParaAlertarPorEmailResponsavelDaFilialDaCargaQueAindaNaoTeveOsDadosDeTransporteInformados");
            configuracaoTipoOperacaoCarga.TempoParaRecebimentoDosPacotes = Request.GetIntParam("TempoParaRecebimentoDosPacotes");
            configuracaoTipoOperacaoCarga.PercentualToleranciaParaEmissaoEntreCTesRecebidosVersusPacotes = Request.GetIntParam("PercentualToleranciaParaEmissaoEntreCTesRecebidosVersusPacotes");
            configuracaoTipoOperacaoCarga.QuantidadeDiasValidacaoNFeDataCarregamento = Request.GetIntParam("QuantidadeDiasValidacaoNFeDataCarregamento");
            configuracaoTipoOperacaoCarga.ExibirFiltroDePedidosEtapaNotaFiscal = Request.GetBoolParam("ExibirFiltroDePedidosEtapaNotaFiscal");
            configuracaoTipoOperacaoCarga.PermitirAlterarDataRetornoCDCarga = Request.GetBoolParam("PermitirAlterarDataRetornoCDCarga");
            configuracaoTipoOperacaoCarga.IgnorarRateioConfiguradoPorto = Request.GetBoolParam("IgnorarRateioConfiguradoPorto");
            configuracaoTipoOperacaoCarga.ValidarLoteProdutoVersusLoteNotaFiscal = Request.GetBoolParam("ValidarLoteProdutoVersusLoteNotaFiscal");
            configuracaoTipoOperacaoCarga.ExecutarCalculoRelevanciaDeCustoNFePorCFOP = Request.GetBoolParam("ExecutarCalculoRelevanciaDeCustoNFePorCFOP");
            configuracaoTipoOperacaoCarga.AguardarRecebimentoProdutoParaProvisionar = Request.GetBoolParam("AguardarRecebimentoProdutoParaProvisionar");
            configuracaoTipoOperacaoCarga.AlertarAlteracoesPedidoNoFluxoPatio = Request.GetBoolParam("AlertarAlteracoesPedidoNoFluxoPatio");
            configuracaoTipoOperacaoCarga.AtivoModuloNaoConformidades = Request.GetBoolParam("AtivoModuloNaoConformidades");
            configuracaoTipoOperacaoCarga.HerdarDadosDeTransporteCargaPrimeiroTrecho = Request.GetBoolParam("HerdarDadosDeTransporteCargaPrimeiroTrecho");
            configuracaoTipoOperacaoCarga.ConsiderarKMRecibidoDoEmbarcador = Request.GetBoolParam("ConsiderarKMRecibidoDoEmbarcador");
            configuracaoTipoOperacaoCarga.PermitirAdicionarNovosPedidosPorNotasAvulsas = Request.GetBoolParam("PermitirAdicionarNovosPedidosPorNotasAvulsas");
            configuracaoTipoOperacaoCarga.IntegradoERP = false;
            configuracaoTipoOperacaoCarga.GerarRetornoAutomaticoMomento = Request.GetEnumParam<GerarRetornoAutomaticoMomento>("GerarRetornoAutomaticoMomento");
            configuracaoTipoOperacaoCarga.ObrigarInformarValePedagio = Request.GetBoolParam("ObrigarInformarValePedagio");
            configuracaoTipoOperacaoCarga.PermitirRelacionarOutrasCargas = Request.GetBoolParam("PermitirRelacionarOutrasCargas");
            configuracaoTipoOperacaoCarga.ExigeNotaFiscalTenhaTagRetirada = Request.GetBoolParam("ExigeNotaFiscalTenhaTagRetirada");
            configuracaoTipoOperacaoCarga.VincularPedidoDeAcordoComNumeroOrdem = Request.GetBoolParam("VincularPedidoDeAcordoComNumeroOrdem");
            configuracaoTipoOperacaoCarga.DeixarPedidosDisponiveisParaMontegemCarga = Request.GetBoolParam("DeixarPedidosDisponiveisParaMontegemCarga");
            configuracaoTipoOperacaoCarga.PrecisaEsperarNotasFilhaParaGerarPagamento = Request.GetBoolParam("PrecisaEsperarNotasFilhaParaGerarPagamento");
            configuracaoTipoOperacaoCarga.ValidarPesoDasNotasRelevantes = Request.GetBoolParam("ValidarPesoDasNotasRelevantes");
            configuracaoTipoOperacaoCarga.PrecisaEsperarNotaTransferenciaParaGeraPagamento = Request.GetBoolParam("PrecisaEsperarNotaTransferenciaParaGeraPagamento");
            configuracaoTipoOperacaoCarga.ConsiderarKMDaRotaFrete = Request.GetBoolParam("ConsiderarKMDaRotaFrete");
            configuracaoTipoOperacaoCarga.GerarCargaEspelhoAoConfirmarEntrega = Request.GetBoolParam("GerarCargaEspelhoAoConfirmarEntrega");
            configuracaoTipoOperacaoCarga.UtilizarRotaFreteInformadoPedido = Request.GetBoolParam("UtilizarRotaFreteInformadoPedido");
            configuracaoTipoOperacaoCarga.BloquearInclusaoArquivosXMLDeNFeCarga = Request.GetBoolParam("BloquearInclusaoArquivosXMLDeNFeCarga");
            configuracaoTipoOperacaoCarga.PermitirIntegrarPacotes = Request.GetBoolParam("PermitirIntegrarPacotes");
            configuracaoTipoOperacaoCarga.TipoCancelamentoCargaDocumento = Request.GetEnumParam<TipoCancelamentoCargaDocumento>("TipoDeCancelamentoDaCarga");
            configuracaoTipoOperacaoCarga.TipoOperacaoCargaEspelho = repTipoOperacao.BuscarPorCodigo(Request.GetIntParam("TipoOperacaoCargaEspelho"));
            configuracaoTipoOperacaoCarga.NaoPermitirUsoNotasQueEstaoEmOutraCarga = Request.GetBoolParam("NaoPermitirUsoNotasQueEstaoEmOutraCarga");
            configuracaoTipoOperacaoCarga.InformarLacreNosDadosTransporte = Request.GetBoolParam("InformarLacreNosDadosTransporte");
            configuracaoTipoOperacaoCarga.NaoPermitirEditarPesoBrutoDaNotaFiscalEletronica = Request.GetBoolParam("NaoPermitirEditarPesoBrutoDaNotaFiscalEletronica");
            configuracaoTipoOperacaoCarga.TipoDeEnvioPorSMSDeDocumentos = Request.GetEnumParam<TipoDeEnvioPorSMSDeDocumentos>("TipoDeEnvioPorSMSDeDocumentos");
            configuracaoTipoOperacaoCarga.UtilizarDistribuidorPorRegiaoNaRegiaoDestino = Request.GetBoolParam("UtilizarDistribuidorPorRegiaoNaRegiaoDestino");
            configuracaoTipoOperacaoCarga.GerarCargaRetornoRejeitarTodasColetas = Request.GetBoolParam("GerarCargaRetornoRejeitarTodasColetas");
            configuracaoTipoOperacaoCarga.ValidaValorPreCalculoValorFrete = Request.GetBoolParam("ValidaValorPreCalculoValorFrete");
            configuracaoTipoOperacaoCarga.HabilitarConsultaContainerEMP = Request.GetBoolParam("HabilitarConsultaContainerEMP");
            configuracaoTipoOperacaoCarga.PermitirInformarAjudantesNaCarga = Request.GetBoolParam("PermitirInformarAjudantesNaCarga");
            configuracaoTipoOperacaoCarga.ValidarValorMinimoCarga = Request.GetBoolParam("ValidarValorMinimoCarga");
            configuracaoTipoOperacaoCarga.ValorMinimoCarga = Request.GetDecimalParam("ValorMinimoCarga");
            configuracaoTipoOperacaoCarga.PermiteInformarTransportadorEtapaUmQuandoNotaFiscalNaoRecebidaAntesCalculoFrete = Request.GetBoolParam("PermiteInformarTransportadorEtapaUmQuandoNotaFiscalNaoRecebidaAntesCalculoFrete");
            configuracaoTipoOperacaoCarga.AvancarCargaAutomaticamenteAoReceberIntegracaoNotasWS = Request.GetBoolParam("AvancarCargaAutomaticamenteAoReceberIntegracaoNotasWS");
            configuracaoTipoOperacaoCarga.ManterCargaNaEtapaUmAteXHorasAntesDaDataDeCarregamento = Request.GetBoolParam("ManterCargaNaEtapaUmAteXHorasAntesDaDataDeCarregamento");
            configuracaoTipoOperacaoCarga.HorasManterCargaNaEtapaUmAteXHorasAntesDaDataDeCarregamento = Request.GetIntParam("HorasManterCargaNaEtapaUmAteXHorasAntesDaDataDeCarregamento");
            configuracaoTipoOperacaoCarga.NaoAvancarEtapaSePlacaEstiverEmMonitoramento = configuracaoTipoOperacaoCarga.ManterCargaNaEtapaUmAteXHorasAntesDaDataDeCarregamento && Request.GetBoolParam("NaoAvancarEtapaSePlacaEstiverEmMonitoramento");
            configuracaoTipoOperacaoCarga.TipoOperacaoPreCarga = Request.GetBoolParam("TipoOperacaoPreCarga");
            configuracaoTipoOperacaoCarga.PermitirSelecionarPreCargaNaCarga = Request.GetBoolParam("PermitirSelecionarPreCargaNaCarga");
            configuracaoTipoOperacaoCarga.ObrigatorioInformarAliquotaImpostoSuspensoeValor = Request.GetBoolParam("obrigatorioInformarAliquotaImpostoSuspensoeValor");
            configuracaoTipoOperacaoCarga.LiberarCargaSemPlanejamento = Request.GetBoolParam("LiberarCargaSemPlanejamento");
            configuracaoTipoOperacaoCarga.TipoOperacaoCargaRetornoColetasRejeitadas = repTipoOperacao.BuscarPorCodigo(Request.GetIntParam("TipoOperacaoCargaRetornoColetasRejeitadas"));
            configuracaoTipoOperacaoCarga.IncrementaCodigoPorTipoOperacao = Request.GetBoolParam("IncrementaCodigoPorTipoOperacao");
            configuracaoTipoOperacaoCarga.AdicionaPrefixoCodigoCarga = Request.GetStringParam("AdicionaPrefixoCodigoCarga");
            configuracaoTipoOperacaoCarga.ReceberAtualizacaoDeTransporteEmQualquerSituacaoCarga = Request.GetBoolParam("ReceberAtualizacaoDeTransporteEmQualquerSituacaoCarga");
            configuracaoTipoOperacaoCarga.UtilizarDirecionamentoCustoExtra = Request.GetBoolParam("UtilizarDirecionamentoCustoExtra");
            configuracaoTipoOperacaoCarga.DirecionamentoCustoExtra = configuracaoTipoOperacaoCarga.UtilizarDirecionamentoCustoExtra ? Request.GetEnumParam<TipoDirecionamentoCustoExtra>("DirecionamentoCustoExtra") : TipoDirecionamentoCustoExtra.Nenhum;
            configuracaoTipoOperacaoCarga.ObrigatorioJustificarCustoExtra = Request.GetBoolParam("ObrigatorioJustificarCustoExtra");
            configuracaoTipoOperacaoCarga.UtilizaIntegracaoOKColeta = Request.GetBoolParam("UtilizaIntegracaoOKColeta");
            configuracaoTipoOperacaoCarga.BuscarDocumentosEAverbacaoPelaOSMae = Request.GetBoolParam("BuscarDocumentosEAverbacaoPelaOSMae");
            configuracaoTipoOperacaoCarga.AcordoFaturamento = Request.GetEnumParam<TipoAcordoFaturamento>("AcordoFaturamento");
            configuracaoTipoOperacaoCarga.DocumentoProvedor = Request.GetEnumParam<TipoDocumentoProvedor>("TipoDocumentoProvedor");
            configuracaoTipoOperacaoCarga.GerarRedespachoAutomaticamenteAposEmissaoDocumentos = Request.GetBoolParam("GerarRedespachoAutomaticamenteAposEmissaoDocumentos");
            configuracaoTipoOperacaoCarga.InformarTransportadorSubcontratadoEtapaUm = Request.GetBoolParam("InformarTransportadorSubcontratadoEtapaUm");
            configuracaoTipoOperacaoCarga.GerarCargaEspelhoAutomaticamenteAoFinalizarCarga = Request.GetBoolParam("GerarCargaEspelhoAutomaticamenteAoFinalizarCarga");
            configuracaoTipoOperacaoCarga.NecessitaInformarPlacaCarregamento = Request.GetBoolParam("NecessitaInformarPlacaCarregamento");
            configuracaoTipoOperacaoCarga.DisponibilizarNotaFiscalNoPedidoAoFinalizarCarga = Request.GetBoolParam("DisponibilizarNotaFiscalNoPedidoAoFinalizarCarga");
            configuracaoTipoOperacaoCarga.HabilitarVinculoMotoristaGenericoCarga = Request.GetBoolParam("HabilitarVinculoMotoristaGenericoCarga");
            configuracaoTipoOperacaoCarga.AvancarCargaQuandoPedidoZeroPacotes = Request.GetBoolParam("AvancarCargaQuandoPedidoZeroPacotes");
            configuracaoTipoOperacaoCarga.LiberarPedidoComRecebedorParaMontagemCarga = Request.GetBoolParam("LiberarPedidoComRecebedorParaMontagemCarga");
            configuracaoTipoOperacaoCarga.BuscarPacoteMesmoCNPJAdicionarCargaAposConsulta = Request.GetBoolParam("BuscarPacoteMesmoCNPJAdicionarCargaAposConsulta");
            configuracaoTipoOperacaoCarga.GerarMDFeParaRecebedorDaCarga = Request.GetBoolParam("GerarMDFeParaRecebedorDaCarga");
            configuracaoTipoOperacaoCarga.TipoRotaCarga = Request.GetEnumParam<TipoRotaCarga>("TipoRotaCarga");
            configuracaoTipoOperacaoCarga.TipoOperacaoInternacional = Request.GetBoolParam("TipoOperacaoInternacional");
            configuracaoTipoOperacaoCarga.LayoutEmailTipoPropostaTipoOperacao = Request.GetBoolParam("LayoutEmailTipoPropostaTipoOperacao");
            configuracaoTipoOperacaoCarga.NaoPermitirAvancarCargaComTracaoSemReboque = Request.GetBoolParam("NaoPermitirAvancarCargaComTracaoSemReboque");
            configuracaoTipoOperacaoCarga.RoteirizarCargaEtapaNotaFiscal = Request.GetBoolParam("RoteirizarCargaEtapaNotaFiscal");
            configuracaoTipoOperacaoCarga.RetornarSituacaoAoRemoverPedidos = Request.GetBoolParam("RetornarSituacaoAoRemoverPedidos");
            configuracaoTipoOperacaoCarga.SituacaoAposRemocaoPedidos = Request.GetEnumParam<SituacaoCarga>("SituacaoAposRemocaoPedidos");
            configuracaoTipoOperacaoCarga.RemarkSped = Request.GetEnumParam<RemarkSped>("RemarkSped", RemarkSped.OutrosServicos);
            configuracaoTipoOperacaoCarga.GerarCargaDeRedespachoVinculandoApenasUmaNotaPorEntrega = Request.GetBoolParam("GerarCargaDeRedespachoVinculandoApenasUmaNotaPorEntrega");
            configuracaoTipoOperacaoCarga.PermitirIncluirCTesDeDiferentesUFsEmMDFeUnico = Request.GetBoolParam("PermitirIncluirCTesDeDiferentesUFsEmMDFeUnico");
            configuracaoTipoOperacaoCarga.PermiteInformarMotoristasSomenteEtapaUmQuandoNotasFiscaisNaoSaoRecebidasAntesCalcularFrete = Request.GetBoolParam("PermiteInformarMotoristasSomenteEtapaUmQuandoNotasFiscaisNaoSaoRecebidasAntesCalcularFrete");

            if (tipoOperacao.ExigeNotaFiscalParaCalcularFrete)
            {
                configuracaoTipoOperacaoCarga.PermiteInformarMotoristasSomenteEtapaUmQuandoNotasFiscaisNaoSaoRecebidasAntesCalcularFrete = false;
            }

            if (configuracaoTipoOperacaoCarga.Codigo == 0)
                repConfiguracaoTipoOperacaoCarga.Inserir(configuracaoTipoOperacaoCarga);
            else
                repConfiguracaoTipoOperacaoCarga.Atualizar(configuracaoTipoOperacaoCarga, Auditado, historico);

            // Confiurações estaduais..
            dynamic configEstado = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ConfiguracoesCargaEstado"));
            if (configEstado != null)
            {
                Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoCargaEstado repositorioConfiguracaoTipoOperacaoCargaEstado = new Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoCargaEstado(unidadeDeTrabalho);
                List<Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoCargaEstado> configuracoesTipoOperacaoCargaEstado = repositorioConfiguracaoTipoOperacaoCargaEstado.BuscarPorConfiguracao(configuracaoTipoOperacaoCarga.Codigo);

                List<int> codigosSalvando = new List<int>();
                foreach (dynamic config in configEstado)
                {
                    int codigo = ((string)config.Codigo).ToInt();
                    if (codigo > 0)
                        codigosSalvando.Add(codigo);
                }

                //Excluindo os removidos....
                foreach (Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoCargaEstado configuracaoTipoOperacaoCargaEstado in configuracoesTipoOperacaoCargaEstado)
                {
                    if (!codigosSalvando.Contains(configuracaoTipoOperacaoCargaEstado.Codigo))
                        repositorioConfiguracaoTipoOperacaoCargaEstado.Deletar(configuracaoTipoOperacaoCargaEstado);
                }

                //Agora vamos salvar...
                foreach (dynamic config in configEstado)
                {
                    int codigo = ((string)config.Codigo).ToInt();

                    Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoCargaEstado configuracaoTipoOperacaoCargaEstado = new Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoCargaEstado();
                    if (codigo > 0)
                        configuracaoTipoOperacaoCargaEstado = repositorioConfiguracaoTipoOperacaoCargaEstado.BuscarPorCodigo(codigo, false);

                    configuracaoTipoOperacaoCargaEstado.Configuracao = configuracaoTipoOperacaoCarga;
                    configuracaoTipoOperacaoCargaEstado.Estado = new Dominio.Entidades.Estado() { Sigla = (string)config.CodigoEstado };
                    configuracaoTipoOperacaoCargaEstado.ExigeInformarIscaNaCargaComValorMaiorQue = ((string)config.ExigeInformarIscaNaCargaComValorMaiorQue).ToDecimal();

                    if (codigo > 0)
                        repositorioConfiguracaoTipoOperacaoCargaEstado.Atualizar(configuracaoTipoOperacaoCargaEstado);
                    else
                        repositorioConfiguracaoTipoOperacaoCargaEstado.Inserir(configuracaoTipoOperacaoCargaEstado);
                }
            }

            tipoOperacao.ConfiguracaoCarga = configuracaoTipoOperacaoCarga;

            repTipoOperacao.Atualizar(tipoOperacao);
        }

        private void AtualizarConfiguracoesImpressao(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, Dominio.Entidades.Auditoria.HistoricoObjeto historico, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unidadeDeTrabalho);
            Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoImpressao repConfiguracaoTipoOperacaoImpressao = new Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoImpressao(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoImpressao configuracaoTipoOperacaoImpressao = tipoOperacao.ConfiguracaoImpressao ?? new Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoImpressao();

            if (configuracaoTipoOperacaoImpressao.Codigo > 0)
                configuracaoTipoOperacaoImpressao.Initialize();

            configuracaoTipoOperacaoImpressao.PermiteBaixarComprovanteColeta = Request.GetBoolParam("PermiteBaixarComprovanteColeta");
            configuracaoTipoOperacaoImpressao.EnviarPlanoViagemTransportador = Request.GetBoolParam("EnviarEmailPlanoViagemTransportador");
            configuracaoTipoOperacaoImpressao.OcultarQuantidadeValoresOrdemColeta = Request.GetBoolParam("OcultarQuantidadeValoresOrdemColeta");
            configuracaoTipoOperacaoImpressao.ImprimirMinuta = Request.GetBoolParam("ImprimirMinuta");
            configuracaoTipoOperacaoImpressao.AlterarLayoutDaFaturaIncluirTipoServico = Request.GetBoolParam("AlterarLayoutDaFaturaIncluirTipoServico");

            if (configuracaoTipoOperacaoImpressao.Codigo == 0)
                repConfiguracaoTipoOperacaoImpressao.Inserir(configuracaoTipoOperacaoImpressao);
            else
                repConfiguracaoTipoOperacaoImpressao.Atualizar(configuracaoTipoOperacaoImpressao, Auditado, historico);

            tipoOperacao.ConfiguracaoImpressao = configuracaoTipoOperacaoImpressao;

            repTipoOperacao.Atualizar(tipoOperacao);
        }

        private void AtualizarConfiguracoesPagamentos(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, Dominio.Entidades.Auditoria.HistoricoObjeto historico, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unidadeDeTrabalho);
            Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoPagamentos repConfiguracaoTipoOperacaoPagamentos = new Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoPagamentos(unidadeDeTrabalho);
            Repositorio.Embarcador.Financeiro.CentroResultado repositorioCentroResultado = new Repositorio.Embarcador.Financeiro.CentroResultado(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoPagamentos configuracaoTipoOperacaoPagamentos = tipoOperacao.ConfiguracaoPagamentos ?? new Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoPagamentos();

            if (configuracaoTipoOperacaoPagamentos.Codigo > 0)
                configuracaoTipoOperacaoPagamentos.Initialize();

            configuracaoTipoOperacaoPagamentos.TipoLiberacaoPagamento = Request.GetEnumParam<TipoLiberacaoPagamento>("TipoLiberacaoPagamento");

            int codigoCentroResultado = Request.GetIntParam("CentroDeResultado");
            configuracaoTipoOperacaoPagamentos.CentroResultado = codigoCentroResultado > 0 ? repositorioCentroResultado.BuscarPorCodigo(codigoCentroResultado) : null;

            if (configuracaoTipoOperacaoPagamentos.Codigo == 0)
                repConfiguracaoTipoOperacaoPagamentos.Inserir(configuracaoTipoOperacaoPagamentos);
            else
                repConfiguracaoTipoOperacaoPagamentos.Atualizar(configuracaoTipoOperacaoPagamentos, Auditado, historico);

            tipoOperacao.ConfiguracaoPagamentos = configuracaoTipoOperacaoPagamentos;

            repTipoOperacao.Atualizar(tipoOperacao);
        }

        private void AtualizarConfiguracoesPedido(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, Dominio.Entidades.Auditoria.HistoricoObjeto historico, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unidadeDeTrabalho);
            Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoPedido repConfiguracaoTipoOperacaoPedido = new Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoPedido(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoPedido configuracaoTipoOperacaoPedido = tipoOperacao.ConfiguracaoPedido ?? new Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoPedido();

            if (configuracaoTipoOperacaoPedido.Codigo > 0)
                configuracaoTipoOperacaoPedido.Initialize();

            configuracaoTipoOperacaoPedido.BloquearInclusaoAlteracaoPedidosNaoTenhamTabelaFreteConfigurada = Request.GetBoolParam("BloquearInclusaoAlteracaoPedidosNaoTenhamTabelaFreteConfigurada");
            configuracaoTipoOperacaoPedido.FiltrarPedidosPorRemetenteRetiradaProduto = Request.GetBoolParam("FiltrarPedidosPorRemetenteRetiradaProduto");
            configuracaoTipoOperacaoPedido.EnviarPedidoReentregaAutomaticamenteRoteirizar = Request.GetBoolParam("EnviarPedidoReentregaAutomaticamenteRoteirizar");

            if (configuracaoTipoOperacaoPedido.Codigo == 0)
                repConfiguracaoTipoOperacaoPedido.Inserir(configuracaoTipoOperacaoPedido);
            else
                repConfiguracaoTipoOperacaoPedido.Atualizar(configuracaoTipoOperacaoPedido, Auditado, historico);

            tipoOperacao.ConfiguracaoPedido = configuracaoTipoOperacaoPedido;

            repTipoOperacao.Atualizar(tipoOperacao);
        }
        private void AtualizarConfiguracoesIntercab(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, Dominio.Entidades.Auditoria.HistoricoObjeto historico, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unidadeDeTrabalho);
            Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoIntercab repConfiguracaoTipoOperacaoIntercab = new Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoIntercab(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoIntercab configuracaoTipoOperacaoIntercab = tipoOperacao.ConfiguracaoIntercab ?? new Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoIntercab();

            if (configuracaoTipoOperacaoIntercab.Codigo > 0)
                configuracaoTipoOperacaoIntercab.Initialize();

            configuracaoTipoOperacaoIntercab.Tomador = Request.GetEnumParam<TipoTomadorCabotagem>("Tomador");
            configuracaoTipoOperacaoIntercab.ModalProposta = Request.GetEnumParam<TipoModalPropostaCabotagem>("ModalProposta");
            configuracaoTipoOperacaoIntercab.TipoProposta = Request.GetEnumParam<TipoPropostaCabotagem>("TipoProposta");

            if (configuracaoTipoOperacaoIntercab.Codigo == 0)
                repConfiguracaoTipoOperacaoIntercab.Inserir(configuracaoTipoOperacaoIntercab);
            else
                repConfiguracaoTipoOperacaoIntercab.Atualizar(configuracaoTipoOperacaoIntercab, Auditado, historico);

            tipoOperacao.ConfiguracaoIntercab = configuracaoTipoOperacaoIntercab;

            repTipoOperacao.Atualizar(tipoOperacao);
        }

        private async Task AtualizarConfiguracoesTrizyAsync(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, Dominio.Entidades.Auditoria.HistoricoObjeto historico, Repositorio.UnitOfWork unidadeDeTrabalho, CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unidadeDeTrabalho);
            Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoTrizy repConfiguracaoTipoOperacaoTrizy = new Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoTrizy(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoTrizy configuracaoTipoOperacaoTrizy = tipoOperacao.ConfiguracaoTrizy ?? new Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoTrizy();

            if (configuracaoTipoOperacaoTrizy.Codigo > 0)
                configuracaoTipoOperacaoTrizy.Initialize();

            configuracaoTipoOperacaoTrizy.EnviarEstouIndoColeta = Request.GetBoolParam("EnviarEstouIndoColeta");
            configuracaoTipoOperacaoTrizy.EnviarEstouIndoEntrega = Request.GetBoolParam("EnviarEstouIndoEntrega");
            configuracaoTipoOperacaoTrizy.SolicitarComprovanteColetaEntrega = Request.GetBoolParam("SolicitarComprovanteColetaEntrega");
            configuracaoTipoOperacaoTrizy.EnviarPrimeiraColetaComoOrigemNoLugarDoRemetente = Request.GetBoolParam("EnviarPrimeiraColetaComoOrigemNoLugarDoRemetente");
            configuracaoTipoOperacaoTrizy.HabilitarChat = Request.GetBoolParam("HabilitarChat");
            configuracaoTipoOperacaoTrizy.EnviarInicioViagemColeta = Request.GetBoolParam("EnviarInicioViagemColeta");
            configuracaoTipoOperacaoTrizy.EnviarInicioViagemEntrega = Request.GetBoolParam("EnviarInicioViagemEntrega");
            configuracaoTipoOperacaoTrizy.EnviarEventoIniciarViagemComoOpcional = configuracaoTipoOperacaoTrizy.EnviarInicioViagemColeta && Request.GetBoolParam("EnviarEventoIniciarViagemComoOpcional");
            configuracaoTipoOperacaoTrizy.EnviarMensagemAlertaPreTrip = Request.GetBoolParam("EnviarMensagemAlertaPreTrip");
            configuracaoTipoOperacaoTrizy.EnviarPreTripJuntoAoNumeroCarga = Request.GetBoolParam("EnviarPreTripJuntoAoNumeroCarga");
            configuracaoTipoOperacaoTrizy.EnviarIniciarViagemColetaPreTrip = Request.GetBoolParam("EnviarIniciarViagemColetaPreTrip");
            configuracaoTipoOperacaoTrizy.EnviarIniciarViagemEntregaPreTrip = Request.GetBoolParam("EnviarIniciarViagemEntregaPreTrip");
            configuracaoTipoOperacaoTrizy.EnviarEventoIniciarViagemComoOpcionalPreTrip = configuracaoTipoOperacaoTrizy.EnviarIniciarViagemColetaPreTrip && Request.GetBoolParam("EnviarEventoIniciarViagemComoOpcionalPreTrip");
            configuracaoTipoOperacaoTrizy.EnviarEstouIndoColetaPreTrip = Request.GetBoolParam("EnviarEstouIndoColetaPreTrip");
            configuracaoTipoOperacaoTrizy.EnviarEstouIndoEntregaPreTrip = Request.GetBoolParam("EnviarEstouIndoEntregaPreTrip");
            configuracaoTipoOperacaoTrizy.EnviarPrimeiraColetaComoOrigemNoLugarDoRemetentePreTrip = Request.GetBoolParam("EnviarPrimeiraColetaComoOrigemNoLugarDoRemetentePreTrip");
            configuracaoTipoOperacaoTrizy.EnviarChegueiParaCarregar = Request.GetBoolParam("EnviarChegueiParaCarregar");
            configuracaoTipoOperacaoTrizy.EnviarChegueiParaDescarregar = Request.GetBoolParam("EnviarChegueiParaDescarregar");
            configuracaoTipoOperacaoTrizy.EnviarChegueiParaCarregarPreTrip = Request.GetBoolParam("EnviarChegueiParaCarregarPreTrip");
            configuracaoTipoOperacaoTrizy.EnviarChegueiParaDescarregarPreTrip = Request.GetBoolParam("EnviarChegueiParaDescarregarPreTrip");
            configuracaoTipoOperacaoTrizy.NaoFinalizarPreTrip = Request.GetBoolParam("NaoFinalizarPreTrip");
            configuracaoTipoOperacaoTrizy.ExigirEnvioFotosDasNotasNaOrigemPreTrip = Request.GetBoolParam("ExigirEnvioFotosDasNotasNaOrigemPreTrip");
            configuracaoTipoOperacaoTrizy.SolicitarComprovanteEntregaSemOCRPreTrip = Request.GetBoolParam("SolicitarComprovanteEntregaSemOCRPreTrip");
            configuracaoTipoOperacaoTrizy.SolicitarDataeHoraDoCanhoto = Request.GetBoolParam("SolicitarDataeHoraDoCanhoto");
            configuracaoTipoOperacaoTrizy.NaoEnviarEventosViagemColetaEntregaSolicitarApenasCanhotos = Request.GetBoolParam("NaoEnviarEventosViagemColetaEntregaSolicitarApenasCanhotos");
            configuracaoTipoOperacaoTrizy.NaoEnviarEventosNaOrigem = Request.GetBoolParam("NaoEnviarEventosNaOrigem");
            configuracaoTipoOperacaoTrizy.VincularDataEHoraSolicitadaNoCanhoto = configuracaoTipoOperacaoTrizy.SolicitarDataeHoraDoCanhoto && Request.GetBoolParam("VincularDataEHoraSolicitadaNoCanhoto");
            configuracaoTipoOperacaoTrizy.NaoEnviarDocumentosFiscais = Request.GetBoolParam("NaoEnviarDocumentosFiscais");
            configuracaoTipoOperacaoTrizy.NaoEnviarTagValidacao = Request.GetBoolParam("NaoEnviarTagValidacao");
            configuracaoTipoOperacaoTrizy.SolicitarAssinaturaNaConfirmacaoDeColetaEntrega = Request.GetBoolParam("SolicitarAssinaturaNaConfirmacaoDeColetaEntrega");
            configuracaoTipoOperacaoTrizy.TituloInformacaoAdicional = Request.GetStringParam("TituloInformacaoAdicional");

            configuracaoTipoOperacaoTrizy.SolicitarFotoComoEvidenciaObrigatoria = Request.GetBoolParam("SolicitarFotoComoEvidenciaObrigatoria");
            configuracaoTipoOperacaoTrizy.SolicitarFotoComoEvidenciaOpcional = Request.GetBoolParam("SolicitarFotoComoEvidenciaOpcional");

            configuracaoTipoOperacaoTrizy.DataEsperadaParaColetas = Request.GetEnumParam<DataEsperadaColetaEntregaTrizy>("DataEsperadaParaColetas");
            configuracaoTipoOperacaoTrizy.DataEsperadaParaEntregas = Request.GetEnumParam<DataEsperadaColetaEntregaTrizy>("DataEsperadaParaEntregas");

            configuracaoTipoOperacaoTrizy.EnviarDadosEmpresaGR = Request.GetBoolParam("EnviarDadosEmpresaGR");
            configuracaoTipoOperacaoTrizy.CNPJEmpresaGR = Request.GetStringParam("CNPJEmpresaGR");
            configuracaoTipoOperacaoTrizy.DescricaoEmpresaGR = Request.GetStringParam("DescricaoEmpresaGR");

            configuracaoTipoOperacaoTrizy.SolicitarNomeRecebedorNaConfirmacaoDeColetaEntrega = Request.GetBoolParam("SolicitarNomeRecebedorNaConfirmacaoDeColetaEntrega");
            configuracaoTipoOperacaoTrizy.SolicitarDocumentoNaConfirmacaoDeColetaEntrega = Request.GetBoolParam("SolicitarDocumentoNaConfirmacaoDeColetaEntrega");

            configuracaoTipoOperacaoTrizy.IdentificarNotaDeMercadoriaENotaDePallet = Request.GetBoolParam("IdentificarNotaDeMercadoriaENotaDePallet");

            configuracaoTipoOperacaoTrizy.NaoPermitirVincularFotosDaGaleriaParaCanhotos = Request.GetBoolParam("NaoPermitirVincularFotosDaGaleriaParaCanhotos");

            configuracaoTipoOperacaoTrizy.ConverterCanhotoParaPretoeBrancoERotacionarAutomaticamente = configuracaoTipoOperacaoTrizy.SolicitarComprovanteColetaEntrega && Request.GetBoolParam("ConverterCanhotoParaPretoeBrancoERotacionarAutomaticamente");
            configuracaoTipoOperacaoTrizy.EnviarMascaraFixaParaoCanhoto = configuracaoTipoOperacaoTrizy.SolicitarComprovanteColetaEntrega && Request.GetBoolParam("EnviarMascaraFixaParaoCanhoto");
            configuracaoTipoOperacaoTrizy.EnviarMascaraDinamicaParaoCanhoto = configuracaoTipoOperacaoTrizy.SolicitarComprovanteColetaEntrega && Request.GetBoolParam("EnviarMascaraDinamicaParaoCanhoto");

            configuracaoTipoOperacaoTrizy.NaoEnviarPolilinha = Request.GetBoolParam("NaoEnviarPolilinha");
            configuracaoTipoOperacaoTrizy.HabilitarDevolucao = Request.GetBoolParam("HabilitarDevolucao");
            configuracaoTipoOperacaoTrizy.HabilitarDevolucaoParcial = Request.GetBoolParam("HabilitarDevolucaoParcial");
            configuracaoTipoOperacaoTrizy.VersaoIntegracao = Request.GetEnumParam<VersaoIntegracaoTrizy>("VersaoIntegracaoTrizy");

            configuracaoTipoOperacaoTrizy.EnviarInformacoesAdicionaisEntrega = Request.GetBoolParam("EnviarInformacoesAdicionaisEntrega");
            configuracaoTipoOperacaoTrizy.InformacoesAdicionaisEntrega = new List<InformacoesAdicionaisEntregaTrizy>();

            List<InformacoesAdicionaisEntregaTrizy> listaInformacoesAdicionaisEntrega = Request.GetNullableListParam<InformacoesAdicionaisEntregaTrizy>("InformacoesAdicionaisEntrega");
            if (configuracaoTipoOperacaoTrizy.EnviarInformacoesAdicionaisEntrega && (listaInformacoesAdicionaisEntrega?.Count ?? 0) > 0)
            {
                foreach (InformacoesAdicionaisEntregaTrizy codigoInformacoesAdicionaisEntregaTrizy in listaInformacoesAdicionaisEntrega)
                    configuracaoTipoOperacaoTrizy.InformacoesAdicionaisEntrega.Add(codigoInformacoesAdicionaisEntregaTrizy);
            }

            configuracaoTipoOperacaoTrizy.EnviarQuantidadeDeCaixasNoLugarDoPesoDosProdutos = Request.GetBoolParam("EnviarQuantidadeDeCaixasNoLugarDoPesoDosProdutos");

            configuracaoTipoOperacaoTrizy.EnviarContatoInformacoesEntrega = Request.GetBoolParam("EnviarContatoInformacoesEntrega");

            List<EnumContatosInformacoesEntregaTrizy> listaContatosInformacoesEntrega = Request.GetNullableListParam<EnumContatosInformacoesEntregaTrizy>("ContatosInformacoesEntrega");

            configuracaoTipoOperacaoTrizy.ContatosInformacoesEntrega = new List<EnumContatosInformacoesEntregaTrizy>();

            if (configuracaoTipoOperacaoTrizy.EnviarContatoInformacoesEntrega && (listaContatosInformacoesEntrega?.Count ?? 0) > 0)
            {
                foreach (EnumContatosInformacoesEntregaTrizy contatoInformacoesEntrega in listaContatosInformacoesEntrega)
                    configuracaoTipoOperacaoTrizy.ContatosInformacoesEntrega.Add(contatoInformacoesEntrega);
            }

            configuracaoTipoOperacaoTrizy.HabilitarEnvioRelatorio = Request.GetBoolParam("HabilitarEnvioRelatorio");
            configuracaoTipoOperacaoTrizy.TituloRelatorioViagem = Request.GetStringParam("TituloRelatorioViagem");
            configuracaoTipoOperacaoTrizy.TituloReciboViagem = Request.GetStringParam("TituloReciboViagem");

            configuracaoTipoOperacaoTrizy.NecessarioFinalizarOrigem = Request.GetBoolParam("NecessarioFinalizarOrigem");

            if (configuracaoTipoOperacaoTrizy.Codigo == 0)
                repConfiguracaoTipoOperacaoTrizy.Inserir(configuracaoTipoOperacaoTrizy);
            else
                repConfiguracaoTipoOperacaoTrizy.Atualizar(configuracaoTipoOperacaoTrizy, Auditado, historico);

            tipoOperacao.ConfiguracaoTrizy = configuracaoTipoOperacaoTrizy;

            await AtualizarInformacoesAdicionaisRelatorioViagemSuperAppAsync(tipoOperacao.ConfiguracaoTrizy, unidadeDeTrabalho, cancellationToken);

            repTipoOperacao.Atualizar(tipoOperacao);
        }

        private void AtualizarNotificacoesAppTrizy(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            dynamic listaNotificacoes = Newtonsoft.Json.JsonConvert.DeserializeObject<List<dynamic>>(Request.GetStringParam("ListaNotificacoesApp"));

            ExcluirNotificacoesRemovidas(tipoOperacao, listaNotificacoes, unidadeDeTrabalho);
            AtualizarNotificacoesAdicionadas(tipoOperacao, listaNotificacoes, unidadeDeTrabalho);
        }

        private async Task AtualizarEventosSuperAppAsync(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, Repositorio.UnitOfWork unidadeDeTrabalho, CancellationToken cancellationToken)
        {
            dynamic listaEventos = Newtonsoft.Json.JsonConvert.DeserializeObject<List<dynamic>>(Request.GetStringParam("ListaEventosSuperApp"));

            await ExcluirEventosRemovidosAsync(tipoOperacao, listaEventos, unidadeDeTrabalho, cancellationToken);
            await AtualizarEventosAdicionadosAsync(tipoOperacao, listaEventos, unidadeDeTrabalho, cancellationToken);
        }

        private async Task AtualizarInformacoesAdicionaisRelatorioViagemSuperAppAsync(Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoTrizy configuracaoTipoOperacao, Repositorio.UnitOfWork unidadeDeTrabalho, CancellationToken cancellationToken)
        {
            dynamic listaInformacoesAdicionais = Newtonsoft.Json.JsonConvert.DeserializeObject<List<dynamic>>(Request.GetStringParam("ListaInformacaoAdicionalRelatorioViagemSuperApp"));

            await ExcluirInformacoesAdicionaisRelatorioViagemSuperAppAsync(configuracaoTipoOperacao, listaInformacoesAdicionais, unidadeDeTrabalho, cancellationToken);
            await AtualizarInformacoesAdicionaisRelatorioViagemSuperAppAdicionadosAsync(configuracaoTipoOperacao, listaInformacoesAdicionais, unidadeDeTrabalho, cancellationToken);
        }

        private void AtualizarConfiguracoesIntegracaoTransSat(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, Dominio.Entidades.Auditoria.HistoricoObjeto historico, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
            Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoIntegracaoTransSat repositorioConfiguracaoTipoOperacaoMontagemCarga = new Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoIntegracaoTransSat(unitOfWork);

            Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoIntegracaoTransSat configuracaoTransSat = tipoOperacao.ConfiguracaoTipoOperacaoIntegracaoTransSat ?? new Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoIntegracaoTransSat();

            if (configuracaoTransSat.Codigo > 0)
                configuracaoTransSat.Initialize();

            configuracaoTransSat.PossuiIntegracaoTransSat = Request.GetBoolParam("PossuiIntegracaoTransSat");

            if (configuracaoTransSat.Codigo == 0)
                repositorioConfiguracaoTipoOperacaoMontagemCarga.Inserir(configuracaoTransSat);
            else
                repositorioConfiguracaoTipoOperacaoMontagemCarga.Atualizar(configuracaoTransSat, Auditado, historico);

            tipoOperacao.ConfiguracaoTipoOperacaoIntegracaoTransSat = configuracaoTransSat;

            repTipoOperacao.Atualizar(tipoOperacao);
        }

        private void AtualizarConfiguracoesTipoPropriedadeVeiculo(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, Dominio.Entidades.Auditoria.HistoricoObjeto historico, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unidadeDeTrabalho);
            Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoTipoPropriedadeVeiculo repConfiguracaoTipoOperacaoTipoPropriedadeVeiculo = new Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoTipoPropriedadeVeiculo(unidadeDeTrabalho);
            Repositorio.Embarcador.Pessoas.TipoTerceiro repTipoTerceiro = new Repositorio.Embarcador.Pessoas.TipoTerceiro(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoTipoPropriedadeVeiculo configuracaoTipoOperacaoTipoPropriedadeVeiculo = tipoOperacao.ConfiguracaoTipoPropriedadeVeiculo ?? new Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoTipoPropriedadeVeiculo();

            if (configuracaoTipoOperacaoTipoPropriedadeVeiculo.Codigo > 0)
                configuracaoTipoOperacaoTipoPropriedadeVeiculo.Initialize();

            configuracaoTipoOperacaoTipoPropriedadeVeiculo.TipoPropriedadeVeiculo = Request.GetEnumParam<TipoPropriedadeVeiculo>("TipoPropriedadeVeiculo");
            configuracaoTipoOperacaoTipoPropriedadeVeiculo.TipoProprietarioVeiculo = Request.GetEnumParam<Dominio.Enumeradores.TipoProprietarioVeiculo>("TipoProprietarioVeiculo");

            if (configuracaoTipoOperacaoTipoPropriedadeVeiculo.TipoPropriedadeVeiculo == TipoPropriedadeVeiculo.Ambos || configuracaoTipoOperacaoTipoPropriedadeVeiculo.TipoPropriedadeVeiculo == TipoPropriedadeVeiculo.Terceiros)
            {
                if (configuracaoTipoOperacaoTipoPropriedadeVeiculo.TiposTerceiros == null)
                    configuracaoTipoOperacaoTipoPropriedadeVeiculo.TiposTerceiros = new List<Dominio.Entidades.Embarcador.Pessoas.TipoTerceiro>();

                var jsonString = Request.GetStringParam("TiposTerceirosPropriedadeVeiculo");

                if (jsonString.StartsWith("\"") && jsonString.EndsWith("\""))
                    jsonString = jsonString[1..^1];
                jsonString = jsonString.Replace("\\", "");
                var tipoTerceiroLista = Newtonsoft.Json.JsonConvert.DeserializeObject<List<dynamic>>(jsonString);

                configuracaoTipoOperacaoTipoPropriedadeVeiculo.TiposTerceiros.Clear();
                foreach (var item in tipoTerceiroLista)
                {
                    var codigoTerceiro = (int)item.TerceiroPropriedadeVeiculo.Codigo;
                    var tipoTerceiro = repTipoTerceiro.BuscarPorCodigo(codigoTerceiro);
                    if (tipoTerceiro != null)
                        configuracaoTipoOperacaoTipoPropriedadeVeiculo.TiposTerceiros.Add(tipoTerceiro);
                }
            }

            if (configuracaoTipoOperacaoTipoPropriedadeVeiculo.Codigo == 0)
                repConfiguracaoTipoOperacaoTipoPropriedadeVeiculo.Inserir(configuracaoTipoOperacaoTipoPropriedadeVeiculo);
            else
                repConfiguracaoTipoOperacaoTipoPropriedadeVeiculo.Atualizar(configuracaoTipoOperacaoTipoPropriedadeVeiculo, Auditado, historico);

            tipoOperacao.ConfiguracaoTipoPropriedadeVeiculo = configuracaoTipoOperacaoTipoPropriedadeVeiculo;
            repTipoOperacao.Atualizar(tipoOperacao);
        }

        private void ExcluirNotificacoesRemovidas(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, dynamic notificacoes, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Pedidos.TipoOperacaoNotificacaoApp repositorioTipoOperacaoNotificacaoApp = new Repositorio.Embarcador.Pedidos.TipoOperacaoNotificacaoApp(unidadeDeTrabalho);

            List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoNotificacaoApp> listaTipoOperacaoNotificacaoApp = repositorioTipoOperacaoNotificacaoApp.BuscarPorTipoOperacao(tipoOperacao.Codigo);

            List<int> listaCodigosAtualizados = new List<int>();

            foreach (dynamic notificacao in notificacoes)
            {
                int? codigoNotificacao = ((string)notificacao.Codigo).ToNullableInt();

                if (codigoNotificacao.HasValue)
                    listaCodigosAtualizados.Add(codigoNotificacao.Value);
            }

            List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoNotificacaoApp> listaIntegracoesRemover = (from notificacao in listaTipoOperacaoNotificacaoApp where !listaCodigosAtualizados.Contains(notificacao.NotificacaoApp.Codigo) select notificacao).ToList();

            foreach (Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoNotificacaoApp notificacao in listaIntegracoesRemover)
                repositorioTipoOperacaoNotificacaoApp.Deletar(notificacao);
        }

        private void AtualizarNotificacoesAdicionadas(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, dynamic notificacoes, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.SuperApp.NotificacaoApp repositorioNotificacaoApp = new Repositorio.Embarcador.SuperApp.NotificacaoApp(unidadeDeTrabalho);
            Repositorio.Embarcador.Pedidos.TipoOperacaoNotificacaoApp repositorioTipoOperacaoNotificacaoApp = new Repositorio.Embarcador.Pedidos.TipoOperacaoNotificacaoApp(unidadeDeTrabalho);

            List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoNotificacaoApp> listaTipoOperacaoNotificacaoApp = repositorioTipoOperacaoNotificacaoApp.BuscarPorTipoOperacao(tipoOperacao.Codigo);
            List<Dominio.Entidades.Embarcador.SuperApp.NotificacaoApp> listaNotificacoesApp = repositorioNotificacaoApp.BuscarPorCodigos(listaTipoOperacaoNotificacaoApp.Select(x => x.NotificacaoApp.Codigo).ToList());

            List<int> listaCodigosAdicionar = new List<int>();

            foreach (dynamic notificacao in notificacoes)
            {
                int? codigoNotificacao = ((string)notificacao.Codigo).ToNullableInt();

                if (!codigoNotificacao.HasValue)
                {
                    Dominio.Entidades.Embarcador.SuperApp.NotificacaoApp notificacaoApp = new Dominio.Entidades.Embarcador.SuperApp.NotificacaoApp()
                    {
                        Tipo = ((string)notificacao.Tipo).ToEnum<TipoNotificacaoApp>(),
                        Titulo = notificacao.Titulo,
                        Mensagem = notificacao.Mensagem
                    };
                    repositorioNotificacaoApp.Inserir(notificacaoApp);

                    Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoNotificacaoApp tipoOperacaoNotificacaoApp = new Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoNotificacaoApp()
                    {
                        TipoOperacao = tipoOperacao,
                        NotificacaoApp = notificacaoApp
                    };
                    repositorioTipoOperacaoNotificacaoApp.Inserir(tipoOperacaoNotificacaoApp);
                }
                else
                {
                    Dominio.Entidades.Embarcador.SuperApp.NotificacaoApp notificacaoApp = repositorioNotificacaoApp.BuscarPorCodigo(codigoNotificacao.Value, false);
                    notificacaoApp.Tipo = ((string)notificacao.Tipo).ToEnum<TipoNotificacaoApp>();
                    notificacaoApp.Titulo = notificacao.Titulo;
                    notificacaoApp.Mensagem = notificacao.Mensagem;
                    repositorioNotificacaoApp.Atualizar(notificacaoApp);
                }
            }
        }

        private async Task ExcluirEventosRemovidosAsync(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, dynamic eventos, Repositorio.UnitOfWork unidadeDeTrabalho, CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.Pedidos.TipoOperacaoEventoSuperApp repositorioTipoOperacaoEventoSuperApp = new Repositorio.Embarcador.Pedidos.TipoOperacaoEventoSuperApp(unidadeDeTrabalho, cancellationToken);

            List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoEventoSuperApp> listaTipoOperacaoEventoSuperApp = await repositorioTipoOperacaoEventoSuperApp.BuscarPorTipoOperacaoAsync(tipoOperacao.Codigo);

            List<int> listaCodigosAtualizados = new List<int>();

            foreach (dynamic evento in eventos)
            {
                int? codigoEvento = ((string)evento.Codigo).ToNullableInt();

                if (codigoEvento.HasValue)
                    listaCodigosAtualizados.Add(codigoEvento.Value);
            }

            List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoEventoSuperApp> listaIntegracoesRemover = (from evento in listaTipoOperacaoEventoSuperApp where !listaCodigosAtualizados.Contains(evento.EventoSuperApp.Codigo) select evento).ToList();

            foreach (Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoEventoSuperApp evento in listaIntegracoesRemover)
                await repositorioTipoOperacaoEventoSuperApp.DeletarAsync(evento);
        }

        private async Task AtualizarEventosAdicionadosAsync(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, dynamic eventos, Repositorio.UnitOfWork unidadeDeTrabalho, CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.SuperApp.EventoSuperApp repositorioEventoSuperApp = new Repositorio.Embarcador.SuperApp.EventoSuperApp(unidadeDeTrabalho, cancellationToken);
            Repositorio.Embarcador.Pedidos.TipoOperacaoEventoSuperApp repositorioTipoOperacaoEventoSuperApp = new Repositorio.Embarcador.Pedidos.TipoOperacaoEventoSuperApp(unidadeDeTrabalho, cancellationToken);

            List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoEventoSuperApp> listaTipoOperacaoEventoSuperApp = await repositorioTipoOperacaoEventoSuperApp.BuscarPorTipoOperacaoAsync(tipoOperacao.Codigo);
            List<Dominio.Entidades.Embarcador.SuperApp.EventoSuperApp> listaEventoSuperApp = await repositorioEventoSuperApp.BuscarPorCodigosAsync(listaTipoOperacaoEventoSuperApp.Select(x => x.EventoSuperApp.Codigo).ToList());

            List<int> listaCodigosAdicionar = new List<int>();

            foreach (dynamic evento in eventos)
            {
                int? codigoEvento = ((string)evento.Codigo).ToNullableInt();

                if (!codigoEvento.HasValue)
                {
                    Dominio.Entidades.Embarcador.SuperApp.EventoSuperApp eventoSuperApp = new Dominio.Entidades.Embarcador.SuperApp.EventoSuperApp()
                    {
                        Tipo = ((string)evento.Tipo).ToEnum<TipoEventoSuperApp>(),
                        Titulo = evento.Titulo,
                        Obrigatorio = evento.Obrigatorio,
                        Ordem = evento.Ordem,
                        TipoEventoCustomizado = ((string)evento.TipoEventoCustomizado).ToEnum<TipoCustomEventAppTrizy>(),
                        TipoParada = ((string)evento.TipoParada).ToEnum<TipoParadaEventoSuperApp>(),
                        ChecklistSuperApp = evento.ChecklistSuperApp.Codigo > 0 ? new()
                        {
                            Codigo = evento.ChecklistSuperApp.Codigo
                        } : null,
                    };
                    await repositorioEventoSuperApp.InserirAsync(eventoSuperApp);

                    Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoEventoSuperApp tipoOperacaoEventoSuperApp = new Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoEventoSuperApp()
                    {
                        TipoOperacao = tipoOperacao,
                        EventoSuperApp = eventoSuperApp
                    };
                    await repositorioTipoOperacaoEventoSuperApp.InserirAsync(tipoOperacaoEventoSuperApp);
                }
                else
                {
                    Dominio.Entidades.Embarcador.SuperApp.EventoSuperApp eventoSuperApp = await repositorioEventoSuperApp.BuscarPorCodigoAsync(codigoEvento.Value);
                    eventoSuperApp.Tipo = ((string)evento.Tipo).ToEnum<TipoEventoSuperApp>();
                    eventoSuperApp.Titulo = evento.Titulo;
                    eventoSuperApp.Obrigatorio = evento.Obrigatorio;
                    eventoSuperApp.Ordem = evento.Ordem;
                    eventoSuperApp.TipoEventoCustomizado = ((string)evento.TipoEventoCustomizado).ToEnum<TipoCustomEventAppTrizy>();
                    eventoSuperApp.TipoParada = ((string)evento.TipoParada).ToEnum<TipoParadaEventoSuperApp>();
                    eventoSuperApp.ChecklistSuperApp = evento.ChecklistSuperApp.Codigo > 0 ? new()
                    {
                        Codigo = evento.ChecklistSuperApp.Codigo
                    } : null;
                    await repositorioEventoSuperApp.AtualizarAsync(eventoSuperApp);
                }
            }
        }

        private async Task ExcluirInformacoesAdicionaisRelatorioViagemSuperAppAsync(Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoTrizy configuracaoTipoOperacaoTrizy, dynamic listaInformacoesAdicionais, Repositorio.UnitOfWork unidadeDeTrabalho, CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoTrizyInformacaoAdicionalRelatorio repositorioConfiguracaoTipoOperacaoTrizyInformacaoAdicionalRelatorio = new Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoTrizyInformacaoAdicionalRelatorio(unidadeDeTrabalho, cancellationToken);

            List<Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoTrizyInformacaoAdicionalRelatorio> listaConfiguracaoTipoOperacaoTrizyInformacaoAdicionalRelatorio = await repositorioConfiguracaoTipoOperacaoTrizyInformacaoAdicionalRelatorio.BuscarPorConfiguracaoTipoOperacaoTrizyAsync(configuracaoTipoOperacaoTrizy.Codigo);

            List<int> listaCodigosAtualizados = new List<int>();

            foreach (dynamic informacaoAdicional in listaInformacoesAdicionais)
            {
                int? codigoInformacaoAdicional = ((string)informacaoAdicional.Codigo).ToNullableInt();

                if (codigoInformacaoAdicional.HasValue)
                    listaCodigosAtualizados.Add(codigoInformacaoAdicional.Value);
            }

            List<Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoTrizyInformacaoAdicionalRelatorio> listaInformacoesAdicionaisRemover = (from informacaoAdicional in listaConfiguracaoTipoOperacaoTrizyInformacaoAdicionalRelatorio where !listaCodigosAtualizados.Contains(informacaoAdicional.Codigo) select informacaoAdicional).ToList();

            foreach (Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoTrizyInformacaoAdicionalRelatorio informacaoAdicional in listaInformacoesAdicionaisRemover)
                await repositorioConfiguracaoTipoOperacaoTrizyInformacaoAdicionalRelatorio.DeletarAsync(informacaoAdicional);
        }

        private async Task AtualizarInformacoesAdicionaisRelatorioViagemSuperAppAdicionadosAsync(Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoTrizy configuracaoTipoOperacaoTrizy, dynamic listaInformacoesAdicionais, Repositorio.UnitOfWork unidadeDeTrabalho, CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoTrizyInformacaoAdicionalRelatorio repositorioConfiguracaoTipoOperacaoTrizyInformacaoAdicionalRelatorio = new Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoTrizyInformacaoAdicionalRelatorio(unidadeDeTrabalho, cancellationToken);

            List<Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoTrizyInformacaoAdicionalRelatorio> listaConfiguracaoTipoOperacaoTrizyInformacaoAdicionalRelatorio = await repositorioConfiguracaoTipoOperacaoTrizyInformacaoAdicionalRelatorio.BuscarPorConfiguracaoTipoOperacaoTrizyAsync(configuracaoTipoOperacaoTrizy.Codigo);

            List<int> listaCodigosAdicionar = new List<int>();

            foreach (dynamic informacaoAdicional in listaInformacoesAdicionais)
            {
                int? codigoInformacaoAdicional = ((string)informacaoAdicional.Codigo).ToNullableInt();

                if (!codigoInformacaoAdicional.HasValue)
                {
                    Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoTrizyInformacaoAdicionalRelatorio configuracaoTipoOperacaoTrizyInformacaoAdicionalRelatorio = new Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoTrizyInformacaoAdicionalRelatorio()
                    {
                        Rotulo = informacaoAdicional.Rotulo,
                        Descricao = informacaoAdicional.Descricao,
                        ConfiguracaoTipoOperacaoTrizy = configuracaoTipoOperacaoTrizy
                    };
                    await repositorioConfiguracaoTipoOperacaoTrizyInformacaoAdicionalRelatorio.InserirAsync(configuracaoTipoOperacaoTrizyInformacaoAdicionalRelatorio);
                }
                else
                {
                    Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoTrizyInformacaoAdicionalRelatorio configuracaoTipoOperacaoTrizyInformacaoAdicionalRelatorio = await repositorioConfiguracaoTipoOperacaoTrizyInformacaoAdicionalRelatorio.BuscarPorCodigoAsync(codigoInformacaoAdicional ?? 0, false);
                    configuracaoTipoOperacaoTrizyInformacaoAdicionalRelatorio.Rotulo = informacaoAdicional.Rotulo;
                    configuracaoTipoOperacaoTrizyInformacaoAdicionalRelatorio.Descricao = informacaoAdicional.Descricao;

                    await repositorioConfiguracaoTipoOperacaoTrizyInformacaoAdicionalRelatorio.AtualizarAsync(configuracaoTipoOperacaoTrizyInformacaoAdicionalRelatorio);
                }
            }
        }

        private void AtualizarConfiguracoesEMP(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, Dominio.Entidades.Auditoria.HistoricoObjeto historico, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unidadeDeTrabalho);
            Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoEMP repConfiguracaoTipoOperacaoEMP = new Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoEMP(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoEMP configuracaoTipoOperacaoEMP = tipoOperacao.ConfiguracaoEMP ?? new Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoEMP();

            if (configuracaoTipoOperacaoEMP.Codigo > 0)
                configuracaoTipoOperacaoEMP.Initialize();

            configuracaoTipoOperacaoEMP.AtivarIntegracaoComSIL = Request.GetBoolParam("AtivarIntegracaoComSIL");

            if (configuracaoTipoOperacaoEMP.Codigo == 0)
                repConfiguracaoTipoOperacaoEMP.Inserir(configuracaoTipoOperacaoEMP);
            else
                repConfiguracaoTipoOperacaoEMP.Atualizar(configuracaoTipoOperacaoEMP, Auditado, historico);

            tipoOperacao.ConfiguracaoEMP = configuracaoTipoOperacaoEMP;

            repTipoOperacao.Atualizar(tipoOperacao);
        }

        private void AtualizarConfiguracoesTransportador(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, Dominio.Entidades.Auditoria.HistoricoObjeto historico, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unidadeDeTrabalho);
            Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoTransportador repConfiguracaoTipoOperacaoTransportador = new Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoTransportador(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoTransportador configuracaoTipoOperacaoTransportador = tipoOperacao.ConfiguracaoTransportador ?? new Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoTransportador();

            if (configuracaoTipoOperacaoTransportador.Codigo > 0)
                configuracaoTipoOperacaoTransportador.Initialize();

            configuracaoTipoOperacaoTransportador.PermitirTransportadorSolicitarNotasFiscais = Request.GetBoolParam("PermitirTransportadorSolicitarNotasFiscais");
            configuracaoTipoOperacaoTransportador.PermitirEnvioImagemMultiplosCanhotos = Request.GetBoolParam("PermitirEnvioImagemMultiplosCanhotos");
            configuracaoTipoOperacaoTransportador.PermitirTransportadorAjusteCargaSegundoTrecho = Request.GetBoolParam("PermitirTransportadorAjusteCargaSegundoTrecho");
            configuracaoTipoOperacaoTransportador.TipoOperacaoModalFerroviario = repTipoOperacao.BuscarPorCodigo(Request.GetIntParam("TipoOperacaoPadraoFerroviario"));
            configuracaoTipoOperacaoTransportador.PermitirRetornarEtapa = Request.GetBoolParam("PermitirRetornarEtapa");
            configuracaoTipoOperacaoTransportador.AlertarTransportadorNaoIMOCargasPerigosas = Request.GetBoolParam("AlertarTransportadorNaoIMOCargasPerigosas");
            configuracaoTipoOperacaoTransportador.BloquearTransportadorNaoIMOAptoCargasPerigosas = Request.GetBoolParam("BloquearTransportadorNaoIMOAptoCargasPerigosas");
            configuracaoTipoOperacaoTransportador.BloquearVeiculoSemEspelhamento = Request.GetBoolParam("BloquearVeiculoSemEspelhamento");
            configuracaoTipoOperacaoTransportador.BloquearVeiculoSemEspelhamentoJanela = Request.GetBoolParam("BloquearVeiculoSemEspelhamentoJanela");

            if (configuracaoTipoOperacaoTransportador.Codigo == 0)
                repConfiguracaoTipoOperacaoTransportador.Inserir(configuracaoTipoOperacaoTransportador);
            else
                repConfiguracaoTipoOperacaoTransportador.Atualizar(configuracaoTipoOperacaoTransportador, Auditado, historico);

            tipoOperacao.ConfiguracaoTransportador = configuracaoTipoOperacaoTransportador;

            repTipoOperacao.Atualizar(tipoOperacao);
        }

        private void AtualizarConfiguracoesDocumentoEmissao(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, Dominio.Entidades.Auditoria.HistoricoObjeto historico, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unidadeDeTrabalho);
            Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoDocumentoEmissao repConfiguracaoTipoOperacaoDocumentoEmissao = new Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoDocumentoEmissao(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoDocumentoEmissao configuracaoTipoOperacaoDocumentoEmissao = tipoOperacao.ConfiguracaoDocumentoEmissao ?? new Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoDocumentoEmissao();

            if (configuracaoTipoOperacaoDocumentoEmissao.Codigo > 0)
                configuracaoTipoOperacaoDocumentoEmissao.Initialize();

            configuracaoTipoOperacaoDocumentoEmissao.NaoAlterarTomadorCargaPedidoImportacaoCTeSubcontratacao = Request.GetBoolParam("NaoAlterarTomadorCargaPedidoImportacaoCTeSubcontratacao");
            configuracaoTipoOperacaoDocumentoEmissao.ImportarCTeSempreComoSubcontratacao = Request.GetBoolParam("ImportarCTeSempreComoSubcontratacao");
            configuracaoTipoOperacaoDocumentoEmissao.PossuiNotaOrdemVenda = Request.GetBoolParam("PossuiNotaOrdemVenda");
            configuracaoTipoOperacaoDocumentoEmissao.UtilizaNotaVendaObjetoCTE = Request.GetBoolParam("UtilizaNotaVendaObjetoCTE");
            configuracaoTipoOperacaoDocumentoEmissao.MinutosAvancarParaEmissaoseInformadosDadosTransporte = Request.GetIntParam("MinutosAvancarParaEmissaoseInformadosDadosTransporte");
            configuracaoTipoOperacaoDocumentoEmissao.NaoUtilizaNotaVendaObjetoCTE = Request.GetBoolParam("NaoUtilizaNotaVendaObjetoCTE");
            configuracaoTipoOperacaoDocumentoEmissao.EmitirCTENotaRemessa = Request.GetBoolParam("EmitirCTENotaRemessa");
            configuracaoTipoOperacaoDocumentoEmissao.DesconsiderarNotaPalletEmissaoCTE = Request.GetBoolParam("DesconsiderarNotaPalletEmissaoCTE");

            if (configuracaoTipoOperacaoDocumentoEmissao.Codigo == 0)
                repConfiguracaoTipoOperacaoDocumentoEmissao.Inserir(configuracaoTipoOperacaoDocumentoEmissao);
            else
                repConfiguracaoTipoOperacaoDocumentoEmissao.Atualizar(configuracaoTipoOperacaoDocumentoEmissao, Auditado, historico);

            tipoOperacao.ConfiguracaoDocumentoEmissao = configuracaoTipoOperacaoDocumentoEmissao;

            repTipoOperacao.Atualizar(tipoOperacao);
        }

        private void AtualizarConfiguracoesCanhoto(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, Dominio.Entidades.Auditoria.HistoricoObjeto historico, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.TipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
            Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoCanhoto repositorioConfiguracaoTipoOperacaoCanhoto = new Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoCanhoto(unitOfWork);
            Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoCanhoto configuracaoTipoOperacaoCanhoto = tipoOperacao.ConfiguracaoCanhoto ?? new Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoCanhoto();

            if (configuracaoTipoOperacaoCanhoto.Codigo > 0)
                configuracaoTipoOperacaoCanhoto.Initialize();

            configuracaoTipoOperacaoCanhoto.NotificarCanhotosPendentes = Request.GetBoolParam("NotificarCanhotosPendentes");
            configuracaoTipoOperacaoCanhoto.NotificarCanhotosRejeitados = Request.GetBoolParam("NotificarCanhotosRejeitados");
            configuracaoTipoOperacaoCanhoto.NotificarCanhotosPendentesDiariamente = Request.GetBoolParam("NotificarCanhotosPendentesDiariamente");
            configuracaoTipoOperacaoCanhoto.NotificarCanhotosRejeitadosDiariamente = Request.GetBoolParam("NotificarCanhotosRejeitadosDiariamente");
            configuracaoTipoOperacaoCanhoto.PrazoAposDataEmissaoCanhoto = Request.GetIntParam("PrazoAposDataEmissaoCanhoto");
            configuracaoTipoOperacaoCanhoto.DiaSemanaNotificarCanhotosPendentes = Request.GetEnumParam<DiaSemana>("DiaSemanaNotificarCanhotosPendentes");
            configuracaoTipoOperacaoCanhoto.NaoPermiteUploadDeCanhotosComCTeNaoAutorizado = Request.GetBoolParam("NaoPermiteUploadDeCanhotosComCTeNaoAutorizado");
            configuracaoTipoOperacaoCanhoto.NaoGerarCanhotoAvulsoEmCargasComAoMenosUmRecebedor = Request.GetBoolParam("NaoGerarCanhotoAvulsoEmCargasComAoMenosUmRecebedor");

            if (configuracaoTipoOperacaoCanhoto.Codigo == 0)
                repositorioConfiguracaoTipoOperacaoCanhoto.Inserir(configuracaoTipoOperacaoCanhoto);
            else
                repositorioConfiguracaoTipoOperacaoCanhoto.Atualizar(configuracaoTipoOperacaoCanhoto, Auditado, historico);

            tipoOperacao.ConfiguracaoCanhoto = configuracaoTipoOperacaoCanhoto;

            repositorioTipoOperacao.Atualizar(tipoOperacao);
        }

        private void AtualizarConfiguracoesFreeTime(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, Dominio.Entidades.Auditoria.HistoricoObjeto historico, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unidadeDeTrabalho);
            Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoFreeTime repConfiguracaoTipoOperacaoFreeTime = new Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoFreeTime(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoFreeTime configuracaoTipoOperacaoFreeTime = tipoOperacao.ConfiguracaoFreeTime ?? new Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoFreeTime();

            if (configuracaoTipoOperacaoFreeTime.Codigo > 0)
                configuracaoTipoOperacaoFreeTime.Initialize();

            configuracaoTipoOperacaoFreeTime.TipoFreeTime = Request.GetEnumParam<TipoFreeTime>("TipoFreeTime");
            configuracaoTipoOperacaoFreeTime.TempoColetas = Request.GetIntParam("TempoColetas");
            configuracaoTipoOperacaoFreeTime.TempoFronteiras = Request.GetIntParam("TempoFronteiras");
            configuracaoTipoOperacaoFreeTime.TempoEntregas = Request.GetIntParam("TempoEntregas");
            configuracaoTipoOperacaoFreeTime.TempoTotalViagem = Request.GetIntParam("TempoTotalViagem");
            configuracaoTipoOperacaoFreeTime.ConsiderarDatasDePrevisaoDoPedidoParaEstadia = Request.GetBoolParam("ConsiderarDatasDePrevisaoDoPedidoParaEstadia");

            if (configuracaoTipoOperacaoFreeTime.Codigo == 0)
                repConfiguracaoTipoOperacaoFreeTime.Inserir(configuracaoTipoOperacaoFreeTime);
            else
                repConfiguracaoTipoOperacaoFreeTime.Atualizar(configuracaoTipoOperacaoFreeTime, Auditado, historico);

            tipoOperacao.ConfiguracaoFreeTime = configuracaoTipoOperacaoFreeTime;

            repTipoOperacao.Atualizar(tipoOperacao);
        }

        private void AtualizarConfiguracoesTerceiro(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, Dominio.Entidades.Auditoria.HistoricoObjeto historico, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unidadeDeTrabalho);
            Repositorio.Embarcador.Fatura.Justificativa repJustificativa = new Repositorio.Embarcador.Fatura.Justificativa(unidadeDeTrabalho);
            Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoTerceiro repConfiguracaoTipoOperacaoTerceiro = new Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoTerceiro(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoTerceiro configuracaoTipoOperacaoTerceiro = tipoOperacao.ConfiguracaoTerceiro ?? new Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoTerceiro();

            if (configuracaoTipoOperacaoTerceiro.Codigo > 0)
                configuracaoTipoOperacaoTerceiro.Initialize();

            int.TryParse(Request.Params("JustificativaAdicionarValorContratoFreteComoAcrescimoDescontoProximoContrato"), out int justificativa);

            configuracaoTipoOperacaoTerceiro.NaoSomarValorPedagioContratoFrete = Request.GetNullableBoolParam("NaoSomarValorPedagioContratoFrete");
            configuracaoTipoOperacaoTerceiro.NaoSubtrairValePedagioDoContrato = Request.GetNullableBoolParam("NaoSubtrairValePedagioDoContrato");
            configuracaoTipoOperacaoTerceiro.AdicionarValorContratoFreteComoAcrescimoDescontoProximoContrato = Request.GetNullableBoolParam("AdicionarValorContratoFreteComoAcrescimoDescontoProximoContrato");
            configuracaoTipoOperacaoTerceiro.JustificativaAdicionarValorContratoFreteComoAcrescimoDescontoProximoContrato = justificativa > 0 ? repJustificativa.BuscarPorCodigo(justificativa) : null;
            configuracaoTipoOperacaoTerceiro.NaoUtilizarOperadoraConfiguradaNoTransportadorTerceiro = Request.GetNullableBoolParam("NaoUtilizarOperadoraConfiguradaNoTransportadorTerceiro") ?? false;

            if (configuracaoTipoOperacaoTerceiro.Codigo == 0)
                repConfiguracaoTipoOperacaoTerceiro.Inserir(configuracaoTipoOperacaoTerceiro);
            else
                repConfiguracaoTipoOperacaoTerceiro.Atualizar(configuracaoTipoOperacaoTerceiro, Auditado, historico);

            tipoOperacao.ConfiguracaoTerceiro = configuracaoTipoOperacaoTerceiro;

            repTipoOperacao.Atualizar(tipoOperacao);
        }

        private void PreencherConfiguracaoLicenca(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao)
        {
            tipoOperacao.ValidarLicencaVeiculo = Request.GetBoolParam("ValidarLicencaVeiculo");
            tipoOperacao.ValidarLicencaVeiculoPorCarga = Request.GetBoolParam("ValidarLicencaVeiculoPorCarga");
            tipoOperacao.PermitirAvancarEtapaComLicencaInvalida = Request.GetBoolParam("PermitirAvancarEtapaComLicencaInvalida");
        }

        private void PreencherConfiguracaoAgendamentoColeta(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoAgendamentoColetaEntrega repositorioTipoOperacaoConfiguracaoAgendamentoColetaEntrega = new Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoAgendamentoColetaEntrega(unitOfWork);

            tipoOperacao.EmailAgendamentoColeta = Request.GetStringParam("EmailAgendamentoColeta");
            tipoOperacao.BloquearMontagemCargaSemNotaFiscal = Request.GetBoolParam("BloquearMontagemCargaSemNotaFiscal");
            tipoOperacao.PermiteInformarRecebedorAgendamento = Request.GetBoolParam("PermiteInformarRecebedorAgendamento");
            bool.TryParse(Request.Params("EnviarEmailAoClienteComLinkDeAgendamentoQuandoGerarACarga"), out bool enviarEmailAoClienteComLinkDeAgendamento);

            if (tipoOperacao.ConfiguracaoAgendamentoColetaEntrega == null)
                tipoOperacao.ConfiguracaoAgendamentoColetaEntrega = new Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoAgendamentoColetaEntrega();

            tipoOperacao.ConfiguracaoAgendamentoColetaEntrega.EnviarEmailAoCLienteComLinkDeAgendamentoQuandoGerarCarga = enviarEmailAoClienteComLinkDeAgendamento;
            tipoOperacao.ConfiguracaoAgendamentoColetaEntrega.UtilizarDataSaidaGuaritaComoTerminoCarregamento = Request.GetBoolParam("UtilizarDataSaidaGuaritaComoTerminoCarregamento");
            tipoOperacao.ConfiguracaoAgendamentoColetaEntrega.ObrigarInformarCTePortalFornecedor = Request.GetBoolParam("ObrigarInformarCTePortalFornecedor");
            tipoOperacao.ConfiguracaoAgendamentoColetaEntrega.ExigirNumeroIsisReturnParaAgendarEntrega = Request.GetBoolParam("ExigirNumeroIsisReturnParaAgendarEntrega");
            tipoOperacao.ConfiguracaoAgendamentoColetaEntrega.RemoverEtapaAgendamentoDoAgendamentoColeta = Request.GetBoolParam("RemoverEtapaAgendamentoDoAgendamentoColeta");
            tipoOperacao.ConfiguracaoAgendamentoColetaEntrega.NaoObrigarInformarModeloVeicularAgendamento = Request.GetBoolParam("NaoObrigarInformarModeloVeicularAgendamento");
            tipoOperacao.ConfiguracaoAgendamentoColetaEntrega.NaoObrigarInformarTransportadorAgendamento = Request.GetBoolParam("NaoObrigarInformarTransportadorAgendamento");
            tipoOperacao.ConfiguracaoAgendamentoColetaEntrega.AvancarEtapaNFeCargaAoConfirmarAgendamentoRetira = Request.GetBoolParam("AvancarEtapaNFeCargaAoConfirmarAgendamentoRetira");
            tipoOperacao.ConfiguracaoAgendamentoColetaEntrega.ExigirQueCDDestinoSejaInformadoAgendamento = Request.GetBoolParam("ExigirQueCDDestinoSejaInformadoAgendamento");
            tipoOperacao.ConfiguracaoAgendamentoColetaEntrega.ConsiderarDataEntregaComoInicioDoFluxoPatio = Request.GetBoolParam("ConsiderarDataEntregaComoInicioDoFluxoPatio");

            if (tipoOperacao.ConfiguracaoAgendamentoColetaEntrega.Codigo > 0)
                repositorioTipoOperacaoConfiguracaoAgendamentoColetaEntrega.Atualizar(tipoOperacao.ConfiguracaoAgendamentoColetaEntrega);
            else
                repositorioTipoOperacaoConfiguracaoAgendamentoColetaEntrega.Inserir(tipoOperacao.ConfiguracaoAgendamentoColetaEntrega);
        }

        private void AtualizarConfiguracoesControleEntrega(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, Dominio.Entidades.Auditoria.HistoricoObjeto historico, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
            Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoControleEntrega repositorioConfiguracaoTipoOperacaoControleEntrega = new Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoControleEntrega(unitOfWork);
            Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacaoControleEntregaSetor repTipoOperacaoControleEntregaSetor = new Repositorio.Embarcador.Pedidos.TipoOperacaoControleEntregaSetor(unitOfWork);

            Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoControleEntrega configuracaoTipoOperacaoControleEntrega = tipoOperacao.ConfiguracaoControleEntrega ?? new Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoControleEntrega();

            if (configuracaoTipoOperacaoControleEntrega.Codigo > 0)
                configuracaoTipoOperacaoControleEntrega.Initialize();

            double codigoLocalDeParqueamentoCliente = Request.GetDoubleParam("LocalDeParqueamentoCliente");
            Dominio.Entidades.Cliente localDeParqueamentoCliente = codigoLocalDeParqueamentoCliente > 0 ? repositorioCliente.BuscarPorCPFCNPJ(codigoLocalDeParqueamentoCliente) : null;
            configuracaoTipoOperacaoControleEntrega.LocalDeParqueamentoCliente = localDeParqueamentoCliente;

            configuracaoTipoOperacaoControleEntrega.ExigirConferenciaProdutosAoConfirmarEntrega = Request.GetBoolParam("ExigirConferenciaProdutosAoConfirmarEntrega");
            configuracaoTipoOperacaoControleEntrega.EnviarBoletimViagemAoFinalizarViagem = Request.GetBoolParam("EnviarBoletimViagemAoFinalizarViagem");
            configuracaoTipoOperacaoControleEntrega.EnviarBoletimViagemAoFinalizarViagemParaRemetente = Request.GetBoolParam("EnviarBoletimViagemAoFinalizarViagemParaRemetente");
            configuracaoTipoOperacaoControleEntrega.EnviarBoletimViagemAoFinalizarViagemParaTransportador = Request.GetBoolParam("EnviarBoletimViagemAoFinalizarViagemParaTransportador");
            configuracaoTipoOperacaoControleEntrega.AlterarSituacaoEntregaNFeParaDevolvidaAoConfirmarEntrega = Request.GetBoolParam("AlterarSituacaoEntregaNFeParaDevolvidaAoConfirmarEntrega");
            configuracaoTipoOperacaoControleEntrega.GerarEventoColetaEntregaUnicoParaTodosTrechos = Request.GetBoolParam("GerarEventoColetaEntregaUnicoParaTodosTrechos");
            configuracaoTipoOperacaoControleEntrega.PermitirInformarNotasFiscaisNoControleEntrega = Request.GetBoolParam("PermitirInformarNotasFiscaisNoControleEntrega");
            configuracaoTipoOperacaoControleEntrega.ExigirInformarNumeroPacotesNaColetaTrizy = Request.GetBoolParam("ExigirInformarNumeroPacotesNaColetaTrizy");
            configuracaoTipoOperacaoControleEntrega.FinalizarControleEntregaAoFinalizarMonitoramentoCarga = Request.GetBoolParam("FinalizarControleEntregaAoFinalizarMonitoramentoCarga");
            configuracaoTipoOperacaoControleEntrega.RecriarControleDeEntregasAoConfirmarEnvioDocumentos = Request.GetBoolParam("RecriarControleDeEntregasAoConfirmarEnvioDocumentos");
            configuracaoTipoOperacaoControleEntrega.ConfirmarColetasQuandoTodasAsEntregasDaCargaForemConcluidas = Request.GetBoolParam("ConfirmarColetasQuandoTodasAsEntregasDaCargaForemConcluidas");
            configuracaoTipoOperacaoControleEntrega.DisponibilizarFotoDaColetaNaTelaDeAprovacaoDeNotaFiscal = Request.GetBoolParam("DisponibilizarFotoDaColetaNaTelaDeAprovacaoDeNotaFiscal");
            configuracaoTipoOperacaoControleEntrega.NaoFinalizarEntregasPorTrackingMonitoramento = Request.GetBoolParam("NaoFinalizarEntregasPorTrackingMonitoramento");
            configuracaoTipoOperacaoControleEntrega.SobrescreverDataEntradaSaidaAlvo = Request.GetBoolParam("SobrescreverDataEntradaSaidaAlvo");
            configuracaoTipoOperacaoControleEntrega.NaoFinalizarColetasPorTrackingMonitoramento = Request.GetBoolParam("NaoFinalizarColetasPorTrackingMonitoramento");
            configuracaoTipoOperacaoControleEntrega.GerarControleEntregaSemRota = Request.GetBoolParam("GerarControleEntregaSemRota");
            configuracaoTipoOperacaoControleEntrega.OrdenarColetasPorDataCarregamento = Request.GetBoolParam("OrdenarColetasPorDataCarregamento");
            configuracaoTipoOperacaoControleEntrega.BloquearInicioeFimDeViagemPeloTransportadorEmCargaNaoEmitida = Request.GetBoolParam("BloquearInicioeFimDeViagemPeloTransportadorEmCargaNaoEmitida");

            configuracaoTipoOperacaoControleEntrega.ConsiderarConfiguracoesPrevisaoEntregaTipoOperacao = Request.GetBoolParam("ConsiderarConfiguracoesPrevisaoEntregaTipoOperacao");
            configuracaoTipoOperacaoControleEntrega.DesconsiderarSabadosCalculoPrevisao = Request.GetBoolParam("DesconsiderarSabadosCalculoPrevisao");
            configuracaoTipoOperacaoControleEntrega.DesconsiderarFeriadosCalculoPrevisao = Request.GetBoolParam("DesconsiderarFeriadosCalculoPrevisao");
            configuracaoTipoOperacaoControleEntrega.DesconsiderarDomingosCalculoPrevisao = Request.GetBoolParam("DesconsiderarDomingosCalculoPrevisao");
            configuracaoTipoOperacaoControleEntrega.ConsiderarJornadaMotorista = Request.GetBoolParam("ConsiderarJornadaMotorista");
            configuracaoTipoOperacaoControleEntrega.HorarioInicialAlmoco = Request.GetTimeParam("HorarioInicialAlmoco");
            configuracaoTipoOperacaoControleEntrega.MinutosIntervalo = Request.GetIntParam("MinutosIntervalo");
            configuracaoTipoOperacaoControleEntrega.DesconsiderarHorariosParaPrazoEntrega = Request.GetBoolParam("DesconsiderarHorariosParaPrazoEntrega");
            configuracaoTipoOperacaoControleEntrega.DataRealizacaoDoEvento = Request.GetNullableEnumParam<TipoDataCalculoParadaNoPrazo>("DataRealizacaoDoEvento");
            configuracaoTipoOperacaoControleEntrega.DataPrevistaDoEvento = Request.GetNullableEnumParam<TipoDataCalculoParadaNoPrazo>("DataPrevistaDoEvento");

            List<int> codigosSetores = Request.GetListParam<int>("Setores");
            repTipoOperacaoControleEntregaSetor.DeletarPorConfiguracaoTipoOperacaoControleEntrega(configuracaoTipoOperacaoControleEntrega.Codigo);
            foreach (int codigoSetor in codigosSetores)
            {
                Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoControleEntregaSetor controleEntregaSetor = new Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoControleEntregaSetor()
                {
                    Setor = new Dominio.Entidades.Setor() { Codigo = codigoSetor },
                    ConfiguracaoTipoOperacaoControleEntrega = configuracaoTipoOperacaoControleEntrega
                };
                repTipoOperacaoControleEntregaSetor.Inserir(controleEntregaSetor);
            }

            if (configuracaoTipoOperacaoControleEntrega.Codigo == 0)
                repositorioConfiguracaoTipoOperacaoControleEntrega.Inserir(configuracaoTipoOperacaoControleEntrega);
            else
                repositorioConfiguracaoTipoOperacaoControleEntrega.Atualizar(configuracaoTipoOperacaoControleEntrega, Auditado, historico);

            tipoOperacao.ConfiguracaoControleEntrega = configuracaoTipoOperacaoControleEntrega;

            repTipoOperacao.Atualizar(tipoOperacao);
        }

        private void AtualizarConfiguracoesLicenca(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, Dominio.Entidades.Auditoria.HistoricoObjeto historico, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
            Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoLicenca repositorioConfiguracaoTipoOperacaoLicenca = new Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoLicenca(unitOfWork);

            Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoLicenca configuracaoTipoOperacaoLicenca = tipoOperacao.ConfiguracaoTipoOperacaoLicenca ?? new Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoLicenca();

            if (configuracaoTipoOperacaoLicenca.Codigo > 0)
                configuracaoTipoOperacaoLicenca.Initialize();

            configuracaoTipoOperacaoLicenca.ValidarLicencaMotorista = Request.GetBoolParam("ValidarLicencaMotorista");

            if (configuracaoTipoOperacaoLicenca.Codigo == 0)
                repositorioConfiguracaoTipoOperacaoLicenca.Inserir(configuracaoTipoOperacaoLicenca);
            else
                repositorioConfiguracaoTipoOperacaoLicenca.Atualizar(configuracaoTipoOperacaoLicenca, Auditado, historico);

            tipoOperacao.ConfiguracaoTipoOperacaoLicenca = configuracaoTipoOperacaoLicenca;

            repTipoOperacao.Atualizar(tipoOperacao);
        }

        private void AtualizarConfiguracoesJanelaCarregamento(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, Dominio.Entidades.Auditoria.HistoricoObjeto historico, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
            Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoJanelaCarregamento repositorioConfiguracaoTipoOperacaoJanelaCarregamento = new Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoJanelaCarregamento(unitOfWork);

            Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoJanelaCarregamento configuracaoJanelaCarregamento = tipoOperacao.ConfiguracaoJanelaCarregamento ?? new Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoJanelaCarregamento();

            if (configuracaoJanelaCarregamento.Codigo > 0)
                configuracaoJanelaCarregamento.Initialize();

            configuracaoJanelaCarregamento.PermitirRejeitarCargaJanelaCarregamentoTransportador = Request.GetBoolParam("PermitirRejeitarCargaJanelaCarregamentoTransportador");
            configuracaoJanelaCarregamento.PermitirLiberarCargaComTipoCondicaoPagamentoFOBParaTransportadores = Request.GetBoolParam("PermitirLiberarCargaComTipoCondicaoPagamentoFOBParaTransportadores");
            configuracaoJanelaCarregamento.LayoutImpressaoOrdemColeta = Request.GetEnumParam("LayoutImpressaoOrdemColeta", LayoutImpressaoOrdemColeta.LayoutPadrao);

            if (configuracaoJanelaCarregamento.Codigo == 0)
                repositorioConfiguracaoTipoOperacaoJanelaCarregamento.Inserir(configuracaoJanelaCarregamento);
            else
                repositorioConfiguracaoTipoOperacaoJanelaCarregamento.Atualizar(configuracaoJanelaCarregamento, Auditado, historico);

            tipoOperacao.ConfiguracaoJanelaCarregamento = configuracaoJanelaCarregamento;

            repTipoOperacao.Atualizar(tipoOperacao);
        }

        private void AtualizarConfiguracoesMontagemCarga(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, Dominio.Entidades.Auditoria.HistoricoObjeto historico, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoMontagemCarga repositorioConfiguracaoTipoOperacaoMontagemCarga = new Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoMontagemCarga(unitOfWork);

            Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoMontagemCarga configuracaomontagem = tipoOperacao.ConfiguracaoMontagemCarga ?? new Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoMontagemCarga();

            if (configuracaomontagem.Codigo > 0)
                configuracaomontagem.Initialize();

            configuracaomontagem.DisponibilizarPedidosMontagemAoFinalizarTransporte = Request.GetBoolParam("DisponibilizarPedidosMontagemAoFinalizarTransporte");
            configuracaomontagem.ExibirPedidosMontagemIntegracao = Request.GetBoolParam("ExibirPedidosMontagemIntegracao");
            configuracaomontagem.DisponibilizarPedidosMontagemDeterminadosTransportadores = Request.GetBoolParam("DisponibilizarPedidosMontagemDeterminadosTransportadores");
            configuracaomontagem.OcultarTipoDeOperacaoNaMontagemDaCarga = Request.GetBoolParam("OcultarTipoDeOperacaoNaMontagemDaCarga");
            configuracaomontagem.ExigirInformarDataPrevisaoInicioViagem = Request.GetBoolParam("ExigirInformarDataPrevisaoInicioViagem");
            configuracaomontagem.ControlarCapacidadePorUnidade = Request.GetBoolParam("ControlarCapacidadePorUnidade");
            configuracaomontagem.RoteirizarNovamenteAoConfirmarDocumentos = Request.GetBoolParam("RoteirizarNovamenteAoConfirmarDocumentos");
            configuracaomontagem.MontagemComRecebedorNaoGerarCargaComoColeta = Request.GetBoolParam("MontagemComRecebedorNaoGerarCargaComoColeta");

            if (configuracaomontagem.TransportadoresMontagemCarga == null)
                configuracaomontagem.TransportadoresMontagemCarga = new List<Dominio.Entidades.Empresa>();
            else
                configuracaomontagem.TransportadoresMontagemCarga.Clear();

            if (configuracaomontagem.DisponibilizarPedidosMontagemDeterminadosTransportadores)
            {
                List<int> codigoTranspotadores = Request.GetListParam<int>("TransportadoresMontagem");
                foreach (int transp in codigoTranspotadores)
                {
                    Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(transp);
                    if (empresa != null)
                        configuracaomontagem.TransportadoresMontagemCarga.Add(empresa);
                }
            }

            if (configuracaomontagem.Codigo == 0)
                repositorioConfiguracaoTipoOperacaoMontagemCarga.Inserir(configuracaomontagem);
            else
                repositorioConfiguracaoTipoOperacaoMontagemCarga.Atualizar(configuracaomontagem, Auditado, historico);

            tipoOperacao.ConfiguracaoMontagemCarga = configuracaomontagem;

            repTipoOperacao.Atualizar(tipoOperacao);
        }

        private void AtualizarConfiguracoesGestaoDevolucao(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoGestaoDevolucao repositorioConfiguracaoTipoOperacaoGestaoDevolucao = new Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoGestaoDevolucao(unitOfWork);

            Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoGestaoDevolucao configuracaoGestaoDevolucao = tipoOperacao.ConfiguracaoGestaoDevolucao ?? new Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoGestaoDevolucao();

            if (configuracaoGestaoDevolucao.Codigo > 0)
                configuracaoGestaoDevolucao.Initialize();

            configuracaoGestaoDevolucao.UsarComoPadraoQuandoCargaForDevolucaoDoTipoColeta = Request.GetBoolParam("UsarComoPadraoQuandoCargaForDevolucaoDoTipoColeta");

            if (configuracaoGestaoDevolucao.Codigo == 0)
                repositorioConfiguracaoTipoOperacaoGestaoDevolucao.Inserir(configuracaoGestaoDevolucao);
            else
                repositorioConfiguracaoTipoOperacaoGestaoDevolucao.Atualizar(configuracaoGestaoDevolucao);

            tipoOperacao.ConfiguracaoGestaoDevolucao = configuracaoGestaoDevolucao;

            repTipoOperacao.Atualizar(tipoOperacao);
        }

        private void AtualizarConfiguracoesCotacaoPedido(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
            Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoCotacaoPedido repositorioConfiguracaoTipoOperacaoCotacaoPedido = new Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoCotacaoPedido(unitOfWork);

            Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoCotacaoPedido configuracaoCotacaoPedido = tipoOperacao.ConfiguracaoCotacaoPedido ?? new Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoCotacaoPedido();

            if (configuracaoCotacaoPedido.Codigo > 0)
                configuracaoCotacaoPedido.Initialize();

            configuracaoCotacaoPedido.HabilitaInformarDadosDosPedidosNaCotacao = Request.GetBoolParam("HabilitaInformarDadosDosPedidosNaCotacao");

            if (configuracaoCotacaoPedido.Codigo == 0)
                repositorioConfiguracaoTipoOperacaoCotacaoPedido.Inserir(configuracaoCotacaoPedido);
            else
                repositorioConfiguracaoTipoOperacaoCotacaoPedido.Atualizar(configuracaoCotacaoPedido);

            tipoOperacao.ConfiguracaoCotacaoPedido = configuracaoCotacaoPedido;

            repTipoOperacao.Atualizar(tipoOperacao);
        }
        private void AtualizarConfiguracoesContainer(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
            Repositorio.ModeloDocumentoFiscal repModelo = new Repositorio.ModeloDocumentoFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.ComprovanteCarga.TipoComprovante repTipoComprovante = new Repositorio.Embarcador.Cargas.ComprovanteCarga.TipoComprovante(unitOfWork);

            Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoContainer repositorioConfiguracaoTipoOperacaoContainer = new Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoContainer(unitOfWork);
            Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTipo repTipoPagamento = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTipo(unitOfWork);
            Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoContainer configuracaoContainer = tipoOperacao.ConfiguracaoContainer ?? new Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoContainer();

            if (configuracaoContainer.Codigo > 0)
                configuracaoContainer.Initialize();

            configuracaoContainer.GestaoViagemContainerFluxoUnico = Request.GetBoolParam("GestaoViagemContainerFluxoUnico");
            configuracaoContainer.NaoPermitirAlterarMotoristaAposAverbacaoContainer = Request.GetBoolParam("NaoPermitirAlterarMotoristaAposAverbacaoContainer");
            configuracaoContainer.ExigirComprovanteColetaContainerParaSeguir = Request.GetBoolParam("ExigirComprovanteColetaContainerParaSeguir");
            configuracaoContainer.GerarPagamentoMotoristaAutomaticamenteComValorAdiantamentoContainer = Request.GetBoolParam("GerarPagamentoMotoristaAutomaticamenteComValorAdiantamentoContainer");
            configuracaoContainer.ComprarValePedagioEtapaContainer = Request.GetBoolParam("ComprarValePedagioEtapaContainer");
            configuracaoContainer.PagamentoMotoristaTipo = repTipoPagamento.BuscarPorCodigo(Request.GetIntParam("TipoPagamentoAdiantamentoContainer"));
            configuracaoContainer.ModeloDocumentoContainer = repModelo.BuscarPorId(Request.GetIntParam("ModeloDocumentoDocumentoContainer"));
            configuracaoContainer.TipoComprovanteColetaContainer = repTipoComprovante.BuscarPorCodigo(Request.GetIntParam("TipoComprovanteColetaContainer"));

            if (configuracaoContainer.Codigo == 0)
                repositorioConfiguracaoTipoOperacaoContainer.Inserir(configuracaoContainer);
            else
                repositorioConfiguracaoTipoOperacaoContainer.Atualizar(configuracaoContainer);

            tipoOperacao.ConfiguracaoContainer = configuracaoContainer;

            repTipoOperacao.Atualizar(tipoOperacao);
        }

        private void AtualizarConfiguracoesIntegracaoDiageo(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, Dominio.Entidades.Auditoria.HistoricoObjeto historico, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
            Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoIntegracaoDiageo repositorioConfiguracaoTipoOperacaoMontagemCarga = new Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoIntegracaoDiageo(unitOfWork);

            Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoIntegracaoDiageo configuracaoDiageo = tipoOperacao.ConfiguracaoTipoOperacaoIntegracaoDiageo ?? new Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoIntegracaoDiageo();

            if (configuracaoDiageo.Codigo > 0)
                configuracaoDiageo.Initialize();

            configuracaoDiageo.PossuiIntegracaoDiageo = Request.GetBoolParam("PossuiIntegracaoDiageo");

            if (configuracaoDiageo.Codigo == 0)
                repositorioConfiguracaoTipoOperacaoMontagemCarga.Inserir(configuracaoDiageo);
            else
                repositorioConfiguracaoTipoOperacaoMontagemCarga.Atualizar(configuracaoDiageo, Auditado, historico);

            tipoOperacao.ConfiguracaoTipoOperacaoIntegracaoDiageo = configuracaoDiageo;

            repTipoOperacao.Atualizar(tipoOperacao);
        }

        private void PreencherConfiguracaoCarga(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao)
        {
            tipoOperacao.EmailDisponibilidadeCarga = Request.GetStringParam("EmailDisponibilidadeCarga");
            tipoOperacao.ExigirVeiculoComRastreador = Request.GetBoolParam("ExigirVeiculoComRastreador");
            tipoOperacao.ObrigatorioVincularContainerCarga = Request.GetBoolParam("ObrigatorioVincularContainerCarga");
            tipoOperacao.ObrigatorioRealizarConferenciaContainerCarga = Request.GetBoolParam("ObrigatorioRealizarConferenciaContainerCarga");
            tipoOperacao.ValidarSeCargaPossuiVinculoComPreCarga = Request.GetBoolParam("ValidarSeCargaPossuiVinculoComPreCarga");
            tipoOperacao.CargaTipoConsolidacao = Request.GetBoolParam("CargaTipoConsolidacao");
            tipoOperacao.TipoOperacaoMercosul = Request.GetBoolParam("TipoOperacaoMercosul");
            tipoOperacao.CalcularPautaFiscal = Request.GetBoolParam("CalcularPautaFiscal");
            tipoOperacao.NaoPermitirAvancarCargaSemRegraICMS = Request.GetBoolParam("NaoPermitirAvancarCargaSemRegraICMS");
            tipoOperacao.ObrigarInformarRICnaColetaDeConteiner = Request.GetBoolParam("ObrigarInformarRICnaColetaDeConteiner") && tipoOperacao.ObrigatorioVincularContainerCarga;
            tipoOperacao.ExigirCargaRoteirizada = Request.GetBoolParam("ExigirCargaRoteirizada");
            tipoOperacao.ExpressaoRegularNumeroBookingObservacaoCTe = Request.GetBoolParam("ExpressaoRegularNumeroBookingObservacaoCTe");
            tipoOperacao.ExpressaoRegularNumeroContainerObservacaoCTe = Request.GetBoolParam("ExpressaoRegularNumeroContainerObservacaoCTe");
            tipoOperacao.ExpressaoBooking = Request.GetStringParam("ExpressaoBooking");
            tipoOperacao.ExpressaoContainer = Request.GetStringParam("ExpressaoContainer");
            tipoOperacao.CargaBloqueadaParaEdicaoIntegracao = Request.GetBoolParam("CargaBloqueadaParaEdicaoIntegracao");
            tipoOperacao.NaoCriarAprovacaoCargaConfirmarDocumento = Request.GetBoolParam("NaoCriarAprovacaoCargaConfirmarDocumento");
        }

        private void PreencherConfiguracaoTransportador(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao)
        {
            if (tipoOperacao.PermiteImportarDocumentosManualmente)
                tipoOperacao.PermitirTransportadorEnviarNotasFiscais = Request.GetBoolParam("PermitirTransportadorEnviarNotasFiscais");
            else
                tipoOperacao.PermitirTransportadorEnviarNotasFiscais = false;
        }

        private void AtualizarIntegracoes(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            dynamic integracoes = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Integracoes"));

            ExcluirIntegracoesRemovidas(tipoOperacao, integracoes, unidadeDeTrabalho);
            SalvarIntegracoesAdicionadas(tipoOperacao, integracoes, unidadeDeTrabalho);
        }

        private void ExcluirIntegracoesRemovidas(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, dynamic integracoes, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (tipoOperacao.Integracoes?.Count > 0)
            {
                Repositorio.Embarcador.Pedidos.TipoOperacaoIntegracao repositorioIntegracao = new Repositorio.Embarcador.Pedidos.TipoOperacaoIntegracao(unidadeDeTrabalho);
                List<int> listaCodigosAtualizados = new List<int>();

                foreach (dynamic integracao in integracoes)
                {
                    int? codigoIntegracao = ((string)integracao.Codigo).ToNullableInt();

                    if (codigoIntegracao.HasValue)
                        listaCodigosAtualizados.Add(codigoIntegracao.Value);
                }

                List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoIntegracao> listaIntegracoesRemover = (from tipoOperacaoIntegracao in tipoOperacao.Integracoes where !listaCodigosAtualizados.Contains(tipoOperacaoIntegracao.Codigo) select tipoOperacaoIntegracao).ToList();

                foreach (Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoIntegracao integracao in listaIntegracoesRemover)
                {
                    repositorioIntegracao.Deletar(integracao);
                }

                if (listaIntegracoesRemover.Count > 0)
                {
                    string descricaoAcao = listaIntegracoesRemover.Count == 1 ? Localization.Resources.Pedidos.TipoOperacao.IntegracaoRemovida : Localization.Resources.Pedidos.TipoOperacao.MultiplasIntegracaesRemovidas;

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, tipoOperacao, null, descricaoAcao, unidadeDeTrabalho, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Registro);
                }
            }
        }

        private void SalvarIntegracoesAdicionadas(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, dynamic integracoes, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Pedidos.TipoOperacaoIntegracao repositorioIntegracao = new Repositorio.Embarcador.Pedidos.TipoOperacaoIntegracao(unidadeDeTrabalho);
            List<int> listaCodigosAdicionar = new List<int>();

            foreach (dynamic integracao in integracoes)
            {
                int? codigoIntegracao = ((string)integracao.Codigo).ToNullableInt();

                if (!codigoIntegracao.HasValue)
                {
                    Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoIntegracao tipoOperacaoIntegracao = new Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoIntegracao()
                    {
                        Tipo = ((string)integracao.Tipo).ToEnum<TipoIntegracao>(),
                        TipoOperacao = tipoOperacao
                    };

                    repositorioIntegracao.Inserir(tipoOperacaoIntegracao);
                    listaCodigosAdicionar.Add(tipoOperacaoIntegracao.Codigo);
                }
            }

            if (tipoOperacao.IsInitialized() && (listaCodigosAdicionar.Count > 0))
            {
                string descricaoAcao = listaCodigosAdicionar.Count == 1 ? Localization.Resources.Pedidos.TipoOperacao.IntegracaoAdicionada : Localization.Resources.Pedidos.TipoOperacao.MultiplasIntegracoesAdicionadas;

                Servicos.Auditoria.Auditoria.Auditar(Auditado, tipoOperacao, null, descricaoAcao, unidadeDeTrabalho, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Registro);
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaTipoOperacao ObterFiltrosPesquisa(Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Pessoas.ModalidadeFornecedorPessoas repModalidadeFornecedorPessoas = new Repositorio.Embarcador.Pessoas.ModalidadeFornecedorPessoas(unidadeDeTrabalho);

            Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaTipoOperacao filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaTipoOperacao()
            {
                Ativo = Request.GetEnumParam("Ativo", SituacaoAtivoPesquisa.Ativo),
                CodigoGrupoPessoas = Request.GetIntParam("GrupoPessoas"),
                ContratoFrete = Request.GetIntParam("ContratoFrete"),
                Descricao = Request.GetStringParam("Descricao"),
                CodigoIntegracao = Request.GetStringParam("CodigoIntegracao"),
                SomenteTipoOperacaoPermiteGerarRedespacho = Request.GetBoolParam("SomenteTipoOperacaoPermiteGerarRedespacho"),
                TipoOperacaoPorTransportador = Request.GetBoolParam("TipoOperacaoPorTransportador"),
                CodigoTransportadorLogado = this.Usuario?.Empresa?.Codigo ?? 0,
                CodigoTipoCargaEmissao = Request.GetIntParam("TipoCargaEmissao"),
                FiltrarTipoOperacaoOcultas = Request.GetBoolParam("FiltrarTipoOperacaoOcultas"),
                FiltrarPorTipoDevolucao = Request.GetBoolParam("FiltrarPorTipoDevolucao"),

            };

            int codigoCarga = Request.GetIntParam("Carga");
            double cpfCnpjPessoa = Request.GetStringParam("Pessoa").ObterSomenteNumeros().ToDouble();

            if (cpfCnpjPessoa > 0)
            {
                Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unidadeDeTrabalho);
                filtrosPesquisa.Pessoa = repositorioCliente.BuscarPorCPFCNPJ(cpfCnpjPessoa);
            }

            if (codigoCarga > 0)
            {
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeDeTrabalho);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(codigoCarga);
                filtrosPesquisa.Pessoa = carga.Pedidos.FirstOrDefault().ObterTomador();
            }

            if (Request.GetBoolParam("FiltrarPorConfiguracaoOperadorLogistica", valorPadrao: true))
                filtrosPesquisa.ListaCodigoTipoOperacaoPermitidos = ObterListaCodigoTipoOperacaoPermitidosOperadorLogistica(unidadeDeTrabalho);

            if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                filtrosPesquisa.CodigoTransportadorLogado = 0;

            filtrosPesquisa.CodigosTiposOperacao = new List<int>();

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor)
            {
                Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoas modalidadeFornecedor = Usuario.ClienteFornecedor != null ? repModalidadeFornecedorPessoas.BuscarPorCliente(Usuario.ClienteFornecedor.CPF_CNPJ) : null;
                filtrosPesquisa.CodigosTiposOperacao = modalidadeFornecedor?.TipoOperacoes?.Select(o => o.Codigo).ToList() ?? null;
            }

            return filtrosPesquisa;
        }

        private dynamic SalvarConfiguracaoEmissaoCTe(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unidadeDeTrabalho);
            Repositorio.Embarcador.Rateio.RateioFormula repFormulaRateio = new Repositorio.Embarcador.Rateio.RateioFormula(unidadeDeTrabalho);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unidadeDeTrabalho);
            Repositorio.ModeloDocumentoFiscal repModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(unidadeDeTrabalho);
            Repositorio.Embarcador.Configuracoes.ArquivoImportacaoNotaFiscal repArquivoImportacaoNotaFiscal = new Repositorio.Embarcador.Configuracoes.ArquivoImportacaoNotaFiscal(unidadeDeTrabalho);
            Repositorio.Embarcador.Pedidos.TipoOperacaoIntegracaoFTP repTipoOperacaoIntegracaoFTP = new Repositorio.Embarcador.Pedidos.TipoOperacaoIntegracaoFTP(unidadeDeTrabalho);
            Repositorio.Embarcador.Pedidos.TipoOperacaoApoliceSeguro repTipoOperacaoApoliceSeguro = new Repositorio.Embarcador.Pedidos.TipoOperacaoApoliceSeguro(unidadeDeTrabalho);
            Repositorio.Embarcador.Seguros.ApoliceSeguro repApoliceSeguro = new Repositorio.Embarcador.Seguros.ApoliceSeguro(unidadeDeTrabalho);
            Repositorio.TipoDeOcorrenciaDeCTe repTipoOcorrencia = new Repositorio.TipoDeOcorrenciaDeCTe(unidadeDeTrabalho);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);

            Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoIntegracaoFTP tipoOperacaoIntegracaoFTP = repTipoOperacaoIntegracaoFTP.BuscarPorTipoOperacaoAsync(tipoOperacao.Codigo, default).GetAwaiter().GetResult();

            dynamic configuracao = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ConfiguracaoEmissaoCTe"));

            int codigoTipoOcorrenciaComplementoSubcontratacao = (int)configuracao.TipoOcorrenciaComplementoSubcontratacao;

            tipoOperacao.TipoOcorrenciaComplementoSubcontratacao = codigoTipoOcorrenciaComplementoSubcontratacao > 0 ? repTipoOcorrencia.BuscarPorCodigo(codigoTipoOcorrenciaComplementoSubcontratacao) : null;
            tipoOperacao.GerarOcorrenciaComplementoSubcontratacao = (bool)configuracao.GerarOcorrenciaComplementoSubcontratacao;
            tipoOperacao.ValorFreteLiquidoDeveSerValorAReceber = (bool)configuracao.ValorFreteLiquidoDeveSerValorAReceber;
            tipoOperacao.ValorFreteLiquidoDeveSerValorAReceberSemICMS = (bool)configuracao.ValorFreteLiquidoDeveSerValorAReceberSemICMS;
            tipoOperacao.ValorMaximoEmissaoPendentePagamento = Utilidades.Decimal.Converter((string)configuracao.ValorMaximoEmissaoPendentePagamento);
            tipoOperacao.TipoEnvioEmail = (TipoEnvioEmailCTe)configuracao.TipoEnvioEmail;
            tipoOperacao.ObservacaoEmissaoCarga = (string)configuracao.ObservacaoEmissaoCarga;
            tipoOperacao.GerarMDFeTransbordoSemConsiderarOrigem = (bool)configuracao.GerarMDFeTransbordoSemConsiderarOrigem;
            tipoOperacao.AgruparMovimentoFinanceiroPorPedido = (bool)configuracao.AgruparMovimentoFinanceiroPorPedido;
            tipoOperacao.ValePedagioObrigatorio = (bool)configuracao.ValePedagioObrigatorio;
            tipoOperacao.NaoValidarNotaFiscalExistente = (bool)configuracao.NaoValidarNotaFiscalExistente;
            tipoOperacao.NaoValidarNotasFiscaisComDiferentesPortos = (bool)configuracao.NaoValidarNotasFiscaisComDiferentesPortos;
            tipoOperacao.NaoEmitirMDFe = (bool)configuracao.NaoEmitirMDFe;
            tipoOperacao.ProvisionarDocumentos = (bool)configuracao.ProvisionarDocumentos;
            tipoOperacao.DisponibilizarDocumentosParaLoteEscrituracao = (bool)configuracao.DisponibilizarDocumentosParaLoteEscrituracao;
            tipoOperacao.DisponibilizarDocumentosParaLoteEscrituracaoCancelamento = (bool)configuracao.DisponibilizarDocumentosParaLoteEscrituracaoCancelamento;
            tipoOperacao.DisponibilizarDocumentosParaPagamento = (bool)configuracao.DisponibilizarDocumentosParaPagamento;
            tipoOperacao.QuitarDocumentoAutomaticamenteAoGerarLote = ((string)configuracao.QuitarDocumentoAutomaticamenteAoGerarLote).ToBool();
            tipoOperacao.EscriturarSomenteDocumentosEmitidosParaNFe = (bool)configuracao.EscriturarSomenteDocumentosEmitidosParaNFe;
            tipoOperacao.CTeEmitidoNoEmbarcador = (bool)configuracao.CTeEmitidoNoEmbarcador;
            tipoOperacao.ExigirNumeroPedido = configuracao.ExigirNumeroPedido != null ? (bool)configuracao.ExigirNumeroPedido : false;
            tipoOperacao.RegexValidacaoNumeroPedidoEmbarcador = (string)configuracao.RegexValidacaoNumeroPedidoEmbarcador;
            tipoOperacao.TipoEmissaoCTeDocumentos = (TipoEmissaoCTeDocumentos)configuracao.TipoRateioDocumentos;
            tipoOperacao.TipoEmissaoCTeParticipantes = (TipoEmissaoCTeParticipantes)configuracao.TipoEmissaoCTeParticipantes;
            tipoOperacao.BloquearDiferencaValorFreteEmbarcador = (bool)configuracao.BloquearDiferencaValorFreteEmbarcador;
            tipoOperacao.EmitirComplementoDiferencaFreteEmbarcador = (bool)configuracao.EmitirComplementoDiferencaFreteEmbarcador;
            tipoOperacao.GerarOcorrenciaSemTabelaFrete = (bool)configuracao.GerarOcorrenciaSemTabelaFrete;
            tipoOperacao.TipoOcorrenciaSemTabelaFrete = repTipoOcorrencia.BuscarPorCodigo((int)configuracao.TipoOcorrenciaSemTabelaFrete);
            tipoOperacao.PercentualBloquearDiferencaValorFreteEmbarcador = (decimal)configuracao.PercentualBloquearDiferencaValorFreteEmbarcador;
            tipoOperacao.TipoOcorrenciaComplementoDiferencaFreteEmbarcador = repTipoOcorrencia.BuscarPorCodigo((int)configuracao.TipoOcorrenciaComplementoDiferencaFreteEmbarcador);
            tipoOperacao.TipoOcorrenciaCTeEmitidoEmbarcador = repTipoOcorrencia.BuscarPorCodigo((int)configuracao.TipoOcorrenciaCTeEmitidoEmbarcador);
            tipoOperacao.GerarCIOTParaTodasAsCargas = (bool)configuracao.GerarCIOTParaTodasAsCargas;
            tipoOperacao.NaoPermitirVincularCTeComplementarEmCarga = configuracao.NaoPermitirVincularCTeComplementarEmCarga != null ? (bool)configuracao.NaoPermitirVincularCTeComplementarEmCarga : false;
            tipoOperacao.TempoCarregamento = RetornarTimeSpan((string)configuracao.TempoCarregamento);
            tipoOperacao.TempoDescarregamento = RetornarTimeSpan((string)configuracao.TempoDescarregamento);
            tipoOperacao.TipoIntegracaoMercadoLivre = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoMercadoLivre)configuracao.TipoIntegracaoMercadoLivre;
            tipoOperacao.IntegracaoMercadoLivreRealizarConsultaRotaEFacilityAutomaticamente = (bool)configuracao.IntegracaoMercadoLivreRealizarConsultaRotaEFacilityAutomaticamente;
            tipoOperacao.IntegracaoMercadoLivreAvancarCargaEtapaNFeAutomaticamente = (bool)configuracao.IntegracaoMercadoLivreAvancarCargaEtapaNFeAutomaticamente;
            tipoOperacao.TipoTempoAcrescimoDecrescimoDataPrevisaoSaida = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTempoAcrescimoDecrescimoDataPrevisaoSaida)configuracao.TipoTempoAcrescimoDecrescimoDataPrevisaoSaida;
            tipoOperacao.TempoAcrescimoDecrescimoDataPrevisaoSaida = RetornarTimeSpan((string)configuracao.TempoAcrescimoDecrescimoDataPrevisaoSaida);
            tipoOperacao.ObrigatorioInformarMDFeEmitidoPeloEmbarcador = tipoOperacao.CTeEmitidoNoEmbarcador ? (bool)configuracao.ObrigatorioInformarMDFeEmitidoPeloEmbarcador : false;

            tipoOperacao.TipoEmissaoIntramunicipal = (TipoEmissaoIntramunicipal)configuracao.TipoEmissaoIntramunicipal;

            if (tipoOperacao.TipoEmissaoIntramunicipal == TipoEmissaoIntramunicipal.SempreNFSManual)
            {
                tipoOperacao.UtilizarOutroModeloDocumentoEmissaoMunicipal = (bool)configuracao.UtilizarOutroModeloDocumentoEmissaoMunicipal;

                if (tipoOperacao.UtilizarOutroModeloDocumentoEmissaoMunicipal)
                    tipoOperacao.ModeloDocumentoFiscalEmissaoMunicipal = repModeloDocumentoFiscal.BuscarPorId((int)configuracao.ModeloDocumentoFiscalEmissaoMunicipal);
                else
                    tipoOperacao.ModeloDocumentoFiscalEmissaoMunicipal = null;
            }
            else
            {
                tipoOperacao.UtilizarOutroModeloDocumentoEmissaoMunicipal = false;
                tipoOperacao.ModeloDocumentoFiscalEmissaoMunicipal = null;
            }

            tipoOperacao.RateioFormula = repFormulaRateio.BuscarPorCodigo((int)configuracao.FormulaRateioFrete);
            tipoOperacao.TipoIntegracao = repTipoIntegracao.BuscarPorTipo((TipoIntegracao)(int)configuracao.TipoIntegracao);
            tipoOperacao.ArquivoImportacaoNotaFiscal = repArquivoImportacaoNotaFiscal.BuscarPorCodigo((int)configuracao.ArquivoImportacaoNotasFiscais);
            tipoOperacao.DescricaoComponenteFreteEmbarcador = (string)configuracao.DescricaoComponenteFreteEmbarcador;

            if ((int)configuracao.ModeloDocumentoFiscal > 0)
                tipoOperacao.ModeloDocumentoFiscal = repModeloDocumentoFiscal.BuscarPorId((int)configuracao.ModeloDocumentoFiscal);
            else
                tipoOperacao.ModeloDocumentoFiscal = null;

            tipoOperacao.DisponibilizarDocumentosParaNFsManual = (bool)configuracao.DisponibilizarDocumentosParaNFsManual;

            if ((int)configuracao.EmpresaEmissora > 0)
                tipoOperacao.EmpresaEmissora = repEmpresa.BuscarPorCodigo((int)configuracao.EmpresaEmissora);
            else
                tipoOperacao.EmpresaEmissora = null;

            double.TryParse((string)configuracao.EmitenteImportacaoRedespachoIntermediario, out double cpfCnpjEmitenteImportacaoRedespachoIntermediario);
            double.TryParse((string)configuracao.ExpedidorImportacaoRedespachoIntermediario, out double cpfCnpjExpedidorImportacaoRedespachoIntermediario);
            double.TryParse((string)configuracao.RecebedorImportacaoRedespachoIntermediario, out double cpfCnpjRecebedorImportacaoRedespachoIntermediario);

            tipoOperacao.ImportarRedespachoIntermediario = (bool)configuracao.ImportarRedespachoIntermediario;
            tipoOperacao.EmitenteImportacaoRedespachoIntermediario = cpfCnpjEmitenteImportacaoRedespachoIntermediario > 0D ? repCliente.BuscarPorCPFCNPJ(cpfCnpjEmitenteImportacaoRedespachoIntermediario) : null;
            tipoOperacao.ExpedidorImportacaoRedespachoIntermediario = cpfCnpjExpedidorImportacaoRedespachoIntermediario > 0D ? repCliente.BuscarPorCPFCNPJ(cpfCnpjExpedidorImportacaoRedespachoIntermediario) : null;
            tipoOperacao.RecebedorImportacaoRedespachoIntermediario = cpfCnpjRecebedorImportacaoRedespachoIntermediario > 0D ? repCliente.BuscarPorCPFCNPJ(cpfCnpjRecebedorImportacaoRedespachoIntermediario) : null;

            tipoOperacao.DescricaoItemPesoCTeSubcontratacao = (string)configuracao.DescricaoItemPesoCTeSubcontratacao;
            tipoOperacao.CaracteristicaTransporteCTe = (string)configuracao.CaracteristicaTransporteCTe;
            tipoOperacao.ObservacaoCTe = (string)configuracao.Observacao;
            tipoOperacao.ObservacaoCTeTerceiro = (string)configuracao.ObservacaoTerceiro;

            // Adiciona as apolices
            List<int> codigoApolicesNaoExcluir = new List<int>();
            List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoApoliceSeguro> apolicesParaCadastrar = new List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoApoliceSeguro>();

            for (int i = 0; i < configuracao.ApolicesSeguro.Count; i++)
            {
                Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro apoliceSeguro = repApoliceSeguro.BuscarPorCodigo((int)configuracao.ApolicesSeguro[i]);

                // Apolice valida
                if (apoliceSeguro != null)
                {
                    Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoApoliceSeguro apolice = new Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoApoliceSeguro
                    {
                        ApoliceSeguro = apoliceSeguro,
                        TipoOperacao = tipoOperacao,
                        Desconto = null
                    };

                    if (repTipoOperacaoApoliceSeguro.TipoOperacaoApoliceSeguroJaExiste(tipoOperacao.Codigo, apolice.ApoliceSeguro.Codigo))
                        codigoApolicesNaoExcluir.Add(apolice.ApoliceSeguro.Codigo);
                    else
                        apolicesParaCadastrar.Add(apolice);
                }
            }

            if (tipoOperacao.ClientesBloquearEmissaoDosDestinatario == null)
                tipoOperacao.ClientesBloquearEmissaoDosDestinatario = new List<Dominio.Entidades.Cliente>();

            tipoOperacao.TipoPropostaMultimodal = (TipoPropostaMultimodal)configuracao.TipoPropostaMultimodal;
            tipoOperacao.TipoServicoMultimodal = (TipoServicoMultimodal)configuracao.TipoServicoMultimodal;
            tipoOperacao.ModalPropostaMultimodal = (ModalPropostaMultimodal)configuracao.ModalPropostaMultimodal;
            tipoOperacao.TipoCobrancaMultimodal = (TipoCobrancaMultimodal)configuracao.TipoCobrancaMultimodal;

            bool.TryParse((string)configuracao.BloquearEmissaoDeEntidadeSemCadastro, out bool bloquearEmissaoDeEntidadeSemCadastro);
            bool.TryParse((string)configuracao.BloquearEmissaoDosDestinatario, out bool bloquearEmissaoDosDestinatario);
            tipoOperacao.BloquearEmissaoDeEntidadeSemCadastro = bloquearEmissaoDeEntidadeSemCadastro;
            tipoOperacao.BloquearEmissaoDosDestinatario = bloquearEmissaoDosDestinatario;

            List<double> codigosClientesBloqueio = new List<double>();

            for (int i = 0; i < configuracao.ClientesBloqueados.Count; i++)
                codigosClientesBloqueio.Add((double)configuracao.ClientesBloqueados[i]);

            List<Dominio.Entidades.Cliente> clientesRemover = tipoOperacao.ClientesBloquearEmissaoDosDestinatario.Where(o => !codigosClientesBloqueio.Contains(o.CPF_CNPJ)).ToList();

            foreach (Dominio.Entidades.Cliente clienteRemover in clientesRemover)
            {
                tipoOperacao.ClientesBloquearEmissaoDosDestinatario.Remove(clienteRemover);
            }

            foreach (double codigoClientesBloquei in codigosClientesBloqueio)
            {
                if (!tipoOperacao.ClientesBloquearEmissaoDosDestinatario.Any(o => o.Codigo == codigoClientesBloquei))
                {
                    Dominio.Entidades.Cliente clienteBloqueio = repCliente.BuscarPorCPFCNPJ(codigoClientesBloquei);

                    tipoOperacao.ClientesBloquearEmissaoDosDestinatario.Add(clienteBloqueio);
                }
            }

            /* Uma aba espec�ifica para v�nculo de apolices � exibida no cadastro do tipo de opera��o quando � Embarcador
             * Ao salvar, os dados atualizados da aba não s�o replicados para as apolices dentro da Configura��o Emiss�o
             * Sendo assim, todas apolices cadastradas na aba se Apolices s�o excluidas
             * A aba de apolices � apenas para embarcador
             */
            if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
            {
                // Buscas apolices que deverao ser excluidas
                List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoApoliceSeguro> excluirApolices = repTipoOperacaoApoliceSeguro.BuscarPorCodigosDiferente(tipoOperacao.Codigo, codigoApolicesNaoExcluir);

                // Exclui todas
                for (int i = 0; i < excluirApolices.Count; i++)
                {
                    Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoApoliceSeguro tipoOperacaoApoliceSeguro = excluirApolices[i];

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, tipoOperacao, Localization.Resources.Pedidos.TipoOperacao.RemoveuApoliceDeSeguro + tipoOperacaoApoliceSeguro.Descricao + ".", unidadeDeTrabalho);

                    repTipoOperacaoApoliceSeguro.Deletar(tipoOperacaoApoliceSeguro);
                }

                // Insere as apolices
                for (int i = 0; i < apolicesParaCadastrar.Count; i++)
                {
                    Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoApoliceSeguro tipoOperacaoApoliceSeguro = apolicesParaCadastrar[i];

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, tipoOperacao, Localization.Resources.Pedidos.TipoOperacao.AdicionouApoliceDeSeguro + tipoOperacaoApoliceSeguro.Descricao + ".", unidadeDeTrabalho);

                    repTipoOperacaoApoliceSeguro.Inserir(tipoOperacaoApoliceSeguro);
                }
            }

            if (tipoOperacao.TipoIntegracao != null && tipoOperacao.TipoIntegracao.Tipo == TipoIntegracao.FTP)
            {
                if (tipoOperacaoIntegracaoFTP == null)
                    tipoOperacaoIntegracaoFTP = new Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoIntegracaoFTP();
                else
                    tipoOperacaoIntegracaoFTP.Initialize();

                tipoOperacaoIntegracaoFTP.TipoOperacao = tipoOperacao;
                tipoOperacaoIntegracaoFTP.Diretorio = (string)configuracao.Diretorio;
                tipoOperacaoIntegracaoFTP.EnderecoFTP = (string)configuracao.EnderecoFTP;
                tipoOperacaoIntegracaoFTP.Passivo = (bool)configuracao.Passivo;
                tipoOperacaoIntegracaoFTP.Porta = (string)configuracao.Porta;
                tipoOperacaoIntegracaoFTP.Senha = (string)configuracao.Senha;
                tipoOperacaoIntegracaoFTP.Usuario = (string)configuracao.Usuario;
                tipoOperacaoIntegracaoFTP.NomenclaturaArquivo = (string)configuracao.NomenclaturaArquivo;
                tipoOperacaoIntegracaoFTP.UtilizarSFTP = (bool)configuracao.UtilizarSFTP;

                tipoOperacaoIntegracaoFTP.SSL = (bool)configuracao.SSL;

                if (tipoOperacaoIntegracaoFTP.Codigo > 0)
                {
                    repTipoOperacaoIntegracaoFTP.Atualizar(tipoOperacaoIntegracaoFTP, Auditado);
                    List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = tipoOperacaoIntegracaoFTP.GetChanges();
                    if (alteracoes.Count > 0)
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, tipoOperacao, alteracoes, "FTP " + tipoOperacaoIntegracaoFTP.Descricao + Localization.Resources.Pedidos.TipoOperacao.Alterado, unidadeDeTrabalho);
                }
                else
                {
                    repTipoOperacaoIntegracaoFTP.Inserir(tipoOperacaoIntegracaoFTP, Auditado);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, tipoOperacao, null, "FTP " + tipoOperacaoIntegracaoFTP.Descricao + Localization.Resources.Pedidos.TipoOperacao.AdicionadoAoTipoDeOperacao, unidadeDeTrabalho);
                }
            }
            else if (tipoOperacaoIntegracaoFTP != null)
            {
                Servicos.Auditoria.Auditoria.Auditar(Auditado, tipoOperacao, null, "FTP " + tipoOperacaoIntegracaoFTP.Descricao + Localization.Resources.Pedidos.TipoOperacao.RemovidoAoTipoDeOperacao, unidadeDeTrabalho);
                repTipoOperacaoIntegracaoFTP.Deletar(tipoOperacaoIntegracaoFTP);
            }

            SalvarConfiguracaoComponentesFrete(tipoOperacao, configuracao.ComponentesFrete, unidadeDeTrabalho);

            repTipoOperacao.Atualizar(tipoOperacao);

            return configuracao;
        }

        private void SalvarConfiguracaoFatura(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacaoParam, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unidadeDeTrabalho);
            Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoFatura repConfiguracaoTipoOperacaoFatura = new Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoFatura(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoFatura tipoOperacao = tipoOperacaoParam?.ConfiguracaoTipoOperacaoFatura ?? new Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoFatura();
            if (tipoOperacao.Codigo > 0)
                tipoOperacao.Initialize();

            if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                return;

            if (string.IsNullOrWhiteSpace(Request.Params("ConfiguracaoFatura")))
                return;

            dynamic configuracaoFatura = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ConfiguracaoFatura"));
            if (configuracaoFatura == null)
                return;

            Repositorio.Embarcador.Financeiro.TipoPagamentoRecebimento repTipoPagamentoRecebimento = new Repositorio.Embarcador.Financeiro.TipoPagamentoRecebimento(unidadeDeTrabalho);
            Repositorio.Embarcador.Financeiro.BoletoConfiguracao repBoletoConfiguracao = new Repositorio.Embarcador.Financeiro.BoletoConfiguracao(unidadeDeTrabalho);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);

            TipoPrazoFaturamento tipoPrazoFaturamento;
            FormaGeracaoTituloFatura formaGeracaoTituloFatura;
            TipoAgrupamentoEnvioDocumentacao tipoAgrupamentoEnvioDocumentacao;
            TipoAgrupamentoEnvioDocumentacao tipoAgrupamentoEnvioDocumentacaoPorta;
            FormaEnvioDocumentacao formaEnvioDocumentacao;
            FormaEnvioDocumentacao formaEnvioDocumentacaoPorta;

            #region preenchendo dados
            tipoOperacao.GerarTituloAutomaticamenteComAdiantamentoSaldo = ((string)configuracaoFatura.GerarTituloAutomaticamenteComAdiantamentoSaldo).ToBool();
            tipoOperacao.PercentualAdiantamentoTituloAutomatico = ((string)configuracaoFatura.PercentualAdiantamentoTituloAutomatico).ToDecimal();
            tipoOperacao.PrazoAdiantamentoEmDiasTituloAutomatico = ((string)configuracaoFatura.PrazoAdiantamentoEmDiasTituloAutomatico).ToInt();
            tipoOperacao.PercentualSaldoTituloAutomatico = ((string)configuracaoFatura.PercentualSaldoTituloAutomatico).ToDecimal();
            tipoOperacao.PrazoSaldoEmDiasTituloAutomatico = ((string)configuracaoFatura.PrazoSaldoEmDiasTituloAutomatico).ToInt();

            tipoOperacao.TipoPrazoFaturamento = null;
            if (configuracaoFatura.TipoPrazoFaturamento != null)
            {
                Enum.TryParse((string)configuracaoFatura.TipoPrazoFaturamento, out tipoPrazoFaturamento);
                tipoOperacao.TipoPrazoFaturamento = tipoPrazoFaturamento;
            }

            tipoOperacao.FormaGeracaoTituloFatura = null;
            if (configuracaoFatura.FormaGeracaoTituloFatura != null)
            {
                Enum.TryParse((string)configuracaoFatura.FormaGeracaoTituloFatura, out formaGeracaoTituloFatura);
                tipoOperacao.FormaGeracaoTituloFatura = formaGeracaoTituloFatura;
            }

            tipoOperacao.DiasDePrazoFatura = 0;
            if (configuracaoFatura.DiasDePrazoFatura != null)
            {
                int diasPrazo = 0;
                int.TryParse((string)configuracaoFatura.DiasDePrazoFatura, out diasPrazo);
                tipoOperacao.DiasDePrazoFatura = diasPrazo;
            }

            tipoOperacao.PermiteFinalDeSemana = false;
            if (configuracaoFatura.PermiteFinalSemana != null)
                tipoOperacao.PermiteFinalDeSemana = (bool)configuracaoFatura.PermiteFinalSemana;

            tipoOperacao.ExigeCanhotoFisico = false;
            if (configuracaoFatura.ExigeCanhotoFisico != null)
                tipoOperacao.ExigeCanhotoFisico = (bool)configuracaoFatura.ExigeCanhotoFisico;

            tipoOperacao.ArmazenaCanhotoFisicoCTe = false;
            if (configuracaoFatura.ArmazenaCanhotoFisicoCTe != null)
                tipoOperacao.ArmazenaCanhotoFisicoCTe = (bool)configuracaoFatura.ArmazenaCanhotoFisicoCTe;

            tipoOperacao.SomenteOcorrenciasFinalizadoras = false;
            if (configuracaoFatura.SomenteOcorrenciasFinalizadoras != null)
                tipoOperacao.SomenteOcorrenciasFinalizadoras = (bool)configuracaoFatura.SomenteOcorrenciasFinalizadoras;

            tipoOperacao.FaturarSomenteOcorrenciasFinalizadoras = false;
            if (configuracaoFatura.FaturarSomenteOcorrenciasFinalizadoras != null)
                tipoOperacao.FaturarSomenteOcorrenciasFinalizadoras = (bool)configuracaoFatura.FaturarSomenteOcorrenciasFinalizadoras;

            tipoOperacao.NaoGerarFaturaAteReceberCanhotos = false;
            if (configuracaoFatura.NaoGerarFaturaAteReceberCanhotos != null)
                tipoOperacao.NaoGerarFaturaAteReceberCanhotos = (bool)configuracaoFatura.NaoGerarFaturaAteReceberCanhotos;

            tipoOperacao.GerarFaturamentoMultiplaParcela = false;
            if (configuracaoFatura.GerarFaturamentoMultiplaParcela != null)
                tipoOperacao.GerarFaturamentoMultiplaParcela = (bool)configuracaoFatura.GerarFaturamentoMultiplaParcela;

            tipoOperacao.AvisoVencimetoHabilitarConfiguracaoPersonalizada = false;
            if (configuracaoFatura.AvisoVencimetoHabilitarConfiguracaoPersonalizada != null)
                tipoOperacao.AvisoVencimetoHabilitarConfiguracaoPersonalizada = (bool)configuracaoFatura.AvisoVencimetoHabilitarConfiguracaoPersonalizada;

            tipoOperacao.AvisoVencimetoNaoEnviarEmail = false;
            if (configuracaoFatura.AvisoVencimetoNaoEnviarEmail != null)
                tipoOperacao.AvisoVencimetoNaoEnviarEmail = (bool)configuracaoFatura.AvisoVencimetoNaoEnviarEmail;

            tipoOperacao.CobrancaNaoEnviarEmail = false;
            if (configuracaoFatura.CobrancaNaoEnviarEmail != null)
                tipoOperacao.CobrancaNaoEnviarEmail = (bool)configuracaoFatura.CobrancaNaoEnviarEmail;

            tipoOperacao.AvisoVencimetoQunatidadeDias = int.TryParse(configuracaoFatura.AvisoVencimetoQunatidadeDias?.ToString(), out int dias) ? dias : 0;

            tipoOperacao.AvisoVencimetoEnviarDiariamente = false;
            if (configuracaoFatura.AvisoVencimetoEnviarDiariamente != null)
                tipoOperacao.AvisoVencimetoEnviarDiariamente = (bool)configuracaoFatura.AvisoVencimetoEnviarDiariamente;

            tipoOperacao.CobrancaHabilitarConfiguracaoPersonalizada = false;
            if (configuracaoFatura.CobrancaHabilitarConfiguracaoPersonalizada != null)
                tipoOperacao.CobrancaHabilitarConfiguracaoPersonalizada = (bool)configuracaoFatura.CobrancaHabilitarConfiguracaoPersonalizada;

            tipoOperacao.CobrancaQunatidadeDias = int.TryParse(configuracaoFatura.CobrancaQunatidadeDias?.ToString(), out int cobrancadias) ? cobrancadias : 0;

            if (tipoOperacao.ExigeCanhotoFisico.Value == false)
                tipoOperacao.NaoGerarFaturaAteReceberCanhotos = false;

            if (configuracaoFatura.TomadorFatura != null && (double)configuracaoFatura.TomadorFatura > 0)
                tipoOperacao.ClienteTomadorFatura = repCliente.BuscarPorCPFCNPJ((double)configuracaoFatura.TomadorFatura);
            else
                tipoOperacao.ClienteTomadorFatura = null;

            if (!string.IsNullOrWhiteSpace((string)configuracaoFatura.ObservacaoFatura))
                tipoOperacao.ObservacaoFatura = (string)configuracaoFatura.ObservacaoFatura;
            else
                tipoOperacao.ObservacaoFatura = null;

            if (!string.IsNullOrWhiteSpace((string)configuracaoFatura.EmailFatura))
                tipoOperacao.EmailFatura = (string)configuracaoFatura.EmailFatura;
            else
                tipoOperacao.EmailFatura = null;

            if (!string.IsNullOrWhiteSpace((string)configuracaoFatura.AssuntoEmailFatura))
                tipoOperacao.AssuntoEmailFatura = (string)configuracaoFatura.AssuntoEmailFatura;
            else
                tipoOperacao.AssuntoEmailFatura = null;

            if (!string.IsNullOrWhiteSpace((string)configuracaoFatura.CorpoEmailFatura))
                tipoOperacao.CorpoEmailFatura = (string)configuracaoFatura.CorpoEmailFatura;
            else
                tipoOperacao.CorpoEmailFatura = null;

            bool informarEmailEnvioDocumentacao = (bool)configuracaoFatura.InformarEmailEnvioDocumentacao;
            if (!string.IsNullOrWhiteSpace((string)configuracaoFatura.EmailEnvioDocumentacao) && informarEmailEnvioDocumentacao)
                tipoOperacao.EmailEnvioDocumentacao = (string)configuracaoFatura.EmailEnvioDocumentacao;
            else
                tipoOperacao.EmailEnvioDocumentacao = null;

            if (!string.IsNullOrWhiteSpace((string)configuracaoFatura.AssuntoDocumentacao))
                tipoOperacao.AssuntoDocumentacao = (string)configuracaoFatura.AssuntoDocumentacao;
            else
                tipoOperacao.AssuntoDocumentacao = null;

            if (!string.IsNullOrWhiteSpace((string)configuracaoFatura.CorpoEmailDocumentacao))
                tipoOperacao.CorpoEmailDocumentacao = (string)configuracaoFatura.CorpoEmailDocumentacao;
            else
                tipoOperacao.CorpoEmailDocumentacao = null;

            tipoOperacao.TipoAgrupamentoEnvioDocumentacao = null;
            if (configuracaoFatura.TipoAgrupamentoEnvioDocumentacao != null)
            {
                Enum.TryParse((string)configuracaoFatura.TipoAgrupamentoEnvioDocumentacao, out tipoAgrupamentoEnvioDocumentacao);
                tipoOperacao.TipoAgrupamentoEnvioDocumentacao = tipoAgrupamentoEnvioDocumentacao;
            }

            tipoOperacao.FormaEnvioDocumentacao = null;
            if (configuracaoFatura.FormaEnvioDocumentacao != null)
            {
                Enum.TryParse((string)configuracaoFatura.FormaEnvioDocumentacao, out formaEnvioDocumentacao);
                tipoOperacao.FormaEnvioDocumentacao = formaEnvioDocumentacao;
            }

            bool informarEmailEnvioDocumentacaoPorta = (bool)configuracaoFatura.InformarEmailEnvioDocumentacaoPorta;
            if (!string.IsNullOrWhiteSpace((string)configuracaoFatura.EmailEnvioDocumentacaoPorta) && informarEmailEnvioDocumentacaoPorta)
                tipoOperacao.EmailEnvioDocumentacaoPorta = (string)configuracaoFatura.EmailEnvioDocumentacaoPorta;
            else
                tipoOperacao.EmailEnvioDocumentacaoPorta = null;

            if (!string.IsNullOrWhiteSpace((string)configuracaoFatura.AssuntoDocumentacaoPorta))
                tipoOperacao.AssuntoDocumentacaoPorta = (string)configuracaoFatura.AssuntoDocumentacaoPorta;
            else
                tipoOperacao.AssuntoDocumentacaoPorta = null;

            if (!string.IsNullOrWhiteSpace((string)configuracaoFatura.CorpoEmailDocumentacaoPorta))
                tipoOperacao.CorpoEmailDocumentacaoPorta = (string)configuracaoFatura.CorpoEmailDocumentacaoPorta;
            else
                tipoOperacao.CorpoEmailDocumentacaoPorta = null;

            if (!string.IsNullOrWhiteSpace((string)configuracaoFatura.QuantidadeParcelasFaturamento))
                tipoOperacao.QuantidadeParcelasFaturamento = (string)configuracaoFatura.QuantidadeParcelasFaturamento;
            else
                tipoOperacao.QuantidadeParcelasFaturamento = null;

            tipoOperacao.TipoAgrupamentoEnvioDocumentacaoPorta = null;
            if (configuracaoFatura.TipoAgrupamentoEnvioDocumentacaoPorta != null)
            {
                Enum.TryParse((string)configuracaoFatura.TipoAgrupamentoEnvioDocumentacaoPorta, out tipoAgrupamentoEnvioDocumentacaoPorta);
                tipoOperacao.TipoAgrupamentoEnvioDocumentacaoPorta = tipoAgrupamentoEnvioDocumentacaoPorta;
            }

            tipoOperacao.FormaEnvioDocumentacaoPorta = null;
            if (configuracaoFatura.FormaEnvioDocumentacaoPorta != null)
            {
                Enum.TryParse((string)configuracaoFatura.FormaEnvioDocumentacaoPorta, out formaEnvioDocumentacaoPorta);
                tipoOperacao.FormaEnvioDocumentacaoPorta = formaEnvioDocumentacaoPorta;
            }

            if (configuracaoFatura.BoletoConfiguracao != null && !string.IsNullOrWhiteSpace((string)configuracaoFatura.BoletoConfiguracao))
            {
                int.TryParse((string)configuracaoFatura.BoletoConfiguracao, out int codigoBoletoConfiguracao);
                if (codigoBoletoConfiguracao > 0)
                    tipoOperacao.BoletoConfiguracao = repBoletoConfiguracao.BuscarPorCodigo(codigoBoletoConfiguracao);
                else
                    tipoOperacao.BoletoConfiguracao = null;
            }
            else
                tipoOperacao.BoletoConfiguracao = null;
            tipoOperacao.EnviarBoletoPorEmailAutomaticamente = (bool)configuracaoFatura.EnviarBoletoPorEmailAutomaticamente;
            tipoOperacao.EnviarDocumentacaoFaturamentoCTe = (bool)configuracaoFatura.EnviarDocumentacaoFaturamentoCTe;
            tipoOperacao.FormaPagamento = repTipoPagamentoRecebimento.BuscarPorCodigo((int)configuracaoFatura.FormaPagamento);
            tipoOperacao.GerarTituloPorDocumentoFiscal = (bool)configuracaoFatura.GerarTituloPorDocumentoFiscal;
            tipoOperacao.GerarTituloAutomaticamente = (bool)configuracaoFatura.GerarTituloAutomaticamente;
            tipoOperacao.GerarFaturaAutomaticaCte = (bool)configuracaoFatura.GerarFaturaAutomaticaCte;
            tipoOperacao.GerarFaturamentoAVista = (bool)configuracaoFatura.GerarFaturamentoAVista;
            tipoOperacao.GerarBoletoAutomaticamente = (bool)configuracaoFatura.GerarBoletoAutomaticamente;
            tipoOperacao.EnviarArquivosDescompactados = (bool)configuracaoFatura.EnviarArquivosDescompactados;
            tipoOperacao.NaoEnviarEmailFaturaAutomaticamente = (bool)configuracaoFatura.NaoEnviarEmailFaturaAutomaticamente;
            tipoOperacao.HabilitarPeriodoVencimentoEspecifico = ((string)configuracaoFatura.HabilitarPeriodoVencimentoEspecifico).ToBool();
            tipoOperacao.NaoValidarPossuiAcordoFaturamentoAvancoCarga = (bool)configuracaoFatura.NaoValidarPossuiAcordoFaturamentoAvancoCarga;

            if (configuracaoFatura.TipoEnvioFatura != null && (int)configuracaoFatura.TipoEnvioFatura > 0)
            {
                Enum.TryParse((string)configuracaoFatura.TipoEnvioFatura, out Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEnvioFatura tipoEnvioFatura);
                tipoOperacao.TipoEnvioFatura = tipoEnvioFatura;
            }
            else
                tipoOperacao.TipoEnvioFatura = null;

            if (configuracaoFatura.TipoAgrupamentoFatura != null && (int)configuracaoFatura.TipoAgrupamentoFatura > 0)
            {
                Enum.TryParse((string)configuracaoFatura.TipoAgrupamentoFatura, out Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAgrupamentoFatura tipoAgrupamentoFatura);
                tipoOperacao.TipoAgrupamentoFatura = tipoAgrupamentoFatura;
            }
            else
                tipoOperacao.TipoAgrupamentoFatura = null;


            if (configuracaoFatura.FormaTitulo != null && (int)configuracaoFatura.FormaTitulo > 0)
            {
                Enum.TryParse((string)configuracaoFatura.FormaTitulo, out Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaTitulo formaTitulo);
                tipoOperacao.FormaTitulo = formaTitulo;
            }
            else
                tipoOperacao.FormaTitulo = null;

            #endregion  preencher dados

            SalvarConfiguracaoFaturaVencimentos(tipoOperacaoParam, configuracaoFatura.FaturaVencimentos, unidadeDeTrabalho);

            if (tipoOperacao.Codigo == 0)
                repConfiguracaoTipoOperacaoFatura.Inserir(tipoOperacao);
            else
                repConfiguracaoTipoOperacaoFatura.Atualizar(tipoOperacao, Auditado, null);

            tipoOperacaoParam.ConfiguracaoTipoOperacaoFatura = tipoOperacao;
            repTipoOperacao.Atualizar(tipoOperacaoParam);
        }

        private void SalvarConfiguracaoFaturaVencimentos(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, dynamic dynConfiguracaoFaturaVencimentos, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (dynConfiguracaoFaturaVencimentos == null)
                return;

            Repositorio.Embarcador.Pedidos.TipoOperacaoFaturaVencimento repTipoOperacaoFaturaVencimento = new Repositorio.Embarcador.Pedidos.TipoOperacaoFaturaVencimento(unidadeDeTrabalho);
            List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoFaturaVencimento> tipoOperacaoFaturaVencimentos = repTipoOperacaoFaturaVencimento.BuscarPorTipoOperacao(tipoOperacao.Codigo);

            if (tipoOperacaoFaturaVencimentos.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (dynamic configuracaoFaturaVencimentos in dynConfiguracaoFaturaVencimentos)
                {
                    int codigo = ((string)configuracaoFaturaVencimentos.Codigo).ToInt();
                    if (codigo > 0)
                        codigos.Add(codigo);
                }

                List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoFaturaVencimento> deletar = (from obj in tipoOperacaoFaturaVencimentos where !codigos.Contains(obj.Codigo) select obj).ToList();

                for (int i = 0; i < deletar.Count; i++)
                    repTipoOperacaoFaturaVencimento.Deletar(deletar[i]);
            }

            foreach (dynamic configuracaoFaturaVencimentos in dynConfiguracaoFaturaVencimentos)
            {
                int codigo = ((string)configuracaoFaturaVencimentos.Codigo).ToInt();

                Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoFaturaVencimento tipoOperacaoFaturaVencimento = codigo > 0 ? repTipoOperacaoFaturaVencimento.BuscarPorCodigo(codigo, false) : null;

                if (tipoOperacaoFaturaVencimento == null)
                {
                    tipoOperacaoFaturaVencimento = new Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoFaturaVencimento();
                    tipoOperacaoFaturaVencimento.TipoOperacao = tipoOperacao;
                }

                tipoOperacaoFaturaVencimento.DiaInicial = ((string)configuracaoFaturaVencimentos.DiaInicial).ToInt();
                tipoOperacaoFaturaVencimento.DiaFinal = ((string)configuracaoFaturaVencimentos.DiaFinal).ToInt();
                tipoOperacaoFaturaVencimento.DiaVencimento = ((string)configuracaoFaturaVencimentos.DiaVencimento).ToInt();

                if (tipoOperacaoFaturaVencimento.Codigo > 0)
                    repTipoOperacaoFaturaVencimento.Atualizar(tipoOperacaoFaturaVencimento);
                else
                    repTipoOperacaoFaturaVencimento.Inserir(tipoOperacaoFaturaVencimento);
            }
        }

        private void SalvarConfiguracaoComponentesFrete(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, dynamic configuracaoComponentes, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (configuracaoComponentes != null)
            {
                Repositorio.Embarcador.Pedidos.TipoOperacaoConfiguracaoComponentes repTipoOperacaoConfiguracaoComponentes = new Repositorio.Embarcador.Pedidos.TipoOperacaoConfiguracaoComponentes(unidadeDeTrabalho);
                List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoConfiguracaoComponentes> tiposOperacaoesConfiguracoesComponentesExcluir = repTipoOperacaoConfiguracaoComponentes.BuscarPorTipoOperacao(tipoOperacao.Codigo);
                foreach (Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoConfiguracaoComponentes TipoOperacaoConfiguracaoComponentesExcluir in tiposOperacaoesConfiguracoesComponentesExcluir)
                {
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, tipoOperacao, null, Localization.Resources.Pedidos.TipoOperacao.ConfiguracaoDoComponente + TipoOperacaoConfiguracaoComponentesExcluir.Descricao + Localization.Resources.Pedidos.TipoOperacao.RemovidoDoGrupo, unidadeDeTrabalho);
                    repTipoOperacaoConfiguracaoComponentes.Deletar(TipoOperacaoConfiguracaoComponentesExcluir);
                }

                for (int i = 0; i < configuracaoComponentes.Count; i++)
                {
                    dynamic dynConfiguracaoComponentes = configuracaoComponentes[i];

                    Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoConfiguracaoComponentes tipoOperacaoConfiguracaoComponentes = new Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoConfiguracaoComponentes();

                    tipoOperacaoConfiguracaoComponentes.ComponenteFrete = new Dominio.Entidades.Embarcador.Frete.ComponenteFrete() { Codigo = (int)dynConfiguracaoComponentes.ComponenteFrete.Codigo };
                    tipoOperacaoConfiguracaoComponentes.TipoOperacao = tipoOperacao;
                    if ((int)dynConfiguracaoComponentes.ModeloDocumentoFiscal.Codigo > 0)
                        tipoOperacaoConfiguracaoComponentes.ModeloDocumentoFiscal = new Dominio.Entidades.ModeloDocumentoFiscal() { Codigo = (int)dynConfiguracaoComponentes.ModeloDocumentoFiscal.Codigo };

                    if ((int)dynConfiguracaoComponentes.FormulaRateioFrete.Codigo > 0)
                        tipoOperacaoConfiguracaoComponentes.RateioFormula = new Dominio.Entidades.Embarcador.Rateio.RateioFormula() { Codigo = (int)dynConfiguracaoComponentes.FormulaRateioFrete.Codigo };

                    tipoOperacaoConfiguracaoComponentes.OutraDescricaoCTe = (string)dynConfiguracaoComponentes.DescricaoCTe;
                    tipoOperacaoConfiguracaoComponentes.IncluirICMS = (bool)dynConfiguracaoComponentes.IncluirICMS;
                    tipoOperacaoConfiguracaoComponentes.IncluirIntegralmenteContratoFreteTerceiro = (bool)dynConfiguracaoComponentes.IncluirIntegralmenteContratoFreteTerceiro;

                    repTipoOperacaoConfiguracaoComponentes.Inserir(tipoOperacaoConfiguracaoComponentes);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, tipoOperacao, null, Localization.Resources.Pedidos.TipoOperacao.ConfiguracaoDoComponente + tipoOperacaoConfiguracaoComponentes.Descricao + Localization.Resources.Pedidos.TipoOperacao.AdicionadoAoTipoDeOperacao, unidadeDeTrabalho);
                }
            }
        }

        private void AtualizarConfiguracoesEmissao(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, dynamic configuracao, Dominio.Entidades.Auditoria.HistoricoObjeto historico, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (configuracao == null)
                return;

            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unidadeDeTrabalho);
            Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoEmissao repConfiguracaoTipoOperacaoEmissao = new Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoEmissao(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoEmissao configuracaoTipoOperacaoEmissao = tipoOperacao.ConfiguracaoEmissao ?? new Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoEmissao();

            if (configuracaoTipoOperacaoEmissao.Codigo > 0)
                configuracaoTipoOperacaoEmissao.Initialize();

            configuracaoTipoOperacaoEmissao.UtilizarPrimeiraUnidadeMedidaPesoCTeSubcontratacao = ((string)configuracao.UtilizarPrimeiraUnidadeMedidaPesoCTeSubcontratacao).ToBool();
            configuracaoTipoOperacaoEmissao.LiberarDocumentosEmitidosQuandoEntregaForConfirmada = ((string)configuracao.LiberarDocumentosEmitidosQuandoEntregaForConfirmada).ToBool();
            configuracaoTipoOperacaoEmissao.DisponibilizarComposicaoRateioCarga = ((string)configuracao.DisponibilizarComposicaoRateioCarga).ToBool();
            configuracaoTipoOperacaoEmissao.AverbarCTeImportadoDoEmbarcador = ((string)configuracao.AverbarCTeImportadoDoEmbarcador).ToBool();
            configuracaoTipoOperacaoEmissao.TipoUsoFatorCubagemRateioFormula = ((string)configuracao.TipoUsoFatorCubagemRateioFormula).ToNullableEnum<TipoUsoFatorCubagem>();
            configuracaoTipoOperacaoEmissao.FatorCubagemRateioFormula = ((string)configuracao.FatorCubagemRateioFormula).ToDecimal();
            configuracaoTipoOperacaoEmissao.TipoReceita = ((string)configuracao.TipoReceita).ToNullableEnum<TipoReceita>();

            if (configuracaoTipoOperacaoEmissao.Codigo == 0)
                repConfiguracaoTipoOperacaoEmissao.Inserir(configuracaoTipoOperacaoEmissao);
            else
                repConfiguracaoTipoOperacaoEmissao.Atualizar(configuracaoTipoOperacaoEmissao, Auditado, historico);

            tipoOperacao.ConfiguracaoEmissao = configuracaoTipoOperacaoEmissao;

            repTipoOperacao.Atualizar(tipoOperacao);
        }

        private void SalvarLayoutsEDI(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.LayoutEDI repLayoutEDI = new Repositorio.LayoutEDI(unidadeDeTrabalho);
            Repositorio.Embarcador.Pedidos.TipoOperacaoLayoutEDI repTipoOperacaoLayoutEDI = new Repositorio.Embarcador.Pedidos.TipoOperacaoLayoutEDI(unidadeDeTrabalho);
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unidadeDeTrabalho);

            dynamic layoutsEDI = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ConfiguracaoLayoutEDI"));
            List<int> codigosExistentes = new List<int>();
            int codigo = 0;

            for (int i = 0; i < layoutsEDI.Count; i++)
                if (int.TryParse((string)layoutsEDI[i].Codigo, out codigo))
                    codigosExistentes.Add(codigo);

            List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoLayoutEDI> layoutsExistentes = repTipoOperacaoLayoutEDI.BuscarPorTipoOperacao(tipoOperacao.Codigo);

            List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoLayoutEDI> layoutsDeletar = (from obj in layoutsExistentes where !codigosExistentes.Contains(obj.Codigo) select obj).ToList();

            foreach (Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoLayoutEDI layoutDeletar in layoutsDeletar)
            {
                layoutsExistentes.Remove(layoutDeletar);
                repTipoOperacaoLayoutEDI.Deletar(layoutDeletar);
            }

            for (int i = 0; i < layoutsEDI.Count; i++)
            {
                Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoLayoutEDI tipoOperacaoLayoutEDI = null;

                if (int.TryParse((string)layoutsEDI[i].Codigo, out codigo))
                    tipoOperacaoLayoutEDI = (from obj in layoutsExistentes where obj.Codigo == codigo select obj).FirstOrDefault();

                if (tipoOperacaoLayoutEDI == null)
                    tipoOperacaoLayoutEDI = new Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoLayoutEDI();

                tipoOperacaoLayoutEDI.TipoOperacao = tipoOperacao;
                tipoOperacaoLayoutEDI.LayoutEDI = repLayoutEDI.Buscar((int)layoutsEDI[i].CodigoLayoutEDI);
                tipoOperacaoLayoutEDI.TipoIntegracao = repTipoIntegracao.BuscarPorTipo((TipoIntegracao)(int)layoutsEDI[i].TipoIntegracao);

                tipoOperacaoLayoutEDI.EnderecoFTP = null;
                tipoOperacaoLayoutEDI.Diretorio = null;
                tipoOperacaoLayoutEDI.Passivo = false;
                tipoOperacaoLayoutEDI.UtilizarSFTP = false;
                tipoOperacaoLayoutEDI.CriarComNomeTemporaraio = false;
                tipoOperacaoLayoutEDI.SSL = false;
                tipoOperacaoLayoutEDI.Porta = null;
                tipoOperacaoLayoutEDI.Senha = null;
                tipoOperacaoLayoutEDI.Usuario = null;
                tipoOperacaoLayoutEDI.Emails = null;

                if (tipoOperacaoLayoutEDI.TipoIntegracao.Tipo == TipoIntegracao.FTP)
                {
                    tipoOperacaoLayoutEDI.EnderecoFTP = (string)layoutsEDI[i].EnderecoFTP;
                    tipoOperacaoLayoutEDI.Diretorio = (string)layoutsEDI[i].Diretorio;
                    tipoOperacaoLayoutEDI.Passivo = (bool)layoutsEDI[i].Passivo;
                    tipoOperacaoLayoutEDI.UtilizarSFTP = (bool)layoutsEDI[i].UtilizarSFTP;
                    tipoOperacaoLayoutEDI.CriarComNomeTemporaraio = (bool)layoutsEDI[i].CriarComNomeTemporaraio;
                    tipoOperacaoLayoutEDI.SSL = (bool)layoutsEDI[i].SSL;
                    tipoOperacaoLayoutEDI.Porta = (string)layoutsEDI[i].Porta;
                    tipoOperacaoLayoutEDI.Senha = (string)layoutsEDI[i].Senha;
                    tipoOperacaoLayoutEDI.Usuario = (string)layoutsEDI[i].Usuario;
                }
                else if (tipoOperacaoLayoutEDI.TipoIntegracao.Tipo == TipoIntegracao.Email)
                {
                    tipoOperacaoLayoutEDI.Emails = (string)layoutsEDI[i].Emails;
                }

                if (tipoOperacaoLayoutEDI.Codigo > 0)
                {
                    tipoOperacaoLayoutEDI.Initialize();
                    repTipoOperacaoLayoutEDI.Atualizar(tipoOperacaoLayoutEDI, Auditado);
                    List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoesLayout = tipoOperacaoLayoutEDI.GetChanges();
                    if (alteracoesLayout.Count > 0)
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, tipoOperacao, alteracoesLayout, Localization.Resources.Pedidos.TipoOperacao.AlterouLayout + tipoOperacaoLayoutEDI.Descricao + ".", unidadeDeTrabalho);
                }
                else
                {
                    repTipoOperacaoLayoutEDI.Inserir(tipoOperacaoLayoutEDI, Auditado);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, tipoOperacao, null, Localization.Resources.Pedidos.TipoOperacao.AdicionouLayout + tipoOperacaoLayoutEDI.Descricao + ".", unidadeDeTrabalho);
                }
            }
        }

        private void SalvarTipoComprovantes(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Cargas.ComprovanteCarga.TipoComprovante repTipoComprovante = new Repositorio.Embarcador.Cargas.ComprovanteCarga.TipoComprovante(unidadeDeTrabalho);
            dynamic tipoComprovantes = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Comprovantes"));

            if (tipoOperacao.TiposComprovante == null)
                tipoOperacao.TiposComprovante = new List<Dominio.Entidades.Embarcador.Cargas.ComprovanteCarga.TipoComprovante>();
            else
            {
                List<int> codigos = new List<int>();

                foreach (dynamic tipoComprovante in tipoComprovantes)
                    codigos.Add((int)tipoComprovante.Tipo.Codigo);

                List<Dominio.Entidades.Embarcador.Cargas.ComprovanteCarga.TipoComprovante> tipoComprovantesDeletar = tipoOperacao.TiposComprovante.Where(o => !codigos.Contains(o.Codigo)).ToList();

                foreach (Dominio.Entidades.Embarcador.Cargas.ComprovanteCarga.TipoComprovante tipoComprovanteDeletar in tipoComprovantesDeletar)
                    tipoOperacao.TiposComprovante.Remove(tipoComprovanteDeletar);
            }

            foreach (dynamic tipoComprovante in tipoComprovantes)
            {
                if (tipoOperacao.TiposComprovante.Any(o => o.Codigo == (int)tipoComprovante.Tipo.Codigo))
                    continue;

                Dominio.Entidades.Embarcador.Cargas.ComprovanteCarga.TipoComprovante tipoComprovanteObj = repTipoComprovante.BuscarPorCodigo((int)tipoComprovante.Tipo.Codigo);
                tipoOperacao.TiposComprovante.Add(tipoComprovanteObj);
            }
        }

        private void SetarDadosPagbem(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao)
        {
            tipoOperacao.DataEntregaPagbem = Request.GetNullableBoolParam("DataEntregaPagbem");
            tipoOperacao.PesoPagbem = Request.GetNullableBoolParam("PesoPagbem");
            tipoOperacao.TicketBalancaPagbem = Request.GetNullableBoolParam("TicketBalancaPagbem");
            tipoOperacao.AvariaPagbem = Request.GetNullableBoolParam("AvariaPagbem");
            tipoOperacao.CanhotoNFePagbem = Request.GetNullableBoolParam("CanhotoNFePagbem");
            tipoOperacao.ComprovantePedagioPagbem = Request.GetNullableBoolParam("ComprovantePedagioPagbem");
            tipoOperacao.DACTEPagbem = Request.GetNullableBoolParam("DACTEPagbem");
            tipoOperacao.ContratoTransportePagbem = Request.GetNullableBoolParam("ContratoTransportePagbem");
            tipoOperacao.DataDesembarquePagbem = Request.GetNullableBoolParam("DataDesembarquePagbem");
            tipoOperacao.RelatorioInspecaoDesembarquePagbem = Request.GetNullableBoolParam("RelatorioInspecaoDesembarquePagbem");
            tipoOperacao.FreteTipoPesoPagBem = Request.GetStringParam("FreteTipoPesoPagBem");
        }

        private TimeSpan RetornarTimeSpan(string strTempo)
        {
            if (strTempo != string.Empty)
            {
                string[] HrMin = strTempo.Split(':');
                double hr = HrMin[0].ToDouble();
                double min = HrMin[1].ToDouble();
                TimeSpan tempo = TimeSpan.FromHours(hr) + TimeSpan.FromMinutes(min);

                return tempo;
            }
            else
                return TimeSpan.Zero;
        }

        private dynamic ObterExcecoesAngelLira(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, Repositorio.UnitOfWork unidadeDeTrabalho, bool duplicar)
        {
            Repositorio.Embarcador.Pedidos.TipoOperacaoExcecaoAngelLira repositorioExcecaoAngelLira = new Repositorio.Embarcador.Pedidos.TipoOperacaoExcecaoAngelLira(unidadeDeTrabalho);

            List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoExcecaoAngelLira> listaExcecoes = repositorioExcecaoAngelLira.BuscarPorTipoOperacao(tipoOperacao.Codigo);

            return (from obj in listaExcecoes
                    select new
                    {
                        Codigo = duplicar ? 0 : obj.Codigo,
                        CodigoDestino = obj.Destino?.Sigla ?? "",
                        Destino = obj.Destino?.Descricao ?? "",
                        ValorMinimo = obj.ValorMinimo.ToString("n2"),
                        ProcedimentoEmbarque = obj.ProcedimentoEmbarque
                    }).ToList();
        }

        private void SalvarExcecoesAngelLira(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.TipoOperacaoExcecaoAngelLira repositorioExcecaoAngelLira = new Repositorio.Embarcador.Pedidos.TipoOperacaoExcecaoAngelLira(unitOfWork);
            Repositorio.Estado repositorioEstado = new Repositorio.Estado(unitOfWork);

            List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoExcecaoAngelLira> excecoesDeletar = repositorioExcecaoAngelLira.BuscarPorTipoOperacao(tipoOperacao.Codigo);

            foreach (Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoExcecaoAngelLira excecaoDeletar in excecoesDeletar)
                repositorioExcecaoAngelLira.Deletar(excecaoDeletar);

            dynamic dynAngelLiraExcecoes = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("AngelLiraExcecoes"));

            foreach (dynamic excecao in dynAngelLiraExcecoes)
            {
                string ufDestino = ((string)excecao.CodigoDestino).ToString();
                decimal valorMinimo = ((string)excecao.ValorMinimo).ToDecimal();
                int procedimentoEmbarque = ((string)excecao.ProcedimentoEmbarque).ToInt();

                Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoExcecaoAngelLira excecaoAdicionar = new Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoExcecaoAngelLira()
                {
                    ProcedimentoEmbarque = procedimentoEmbarque,
                    ValorMinimo = valorMinimo,
                    TipoOperacao = tipoOperacao,
                    Destino = !string.IsNullOrWhiteSpace(ufDestino) ? repositorioEstado.BuscarPorSigla(ufDestino) : null
                };

                repositorioExcecaoAngelLira.Inserir(excecaoAdicionar);
            }
        }

        private void AtualizarConfiguracaoChamado(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, Dominio.Entidades.Auditoria.HistoricoObjeto historico, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.TipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
            Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoChamado repositorioConfiguracaoTipoOperacaoChamado = new Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoChamado(unitOfWork);
            Repositorio.Embarcador.Chamados.MotivoChamado repositorioMotivoChamado = new Repositorio.Embarcador.Chamados.MotivoChamado(unitOfWork);

            Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoChamado configuracaoTipoOperacaoChamado = tipoOperacao.ConfiguracaoTipoOperacaoChamado ?? new Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoChamado();

            if (configuracaoTipoOperacaoChamado.Codigo > 0)
                configuracaoTipoOperacaoChamado.Initialize();

            configuracaoTipoOperacaoChamado.NaoPermitirGerarAtendimento = Request.GetBoolParam("NaoPermitirGerarAtendimento");
            configuracaoTipoOperacaoChamado.PermitirSelecionarApenasAlgunsMotivosAtendimento = Request.GetBoolParam("PermitirSelecionarApenasAlgunsMotivosAtendimento");
            configuracaoTipoOperacaoChamado.NaoValidarRetornoGeradoParaFinalizacaoAtendimento = Request.GetBoolParam("NaoValidarRetornoGeradoParaFinalizacaoAtendimento");
            configuracaoTipoOperacaoChamado.FinalizarAutomaticamenteAtendimentoNfeEntregue = Request.GetBoolParam("FinalizarAutomaticamenteAtendimentoNfeEntregue");


            AdicionarConfiguracaoMotivoChamado(configuracaoTipoOperacaoChamado, unitOfWork);
            AdicionarConfiguracaoChamadoTransportador(configuracaoTipoOperacaoChamado, unitOfWork);

            if (configuracaoTipoOperacaoChamado.Codigo == 0)
                repositorioConfiguracaoTipoOperacaoChamado.Inserir(configuracaoTipoOperacaoChamado);
            else
                repositorioConfiguracaoTipoOperacaoChamado.Atualizar(configuracaoTipoOperacaoChamado, Auditado, historico);

            tipoOperacao.ConfiguracaoTipoOperacaoChamado = configuracaoTipoOperacaoChamado;
            repositorioTipoOperacao.Atualizar(tipoOperacao);
        }

        private void SalvarTiposCargas(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.TipoOperacaoTipoCarga repositorioTipoOperacaoTipoCarga = new Repositorio.Embarcador.Pedidos.TipoOperacaoTipoCarga(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoDeCarga repositorioTipoCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);

            dynamic dynTiposCargas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("TiposCargas"));

            if (dynTiposCargas == null)
                return;

            List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoTipoCarga> tiposCarga = repositorioTipoOperacaoTipoCarga.BuscarPorTipoOperacao(tipoOperacao.Codigo);
            List<Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade> alteracoes = new List<Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade>();

            if (tiposCarga.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (dynamic tipoCarga in dynTiposCargas)
                {
                    int codigo = ((string)tipoCarga.Codigo).ToInt();
                    if (codigo > 0)
                        codigos.Add(codigo);
                }

                List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoTipoCarga> listaDeletar = (from obj in tiposCarga where !codigos.Contains(obj.TipoCarga.Codigo) select obj).ToList();

                foreach (Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoTipoCarga deletar in listaDeletar)
                {
                    alteracoes.Add(new Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade()
                    {
                        Propriedade = "Tipo de Carga",
                        De = $"{deletar.TipoCarga.Descricao}",
                        Para = ""
                    });

                    repositorioTipoOperacaoTipoCarga.Deletar(deletar);
                }
            }

            foreach (dynamic tipoCarga in dynTiposCargas)
            {
                int codigo = ((string)tipoCarga.Codigo).ToInt();

                Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoTipoCarga tipoOperacaoTipoCarga = codigo > 0 ? repositorioTipoOperacaoTipoCarga.BuscarPorTipoOperacaoETipoCarga(tipoOperacao.Codigo, codigo) : null;

                if (tipoOperacaoTipoCarga == null)
                    tipoOperacaoTipoCarga = new Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoTipoCarga();

                int codigoTipoCarga = ((string)tipoCarga.Codigo).ToInt();

                tipoOperacaoTipoCarga.TipoOperacao = tipoOperacao;
                tipoOperacaoTipoCarga.TipoCarga = repositorioTipoCarga.BuscarPorCodigo(codigoTipoCarga);

                alteracoes.Add(new Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade()
                {
                    Propriedade = "Tipo de Carga",
                    De = "",
                    Para = $"{tipoOperacaoTipoCarga.TipoCarga.Descricao}"
                });

                if (tipoOperacaoTipoCarga.Codigo > 0)
                    repositorioTipoOperacaoTipoCarga.Atualizar(tipoOperacaoTipoCarga);
                else
                    repositorioTipoOperacaoTipoCarga.Inserir(tipoOperacaoTipoCarga);

            }

            tipoOperacao.SetExternalChanges(alteracoes);
        }

        private void SalvarTiposCargasEmissao(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.TipoOperacaoTipoCargaEmissao repositorioTipoOperacaoTipoCargaEmissao = new Repositorio.Embarcador.Pedidos.TipoOperacaoTipoCargaEmissao(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoDeCarga repositorioTipoCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);

            dynamic dynTiposCargasEmissao = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("TiposCargasEmissao"));

            if (dynTiposCargasEmissao == null)
                return;

            List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoTipoCargaEmissao> tiposCarga = repositorioTipoOperacaoTipoCargaEmissao.BuscarPorTipoOperacao(tipoOperacao);
            List<Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade> alteracoes = new List<Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade>();

            if (tiposCarga.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (dynamic tipoCarga in dynTiposCargasEmissao)
                {
                    int codigo = ((string)tipoCarga.Codigo).ToInt();
                    if (codigo > 0)
                        codigos.Add(codigo);
                }

                List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoTipoCargaEmissao> listaDeletar = tiposCarga.Where(obj => !codigos.Contains(obj.TipoCarga.Codigo)).ToList();

                foreach (Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoTipoCargaEmissao deletar in listaDeletar)
                {
                    alteracoes.Add(new Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade()
                    {
                        Propriedade = "Tipo de Carga",
                        De = $"{deletar.TipoCarga.Descricao}",
                        Para = ""
                    });

                    repositorioTipoOperacaoTipoCargaEmissao.Deletar(deletar);
                }
            }

            foreach (dynamic tipoCarga in dynTiposCargasEmissao)
            {
                int codigoTipoCarga = ((string)tipoCarga.Codigo).ToInt();

                Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoDeCarga = repositorioTipoCarga.BuscarPorCodigo(codigoTipoCarga);
                Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoTipoCargaEmissao tipoOperacaoTipoCargaEmissao = repositorioTipoOperacaoTipoCargaEmissao.BuscarPorTipoOperacaoETipoCarga(tipoOperacao, tipoDeCarga);

                if (tipoOperacaoTipoCargaEmissao == null)
                    tipoOperacaoTipoCargaEmissao = new Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoTipoCargaEmissao();

                tipoOperacaoTipoCargaEmissao.TipoOperacao = tipoOperacao;
                tipoOperacaoTipoCargaEmissao.TipoCarga = tipoDeCarga;

                alteracoes.Add(new Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade()
                {
                    Propriedade = "Tipo de Carga",
                    De = "",
                    Para = $"{tipoOperacaoTipoCargaEmissao.TipoCarga.Descricao}"
                });

                if (tipoOperacaoTipoCargaEmissao.Codigo > 0)
                    repositorioTipoOperacaoTipoCargaEmissao.Atualizar(tipoOperacaoTipoCargaEmissao);
                else
                    repositorioTipoOperacaoTipoCargaEmissao.Inserir(tipoOperacaoTipoCargaEmissao);
            }

            tipoOperacao.SetExternalChanges(alteracoes);
        }

        private void SalvarConfiguracaoVendedores(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacaoParam, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.TipoOperacaoVendedores repTipoOperacaoVendedores = new Repositorio.Embarcador.Pedidos.TipoOperacaoVendedores(unitOfWork);
            Repositorio.Usuario repFuncionario = new Repositorio.Usuario(unitOfWork);

            repTipoOperacaoVendedores.DeletarPorTipoOperacao(tipoOperacaoParam.Codigo);
            dynamic dynVendedores = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("Vendedores"));

            if (dynVendedores.Count > 0)
            {
                foreach (dynamic dynVendedor in dynVendedores)
                {
                    Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoVendedores vendedor = new Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoVendedores();

                    int codigoFuncionario = int.Parse((string)dynVendedor.Funcionario.Codigo);
                    decimal percentualComissao;
                    decimal.TryParse((string)dynVendedor.PercentualComissao, out percentualComissao);
                    DateTime.TryParse((string)dynVendedor.DataInicioVigencia, out DateTime dataInicioVigencia);
                    DateTime.TryParse((string)dynVendedor.DataFimVigencia, out DateTime dataFimVigencia);

                    vendedor.PercentualComissao = percentualComissao;
                    if (dataInicioVigencia > DateTime.MinValue)
                        vendedor.DataInicioVigencia = dataInicioVigencia;
                    if (dataFimVigencia > DateTime.MinValue)
                        vendedor.DataFimVigencia = dataFimVigencia;
                    if (codigoFuncionario > 0)
                        vendedor.Funcionario = repFuncionario.BuscarPorCodigo(codigoFuncionario);
                    vendedor.TipoOperacao = tipoOperacaoParam;

                    repTipoOperacaoVendedores.Inserir(vendedor);
                }
            }
        }

        private void AdicionarConfiguracaoMotivoChamado(Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoChamado configuracaoTipoOperacaoChamado, Repositorio.UnitOfWork unitOfWork)
        {
            dynamic dynMotivosChamados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ConfiguracaoMotivosChamados"));
            Repositorio.Embarcador.Chamados.MotivoChamado repositorioMotivoChamado = new Repositorio.Embarcador.Chamados.MotivoChamado(unitOfWork);

            List<Dominio.Entidades.Embarcador.Chamados.MotivoChamado> motivosChamados = new List<Dominio.Entidades.Embarcador.Chamados.MotivoChamado>();
            if (configuracaoTipoOperacaoChamado?.MotivosChamados?.Count > 0)
                motivosChamados = configuracaoTipoOperacaoChamado.MotivosChamados.ToList();

            List<int> codigosMotivosChamados = new List<int>();
            foreach (dynamic motivoChamado in dynMotivosChamados)
            {
                int codigoMotivoChamado = ((string)motivoChamado.Codigo).ToInt();
                if (codigoMotivoChamado > 0)
                    codigosMotivosChamados.Add(codigoMotivoChamado);
            }

            if (motivosChamados.Count > 0)
            {
                List<Dominio.Entidades.Embarcador.Chamados.MotivoChamado> motivosChamadosRemover = motivosChamados.Where(o => !codigosMotivosChamados.Contains(o.Codigo)).ToList();

                foreach (Dominio.Entidades.Embarcador.Chamados.MotivoChamado motivoChamado in motivosChamadosRemover)
                {
                    configuracaoTipoOperacaoChamado.MotivosChamados.Remove(motivoChamado);
                    motivosChamados.Remove(motivoChamado);
                }
            }

            List<int> motivosChamadoAdicionar = new List<int>();
            motivosChamadoAdicionar = codigosMotivosChamados.Where(o => !motivosChamados.Any(m => m.Codigo == o)).ToList();

            if (configuracaoTipoOperacaoChamado.MotivosChamados == null)
                configuracaoTipoOperacaoChamado.MotivosChamados = new List<Dominio.Entidades.Embarcador.Chamados.MotivoChamado>();

            foreach (int codigoMotivoChamado in motivosChamadoAdicionar)
            {
                Dominio.Entidades.Embarcador.Chamados.MotivoChamado motivoChamado = repositorioMotivoChamado.BuscarPorCodigo(codigoMotivoChamado);

                if (motivoChamado != null)
                    configuracaoTipoOperacaoChamado.MotivosChamados.Add(motivoChamado);
            }
        }

        private void AdicionarConfiguracaoChamadoTransportador(Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoChamado configuracaoTipoOperacaoChamado, Repositorio.UnitOfWork unitOfWork)
        {
            dynamic dynChamadoTransportador = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ConfiguracaoChamadoTransportadores"));
            Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(unitOfWork);

            List<Dominio.Entidades.Empresa> transportadores = new List<Dominio.Entidades.Empresa>();
            if (configuracaoTipoOperacaoChamado?.Transportadores?.Count > 0)
                transportadores = configuracaoTipoOperacaoChamado.Transportadores.ToList();

            List<int> codigosTransportadores = new List<int>();
            foreach (dynamic dynTransportador in dynChamadoTransportador)
            {
                int codigoTransportador = ((string)dynTransportador.Codigo).ToInt();
                if (codigoTransportador > 0)
                    codigosTransportadores.Add(codigoTransportador);
            }

            if (transportadores.Count > 0)
            {
                List<Dominio.Entidades.Empresa> transportadoresRemover = transportadores.Where(o => !codigosTransportadores.Contains(o.Codigo)).ToList();

                foreach (Dominio.Entidades.Empresa transportadorRemover in transportadoresRemover)
                {
                    configuracaoTipoOperacaoChamado.Transportadores.Remove(transportadorRemover);
                    transportadores.Remove(transportadorRemover);
                }
            }

            List<int> transportadoresAdicionar = new List<int>();
            transportadoresAdicionar = codigosTransportadores.Where(o => !transportadores.Any(m => m.Codigo == o)).ToList();

            if (configuracaoTipoOperacaoChamado.Transportadores == null)
                configuracaoTipoOperacaoChamado.Transportadores = new List<Dominio.Entidades.Empresa>();

            foreach (int codigoMotivoChamado in transportadoresAdicionar)
            {
                Dominio.Entidades.Empresa transportador = repositorioEmpresa.BuscarPorCodigo(codigoMotivoChamado);

                if (transportador != null)
                    configuracaoTipoOperacaoChamado.Transportadores.Add(transportador);
            }
        }

        private void SalvarConfiguracaoPesoConsideradoCarga(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacaoParam, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoPesoConsideradoCarga repConfiguracaoTipoOperacaoPesoConsideradoCarga = new Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoPesoConsideradoCarga(unitOfWork);
            Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoPesoConsideradoCarga configuracaoTipoOperacaoPesoConsideradoCarga;

            EnumPesoConsideradoCarga PesoConsideradoNaCarga = Request.GetEnumParam<EnumPesoConsideradoCarga>("PesoConsideradoNaCarga");
            configuracaoTipoOperacaoPesoConsideradoCarga = repConfiguracaoTipoOperacaoPesoConsideradoCarga.BuscarPorTipoOperacao(tipoOperacaoParam);

            configuracaoTipoOperacaoPesoConsideradoCarga ??= new Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoPesoConsideradoCarga();

            configuracaoTipoOperacaoPesoConsideradoCarga.TipoOperacao = tipoOperacaoParam;
            configuracaoTipoOperacaoPesoConsideradoCarga.PesoConsideradoNaCarga = PesoConsideradoNaCarga;

            if (configuracaoTipoOperacaoPesoConsideradoCarga.Codigo > 0)
                repConfiguracaoTipoOperacaoPesoConsideradoCarga.Atualizar(configuracaoTipoOperacaoPesoConsideradoCarga);
            else
                repConfiguracaoTipoOperacaoPesoConsideradoCarga.Inserir(configuracaoTipoOperacaoPesoConsideradoCarga);
        }

        private async Task SalvarCodigosIntegracaoAsync(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.Pedidos.CodigoIntegracaoGerenciadoraRisco repositorioCodigoIntegracaoGerenciadoraRisco = new Repositorio.Embarcador.Pedidos.CodigoIntegracaoGerenciadoraRisco(unitOfWork, cancellationToken);

            List<Dominio.Entidades.Embarcador.Pedidos.CodigoIntegracaoGerenciadoraRisco> listaCodigosIntegracao = await repositorioCodigoIntegracaoGerenciadoraRisco.BuscarPorTipoOperacaoAsync(tipoOperacao.Codigo, cancellationToken);
            dynamic[] codigosIntegracaoParametros = Request.GetArrayParam<dynamic>("CodigosIntegracao");

            foreach (dynamic codigoIntegracaoParametro in codigosIntegracaoParametros)
            {
                int codigo = ((string)codigoIntegracaoParametro.Codigo).ToInt();
                Dominio.Entidades.Embarcador.Pedidos.CodigoIntegracaoGerenciadoraRisco codigoIntegracao = null;

                if (codigo > 0)
                    codigoIntegracao = listaCodigosIntegracao.First(o => o.Codigo == codigo);

                if (codigoIntegracao != null && listaCodigosIntegracao.Contains(codigoIntegracao))
                {
                    listaCodigosIntegracao.Remove(codigoIntegracao);
                    continue;
                }

                Dominio.Entidades.Embarcador.Pedidos.CodigoIntegracaoGerenciadoraRisco codigoIntegracaoGerenciadoraRisco = new Dominio.Entidades.Embarcador.Pedidos.CodigoIntegracaoGerenciadoraRisco()
                {
                    TipoOperacao = tipoOperacao,
                    CodigoIntegracao = codigoIntegracaoParametro.CodigoIntegracao,
                    EtapaCarga = codigoIntegracaoParametro.EtapaCarga
                };

                await repositorioCodigoIntegracaoGerenciadoraRisco.InserirAsync(codigoIntegracaoGerenciadoraRisco);
                await Servicos.Auditoria.Auditoria.AuditarAsync(Auditado, tipoOperacao, $"Adicionou o código de integração {(string)codigoIntegracaoParametro.CodigoIntegracao}.", unitOfWork, cancellationToken);
            }

            foreach (Dominio.Entidades.Embarcador.Pedidos.CodigoIntegracaoGerenciadoraRisco codigoIntegracaoARemover in listaCodigosIntegracao)
            {
                await repositorioCodigoIntegracaoGerenciadoraRisco.DeletarAsync(codigoIntegracaoARemover);
                await Servicos.Auditoria.Auditoria.AuditarAsync(Auditado, tipoOperacao, $"Removeu o código de integração {codigoIntegracaoARemover.CodigoIntegracao}.", unitOfWork, cancellationToken);
            }
        }

        #endregion

        #region Métodos Privados - Gatilhos para Gera��o Automatica de Ocorr�ncia de Coleta/Entrega

        private void AtualizarGatilhosGeracaoAutomaticaPedidoOcorrenciaColetaEntrega(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, Dominio.Entidades.Auditoria.HistoricoObjeto historico, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.GatilhoGeracaoAutomaticaPedidoOcorrenciaColetaEntrega repositorioGatilhoGeracaoAutomaticaPedidoOcorrenciaColetaEntrega = new Repositorio.Embarcador.Pedidos.GatilhoGeracaoAutomaticaPedidoOcorrenciaColetaEntrega(unitOfWork);
            List<Dominio.Entidades.Embarcador.Pedidos.GatilhoGeracaoAutomaticaPedidoOcorrenciaColetaEntrega> gatilhosGeracaoAutomaticaPedidoOcorrenciaColetaEntrega = repositorioGatilhoGeracaoAutomaticaPedidoOcorrenciaColetaEntrega.BuscarPorTipoOperacao(tipoOperacao.Codigo);
            dynamic gatilhosGeracaoAutomaticaPedidoOcorrenciaColetaEntregaAdicionadosOuAtualizados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ListaGatilhoGeracaoAutomaticaPedidoOcorrenciaColetaEntrega"));

            ExcluirGatilhosGeracaoAutomaticaPedidoOcorrenciaColetaEntregaRemovidos(tipoOperacao, gatilhosGeracaoAutomaticaPedidoOcorrenciaColetaEntrega, gatilhosGeracaoAutomaticaPedidoOcorrenciaColetaEntregaAdicionadosOuAtualizados, historico, unitOfWork);
            SalvarGatilhosGeracaoAutomaticaPedidoOcorrenciaColetaEntregaAdicionadosOuAtualizados(tipoOperacao, gatilhosGeracaoAutomaticaPedidoOcorrenciaColetaEntrega, gatilhosGeracaoAutomaticaPedidoOcorrenciaColetaEntregaAdicionadosOuAtualizados, historico, unitOfWork);
        }

        private void ExcluirGatilhosGeracaoAutomaticaPedidoOcorrenciaColetaEntregaRemovidos(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, List<Dominio.Entidades.Embarcador.Pedidos.GatilhoGeracaoAutomaticaPedidoOcorrenciaColetaEntrega> gatilhosGeracaoAutomaticaPedidoOcorrenciaColetaEntrega, dynamic gatilhosGeracaoAutomaticaPedidoOcorrenciaColetaEntregaAdicionadosOuAtualizados, Dominio.Entidades.Auditoria.HistoricoObjeto historico, Repositorio.UnitOfWork unitOfWork)
        {
            if ((gatilhosGeracaoAutomaticaPedidoOcorrenciaColetaEntrega == null) || (gatilhosGeracaoAutomaticaPedidoOcorrenciaColetaEntrega.Count == 0))
                return;

            Repositorio.Embarcador.Pedidos.GatilhoGeracaoAutomaticaPedidoOcorrenciaColetaEntrega repositorioGatilhoGeracaoAutomaticaPedidoOcorrenciaColetaEntrega = new Repositorio.Embarcador.Pedidos.GatilhoGeracaoAutomaticaPedidoOcorrenciaColetaEntrega(unitOfWork);
            List<int> listaCodigosAtualizados = new List<int>();

            foreach (dynamic gatilhoGeracaoAutomaticaPedidoOcorrenciaColetaEntrega in gatilhosGeracaoAutomaticaPedidoOcorrenciaColetaEntregaAdicionadosOuAtualizados)
            {
                int? codigo = ((string)gatilhoGeracaoAutomaticaPedidoOcorrenciaColetaEntrega.Codigo).ToNullableInt();

                if (codigo.HasValue)
                    listaCodigosAtualizados.Add(codigo.Value);
            }

            List<Dominio.Entidades.Embarcador.Pedidos.GatilhoGeracaoAutomaticaPedidoOcorrenciaColetaEntrega> listaGatilhoGeracaoAutomaticaPedidoOcorrenciaColetaEntregaRemover = (from o in gatilhosGeracaoAutomaticaPedidoOcorrenciaColetaEntrega where !listaCodigosAtualizados.Contains(o.Codigo) select o).ToList();

            foreach (Dominio.Entidades.Embarcador.Pedidos.GatilhoGeracaoAutomaticaPedidoOcorrenciaColetaEntrega gatilhoGeracaoAutomaticaPedidoOcorrenciaColetaEntrega in listaGatilhoGeracaoAutomaticaPedidoOcorrenciaColetaEntregaRemover)
                repositorioGatilhoGeracaoAutomaticaPedidoOcorrenciaColetaEntrega.Deletar(gatilhoGeracaoAutomaticaPedidoOcorrenciaColetaEntrega, (historico != null ? Auditado : null), historico);
        }

        private void SalvarGatilhosGeracaoAutomaticaPedidoOcorrenciaColetaEntregaAdicionadosOuAtualizados(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, List<Dominio.Entidades.Embarcador.Pedidos.GatilhoGeracaoAutomaticaPedidoOcorrenciaColetaEntrega> gatilhosGeracaoAutomaticaPedidoOcorrenciaColetaEntrega, dynamic gatilhosGeracaoAutomaticaPedidoOcorrenciaColetaEntregaAdicionadosOuAtualizados, Dominio.Entidades.Auditoria.HistoricoObjeto historico, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.GatilhoGeracaoAutomaticaPedidoOcorrenciaColetaEntrega repositorioGatilhoGeracaoAutomaticaPedidoOcorrenciaColetaEntrega = new Repositorio.Embarcador.Pedidos.GatilhoGeracaoAutomaticaPedidoOcorrenciaColetaEntrega(unitOfWork);
            Repositorio.TipoDeOcorrenciaDeCTe repositorioTipoOcorrencia = new Repositorio.TipoDeOcorrenciaDeCTe(unitOfWork);
            List<Dominio.Entidades.Embarcador.Pedidos.GatilhoGeracaoAutomaticaPedidoOcorrenciaColetaEntrega> gatilhosGeracaoAutomaticaPedidoOcorrenciaColetaEntregaCadastradosOuAtualizados = new List<Dominio.Entidades.Embarcador.Pedidos.GatilhoGeracaoAutomaticaPedidoOcorrenciaColetaEntrega>();

            foreach (dynamic gatilhoGeracaoAutomaticaPedidoOcorrenciaColetaEntrega in gatilhosGeracaoAutomaticaPedidoOcorrenciaColetaEntregaAdicionadosOuAtualizados)
            {
                Dominio.Entidades.Embarcador.Pedidos.GatilhoGeracaoAutomaticaPedidoOcorrenciaColetaEntrega gatilhoGeracaoAutomaticaPedidoOcorrenciaColetaEntregaSalvar;
                int? codigo = ((string)gatilhoGeracaoAutomaticaPedidoOcorrenciaColetaEntrega.Codigo).ToNullableInt();

                if (codigo.HasValue)
                    gatilhoGeracaoAutomaticaPedidoOcorrenciaColetaEntregaSalvar = repositorioGatilhoGeracaoAutomaticaPedidoOcorrenciaColetaEntrega.BuscarPorCodigo(codigo.Value, auditavel: true) ?? throw new ControllerException(Localization.Resources.Pedidos.TipoOperacao.ConfiguracaoDeOcorrenciaAutomaticaNaoEncontrada);
                else
                    gatilhoGeracaoAutomaticaPedidoOcorrenciaColetaEntregaSalvar = new Dominio.Entidades.Embarcador.Pedidos.GatilhoGeracaoAutomaticaPedidoOcorrenciaColetaEntrega() { TipoOperacao = tipoOperacao };

                gatilhoGeracaoAutomaticaPedidoOcorrenciaColetaEntregaSalvar.Gatilho = ((string)gatilhoGeracaoAutomaticaPedidoOcorrenciaColetaEntrega.Gatilho).ToEnum<TipoGatilhoPedidoOcorrenciaColetaEntrega>();
                gatilhoGeracaoAutomaticaPedidoOcorrenciaColetaEntregaSalvar.Observacao = (string)gatilhoGeracaoAutomaticaPedidoOcorrenciaColetaEntrega.Observacao;
                gatilhoGeracaoAutomaticaPedidoOcorrenciaColetaEntregaSalvar.TipoOcorrencia = repositorioTipoOcorrencia.BuscarPorCodigo(((string)gatilhoGeracaoAutomaticaPedidoOcorrenciaColetaEntrega.TipoOcorrencia).ToInt()) ?? throw new ControllerException(Localization.Resources.Pedidos.TipoOperacao.TipoDeOcorrenciaNaoEncontrado);

                ValidarDadosGatilhoGeracaoAutomaticaPedidoOcorrenciaColetaEntregaDuplicado(gatilhosGeracaoAutomaticaPedidoOcorrenciaColetaEntregaCadastradosOuAtualizados, gatilhoGeracaoAutomaticaPedidoOcorrenciaColetaEntregaSalvar);

                gatilhosGeracaoAutomaticaPedidoOcorrenciaColetaEntregaCadastradosOuAtualizados.Add(gatilhoGeracaoAutomaticaPedidoOcorrenciaColetaEntregaSalvar);

                if (codigo.HasValue)
                    repositorioGatilhoGeracaoAutomaticaPedidoOcorrenciaColetaEntrega.Atualizar(gatilhoGeracaoAutomaticaPedidoOcorrenciaColetaEntregaSalvar, (historico != null ? Auditado : null), historico);
                else
                    repositorioGatilhoGeracaoAutomaticaPedidoOcorrenciaColetaEntrega.Inserir(gatilhoGeracaoAutomaticaPedidoOcorrenciaColetaEntregaSalvar, (historico != null ? Auditado : null), historico);
            }
        }

        private void ValidarDadosGatilhoGeracaoAutomaticaPedidoOcorrenciaColetaEntregaDuplicado(List<Dominio.Entidades.Embarcador.Pedidos.GatilhoGeracaoAutomaticaPedidoOcorrenciaColetaEntrega> gatilhosGeracaoAutomaticaPedidoOcorrenciaColetaEntregaCadastradosOuAtualizados, Dominio.Entidades.Embarcador.Pedidos.GatilhoGeracaoAutomaticaPedidoOcorrenciaColetaEntrega gatilhoGeracaoAutomaticaPedidoOcorrenciaColetaEntregaSalvar)
        {
            Dominio.Entidades.Embarcador.Pedidos.GatilhoGeracaoAutomaticaPedidoOcorrenciaColetaEntrega gatilhoGeracaoAutomaticaPedidoOcorrenciaColetaEntregaDuplicado = (
                from o in gatilhosGeracaoAutomaticaPedidoOcorrenciaColetaEntregaCadastradosOuAtualizados
                where (
                    o.TipoOcorrencia.Codigo == gatilhoGeracaoAutomaticaPedidoOcorrenciaColetaEntregaSalvar.TipoOcorrencia.Codigo &&
                    o.Gatilho == gatilhoGeracaoAutomaticaPedidoOcorrenciaColetaEntregaSalvar.Gatilho
                )
                select o
            ).FirstOrDefault();

            if (gatilhoGeracaoAutomaticaPedidoOcorrenciaColetaEntregaDuplicado != null)
                throw new ControllerException(Localization.Resources.Pedidos.TipoOperacao.ConfiguracaoDeOcorrenciaAutomaticaDuplicadaParaTipoDeOcorrencia + gatilhoGeracaoAutomaticaPedidoOcorrenciaColetaEntregaDuplicado.TipoOcorrencia.Descricao);
        }

        #endregion

        #region Métodos Privados - Transportadores

        private void AtualizarTransportadores(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, Dominio.Entidades.Auditoria.HistoricoObjeto historico, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.TipoOperacaoTransportador repositorioTipoOperacaoTransportador = new Repositorio.Embarcador.Pedidos.TipoOperacaoTransportador(unitOfWork);
            List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoTransportador> transportadores = repositorioTipoOperacaoTransportador.BuscarPorTipoOperacao(tipoOperacao.Codigo);
            dynamic transportadoresAdicionadosOuAtualizados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ListaTransportador"));

            ExcluirTransportadoresRemovidos(tipoOperacao, transportadores, transportadoresAdicionadosOuAtualizados, historico, unitOfWork);
            SalvarTransportadoresAdicionadosOuAtualizados(tipoOperacao, transportadores, transportadoresAdicionadosOuAtualizados, historico, unitOfWork);
        }

        private void ExcluirTransportadoresRemovidos(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoTransportador> transportadores, dynamic transportadoresAdicionadosOuAtualizados, Dominio.Entidades.Auditoria.HistoricoObjeto historico, Repositorio.UnitOfWork unitOfWork)
        {
            if ((transportadores == null) || (transportadores.Count == 0))
                return;

            Repositorio.Embarcador.Pedidos.TipoOperacaoTransportador repositorioTipoOperacaoTransportador = new Repositorio.Embarcador.Pedidos.TipoOperacaoTransportador(unitOfWork);
            List<int> listaCodigosAtualizados = new List<int>();

            foreach (dynamic transportador in transportadoresAdicionadosOuAtualizados)
            {
                int? codigo = ((string)transportador.Codigo).ToNullableInt();

                if (codigo.HasValue)
                    listaCodigosAtualizados.Add(codigo.Value);
            }

            List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoTransportador> listaTransportadorRemover = (from o in transportadores where !listaCodigosAtualizados.Contains(o.Codigo) select o).ToList();

            foreach (Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoTransportador transportador in listaTransportadorRemover)
                repositorioTipoOperacaoTransportador.Deletar(transportador, (historico != null ? Auditado : null), historico);
        }

        private void SalvarTransportadoresAdicionadosOuAtualizados(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoTransportador> transportadores, dynamic transportadoresAdicionadosOuAtualizados, Dominio.Entidades.Auditoria.HistoricoObjeto historico, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.TipoOperacaoTransportador repositorioTipoOperacaoTransportador = new Repositorio.Embarcador.Pedidos.TipoOperacaoTransportador(unitOfWork);
            Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(unitOfWork);
            List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoTransportador> transportadoresCadastradosOuAtualizados = new List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoTransportador>();

            foreach (dynamic transportador in transportadoresAdicionadosOuAtualizados)
            {
                Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoTransportador transportadorSalvar;
                int? codigo = ((string)transportador.Codigo).ToNullableInt();

                if (codigo.HasValue)
                    transportadorSalvar = repositorioTipoOperacaoTransportador.BuscarPorCodigo(codigo.Value, auditavel: true) ?? throw new ControllerException(Localization.Resources.Pedidos.TipoOperacao.ConfiguracaoPorTransportadorNaoEncontrada);
                else
                    transportadorSalvar = new Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoTransportador() { TipoOperacao = tipoOperacao };

                transportadorSalvar.Transportador = repositorioEmpresa.BuscarPorCodigo(((string)transportador.Transportador).ToInt()) ?? throw new ControllerException(Localization.Resources.Pedidos.TipoOperacao.TransportadorNaoEncontrado);
                transportadorSalvar.TipoUltimoPontoRoteirizacao = ((string)transportador.TipoUltimoPontoRoteirizacao).ToEnum<TipoUltimoPontoRoteirizacao>();

                ValidarDadosTransportadorDuplicado(transportadoresCadastradosOuAtualizados, transportadorSalvar);

                transportadoresCadastradosOuAtualizados.Add(transportadorSalvar);

                if (codigo.HasValue)
                    repositorioTipoOperacaoTransportador.Atualizar(transportadorSalvar, (historico != null ? Auditado : null), historico);
                else
                    repositorioTipoOperacaoTransportador.Inserir(transportadorSalvar, (historico != null ? Auditado : null), historico);
            }
        }

        private void ValidarDadosTransportadorDuplicado(List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoTransportador> transportadoresCadastradosOuAtualizados, Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoTransportador transportadorSalvar)
        {
            Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoTransportador transportadorDuplicado = (
                from o in transportadoresCadastradosOuAtualizados
                where o.Transportador.Codigo == transportadorSalvar.Transportador.Codigo
                select o
            ).FirstOrDefault();

            if (transportadorDuplicado != null)
                throw new ControllerException(Localization.Resources.Pedidos.TipoOperacao.Transportador + transportadorDuplicado.Transportador.Descricao + Localization.Resources.Pedidos.TipoOperacao.Duplicado);
        }
        private async Task<dynamic> ObterCodigosIntegracaoAsync(int codigoTipoOperacao, Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.Pedidos.CodigoIntegracaoGerenciadoraRisco repositorioCodigoIntegracaoGerenciadoraRisco = new(unitOfWork, cancellationToken);
            List<Dominio.Entidades.Embarcador.Pedidos.CodigoIntegracaoGerenciadoraRisco> codigosIntegracao = await repositorioCodigoIntegracaoGerenciadoraRisco.BuscarPorTipoOperacaoAsync(codigoTipoOperacao, cancellationToken);

            return (from obj in codigosIntegracao
                    select new
                    {
                        obj.Codigo,
                        obj.CodigoIntegracao,
                        obj.EtapaCarga,
                        EtapaCargaDescricao = obj.EtapaCarga.ObterDescricaoIntegracaoApisul()
                    }).ToList();
        }


        #endregion
    }
}
