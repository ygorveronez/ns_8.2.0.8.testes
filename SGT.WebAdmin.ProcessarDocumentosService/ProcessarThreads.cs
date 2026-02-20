using Repositorio;
using Servicos.Embarcador.EmissorDocumento;
using SGT.WebAdmin.ProcessarDocumentosService.Thread;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;

namespace SGT.WebAdmin.ProcessarDocumentosService
{
    public partial class ProcessarThreads : ServiceBase
    {
        private List<int> _cargasEmProcessamento;
        private List<System.Threading.Thread> _threadsGeracaoCargas;
        private System.Threading.Thread _threadCargasPendentes;
        private static string _arquivoLog = "Logs";
        private List<ConfiguracaoThread> _configuracoes;



        public ProcessarThreads()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Servicos.Log.TratarErro("Serviço iniciado.");
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

            #region ConfiguraTreadCargasPendentes
            _cargasEmProcessamento = new List<int>();
            _threadsGeracaoCargas = new List<System.Threading.Thread>();
            try
            { // INICIALIZA _threadConsultaCargasPendentes

                int Id = Int16.Parse(Program.appSettings["ThreadCargasPendentes:Id"]);
                string Nome = Program.appSettings["ThreadCargasPendentes:Nome"];
                bool Ativa = bool.Parse(Program.appSettings["ThreadCargasPendentes:Ativa"]);
                int NumeroTheadsParalelas = int.Parse(Program.appSettings["ThreadCargasPendentes:NumeroTheadsParalelas"]);
                bool utilizarInsertEmMassa = bool.Parse(Program.appSettings["ThreadCargasPendentes:UtilizarInsertEmMassa"]);
                _configuracoes.Add(new ConfiguracaoThread(Id, Nome, Ativa, NumeroTheadsParalelas, utilizarInsertEmMassa));
                if (_threadCargasPendentes == null)
                {
                    _threadCargasPendentes = new System.Threading.Thread(ThreadsCargasPendentesStart);
                    _threadCargasPendentes.Start();
                }
            }
            catch (Exception e)
            {
                Servicos.Log.TratarErro(e, _arquivoLog);
                throw;
            }
            #endregion

        }

        private void ThreadsCargasPendentesStart()
        {
            while (true)
            {
                GC.Collect();
                try
                {
                    using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Program.StringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
                    {

                        Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork);
                        var configuracao = ObterConfiguracaoThread("ThreadCargasPendentes");
                        if (!(configuracao?.Ativa ?? false))
                        {
                            Servicos.Log.TratarErro("Serviço ThreadCargasPendentes desabilitado, aguardando 10 minutos para nova execução.", _arquivoLog);
                            System.Threading.Thread.Sleep(600000);
                            continue;
                        }

                        int quantidadeThreadsDisponiveis = 0;
                        int quantidadeThreadsAtivas = _threadsGeracaoCargas.Count;

                        List<int> threadsRemover = new List<int>();
                        for (int i = quantidadeThreadsAtivas - 1; i >= 0; i--)
                        {
                            if (_threadsGeracaoCargas[i].IsAlive)
                                continue;
                            _threadsGeracaoCargas.RemoveAt(i);
                        }
                        quantidadeThreadsDisponiveis = configuracao.NumeroTheadsParalelas - _threadsGeracaoCargas.Count;
                        if (quantidadeThreadsDisponiveis <= 0)
                        {
                            Servicos.Log.TratarErro($"Já existem {_threadsGeracaoCargas.Count} threads ativas. Aguardando 10 segundos para nova verificação.", _arquivoLog);
                            System.Threading.Thread.Sleep(1000);
                            continue;
                        }

                        List<int> cargasPendentesGeracao = ObterCargasPendentes(unitOfWork);

                        if (cargasPendentesGeracao.Count <= 0)
                        {
                            Servicos.Log.TratarErro($"Não existem cargas pendentes para geração. Aguardando 10 segundos para nova verificação. {_threadsGeracaoCargas.Count} de {configuracao.NumeroTheadsParalelas} threads em execução.", _arquivoLog);
                            System.Threading.Thread.Sleep(1000);
                            continue;
                        }

                        for (int i = 0; i < quantidadeThreadsDisponiveis && i < cargasPendentesGeracao.Count; i++)
                        {
                            int codigoCargaPendenteGeracao = cargasPendentesGeracao[i];
                            System.Threading.Thread thread = new System.Threading.Thread(() => GerarCarga(codigoCargaPendenteGeracao));
                            thread.Start();
                            _threadsGeracaoCargas.Add(thread);
                        }
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

        private void GerarCarga(int codigoCargaPendenteGeracao)
        {
            try
            {
                lock (_cargasEmProcessamento)
                {
                    if (!_cargasEmProcessamento.Any(o => o == codigoCargaPendenteGeracao))
                        _cargasEmProcessamento.Add(codigoCargaPendenteGeracao);
                    else
                        return;
                    Servicos.Log.TratarErro($"Iniciou thread para geração de cargas pendentes {codigoCargaPendenteGeracao}.", _arquivoLog);
                }

                using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Program.StringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
                {

                    Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                    Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                    Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();

                    Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCargaPendenteGeracao);

                    carga.PosicaoFilaProcessamento++;

                    repCarga.Atualizar(carga);

                        new Servicos.Embarcador.Carga.Carga(unitOfWork).ConfirmarEnvioDosDocumentos(carga, unitOfWork, Program.StringConexao, Program.TipoServicoMultisoftware, configuracao, Program.Auditado, Program.Cliente, ObterConfiguracaoThread("ThreadCargasPendentes").UtilizarInsertEmMassa);

                    unitOfWork.FlushAndClear();
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, _arquivoLog);
            }
            finally
            {
                lock (_cargasEmProcessamento)
                {
                    if (_cargasEmProcessamento.Any(o => o == codigoCargaPendenteGeracao))
                        _cargasEmProcessamento.Remove(codigoCargaPendenteGeracao);

                    Servicos.Log.TratarErro($"Finalizou thread para geração de cargas pendentes {codigoCargaPendenteGeracao}.", _arquivoLog);
                }
            }
        }

        private List<int> ObterCargasPendentes(UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            List<int> codigosCargaPendentes = repCarga.BuscarCargasEmProcessamentoDocumentosFiscais(false);
            List<int> codigosSemThread = new List<int>();

            lock (_cargasEmProcessamento)
            {
                foreach (var carga in codigosCargaPendentes)
                    if (!_cargasEmProcessamento.Any(o => o == carga))
                        codigosSemThread.Add(carga);
            }
            return codigosSemThread;
        }

        private ConfiguracaoThread ObterConfiguracaoThread(string nome)
        {
            return _configuracoes.Find(x => x.Nome == nome);
        }


        protected override void OnStop()
        {
            if (_threadCargasPendentes != null)
            {
                _threadCargasPendentes.Abort();
                _threadCargasPendentes = null;
            }

            if (_threadsGeracaoCargas != null)
            {
                _threadsGeracaoCargas.ForEach(thread => thread.Abort());
                _threadsGeracaoCargas = null;
            }
            GC.Collect();
            Servicos.Log.TratarErro("Serviço parado.");
        }
    }
}
