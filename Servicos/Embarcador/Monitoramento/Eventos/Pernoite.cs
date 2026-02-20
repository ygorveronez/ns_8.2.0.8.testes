using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Monitoramento.Eventos
{
    public class Pernoite : AbstractEvento
    {
        #region Atributos públicos

        public Dominio.Entidades.Embarcador.Logistica.Locais[] LocaisPernoite { get; set; }

        #endregion

        #region Métodos públicos

        public Pernoite(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta.Pernoite)
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
            if (monitoramentoProcessarEvento != null && monitoramentoProcessarEvento.DataVeiculoPosicao != null && monitoramentoProcessarEvento.LatitudePosicao != null && monitoramentoProcessarEvento.LongitudePosicao != null)
            {
                // Período do pernoite das 18h do dia até as 8h do dia seguinte
                DateTime dataPernoiteInicial = monitoramentoProcessarEvento.DataVeiculoPosicao.Value.Date.AddHours(18);
                DateTime dataPernoiteFinal = monitoramentoProcessarEvento.DataVeiculoPosicao.Value.Date.AddHours(8);

                // A data da posição está no período de pernoite
                if (monitoramentoProcessarEvento.DataVeiculoPosicao.Value >= dataPernoiteInicial || monitoramentoProcessarEvento.DataVeiculoPosicao.Value <= dataPernoiteFinal)
                {

                    // Verifica se a posição atual do veículo está no raio de algum local cadastrado como pernoite
                    Dominio.Entidades.Embarcador.Logistica.Locais localPernoite = Localizacao.ValidarArea.BuscarLocalEmArea(LocaisPernoite, monitoramentoProcessarEvento.LatitudePosicao.Value, monitoramentoProcessarEvento.LongitudePosicao.Value);

                    // Está em algum local de pernoite?
                    if (localPernoite != null)
                    {
                        // Cria o alerta para a carga se não existir algum
                        base.CriarAlertaSeNaoExistir(alertas, monitoramentoProcessarEvento, monitoramento?.Carga ?? null, entregas, localPernoite.Descricao);
                    }

                }
            }
        }

        public override void ExecutarDepoisDeCriarAlerta(Dominio.Entidades.Embarcador.Logistica.AlertaMonitor alerta, Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoProcessarEvento monitoramentoProcessarEvento) { }

        #endregion

    }
}