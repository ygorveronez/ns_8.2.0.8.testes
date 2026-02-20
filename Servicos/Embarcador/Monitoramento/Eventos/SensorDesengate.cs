using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Monitoramento.Eventos
{
    public class SensorDesengate : AbstractEvento
    {

        #region Métodos públicos

        public SensorDesengate(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta.SensorDesengate)
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
            if (monitoramento != null && monitoramento.Carga != null)
            {
                Dominio.ObjetosDeValor.Embarcador.Logistica.PosicaoAlertaSensor posicaoParaAlerta = buscarPosicaoAlertaSensorDesengate(monitoramentoProcessarEvento, posicoesObjetoValor, alertas, monitoramento.Carga, this.MonitoramentoEvento.Gatilho.TempoEvento);

                if (posicaoParaAlerta != null)
                {
                    Repositorio.Veiculo repveiculo = new Repositorio.Veiculo(this.unitOfWork);
                    Dominio.Entidades.Veiculo veiculo = repveiculo.BuscarPorCodigo(posicaoParaAlerta.IDVeiculo);

                    string texto = $"Desengate Ativado! Veículo: {veiculo.Placa} em " + posicaoParaAlerta.DataVeiculo.ToString("dd/MM/yyyy HH:mm");
                    base.CriarAlertaSeNaoExistir(alertas, monitoramentoProcessarEvento, monitoramento.Carga, entregas, texto);
                }
            }
        }

        public override void ExecutarDepoisDeCriarAlerta(Dominio.Entidades.Embarcador.Logistica.AlertaMonitor alerta, Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoProcessarEvento monitoramentoProcessarEvento) { }

        #endregion

        #region MetodosPrivados

        protected Dominio.ObjetosDeValor.Embarcador.Logistica.PosicaoAlertaSensor buscarPosicaoAlertaSensorDesengate(Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoProcessarEvento monitoramentoProcessarEvento, List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoesObjetoValor, List<Dominio.ObjetosDeValor.Embarcador.Logistica.AlertaMonitor> alertas, Dominio.Entidades.Embarcador.Cargas.Carga carga, int tempoMinutos)
        {
            Repositorio.Embarcador.Logistica.PosicaoAlertaSensor repPosicoesAlertaSensor = new Repositorio.Embarcador.Logistica.PosicaoAlertaSensor(this.unitOfWork);

            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoesDoPeriodo = ExtrairPosicoesAposAlertaFechadoAtePosicao(monitoramentoProcessarEvento, posicoesObjetoValor, alertas);
            DateTime dataFimPeriodo = posicoesDoPeriodo.OrderBy(x => x.DataVeiculo).Select(x => x.DataVeiculo).LastOrDefault();
            DateTime dataInicioPeriodo = posicoesDoPeriodo.OrderBy(x => x.DataVeiculo).Select(x => x.DataVeiculo).FirstOrDefault();

            Dominio.ObjetosDeValor.Embarcador.Logistica.PosicaoAlertaSensor PosicaoSensorAtivado = repPosicoesAlertaSensor.BuscarListaPosicoesPorVeiculo(monitoramentoProcessarEvento.CodigoVeiculo.Value, dataInicioPeriodo, dataFimPeriodo, true);

            if (PosicaoSensorAtivado == null)
                return null;

            return PosicaoSensorAtivado;
        }



        #endregion

    }
}