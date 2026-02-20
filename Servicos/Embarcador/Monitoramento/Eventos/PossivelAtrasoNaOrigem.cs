using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Monitoramento.Eventos
{
    public class PossivelAtrasoNaOrigem : AbstractEvento
    {

        #region Métodos públicos

        public PossivelAtrasoNaOrigem(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta.PossivelAtrasoNaOrigem)
        {

        }

        /**
         * Confirma que o evento está ativo e com as configurações mínimas
         */
        public new bool EstaAtivo()
        {
            return base.EstaAtivo() && this.ListaMonitoramentoEvento.Any(x => x.Gatilho?.Tempo != null) && this.ListaMonitoramentoEvento.Any(x => x.Gatilho?.DataBase != null) && this.ListaMonitoramentoEvento.Any(x => x.Gatilho?.DataReferencia != null);
        }

        /**
         * Processa a posição atual recebida para gerar um possível alerta
         */
        public override void ProcessarEvento(Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoProcessarEvento monitoramentoProcessarEvento, Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento, List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoesObjetoValor, List<Dominio.ObjetosDeValor.Embarcador.Logistica.AlertaMonitor> alertas, List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> entregas, List<double> codigosClientesAlvo, Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento> cargaJanelasDescarregamento)
        {
            if (monitoramento != null && monitoramento.Carga != null &&
                (!this.MonitoramentoEvento.Gatilho.ValidarApenasCargasNaoIniciadas || !monitoramentoProcessarEvento.DataInicioViagem.HasValue) )
            {
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega entrega = entregas.Find(x => x.Coleta);
                if (entrega != null)
                {
                    DateTime? dataBase = BuscarDataMonitoramentoEvento(this.MonitoramentoEvento.Gatilho.DataBase, monitoramentoProcessarEvento, monitoramento, entrega, cargaJanelaCarregamento, cargaJanelasDescarregamento);
                    if (dataBase == null) dataBase = entrega.DataReprogramada;

                    if (entrega.Coleta && dataBase != null)
                    {
                        int tempoAtraso = this.MonitoramentoEvento.Gatilho.TempoEvento;

                        DateTime? dataReferencia = BuscarDataMonitoramentoEvento(this.MonitoramentoEvento.Gatilho.DataReferencia, monitoramentoProcessarEvento, monitoramento, entrega, cargaJanelaCarregamento, cargaJanelasDescarregamento);

                        if (dataReferencia != null && dataBase.Value > dataReferencia.Value)
                        {
                            TimeSpan atraso = dataBase.Value - dataReferencia.Value;
                            if (atraso.TotalMinutes > tempoAtraso)
                            {
                                string texto = $"Possível atraso: " + Servicos.Embarcador.Monitoramento.AlertaMonitor.FormatarTempo(atraso);
                                base.CriarAlertaSeNaoExistir(alertas, monitoramentoProcessarEvento, monitoramento.Carga, new List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> { entrega }, texto, false);
                            }
                        }
                    }
                }
            }
        }

        public override void ExecutarDepoisDeCriarAlerta(Dominio.Entidades.Embarcador.Logistica.AlertaMonitor alerta, Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoProcessarEvento monitoramentoProcessarEvento) { }

        #endregion

    }
}