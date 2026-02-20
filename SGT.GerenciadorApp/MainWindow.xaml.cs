using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;

namespace SGT.GerenciadorApp
{
    public partial class MainWindow : Window
    {
        #region Atributos públicos

        public static ReaderWriterLock locker = new ReaderWriterLock();

        #endregion

        #region Atributos privados apenas leitura

        private static readonly string stringConexaoDebug = "";

        #endregion

        #region Construtores

        public MainWindow()
        {
            InitializeComponent();
            SetarAppSystemTray();
           
            if (Habilitado())
            {
                SetarExecucaoThreads();
                WindowState = WindowState.Minimized;
                Hide();
            }
            else
            {
                this.Close();
            }
        }

        #endregion

        #region Manipuladores de Eventos

        private void Configuracoes_Click(object sender, RoutedEventArgs e)
        {
            Configuracoes configuracoesWindow = new Configuracoes();
            ConfiguracaoEBSViewModel configViewModel = new ConfiguracaoEBSViewModel(configuracoesWindow);

            configuracoesWindow.ShowInTaskbar = false;
            configuracoesWindow.Owner = Application.Current.MainWindow;
            configuracoesWindow.DataContext = configViewModel;
            configuracoesWindow.ShowDialog();
        }

        private void Sair_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Reiniciar_Click(object sender, RoutedEventArgs e)
        {
            ReiniciarAplicacao();
        }

        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowState == System.Windows.WindowState.Minimized)
                this.Hide();

            base.OnStateChanged(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            if (App.NotificationIcon != null)
            {
                App.NotificationIcon.Dispose();
                App.NotificationIcon = null;
            }
        }

        #endregion

        #region Metodos privados

        private bool Habilitado()
        {
            bool enable = false;
            try
            {
                enable = bool.Parse(ConfigurationManager.AppSettings["Enable"]);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao ler configuração Enable do GerenciadorApp: {ex.ToString()}", "CatchNoAction");
            }
            string msg = "GerenciadorApp " + ((enable) ? "ativado! Iniciando ..." : "desativado");
            Servicos.Log.TratarErro(msg);
            return enable;
        }

