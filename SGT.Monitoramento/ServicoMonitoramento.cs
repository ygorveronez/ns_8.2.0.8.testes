using AdminMultisoftware.Dominio.Enumeradores;
using NHibernate.Linq.Functions;
using System;
using System.Configuration;
using System.Reflection.Emit;
using System.ServiceProcess;

namespace SGT.Monitoramento
{
    public partial class ServicoMonitoramento : ServiceBase
    {
        #region Metodos Publicos

        public ServicoMonitoramento()
        {
            InitializeComponent();
        }

        public void SetarExecucaoThreads()
        {
            Log("", false);
            Log("Inicio do controle de monitoramento");

            LoadConfigurations(Program.StringConexao);
            string ambiente = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance()?.ObterConfiguracaoAmbiente()?.IdentificacaoAmbiente ?? string.Empty;

            if (Program.Cliente == null || Program.Cliente == null)
            {
                Servicos.Log.TratarErro("Nenhum cliente encontrado.");
                Log("Nenhum cliente encontrado.");
                return;
            }
            Log(Program.Cliente.Codigo + " - " + Program.Cliente.RazaoSocial);
            Log(Program.Cliente.ClienteConfiguracao.TipoServicoMultisoftware.ToString());

            VerificaVersao();
            VerificaAmbiente(ambiente);
#if DEBUG
            // colocar aqui a tread para execução             
            CriarThreadsIntegracoes(Program.StringConexao);

#else
            CriarThreadsMonitoramento(Program.StringConexao, ambiente, Program.Cliente);
            CriarThreadsIntegracoes(Program.StringConexao);
#endif
        }

        #endregion

        #region Metodos Protegidos

        protected override void OnStart(string[] args)
        {
            Servicos.Log.TratarErro("Serviço Monitoramento iniciado.");

            try
            {
                SetarExecucaoThreads();
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                throw;
            }
        }

        protected override void OnStop()
        {
            FinalizarThreadsMonitoramentos();
            FinalizarThreadsIntegracoes();

            Servicos.Log.TratarErro("Serviço Monitoramento Parado.");
        }

        #endregion

        #region Criar Threads Integracao

        private static void CriarThreadsIntegracoes(string stringConexao)
        {
            //SetThreadMonitoramentoIntegracaoOnixSat(stringConexao);
            //SetThreadMonitoramentoIntegracaoSascar(stringConexao);
            //SetThreadMonitoramentoIntegracaoOmnilink(stringConexao);
            //SetThreadMonitoramentoIntegracaoAutotrac(stringConexao);
            //SetThreadMonitoramentoIntegracaoTrafegus(stringConexao);
            //SetThreadMonitoramentoIntegracaoOpentech(stringConexao);
            //SetThreadMonitoramentoIntegracaoSighra(stringConexao);
            //SetThreadMonitoramentoIntegracaoRavex(stringConexao);
            //SetThreadMonitoramentoIntegracaoSmartTracking(stringConexao);
            //SetThreadMonitoramentoIntegracaoMixTelematics(stringConexao);
            //SetThreadMonitoramentoIntegracaoPositron(stringConexao);
            //SetThreadMonitoramentoIntegracaoGVSAT(stringConexao);
            //SetThreadMonitoramentoIntegracaoKagoo(stringConexao);
            //SetThreadMonitoramentoIntegracaoSystemSat(stringConexao);
            //SetThreadMonitoramentoIntegracaoGetrak(stringConexao);
            //SetThreadMonitoramentoIntegracaoVrack(stringConexao);
            //SetThreadMonitoramentoIntegracaoA2(stringConexao);
            //SetThreadMonitoramentoIntegracaoSpyTruck(stringConexao);
            //SetThreadMonitoramentoIntegracaoLink(stringConexao);
            //SetThreadMonitoramentoIntegracaoCSX(stringConexao);
            //SetThreadMonitoramentoIntegracaoTraffilog(stringConexao);
            //SetThreadMonitoramentoIntegracaoNSTech(stringConexao);
            //SetThreadMonitoramentoIntegracaoIturan(stringConexao);
            //SetThreadMonitoramentoIntegracao3S(stringConexao);
            //SetThreadMonitoramentoIntegracaoSkyWorld(stringConexao);
            //SetThreadMonitoramentoIntegracaoSigaSul(stringConexao);
            //SetThreadMonitoramentoIntegracaoABFSat(stringConexao);
            //SetThreadMonitoramentoIntegracaoAutovision(stringConexao);
            //SetThreadMonitoramentoIntegracaoWebRotas(stringConexao);
            //SetThreadMonitoramentoIntegracaoLogRisk(stringConexao);
            //SetThreadMonitoramentoIntegracaoMaxtrack(stringConexao);
            //SetThreadMonitoramentoIntegracaoTranspanorama(stringConexao);
            //SetThreadMonitoramentoIntegracaoTrixlog(stringConexao);
            //SetThreadMonitoramentoIntegracaoTrustTrack(stringConexao);
            //SetThreadMonitoramentoIntegracaoAutotracEmbarcador(stringConexao);
            //SetThreadMonitoramentoIntegracaoGNSBrasil(stringConexao);
            //SetThreadMonitoramentoIntegracaoEagleTrack(stringConexao);
            //SetThreadMonitoramentoIntegracaoCCNTelematica(stringConexao);
            //SetThreadMonitoramentoIntegracaoAngelLira(stringConexao);
            //SetThreadMonitoramentoIntegracaoRaster(stringConexao);
            //SetThreadMonitoramentoIntegracaoBrasilRiskPosicoes(stringConexao);
            //SetThreadMonitoramentoIntegracaoRotaExata(stringConexao);
            SetThreadMonitoramentoIntegracaoRastrear(stringConexao);

        }



