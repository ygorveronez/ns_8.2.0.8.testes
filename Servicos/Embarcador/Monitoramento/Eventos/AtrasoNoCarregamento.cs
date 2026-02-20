using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Monitoramento.Eventos
{
    public class AtrasoNoCarregamento : AbstractEvento
    {

        #region Métodos públicos

        public AtrasoNoCarregamento(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta.AtrasoNoCarregamento)
        {
            
        }

        /**
         * Confirma que o evento está ativo e com as configurações mínimas
         */
        public new bool EstaAtivo()
        {
            return base.EstaAtivo() && this.ListaMonitoramentoEvento.Any(x => x.Gatilho?.Tempo != null) && this.ListaMonitoramentoEvento.Any(x => x.Gatilho?.TempoEvento != null);
        }

        /**
         * Processa a posição atual recebida para gerar um possível alerta
         */
        public override void ProcessarEvento(Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoProcessarEvento monitoramentoProcessarEvento, Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento, List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoesObjetoValor, List<Dominio.ObjetosDeValor.Embarcador.Logistica.AlertaMonitor> alertas, List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> entregas, List<double> codigosClientesAlvo, Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento> cargaJanelasDescarregamento)
        {

            // Há carga com data de carregamento e ainda não iniciou a viagem
            if (monitoramento != null && monitoramento.Carga != null && monitoramento.Carga.DataInicioViagem == null && monitoramento.Carga.DataPrevisaoChegadaOrigem != null && cargaJanelaCarregamento != null)
            {

                // previs!ao de chegada na planta 
                // Está atrasado?
                DateTime dataReferencia = cargaJanelaCarregamento.InicioCarregamento.AddMinutes(this.MonitoramentoEvento.Gatilho.TempoEvento);
                if (monitoramento.Carga.DataPrevisaoChegadaOrigem.Value > dataReferencia)
                {

                    // Cliente origem da carga
                    Dominio.Entidades.Cliente clienteOrigem = Servicos.Embarcador.Monitoramento.Monitoramento.BuscarClienteOrigemDaCargaPeloPedido(unitOfWork, monitoramento.Carga);

                    // Confirma que o veículo não está no cliente de origem
                    if (!EstaNoCliente(monitoramentoProcessarEvento, codigosClientesAlvo, clienteOrigem))
                    {
                        TimeSpan atraso = monitoramento.Carga.DataPrevisaoChegadaOrigem.Value - dataReferencia;
                        string texto = Servicos.Embarcador.Monitoramento.AlertaMonitor.FormatarTempo(atraso);
                        base.CriarAlertaSeNaoExistir(alertas, monitoramentoProcessarEvento, monitoramento.Carga, entregas, texto);
                    }

                }
            }
        }

        public override void ExecutarDepoisDeCriarAlerta(Dominio.Entidades.Embarcador.Logistica.AlertaMonitor alerta, Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoProcessarEvento monitoramentoProcessarEvento) { }

        #endregion

    }
}