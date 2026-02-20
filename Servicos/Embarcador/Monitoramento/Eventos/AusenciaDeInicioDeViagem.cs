using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Monitoramento.Eventos
{
    public class AusenciaDeInicioDeViagem : AbstractEvento
    {

        #region Métodos públicos

        public AusenciaDeInicioDeViagem(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta.AusenciaDeInicioDeViagem)
        {

        }

        /**
         * Confirma que o evento está ativo e com as configurações mínimas
         */
        public new bool EstaAtivo()
        {
            return base.EstaAtivo() && this.ListaMonitoramentoEvento.Any(x => x.Gatilho?.Tempo != null) && this.ListaMonitoramentoEvento.Any(x => x.Gatilho?.TempoEvento != null) && this.ListaMonitoramentoEvento.Any(x => x.Gatilho?.TempoEvento2 != null);
        }

        /**
         * Processa a posição atual recebida para gerar um possível alerta
         */
        public override void ProcessarEvento(Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoProcessarEvento monitoramentoProcessarEvento, Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento, List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoesObjetoValor, List<Dominio.ObjetosDeValor.Embarcador.Logistica.AlertaMonitor> alertas, List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> entregas, List<double> codigosClientesAlvo, Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento> cargaJanelasDescarregamento)
        {
            if (monitoramento != null && monitoramento.Carga != null)
            {
                if (monitoramento.Carga?.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte &&
                    (bool)monitoramento.Carga?.DataMudouSituacaoParaEmTransporte.HasValue &&
                    !monitoramentoProcessarEvento.DataInicioViagem.HasValue) {

                    DateTime? dataBase = monitoramento.Carga.DataMudouSituacaoParaEmTransporte;
                    DateTime? dataReferencia = DateTime.Now;

                    if (dataBase < dataReferencia)
                    {
                        TimeSpan atraso = dataReferencia.Value - dataBase.Value;
                        if (atraso.TotalMinutes > this.MonitoramentoEvento.Gatilho.TempoEvento)
                        {
                            string texto = Servicos.Embarcador.Monitoramento.AlertaMonitor.FormatarTempo(atraso) + " horas";
                            base.CriarAlertaSeNaoExistir(alertas, monitoramentoProcessarEvento, monitoramento.Carga, entregas, texto, false);
                        }
                    }
                }
            }
        }

        public override void ExecutarDepoisDeCriarAlerta(Dominio.Entidades.Embarcador.Logistica.AlertaMonitor alerta, Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoProcessarEvento monitoramentoProcessarEvento) { }

        #endregion

    }
}