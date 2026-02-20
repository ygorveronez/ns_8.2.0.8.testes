namespace Servicos.Embarcador.Carga.AlertaCarga
{
    public class AlertaCargaEvento
    {
        #region Atributos Privados

        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS _configuracaoEmbarcador;
        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion Atributos Privados


        #region Construtores

        public AlertaCargaEvento(Repositorio.UnitOfWork unitOfWork) : this(unitOfWork, configuracaoEmbarcador: null) { }

        public AlertaCargaEvento(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador)
        {
            _unitOfWork = unitOfWork;
            _configuracaoEmbarcador = configuracaoEmbarcador;
        }

        #endregion

        #region Métodos públicos 

        public void EfetuarTratativaCargaEvento(Dominio.Entidades.Embarcador.Cargas.AlertaCarga.CargaEvento cargaEvento, string ObservacaoFinalizacao)
        {
            Repositorio.Embarcador.Cargas.AlertaCarga.CargaEvento repCargaEvento = new Repositorio.Embarcador.Cargas.AlertaCarga.CargaEvento(_unitOfWork);
            cargaEvento.Status = Dominio.ObjetosDeValor.Embarcador.Enumeradores.AlertaMonitorStatus.Finalizado;
            cargaEvento.Observacao = ObservacaoFinalizacao;

            repCargaEvento.Atualizar(cargaEvento);
        }

        #endregion

    }
}
