using AdminMultisoftware.Dominio.Enumeradores;
using System.Diagnostics;

namespace Monitoramento
{
    public sealed class MonitoramentoService
    {
        private AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente? cliente;
        private string? stringConexao;
        private CancellationToken cancellationToken;

        #region Metodos Publicos

        public void SetarExecucaoThreads(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, string stringConexaoCliente, CancellationToken stoppingToken)
        {
            cliente = clienteMultisoftware;
            stringConexao = stringConexaoCliente;
            cancellationToken = stoppingToken;

            Log("", false);
            Log("Inicio do controle de monitoramento");

            LoadConfigurations();
            string ambiente = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance()?.ObterConfiguracaoAmbiente()?.IdentificacaoAmbiente ?? string.Empty;

            if (cliente == null)
            {
                Servicos.Log.TratarErro("Nenhum cliente encontrado.");
                Log("Nenhum cliente encontrado.");
                return;
            }

            Log(cliente.Codigo + " - " + cliente.RazaoSocial);
            Log(cliente.ClienteConfiguracao.TipoServicoMultisoftware.ToString());

            VerificaVersao();
            VerificaAmbiente(ambiente);

#if DEBUG
            CriarThreadsMonitoramento(ambiente);
            CriarThreadsIntegracoes();
#else
            CriarThreadsMonitoramento(ambiente);
            CriarThreadsIntegracoes();
#endif
        }

        #endregion

        #region Criar Threads Integracao

        private void CriarThreadsIntegracoes()
        {
            SetThreadMonitoramentoIntegracaoOnixSat();
            SetThreadMonitoramentoIntegracaoSascar();
            SetThreadMonitoramentoIntegracaoOmnilink();
            SetThreadMonitoramentoIntegracaoAutotrac();
            SetThreadMonitoramentoIntegracaoTrafegus();
            SetThreadMonitoramentoIntegracaoOpentech();
            SetThreadMonitoramentoIntegracaoSighra();
            SetThreadMonitoramentoIntegracaoRavex();
            SetThreadMonitoramentoIntegracaoSmartTracking();
            SetThreadMonitoramentoIntegracaoMixTelematics();
            SetThreadMonitoramentoIntegracaoPositron();
            SetThreadMonitoramentoIntegracaoGVSAT();
            SetThreadMonitoramentoIntegracaoKaboo();
            SetThreadMonitoramentoIntegracaoSystemSat();
            SetThreadMonitoramentoIntegracaoGetrak();
            SetThreadMonitoramentoIntegracaoVtrack();
            SetThreadMonitoramentoIntegracaoA2();
            SetThreadMonitoramentoIntegracaoSpytruck();
            SetThreadMonitoramentoIntegracaoRastreamentoLink();
            SetThreadMonitoramentoIntegracaoCSX();
            SetThreadMonitoramentoIntegracaoTraffilog();
            SetThreadMonitoramentoIntegracaoNSTech();
            SetThreadMonitoramentoIntegracaoIturan();
            SetThreadMonitoramentoIntegracaoTresS();
            SetThreadMonitoramentoIntegracaoSkyWorld();
            SetThreadMonitoramentoIntegracaoSigaSul();
            SetThreadMonitoramentoIntegracaoABFSat();
            SetThreadMonitoramentoIntegracaoAutovision();
            SetThreadMonitoramentoIntegracaoWebRotas();
            SetThreadMonitoramentoIntegracaoLogRisk();
            SetThreadMonitoramentoIntegracaoMaxtrack();
            SetThreadMonitoramentoIntegracaoTransPanorama();
            SetThreadMonitoramentoIntegracaoTrixlog();
            SetThreadMonitoramentoIntegracaoTrustTrack();
            SetThreadMonitoramentoIntegracaoAutotracEmbarcador();
            SetThreadMonitoramentoIntegracaoGNSBrasil();
            SetThreadMonitoramentoIntegracaoEagleTrack();
            SetThreadMonitoramentoIntegracaoCCNTelematica();
            SetThreadMonitoramentoIntegracaoAngelLira();
            SetThreadMonitoramentoIntegracaoRaster();
            SetThreadMonitoramentoIntegracaoBrasilRiskPosicoes();
            SetThreadMonitoramentoIntegracaoBrasilRiskPosicoesPlaca();
            SetThreadMonitoramentoIntegracaoRotaExata();
            SetThreadMonitoramentoIntegracaoRastrear();
            SetThreadMonitoramentoIntegracaoApisulLog();
            SetThreadMonitoramentoIntegracaoViaLink();
            SetThreadMonitoramentoIntegracaoBuonny();
            SetThreadMonitoramentoIntegracaoMultiPortal();
            SetThreadMonitoramentoIntegracaoRastreamentoEvo();
            SetThreadMonitoramentoIntegracaoUnitop();
            SetThreadMonitoramentoIntegracaoTotalTrac();
            SetThreadMonitoramentoIntegracaoCobli();
            SetThreadMonitoramentoIntegracaoOmnicomm();

            /** Quando adicionar uma nova integração aqui, adicionar também em:
             * Serviço de refresh de configurações: ConfiguracaoRefreshService => AtualizarConfiguracoesIntegracoes 
             * Método de finalização das threads: FinalizarThreadsIntegracoes **/
        }

