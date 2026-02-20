namespace Servicos.Embarcador.GestaoEntregas
{
    public class EventoEntrega
    {
        #region Atributos Privados

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware _tipoServicoMultisoftware;
        private readonly AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente _cliente;

        #endregion

        #region Construtores

        public EventoEntrega(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente)
        {
            _unitOfWork = unitOfWork;
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
            _cliente = cliente;
        }


        public void VerificarIntegracoesEventosEntrega()
        {
            //buscar integracoes que devem integrar eventos de entregas;
            Repositorio.Embarcador.Configuracoes.IntegracaoVLI repositorioConfiguracaoIntegracaoVLI = new Repositorio.Embarcador.Configuracoes.IntegracaoVLI(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoVLI configuracaoIntegracaovli = repositorioConfiguracaoIntegracaoVLI.Buscar();

            if (configuracaoIntegracaovli?.PossuiIntegracaoVLI ?? false)
            {
                Servicos.Embarcador.Integracao.VLI.IntegracaoVLI servicoIntegracaoVLI = new Servicos.Embarcador.Integracao.VLI.IntegracaoVLI(_unitOfWork, _tipoServicoMultisoftware, _cliente);
                servicoIntegracaoVLI.IntegrarEventosCarregamentoVLI();
                servicoIntegracaoVLI.IntegrarEventosRastreamentoVLI();
                servicoIntegracaoVLI.IntegrarEventosDescarregamentoVLI();

            }

        }

        #endregion


    }
}
