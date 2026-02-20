namespace Monitoramento
{
    public class ConfiguracaoRefreshService : WindowsBackgroundServiceBase
    {
        private readonly TimeSpan _intervalo;
        private Repositorio.UnitOfWork _unitOfWork;

        public ConfiguracaoRefreshService(ILogger<ConfiguracaoRefreshService> logger, IConfiguration configuration) : base(null, logger, configuration)
        {
            _intervalo = TimeSpan.FromHours(1);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Servicos.Log.TratarErro($"Serviço de atualização de configurações iniciado. Intervalo: {_intervalo}", "ConfiguracaoRefreshService");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    Servicos.Log.TratarErro("Iniciando atualização das configurações do banco de dados", "ConfiguracaoRefreshService");

                    using (_unitOfWork = new Repositorio.UnitOfWork(StringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
                    {
                        Servicos.Embarcador.Configuracoes.ConfigurationInstance configurationInstance = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(_unitOfWork);
                        configurationInstance.AtualizarConfiguracoes(_unitOfWork);
                        Servicos.Log.TratarErro("Configurações gerais atualizadas com sucesso", "ConfiguracaoRefreshService");

                        Servicos.Embarcador.Configuracoes.ConfiguracaoMonioramentoInstance configInstance = Servicos.Embarcador.Configuracoes.ConfiguracaoMonioramentoInstance.GetInstance(_unitOfWork);
                        configInstance.AtualizarConfiguracoes(_unitOfWork);
                        Servicos.Log.TratarErro("Configurações de monitoramento atualizadas com sucesso", "ConfiguracaoRefreshService");

                        AtualizarConfiguracoesIntegracoes(_unitOfWork);
                    }

                    Servicos.Log.TratarErro($"Atualização das configurações finalizada. Próxima atualização em {_intervalo}", "ConfiguracaoRefreshService");
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex, "ConfiguracaoRefreshService");
                    _logger.LogError(ex, "{Message}", ex.Message);
                    Environment.Exit(1);
                }

                await Task.Delay(_intervalo, stoppingToken);
            }
        }

        private void AtualizarConfiguracoesIntegracoes(Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente = Cliente;
                if (cliente == null)
                {
                    Servicos.Log.TratarErro("Cliente não encontrado para atualizar configurações das integrações", "ConfiguracaoRefreshService");
                    return;
                }

                Servicos.Log.TratarErro("Iniciando atualização das configurações de integrações", "ConfiguracaoRefreshService");

                List<Servicos.Embarcador.Monitoramento.Integracoes.Abstract.AbstractIntegracao> integracoes = new List<Servicos.Embarcador.Monitoramento.Integracoes.Abstract.AbstractIntegracao>
                {
                    Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoOnixSat.GetInstance(cliente),
                    Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoSascar.GetInstance(cliente),
                    Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoOmnilink.GetInstance(cliente),
                    Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoAutotrac.GetInstance(cliente),
                    Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoTrafegus.GetInstance(cliente),
                    Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoOpentech.GetInstance(cliente),
                    Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoSighra.GetInstance(cliente),
                    Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoRavex.GetInstance(cliente),
                    Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoSmartTracking.GetInstance(cliente),
                    Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoMixTelematics.GetInstance(cliente),
                    Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoPositron.GetInstance(cliente),
                    Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoGVSAT.GetInstance(cliente),
                    Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoKaboo.GetInstance(cliente),
                    Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoSystemSat.GetInstance(cliente),
                    Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoGetrak.GetInstance(cliente),
                    Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoVtrack.GetInstance(cliente),
                    Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoA2.GetInstance(cliente),
                    Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoSpytruck.GetInstance(cliente),
                    Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoRastreamentoLink.GetInstance(cliente),
                    Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoCSX.GetInstance(cliente),
                    Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoTraffilog.GetInstance(cliente),
                    Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoNSTech.GetInstance(cliente),
                    Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoIturan.GetInstance(cliente),
                    Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoTresS.GetInstance(cliente),
                    Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoSkyWorld.GetInstance(cliente),
                    Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoSigaSul.GetInstance(cliente),
                    Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoABFSat.GetInstance(cliente),
                    Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoAutovision.GetInstance(cliente),
                    Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoWebRotas.GetInstance(cliente),
                    Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoLogRisk.GetInstance(cliente),
                    Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoMaxtrack.GetInstance(cliente),
                    Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoTransPanorama.GetInstance(cliente),
                    Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoTrixlog.GetInstance(cliente),
                    Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoTrustTrack.GetInstance(cliente),
                    Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoAutotracEmbarcador.GetInstance(cliente),
                    Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoGNSBrasil.GetInstance(cliente),
                    Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoEagleTrack.GetInstance(cliente),
                    Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoCCNTelematica.GetInstance(cliente),
                    Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoAngelLira.GetInstance(cliente),
                    Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoRaster.GetInstance(cliente),
                    Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoBrasilRiskPosicoes.GetInstance(cliente),
                    Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoBrasilRiskPosicoesPlaca.GetInstance(cliente),
                    Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoRotaExata.GetInstance(cliente),
                    Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoRastrear.GetInstance(cliente),
                    Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoApisulLog.GetInstance(cliente),
                    Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoViaLink.GetInstance(cliente),
                    Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoBuonny.GetInstance(cliente),
                    Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoMultiPortal.GetInstance(cliente),
                    Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoRastreamentoEvo.GetInstance(cliente),
                    Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoUnitop.GetInstance(cliente),
                    Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoTotalTrac.GetInstance(cliente),
                    Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoCobli.GetInstance(cliente),
                    Servicos.Embarcador.Monitoramento.Integracoes.IntegracaoOmnicomm.GetInstance(cliente),
                };

                foreach (Servicos.Embarcador.Monitoramento.Integracoes.Abstract.AbstractIntegracao integracao in integracoes)
                {
                    try
                    {
                        integracao.AtualizarConfiguracoes(unitOfWork);
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro(ex, "ConfiguracaoRefreshService");
                    }
                }

                Servicos.Log.TratarErro("Configurações das integrações atualizadas", "ConfiguracaoRefreshService");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "ConfiguracaoRefreshService");
            }
        }
    }
}