        private static void SetThreadMonitoramentoIntegracaoRastrear(string stringConexao)
        {
            try
            {
                Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoRastrear.GetInstance(Program.Cliente).Iniciar(stringConexao);
                Log("Iniciada thread IntegracaoRastrear");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "IntegracaoRastrear");
            }

        }

        private static void SetThreadMonitoramentoIntegracaoRotaExata(string stringConexao)
        {
            try
            {
                Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoRotaExata.GetInstance(Program.Cliente).Iniciar(stringConexao);
                Log("Iniciada thread IntegracaoRotaExata");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "Integracao IntegracaoRotaExata");
            }
        }

        private static void SetThreadMonitoramentoIntegracaoBrasilRiskPosicoes(string stringConexao)
        {
            try
            {
                Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoBrasilRiskPosicoes.GetInstance(Program.Cliente).Iniciar(stringConexao);
                Log("Iniciada thread IntegracaoBrasilRiskPosicoes");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "Integracao BrasilRiskPosicoes");
            }
        }

        private static void SetThreadMonitoramentoIntegracaoRaster(string stringConexao)
        {
            try
            {
                Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoRaster.GetInstance(Program.Cliente).Iniciar(stringConexao);
                Log("Iniciada thread IntegracaoRaster");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "Integracao IntegracaoRaster");
            }
        }

        private static void SetThreadMonitoramentoIntegracaoAngelLira(string stringConexao)
        {
            try
            {
                Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoAngelLira.GetInstance(Program.Cliente).Iniciar(stringConexao);
                Log("Iniciada thread IntegracaoAngelLira");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "Integracao IntegracaoAngelLira");
            }
        }

        private static void SetThreadMonitoramentoIntegracaoCCNTelematica(string stringConexao)
        {
            try
            {
                Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoCCNTelematica.GetInstance(Program.Cliente).Iniciar(stringConexao);
                Log("Iniciada thread IntegracaoCCNTelematica");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "Integracao IntegracaoCCNTelematica");
            }
        }

        private static void SetThreadMonitoramentoIntegracaoEagleTrack(string stringConexao)
        {
            try
            {
                Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoEagleTrack.GetInstance(Program.Cliente).Iniciar(stringConexao);
                Log("Iniciada thread IntegracaoEagleTrack");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "Integracao IntegracaoEagleTrack");
            }
        }

        private static void SetThreadMonitoramentoIntegracaoAutotracEmbarcador(string stringConexao)
        {
            try
            {
                Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoAutotracEmbarcador.GetInstance(Program.Cliente).Iniciar(stringConexao);
                Log("Iniciada thread IntegracaoAutotracEmbarcador");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "Integracao AutotracEmbarcador");
            }
        }

        private static void SetThreadMonitoramentoIntegracaoGNSBrasil(string stringConexao)
        {
            try
            {
                Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoGNSBrasil.GetInstance(Program.Cliente).Iniciar(stringConexao);
                Log("Iniciada thread IntegracaoGNSBrasil");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "IntegracaoGNSBrasil");
            }
        }

        private static void SetThreadMonitoramentoIntegracaoTrustTrack(string stringConexao)
        {
            try
            {
                Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoTrustTrack.GetInstance(Program.Cliente).Iniciar(stringConexao);
                Log("Iniciada thread IntegracaoTrustTrack");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "IntegracaoTrustTrack");
            }
        }

        private static void SetThreadMonitoramentoIntegracaoTrixlog(string stringConexao)
        {
            try
            {
                Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoTrixlog.GetInstance(Program.Cliente).Iniciar(stringConexao);
                Log("Iniciada thread IntegracaoTrixlog");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "IntegracaoTrixlog");
            }
        }

        private static void SetThreadMonitoramentoIntegracaoTranspanorama(string stringConexao)
        {
            try
            {
                Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoTransPanorama.GetInstance(Program.Cliente).Iniciar(stringConexao);
                Log("Iniciada thread IntegracaoTranspanorama");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "IntegracaoTranspanorama");
            }
        }

        private static void SetThreadMonitoramentoIntegracaoOnixSat(string stringConexao)
        {
            try
            {
                Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoOnixSat.GetInstance(Program.Cliente).Iniciar(stringConexao);
                Log("Iniciada thread IntegracaoOnixSat");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "IntegracaoOnixSat");
            }
        }

        private static void SetThreadMonitoramentoIntegracaoSascar(string stringConexao)
        {
            try
            {
                Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoSascar.GetInstance(Program.Cliente).Iniciar(stringConexao);
                Log("Iniciada thread IntegracaoSascar");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "IntegracaoTrafegus");
            }
        }

        private static void SetThreadMonitoramentoIntegracaoTrafegus(string stringConexao)
        {
            try
            {
                Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoTrafegus.GetInstance(Program.Cliente).Iniciar(stringConexao);
                Log("Iniciada thread IntegracaoTrafegus");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "IntegracaoTrafegus");
            }
        }

        private static void SetThreadMonitoramentoIntegracaoOmnilink(string stringConexao)
        {
            try
            {
                Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoOmnilink.GetInstance(Program.Cliente).Iniciar(stringConexao);
                Log("Iniciada thread IntegracaoOmnilink");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "Integracao Omnilink");
            }
        }

        private static void SetThreadMonitoramentoIntegracaoAutotrac(string stringConexao)
        {
            try
            {
                Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoAutotrac.GetInstance(Program.Cliente).Iniciar(stringConexao);
                Log("Iniciada thread IntegracaoAutotrac");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "Integracao Autotrac");
            }
        }

        private static void SetThreadMonitoramentoIntegracaoOpentech(string stringConexao)
        {
            try
            {
                Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoOpentech.GetInstance(Program.Cliente).Iniciar(stringConexao);
                Log("Iniciada thread IntegracaoOpentech");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "IntegracaoOpentech");
            }
        }

        private static void SetThreadMonitoramentoIntegracaoSighra(string stringConexao)
        {
            try
            {
                Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoSighra.GetInstance(Program.Cliente).Iniciar(stringConexao);
                Log("Iniciada thread IntegracaoSighra");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "IntegracaoSighra");
            }
        }

        private static void SetThreadMonitoramentoIntegracaoRavex(string stringConexao)
        {
            try
            {
                Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoRavex.GetInstance(Program.Cliente).Iniciar(stringConexao);
                Log("Iniciada thread IntegracaoRavex");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "IntegracaoSighra");
            }
        }

        private static void SetThreadMonitoramentoIntegracaoSmartTracking(string stringConexao)
        {
            try
            {
                Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoSmartTracking.GetInstance(Program.Cliente).Iniciar(stringConexao);
                Log("Iniciada thread IntegracaoSmartTracking");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "IntegracaoSmartTracking");
            }
        }

        private static void SetThreadMonitoramentoIntegracaoMixTelematics(string stringConexao)
        {
            try
            {
                Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoMixTelematics.GetInstance(Program.Cliente).Iniciar(stringConexao);
                Log("Iniciada thread IntegracaoMixTelematics");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "IntegracaoMixTelematics");
            }
        }

        private static void SetThreadMonitoramentoIntegracaoPositron(string stringConexao)
        {
            try
            {
                Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoPositron.GetInstance(Program.Cliente).Iniciar(stringConexao);
                Log("Iniciada thread IntegracaoPositron");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "IntegracaoPositron");
            }
        }

        private static void SetThreadMonitoramentoIntegracaoGVSAT(string stringConexao)
        {
            try
            {
                Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoGVSAT.GetInstance(Program.Cliente).Iniciar(stringConexao);
                Log("Iniciada thread IntegracaoGVSAT");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "IntegracaoGVSAT");
            }
        }

        private static void SetThreadMonitoramentoIntegracaoKagoo(string stringConexao)
        {
            try
            {
                Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoKaboo.GetInstance(Program.Cliente).Iniciar(stringConexao);
                Log("Iniciada thread IntegracaoKaboo");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "IntegracaoKaboo");
            }
        }

        private static void SetThreadMonitoramentoIntegracaoSystemSat(string stringConexao)
        {
            try
            {
                Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoSystemSat.GetInstance(Program.Cliente).Iniciar(stringConexao);
                Log("Iniciada thread IntegracaoSystemSat");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "IntegracaoSystemSat");
            }
        }

        private static void SetThreadMonitoramentoIntegracaoGetrak(string stringConexao)
        {
            try
            {
                Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoGetrak.GetInstance(Program.Cliente).Iniciar(stringConexao);
                Log("Iniciada thread IntegracaoGetrak");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "IntegracaoGetrak");
            }
        }

        private static void SetThreadMonitoramentoIntegracaoVrack(string stringConexao)
        {
            try
            {
                Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoVtrack.GetInstance(Program.Cliente).Iniciar(stringConexao);
                Log("Iniciada thread IntegracaoVtrack");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "Integracao Vtrack");
            }
        }

        private static void SetThreadMonitoramentoIntegracaoA2(string stringConexao)
        {
            try
            {
                Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoA2.GetInstance(Program.Cliente).Iniciar(stringConexao);
                Log("Iniciada thread IntegracaoA2");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "Integracao A2");
            }
        }

        private static void SetThreadMonitoramentoIntegracaoSpyTruck(string stringConexao)
        {
            try
            {
                Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoSpytruck.GetInstance(Program.Cliente).Iniciar(stringConexao);
                Log("Iniciada thread IntegracaoSpytruck");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "IntegracaoSpytruck");
            }
        }

        private static void SetThreadMonitoramentoIntegracaoLink(string stringConexao)
        {
            try
            {
                Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoRastreamentoLink.GetInstance(Program.Cliente).Iniciar(stringConexao);
                Log("Iniciada thread IntegracaoRastreamentoLink");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "IntegracaoRastreamentoLink");
            }
        }

        private static void SetThreadMonitoramentoIntegracaoCSX(string stringConexao)
        {
            try
            {
                Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoCSX.GetInstance(Program.Cliente).Iniciar(stringConexao);
                Log("Iniciada thread IntegracaoCSX");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "IntegracaoCSX");
            }
        }

        private static void SetThreadMonitoramentoIntegracaoTraffilog(string stringConexao)
        {
            try
            {
                Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoTraffilog.GetInstance(Program.Cliente).Iniciar(stringConexao);
                Log("Iniciada thread IntegracaoTraffilog");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "IntegracaoTraffilog");
            }
        }

        private static void SetThreadMonitoramentoIntegracaoNSTech(string stringConexao)
        {
            try
            {
                Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoNSTech.GetInstance(Program.Cliente).Iniciar(stringConexao);
                Log("Iniciada thread IntegracaoNSTech");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "IntegracaoNSTech");
            }

        }

        private static void SetThreadMonitoramentoIntegracaoIturan(string stringConexao)
        {
            try
            {
                Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoIturan.GetInstance(Program.Cliente).Iniciar(stringConexao);
                Log("Iniciada thread IntegracaoIturan");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "IntegracaoIturan");
            }
        }

        private static void SetThreadMonitoramentoIntegracao3S(string stringConexao)
        {
            try
            {
                Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoTresS.GetInstance(Program.Cliente).Iniciar(stringConexao);
                Log("Iniciada thread Integracao3S");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "Integracao3S");
            }
        }

        private static void SetThreadMonitoramentoIntegracaoSkyWorld(string stringConexao)
        {
            try
            {
                Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoSkyWorld.GetInstance(Program.Cliente).Iniciar(stringConexao);
                Log("Iniciada thread IntegracaoSkyWorld");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "IntegracaoSkyWorld");
            }
        }

        private static void SetThreadMonitoramentoIntegracaoSigaSul(string stringConexao)
        {
            try
            {
                Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoSigaSul.GetInstance(Program.Cliente).Iniciar(stringConexao);
                Log("Iniciada thread IntegracaoSigaSul");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "IntegracaoSigaSul");
            }
        }

        private static void SetThreadMonitoramentoIntegracaoABFSat(string stringConexao)
        {
            try
            {
                Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoABFSat.GetInstance(Program.Cliente).Iniciar(stringConexao);
                Log("Iniciada thread IntegracaoABFSat");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "IntegracaoABFSat");
            }
        }

        private static void SetThreadMonitoramentoIntegracaoAutovision(string stringConexao)
        {
            try
            {
                Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoAutovision.GetInstance(Program.Cliente).Iniciar(stringConexao);
                Log("Iniciada thread IntegracaoAutovision");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "IntegracaoAutovision");
            }
        }

        private static void SetThreadMonitoramentoIntegracaoWebRotas(string stringConexao)
        {
            try
            {
                Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoWebRotas.GetInstance(Program.Cliente).Iniciar(stringConexao);
                Log("Iniciada thread IntegracaoWebRotas");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "IntegracaoWebRotas");
            }
        }

        private static void SetThreadMonitoramentoIntegracaoLogRisk(string stringConexao)
        {
            try
            {
                Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoLogRisk.GetInstance(Program.Cliente).Iniciar(stringConexao);
                Log("Iniciada thread IntegracaoLogRisk");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "IntegracaoLogRisk");
            }
        }

        private static void SetThreadMonitoramentoIntegracaoMaxtrack(string stringConexao)
        {
            try
            {
                Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoMaxtrack.GetInstance(Program.Cliente).Iniciar(stringConexao);
                Log("Iniciada thread IntegracaoMaxtrack");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "IntegracaoLogRisk");
            }
        }

        #endregion

        #region Criar Threads Monitoramentos

        private static void CriarThreadsMonitoramento(string stringConexao, string ambiente, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente)
        {
            CriarThreadMonitorarPosicoes(stringConexao, ambiente, cliente.ClienteConfiguracao.TipoServicoMultisoftware, cliente);
            CriarThreadProcessarTrocaAlvo(stringConexao, ambiente, cliente.ClienteConfiguracao.TipoServicoMultisoftware, cliente);
            CriarThreadProcessarMonitoramentos(stringConexao, ambiente, cliente.ClienteConfiguracao.TipoServicoMultisoftware, cliente);
            CriarThreadProcessarMonitoramentosStatusViagem(stringConexao, ambiente, cliente.ClienteConfiguracao.TipoServicoMultisoftware, cliente);
            CriarThreadProcessarEventos(stringConexao, ambiente, cliente.ClienteConfiguracao.TipoServicoMultisoftware, cliente);
            CriarThreadProcessarEventosSinal(stringConexao, ambiente, cliente.ClienteConfiguracao.TipoServicoMultisoftware, cliente);
            CriarThreadEnviarNotificacoesAlertas(stringConexao, ambiente, cliente.ClienteConfiguracao.TipoServicoMultisoftware, cliente);
            CriarThreadImportarPosicoesPendentes(stringConexao, ambiente, cliente.ClienteConfiguracao.TipoServicoMultisoftware, cliente);
            CriarThreadControleArquivos(stringConexao, ambiente, cliente.ClienteConfiguracao.TipoServicoMultisoftware, cliente);
        }

        private static void CriarThreadControleArquivos(string stringConexao, string ambiente, TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware)
        {
            try
            {
                Thread.ControleArquivos.GetInstance(stringConexao).Iniciar(stringConexao, ambiente, tipoServicoMultisoftware, clienteMultisoftware);
                Log("Iniciada thread ControleArquivos");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "Controle Arquivos");
            }
        }

        private static void CriarThreadMonitorarPosicoes(string stringConexao, string ambiente, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware)
        {
            try
            {
                Thread.MonitorarPosicoes.GetInstance(stringConexao).Iniciar(stringConexao, ambiente, tipoServicoMultisoftware, clienteMultisoftware);
                Log("Iniciada thread MonitorarPosicoes");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "Monitorar posições");
            }
        }

        private static void CriarThreadProcessarTrocaAlvo(string stringConexao, string ambiente, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware)
        {
            try
            {
                Thread.ProcessarTrocaAlvo.GetInstance(stringConexao).Iniciar(stringConexao, ambiente, tipoServicoMultisoftware, clienteMultisoftware);
                Log("Iniciada thread ProcessarTrocaAlvo");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "Processar trocas de alvo");
            }
        }

        private static void CriarThreadProcessarMonitoramentos(string stringConexao, string ambiente, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware)
        {
            try
            {
                Thread.ProcessarMonitoramentos.GetInstance(stringConexao).Iniciar(stringConexao, ambiente, tipoServicoMultisoftware, clienteMultisoftware);
                Log("Iniciada thread ProcessarMonitoramentos");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "Processar monitoramentos");
            }
        }

        private static void CriarThreadProcessarMonitoramentosStatusViagem(string stringConexao, string ambiente, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware)
        {
            try
            {
                Thread.ProcessarMonitoramentosStatusViagem.GetInstance(stringConexao).Iniciar(stringConexao, ambiente, tipoServicoMultisoftware, clienteMultisoftware);
                Log("Iniciada thread ProcessarMonitoramentosStatusViagem");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "Processar monitoramentos");
            }
        }

        private static void CriarThreadProcessarEventos(string stringConexao, string ambiente, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware)
        {
            try
            {
                Thread.ProcessarEventos.GetInstance(stringConexao).Iniciar(stringConexao, ambiente, tipoServicoMultisoftware, clienteMultisoftware);
                Log("Iniciada thread ProcessarEventos");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "Processar eventos");
            }
        }

        private static void CriarThreadProcessarEventosSinal(string stringConexao, string ambiente, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware)
        {
            try
            {
                Thread.ProcessarEventosSinal.GetInstance(stringConexao).Iniciar(stringConexao, ambiente, tipoServicoMultisoftware, clienteMultisoftware);
                Log("Iniciada thread ProcessarEventosSinal");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "Processar eventos sinal");
            }
        }

        private static void CriarThreadEnviarNotificacoesAlertas(string stringConexao, string ambiente, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware)
        {
            try
            {
                Thread.EnviarNotificacoesAlertas.GetInstance(stringConexao).Iniciar(stringConexao, ambiente, tipoServicoMultisoftware, clienteMultisoftware);
                Log("Iniciada thread EnviarNotificacoesAlertas");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "Enviar notificacoes de alertas");
            }
        }

        private static void CriarThreadImportarPosicoesPendentes(string stringConexao, string ambiente, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware)
        {
            try
            {
                Thread.ImportarPosicoesPendentesIntegracao.GetInstance(stringConexao).Iniciar(stringConexao, ambiente, tipoServicoMultisoftware, clienteMultisoftware);
                Log("Iniciada thread ImportarPosicoesPendentesIntegracao");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "Importar posicoes pendentes");
            }
        }

        #endregion

        #region Finalizar Threads Monitoramentos

        private static void FinalizarThreadsMonitoramentos()
        {
            FinalizarThreadMonitorarPosicoes();
            FinalizarThreadProcessarTrocaAlvo();
            FinalizarThreadProcessarMonitoramentos();
            FinalizarThreadProcessarMonitoramentosStatusViagem();
            FinalizarThreadProcessarEventos();
            FinalizarThreadProcessarEventosSinal();
            FinalizarThreadEnviarNotificacoesAlertas();
            FinalizarThreadImportarPosicoesPendentes();
            FinalizarThreadControleArquivos();
        }

        private static void FinalizarThreadControleArquivos()
        {
            try
            {
                Thread.ControleArquivos.GetInstance(string.Empty).Finalizar();
                Log("Finalizada thread ControleArquivos");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "Controle Arquivos");
            }
        }

        private static void FinalizarThreadImportarPosicoesPendentes()
        {
            try
            {
                Thread.ImportarPosicoesPendentesIntegracao.GetInstance(string.Empty).Finalizar();
                Log("Finalizada thread ImportarPosicoesPendentes");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "Monitorar posições");
            }
        }

        private static void FinalizarThreadEnviarNotificacoesAlertas()
        {
            try
            {
                Thread.EnviarNotificacoesAlertas.GetInstance(string.Empty).Finalizar();
                Log("Finalizada thread EnviarNotificacoesAlertas");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "Enviar notificacoes de alertas");
            }
        }

        private static void FinalizarThreadProcessarEventosSinal()
        {
            try
            {
                Thread.ProcessarEventosSinal.GetInstance(string.Empty).Finalizar();
                Log("Finalizada thread ProcessarEventosSinal");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "Processar eventos sinal");
            }
        }

        private static void FinalizarThreadProcessarEventos()
        {
            try
            {
                Thread.ProcessarEventos.GetInstance(string.Empty).Finalizar();
                Log("Finalizada thread ProcessarEventos");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "Processar Eventos");
            }
        }

        private static void FinalizarThreadProcessarMonitoramentos()
        {
            try
            {
                Thread.ProcessarMonitoramentos.GetInstance(string.Empty).Finalizar();
                Log("Finalizada thread ProcessarMonitoramentos");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "Processar monitoramentos");
            }
        }

        private static void FinalizarThreadProcessarMonitoramentosStatusViagem()
        {
            try
            {
                Thread.ProcessarMonitoramentosStatusViagem.GetInstance(string.Empty).Finalizar();
                Log("Finalizada thread ProcessarMonitoramentosStatusViagem");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "Processar monitoramentos");
            }
        }


        private static void FinalizarThreadProcessarTrocaAlvo()
        {
            try
            {
                Thread.ProcessarTrocaAlvo.GetInstance(string.Empty).Finalizar();
                Log("Finalizada thread ProcessarTrocaAlvo");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "Processar trocas de alvo");
            }
        }

        private static void FinalizarThreadMonitorarPosicoes()
        {
            try
            {
                Thread.MonitorarPosicoes.GetInstance(string.Empty).Finalizar();
                Log("Finalizada thread MonitorarPosicoes");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "Monitorar posições");
            }
        }

        #endregion

        #region Finalizar Threads Integrações

        private void FinalizarThreadsIntegracoes()
        {

            Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoOnixSat.GetInstance(Program.Cliente).Finalizar();
            Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoSascar.GetInstance(Program.Cliente).Finalizar();
            Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoTrafegus.GetInstance(Program.Cliente).Finalizar();
            Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoOmnilink.GetInstance(Program.Cliente).Finalizar();
            Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoAutotrac.GetInstance(Program.Cliente).Finalizar();
            Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoOpentech.GetInstance(Program.Cliente).Finalizar();
            Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoSighra.GetInstance(Program.Cliente).Finalizar();
            Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoRavex.GetInstance(Program.Cliente).Finalizar();
            Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoSmartTracking.GetInstance(Program.Cliente).Finalizar();
            Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoMixTelematics.GetInstance(Program.Cliente).Finalizar();
            Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoPositron.GetInstance(Program.Cliente).Finalizar();
            Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoGVSAT.GetInstance(Program.Cliente).Finalizar();
            Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoKaboo.GetInstance(Program.Cliente).Finalizar();
            Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoSystemSat.GetInstance(Program.Cliente).Finalizar();
            Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoGetrak.GetInstance(Program.Cliente).Finalizar();
            Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoVtrack.GetInstance(Program.Cliente).Finalizar();
            Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoA2.GetInstance(Program.Cliente).Finalizar();
            Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoSpytruck.GetInstance(Program.Cliente).Finalizar();
            Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoRastreamentoLink.GetInstance(Program.Cliente).Finalizar();
            Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoCSX.GetInstance(Program.Cliente).Finalizar();
            Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoTraffilog.GetInstance(Program.Cliente).Finalizar();
            Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoNSTech.GetInstance(Program.Cliente).Finalizar();
            Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoTresS.GetInstance(Program.Cliente).Finalizar();
            Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoSkyWorld.GetInstance(Program.Cliente).Finalizar();
            Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoSigaSul.GetInstance(Program.Cliente).Finalizar();
            Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoWebRotas.GetInstance(Program.Cliente).Finalizar();
            Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoABFSat.GetInstance(Program.Cliente).Finalizar();
            Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoAutovision.GetInstance(Program.Cliente).Finalizar();
            Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoLogRisk.GetInstance(Program.Cliente).Finalizar();
            Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoTransPanorama.GetInstance(Program.Cliente).Finalizar();
            Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoTrixlog.GetInstance(Program.Cliente).Finalizar();
            Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoTrustTrack.GetInstance(Program.Cliente).Finalizar();
            Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoAutotracEmbarcador.GetInstance(Program.Cliente).Finalizar();
            Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoGNSBrasil.GetInstance(Program.Cliente).Finalizar();
            Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoEagleTrack.GetInstance(Program.Cliente).Finalizar();
            Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoBrasilRiskPosicoes.GetInstance(Program.Cliente).Finalizar();
            Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoRotaExata.GetInstance(Program.Cliente).Finalizar();
        }

        #endregion

        #region Metodos Privados

        private static void VerificaAmbiente(string ambiente)
        {
            Log($"Ambiente {ambiente}");
        }

        private bool Habilitado()
        {
            bool enable = false;
            Servicos.Log.TratarErro("app.Config: " + ConfigurationManager.AppSettings["Enable"]);
            try
            {
                enable = bool.Parse(ConfigurationManager.AppSettings["Enable"]);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao fazer parse da configuração Enable: {ex.ToString()}", "CatchNoAction");
            }
            string msg = "GerenciadorApp " + ((enable) ? "ativado! Iniciando ..." : "desativado");
            Servicos.Log.TratarErro(msg);
            return enable;
        }

        private static void VerificaVersao()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            System.Diagnostics.FileVersionInfo fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
            Log($"Versao {fvi.FileVersion}");
        }

        private static void Log(string msg, bool dt = true)
        {
            Servicos.Embarcador.Monitoramento.MonitoramentoUtils.GravarLogTracking(msg, "ControleMonitoramento", dt);
        }

        private static void LoadConfigurations(string stringConexao)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);
            try
            {
#if !DEBUG
                new Servicos.Embarcador.Configuracoes.Arquivo().AjustarConfiguracoes(unitOfWork);
                new Servicos.Embarcador.Configuracoes.Arquivo().AjustarConfiguracoesMonitoramento(unitOfWork);

                Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork);
                Servicos.Embarcador.Configuracoes.ConfiguracaoMonioramentoInstance.GetInstance(unitOfWork);

#else
                new Servicos.Embarcador.Configuracoes.Arquivo().AjustarConfiguracoes(unitOfWork);
                new Servicos.Embarcador.Configuracoes.Arquivo().AjustarConfiguracoesMonitoramento(unitOfWork);

                Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork);
                Servicos.Embarcador.Configuracoes.ConfiguracaoMonioramentoInstance.GetInstance(unitOfWork);
#endif
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion
    }
}