        private void SetarExecucaoThreads()
        {

            try
            {
                string ambiente = ConfigurationManager.AppSettings["Ambiente"].Trim().ToUpper();
                string[] codigosClientes = ConfigurationManager.AppSettings["CodigoClienteMultisoftware"].Split(',');
                var codigoCliente = codigosClientes.FirstOrDefault();

#if DEBUG
                //setar aqui a thread para debug
                SetarThreadConsultaNFSeDestinadas(ambiente);
#else
                SetarThreadImportacaoCTF(ambiente);
                SetarThreadConsultaNFSeDestinadas(ambiente);
                SetarThreadConsultaDocumentosDestinados(ambiente);
                SetarThreadConsultaCTeDestinados(ambiente);//Multi-NFe
                SetarThreadConsultaMDFeDestinados(ambiente);
                SetarThreadConsultaSituacaoMDFe(ambiente);
#endif


                //SetarThreadEBS();
                //SetarThreadEnvioEmailEncerramentoMDFe();
                //SetarThreadImportacaoAbastecimentoInterno();
                //SetarThreadIntegracaoDocumentosDestinados();//Multi-NFe

                //SetarThreadConsultaXMLEmail();//Multi-NFe
                //ProcessarXMLCTEs();//Para importar os XML para Danone

                //ControleMonitoramento.IniciarMonitoramentoLogistica(ambiente, ObterStringConexao(ambiente, codigoCliente));
                //SetarThreadAverbacaoATM();


            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
            finally
            {
                GC.Collect();
            }
        }

        private void SetarThreadEBS()
        {
            //essa thread é específica para a tombini, não sendo necessária ser multiclientes

            string[] codigosClientes = ConfigurationManager.AppSettings["CodigoClienteMultisoftware"].Split(',');

            int.TryParse(codigosClientes[0], out int codigoClienteMultisoftware);

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(App.StringConexaoAdmin);
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(App.StringConexao(codigoClienteMultisoftware, unitOfWorkAdmin));

            try
            {
                Repositorio.Embarcador.Configuracoes.EBS repConfiguracaoEBS = new Repositorio.Embarcador.Configuracoes.EBS(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.EBS configuracaoEBS = repConfiguracaoEBS.Buscar();

                if (configuracaoEBS != null)
                {

                    DateTime dataExecucao = DateTime.Now;

                    if (configuracaoEBS.HoraExecucao < DateTime.Now.TimeOfDay)
                        dataExecucao = DateTime.Now.AddDays(1);

                    dataExecucao = new DateTime(dataExecucao.Year, dataExecucao.Month, dataExecucao.Day, configuracaoEBS.HoraExecucao.Hours, configuracaoEBS.HoraExecucao.Minutes, 0);

                    TimeSpan timeToGo = dataExecucao - DateTime.Now;

#if DEBUG
                    timeToGo = new TimeSpan(0, 0, 10);
#endif

                    App.ThreadExecucaoEBS = new System.Threading.Timer(x =>
                    {
                        GerarEBS();
                    }, null, timeToGo, Timeout.InfiniteTimeSpan);
                }

                repConfiguracaoEBS = null;
                configuracaoEBS = null;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
            finally
            {
                unitOfWork.Dispose();
                unitOfWorkAdmin.Dispose();
            }
        }

        private void SetarThreadEnvioEmailEncerramentoMDFe()
        {
            try
            {
                string hora = ConfigurationManager.AppSettings["HoraEnvioEmailMDFe"];

                if (!string.IsNullOrWhiteSpace(hora))
                {
                    TimeSpan horaExecucao = new TimeSpan();
                    TimeSpan.TryParseExact(hora, "g", null, out horaExecucao);

                    DateTime dataExecucao = DateTime.Now;

                    if (horaExecucao < DateTime.Now.TimeOfDay)
                        dataExecucao = DateTime.Now.AddDays(1);

                    dataExecucao = new DateTime(dataExecucao.Year, dataExecucao.Month, dataExecucao.Day, horaExecucao.Hours, horaExecucao.Minutes, 0);

                    TimeSpan timeToGo = dataExecucao - DateTime.Now;

#if DEBUG
                    timeToGo = new TimeSpan(0, 0, 10);
#endif

                    App.ThreadEnvioEmailEncerramentoMDFe = new System.Threading.Timer(x =>
                    {
                        EnviarEmailsEncerramentoMDFe();
                    }, null, timeToGo, Timeout.InfiniteTimeSpan);

                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
        }

        private void SetarThreadConsultaXMLEmail()
        {
            try
            {
                TimeSpan timeToGo = new TimeSpan(0, 0, 60); //daqui a 5 segundos começa a execução

                App.ThreadConsultaXMLEmail = new System.Threading.Timer(x =>
                {
                    BuscarXMLEmail();
                }, null, timeToGo, new TimeSpan(0, 10, 0));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
        }

        private void SetarThreadIntegracaoDocumentosDestinados()
        {
            try
            {
                TimeSpan timeToGo = new TimeSpan(0, 0, 30); //daqui a 5 segundos começa a execução

                App.ThreadIntegracaoDocumentosDestinados = new System.Threading.Timer(x =>
                {
                    BuscarIntegracaoDocumentosDestinados();
                }, null, timeToGo, new TimeSpan(0, 30, 0));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
        }

        private void BuscarXMLEmail()
        {
            Servicos.Log.TratarErro("Inicio BuscarXMLEmail", "XMLEmail");
            string[] codigosClientes = ConfigurationManager.AppSettings["CodigoClienteMultisoftware"].Split(',');

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(App.StringConexaoAdmin);

            foreach (string codigoCliente in codigosClientes)
            {
                int.TryParse(codigoCliente, out int codigoClienteMultisoftware);
                string stringConexao = App.StringConexao(codigoClienteMultisoftware, unitOfWorkAdmin);

                Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);

                try
                {
                    AdminMultisoftware.Repositorio.Pessoas.Cliente repCliente = new AdminMultisoftware.Repositorio.Pessoas.Cliente(unitOfWorkAdmin);
                    AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente = repCliente.BuscarPorCodigo(codigoClienteMultisoftware);
                    Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                    Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);

                    if (cliente == null || cliente.ClienteConfiguracao == null)
                    {
                        Servicos.Log.TratarErro("Nenhum cliente encontrado.", "XMLEmail");
                        return;
                    }

                    if (cliente.ClienteConfiguracao.TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe && cliente.ClienteConfiguracao.TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFeAdmin)
                    {
                        Servicos.Log.TratarErro("Não é Multi NFe. " + cliente.ClienteConfiguracao.TipoServicoMultisoftware.ToString(), "XMLEmail");
                        return;
                    }

                    List<int> codigosEmpresas = new List<int>();
                    codigosEmpresas = repEmpresa.BuscarCodigosEmpresasAtivasIntegracaoEmail();
                    Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado = null;
                    foreach (int codigoEmpresa in codigosEmpresas)
                    {
                        Servicos.Log.TratarErro("Inicio empresa " + codigoEmpresa.ToString(), "XMLEmail");
                        Auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado();
                        Auditado.Empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);
                        Auditado.Integradora = null;
                        Auditado.IP = "";
                        Auditado.Texto = "";
                        Auditado.TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema;
                        Auditado.Usuario = null;

                        Servicos.Embarcador.Documentos.DocumentoDestinadoEmpresa.VerificarCaixaDeEntrada(codigoEmpresa, stringConexao, out string msgErro, Auditado, cliente.ClienteConfiguracao.TipoServicoMultisoftware);
                        Servicos.Embarcador.Documentos.DocumentoDestinadoEmpresa.VerificarEmails(codigoEmpresa, stringConexao, out string msgErro2, Auditado, cliente.ClienteConfiguracao.TipoServicoMultisoftware);
                    }
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex, "XMLEmail");
                }
                finally
                {
                    Servicos.Log.TratarErro("Fim BuscarXMLEmail", "XMLEmail");
                    unitOfWork.Dispose();
                    unitOfWork = null;
                    GC.Collect();
                }
            }
            unitOfWorkAdmin.Dispose();
        }

        private void BuscarIntegracaoDocumentosDestinados()
        {
            string[] codigosClientes = ConfigurationManager.AppSettings["CodigoClienteMultisoftware"].Split(',');

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(App.StringConexaoAdmin);

            foreach (string codigoCliente in codigosClientes)
            {
                int.TryParse(codigoCliente, out int codigoClienteMultisoftware);
                string stringConexao = App.StringConexao(codigoClienteMultisoftware, unitOfWorkAdmin);

                Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);

                try
                {
                    AdminMultisoftware.Repositorio.Pessoas.Cliente repCliente = new AdminMultisoftware.Repositorio.Pessoas.Cliente(unitOfWorkAdmin);
                    AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente = repCliente.BuscarPorCodigo(codigoClienteMultisoftware);
                    Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                    Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);

                    if (cliente == null || cliente.ClienteConfiguracao == null)
                    {
                        Servicos.Log.TratarErro("Nenhum cliente encontrado.", "IntegracaoDocumentosDestinados");
                        return;
                    }

                    if (cliente.ClienteConfiguracao.TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe && cliente.ClienteConfiguracao.TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFeAdmin)
                    {
                        Servicos.Log.TratarErro("Não é Multi NFe. " + cliente.ClienteConfiguracao.TipoServicoMultisoftware.ToString(), "IntegracaoDocumentosDestinados");
                        return;
                    }

                    List<int> codigosEmpresas = new List<int>();
                    codigosEmpresas = repEmpresa.BuscarCodigosEmpresasAtivasIntegracaoDocumentosDestinados();
                    Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado = null;
                    foreach (int codigoEmpresa in codigosEmpresas)
                    {
                        Servicos.Log.TratarErro("Inicio empresa: " + codigoEmpresa.ToString(), "IntegracaoDocumentosDestinados");

                        Auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado();
                        Auditado.Empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);
                        Auditado.Integradora = null;
                        Auditado.IP = "";
                        Auditado.Texto = "";
                        Auditado.TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema;
                        Auditado.Usuario = null;

                        if (!Servicos.Embarcador.Documentos.DocumentoDestinadoEmpresa.EnviarIMPUTDocumentosDestinadosEmpresa(codigoEmpresa, stringConexao, out string msgErro, Auditado))
                            Servicos.Log.TratarErro("Erro Enviar IMPUT: " + msgErro, "IntegracaoDocumentosDestinados");
                        if (!Servicos.Embarcador.Documentos.DocumentoDestinadoEmpresa.ProcessarOUTPUTDocumentosDestinadosEmpresa(codigoEmpresa, stringConexao, out string msgErroOUTPUT, Auditado, cliente.ClienteConfiguracao.TipoServicoMultisoftware))
                            Servicos.Log.TratarErro("Erro Processar OUTPUT: " + msgErroOUTPUT, "IntegracaoDocumentosDestinados");
                        if (!Servicos.Embarcador.Documentos.DocumentoDestinadoEmpresa.ProcessarXMLDocumentosDestinadosEmpresa(codigoEmpresa, stringConexao, out string msgErroOUTPUTImp, Auditado, cliente.ClienteConfiguracao.TipoServicoMultisoftware))
                            Servicos.Log.TratarErro("Erro Processar OUTPUT: " + msgErroOUTPUTImp, "IntegracaoDocumentosDestinados");
                    }
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex, "IntegracaoDocumentosDestinados");
                }
                finally
                {
                    unitOfWork.Dispose();
                    unitOfWork = null;
                    GC.Collect();
                }
            }
            unitOfWorkAdmin.Dispose();
        }

        private void SetarThreadConsultaMDFeDestinados(string ambiente)
        {
            bool enable = false;
            try
            {
                var configSettings = ConfigurationManager.GetSection("ConsultaMDFeDestinados") as NameValueCollection;
                enable = bool.Parse(configSettings["Enable"]);
            }
            catch (Exception e)
            {
                Servicos.Log.TratarErro(e);
            }

            if (enable)
            {
                try
                {
                    TimeSpan timeToGo = new TimeSpan(0, 0, 9); //daqui a 5 segundos começa a execução

                    App.ThreadConsultaMDFeDestinados = new System.Threading.Timer(x =>
                    {
                        BuscarMDFesDestinados(ambiente);
                    }, null, timeToGo, new TimeSpan(0, 25, 0));
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                }
            }
        }

        private void SetarThreadConsultaCTeDestinados(string ambiente)
        {
            bool enable = false;
            try
            {
                var configSettings = ConfigurationManager.GetSection("ConsultaCTeDestinados") as NameValueCollection;
                enable = bool.Parse(configSettings["Enable"]);
            }
            catch (Exception e)
            {
                Servicos.Log.TratarErro(e);
            }

            if (enable)
            {
                try
                {
                    TimeSpan timeToGo = new TimeSpan(0, 0, 5); //daqui a 5 segundos começa a execução

                    App.ThreadConsultaCTeDestinados = new System.Threading.Timer(x =>
                    {
                        BuscarCTEsDestinados(ambiente);
                    }, null, timeToGo, new TimeSpan(0, 60, 0));
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                }
            }
        }

        private void SetarThreadAverbacaoATM()
        {
            bool enable = false;
            int tempoSleepMinutos = 60;

            try
            {
                var configSettings = ConfigurationManager.GetSection("AverbacaoATM") as NameValueCollection;
                enable = configSettings["Enable"]?.ToBool() ?? false;
                tempoSleepMinutos = configSettings["TempoSleepThread"]?.ToInt() ?? 60;
            }
            catch (Exception e)
            {
                Servicos.Log.TratarErro(e);
            }

            if (enable)
            {
                try
                {
                    TimeSpan timeToGo = new TimeSpan(0, 0, 5); //daqui a 5 segundos começa a execução

                    App.ThreadAverbacaoATM = new System.Threading.Timer(x =>
                    {
                        AverbacaoATM();
                    }, null, timeToGo, new TimeSpan(0, tempoSleepMinutos, 0));
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                }
            }
        }

        private void SetarThreadConsultaNFSeDestinadas(string ambiente)
        {
            bool enable = false;
            int tempoSleepMinutos = 60;            
            try
            {
                var configSettings = ConfigurationManager.GetSection("ConsultaNFSeDestinadasSaoPauloSP") as NameValueCollection;
                if (configSettings != null)
                {
                    enable = configSettings["Enable"]?.ToBool() ?? false;
                    tempoSleepMinutos = configSettings["TempoSleepThread"]?.ToInt() ?? 60;
                }   
            }
            catch (Exception e)
            {
                Servicos.Log.TratarErro(e);
            }

            if (enable)
            {
                try
                {
                    TimeSpan timeToGo = new TimeSpan(0, 0, 5); //daqui a 5 segundos começa a execução

                    App.ThreadConsultaNFSeDestinadasSaoPauloSP = new System.Threading.Timer(x =>
                    {
                        BuscarNFSeDestinadasSaoPauloSP(ambiente);
                    }, null, timeToGo, new TimeSpan(0, tempoSleepMinutos, 0));
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                }
            }
        }

        private void SetarThreadConsultaDocumentosDestinados(string ambiente)
        {
            bool enable = false;
            int tempoSleepMinutos = 60;
            try
            {
                var configSettings = ConfigurationManager.GetSection("ConsultaDocumentosDestinados") as NameValueCollection;
                enable = configSettings["Enable"]?.ToBool() ?? false;
                tempoSleepMinutos = configSettings["TempoSleepThread"]?.ToInt() ?? 60;
            }
            catch (Exception e)
            {
                Servicos.Log.TratarErro(e);
            }

            if (enable)
            {
                try
                {
                    TimeSpan timeToGo = new TimeSpan(0, 0, 5); //daqui a 5 segundos começa a execução

                    App.ThreadConsultaDocumentosDestinados = new System.Threading.Timer(x =>
                    {
                        BuscarDocumentosDestinados(ambiente);
                    }, null, timeToGo, new TimeSpan(0, tempoSleepMinutos, 0));
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                }
            }
        }

        private void SetarThreadImportacaoCTF(string ambiente)
        {
            try
            {
                TimeSpan timeToGo = new TimeSpan(0, 0, 30); //daqui a 5 segundos começa a execução

                App.ThreadImportacaoCTF = new System.Threading.Timer(x =>
                {
                    BuscarIntegracaoCTF(ambiente);
                }, null, timeToGo, new TimeSpan(0, 60, 0));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
        }

        private void ProcessarXMLCTEs()
        {
            string[] codigosClientes = ConfigurationManager.AppSettings["CodigoClienteMultisoftware"].Split(',');

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(App.StringConexaoAdmin);

            foreach (string codigoCliente in codigosClientes)
            {
                int.TryParse(codigoCliente, out int codigoClienteMultisoftware);

                string stringConexao = App.StringConexao(codigoClienteMultisoftware, unitOfWorkAdmin);

                try
                {
                    Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);

                    AdminMultisoftware.Repositorio.Pessoas.Cliente repCliente = new AdminMultisoftware.Repositorio.Pessoas.Cliente(unitOfWorkAdmin);
                    Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                    Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);

                    AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente = repCliente.BuscarPorCodigo(codigoClienteMultisoftware);

                    if (cliente == null || cliente.ClienteConfiguracao == null)
                    {
                        Servicos.Log.TratarErro("Nenhum cliente encontrado.");
                        return;
                    }

                    if (cliente.ClienteConfiguracao.TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador && cliente.ClienteConfiguracao.TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && cliente.ClienteConfiguracao.TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe && cliente.ClienteConfiguracao.TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFeAdmin)
                    {
                        Servicos.Log.TratarErro("Não é TMS, Multi NFe e Multiembarcador. " + cliente.ClienteConfiguracao.TipoServicoMultisoftware.ToString());
                        return;
                    }

                    Dominio.Entidades.Empresa empresa = repEmpresa.BuscarEmpresaPai();

                    if (empresa == null)
                    {
                        Servicos.Log.TratarErro("Empresa Pai não configurada.");
                    }

                    string mensagemRetorno = string.Empty;

                    if (!Servicos.Embarcador.Documentos.DocumentoDestinadoEmpresa.ProcessarXMLDocumentosDestinadosCTeEmpresa(empresa.Codigo, stringConexao, ref mensagemRetorno, null, cliente.ClienteConfiguracao.TipoServicoMultisoftware, unitOfWork))
                    {
                        Servicos.Log.TratarErro("Não foi possível processar os arquivos XML: " + mensagemRetorno);
                        return;
                    }

                    unitOfWork.Dispose();
                    unitOfWork = null;
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                }
                finally
                {
                    GC.Collect();
                }
            }
            
            unitOfWorkAdmin.Dispose();
            unitOfWorkAdmin = null;
            GC.Collect();
        }

        private void BuscarMDFesDestinados(string ambiente)
        {
            string[] codigosClientes = ConfigurationManager.AppSettings["CodigoClienteMultisoftware"].Split(',');

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(App.StringConexaoAdmin);

            foreach (string codigoCliente in codigosClientes)
            {
                try
                {
                    string stringConexao = ObterStringConexao(ambiente, codigoCliente);

                    int.TryParse(codigoCliente, out int codigoClienteMultisoftware);

                    Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);

                    AdminMultisoftware.Repositorio.Pessoas.Cliente repCliente = new AdminMultisoftware.Repositorio.Pessoas.Cliente(unitOfWorkAdmin);
                    Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                    Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);

                    AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente = repCliente.BuscarPorCodigo(codigoClienteMultisoftware);

                    if (cliente == null || cliente.ClienteConfiguracao == null)
                    {
                        Servicos.Log.TratarErro("Nenhum cliente encontrado.");
                        return;
                    }

                    if (cliente.ClienteConfiguracao.TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador && cliente.ClienteConfiguracao.TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && cliente.ClienteConfiguracao.TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe && cliente.ClienteConfiguracao.TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFeAdmin)
                    {
                        Servicos.Log.TratarErro("Não é TMS, Multi NFe e Multiembarcador. " + cliente.ClienteConfiguracao.TipoServicoMultisoftware.ToString());
                        return;
                    }
                    List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas> grupoPessoas = repGrupoPessoas.BuscarQueNaoImportaDocumentoDestinadoTransporte();
                    List<string> cnpjsNaoImportar = (from obj in grupoPessoas select obj.Clientes.Select(o => o.CPF_CNPJ_SemFormato)).SelectMany(o => o).ToList();
                    List<int> codigosEmpresas = new List<int>();
                    if (cliente.ClienteConfiguracao.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador || cliente.ClienteConfiguracao.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe || cliente.ClienteConfiguracao.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFeAdmin)
                        codigosEmpresas = repEmpresa.BuscarCodigosEmpresasAtivasSincronismoDocumentosDestinados();
                    else
                        codigosEmpresas = repEmpresa.BuscarCodigosEmpresasAtivas(cnpjsNaoImportar);

                    foreach (int codigoEmpresa in codigosEmpresas)
                    {
                        Servicos.Log.TratarErro("Buscando empresa " + codigoEmpresa.ToString());
                        string msgErro = string.Empty;
                        if (!Servicos.Embarcador.Documentos.DocumentoDestinadoEmpresa.ObterDocumentosDestinadosMDFeEmpresa(codigoEmpresa, string.Empty, stringConexao, 0, ref msgErro, cliente.ClienteConfiguracao.CaminhoDocumentosFiscais, cliente.ClienteConfiguracao.TipoServicoMultisoftware))
                        {
                            Servicos.Log.TratarErro("Erro consultando documentos MDF-e: " + msgErro);
                        }
                    }

                    unitOfWork.Dispose();
                    unitOfWork = null;
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                }
                finally
                {

                    GC.Collect();
                }
            }

            unitOfWorkAdmin.Dispose();
            unitOfWorkAdmin = null;
            GC.Collect();
        }

        private void BuscarCTEsDestinados(string ambiente)
        {
            string[] codigosClientes = ConfigurationManager.AppSettings["CodigoClienteMultisoftware"].Split(',');

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(App.StringConexaoAdmin);

            foreach (string codigoCliente in codigosClientes)
            {
                try
                {
                    string stringConexao = ObterStringConexao(ambiente, codigoCliente);

                    int.TryParse(codigoCliente, out int codigoClienteMultisoftware);

                    Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);

                    AdminMultisoftware.Repositorio.Pessoas.Cliente repCliente = new AdminMultisoftware.Repositorio.Pessoas.Cliente(unitOfWorkAdmin);
                    Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                    Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);

                    AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente = repCliente.BuscarPorCodigo(codigoClienteMultisoftware);

                    if (cliente == null || cliente.ClienteConfiguracao == null)
                    {
                        Servicos.Log.TratarErro("Nenhum cliente encontrado.", "BuscarCTEsDestinados");
                        return;
                    }

                    if (cliente.ClienteConfiguracao.TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador && cliente.ClienteConfiguracao.TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && cliente.ClienteConfiguracao.TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe && cliente.ClienteConfiguracao.TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFeAdmin)
                    {
                        Servicos.Log.TratarErro(cliente.CNPJ.ToString() + " não é TMS, Multi NFe e Multiembarcador. Tipo:" + cliente.ClienteConfiguracao.TipoServicoMultisoftware.ToString(), "BuscarCTEsDestinados");
                        return;
                    }
                    List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas> grupoPessoas = repGrupoPessoas.BuscarQueNaoImportaDocumentoDestinadoTransporte();
                    List<string> cnpjsNaoImportar = (from obj in grupoPessoas select obj.Clientes.Select(o => o.CPF_CNPJ_SemFormato)).SelectMany(o => o).ToList();
                    List<int> codigosEmpresas = new List<int>();
                    if (cliente.ClienteConfiguracao.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador || cliente.ClienteConfiguracao.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe || cliente.ClienteConfiguracao.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFeAdmin)
                        codigosEmpresas = repEmpresa.BuscarCodigosEmpresasAtivasSincronismoDocumentosDestinados();
                    else
                        codigosEmpresas = repEmpresa.BuscarCodigosEmpresasAtivas(cnpjsNaoImportar);
                                        
                    foreach (int codigoEmpresa in codigosEmpresas)
                    {
                        try
                        {
                            Servicos.Log.TratarErro("Buscando empresa " + codigoEmpresa.ToString(), "BuscarCTEsDestinados");
                            string msgErro = string.Empty;

                            if (!Servicos.Embarcador.Documentos.DocumentoDestinadoEmpresa.ObterDocumentosDestinadosCTeEmpresa(codigoEmpresa, string.Empty, stringConexao, 0, ref msgErro, out string codigoStatusRetornoSefaz, cliente.ClienteConfiguracao.CaminhoDocumentosFiscais, cliente.ClienteConfiguracao.TipoServicoMultisoftware))
                            {
                                Servicos.Log.TratarErro("Erro consultando documentos CT-e: " + msgErro, "BuscarCTEsDestinados");
                            }                            
                        }
                        catch(Exception ex)
                        {
                            Servicos.Log.TratarErro(ex, "BuscarCTEsDestinados");
                            continue;
                        }
                    }

                    unitOfWork.Dispose();
                    unitOfWork = null;
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex, "BuscarCTEsDestinados");
                }
                finally
                {

                    GC.Collect();
                }
            }

            unitOfWorkAdmin.Dispose();
            unitOfWorkAdmin = null;
            GC.Collect();
        }

        private void AverbacaoATM()
        {
            string[] codigosClientes = ConfigurationManager.AppSettings["CodigoClienteMultisoftware"].Split(',');

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(App.StringConexaoAdmin);

            foreach (string codigoCliente in codigosClientes)
            {
                try
                {
                    int.TryParse(codigoCliente, out int codigoClienteMultisoftware);

                    string stringConexao = App.StringConexao(codigoClienteMultisoftware, unitOfWorkAdmin);

                    Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);

                    AdminMultisoftware.Repositorio.Pessoas.Cliente repCliente = new AdminMultisoftware.Repositorio.Pessoas.Cliente(unitOfWorkAdmin);
                    Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                    Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);

                    AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente = repCliente.BuscarPorCodigo(codigoClienteMultisoftware);

                    if (cliente == null || cliente.ClienteConfiguracao == null)
                    {
                        Servicos.Log.TratarErro("Nenhum cliente encontrado.");
                        return;
                    }

                    if (cliente.ClienteConfiguracao.TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador && cliente.ClienteConfiguracao.TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && cliente.ClienteConfiguracao.TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe && cliente.ClienteConfiguracao.TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFeAdmin)
                    {
                        Servicos.Log.TratarErro("Não é TMS, Multi NFe e Multiembarcador. " + cliente.ClienteConfiguracao.TipoServicoMultisoftware.ToString());
                        return;
                    }

                    try
                    {
                        Servicos.Embarcador.CTe.Averbacao.IntegrarAverbacoesPendentesAutorizacaoInverso(cliente.ClienteConfiguracao.TipoServicoMultisoftware, unitOfWork, stringConexao);
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro("Erro averbanco CT-e" + ex);
                    }

                    unitOfWork.Dispose();
                    unitOfWork = null;
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                }
                finally
                {
                    GC.Collect();
                }
            }

            unitOfWorkAdmin.Dispose();
            unitOfWorkAdmin = null;
            GC.Collect();
        }

        private void BuscarNFSeDestinadasSaoPauloSP(string ambiente)
        {
            
            var configSettings = ConfigurationManager.GetSection("ConsultaNFSeDestinadasSaoPauloSP") as NameValueCollection;
            string[] cnpjs = configSettings["Cnpjs"].Split(',');
            string codigoCliente = ConfigurationManager.AppSettings["CodigoClienteMultisoftware"];
            int.TryParse(codigoCliente, out int codigoClienteMultisoftware);            
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(App.StringConexaoAdmin);
            AdminMultisoftware.Repositorio.Pessoas.Cliente repCliente = new AdminMultisoftware.Repositorio.Pessoas.Cliente(unitOfWorkAdmin);
            AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente = repCliente.BuscarPorCodigo(codigoClienteMultisoftware);

            if (cliente == null || cliente.ClienteConfiguracao == null)
            {
                Servicos.Log.TratarErro("Cliente não encontrado, codigo: " + codigoCliente);                
            }
            else if (cliente.ClienteConfiguracao.TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador && cliente.ClienteConfiguracao.TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && cliente.ClienteConfiguracao.TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe && cliente.ClienteConfiguracao.TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFeAdmin)
            {
                Servicos.Log.TratarErro("CLiente não é TMS, Multi NFe ou Multiembarcador. " + cliente.ClienteConfiguracao.TipoServicoMultisoftware.ToString());                                
            }
            else
            {
                string stringConexao = ObterStringConexao(ambiente, cliente.Codigo.ToString());

                Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                foreach (string cnpj in cnpjs)
                {                    
                    try
                    {
                        Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCNPJ(cnpj);
                        Servicos.Log.TratarErro("Buscando empresa " + cnpj);

                        if (empresa == null)
                            Servicos.Log.TratarErro("Empresa não encontrada no repositorio:" + cnpj);
                        else if (!repEmpresa.EmpresaAtivaDocumentosDestinados(empresa.Codigo))                        
                            Servicos.Log.TratarErro("Documentos destinados não está habilitado na empresa " + empresa.NomeCNPJ);                                                        
                        else if (string.IsNullOrWhiteSpace(empresa.NomeCertificado) || !Utilidades.IO.FileStorageService.Storage.Exists(empresa.NomeCertificado) || string.IsNullOrWhiteSpace(empresa.SenhaCertificado) || empresa.DataFinalCertificado < DateTime.Today)                        
                            Servicos.Log.TratarErro("Certificado vencido ou inexistente na empresa " + empresa.NomeCNPJ);                                                        
                        else
                        {
                            Servicos.Embarcador.Documentos.NotaFiscalServicoDestinada servicoNFSeDestinada = new Servicos.Embarcador.Documentos.NotaFiscalServicoDestinada(unitOfWork);
                                
                            if (!servicoNFSeDestinada.ConsultarNFSeDestinadaSaoPauloSP(DateTime.Today, DateTime.Today, empresa.Codigo, stringConexao))
                            {
                                Servicos.Log.TratarErro("Falha ao consultar NFSe destianda Sao Paulo SP empresa:" + empresa.NomeCNPJ);
                            }
                        }                          
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro("Erro buscando empresa " + cnpj + " - " + ex);                        
                    }                    
                }
                unitOfWork.Dispose();                
            }
            unitOfWorkAdmin.Dispose();
            GC.Collect();
        }


        private void BuscarDocumentosDestinados(string ambiente)
        {
            string[] codigosClientes = ConfigurationManager.AppSettings["CodigoClienteMultisoftware"].Split(',');

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(App.StringConexaoAdmin);

            foreach (string codigoCliente in codigosClientes)
            {
                try
                {
                    string stringConexao = ObterStringConexao(ambiente, codigoCliente);
                    int.TryParse(codigoCliente, out int codigoClienteMultisoftware);

                    Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);

                    AdminMultisoftware.Repositorio.Pessoas.Cliente repCliente = new AdminMultisoftware.Repositorio.Pessoas.Cliente(unitOfWorkAdmin);
                    Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                    Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);

                    AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente = repCliente.BuscarPorCodigo(codigoClienteMultisoftware);

                    if (cliente == null || cliente.ClienteConfiguracao == null)
                    {
                        Servicos.Log.TratarErro("Nenhum cliente encontrado.");
                        return;
                    }

                    if (cliente.ClienteConfiguracao.TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador && cliente.ClienteConfiguracao.TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && cliente.ClienteConfiguracao.TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe && cliente.ClienteConfiguracao.TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFeAdmin)
                    {
                        Servicos.Log.TratarErro("Não é TMS, Multi NFe e Multiembarcador. " + cliente.ClienteConfiguracao.TipoServicoMultisoftware.ToString());
                        return;
                    }
                    List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas> grupoPessoas = repGrupoPessoas.BuscarQueNaoImportaDocumentoDestinadoTransporte();
                    List<string> cnpjsNaoImportar = (from obj in grupoPessoas select obj.Clientes.Select(o => o.CPF_CNPJ_SemFormato)).SelectMany(o => o).ToList();
                    List<int> codigosEmpresas = new List<int>();
                    if (cliente.ClienteConfiguracao.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador || cliente.ClienteConfiguracao.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe || cliente.ClienteConfiguracao.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFeAdmin)
                        codigosEmpresas = repEmpresa.BuscarCodigosEmpresasAtivasSincronismoDocumentosDestinados();
                    else
                        codigosEmpresas = repEmpresa.BuscarCodigosEmpresasAtivasDocumentosDestinados();

                    foreach (int codigoEmpresa in codigosEmpresas)
                    {
                        Servicos.Log.TratarErro("Buscando empresa " + codigoEmpresa.ToString());
                        try
                        {
                            if (!Servicos.Embarcador.Documentos.DocumentoDestinadoEmpresa.ObterDocumentosDestinadosEmpresa(codigoEmpresa, stringConexao, cnpjsNaoImportar, out string msgErro, out string codigoStatusRetornoSefaz, 0, cliente.ClienteConfiguracao.CaminhoDocumentosFiscais, cliente.ClienteConfiguracao.TipoServicoMultisoftware))
                            {
                                Servicos.Log.TratarErro("Erro consultando documentos: " + msgErro);
                            }
                        }
                        catch (Exception ex)
                        {
                            Servicos.Log.TratarErro("Erro buscando empresa " + codigoEmpresa.ToString() + " - " + ex);
                        }
                    }

                    unitOfWork.Dispose();
                    unitOfWork = null;
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                }
                finally
                {
                    GC.Collect();
                }
            }

            unitOfWorkAdmin.Dispose();
            unitOfWorkAdmin = null;
            GC.Collect();
        }

        private void BuscarIntegracaoCTF(string ambiente)
        {
            string[] codigosClientes = ConfigurationManager.AppSettings["CodigoClienteMultisoftware"].Split(',');

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(App.StringConexaoAdmin);

            foreach (string codigoCliente in codigosClientes)
            {
                string stringConexao = ObterStringConexao(ambiente, codigoCliente);
                int.TryParse(codigoCliente, out int codigoClienteMultisoftware);

                try
                {
                    Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);

                    AdminMultisoftware.Repositorio.Pessoas.Cliente repCliente = new AdminMultisoftware.Repositorio.Pessoas.Cliente(unitOfWorkAdmin);
                    Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                    Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);

                    AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente = repCliente.BuscarPorCodigo(codigoClienteMultisoftware);

                    if (cliente == null || cliente.ClienteConfiguracao == null)
                    {
                        Servicos.Log.TratarErro("Nenhum cliente encontrado.");
                        return;
                    }

                    if (cliente.ClienteConfiguracao.TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    {
                        Servicos.Log.TratarErro("Não é TMS. " + cliente.ClienteConfiguracao.TipoServicoMultisoftware.ToString());
                        return;
                    }
                    List<int> codigosEmpresas = repEmpresa.BuscarCodigosEmpresasCTFAtivas();

                    foreach (int codigoEmpresa in codigosEmpresas)
                    {
                        Servicos.Log.TratarErro("Buscando CTF empresa " + codigoEmpresa.ToString());
                        if (!Servicos.Embarcador.Abastecimento.ImportacaoCTF.ObterAbastecimentosCTFEmpresa(codigoEmpresa, stringConexao, out string msgErro))
                        {
                            Servicos.Log.TratarErro("Erro consultando abastecimentos CTF : " + msgErro);
                        }
                    }

                    unitOfWork.Dispose();
                    unitOfWork = null;
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                }
                finally
                {
                    GC.Collect();
                }
            }

            unitOfWorkAdmin.Dispose();
            unitOfWorkAdmin = null;
            GC.Collect();
        }

        private void EnviarEmailsEncerramentoMDFe()
        {
            string[] codigosClientes = ConfigurationManager.AppSettings["CodigoClienteMultisoftware"].Split(',');

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(App.StringConexaoAdmin);

            foreach (string codigoCliente in codigosClientes)
            {
                int.TryParse(codigoCliente, out int codigoClienteMultisoftware);

                Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(App.StringConexao(codigoClienteMultisoftware, unitOfWorkAdmin));

                try
                {
                    Servicos.Embarcador.MDFe.Encerramento.EnviarEmailsEncerramentoTransportadores(codigoClienteMultisoftware, unitOfWork, unitOfWorkAdmin, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador);

                    SetarThreadEnvioEmailEncerramentoMDFe();
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                }
                finally
                {
                    unitOfWork.Dispose();
                    unitOfWork = null;
                    GC.Collect();
                }
            }

            unitOfWorkAdmin.Dispose();
            unitOfWorkAdmin = null;
            GC.Collect();
        }

        private void GerarEBS()
        {
            string[] codigosClientes = ConfigurationManager.AppSettings["CodigoClienteMultisoftware"].Split(',');

            int.TryParse(codigosClientes[0], out int codigoClienteMultisoftware);

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(App.StringConexaoAdmin);
            try
            {
                string stringConexao = App.StringConexao(codigoClienteMultisoftware, unitOfWorkAdmin);

                Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);


                Repositorio.Embarcador.Configuracoes.EBS repConfiguracaoEBS = new Repositorio.Embarcador.Configuracoes.EBS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.EBS configuracaoEBS = repConfiguracaoEBS.Buscar();

                Repositorio.Embarcador.Email.ConfigEmailDocTransporte repConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unitOfWork);
                Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte email = repConfigEmailDocTransporte.BuscarEmailEnviaDocumentoAtivo();

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Repositorio.LayoutEDI repLayout = new Repositorio.LayoutEDI(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

                List<Dominio.Entidades.Empresa> empresas = repEmpresa.BuscarTodas("A");

                DateTime dataConsulta = DateTime.Now.AddDays(-configuracaoEBS.DiasRetroativos);

                Dominio.Enumeradores.TipoAmbiente tipoAmbiente = Dominio.Enumeradores.TipoAmbiente.Producao;

#if DEBUG
                tipoAmbiente = Dominio.Enumeradores.TipoAmbiente.Homologacao;
#endif

                foreach (Dominio.Entidades.Empresa empresa in empresas)
                {
                    string rootPath = Utilidades.IO.FileStorageService.Storage.Combine(configuracaoEBS.CaminhoSalvarArquivo, empresa.CNPJ);

                    if (repCTe.ContarPorEmpresa(empresa.Codigo, dataConsulta, dataConsulta, tipoAmbiente, new string[] { "A", "Z", "C" }) > 0)
                    {
                        Servicos.Log.TratarErro("Gerando arquivo dos CT-es da empresa " + empresa.CNPJ);

                        Dominio.Entidades.LayoutEDI layout = repLayout.Buscar(Dominio.Enumeradores.TipoLayoutEDI.EBS);

                        string path = Utilidades.IO.FileStorageService.Storage.Combine(rootPath, dataConsulta.ToString("dd-MM-yyyy") + ".txt");

                        Dominio.ObjetosDeValor.EDI.EBS.DocumentoSaida ebs = Servicos.Embarcador.Integracao.EDI.EBSDocumentoSaida.GerarEBS(empresa, dataConsulta, dataConsulta, unitOfWork);

                        Servicos.GeracaoEDI serGeracaoEDI = new Servicos.GeracaoEDI(unitOfWork, layout, empresa);

                        using (System.IO.MemoryStream arquivo = serGeracaoEDI.GerarArquivoRecursivo(ebs, System.Text.Encoding.UTF8))
                        {
                            using (System.IO.Stream fileStream = Utilidades.IO.FileStorageService.Storage.OpenWrite(path))
                                arquivo.WriteTo(fileStream);

                            EnviarEmailEBS(arquivo, dataConsulta, email, empresa, configuracaoEBS);
                        }
                    }

                    unitOfWork.Dispose();
                    unitOfWork = null;
                    unitOfWork = new Repositorio.UnitOfWork(stringConexao);
                    repCTe = null;
                    repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                    repLayout = null;
                    repLayout = new Repositorio.LayoutEDI(unitOfWork);

                    GC.Collect();
                }

                repEmpresa = null;
                repConfigEmailDocTransporte = null;
                repConfiguracaoEBS = null;
                repCTe = null;
                repLayout = null;
                configuracaoEBS = null;
                email = null;
                empresas = null;

                SetarThreadEBS();

                unitOfWork.Dispose();
                unitOfWork = null;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
                GC.Collect();
            }
        }

        private void EnviarEmailEBS(System.IO.MemoryStream arquivo, DateTime dataConsulta, Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte email, Dominio.Entidades.Empresa empresa, Dominio.Entidades.Embarcador.Configuracoes.EBS configuracaoEBS)
        {
            if (email == null || string.IsNullOrWhiteSpace(configuracaoEBS.Emails))
                return;

            using (MemoryStream fZip = new MemoryStream())
            {
                using (ZipOutputStream zipOStream = new ZipOutputStream(fZip))
                {
                    zipOStream.SetLevel(9);

                    if (arquivo != null)
                    {
                        byte[] bytesArquivoAutorizados = arquivo.ToArray();

                        ZipEntry entry = new ZipEntry(dataConsulta.ToString("dd-MM-yyyy") + ".txt");
                        entry.DateTime = DateTime.Now;

                        zipOStream.PutNextEntry(entry);
                        zipOStream.Write(bytesArquivoAutorizados, 0, bytesArquivoAutorizados.Length);
                        zipOStream.CloseEntry();
                    }

                    StringBuilder mensagem = new StringBuilder();
                    StringBuilder assunto = new StringBuilder();
                    StringBuilder rodape = new StringBuilder();

                    mensagem.Append("Segue em anexo o arquivo EBS da empresa ").Append(empresa.CNPJ).Append(" referente ao dia ").Append(dataConsulta.ToString("dd/MM/yyyy"));
                    assunto.Append("EBS ").Append(empresa.CNPJ).Append(" - ").Append(dataConsulta.ToString("dd/MM/yyyy"));
                    rodape.Append("Atenciosamente,").AppendLine().Append("MultiCTe");

                    zipOStream.IsStreamOwner = false;
                    zipOStream.Close();

                    fZip.Position = 0;

                    List<string> emails = configuracaoEBS.Emails.Split(';').ToList();

#if DEBUG
                    emails = new List<string>() { "willian@multisoftware.com.br" };
#endif

                    string erro = string.Empty;

                    List<System.Net.Mail.Attachment> anexos = new List<System.Net.Mail.Attachment>();
                    anexos.Add(new System.Net.Mail.Attachment(fZip, dataConsulta.ToString("dd-MM-yyyy") + ".zip"));

                    if (!Servicos.Email.EnviarEmail(email.Email, email.Email, email.Senha, null, emails.ToArray(), null, assunto.ToString(), mensagem.ToString(), email.Smtp, out erro, email.DisplayEmail, anexos, rodape.ToString(), email.RequerAutenticacaoSmtp, "", email.PortaSmtp))
                        Servicos.Log.TratarErro(erro);
                }
            }
        }

        private void ReiniciarAplicacao()
        {
            System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
            Application.Current.Shutdown();
        }

        private void SetarAppSystemTray()
        {
            if (App.NotificationIcon == null)
            {
                App.NotificationIcon = new System.Windows.Forms.NotifyIcon();
                App.NotificationIcon.Icon = new System.Drawing.Icon("Images/Application.ico");
                App.NotificationIcon.Visible = true;
                App.NotificationIcon.Text = "Gerenciador MultiTMS";
                App.NotificationIcon.DoubleClick +=
                    delegate (object sender, EventArgs args)
                    {
                        this.Show();
                        this.WindowState = WindowState.Normal;
                    };
            }
        }

        private void SetarThreadImportacaoAbastecimentoInterno()
        {
            string[] codigosClientes = ConfigurationManager.AppSettings["CodigoClienteMultisoftware"].Split(',');

            int.TryParse(codigosClientes[0], out int codigoClienteMultisoftware);

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(App.StringConexaoAdmin);

            string stringConexao = App.StringConexao(codigoClienteMultisoftware, unitOfWorkAdmin);

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);

            string adminStringConexao = App.StringConexaoAdmin;
            Servicos.Embarcador.Abastecimento.ImportacaoAbastecimentoInterno importacaoAbastecimentoInterno = new Servicos.Embarcador.Abastecimento.ImportacaoAbastecimentoInterno(unitOfWork);
            string caminho = ConfigurationManager.AppSettings["CaminhoRaiz"];
            string tipoArmazenamento = ConfigurationManager.AppSettings["TipoArmazemento"];
            string enderecoFTP = ConfigurationManager.AppSettings["EnderecoFTP"];
            string usuarioFTP = ConfigurationManager.AppSettings["UsuarioFTP"];
            string senhaFTP = ConfigurationManager.AppSettings["SenhaFTP"];
            string caminhoRaizFTP = ConfigurationManager.AppSettings["CaminhoRaizFTP"];
            bool ftpPassivo = false;
            bool.TryParse(ConfigurationManager.AppSettings["FTPPassivo"], out ftpPassivo);
            string portaFTP = ConfigurationManager.AppSettings["PortaFTP"];
            bool utilizaSFTP = false;
            bool.TryParse(ConfigurationManager.AppSettings["UtilizaSFTP"], out utilizaSFTP);
            int tempoThread = 0;
            int.TryParse(ConfigurationManager.AppSettings["TempoThread"], out tempoThread);
            string caminhoArmazenamento = ConfigurationManager.AppSettings["CaminhoArmazenamento"];

            importacaoAbastecimentoInterno.Iniciar(caminho, tipoArmazenamento, enderecoFTP, usuarioFTP, senhaFTP, caminhoRaizFTP, ftpPassivo, portaFTP, utilizaSFTP, adminStringConexao, tempoThread, caminhoArmazenamento);
            unitOfWorkAdmin.Dispose();
        }

        private void SetarThreadConsultaSituacaoMDFe(string ambiente)
        {
            bool enable = false;
            try
            {
                NameValueCollection configSettings = ConfigurationManager.GetSection("ConsultaSituacaoMDFe") as NameValueCollection;
                enable = configSettings["Enable"].ToBool();
            }
            catch (Exception e)
            {
                Servicos.Log.TratarErro(e);
            }

            if (enable)
            {
                try
                {
                    TimeSpan timeToGo = new TimeSpan(0, 0, 5); //daqui a 5 segundos começa a execução
                    App.ThreadConsultaSituacaoMDFe = new System.Threading.Timer(x =>
                    {
                        ConsultarSituacaoMDFes(ambiente);
                    }, null, timeToGo, new TimeSpan(3, 0, 0));
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                }
            }
        }

        private void ConsultarSituacaoMDFes(string ambiente)
        {
            string[] codigosClientes = ConfigurationManager.AppSettings["CodigoClienteMultisoftware"].Split(',');

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(App.StringConexaoAdmin);

            foreach (string codigoCliente in codigosClientes)
            {
                try
                {
                    string stringConexao = ObterStringConexao(ambiente, codigoCliente);

                    int.TryParse(codigoCliente, out int codigoClienteMultisoftware);

                    Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);


                    AdminMultisoftware.Repositorio.Pessoas.Cliente repCliente = new AdminMultisoftware.Repositorio.Pessoas.Cliente(unitOfWorkAdmin);

                    AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente = repCliente.BuscarPorCodigo(codigoClienteMultisoftware);

                    if (cliente == null || cliente.ClienteConfiguracao == null)
                    {
                        Servicos.Log.TratarErro("Nenhum cliente encontrado.");
                        return;
                    }

                    if (cliente.ClienteConfiguracao.TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador && cliente.ClienteConfiguracao.TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    {
                        Servicos.Log.TratarErro("Não é TMS ou Multiembarcador. " + cliente.ClienteConfiguracao.TipoServicoMultisoftware.ToString());
                        return;
                    }

                    Servicos.Embarcador.MDFe.MDFe.ConsultarSituacaoMDFesAutorizados(unitOfWork);

                    unitOfWork.Dispose();
                    unitOfWork = null;
                }

                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                }
                finally
                {
                    GC.Collect();
                }
            }

            unitOfWorkAdmin.Dispose();
            unitOfWorkAdmin = null;
            GC.Collect();
        }

        private static string ObterStringConexao(string ambiente, string codigoCliente)
        {
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(App.StringConexaoAdmin);

            string stringConexao = null;
            if (codigoCliente != null)
            {
                int.TryParse(codigoCliente, out int codigoClienteMultisoftware);
                if (ambiente == "PRODUCAO")
                {
                    stringConexao = App.StringConexao(codigoClienteMultisoftware, unitOfWorkAdmin);
                }
                else
                {
                    stringConexao = !string.IsNullOrWhiteSpace(stringConexaoDebug) ? stringConexaoDebug : App.StringConexaoHomologacao(codigoClienteMultisoftware, unitOfWorkAdmin);
                }

                string pool = ConfigurationManager.AppSettings["Pool"].Trim();
                if (!string.IsNullOrWhiteSpace(pool)) stringConexao += pool;
            }
            unitOfWorkAdmin.Dispose();
            return stringConexao;
        }

        #endregion
    }
}

