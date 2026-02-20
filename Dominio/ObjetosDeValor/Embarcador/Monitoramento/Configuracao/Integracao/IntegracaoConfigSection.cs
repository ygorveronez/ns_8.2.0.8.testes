using System.Configuration;

namespace Dominio.ObjetosDeValor.Embarcador.Monitoramento.Configuracao.Integracao
{
    public class IntegracaoConfigSection : ConfigurationSection
    {
        private const string HabilitadaKey = "Habilitada";
        private const string TempoSleepThreadKey = "TempoSleepThread";
        private const string TempoSleepEntreContasKey = "TempoSleepEntreContas";
        private const string MinutosDiferencaMinimaEntrePosicoesKey = "MinutosDiferencaMinimaEntrePosicoes";
        private const string CodigoIntegracaoKey = "CodigoIntegracao";
        private const string ContasKey = "Contas";
        private const string OpcoesKey = "Opcoes";
        private const string MonitorarKey = "Monitorar";
        private const string ProcessarSensoresKey = "ProcessaSensores";

        [ConfigurationProperty(HabilitadaKey, IsRequired = false)]
        public bool Habilitada => (bool)this[HabilitadaKey];

        [ConfigurationProperty(TempoSleepThreadKey, IsRequired = true)]
        public int TempoSleepThread => (int)this[TempoSleepThreadKey];

        [ConfigurationProperty(TempoSleepEntreContasKey, IsRequired = true)]
        public int TempoSleepEntreContas => (int)this[TempoSleepEntreContasKey];

        [ConfigurationProperty(MinutosDiferencaMinimaEntrePosicoesKey, IsRequired = true)]
        public int MinutosDiferencaMinimaEntrePosicoes => (int)this[MinutosDiferencaMinimaEntrePosicoesKey];

        [ConfigurationProperty(CodigoIntegracaoKey, IsRequired = true)]
        public string CodigoIntegracao => (string)this[CodigoIntegracaoKey];

        [ConfigurationProperty(ContasKey, IsRequired = true)]
        public ContaElementCollection Contas => (ContaElementCollection)this[ContasKey];

        [ConfigurationProperty(OpcoesKey, IsRequired = false)]
        public OpcaoElementCollection Opcoes => (OpcaoElementCollection)this[OpcoesKey];

        [ConfigurationProperty(MonitorarKey, IsRequired = false)]
        public OpcaoElementCollection Monitorar => (OpcaoElementCollection)this[MonitorarKey];

        [ConfigurationProperty(ProcessarSensoresKey, IsRequired = false)]
        public bool ProcessarSensores => (bool)this[ProcessarSensoresKey];

    }
}
