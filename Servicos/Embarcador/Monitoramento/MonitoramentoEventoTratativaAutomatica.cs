using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Servicos.Embarcador.Monitoramento
{
    public class MonitoramentoEventoTratativaAutomatica
    {
        #region Atributos

        private Repositorio.UnitOfWork _unitOfWork;

        #endregion Atributos

        #region Construtores

        public MonitoramentoEventoTratativaAutomatica(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Metodos Publicos

        public void TratarEventoAutomaticamente(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TratativaAutomaticaMonitoramentoEvento tipoTratativa)
        {
            if (carga == null) return;

            Repositorio.Embarcador.Logistica.MonitoramentoEvento repMonitoramentoEvento = new Repositorio.Embarcador.Logistica.MonitoramentoEvento(_unitOfWork);
            Repositorio.Embarcador.Logistica.AlertaMonitor repAlertaMonitor = new Repositorio.Embarcador.Logistica.AlertaMonitor(_unitOfWork);
            Servicos.Embarcador.Monitoramento.AlertaMonitor servicoAlertaMonitor = new Servicos.Embarcador.Monitoramento.AlertaMonitor();

            //Busca todos os eventos com tratativa automatica do tipo.
            List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoEvento> listaEventos = repMonitoramentoEvento.BuscarEventosComTipoTratativaAutomatica(tipoTratativa);
            foreach (Dominio.Entidades.Embarcador.Logistica.MonitoramentoEvento evento in listaEventos)
            {
                //Busca os alertas que estão vinculados ao veiculo/monitoramento, tipo de alerta e evento.
                List<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> listaAlertas = repAlertaMonitor.BuscarAlertasEmAbertoPorCargaETipoDeAlerta(carga.Codigo, evento.TipoAlerta, evento.Codigo);

                //Processar tratativa automatica dos alertas com base no evento.
                servicoAlertaMonitor.FinalizarAlertas(listaAlertas, DateTime.Now, "Finalizado automaticamente via gatilho " + tipoTratativa.ObterDescricao(), _unitOfWork, null, false, null, null, true);
            }
        }

        #endregion

    }
}
