using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Monitoramento.Eventos
{
    public class AtrasoNaLiberacao : AbstractEvento
    {

        #region Métodos públicos

        public AtrasoNaLiberacao(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta.AtrasoNaLiberacao)
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
            if (monitoramento != null && monitoramento.Carga != null && monitoramento.Carga.DataCarregamentoCarga != null && monitoramento.Carga.DataInicioViagem == null && monitoramentoProcessarEvento != null && EmAlvoEntrega(codigosClientesAlvo, entregas))
            {

                // Cliente origem da carga
                Dominio.Entidades.Cliente clienteOrigem = Servicos.Embarcador.Monitoramento.Monitoramento.BuscarClienteOrigemDaCargaPeloPedido(unitOfWork, monitoramento.Carga);

                // Confirma que o está na origem da carga
                if (base.EstaNoCliente(monitoramentoProcessarEvento, codigosClientesAlvo, clienteOrigem))
                {

                    DateTime? dataEntradaCliente = base.BuscarDataEntradaNoAlvo(clienteOrigem, entregas);
                    if (dataEntradaCliente != null)
                    {

                        DateTime dataReferencia;
                        if (cargaJanelaCarregamento != null && cargaJanelaCarregamento.InicioCarregamento != null && dataEntradaCliente < cargaJanelaCarregamento.InicioCarregamento)
                        {
                            dataReferencia = cargaJanelaCarregamento.InicioCarregamento;
                        }
                        else
                        {
                            dataReferencia = dataEntradaCliente.Value;
                        }

                        DateTime dataCarregamentoLimite = dataReferencia.AddMinutes(this.MonitoramentoEvento.Gatilho.TempoEvento);
                        if (monitoramentoProcessarEvento.DataVeiculoPosicao.Value > dataCarregamentoLimite)
                        {
                            // Cria o alerta para a carga se não existir algum
                            TimeSpan tempoCarregamento = monitoramentoProcessarEvento.DataVeiculoPosicao.Value - dataCarregamentoLimite;
                            string texto = Servicos.Embarcador.Monitoramento.AlertaMonitor.FormatarTempo(tempoCarregamento);
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