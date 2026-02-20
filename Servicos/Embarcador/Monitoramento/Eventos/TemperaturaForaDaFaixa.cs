using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Monitoramento.Eventos
{
    public class TemperaturaForaDaFaixa : AbstractEvento
    {

        #region Métodos públicos

        public TemperaturaForaDaFaixa(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta.TemperaturaForaDaFaixa)
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
            if (monitoramento != null && monitoramento.Carga != null && (monitoramento.Carga?.TipoDeCarga?.ControlaTemperatura ?? false) && monitoramento.Carga?.TipoDeCarga?.FaixaDeTemperatura != null)
            {

                // A temperatura observada está fora da faixa?
                if (EstaForaDaFaixa(monitoramentoProcessarEvento.TemperaturaPosicao, monitoramento.Carga.FaixaTemperatura ?? monitoramento.Carga.TipoDeCarga.FaixaDeTemperatura))
                {

                    // Tempo que a temperatura deve ficar fora da faixa sem interrupção
                    // Não tem tempo configurado ou há um tempo configurado e ficou este tempo 
                    if (this.MonitoramentoEvento.Gatilho.TempoEvento == 0 || ManteveTemperaturaForaDaFaixa(monitoramentoProcessarEvento, posicoesObjetoValor, alertas, monitoramento.Carga, this.MonitoramentoEvento.Gatilho.TempoEvento))
                    {
                        // Cria o alerta para a carga se não existir algum
                        string texto = (monitoramentoProcessarEvento.TemperaturaPosicao != null) ? monitoramentoProcessarEvento.TemperaturaPosicao.Value.ToString() + "°C" : "Temperatura ausente";
                        base.CriarAlertaSeNaoExistir(alertas, monitoramentoProcessarEvento, monitoramento.Carga, entregas, texto);
                    }

                }

            }

        }

        public override void ExecutarDepoisDeCriarAlerta(Dominio.Entidades.Embarcador.Logistica.AlertaMonitor alerta, Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoProcessarEvento monitoramentoProcessarEvento) { }

        #endregion

        #region Métodos privados

        protected bool ManteveTemperaturaForaDaFaixa(Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoProcessarEvento monitoramentoProcessarEvento, List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoesObjetoValor, List<Dominio.ObjetosDeValor.Embarcador.Logistica.AlertaMonitor> alertas, Dominio.Entidades.Embarcador.Cargas.Carga carga, int tempoMinutos)
        {

            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoesDoPeriodo = ExtrairPosicoesAposAlertaFechadoAtePosicao(monitoramentoProcessarEvento, posicoesObjetoValor, alertas);

            // Percorre em ordem inversa, das posições mais atuais para as mais antigas
            for (int i = posicoesDoPeriodo.Count - 1; i >= 0; i--)
            {
                if (EstaForaDaFaixa(posicoesDoPeriodo[i].Temperatura, carga.FaixaTemperatura ?? carga.TipoDeCarga.FaixaDeTemperatura))
                {
                    TimeSpan tempo = monitoramentoProcessarEvento.DataVeiculoPosicao.Value - posicoesDoPeriodo[i].DataVeiculo;
                    if (tempo.TotalMinutes > tempoMinutos)
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            return false;
        }


        private bool EstaForaDaFaixa(decimal? temperatura, Dominio.Entidades.Embarcador.Cargas.FaixaTemperatura faixa)
        {
            return (temperatura == null || temperatura < faixa.FaixaInicial || temperatura > faixa.FaixaFinal);
        }
        #endregion

    }

}
