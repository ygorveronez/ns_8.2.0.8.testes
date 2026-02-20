using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Servicos.Embarcador.EmissorDocumento;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.WebAdmin.ProcessamentoDocumentoTransporte
{
    public partial class ProcessarThreads : ServiceBase
    {
        private List<int> _ArquivosProcessamentoDocumentoTransporte;
        private List<Task> _tasksGeracaoProcessamentoDocumentoTransporte;
        private static string _arquivoLog = "Logs";
        private List<ConfiguracaoThread> _configuracoes;
        private CancellationTokenSource _cts;

        public ProcessarThreads()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Servicos.Log.TratarErro("Serviço ProcessamentoDocumentoTransporte iniciado.");
            _cts = new CancellationTokenSource();

            try
            {
                string defaultCulture = Program.appSettings["AppSettings:DefaultCulture"];

                if (string.IsNullOrWhiteSpace(defaultCulture))
                    defaultCulture = "pt-BR";

                System.Globalization.CultureInfo cultureInfo = new System.Globalization.CultureInfo(defaultCulture);

                System.Threading.Thread.CurrentThread.CurrentCulture = cultureInfo;
                System.Threading.Thread.CurrentThread.CurrentUICulture = cultureInfo;

                System.Globalization.CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
                System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

                Servicos.IO.FileStorage.ConfigureApplicationFileStorage(Program.StringConexaoAdmin, Program.Host);
                EmissorDocumentoService.ConfigureApplicationEmissorDocumento(Program.StringConexao);
                ConfigurarThreads();

                Task.Run(() => ThreadProcessamentoDocumentoTransportePendentesStartAsync(_cts.Token));

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                throw;
            }
        }

        public void OnStartDebugger(string[] args)
        {
            Servicos.Log.TratarErro("Serviço ProcessamentoDocumentoTransporte iniciado.");
            _cts = new CancellationTokenSource();

            try
            {
                string defaultCulture = Program.appSettings["AppSettings:DefaultCulture"];

                if (string.IsNullOrWhiteSpace(defaultCulture))
                    defaultCulture = "pt-BR";

                System.Globalization.CultureInfo cultureInfo = new System.Globalization.CultureInfo(defaultCulture);

                System.Threading.Thread.CurrentThread.CurrentCulture = cultureInfo;
                System.Threading.Thread.CurrentThread.CurrentUICulture = cultureInfo;

                System.Globalization.CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
                System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

                Servicos.IO.FileStorage.ConfigureApplicationFileStorage(Program.StringConexaoAdmin, Program.Host);
                EmissorDocumentoService.ConfigureApplicationEmissorDocumento(Program.StringConexao);
                ConfigurarThreads();

                Task.Run(() => ThreadProcessamentoDocumentoTransportePendentesStartAsync(_cts.Token));

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                throw;
            }
        }

        public void ConfigurarThreads()
        {
            _configuracoes = new List<ConfiguracaoThread>();

            #region ConfiguraTreadProcessamentoDocumentoTransporte
            _ArquivosProcessamentoDocumentoTransporte = new List<int>();
            _tasksGeracaoProcessamentoDocumentoTransporte = new List<Task>();
            
            try
            { // INICIALIZA _threadConsultaArquivosXmlNotaFiscaisPendentes
                int Id = Int16.Parse(Program.appSettings["AppSettings:Id"]);
                string Nome = Program.appSettings["AppSettings:Nome"];
                bool Ativa = bool.Parse(Program.appSettings["AppSettings:Ativa"]);
                int NumeroTheadsParalelas = int.Parse(Program.appSettings["AppSettings:NumeroTheadsParalelas"]);
                int NumeroMaximoTentativas = int.Parse(Program.appSettings["AppSettings:NumeroMaximoTentativas"]);
                int MinutosEsperaIntegracoesQueFalharam = int.Parse(Program.appSettings["AppSettings:MinutosEsperaIntegracoesQueFalharam"]);
                _configuracoes.Add(new ConfiguracaoThread(Id, Nome, Ativa, NumeroTheadsParalelas, NumeroMaximoTentativas, MinutosEsperaIntegracoesQueFalharam));
            }
            catch (Exception e)
            {
                Servicos.Log.TratarErro(e, _arquivoLog);
                throw;
            }
            #endregion
        }

        private async Task ThreadProcessamentoDocumentoTransportePendentesStartAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                GC.Collect();
                try
                {

                    var configuracao = ObterConfiguracaoThread("ThreadProcessamentoDocumentoTransportePendentes");
                    if (!(configuracao?.Ativa ?? false))
                    {
                        Servicos.Log.TratarErro("Serviço ThreadProcessamentoDocumentoTransportePendentes desabilitado, aguardando 10 minutos para nova execução.", _arquivoLog);
                        await Task.Delay(TimeSpan.FromMinutes(10), cancellationToken);
                        continue;
                    }

                    int quantidadeThreadsAtivas = _tasksGeracaoProcessamentoDocumentoTransporte.Count;

                    for (int i = quantidadeThreadsAtivas - 1; i >= 0; i--)
                    {
                        if (_tasksGeracaoProcessamentoDocumentoTransporte[i].IsCompleted)
                            _tasksGeracaoProcessamentoDocumentoTransporte.RemoveAt(i);
                    }

                    int quantidadeTasksDisponiveis = configuracao.NumeroTheadsParalelas - _tasksGeracaoProcessamentoDocumentoTransporte.Count;
                    
                    if (quantidadeTasksDisponiveis <= 0)
                    {
                        Servicos.Log.TratarErro($"Já existem {_tasksGeracaoProcessamentoDocumentoTransporte.Count} threads ativas. Aguardando 10 segundos para nova verificação.", _arquivoLog);
                        await Task.Delay(2000, cancellationToken);
                        continue;
                    }

                    List<Dominio.ObjetosDeValor.WebService.Rest.CargaPendenteProcessarDocumentoTransporte> TaskDocumentoTransportePendentesGeracao = new List<Dominio.ObjetosDeValor.WebService.Rest.CargaPendenteProcessarDocumentoTransporte>();

                    using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Program.StringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
                    {
                        Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork);
                        TaskDocumentoTransportePendentesGeracao = ObterCargasPendentes(unitOfWork, configuracao.NumeroTheadsParalelas, configuracao.NumeroMaximoTentativas, configuracao.MinutosEsperaIntegracoesQueFalharam);

#if DEBUG
                        TaskDocumentoTransportePendentesGeracao.Add(new Dominio.ObjetosDeValor.WebService.Rest.CargaPendenteProcessarDocumentoTransporte { CodigoArquivo = 1781521, CodigoCarga = 375941 });
#endif
                    }

                    if (TaskDocumentoTransportePendentesGeracao.Count <= 0)
                    {
                        Servicos.Log.TratarErro($"Não existem Cargas DT pendentes para CargaPendenteProcessarDocumentoTransporte. Aguardando 10 segundos para nova verificação. {_tasksGeracaoProcessamentoDocumentoTransporte.Count} de {configuracao.NumeroTheadsParalelas} threads em execução.", _arquivoLog);
                        System.Threading.Thread.Sleep(1000);
                        continue;
                    }

                    for (int i = 0; i < quantidadeTasksDisponiveis && i < TaskDocumentoTransportePendentesGeracao.Count; i++)
                    {
                        Dominio.ObjetosDeValor.WebService.Rest.CargaPendenteProcessarDocumentoTransporte integracaoPendente = TaskDocumentoTransportePendentesGeracao[i];
                        var task = Task.Run(() => ProcessamentoDocumentoTransporteIntegracaoAsync(integracaoPendente, cancellationToken), cancellationToken);
                        _tasksGeracaoProcessamentoDocumentoTransporte.Add(task);

                        SanitizarDocumentoTransporteCargaAnterior(integracaoPendente);
                    }

                    await Task.Delay(2000, cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    Servicos.Log.TratarErro("Cancelamento solicitado.", _arquivoLog);
                    break;
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex, _arquivoLog);
                    await Task.Delay(5000, cancellationToken);
                }
            }
        }

        private async Task ProcessamentoDocumentoTransporteIntegracaoAsync(Dominio.ObjetosDeValor.WebService.Rest.CargaPendenteProcessarDocumentoTransporte arquivoProcessar, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                lock (_ArquivosProcessamentoDocumentoTransporte)
                {
                    if (!_ArquivosProcessamentoDocumentoTransporte.Any(o => o == arquivoProcessar.CodigoCarga))
                        _ArquivosProcessamentoDocumentoTransporte.Add(arquivoProcessar.CodigoCarga);
                    else
                        return;

                    Servicos.Log.TratarErro($"Iniciou thread para geração de ProcessamentoDocumentoTransporteIntegracao {arquivoProcessar.CodigoCarga}", _arquivoLog);
                }

                using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Program.StringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
                {
                    await unitOfWork.StartAsync(cancellationToken);

                    Repositorio.Embarcador.Cargas.CargaCancelamento repCargaCancelamento = new Repositorio.Embarcador.Cargas.CargaCancelamento(unitOfWork);
                    Repositorio.WebService.IntegradoraIntegracaoRetorno repIntegradoraIntegracaoRetorno = new Repositorio.WebService.IntegradoraIntegracaoRetorno(unitOfWork);
                    Dominio.Entidades.WebService.IntegradoraIntegracaoRetorno integracao = repIntegradoraIntegracaoRetorno.BuscarPorCodigo(arquivoProcessar.CodigoArquivo);

                    try
                    {
                        if (integracao != null)
                        {
                            if (integracao.ArquivoRequisicao == null)
                            {
                                integracao.Mensagem = "Arquivo de requisição não encontrado";
                                integracao.Situacao = SituacaoIntegracao.ProblemaIntegracao;
                                integracao.Tentativas++;
                                integracao.DataUltimaTentativa = DateTime.Now;
                                await repIntegradoraIntegracaoRetorno.AtualizarAsync(integracao);

                                new Servicos.Embarcador.Integracao.Unilever.IntegracaoUnilever(unitOfWork).IntegrarRertornoCarga(integracao);

                                await unitOfWork.CommitChangesAsync(cancellationToken);
                                unitOfWork.FlushAndClear();
                            }
                            else
                            {
                                Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado();
                                auditado.TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Integradoras;
                                auditado.Integradora = integracao.Integradora;

                                string request = Servicos.Embarcador.Integracao.ArquivoIntegracao.RetornarArquivoTexto(integracao.ArquivoRequisicao);
                                Dominio.ObjetosDeValor.WebService.Rest.DocumentoTransporte documentoTransporte = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.WebService.Rest.DocumentoTransporte>(request);

                                string mensagem = string.Empty;

                                if (repCargaCancelamento.ExistePorCargaEData(integracao.Data, integracao.Carga?.Codigo ?? 0))
                                {
                                    integracao.Mensagem = "Carga integrada com sucesso (integração anterior ao cancelamento da carga)";
                                    integracao.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                                    integracao.Sucesso = true;
                                    integracao.Tentativas++;
                                    integracao.DataUltimaTentativa = DateTime.Now;
                                    await repIntegradoraIntegracaoRetorno.AtualizarAsync(integracao);
                                    return;
                                }

                                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork).BuscarConfiguracaoPadrao();

                                Dominio.Entidades.Embarcador.Cargas.Carga cargaProcessar = new Repositorio.Embarcador.Cargas.Carga(unitOfWork).BuscarPorProtocoloFetchProcessarDocumento(integracao.Carga?.Codigo ?? 0);

                                if (cargaProcessar == null)
                                    throw new ServicoException($"Não encontrada carga com o protocolo {integracao.Carga?.Codigo ?? 0}.");

                                Dominio.Entidades.Embarcador.Cargas.Carga carga = Servicos.Embarcador.Carga.DocumentoTransporte.ProcessarDocumentoTransporte.GerarCargaPorDocumentoTransporte(cargaProcessar, documentoTransporte, auditado, Program.TipoServicoMultisoftware, Program.ClienteURLAcesso, Program.Cliente, Program.StringConexaoAdmin, unitOfWork, configuracaoTMS);

                                integracao.Mensagem = "Carga integrada com sucesso.";
                                integracao.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                                integracao.Sucesso = true;
                                integracao.Tentativas++;
                                integracao.DataUltimaTentativa = DateTime.Now;
                                await repIntegradoraIntegracaoRetorno.AtualizarAsync(integracao);

                                await unitOfWork.CommitChangesAsync(cancellationToken);

                                Servicos.Log.TratarErro($"Sucesso Carga: {arquivoProcessar.CodigoCarga}", _arquivoLog);

                                new Servicos.Embarcador.Integracao.Unilever.IntegracaoUnilever(unitOfWork).IntegrarRertornoCarga(integracao);
                                new Servicos.Embarcador.Integracao.IntegracaoCargaEvento(unitOfWork, TipoServicoMultisoftware.MultiEmbarcador).AdicionarIntegracaoIndividual(carga, EtapaCarga.SalvarDadosTransporte, $"Salvar DT processado com sucesso.", new List<TipoIntegracao>() { TipoIntegracao.ArcelorMittal }, true);

                                unitOfWork.FlushAndClear();
                            }
                        }
                        else
                            await unitOfWork.RollbackAsync(cancellationToken);
                    }
                    catch (ServicoException se)
                    {
                        await unitOfWork.RollbackAsync(cancellationToken);

                        Servicos.Log.TratarErro($"{se}. Carga código: {integracao.Carga?.Codigo ?? 0}");
     
                        if(integracao?.Codigo > 0)
                            AtualizarIntegracaoError(integracao.Codigo);
                        
                        throw new Exception(se.Message);

                    }
                    catch (Exception ex)
                    {
                        await unitOfWork.RollbackAsync(cancellationToken);

                        Servicos.Log.TratarErro($"{ex}. Carga código: {integracao.Carga?.Codigo ?? 0}");

                        if (integracao?.Codigo > 0)
                            AtualizarIntegracaoError(integracao.Codigo);

                        throw new Exception(ex.Message);
                    }
                }

                using (Repositorio.UnitOfWork unitOfWorkAtualizar = new Repositorio.UnitOfWork(Program.StringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
                {
                    try
                    {
                        await unitOfWorkAtualizar.StartAsync(cancellationToken);

                        Repositorio.WebService.IntegradoraIntegracaoRetorno repIntegradoraIntegracaoRetorno = new Repositorio.WebService.IntegradoraIntegracaoRetorno(unitOfWorkAtualizar);

                        Dominio.Entidades.WebService.IntegradoraIntegracaoRetorno integracao = repIntegradoraIntegracaoRetorno.BuscarPorCodigo(arquivoProcessar.CodigoArquivo);

                        if (integracao != null)
                        {
                            string request = Servicos.Embarcador.Integracao.ArquivoIntegracao.RetornarArquivoTexto(integracao.ArquivoRequisicao);
                            Dominio.ObjetosDeValor.WebService.Rest.DocumentoTransporte documentoTransporte = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.WebService.Rest.DocumentoTransporte>(request);
                            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWorkAtualizar).BuscarConfiguracaoPadrao();
                            Dominio.Entidades.Embarcador.Cargas.Carga cargaProcessar = new Repositorio.Embarcador.Cargas.Carga(unitOfWorkAtualizar).BuscarPorProtocoloFetchProcessarDocumento(integracao.Carga?.Codigo ?? 0);

                            if (configuracaoTMS.IncluirCargaCanceladaProcessarDT)
                            {
                                Servicos.Embarcador.Carga.DocumentoTransporte.ProcessarDocumentoTransporte.AtualizarDadosSumarizadosCarga(cargaProcessar, documentoTransporte, unitOfWorkAtualizar, Program.TipoServicoMultisoftware, configuracaoTMS);

                                await unitOfWorkAtualizar.CommitChangesAsync(cancellationToken);

                                unitOfWorkAtualizar.FlushAndClear();
                            }
                        }
                        else
                            await unitOfWorkAtualizar.RollbackAsync(cancellationToken);
                    }
                    catch (ServicoException se)
                    {
                        await unitOfWorkAtualizar.RollbackAsync(cancellationToken);

                        Servicos.Log.TratarErro(se);
                    }
                    catch (Exception ex)
                    {
                        await unitOfWorkAtualizar.RollbackAsync(cancellationToken);

                        Servicos.Log.TratarErro(ex);
                    }

                    unitOfWorkAtualizar.FlushAndClear();
                }
            }
            catch (Exception se)
            {
                Servicos.Log.TratarErro(se);
            }
            finally
            {
                lock (_ArquivosProcessamentoDocumentoTransporte)
                {
                    if (_ArquivosProcessamentoDocumentoTransporte.Any(o => o == arquivoProcessar.CodigoCarga))
                        _ArquivosProcessamentoDocumentoTransporte.Remove(arquivoProcessar.CodigoCarga);

                    Servicos.Log.TratarErro($"Finalizou thread para geração de ProcessamentoDocumentoTransporteIntegracao carga: {arquivoProcessar.CodigoCarga}.", _arquivoLog);
                }
            }
        }

        private void AtualizarIntegracaoError(long codigoArquivo)
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Program.StringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
            {
                unitOfWork.Start();

                Dominio.Entidades.WebService.IntegradoraIntegracaoRetorno integracao = new Dominio.Entidades.WebService.IntegradoraIntegracaoRetorno();
                Repositorio.WebService.IntegradoraIntegracaoRetorno repIntegradoraIntegracaoRetorno = new Repositorio.WebService.IntegradoraIntegracaoRetorno(unitOfWork);
                
                integracao = repIntegradoraIntegracaoRetorno.BuscarPorCodigo(codigoArquivo);

                if (integracao == null)
                    throw new Exception("Não foi possível atualizar o motivo do erro com a integração.");

                integracao.Sucesso = false;
                integracao.Mensagem = "Problema ao tentar processar a integração";
                integracao.Situacao = SituacaoIntegracao.ProblemaIntegracao;
                integracao.Tentativas++;
                integracao.DataUltimaTentativa = DateTime.Now;
                repIntegradoraIntegracaoRetorno.Atualizar(integracao);

                unitOfWork.CommitChanges();

                new Servicos.Embarcador.Integracao.Unilever.IntegracaoUnilever(unitOfWork).IntegrarRertornoCarga(integracao);
                new Servicos.Embarcador.Integracao.IntegracaoCargaEvento(unitOfWork, TipoServicoMultisoftware.MultiEmbarcador).AdicionarIntegracaoIndividual(integracao.Carga, EtapaCarga.SalvarDadosTransporte, $"Falha no processamento do Salvar DT.", new List<TipoIntegracao>() { TipoIntegracao.ArcelorMittal });

                unitOfWork.FlushAndClear();
            }
        }

        private void SanitizarDocumentoTransporteCargaAnterior(Dominio.ObjetosDeValor.WebService.Rest.CargaPendenteProcessarDocumentoTransporte arquivoProcessar)
        {
            try
            {
                using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Program.StringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
                {
                    Repositorio.WebService.IntegradoraIntegracaoRetorno repIntegradoraIntegracaoRetorno = new Repositorio.WebService.IntegradoraIntegracaoRetorno(unitOfWork);
                    repIntegradoraIntegracaoRetorno.SanitizarIntegracoesCargaAnteriores(arquivoProcessar);
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, _arquivoLog);
            }
            finally
            {
                Servicos.Log.TratarErro($"Sanitizou documento tranporte carga Anterior Carga: {arquivoProcessar.CodigoCarga}.", _arquivoLog);
            }
        }

        private List<Dominio.ObjetosDeValor.WebService.Rest.CargaPendenteProcessarDocumentoTransporte> ObterCargasPendentes(Repositorio.UnitOfWork unitOfWork, int numeroThreads, int numeroMaximoTentativas, int minutosEsperaIntegracoesQueFalharam)
        {
            Repositorio.WebService.IntegradoraIntegracaoRetorno repIntegradoraIntegracaoRetorno = new Repositorio.WebService.IntegradoraIntegracaoRetorno(unitOfWork);
            IList<Dominio.ObjetosDeValor.WebService.Rest.CargaPendenteProcessarDocumentoTransporte> codigosCargaPendentes = repIntegradoraIntegracaoRetorno.BuscarCargasPendenteIntegracaoServico(numeroThreads, numeroMaximoTentativas, minutosEsperaIntegracoesQueFalharam);

            List<Dominio.ObjetosDeValor.WebService.Rest.CargaPendenteProcessarDocumentoTransporte> codigosCargaSemThread = new List<Dominio.ObjetosDeValor.WebService.Rest.CargaPendenteProcessarDocumentoTransporte>();
            lock (_ArquivosProcessamentoDocumentoTransporte)
            {
                foreach (var codigo in codigosCargaPendentes)
                    if (!_ArquivosProcessamentoDocumentoTransporte.Any(o => o == codigo.CodigoCarga))
                        codigosCargaSemThread.Add(codigo);
            }

#if DEBUG
            List<Dominio.ObjetosDeValor.WebService.Rest.CargaPendenteProcessarDocumentoTransporte> codigosCargas = new List<Dominio.ObjetosDeValor.WebService.Rest.CargaPendenteProcessarDocumentoTransporte>() { /*Adicionar códigos aqui para debug*/ };
            if (codigosCargas.Count > 0)
                codigosCargaSemThread = codigosCargas;
#endif

            return codigosCargaSemThread;
        }

        private ConfiguracaoThread ObterConfiguracaoThread(string nome)
        {
            return _configuracoes.Where(x => x.Nome == nome).FirstOrDefault();
        }

        protected override void OnStop()
        {
            _cts?.Cancel();
            GC.Collect();
            Servicos.Log.TratarErro("Serviço parado.");
        }
    }
}