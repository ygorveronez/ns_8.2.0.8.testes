using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Monitoramento.Eventos
{
    public class PermanenciaNoRaio : AbstractEvento
    {

        #region Métodos públicos

        public PermanenciaNoRaio(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta.PermanenciaNoRaio)
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
            if (monitoramentoProcessarEvento != null && monitoramento != null && monitoramento.Carga != null && EmAlvoEntrega(codigosClientesAlvo, entregas))
            {
                List<double> codigosClientesDaCarga = ExtrairCodigosClientesColetaEntregaCarga(monitoramento.Carga, entregas);
                List<double> codigosClientesAlvoCarga = Util.Intersecao(codigosClientesDaCarga, codigosClientesAlvo);
                int total = codigosClientesAlvoCarga.Count;
                for (int i = 0; i < total; i++)
                {
                    Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega entrega = (from obj in entregas where obj.Cliente?.Codigo == codigosClientesAlvoCarga[i] select obj).FirstOrDefault();
                    if (entrega != null)
                    {
                        Repositorio.Embarcador.Logistica.PermanenciaCliente repPermanenciaCliente = new Repositorio.Embarcador.Logistica.PermanenciaCliente(this.unitOfWork);
                        Dominio.Entidades.Embarcador.Logistica.PermanenciaCliente permanenciaAbertaNoCliente = repPermanenciaCliente.BuscarAbertaPorClienteECargaEntrega(codigosClientesAlvoCarga[i], entrega.Codigo);
                        if (permanenciaAbertaNoCliente != null)
                        {
                            TimeSpan tempo = monitoramentoProcessarEvento.DataVeiculoPosicao.Value - permanenciaAbertaNoCliente.DataInicio;
                            if (tempo.TotalMinutes >= this.MonitoramentoEvento.Gatilho.TempoEvento)
                            {
                                string texto = Servicos.Embarcador.Monitoramento.AlertaMonitor.FormatarTempo(tempo);
                                base.CriarAlertaSeNaoExistir(alertas, monitoramentoProcessarEvento, monitoramento?.Carga ?? null, entregas, texto);
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