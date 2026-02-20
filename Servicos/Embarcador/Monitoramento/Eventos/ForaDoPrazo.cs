using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Monitoramento.Eventos
{
    public class ForaDoPrazo : AbstractEvento
    {

        #region Métodos públicos

        public ForaDoPrazo(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta.ForaDoPrazo)
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
            if (monitoramento != null && monitoramento.Carga != null && monitoramento.Carga.DataFimViagem != null)
            {
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(base.unitOfWork);
                
                int total = entregas.Count;
                for (int i = 0; i < total; i++)
                {
                    DateTime? dataBase = BuscarDataMonitoramentoEvento(this.MonitoramentoEvento.Gatilho.DataBase, monitoramentoProcessarEvento, monitoramento, entregas[i], cargaJanelaCarregamento, cargaJanelasDescarregamento);
                    if (dataBase == null) dataBase = DateTime.Now;

                    DateTime? dataReferencia = BuscarDataMonitoramentoEvento(this.MonitoramentoEvento.Gatilho.DataReferencia, monitoramentoProcessarEvento, monitoramento, entregas[i], cargaJanelaCarregamento, cargaJanelasDescarregamento);
                    if (dataReferencia == null) dataReferencia = monitoramentoProcessarEvento.DataVeiculoPosicao;
                    
                    if (dataBase.Value > dataReferencia.Value)
                    {
                        TimeSpan atraso = dataBase.Value - dataReferencia.Value;
                        if (atraso.TotalMinutes > this.MonitoramentoEvento.Gatilho.TempoEvento)
                        {
                            string texto = $"Entrega {entregas[i].Cliente?.CPF_CNPJ_Formatado ?? ""} atrasada por " + Servicos.Embarcador.Monitoramento.AlertaMonitor.FormatarTempo(atraso);
                            base.CriarAlertaSeNaoExistir(alertas, monitoramentoProcessarEvento, monitoramento.Carga, entregas, texto);
                        }
                    }
                }
            }
        }

        public override void ExecutarDepoisDeCriarAlerta(Dominio.Entidades.Embarcador.Logistica.AlertaMonitor alerta, Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoProcessarEvento monitoramentoProcessarEvento) { }

        #endregion

    }
}