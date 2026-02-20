using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Monitoramento.Eventos
{
    public class ParadaExcessiva : AbstractParada
    {

        #region Métodos públicos

        public ParadaExcessiva(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta.ParadaExcessiva)
        {

        }

        /**
         * Confirma que o evento está ativo e com as configurações mínimas
         */
        public new bool EstaAtivo()
        {
            return base.EstaAtivo() && this.ListaMonitoramentoEvento.Any(x => x.Gatilho?.Tempo != null) && this.ListaMonitoramentoEvento.Any(x => x.Gatilho?.TempoEvento != null) && this.ListaMonitoramentoEvento.Any(x => x.Gatilho?.Raio != null);
        }

        /**
         * Processa a posição atual recebida para gerar um possível alerta
         */
        public override void ProcessarEvento(Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoProcessarEvento monitoramentoProcessarEvento, Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento, List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoesObjetoValor, List<Dominio.ObjetosDeValor.Embarcador.Logistica.AlertaMonitor> alertas, List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> entregas, List<double> codigosClientesAlvo, Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento> cargaJanelasDescarregamento)
        {
            var estaEmAlvo = monitoramentoProcessarEvento.EmAlvoPosicao ?? false;
            if (entregas?.Count > 0 && codigosClientesAlvo?.Count > 0)
                estaEmAlvo = EmAlvoEntrega(codigosClientesAlvo, entregas);

            // Não está em um raio de cliente e está parado 
            if (monitoramentoProcessarEvento != null && monitoramentoProcessarEvento.DataVeiculoPosicao != null && monitoramentoProcessarEvento.LatitudePosicao != null && monitoramentoProcessarEvento.LongitudePosicao != null && !estaEmAlvo && monitoramentoProcessarEvento.VelocidadePosicao == 0)
            {

                // Garante que não está em uma área de risco ou área de pernoite
                Dominio.Entidades.Embarcador.Logistica.Locais localPernoite = Servicos.Embarcador.Monitoramento.Localizacao.ValidarArea.BuscarLocalEmArea(LocaisPernoite, monitoramentoProcessarEvento.LatitudePosicao.Value, monitoramentoProcessarEvento.LongitudePosicao.Value);
                Dominio.Entidades.Embarcador.Logistica.Locais localAreaDeRisco = Servicos.Embarcador.Monitoramento.Localizacao.ValidarArea.BuscarLocalEmArea(LocaisAreaDeRisco, monitoramentoProcessarEvento.LatitudePosicao.Value, monitoramentoProcessarEvento.LongitudePosicao.Value);
                Dominio.Entidades.Embarcador.Logistica.Locais localPontoDeApoio = Servicos.Embarcador.Monitoramento.Localizacao.ValidarArea.BuscarLocalEmArea(LocaisPontoDeApoio, monitoramentoProcessarEvento.LatitudePosicao.Value, monitoramentoProcessarEvento.LongitudePosicao.Value);

                if (localPernoite == null && localAreaDeRisco == null && localPontoDeApoio == null)
                {

                    // Confirma que ficou dentro do raio durante o tempo
                    DateTime? dataInicioParada = PermaneceuNoRaio(monitoramentoProcessarEvento, posicoesObjetoValor, alertas, this.MonitoramentoEvento.Gatilho.TempoEvento, this.MonitoramentoEvento.Gatilho.Raio);
                    if (dataInicioParada != null)
                    {
                        // Cria o alerta para a carga se não existir algum
                        string texto = Servicos.Embarcador.Monitoramento.AlertaMonitor.FormatarTextoParada(dataInicioParada.Value, monitoramentoProcessarEvento.DataVeiculoPosicao.Value);
                        base.CriarAlertaSeNaoExistir(alertas, monitoramentoProcessarEvento, monitoramento?.Carga ?? null, entregas, texto);
                    }

                }

            }

        }

        public override void ExecutarDepoisDeCriarAlerta(Dominio.Entidades.Embarcador.Logistica.AlertaMonitor alerta, Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoProcessarEvento monitoramentoProcessarEvento) { }

        #endregion

    }
}