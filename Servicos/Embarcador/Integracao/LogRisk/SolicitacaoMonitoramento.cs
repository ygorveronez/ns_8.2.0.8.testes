namespace Servicos.Embarcador.Integracao.LogRisk
{
    public sealed class SolicitacaoMonitoramento
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware _tipoServicoMultisoftware;
        private Dominio.Entidades.Embarcador.Configuracoes.Integracao _configuracaoIntegracao;

        #endregion

        #region Construtores

        public SolicitacaoMonitoramento(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            _unitOfWork = unitOfWork;
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
        }

        #endregion

        #region Métodos Privados

        public void IntegrarCarga(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao)
        {
            if (cargaIntegracao.TipoIntegracao.IntegrarComPlataformaNstech)
            { 
                Servicos.Embarcador.Integracao.Nstech.IntegracaoSM svcIntegracaoSMNstech = new Servicos.Embarcador.Integracao.Nstech.IntegracaoSM(_tipoServicoMultisoftware, _unitOfWork);
                svcIntegracaoSMNstech.IntegrarSM(cargaIntegracao);
            }
        }
        #endregion

    }
}