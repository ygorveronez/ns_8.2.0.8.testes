using System;
using System.Collections.Generic;
using AdminMultisoftware.Dominio.Enumeradores;
using System.Linq;

namespace Servicos.Embarcador.Monitoramento.Eventos
{
    public class ParadaNaoProgramada : AbstractParada
    {
        private readonly AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente _cliente;
        #region Métodos públicos

        public ParadaNaoProgramada(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente) : base(unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta.ParadaNaoProgramada)
        {
            _cliente = cliente;
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

            // Não está em um raio de cliente e está parado 
            if (monitoramentoProcessarEvento != null && monitoramentoProcessarEvento.DataVeiculoPosicao != null && monitoramentoProcessarEvento.LatitudePosicao != null && monitoramentoProcessarEvento.LongitudePosicao != null && !EmAlvoEntrega(codigosClientesAlvo, entregas) && monitoramentoProcessarEvento.VelocidadePosicao == 0)
            {

                // Garante que não está em uma área de risco ou área de pernoite
                Dominio.Entidades.Embarcador.Logistica.Locais localPernoite = Localizacao.ValidarArea.BuscarLocalEmArea(LocaisPernoite, monitoramentoProcessarEvento.LatitudePosicao.Value, monitoramentoProcessarEvento.LongitudePosicao.Value);
                Dominio.Entidades.Embarcador.Logistica.Locais localAreaDeRisco = Localizacao.ValidarArea.BuscarLocalEmArea(LocaisAreaDeRisco, monitoramentoProcessarEvento.LatitudePosicao.Value, monitoramentoProcessarEvento.LongitudePosicao.Value);
                if (localPernoite == null && localAreaDeRisco == null)
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

        public override void ExecutarDepoisDeCriarAlerta(Dominio.Entidades.Embarcador.Logistica.AlertaMonitor alerta, Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoProcessarEvento monitoramentoProcessarEvento)
        {
            if (alerta.Carga == null)
                return;

            if (alerta.Carga.Motoristas == null)
                return;

            foreach (var motorista in alerta.Carga.Motoristas)
            {
                EnviarNotificacaoApp(motorista, alerta);
            }
        }

        private void EnviarNotificacaoApp(Dominio.Entidades.Usuario motorista, Dominio.Entidades.Embarcador.Logistica.AlertaMonitor alerta)
        {
            // Na hora de implementar a função de verdade, criar um novo tipo de notificação apenas para a parada não programada

            dynamic conteudo = new
            {
                Codigo = alerta.Codigo,
                horario = alerta.Data.ToString("dd/MM/yyyy HH:mm"),
                latitude = alerta.Latitude,
                longitude = alerta.Longitude,
            };

            Dominio.MSMQ.Notification notification = new Dominio.MSMQ.Notification(
                conteudo,
                _cliente.Codigo,
                motorista.CodigoMobile,
                Dominio.MSMQ.MSMQQueue.SGTMobile,
                Dominio.SignalR.Hubs.Mobile,
                Servicos.SignalR.Mobile.GetHub(MobileHubs.ParadaNaoProgramada)
            );

            Servicos.MSMQ.MSMQ.SendPrivateMessage(notification);
        }

        #endregion

    }
}