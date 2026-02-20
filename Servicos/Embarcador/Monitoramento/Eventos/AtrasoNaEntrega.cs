using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Monitoramento.Eventos
{
    public class AtrasoNaEntrega : AbstractEvento
    {

        #region Métodos públicos

        public AtrasoNaEntrega(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta.AtrasoNaEntrega)
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
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(base.unitOfWork);

                int total = entregas.Count;
                for (int i = 0; i < total; i++)
                {
                    DateTime? dataBase = BuscarDataMonitoramentoEvento(this.MonitoramentoEvento.Gatilho.DataBase, monitoramentoProcessarEvento, monitoramento, entregas[i], cargaJanelaCarregamento, cargaJanelasDescarregamento);
                    if (dataBase == null) dataBase = entregas[i].DataReprogramada;

                    if (entregas[i].Coleta == false && dataBase != null)
                    {
                        int tempoAtraso = this.MonitoramentoEvento.Gatilho.TempoEvento;

                        if (tempoAtraso <= 0)
                            tempoAtraso = this.MonitoramentoEvento.Gatilho.TempoEvento2;

                        DateTime? dataReferencia = BuscarDataMonitoramentoEvento(this.MonitoramentoEvento.Gatilho.DataReferencia, monitoramentoProcessarEvento, monitoramento, entregas[i], cargaJanelaCarregamento, cargaJanelasDescarregamento);
                        if (dataReferencia == null)
                        {
                            // Se houver janela de descarga, deve considerar o horário da janela
                            DateTime? dataDescarregamentoProgramada = base.BuscaDataJanelaDescarregamentoNoCliente(entregas[i].Cliente, cargaJanelasDescarregamento);
                            if (dataDescarregamentoProgramada != null)
                            {
                                dataReferencia = dataDescarregamentoProgramada.Value;
                                tempoAtraso = this.MonitoramentoEvento.Gatilho.TempoEvento2;
                            }
                            else
                            {
                                DateTime? dataPrevisaoEntregaPedido = repCargaPedido.BuscarMaiorPrevisaoEntrega(monitoramento.Carga.Codigo, entregas[i].Cliente?.Codigo ?? 0);
                                if (dataPrevisaoEntregaPedido != null)
                                {
                                    dataReferencia = dataPrevisaoEntregaPedido.Value;
                                }
                            }
                        }

                        //caso o gatilho tem configuracao de considerar apenas data sem hora
                        if (this.MonitoramentoEvento.Gatilho.ConsiderarApenasDataNaReferencia && dataReferencia != null && dataBase != null)
                        {
                            dataReferencia = dataReferencia.Value.Date;
                            dataBase = dataBase.Value.Date;
                        }

                        if (dataReferencia != null && dataBase != null && dataBase.Value > dataReferencia.Value)
                        {

                            TimeSpan atraso = dataBase.Value - dataReferencia.Value;
                            if (atraso.TotalMinutes > tempoAtraso)
                            {
                                bool tratativaAutomatica = false;
                                if (dataReferencia.Value < DateTime.Now && this.MonitoramentoEvento.Gatilho.TratativaAutomatica)
                                    tratativaAutomatica = true;

                                string texto = $"Entrega:{entregas[i].Cliente?.CPF_CNPJ_Formatado ?? ""} Ordem:{entregas[i].Ordem + 1} atraso:" + Servicos.Embarcador.Monitoramento.AlertaMonitor.FormatarTempo(atraso);
                                base.CriarAlertaSeNaoExistir(alertas, monitoramentoProcessarEvento, monitoramento.Carga, entregas, texto, tratativaAutomatica, entregas[i]);
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