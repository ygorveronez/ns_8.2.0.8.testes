using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Repositorio;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.ServiceProcess;

namespace SGT.WebAdmin.ProcessarArquivoXMLNotaFiscalIntegracao
{
    public partial class ProcessarThreads : ServiceBase
    {
        private List<int> _ArquivosXmlEmProcessamento;
        private List<System.Threading.Thread> _threadsGeracaoArquivosXmlNotaFiscais;
        private System.Threading.Thread _threadArquivosXmlNotaFiscaisPendentes;
        private System.Threading.Thread _threadArquivosXmlNotaFiscaisLiberacao;
        private static string _arquivoLog = "Logs";
        private List<ConfiguracaoThread> _configuracoes;

        public ProcessarThreads()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Servicos.Log.TratarErro("Serviço ProcessarArquivoXMLNotaFiscalIntegracao iniciado.");
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
                ConfigurarThreads();
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

            #region ConfiguraTreadArquivosXmlNotaFiscaisPendentes
            _ArquivosXmlEmProcessamento = new List<int>();
            _threadsGeracaoArquivosXmlNotaFiscais = new List<System.Threading.Thread>();
            try
            { // INICIALIZA _threadConsultaArquivosXmlNotaFiscaisPendentes

                int Id = Int16.Parse(Program.appSettings["ThreadArquivosXmlNotaFiscaisPendentes:Id"]);
                string Nome = Program.appSettings["ThreadArquivosXmlNotaFiscaisPendentes:Nome"];
                bool Ativa = bool.Parse(Program.appSettings["ThreadArquivosXmlNotaFiscaisPendentes:Ativa"]);
                int NumeroTheadsParalelas = int.Parse(Program.appSettings["ThreadArquivosXmlNotaFiscaisPendentes:NumeroTheadsParalelas"]);
                _configuracoes.Add(new ConfiguracaoThread(Id, Nome, Ativa, NumeroTheadsParalelas));

                int IdLiberacao = Int16.Parse(Program.appSettings["ThreadArquivosXmlNotaFiscaisLiberacao:Id"]); ;
                string NomeLiberacao = Program.appSettings["ThreadArquivosXmlNotaFiscaisLiberacao:Nome"];
                bool AtivaLiberacao = bool.Parse(Program.appSettings["ThreadArquivosXmlNotaFiscaisLiberacao:Ativa"]);
                _configuracoes.Add(new ConfiguracaoThread(IdLiberacao, NomeLiberacao, AtivaLiberacao, 1));

                if (_threadArquivosXmlNotaFiscaisPendentes == null)
                {
                    _threadArquivosXmlNotaFiscaisPendentes = new System.Threading.Thread(ThreadArquivosXmlNotaFiscaisPendentesStart);
                    _threadArquivosXmlNotaFiscaisPendentes.Start();
                }

                if (_threadArquivosXmlNotaFiscaisLiberacao == null)
                {
                    _threadArquivosXmlNotaFiscaisLiberacao = new System.Threading.Thread(ThreadArquivosXmlNotaFiscaisLiberacaoStart);
                    _threadArquivosXmlNotaFiscaisLiberacao.Start();
                }


            }
            catch (Exception e)
            {
                Servicos.Log.TratarErro(e, _arquivoLog);
                throw;
            }
            #endregion
        }