        private void SetThreadMonitoramentoIntegracaoViaLink()
        {
            try
            {
                Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoViaLink.GetInstance(cliente).Iniciar(stringConexao, cancellationToken);
                Log("Iniciada thread IntegracaoViaLink");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "Integracao ViaLink");
            }
        }

        private void SetThreadMonitoramentoIntegracaoBuonny()
        {
            try
            {
                Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoBuonny.GetInstance(cliente).Iniciar(stringConexao, cancellationToken);
                Log("Iniciada thread IntegracaoBuonny");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "Integracao Buonny");
            }
        }

        private void SetThreadMonitoramentoIntegracaoMultiPortal()
        {
            try
            {
                Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoMultiPortal.GetInstance(cliente).Iniciar(stringConexao, cancellationToken);
                Log("Iniciada thread IntegracaoMultiPortal");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "Integracao MultiPortal");
            }
        }

        private void SetThreadMonitoramentoIntegracaoRotaExata()
        {
            try
            {
                Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoRotaExata.GetInstance(cliente).Iniciar(stringConexao, cancellationToken);
                Log("Iniciada thread IntegracaoRotaExata");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "Integracao IntegracaoRotaExata");
            }
        }

        private void SetThreadMonitoramentoIntegracaoBrasilRiskPosicoes()
        {
            try
            {
                Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoBrasilRiskPosicoes.GetInstance(cliente).Iniciar(stringConexao, cancellationToken);
                Log("Iniciada thread IntegracaoBrasilRiskPosicoes");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "Integracao BrasilRiskPosicoes");
            }
        }

        private void SetThreadMonitoramentoIntegracaoBrasilRiskPosicoesPlaca()
        {
            try
            {
                Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoBrasilRiskPosicoesPlaca.GetInstance(cliente).Iniciar(stringConexao, cancellationToken);
                Log("Iniciada thread IntegracaoBrasilRiskPosicoesPlaca");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "Integracao IntegracaoBrasilRiskPosicoesPlaca");
            }
        }

        private void SetThreadMonitoramentoIntegracaoRaster()
        {
            try
            {
                Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoRaster.GetInstance(cliente).Iniciar(stringConexao, cancellationToken);
                Log("Iniciada thread IntegracaoRaster");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "Integracao IntegracaoRaster");
            }
        }

        private void SetThreadMonitoramentoIntegracaoAngelLira()
        {
            try
            {
                Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoAngelLira.GetInstance(cliente).Iniciar(stringConexao, cancellationToken);
                Log("Iniciada thread IntegracaoAngelLira");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "Integracao IntegracaoAngelLira");
            }
        }

        private void SetThreadMonitoramentoIntegracaoCCNTelematica()
        {
            try
            {
                Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoCCNTelematica.GetInstance(cliente).Iniciar(stringConexao, cancellationToken);
                Log("Iniciada thread IntegracaoCCNTelematica");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "Integracao IntegracaoCCNTelematica");
            }
        }

        private void SetThreadMonitoramentoIntegracaoEagleTrack()
        {
            try
            {
                Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoEagleTrack.GetInstance(cliente).Iniciar(stringConexao, cancellationToken);
                Log("Iniciada thread IntegracaoEagleTrack");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "Integracao IntegracaoEagleTrack");
            }
        }

        private void SetThreadMonitoramentoIntegracaoAutotracEmbarcador()
        {
            try
            {
                Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoAutotracEmbarcador.GetInstance(cliente).Iniciar(stringConexao, cancellationToken);
                Log("Iniciada thread IntegracaoAutotracEmbarcador");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "Integracao AutotracEmbarcador");
            }
        }

        private void SetThreadMonitoramentoIntegracaoGNSBrasil()
        {
            try
            {
                Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoGNSBrasil.GetInstance(cliente).Iniciar(stringConexao, cancellationToken);
                Log("Iniciada thread IntegracaoGNSBrasil");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "IntegracaoGNSBrasil");
            }
        }

        private void SetThreadMonitoramentoIntegracaoTrustTrack()
        {
            try
            {
                Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoTrustTrack.GetInstance(cliente).Iniciar(stringConexao, cancellationToken);
                Log("Iniciada thread IntegracaoTrustTrack");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "IntegracaoTrustTrack");
            }
        }

        private void SetThreadMonitoramentoIntegracaoTrixlog()
        {
            try
            {
                Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoTrixlog.GetInstance(cliente).Iniciar(stringConexao, cancellationToken);
                Log("Iniciada thread IntegracaoTrixlog");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "IntegracaoTrixlog");
            }
        }

        private void SetThreadMonitoramentoIntegracaoTransPanorama()
        {
            try
            {
                Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoTransPanorama.GetInstance(cliente).Iniciar(stringConexao, cancellationToken);
                Log("Iniciada thread IntegracaoTranspanorama");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "IntegracaoTranspanorama");
            }
        }

        private void SetThreadMonitoramentoIntegracaoOnixSat()
        {
            try
            {
                Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoOnixSat.GetInstance(cliente).Iniciar(stringConexao, cancellationToken);
                Log("Iniciada thread IntegracaoOnixSat");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "IntegracaoOnixSat");
            }
        }

        private void SetThreadMonitoramentoIntegracaoSascar()
        {
            try
            {
                Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoSascar.GetInstance(cliente).Iniciar(stringConexao, cancellationToken);
                Log("Iniciada thread IntegracaoSascar");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "IntegracaoTrafegus");
            }
        }

        private void SetThreadMonitoramentoIntegracaoTrafegus()
        {
            try
            {
                Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoTrafegus.GetInstance(cliente).Iniciar(stringConexao, cancellationToken);
                Log("Iniciada thread IntegracaoTrafegus");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "IntegracaoTrafegus");
            }
        }

        private void SetThreadMonitoramentoIntegracaoOmnilink()
        {
            try
            {
                Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoOmnilink.GetInstance(cliente).Iniciar(stringConexao, cancellationToken);
                Log("Iniciada thread IntegracaoOmnilink");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "Integracao Omnilink");
            }
        }

        private void SetThreadMonitoramentoIntegracaoAutotrac()
        {
            try
            {
                Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoAutotrac.GetInstance(cliente).Iniciar(stringConexao, cancellationToken);
                Log("Iniciada thread IntegracaoAutotrac");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "Integracao Autotrac");
            }
        }

        private void SetThreadMonitoramentoIntegracaoOpentech()
        {
            try
            {
                Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoOpentech.GetInstance(cliente).Iniciar(stringConexao, cancellationToken);
                Log("Iniciada thread IntegracaoOpentech");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "IntegracaoOpentech");
            }
        }

        private void SetThreadMonitoramentoIntegracaoSighra()
        {
            try
            {
                Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoSighra.GetInstance(cliente).Iniciar(stringConexao, cancellationToken);
                Log("Iniciada thread IntegracaoSighra");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "IntegracaoSighra");
            }
        }

        private void SetThreadMonitoramentoIntegracaoRavex()
        {
            try
            {
                Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoRavex.GetInstance(cliente).Iniciar(stringConexao, cancellationToken);
                Log("Iniciada thread IntegracaoRavex");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "IntegracaoSighra");
            }
        }

        private void SetThreadMonitoramentoIntegracaoSmartTracking()
        {
            try
            {
                Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoSmartTracking.GetInstance(cliente).Iniciar(stringConexao, cancellationToken);
                Log("Iniciada thread IntegracaoSmartTracking");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "IntegracaoSmartTracking");
            }
        }

        private void SetThreadMonitoramentoIntegracaoMixTelematics()
        {
            try
            {
                Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoMixTelematics.GetInstance(cliente).Iniciar(stringConexao, cancellationToken);
                Log("Iniciada thread IntegracaoMixTelematics");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "IntegracaoMixTelematics");
            }
        }

        private void SetThreadMonitoramentoIntegracaoPositron()
        {
            try
            {
                Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoPositron.GetInstance(cliente).Iniciar(stringConexao, cancellationToken);
                Log("Iniciada thread IntegracaoPositron");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "IntegracaoPositron");
            }
        }

        private void SetThreadMonitoramentoIntegracaoGVSAT()
        {
            try
            {
                Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoGVSAT.GetInstance(cliente).Iniciar(stringConexao, cancellationToken);
                Log("Iniciada thread IntegracaoGVSAT");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "IntegracaoGVSAT");
            }
        }

        private void SetThreadMonitoramentoIntegracaoKaboo()
        {
            try
            {
                Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoKaboo.GetInstance(cliente).Iniciar(stringConexao, cancellationToken);
                Log("Iniciada thread IntegracaoKaboo");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "IntegracaoKaboo");
            }
        }

        private void SetThreadMonitoramentoIntegracaoSystemSat()
        {
            try
            {
                Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoSystemSat.GetInstance(cliente).Iniciar(stringConexao, cancellationToken);
                Log("Iniciada thread IntegracaoSystemSat");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "IntegracaoSystemSat");
            }
        }

        private void SetThreadMonitoramentoIntegracaoGetrak()
        {
            try
            {
                Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoGetrak.GetInstance(cliente).Iniciar(stringConexao, cancellationToken);
                Log("Iniciada thread IntegracaoGetrak");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "IntegracaoGetrak");
            }
        }

        private void SetThreadMonitoramentoIntegracaoVtrack()
        {
            try
            {
                Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoVtrack.GetInstance(cliente).Iniciar(stringConexao, cancellationToken);
                Log("Iniciada thread IntegracaoVtrack");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "Integracao Vtrack");
            }
        }

        private void SetThreadMonitoramentoIntegracaoA2()
        {
            try
            {
                Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoA2.GetInstance(cliente).Iniciar(stringConexao, cancellationToken);
                Log("Iniciada thread IntegracaoA2");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "Integracao A2");
            }
        }

        private void SetThreadMonitoramentoIntegracaoSpytruck()
        {
            try
            {
                Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoSpytruck.GetInstance(cliente).Iniciar(stringConexao, cancellationToken);
                Log("Iniciada thread IntegracaoSpytruck");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "IntegracaoSpytruck");
            }
        }

        private void SetThreadMonitoramentoIntegracaoRastreamentoLink()
        {
            try
            {
                Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoRastreamentoLink.GetInstance(cliente).Iniciar(stringConexao, cancellationToken);
                Log("Iniciada thread IntegracaoRastreamentoLink");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "IntegracaoRastreamentoLink");
            }
        }

        private void SetThreadMonitoramentoIntegracaoCSX()
        {
            try
            {
                Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoCSX.GetInstance(cliente).Iniciar(stringConexao, cancellationToken);
                Log("Iniciada thread IntegracaoCSX");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "IntegracaoCSX");
            }
        }

        private void SetThreadMonitoramentoIntegracaoTraffilog()
        {
            try
            {
                Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoTraffilog.GetInstance(cliente).Iniciar(stringConexao, cancellationToken);
                Log("Iniciada thread IntegracaoTraffilog");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "IntegracaoTraffilog");
            }
        }

        private void SetThreadMonitoramentoIntegracaoNSTech()
        {
            try
            {
                Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoNSTech.GetInstance(cliente).Iniciar(stringConexao, cancellationToken);
                Log("Iniciada thread IntegracaoNSTech");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "IntegracaoNSTech");
            }

        }

        private void SetThreadMonitoramentoIntegracaoIturan()
        {
            try
            {
                Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoIturan.GetInstance(cliente).Iniciar(stringConexao, cancellationToken);
                Log("Iniciada thread IntegracaoIturan");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "IntegracaoIturan");
            }
        }

        private void SetThreadMonitoramentoIntegracaoTresS()
        {
            try
            {
                Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoTresS.GetInstance(cliente).Iniciar(stringConexao, cancellationToken);
                Log("Iniciada thread IntegracaoTresS");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "IntegracaoTresS");
            }
        }

        private void SetThreadMonitoramentoIntegracaoSkyWorld()
        {
            try
            {
                Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoSkyWorld.GetInstance(cliente).Iniciar(stringConexao, cancellationToken);
                Log("Iniciada thread IntegracaoSkyWorld");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "IntegracaoSkyWorld");
            }
        }

        private void SetThreadMonitoramentoIntegracaoSigaSul()
        {
            try
            {
                Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoSigaSul.GetInstance(cliente).Iniciar(stringConexao, cancellationToken);
                Log("Iniciada thread IntegracaoSigaSul");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "IntegracaoSigaSul");
            }
        }

        private void SetThreadMonitoramentoIntegracaoABFSat()
        {
            try
            {
                Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoABFSat.GetInstance(cliente).Iniciar(stringConexao, cancellationToken);
                Log("Iniciada thread IntegracaoABFSat");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "IntegracaoABFSat");
            }
        }

        private void SetThreadMonitoramentoIntegracaoAutovision()
        {
            try
            {
                Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoAutovision.GetInstance(cliente).Iniciar(stringConexao, cancellationToken);
                Log("Iniciada thread IntegracaoAutovision");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "IntegracaoAutovision");
            }
        }

        private void SetThreadMonitoramentoIntegracaoWebRotas()
        {
            try
            {
                Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoWebRotas.GetInstance(cliente).Iniciar(stringConexao, cancellationToken);
                Log("Iniciada thread IntegracaoWebRotas");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "IntegracaoWebRotas");
            }
        }

        private void SetThreadMonitoramentoIntegracaoLogRisk()
        {
            try
            {
                Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoLogRisk.GetInstance(cliente).Iniciar(stringConexao, cancellationToken);
                Log("Iniciada thread IntegracaoLogRisk");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "IntegracaoLogRisk");
            }
        }

        private void SetThreadMonitoramentoIntegracaoMaxtrack()
        {
            try
            {
                Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoMaxtrack.GetInstance(cliente).Iniciar(stringConexao, cancellationToken);
                Log("Iniciada thread IntegracaoMaxtrack");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "IntegracaoLogRisk");
            }
        }

        private void SetThreadMonitoramentoIntegracaoRastrear()
        {
            try
            {
                Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoRastrear.GetInstance(cliente).Iniciar(stringConexao, cancellationToken);
                Log("Iniciada thread IntegracaoRastrear");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "IntegracaoRastrear");
            }

        }

        private void SetThreadMonitoramentoIntegracaoApisulLog()
        {
            try
            {
                Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoApisulLog.GetInstance(cliente).Iniciar(stringConexao, cancellationToken);
                Log("Iniciada thread IntegracaoApisulLog");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "Integracao IntegracaoApisulLog");
            }
        }

        private void SetThreadMonitoramentoIntegracaoRastreamentoEvo()
        {
            try
            {
                Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoRastreamentoEvo.GetInstance(cliente).Iniciar(stringConexao, cancellationToken);
                Log("Iniciada thread IntegracaoRastreamentoEvo");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "Integracao Evo Solutions");
            }
        }

        private void SetThreadMonitoramentoIntegracaoUnitop()
        {
            try
            {
                Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoUnitop.GetInstance(cliente).Iniciar(stringConexao, cancellationToken);
                Log("Iniciada thread IntegracaoUnitop");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "IntegracaoUnitop");
            }
        }

        private void SetThreadMonitoramentoIntegracaoTotalTrac()
        {
            try
            {
                Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoTotalTrac.GetInstance(cliente).Iniciar(stringConexao, cancellationToken);
                Log("Iniciada thread IntegracaoTotalTrac");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "IntegracaoTotalTrac");
            }
        }

        private void SetThreadMonitoramentoIntegracaoCobli()
        {
            try
            {
                Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoCobli.GetInstance(cliente).Iniciar(stringConexao, cancellationToken);
                Log("Iniciada thread IntegracaoCobli");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "IntegracaoCobli");
            }
        }

        private void SetThreadMonitoramentoIntegracaoOmnicomm()
        {
            try
            {
                Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoOmnicomm.GetInstance(cliente).Iniciar(stringConexao, cancellationToken);
                Log("Iniciada thread IntegracaoOmnicomm");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "IntegracaoOmnicomm");
            }
        }

        #endregion

        #region Criar Threads Monitoramentos

        private void CriarThreadsMonitoramento(string ambiente)
        {
            CriarThreadMonitorarPosicoes(ambiente, cliente.ClienteConfiguracao.TipoServicoMultisoftware);
            CriarThreadProcessarTrocaAlvo(ambiente, cliente.ClienteConfiguracao.TipoServicoMultisoftware);
            CriarThreadProcessarMonitoramentos(ambiente, cliente.ClienteConfiguracao.TipoServicoMultisoftware);
            CriarThreadProcessarMonitoramentosStatusViagem(ambiente, cliente.ClienteConfiguracao.TipoServicoMultisoftware);
            CriarThreadProcessarEventos(ambiente, cliente.ClienteConfiguracao.TipoServicoMultisoftware);
            CriarThreadProcessarEventosSinal(ambiente, cliente.ClienteConfiguracao.TipoServicoMultisoftware);
            CriarThreadEnviarNotificacoesAlertas(ambiente, cliente.ClienteConfiguracao.TipoServicoMultisoftware);
            CriarThreadImportarPosicoesPendentes(ambiente, cliente.ClienteConfiguracao.TipoServicoMultisoftware);
            CriarThreadControleArquivos(ambiente, cliente.ClienteConfiguracao.TipoServicoMultisoftware);
        }

        private void CriarThreadControleArquivos(string ambiente, TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            try
            {
                Thread.ControleArquivos.GetInstance(stringConexao).Iniciar(stringConexao, ambiente, tipoServicoMultisoftware, cliente, cancellationToken);
                Log("Iniciada thread ControleArquivos");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "Controle Arquivos");
            }
        }

        private void CriarThreadMonitorarPosicoes(string ambiente, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            try
            {
                Thread.MonitorarPosicoes.GetInstance(stringConexao).Iniciar(stringConexao, ambiente, tipoServicoMultisoftware, cliente, cancellationToken);
                Log("Iniciada thread MonitorarPosicoes");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "Monitorar posições");
            }
        }

        private void CriarThreadProcessarTrocaAlvo(string ambiente, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            try
            {
                Thread.ProcessarTrocaAlvo.GetInstance(stringConexao).Iniciar(stringConexao, ambiente, tipoServicoMultisoftware, cliente, cancellationToken);
                Log("Iniciada thread ProcessarTrocaAlvo");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "Processar trocas de alvo");
            }
        }

        private void CriarThreadProcessarMonitoramentos(string ambiente, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            try
            {
                Thread.ProcessarMonitoramentos.GetInstance(stringConexao).Iniciar(stringConexao, ambiente, tipoServicoMultisoftware, cliente, cancellationToken);
                Log("Iniciada thread ProcessarMonitoramentos");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "Processar monitoramentos");
            }
        }

        private void CriarThreadProcessarMonitoramentosStatusViagem(string ambiente, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            try
            {
                Thread.ProcessarMonitoramentosStatusViagem.GetInstance(stringConexao).Iniciar(stringConexao, ambiente, tipoServicoMultisoftware, cliente, cancellationToken);
                Log("Iniciada thread ProcessarMonitoramentosStatusViagem");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "Processar monitoramentos");
            }
        }

        private void CriarThreadProcessarEventos(string ambiente, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            try
            {
                Thread.ProcessarEventos.GetInstance(stringConexao).Iniciar(stringConexao, ambiente, tipoServicoMultisoftware, cliente, cancellationToken);
                Log("Iniciada thread ProcessarEventos");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "Processar eventos");
            }
        }

        private void CriarThreadProcessarEventosSinal(string ambiente, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            try
            {
                Thread.ProcessarEventosSinal.GetInstance(stringConexao).Iniciar(stringConexao, ambiente, tipoServicoMultisoftware, cliente, cancellationToken);
                Log("Iniciada thread ProcessarEventosSinal");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "Processar eventos sinal");
            }
        }

        private void CriarThreadEnviarNotificacoesAlertas(string ambiente, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            try
            {
                Thread.EnviarNotificacoesAlertas.GetInstance(stringConexao).Iniciar(stringConexao, ambiente, tipoServicoMultisoftware, cliente, cancellationToken);
                Log("Iniciada thread EnviarNotificacoesAlertas");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "Enviar notificacoes de alertas");
            }
        }

        private void CriarThreadImportarPosicoesPendentes(string ambiente, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            try
            {
                Thread.ImportarPosicoesPendentesIntegracao.GetInstance(stringConexao).Iniciar(stringConexao, ambiente, tipoServicoMultisoftware, cliente, cancellationToken);
                Log("Iniciada thread ImportarPosicoesPendentesIntegracao");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "Importar posicoes pendentes");
            }
        }

        #endregion

        #region Finalizar Threads Monitoramentos

        public void FinalizarThreadsMonitoramentos()
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

        private void FinalizarThreadControleArquivos()
        {
            try
            {
                Thread.ControleArquivos.GetInstance(stringConexao).Finalizar();
                Log("Finalizada thread ControleArquivos");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "Controle Arquivos");
            }
        }

        private void FinalizarThreadImportarPosicoesPendentes()
        {
            try
            {
                Thread.ImportarPosicoesPendentesIntegracao.GetInstance(stringConexao).Finalizar();
                Log("Finalizada thread ImportarPosicoesPendentes");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "Monitorar posições");
            }
        }

        private void FinalizarThreadEnviarNotificacoesAlertas()
        {
            try
            {
                Thread.EnviarNotificacoesAlertas.GetInstance(stringConexao).Finalizar();
                Log("Finalizada thread EnviarNotificacoesAlertas");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "Enviar notificacoes de alertas");
            }
        }

        private void FinalizarThreadProcessarEventosSinal()
        {
            try
            {
                Thread.ProcessarEventosSinal.GetInstance(stringConexao).Finalizar();
                Log("Finalizada thread ProcessarEventosSinal");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "Processar eventos sinal");
            }
        }

        private void FinalizarThreadProcessarEventos()
        {
            try
            {
                Thread.ProcessarEventos.GetInstance(stringConexao).Finalizar();
                Log("Finalizada thread ProcessarEventos");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "Processar Eventos");
            }
        }

        private void FinalizarThreadProcessarMonitoramentos()
        {
            try
            {
                Thread.ProcessarMonitoramentos.GetInstance(stringConexao).Finalizar();
                Log("Finalizada thread ProcessarMonitoramentos");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "Processar monitoramentos");
            }
        }

        private void FinalizarThreadProcessarMonitoramentosStatusViagem()
        {
            try
            {
                Thread.ProcessarMonitoramentosStatusViagem.GetInstance(stringConexao).Finalizar();
                Log("Finalizada thread ProcessarMonitoramentosStatusViagem");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "Processar monitoramentos");
            }
        }


        private void FinalizarThreadProcessarTrocaAlvo()
        {
            try
            {
                Thread.ProcessarTrocaAlvo.GetInstance(stringConexao).Finalizar();
                Log("Finalizada thread ProcessarTrocaAlvo");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "Processar trocas de alvo");
            }
        }

        private void FinalizarThreadMonitorarPosicoes()
        {
            try
            {
                Thread.MonitorarPosicoes.GetInstance(stringConexao).Finalizar();
                Log("Finalizada thread MonitorarPosicoes");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "Monitorar posições");
            }
        }

        #endregion

        #region Finalizar Threads Integrações

        public void FinalizarThreadsIntegracoes()
        {
            Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoOnixSat.GetInstance(cliente).Finalizar();
            Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoSascar.GetInstance(cliente).Finalizar();
            Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoOmnilink.GetInstance(cliente).Finalizar();
            Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoAutotrac.GetInstance(cliente).Finalizar();
            Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoTrafegus.GetInstance(cliente).Finalizar();
            Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoOpentech.GetInstance(cliente).Finalizar();
            Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoSighra.GetInstance(cliente).Finalizar();
            Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoRavex.GetInstance(cliente).Finalizar();
            Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoSmartTracking.GetInstance(cliente).Finalizar();
            Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoMixTelematics.GetInstance(cliente).Finalizar();
            Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoPositron.GetInstance(cliente).Finalizar();
            Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoGVSAT.GetInstance(cliente).Finalizar();
            Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoKaboo.GetInstance(cliente).Finalizar();
            Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoSystemSat.GetInstance(cliente).Finalizar();
            Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoGetrak.GetInstance(cliente).Finalizar();
            Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoVtrack.GetInstance(cliente).Finalizar();
            Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoA2.GetInstance(cliente).Finalizar();
            Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoSpytruck.GetInstance(cliente).Finalizar();
            Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoRastreamentoLink.GetInstance(cliente).Finalizar();
            Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoCSX.GetInstance(cliente).Finalizar();
            Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoTraffilog.GetInstance(cliente).Finalizar();
            Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoNSTech.GetInstance(cliente).Finalizar();
            Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoIturan.GetInstance(cliente).Finalizar();
            Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoTresS.GetInstance(cliente).Finalizar();
            Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoSkyWorld.GetInstance(cliente).Finalizar();
            Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoSigaSul.GetInstance(cliente).Finalizar();
            Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoABFSat.GetInstance(cliente).Finalizar();
            Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoAutovision.GetInstance(cliente).Finalizar();
            Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoWebRotas.GetInstance(cliente).Finalizar();
            Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoLogRisk.GetInstance(cliente).Finalizar();
            Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoMaxtrack.GetInstance(cliente).Finalizar();
            Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoTransPanorama.GetInstance(cliente).Finalizar();
            Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoTrixlog.GetInstance(cliente).Finalizar();
            Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoTrustTrack.GetInstance(cliente).Finalizar();
            Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoAutotracEmbarcador.GetInstance(cliente).Finalizar();
            Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoGNSBrasil.GetInstance(cliente).Finalizar();
            Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoEagleTrack.GetInstance(cliente).Finalizar();
            Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoCCNTelematica.GetInstance(cliente).Finalizar();
            Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoAngelLira.GetInstance(cliente).Finalizar();
            Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoRaster.GetInstance(cliente).Finalizar();
            Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoBrasilRiskPosicoes.GetInstance(cliente).Finalizar();
            Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoBrasilRiskPosicoesPlaca.GetInstance(cliente).Finalizar();
            Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoRotaExata.GetInstance(cliente).Finalizar();
            Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoRastrear.GetInstance(cliente).Finalizar();
            Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoApisulLog.GetInstance(cliente).Finalizar();
            Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoViaLink.GetInstance(cliente).Finalizar();
            Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoBuonny.GetInstance(cliente).Finalizar();
            Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoMultiPortal.GetInstance(cliente).Finalizar();
            Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoRastreamentoEvo.GetInstance(cliente).Finalizar();
            Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoUnitop.GetInstance(cliente).Finalizar();
            Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoTotalTrac.GetInstance(cliente).Finalizar();
            Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoCobli.GetInstance(cliente).Finalizar();
            Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoOmnicomm.GetInstance(cliente).Finalizar();
        }

        #endregion

        #region Metodos Privados

        private void VerificaAmbiente(string ambiente)
        {
            Log($"Ambiente {ambiente}");
        }

        private void VerificaVersao()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            System.Diagnostics.FileVersionInfo fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);

            Log($"Versao {fvi.FileVersion}");

            GravarVersaoBanco(fvi, assembly);
        }

        private void GravarVersaoBanco(FileVersionInfo fvi, System.Reflection.Assembly assembly)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);

            try
            {
                string version = $"{fvi.FileMajorPart}.{fvi.FileMinorPart:00}{(fvi.FilePrivatePart > 0 ? $".{fvi.FileBuildPart}.{fvi.FilePrivatePart:00}" : "")}";

                unitOfWork.Start();
                Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramento repositorioConfiguracaoMonitoramento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramento(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramento configuracaoMonitoramento = repositorioConfiguracaoMonitoramento.BuscarConfiguracaoPadrao();
                configuracaoMonitoramento.VersaoMonitoramento = version;
                configuracaoMonitoramento.CaminhoCompletoMonitoramento = assembly.Location;
                repositorioConfiguracaoMonitoramento.Atualizar(configuracaoMonitoramento);
                unitOfWork.CommitChanges();
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex, "VersaoMonitoramento");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private void Log(string msg, bool dt = true)
        {
            Servicos.Embarcador.Monitoramento.MonitoramentoUtils.GravarLogTracking(msg, "ControleMonitoramento", dt);
        }

        private void LoadConfigurations()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);

            try
            {
#if !DEBUG
                //new Servicos.Embarcador.Configuracoes.Arquivo().AjustarConfiguracoes(unitOfWork);
                //new Servicos.Embarcador.Configuracoes.Arquivo().AjustarConfiguracoesMonitoramento(unitOfWork);

                Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork);
                Servicos.Embarcador.Configuracoes.ConfiguracaoMonioramentoInstance.GetInstance(unitOfWork);

#else
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
