namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public class RetornoCargaPossuiOperacao
    {
        public bool TabelaQuePossuiOperacao { get; set; }
        public bool TabelaQueNaoPossuiOperacao { get; set; }
        public bool PossuiIntegracao { get; set; }
        public bool PossuiObservacao { get; set; }
        public bool PermitirTransportadorReenviarIntegracoesComProblemasOpenTech { get; set; }
        public bool PortalMultiTransportador { get; set; }
        public bool PermitirVisualizarOrdernarZonasTransporte { get; set; }
        public bool PermitirVisualizarTipoCarregamento { get; set; }
        public bool PermitirVisualizarCentroResultado { get; set; }
        public bool PermitirInformarAjudantesNaCarga { get; set; }
        public bool PossuiIntegracaoIntegrada { get; set; }
        public bool PossuiIntegracaoMotorista { get; set; }
        public bool PossuiIntegracaoVeiculo { get; set; }
        public bool ExigePlacaTracao { get; set; }
        public bool NaoPermitirAlterarMotoristaAposAverbacaoContainer { get; set; }
    }
}
