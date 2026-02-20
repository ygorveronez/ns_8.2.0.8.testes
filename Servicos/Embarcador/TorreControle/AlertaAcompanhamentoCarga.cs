using System;
using System.Collections.Generic;

namespace Servicos.Embarcador.TorreControle
{
    public class AlertaAcompanhamentoCarga
    {
        #region Atributos Privados

        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS _configuracaoEmbarcador;
        private readonly Repositorio.UnitOfWork _unitOfWork;


        #endregion Atributos Privados


        #region Construtores

        public AlertaAcompanhamentoCarga(Repositorio.UnitOfWork unitOfWork) : this(unitOfWork, configuracaoEmbarcador: null) { }

        public AlertaAcompanhamentoCarga(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador)
        {
            _unitOfWork = unitOfWork;
            _configuracaoEmbarcador = configuracaoEmbarcador;
        }

        #endregion

        #region Métodos públicos 

        public void AtualizarTratativaAlertaAcompanhamentoCarga(Dominio.Entidades.Embarcador.Logistica.AlertaMonitor alertaMonitoramento, Dominio.Entidades.Embarcador.Cargas.AlertaCarga.CargaEvento alertaCarga)
        {
            Repositorio.Embarcador.TorreControle.AlertaAcompanhamentoCarga repAlertaAcompanhamentoCarga = new Repositorio.Embarcador.TorreControle.AlertaAcompanhamentoCarga(_unitOfWork);
            if (alertaMonitoramento != null && alertaMonitoramento.Carga != null)
            {
                //buscar e tratar alerta acompanhamento pelo alerta do monitoramento
                Dominio.Entidades.Embarcador.TorreControle.AlertasAcompanhamentoCarga alertaAcompanhamento = repAlertaAcompanhamentoCarga.BuscarAlertaAbertoPorAlertaMonitoramento(alertaMonitoramento.Codigo);
                if (alertaAcompanhamento != null)
                {
                    alertaAcompanhamento.AlertaTratado = true;
                    repAlertaAcompanhamentoCarga.Atualizar(alertaAcompanhamento);
                }

                informarAtualizacaoCardCargaAcompanhamento(alertaMonitoramento.Carga);
            }

            if (alertaCarga != null && alertaCarga.Carga != null)
            {
                //buscar e tratar alerta acompanhamento pelo evento carga
                Dominio.Entidades.Embarcador.TorreControle.AlertasAcompanhamentoCarga alertaAcompanhamento = repAlertaAcompanhamentoCarga.BuscarAlertaAbertoAlertaEventoCarga(alertaCarga.Codigo);
                if (alertaAcompanhamento != null)
                {
                    alertaAcompanhamento.AlertaTratado = true;
                    repAlertaAcompanhamentoCarga.Atualizar(alertaAcompanhamento);
                }

                informarAtualizacaoCardCargaAcompanhamento(alertaCarga.Carga);
            }
        }


        /// <summary>
        /// Inserir novo card na tela
        /// </summary>
        /// <param name="Carga"></param>
        /// <param name="codigoClienteMultisoftware"></param>
        public void informarNovoCardCargasAcompanamentoMSMQ(Dominio.Entidades.Embarcador.Cargas.Carga Carga, int codigoClienteMultisoftware)
        {
            try
            {
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.TorreControle.CardAcompanhamentoCarga.Carga objetoCard = repositorioCarga.ConsultarCargasAcompanhamentoCargaPorCarga(Carga.Codigo);

                Dominio.MSMQ.Notification notification = new Dominio.MSMQ.Notification(objetoCard, codigoClienteMultisoftware, Dominio.MSMQ.MSMQQueue.SGTWebAdmin, Dominio.SignalR.Hubs.AcompanhamentoCarga, Servicos.SignalR.Hubs.AcompanhamentoCarga.GetHub(Servicos.SignalR.Hubs.AcompanhamentoCargaHubs.CargaInserida));
                Servicos.MSMQ.MSMQ.SendPrivateMessage(notification);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "MessageQueue");
            }
        }

        /// <summary>
        /// Atualização do card, chamados pelo projeto SGT.WebAdmin
        /// </summary>
        /// <param name="Carga"></param>
        public void informarAtualizacaoCardCargaAcompanhamento(Dominio.Entidades.Embarcador.Cargas.Carga Carga)
        {
            if (Carga == null)
                return;

            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);

            Servicos.SignalR.Hubs.AcompanhamentoCarga hubAcompanhamentoCarga = new SignalR.Hubs.AcompanhamentoCarga();

            Dominio.ObjetosDeValor.Embarcador.TorreControle.CardAcompanhamentoCarga.Carga objetoCard = repositorioCarga.ConsultarCargasAcompanhamentoCargaPorCarga(Carga.Codigo);
            if (objetoCard != null)
                hubAcompanhamentoCarga.InformarCardAtualizado(objetoCard, false);
        }

        /// <summary>
        /// Atualizacao do Card, Quando chamado por WEBSevices e projetos secundarios
        /// </summary>
        /// <param name="Carga"></param>
        /// <param name="codigoClienteMultisoftware"></param>
        public void informarAtualizacaoCardCargasAcompanamentoMSMQ(Dominio.Entidades.Embarcador.Cargas.Carga Carga, int codigoClienteMultisoftware)
        {
            if (Carga == null)
                return;

            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Dominio.ObjetosDeValor.Embarcador.TorreControle.CardAcompanhamentoCarga.Carga objetoCard = repositorioCarga.ConsultarCargasAcompanhamentoCargaPorCarga(Carga.Codigo);

            Dominio.MSMQ.Notification notification = new Dominio.MSMQ.Notification(objetoCard, codigoClienteMultisoftware, Dominio.MSMQ.MSMQQueue.SGTWebAdmin, Dominio.SignalR.Hubs.AcompanhamentoCarga, Servicos.SignalR.Hubs.AcompanhamentoCarga.GetHub(Servicos.SignalR.Hubs.AcompanhamentoCargaHubs.CargaAtualizada));
            Servicos.MSMQ.MSMQ.SendPrivateMessage(notification);
        }

        /// <summary>
        /// Atualizacao de N cards, Quando chamado por WEBSevices e projetos secundarios
        /// </summary>
        /// <param name="Carga"></param>
        /// <param name="codigoClienteMultisoftware"></param>
        public void informarAtualizacaoListaCargasAcompanamentoMSMQ(List<int> ListaCodigosCarga, int codigoClienteMultisoftware)
        {
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            IList<Dominio.ObjetosDeValor.Embarcador.TorreControle.CardAcompanhamentoCarga.Carga> ListaCards = repositorioCarga.ConsultarCargasAcompanhamentoCargaPorListaCarga(ListaCodigosCarga);

            dynamic objeto = new System.Dynamic.ExpandoObject();
            objeto.objetoCard = ListaCards;

            Dominio.MSMQ.Notification notification = new Dominio.MSMQ.Notification(objeto, codigoClienteMultisoftware, Dominio.MSMQ.MSMQQueue.SGTWebAdmin, Dominio.SignalR.Hubs.AcompanhamentoCarga, Servicos.SignalR.Hubs.AcompanhamentoCarga.GetHub(Servicos.SignalR.Hubs.AcompanhamentoCargaHubs.ListaCargasAtualizadas));
            Servicos.MSMQ.MSMQ.SendPrivateMessage(notification);
        }

        #endregion



    }
}
