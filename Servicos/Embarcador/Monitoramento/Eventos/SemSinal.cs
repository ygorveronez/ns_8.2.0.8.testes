using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Monitoramento.Eventos
{
    public class SemSinal : AbstractEvento
    {

        #region Métodos públicos

        public SemSinal(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta.SemSinal)
        {

        }

        /**
         * Confirma que o evento está ativo e com as configurações mínimas
         */
        public new bool EstaAtivo()
        {
            return base.EstaAtivo() && this.ListaMonitoramentoEvento.Exists(x => x.Gatilho?.Tempo != null && x.Gatilho?.TempoEvento != null && !(x.Gatilho?.TempoReferenteaDataCarregamentoCarga ?? false));
        }

        /**
         * Processa a posição atual recebida para gerar um possível alerta
         */
        public override void ProcessarEvento(Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoProcessarEvento monitoramentoProcessarEvento, Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento, List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoesObjetoValor, List<Dominio.ObjetosDeValor.Embarcador.Logistica.AlertaMonitor> alertas, List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> entregas, List<double> codigosClientesAlvo, Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento> cargaJanelasDescarregamento)
        {
            if (monitoramentoProcessarEvento != null && monitoramentoProcessarEvento.DataVeiculoPosicao != null)
            {
                TimeSpan atraso = DateTime.Now - monitoramentoProcessarEvento.DataVeiculoPosicao.Value;
                double atrasoMinutos = atraso.TotalMinutes;

                if (monitoramento != null && !monitoramentoProcessarEvento.CodigoMonitoramento.HasValue)
                    monitoramentoProcessarEvento.CodigoMonitoramento = monitoramento.Codigo;

                if (atrasoMinutos > this.MonitoramentoEvento.Gatilho.TempoEvento)
                {
                    if (base.ExisteAlertaAbertoOuFechadoHaPouco(monitoramentoProcessarEvento, monitoramento, alertas)) return;

                    // Cria o alerta para a carga se não existir algum
                    string texto = (atrasoMinutos < 60) ? $"Há {(int)atrasoMinutos} minutos" : Servicos.Embarcador.Monitoramento.AlertaMonitor.FormatarTempo(atraso);
                    texto += ", desde " + monitoramentoProcessarEvento.DataVeiculoPosicao.Value.ToString("dd/MM/yyyy HH:mm");
                    base.CriarAlertaSeNaoExistir(alertas, monitoramentoProcessarEvento, monitoramento?.Carga ?? null, entregas, texto);
                }
                else
                {
                    // sinal voltou ou esta normal, verificar entidade perda de sinal monitoramento
                    if (monitoramentoProcessarEvento != null && !monitoramentoProcessarEvento.CodigoMonitoramento.HasValue)
                        return;

                    var parametrosPerdaSinal = new Dominio.ObjetosDeValor.Embarcador.Logistica.ParametrosPerdaSinalMonitoramento
                    {
                        codigoMonitoramento = monitoramentoProcessarEvento.CodigoMonitoramento.Value,
                        CodigoVeiculo = monitoramentoProcessarEvento.CodigoVeiculo.HasValue ? monitoramentoProcessarEvento.CodigoVeiculo.Value : 0,
                        DataFim = monitoramentoProcessarEvento.DataVeiculoPosicao.Value,
                        Latitude = monitoramentoProcessarEvento.LatitudePosicao.Value,
                        Longitude = monitoramentoProcessarEvento.LongitudePosicao.Value,
                    };

                    Servicos.Embarcador.Monitoramento.PerdaSinalMonitoramento servPerdaSinal = new Servicos.Embarcador.Monitoramento.PerdaSinalMonitoramento(unitOfWork);
                    servPerdaSinal.FinalizarRegistroPerdaSinal(parametrosPerdaSinal);
                }
            }
        }

        public override void ExecutarDepoisDeCriarAlerta(Dominio.Entidades.Embarcador.Logistica.AlertaMonitor alerta, Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoProcessarEvento monitoramentoProcessarEvento)
        {
            //criar entidade perda de sinal
            if (monitoramentoProcessarEvento != null && !monitoramentoProcessarEvento.CodigoMonitoramento.HasValue)
                return;

            var parametrosPerdaSinal = new Dominio.ObjetosDeValor.Embarcador.Logistica.ParametrosPerdaSinalMonitoramento
            {
                codigoMonitoramento = monitoramentoProcessarEvento.CodigoMonitoramento.Value,
                CodigoVeiculo = monitoramentoProcessarEvento.CodigoVeiculo.HasValue ? monitoramentoProcessarEvento.CodigoVeiculo.Value : 0,
                DataInicio = monitoramentoProcessarEvento.DataVeiculoPosicao.Value,
                Latitude = monitoramentoProcessarEvento.LatitudePosicao.Value,
                Longitude = monitoramentoProcessarEvento.LongitudePosicao.Value,
                AlertaMonitor = alerta,
            };

            Servicos.Embarcador.Monitoramento.PerdaSinalMonitoramento servPerdaSinal = new Servicos.Embarcador.Monitoramento.PerdaSinalMonitoramento(unitOfWork);
            servPerdaSinal.CriarNovoRegistroPerdaSinal(parametrosPerdaSinal);
        }

        #endregion

    }
}