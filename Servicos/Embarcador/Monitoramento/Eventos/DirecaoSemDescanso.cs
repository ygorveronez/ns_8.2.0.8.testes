using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Monitoramento.Eventos
{
    public class DirecaoSemDescanso : AbstractParada
    {

        #region Métodos públicos

        public DirecaoSemDescanso(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta.DirecaoSemDescanso)
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

            // Deve haver alguma temperatura lida, a carga deve controlar temperatura, com faixa cadastrada e com algum veículo
            if (monitoramento != null)
            {
                TimeSpan? tempoDirecao = TempoEmMovimentoContinuoSemParada(monitoramentoProcessarEvento, posicoesObjetoValor, alertas, this.MonitoramentoEvento.Gatilho.TempoEvento, this.MonitoramentoEvento.Gatilho.TempoEvento2, this.MonitoramentoEvento.Gatilho.Raio);
                if (tempoDirecao != null)
                {
                    string texto = Servicos.Embarcador.Monitoramento.AlertaMonitor.FormatarTempo(tempoDirecao.Value);
                    base.CriarAlertaSeNaoExistir(alertas, monitoramentoProcessarEvento, monitoramento.Carga, entregas, texto);
                }
            }

        }

        public override void ExecutarDepoisDeCriarAlerta(Dominio.Entidades.Embarcador.Logistica.AlertaMonitor alerta, Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoProcessarEvento monitoramentoProcessarEvento) { }

        #endregion

    }

}
