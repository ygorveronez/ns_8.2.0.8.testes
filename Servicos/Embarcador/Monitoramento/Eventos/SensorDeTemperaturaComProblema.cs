using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Monitoramento.Eventos
{
    public class SensorDeTemperaturaComProblema : AbstractEvento
    {

        #region Métodos públicos

        public SensorDeTemperaturaComProblema(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta.SensorTemperaturaComProblema)
        {

        }

        /**
         * Confirma que o evento está ativo e com as configurações mínimas
         */
        public new bool EstaAtivo()
        {
            return base.EstaAtivo() && this.ListaMonitoramentoEvento.Any(x => x.Gatilho?.Tempo != null);
        }

        /**
         * Processa a posição atual recebida para gerar um possível alerta
         */
        public override void ProcessarEvento(Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoProcessarEvento monitoramentoProcessarEvento, Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento, List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoesObjetoValor, List<Dominio.ObjetosDeValor.Embarcador.Logistica.AlertaMonitor> alertas, List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> entregas, List<double> codigosClientesAlvo, Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento> cargaJanelasDescarregamento)
        {

            // A carga deve controlar temperatura com algum veículo
            if (monitoramento != null && monitoramento.Carga != null && (monitoramento.Carga.TipoDeCarga?.ControlaTemperatura ?? false))
            {

                // Há problema com o sensor de temperatura
                if (!(monitoramentoProcessarEvento.SensorTemperaturaPosicao ?? false))
                {
                    TimeSpan atraso = DateTime.Now - monitoramentoProcessarEvento.DataVeiculoPosicao.Value;
                    double atrasoMinutos = atraso.TotalMinutes;

                    if (monitoramento != null && !monitoramentoProcessarEvento.CodigoMonitoramento.HasValue)
                        monitoramentoProcessarEvento.CodigoMonitoramento = monitoramento.Codigo;

                    if (atrasoMinutos > this.MonitoramentoEvento.Gatilho.TempoEvento)
                    {
                        if (base.ExisteAlertaAbertoOuFechadoHaPouco(monitoramentoProcessarEvento, monitoramento, alertas)) return;

                        // Cria o alerta para a carga se não existir algum
                        base.CriarAlertaSeNaoExistir(alertas, monitoramentoProcessarEvento, monitoramento?.Carga ?? null, entregas, "Falha");
                    }
                    else
                    {
                        if (monitoramentoProcessarEvento != null && !monitoramentoProcessarEvento.CodigoMonitoramento.HasValue)
                            return;

                        Servicos.Embarcador.Monitoramento.MonitoramentoEventoTratativaAutomatica serMonitoramentoEventoTratativaAutomatica = new Servicos.Embarcador.Monitoramento.MonitoramentoEventoTratativaAutomatica(unitOfWork);
                        serMonitoramentoEventoTratativaAutomatica.TratarEventoAutomaticamente(monitoramento.Carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TratativaAutomaticaMonitoramentoEvento.SensorTemperaturaComProblema);
                    }
                        
                }

            }

        }

        public override void ExecutarDepoisDeCriarAlerta(Dominio.Entidades.Embarcador.Logistica.AlertaMonitor alerta, Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoProcessarEvento monitoramentoProcessarEvento) { }

        #endregion

    }
}