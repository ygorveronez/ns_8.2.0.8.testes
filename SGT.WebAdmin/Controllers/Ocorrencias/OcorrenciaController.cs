using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Importacao;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Ocorrencias
{
    [CustomAuthorize(new string[] { "ConsultarAutorizacoes", "ConsultarAutorizacoesEmissao", "ConsultaCTesComplementados", "ConsultarCTesOcorrencia", "ConsultarNFSManualOcorrencia", "ObterConfiguracao", "DetalhesAutorizacao", "ObterDetalhesCarga", "ConsultaDocumentosParaEmissaoNFSManualComplementados" }, "Ocorrencias/Ocorrencia", "Chamados/ChamadoOcorrencia", "Chamados/ChamadoOcorrencia", "Frota/Infracao", "Documentos/ControleDocumento")]
    public class OcorrenciaController : BaseController
    {
        #region Construtores

        public OcorrenciaController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

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

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa()
        {
            try
            {
                Models.Grid.Grid grid = ObterGridPesquisa();
                byte[] arquivoBinario = grid.GerarExcel();

                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", $"{grid.tituloExportacao}.{grid.extensaoCSV}");

                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuUmaFalhaAoGerarArquivo);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoExportar);
            }
        }

        public async Task<IActionResult> ObterConfiguracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracaoInteliPost = repTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.InteliPost);

                return new JsonpResult(new
                {
                    TemIntegracaoIntelipost = tipoIntegracaoInteliPost != null ? true : false,
                });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, Localization.Resources.Ocorrencias.Ocorrencia.OcorreuUmaFalhaAoObterOsAsConfiguracoes);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> EnviarArquivo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                IList<Servicos.DTO.CustomFile> arquivos = HttpContext.GetFiles("Arquivo");
                List<string> erros = new List<string>();
                List<string> extensoesValidas = new List<string>() { ".txt" };
                int adicionados = 0;

                if (arquivos.Count <= 0)
                    return new JsonpResult(false, true, Localization.Resources.Ocorrencias.Ocorrencia.NenhumArquivoSelecionadoParaEnvio);

                Repositorio.LayoutEDI repLayoutEDI = new Repositorio.LayoutEDI(unitOfWork);

                Dominio.Entidades.LayoutEDI layoutEDI = repLayoutEDI.BuscarPorTipo(Dominio.Enumeradores.TipoLayoutEDI.OCOREN).FirstOrDefault();
                for (int i = 0; i < arquivos.Count(); i++)
                {
                    Servicos.DTO.CustomFile file = arquivos[i];
                    string extensaoArquivo = System.IO.Path.GetExtension(file.FileName).ToLower();

                    if (!extensaoArquivo.Contains(extensaoArquivo))
                    {
                        erros.Add(string.Format(Localization.Resources.Ocorrencias.Ocorrencia.ExtensaoNaoPermitida, extensaoArquivo));
                        continue;
                    }
                    else
                    {
                        Servicos.Embarcador.CargaOcorrencia.Ocorrencia serOcorrencia = new Servicos.Embarcador.CargaOcorrencia.Ocorrencia();
                        string retorno = serOcorrencia.ProcessarOcorren(layoutEDI, file.InputStream, System.IO.Path.GetFileName(file.FileName), TipoServicoMultisoftware, ConfiguracaoEmbarcador, Usuario, TipoEnvioArquivo.Manual, Cliente, unitOfWork, Auditado);
                        if (!string.IsNullOrEmpty(retorno))
                            return new JsonpResult(false, retorno);

                        adicionados++;
                    }
                }

                return new JsonpResult(new
                {
                    Adicionados = adicionados,
                    Erros = erros
                });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Ocorrencias.Ocorrencia.OcorreuUmaFalhaAoEnvioOArquivo);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ComplementarInformacoesOcorrencia()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync();

                Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);
                Repositorio.Embarcador.Creditos.HierarquiaSolicitacaoCredito repHierarquiaSolicitacaoCredito = new Repositorio.Embarcador.Creditos.HierarquiaSolicitacaoCredito(unitOfWork);
                Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaDocumento repCargaOcorrenciaDocumento = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaDocumento(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(unitOfWork);
                Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao repCargaOcorrenciaAutorizacao = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoOcorrencia repConfiguracaoOcorrencia = new Repositorio.Embarcador.Configuracoes.ConfiguracaoOcorrencia(unitOfWork);

                Servicos.Embarcador.CargaOcorrencia.Ocorrencia srvOcorrencia = new Servicos.Embarcador.CargaOcorrencia.Ocorrencia(unitOfWork);
                Servicos.Embarcador.Carga.CTeComplementar serCargaCTeComplementar = new Servicos.Embarcador.Carga.CTeComplementar(unitOfWork);
                Servicos.Embarcador.Carga.Ocorrencia serOcorrencia = new Servicos.Embarcador.Carga.Ocorrencia(unitOfWork);
                Servicos.Embarcador.CargaOcorrencia.Ocorrencia svcCargaOcorrencia = new Servicos.Embarcador.CargaOcorrencia.Ocorrencia();

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = await repConfiguracaoTMS.BuscarConfiguracaoPadraoAsync();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoOcorrencia configuracaoOcorrencia = await repConfiguracaoOcorrencia.BuscarConfiguracaoPadraoAsync();
                List<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.Notificacao> notificacoes = new List<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.Notificacao>();
                List<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> cargaDocumentosParaEmissaoNFSManual = new List<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual>();
                List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTEs = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

                int.TryParse(Request.Params("Codigo"), out int codigo);

                Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia = repOcorrencia.BuscarPorCodigo(codigo);

                if (ocorrencia.SituacaoOcorrencia != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.AgInformacoes)
                    throw new ControllerException(string.Format(Localization.Resources.Ocorrencias.Ocorrencia.NaoEPossivelConfirmarOcorrenciaNaAtualSituacao, ocorrencia.DescricaoSituacao));

                ocorrencia.Initialize();

                PreencherConfirmacaoComRequest(ref ocorrencia, unitOfWork);

                if (ocorrencia.OcorrenciaDeEstadia || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                {
                    cargaCTEs = await srvOcorrencia.BuscarCTesSelecionadosOuCargas(ObterFiltrosDePesquisaConsultaCtes(unitOfWork), ocorrencia.Carga, ConfiguracaoEmbarcador, unitOfWork, Request.Params("CargaCTes"), bool.Parse(Request.Params("SelecionarTodos")), 0, 0, TipoServicoMultisoftware, Usuario, 0);

                    if (ocorrencia.OcorrenciaDeEstadia && (cargaCTEs == null || cargaCTEs.Count == 0))
                        throw new ControllerException(Localization.Resources.Ocorrencias.Ocorrencia.NecessarioSelecionarAoMenosUmCtEParaGerarAOcorrencia);

                    if (ocorrencia.OcorrenciaDeEstadia && (cargaCTEs.Count > 100 && ocorrencia.ValorOcorrencia > 0m))
                        throw new ControllerException(Localization.Resources.Ocorrencias.Ocorrencia.AQuantidadeMaximaPermitidaParaAGeracaoDeComplementosEDeCtEsSelecioneMenosCtEsOuFacaMaisDeUmaOcorrenciaCasoNecessario);

                    if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                        cargaDocumentosParaEmissaoNFSManual = BuscarDocumentosParaEmissaoNFSManualSelecionados(ocorrencia.Carga, 0, 0, 0, unitOfWork);

                    if (ocorrencia.OcorrenciaDeEstadia)
                        repCargaOcorrenciaAutorizacao.DeletarPorOcorrencia(ocorrencia.Codigo);

                    await srvOcorrencia.DeletarDadosCargaCTeOcorrenciaAsync(ocorrencia, unitOfWork);

                    string mensagemRetorno = string.Empty;

                    if (!srvOcorrencia.FluxoGeralOcorrencia(ref ocorrencia, cargaCTEs, cargaDocumentosParaEmissaoNFSManual, ref mensagemRetorno, unitOfWork, TipoServicoMultisoftware, this.Usuario, this.ConfiguracaoEmbarcador, this.Cliente, Request.Params("CargaCTesImportados"), true, this.Usuario?.PerfilAcesso?.PermitirAbrirOcorrenciaAposPrazoSolicitacao ?? false, Auditado, null, false, ClienteAcesso.URLAcesso))
                        throw new ControllerException(string.Format(Localization.Resources.Ocorrencias.Ocorrencia.NaoFoiPossivelConfirmarOcorrencia, mensagemRetorno));
                }

                if (configuracaoTMS.ObrigatorioRegrasOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SimNao.Sim)
                {
                    if (ocorrencia.OcorrenciaDeEstadia)
                        repCargaOcorrenciaAutorizacao.DeletarPorOcorrencia(ocorrencia.Codigo);

                    ocorrencia.SituacaoOcorrencia = srvOcorrencia.VerificarRegrasAutorizacaoAprovacaoOcorrencia(ocorrencia, out notificacoes, TipoServicoMultisoftware, unitOfWork, this.Usuario, ClienteAcesso.URLAcesso);

                    if ((ocorrencia.SituacaoOcorrencia == SituacaoOcorrencia.EmEmissaoCTeComplementar) && !ocorrencia.DataAprovacao.HasValue)
                        ocorrencia.DataAprovacao = DateTime.Now;
                }
                else
                {
                    List<Dominio.Entidades.Embarcador.Creditos.HierarquiaSolicitacaoCredito> hierarquiaCreditos = repHierarquiaSolicitacaoCredito.BuscarPorRecebedor(this.Usuario.Codigo);

                    if (hierarquiaCreditos.Count > 0)
                    {
                        string retorno = srvOcorrencia.ValidarHerarquiaDeCredito(ref ocorrencia, unitOfWork, this.Usuario, TipoServicoMultisoftware, Request.Params("CreditosUtilizados"), int.Parse(Request.Params("CodigoCreditorSolicitar")));
                        if (!string.IsNullOrWhiteSpace(retorno))
                            throw new ControllerException(retorno);
                    }
                    else
                        ocorrencia.SituacaoOcorrencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.EmEmissaoCTeComplementar;
                }

                List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento> cargaOcorrenciaDocumentos = repCargaOcorrenciaDocumento.BuscarPorOcorrencia(ocorrencia.Codigo);
                foreach (Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento cargaOcorrenciaDocumento in cargaOcorrenciaDocumentos)
                {
                    Dominio.Entidades.OcorrenciaDeCTe ocorrenciaDeCTe = srvOcorrencia.GerarOcorrenciaCTe(ocorrencia, cargaOcorrenciaDocumento.CargaCTe, unitOfWork);

                    cargaOcorrenciaDocumento.OcorrenciaDeCTe = ocorrenciaDeCTe;

                    await repCargaOcorrenciaDocumento.AtualizarAsync(cargaOcorrenciaDocumento);

                    if (cargaOcorrenciaDocumento.CTeImportado != null && !svcCargaOcorrencia.AjustarCTeImportado(out string erro, cargaOcorrenciaDocumento.CTeImportado, ocorrencia.Carga, ocorrencia.ComponenteFrete, unitOfWork))
                        throw new ControllerException(erro);
                }

                if (configuracaoOcorrencia.PermitirReabrirOcorrenciaEmCasoDeRejeicao && ocorrencia.TipoOcorrencia.PermitirReabrirOcorrenciaEmCasoDeRejeicao)
                    GerarParametrosOcorrencia(ocorrencia, unitOfWork, true);

                if (ocorrencia.TipoOcorrencia?.DataOcorrenciaIgualDataCTeComplementado ?? false)
                    ocorrencia.DataOcorrencia = cargaOcorrenciaDocumentos.FirstOrDefault()?.CargaCTe?.CTe.DataEmissao ?? ocorrencia.DataOcorrencia;

                if (ocorrencia.SituacaoOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.EmEmissaoCTeComplementar || ocorrencia.SituacaoOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.AgAprovacao)
                {
                    serCargaCTeComplementar.ImportarCTesComplementaresParaOcorrencia(ocorrencia, unitOfWork, TipoServicoMultisoftware);
                }

                Servicos.Embarcador.Integracao.IntegracaoEDI.AdicionarEDIParaIntegracao(ocorrencia, false, TipoServicoMultisoftware, unitOfWork);

                if (ocorrencia.SituacaoOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.EmEmissaoCTeComplementar)
                {
                    serOcorrencia.ValidarEnviarEmissaoComplementosOcorrencia(ocorrencia, unitOfWork);
                }

                if (notificacoes != null)
                {
                    Servicos.Embarcador.Notificacao.Notificacao serNotificacao = new Servicos.Embarcador.Notificacao.Notificacao(unitOfWork.StringConexao, null, TipoServicoMultisoftware, string.Empty);
                    foreach (Dominio.ObjetosDeValor.Embarcador.Ocorrencia.Notificacao notificacao in notificacoes)
                    {
                        string titulo = string.Concat(ocorrencia.Carga?.Filial?.Descricao, Localization.Resources.Ocorrencias.Ocorrencia.OcorrenciaBarra, ocorrencia.NumeroOcorrencia, " - ", ocorrencia.TipoOcorrencia?.Descricao, Localization.Resources.Ocorrencias.Ocorrencia.CargaBarra, ocorrencia.Carga?.CodigoCargaEmbarcador);

                        serNotificacao.GerarNotificacaoEmail(notificacao.Aprovador, Usuario, ocorrencia.Codigo, "Ocorrencias/AutorizacaoOcorrencia", titulo, notificacao.Mensagem, Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.cifra, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.credito, TipoServicoMultisoftware, unitOfWork);
                    }
                }

                await Servicos.Auditoria.Auditoria.AuditarComAlteracoesRealizadasAsync(Auditado, ocorrencia, ocorrencia.GetChanges(), Localization.Resources.Ocorrencias.Ocorrencia.ComplementouInformacoesDaOcorrencia, unitOfWork);

                await repOcorrencia.AtualizarAsync(ocorrencia, Auditado);

                await unitOfWork.CommitChangesAsync();

                return new JsonpResult(RetornarOcorrencia(ocorrencia, unitOfWork));
            }
            catch (BaseException ex)
            {
                await unitOfWork.RollbackAsync();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Ocorrencias.Ocorrencia.OcorreuUmaFalhaAoConfirmarAOcorrencia);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                // Repositorios e Servicos
                Servicos.Embarcador.CargaOcorrencia.Ocorrencia srvOcorrencia = new Servicos.Embarcador.CargaOcorrencia.Ocorrencia(unitOfWork);
                Servicos.Embarcador.Chamado.Chamado servicoChamado = new Servicos.Embarcador.Chamado.Chamado(unitOfWork);

                Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCTe repCargaCTeComplementar = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
                Repositorio.Embarcador.Frete.ContratoFreteTransportador repContratoFreteTransportador = new Repositorio.Embarcador.Frete.ContratoFreteTransportador(unitOfWork);
                Repositorio.Embarcador.Transportadores.TransportadorConfiguracaoNFSe repTransportadorConfiguracaoNFSe = new Repositorio.Embarcador.Transportadores.TransportadorConfiguracaoNFSe(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Embarcador.Chamados.Chamado repChamado = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);
                Repositorio.ModeloDocumentoFiscal repModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(unitOfWork);
                Repositorio.Embarcador.Chamados.ChamadoOcorrencia repChamadoOcorrencia = new Repositorio.Embarcador.Chamados.ChamadoOcorrencia(unitOfWork);
                Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoOcorrencia repConfiguracaoOcorrencia = new Repositorio.Embarcador.Configuracoes.ConfiguracaoOcorrencia(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoOcorrencia configuracaoOcorrencia = repConfiguracaoOcorrencia.BuscarConfiguracaoPadrao();

                //-- Instancia e preenche ocorrencia
                //-----------------------------------
                int codigo = Request.GetIntParam("Codigo");
                bool confirmarOcorrencia = Request.GetBoolParam("ConfirmarOcorrencia");

                Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia = codigo > 0 ? repOcorrencia.BuscarPorCodigo(codigo) : new Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia();

                PreencherEntidadeComRequest(ref ocorrencia, unitOfWork);

                if (ocorrencia.TipoOcorrencia == null)
                    throw new ControllerException(Localization.Resources.Ocorrencias.Ocorrencia.EObrigatorioSelecionarOTipoDeOcorrencia);

                if ((ocorrencia.TipoOcorrencia?.NaoPermitirValorSuplementoMaiorQueDocumento ?? false) &&
                    ocorrencia.TipoOcorrencia?.ModeloDocumentoFiscal?.Abreviacao?.ToUpper() == "ND" ||
                    ocorrencia.TipoOcorrencia?.ModeloDocumentoFiscal?.Abreviacao?.ToUpper() == "NOTA DE DESCONTO")
                {
                    decimal valorAReceber = await BuscarValorAReceber(ocorrencia.Codigo, ocorrencia.Carga.Codigo, unitOfWork);

                    if (ocorrencia.ValorOcorrencia > valorAReceber)
                        throw new ControllerException(Localization.Resources.Ocorrencias.Ocorrencia.NaoPermitirValorSuplementoMaiorQueDocumento.Replace("XXX", valorAReceber.ToString("n2")));
                }

                if (ocorrencia.TipoOcorrencia?.NovaOcorrenciaAguardandoInformacoes ?? false && !confirmarOcorrencia)
                {
                    ocorrencia.SituacaoOcorrencia = SituacaoOcorrencia.AgInformacoes;


                    if (codigo > 0)
                        repOcorrencia.Atualizar(ocorrencia);
                    else
                        repOcorrencia.Inserir(ocorrencia);

                    dynamic retorno = RetornarOcorrencia(ocorrencia, unitOfWork);

                    unitOfWork.CommitChanges();

                    return new JsonpResult(retorno);
                }


                if (ocorrencia.DataOcorrencia < DateTime.Today && (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Terceiros))
                    throw new ControllerException(Localization.Resources.Ocorrencias.Ocorrencia.NaoEPossivelAdicionarOcorrenciaComDataRetroativa);

                if (ocorrencia.ValorOcorrencia > 10000000m)
                    throw new ControllerException(Localization.Resources.Ocorrencias.Ocorrencia.OValorOcorrenciaNaoPodeSerMaiorQue);

                if (ocorrencia.TipoOcorrencia.ExigirInformarObservacao && string.IsNullOrWhiteSpace(ocorrencia.Observacao))
                    throw new ControllerException(Localization.Resources.Ocorrencias.Ocorrencia.EObrigatorioInformarUmaObservacao);

                if (ocorrencia.TipoOcorrencia.OcorrenciaExclusivaParaIntegracao)
                    throw new ControllerException(Localization.Resources.Ocorrencias.Ocorrencia.TipoDeOcorrenciaEExcluisivaParaIntegracao);

                if (ocorrencia.OrigemOcorrenciaPorPeriodo && ((!ocorrencia.PeriodoInicio.HasValue || !ocorrencia.PeriodoFim.HasValue) || ocorrencia.PeriodoInicio.Value > ocorrencia.PeriodoFim.Value))
                    throw new ControllerException(Localization.Resources.Ocorrencias.Ocorrencia.PeriodoSelecionadoEInvalido);

                if (ocorrencia.Carga?.CargaBloqueadaParaEdicaoIntegracao ?? false)
                    throw new ControllerException(Localization.Resources.Ocorrencias.Ocorrencia.NaoEPossivelGerarUmaOcorenciaParaUmaCargaBloqueada);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                {
                    int.TryParse(Request.Params("Empresa"), out int codigoTransportador);
                    ocorrencia.Emitente = repEmpresa.BuscarPorCodigo(codigoTransportador);

                    if (ocorrencia.TipoOcorrencia.PermiteInformarValor == true && ocorrencia.ValorOcorrencia <= 0m && !ocorrencia.NaoGerarDocumento)
                        throw new ControllerException(Localization.Resources.Ocorrencias.Ocorrencia.ValorOcorrenciaNaoInformadoVerifiqueTenteNovamente);
                }

                if (ConfiguracaoEmbarcador.ExigirMotivoOcorrencia && string.IsNullOrEmpty(ocorrencia.Motivo))
                    throw new ControllerException(Localization.Resources.Ocorrencias.Ocorrencia.MotivoDaOcorrenciaEObrigatorio);

                if (ocorrencia.TipoOcorrencia.OcorrenciaParaCobrancaDePedagio && PedagioPagoNaCarga(ocorrencia.Carga.Codigo, unitOfWork))
                    throw new ControllerException(Localization.Resources.Ocorrencias.Ocorrencia.ValorDePedagioAntecipadoNaCarga);

                if (ocorrencia.Carga != null && ocorrencia.TipoOcorrencia.BloquearOcorrenciaCargaCanhotoDigitalizadoAprovado && !ValidarCanhotoCarga(ocorrencia.Carga, unitOfWork))

                    if (ocorrencia.TipoOcorrencia.ImpedirCriarOcorrenciaCasoExistirCanhotosPendentes && ocorrencia.Carga != null)
                    {
                        if (repCanhoto.ContarCanhotosPedentesPorCarga(ocorrencia.Carga.Codigo) > 0)
                        {

                            // Implementar a validacao das notas fiscais?? onde encontro no objeto ocorrencia as Nota Fiscais CTes para poder validar ??
                            // seria no campo NumerosCTes? e aquele entidade PedidoXMLNotaFiscalCTe, ela nao esta presente em ocorrencia , entao como posso
                            // atraves do objeto Ocorrencia , conferir as notas fiscais?? e a questao dos status :
                            // Devolvida total
                            // Reentregue
                            throw new ControllerException(Localization.Resources.Ocorrencias.Ocorrencia.ExistemCanhotosAguardandoAprovacaoOuPendentesVerifiqueAntesDeGerarUmaOcorrencia);
                        }
                    }



                ocorrencia.OrigemOcorrencia = ocorrencia.TipoOcorrencia.OrigemOcorrencia;

                int numeroNF = Request.GetIntParam("NumeroNF");
                int numeroDocumento = Request.GetIntParam("NumeroDocumento");
                double cnpjdestinatario = Request.GetDoubleParam("Destinatario");
                bool ocorrenciaSalvaPelaTelaChamadoOcorrencia = Request.GetBoolParam("OcorrenciaSalvaPelaTelaChamadoOcorrencia");

                //-- Dados para preenchimento
                //---------------------------
                List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTEs = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
                List<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> cargaDocumentosParaEmissaoNFSManual = new List<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual>();
                List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> xMLNotasFiscais = new List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();
                List<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.OcorrenciaCargaCTeNotaFiscal> listaOcorrenciaCargaCTeNotaFiscal = new List<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.OcorrenciaCargaCTeNotaFiscal>();

                // Vincula as cargas a ocorrencia
                if (ocorrencia.DTNatura != null)
                {
                    cargaCTEs = srvOcorrencia.BuscarCTesPorCargaEDTNatura(ocorrencia.Carga, ocorrencia.DTNatura);

                    if (cargaCTEs.Count <= 0)
                        throw new ControllerException(Localization.Resources.Ocorrencias.Ocorrencia.NenhumCtEEncontradoNestaCargaComAsNotasFiscaisDesteDt);
                }
                else
                {
                    switch (ocorrencia.OrigemOcorrencia)
                    {
                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemOcorrencia.PorCarga:
                            Repositorio.Embarcador.NFS.LancamentoNFSManual repLancamentoNFSManual = new Repositorio.Embarcador.NFS.LancamentoNFSManual(unitOfWork);
                            Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual repCargaDocumentoParaEmissaoNFSManual = new Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual(unitOfWork);

                            cargaCTEs = await srvOcorrencia.BuscarCTesSelecionadosOuCargas(ObterFiltrosDePesquisaConsultaCtes(unitOfWork), ocorrencia.Carga, ConfiguracaoEmbarcador, unitOfWork, Request.Params("CargaCTes"), bool.Parse(Request.Params("SelecionarTodos")), numeroNF, cnpjdestinatario, TipoServicoMultisoftware, Usuario, numeroDocumento, listaOcorrenciaCargaCTeNotaFiscal);
                            cargaDocumentosParaEmissaoNFSManual = BuscarDocumentosParaEmissaoNFSManualSelecionados(ocorrencia.Carga, numeroNF, cnpjdestinatario, numeroDocumento, unitOfWork);

                            // Antigo comportamento em que era vinculado a NFSManuais
                            // List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> nfsManuais = (from obj in cargaCTEs where obj.CTe != null && obj.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFS select obj.CTe).ToList();

                            // foreach (Dominio.Entidades.ConhecimentoDeTransporteEletronico nfsManual in nfsManuais)
                            // {
                            //     if (repCargaDocumentoParaEmissaoNFSManual.ExisteNFSDeOcorrenciaPorCTe(nfsManual.Codigo))
                            //     {
                            //         unitOfWork.Rollback();
                            //         return new JsonpResult(false, true, "A NFS " + nfsManual.Numero + " também foi gerada a partir de outra ocorrencia por esse motivo não é possível complementá-la.");
                            //     }
                            // }

                            //

                            break;
                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemOcorrencia.PorPeriodo:
                            int codigoTransportador = 0;
                            int.TryParse(Request.Params("Empresa"), out codigoTransportador);
                            int.TryParse(Request.Params("Filial"), out int codigoFilial);
                            ocorrencia.Cargas = srvOcorrencia.BuscarCargasDoPeriodoSelecionado(ocorrencia, unitOfWork, TipoServicoMultisoftware, this.Usuario, codigoTransportador, codigoFilial);

                            break;
                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemOcorrencia.PorContrato:
                            Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCNPJ(this.Usuario.ClienteTerceiro?.CPF_CNPJ_SemFormato ?? string.Empty);
                            int transportador = empresa?.Codigo ?? 0;

                            ocorrencia.ContratoFrete = repContratoFreteTransportador.BuscarContratoAtivo(transportador, ocorrencia.TipoOcorrencia.Codigo, DateTime.Now.Date);
                            ocorrencia.Tomador = ocorrencia.ContratoFrete.ClienteTomador;

                            if (ocorrencia.TipoOcorrencia.OcorrenciaComVeiculo)
                            {
                                if (ocorrencia.Veiculo == null)
                                    throw new ControllerException(Localization.Resources.Ocorrencias.Ocorrencia.NenhumCtEEncontradoNestaCargaComAsNotasFiscaisDesteDt);

                                int codigoTransportador2 = 0;
                                int.TryParse(Request.Params("Transportador"), out codigoTransportador2);
                                ocorrencia.Cargas = srvOcorrencia.BuscarCargasDoPeriodoSelecionadoComVeiculo(ocorrencia, unitOfWork, TipoServicoMultisoftware, this.Usuario, codigoTransportador2);
                            }

                            break;
                    }


                    if (cargaCTEs.Count > 100 && ocorrencia.ValorOcorrencia > 0m)
                        throw new ControllerException(Localization.Resources.Ocorrencias.Ocorrencia.AQuantidadeMaximaDeCtEsPermitidaParaAGeracaoDeComplementosEDeSelecioneMenosCtEsOuFacaMaisDeUmaOcorrenciaCasoNecessario);
                }

                int.TryParse(Request.Params("CodigoParametroPeriodo"), out int codigoParametroPeriodo);

                bool.TryParse(Request.Params("DividirOcorrencia"), out bool dividirOcorrencia);
                bool? incluirICMSFrete = null;
                if (bool.TryParse(Request.Params("IncluirICMSFrete"), out bool incluirICMSFreteAux))
                    incluirICMSFrete = incluirICMSFreteAux;

                if (cargaCTEs.Exists(obj => obj.CargaCTeFilialEmissora != null || EmitirDocumentoParaFilialEmissoraComPreCTe(obj, ocorrencia)))
                    ocorrencia.EmiteComplementoFilialEmissora = true;

                if (incluirICMSFrete.HasValue)
                    ocorrencia.IncluirICMSFrete = incluirICMSFrete.Value;
                else
                    // TODO: Validar essa regra, pois quando é ocorrencia por periodo, não tem carga CTe
                    ocorrencia.IncluirICMSFrete = cargaCTEs.Count > 0 ? (cargaCTEs.FirstOrDefault().CTe.IncluirICMSNoFrete == Dominio.Enumeradores.OpcaoSimNao.Sim ? true : false) : false;


                //Setar CFOP do tipo de ocorrência
                string codigoCFOPIntegracao = string.Empty;
                if (ocorrencia != null && ocorrencia.TipoOcorrencia != null && cargaCTEs != null && cargaCTEs.Count > 0)
                {
                    Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = (from o in cargaCTEs where o.CTe != null && o.CargaCTeComplementoInfo == null select o.CTe).FirstOrDefault();

                    if (cte != null && cte.LocalidadeInicioPrestacao.Estado.Sigla == cte.LocalidadeTerminoPrestacao.Estado.Sigla)
                    {
                        if (cte.ValorICMS == 0 && !string.IsNullOrWhiteSpace(ocorrencia.TipoOcorrencia.CodigoIntegracaoCFOPEstadualIsento))
                            codigoCFOPIntegracao = ocorrencia.TipoOcorrencia.CodigoIntegracaoCFOPEstadualIsento;
                        else
                            codigoCFOPIntegracao = !string.IsNullOrWhiteSpace(ocorrencia.TipoOcorrencia.CodigoIntegracaoCFOPEstadual) ? ocorrencia.TipoOcorrencia.CodigoIntegracaoCFOPEstadual : string.Empty;
                    }
                    else if (cte != null)
                    {
                        if (cte.ValorICMS == 0 && !string.IsNullOrWhiteSpace(ocorrencia.TipoOcorrencia.CodigoIntegracaoCFOPEstadualIsento))
                            codigoCFOPIntegracao = ocorrencia.TipoOcorrencia.CodigoIntegracaoCFOPEstadualIsento;
                        else
                            codigoCFOPIntegracao = !string.IsNullOrWhiteSpace(ocorrencia.TipoOcorrencia.CodigoIntegracaoCFOPInterestadual) ? ocorrencia.TipoOcorrencia.CodigoIntegracaoCFOPInterestadual : !string.IsNullOrWhiteSpace(ocorrencia.TipoOcorrencia.CodigoIntegracaoCFOPEstadual) ? ocorrencia.TipoOcorrencia.CodigoIntegracaoCFOPEstadual : string.Empty;
                    }
                }

                if (!string.IsNullOrWhiteSpace(codigoCFOPIntegracao))
                    ocorrencia.CFOP = codigoCFOPIntegracao;

                if (ocorrencia.TipoOcorrencia.DataOcorrenciaIgualDataCTeComplementado)
                    ocorrencia.DataOcorrencia = cargaCTEs.FirstOrDefault()?.CTe.DataEmissao ?? ocorrencia.DataOcorrencia;

                srvOcorrencia.SetaEmitenteOcorrencia(ref ocorrencia, unitOfWork, TipoServicoMultisoftware, this.Usuario);

                if (codigoParametroPeriodo > 0 && dividirOcorrencia)
                    ocorrencia.Responsavel = Dominio.Enumeradores.TipoTomador.Remetente;

                int codigoChamado = Request.GetIntParam("Chamado");
                List<int> codigosChamados = Request.GetListParam<int>("Chamados");

                ValidarDadosOcorrencia(ocorrencia, cargaCTEs, cargaDocumentosParaEmissaoNFSManual, numeroNF, cnpjdestinatario, numeroDocumento, codigoChamado, unitOfWork);

                //-- Persistencia dos dados
                //-------------------------

                repOcorrencia.Inserir(ocorrencia, Auditado);

                SalvarListaProdutosCargaCTe(cargaCTEs, unitOfWork);

                if (ocorrenciaSalvaPelaTelaChamadoOcorrencia)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, ocorrencia, null, string.Format(Localization.Resources.Ocorrencias.Ocorrencia.OcorrenciaAdicionadaVinculadaAtendimento, ocorrencia.NumeroOcorrencia), unitOfWork);

                SalvarObservacoesFiscoContribuinte(ocorrencia, unitOfWork);

                Servicos.Embarcador.Integracao.IntegracaoOcorrencia.AdicionarIntegracoesOcorrencia(ocorrencia, cargaCTEs, unitOfWork);

                GerarParametrosOcorrencia(ocorrencia, unitOfWork);

                SalvarNotasParaComplementoCTEGlobalizado(ocorrencia, cargaCTEs, Request.GetStringParam("XMLNotasFiscaisParaCTeGlobalizado"), unitOfWork);

                //Copiar anexos dos chamados
                List<int> codigosChamadosCopiarAnexos = codigosChamados.ToList();
                List<Dominio.Entidades.Embarcador.Chamados.Chamado> chamadosFinalizar = repChamado.BuscarPorCodigos(codigosChamados);
                decimal valorChamados = 0;

                if (codigoChamado > 0)
                {
                    Dominio.Entidades.Embarcador.Chamados.Chamado atendimento = repChamado.BuscarPorCodigo(codigoChamado);
                    if (atendimento != null)
                    {
                        if (atendimento.Situacao == SituacaoChamado.Cancelada)
                            throw new ControllerException(string.Format(Localization.Resources.Ocorrencias.Ocorrencia.AtendimentoEstaCancelado, atendimento.Numero));

                        codigosChamadosCopiarAnexos.Add(codigoChamado);

                        Dominio.Entidades.Embarcador.Chamados.ChamadoOcorrencia chamadoOcorrencia = new Dominio.Entidades.Embarcador.Chamados.ChamadoOcorrencia()
                        {
                            CargaOcorrencia = ocorrencia,
                            Chamado = atendimento
                        };
                        repChamadoOcorrencia.Inserir(chamadoOcorrencia);
                    }
                }
                else
                {
                    if (configuracaoOcorrencia.PermitirVinculoAutomaticoEntreOcorreciaEAtendimento && !ocorrenciaSalvaPelaTelaChamadoOcorrencia)
                    {

                        foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cte in cargaCTEs)
                        {

                            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe nota in cte.NotasFiscais)
                            {
                                //Retorna os Atendimentos Vinculados a Nota Fiscal
                                IList<Dominio.ObjetosDeValor.Embarcador.Chamado.Chamado> litaAtendimento = repChamado.BuscarChamadosPorCodigoNotaFiscaECarga(ocorrencia.Carga.Codigo, nota.PedidoXMLNotaFiscal.XMLNotaFiscal.Codigo);

                                int codigoAtendimento = litaAtendimento.OrderByDescending(x => x.Codigo).Select(x => x.Codigo).FirstOrDefault();
                                Dominio.Entidades.Embarcador.Chamados.Chamado atendimento = repChamado.BuscarPorCodigo(codigoAtendimento);

                                if (atendimento != null)
                                {
                                    if (atendimento.Situacao == SituacaoChamado.Cancelada)
                                        throw new ControllerException(string.Format(Localization.Resources.Ocorrencias.Ocorrencia.AtendimentoEstaCancelado, atendimento.Numero));

                                    codigosChamadosCopiarAnexos.Add(codigoChamado);

                                    // Faz o vinculo do Atendimento através da Nota com a Ocorrência
                                    Dominio.Entidades.Embarcador.Chamados.ChamadoOcorrencia chamadoOcorrencia = new Dominio.Entidades.Embarcador.Chamados.ChamadoOcorrencia()
                                    {
                                        CargaOcorrencia = ocorrencia,
                                        Chamado = atendimento
                                    };
                                    repChamadoOcorrencia.Inserir(chamadoOcorrencia);
                                }
                            }
                        }

                    }
                }

                bool copiouAnexos = CopiarAnexosChamado(ocorrencia, codigosChamadosCopiarAnexos, unitOfWork);

                foreach (Dominio.Entidades.Embarcador.Chamados.Chamado chamado in chamadosFinalizar)
                {
                    valorChamados += chamado.Valor;

                    servicoChamado.GerarIntegracoes(chamado, unitOfWork, Auditado, TipoServicoMultisoftware);

                    if (chamado.Situacao != SituacaoChamado.AgIntegracao)
                    {
                        chamado.Situacao = SituacaoChamado.Finalizado;
                        chamado.DataFinalizacao = DateTime.Now;
                        chamado.ControleDuplicidade = chamado.Codigo;

                        repChamado.Atualizar(chamado);

                        new Servicos.Embarcador.SuperApp.IntegracaoNotificacaoApp(unitOfWork).GerarIntegracaoNotificacao(chamado, TipoNotificacaoApp.TratativaDoAtendimento);
                    }

                    if (codigoChamado > 0 && chamado.Codigo == codigoChamado)
                        continue;

                    Dominio.Entidades.Embarcador.Chamados.ChamadoOcorrencia chamadoOcorrencia = new Dominio.Entidades.Embarcador.Chamados.ChamadoOcorrencia()
                    {
                        CargaOcorrencia = ocorrencia,
                        Chamado = chamado
                    };
                    repChamadoOcorrencia.Inserir(chamadoOcorrencia);
                }

                bool possuiAnexos = Request.GetBoolParam("PossuiAnexos");
                if (ocorrencia.TipoOcorrencia.AnexoObrigatorio && !possuiAnexos && !copiouAnexos)
                    throw new ControllerException(Localization.Resources.Ocorrencias.Ocorrencia.EObrigatorioAdicionarUmAnexo);

                //-- Valor da Ocorrrência
                //-------------------------
                if (ocorrencia.OrigemOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemOcorrencia.PorPeriodo)
                {
                    // Dados Sumarizados carga ocorrencia
                    int.TryParse(Request.Params("Empresa"), out int transportador);
                    int.TryParse(Request.Params("Filial"), out int filial);

                    if (ocorrencia.TipoOcorrencia.OcorrenciaDestinadaFranquias)
                    {
                        // Salva os veiculos improdutivos
                        Servicos.Embarcador.CargaOcorrencia.Ocorrencia.SalvarVeiculosImprodutivos(ocorrencia, unitOfWork);
                    }
                    else
                    {
                        srvOcorrencia.SalvarCargasSumarizadas(ocorrencia, unitOfWork, TipoServicoMultisoftware, this.Usuario, transportador, filial, Request.Params("CargasComplementadasDias"));
                    }

                    // Atuliza valor da ocorrencia
                    if (!ocorrencia.TipoOcorrencia.PermiteInformarValor)
                        ocorrencia.ValorOcorrencia = srvOcorrencia.CalcularValorOcorrenciaPorTipoOcorrencia(ocorrencia);
                }
                else if (ocorrencia.OrigemOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemOcorrencia.PorContrato)
                {

                    if (ocorrencia.TipoOcorrencia.OcorrenciaComVeiculo)
                    {
                        // Atuliza valor da ocorrencia
                        decimal.TryParse(Request.Params("ParametroTexto"), out decimal kmExcedenteTotal);
                        ocorrencia.ValorOcorrencia = ocorrencia.ContratoFrete.OutrosValoresValorKmExcedente * kmExcedenteTotal;
                    }
                    else
                    {
                        // Dados dos veiculos
                        List<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaContratoVeiculo> veiculosContrato = Servicos.Embarcador.CargaOcorrencia.Ocorrencia.SalvarVeiculosContrato(ocorrencia, Request, unitOfWork);

                        // Dados dos motoristas/manobristas
                        Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaContratoMotorista motoristaContrato = Servicos.Embarcador.CargaOcorrencia.Ocorrencia.SalvarMotoristaContrato(ocorrencia, Request, ocorrencia.ContratoFrete, unitOfWork);

                        // Atuliza valor da ocorrencia
                        if (ocorrencia.ValorOcorrencia <= 0) //Adicionado pois no GPA a o valor é calculado na tela
                            ocorrencia.ValorOcorrencia = Servicos.Embarcador.CargaOcorrencia.Ocorrencia.CalcularValorOcorrenciaContrato(veiculosContrato, motoristaContrato);
                    }
                }
                else if (ocorrencia.OrigemOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemOcorrencia.PorCarga)
                {
                    if (ocorrencia.TipoOcorrencia.OcorrenciaPorQuantidade)
                    {
                        ocorrencia.ValorOcorrencia = ocorrencia.Quantidade * ocorrencia.TipoOcorrencia.Valor;
                    }
                }

                if (ocorrencia.TipoOcorrencia.GerarOcorrenciaComMesmoValorCTesAnteriores && ocorrencia.ValorOcorrencia == 0 && cargaCTEs.Count > 0)
                    ocorrencia.ValorOcorrencia = Math.Round(cargaCTEs.Sum(o => o.CTe.ValorFrete), 2);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRateioOcorrenciaLote? tipoRateioOcorrenciaLote = null;
                if (ocorrencia.TipoOcorrencia.TipoRateio.HasValue)
                {
                    if (ocorrencia.TipoOcorrencia.TipoRateio == ParametroRateioFormula.peso)
                        tipoRateioOcorrenciaLote = TipoRateioOcorrenciaLote.Peso;
                    else if (ocorrencia.TipoOcorrencia.TipoRateio == ParametroRateioFormula.porCTe)
                        tipoRateioOcorrenciaLote = TipoRateioOcorrenciaLote.QuantidadeCTe;
                    else if (ocorrencia.TipoOcorrencia.TipoRateio == ParametroRateioFormula.ValorMercadoria)
                        tipoRateioOcorrenciaLote = TipoRateioOcorrenciaLote.ValorMercadoria;
                }

                // Detalhes da ocorrencia
                string mensagemRetorno = string.Empty;
                if (!srvOcorrencia.FluxoGeralOcorrencia(ref ocorrencia, cargaCTEs, cargaDocumentosParaEmissaoNFSManual, ref mensagemRetorno, unitOfWork, TipoServicoMultisoftware, this.Usuario, this.ConfiguracaoEmbarcador, this.Cliente, Request.Params("CargaCTesImportados"), true, this.Usuario?.PerfilAcesso?.PermitirAbrirOcorrenciaAposPrazoSolicitacao ?? false, Auditado, tipoRateioOcorrenciaLote, false, ClienteAcesso.URLAcesso, listaOcorrenciaCargaCTeNotaFiscal))
                    throw new ControllerException(mensagemRetorno);

                int codigoGestaoDevolucao = Request.GetIntParam("CodigoGestaoDevolucao");

                if (codigoGestaoDevolucao > 0)
                {
                    Repositorio.Embarcador.Devolucao.GestaoDevolucao repositorioGestaoDevolucao = new Repositorio.Embarcador.Devolucao.GestaoDevolucao(unitOfWork);
                    ocorrencia.GestaoDevolucao = repositorioGestaoDevolucao.BuscarPorCodigo(codigoGestaoDevolucao);
                }

                // Dividir ocorrencia
                if (dividirOcorrencia)
                {
                    decimal.TryParse(Request.Params("ValorOcorrenciaDestino"), out decimal valorOcorrenciaDestino);

                    if (valorOcorrenciaDestino > 0)
                    {
                        Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrenciaDestino = new Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia();
                        this.DividirOcorrencia(ocorrencia, ref ocorrenciaDestino, unitOfWork);

                        if (ocorrenciaDestino.ValorOcorrencia > 0)
                        {
                            repOcorrencia.Inserir(ocorrenciaDestino, Auditado);

                            GerarParametrosOcorrencia(ocorrenciaDestino, unitOfWork);

                            mensagemRetorno = string.Empty;
                            if (!srvOcorrencia.FluxoGeralOcorrencia(ref ocorrenciaDestino, cargaCTEs, cargaDocumentosParaEmissaoNFSManual, ref mensagemRetorno, unitOfWork, TipoServicoMultisoftware, this.Usuario, this.ConfiguracaoEmbarcador, this.Cliente, Request.Params("CargaCTesImportados"), true, this.Usuario?.PerfilAcesso?.PermitirAbrirOcorrenciaAposPrazoSolicitacao ?? false, Auditado, tipoRateioOcorrenciaLote, false, ClienteAcesso.URLAcesso))
                                throw new ControllerException(mensagemRetorno);

                            ocorrencia.CargaOcorrenciaVinculada = ocorrenciaDestino.Codigo;

                            if (ocorrencia.ValorOcorrencia == 0)
                                ocorrencia.Inativa = true;
                        }
                    }
                }

                if (valorChamados > 0 && valorChamados != ocorrencia.ValorOcorrencia)
                    throw new ControllerException(Localization.Resources.Ocorrencias.Ocorrencia.OValorInformadoDifereDoValorTotalDosAtendimentosInformados);

                repOcorrencia.Atualizar(ocorrencia, Auditado);

                int codigoControleDocumento = Request.GetIntParam("ControleDocumento");
                if (codigoControleDocumento > 0)
                {
                    Repositorio.Embarcador.Documentos.ControleDocumento repControleDocumento = new Repositorio.Embarcador.Documentos.ControleDocumento(unitOfWork);
                    Dominio.Entidades.Embarcador.Documentos.ControleDocumento controleDocumento = repControleDocumento.BuscarPorCodigo(codigoControleDocumento);

                    if (controleDocumento != null)
                    {
                        controleDocumento.AcaoTratativa = AcaoTratativaIrregularidade.AnexarComplementar;
                        repControleDocumento.Atualizar(controleDocumento, Auditado);
                    }
                }

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe in cargaCTEs)
                {
                    cargaCTe.CTe.UsuarioEmissaoCTe = Usuario;
                    cargaCTe.CTe.CentroResultado = (configuracaoOcorrencia?.TrazerCentroResultadoOcorrencia ?? false) ? cargaCTe.Carga.TipoOperacao?.ConfiguracaoPagamentos?.CentroResultado : null;
                    repCargaCTe.Atualizar(cargaCTe);
                }

                unitOfWork.CommitChanges();

                if (ocorrencia.TipoOcorrencia?.EnviarEmailGeracaoOcorrencia ?? false)
                {
                    string stringConexao = _conexao.StringConexao;

                    Task.Factory.StartNew(() => EnviarEmailGeracaoOcorrencia(ocorrencia.Codigo, TipoServicoMultisoftware, stringConexao));
                }

                foreach (Dominio.Entidades.Embarcador.Chamados.Chamado chamado in chamadosFinalizar)
                    servicoChamado.EnviarEmailChamadoFinalizado(chamado, unitOfWork);

                dynamic retornoSalvar = RetornarOcorrencia(ocorrencia, unitOfWork);

                return new JsonpResult(retornoSalvar);
            }
            catch (BaseException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAdicionar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private static bool EmitirDocumentoParaFilialEmissoraComPreCTe(Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe, Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia)
        {
            return ocorrencia.TipoOcorrencia.EmitirDocumentoParaFilialEmissoraComPreCTe
                         && ocorrencia.TipoOcorrencia.TipoEmissaoDocumentoOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoDocumentoOcorrencia.SomenteFilialEmissora
                         && cargaCTe.CargaCTeFilialEmissora == null
                         && cargaCTe.CargaCTeSubContratacaoFilialEmissora != null
                         && cargaCTe.CargaCTeSubContratacaoFilialEmissora.CargaCTeFilialEmissora != null;
        }

        public async Task<IActionResult> Reabrir()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = int.Parse(Request.Params("codigo"));
                Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);
                Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao repositorioCargaOcorrenciaAutorizacao = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao(unitOfWork);

                Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia = repCargaOcorrencia.BuscarPorCodigo(codigo);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoOcorrencia configuracaoOcorrencia = new Repositorio.Embarcador.Configuracoes.ConfiguracaoOcorrencia(unitOfWork).BuscarConfiguracaoPadrao();

                if (configuracaoOcorrencia.PermitirReabrirOcorrenciaEmCasoDeRejeicao && cargaOcorrencia.TipoOcorrencia.PermitirReabrirOcorrenciaEmCasoDeRejeicao)
                {
                    cargaOcorrencia.SituacaoOcorrencia = SituacaoOcorrencia.AgInformacoes;
                    cargaOcorrencia.OcorrenciaReprovada = true;
                    repositorioCargaOcorrenciaAutorizacao.DeletarPorOcorrencia(cargaOcorrencia.Codigo);
                    repCargaOcorrencia.Atualizar(cargaOcorrencia);

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaOcorrencia, null, Localization.Resources.Ocorrencias.Ocorrencia.ReAbriuAOcorrencia, unitOfWork);
                }

                unitOfWork.CommitChanges();

                //var retorno = new
                //{
                //    cargaOcorrencia.SituacaoOcorrencia,
                //    cargaOcorrencia.Codigo
                //};
                //return new JsonpResult(retorno);

                dynamic retornoOcorrenciaFormatada = RetornarOcorrencia(cargaOcorrencia, unitOfWork);

                return new JsonpResult(retornoOcorrenciaFormatada);

            }
            catch (ServicoException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, ex.ToString());
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Ocorrencias.Ocorrencia.OcorreuUmaFalhaAoReabrirAOcorrencia);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ConfirmarUtilizacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = int.Parse(Request.Params("codigo"));
                Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);
                Repositorio.Embarcador.Creditos.CreditoDisponivelUtilizado repCreditoDisponivelUtilizado = new Repositorio.Embarcador.Creditos.CreditoDisponivelUtilizado(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(unitOfWork);

                Servicos.Embarcador.Carga.Ocorrencia serCargaOcorrencia = new Servicos.Embarcador.Carga.Ocorrencia(unitOfWork);
                Servicos.Embarcador.Carga.RateioCTeComplementar serRateioCTeComplementar = new Servicos.Embarcador.Carga.RateioCTeComplementar(unitOfWork);
                Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia = repCargaOcorrencia.BuscarPorCodigo(codigo);

                Servicos.Embarcador.Credito.CreditoMovimentacao serCreditoMovimentacao = new Servicos.Embarcador.Credito.CreditoMovimentacao(unitOfWork);
                Servicos.Embarcador.Carga.CTeComplementar serCargaCTeComplementar = new Servicos.Embarcador.Carga.CTeComplementar(unitOfWork);
                Servicos.Embarcador.Carga.Ocorrencia serOcorrencia = new Servicos.Embarcador.Carga.Ocorrencia(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaOcorrencia.Carga;

                List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> cargaCTesComplementaresInfo = repCargaCTeComplementoInfo.BuscarPorOcorrencia(cargaOcorrencia.Codigo);

                if (cargaOcorrencia.SolicitacaoCredito != null)
                {
                    List<Dominio.Entidades.Embarcador.Creditos.CreditoDisponivelUtilizado> utilizados = repCreditoDisponivelUtilizado.BuscarPorOcorrencia(cargaOcorrencia.Codigo);
                    serCreditoMovimentacao.ConfirmarUtilizacaoCreditos(utilizados, unitOfWork);

                    cargaOcorrencia.ValorOcorrencia = cargaOcorrencia.SolicitacaoCredito.ValorLiberado + utilizados.Sum(obj => obj.ValorUtilizado);

                    if (cargaOcorrencia.ComponenteFrete.SomarComponenteFreteLiquido)
                        cargaOcorrencia.ValorOcorrenciaLiquida = cargaOcorrencia.ValorOcorrencia;

                    cargaOcorrencia.SituacaoOcorrencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.EmEmissaoCTeComplementar; //Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.AgEmissaoCTeComplementar;
                    repCargaOcorrencia.Atualizar(cargaOcorrencia);
                }

                //if (cargaOcorrencia.SituacaoOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.AgEmissaoCTeComplementar)
                if (cargaOcorrencia.SituacaoOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.EmEmissaoCTeComplementar)
                {
                    serOcorrencia.ValidarEnviarEmissaoComplementosOcorrencia(cargaOcorrencia, unitOfWork);
                }
                else
                {
                    cargaOcorrencia.SituacaoOcorrencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.Finalizada;
                    repCargaOcorrencia.Atualizar(cargaOcorrencia);
                }

                Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaOcorrencia, null, Localization.Resources.Ocorrencias.Ocorrencia.ConfirmouUtilizacaoDeCredito, unitOfWork);

                unitOfWork.CommitChanges();

                var retorno = new
                {
                    cargaOcorrencia.SituacaoOcorrencia,
                    cargaOcorrencia.Codigo
                };
                return new JsonpResult(retorno);

            }
            catch (ServicoException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, ex.ToString());
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Ocorrencias.Ocorrencia.OcorreuUmaFalhaAoExtornarOUsoDoComponente);
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
                Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);

                int.TryParse(Request.Params("Codigo"), out int codigo);

                Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia = repOcorrencia.BuscarPorCodigoComFetch(codigo);
                if (ocorrencia == null)
                    return new JsonpResult(false, true, Localization.Resources.Ocorrencias.Ocorrencia.NaoFoiPossivelEncontrarOsDados);

                dynamic retornoOcorrenciaFormatada = RetornarOcorrencia(ocorrencia, unitOfWork);

                return new JsonpResult(retornoOcorrenciaFormatada);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoBuscarPorCodigo);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ReenviarIntegracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);

                int.TryParse(Request.Params("Codigo"), out int codigo);

                Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia = repOcorrencia.BuscarPorCodigo(codigo);

                if (ocorrencia == null)
                    return new JsonpResult(false, true, Localization.Resources.Ocorrencias.Ocorrencia.NaoFoiPossivelEncontrarOsDados);

                if (ocorrencia.SituacaoOcorrencia != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.EmEmissaoCTeComplementar)
                    return new JsonpResult(false, true, Localization.Resources.Ocorrencias.Ocorrencia.ASituacaoDaOcorrenciaNaoPermiteEssaAlteracao);

                if (ocorrencia.ErroIntegracaoComGPA)
                {
                    unitOfWork.Start();

                    ocorrencia.ErroIntegracaoComGPA = false;
                    ocorrencia.IntegradoComGPA = false;

                    repOcorrencia.Atualizar(ocorrencia);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, ocorrencia, null, Localization.Resources.Ocorrencias.Ocorrencia.ReenviouAIntegracao, unitOfWork);

                    unitOfWork.CommitChanges();
                }

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return new JsonpResult(false, Localization.Resources.Ocorrencias.Ocorrencia.OcorreuUmaFalhaAoReenviar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AtualizarRegrasEtapas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Inicia instancia
                unitOfWork.Start();

                // Repositoriso
                Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);
                Servicos.Embarcador.CargaOcorrencia.Ocorrencia srvOcorrencia = new Servicos.Embarcador.CargaOcorrencia.Ocorrencia(unitOfWork);

                // Converte parametros
                int.TryParse(Request.Params("Ocorrencia"), out int codigo);

                // Busca Ocorrencia
                Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia = repOcorrencia.BuscarPorCodigo(codigo);

                // Valida
                if (ocorrencia == null)
                    return new JsonpResult(false, true, Localization.Resources.Ocorrencias.Ocorrencia.OcorrenciaNaoEncontrada);

                // Verifica qual regras consultar
                bool atualizarOcorrencia = false;
                List<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.Notificacao> notificoes = new List<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.Notificacao>();
                if (ocorrencia.SituacaoOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.SemRegraAprovacao)
                {
                    // Busca se ha regras e cria
                    ocorrencia.SituacaoOcorrencia = srvOcorrencia.VerificarRegrasAutorizacaoAprovacaoOcorrencia(ocorrencia, out notificoes, TipoServicoMultisoftware, unitOfWork, this.Usuario, ClienteAcesso.URLAcesso);

                    if (ocorrencia.SituacaoOcorrencia == SituacaoOcorrencia.SemRegraEmissao ||
                        ocorrencia.SituacaoOcorrencia == SituacaoOcorrencia.AgAutorizacaoEmissao ||
                        ocorrencia.SituacaoOcorrencia == SituacaoOcorrencia.EmEmissaoCTeComplementar) //ocorrencia.SituacaoOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.AgEmissaoCTeComplementar)
                        Servicos.Embarcador.Escrituracao.DocumentoProvisao.AdicionarDocumentosParaProvisaoOcorrencia(ocorrencia, TipoServicoMultisoftware, unitOfWork, ConfiguracaoEmbarcador);

                    if ((ocorrencia.SituacaoOcorrencia == SituacaoOcorrencia.EmEmissaoCTeComplementar) && !ocorrencia.DataAprovacao.HasValue)
                        ocorrencia.DataAprovacao = DateTime.Now;

                    atualizarOcorrencia = true;
                }
                else if (ocorrencia.SituacaoOcorrencia == SituacaoOcorrencia.SemRegraEmissao)
                {
                    // Busca se ha regras e cria
                    ocorrencia.SituacaoOcorrencia = srvOcorrencia.VerificarRegrasAutorizacaoEmissaoOcorrencia(ocorrencia, out notificoes, TipoServicoMultisoftware, unitOfWork, this.Usuario);

                    if ((ocorrencia.SituacaoOcorrencia == SituacaoOcorrencia.EmEmissaoCTeComplementar) && !ocorrencia.DataAprovacao.HasValue)
                        ocorrencia.DataAprovacao = DateTime.Now;

                    atualizarOcorrencia = true;
                }

                Servicos.Embarcador.Notificacao.Notificacao serNotificacao = new Servicos.Embarcador.Notificacao.Notificacao(unitOfWork.StringConexao, null, TipoServicoMultisoftware, string.Empty);
                foreach (Dominio.ObjetosDeValor.Embarcador.Ocorrencia.Notificacao notificacao in notificoes)
                {
                    string titulo = Localization.Resources.Ocorrencias.Ocorrencia.DescricaoOcorrencia;
                    if (ocorrencia.Carga != null)
                        titulo = string.Concat(ocorrencia.Carga.Filial?.Descricao, Localization.Resources.Ocorrencias.Ocorrencia.OcorrenciaBarra, ocorrencia.NumeroOcorrencia, " - ", ocorrencia.TipoOcorrencia?.Descricao, Localization.Resources.Ocorrencias.Ocorrencia.CargaBarra, ocorrencia.Carga.CodigoCargaEmbarcador);
                    else
                        titulo = string.Concat(Localization.Resources.Ocorrencias.Ocorrencia.DescricaoOcorrencia, ocorrencia.NumeroOcorrencia, " - ", ocorrencia.TipoOcorrencia?.Descricao);

                    serNotificacao.GerarNotificacaoEmail(notificacao.Aprovador, ocorrencia.Usuario, ocorrencia.Codigo, "Ocorrencias/AutorizacaoOcorrencia", titulo, notificacao.Mensagem, Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.cifra, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.credito, TipoServicoMultisoftware, unitOfWork);
                }

                Servicos.Embarcador.CargaOcorrencia.Ocorrencia.ExecutaProximoPassoOcorrencia(ocorrencia, _conexao.StringConexao, unitOfWork);

                // Retorno de informacoes
                var retorno = new
                {
                    ocorrencia.SituacaoOcorrencia
                };

                // Atualiza a ocorrencia
                if (atualizarOcorrencia)
                    repOcorrencia.Atualizar(ocorrencia);
                else
                    retorno = null;

                // Finaliza instancia
                unitOfWork.CommitChanges();

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Ocorrencias.Ocorrencia.OcorreuUmaFalhaAoBuscarInformacoes);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> CalcularValorTipoOcorrencia()
        {
            /* Calcula o valor da ocorrencia a partir da formula:
             * Valor Diária x Cargas
             * 
             * Valor Diária esta temporariamente setado do Tipo Ocorrencia
             */
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Servicos.Embarcador.CargaOcorrencia.Ocorrencia srvOcorrencia = new Servicos.Embarcador.CargaOcorrencia.Ocorrencia(unitOfWork);

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

                int transportador = 0;
                string proprietario = "";

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Terceiros)
                {
                    Dominio.Entidades.Empresa empresaTerceiro = repEmpresa.BuscarPorCNPJ(Usuario.ClienteTerceiro.CPF_CNPJ_SemFormato);

                    transportador = empresaTerceiro != null ? empresaTerceiro.Codigo : 0;
                    proprietario = Usuario.ClienteTerceiro != null ? Usuario.ClienteTerceiro.CPF_CNPJ_SemFormato : string.Empty;
                }
                else if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                {
                    transportador = Usuario.Empresa.Codigo;
                    proprietario = Usuario.Empresa != null ? Usuario.Empresa.CNPJ_SemFormato : string.Empty;
                }
                else
                {
                    int.TryParse(Request.Params("Transportador"), out transportador);
                    Dominio.Entidades.Empresa transportadoraSelecionada = repEmpresa.BuscarPorCodigo(transportador);
                    proprietario = transportadoraSelecionada?.CNPJ_SemFormato ?? string.Empty;
                }

                int.TryParse(Request.Params("Filial"), out int filial);

                int.TryParse(Request.Params("TipoOcorrencia"), out int codigoTipoOcorrencia);
                DateTime.TryParse(Request.Params("PeriodoInicio"), out DateTime periodoInicial);
                DateTime.TryParse(Request.Params("PeriodoFim"), out DateTime periodoFim);


                dynamic cargasComplementadasDias = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("CargasComplementadasDias"));

                decimal valorOcorrencia = srvOcorrencia.CalcularValorOcorrenciaPorTipoOcorrencia(codigoTipoOcorrencia, periodoInicial, periodoFim, transportador, filial, proprietario, cargasComplementadasDias, out string erro);
                if (!string.IsNullOrWhiteSpace(erro))
                    return new JsonpResult(false, true, erro);

                var dynValores = new
                {
                    ValorOcorrencia = valorOcorrencia.ToString("n2"),
                };

                return new JsonpResult(dynValores);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Ocorrencias.Ocorrencia.OcorreuUmaFalhaAoCalcularOValorDaOcorrencia);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> CalcularValorOcorrencia()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Servicos.Embarcador.CargaOcorrencia.Ocorrencia servicoOcorrencia = new Servicos.Embarcador.CargaOcorrencia.Ocorrencia();
                Servicos.Embarcador.Carga.Ocorrencia servicoOcorrenciaCalculoFrete = new Servicos.Embarcador.Carga.Ocorrencia();
                Servicos.Embarcador.CargaOcorrencia.OcorrenciaAutomaticaPorPeriodo servicoGatilhoOcorrencia = new Servicos.Embarcador.CargaOcorrencia.OcorrenciaAutomaticaPorPeriodo(unitOfWork);

                Repositorio.TipoDeOcorrenciaDeCTe repTipoDeOcorrencia = new Repositorio.TipoDeOcorrenciaDeCTe(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Ocorrencias.GatilhoGeracaoAutomaticaOcorrencia repGatilhoGeracaoAutomaticaOcorrencia = new Repositorio.Embarcador.Ocorrencias.GatilhoGeracaoAutomaticaOcorrencia(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Ocorrencia.ParametroCalcularValorOcorrencia parametrosCalcularValorOcorrencia = new Dominio.ObjetosDeValor.Embarcador.Ocorrencia.ParametroCalcularValorOcorrencia()
                {
                    ApenasReboque = Request.GetBoolParam("ApenasReboque"),
                    CodigoCarga = Request.GetIntParam("Carga"),
                    CodigoParametroBooleano = Request.GetIntParam("CodigoParametroBooleano"),
                    CodigoParametroInteiro = Request.GetIntParam("CodigoParametroInteiro"),
                    CodigoParametroPeriodo = Request.GetIntParam("CodigoParametroPeriodo"),
                    CodigoParametroData = Request.GetIntParam("CodigoParametroData1"),
                    CodigoTipoOcorrencia = Request.GetIntParam("TipoOcorrencia"),
                    QuantidadeAjudantes = Request.GetIntParam("QuantidadeAjudantes"),
                    DataFim = Request.GetDateTimeParam("DataFim"),
                    DataInicio = Request.GetDateTimeParam("DataInicio"),
                    ParametroData = Request.GetDateTimeParam("ParametroData1"),
                    Minutos = Request.GetIntParam("ParametroInteiro"),
                    KmInformado = Request.GetIntParam("ParametroTexto"),
                    PermiteInformarValor = Request.GetBoolParam("PermiteInformarValor"),
                    ValorOcorrencia = Request.GetDecimalParam("ValorOcorrencia")
                };

                Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoOcorrencia = repTipoDeOcorrencia.BuscarPorCodigo(parametrosCalcularValorOcorrencia.CodigoTipoOcorrencia);

                if (tipoOcorrencia?.EfetuarCalculoValorOcorrenciaBaseadoNotasDevolucao ?? false)
                    throw new ServicoException("Esta ocorrência não pode ser calculada com base em notas de devolução. Crie a ocorrência em Atendimentos ou desative o cálculo por notas de devolução no Tipo de Ocorrência.");

                if (tipoOcorrencia?.UtilizarEntradaSaidaDoRaioCargaEntrega ?? false)
                {
                    if ((parametrosCalcularValorOcorrencia.DataFim - parametrosCalcularValorOcorrencia.DataInicio).TotalHours < tipoOcorrencia.HorasToleranciaEntradaSaidaRaio)
                        return new JsonpResult(false, true, string.Format(Localization.Resources.Ocorrencias.Ocorrencia.TempoEstadiaEMenorQUENaoSeraGaradaNenhumaOcorrencia, tipoOcorrencia.HorasToleranciaEntradaSaidaRaio));
                }

                if (tipoOcorrencia?.EfetuarOControleQuilometragem ?? false)
                    parametrosCalcularValorOcorrencia.KmInformado = (int)Math.Round(Request.GetDecimalParam("Quilometragem"));

                parametrosCalcularValorOcorrencia.HorasSemFranquia = tipoOcorrencia?.HorasSemFranquia ?? 0;

                Dominio.Entidades.Embarcador.Ocorrencias.GatilhoGeracaoAutomaticaOcorrencia gatilho = repGatilhoGeracaoAutomaticaOcorrencia.BuscarPorTipoOcorrencia(parametrosCalcularValorOcorrencia.CodigoTipoOcorrencia);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(parametrosCalcularValorOcorrencia.CodigoCarga);
                bool selecionarTodosCtes = Request.GetBoolParam("SelecionarTodos");
                double cnpjdestinatario = Request.GetDoubleParam("Destinatario");
                int numeroNF = Request.GetIntParam("NumeroNF");
                int numeroDocumento = Request.GetIntParam("NumeroDocumento");

                if (gatilho != null && carga != null)
                {
                    parametrosCalcularValorOcorrencia.DeducaoHoras = servicoGatilhoOcorrencia.ObterHorasDeducaoPorGatilho(carga, gatilho);

                    if (!gatilho.GerarAutomaticamente)
                    {
                        (DateTime? DataInicio, DateTime? DataFim) dados = servicoGatilhoOcorrencia.ObterDataInicioEFimGatilho(parametrosCalcularValorOcorrencia.ParametroData, parametrosCalcularValorOcorrencia.DataInicio, parametrosCalcularValorOcorrencia.DataFim);

                        if (dados.DataInicio.HasValue && dados.DataFim.HasValue)
                        {
                            parametrosCalcularValorOcorrencia.DataInicio = dados.DataInicio.Value;
                            parametrosCalcularValorOcorrencia.DataFim = dados.DataFim.Value;
                        }
                    }
                }

                if ((tipoOcorrencia?.DebitaFreeTimeCalculoValorOcorrencia ?? false) && (tipoOcorrencia?.FreeTime ?? 0) > 0)
                {
                    parametrosCalcularValorOcorrencia.FreeTime = tipoOcorrencia.FreeTime;
                }

                parametrosCalcularValorOcorrencia.ListaCargaCTe = await servicoOcorrencia.BuscarCTesSelecionadosOuCargas(ObterFiltrosDePesquisaConsultaCtes(unitOfWork), carga, ConfiguracaoEmbarcador, unitOfWork, Request.Params("CargaCTes"), selecionarTodosCtes, numeroNF, cnpjdestinatario, TipoServicoMultisoftware, Usuario, numeroDocumento);
                if (tipoOcorrencia?.CalcularDistanciaPorCTe ?? false)
                    parametrosCalcularValorOcorrencia.KmInformado = servicoOcorrenciaCalculoFrete.CalcularDistanciaCTEsOcorrencia(parametrosCalcularValorOcorrencia.ListaCargaCTe, unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Ocorrencia.CalculoFreteOcorrencia calculoFreteOcorrencia = servicoOcorrenciaCalculoFrete.CalcularValorOcorrencia(parametrosCalcularValorOcorrencia, unitOfWork, ConfiguracaoEmbarcador, TipoServicoMultisoftware);

                if (calculoFreteOcorrencia.ValorCalculadoPorTabelaFrete)
                    return new JsonpResult(new
                    {
                        calculoFreteOcorrencia.IncluirICMSFrete,
                        calculoFreteOcorrencia.ObservacaoCTe,
                        calculoFreteOcorrencia.HorasOcorrencia,
                        TotalHoras = calculoFreteOcorrencia.HorasOcorrencia.FromHoursToFormattedTime(),
                        ValorOcorrencia = calculoFreteOcorrencia.ValorOcorrencia.ToString("n2")
                    });

                return new JsonpResult(new
                {
                    calculoFreteOcorrencia.DividirOcorrencia,
                    calculoFreteOcorrencia.HorasOcorrencia,
                    calculoFreteOcorrencia.HorasOcorrenciaDestino,
                    calculoFreteOcorrencia.IncluirICMSFrete,
                    calculoFreteOcorrencia.ObservacaoCTe,
                    calculoFreteOcorrencia.ObservacaoCTeDestino,
                    calculoFreteOcorrencia.ObservacaoOcorrencia,
                    calculoFreteOcorrencia.ObservacaoOcorrenciaDestino,
                    TotalHoras = calculoFreteOcorrencia.HorasOcorrencia.FromHoursToFormattedTime(),
                    PercentualAcrescimoValor = calculoFreteOcorrencia.PercentualAcrescimoValor.ToString("n2"),
                    ValorOcorrencia = calculoFreteOcorrencia.ValorOcorrencia.ToString("n2"),
                    ValorOcorrenciaDestino = calculoFreteOcorrencia.ValorOcorrenciaDestino.ToString("n2")
                });
            }
            catch (ServicoException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Ocorrencias.Ocorrencia.OcorreuUmaFalhaAoBuscarPorValorFreteOcorrencia);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ObterVeiculosImprodutivos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.CargaPercurso repCargaPercurso = new Repositorio.Embarcador.Cargas.CargaPercurso(unitOfWork);
                Repositorio.Embarcador.Frete.ContratoFreteTransportador repContratoFreteTransportador = new Repositorio.Embarcador.Frete.ContratoFreteTransportador(unitOfWork);
                Repositorio.Embarcador.Ocorrencias.OcorrenciaContratoVeiculo repOcorrenciaContratoVeiculo = new Repositorio.Embarcador.Ocorrencias.OcorrenciaContratoVeiculo(unitOfWork);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.Ocorrencia.Placa, "Placa", 25, Models.Grid.Align.left, false);

                int.TryParse(Request.Params("Codigo"), out int codigo);

                if (codigo > 0)
                {
                    Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador contratoFreteTransportador = repContratoFreteTransportador.BuscarPorCodigo(codigo);

                    List<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaContratoVeiculo> veiculos = repOcorrenciaContratoVeiculo.BuscarPorOcorrencia(codigo);
                    var lista = (from o in veiculos
                                 select new
                                 {
                                     o.Veiculo.Codigo,
                                     o.Veiculo.Placa
                                 }).ToList();

                    grid.AdicionaRows(lista);
                    grid.setarQuantidadeTotal(veiculos.Count);
                }
                else
                {
                    DateTime.TryParse(Request.Params("PeriodoInicio"), out DateTime periodoInicial);
                    DateTime.TryParse(Request.Params("PeriodoFim"), out DateTime periodoFinal);

                    int.TryParse(Request.Params("ContratoFreteTransportador"), out int contrato);

                    List<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorVeiculo> veiculos = repCargaPercurso.ConsultarVeiculosImprodutivos(periodoInicial, periodoFinal, contrato);
                    var lista = (from o in veiculos
                                 select new
                                 {
                                     o.Veiculo.Codigo,
                                     o.Veiculo.Placa
                                 }).ToList();

                    grid.AdicionaRows(lista);
                    grid.setarQuantidadeTotal(veiculos.Count);
                }

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ConsultarCTesOcorrencia()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoOcorrencia = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repositorioOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);
                Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia = repositorioOcorrencia.BuscarPorCodigo(codigoOcorrencia);

                if (ocorrencia.SituacaoOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.PendenciaEmissao)// se está com pendencia na emissão o sistema tente ver se autorizou os rejeitados para o fluxo andar.
                {
                    Servicos.Embarcador.Carga.Ocorrencia servicoOcorrencia = new Servicos.Embarcador.Carga.Ocorrencia(unitOfWork);
                    servicoOcorrencia.ValidarEmissaoComplementosOcorrencia(ocorrencia, unitOfWork, WebServiceConsultaCTe, TipoServicoMultisoftware, ConfiguracaoEmbarcador, false, Auditado, WebServiceOracle, clienteMultisoftware: Cliente);
                }

                bool cteFilialEmissora = Request.GetBoolParam("CTeFilialEmissora");
                string propriedadeOrdenar = "";
                Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repositorioCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(unitOfWork);
                Repositorio.Embarcador.Ocorrencias.OcorrenciaCancelamento repOcorrenciaCancelamento = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCancelamento(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
                Models.Grid.Grid grid = MontarGridCTes(ref propriedadeOrdenar, exibirEspecieDocumento: (configuracaoEmbarcador?.ExibirEspecieDocumentoCteComplementarOcorrencia ?? false));
                List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> cargaCTesComplementoInfo = repositorioCargaCTeComplementoInfo.BuscarCTesPorOcorrencia(ocorrencia.Codigo, cteFilialEmissora, propriedadeOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCancelamento ocorrenciaCancelamento = repOcorrenciaCancelamento.BuscarPorOcorrencia(ocorrencia.Codigo);

                grid.setarQuantidadeTotal(repositorioCargaCTeComplementoInfo.ContarPorCTEsOcorrencia(ocorrencia.Codigo, cteFilialEmissora));
                grid.AdicionaRows(MontarListaCTes(cargaCTesComplementoInfo, ocorrenciaCancelamento));

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ConsultarNFSManualOcorrencia()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoOcorrencia = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual repCargaNFeParaEmissaoNFSManual = new Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual(unitOfWork);
                Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repositorioOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);
                Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia = repositorioOcorrencia.BuscarPorCodigo(codigoOcorrencia);

                if (ocorrencia == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.Ocorrencia.Numero, "Numero", 5, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.Ocorrencia.Chave, "Chave", 18, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.Ocorrencia.Documento, "Abreviacao", 5, Models.Grid.Align.center, false);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.Ocorrencia.Remetente, "Remetente", 18, Models.Grid.Align.left, true);

                grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.Ocorrencia.Destinatario, "Destinatario", 18, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.Ocorrencia.Destino, "Destino", 13, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.Ocorrencia.Peso, "Peso", 6, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.Ocorrencia.ValorDoFrete, "ValorFrete", 6, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.Ocorrencia.NotaFiscalDeServico, "NFS", 6, Models.Grid.Align.left, true);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                if (propOrdenar == "Remetente" || propOrdenar == "Destinatario")
                    propOrdenar += ".Nome";

                if (propOrdenar == "Destino")
                    propOrdenar = "Destinatario.Localidade.Descricao";

                List<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> registros = new List<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual>();
                int totalRegistro = repCargaNFeParaEmissaoNFSManual.ContarConsultaPorOcorrencia(ocorrencia.Codigo);

                if (totalRegistro > 0)
                    registros = repCargaNFeParaEmissaoNFSManual.ConsultarPorOcorrencia(ocorrencia.Codigo, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);

                grid.setarQuantidadeTotal(totalRegistro);
                grid.AdicionaRows((from obj in registros
                                   select new
                                   {
                                       obj.Codigo,
                                       obj.Numero,
                                       obj.Chave,
                                       obj.ModeloDocumentoFiscal.Abreviacao,
                                       Remetente = obj.Remetente.Nome + "(" + obj.Remetente.CPF_CNPJ_Formatado + ")",
                                       Destinatario = obj.Destinatario.Nome + "(" + obj.Destinatario.CPF_CNPJ_Formatado + ")",
                                       Destino = obj.Destinatario.Localidade.DescricaoCidadeEstado,
                                       ValorFrete = obj.ValorFrete.ToString(),
                                       Peso = obj.Peso.ToString(),
                                       NFS = obj.CTe != null ? obj.CTe.Numero.ToString() : "",
                                       DT_RowColor = obj.CTe != null ? "#dff0d8" : "#fcf8e3"
                                   }));

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> VerificarTomadorOcorrenciaEmiteCTeNoEmbarcador()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Servicos.Embarcador.CargaOcorrencia.Ocorrencia srvOcorrencia = new Servicos.Embarcador.CargaOcorrencia.Ocorrencia(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
                Repositorio.ConhecimentoDeTransporteEletronico repConhecimento = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);

                int.TryParse(Request.Params("Carga"), out int codigoCarga);
                int.TryParse(Request.Params("Conhecimento"), out int codigoConhecimento);

                Dominio.Enumeradores.TipoTomador? tipoTomador = null;
                if (Request.Params("TipoTomador") != "99" && Enum.TryParse(Request.Params("TipoTomador"), out Dominio.Enumeradores.TipoTomador tipoTomadorAux))
                    tipoTomador = tipoTomadorAux;

                double.TryParse(Request.Params("Tomador"), out double cpfCnpjTomador);

                bool cteEmitidoNoEmbarcador = srvOcorrencia.RetornarTomadorEmiteCTeNoEmbarcador(null, codigoCarga, tipoTomador, cpfCnpjTomador, unitOfWork);

                List<dynamic> ctesImportados = new List<dynamic>();
                decimal valorTotalOcorrencia = 0;
                string observacaoOcorrencia = "";
                if (codigoConhecimento > 0)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = repCargaCTe.BuscarUltimaCargaDoCTe(codigoConhecimento);
                    if (cargaCTe != null)
                    {
                        var cteComplementado = new
                        {
                            CodigoCargaCTeParaComplementar = cargaCTe.Codigo,
                            CodigoCTeComplemetarImportado = cargaCTe.CTe.Codigo,
                            NumeroCTeComplemetarImportado = cargaCTe.CTe.Numero,
                            ValorCTeComplemetarImportado = cargaCTe.CTe.ValorAReceber,
                        };
                        ctesImportados.Add(cteComplementado);
                        valorTotalOcorrencia += cargaCTe.CTe.ValorAReceber;
                        observacaoOcorrencia = cargaCTe.CTe.ObservacoesGerais;
                    }
                }
                else if (cteEmitidoNoEmbarcador)
                {

                    List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctesComplementaresSemCarga = repConhecimento.ConsultarCTesSemCargaComplementares("A");

                    foreach (Dominio.Entidades.ConhecimentoDeTransporteEletronico cteComplementar in ctesComplementaresSemCarga)
                    {
                        Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = repCargaCTe.BuscarPorCargaEChaveCTe(codigoCarga, cteComplementar.ChaveCTESubComp);
                        if (cargaCTe != null)
                        {
                            var cteComplementado = new
                            {
                                CodigoCargaCTeParaComplementar = cargaCTe.Codigo,
                                CodigoCTeComplemetarImportado = cteComplementar.Codigo,
                                NumeroCTeComplemetarImportado = cteComplementar.Numero,
                                ValorCTeComplemetarImportado = cteComplementar.ValorAReceber,
                            };
                            ctesImportados.Add(cteComplementado);
                            valorTotalOcorrencia += cteComplementar.ValorAReceber;
                            observacaoOcorrencia = cteComplementar.ObservacoesGerais;
                        }
                    }
                }

                var retorno = new
                {
                    CTesImportados = ctesImportados,
                    ValorOcorrencia = valorTotalOcorrencia.ToString("n2"),
                    Observacao = observacaoOcorrencia,
                    CTeEmitidoNoEmbarcador = cteEmitidoNoEmbarcador
                };

                return new JsonpResult(retorno);
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

        public async Task<IActionResult> ConsultaCTesComplementados()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);
                Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaDocumento repCargaOcorrenciaDocumento = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaDocumento(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);

                int codOcorrencia = int.Parse(Request.Params("Codigo"));
                bool utilizarSelecaoPorNotasFiscaisCTe = Request.GetBoolParam("UtilizarSelecaoPorNotasFiscaisCTe");

                Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia = repOcorrencia.BuscarPorCodigo(codOcorrencia);

                if (ocorrencia.SituacaoOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.PendenciaEmissao)// se está com pendencia na emissão o sistema tente ver se autorizou os rejeitados para o fluxo andar.
                {
                    Servicos.Embarcador.Carga.Ocorrencia serOcorrencia = new Servicos.Embarcador.Carga.Ocorrencia(unitOfWork);
                    serOcorrencia.ValidarEmissaoComplementosOcorrencia(ocorrencia, unitOfWork, WebServiceConsultaCTe, TipoServicoMultisoftware, ConfiguracaoEmbarcador, false, Auditado, WebServiceOracle, clienteMultisoftware: Cliente);
                }
                string propOrdenacao = "";
                Models.Grid.Grid grid = MontarGridCTes(ref propOrdenacao, exibirEspecieDocumento: false);

                if (utilizarSelecaoPorNotasFiscaisCTe)
                {
                    List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento> cargaOcorrenciaDocumentos = repCargaOcorrenciaDocumento.BuscarPorOcorrencia(ocorrencia.Codigo);
                    grid.setarQuantidadeTotal(cargaOcorrenciaDocumentos.Sum(o => o.XMLNotaFiscais.Count));
                    grid.AdicionaRows(MontarListaCTesNotaFiscais(cargaOcorrenciaDocumentos));
                }
                else
                {
                    List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> CargaCTes = repCargaOcorrenciaDocumento.BuscarCTesPorOcorrencia(ocorrencia.Codigo, propOrdenacao, grid.dirOrdena, grid.inicio, grid.limite);
                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe> cargaPedidosXMLsNotaFiscalCTe = repCargaPedidoXMLNotaFiscalCTe.BuscarPorCargaPedidoXMLNotaFiscalCTePorCargaCTe((from obj in CargaCTes select obj.Codigo).ToList());
                    grid.setarQuantidadeTotal(repCargaOcorrenciaDocumento.ContarPorCTEsOcorrencia(ocorrencia.Codigo));
                    grid.AdicionaRows(MontarListaCTes(CargaCTes, cargaPedidosXMLsNotaFiscalCTe));
                }

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ConsultaDocumentosParaEmissaoNFSManualComplementados()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);
                Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaDocumento repCargaOcorrenciaDocumento = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaDocumento(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);

                int codOcorrencia = int.Parse(Request.Params("Codigo"));

                Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia = repOcorrencia.BuscarPorCodigo(codOcorrencia);

                string propOrdenacao = "";
                Models.Grid.Grid grid = MontarGridDocumentosNFSManual(ref propOrdenacao);

                List<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> cargaDocumentosParaEmissaoNFSmanual = repCargaOcorrenciaDocumento.BuscarNFSManualPorOcorrencia(ocorrencia.Codigo);

                grid.setarQuantidadeTotal(cargaDocumentosParaEmissaoNFSmanual.Count);
                grid.AdicionaRows((from obj in cargaDocumentosParaEmissaoNFSmanual
                                   select new
                                   {
                                       obj.Codigo,
                                       obj.Numero,
                                       obj.Chave,
                                       obj.ModeloDocumentoFiscal.Abreviacao,
                                       Remetente = obj.Remetente.Nome + "(" + obj.Remetente.CPF_CNPJ_Formatado + ")",
                                       Destinatario = obj.Destinatario.Nome + "(" + obj.Destinatario.CPF_CNPJ_Formatado + ")",
                                       Destino = obj.Destinatario.Localidade.DescricaoCidadeEstado,
                                       ValorFrete = obj.ValorFrete.ToString(),
                                       Peso = obj.Peso.ToString(),
                                       NFS = obj.CTe != null ? obj.CTe.Numero.ToString() : "",
                                       DT_RowColor = obj.CTe != null ? "#dff0d8" : "#fcf8e3"
                                   }).ToList());

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> DetalhesAutorizacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                // Instancia
                Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao repCargaOcorrenciaAutorizacao = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao(unitOfWork);

                // Converte dados
                int codigoAutorizacao = int.Parse(Request.Params("Codigo"));

                // Busca a autorizacao
                Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao cargaOcorrenciaAutorizacao = repCargaOcorrenciaAutorizacao.BuscarPorCodigo(codigoAutorizacao);

                var retorno = new
                {
                    cargaOcorrenciaAutorizacao.Codigo,
                    Regra = TituloRegra(cargaOcorrenciaAutorizacao),
                    SituacaoDescricao = cargaOcorrenciaAutorizacao.DescricaoSituacao,
                    cargaOcorrenciaAutorizacao.Bloqueada,
                    Usuario = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe ? cargaOcorrenciaAutorizacao.Usuario?.ExibirUsuarioAprovacao ?? false ? cargaOcorrenciaAutorizacao.Usuario?.Nome ?? string.Empty : cargaOcorrenciaAutorizacao.RegrasAutorizacaoOcorrencia?.Descricao ?? string.Empty : cargaOcorrenciaAutorizacao.Usuario?.Nome ?? string.Empty,
                    Data = cargaOcorrenciaAutorizacao.Data.HasValue ? cargaOcorrenciaAutorizacao.Data.Value.ToString("dd/MM/yyyy") : string.Empty,
                    Justificativa = cargaOcorrenciaAutorizacao.MotivoRejeicaoOcorrencia != null ? cargaOcorrenciaAutorizacao.MotivoRejeicaoOcorrencia.Descricao : string.Empty,
                    Motivo = !string.IsNullOrWhiteSpace(cargaOcorrenciaAutorizacao.Motivo) ? cargaOcorrenciaAutorizacao.Motivo : string.Empty,
                    CodigoUsuario = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe ? cargaOcorrenciaAutorizacao.Usuario?.ExibirUsuarioAprovacao ?? false ? cargaOcorrenciaAutorizacao.Usuario?.Codigo ?? 0 : cargaOcorrenciaAutorizacao.RegrasAutorizacaoOcorrencia?.Codigo ?? 0 : cargaOcorrenciaAutorizacao.Usuario?.Codigo ?? 0,
                    UsuarioLogado = this.Usuario?.Codigo ?? 0,
                    Situacao = cargaOcorrenciaAutorizacao.Situacao,
                };

                return new JsonpResult(retorno);
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

        [AllowAuthenticate]
        public async Task<IActionResult> ConsultarAutorizacoesEmissao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = GridConsultarAutorizacoes(Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoOcorrencia.EmissaoOcorrencia, unitOfWork);

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

        [AllowAuthenticate]
        public async Task<IActionResult> ConsultarAutorizacoes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = GridConsultarAutorizacoes(Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoOcorrencia.AprovacaoOcorrencia, unitOfWork);

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

        public async Task<IActionResult> VoltarParaEtapaCadastro()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repositorioOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);
                Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia = repositorioOcorrencia.BuscarPorCodigo(codigo);

                if (ocorrencia == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                if (!ocorrencia.SituacaoOcorrencia.IsPermiteVoltarParaEtapaCadastro())
                    return new JsonpResult(false, true, Localization.Resources.Ocorrencias.Ocorrencia.AAtualSituacaoDaOcorrenciaNaoPermiteVoltarParaAEtapaDeCadastro);

                unitOfWork.Start();

                Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao repositorioOcorrenciaAutorizacao = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao(unitOfWork);
                ocorrencia.SituacaoOcorrencia = SituacaoOcorrencia.AgInformacoes;

                repositorioOcorrencia.Atualizar(ocorrencia);
                repositorioOcorrenciaAutorizacao.DeletarPorOcorrencia(ocorrencia.Codigo);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, ocorrencia, Localization.Resources.Ocorrencias.Ocorrencia.OcorrenciaRetornadaParaAEtapaDeCadastro, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Ocorrencias.Ocorrencia.OcorreuUmaFalhaAoVoltarParaAEtapaDeCadastro);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ObterDetalhesCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga = Request.GetIntParam("Carga");

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaIntegracao repCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaIntegracao(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                if (carga == null)
                    return new JsonpResult(false, true, Localization.Resources.Ocorrencias.Ocorrencia.NaoFoiPossivelEncontrarACarga);

                List<Dominio.Entidades.Embarcador.Cargas.CargaIntegracao> integracoes = repCargaIntegracao.BuscarPorCarga(codigoCarga);

                return new JsonpResult(new
                {
                    Moeda = carga.Moeda ?? MoedaCotacaoBancoCentral.Real,
                    ValorCotacaoMoeda = carga.ValorCotacaoMoeda ?? 0m,
                    ValorTotalMoeda = carga.ValorTotalMoeda ?? 0m,
                    Integracoes = integracoes.Select(o => o.TipoIntegracao.Tipo).ToList()
                });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Ocorrencias.Ocorrencia.OcorreuUmaFalhaAoObterOsDetalhesDaCarga);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AlterarMoedaOcorrencia()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Ocorrencias/Ocorrencia");
                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_AlterarMoeda))
                    return new JsonpResult(false, true, Localization.Resources.Ocorrencias.Ocorrencia.VoceNaoPossuiPermissoesParaExecutarEstaAcao);

                if (!ConfiguracaoEmbarcador.UtilizaMoedaEstrangeira)
                    return new JsonpResult(Localization.Resources.Ocorrencias.Ocorrencia.NaoEPossivelUtilizarMoedaEstrangeiraNesteAmbiente);

                int codigoCargaOcorrencia = Request.GetIntParam("Codigo");

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral? moeda = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral>("Moeda");
                decimal valorCotacaoMoeda = Request.GetDecimalParam("ValorCotacaoMoeda");

                if (valorCotacaoMoeda <= 0m)
                    return new JsonpResult(false, true, Localization.Resources.Ocorrencias.Ocorrencia.OValorDaCotacaoDaMoedaDeveSerMaiorQueZero);

                if (!moeda.HasValue)
                    return new JsonpResult(false, true, Localization.Resources.Ocorrencias.Ocorrencia.MoedaInvalida);

                if (!new Servicos.Embarcador.Carga.Moeda(unitOfWork, Auditado).AlterarMoedaOcorrencia(out string erro, codigoCargaOcorrencia, moeda.Value, valorCotacaoMoeda))
                    return new JsonpResult(false, true, erro);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Ocorrencias.Ocorrencia.OcorreuUmaFalhaAoAlterarAMoedaDaCarga);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ObterDetalhesCTeTerceiroParaOcorrencia()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.CTe.CTeTerceiro repCTeTerceiro = new Repositorio.Embarcador.CTe.CTeTerceiro(unitOfWork);

                int codigoCTeTerceiro = Request.GetIntParam("CTeTerceiro");

                Dominio.Entidades.Embarcador.CTe.CTeTerceiro cteTerceiro = repCTeTerceiro.BuscarPorCodigo(codigoCTeTerceiro);

                Servicos.Embarcador.CTe.CTEsImportados.ObterCTeVinculadoAoCTeTerceiro(cteTerceiro, unitOfWork, out Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubcontratacao, out Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe);

                if (cargaCTe == null)
                    return new JsonpResult(false, true, string.Format(Localization.Resources.Ocorrencias.Ocorrencia.NaoFoiPossivelEncontrarCTeEmitido, cteTerceiro.Descricao));

                return new JsonpResult(new
                {
                    Carga = new
                    {
                        cargaCTe.Carga.Codigo,
                        cargaCTe.Carga.CodigoCargaEmbarcador
                    },
                    CTe = new
                    {
                        cargaCTe.CTe.Codigo,
                        cargaCTe.CTe.Descricao,
                        CodigoCargaCTe = cargaCTe.Codigo
                    },
                    cteTerceiro.ValorAReceber
                });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Ocorrencias.Ocorrencia.OcorreuUmaFalhaAoObterOsDetalhesDaCarga);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> DownloadLoteAnexo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repositorioOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);
                Servicos.Embarcador.Arquivo.ControleGeracaoArquivo servicoArquivo = new Servicos.Embarcador.Arquivo.ControleGeracaoArquivo(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaOcorrencia filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);

                string stringConexao = _conexao.StringConexao;
                int totalRegistros = repositorioOcorrencia.ContarConsulta(filtrosPesquisa);

                if (0 == ConfiguracaoEmbarcador.MaxDownloadsPorVez)
                    return new JsonpResult(false, true, Localization.Resources.Ocorrencias.Ocorrencia.AConfiguracaoDoSistemaNaoPermiteODownloadEmLote);

                if (totalRegistros > ConfiguracaoEmbarcador.MaxDownloadsPorVez)
                    return new JsonpResult(false, true, string.Format(Localization.Resources.Ocorrencias.Ocorrencia.QuantiadeOcorrenciasEMaiorQueoMaximoOcorrenciasPermitidas, totalRegistros, ConfiguracaoEmbarcador.MaxDownloadsPorVez));

                List<int> codigosOcorrencias = repositorioOcorrencia.ConsultarCodigosOcorrencia(filtrosPesquisa);

                Dominio.Entidades.Embarcador.Arquivo.ControleGeracaoArquivo controleGeracaoArquivo = servicoArquivo.Adicionar("Documentos das Ocorrências", Usuario, TipoArquivo.Zip);
                Task.Factory.StartNew(() => ProcessarDownloadAnexoOcorrencias(stringConexao, codigosOcorrencias, controleGeracaoArquivo));

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Ocorrencias.Ocorrencia.OcorreuUmaFalhaAoEfetuarODownloadEmLote);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> DownloadDocumentosOcorrencia()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia = repCargaOcorrencia.BuscarPorCodigo(codigo);

                if (null == ocorrencia)
                    return new JsonpResult(false, true, Localization.Resources.Ocorrencias.Ocorrencia.NaoFoiPossivelEncontrarOsDados);

                if (0 == ConfiguracaoEmbarcador.MaxDownloadsPorVez)
                    return new JsonpResult(false, true, Localization.Resources.Ocorrencias.Ocorrencia.AConfiguracaoDoSistemaNaoPermiteODownloadDosArquivos);

                Servicos.CTe svcCTe = new Servicos.CTe(unitOfWork);
                Servicos.Embarcador.CTe.DACTE svcDACTE = new Servicos.Embarcador.CTe.DACTE(unitOfWork);

                string nomeArquivoOcorrencia = $"{ocorrencia.NumeroOcorrencia}.{TipoArquivo.Zip.ObterExtensao()}";
                byte[] arquivoBinarioOcorrencia = ObterzIPDocumentosOcorrencia(ocorrencia, svcCTe, svcDACTE, unitOfWork);

                return Arquivo(arquivoBinarioOcorrencia, "application/zip", nomeArquivoOcorrencia);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Ocorrencias.Ocorrencia.OcorreuUmaFalhaAoEfetuarODownloadDosArquivos);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> DownloadPDFOcorrencia()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);

                Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia = repCargaOcorrencia.BuscarPorCodigo(codigo);

                if (ocorrencia == null)
                    return new JsonpResult(false, true, Localization.Resources.Ocorrencias.Ocorrencia.NaoFoiPossivelEncontrarOsDados);

                (byte[] arquivo, string nomeArquivo) = MontarPDFDocumentosOcorrencia(ocorrencia, ConfiguracaoEmbarcador, unitOfWork);

                return Arquivo(arquivo, "application/pdf", nomeArquivo);
            }
            catch (ControllerException ex)
            {
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Ocorrencias.Ocorrencia.OcorreuUmaFalhaAoEfetuarODownloadDoArquivoPdf);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> DownloadPDFsOcorrenciaLote()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repositorioOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaOcorrencia filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);

                bool selecionarTodos = Request.GetBoolParam("SelecionarTodos");
                List<int> codigosOcorrencias = Request.GetListParam<int>("ListaOcorrenciasPesquisa");

                if (codigosOcorrencias?.Count == 0 && !selecionarTodos)
                    return new JsonpResult(false, true, Localization.Resources.Ocorrencias.Ocorrencia.ENecessarioSelecionarAoMenosUmaOcorrencia);

                List<int> listaCodigosOcorrenciasGeracao = repositorioOcorrencia.ObterCodigosOcorrenciasSelecionadas(filtrosPesquisa, selecionarTodos, codigosOcorrencias);

                return Arquivo(ObterPDFDocumentosOcorrenciaLoteCompactado(listaCodigosOcorrenciasGeracao, unitOfWork), "application/zip", $"Documentos_de_Ocorrências_{DateTime.Now.ToDateString()}_{DateTime.Now.ToTimeString()}.zip");
            }
            catch (ControllerException ex)
            {
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Ocorrencias.Ocorrencia.OcorreuUmaFalhaAoEfetuarODownloadDePdfsEmLote);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterProdutosNotasCTesOcorrencia()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Pedidos.XMLNotaFiscalProduto repProdutos = new Repositorio.Embarcador.Pedidos.XMLNotaFiscalProduto(unitOfWork);

                int codigoCTe = Request.GetIntParam("CodigoCTe");
                List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalProduto> produtos = repProdutos.BuscarPorCTe(codigoCTe);

                if (produtos.Count == 0)
                    return new JsonpResult(false, true, "Não foram encontrados produtos para esta nota deste CTe");

                List<dynamic> retorno = new List<dynamic>();

                foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalProduto produto in produtos)
                {
                    retorno.Add(new
                    {
                        produto.Codigo,
                        Nota = produto.XMLNotaFiscal.Numero,
                        produto.Produto.CodigoProdutoEmbarcador,
                        Produto = produto.Produto.Descricao,
                        Quantidade = produto.Quantidade.ToString("n2"),
                        QuantidadeDevolucao = 0.ToString("n2"),
                        ValorProduto = produto.ValorProduto.ToString("n2"),
                        ValorTotal = produto.ValorTotal.ToString("n2"),
                        ValorTotalDevolver = 0.ToString("n2"),
                        produto.XMLNotaFiscal.NomeDestinatario,
                        CodigoCTe = codigoCTe,
                        CodigoXMLNotaFiscal = produto.XMLNotaFiscal.Codigo,
                    });
                }

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Falha ao buscar produtos das notas fiscais do CT-e");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarCentroResultadoCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = int.Parse(Request.Params("Codigo"));
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigo);

                var retorno = new
                {
                    CentroResultado = carga.TipoOperacao != null ? carga.TipoOperacao?.ConfiguracaoPagamentos?.CentroResultado?.Descricao : ""
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoBuscarPorCodigo);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ValidarTipoPropostaCargaOcorrencia()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoCarga = int.Parse(Request.Params("CodigoCarga"));
                int codigoOcorrencia = int.Parse(Request.Params("CodigoOcorrencia"));

                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.TipoDeOcorrenciaDeCTe repositorioTipoDeOcorrenciaDeCTe = new Repositorio.TipoDeOcorrenciaDeCTe(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPrimeiroPorCargaSemFetch(codigoCarga);
                Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoDeOcorrenciaDeCTe = repositorioTipoDeOcorrenciaDeCTe.BuscarPorCodigo(codigoOcorrencia);

                var retorno = new
                {
                    Valido = !tipoDeOcorrenciaDeCTe.TipoProposta.HasValue || tipoDeOcorrenciaDeCTe.TipoProposta == TipoPropostaOcorrencia.Nenhum || (int)cargaPedido.TipoPropostaMultimodal == (int)tipoDeOcorrenciaDeCTe.TipoProposta
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoBuscarPorCodigo);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ConsultarCargaCTeOcorrencia()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                bool utilizarSelecaoPorNotasFiscaisCTe = Request.GetBoolParam("UtilizarSelecaoPorNotasFiscaisCTe");
                bool definirPeriodoEstadiaAutomaticamente = Request.GetBoolParam("DefinirPeriodoEstadiaAutomaticamente");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("CodigoCargaCTe", false);
                grid.AdicionarCabecalho("CodigoNotaFiscal", false);
                grid.AdicionarCabecalho("SituacaoCTe", false);
                grid.AdicionarCabecalho("CodigoCTE", false);
                grid.AdicionarCabecalho("NumeroModeloDocumentoFiscal", false);
                grid.AdicionarCabecalho("CodigoEmpresa", false);
                grid.AdicionarCabecalho("DataEntrada", false);
                grid.AdicionarCabecalho("DataEntradaRaio", false);
                grid.AdicionarCabecalho("DataSaidaRaio", false);
                grid.AdicionarCabecalho("CTeGlobalizado", false);
                grid.AdicionarCabecalho(Localization.Resources.Chamado.ChamadoOcorrencia.Numero, "Numero", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho(Localization.Resources.Chamado.ChamadoOcorrencia.Notas, "NumeroNotas", 8, Models.Grid.Align.center, false);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                {
                    grid.AdicionarCabecalho(Localization.Resources.Chamado.ChamadoOcorrencia.Serie, "Serie", 5, Models.Grid.Align.center, true);
                    grid.AdicionarCabecalho(Localization.Resources.Chamado.ChamadoOcorrencia.Documento, "AbreviacaoModeloDocumentoFiscal", 10, Models.Grid.Align.center, true);
                }
                else if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                {
                    grid.AdicionarCabecalho(Localization.Resources.Chamado.ChamadoOcorrencia.Serie, "Serie", 5, Models.Grid.Align.center, true);
                    grid.AdicionarCabecalho(Localization.Resources.Chamado.ChamadoOcorrencia.Documento, "AbreviacaoModeloDocumentoFiscal", 10, Models.Grid.Align.center, true);
                    grid.AdicionarCabecalho(Localization.Resources.Chamado.ChamadoOcorrencia.TipoPagamento, "DescricaoTipoPagamento", 10, Models.Grid.Align.center, true);
                    grid.AdicionarCabecalho(Localization.Resources.Chamado.ChamadoOcorrencia.Remetente, "Remetente", 18, Models.Grid.Align.left, true);
                }
                else if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Terceiros)
                {
                    grid.AdicionarCabecalho(Localization.Resources.Chamado.ChamadoOcorrencia.Emissao, "DataEmissao", 10, Models.Grid.Align.left, true);
                    grid.AdicionarCabecalho(Localization.Resources.Chamado.ChamadoOcorrencia.Remetente, "Remetente", 18, Models.Grid.Align.left, true);
                }

                grid.AdicionarCabecalho(Localization.Resources.Chamado.ChamadoOcorrencia.TipoServico, "DescricaoTipoServico", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho(Localization.Resources.Chamado.ChamadoOcorrencia.Destinatario, "Destinatario", 18, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Chamado.ChamadoOcorrencia.Destino, "Destino", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Chamado.ChamadoOcorrencia.ValorAReceber, "ValorFrete", 8, Models.Grid.Align.right, true);

                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Terceiros)
                    grid.AdicionarCabecalho(Localization.Resources.Chamado.ChamadoOcorrencia.Aliquota, "Aliquota", 5, Models.Grid.Align.right, false);

                string propOrdenacao = grid.header[grid.indiceColunaOrdena].data;

                if (propOrdenacao == "Remetente" || propOrdenacao == "Destinatario")
                    propOrdenacao += ".Nome";
                if (propOrdenacao == "Destino")
                    propOrdenacao = "LocalidadeTerminoPrestacao.Descricao";

                if (propOrdenacao == "DescricaoTipoPagamento")
                    propOrdenacao = "TipoPagamento";

                if (propOrdenacao == "DescricaoTipoServico")
                    propOrdenacao = "TipoServico";

                if (propOrdenacao == "AbreviacaoModeloDocumentoFiscal")
                    propOrdenacao = "ModeloDocumentoFiscal.Abreviacao";

                propOrdenacao = "CTe." + propOrdenacao;

                Servicos.Embarcador.CargaOcorrencia.Ocorrencia servicoOcorrencia = new Servicos.Embarcador.CargaOcorrencia.Ocorrencia(unitOfWork);

                Repositorio.Embarcador.Cargas.CargaCTe repositorioCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaMontarConsultaCtes filtroPesquisa = ObterFiltrosDePesquisaConsultaCtes(unitOfWork);
                int codigoCarga = Request.GetIntParam("Carga");
                Dominio.Entidades.Embarcador.Cargas.Carga carga = await repositorioCarga.BuscarPorCodigoAsync(codigoCarga);

                servicoOcorrencia.ObterFiltroBuscarCTesSelecionadosOuCargas(filtroPesquisa, carga, unitOfWork, TipoServicoMultisoftware, Usuario);

                int totalRegistros = await repositorioCargaCTe.ContarConsultaCTes(filtroPesquisa);

                if (totalRegistros == 0)
                {
                    grid.AdicionaRows(new List<dynamic>());
                    grid.setarQuantidadeTotal(totalRegistros);

                    return new JsonpResult(grid);
                }

                Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repositorioCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);

                List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = await repositorioCargaCTe.ConsultarCTes(filtroPesquisa, propOrdenacao, grid.dirOrdena, grid.inicio, grid.limite);

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe> cargaPedidosXMLsNotaFiscalCTe = await repositorioCargaPedidoXMLNotaFiscalCTe.BuscarPorCargaPedidoXMLNotaFiscalCTePorCargaCTeAsync((from o in cargaCTes select o.Codigo).ToList());
                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> cargaEntregaPedidos = null;

                if (definirPeriodoEstadiaAutomaticamente)
                {
                    Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido repositorioCargaEntregaPedido = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido(unitOfWork);
                    List<int> codigosCargaPedido = (from o in cargaPedidosXMLsNotaFiscalCTe select o.PedidoXMLNotaFiscal.CargaPedido.Codigo).Distinct().ToList();
                    cargaEntregaPedidos = await repositorioCargaEntregaPedido.BuscarPorCargaPedidosAsync(codigosCargaPedido);
                }
                else
                    cargaEntregaPedidos = new List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido>();

                List<dynamic> lista = null;

                if (utilizarSelecaoPorNotasFiscaisCTe)
                    lista = ObterConsultaCargaCTeOcorrenciaPorNota(cargaCTes, cargaPedidosXMLsNotaFiscalCTe, cargaEntregaPedidos, ref totalRegistros);
                else
                    lista = (from cargaCTe in cargaCTes select ObterCargaCTeOcorrencia(cargaCTe, cargaPedidosXMLsNotaFiscalCTe, cargaEntregaPedidos)).ToList();

                grid.AdicionaRows(lista);
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
                await unitOfWork.DisposeAsync();
            }
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaMontarConsultaCtes ObterFiltrosDePesquisaConsultaCtes(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

            int codigoCarga = Request.GetIntParam("Carga");
            Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(codigoCarga);
            TipoEmissaoDocumentoOcorrencia tipoEmissaoDocumentoOcorrencia = Request.GetEnumParam<TipoEmissaoDocumentoOcorrencia>("TipoEmissaoDocumentoOcorrencia", TipoEmissaoDocumentoOcorrencia.Todos);
            bool emitirDocumentoParaFilialEmissoraComPreCTe = Request.GetBoolParam("EmitirDocumentoParaFilialEmissoraComPreCTe") && tipoEmissaoDocumentoOcorrencia == TipoEmissaoDocumentoOcorrencia.SomenteFilialEmissora;
            int chamado = (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS) ? Request.GetIntParam("Chamado") : 0;
            bool ctesSubContratacaoFilialEmissora = !emitirDocumentoParaFilialEmissoraComPreCTe;

            Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaMontarConsultaCtes filtro = new Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaMontarConsultaCtes
            {
                Carga = carga.Codigo,
                NumeroDocumento = Request.GetIntParam("NumeroDocumento"),
                NumeroNF = Request.GetIntParam("NumeroNF"),
                StatusCTe = Request.Params("Status") != null ? JsonConvert.DeserializeObject<string[]>(Request.Params("Status")) : Array.Empty<string>(),
                ApenasCTesNormais = true,
                CtesSubContratacaoFilialEmissora = ctesSubContratacaoFilialEmissora,
                CtesSemSubContratacaoFilialEmissora = false,
                EmpresasFilialEmissora = new List<int>(),
                Destinatario = Request.GetDoubleParam("Destinatario"),
                EmitirDocumentoParaFilialEmissoraComPreCTe = emitirDocumentoParaFilialEmissoraComPreCTe,
                CodigoChamado = chamado,
            };

            return filtro;
        }

        private void ValidarDadosOcorrencia(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia, List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTEs, List<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> cargaDocumentosParaEmissaoNFSManual, int numeroNF, double cnpjdestinatario, int numeroDocumento, int codigoChamado, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.CargaOcorrencia.Ocorrencia srvOcorrencia = new Servicos.Embarcador.CargaOcorrencia.Ocorrencia(unitOfWork);

            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.Embarcador.Transportadores.TransportadorConfiguracaoNFSe repTransportadorConfiguracaoNFSe = new Repositorio.Embarcador.Transportadores.TransportadorConfiguracaoNFSe(unitOfWork);
            Repositorio.Embarcador.Chamados.ChamadoOcorrencia repChamadoOcorrencia = new Repositorio.Embarcador.Chamados.ChamadoOcorrencia(unitOfWork);

            if (ocorrencia.TipoOcorrencia.ClientesBloqueados != null && ocorrencia.TipoOcorrencia.ClientesBloqueados.Count > 0 && cargaCTEs.Count > 0)
            {
                foreach (Dominio.Entidades.Embarcador.Ocorrencias.ClientesBloqueados clienteBloqueado in ocorrencia.TipoOcorrencia.ClientesBloqueados)
                {
                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe in cargaCTEs)
                    {
                        if (cargaCTe.CTe != null)
                        {
                            if (clienteBloqueado.TipoCliente == Dominio.Enumeradores.TipoTomador.Remetente)
                            {
                                if (clienteBloqueado.Cliente.CPF_CNPJ_SemFormato == cargaCTe.CTe.Remetente.CPF_CNPJ_SemFormato)
                                {
                                    string clienteCodigoIntegracao = string.IsNullOrWhiteSpace(clienteBloqueado.Cliente.CodigoIntegracao) ? string.Empty : " (" + clienteBloqueado.Cliente.CodigoIntegracao + ")";
                                    throw new ControllerException(string.Format(Localization.Resources.Ocorrencias.Ocorrencia.ClienteOrigemNaoPermitido, clienteBloqueado.Cliente.CPF_CNPJ_SemFormato, clienteCodigoIntegracao, clienteBloqueado.Cliente.Nome));
                                }
                            }
                            else if (clienteBloqueado.TipoCliente == Dominio.Enumeradores.TipoTomador.Destinatario)
                            {
                                if (clienteBloqueado.Cliente.CPF_CNPJ_SemFormato == cargaCTe.CTe.Destinatario.CPF_CNPJ_SemFormato)
                                {
                                    string clienteCodigoIntegracao = string.IsNullOrWhiteSpace(clienteBloqueado.Cliente.CodigoIntegracao) ? string.Empty : " (" + clienteBloqueado.Cliente.CodigoIntegracao + ")";
                                    throw new ControllerException(string.Format(Localization.Resources.Ocorrencias.Ocorrencia.ClienteDestinoNaoPermitido, clienteBloqueado.Cliente.CPF_CNPJ_SemFormato, clienteCodigoIntegracao, clienteBloqueado.Cliente.Nome));
                                }
                            }
                        }
                    }
                }
            }

            if (ocorrencia.OrigemOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemOcorrencia.PorCarga)
            {
                if (!ocorrencia.ComplementoValorFreteCarga)
                {
                    if (cargaCTEs.Count + cargaDocumentosParaEmissaoNFSManual.Count == 0)
                        throw new ControllerException(Localization.Resources.Ocorrencias.Ocorrencia.EObrigatorioSelecionarAoMenosUmCtEOuDocumentoParaNfsManualParaGerarAOcorrencia);

                    if (ocorrencia.TipoOcorrencia.NaoPermiteSelecionarTodosCTes)
                    {
                        List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> todosCtesCarga = srvOcorrencia.BuscarCTesSelecionadosOuCargas(ObterFiltrosDePesquisaConsultaCtes(unitOfWork), ocorrencia.Carga, ConfiguracaoEmbarcador, unitOfWork, string.Empty, true, numeroNF, cnpjdestinatario, TipoServicoMultisoftware, Usuario, numeroDocumento).ConfigureAwait(false).GetAwaiter().GetResult();
                        if (cargaCTEs.Count == todosCtesCarga.Count && todosCtesCarga.Count > 1)
                            throw new ControllerException(string.Format(Localization.Resources.Ocorrencias.Ocorrencia.NaoEPermitidoSelecionarTodoCTesFavorSelecionar, ocorrencia.TipoOcorrencia.Descricao));
                    }
                }
                else
                {
                    if (ocorrencia.ValorOcorrencia <= 0m && TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                        throw new ControllerException(Localization.Resources.Ocorrencias.Ocorrencia.EObrigatorioInformarUmValorParaAOcorrencia);

                    if (ocorrencia.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Nova &&
                        ocorrencia.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe &&
                        ocorrencia.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete)
                    {
                        throw new ControllerException(Localization.Resources.Ocorrencias.Ocorrencia.ASituacaoDaCargaNaoPermiteQueAOcorrenciaDeComplementoDoValorDoFreteSejaAdicionada);
                    }
                }
            }

            if (ocorrencia.OrigemOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemOcorrencia.PorPeriodo)
            {
                if (!ocorrencia.TipoOcorrencia.OcorrenciaDestinadaFranquias)
                {
                    if (ocorrencia.Cargas.Count == 0)
                        throw new ControllerException(Localization.Resources.Ocorrencias.Ocorrencia.OPeriodoSelecionadoNaoPossuiNenhumaCargaComDocumento);

                    if (ocorrencia.Cargas.FirstOrDefault().Empresa.EmissaoDocumentosForaDoSistema)
                        throw new ControllerException(Localization.Resources.Ocorrencias.Ocorrencia.AEmissaoPorPeriodoSoEPermitidaParaTransportadoresQueEmitemNoMultiembarcador);
                }
                else
                {
                    if (ocorrencia.ContratoFrete == null)
                        throw new ControllerException(Localization.Resources.Ocorrencias.Ocorrencia.EObrigatorioInformarOContratoDeFreteParaGerarAOcorrencia);
                }
            }

            if (ocorrencia.ComponenteFrete != null && ocorrencia.ComponenteFrete.TipoComponenteFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ICMS)
            {
                if (ocorrencia.ValorICMS <= 0m && ocorrencia.AliquotaICMS <= 0m)
                    throw new ControllerException(Localization.Resources.Ocorrencias.Ocorrencia.ENecessarioInformarUmValorOuAliquotaDeIcmsParaAEmissaoDeComplementoDeIcms);

                if (ocorrencia.OrigemOcorrenciaPorPeriodo)
                    throw new ControllerException(Localization.Resources.Ocorrencias.Ocorrencia.AEmissaoDeComplementosDeIcmsNaoEPermitidaParaOcorrenciasPorPeriodo);

                if (cargaCTEs.Count > 1 && ocorrencia.ValorICMS > 0m)
                    throw new ControllerException(Localization.Resources.Ocorrencias.Ocorrencia.SelecioneApenasUmCtEParaEmissaoDeComplementoDeIcmsComValorDeIcmsCasoNecessarioSelecionarMaisDeUmUtilizeSomenteAAliquota);
            }

            if (ocorrencia.TipoOcorrencia.BloqueiaOcorrenciaDuplicada)
            {
                if (ocorrencia.OrigemOcorrenciaPorPeriodo)
                {
                    // Validacao de ocorrencia por periodo
                    if (!srvOcorrencia.ValidaSeExisteOcorrenciaPorPeriodo(ocorrencia, out string erro, unitOfWork, this.Usuario))
                        throw new ControllerException(erro);
                }
                else
                {
                    // Validacao de ocorrencia por CTe
                    if (!srvOcorrencia.ValidaSeExisteOcorrenciaPorCTe(cargaCTEs, ocorrencia, out string erro, unitOfWork, TipoServicoMultisoftware, codigoChamado))
                        throw new ControllerException(erro);

                    // Validacao de ocorrencia por Docs para Emissão de NFS Manual
                    if (!srvOcorrencia.ValidaSeExisteOcorrenciaPorDocParaEmissaoNFSManual(cargaDocumentosParaEmissaoNFSManual, ocorrencia, out string erroDocs, unitOfWork, TipoServicoMultisoftware, codigoChamado))
                        throw new ControllerException(erroDocs);
                }
            }

            //Se o motivo do atendimento estiver vinculado a uma ocorrência que não permite inserir em duplicidade, não permitir inserir o atendimento em duplicidade também.
            if (codigoChamado > 0 && repChamadoOcorrencia.ExisteOcorrenciaDuplicadaPorChamado(codigoChamado))
                throw new ControllerException(Localization.Resources.Ocorrencias.Ocorrencia.JaExisteUmaOcorrenciaLancadaParaEsseAtendimentoNaoSendoPossivelAdicionarOutra);

            if (ocorrencia.TipoOcorrencia.BloquearOcorrenciaDuplicadaCargaMesmoMDFe)
            {
                if (!srvOcorrencia.ValidaOcorrenciaDuplicadaCargaMDFe(ocorrencia.Carga, ocorrencia, out string erro, unitOfWork))
                    throw new ControllerException(erro);
            }

            if (!srvOcorrencia.SetaModeloDocumentoFiscal(ref ocorrencia, cargaCTEs, out string erroModeloDocumento, unitOfWork, TipoServicoMultisoftware))
                throw new ControllerException(erroModeloDocumento);

            if ((TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Terceiros || ocorrencia.TipoOcorrencia.OcorrenciaTerceiros) && !ocorrencia.TipoOcorrencia.NaoGerarDocumento)
            {
                decimal.TryParse(Request.Params("ValorOcorrenciaDestino"), out decimal valorDestino);
                if (ocorrencia.ValorOcorrencia <= 0 && valorDestino <= 0)
                {
                    unitOfWork.Rollback();

                    if (ocorrencia.TipoOcorrencia.PermiteInformarValor)
                        throw new ControllerException(Localization.Resources.Ocorrencias.Ocorrencia.ValorDaOcorrenciaNaoInformadoVerifiqueETenteNovamente);
                    if (ocorrencia.OrigemOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemOcorrencia.PorCarga)
                    {
                        if (cargaCTEs.FirstOrDefault().Carga.ModeloVeicularCarga == null && cargaCTEs.FirstOrDefault().Carga.Veiculo != null && cargaCTEs.FirstOrDefault().Carga.Veiculo.ModeloVeicularCarga == null)
                            throw new ControllerException(string.Format(Localization.Resources.Ocorrencias.Ocorrencia.VeiculoPlacaEstaSemModeloVeicularCadastradoAcesseOCadastroDeVeiculoEInformeParaCalcularOValorDaOcorrencia, cargaCTEs.FirstOrDefault().Carga.Veiculo?.Placa ?? ""));
                        else
                        {
                            DateTime dataInicio = Request.GetDateTimeParam("DataInicio");
                            DateTime dataFim = Request.GetDateTimeParam("DataFim");
                            int horasSemFranquia = ocorrencia.TipoOcorrencia?.HorasSemFranquia ?? 0;

                            if ((dataFim - dataInicio).TotalHours < horasSemFranquia)
                                throw new ControllerException(string.Format(Localization.Resources.Ocorrencias.Ocorrencia.PeriodoDefinidoNosParametrosNaoAtingeHoras, horasSemFranquia));
                            else
                            {
                                Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = repCargaCTe.BuscarPorCodigo(cargaCTEs.FirstOrDefault().Codigo);

                                throw new ControllerException(string.Format(Localization.Resources.Ocorrencias.Ocorrencia.TabelaDeFreteNaoEncontradaParaOrigemDestinoETipoDeVeiculoComVigenciaNaDataDoCtE, cargaCTe.CTe.Remetente.CPF_CNPJ_Formatado, cargaCTe.CTe.Destinatario.CPF_CNPJ_Formatado, cargaCTe.Carga.ModeloVeicularCarga?.CodigoIntegracao) + $"({cargaCTe.CTe.DataEmissao.Value:dd/MM/yyyy})" + Localization.Resources.Ocorrencias.Ocorrencia.ContateOGpaParaVerificacao);
                            }
                        }
                    }
                    else if (ocorrencia.OrigemOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemOcorrencia.PorContrato)
                        throw new ControllerException(Localization.Resources.Ocorrencias.Ocorrencia.OcorrenciaPorContratoDevemPossuirValorCalculado);
                }
            }

            if (ocorrencia.OrigemOcorrenciaPorPeriodo)
            {
                // Valida emitente
                if (ocorrencia.TipoOcorrencia.TipoEmissaoIntramunicipal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoIntramunicipal.SempreNFSe)
                {
                    if (ocorrencia.Emitente == null)
                        throw new ControllerException(Localization.Resources.Ocorrencias.Ocorrencia.EmitenteNaoSelecionado);

                    Dominio.Entidades.Embarcador.Transportadores.TransportadorConfiguracaoNFSe transportadorConfiguracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorEmpresa(ocorrencia.Emitente.Codigo);
                    if (transportadorConfiguracaoNFSe == null)
                        throw new ControllerException(Localization.Resources.Ocorrencias.Ocorrencia.EmitenteNaoPossuiConfiguracaoParaEmitirNfse);
                }
            }

            Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscal = ocorrencia.TipoOcorrencia.ModeloDocumentoFiscal;
            Dominio.Entidades.Empresa empresaOcorrencia = ocorrencia.ObterEmitenteOcorrencia();
            Dominio.Entidades.EmpresaSerie empresaSerieModelo = null;
            if (modeloDocumentoFiscal?.Abreviacao == "ND" && ConfiguracaoEmbarcador.NumeroSerieNotaDebitoPadrao > 0)
            {
                if (empresaOcorrencia == null)
                    throw new ControllerException(Localization.Resources.Ocorrencias.Ocorrencia.TransportadorNaoSelecionado);
                if (modeloDocumentoFiscal.Series.Count > 0)
                    empresaSerieModelo = (from obj in modeloDocumentoFiscal.Series where obj.Empresa.Codigo == empresaOcorrencia.Codigo select obj).FirstOrDefault();
                if (empresaSerieModelo == null)
                    throw new ControllerException(Localization.Resources.Ocorrencias.Ocorrencia.TransportadorSemSerieCadastradaParaODocumentoNd);
            }
            else if (modeloDocumentoFiscal?.Abreviacao == "NC" && ConfiguracaoEmbarcador.NumeroSerieNotaCreditoPadrao > 0)
            {
                if (empresaOcorrencia == null)
                    throw new ControllerException(Localization.Resources.Ocorrencias.Ocorrencia.TransportadorNaoSelecionado);
                if (modeloDocumentoFiscal.Series.Count > 0)
                    empresaSerieModelo = (from obj in modeloDocumentoFiscal.Series where obj.Empresa.Codigo == empresaOcorrencia.Codigo select obj).FirstOrDefault();
                if (empresaSerieModelo == null)
                    throw new ControllerException(Localization.Resources.Ocorrencias.Ocorrencia.TransportadorSemSerieCadastradaParaODocumentoNc);
            }

            if (ocorrencia.TipoOcorrencia.UtilizarEntradaSaidaDoRaioCargaEntrega)
            {
                DateTime? dataInicio = Request.GetNullableDateTimeParam("DataInicio");
                DateTime? dataFim = Request.GetNullableDateTimeParam("DataFim");

                if (!dataInicio.HasValue || !dataFim.HasValue)
                    throw new ControllerException(Localization.Resources.Ocorrencias.Ocorrencia.TodosOsParametrosDaOcorrenciaSaoObrigatorios);

                if ((dataFim.Value - dataInicio.Value).TotalHours < ocorrencia.TipoOcorrencia.HorasToleranciaEntradaSaidaRaio)
                    throw new ControllerException(string.Format(Localization.Resources.Ocorrencias.Ocorrencia.TempoEstadiaEMenorQUENaoSeraGaradaNenhumaOcorrencia, ocorrencia.TipoOcorrencia.HorasToleranciaEntradaSaidaRaio));
            }
        }

        private void EnviarEmailGeracaoOcorrencia(int codigoCargaOcorrencia, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string stringConexao)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);

            try
            {
                Servicos.Embarcador.CargaOcorrencia.Ocorrencia.EnviarEmailGeracaoOcorrencia(codigoCargaOcorrencia, tipoServicoMultisoftware, unitOfWork);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaOcorrencia ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaOcorrencia filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaOcorrencia()
            {
                Codigo = Request.GetIntParam("Codigo"),
                CodigoCarga = Request.GetStringParam("Carga"),
                CodigoChamado = Request.GetIntParam("Chamado"),
                CodigoCteComplementar = Request.GetIntParam("CTeComplementar"),
                CodigoCteOrigem = Request.GetIntParam("CTeOrigem"),
                CodigoEmpresa = Request.GetIntParam("Empresa"),
                CodigoGrupoPessoa = Request.GetIntParam("GrupoPessoas"),
                CodigoLoteAvaria = Request.GetIntParam("LoteAvaria"),
                CodigoTipoOcorrencia = Request.GetIntParam("Ocorrencia"),
                CodigoUsuario = this.Usuario.Codigo,
                CpfCnpjPessoa = Request.GetDoubleParam("Pessoa"),
                CpfCnpjTomadorCTeComplementar = Request.GetDoubleParam("Tomador"),
                DataInicial = Request.GetNullableDateTimeParam("DataInicio"),
                DataInicialAprovacao = Request.GetNullableDateTimeParam("DataInicialAprovacao"),
                DataInicialEmissaoDocumento = Request.GetNullableDateTimeParam("DataInicialEmissaoDocumento"),
                DataLimite = Request.GetNullableDateTimeParam("DataFim"),
                DataLimiteAprovacao = Request.GetNullableDateTimeParam("DataLimiteAprovacao"),
                DataLimiteEmissaoDocumento = Request.GetNullableDateTimeParam("DataLimiteEmissaoDocumento"),
                FiltrarCargasPorParteDoNumero = ConfiguracaoEmbarcador?.FiltrarCargasPorParteDoNumero ?? false,
                NumeroBooking = Request.GetStringParam("NumeroBooking"),
                NumeroControle = Request.GetStringParam("NumeroControle"),
                NumeroDocumentoOriginario = Request.GetIntParam("NumeroDocumentoOriginario"),
                NumeroNFe = Request.GetIntParam("NumeroNFe"),
                NumeroOcorrencia = Request.GetIntParam("NumeroOcorrencia"),
                NumeroOcorrenciaCliente = Request.GetStringParam("NumeroOcorrenciaCliente"),
                NumeroOS = Request.GetStringParam("NumeroOS"),
                NumeroPedido = Request.GetStringParam("Pedido"),
                ObservacaoCTe = Request.GetStringParam("ObservacaoCTe"),
                Situacao = Request.GetEnumParam("SituacaoOcorrencia", SituacaoOcorrencia.Todas),
                TipoDocumentoCreditoDebito = Request.GetEnumParam("TipoDocumentoCreditoDebito", TipoDocumentoCreditoDebito.Todos),
                TipoPessoa = Request.GetNullableEnumParam<TipoPessoa>("TipoPessoa"),
                CodigosCentroResultado = Request.GetListParam<int>("CentroResultado"),
                CodigoMotorista = Request.GetIntParam("Motorista"),
                CpfCnpjTomadorCTeOriginal = Request.GetDoubleParam("TomadorCTeOriginal"),
                CodigoGrupoPessoasTomadorCteComplementar = Request.GetIntParam("GrupoPessoasTomadorCteComplementar"),
                CodigoGrupoPessoasTomadorCteOriginal = Request.GetIntParam("GrupoPessoasTomadorCteOriginal"),
                AguardandoImportacaoCTe = Request.GetNullableBoolParam("AguardandoImportacaoCTe"),
                CodigoTiposCausadoresOcorrencia = Request.GetIntParam("TiposCausadoresOcorrencia"),
                CodigoCausasTipoOcorrencia = Request.GetIntParam("CausasTipoOcorrencia"),
                CodigosGrupoOcorrencia = Request.GetListParam<int>("GrupoOcorrencia"),
                NumeroPedidoCliente = Request.GetStringParam("NumeroPedidoCliente"),
                CodigosClienteComplementar = Request.GetListParam<int>("ClienteComplementar"),
                CodigosVendedor = Request.GetListParam<int>("Vendedor"),
                CodigosSupervisor = Request.GetListParam<int>("Supervisor"),
                CodigosGerente = Request.GetListParam<int>("Gerente"),
                CodigosUFDestino = Request.GetListParam<string>("UFDestino"),
                NumeroNF = Request.GetIntParam("NumeroNF"),
                NumeroAtendimento = Request.GetIntParam("NumeroAtendimento")
            };

            Repositorio.Embarcador.Operacional.OperadorLogistica repositorioOperadorLogistica = new Repositorio.Embarcador.Operacional.OperadorLogistica(unitOfWork);
            Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operadorLogisitca = repositorioOperadorLogistica.BuscarPorUsuario(this.Usuario.Codigo);

            if (operadorLogisitca?.SupervisorLogistica ?? false)
                filtrosPesquisa.CodigoUsuario = 0;


            List<int> codigosEmpresa = Request.GetListParam<int>("Empresa");

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Terceiros)
            {
                filtrosPesquisa.CnpjTransportador = Usuario.ClienteTerceiro != null ? Usuario.ClienteTerceiro.CPF_CNPJ_SemFormato : string.Empty;
                filtrosPesquisa.OcultarOcorrenciasAutomaticas = true;
            }
            else if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
            {
                filtrosPesquisa.CnpjTransportador = Usuario.Empresa != null ? Usuario.Empresa.CNPJ_SemFormato : string.Empty;

                if (ConfiguracaoEmbarcador.Pais == TipoPais.Exterior)
                    filtrosPesquisa.CnpjTransportadorExterior = Usuario.Empresa != null ? Usuario.Empresa.CNPJ : string.Empty;

                filtrosPesquisa.CodigoUsuario = 0;
                filtrosPesquisa.CodigoEmpresa = Usuario.Empresa.Codigo;
                filtrosPesquisa.OcultarOcorrenciasAutomaticas = true;

                if (Usuario.Empresa?.Filiais?.Count > 0)
                {
                    codigosEmpresa.Add(Usuario.Empresa.Codigo);
                    foreach (Dominio.Entidades.Empresa empresaFilial in Usuario.Empresa.Filiais)
                        codigosEmpresa.Add(empresaFilial.Codigo);
                }
            }

            if (!ConfiguracaoEmbarcador.OcultarOcorrenciasGeradasAutomaticamente)
                filtrosPesquisa.OcultarOcorrenciasAutomaticas = false;

            List<int> codigosFilial = Request.GetListParam<int>("Filial");
            List<int> codigosTipoOcorrencia = Request.GetListParam<int>("Ocorrencia");
            List<int> codigosTipoOperacao = Request.GetListParam<int>("TipoOperacao");
            List<int> codigosTipoOperacaoPermitidosOperadorLogistica = ObterListaCodigoTipoOperacaoPermitidosOperadorLogistica(unitOfWork);
            List<SituacaoOcorrencia> situacoes = Request.GetListParam<SituacaoOcorrencia>("SituacaoOcorrencia");

            filtrosPesquisa.CodigosTipoOperacao = codigosTipoOperacao;
            filtrosPesquisa.CodigosFilial = codigosFilial.Count == 0 ? ObterListaCodigoFilialPermitidasOperadorLogistica(unitOfWork) : codigosFilial;
            filtrosPesquisa.CodigosRecebedor = ObterListaCnpjCpfRecebedorPermitidosOperadorLogistica(unitOfWork);
            filtrosPesquisa.CodigosFilialVenda = ObterListaCodigoFilialVendaPermitidasOperadorLogistica(unitOfWork);
            filtrosPesquisa.CodigosTipoCarga = ObterListaCodigoTipoCargaPermitidosOperadorLogistica(unitOfWork);

            if (codigosTipoOperacaoPermitidosOperadorLogistica.Count > 0)
                filtrosPesquisa.CodigosTipoOperacao = codigosTipoOperacaoPermitidosOperadorLogistica;

            if (codigosEmpresa.Count > 0)
                filtrosPesquisa.CodigosEmpresa = codigosEmpresa;

            if (codigosTipoOcorrencia.Count > 0)
                filtrosPesquisa.CodigosTipoOcorrencia = codigosTipoOcorrencia;

            if (situacoes.Count > 0)
                filtrosPesquisa.Situacoes = situacoes;

            return filtrosPesquisa;
        }

        private byte[] ObterzIPDocumentosOcorrencia(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia, Servicos.CTe svcCTe, Servicos.Embarcador.CTe.DACTE svcDACTE, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repositorioCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaDocumento repCargaOcorrenciaDocumento = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaDocumento(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAnexos repCargaOcorrenciaAnexos = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAnexos(unitOfWork);

            Dictionary<string, byte[]> conteudoCompactarOcorrencia = new Dictionary<string, byte[]>();
            string caminhoAnexo = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "Ocorrencia");

            // Documentos Gerados
            List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> documentosGerado = repositorioCargaCTeComplementoInfo.BuscarCTesPorOcorrencia(ocorrencia.Codigo);
            // Documentos de Origem
            List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> documentosOrigem = repCargaOcorrenciaDocumento.BuscarCTesENFSesPorOcorrencia(ocorrencia.Codigo);

            List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> documentosOcorrencia = new List<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            documentosOcorrencia.AddRange(documentosGerado);
            documentosOcorrencia.AddRange(documentosOrigem);
            foreach (Dominio.Entidades.ConhecimentoDeTransporteEletronico cte in documentosOcorrencia)
            {
                string nomeArquivoXML = Servicos.Embarcador.CTe.CTe.ObterNomeArquivoDownloadCTe(cte, "xml");
                if (cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe)
                    nomeArquivoXML = string.IsNullOrWhiteSpace(nomeArquivoXML) ? string.Concat(cte.Chave, ".xml") : nomeArquivoXML;
                else
                    nomeArquivoXML = string.IsNullOrWhiteSpace(nomeArquivoXML) ? string.Concat("NFSe_", cte.Numero, ".xml") : nomeArquivoXML;

                byte[] data = svcCTe.ObterXMLAutorizacao(cte, unitOfWork);

                if (data != null)
                    conteudoCompactarOcorrencia.Add(nomeArquivoXML, data);

                string nomeArquivoPDF = svcDACTE.ObterNomeArquivoPDF(cte, unitOfWork);
                byte[] pdf = svcDACTE.ObterPDF(cte, unitOfWork);

                if (pdf != null)
                    conteudoCompactarOcorrencia.Add(nomeArquivoPDF, pdf);
            }

            // Todos os Anexos
            List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAnexos> anexos = repCargaOcorrenciaAnexos.BuscarPorCodigoOcorrencia(ocorrencia.Codigo);
            foreach (Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAnexos anexo in anexos)
            {
                string nomeArquivo = anexo.NomeArquivo;

                string extencao = System.IO.Path.GetExtension(anexo.NomeArquivo).ToLower();
                string arquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminhoAnexo, anexo.GuidArquivo + extencao);
                byte[] bArquivo = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(arquivo);

                if (bArquivo != null)
                    conteudoCompactarOcorrencia.Add(nomeArquivo, bArquivo);
            }

            if (0 == conteudoCompactarOcorrencia.Count)
                return null;

            System.IO.MemoryStream arquivosOcorrencias = Utilidades.File.GerarArquivoCompactado(conteudoCompactarOcorrencia);
            byte[] arquivoBinarioOcorrencia = arquivosOcorrencias.ToArray();
            arquivosOcorrencias.Dispose();

            return arquivoBinarioOcorrencia;
        }

        private void ProcessarDownloadAnexoOcorrencias(string stringConexao, List<int> codigosOcorrencias, Dominio.Entidades.Embarcador.Arquivo.ControleGeracaoArquivo controleGeracaoArquivo)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);
            Servicos.Embarcador.Arquivo.ControleGeracaoArquivo servicoArquivo = new Servicos.Embarcador.Arquivo.ControleGeracaoArquivo(unitOfWork);

            Servicos.CTe svcCTe = new Servicos.CTe(unitOfWork);
            Servicos.Embarcador.CTe.DACTE svcDACTE = new Servicos.Embarcador.CTe.DACTE(unitOfWork);

            string urlPagina = "Ocorrencias/Ocorrencia";

            try
            {
                Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);

                Dictionary<string, byte[]> conteudoCompactarLoteOcorrencias = new Dictionary<string, byte[]>();

                foreach (int codigoOcorrencia in codigosOcorrencias)
                {
                    Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia = repCargaOcorrencia.BuscarPorCodigo(codigoOcorrencia);
                    string nomeArquivoOcorrencia = $"{ocorrencia.NumeroOcorrencia}.{TipoArquivo.Zip.ObterExtensao()}";

                    byte[] arquivoBinarioOcorrencia = ObterzIPDocumentosOcorrencia(ocorrencia, svcCTe, svcDACTE, unitOfWork);
                    conteudoCompactarLoteOcorrencias.Add(nomeArquivoOcorrencia, arquivoBinarioOcorrencia);
                }

                System.IO.MemoryStream arquivosTodasOcorrencias = Utilidades.File.GerarArquivoCompactado(conteudoCompactarLoteOcorrencias);
                byte[] arquivoBinarioTodosXML = arquivosTodasOcorrencias.ToArray();
                arquivosTodasOcorrencias.Dispose();

                servicoArquivo.SalvarArquivo(controleGeracaoArquivo, arquivosTodasOcorrencias);
                servicoArquivo.Finalizar(controleGeracaoArquivo, string.Format(Localization.Resources.Ocorrencias.Ocorrencia.GeracaoDoArquivoDasOcorrenciasConcluido, controleGeracaoArquivo.TipoArquivo.ObterDescricao()), urlPagina);
            }
            catch (Exception excecao)
            {
                servicoArquivo.FinalizarComFalha(controleGeracaoArquivo, string.Format(Localization.Resources.Ocorrencias.Ocorrencia.OcorreuUmaFalhaAoGerarOArquivoDasOcorrencias, controleGeracaoArquivo.TipoArquivo.ObterDescricao()), urlPagina, excecao);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ConfiguracaoEmbarcador;

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descricao", false);
                grid.AdicionarCabecalho("SituacaoOcorrencia", false);
                grid.AdicionarCabecalho(Localization.Resources.Chamado.ChamadoOcorrencia.NumeroOcorrencia, "NumeroOcorrencia", 10, Models.Grid.Align.center, true);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                {
                    grid.AdicionarCabecalho(Localization.Resources.Chamado.ChamadoOcorrencia.Tomador, "Tomador", 10, Models.Grid.Align.left, true);
                    grid.AdicionarCabecalho(Localization.Resources.Chamado.ChamadoOcorrencia.NumeroCliente, "NumeroOcorrenciaCliente", 10, Models.Grid.Align.center, true);
                }

                grid.AdicionarCabecalho(Localization.Resources.Chamado.ChamadoOcorrencia.DataOcorrencia, "DataOcorrencia", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Chamado.ChamadoOcorrencia.Carga, "CodigoCargaEmbarcador", 10, Models.Grid.Align.left, true);

                if (configuracaoEmbarcador.ExibirInformacoesAdicionaisChamado)
                    grid.AdicionarCabecalho(Localization.Resources.Chamado.ChamadoOcorrencia.CargasAgrupadas, "CodigosAgrupadosCarga", 10, Models.Grid.Align.left, false, false, false, false, true);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    grid.AdicionarCabecalho(Localization.Resources.Chamado.ChamadoOcorrencia.OrigemDestino, "OrigemDestino", 10, Models.Grid.Align.left, false);

                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                {
                    grid.AdicionarCabecalho(Localization.Resources.Chamado.ChamadoOcorrencia.Veiculo, "Veiculo", 10, Models.Grid.Align.left, false);
                    grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Motorista, "Motorista", 10, Models.Grid.Align.left, false);
                }

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                {
                    grid.AdicionarCabecalho(Localization.Resources.Chamado.ChamadoOcorrencia.Transportador, "Transportador", 20, Models.Grid.Align.left, false);
                    grid.AdicionarCabecalho(Localization.Resources.Chamado.ChamadoOcorrencia.Filial, "Filial", 20, Models.Grid.Align.left, false, false);
                }

                grid.AdicionarCabecalho(Localization.Resources.Chamado.ChamadoOcorrencia.TipoOcorrencia, "TipoOcorrencia", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.Ocorrencia.CausadorOcorrencia, "TiposCausadoresOcorrencia", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.Ocorrencia.CausasTipoOcorrencia, "CausasTipoOcorrencia", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Chamado.ChamadoOcorrencia.Valor, "Valor", 10, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho(Localization.Resources.Chamado.ChamadoOcorrencia.Situacao, "DescricaoSituacao", 10, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho(Localization.Resources.Chamado.ChamadoOcorrencia.TipoOperacao, "TipoOperacao", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Chamado.ChamadoOcorrencia.TipoCreditoDebitp, "TipoDocumentoCreditoDebito", 15, Models.Grid.Align.left, false, false);
                grid.AdicionarCabecalho(Localization.Resources.Chamado.ChamadoOcorrencia.NumeroDocumento, "NumeroDocumento", 15, Models.Grid.Align.center, false, false);
                grid.AdicionarCabecalho(Localization.Resources.Chamado.ChamadoOcorrencia.DataEmissaoDocumento, "DataEmissaoDocumento", 15, Models.Grid.Align.center, false, false);

                grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.Ocorrencia.NumeroPedidoCliente, "NumeroPedidoCliente", 15, Models.Grid.Align.center, false, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.EscritorioVendas, "EscritorioVendas", 6, Models.Grid.Align.left, false, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.Matriz, "Matriz", 6, Models.Grid.Align.left, false, false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Vendedor, "Vendedor", 15, Models.Grid.Align.left, false, false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Supervisor, "Supervisor", 15, Models.Grid.Align.left, false, false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Gerente, "Gerente", 15, Models.Grid.Align.left, false, false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.UFDestino, "UFDestino", 15, Models.Grid.Align.left, false, false);
                grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.Ocorrencia.NumeroNF, "NumeroNF", 15, Models.Grid.Align.center, false, false);

                if (configuracaoEmbarcador.ExibirInformacoesAdicionaisChamado)
                {
                    grid.AdicionarCabecalho(Localization.Resources.Chamado.ChamadoOcorrencia.OperadorContratouCarga, "OperadorContratouCarga", 10, Models.Grid.Align.left, false, false, false, false, true);
                    grid.AdicionarCabecalho(Localization.Resources.Chamado.ChamadoOcorrencia.PesoNotas, "PesoNotasFiscais", 10, Models.Grid.Align.left, false, false, false, false, true);
                }

                grid.AdicionarCabecalho(Localization.Resources.Chamado.ChamadoOcorrencia.GrupoOcorrencia, "GrupoOcorrencia", 15, Models.Grid.Align.center, false, false);

                Models.Grid.GridPreferencias gridPreferencias = new Models.Grid.GridPreferencias(unitOfWork, "Ocorrencia/Pesquisa", "grid-pesquisa-ocorrencias");
                grid.AplicarPreferenciasGrid(gridPreferencias.ObterPreferenciaGrid(this.Usuario.Codigo, grid.modelo));
                grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.Ocorrencia.NumeroAtendimento, "NumeroAtendimento", 15, Models.Grid.Align.center, false, true);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaOcorrencia filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);

                Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repositorioPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repositorioCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(unitOfWork);
                Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repositorioOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);
                Repositorio.Embarcador.Ocorrencias.GrupoTipoDeOcorrenciaDeCTe repositorioGrupoOcorrencia = new Repositorio.Embarcador.Ocorrencias.GrupoTipoDeOcorrenciaDeCTe(unitOfWork);
                Repositorio.Embarcador.Chamados.ChamadoOcorrencia repositorioChamadoOcorrencia = new Repositorio.Embarcador.Chamados.ChamadoOcorrencia(unitOfWork);

                int totalRegistros = repositorioOcorrencia.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> ocorrencias = totalRegistros > 0 ? repositorioOcorrencia.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia>();

                List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> dadosCTes = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo>();
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> dadosPedidos = new List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
                IList<(int carga, string notaFiscal)> dadosNotasFiscais = new List<(int carga, string notaFiscal)>();
                List<Dominio.ObjetosDeValor.Embarcador.Pessoas.ClienteComplementar> dadosClienteComplementares = new List<Dominio.ObjetosDeValor.Embarcador.Pessoas.ClienteComplementar>();

                if (ocorrencias.Count > 0)
                {
                    List<int> codigosOcorrencias = ocorrencias.Select(o => o.Codigo).ToList();
                    dadosCTes = repositorioCargaCTeComplementoInfo.BuscarDadosCTesPorOcorrencia(codigosOcorrencias);
                    List<int> codigosCargas = ocorrencias.Where(ocorrencia => ocorrencia.Carga != null).Select(ocorrencia => ocorrencia.Carga.Codigo).ToList();

                    if (codigosCargas.Count > 0)
                    {
                        dadosPedidos = repositorioCargaPedido.BuscarCargasPedidosPorCargas(codigosCargas);
                        dadosClienteComplementares = repositorioCargaPedido.BuscarClientesComplementaresPorCargas(codigosCargas).Result;
                        dadosNotasFiscais = repositorioCargaPedido.BuscarNFsPorCargas(codigosCargas);
                    }
                }

                var ocorrenciasRetornar = (
                    from ocorrencia in ocorrencias
                    select new
                    {
                        ocorrencia.Codigo,
                        Descricao = ocorrencia.NumeroOcorrencia,
                        ocorrencia.NumeroOcorrencia,
                        Transportador = ocorrencia.Carga?.Empresa != null ? ocorrencia.Carga.Empresa.Descricao : (ocorrencia.Emitente?.Descricao ?? ""),
                        Tomador = ocorrencia.Tomador?.Nome,
                        ocorrencia.NumeroOcorrenciaCliente,
                        DataOcorrencia = ocorrencia.DataOcorrencia.ToString("dd/MM/yyyy HH:mm"),
                        ocorrencia.Carga?.CodigoCargaEmbarcador,
                        CodigosAgrupadosCarga = !configuracaoEmbarcador.ExibirInformacoesAdicionaisChamado || (ocorrencia.Carga == null) ? "" : string.Join(", ", ocorrencia.Carga.CodigosAgrupados),
                        Veiculo = TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador ? ocorrencia.Carga?.RetornarPlacas : "",
                        TipoOcorrencia = ocorrencia.TipoOcorrencia != null ? ocorrencia.TipoOcorrencia.Descricao : string.Empty,
                        Motorista = TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador ? ocorrencia.Carga?.DadosSumarizados?.Motoristas ?? "" : "",
                        Componente = ocorrencia.ComponenteFrete != null ? ocorrencia.ComponenteFrete.Descricao : Localization.Resources.Ocorrencias.Ocorrencia.SemComplemento,
                        Valor = ocorrencia.ValorOcorrencia.ToString("n2"),
                        OrigemDestino = $"{ocorrencia.Carga?.DadosSumarizados?.Remetentes} ({ocorrencia.Carga?.DadosSumarizados?.Origens}) até {ocorrencia.Carga?.DadosSumarizados?.Destinatarios} ({ocorrencia.Carga?.DadosSumarizados?.Destinos})",
                        ocorrencia.DescricaoSituacao,
                        ocorrencia.SituacaoOcorrencia,
                        OperadorContratouCarga = !configuracaoEmbarcador.ExibirInformacoesAdicionaisChamado || (ocorrencia.Carga == null) ? "" : ocorrencia.Carga.OperadorContratouCarga?.Descricao ?? ocorrencia.Carga.Operador?.Descricao ?? "",
                        PesoNotasFiscais = !configuracaoEmbarcador.ExibirInformacoesAdicionaisChamado || (ocorrencia.Carga == null) ? "" : repositorioPedidoXMLNotaFiscal.ObterPesoTotalPorCarga(ocorrencia.Carga.Codigo).ToString("n2"),
                        TipoOperacao = ocorrencia.Carga?.TipoOperacao?.Descricao ?? string.Empty,
                        Filial = ocorrencia.Carga?.Filial?.Descricao ?? "",
                        TipoDocumentoCreditoDebito = ocorrencia.ModeloDocumentoFiscal?.TipoDocumentoCreditoDebito.ObterDescricao() ?? "",
                        NumeroDocumento = string.Join(", ", (from o in dadosCTes where o.CargaOcorrencia.Codigo == ocorrencia.Codigo select o.CTe.Numero)),
                        DataEmissaoDocumento = (from o in dadosCTes where o.CargaOcorrencia.Codigo == ocorrencia.Codigo && o.CTe.DataEmissao.HasValue select o.CTe.DataEmissao).FirstOrDefault()?.ToDateTimeString() ?? "",
                        TiposCausadoresOcorrencia = ocorrencia.TiposCausadoresOcorrencia?.Descricao,
                        CausasTipoOcorrencia = ocorrencia.CausasTipoOcorrencia?.Descricao,
                        GrupoOcorrencia = ocorrencia.GrupoOcorrencia != null ? ocorrencia.GrupoOcorrencia.Descricao : string.Empty,
                        NumeroPedidoCliente = ocorrencia.Carga == null ? "" : string.Join(", ", (from o in dadosPedidos where o.Carga.Codigo == ocorrencia.Carga.Codigo select o.Pedido.CodigoPedidoCliente)),
                        EscritorioVendas = ocorrencia.Carga == null ? "" : string.Join(", ", (from o in dadosClienteComplementares where ObterCPFCNPJDestinatarios(ocorrencia.Carga.Codigo, dadosPedidos).Contains(o.CpfCnpjCliente) select o.EscritorioVendas)),
                        Matriz = ocorrencia.Carga == null ? "" : string.Join(", ", (from o in dadosClienteComplementares where ObterCPFCNPJDestinatarios(ocorrencia.Carga.Codigo, dadosPedidos).Contains(o.CpfCnpjCliente) select o.Matriz)),
                        Vendedor = ocorrencia.Carga == null ? "" : string.Join(", ", (from o in dadosPedidos where o.Carga.Codigo == ocorrencia.Carga.Codigo && o.Pedido.FuncionarioVendedor != null select o.Pedido.FuncionarioVendedor.Nome).Distinct()),
                        Supervisor = ocorrencia.Carga == null ? "" : string.Join(", ", (from o in dadosPedidos where o.Carga.Codigo == ocorrencia.Carga.Codigo && o.Pedido.FuncionarioSupervisor != null select o.Pedido.FuncionarioSupervisor.Nome).Distinct()),
                        Gerente = ocorrencia.Carga == null ? "" : string.Join(", ", (from o in dadosPedidos where o.Carga.Codigo == ocorrencia.Carga.Codigo && o.Pedido.FuncionarioGerente != null select o.Pedido.FuncionarioGerente.Nome).Distinct()),
                        UFDestino = ocorrencia.Carga == null ? "" : string.Join(", ", (from o in dadosPedidos where o.Carga.Codigo == ocorrencia.Carga.Codigo select o.Destino.Estado.Sigla).Distinct()),
                        NumeroNF = ocorrencia.Carga == null ? "" : string.Join(", ", (from o in dadosNotasFiscais where o.carga == ocorrencia.Carga.Codigo select o.notaFiscal)),
                        NumeroAtendimento = repositorioChamadoOcorrencia.BuscarNumeroChamadoPorOcorrencia(ocorrencia.Codigo)
                    }
                ).ToList();

                grid.AdicionaRows(ocorrenciasRetornar);
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
            if (propriedadeOrdenar == "CodigoCargaEmbarcador")
                return "Carga.CodigoCargaEmbarcador";

            if (propriedadeOrdenar == "Valor")
                return "ValorOcorrencia";

            if (propriedadeOrdenar == "Tomador")
                return "Tomador.Nome";

            return propriedadeOrdenar;
        }

        private Dominio.Entidades.ModeloDocumentoFiscal ObterModeloDocumentoFiscalParaOcorrencia(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia, Repositorio.UnitOfWork unitOfWork)
        {
            bool.TryParse(Request.Params("CobrarOutroDocumento"), out bool cobrarOutroDocumento);

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS || (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador && cobrarOutroDocumento))
            {
                Repositorio.ModeloDocumentoFiscal repModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(unitOfWork);
                int.TryParse(Request.Params("ModeloDocumentoFiscal"), out int codigoModeloDocumento);

                return repModeloDocumentoFiscal.BuscarPorId(codigoModeloDocumento);
            }

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                return ocorrencia.TipoOcorrencia?.ModeloDocumentoFiscal;

            return null;
        }

        private void GerarParametrosOcorrencia(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia, Repositorio.UnitOfWork unitOfWork, bool atualizandoOcorrencia = false)
        {
            Repositorio.Embarcador.Ocorrencias.ParametroOcorrencia repParametroOcorrencia = new Repositorio.Embarcador.Ocorrencias.ParametroOcorrencia(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaParametros repCargaOcorrenciaParametros = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaParametros(unitOfWork);

            int.TryParse(Request.Params("CodigoParametroPeriodo"), out int codigoParametroPeriodo);
            int.TryParse(Request.Params("CodigoParametroBooleano"), out int codigoParametroBooleano);
            int.TryParse(Request.Params("CodigoParametroInteiro"), out int codigoParametroInteiro);
            int.TryParse(Request.Params("CodigoParametroData1"), out int codigoParametroData1);
            int.TryParse(Request.Params("CodigoParametroData2"), out int codigoParametroData2);
            int.TryParse(Request.Params("CodigoParametroTexto"), out int codigoParametroTexto);
            int.TryParse(Request.Params("ParametroInteiro"), out int parametroInteiro);

            string parametroTexto = Request.Params("ParametroTexto") ?? string.Empty;

            bool.TryParse(Request.Params("ApenasReboque"), out bool apenasReboque);

            string hora = Request.Params("HorasOcorrencia") ?? string.Empty;
            decimal.TryParse(!string.IsNullOrEmpty(hora) ? hora.Replace(".", ",") : "0", out decimal horasOcorrencia);

            DateTime.TryParseExact(Request.Params("DataInicio"), "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out DateTime dataInicio);
            DateTime.TryParseExact(Request.Params("DataFim"), "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out DateTime dataFim);
            DateTime.TryParseExact(Request.Params("ParametroData1"), "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out DateTime parametroData1);
            DateTime.TryParseExact(Request.Params("ParametroData2"), "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out DateTime parametroData2);

            if (codigoParametroPeriodo > 0 && dataInicio > DateTime.MinValue && dataFim > DateTime.MinValue)
            {
                Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaParametros cargaOcorrencaParametros = null;

                if (atualizandoOcorrencia)
                    cargaOcorrencaParametros = repCargaOcorrenciaParametros.BuscarPorCargaOcorrenciaETipo(ocorrencia.Codigo, TipoParametroOcorrencia.Periodo);

                cargaOcorrencaParametros ??= new Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaParametros();

                cargaOcorrencaParametros.CargaOcorrencia = ocorrencia;
                cargaOcorrencaParametros.ParametroOcorrencia = repParametroOcorrencia.BuscarPorCodigo(codigoParametroPeriodo);
                cargaOcorrencaParametros.DataInicio = dataInicio;
                cargaOcorrencaParametros.DataFim = dataFim;
                cargaOcorrencaParametros.TotalHoras = horasOcorrencia;

                if (cargaOcorrencaParametros.Codigo == 0)
                    repCargaOcorrenciaParametros.Inserir(cargaOcorrencaParametros);
                else
                    repCargaOcorrenciaParametros.Atualizar(cargaOcorrencaParametros);
            }

            if (codigoParametroBooleano > 0)
            {
                Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaParametros cargaOcorrencaParametrosBooleano = null;

                if (atualizandoOcorrencia)
                    cargaOcorrencaParametrosBooleano = repCargaOcorrenciaParametros.BuscarPorCargaOcorrenciaETipo(ocorrencia.Codigo, TipoParametroOcorrencia.Booleano);

                cargaOcorrencaParametrosBooleano ??= new Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaParametros();

                cargaOcorrencaParametrosBooleano.CargaOcorrencia = ocorrencia;
                cargaOcorrencaParametrosBooleano.ParametroOcorrencia = repParametroOcorrencia.BuscarPorCodigo(codigoParametroBooleano);
                cargaOcorrencaParametrosBooleano.Booleano = apenasReboque;

                if (cargaOcorrencaParametrosBooleano.Codigo == 0)
                    repCargaOcorrenciaParametros.Inserir(cargaOcorrencaParametrosBooleano);
                else
                    repCargaOcorrenciaParametros.Atualizar(cargaOcorrencaParametrosBooleano);
            }

            if (codigoParametroInteiro > 0)
            {
                Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaParametros cargaOcorrencaParametrosInteiro = null;

                if (atualizandoOcorrencia)
                    cargaOcorrencaParametrosInteiro = repCargaOcorrenciaParametros.BuscarPorCargaOcorrenciaETipo(ocorrencia.Codigo, TipoParametroOcorrencia.Inteiro);

                cargaOcorrencaParametrosInteiro ??= new Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaParametros();

                cargaOcorrencaParametrosInteiro.CargaOcorrencia = ocorrencia;
                cargaOcorrencaParametrosInteiro.ParametroOcorrencia = repParametroOcorrencia.BuscarPorCodigo(codigoParametroInteiro);
                cargaOcorrencaParametrosInteiro.Texto = parametroInteiro.ToString();

                if (cargaOcorrencaParametrosInteiro.Codigo == 0)
                    repCargaOcorrenciaParametros.Inserir(cargaOcorrencaParametrosInteiro);
                else
                    repCargaOcorrenciaParametros.Atualizar(cargaOcorrencaParametrosInteiro);
            }

            if (codigoParametroData1 > 0 && (parametroData1 > DateTime.MinValue))
            {
                Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaParametros cargaOcorrencaParametrosData1 = new Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaParametros
                {
                    CargaOcorrencia = ocorrencia,
                    ParametroOcorrencia = repParametroOcorrencia.BuscarPorCodigo(codigoParametroData1),
                    Data = parametroData1
                };
                repCargaOcorrenciaParametros.Inserir(cargaOcorrencaParametrosData1);
            }

            if (codigoParametroData2 > 0 && parametroData2 > DateTime.MinValue)
            {
                Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaParametros cargaOcorrencaParametrosData2 = new Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaParametros
                {
                    CargaOcorrencia = ocorrencia,
                    ParametroOcorrencia = repParametroOcorrencia.BuscarPorCodigo(codigoParametroData2),
                    Data = parametroData2
                };
                repCargaOcorrenciaParametros.Inserir(cargaOcorrencaParametrosData2);
            }

            if (codigoParametroTexto > 0 && !string.IsNullOrWhiteSpace(parametroTexto))
            {
                Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaParametros cargaOcorrencaParametrosTexto = null;

                if (atualizandoOcorrencia)
                    cargaOcorrencaParametrosTexto = repCargaOcorrenciaParametros.BuscarPorCargaOcorrenciaETipo(ocorrencia.Codigo, TipoParametroOcorrencia.Texto);

                cargaOcorrencaParametrosTexto ??= new Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaParametros();

                cargaOcorrencaParametrosTexto.CargaOcorrencia = ocorrencia;
                cargaOcorrencaParametrosTexto.ParametroOcorrencia = repParametroOcorrencia.BuscarPorCodigo(codigoParametroTexto);
                cargaOcorrencaParametrosTexto.Texto = parametroTexto;

                if (cargaOcorrencaParametrosTexto.Codigo == 0)
                    repCargaOcorrenciaParametros.Inserir(cargaOcorrencaParametrosTexto);
                else
                    repCargaOcorrenciaParametros.Atualizar(cargaOcorrencaParametrosTexto);
            }
        }

        private void CopiarOcorrencia(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrenciaOrigem, ref Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrenciaDestino)
        {
            ocorrenciaDestino.Carga = ocorrenciaOrigem.Carga;
            ocorrenciaDestino.CFOP = ocorrenciaOrigem.CFOP;
            ocorrenciaDestino.ComponenteFrete = ocorrenciaOrigem.ComponenteFrete;
            ocorrenciaDestino.ContaContabil = ocorrenciaOrigem.ContaContabil;
            ocorrenciaDestino.DataAlteracao = ocorrenciaOrigem.DataAlteracao;
            ocorrenciaDestino.DataFinalizacaoEmissaoOcorrencia = ocorrenciaOrigem.DataFinalizacaoEmissaoOcorrencia;
            ocorrenciaDestino.DataOcorrencia = ocorrenciaOrigem.DataOcorrencia;
            ocorrenciaDestino.GuidNomeArquivo = ocorrenciaOrigem.GuidNomeArquivo;
            ocorrenciaDestino.IncluirICMSFrete = ocorrenciaOrigem.IncluirICMSFrete;
            ocorrenciaDestino.ModeloDocumentoFiscal = ocorrenciaOrigem.ModeloDocumentoFiscal;
            ocorrenciaDestino.MotivoRejeicaoCancelamento = ocorrenciaOrigem.MotivoRejeicaoCancelamento;
            ocorrenciaDestino.NomeArquivo = ocorrenciaOrigem.NomeArquivo;
            ocorrenciaDestino.Observacao = ocorrenciaOrigem.Observacao;
            ocorrenciaDestino.ObservacaoCancelamento = ocorrenciaOrigem.ObservacaoCancelamento;
            ocorrenciaDestino.ObservacaoCTe = ocorrenciaOrigem.ObservacaoCTe;
            ocorrenciaDestino.PercentualAcresciomoValor = ocorrenciaOrigem.PercentualAcresciomoValor;
            ocorrenciaDestino.Responsavel = ocorrenciaOrigem.Responsavel;
            ocorrenciaDestino.SituacaoOcorrencia = ocorrenciaOrigem.SituacaoOcorrencia;
            ocorrenciaDestino.SituacaoOcorrenciaNoCancelamento = ocorrenciaOrigem.SituacaoOcorrenciaNoCancelamento;
            ocorrenciaDestino.SolicitacaoCredito = ocorrenciaOrigem.SolicitacaoCredito;
            ocorrenciaDestino.TipoOcorrencia = ocorrenciaOrigem.TipoOcorrencia;
            ocorrenciaDestino.OrigemOcorrencia = ocorrenciaOrigem.OrigemOcorrencia;
            ocorrenciaDestino.Usuario = ocorrenciaOrigem.Usuario;
            ocorrenciaDestino.ValorOcorrencia = ocorrenciaOrigem.ValorOcorrencia;
            ocorrenciaDestino.ValorOcorrenciaLiquida = ocorrenciaOrigem.ValorOcorrenciaLiquida;
            ocorrenciaDestino.ValorOcorrenciaOriginal = ocorrenciaOrigem.ValorOcorrenciaOriginal;
            ocorrenciaDestino.GrupoOcorrencia = ocorrenciaOrigem.GrupoOcorrencia;

            ocorrenciaDestino.ErroIntegracaoComGPA = ocorrenciaOrigem.ErroIntegracaoComGPA;
            ocorrenciaDestino.IntegradoComGPA = ocorrenciaOrigem.IntegradoComGPA;
            ocorrenciaDestino.AgImportacaoCTe = ocorrenciaOrigem.AgImportacaoCTe;
        }

        private Models.Grid.Grid MontarGridCTes(ref string propOrdenacao, bool exibirEspecieDocumento)
        {

            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("SituacaoCTe", false);
            grid.AdicionarCabecalho("CodigoCTE", false);
            grid.AdicionarCabecalho("NumeroModeloDocumentoFiscal", false);
            grid.AdicionarCabecalho("CodigoEmpresa", false);
            grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.Ocorrencia.Numero, "Numero", 7, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.Ocorrencia.Serie, "Serie", 7, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.Ocorrencia.Documento, "AbreviacaoModeloDocumentoFiscal", 10, Models.Grid.Align.center, true);

            if (exibirEspecieDocumento)
                grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.Ocorrencia.Especie, "Especie", 8, Models.Grid.Align.center, true);

            grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.Ocorrencia.NotaSFiscaiS, "NumeroNotas", 10, Models.Grid.Align.right, false);
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Terceiros)
            {
                grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.Ocorrencia.Tomador, "Tomador", 18, Models.Grid.Align.left, true);
            }
            else
            {
                grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.Ocorrencia.TipoDePagamento, "DescricaoTipoPagamento", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.Ocorrencia.Remetente, "Remetente", 18, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.Ocorrencia.Destinatario, "Destinatario", 18, Models.Grid.Align.left, true);
            }
            grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.Ocorrencia.Destino, "Destino", 15, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.Ocorrencia.ValorAReceber, "ValorFrete", 8, Models.Grid.Align.right, true);
            grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.Ocorrencia.Aliquota, "Aliquota", 5, Models.Grid.Align.right, false);
            grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.Ocorrencia.Status, "Status", 8, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.Ocorrencia.RetornoSefaz, "RetornoSefaz", 15, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("TipoDocumentoEmissao", false);
            grid.AdicionarCabecalho("CodigoSerie", false);
            grid.AdicionarCabecalho("Empresa", false);
            grid.AdicionarCabecalho("HabilitarSincronizarDocumento", false);

            propOrdenacao = grid.header[grid.indiceColunaOrdena].data;

            if (propOrdenacao == "Remetente" || propOrdenacao == "Destinatario")
                propOrdenacao += ".Nome";
            if (propOrdenacao == "Destino")
                propOrdenacao = "LocalidadeTerminoPrestacao.Descricao";

            if (propOrdenacao == "DescricaoTipoPagamento")
                propOrdenacao = "TipoPagamento";

            if (propOrdenacao == "DescricaoTipoServico")
                propOrdenacao = "TipoServico";

            if (propOrdenacao == "AbreviacaoModeloDocumentoFiscal")
                propOrdenacao = "ModeloDocumentoFiscal.Abreviacao";

            propOrdenacao = "CTe." + propOrdenacao;

            return grid;
        }

        private Models.Grid.Grid MontarGridDocumentosNFSManual(ref string propOrdenacao)
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request);
            grid.header = new List<Models.Grid.Head>();
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.Ocorrencia.Numero, "Numero", 5, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.Ocorrencia.Chave, "Chave", 18, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.Ocorrencia.Documento, "Abreviacao", 5, Models.Grid.Align.center, false);

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.Ocorrencia.Remetente, "Remetente", 18, Models.Grid.Align.left, true);

            grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.Ocorrencia.Destinatario, "Destinatario", 18, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.Ocorrencia.Destino, "Destino", 13, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.Ocorrencia.Peso, "Peso", 6, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.Ocorrencia.ValorDoFrete, "ValorFrete", 6, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.Ocorrencia.NotaFiscalDeServico, "NFS", 6, Models.Grid.Align.left, true);

            return grid;
        }

        private List<dynamic> MontarListaCTesNotaFiscais(List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento> cargaOcorrenciasDocumento)
        {
            List<dynamic> retorno = new List<dynamic>();

            foreach (Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento cargaOcorrenciaDocumento in cargaOcorrenciasDocumento)
            {
                foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal XMLNotaFiscais in cargaOcorrenciaDocumento.XMLNotaFiscais)
                {
                    retorno.Add(new
                    {
                        cargaOcorrenciaDocumento.CargaCTe.Codigo,
                        CodigoCargaCTe = cargaOcorrenciaDocumento.CargaCTe.Codigo,
                        CodigoCTE = cargaOcorrenciaDocumento.CargaCTe.CTe.Codigo,
                        cargaOcorrenciaDocumento.CargaCTe.CTe.DescricaoTipoServico,
                        NumeroModeloDocumentoFiscal = cargaOcorrenciaDocumento.CargaCTe.CTe.ModeloDocumentoFiscal.Numero,
                        TipoDocumentoEmissao = cargaOcorrenciaDocumento.CargaCTe.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao,
                        AbreviacaoModeloDocumentoFiscal = cargaOcorrenciaDocumento.CargaCTe.CTe.ModeloDocumentoFiscal.Abreviacao,
                        Especie = cargaOcorrenciaDocumento.CargaCTe.CTe.ModeloDocumentoFiscal.Especie ?? "",
                        CodigoEmpresa = cargaOcorrenciaDocumento.CargaCTe.CTe.Empresa.Codigo,
                        Empresa = cargaOcorrenciaDocumento.CargaCTe.CTe.Empresa.RazaoSocial,
                        cargaOcorrenciaDocumento.CargaCTe.CTe.Numero,
                        SituacaoCTe = cargaOcorrenciaDocumento.CargaCTe.CTe.Status,
                        CodigoSerie = cargaOcorrenciaDocumento.CargaCTe.CTe.Serie.Codigo,
                        Serie = cargaOcorrenciaDocumento.CargaCTe.CTe.Serie.Numero,
                        cargaOcorrenciaDocumento.CargaCTe.CTe.DescricaoTipoPagamento,
                        Remetente = cargaOcorrenciaDocumento.CargaCTe.CTe.Remetente?.Descricao,
                        Destinatario = cargaOcorrenciaDocumento.CargaCTe.CTe.Destinatario?.Descricao,
                        Destino = cargaOcorrenciaDocumento.CargaCTe.CTe.LocalidadeTerminoPrestacao.DescricaoCidadeEstado,
                        ValorFrete = cargaOcorrenciaDocumento.CargaCTe.CTe.ValorAReceber.ToString("n2"),
                        Aliquota = cargaOcorrenciaDocumento.CargaCTe.CTe.AliquotaICMS > 0 ? cargaOcorrenciaDocumento.CargaCTe.CTe.AliquotaICMS.ToString("n2") : cargaOcorrenciaDocumento.CargaCTe.CTe.AliquotaISS.ToString("n4"),
                        NumeroNotas = XMLNotaFiscais.Numero,
                        Status = cargaOcorrenciaDocumento.CargaCTe.CTe.DescricaoStatus,
                        RetornoSefaz = !string.IsNullOrWhiteSpace(cargaOcorrenciaDocumento.CargaCTe.CTe.MensagemRetornoSefaz) ? (cargaOcorrenciaDocumento.CargaCTe.CTe.MensagemRetornoSefaz != " - " ? System.Web.HttpUtility.HtmlEncode(cargaOcorrenciaDocumento.CargaCTe.CTe.MensagemRetornoSefaz) : "") : "",
                        Tomador = cargaOcorrenciaDocumento.CargaCTe.CTe.TomadorPagador != null ? cargaOcorrenciaDocumento.CargaCTe.CTe.TomadorPagador.Descricao : string.Empty,
                        HabilitarSincronizarDocumento = false
                    });
                }
            }

            return retorno;
        }

        private dynamic MontarListaCTes(List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes, List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe> cargaPedidosXMLsNotaFiscalCTe)
        {
            var lista = (from obj in cargaCTes
                         select new
                         {
                             obj.Codigo,
                             CodigoCTE = obj.CTe?.Codigo ?? 0,
                             DescricaoTipoServico = obj.CTe?.DescricaoTipoServico ?? obj.PreCTe.DescricaoTipoServico,
                             NumeroModeloDocumentoFiscal = obj.CTe?.ModeloDocumentoFiscal.Numero ?? "",
                             TipoDocumentoEmissao = obj.CTe?.ModeloDocumentoFiscal.TipoDocumentoEmissao ?? Dominio.Enumeradores.TipoDocumento.Nenhum,
                             AbreviacaoModeloDocumentoFiscal = obj.CTe?.ModeloDocumentoFiscal.Abreviacao ?? "Pré CT-e",
                             CodigoEmpresa = obj.CTe?.Empresa.Codigo ?? obj.PreCTe.Empresa.Codigo,
                             Empresa = obj.CTe?.Empresa.RazaoSocial ?? obj.PreCTe.Empresa.RazaoSocial,
                             Numero = obj.CTe?.Numero.ToString() ?? "",
                             SituacaoCTe = obj.CTe?.Status ?? "",
                             CodigoSerie = obj.CTe?.Serie.Codigo ?? 0,
                             Serie = obj.CTe?.Serie.Numero.ToString() ?? "",
                             DescricaoTipoPagamento = obj.CTe?.DescricaoTipoPagamento ?? "",
                             Remetente = obj.CTe != null ? (obj.CTe.Remetente != null ? obj.CTe.Remetente.Cliente.Descricao : string.Empty) : obj.PreCTe.Remetente.Cliente.Descricao,
                             Destinatario = obj.CTe != null ? (obj.CTe.Destinatario != null ? obj.CTe.Destinatario.Cliente.Descricao : string.Empty) : obj.PreCTe.Destinatario.Cliente.Descricao,
                             Destino = obj.CTe?.LocalidadeTerminoPrestacao.DescricaoCidadeEstado ?? obj.PreCTe.LocalidadeTerminoPrestacao.DescricaoCidadeEstado,
                             ValorFrete = obj.CTe?.ValorAReceber.ToString("n2") ?? obj.PreCTe.ValorAReceber.ToString("n2"),
                             Aliquota = obj.CTe != null ? (obj.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe ? obj.CTe.AliquotaICMS.ToString("n2") : obj.CTe.AliquotaISS.ToString("n4")) : obj.PreCTe.AliquotaICMS.ToString("n2"),
                             NumeroNotas = string.Join(", ", (from nf in cargaPedidosXMLsNotaFiscalCTe where nf.CargaCTe.Codigo == obj.Codigo select nf.PedidoXMLNotaFiscal.XMLNotaFiscal.Numero).Distinct().ToList()),
                             Status = obj.CTe?.DescricaoStatus ?? "",
                             RetornoSefaz = obj.CTe != null ? (!string.IsNullOrWhiteSpace(obj.CTe.MensagemRetornoSefaz) ? obj.CTe.MensagemRetornoSefaz != " - " ? System.Web.HttpUtility.HtmlEncode(obj.CTe.MensagemRetornoSefaz) : "" : "") : "",
                             Tomador = obj.CTe != null ? (obj.CTe.TomadorPagador != null ? obj.CTe.TomadorPagador.Cliente.Descricao : string.Empty) : (obj.PreCTe.Tomador?.Cliente?.Descricao ?? ""),
                             HabilitarSincronizarDocumento = false
                         }).ToList();

            return lista;
        }

        private dynamic MontarListaCTes(List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> cargaCTesComplementoInfo, Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCancelamento ocorrenciaCancelamento)
        {
            var lista = (from obj in cargaCTesComplementoInfo
                         where obj.CTe != null
                         select new
                         {
                             obj.Codigo,
                             CodigoCTE = obj.CTe.Codigo,
                             obj.CTe.DescricaoTipoServico,
                             NumeroModeloDocumentoFiscal = obj.CTe.ModeloDocumentoFiscal.Numero,
                             TipoDocumentoEmissao = obj.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao,
                             AbreviacaoModeloDocumentoFiscal = obj.CTe.ModeloDocumentoFiscal.Abreviacao,
                             Especie = obj.CTe.ModeloDocumentoFiscal.Especie ?? "",
                             CodigoEmpresa = obj.CTe.Empresa.Codigo,
                             Empresa = obj.CTe.Empresa.RazaoSocial,
                             obj.CTe.Numero,
                             SituacaoCTe = obj.CTe.Status,
                             CodigoSerie = obj.CTe.Serie.Codigo,
                             Serie = obj.CTe.Serie.Numero,
                             obj.CTe.DescricaoTipoPagamento,
                             Remetente = obj.CTe.Remetente?.Descricao,
                             Destinatario = obj.CTe.Destinatario?.Descricao,
                             Destino = obj.CTe.LocalidadeTerminoPrestacao.DescricaoCidadeEstado,
                             ValorFrete = obj.CTe.ValorAReceber.ToString("n2"),
                             Aliquota = obj.CTe.AliquotaICMS > 0 ? obj.CTe.AliquotaICMS.ToString("n2") : obj.CTe.AliquotaISS.ToString("n4"),
                             NumeroNotas = obj.CTe.ModeloDocumentoFiscal.Numero == "39" ? string.Empty : obj.CTe.NumeroNotas,
                             Status = obj.CTe.DescricaoStatus,
                             RetornoSefaz = !string.IsNullOrWhiteSpace(obj.CTe.MensagemRetornoSefaz) ? (obj.CTe.MensagemRetornoSefaz != " - " ? System.Web.HttpUtility.HtmlEncode(obj.CTe.MensagemRetornoSefaz) : "") : "",
                             Tomador = obj.CTe.TomadorPagador != null ? obj.CTe.TomadorPagador.Descricao : string.Empty,
                             HabilitarSincronizarDocumento = ObterHabilitarSincronizarDocumento(obj.CTe, ocorrenciaCancelamento) // obj.CTe.Status == "E" && obj.CTe.DataIntegracao != null && (System.DateTime.Now.AddMinutes(-30) > obj.CTe.DataIntegracao) && ((obj.CTe.CodigoCTeIntegrador != 0 && obj.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe) || (obj.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe && obj.CTe.SistemaEmissor == TipoEmissorDocumento.NSTech) || (obj.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe && obj.CTe.SistemaEmissor == TipoEmissorDocumento.Migrate)) ? true : false
                         }).ToList();

            return lista;
        }

        private bool ObterHabilitarSincronizarDocumento(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCancelamento ocorrenciaCancelamento)
        {
            if (cte.Status == "E" && cte.DataIntegracao != null && (System.DateTime.Now.AddMinutes(-30) > cte.DataIntegracao))
            {
                if (cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe)
                {
                    if (cte.SistemaEmissor == null || cte.SistemaEmissor == TipoEmissorDocumento.Integrador && cte.CodigoCTeIntegrador != 0)
                        return true;
                    else if (cte.SistemaEmissor == TipoEmissorDocumento.NSTech)
                        return true;
                }
                else if (cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe)
                {
                    if (cte.SistemaEmissor == TipoEmissorDocumento.Migrate)
                        return true;
                }
            }
            else if (cte.Status == "K" && ocorrenciaCancelamento?.DataCancelamento != null && (System.DateTime.Now.AddMinutes(-30) > ocorrenciaCancelamento?.DataCancelamento))
            {
                if (cte.SistemaEmissor == TipoEmissorDocumento.NSTech)
                    return true;
            }

            return false;
        }

        private Dominio.Entidades.Cliente RetornarClientePedido(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaDocumento repCargaOcorrenciaDocumento = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaDocumento(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.OcorrenciaTipoIntegracao repOcorrenciaTipoIntegracao = new Repositorio.Embarcador.Ocorrencias.OcorrenciaTipoIntegracao(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoOcorrencia repConfiguracaoOcorrencia = new Repositorio.Embarcador.Configuracoes.ConfiguracaoOcorrencia(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoOcorrencia configuracaoOcorrencia = repConfiguracaoOcorrencia.BuscarConfiguracaoPadrao();

            List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> listaCTe = repCargaOcorrenciaDocumento.BuscarCTesENFSesPorOcorrencia(ocorrencia.Codigo);
            if (listaCTe != null && listaCTe.Count > 0 && listaCTe.FirstOrDefault().TomadorPagador != null)
            {
                if (configuracaoOcorrencia?.ExibirDestinatarioOcorrencia ?? false)
                    return listaCTe.FirstOrDefault().Destinatario?.Cliente ?? null;

                if (ocorrencia.TipoOcorrencia != null && repOcorrenciaTipoIntegracao.PossuiIntegracaoPorTipoOcorrenciaETipoIntegracao(ocorrencia.TipoOcorrencia.Codigo, TipoIntegracao.Riachuelo))
                    return listaCTe.FirstOrDefault().Destinatario?.Cliente ?? null;
                else
                    return listaCTe.FirstOrDefault().TomadorPagador?.Cliente ?? null;
            }
            else
            {
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = null;
                if (ocorrencia.Carga.CargaAgrupada && !ConfiguracaoEmbarcador.GerarOcorrenciaParaCargaAgrupada)
                    cargaPedido = repCargaPedido.BuscarPrimeiraPorCargaOrigem(ocorrencia.Carga.Codigo);
                else
                    cargaPedido = repCargaPedido.BuscarPrimeiraPorCarga(ocorrencia.Carga.Codigo);

                return cargaPedido?.ObterTomador();
            }
        }

        private dynamic RetornarOcorrencia(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.CargaOcorrencia.Ocorrencia srvOcorrencia = new Servicos.Embarcador.CargaOcorrencia.Ocorrencia(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaParametros repCargaOcorrenciaParametros = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaParametros(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAnexos repCargaOcorrenciaAnexos = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAnexos(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaImagem repCargaOcorrenciaImagem = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaImagem(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.OcorrenciaCancelamento repOcorrenciaCancelamento = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCancelamento(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.OcorrenciaContratoMotorista repOcorrenciaContratoMotorista = new Repositorio.Embarcador.Ocorrencias.OcorrenciaContratoMotorista(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaObservacaoContribuinte repObservacaoContribuinte = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaObservacaoContribuinte(unitOfWork);
            Repositorio.Embarcador.Chamados.ChamadoOcorrencia repChamadoOcorrencia = new Repositorio.Embarcador.Chamados.ChamadoOcorrencia(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(unitOfWork);

            Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaParametros ocorrenciaParametroPeriodo = repCargaOcorrenciaParametros.BuscarPorCargaOcorrenciaETipo(ocorrencia.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroOcorrencia.Periodo);
            Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaParametros ocorrenciaParametroBooleano = repCargaOcorrenciaParametros.BuscarPorCargaOcorrenciaETipo(ocorrencia.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroOcorrencia.Booleano);
            List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaParametros> listaOcorrenciaParametroData = repCargaOcorrenciaParametros.BuscarListaPorCargaOcorrenciaETipo(ocorrencia.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroOcorrencia.Data);
            Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaParametros ocorrenciaParametroTexto = repCargaOcorrenciaParametros.BuscarPorCargaOcorrenciaETipo(ocorrencia.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroOcorrencia.Texto);
            Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaParametros ocorrenciaParametroInteiro = repCargaOcorrenciaParametros.BuscarPorCargaOcorrenciaETipo(ocorrencia.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroOcorrencia.Inteiro);
            List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAnexos> ocorrenciasAnexos = repCargaOcorrenciaAnexos.BuscarPorCodigoOcorrencia(ocorrencia.Codigo);
            List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaImagem> ocorrenciasImagens = repCargaOcorrenciaImagem.BuscarPorCodigoOcorrencia(ocorrencia.Codigo);
            Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCancelamento cancelamentoOcorrencia = repOcorrenciaCancelamento.BuscarPorOcorrencia(ocorrencia.Codigo);
            Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaContratoMotorista contratoMotorista = repOcorrenciaContratoMotorista.BuscarPorOcorrencia(ocorrencia.Codigo);
            List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaObservacaoContribuinte> observacoesFiscoContribuinte = repObservacaoContribuinte.BuscarPorOcorrencia(ocorrencia.Codigo);

            Dominio.Entidades.Cliente cliente = null;
            decimal minutosPeriodo = 0m;
            decimal distanciaCTes = 0m;
            string dataEntradaRaio = "";
            string dataSaidaRaio = "";

            if (!ocorrencia.OrigemOcorrenciaPorPeriodo)
                cliente = RetornarClientePedido(ocorrencia, unitOfWork);

            if (ocorrenciaParametroPeriodo != null)
            {
                if (ocorrenciaParametroPeriodo.TotalHoras == 0)
                    minutosPeriodo = Convert.ToDecimal((ocorrenciaParametroPeriodo.DataFim.Value - ocorrenciaParametroPeriodo.DataInicio.Value).TotalMinutes);
                else
                    minutosPeriodo = ocorrenciaParametroPeriodo.TotalHoras * 60;
            }

            List<Dominio.Entidades.Embarcador.Chamados.Chamado> chamados = repChamadoOcorrencia.BuscarChamadosPorOcorrencia(ocorrencia.Codigo);
            Dominio.Entidades.Embarcador.Chamados.Chamado chamado = chamados.Count > 0 ? chamados.FirstOrDefault() : null;

            if (ocorrencia.TipoOcorrencia != null)
            {
                Repositorio.Embarcador.Ocorrencias.GatilhoGeracaoAutomaticaOcorrencia repositorioGatilho = new Repositorio.Embarcador.Ocorrencias.GatilhoGeracaoAutomaticaOcorrencia(unitOfWork);
                Dominio.Entidades.Embarcador.Ocorrencias.GatilhoGeracaoAutomaticaOcorrencia gatilho = repositorioGatilho.BuscarPorTipoOcorrencia(ocorrencia.TipoOcorrencia.Codigo);

                if (ocorrencia.TipoOcorrencia.CalcularDistanciaPorCTe)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> cargaCTeComplementoInfos = repCargaCTeComplementoInfo.BuscarPorOcorrencia(ocorrencia.Codigo);
                    distanciaCTes = cargaCTeComplementoInfos.Sum(x => x.CTeComplementado.Distancia);
                }

                if ((ocorrencia.TipoOcorrencia?.UtilizarEntradaSaidaDoRaioCargaEntrega ?? false) || (gatilho?.DefinirAutomaticamenteTempoEstadiaPorTempoParadaNoLocalEntrega ?? false))
                {
                    Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repositorioCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);
                    Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                    Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaDocumento repositorioCargaOcorrenciaDocumento = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaDocumento(unitOfWork);

                    Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = repositorioCargaOcorrenciaDocumento.BuscarCTePorOcorrencia(ocorrencia.Codigo);
                    Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = (cargaCTe != null) ? repositorioCargaPedidoXMLNotaFiscalCTe.BuscarPrimeiraCargaPedidoPorCargaCTe(cargaCTe.Codigo) : null;
                    Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = (cargaPedido != null) ? repositorioCargaEntrega.BuscarEntregaPorCargaPedido(cargaPedido.Codigo) : null;

                    if ((cargaEntrega != null) && cargaEntrega.DataEntradaRaio.HasValue && cargaEntrega.DataSaidaRaio.HasValue)
                    {
                        dataEntradaRaio = cargaEntrega.DataEntradaRaio.Value.ToString("dd/MM/yyyy HH:mm");
                        dataSaidaRaio = cargaEntrega.DataSaidaRaio.Value.ToString("dd/MM/yyyy HH:mm");
                    }
                }
            }

            var dynOcorrencia = new
            {
                ocorrencia.Codigo,
                ocorrencia.NumeroOcorrencia,
                ocorrencia.NumeroOcorrenciaCliente,
                ocorrencia.Quantidade,
                ocorrencia.OcorrenciaReprovada,
                ocorrencia.OcorrenciaDeEstadia,
                CTeEmitidoNoEmbarcador = ocorrencia.Carga == null ? false : srvOcorrencia.RetornarTomadorEmiteCTeNoEmbarcador(ocorrencia, ocorrencia.Carga.Codigo, ocorrencia.Responsavel, ocorrencia.Tomador?.CPF_CNPJ ?? 0d, unitOfWork),
                CodigosAgrupadosCarga = ocorrencia.Carga == null ? "" : string.Join(", ", ocorrencia.Carga.CodigosAgrupados) ?? "",
                OcorrenciaPorPeriodo = ocorrencia.OrigemOcorrenciaPorPeriodo,
                TipoEmissaoDocumentoOcorrencia = ocorrencia.TipoOcorrencia?.TipoEmissaoDocumentoOcorrencia,
                ocorrencia.OrigemOcorrencia,
                Periodo = ocorrencia.Periodo,
                PeriodoInicio = ocorrencia.PeriodoInicio.HasValue ? ocorrencia.PeriodoInicio.Value.ToString("dd/MM/yyyy") : string.Empty,
                PeriodoFim = ocorrencia.PeriodoFim.HasValue ? ocorrencia.PeriodoFim.Value.ToString("dd/MM/yyyy") : string.Empty,
                Carga = ocorrencia.Carga == null ? null : new { ocorrencia.Carga.Codigo, Descricao = (ocorrencia.Carga.Filial != null && TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador) ? (ocorrencia.Carga.CodigoCargaEmbarcador + " (" + ocorrencia.Carga.Filial.Descricao + ")") : ocorrencia.Carga.CodigoCargaEmbarcador },
                ComponenteFrete = new
                {
                    Codigo = ocorrencia.ComponenteFrete?.Codigo ?? 0,
                    Descricao = ocorrencia.ComponenteFrete?.Descricao ?? string.Empty,
                    TipoComponenteFrete = ocorrencia.ComponenteFrete?.TipoComponenteFrete ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.OUTROS
                },
                TipoOcorrencia = new
                {
                    Codigo = ocorrencia.TipoOcorrencia?.Codigo ?? 0,
                    Descricao = ocorrencia.TipoOcorrencia?.Descricao ?? string.Empty,
                    CalculaValorPorTabelaFrete = ocorrencia.TipoOcorrencia?.CalculaValorPorTabelaFrete ?? false,
                    NaoCalcularValorOcorrenciaAutomaticamente = ocorrencia.TipoOcorrencia?.NaoCalcularValorOcorrenciaAutomaticamente ?? false,
                    OcorrenciaComplementoValorFreteCarga = ocorrencia.TipoOcorrencia?.OcorrenciaComplementoValorFreteCarga ?? false
                },
                ocorrencia.NotificarDebitosAtivos,
                DataOcorrencia = ocorrencia.DataOcorrencia.ToString("dd/MM/yyyy HH:mm"),
                ocorrencia.SituacaoOcorrencia,
                MotivoCancelamento = cancelamentoOcorrencia?.MotivoCancelamento ?? string.Empty,
                SituacaoOcorrenciaNoCancelamento = cancelamentoOcorrencia?.SituacaoOcorrenciaNoCancelamento ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.Todas,
                ocorrencia.ValorOcorrencia,
                ocorrencia.ValorOcorrenciaOriginal,
                ocorrencia.Observacao,
                ocorrencia.Motivo,
                ocorrencia.LiberadaParaEmissaoCTeSubContratacaoFilialEmissora,
                ocorrencia.IntegrandoFilialEmissora,
                ocorrencia.ReenviouIntegracaoFilialEmissora,
                ocorrencia.PossuiNFSManual,
                ocorrencia.NFSManualPendenteGeracao,
                QuantidadeMotorista = contratoMotorista?.QuantidadeMotoristas ?? 0,
                QuantidadeDiasMotorista = contratoMotorista?.QuantidadeDias ?? 0,
                ValorDiarioMotorista = (contratoMotorista?.ValorDiaria ?? 0).ToString("n2"),
                ValorQuinzenalMotorista = (contratoMotorista?.ValorQuinzena ?? 0).ToString("n2"),
                TotalMotorista = (contratoMotorista?.Total ?? 0).ToString("n2"),
                ocorrencia.ObservacaoCTe,
                Empresa = new { Codigo = ocorrencia.Emitente?.Codigo ?? 0, Descricao = ocorrencia.Emitente?.Descricao ?? "" },
                ocorrencia.AgImportacaoCTe,
                ocorrencia.EmiteComplementoFilialEmissora,
                GrupoOcorrencia = new
                {
                    Codigo = ocorrencia.GrupoOcorrencia?.Codigo ?? 0,
                    Descricao = ocorrencia.GrupoOcorrencia?.Descricao ?? string.Empty
                },
                Chamado = new { Codigo = chamado?.Codigo ?? 0, Descricao = chamado?.Numero.ToString() ?? string.Empty },
                Chamados = (from obj in chamados
                            select new
                            {
                                obj.Codigo,
                                obj.Descricao
                            }),
                ContratoFreteTransportador = ocorrencia.ContratoFrete != null ? new { ocorrencia.ContratoFrete.Codigo, ocorrencia.ContratoFrete.Descricao } : new { Codigo = 0, Descricao = "" },
                TipoTomador = ocorrencia.Responsavel.HasValue ? (int)ocorrencia.Responsavel : 99,
                Tomador = new
                {
                    Descricao = ocorrencia.Tomador != null ? ocorrencia.Tomador.Descricao : string.Empty,
                    Codigo = ocorrencia.Tomador?.CPF_CNPJ_SemFormato ?? "0"
                },
                Cliente = cliente != null ? cliente.Descricao : string.Empty,
                ocorrencia.DescricaoSituacao,
                DadosModeloDocumentoFiscal = ocorrencia.ModeloDocumentoFiscal != null ? new
                {
                    ocorrencia.ModeloDocumentoFiscal.Codigo,
                    ocorrencia.ModeloDocumentoFiscal.Abreviacao,
                    ocorrencia.ModeloDocumentoFiscal.Data,
                    ocorrencia.ModeloDocumentoFiscal.Descricao,
                    ocorrencia.ModeloDocumentoFiscal.Editavel,
                    ocorrencia.ModeloDocumentoFiscal.Numero,
                    ocorrencia.ModeloDocumentoFiscal.Status
                } : null,
                Solicitante = new { Codigo = ocorrencia.Usuario != null ? ocorrencia.Usuario.Codigo : 0, Descricao = ocorrencia.Usuario != null ? ocorrencia.Usuario.Nome : "" },
                SolicitacaoCredito = ObterSolicitacaoCredito(ocorrencia),
                ResumoAutorizacaoAprovacaoOcorrencia = ObterResumoAutorizacao(ocorrencia, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoOcorrencia.AprovacaoOcorrencia, unitOfWork),
                ResumoAutorizacaoEmissaoOcorrencia = ObterResumoAutorizacao(ocorrencia, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoOcorrencia.EmissaoOcorrencia, unitOfWork),
                ModeloDocumentoFiscal = (ocorrencia.ModeloDocumentoFiscal != null && ocorrencia.ModeloDocumentoFiscal.Numero != "57") ? new { ocorrencia.ModeloDocumentoFiscal.Codigo, ocorrencia.ModeloDocumentoFiscal.Descricao } : new { Codigo = 0, Descricao = "" },
                CobrarOutroDocumento = (ocorrencia.ModeloDocumentoFiscal != null && ocorrencia.ModeloDocumentoFiscal.Numero != "57") ? true : false,
                CodigoParametroPeriodo = ocorrenciaParametroPeriodo != null ? ocorrenciaParametroPeriodo.ParametroOcorrencia.Codigo : 0,
                DataEntradaRaio = dataEntradaRaio,
                DataSaidaRaio = dataSaidaRaio,
                DataInicio = ocorrenciaParametroPeriodo != null ? ocorrenciaParametroPeriodo.DataInicio.ToString() : string.Empty,
                DataFim = ocorrenciaParametroPeriodo != null ? ocorrenciaParametroPeriodo.DataFim.ToString() : string.Empty,
                TotalHoras = minutosPeriodo.FromMinutesToFormattedTime(),
                CodigoParametroBooleano = ocorrenciaParametroBooleano != null ? ocorrenciaParametroBooleano.ParametroOcorrencia.Codigo : 0,
                ApenasReboque = ocorrenciaParametroBooleano != null ? ocorrenciaParametroBooleano.Booleano : false,
                CargaOcorrenciaVinculada = ocorrencia.CargaOcorrenciaVinculada,
                CodigoPagametroData1 = listaOcorrenciaParametroData.Count > 0 ? listaOcorrenciaParametroData[0].ParametroOcorrencia.Codigo : 0,
                ParametroData1 = listaOcorrenciaParametroData.Count > 0 ? listaOcorrenciaParametroData[0].Data.ToString() : string.Empty,
                CodigoPagametroData2 = listaOcorrenciaParametroData.Count > 1 ? listaOcorrenciaParametroData[1].ParametroOcorrencia.Codigo : 0,
                ParametroData2 = listaOcorrenciaParametroData.Count > 1 ? listaOcorrenciaParametroData[1].Data.ToString() : string.Empty,
                CodigoParametroTexto = ocorrenciaParametroTexto != null ? ocorrenciaParametroTexto.Codigo : 0,
                ParametroTexto = ocorrenciaParametroTexto != null ? ocorrenciaParametroTexto.Texto : string.Empty,
                CodigoParametroInteiro = ocorrenciaParametroInteiro != null ? ocorrenciaParametroInteiro.Codigo : 0,
                ParametroInteiro = ocorrenciaParametroInteiro != null ? ocorrenciaParametroInteiro.Texto : string.Empty,
                Anexos = from obj in ocorrenciasAnexos
                         select new
                         {
                             obj.Codigo,
                             obj.Descricao,
                             obj.NomeArquivo,
                         },
                // Envia essa informacao para permitir inserir anexos numa ocorrencia finalizada
                HashAnexos = ocorrencia.DataAlteracao.ToString("ddMMyyyyHHmm"),
                ocorrencia.BaseCalculoICMS,
                ocorrencia.AliquotaICMS,
                ocorrencia.ValorICMS,
                ocorrencia.CSTICMS,
                ocorrencia.QuantidadeAjudantes,
                ocorrencia.Quilometragem,
                Veiculo = ocorrencia.Veiculo != null ? new { ocorrencia.Veiculo.Codigo, Descricao = ocorrencia.Veiculo.Placa } : null,
                DTNatura = new
                {
                    Codigo = ocorrencia.DTNatura?.Codigo ?? 0,
                    Descricao = ocorrencia.DTNatura?.Numero.ToString() ?? string.Empty
                },
                EmiteNFSeFora = (ocorrencia.ObterEmitenteOcorrencia()?.EmiteNFSeOcorrenciaForaEmbarcador) ?? false,
                ocorrencia.ErroIntegracaoComGPA,
                MensagemPendencia = ocorrencia.MensagemPendencia ?? string.Empty,
                ListaImagens = (from obj in ocorrenciasImagens
                                select new
                                {
                                    obj.Codigo,
                                    CodigoOcorrencia = obj.CargaOcorrencia.Codigo,
                                    Arquivo = obj.NomeArquivo
                                }).ToList(),
                ocorrencia.Latitude,
                ocorrencia.Longitude,
                ocorrencia.NomeRecebedor,
                ocorrencia.TipoDocumentoRecebedor,
                ocorrencia.NumeroDocumentoRecebedor,
                UsuarioResponsavelAprovacao = ocorrencia.UsuarioResponsavelAprovacao != null ? new { ocorrencia.UsuarioResponsavelAprovacao.Codigo, Descricao = ocorrencia.UsuarioResponsavelAprovacao.Nome } : null,
                DataEvento = ocorrencia.DataEvento?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                Moeda = ocorrencia.Moeda ?? MoedaCotacaoBancoCentral.Real,
                ValorCotacaoMoeda = (ocorrencia.ValorCotacaoMoeda ?? 0m).ToString("n10"),
                ValorTotalMoeda = ocorrencia.ValorTotalMoeda ?? 0m,
                Filial = new { Codigo = ocorrencia.Filial?.Codigo ?? 0, Descricao = ocorrencia.Filial?.Descricao ?? "" },
                CTeTerceiro = new { Codigo = ocorrencia.CTeTerceiro?.Codigo ?? 0, Descricao = ocorrencia.CTeTerceiro?.Descricao ?? string.Empty },
                RecarregarDados = ocorrencia.TipoOcorrencia?.NovaOcorrenciaAguardandoInformacoes ?? false,
                ObservacoesFiscoContribuinte = (from obj in observacoesFiscoContribuinte
                                                select new
                                                {
                                                    obj.Codigo,
                                                    obj.Identificador,
                                                    Descricao = obj.Texto,
                                                    Tipo = obj.Tipo.ObterDescricao(),
                                                    TipoCodigo = obj.Tipo.ToString("d")
                                                }).ToList(),
                DistanciaCTes = distanciaCTes > 0 ? distanciaCTes.ToString("n2") : "",
                ocorrencia.UtilizarSelecaoPorNotasFiscaisCTe,
                TiposCausadoresOcorrencia = new { Codigo = ocorrencia.TiposCausadoresOcorrencia?.Codigo, Descricao = ocorrencia.TiposCausadoresOcorrencia?.Descricao },
                CausasTipoOcorrencia = new { Codigo = ocorrencia.CausasTipoOcorrencia?.Codigo, Descricao = ocorrencia.CausasTipoOcorrencia?.Descricao },
            };

            return dynOcorrencia;
        }

        private object ObterResumoAutorizacao(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoOcorrencia etapa, Repositorio.UnitOfWork unitOfWork)
        {
            if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Terceiros &&
                TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                return null;

            Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao repCargaOcorrenciaAutorizacao = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao(unitOfWork);

            List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao> aprovacoes = repCargaOcorrenciaAutorizacao.BuscarPorOcorrenciaUsuarioEtapa(ocorrencia.Codigo, 0, etapa);
            Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao regra = null;

            if ((ocorrencia.SituacaoOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.Rejeitada && etapa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoOcorrencia.AprovacaoOcorrencia) ||
            (ocorrencia.SituacaoOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.RejeitadaEtapaEmissao && etapa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoOcorrencia.EmissaoOcorrencia))
                regra = (from o in aprovacoes where o.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaAutorizacao.Rejeitada orderby o.Motivo.Length descending select o).FirstOrDefault();
            else
            {
                if (aprovacoes.Count > 0 && (ocorrencia.TipoOcorrencia?.ExibirMotivoUltimaAprovacaoPortalTransportador ?? false))
                {
                    regra = aprovacoes.OrderByDescending(obj => obj.Prioridade)
                            .ThenBy(obj => obj.Data.HasValue ? 0 : 1)
                            .ThenByDescending(obj => obj.Data)
                            .FirstOrDefault();

                    if (ocorrencia.SituacaoOcorrencia == SituacaoOcorrencia.AgAprovacao)
                    {
                        string nomeRegra = regra?.RegrasAutorizacaoOcorrencia?.Descricao
                            ?? aprovacoes
                                .OrderByDescending(aprovacao => aprovacao.Prioridade)
                                .FirstOrDefault(aprovacao => aprovacao.RegrasAutorizacaoOcorrencia != null)?.RegrasAutorizacaoOcorrencia.Descricao
                            ?? "";

                        return new
                        {
                            RegraResumo = nomeRegra,
                            SituacaoResumo = SituacaoOcorrenciaAutorizacaoHelper.ObterDescricao(SituacaoOcorrenciaAutorizacao.Pendente),
                        };
                    }
                }
                else
                {
                    regra = (from o in aprovacoes where o.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaAutorizacao.Aprovada orderby o.Data ascending select o).FirstOrDefault();
                }
            }

            if (regra == null)
                return null;

            var retorno = new
            {
                RegraResumo = regra.RegrasAutorizacaoOcorrencia?.Descricao ?? "",
                DataResumo = regra.Data?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                SituacaoResumo = regra.DescricaoSituacao,
                JustificativaResumo = regra.MotivoRejeicaoOcorrencia?.Descricao ?? string.Empty,
                MotivoResumo = regra.Motivo ?? string.Empty,
            };

            return retorno;
        }

        private dynamic ObterSolicitacaoCredito(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia)
        {
            var retorno = new
            {
                RetornoSolicitacao = ocorrencia.SolicitacaoCredito != null ? ocorrencia.SolicitacaoCredito.RetornoSolicitacao : string.Empty,
                ValorLiberado = ocorrencia.SolicitacaoCredito != null ? ocorrencia.SolicitacaoCredito.ValorLiberado.ToString("n2") : string.Empty,
                ValorSolicitado = ocorrencia.SolicitacaoCredito != null ? ocorrencia.SolicitacaoCredito.ValorSolicitado.ToString("n2") : ocorrencia.ValorOcorrenciaOriginal.ToString("n2"),
                Creditor = ocorrencia.SolicitacaoCredito != null && ocorrencia.SolicitacaoCredito.Creditor != null ? ocorrencia.SolicitacaoCredito.Creditor.Nome : "",
                Solicitado = ocorrencia.SolicitacaoCredito != null ? ocorrencia.SolicitacaoCredito.Solicitado.Nome : ocorrencia.Usuario?.Nome ?? string.Empty,
                Solicitante = ocorrencia.SolicitacaoCredito != null ? ocorrencia.SolicitacaoCredito.Solicitante.Nome : string.Empty,
                MotivoSolicitacao = ocorrencia.Observacao,
                DataSolicitacao = ocorrencia.DataOcorrencia.ToString("dd/MM/yyyy HH:mm"),
                SituacaoSolicitacaoCredito = ocorrencia.SolicitacaoCredito != null ? ocorrencia.SolicitacaoCredito.SituacaoSolicitacaoCredito : Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSolicitacaoCredito.Todos,
                DescricaoSituacao = ocorrencia.SolicitacaoCredito != null ? ocorrencia.SolicitacaoCredito.DescricaoSituacao : string.Empty,
                DataRetorno = ocorrencia.SolicitacaoCredito != null && ocorrencia.SolicitacaoCredito.DataRetorno.HasValue ? ocorrencia.SolicitacaoCredito.DataRetorno.Value.ToString("dd/MM/yyyy HH:mm") : "",
                ocorrencia.ObservacaoAprovador
            };

            return retorno;
        }

        private Models.Grid.Grid GridConsultarAutorizacoes(Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoOcorrencia etapa, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao repCargaOcorrenciaAutorizacao = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao(unitOfWork);

            int codOcorrencia = int.Parse(Request.Params("Codigo"));

            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Regra", false);
            grid.AdicionarCabecalho("Data", false);
            grid.AdicionarCabecalho("Motivo", false);
            grid.AdicionarCabecalho("Justificativa", false);
            grid.AdicionarCabecalho("DT_RowColor", false);
            grid.AdicionarCabecalho("DT_FontColor", false);
            grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.Ocorrencia.Usuario, "Usuario", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.Ocorrencia.Prioridade, "PrioridadeAprovacao", 5, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.Ocorrencia.Situacao, "Situacao", 5, Models.Grid.Align.center, false);

            string propOrdenacao = grid.header[grid.indiceColunaOrdena].data;

            List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao> listaCargaOcorrenciaAutorizacao = repCargaOcorrenciaAutorizacao.ConsultarAutorizacoesPorOcorrenciaEEtapa(codOcorrencia, etapa, propOrdenacao, grid.dirOrdena, grid.inicio, grid.limite);
            grid.setarQuantidadeTotal(repCargaOcorrenciaAutorizacao.ContarConsultaAutorizacoesPorOcorrencia(codOcorrencia, etapa));

            var lista = (from obj in listaCargaOcorrenciaAutorizacao
                         select new
                         {
                             obj.Codigo,
                             PrioridadeAprovacao = obj.Prioridade,
                             Situacao = obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaAutorizacao.Pendente ? "Pendente" : obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaAutorizacao.Aprovada ? "Aprovada" : obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaAutorizacao.Rejeitada ? "Rejeitada" : string.Empty,
                             Usuario = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe ? obj.Usuario?.ExibirUsuarioAprovacao ?? false ? obj.Usuario?.Nome ?? string.Empty : obj.RegrasAutorizacaoOcorrencia?.Descricao ?? string.Empty : obj.Usuario?.Nome ?? string.Empty,
                             Regra = TituloRegra(obj),
                             Data = obj.Data != null ? obj.Data.ToString() : string.Empty,
                             Motivo = !string.IsNullOrWhiteSpace(obj.Motivo) ? obj.Motivo : string.Empty,
                             Justificativa = obj.MotivoRejeicaoOcorrencia?.Descricao,
                             obj.Bloqueada,
                             DT_RowColor = obj.Bloqueada ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Cinza : (obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaAutorizacao.Aprovada ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Verde : obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaAutorizacao.Rejeitada ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Vermelho : obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaAutorizacao.Pendente ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Amarelo : ""),
                             DT_FontColor = obj.Bloqueada ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Branco : (obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaAutorizacao.Rejeitada ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Branco : "")
                         }).ToList();
            grid.AdicionaRows(lista);

            return grid;
        }

        private string TituloRegra(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao regra)
        {
            if (regra.OrigemRegraOcorrencia == OrigemRegraOcorrencia.Delegada)
                return "(Delegado)" + (!string.IsNullOrWhiteSpace(regra.Observacao) ? " - " + regra.Observacao : "");
            else if (regra.OrigemRegraOcorrencia == OrigemRegraOcorrencia.Assumida)
                return "(Assumido)";
            else
                return regra.RegrasAutorizacaoOcorrencia?.Descricao;
        }

        private void PreencherEntidadeComRequest(ref Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unitOfWork);
            Repositorio.Embarcador.Frete.ContratoFreteTransportador repContratoFreteTransportador = new Repositorio.Embarcador.Frete.ContratoFreteTransportador(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);
            Repositorio.TipoDeOcorrenciaDeCTe repTipoDeOcorrenciaDeCTe = new Repositorio.TipoDeOcorrenciaDeCTe(unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Integracao.DocumentoTransporteNatura repDTNatura = new Repositorio.Embarcador.Integracao.DocumentoTransporteNatura(unitOfWork);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
            Repositorio.Embarcador.CTe.CTeTerceiro repCTeTerceiro = new Repositorio.Embarcador.CTe.CTeTerceiro(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao repCargaOcorrenciaAutorizacao = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.TiposCausadoresOcorrencia repositorioTiposCausadoresOcorrencia = new Repositorio.Embarcador.Ocorrencias.TiposCausadoresOcorrencia(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.GrupoTipoDeOcorrenciaDeCTe repGrupoOcorrencia = new Repositorio.Embarcador.Ocorrencias.GrupoTipoDeOcorrenciaDeCTe(unitOfWork);
            Repositorio.Embarcador.TipoOcorrencia.TipoOcorrenciaCausas repositorioTipoOcorrenciaCausas = new Repositorio.Embarcador.TipoOcorrencia.TipoOcorrenciaCausas(unitOfWork);

            bool.TryParse(Request.Params("CTeEmitidoNoEmbarcador"), out bool cteEmitidoNoEmbarcador);

            int codigoCTeTerceiro = Request.GetIntParam("CTeTerceiro");
            int codigoTipoCausadorOcorrencia = Request.GetIntParam("TiposCausadoresOcorrencia");
            int codigoCausasTipoOcorrencia = Request.GetIntParam("CausasTipoOcorrencia");

            int.TryParse(Request.Params("ComponenteFrete"), out int componenteFrete);
            int.TryParse(Request.Params("TipoOcorrencia"), out int tipoOcorrencia);
            int.TryParse(Request.Params("Carga"), out int carga);
            int.TryParse(Request.Params("Veiculo"), out int veiculo);
            int.TryParse(Request.Params("DTNatura"), out int codigoDTNatura);
            int.TryParse(Request.Params("Quantidade"), out int quantidade);
            int.TryParse(Request.Params("ContratoFreteTransportador"), out int contratoFreteTransportador);
            int.TryParse(Request.Params("UsuarioResponsavelAprovacao"), out int codigoUsuarioResponsavel);
            int.TryParse(Request.Params("Filial"), out int filial);
            int.TryParse(Request.Params("GrupoOcorrencia"), out int codigoGrupoOcorrencia);

            DateTime.TryParseExact(Request.Params("DataOcorrencia"), "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out DateTime dataOcorrencia);
            DateTime? periodoInicio = Request.GetNullableDateTimeParam("PeriodoInicio");
            DateTime? periodoFim = Request.GetNullableDateTimeParam("PeriodoFim");
            DateTime? dataEvento = Request.GetNullableDateTimeParam("DataEvento");

            double.TryParse(Request.Params("Tomador"), out double cpfCnpjTomador);

            decimal.TryParse(Request.Params("PercentualAcrescimoValor"), out decimal percentualAcrescimoValor);
            decimal.TryParse(Request.Params("ValorOcorrencia"), out decimal valorOcorrencia);
            decimal.TryParse(Request.Params("BaseCalculoICMS"), out decimal baseCalculoICMS);
            decimal.TryParse(Request.Params("AliquotaICMS"), out decimal aliquotaICMS);
            decimal.TryParse(Request.Params("ValorICMS"), out decimal valorICMS);
            decimal.TryParse(Request.Params("Quilometragem"), out decimal quilometragem);

            string numeroOcorrenciaCliente = Request.Params("NumeroOcorrenciaCliente") ?? string.Empty;
            string motivo = Request.Params("Motivo") ?? string.Empty;
            string observacaoOcorrencia = Request.Params("ObservacaoOcorrencia") ?? string.Empty;
            string observacaoCTe = Request.Params("ObservacaoCTe") ?? string.Empty;
            string observacaoContrato = Request.Params("DescontarValoresOutrasCargas") ?? string.Empty;
            string nomeRecebedor = Request.Params("NomeRecebedor");
            string tipoDocumentoRecebedor = Request.Params("TipoDocumentoRecebedor");
            string numeroDocumentoRecebedor = Request.Params("NumeroDocumentoRecebedor");

            bool.TryParse(Request.Params("NotificarDebitosAtivos"), out bool notificarDebitosAtivos);
            bool.TryParse(Request.Params("UtilizarSelecaoPorNotasFiscaisCTe"), out bool utilizarSelecaoPorNotasFiscaisCTe);

            Dominio.Enumeradores.TipoTomador? tipoTomador = null;
            if (Request.Params("TipoTomador") != "99" && Enum.TryParse(Request.Params("TipoTomador"), out Dominio.Enumeradores.TipoTomador tipoTomadorAux))
                tipoTomador = tipoTomadorAux;

            ocorrencia.CTeTerceiro = codigoCTeTerceiro > 0 ? repCTeTerceiro.BuscarPorCodigo(codigoCTeTerceiro) : null;
            ocorrencia.Veiculo = veiculo > 0 ? repVeiculo.BuscarPorCodigo(veiculo) : null;
            ocorrencia.Carga = carga > 0 ? repCarga.BuscarPorCodigo(carga) : null;
            ocorrencia.Responsavel = tipoTomador;
            ocorrencia.Quantidade = quantidade;
            ocorrencia.ComponenteFrete = componenteFrete > 0 ? repComponenteFrete.BuscarPorCodigo(componenteFrete) : null;
            ocorrencia.TipoOcorrencia = tipoOcorrencia > 0 ? repTipoDeOcorrenciaDeCTe.BuscarPorCodigo(tipoOcorrencia) : null;
            ocorrencia.PercentualAcresciomoValor = percentualAcrescimoValor > 0 ? percentualAcrescimoValor : (ocorrencia.TipoOcorrencia?.PercentualAcrescimo ?? 0m);
            ocorrencia.Tomador = cpfCnpjTomador > 0d ? repCliente.BuscarPorCPFCNPJ(cpfCnpjTomador) : null;
            ocorrencia.DataOcorrencia = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Terceiros ? DateTime.Now : dataOcorrencia;
            ocorrencia.PeriodoInicio = periodoInicio;
            ocorrencia.PeriodoFim = periodoFim;
            ocorrencia.Observacao = Utilidades.String.RemoveAllSpecialCharactersNotCommon(Request.GetStringParam("Observacao"));
            ocorrencia.Motivo = motivo;
            ocorrencia.NumeroOcorrenciaCliente = numeroOcorrenciaCliente;
            ocorrencia.NumeroOcorrencia = Servicos.Embarcador.CargaOcorrencia.OcorrenciaSequencial.GetInstance().ObterProximoNumeroSequencial(unitOfWork);
            ocorrencia.DataAlteracao = DateTime.Now;
            ocorrencia.DataFinalizacaoEmissaoOcorrencia = DateTime.Now;
            ocorrencia.ObservacaoCTe = observacaoCTe;
            ocorrencia.Usuario = this.Usuario;
            ocorrencia.SituacaoOcorrencia = SituacaoOcorrencia.Finalizada;
            ocorrencia.ValorICMS = valorICMS;
            ocorrencia.BaseCalculoICMS = baseCalculoICMS;
            ocorrencia.AliquotaICMS = aliquotaICMS;
            ocorrencia.CSTICMS = Request.GetNullableStringParam("CSTICMS");
            ocorrencia.DTNatura = codigoDTNatura > 0 ? repDTNatura.BuscarPorCodigo(codigoDTNatura) : null;
            ocorrencia.ComplementoValorFreteCarga = ocorrencia.TipoOcorrencia?.OcorrenciaComplementoValorFreteCarga ?? false;
            ocorrencia.NomeRecebedor = nomeRecebedor;
            ocorrencia.TipoDocumentoRecebedor = tipoDocumentoRecebedor;
            ocorrencia.NumeroDocumentoRecebedor = numeroDocumentoRecebedor;
            ocorrencia.NotificarDebitosAtivos = notificarDebitosAtivos;
            ocorrencia.CTeEmitidoNoEmbarcador = cteEmitidoNoEmbarcador;
            ocorrencia.ContratoFrete = contratoFreteTransportador > 0 ? repContratoFreteTransportador.BuscarPorCodigo(contratoFreteTransportador) : null;
            ocorrencia.DataEvento = dataEvento;
            ocorrencia.Filial = filial > 0 ? repFilial.BuscarPorCodigo(filial) : null;
            ocorrencia.NaoGerarDocumento = ocorrencia.TipoOcorrencia?.NaoGerarDocumento ?? false;
            ocorrencia.UsuarioResponsavelAprovacao = codigoUsuarioResponsavel > 0 ? repUsuario.BuscarPorCodigo(codigoUsuarioResponsavel) : null;
            ocorrencia.QuantidadeAjudantes = Request.GetIntParam("QuantidadeAjudantes");
            ocorrencia.UtilizarSelecaoPorNotasFiscaisCTe = utilizarSelecaoPorNotasFiscaisCTe;
            ocorrencia.Quilometragem = quilometragem;
            ocorrencia.TiposCausadoresOcorrencia = codigoTipoCausadorOcorrencia > 0 ? repositorioTiposCausadoresOcorrencia.BuscarPorCodigo(codigoTipoCausadorOcorrencia, false) : null;
            ocorrencia.CausasTipoOcorrencia = codigoCausasTipoOcorrencia > 0 ? repositorioTipoOcorrenciaCausas.BuscarPorCodigo(codigoCausasTipoOcorrencia, false) : null;

            ocorrencia.GrupoOcorrencia = codigoGrupoOcorrencia > 0 ? repGrupoOcorrencia.BuscarPorCodigo(codigoGrupoOcorrencia, false) : null;

            if (ocorrencia.TipoOcorrencia != null && ocorrencia.TipoOcorrencia.TomadorTipoOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TomadorTipoOcorrencia.Outros && ocorrencia.Tomador == null && ocorrencia.TipoOcorrencia.OutroTomador != null)
            {
                ocorrencia.Responsavel = Dominio.Enumeradores.TipoTomador.Outros;
                ocorrencia.Tomador = ocorrencia.TipoOcorrencia.OutroTomador;
            }

            ocorrencia.ModeloDocumentoFiscal = this.ObterModeloDocumentoFiscalParaOcorrencia(ocorrencia, unitOfWork);

            if (!string.IsNullOrWhiteSpace(Request.Params("ValorOcorrencia")))
            {
                ocorrencia.ValorOcorrencia = valorOcorrencia;
                ocorrencia.ValorOcorrenciaOriginal = valorOcorrencia;
                if (ocorrencia.ComponenteFrete != null && ocorrencia.ComponenteFrete.SomarComponenteFreteLiquido)
                    ocorrencia.ValorOcorrenciaLiquida = ocorrencia.ValorOcorrencia;
            }

            if (!string.IsNullOrWhiteSpace(observacaoOcorrencia))
                ocorrencia.Observacao = string.Concat(ocorrencia.Observacao, " / ", observacaoOcorrencia);

            if (!string.IsNullOrWhiteSpace(observacaoContrato))
                ocorrencia.Observacao = string.Concat(ocorrencia.Observacao, " / ", observacaoContrato);

            if (ocorrencia.Carga != null && ocorrencia.Carga.Moeda.HasValue && ocorrencia.Carga.Moeda != MoedaCotacaoBancoCentral.Real)
            {
                ocorrencia.Moeda = ocorrencia.Carga.Moeda;
                ocorrencia.ValorCotacaoMoeda = ocorrencia.Carga.ValorCotacaoMoeda;
                ocorrencia.ValorTotalMoeda = Request.GetDecimalParam("ValorTotalMoeda");
                ocorrencia.ValorOcorrencia = (ocorrencia.ValorTotalMoeda ?? 0m) * (ocorrencia.ValorCotacaoMoeda ?? 0m);
            }

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                Repositorio.Embarcador.Avarias.Lote repLote = new Repositorio.Embarcador.Avarias.Lote(unitOfWork);
                Repositorio.Embarcador.Frota.Infracao repInfracao = new Repositorio.Embarcador.Frota.Infracao(unitOfWork);

                int codigoLoteAvaria = Request.GetIntParam("LoteAvaria");
                int codigoInfracao = Request.GetIntParam("Infracao");

                ocorrencia.LoteAvaria = codigoLoteAvaria > 0 ? repLote.BuscarPorCodigo(codigoLoteAvaria) : null;
                ocorrencia.Infracao = codigoInfracao > 0 ? repInfracao.BuscarPorCodigo(codigoInfracao) : null;
            }

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
            {
                if (repCargaOcorrenciaAutorizacao.ExistePorMotivoRejeicaoSemPermissaoETipoOcorrenciaECarga(tipoOcorrencia, carga))
                    throw new ControllerException(Localization.Resources.Ocorrencias.Ocorrencia.NaoEPossivelCriarUmaOcorrenciaComEsseTipoParaCargaPoisJaExisteUmaOcorrenciaRejeitadaComEstesDados);
            }
        }

        private void PreencherConfirmacaoComRequest(ref Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unitOfWork);
            Repositorio.TipoDeOcorrenciaDeCTe repTipoDeOcorrenciaDeCTe = new Repositorio.TipoDeOcorrenciaDeCTe(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.GrupoTipoDeOcorrenciaDeCTe repGrupoOcorrencia = new Repositorio.Embarcador.Ocorrencias.GrupoTipoDeOcorrenciaDeCTe(unitOfWork);

            int.TryParse(Request.Params("ComponenteFrete"), out int componenteFrete);
            int.TryParse(Request.Params("GrupoOcorrencia"), out int codigoGrupoOcorrencia);
            int.TryParse(Request.Params("TipoOcorrencia"), out int tipoOcorrencia);
            string observacaoCTe = Request.Params("ObservacaoCTe") ?? string.Empty;
            string observacao = Request.GetStringParam("Observacao");
            decimal.TryParse(Request.Params("ValorOcorrencia"), out decimal valorOcorrencia);

            string numeroOcorrenciaCliente = Request.Params("NumeroOcorrenciaCliente");

            ocorrencia.Usuario = this.Usuario;
            ocorrencia.ComponenteFrete = repComponenteFrete.BuscarPorCodigo(componenteFrete);
            ocorrencia.TipoOcorrencia = repTipoDeOcorrenciaDeCTe.BuscarPorCodigo(tipoOcorrencia);
            ocorrencia.NumeroOcorrenciaCliente = numeroOcorrenciaCliente;
            ocorrencia.GrupoOcorrencia = repGrupoOcorrencia.BuscarPorCodigo(codigoGrupoOcorrencia, false);
            ocorrencia.OrigemOcorrencia = ocorrencia.TipoOcorrencia?.OrigemOcorrencia ?? OrigemOcorrencia.PorCarga;
            ocorrencia.CTeEmitidoNoEmbarcador = Request.GetBoolParam("CTeEmitidoNoEmbarcador");

            if (valorOcorrencia > 0)
            {
                ocorrencia.ValorOcorrencia = valorOcorrencia;
                ocorrencia.ValorOcorrenciaOriginal = valorOcorrencia;
            }

            if (!string.IsNullOrWhiteSpace(observacaoCTe))
                ocorrencia.ObservacaoCTe = observacaoCTe;

            if (!string.IsNullOrWhiteSpace(observacao))
                ocorrencia.Observacao = observacao;

            if (ocorrencia.ComponenteFrete.SomarComponenteFreteLiquido)
                ocorrencia.ValorOcorrenciaLiquida = ocorrencia.ValorOcorrencia;
            else
                ocorrencia.ValorOcorrenciaLiquida = 0;
        }
        private void SalvarObservacoesFiscoContribuinte(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaObservacaoContribuinte repObservacaoContribuinte = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaObservacaoContribuinte(unitOfWork);

            dynamic observacoesFiscoContribuinte = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ObservacoesFiscoContribuinte"));

            foreach (dynamic observacaoFiscoContribuinte in observacoesFiscoContribuinte)
            {
                Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaObservacaoContribuinte cargaOcorrenciaObservacaoContribuinte = new Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaObservacaoContribuinte()
                {
                    Identificador = (string)observacaoFiscoContribuinte.Identificador,
                    Texto = (string)observacaoFiscoContribuinte.Descricao,
                    Tipo = ((string)observacaoFiscoContribuinte.TipoCodigo).ToEnum<TipoObservacaoCTe>(),
                    Ocorrencia = ocorrencia
                };

                repObservacaoContribuinte.Inserir(cargaOcorrenciaObservacaoContribuinte);
            }
        }

        private void DividirOcorrencia(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrenciaOrigem, ref Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);
            Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.GrupoTipoDeOcorrenciaDeCTe repGrupoOcorrencia = new Repositorio.Embarcador.Ocorrencias.GrupoTipoDeOcorrenciaDeCTe(unitOfWork);

            string observacaoOcorrenciaDestino = Request.Params("ObservacaoOcorrenciaDestino") ?? string.Empty;
            string observacaoCTeDestino = Request.Params("ObservacaoCTeDestino") ?? string.Empty;
            decimal.TryParse(Request.Params("ValorOcorrenciaDestino"), out decimal valorOcorrenciaDestino);
            int.TryParse(Request.Params("ComponenteFrete"), out int componenteFrete);
            int.TryParse(Request.Params("CodigoGrupoOcorrencia"), out int codigoGrupoOcorrencia);

            CopiarOcorrencia(ocorrenciaOrigem, ref ocorrencia);
            ocorrencia.Observacao = Request.Params("Observacao");

            if (!string.IsNullOrWhiteSpace(observacaoOcorrenciaDestino))
                ocorrencia.Observacao = string.Concat(ocorrencia.Observacao, " / ", observacaoOcorrenciaDestino);
            ocorrencia.ObservacaoCTe = observacaoCTeDestino;

            if (valorOcorrenciaDestino > 0)
            {
                ocorrencia.NumeroOcorrencia = Servicos.Embarcador.CargaOcorrencia.OcorrenciaSequencial.GetInstance().ObterProximoNumeroSequencial(unitOfWork);
                ocorrencia.ComponenteFrete = repComponenteFrete.BuscarPorCodigo(componenteFrete);
                ocorrencia.CargaOcorrenciaVinculada = ocorrencia.Codigo;
                ocorrencia.Responsavel = Dominio.Enumeradores.TipoTomador.Destinatario;
                ocorrencia.GrupoOcorrencia = repGrupoOcorrencia.BuscarPorCodigo(codigoGrupoOcorrencia, false);
                ocorrencia.ValorOcorrencia = valorOcorrenciaDestino;
                ocorrencia.ValorOcorrenciaOriginal = valorOcorrenciaDestino;
            }
        }

        private bool ValidarCanhotoCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Canhotos.Canhoto repositorioCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
            List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> canhotos = repositorioCanhoto.BuscarPorCarga(carga.Codigo);

            foreach (Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto in canhotos)
            {
                if (canhoto.SituacaoDigitalizacaoCanhoto == SituacaoDigitalizacaoCanhoto.Digitalizado)
                    return false;
            }

            return true;
        }

        private bool ValidaTomadoresDasCargas(List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasDoPeriodo, Dominio.Enumeradores.TipoTomador? tipoTomador, double cpfCnpjTomador, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.CargaOcorrencia.Ocorrencia srvOcorrencia = new Servicos.Embarcador.CargaOcorrencia.Ocorrencia(unitOfWork);
            for (int i = 0; i < cargasDoPeriodo.Count; i++)
            {
                if (srvOcorrencia.RetornarTomadorEmiteCTeNoEmbarcador(null, cargasDoPeriodo[i].Codigo, tipoTomador, cpfCnpjTomador, unitOfWork))
                    return false;
            }

            return true;
        }

        private bool CopiarAnexosChamado(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia, List<int> codigosChamados, Repositorio.UnitOfWork unitOfWork)
        {
            if (codigosChamados.Count == 0)
                return false;

            Repositorio.Embarcador.Chamados.ChamadoAnexo repChamadoAnexo = new Repositorio.Embarcador.Chamados.ChamadoAnexo(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAnexos repCargaOcorrenciaAnexos = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAnexos(unitOfWork);

            bool copiouAnexo = false;
            foreach (int codigoChamado in codigosChamados)
            {
                List<Dominio.Entidades.Embarcador.Chamados.ChamadoAnexo> listaAnexosChamado = repChamadoAnexo.BuscarPorChamado(codigoChamado);

                for (int i = 0; i < listaAnexosChamado.Count(); i++)
                {
                    Dominio.Entidades.Embarcador.Chamados.ChamadoAnexo chamadoAnexo = listaAnexosChamado[i];
                    string nomeArquivo = chamadoAnexo.NomeArquivo;
                    string extensaoArquivo = System.IO.Path.GetExtension(nomeArquivo).ToLower();
                    string guidArquivo = chamadoAnexo.GuidArquivo;

                    string arquivo = Utilidades.IO.FileStorageService.Storage.Combine(Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "Chamados" }), guidArquivo + extensaoArquivo);
                    if (Utilidades.IO.FileStorageService.Storage.Exists(arquivo))
                    {
                        string arquivoOcorrencia = Utilidades.IO.FileStorageService.Storage.Combine(Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "Ocorrencia" }), guidArquivo + extensaoArquivo);

                        if (!Utilidades.IO.FileStorageService.Storage.Exists(arquivoOcorrencia))
                            Utilidades.IO.FileStorageService.Storage.Copy(arquivo, arquivoOcorrencia);

                        if (Utilidades.IO.FileStorageService.Storage.Exists(arquivoOcorrencia))
                        {
                            Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAnexos cargaOcorrenciaAnexos = new Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAnexos();

                            cargaOcorrenciaAnexos.CargaOcorrencia = ocorrencia;
                            cargaOcorrenciaAnexos.Descricao = $"Anexo {i + 1} do atendimento {chamadoAnexo.Chamado.Numero}";
                            cargaOcorrenciaAnexos.GuidArquivo = guidArquivo;
                            cargaOcorrenciaAnexos.NomeArquivo = nomeArquivo;

                            repCargaOcorrenciaAnexos.Inserir(cargaOcorrenciaAnexos);
                            copiouAnexo = true;
                        }
                    }
                }
            }

            return copiouAnexo;
        }

        private List<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> BuscarDocumentosParaEmissaoNFSManualSelecionados(Dominio.Entidades.Embarcador.Cargas.Carga carga, int numeroNF, double cpfCnpjDestinatario, int numeroDocumento, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual repCargaDocumentoParaEmissaoNFSManual = new Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual(unitOfWork);

            dynamic codigosDocs = Request.GetListParam<dynamic>("CargaDocumentosParaEmissaoNFSManual");
            bool selecionarTodos = bool.Parse(Request.Params("SelecionarTodosDocumentoParaEmissaoNFSManual"));

            // Retornar aqui a lista de docs relativos a esses parametros

            List<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> documentoParaEmissaoNFSManuals = new List<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual>();

            List<dynamic> listaSelecionados = Newtonsoft.Json.JsonConvert.DeserializeObject<List<dynamic>>(Request.GetStringParam("CargaDocumentosParaEmissaoNFSManual"));
            List<int> idsSelecionados = (from o in listaSelecionados select (int)o.Codigo).ToList();

            if (selecionarTodos)
            {
                // Mesmo filtro que em CargaNFSController@ConsultarCargaDocumentoParaEmissaoNFSManual
                documentoParaEmissaoNFSManuals = repCargaDocumentoParaEmissaoNFSManual.ConsultarDocumentoParaEmissaoNFSManual(carga.Codigo, numeroDocumento, cpfCnpjDestinatario, true, ConfiguracaoEmbarcador.GerarOcorrenciaParaCargaAgrupada, "Codigo", "asc", 0, 1000);

                // Remove os que estavam selecionados, pois é o contrário quando selecionarTodos está marcado
                documentoParaEmissaoNFSManuals = (from o in documentoParaEmissaoNFSManuals where !idsSelecionados.Contains(o.Codigo) select o).ToList();
            }
            else
            {

                documentoParaEmissaoNFSManuals = repCargaDocumentoParaEmissaoNFSManual.BuscarPorCodigos(idsSelecionados);
            }


            return documentoParaEmissaoNFSManuals;
        }

        private byte[] ObterPDFDocumentosOcorrenciaLoteCompactado(List<int> listaCodigosOcorrenciasGeracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repositorioCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);

            Dictionary<string, byte[]> conteudoCompactar = new Dictionary<string, byte[]>();

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ConfiguracaoEmbarcador;

            foreach (int codigo in listaCodigosOcorrenciasGeracao)
            {
                Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia = repositorioCargaOcorrencia.BuscarPorCodigo(codigo);

                (byte[] arquivo, string nomeArquivo) = MontarPDFDocumentosOcorrencia(ocorrencia, configuracaoEmbarcador, unitOfWork);

                if (arquivo != null)
                    conteudoCompactar.Add(nomeArquivo, arquivo);
            }

            System.IO.MemoryStream arquivoCompactado = Utilidades.File.GerarArquivoCompactado(conteudoCompactar);
            byte[] arquivoCompactadoBinario = arquivoCompactado.ToArray();

            return arquivoCompactadoBinario;
        }

        private (byte[] arquivo, string nomeArquivo) MontarPDFDocumentosOcorrencia(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaDocumento repCargaOcorrenciaDocumento = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaDocumento(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repositorioCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAnexos repCargaOcorrenciaAnexos = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAnexos(unitOfWork);

            List<byte[]> arquivos = new List<byte[]>();

            string caminhoRelatorios = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoRelatorios;
            string caminhoAnexo = Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "Ocorrencia" });
            if (string.IsNullOrWhiteSpace(caminhoRelatorios))
                throw new ControllerException(Localization.Resources.Ocorrencias.Ocorrencia.OCaminhoParaODownloadNaoEstaDisponivelContateOSuporteTecnico);

            List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> documentosOrigem = repCargaOcorrenciaDocumento.BuscarCTesENFSesPorOcorrencia(ocorrencia.Codigo);
            List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> documentosGerados = repositorioCargaCTeComplementoInfo.BuscarCTesPorOcorrenciaSemFilialEmissora(ocorrencia.Codigo);

            string nomeArquivo = documentosOrigem.FirstOrDefault()?.TomadorPagador?.CPF_CNPJ_SemFormato ?? string.Empty;
            nomeArquivo += " DOC " + documentosGerados.FirstOrDefault()?.Numero.ToString() ?? string.Empty;
            nomeArquivo += " OC " + ocorrencia.NumeroOcorrencia.ToString();

            foreach (Dominio.Entidades.ConhecimentoDeTransporteEletronico cte in documentosGerados)
            {
                byte[] pdf = ObterDACTE(cte, caminhoRelatorios, configuracaoTMS, unitOfWork);

                if (pdf != null)
                    arquivos.Add(pdf);
            }

            List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAnexos> anexos = repCargaOcorrenciaAnexos.BuscarPorCodigoOcorrencia(ocorrencia.Codigo);
            foreach (Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAnexos anexo in anexos)
            {
                string extencao = "." + anexo.ExtensaoArquivo;
                string arquivoPDF = Utilidades.IO.FileStorageService.Storage.Combine(caminhoAnexo, anexo.GuidArquivo + extencao);

                if (!Utilidades.IO.FileStorageService.Storage.Exists(arquivoPDF))
                    continue;

                byte[] conversaoImagem = Utilidades.Image.ImageToPdf(arquivoPDF, anexo.ExtensaoArquivo);
                if (conversaoImagem != null)
                {
                    arquivos.Add(conversaoImagem);
                    continue;
                }

                byte[] pdf = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(arquivoPDF);
                if (pdf != null)
                    arquivos.Add(pdf);
            }

            foreach (Dominio.Entidades.ConhecimentoDeTransporteEletronico cte in documentosOrigem)
            {
                byte[] pdf = ObterDACTE(cte, caminhoRelatorios, configuracaoTMS, unitOfWork);

                if (pdf != null)
                    arquivos.Add(pdf);
            }

            if (arquivos.Count == 0)
                throw new ControllerException(string.Format(Localization.Resources.Ocorrencias.Ocorrencia.NenhumArquivoFoiEncontradoParaGerarOPdfDaOcorrencia, ocorrencia.NumeroOcorrencia));

            byte[] arquivo = Utilidades.File.MergeFiles(arquivos);

            nomeArquivo += " PAG " + Utilidades.File.ContarPaginasArquivoPDF(arquivo);

            return ValueTuple.Create(arquivo, nomeArquivo + ".pdf");
        }

        private byte[] ObterDACTE(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, string caminhoRelatorios, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.DACTE svcDACTE = new Servicos.DACTE(unitOfWork);
            Servicos.NFSe svcNFSe = new Servicos.NFSe(unitOfWork);

            if (cte.Status != "A" && cte.Status != "C" && cte.Status != "K" && cte.Status != "Z" && cte.Status != "F")
                return null;

            if (cte.ModeloDocumentoFiscal.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.CTe && cte.ModeloDocumentoFiscal.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.NFSe &&
                cte.ModeloDocumentoFiscal.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.NFS)
                return new Servicos.Embarcador.Relatorios.OutrosDocumentos(unitOfWork).ObterPdf(cte);

            string nomeArquivoFisico = cte.Chave;
            if (cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe || cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFS)
                nomeArquivoFisico = cte.Numero.ToString() + "_" + cte.Serie.Numero.ToString();
            if (configuracaoTMS.GerarPDFCTeCancelado && cte.Status == "C" && cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe)
                nomeArquivoFisico = nomeArquivoFisico + "_Canc";
            if (cte.Status == "F")
                nomeArquivoFisico = nomeArquivoFisico + "_FSDA";

            string caminhoPDF = Utilidades.IO.FileStorageService.Storage.Combine(caminhoRelatorios, cte.Empresa.CNPJ, nomeArquivoFisico) + ".pdf";

            if (cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe || cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFS)
            {
                if (cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFS)
                    return svcNFSe.ObterDANFSECTe(cte.Codigo, null, true);
                else
                    return svcNFSe.ObterDANFSECTe(cte.Codigo);
            }
            else
            {
                if (!Utilidades.IO.FileStorageService.Storage.Exists(caminhoPDF))
                    return svcDACTE.GerarPorProcesso(cte.Codigo, null, configuracaoTMS.GerarPDFCTeCancelado);
                else
                    return Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoPDF);
            }
        }

        private void SalvarNotasParaComplementoCTEGlobalizado(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia, List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes, string strXMLNotasCTeGlobalizado, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaOcorrenciaCTeComplementarGlobalizadoNotaFiscal repCargaOcorrenciaCTeComplementarGlobalizadoNotaFiscal = new Repositorio.Embarcador.Cargas.CargaOcorrenciaCTeComplementarGlobalizadoNotaFiscal(unitOfWork);

            List<dynamic> listaXMLNotaFiscalSelecionado = Newtonsoft.Json.JsonConvert.DeserializeObject<List<dynamic>>(strXMLNotasCTeGlobalizado);

            if (listaXMLNotaFiscalSelecionado == null || listaXMLNotaFiscalSelecionado.Count <= 0)
                return;

            List<int> codigosCargaCTE = (from obj in listaXMLNotaFiscalSelecionado select (int)obj.CodigoCargaCTe).Distinct().ToList();

            foreach (int codigoCargaCTe in codigosCargaCTE)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = (from obj in cargaCTes where obj.Codigo == codigoCargaCTe select obj).FirstOrDefault();

                List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> xMLNotasFiscaisCTeSelecionado = repXMLNotaFiscal.BuscarPorCodigoCTe(cargaCTe.CTe.Codigo);

                List<int> nfsMarcadas = (from obj in listaXMLNotaFiscalSelecionado where obj.CodigoCargaCTe == codigoCargaCTe select (int)obj.Codigo).ToList();
                bool todosMarcados = (from obj in listaXMLNotaFiscalSelecionado where obj.CodigoCargaCTe == codigoCargaCTe select obj.MarcarTodos).FirstOrDefault();

                if (todosMarcados)
                    xMLNotasFiscaisCTeSelecionado = xMLNotasFiscaisCTeSelecionado.Where(o => !nfsMarcadas.Contains(o.Codigo)).ToList();
                else
                    xMLNotasFiscaisCTeSelecionado = xMLNotasFiscaisCTeSelecionado.Where(o => nfsMarcadas.Contains(o.Codigo)).ToList();

                foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xMLNotaFiscalCTe in xMLNotasFiscaisCTeSelecionado)
                {
                    if (repCargaOcorrenciaCTeComplementarGlobalizadoNotaFiscal.ExistePorCargaOcorrenciaCargaCTeXMLNotaFiscal(ocorrencia.Codigo, cargaCTe.Codigo, xMLNotaFiscalCTe.Codigo))
                        continue;

                    Dominio.Entidades.Embarcador.Cargas.CargaOcorrenciaCTeComplementarGlobalizadoNotaFiscal cargaOcorrenciaCTeComplementarGlobalizadoNotaFiscal = new Dominio.Entidades.Embarcador.Cargas.CargaOcorrenciaCTeComplementarGlobalizadoNotaFiscal()
                    {
                        CargaOcorrencia = ocorrencia,
                        XMLNotaFiscal = xMLNotaFiscalCTe,
                        CargaCTe = cargaCTe
                    };

                    repCargaOcorrenciaCTeComplementarGlobalizadoNotaFiscal.Inserir(cargaOcorrenciaCTeComplementarGlobalizadoNotaFiscal);
                }
            }
        }

        private async Task<decimal> BuscarValorAReceber(int codigoOcorrencia, int codigoCarga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaDocumento repCargaOcorrenciaDocumento = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaDocumento(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repositorioCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

            decimal valorAReceber = 0m;

            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTesPorOcorrencia = repCargaOcorrenciaDocumento.BuscarCTesPorOcorrencia(codigoOcorrencia, null, null, 0, 0);
            if (cargaCTesPorOcorrencia != null && cargaCTesPorOcorrencia.Count > 0)
            {
                foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe carga in cargaCTesPorOcorrencia)
                {
                    valorAReceber += carga.CTe?.ValorAReceber ?? carga.PreCTe.ValorAReceber;
                }

                return valorAReceber;
            }

            // Listando todos os tipos que não são NFS manual
            List<Dominio.Enumeradores.TipoDocumento> tiposDeDocumentosCte = new List<Dominio.Enumeradores.TipoDocumento>
            {
                Dominio.Enumeradores.TipoDocumento.CTe,
                Dominio.Enumeradores.TipoDocumento.NFSe,
                Dominio.Enumeradores.TipoDocumento.Outros,
                Dominio.Enumeradores.TipoDocumento.Subcontratacao
            };

            Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaMontarConsultaCtes filtro = new()
            {
                Carga = codigoCarga,
                NumeroDocumento = 0,
                NumeroNF = 0,
                StatusCTe = new[] { "A", "Z" },
                ApenasCTesNormais = true,
                CtesSubContratacaoFilialEmissora = true,
                CtesSemSubContratacaoFilialEmissora = false,
                EmpresasFilialEmissora = new List<int>(),
                ProprietarioVeiculo = "",
                Destinatario = 0,
                BuscarPorCargaOrigem = true,
                RetornarPreCtes = false,
                TiposDocumentosDoCte = tiposDeDocumentosCte
            };

            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTesPorCarga = await repositorioCargaCTe.ConsultarCTes(filtro,
                                                                                                                     "CTe.Codigo",
                                                                                                                     "desc",
                                                                                                                     0,
                                                                                                                     25);

            if (cargaCTesPorCarga != null && cargaCTesPorCarga.Count > 0)
            {
                foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe carga in cargaCTesPorCarga)
                {
                    valorAReceber += carga.CTe?.ValorAReceber ?? carga.PreCTe.ValorAReceber;
                }

                return valorAReceber;
            }

            return valorAReceber;
        }

        private void SalvarListaProdutosCargaCTe(List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTEs, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Produtos.ProdutoEmbarcador repProduto = new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeComplementoInfoProduto repCargaCteComplementoInfoProduto = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfoProduto(unitOfWork);
            Repositorio.Embarcador.Cargas.CTeProduto repCTeProduto = new Repositorio.Embarcador.Cargas.CTeProduto(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCtE = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);

            dynamic dynRegistrosPontosEmbarque = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ListaOcorrenciaNFeProduto"));

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe in cargaCTEs)
            {
                List<Dominio.Entidades.Embarcador.Cargas.CTeProduto> listaProdutos = new List<Dominio.Entidades.Embarcador.Cargas.CTeProduto>();

                foreach (dynamic produto in dynRegistrosPontosEmbarque)
                {
                    int quantidadeDevolucao = 0;
                    Int32.TryParse((string)produto.QuantidadeDevolucao, out quantidadeDevolucao);

                    if (quantidadeDevolucao > 0 && cargaCTe.CTe.Codigo == (int)produto.CodigoCTe)
                    {
                        if (cargaCTe.CargaCTeComplementoInfoProduto == null)
                        {
                            Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfoProduto cargaCteComplementoInfoProduto = new Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfoProduto();
                            cargaCteComplementoInfoProduto.CargaCTeComplementado = cargaCTe;
                            repCargaCteComplementoInfoProduto.Inserir(cargaCteComplementoInfoProduto);
                            cargaCTe.CargaCTeComplementoInfoProduto = cargaCteComplementoInfoProduto;
                        }

                        Dominio.Entidades.Embarcador.Cargas.CTeProduto cteProduto = new Dominio.Entidades.Embarcador.Cargas.CTeProduto();
                        cteProduto.Produto = repProduto.buscarPorCodigoEmbarcador((string)produto.CodigoProdutoEmbarcador);
                        cteProduto.XMLNotaFiscal = repXMLNotaFiscal.BuscarPorCodigo((int)produto.CodigoXMLNotaFiscal);
                        cteProduto.Quantidade = (int)produto.QuantidadeDevolucao;

                        repCTeProduto.Inserir(cteProduto);
                        listaProdutos.Add(cteProduto);
                    }
                }

                if (cargaCTe.CargaCTeComplementoInfoProduto != null)
                {
                    if (cargaCTe.CargaCTeComplementoInfoProduto?.CTeProdutos != null)
                        cargaCTe.CargaCTeComplementoInfoProduto.CTeProdutos.Clear();
                    else
                        cargaCTe.CargaCTeComplementoInfoProduto.CTeProdutos = new List<Dominio.Entidades.Embarcador.Cargas.CTeProduto>();

                    foreach (Dominio.Entidades.Embarcador.Cargas.CTeProduto produto in listaProdutos)
                        cargaCTe.CargaCTeComplementoInfoProduto.CTeProdutos.Add(produto);

                    repCargaCteComplementoInfoProduto.Atualizar(cargaCTe.CargaCTeComplementoInfoProduto);
                    repCargaCtE.Atualizar(cargaCTe);
                }
            }
        }

        private bool PedagioPagoNaCarga(int codigoCarga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repCargaIntegracaoValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(unitOfWork);

            return repCargaIntegracaoValePedagio.VerificarSeExisteValePedagioPorStatus(codigoCarga, SituacaoValePedagio.Comprada);
        }

        private List<double> ObterCPFCNPJDestinatarios(int codigoCargaOcorrencia, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos)
        {
            List<double> cpfCnpjDestinatarios = (from o in cargaPedidos where o.Carga.Codigo == codigoCargaOcorrencia select o.Pedido.Destinatario.CPF_CNPJ).Distinct().ToList();
            return cpfCnpjDestinatarios;
        }

        private List<dynamic> ObterConsultaCargaCTeOcorrenciaPorNota(List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargasCTe, List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe> cargaPedidosXMLsNotaFiscalCTe, List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> cargaEntregaPedidos, ref int totalRegistros)
        {
            List<dynamic> retorno = new List<dynamic>();
            totalRegistros = 0;

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe in cargasCTe)
            {
                if (cargaCTe.CTe == null && cargaCTe.PreCTe == null)
                    continue;

                List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidosXMLNotaFiscalPorCargaCTe = (from o in cargaPedidosXMLsNotaFiscalCTe where o.CargaCTe.Codigo == cargaCTe.Codigo select o.PedidoXMLNotaFiscal).ToList();

                foreach (var pedidoXMLNotaFiscalPorCargaCTe in pedidosXMLNotaFiscalPorCargaCTe)
                {
                    totalRegistros++;

                    Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido cargaEntregaPedidoPorCargaCTe = (from o in cargaEntregaPedidos where !o.CargaEntrega.Coleta && o.CargaPedido.Codigo == pedidoXMLNotaFiscalPorCargaCTe.CargaPedido.Codigo select o).FirstOrDefault();
                    string dataEntrada = "";
                    string dataEntradaRaio = "";
                    string dataSaidaRaio = "";

                    if (cargaEntregaPedidoPorCargaCTe != null)
                    {
                        Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = cargaEntregaPedidoPorCargaCTe.CargaEntrega;

                        if (cargaEntrega.DataEntradaRaio.HasValue && cargaEntrega.DataSaidaRaio.HasValue)
                        {
                            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = cargaEntregaPedidoPorCargaCTe?.CargaPedido?.Pedido;

                            dataEntrada = (pedido.PrevisaoEntrega.HasValue && (pedido.PrevisaoEntrega > cargaEntrega.DataEntradaRaio)) ? pedido.PrevisaoEntrega.Value.ToString("dd/MM/yyyy HH:mm") : cargaEntrega.DataEntradaRaio.Value.ToString("dd/MM/yyyy HH:mm");
                            dataEntradaRaio = cargaEntrega.DataEntradaRaio.Value.ToString("dd/MM/yyyy HH:mm");
                            dataSaidaRaio = cargaEntrega.DataSaidaRaio.Value.ToString("dd/MM/yyyy HH:mm");
                        }
                    }

                    retorno.Add(new
                    {
                        Codigo = pedidoXMLNotaFiscalPorCargaCTe.XMLNotaFiscal.Codigo,
                        CodigoCargaCTe = cargaCTe.Codigo,
                        CodigoCTE = cargaCTe.CTe?.Codigo ?? 0,
                        DescricaoTipoServico = cargaCTe.CTe?.DescricaoTipoServico ?? cargaCTe.PreCTe.DescricaoTipoServico,
                        NumeroModeloDocumentoFiscal = cargaCTe.CTe?.ModeloDocumentoFiscal.Numero ?? "",
                        AbreviacaoModeloDocumentoFiscal = cargaCTe.CTe?.ModeloDocumentoFiscal.Abreviacao ?? "Pré CT-e",
                        CodigoEmpresa = cargaCTe.CTe?.Empresa.Codigo ?? cargaCTe.PreCTe.Empresa.Codigo,
                        Numero = cargaCTe.CTe?.Numero.ToString() ?? "",
                        DataEmissao = cargaCTe.CTe?.DataEmissao ?? cargaCTe.PreCTe.DataEmissao,
                        SituacaoCTe = cargaCTe.CTe?.Status ?? "",
                        Serie = cargaCTe.CTe?.Serie?.Numero.ToString() ?? "",
                        DescricaoTipoPagamento = cargaCTe.CTe?.DescricaoTipoPagamento ?? "",
                        Remetente = cargaCTe.CTe != null ? (cargaCTe.CTe.Remetente?.Cliente?.Descricao ?? cargaCTe.CTe.Remetente?.Descricao ?? string.Empty) : cargaCTe.PreCTe.Remetente.Cliente.Descricao,
                        Destinatario = cargaCTe.CTe != null ? (cargaCTe.CTe.Destinatario?.Cliente?.Descricao ?? cargaCTe.CTe.Destinatario?.Descricao ?? string.Empty) : cargaCTe.PreCTe.Destinatario.Cliente.Descricao,
                        Destino = cargaCTe.CTe?.LocalidadeTerminoPrestacao?.DescricaoCidadeEstado ?? cargaCTe.PreCTe?.LocalidadeTerminoPrestacao.DescricaoCidadeEstado ?? "",
                        ValorFrete = cargaCTe.CTe?.ValorAReceber.ToString("n2") ?? cargaCTe.PreCTe.ValorAReceber.ToString("n2"),
                        Aliquota = cargaCTe.CTe != null ? (cargaCTe.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe ? cargaCTe.CTe.AliquotaICMS.ToString("n2") : cargaCTe.CTe.AliquotaISS.ToString("n4")) : cargaCTe.PreCTe.AliquotaICMS.ToString("n2"),
                        Status = cargaCTe.CTe?.DescricaoStatus ?? "",
                        NumeroNotas = pedidoXMLNotaFiscalPorCargaCTe.XMLNotaFiscal.Numero,
                        RetornoSefaz = cargaCTe.CTe != null ? (!string.IsNullOrWhiteSpace(cargaCTe.CTe.MensagemRetornoSefaz) ? cargaCTe.CTe.MensagemRetornoSefaz != " - " ? System.Web.HttpUtility.HtmlEncode(cargaCTe.CTe.MensagemRetornoSefaz) : "" : "") : "",
                        DataEntrada = dataEntrada,
                        DataEntradaRaio = dataEntradaRaio,
                        DataSaidaRaio = dataSaidaRaio,
                        CTeGlobalizado = cargaCTe.CTe != null ? cargaCTe.CTe.IndicadorGlobalizado == Dominio.Enumeradores.OpcaoSimNao.Sim : false,
                        DT_RowColor = cargaCTe.CTe != null ? ((cargaCTe.CTe.Status == "R" ? "rgba(193, 101, 101, 1)" : ((cargaCTe.CTe.Status == "C" || cargaCTe.CTe.Status == "I" || cargaCTe.CTe.Status == "D" || cargaCTe.CTe.Status == "Z") ? "#777" : ""))) : "",
                        DT_FontColor = cargaCTe.CTe != null ? (((cargaCTe.CTe.Status == "R" || cargaCTe.CTe.Status == "C" || cargaCTe.CTe.Status == "I" || cargaCTe.CTe.Status == "D" || cargaCTe.CTe.Status == "Z") ? "#FFFFFF" : "")) : "",
                        CodigoNotaFiscal = pedidoXMLNotaFiscalPorCargaCTe.XMLNotaFiscal.Codigo
                    });
                }
            }

            return retorno;
        }

        private dynamic ObterCargaCTeOcorrencia(Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe, List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe> cargaPedidosXMLsNotaFiscalCTe, List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> cargaEntregaPedidos)
        {
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidosXMLNotaFiscalPorCargaCTe = (from o in cargaPedidosXMLsNotaFiscalCTe where o.CargaCTe.Codigo == cargaCTe.Codigo select o.PedidoXMLNotaFiscal).ToList();
            List<int> codigosCargaPedidos = (from o in pedidosXMLNotaFiscalPorCargaCTe select o.CargaPedido.Codigo).Distinct().ToList();
            Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido cargaEntregaPedidoPorCargaCTe = (from o in cargaEntregaPedidos where !o.CargaEntrega.Coleta && codigosCargaPedidos.Contains(o.CargaPedido.Codigo) select o).FirstOrDefault();
            string dataEntrada = "";
            string dataEntradaRaio = "";
            string dataSaidaRaio = "";

            if (cargaEntregaPedidoPorCargaCTe != null)
            {
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = cargaEntregaPedidoPorCargaCTe.CargaEntrega;

                if (cargaEntrega.DataEntradaRaio.HasValue && cargaEntrega.DataSaidaRaio.HasValue)
                {
                    Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = cargaEntregaPedidoPorCargaCTe?.CargaPedido?.Pedido;

                    dataEntrada = (pedido.PrevisaoEntrega.HasValue && (pedido.PrevisaoEntrega > cargaEntrega.DataEntradaRaio)) ? pedido.PrevisaoEntrega.Value.ToString("dd/MM/yyyy HH:mm") : cargaEntrega.DataEntradaRaio.Value.ToString("dd/MM/yyyy HH:mm");
                    dataEntradaRaio = cargaEntrega.DataEntradaRaio.Value.ToString("dd/MM/yyyy HH:mm");
                    dataSaidaRaio = cargaEntrega.DataSaidaRaio.Value.ToString("dd/MM/yyyy HH:mm");
                }
            }

            return new
            {
                cargaCTe.Codigo,
                CodigoCargaCTe = cargaCTe.Codigo,
                CodigoCTE = cargaCTe.CTe?.Codigo ?? 0,
                DescricaoTipoServico = cargaCTe.CTe?.DescricaoTipoServico ?? cargaCTe.PreCTe.DescricaoTipoServico,
                NumeroModeloDocumentoFiscal = cargaCTe.CTe?.ModeloDocumentoFiscal.Numero ?? "",
                AbreviacaoModeloDocumentoFiscal = cargaCTe.CTe?.ModeloDocumentoFiscal.Abreviacao ?? "Pré CT-e",
                CodigoEmpresa = cargaCTe.CTe?.Empresa.Codigo ?? cargaCTe.PreCTe.Empresa.Codigo,
                Numero = cargaCTe.CTe?.Numero.ToString() ?? "",
                DataEmissao = cargaCTe.CTe?.DataEmissao ?? cargaCTe.PreCTe.DataEmissao,
                SituacaoCTe = cargaCTe.CTe?.Status ?? "",
                Serie = cargaCTe.CTe?.Serie.Numero.ToString() ?? "",
                DescricaoTipoPagamento = cargaCTe.CTe?.DescricaoTipoPagamento ?? "",
                Remetente = cargaCTe.CTe != null ? (cargaCTe.CTe.Remetente?.Cliente?.Descricao ?? cargaCTe.CTe.Remetente?.Descricao ?? string.Empty) : cargaCTe.PreCTe.Remetente.Cliente.Descricao,
                Destinatario = cargaCTe.CTe != null ? (cargaCTe.CTe.Destinatario?.Cliente?.Descricao ?? cargaCTe.CTe.Destinatario?.Descricao ?? string.Empty) : cargaCTe.PreCTe.Destinatario.Cliente.Descricao,
                Destino = cargaCTe.CTe?.LocalidadeTerminoPrestacao.DescricaoCidadeEstado ?? cargaCTe.PreCTe.LocalidadeTerminoPrestacao.DescricaoCidadeEstado,
                ValorFrete = cargaCTe.CTe?.ValorAReceber.ToString("n2") ?? cargaCTe.PreCTe.ValorAReceber.ToString("n2"),
                Aliquota = cargaCTe.CTe != null ? (cargaCTe.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe ? cargaCTe.CTe.AliquotaICMS.ToString("n2") : cargaCTe.CTe.AliquotaISS.ToString("n4")) : cargaCTe.PreCTe.AliquotaICMS.ToString("n2"),
                Status = cargaCTe.CTe?.DescricaoStatus ?? "",
                NumeroNotas = string.Join(", ", (from o in pedidosXMLNotaFiscalPorCargaCTe select o.XMLNotaFiscal.Numero).Distinct().ToList()),
                RetornoSefaz = cargaCTe.CTe != null ? (!string.IsNullOrWhiteSpace(cargaCTe.CTe.MensagemRetornoSefaz) ? cargaCTe.CTe.MensagemRetornoSefaz != " - " ? System.Web.HttpUtility.HtmlEncode(cargaCTe.CTe.MensagemRetornoSefaz) : "" : "") : "",
                DataEntrada = dataEntrada,
                DataEntradaRaio = dataEntradaRaio,
                DataSaidaRaio = dataSaidaRaio,
                CTeGlobalizado = cargaCTe.CTe != null ? cargaCTe.CTe.IndicadorGlobalizado == Dominio.Enumeradores.OpcaoSimNao.Sim : false,
                DT_RowColor = cargaCTe.CTe != null ? ((cargaCTe.CTe.Status == "R" ? "rgba(193, 101, 101, 1)" : ((cargaCTe.CTe.Status == "C" || cargaCTe.CTe.Status == "I" || cargaCTe.CTe.Status == "D" || cargaCTe.CTe.Status == "Z") ? "#777" : ""))) : "",
                DT_FontColor = cargaCTe.CTe != null ? (((cargaCTe.CTe.Status == "R" || cargaCTe.CTe.Status == "C" || cargaCTe.CTe.Status == "I" || cargaCTe.CTe.Status == "D" || cargaCTe.CTe.Status == "Z") ? "#FFFFFF" : "")) : "",
                CodigoNotaFiscal = 0
            };
        }

        #endregion

        #region Importação

        public async Task<IActionResult> ConfiguracaoImportacao()
        {
            List<ConfiguracaoImportacao> configuracoes = ConfiguracaoImportacaoOcorrencia();

            return new JsonpResult(configuracoes.ToList());
        }

        private List<ConfiguracaoImportacao> ConfiguracaoImportacaoOcorrencia()
        {
            List<ConfiguracaoImportacao> configuracoes = new List<ConfiguracaoImportacao>();

            configuracoes.Add(new ConfiguracaoImportacao() { Id = 1, Descricao = Localization.Resources.Ocorrencias.Ocorrencia.Filial, Propriedade = "Filial", Tamanho = 200 });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 2, Descricao = Localization.Resources.Ocorrencias.Ocorrencia.NumeroNota, Propriedade = "NumeroNota", Tamanho = 100, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 3, Descricao = Localization.Resources.Ocorrencias.Ocorrencia.NumeroCarga, Propriedade = "NumeroCarga", Tamanho = 100, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 4, Descricao = Localization.Resources.Ocorrencias.Ocorrencia.DescricaoOcorrencia, Propriedade = "Ocorrencia", Tamanho = 100, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 5, Descricao = Localization.Resources.Ocorrencias.Ocorrencia.Observacao, Propriedade = "Observacao", Tamanho = 500 });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 6, Descricao = Localization.Resources.Ocorrencias.Ocorrencia.ObservacaoCTe, Propriedade = "ObservacaoCTe", Tamanho = 500 });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 7, Descricao = Localization.Resources.Ocorrencias.Ocorrencia.Valor, Propriedade = "Valor", Tamanho = 100 });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 8, Descricao = Localization.Resources.Ocorrencias.Ocorrencia.ChaveDoCte, Propriedade = "ChaveCTe", Tamanho = 100 });

            return configuracoes;
        }

        public async Task<IActionResult> Importar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Servicos.Embarcador.CargaOcorrencia.Ocorrencia serOcorrencia = new Servicos.Embarcador.CargaOcorrencia.Ocorrencia(unitOfWork);
                string dados = Request.Params("Dados");
                RetornoImportacao retornoImportacao = serOcorrencia.ImportarOcorrencia(dados, this.Usuario, TipoServicoMultisoftware, Auditado, this.Cliente, unitOfWork);

                return new JsonpResult(retornoImportacao);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuUmaFalhaAoImportarArquivo);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ImportarNOTFIS()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Servicos.Embarcador.CargaOcorrencia.Ocorrencia serOcorrencia = new Servicos.Embarcador.CargaOcorrencia.Ocorrencia(unitOfWork);
                Repositorio.LayoutEDI repLayoutEDI = new Repositorio.LayoutEDI(unitOfWork);
                Repositorio.TipoDeOcorrenciaDeCTe repTipoOcorrencia = new Repositorio.TipoDeOcorrenciaDeCTe(unitOfWork);


                int.TryParse(Request.Params("LayoutEDI"), out int codigolayoutEDI);
                int.TryParse(Request.Params("TipoOcorrenciaImportacaoNOTFIS"), out int codigoTipoOcorrencia);

                string erro = "";
                IList<Servicos.DTO.CustomFile> arquivos = HttpContext.GetFiles("ArquivoNOTFIS");

                if (arquivos.Count <= 0)
                    return new JsonpResult(false, true, Localization.Resources.Ocorrencias.Ocorrencia.NenhumArquivoSelecionadoParaEnvio);


                Dominio.Entidades.LayoutEDI layoutEDI = repLayoutEDI.BuscarPorCodigo(codigolayoutEDI);
                Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoOcorrencia = repTipoOcorrencia.BuscarPorCodigo(codigoTipoOcorrencia);


                bool retorno = serOcorrencia.ImportarCargaOcorrenciaNOTFIS(unitOfWork, layoutEDI, arquivos[0].InputStream, Usuario?.Empresa, tipoOcorrencia, ref erro);

                if (retorno)
                    return new JsonpResult(false, true, erro);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuUmaFalhaAoImportarArquivo);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
        #endregion
    }
}