        private void ThreadArquivosXmlNotaFiscaisPendentesStart()
        {
            while (true)
            {
                GC.Collect();
                try
                {
                    var configuracao = ObterConfiguracaoThread("ThreadArquivosXmlNotaFiscaisPendentes");
                    if (!(configuracao?.Ativa ?? false))
                    {
                        Servicos.Log.TratarErro("Serviço ThreadProcessarArquivoXMLNotaFiscalIntegracao desabilitado, aguardando 10 minutos para nova execução.", _arquivoLog);
                        System.Threading.Thread.Sleep(600000);
                        continue;
                    }

                    int quantidadeThreadsDisponiveis = 0;
                    int quantidadeThreadsAtivas = _threadsGeracaoArquivosXmlNotaFiscais.Count;

                    List<int> threadsRemover = new List<int>();
                    for (int i = quantidadeThreadsAtivas - 1; i >= 0; i--)
                    {
                        if (_threadsGeracaoArquivosXmlNotaFiscais[i].IsAlive)
                            continue;
                        _threadsGeracaoArquivosXmlNotaFiscais.RemoveAt(i);
                    }
                    quantidadeThreadsDisponiveis = configuracao.NumeroTheadsParalelas - _threadsGeracaoArquivosXmlNotaFiscais.Count;
                    if (quantidadeThreadsDisponiveis <= 0)
                    {
                        Servicos.Log.TratarErro($"Já existem {_threadsGeracaoArquivosXmlNotaFiscais.Count} threads ativas. Aguardando 10 segundos para nova verificação.", _arquivoLog);
                        System.Threading.Thread.Sleep(1000);
                        continue;
                    }

                    Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeral configuracaoGeral;
                    List<int> ThreadArquivosXmlNotaFiscaisPendentesGeracao = new List<int>();

                    using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Program.StringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
                    {
                        Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork);
                        configuracaoGeral = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral(unitOfWork).BuscarConfiguracaoPadrao();
                        if (configuracaoGeral?.ProcessarXMLNotasFiscaisAssincrono ?? false)
                            ThreadArquivosXmlNotaFiscaisPendentesGeracao = ObterArquivosDadosXmlPendentes(unitOfWork, configuracao.NumeroTheadsParalelas);
                        else
                            ThreadArquivosXmlNotaFiscaisPendentesGeracao = ObterArquivosXmlPendentes(unitOfWork, configuracao.NumeroTheadsParalelas, 5, 2);
                    }

                    if (configuracaoGeral?.ProcessarXMLNotasFiscaisAssincrono ?? false)
                    {
                        //PROCESSO DE PROCESSAMENTO NOTAS FISCAIS ACELORMITTAL
                        if (ThreadArquivosXmlNotaFiscaisPendentesGeracao.Count <= 0)
                        {
                            Servicos.Log.TratarErro($"Não existem ArquivosXmlNotaFiscaisPendentes pendentes para ProcessarArquivoXMLNotaFiscalIntegracao. Aguardando 10 segundos para nova verificação. {_threadsGeracaoArquivosXmlNotaFiscais.Count} de {configuracao.NumeroTheadsParalelas} threads em execução.", _arquivoLog);
                            System.Threading.Thread.Sleep(1000);
                            continue;
                        }

                        for (int i = 0; i < quantidadeThreadsDisponiveis && i < ThreadArquivosXmlNotaFiscaisPendentesGeracao.Count; i++)
                        {
                            int codigoCargaPendenteGeracao = ThreadArquivosXmlNotaFiscaisPendentesGeracao[i];
                            System.Threading.Thread thread = new System.Threading.Thread(() => ProcessarDadosXMLNotasFiscaisIntegracao(codigoCargaPendenteGeracao));
                            thread.Start();
                            _threadsGeracaoArquivosXmlNotaFiscais.Add(thread);
                        }
                    }
                    else
                    {
                        //PROCESSO DE PROCESSAMENTO NOTAS FISCAIS UNILEVER
                        if (ThreadArquivosXmlNotaFiscaisPendentesGeracao.Count <= 0)
                        {
                            Servicos.Log.TratarErro($"Não existem ArquivosXmlNotaFiscaisPendentes pendentes para ProcessarArquivoXMLNotaFiscalIntegracao. Aguardando 10 segundos para nova verificação. {_threadsGeracaoArquivosXmlNotaFiscais.Count} de {configuracao.NumeroTheadsParalelas} threads em execução.", _arquivoLog);
                            System.Threading.Thread.Sleep(1000);
                            continue;
                        }

                        for (int i = 0; i < quantidadeThreadsDisponiveis && i < ThreadArquivosXmlNotaFiscaisPendentesGeracao.Count; i++)
                        {
                            int codigoCargaPendenteGeracao = ThreadArquivosXmlNotaFiscaisPendentesGeracao[i];
                            System.Threading.Thread thread = new System.Threading.Thread(() => ProcessarArquivoXMLNotaFiscalIntegracao(codigoCargaPendenteGeracao));
                            thread.Start();
                            _threadsGeracaoArquivosXmlNotaFiscais.Add(thread);
                        }
                    }

                    System.Threading.Thread.Sleep(1000);

                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex, _arquivoLog);
                    System.Threading.Thread.Sleep(5000);
                }
            }
        }

        private void ThreadArquivosXmlNotaFiscaisLiberacaoStart()
        {
            while (true)
            {
                GC.Collect();
                try
                {
                    using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Program.StringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
                    {

                        var configuracao = ObterConfiguracaoThread("ThreadArquivosXmlNotaFiscaisLiberacao");
                        if (!(configuracao?.Ativa ?? false))
                        {
                            Servicos.Log.TratarErro("Serviço ThreadProcessarArquivoXMLNotaFiscalIntegracao desabilitado, aguardando 10 minutos para nova execução.", _arquivoLog);
                            System.Threading.Thread.Sleep(600000);
                            continue;
                        }

                        ProcessarArquivosXmlLiberacao(unitOfWork);

                        System.Threading.Thread.Sleep(1000);
                    }
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex, _arquivoLog);
                    System.Threading.Thread.Sleep(5000);
                }
            }
        }


        private void ProcessarDadosXMLNotasFiscaisIntegracao(int codigoArquivoXml)
        {
            try
            {
                lock (_ArquivosXmlEmProcessamento)
                {
                    if (!_ArquivosXmlEmProcessamento.Any(o => o == codigoArquivoXml))
                        _ArquivosXmlEmProcessamento.Add(codigoArquivoXml);
                    else
                        return;
                    Servicos.Log.TratarErro($"Iniciou thread para geração de ProcessarArquivoXMLNotaFiscalIntegracao {codigoArquivoXml}", _arquivoLog);
                }

                using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Program.StringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
                {

                    Repositorio.Embarcador.NotaFiscal.ProcessamentoNotaFiscalAssincrono repProcessamentoNotaFiscalAssincrono = new Repositorio.Embarcador.NotaFiscal.ProcessamentoNotaFiscalAssincrono(unitOfWork);
                    Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXmlNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
                    Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork).BuscarConfiguracaoPadrao();
                    Servicos.Embarcador.NFe.NFe servicoNfe = new Servicos.Embarcador.NFe.NFe(unitOfWork);
                    Servicos.Embarcador.Pedido.NotaFiscal serCargaNotaFiscal = new Servicos.Embarcador.Pedido.NotaFiscal(unitOfWork);

                    Dominio.Entidades.Embarcador.NotaFiscal.ProcessamentoNotaFiscalAssincrono processamentoNotaFiscalAssincrono = repProcessamentoNotaFiscalAssincrono.BuscarPendentePorCodigo(codigoArquivoXml);
#if DEBUG
                    processamentoNotaFiscalAssincrono = repProcessamentoNotaFiscalAssincrono.BuscarPorCodigo(codigoArquivoXml, false);
#endif

                    string mensagemErro = string.Empty;

                    Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado()
                    {
                        TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Integradoras,
                        Integradora = processamentoNotaFiscalAssincrono.Integradora,
                    };

                    processamentoNotaFiscalAssincrono.Initialize();
                    processamentoNotaFiscalAssincrono.Tentativas++;
                    repProcessamentoNotaFiscalAssincrono.Atualizar(processamentoNotaFiscalAssincrono, auditado);

                    unitOfWork.Start();

                    Servicos.Log.TratarErro($"Lendo arquivo protocolo pedido {processamentoNotaFiscalAssincrono.ProtocoloPedido} {DateTime.Now.ToString("dd/MM/yyyy HH:mm:sss")} Codigo {codigoArquivoXml}", $"ProcessamentoArquivoXMLNotaFiscalIntegracao");

                    try
                    {
                        //deserializar objeto
                        string request = Servicos.Embarcador.Integracao.ArquivoIntegracao.RetornarArquivoTexto(processamentoNotaFiscalAssincrono.ArquivoRequisicao);
                        Dominio.ObjetosDeValor.Embarcador.NFe.DadosNotaProcessar dadosProcessar = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.NFe.DadosNotaProcessar>(request);

                        Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                        Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                        Repositorio.Embarcador.NotaFiscal.ProcessamentoNotaFiscalAssincronoChaveRecebida repNotaFiscalRecebida = new Repositorio.Embarcador.NotaFiscal.ProcessamentoNotaFiscalAssincronoChaveRecebida(unitOfWork);

                        Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorProtocoloCargaOrigemEProtocoloPedido(dadosProcessar.Protocolo.protocoloIntegracaoCarga, dadosProcessar.Protocolo.protocoloIntegracaoPedido);

                        if (cargaPedido == null)
                        {
                            Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(dadosProcessar.Protocolo.protocoloIntegracaoCarga, false);
                            if (carga != null)
                                throw new Exception($"O protocolo pedido {dadosProcessar.Protocolo.protocoloIntegracaoPedido} não esta na carga.");
                        }

                        processamentoNotaFiscalAssincrono.NotasFiscais = new List<Dominio.Entidades.Embarcador.NotaFiscal.ProcessamentoNotaFiscalAssincronoChaveRecebida>();

                        foreach (var nota in dadosProcessar.NotasFiscais)
                        {

                            Dominio.Entidades.Embarcador.NotaFiscal.ProcessamentoNotaFiscalAssincronoChaveRecebida notarecebida = new Dominio.Entidades.Embarcador.NotaFiscal.ProcessamentoNotaFiscalAssincronoChaveRecebida();
                            notarecebida.ChaveNotaFiscal = nota.Chave;
                            notarecebida.NumeroNotaFiscal = nota.Numero;
                            repNotaFiscalRecebida.Inserir(notarecebida);

                            processamentoNotaFiscalAssincrono.NotasFiscais.Add(notarecebida);
                        }
                        repProcessamentoNotaFiscalAssincrono.Atualizar(processamentoNotaFiscalAssincrono);

                        if (cargaPedido.SituacaoEmissao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNF.NFEnviada)
                        {
                            mensagemErro = Servicos.WebService.NFe.NotaFiscal.IntegrarNotaFiscal(cargaPedido, dadosProcessar.NotasFiscais, null, null, configuracaoTMS, Program.TipoServicoMultisoftware, auditado, processamentoNotaFiscalAssincrono.Integradora, unitOfWork);

                            if (string.IsNullOrWhiteSpace(mensagemErro))
                            {
                                Servicos.Auditoria.Auditoria.Auditar(auditado, cargaPedido, "Integrou dados de notas fiscais", unitOfWork);
                                Servicos.Auditoria.Auditoria.Auditar(auditado, cargaPedido.Carga, "Integrou dados de notas fiscais", unitOfWork);
                            }
                            else
                            {
                                if (mensagemErro.Contains("já foi enviada"))//se ja esta alguma delas susse...
                                    mensagemErro = "";
                                else
                                {
                                    Servicos.Log.TratarErro($"problemas ao integrar dados nota protocolo pedido {processamentoNotaFiscalAssincrono.ProtocoloPedido} Falha: {mensagemErro} - {DateTime.Now.ToString("dd/MM/yyyy HH:mm:sss")} Codigo Processamento {codigoArquivoXml}", $"ProcessamentoArquivoXMLNotaFiscalIntegracao");
                                    processamentoNotaFiscalAssincrono.Mensagem = mensagemErro;
                                }
                            }
                        }
                        else
                        {
                            //vamos validar se a nota esta no carga pedido.
                            foreach (var nota in dadosProcessar.NotasFiscais)
                            {
                                if (!repPedidoXmlNotaFiscal.ExisteNotaCargaPedidoPorChave(cargaPedido.Codigo, nota.Chave))
                                {
                                    mensagemErro = Servicos.WebService.NFe.NotaFiscal.IntegrarNotaFiscal(cargaPedido, dadosProcessar.NotasFiscais, null, null, configuracaoTMS, Program.TipoServicoMultisoftware, auditado, processamentoNotaFiscalAssincrono.Integradora, unitOfWork);

                                    if (string.IsNullOrWhiteSpace(mensagemErro))
                                    {
                                        Servicos.Auditoria.Auditoria.Auditar(auditado, cargaPedido, "Integrou dados de notas fiscais", unitOfWork);
                                        Servicos.Auditoria.Auditoria.Auditar(auditado, cargaPedido.Carga, "Integrou dados de notas fiscais", unitOfWork);
                                    }
                                    else
                                    {
                                        if (mensagemErro.Contains("já foi enviada"))//se ja esta alguma delas susse...
                                            mensagemErro = "";
                                        else
                                        {
                                            Servicos.Log.TratarErro($"problemas ao integrar dados nota protocolo pedido {processamentoNotaFiscalAssincrono.ProtocoloPedido} Falha: {mensagemErro} - {DateTime.Now.ToString("dd/MM/yyyy HH:mm:sss")} Codigo Processamento {codigoArquivoXml}", $"ProcessamentoArquivoXMLNotaFiscalIntegracao");
                                            processamentoNotaFiscalAssincrono.Mensagem = mensagemErro;
                                        }
                                    }
                                }
                            }
                        }

                        if (processamentoNotaFiscalAssincrono.Tentativas >= 3 && !string.IsNullOrEmpty(mensagemErro))
                            processamentoNotaFiscalAssincrono.Situacao = SituacaoIntegracao.ProblemaIntegracao;

                        if (string.IsNullOrEmpty(mensagemErro))
                        {
                            processamentoNotaFiscalAssincrono.Situacao = SituacaoIntegracao.Integrado;
                            processamentoNotaFiscalAssincrono.MensagemRetorno = "Notas processadas com sucesso.";
                        }

                        Servicos.Log.TratarErro($"Finalizaou Integrar dados Nota fiscal protocolo Pedido {processamentoNotaFiscalAssincrono.ProtocoloPedido} {DateTime.Now.ToString("dd/MM/yyyy HH:mm:sss")} Codigo Processamento {codigoArquivoXml}", $"ProcessamentoArquivoXMLNotaFiscalIntegracao");

                        unitOfWork.CommitChanges();
                        unitOfWork.FlushAndClear();
                    }
                    catch (ServicoException ex)
                    {
                        unitOfWork.Rollback();

                        unitOfWork.Start();

                        Servicos.Log.TratarErro($"Falha Tratada Arquivo Ex: {ex} {DateTime.Now.ToString("dd/MM/yyyy HH:mm:sss")} Codigo {codigoArquivoXml}", $"ProcessamentoArquivoXMLNotaFiscalIntegracao");

                        processamentoNotaFiscalAssincrono.Tentativas++;
                        processamentoNotaFiscalAssincrono.NotasFiscais = new List<Dominio.Entidades.Embarcador.NotaFiscal.ProcessamentoNotaFiscalAssincronoChaveRecebida>();

                        mensagemErro = ex.Message.Left(450);
                        if (processamentoNotaFiscalAssincrono.Tentativas >= 3)
                        {
                            processamentoNotaFiscalAssincrono.Situacao = SituacaoIntegracao.ProblemaIntegracao;
                            new Servicos.Embarcador.Integracao.IntegracaoCargaEvento(unitOfWork, Program.TipoServicoMultisoftware).AdicionarIntegracaoIndividual(processamentoNotaFiscalAssincrono.Carga, EtapaCarga.NotaFiscal, $"Falha ao processar Nota fiscal {mensagemErro.Left(250)}", new List<TipoIntegracao>() { TipoIntegracao.ArcelorMittal });
                        }

                        processamentoNotaFiscalAssincrono.Mensagem = mensagemErro;
                        repProcessamentoNotaFiscalAssincrono.Atualizar(processamentoNotaFiscalAssincrono, auditado);

                        unitOfWork.CommitChanges();
                    }
                    catch (System.Exception ex)
                    {
                        unitOfWork.Rollback();

                        unitOfWork.Start();

                        Servicos.Log.TratarErro($"Falha Generica Arquivo Ex: {ex} {DateTime.Now.ToString("dd/MM/yyyy HH:mm:sss")} Codigo {codigoArquivoXml}", $"ProcessamentoArquivoXMLNotaFiscalIntegracao");

                        processamentoNotaFiscalAssincrono.Tentativas++;
                        processamentoNotaFiscalAssincrono.NotasFiscais = new List<Dominio.Entidades.Embarcador.NotaFiscal.ProcessamentoNotaFiscalAssincronoChaveRecebida>();

                        mensagemErro = ex.Message.Left(450);
                        if (processamentoNotaFiscalAssincrono.Tentativas >= 3)
                        {
                            processamentoNotaFiscalAssincrono.Situacao = SituacaoIntegracao.ProblemaIntegracao;
                            new Servicos.Embarcador.Integracao.IntegracaoCargaEvento(unitOfWork, Program.TipoServicoMultisoftware).AdicionarIntegracaoIndividual(processamentoNotaFiscalAssincrono.Carga, EtapaCarga.NotaFiscal, $"Falha ao processar Nota fiscal: Falha Genérica ao processar dados nota Fiscal do pedido protocolo: {processamentoNotaFiscalAssincrono.ProtocoloPedido}", new List<TipoIntegracao>() { TipoIntegracao.ArcelorMittal });
                        }

                        processamentoNotaFiscalAssincrono.Mensagem = mensagemErro;
                        repProcessamentoNotaFiscalAssincrono.Atualizar(processamentoNotaFiscalAssincrono, auditado);

                        unitOfWork.CommitChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, _arquivoLog);
            }
            finally
            {
                lock (_ArquivosXmlEmProcessamento)
                {
                    if (_ArquivosXmlEmProcessamento.Any(o => o == codigoArquivoXml))
                        _ArquivosXmlEmProcessamento.Remove(codigoArquivoXml);

                    Servicos.Log.TratarErro($"Finalizou thread para geração de ProcessarArquivoXMLNotaFiscalIntegracao {codigoArquivoXml}.", _arquivoLog);
                }
            }
        }

        private void ProcessarArquivoXMLNotaFiscalIntegracao(int codigoArquivoXml)
        {
            try
            {
                lock (_ArquivosXmlEmProcessamento)
                {
                    if (!_ArquivosXmlEmProcessamento.Any(o => o == codigoArquivoXml))
                        _ArquivosXmlEmProcessamento.Add(codigoArquivoXml);
                    else
                        return;
                    Servicos.Log.TratarErro($"Iniciou thread para geração de ProcessarArquivoXMLNotaFiscalIntegracao {codigoArquivoXml}", _arquivoLog);
                }

                using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Program.StringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
                {

                    Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoArquivo configuracaoArquivo = new Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo(unitOfWork).BuscarPrimeiroRegistro();
                    Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork).BuscarConfiguracaoPadrao();
                    Repositorio.Embarcador.NotaFiscal.ArquivoXMLNotaFiscalIntegracao repArquivos = new Repositorio.Embarcador.NotaFiscal.ArquivoXMLNotaFiscalIntegracao(unitOfWork);
                    Repositorio.Embarcador.Pedidos.XMLNotaFiscal repositorioNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
                    Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repositoriopedidoNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
                    Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService repConfiguracaoWebService = new Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService(unitOfWork);
                   
                    Servicos.Embarcador.NFe.NFe servicoNfe = new Servicos.Embarcador.NFe.NFe(unitOfWork);
                    Servicos.Embarcador.Pedido.NotaFiscal serCargaNotaFiscal = new Servicos.Embarcador.Pedido.NotaFiscal(unitOfWork);

                    Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configuracaoWebservice = repConfiguracaoWebService.BuscarConfiguracaoPadrao();

                    if (configuracaoArquivo == null)
                        return;

                    Dominio.Entidades.Embarcador.NotaFiscal.ArquivoXMLNotaFiscalIntegracao arquivo = repArquivos.BuscarPorCodigo(codigoArquivoXml, false);
#if DEBUG
                    arquivo = repArquivos.BuscarPorCodigo(codigoArquivoXml, false);
#endif

                    string mensagemErro = string.Empty;

                    Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado()
                    {
                        TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Integradoras,
                        Integradora = arquivo.Integradora,
                        IP = arquivo.IP
                    };

                    arquivo.Initialize();
                    arquivo.DataTentativa = DateTime.Now;
                    arquivo.Tentativas++;
                    repArquivos.Atualizar(arquivo, auditado);

                    unitOfWork.Start();

                    Servicos.Log.TratarErro($"Lendo arquivo {arquivo.Chave} {DateTime.Now.ToString("dd/MM/yyyy HH:mm:sss")} Codigo {codigoArquivoXml}", $"ProcessamentoArquivoXMLNotaFiscalIntegracao");

                    try
                    {

                        string caminhoArquivo = Utilidades.IO.FileStorageService.Storage.Combine(configuracaoArquivo.CaminhoArquivosImportacaoXMLNotaFiscal, arquivo.NomeArquivo);
#if DEBUG
                        caminhoArquivo = "C:\\Arquivos" + "\\" + arquivo.NomeArquivo;
#endif
                        if (!Utilidades.IO.FileStorageService.Storage.Exists(caminhoArquivo))
                        {
                            arquivo.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProcessamentoRegistro.Falha;
                            repArquivos.Atualizar(arquivo, auditado);
                            return;
                        }

                        System.IO.StreamReader stReaderXML = new StreamReader(Utilidades.IO.FileStorageService.Storage.OpenRead(caminhoArquivo));

                        string error = string.Empty;
                        Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal = null;
                        string xmlNota = stReaderXML.ReadToEnd();

                        Servicos.Log.TratarErro($"Leu arquivo {arquivo.Chave} {DateTime.Now.ToString("dd/MM/yyyy HH:mm:sss")} Codigo {codigoArquivoXml}", $"ProcessamentoArquivoXMLNotaFiscalIntegracao");

                        if (!servicoNfe.BuscarDadosNotaFiscal(out error, out xmlNotaFiscal, stReaderXML, unitOfWork, null, true, false, true, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador, false, false, null, null, auditado, configuracaoWebservice?.CadastroAutomaticoPessoaExterior ?? false))
                            mensagemErro = error.Substring(0, Math.Min(error.Length, 500));

                        stReaderXML.Close();

                        Servicos.Log.TratarErro($"Retorno leitura {arquivo.Chave} {DateTime.Now.ToString("dd/MM/yyyy HH:mm:sss")} Codigo {codigoArquivoXml} Retorno: {mensagemErro}", $"ProcessamentoArquivoXMLNotaFiscalIntegracao");

                        if (string.IsNullOrEmpty(mensagemErro))
                        {
                            if (xmlNotaFiscal == null)
                                mensagemErro = "Nota Fiscal Inexistente";

                            if (string.IsNullOrEmpty(mensagemErro))
                            {
                                if (xmlNotaFiscal.Codigo > 0)
                                    repositorioNotaFiscal.Atualizar(xmlNotaFiscal);
                                else
                                    repositorioNotaFiscal.Inserir(xmlNotaFiscal);

                                Servicos.Log.TratarErro($"XMLNotaFiscal Codigo {xmlNotaFiscal.Codigo} {DateTime.Now.ToString("dd/MM/yyyy HH:mm:sss")} Codigo {codigoArquivoXml} ", $"ProcessamentoArquivoXMLNotaFiscalIntegracao");

                                serCargaNotaFiscal.VincularXMLNotaFiscal(xmlNotaFiscal, configuracaoTMS, Program.TipoServicoMultisoftware, auditado, false, true);

                                Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedXmlNotaAdicionado = repositoriopedidoNotaFiscal.BuscarPorChave(xmlNotaFiscal.Chave);

                                if (pedXmlNotaAdicionado != null)
                                {
                                    arquivo.Carga = pedXmlNotaAdicionado.CargaPedido.Carga;
                                    arquivo.ProtocoloPedido = pedXmlNotaAdicionado.CargaPedido.Pedido?.Protocolo ?? 0;

                                    Servicos.Log.TratarErro($"Vinculado XMLNotaFiscal Parcial Encontrado {pedXmlNotaAdicionado.Codigo} chave {arquivo.Chave} {DateTime.Now.ToString("dd/MM/yyyy HH:mm:sss")} Codigo {codigoArquivoXml}", $"ProcessamentoArquivoXMLNotaFiscalIntegracao");
                                }

                                Servicos.Log.TratarErro($"ArmazenarProdutosXML {arquivo.Chave} {DateTime.Now.ToString("dd/MM/yyyy HH:mm:sss")} Codigo {codigoArquivoXml}", $"ProcessamentoArquivoXMLNotaFiscalIntegracao");
                                new Servicos.Embarcador.Pedido.PedidoXMLNotaFiscal(unitOfWork).ArmazenarProdutosXML(xmlNota, xmlNotaFiscal, auditado, Program.TipoServicoMultisoftware, null);
                            }
                        }

                        if (!string.IsNullOrEmpty(mensagemErro))
                            arquivo.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProcessamentoRegistro.Falha;
                        else
                            arquivo.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProcessamentoRegistro.Sucesso;

                        Servicos.Log.TratarErro($"Fechou arquivo {arquivo.Chave} {DateTime.Now.ToString("dd/MM/yyyy HH:mm:sss")} Codigo {codigoArquivoXml}", $"ProcessamentoArquivoXMLNotaFiscalIntegracao");

                        arquivo.Mensagem = mensagemErro;

                        repArquivos.Atualizar(arquivo, auditado);

                        if (xmlNotaFiscal != null)
                            Servicos.Log.TratarErro($"Arquivo Processado {xmlNotaFiscal.Chave} {DateTime.Now.ToString("dd/MM/yyyy HH:mm:sss")} Codigo {codigoArquivoXml}", $"ProcessamentoArquivoXMLNotaFiscalIntegracao");

                        unitOfWork.CommitChanges();
                        unitOfWork.FlushAndClear();
                    }
                    catch (System.Exception ex)
                    {
                        unitOfWork.Rollback();

                        unitOfWork.Start();

                        Servicos.Log.TratarErro($"Falha Arquivo Ex: {ex.ToString()} {DateTime.Now.ToString("dd/MM/yyyy HH:mm:sss")} Codigo {codigoArquivoXml}", $"ProcessamentoArquivoXMLNotaFiscalIntegracao");

                        //arquivo.Tentativas++;
                        arquivo.DataTentativa = DateTime.Now;
                        arquivo.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProcessamentoRegistro.Falha;
                        arquivo.Mensagem = ex.Message.Substring(0, Math.Min(ex.Message.Length, 500));

                        repArquivos.Atualizar(arquivo, auditado);

                        unitOfWork.CommitChanges();
                        unitOfWork.FlushAndClear();
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, _arquivoLog);
            }
            finally
            {
                lock (_ArquivosXmlEmProcessamento)
                {
                    if (_ArquivosXmlEmProcessamento.Any(o => o == codigoArquivoXml))
                        _ArquivosXmlEmProcessamento.Remove(codigoArquivoXml);

                    Servicos.Log.TratarErro($"Finalizou thread para geração de ProcessarArquivoXMLNotaFiscalIntegracao {codigoArquivoXml}.", _arquivoLog);
                }
            }
        }

        private void ProcessarArquivosXmlLiberacao(Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoArquivo configuracaoArquivo = new Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo(unitOfWork).BuscarPrimeiroRegistro();
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork).BuscarConfiguracaoPadrao();
            Repositorio.Embarcador.NotaFiscal.ArquivoXMLNotaFiscalIntegracao repArquivos = new Repositorio.Embarcador.NotaFiscal.ArquivoXMLNotaFiscalIntegracao(unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repositorioNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial repositorioCargaPedidoXmlParcial = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService repConfiguracaoWebService = new Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService(unitOfWork);
         
            Servicos.Embarcador.NFe.NFe servicoNfe = new Servicos.Embarcador.NFe.NFe(unitOfWork);
            Servicos.Embarcador.Pedido.NotaFiscal serCargaNotaFiscal = new Servicos.Embarcador.Pedido.NotaFiscal(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configuracaoWebservice = repConfiguracaoWebService.BuscarConfiguracaoPadrao();

            if (configuracaoArquivo == null)
                return;

            List<Dominio.Entidades.Embarcador.NotaFiscal.ArquivoXMLNotaFiscalIntegracao> arquivos = repArquivos.BuscarPendentesLiberacao(3, 100, 10);

            for (int i = 0; i < arquivos.Count; i++)
            {
                Dominio.Entidades.Embarcador.NotaFiscal.ArquivoXMLNotaFiscalIntegracao arquivo = arquivos[i];

                string mensagemErro = string.Empty;

                Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado()
                {
                    TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Integradoras,
                    Integradora = arquivo.Integradora,
                    IP = arquivo.IP
                };

                arquivo.Initialize();
                arquivo.TentativasLiberacao++;
                arquivo.DataTentativa = DateTime.Now;
                repArquivos.Atualizar(arquivo, auditado);

                unitOfWork.Start();

                Servicos.Log.TratarErro($"Lendo arquivo {arquivo.Chave} {DateTime.Now.ToString("dd/MM/yyyy HH:mm:sss")}", "ProcessamentoArquivoXMLNotaFiscalLiberacao");

                try
                {
                    string error = string.Empty;
                    string xmlNota = string.Empty;
                    Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal = repositorioNotaFiscal.BuscarPorChave(arquivo.Chave);

                    if (xmlNotaFiscal == null)
                    {
                        //System.IO.StreamReader stReaderXML = new StreamReader("C:\\Arquivos" + "\\" + arquivo.NomeArquivo);
                        System.IO.StreamReader stReaderXML = new StreamReader(Utilidades.IO.FileStorageService.Storage.OpenRead(Utilidades.IO.FileStorageService.Storage.Combine(configuracaoArquivo.CaminhoArquivosImportacaoXMLNotaFiscal, arquivo.NomeArquivo)));
                        xmlNota = stReaderXML.ReadToEnd();

                        Servicos.Log.TratarErro($"Leu arquivo {arquivo.Chave} {DateTime.Now.ToString("dd/MM/yyyy HH:mm:sss")}", "ProcessamentoArquivoXMLNotaFiscalLiberacao");

                        if (!servicoNfe.BuscarDadosNotaFiscal(out error, out xmlNotaFiscal, stReaderXML, unitOfWork, null, true, false, true, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador, configuracaoWebservice?.CadastroAutomaticoPessoaExterior ?? false))
                            mensagemErro = error.Substring(0, Math.Min(error.Length, 500));

                        if (xmlNotaFiscal != null)
                        {
                            if (xmlNotaFiscal.Codigo > 0)
                                repositorioNotaFiscal.Atualizar(xmlNotaFiscal);
                            else
                                repositorioNotaFiscal.Inserir(xmlNotaFiscal);
                        }

                        stReaderXML.Close();
                    }

                    if (xmlNotaFiscal == null)
                        mensagemErro = "Nota Fiscal Inexistente";

                    if (string.IsNullOrEmpty(mensagemErro))
                    {
                        //encontrar a carga pela nota, validar se a carga ainda nao avancou

                        Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial cargaPedidoXMLParcial = repositorioCargaPedidoXmlParcial.BuscarPorChaveNota(xmlNotaFiscal.Chave);

                        if (cargaPedidoXMLParcial != null)
                        {
                            if (cargaPedidoXMLParcial.XMLNotaFiscal == null)
                            {
                                serCargaNotaFiscal.VincularXMLNotaFiscal(xmlNotaFiscal, configuracaoTMS, Program.TipoServicoMultisoftware, auditado, false, true);

                                Servicos.Log.TratarErro($"VincularXMLNotaFiscal {arquivo.Chave} {DateTime.Now.ToString("dd/MM/yyyy HH:mm:sss")}", "ProcessamentoArquivoXMLNotaFiscalLiberacao");

                                arquivo.Carga = cargaPedidoXMLParcial.CargaPedido?.Carga ?? null;
                                arquivo.ProtocoloPedido = cargaPedidoXMLParcial.CargaPedido?.Pedido?.Protocolo ?? 0;

                                if (!string.IsNullOrEmpty(xmlNota))
                                {
                                    new Servicos.Embarcador.Pedido.PedidoXMLNotaFiscal(unitOfWork).ArmazenarProdutosXML(xmlNota, xmlNotaFiscal, auditado, Program.TipoServicoMultisoftware, null);

                                    Servicos.Log.TratarErro($"ArmazenarProdutosXML {arquivo.Chave} {DateTime.Now.ToString("dd/MM/yyyy HH:mm:sss")}", "ProcessamentoArquivoXMLNotaFiscalLiberacao");
                                }
                            }
                            else if (cargaPedidoXMLParcial.CargaPedido?.Carga?.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe)
                            {
                                serCargaNotaFiscal.VincularXMLNotaFiscal(xmlNotaFiscal, configuracaoTMS, Program.TipoServicoMultisoftware, auditado, false, true);

                                Servicos.Log.TratarErro($"VincularXMLNotaFiscal {arquivo.Chave} {DateTime.Now.ToString("dd/MM/yyyy HH:mm:sss")}", "ProcessamentoArquivoXMLNotaFiscalLiberacao");
                            }
                        }
                    }

                    unitOfWork.Flush();

                    if (string.IsNullOrEmpty(mensagemErro))
                    {
                        Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial cargaPedidoXMLParcialValidacao = repositorioCargaPedidoXmlParcial.BuscarPorChaveNota(xmlNotaFiscal.Chave);
                        if (cargaPedidoXMLParcialValidacao != null && cargaPedidoXMLParcialValidacao.XMLNotaFiscal != null)
                            arquivo.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProcessamentoRegistro.Liberado;
                        else
                            mensagemErro = "Nota Fiscal Integrada - Parcial Não encontrado";
                    }

                    if (arquivo.TentativasLiberacao > 4)
                        arquivo.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProcessamentoRegistro.FalhaLiberacao;

                    if (arquivo.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProcessamentoRegistro.Liberado)
                    {
                        string caminhoArquivo = Utilidades.IO.FileStorageService.Storage.Combine(configuracaoArquivo.CaminhoArquivosImportacaoXMLNotaFiscal, arquivo.NomeArquivo);
                        if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoArquivo))
                            Utilidades.IO.FileStorageService.Storage.Delete(caminhoArquivo);
                    }

                    Servicos.Log.TratarErro($"Fechou arquivo {arquivo.Chave} {DateTime.Now.ToString("dd/MM/yyyy HH:mm:sss")}", "ProcessamentoArquivoXMLNotaFiscalLiberacao");

                    arquivo.Mensagem = mensagemErro;
                    repArquivos.Atualizar(arquivo, auditado);

                    Servicos.Log.TratarErro($"Arquivo Processado {xmlNotaFiscal.Chave} {DateTime.Now.ToString("dd/MM/yyyy HH:mm:sss")}", "ProcessamentoArquivoXMLNotaFiscalLiberacao");

                    unitOfWork.CommitChanges();
                    unitOfWork.Flush();
                }
                catch (System.Exception ex)
                {
                    unitOfWork.Rollback();

                    unitOfWork.Start();

                    Servicos.Log.TratarErro($"Falha Arquivo Ex: {ex.ToString()} {DateTime.Now.ToString("dd/MM/yyyy HH:mm:sss")}", "ProcessamentoArquivoXMLNotaFiscalLiberacao");

                    arquivo.TentativasLiberacao++;
                    arquivo.DataTentativa = DateTime.Now;
                    mensagemErro = ex.Message.Substring(0, Math.Min(ex.Message.Length, 500));

                    if (arquivo.TentativasLiberacao >= 3)
                        arquivo.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProcessamentoRegistro.FalhaLiberacao;

                    arquivo.Mensagem = mensagemErro;

                    repArquivos.Atualizar(arquivo, auditado);

                    unitOfWork.CommitChanges();
                    unitOfWork.Flush();
                }
            }
        }

        private List<int> ObterArquivosXmlPendentes(UnitOfWork unitOfWork, int numeroThreads, int numeroTentativas, double minutosACadaTentativa)
        {
            Repositorio.Embarcador.NotaFiscal.ArquivoXMLNotaFiscalIntegracao repArquivos = new Repositorio.Embarcador.NotaFiscal.ArquivoXMLNotaFiscalIntegracao(unitOfWork);
            List<int> codigosPendentes = repArquivos.BuscarCodigosPendentes(numeroTentativas, numeroThreads, minutosACadaTentativa);

            List<int> codigosSemThread = new List<int>();
            lock (_ArquivosXmlEmProcessamento)
            {
                foreach (var codigo in codigosPendentes)
                    if (!_ArquivosXmlEmProcessamento.Any(o => o == codigo))
                        codigosSemThread.Add(codigo);
            }

#if DEBUG
            List<int> codigosArquivos = new List<int>() { /*Adicionar códigos aqui para debug*/ };
            if (codigosArquivos.Count > 0)
                codigosSemThread = codigosArquivos;
#endif

            return codigosSemThread;
        }

        private List<int> ObterArquivosDadosXmlPendentes(UnitOfWork unitOfWork, int numeroThreads)
        {
            Repositorio.Embarcador.NotaFiscal.ProcessamentoNotaFiscalAssincrono repProcessamentoNotaArquivos = new Repositorio.Embarcador.NotaFiscal.ProcessamentoNotaFiscalAssincrono(unitOfWork);
            List<int> codigosPendentes = repProcessamentoNotaArquivos.BuscarCodigosPendentes(3, numeroThreads);

            List<int> codigosSemThread = new List<int>();
            lock (_ArquivosXmlEmProcessamento)
            {
                foreach (var codigo in codigosPendentes)
                    if (!_ArquivosXmlEmProcessamento.Any(o => o == codigo))
                        codigosSemThread.Add(codigo);
            }

#if DEBUG
            List<int> codigosArquivos = new List<int>() { /*Adicionar códigos aqui para debug*/ };
            if (codigosArquivos.Count > 0)
                codigosSemThread = codigosArquivos;
#endif

            return codigosSemThread;
        }

        private ConfiguracaoThread ObterConfiguracaoThread(string nome)
        {
            return _configuracoes.Where(x => x.Nome == nome).FirstOrDefault();
        }


        protected override void OnStop()
        {
            if (_threadArquivosXmlNotaFiscaisPendentes != null)
            {
                _threadArquivosXmlNotaFiscaisPendentes.Abort();
                _threadArquivosXmlNotaFiscaisPendentes = null;
            }

            if (_threadArquivosXmlNotaFiscaisPendentes != null)
            {
                _threadsGeracaoArquivosXmlNotaFiscais.ForEach(thread => thread.Abort());
                _threadsGeracaoArquivosXmlNotaFiscais = null;
            }
            GC.Collect();
            Servicos.Log.TratarErro("Serviço parado.");
        }
    }
}
