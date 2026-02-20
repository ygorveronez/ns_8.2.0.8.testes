using EmissaoCTe.API;
using EmissaoCTe.WebApp.App_Start;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace EmissaoCTe.WebApp
{
    public class Global : System.Web.HttpApplication
    {
        private static List<Dominio.ObjetosDeValor.SessaoDoSistema> _sessionInfo;
        private static readonly object padlock = new object();

        public static List<Dominio.ObjetosDeValor.SessaoDoSistema> Sessions
        {
            get
            {
                lock (padlock)
                {
                    if (_sessionInfo == null)
                    {
                        _sessionInfo = new List<Dominio.ObjetosDeValor.SessaoDoSistema>();
                    }
                    return _sessionInfo;
                }
            }
        }

        protected void Application_Start(object sender, EventArgs e)
        {
#if !DEBUG
            CriptografarWebConfig("connectionStrings");
#endif

            //this.MigrateDatabase();

#if DEBUG
            log4net.Config.XmlConfigurator.Configure();
#endif

            AreaRegistration.RegisterAllAreas();
            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            BundleConfig.RegisterBundles(BundleTable.Bundles);

            System.Threading.Tasks.Task.Factory.StartNew(() => IniciarFilas());
            //IniciarFilas();

            Servicos.Embarcador.EmissorDocumento.EmissorDocumentoService.ConfigureApplicationEmissorDocumento(Conexao.StringConexao);
        }

        private int tentativasFila = 0;

        private void IniciarFilas()
        {
            tentativasFila++;

            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            LoadConfigurations(unidadeDeTrabalho);

            try
            {
                Repositorio.MDFeIntegracaoRetorno repMDFeIntegracaoRetorno = new Repositorio.MDFeIntegracaoRetorno(unidadeDeTrabalho);

#if !DEBUG

                Servicos.Log.TratarErro("Iniciando Filas.");
                //Processar valor de frete notas Mandae
                //Repositorio.XMLNotaFiscalEletronica repXMLNotaFiscalEletronica = new Repositorio.XMLNotaFiscalEletronica(unidadeDeTrabalho);
                //Servicos.NFSe serNFSe = new Servicos.NFSe(Conexao.StringConexao);

                //List<Dominio.Entidades.XMLNotaFiscalEletronica> listaNotas = repXMLNotaFiscalEletronica.BuscarPorContratante(19782476000150);
                //foreach (var nota in listaNotas)
                //{
                //    nota.ValorDoFrete = serNFSe.CalcularFretePorNotaImportada(nota, nota.Empresa.Codigo, "", unidadeDeTrabalho);
                //    repXMLNotaFiscalEletronica.Atualizar(nota);
                //}


                //unidadeDeTrabalho.Start();

                if (System.Configuration.ConfigurationManager.AppSettings["GerarCTesIntegrarDocumentoPorThread"] == "SIM")
                    this.CarregarGeracaoCTeIntergracao();

                this.InicializarConsultarCaixaEntradaEmail();
                this.InicializarGerarCTesEmails();

                if (System.Configuration.ConfigurationManager.AppSettings["ServicoImportacaoEnvioFTP"] == "SIM")
                {
                    this.IniciarImportacaoFTP();
                    this.IniciarProcessarImportacaoFTP();
                    this.IniciarEnvioFTP();
                }

                this.CarregarGeracaoSubcontratacao();
                this.CarregarServicoRetornoIntegracao();

                if (System.Configuration.ConfigurationManager.AppSettings["ServicoConsultaManifestosAvon"] == "SIM")
                    this.CarregarConsultaManifestosAvon(unidadeDeTrabalho);

                if (System.Configuration.ConfigurationManager.AppSettings["GerarNFSesProcessadasPorThread"] == "SIM")
                    this.CarregarGeracaoNFSesProcessadas();

                if (System.Configuration.ConfigurationManager.AppSettings["ServicoIntegracaoValePedagio"] == "SIM")
                    this.CarregarServicoIntegracaoValePedagio();

                if (System.Configuration.ConfigurationManager.AppSettings["ServicosFinalizarIntegracoesCTe"] == "SIM")
                    this.CarregarServicosFinalizarIntegracoesCTe();

                if ((System.Configuration.ConfigurationManager.AppSettings["VerificarPendenciasEmissao"] == "SIM") || (System.Configuration.ConfigurationManager.AppSettings["VerificarPendenciasEmissaoMDFe"] == "SIM") ||
                    (System.Configuration.ConfigurationManager.AppSettings["ReenviarRejeicaoCTe"] == "SIM") || (System.Configuration.ConfigurationManager.AppSettings["ReenviarRejeicaoMDFe"] == "SIM"))
                    CarregarServicoPendenciasEmissao();

                if (System.Configuration.ConfigurationManager.AppSettings["GerarCargaIntegracao"] == "SIM")
                    CarregarGeracaoCargasIntegracao();

                if (System.Configuration.ConfigurationManager.AppSettings["GerarCargaCTe"] == "SIM")
                    CarregarGeracaoCargaCTesIntegrados();

                if (System.Configuration.ConfigurationManager.AppSettings["GerarCargaNFSe"] == "SIM")
                    CarregarGeracaoCargaNFSesIntegradas();

                if (!string.IsNullOrWhiteSpace(System.Configuration.ConfigurationManager.AppSettings["HoraEnvioVencimentoCertificado"]))
                    this.CarregarServicoEnvioVencimentoCertificado();

                if (!string.IsNullOrWhiteSpace(System.Configuration.ConfigurationManager.AppSettings["HoraEnvioCTeEPEC"]))
                    this.InicializarEmissaoCTeEPEC();

                if (!string.IsNullOrWhiteSpace(System.Configuration.ConfigurationManager.AppSettings["LsTranslogIntervaloEnvio"]))
                    this.CarregarServicoEnvioIntegracaoLsTranslog();

                if (!string.IsNullOrWhiteSpace(System.Configuration.ConfigurationManager.AppSettings["LsTranslogIntervaloConsulta"]))
                    this.CarregarServicoConsultaOcorrenciasLsTranslog();

                if (System.Configuration.ConfigurationManager.AppSettings["ServicosNaturaAutomatico"] == "SIM")
                    this.CarregarServicosNatura();

                if (System.Configuration.ConfigurationManager.AppSettings["ServicoEmailMDFesNaoEncerrados"] == "SIM")
                    this.CarregarServicoEmailMDFesNaoEncerrados();

                this.CarregarConsultaDeNFSes(unidadeDeTrabalho);
                this.CarregarConsultaDeMDFes(unidadeDeTrabalho);
                this.CarregarConsultaDeCCes(unidadeDeTrabalho);
                this.CarregarConsultaDeCTes(unidadeDeTrabalho);

                this.InicializarGerarMDFes();
                this.CarregarServicoEncerramentoMDFe();

                //unidadeDeTrabalho.CommitChanges();

                Servicos.Log.TratarErro("Fila iniciada com sucesso.");
#endif

            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);

                if (tentativasFila == 3)
                    throw;
                else
                    IniciarFilas();
            }
        }

        protected void Session_Start(object sender, EventArgs e)
        {
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            if (Request.AppRelativeCurrentExecutionFilePath == "~/")
                HttpContext.Current.RewritePath("Default.aspx");
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {
            if (Session["IdUsuario"] == null)
                return;

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                Repositorio.LogAcesso repLogAcesso = new Repositorio.LogAcesso(unitOfWork);
                Dominio.Entidades.Usuario usuario = repUsuario.BuscarPorCodigo((int)Session["IdUsuario"]);
                Dominio.Entidades.LogAcesso logAcesso = new Dominio.Entidades.LogAcesso();

                logAcesso.Data = DateTime.Now;
                logAcesso.IPAcesso = string.Empty; //Request não fica disponível neste contexto, por isso não é salvo o IP. Para identificar qual a sessão de início deve-se verificar pelo SessionID.
                logAcesso.Login = usuario.Login;
                logAcesso.Senha = usuario.Senha;
                logAcesso.SessionID = Session.SessionID;
                logAcesso.Tipo = Dominio.Enumeradores.TipoLogAcesso.Saída;
                logAcesso.Usuario = usuario;

                repLogAcesso.Inserir(logAcesso);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        protected void Application_EndRequest(object sender, EventArgs e)
        {
            Repositorio.NHibernateHttpModule.CloseSession(Conexao.StringConexao);
        }

        protected void Application_End(object sender, EventArgs e)
        {

        }

        protected void Application_PreSendRequestHeaders()
        {
            Response.Headers.Remove("Server");
            Response.Headers.Remove("X-AspNet-Version");
            Response.Headers.Remove("X-AspNetMvc-Version");
        }

        static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute("Start", "Default.aspx", new { controller = "Home", action = "Index", id = "" });
            routes.MapRoute("Api", "{controller}/{action}/{id}", new { controller = "Home", action = "Index", id = UrlParameter.Optional });
        }

        private void CarregarConsultaDeCTes(Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            string configAdicionarCTesFilaConsulta = ConfigurationManager.AppSettings["AdicionarCTesFilaConsulta"];
            if (configAdicionarCTesFilaConsulta == null || configAdicionarCTesFilaConsulta == "")
                configAdicionarCTesFilaConsulta = "SIM";

            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);

            List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> listaCTes = repCTe.BuscarTodosPorStatus(configAdicionarCTesFilaConsulta == "SIM" ? new string[] { "E", "K", "L", "X", "V", "B" } : new string[] { "K", "L", "X", "V", "B" });

            //foreach (Dominio.Entidades.ConhecimentoDeTransporteEletronico cte in listaCTes)
            for (var i = 0; i < listaCTes.Count; i++)
            {
                if (listaCTes[i].SistemaEmissor == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissorDocumento.Integrador)
                    FilaConsultaCTe.GetInstance().QueueItem(1, listaCTes[i].Codigo, Dominio.Enumeradores.TipoObjetoConsulta.CTe, Conexao.StringConexao);
            }
        }

        private void CarregarConsultaDeCCes(Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.CartaDeCorrecaoEletronica repCCe = new Repositorio.CartaDeCorrecaoEletronica(unidadeDeTrabalho);

            List<Dominio.Entidades.CartaDeCorrecaoEletronica> listaCCes = repCCe.BuscarPorStatus(new Dominio.Enumeradores.StatusCCe[] { Dominio.Enumeradores.StatusCCe.Enviado });

            //foreach (Dominio.Entidades.CartaDeCorrecaoEletronica cce in listaCCes)
            for (var i = 0; i < listaCCes.Count; i++)
            {
                if (listaCCes[i].SistemaEmissor == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissorDocumento.Integrador)
                    FilaConsultaCTe.GetInstance().QueueItem(1, listaCCes[i].Codigo, Dominio.Enumeradores.TipoObjetoConsulta.CCe, Conexao.StringConexao);
            }
        }

        private void CarregarConsultaDeMDFes(Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeDeTrabalho);

            List<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais> listaMDFes = repMDFe.BuscarPorStatus(new Dominio.Enumeradores.StatusMDFe[] { Dominio.Enumeradores.StatusMDFe.Enviado, Dominio.Enumeradores.StatusMDFe.EmCancelamento, Dominio.Enumeradores.StatusMDFe.EmEncerramento, Dominio.Enumeradores.StatusMDFe.EmitidoContingencia, Dominio.Enumeradores.StatusMDFe.EventoInclusaoMotoristaEnviado });

            //foreach (Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe in listaMDFes)
            for (var i = 0; i < listaMDFes.Count; i++)
            {
                if (listaMDFes[i].SistemaEmissor == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissorDocumento.Integrador)
                    FilaConsultaCTe.GetInstance().QueueItem(2, listaMDFes[i].Codigo, Dominio.Enumeradores.TipoObjetoConsulta.MDFe, Conexao.StringConexao);
            }
        }

        private void CarregarConsultaDeNFSes(Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.NFSe repNFSe = new Repositorio.NFSe(unidadeDeTrabalho);

            List<Dominio.Entidades.NFSe> listaNFSes = repNFSe.BuscarPorStatus(new Dominio.Enumeradores.StatusNFSe[] { Dominio.Enumeradores.StatusNFSe.Enviado, Dominio.Enumeradores.StatusNFSe.EmCancelamento });

            //foreach (Dominio.Entidades.NFSe nfse in listaNFSes)
            for (var i = 0; i < listaNFSes.Count; i++)
            {
                FilaConsultaCTe.GetInstance().QueueItem(3, listaNFSes[i].Codigo, Dominio.Enumeradores.TipoObjetoConsulta.NFSe, Conexao.StringConexao);
            }
        }

        private void CarregarConsultaManifestosAvon(Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.ManifestoAvon repManifestoAvon = new Repositorio.ManifestoAvon(unidadeDeTrabalho);

            List<Dominio.Entidades.ManifestoAvon> listaManifestos = repManifestoAvon.BuscarPorStatus(Dominio.Enumeradores.StatusManifestoAvon.Enviado);

            foreach (Dominio.Entidades.ManifestoAvon manifesto in listaManifestos)
            {
                FilaEmissaoManifestoAvon.GetInstance().QueueItem(manifesto.Empresa.Codigo, manifesto.Codigo, Conexao.StringConexao);
            }

            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
            List<Dominio.Entidades.Empresa> empresas = repEmpresa.BuscarEmpresasAvon();

            foreach (Dominio.Entidades.Empresa empresa in empresas)
            {
                RetornoManifestoAvon.GetInstance().QueueItem(empresa.Codigo, Conexao.StringConexao);
            }
        }

        private void CarregarServicoEncerramentoMDFe()
        {
            ServicoEncerramentoMDFe.GetInstance().IniciarThread(Conexao.StringConexao);
            ServicoEncerramentoAutomaticoMDFe.GetInstance().IniciarThread(Conexao.StringConexao);
        }

        private void CarregarServicoEnvioVencimentoCertificado()
        {
            ServicoEnvioVencimentoCertificados.GetInstance().IniciarThread(Conexao.StringConexao);
        }

        private void CarregarServicoEnvioIntegracaoLsTranslog()
        {
            LsTranslogEnvioIntegracao.GetInstance().IniciarThread(Conexao.StringConexao);
        }


        private void CarregarServicoConsultaOcorrenciasLsTranslog()
        {
            LsTranslogConsultaOcorrencias.GetInstance().IniciarThread(Conexao.StringConexao);
        }

        private void CarregarGeracaoCargasIntegracao()
        {
            GerarCargasIntegracao.GetInstance().QueueItem(1, Conexao.StringConexao);
        }

        private void CarregarGeracaoCargaCTesIntegrados()
        {
            GerarCargaCTesIntegrados.GetInstance().QueueItem(1, Conexao.StringConexao);
        }

        private void CarregarGeracaoCargaNFSesIntegradas()
        {
            GerarCargaNFSesIntegrados.GetInstance().QueueItem(1, Conexao.StringConexao);
        }

        private void CarregarGeracaoCTeIntergracao()
        {
            GerarCTesIntegracao.GetInstance().QueueItem(1, Conexao.StringConexao);
            GerarNFSesIntegracao.GetInstance().QueueItem(1, Conexao.StringConexao);
        }

        private void CarregarGeracaoSubcontratacao()
        {
            GerarSubcontratacoes.GetInstance().QueueItem(1, Conexao.StringConexao);
        }

        private void CarregarGeracaoNFSesProcessadas()
        {
            GerarNFSesProcessadas.GetInstance().QueueItem(1, Conexao.StringConexao);
        }

        private void CarregarServicoPendenciasEmissao()
        {
            ServicoPendenciasEmissao.GetInstance().QueueItem(1, Conexao.StringConexao);
        }

        private void CarregarServicosFinalizarIntegracoesCTe()
        {
            ServicoIntegracaoCTeEmitirCte.GetInstance().IniciarThread(1, Conexao.StringConexao);
            //ServicoIntegracaoCTeGerarMDFe.GetInstance().QueueItem(1, Conexao.StringConexao);
        }

        private void CarregarServicoIntegracaoValePedagio()
        {
            ServicoIntegracaoValePedagio.GetInstance().QueueItem(1, Conexao.StringConexao);
        }

        private void CarregarServicosNatura()
        {
            ServicoEmissaoNatura.GetInstance().IniciarThread(1, Conexao.StringConexao);
            ServicoRetornoNatura.GetInstance().IniciarThread(1, Conexao.StringConexao);
        }

        private void CarregarServicoEmailMDFesNaoEncerrados()
        {
            ServicoEmailMDFesNaoEncerrados.GetInstance().IniciarThread(Conexao.StringConexao);
        }

        private void CarregarServicoRetornoIntegracao()
        {
            ServicoRetornoIntegracao.GetInstance().IniciarThread(Conexao.StringConexao);
        }

        private void MigrateDatabase()
        {
            try
            {
                Migrations.Migrator.MigrateToLatest(WebConfigurationManager.ConnectionStrings["ControleCTe"].ConnectionString);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
        }

        private void InicializarConsultarCaixaEntradaEmail()
        {
            try
            {
                ConsultarCaixaEntradaEmail.GetInstance().QueueItem(1, Conexao.StringConexao);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Problemas ao inicializar Consulta e-mail " + ex);
            }
        }

        private void InicializarGerarCTesEmails()
        {
            try
            {
                GerarCTesEmails.GetInstance().QueueItem(1, Conexao.StringConexao);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Problemas ao inicializar Gerar CTes E-mails " + ex);
            }
        }

        private void IniciarImportacaoFTP()
        {
            try
            {
                ServicoImportacaoFTP.GetInstance().QueueItem(1, Conexao.StringConexao);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Problemas ao inicializar Importação FTP " + ex);
            }
        }

        private void IniciarProcessarImportacaoFTP()
        {
            try
            {
                GerarCTesFTP.GetInstance().QueueItem(1, Conexao.StringConexao);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Problemas ao inicializar GerarCTesFTP " + ex);
            }
        }

        private void IniciarEnvioFTP()
        {
            try
            {
                ServicoEnvioFTP.GetInstance().QueueItem(1, Conexao.StringConexao);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Problemas ao inicializar EnviarFTP " + ex);
            }
        }

        private void InicializarGerarMDFes()
        {
            try
            {
                GerarMDFe.GetInstance().QueueItem(1, Conexao.StringConexao);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Problemas ao inicializar Gerar MDFe " + ex);
            }
        }

        private void InicializarEmissaoCTeEPEC()
        {
            ServicoEmissaoCTeEPEC.GetInstance().IniciarThread(Conexao.StringConexao);
        }

        private void LoadConfigurations(Repositorio.UnitOfWork unitOfWork)
        {

#if !DEBUG
                new Servicos.Embarcador.Configuracoes.Arquivo().AjustarConfiguracoes(unitOfWork);
#else
            Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork);
#endif

        }

        private void CriptografarWebConfig(string section, string applicationPath = null, string provider = "RSAProtectedConfigurationProvider")
        {
            if (applicationPath == null)
                applicationPath = "~";

            Configuration confg = WebConfigurationManager.OpenWebConfiguration(applicationPath);
            ConfigurationSection confStrSect = confg.GetSection(section);

            if (confStrSect != null && !confStrSect.SectionInformation.IsProtected)
            {
                confStrSect.SectionInformation.ProtectSection(provider);
                confg.Save();
            }
        }
    }
}