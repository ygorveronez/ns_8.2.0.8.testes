using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Monitoramento.Eventos
{
    public class DesvioDeRota : AbstractEvento
    {

        #region Métodos públicos

        public DesvioDeRota(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta.DesvioDeRota)
        {

        }

        /**
         * Confirma que o evento está ativo e com as configurações mínimas
         */
        public new bool EstaAtivo()
        {
            return base.EstaAtivo() && this.ListaMonitoramentoEvento.Any(x => x.Gatilho?.Tempo != null) && this.ListaMonitoramentoEvento.Any(x => x.Gatilho?.Raio != null);
        }

        /**
         * Processa a posição atual recebida para gerar um possível alerta
         */
        public override void ProcessarEvento(Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoProcessarEvento monitoramentoProcessarEvento, Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento, List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoesObjetoValor, List<Dominio.ObjetosDeValor.Embarcador.Logistica.AlertaMonitor> alertas, List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> entregas, List<double> codigosClientesAlvo, Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento> cargaJanelasDescarregamento)
        {
            // Não está em um raio de cliente, deve haver uma carga com monitoramento e com rota prevista
            if (monitoramentoProcessarEvento != null &&
                monitoramentoProcessarEvento.DataVeiculoPosicao != null &&
                monitoramentoProcessarEvento.LatitudePosicao != null &&
                monitoramentoProcessarEvento.LongitudePosicao != null &&
                !EmAlvoEntrega(codigosClientesAlvo, entregas) &&
                monitoramento != null &&
                monitoramento.PolilinhaPrevista != null)
            {

                Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint waypoint = new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint(monitoramentoProcessarEvento.LatitudePosicao.Value, monitoramentoProcessarEvento.LongitudePosicao.Value);
                int menorDistanciaDaRota = (int)Servicos.Embarcador.Logistica.Polilinha.CalcularDistanciaEntrePontoERota(waypoint, monitoramento.PolilinhaPrevista);
                if (menorDistanciaDaRota > this.MonitoramentoEvento.Gatilho.Raio)
                {
                    // Cria o alerta para a carga se não existir algum
                    NumberFormatInfo nfi = new NumberFormatInfo();
                    nfi.NumberDecimalSeparator = ",";
                    string texto = (menorDistanciaDaRota < 1000) ? menorDistanciaDaRota + "m" : Math.Round((double)(menorDistanciaDaRota / 1000), 1).ToString(nfi) + "Km";
                    base.CriarAlertaSeNaoExistir(alertas, monitoramentoProcessarEvento, monitoramento?.Carga ?? null, entregas, texto);

                }

            }

        }

        public override void ExecutarDepoisDeCriarAlerta(Dominio.Entidades.Embarcador.Logistica.AlertaMonitor alerta, Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoProcessarEvento monitoramentoProcessarEvento) { }

        #endregion

    }
}